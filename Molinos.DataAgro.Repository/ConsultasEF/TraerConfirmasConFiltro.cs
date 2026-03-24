using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfirmasConFiltro : IConsultaEscalar<List<BasicoConfirma>>
    {
        private readonly ConfirmaFiltroBusquedaDto filtros;
        private readonly List<int> equipo;

        public TraerConfirmasConFiltro(ConfirmaFiltroBusquedaDto filtros, List<int> equipo)
        {
            this.filtros = filtros;
            this.equipo = equipo;
        }

        private static List<BasicoConfirma> Query(DbContext contexto, ConfirmaFiltroBusquedaDto filtros, List<int> equipo)
        {
            // Configuración del timeout de la consulta
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                // Optimización: Pre-filtrar negocio antes del join para reducir dataset
                var negociosFiltrados = contexto.Set<Negocio>()
                    .Where(n => n.ConfirmadoSAP == true
                        && n.EstadoId == (int)EnumEstadoContrato.Finalizado
                        && (
                            // Negocios A_Precio y A_Fijar: Deben tener el BoletoId igual a "CONFIRMA"
                            (n.TipoNegocioId != (int)EnumTipoNegocio.FIJACION && n.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                            ||
                            // Negocios Fijacion: El BoletoId debe estar en el negocio padre
                            (n.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                                && (n as FijacionDePrecioContrato).Contrato != null
                                && (n as FijacionDePrecioContrato).Contrato.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                        )
                        && (n.ComercialId != null && equipo.Contains(n.ComercialId.Value)
                            || n.ComercialCreadorId != null && equipo.Contains(n.ComercialCreadorId.Value)));

                // Aplicar filtros adicionales temprano para reducir dataset
                if (!string.IsNullOrWhiteSpace(filtros.NegocioSAP))
                {
                    var negociosSAPList = filtros.NegocioSAP.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(x => x.Trim())
                        .ToList();

                    if (negociosSAPList.Count > 0)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n =>
                            negociosSAPList.Contains(n.ContratoSAP) ||
                            (n is FijacionDePrecioContrato && negociosSAPList.Contains((n as FijacionDePrecioContrato).FijacionSAP)));
                    }
                }
                else
                {
                    if (filtros.FechaConfirmacionDesde.HasValue)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n => n.FechaConfirmacion >= filtros.FechaConfirmacionDesde.Value);
                    }

                    if (filtros.FechaConfirmacionHasta.HasValue)
                    {
                        var fechaHasta = filtros.FechaConfirmacionHasta.Value.Date.AddDays(1).AddTicks(-1);
                        negociosFiltrados = negociosFiltrados.Where(n => n.FechaConfirmacion <= fechaHasta);
                    }

                    if (filtros.ProveedorId.HasValue && filtros.ProveedorId.Value > 0)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n => n.ProveedorId == filtros.ProveedorId.Value);
                    }

                    if (filtros.ComercialId.HasValue && filtros.ComercialId.Value > 0)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n => n.ComercialId == filtros.ComercialId.Value);
                    }

                    if (filtros.BolsaCompraNetId.HasValue && filtros.BolsaCompraNetId.Value > 0)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n => n.BolsaId == filtros.BolsaCompraNetId.Value
                            || (n is FijacionDePrecioContrato && (n as FijacionDePrecioContrato).Contrato.BolsaId == filtros.BolsaCompraNetId.Value));
                    }

                    if (filtros.MaterialId.HasValue && filtros.MaterialId.Value > 0)
                    {
                        negociosFiltrados = negociosFiltrados.Where(n => n.MaterialId == filtros.MaterialId.Value);
                    }
                }

                var queryBasicoConfirmas = from negocio in negociosFiltrados
                                               // Unimos con las confirmas para obtener la versión más reciente
                                           join confirma in (
                                               from c in contexto.Set<Confirma>()
                                               where c.FechaAnulacion == null
                                               group c by c.NegocioId into g
                                               select new { NegocioId = g.Key, Confirma = g.OrderByDescending(c => c.Version).FirstOrDefault() }
                                           ) on negocio.Id equals confirma.NegocioId into confirmaGroup
                                           from confirmaData in confirmaGroup.DefaultIfEmpty()
                                           let confirma = confirmaData != null ? confirmaData.Confirma : null

                                           // Ordenamos los resultados por ID de negocio de forma descendente
                                           orderby negocio.Id descending

                                           // Selección final para la entidad BasicoConfirma
                                           select new BasicoConfirma
                                           {
                                               // ID del negocio
                                               Id = negocio.Id,

                                               // SAP del negocio
                                               NegocioSAP = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).FijacionSAP
                                                   : negocio.ContratoSAP,

                                               ContratoSAP = negocio.ContratoSAP,
                                               FijacionSAP = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).FijacionSAP
                                                   : string.Empty,

                                               // Tipo de negocio y clasificación
                                               ClaseNegocioId = negocio is Contrato ? "1" : "2",
                                               TipoNegocioId = negocio.TipoNegocioId,
                                               TipoNegocio = negocio.TipoNegocio != null ?
                                                   (negocio is Contrato && (negocio as Contrato).Madre == true) ? "CONVENIO" :
                                                   (negocio is Contrato && (negocio as Contrato).Madre == false) ? "FIJ. CONVENIO" :
                                                   (negocio is Contrato && (negocio as Contrato).EsFason == true) ? "FASON MP" :
                                                   (negocio is Contrato && (negocio as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" :
                                                   (negocio is ContratoAcuerdo && (negocio as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" :
                                                   (negocio is Contrato && (negocio as Contrato).Canje == true) ? "CANJE" :
                                                   (negocio is Contrato && (negocio as Contrato).PrestamoDevolucion == true) ? "PRESTAMO DEVOLUCION" :
                                                   (negocio is Contrato && (negocio as Contrato).Venta == true) ? "VENTA" :
                                                   (negocio is Contrato && (negocio as Contrato).TipoPosicionCBOTId == 3) ? "A FIJAR PASE" :
                                                   (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).Virtual == true) ? "FIJACION VIRTUAL" :
                                                   (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).Canje == true) ? "FIJACION CANJE" :
                                                   (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).TipoPosicionCBOTId == 3) ? "FIJACION PASE" :
                                                   negocio.TipoNegocio.Descripcion
                                                   : "",

                                               // Versión (la más alta de las confirmas)
                                               Version = confirma != null && confirma.Version > 1 ? confirma.Version : 1,

                                               // Estado de la versión (Anulado, Vigente, Pendiente)
                                               Estado_Version = confirma != null && confirma.FechaAnulacion != null
                                                   ? "Anulado"
                                                   : (confirma != null && confirma.FechaGeneracion != null ? "Vigente" : "Pendiente"),

                                               // ID y Descripción del Boleto (si es FIJACION, se extrae del negocio padre)
                                               BoletoId = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).Contrato.BoletoId
                                                   : negocio.BoletoId,
                                               TipoBoleto = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).Contrato.Boleto.Descripcion
                                                   : negocio.Boleto.Descripcion,

                                               // Canje (dependiendo del tipo de negocio)
                                               Canje = negocio is FijacionDePrecioContrato
                                                   ? ((negocio as FijacionDePrecioContrato).Canje == true ? "SI" : "NO")
                                                   : (negocio.Canje == true ? "SI" : "NO"),

                                               // Bolsa
                                               BolsaId = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).Contrato.BolsaId
                                                   : negocio.BolsaId,
                                               Bolsa = negocio is FijacionDePrecioContrato
                                                   ? (negocio as FijacionDePrecioContrato).Contrato.Bolsa.Descripcion
                                                   : negocio.Bolsa.Descripcion,

                                               // Precio y moneda
                                               Precio = negocio.Precio,
                                               Moneda = negocio.MonedaId.Trim() == "ARP" ? "ARP" :
                                                        (negocio.MonedaId.Trim() == "USDM" ? "USD" : string.Empty),

                                               // Fechas importantes
                                               FechaGeneracion = confirma.FechaGeneracion,
                                               FechaOperacion = negocio.FechaOperacion,
                                               FechaCarga = negocio.Fecha,
                                               FechaConfirmacion = negocio.FechaConfirmacion,
                                               FechaAnulacion = confirma.FechaAnulacion,

                                               // Proveedor (Corredor y Vendedor)
                                               Corredor = negocio.Corredor.RazonSocial ?? "",
                                               Vendedor = negocio.Proveedor.RazonSocial ?? "",
                                               ProveedorId = negocio.ProveedorId,

                                               // Material
                                               MaterialId = negocio.MaterialId,
                                               Material = negocio.Material.Descripcion,

                                               // Contratos
                                               ContratoVendedor = negocio.ContratoVendedor,
                                               ContratoCorredor = negocio.ContratoCorredor,

                                               // Comercial
                                               Comercial = negocio.Comercial.Apellido + ", " + negocio.Comercial.Nombres,
                                               ComercialId = negocio.ComercialId,
                                               FechaConfirmadoSAP = negocio.FechaConfirmadoSAP,
                                           };

                // Los filtros ya fueron aplicados antes del join para optimizar performance
                return queryBasicoConfirmas.ToList();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en la consulta: " + ex.Message);
                throw;
            }
        }

        public virtual List<BasicoConfirma> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, filtros, equipo);
            }
        }
    }
}
