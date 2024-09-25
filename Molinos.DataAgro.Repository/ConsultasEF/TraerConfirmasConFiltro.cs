using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Transactions;
using Molinos.DataAgro;
using System.Security.Cryptography;
using System.Web.UI.WebControls.WebParts;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerConfirmasConFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;

        public TraerConfirmasConFiltro(DataSourceRequest request)
        {
            this.request = request;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            // Consulta base para obtener los negocios aplicables
            var queryNegocios = contexto.Set<Negocio>().AsQueryable();

            // Consulta para obtener los confirmas asociados
            var queryConfirmas = contexto.Set<Confirma>().Select(c => new
            {
                c.Id,
                c.NegocioId,
                c.FechaGeneracion,
                c.ComercialId
            });

            // Consulta para obtener la descripción del tipo de boleto
            var queryTiposBoleto = contexto.Set<BoletoCompraNet>().Select(tb => new
            {
                tb.Id,
                tb.Descripcion
            });

            // Consulta para obtener la descripción del tipo de negocio
            var queryTiposNegocio = contexto.Set<TipoNegocio>().Select(tb => new
            {
                tb.TipoNegocioId,
                tb.Descripcion
            });

            // Consulta para obtener la descripción de la bolsa
            var queryBolsas = contexto.Set<BolsaCompraNet>().Select(b => new
            {
                b.Id,
                b.Descripcion
            });

            // Consulta para obtener la descripción del proveedor (corredor)
            var queryProveedores = contexto.Set<Proveedor>().Select(p => new
            {
                p.ProveedorId,
                p.RazonSocial
            });

            // Consulta para obtener datos del negocio padre
            var queryNegociosPadre = queryNegocios
                .Join(queryNegocios,
                    negocio => negocio.ContratoSAP,
                    parent => parent.ContratoSAP,
                    (negocio, parent) => new { negocio, parent })
                .Where(np => np.parent.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                .Select(np => new
                {
                    np.negocio.ContratoSAP,
                    np.parent.BoletoId,
                    np.parent.BolsaId,
                    np.parent.Canje
                });

            // Aplicar los filtros de Kendo Grid
            GridHelper.TruncateTime(request.Filter, ref queryNegocios);

            if (request.Filter == null)
            {
                request.Filter = new Filter
                {
                    Filters = new List<Filter>
                    {
                        new Filter
                        {
                            Field = "Fecha",
                            Operator = "gte",
                            Value = DateTime.Now.Date
                        }
                    },
                    Logic = "and"
                };
            }

            var queryBasicoConfirmas = from negocio in queryNegocios
                                     join confirma in queryConfirmas on negocio.Id equals confirma.NegocioId into c
                                     from confirma in c.DefaultIfEmpty()
                                     join np in queryNegociosPadre on negocio.ContratoSAP equals np.ContratoSAP into npGroup
                                     from np in npGroup.DefaultIfEmpty()
                                     join tipoBoleto in queryTiposBoleto on
                                         (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                                            ? np.BoletoId
                                            : negocio.BoletoId)
                                         equals tipoBoleto.Id into t
                                     from tipoBoleto in t.DefaultIfEmpty()
                                     join tipoNegocio in queryTiposNegocio on negocio.TipoNegocioId equals tipoNegocio.TipoNegocioId into tn
                                     from tipoNegocio in tn.DefaultIfEmpty()
                                     join bolsa in queryBolsas on
                                         (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION
                                            ? np.BolsaId
                                            : negocio.BolsaId)
                                         equals bolsa.Id into bl
                                     from bolsa in bl.DefaultIfEmpty()
                                     join proveedorCorredor in queryProveedores on negocio.CorredorId equals proveedorCorredor.ProveedorId into pCorredor
                                     from proveedorCorredor in pCorredor.DefaultIfEmpty()
                                     join proveedorVendedor in queryProveedores on negocio.ProveedorId equals proveedorVendedor.ProveedorId into pVendedor
                                     from proveedorVendedor in pVendedor.DefaultIfEmpty()
                                     where
                                        negocio.ConfirmadoSAP == true &&
                                        negocio.EstadoId == (int)EnumEstadoContrato.Finalizado &&
                                        (
                                            (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                                             np.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                                            ||
                                            (negocio.TipoNegocioId != (int)EnumTipoNegocio.FIJACION &&
                                             negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                                        )
                                     select new BasicoConfirma
                                     {
                                         Id = negocio.Id,
                                         NegocioSAP = (negocio as FijacionDePrecioContrato).FijacionSAP ?? negocio.ContratoSAP,
                                         ContratoSAP = negocio.ContratoSAP ?? "",
                                         FijacionSAP = (negocio as FijacionDePrecioContrato).FijacionSAP ?? "",
                                         TipoBoleto = tipoBoleto.Descripcion ?? "Ninguno",
                                         TipoNegocio = tipoNegocio.Descripcion ?? "Ninguno",
                                         Canje = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (np.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO"),
                                         Bolsa = bolsa.Descripcion ?? "",
                                         Precio = negocio.Precio,
                                         Moneda = negocio.MonedaId == "ARP" ? "ARP" : (negocio.MonedaId == "USDM" ? "USD" : string.Empty),
                                         FechaGeneracion = confirma.FechaGeneracion,
                                         FechaOperacion = negocio.FechaOperacion,
                                         FechaConfirmacion = negocio.FechaConfirmacion,
                                         Corredor = proveedorCorredor.RazonSocial ?? "",
                                         Vendedor = proveedorVendedor.RazonSocial ?? "",
                                         TipoNegocioId = negocio.TipoNegocioId
                                     };

            var orderedQuery = queryBasicoConfirmas.OrderByDescending(b => b.Id);

            // Aplicar filtros, ordenamientos y paginación de grilla Kendo
            var result = orderedQuery.ToDataSourceResult(request);

            return result;
        }


        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
