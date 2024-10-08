using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Transactions;

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
            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, equipo);
            GridHelper.TruncateTime(request.Filter, ref queryContratos);
            var queryNegocios = contexto.Set<Negocio>().AsQueryable();

            var queryConfirmas = contexto.Set<Confirma>().Select(c => new
            {
                c.Id,
                c.NegocioId,
                c.FechaGeneracion,
                c.ComercialId
            });

            var queryTiposBoleto = contexto.Set<BoletoCompraNet>().Select(tb => new
            {
                tb.Id,
                tb.Descripcion
            });

            var queryTiposNegocio = contexto.Set<TipoNegocio>().Select(tb => new
            {
                tb.TipoNegocioId,
                tb.Descripcion
            });

            var queryBolsas = contexto.Set<BolsaCompraNet>().Select(b => new
            {
                b.Id,
                b.Descripcion
            });

            var queryProveedores = contexto.Set<Proveedor>().Select(p => new
            {
                p.ProveedorId,
                p.RazonSocial
            });

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

            // Construir la consulta
            var queryBasicoConfirmas = from negocio in queryContratos
                                       join confirma in queryConfirmas on negocio.Id equals confirma.NegocioId into confirmaGroup
                                       from confirma in confirmaGroup.DefaultIfEmpty()

                                       join np in queryNegociosPadre on negocio.ContratoSAP equals np.ContratoSAP into npGroup
                                       from np in npGroup.DefaultIfEmpty()

                                       let boletoId = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? np.BoletoId : negocio.BoletoId
                                       let bolsaId = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? np.BolsaId : negocio.BolsaId
                                       let canje = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (np.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO")

                                       join tipoBoleto in queryTiposBoleto on boletoId equals tipoBoleto.Id into tipoBoletoGroup
                                       from tipoBoleto in tipoBoletoGroup.DefaultIfEmpty()

                                       join tipoNegocio in queryTiposNegocio on negocio.TipoNegocioId equals tipoNegocio.TipoNegocioId into tipoNegocioGroup
                                       from tipoNegocio in tipoNegocioGroup.DefaultIfEmpty()

                                       join bolsa in queryBolsas on bolsaId equals bolsa.Id into bolsaGroup
                                       from bolsa in bolsaGroup.DefaultIfEmpty()

                                       join proveedorCorredor in queryProveedores on negocio.CorredorId equals proveedorCorredor.ProveedorId into pCorredor
                                       from proveedorCorredor in pCorredor.DefaultIfEmpty()

                                       join proveedorVendedor in queryProveedores on negocio.ProveedorId equals proveedorVendedor.ProveedorId into pVendedor
                                       from proveedorVendedor in pVendedor.DefaultIfEmpty()

                                       where
                                            negocio.ConfirmadoSAP == true &&
                                            negocio.Estado == (int)EnumEstadoContrato.Finalizado &&
                                            (
                                                (negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION && np.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                                                ||
                                                (negocio.TipoNegocioId != (int)EnumTipoNegocio.FIJACION && negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                                            )
                                       select new BasicoConfirma
                                       {
                                           Id = negocio.Id,
                                           NegocioSAP = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.FijacionSAP : negocio.ContratoSAP,
                                           ContratoSAP = negocio.ContratoSAP ?? "",
                                           FijacionSAP = negocio.FijacionSAP ?? "",
                                           TipoBoleto = tipoBoleto.Descripcion ?? "Ninguno",
                                           TipoNegocio = tipoNegocio.Descripcion ?? "Ninguno",
                                           Canje = canje,
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
                return Query(contexto, request, equipo);
            }
        }
    }
}
