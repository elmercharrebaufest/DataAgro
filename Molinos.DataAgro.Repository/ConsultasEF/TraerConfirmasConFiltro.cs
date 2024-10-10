using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
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

            var queryConfirmas = contexto.Set<Confirma>()
            .GroupBy(c => c.NegocioId)
            .Select(g => g.OrderByDescending(c => c.Version).FirstOrDefault()) // Obtener la última versión
            .Select(c => new
            {
                c.Id,
                c.NegocioId,
                c.FechaGeneracion,
                c.ComercialId,
                c.Version,
                c.FechaAnulacion
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

            var queryMateriales = contexto.Set<Material>().Select(b => new
            {
                b.MaterialId,
                b.Descripcion
            });

            var queryComerciales = contexto.Set<Comercial>().Select(b => new
            {
                b.ComercialId,
                b.Apellido,
                b.Nombres
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
                                       join material in queryMateriales on negocio.MaterialId equals material.MaterialId into m
                                       from material in m.DefaultIfEmpty()
                                       join comercial in queryComerciales on negocio.ComercialId equals comercial.ComercialId into cm
                                       from comercial in cm.DefaultIfEmpty()
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
                                           negocio.Estado == (int)EnumEstadoContrato.Finalizado &&
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
                                           NegocioSAP = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? negocio.FijacionSAP : negocio.ContratoSAP,
                                           ContratoSAP = negocio.ContratoSAP ?? "",
                                           FijacionSAP = negocio.FijacionSAP ?? "",
                                           TipoBoleto = tipoBoleto.Descripcion ?? "Ninguno",
                                           TipoNegocio = negocio.TipoNegocio,
                                           Canje = negocio.TipoNegocioId == (int)EnumTipoNegocio.FIJACION ? (np.Canje == true ? "SI" : "NO") : (negocio.Canje == true ? "SI" : "NO"),
                                           Bolsa = bolsa.Descripcion ?? "",
                                           Precio = negocio.Precio,
                                           Moneda = negocio.MonedaId == "ARP" ? "ARP" : (negocio.MonedaId == "USDM" ? "USD" : string.Empty),
                                           FechaGeneracion = confirma.FechaGeneracion,
                                           FechaOperacion = negocio.FechaOperacion,
                                           FechaConfirmacion = negocio.FechaConfirmacion,
                                           FechaAnulacion = confirma.FechaAnulacion,
                                           Corredor = proveedorCorredor.RazonSocial ?? "",
                                           Vendedor = proveedorVendedor.RazonSocial ?? "",
                                           TipoNegocioId = negocio.TipoNegocioId,
                                           Material = material.Descripcion,
                                           ContratoVendedor = negocio.ContratoVendedor,
                                           ContratoCorredor = negocio.ContratoCorredor,
                                           Comercial = comercial.Apellido + ", " + comercial.Nombres,
                                           Version_Proxima = confirma.Version > 0 ? confirma.Version + 1 : 1,
                                           Estado_Version = confirma.FechaAnulacion != null ? "Anulado":(confirma.FechaGeneracion != null? "Vigente":"Pendiente"),
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
