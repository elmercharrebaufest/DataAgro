using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using Molinos.DataAgro.Entities.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;
using Molinos.DataAgro.Entities.Seguridad;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodoPrecioMoaPizarra : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;

        public TraerTodoPrecioMoaPizarra(DataSourceRequest request)
        {
            this.request = request;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request)
        {

            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryMoa =
                from moa in contexto.Set<PrecioMoa>()
                select new PrecioMoaDto
                {
                    Id = moa.Id,
                    Material = moa.Material.Descripcion,
                    Precio = moa.Precio,
                    TipoNegocio = moa.TipoNegocio.Descripcion,
                    MonedaId = moa.MonedaId,
                    DesdeVigencia = moa.DesdeVigencia,
                    HastaVigencia = moa.HastaVigencia,
                    DesdeEntrega = moa.DesdeEntrega,
                    HastaEntrega = moa.HastaEntrega,
                    DesdeFijacion = moa.DesdeFijacion,
                    HastaFijacion = moa.HastaFijacion,
                    TipoConfiguracion = "PRECIO MOA",
                    UsuarioCreador = moa.UsuarioCreador.Nombres + moa.UsuarioCreador.Apellido,
                    FechaCreacion = moa.FechaCreacion
                };

            var queryPizarra =
                from pizarra in contexto.Set<HabilitacionPizarra>()
                select new PrecioMoaDto
                {
                    Id = pizarra.Id,
                    Material = pizarra.Material.Descripcion,
                    Precio = 0,
                    TipoNegocio = pizarra.TipoNegocio.Descripcion,
                    MonedaId = "",
                    DesdeVigencia = pizarra.DesdeVigencia,
                    HastaVigencia = pizarra.HastaVigencia,
                    DesdeEntrega = pizarra.DesdeEntrega,
                    HastaEntrega = pizarra.HastaEntrega,
                    DesdeFijacion = null,
                    HastaFijacion = null,
                    TipoConfiguracion = "HABILITACION PIZARRA",
                    UsuarioCreador = pizarra.UsuarioCreador.Nombres + pizarra.UsuarioCreador.Apellido,
                    FechaCreacion = pizarra.FechaCreacion
                };
            var query = queryMoa.Union(queryPizarra);
            GridHelper.TruncateTime(request.Filter, ref query);
            return query.ToDataSourceResult<PrecioMoaDto>(request);
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
