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
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerBoletosConFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerBoletosConFiltro(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var queryBasicoBoletos = from negocio in contexto.Set<Negocio>()
                                             // Unimos con las boletos para obtener la versión más reciente
                                         join boleto in (
                                             from c in contexto.Set<Boleto>()
                                             where c.FechaAnulacion == null  // Solo incluimos boletos no anuladas
                                             group c by c.NegocioId into g
                                             select g.OrderByDescending(c => c.Version).FirstOrDefault()
                                         ) on negocio.Id equals boleto.NegocioId into boletoGroup
                                         from boleto in boletoGroup.DefaultIfEmpty()

                                             // Filtros generales para obtener los negocios confirmados y con el estado adecuado
                                         where negocio.ConfirmadoSAP == true
                                               && negocio.EstadoId == (int)EnumEstadoContrato.Finalizado
                                               &&
                                              (
                                                  (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && (negocio.BoletoId == (int)EnumBoletoCompraNet.FISICO || negocio.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA))
                                                  ||
                                                  (negocio.TipoNegocioId != (int)EnumTipoNegocio.FIJACION && (negocio.BoletoId == (int)EnumBoletoCompraNet.FISICO || negocio.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA))
                                              )

                                         // Ordenamos los resultados por ID de negocio de forma descendente
                                         orderby negocio.Id descending

                                         // Selección final para la entidad BasicoConfirma
                                         select new BasicoBoleto
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
                                             Version = boleto != null && boleto.Version > 1 ? boleto.Version : 1,

                                             // Estado de la versión (Anulado, Vigente, Pendiente)
                                             Estado_Version = boleto != null && boleto.FechaAnulacion != null
                                                 ? "Anulado"
                                                 : (boleto != null && boleto.FechaGeneracion != null ? "Vigente" : "Pendiente"),

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
                                             FechaGeneracion = boleto.FechaGeneracion,
                                             FechaOperacion = negocio.FechaOperacion,
                                             FechaCarga = negocio.Fecha,
                                             FechaConfirmacion = negocio.FechaConfirmacion,
                                             FechaAnulacion = boleto.FechaAnulacion,

                                             // Proveedor (Corredor y Vendedor)
                                             Corredor = negocio.Corredor.RazonSocial ?? "",
                                             Vendedor = negocio.Proveedor.RazonSocial ?? "",

                                             // Material
                                             MaterialId = negocio.MaterialId,
                                             Material = negocio.Material.Descripcion,

                                             // Contratos
                                             ContratoVendedor = negocio.ContratoVendedor,
                                             ContratoCorredor = negocio.ContratoCorredor,

                                             // Comercial
                                             Comercial = negocio.Comercial.Apellido + ", " + negocio.Comercial.Nombres,
                                             FechaConfirmadoSAP = negocio.FechaConfirmadoSAP,
                                         };
                GridHelper.TruncateTime(request.Filter, ref queryBasicoBoletos);

                return queryBasicoBoletos.ToDataSourceResult(request);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en la consulta: " + ex.Message);
                throw;
            }
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo);
            }
        }
    }
}
