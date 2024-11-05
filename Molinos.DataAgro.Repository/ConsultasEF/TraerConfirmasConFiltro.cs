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
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            try
            {
                var ne = contexto.Set<Negocio>()
                    .Where(negocio =>
                        negocio.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                        negocio.ConfirmadoSAP == true &&
                        (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA || (negocio.BoletoId == null && negocio.TipoNegocioId==(int)EnumTipoNegocio.FIJACION)) &&
                        ((negocio.ComercialId != null && equipo.Contains(negocio.ComercialId.Value)) ||
                         (negocio.ComercialCreadorId != null && equipo.Contains(negocio.ComercialCreadorId.Value))));

                var co = contexto.Set<Confirma>()
                    .Where(c => c.FechaAnulacion == null)
                    .GroupBy(c => c.NegocioId) // Agrupamos por NegocioId
                    .Select(g => g.OrderByDescending(c => c.Version).FirstOrDefault()); // Seleccionamos la confirmación con la versión más alta

                var nep = contexto.Set<Negocio>()
                    .Where(negocio =>
                        negocio.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                        negocio.ConfirmadoSAP == true &&
                        (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR));

                var queryBasicoConfirmas = from negocio in ne
                                           join confirma in co on negocio.Id equals confirma.NegocioId into c
                                           from confirma in c.DefaultIfEmpty()
                                           join negociopadre in nep on negocio.ContratoSAP equals negociopadre.ContratoSAP into np
                                           from negociopadre in np.DefaultIfEmpty()
                                           orderby negocio.Id descending
                                           select new BasicoConfirma
                                           {
                                               Id = negocio.Id,
                                               NegocioSAP = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).FijacionSAP : negocio.ContratoSAP,
                                               ContratoSAP = negocio.ContratoSAP,
                                               FijacionSAP = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).FijacionSAP : string.Empty,
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
                                               Version = confirma.Version > 1 ? confirma.Version : 1,
                                               Estado_Version = confirma.FechaAnulacion != null ? "Anulado" : (confirma.FechaGeneracion != null ? "Vigente" : "Pendiente"),
                                               BoletoId = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.BoletoId : negocio.BoletoId,
                                               TipoBoleto = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Boleto.Descripcion : negocio.Boleto.Descripcion,
                                               Canje = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (negociopadre.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO"),
                                               BolsaId = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.BolsaId : negocio.BolsaId,
                                               Bolsa = negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).Contrato.Bolsa.Descripcion : negocio.Bolsa.Descripcion,
                                               Precio = negocio.Precio,
                                               Moneda = negocio.MonedaId.Trim() == "ARP" ? "ARP" : (negocio.MonedaId.Trim() == "USDM" ? "USD" : string.Empty),
                                               FechaGeneracion = confirma.FechaGeneracion,
                                               FechaOperacion = negocio.FechaOperacion,
                                               FechaCarga = negocio.Fecha,
                                               FechaConfirmacion = negocio.FechaConfirmacion,
                                               FechaAnulacion = confirma.FechaAnulacion,
                                               Corredor = negocio.Corredor.RazonSocial ?? "",
                                               Vendedor = negocio.Proveedor.RazonSocial ?? "",
                                               MaterialId = negocio.MaterialId,
                                               Material = negocio.Material.Descripcion,
                                               ContratoVendedor = negocio.ContratoVendedor,
                                               ContratoCorredor = negocio.ContratoCorredor,
                                               Comercial = negocio.Comercial.Apellido + ", " + negocio.Comercial.Nombres,
                                           };
                return queryBasicoConfirmas.ToDataSourceResult(request);               
                
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Error en TraerConfirmasConFiltro: " + ex.Message);
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