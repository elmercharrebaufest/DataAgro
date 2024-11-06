using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;
using System.Web.Http.Results;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfirmasConFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerConfirmasConFiltro(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            // Configuración del timeout de la consulta
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var queryBasicoConfirmas = from negocio in contexto.Set<Negocio>()
                                               // Filtrar para negocios de tipo FIJACION y relacionarlos con el negocio padre A_FIJAR
                                           join np in (
                                               from n in contexto.Set<Negocio>()
                                               join parent in contexto.Set<Negocio>()
                                                   on n.ContratoSAP equals parent.ContratoSAP
                                               where parent.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR
                                                     && parent.ConfirmadoSAP == true
                                                     && parent.EstadoId == (int)EnumEstadoContrato.Finalizado
                                                     && (
                                                         (n.ComercialId != null && equipo.Contains(n.ComercialId.Value)) ||
                                                         (n.ComercialCreadorId != null && equipo.Contains(n.ComercialCreadorId.Value))
                                                     )
                                               select new
                                               {
                                                   n.ContratoSAP,
                                                   parent.BoletoId,
                                                   parent.BolsaId,
                                                   parent.Canje
                                               }
                                           ) on negocio.ContratoSAP equals np.ContratoSAP into npGroup
                                           from np in npGroup.DefaultIfEmpty()

                                               // Unir con la confirmación (última versión sin anulaciones)
                                           join confirma in (
                                               from c in contexto.Set<Confirma>()
                                               where c.FechaAnulacion == null  // Solo incluir confirmaciones no anuladas
                                               group c by c.NegocioId into g
                                               select g.OrderByDescending(c => c.Version).FirstOrDefault()
                                           ) on negocio.Id equals confirma.NegocioId into c
                                           from confirma in c.DefaultIfEmpty()

                                               // Filtros generales antes de aplicar los filtros de Kendo
                                           where negocio.ConfirmadoSAP == true
                                                 && negocio.EstadoId == (int)EnumEstadoContrato.Finalizado
                                                 && (
                                                     // Si el negocio es de tipo FIJACION, usar el BoletoId del negocio padre (A_FIJAR)
                                                     (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && np != null && np.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ||
                                                     // Para otros negocios que no sean FIJACION, usar el BoletoId propio si es CONFIRMA
                                                     (negocio.TipoNegocioId != (int)EnumTipoNegocio.FIJACION && negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                                                 )
                                                 // Filtro adicional para ComercialId y ComercialCreadorId
                                                 && (
                                                     (negocio.ComercialId != null && equipo.Contains(negocio.ComercialId.Value)) ||
                                                     (negocio.ComercialCreadorId != null && equipo.Contains(negocio.ComercialCreadorId.Value))
                                                 )

                                           orderby negocio.Id descending
                                           select new BasicoConfirma
                                           {
                                               // ID del negocio
                                               Id = negocio.Id,

                                               // SAP del negocio
                                               NegocioSAP = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).FijacionSAP : negocio.ContratoSAP,
                                               ContratoSAP = negocio.ContratoSAP,
                                               FijacionSAP = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).FijacionSAP : string.Empty,

                                               // Tipo de negocio y clasificación
                                               ClaseNegocioId = negocio is Contrato ? "1" : "2",
                                               TipoNegocioId = negocio.TipoNegocioId,
                                               TipoNegocio = (negocio.TipoNegocio == null ? "" :
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
                                                  negocio.TipoNegocio.Descripcion),

                                               // Version
                                               Version = confirma != null && confirma.Version > 1 ? confirma.Version : 1,  // Asegurarse de que la versión sea válida

                                               // Estado de la versión (Anulado, Vigente, Pendiente)
                                               Estado_Version = confirma != null && confirma.FechaAnulacion != null ? "Anulado" : (confirma != null && confirma.FechaGeneracion != null ? "Vigente" : "Pendiente"),

                                               // ID y Descripción del Boleto
                                               BoletoId = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.BoletoId : negocio.BoletoId,
                                               TipoBoleto = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Boleto.Descripcion : negocio.Boleto.Descripcion,

                                               // Canje (dependiendo del tipo de negocio)
                                               Canje = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (np.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO"),

                                               // Bolsa
                                               BolsaId = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.BolsaId : negocio.BolsaId,
                                               Bolsa = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Bolsa.Descripcion : negocio.Bolsa.Descripcion,

                                               // Precio y moneda
                                               Precio = negocio.Precio,
                                               Moneda = negocio.MonedaId.Trim() == "ARP" ? "ARP" : (negocio.MonedaId.Trim() == "USDM" ? "USD" : string.Empty),

                                               // Fechas importantes
                                               FechaGeneracion = confirma.FechaGeneracion,
                                               FechaOperacion = negocio.FechaOperacion,
                                               FechaCarga = negocio.Fecha,
                                               FechaConfirmacion = negocio.FechaConfirmacion,
                                               FechaAnulacion = confirma.FechaAnulacion,

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
                                               Comercial = negocio.Comercial.Apellido + ", " + negocio.Comercial.Nombres
                                           };

                // Ejecutar la consulta
                return queryBasicoConfirmas.ToDataSourceResult(request);
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