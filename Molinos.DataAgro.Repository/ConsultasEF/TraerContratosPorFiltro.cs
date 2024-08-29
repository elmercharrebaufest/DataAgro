using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerContratosPorFiltro : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerContratosPorFiltro(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, equipo);
            GridHelper.TruncateTime(request.Filter, ref queryContratos);
            if (request.Filter == null)
            {
                request.Filter = new Filter { Filters = new List<Filter>() { new Filter() { Field = "Fecha", Operator = "gte", Value = DateTime.Now.Date } }, Logic = "and" };
            }
            var result = queryContratos.ToDataSourceResult<BasicoContrato>(request);
            var cargaDesde = DateTime.Now.Date;
            var cargaHasta = DateTime.Now.Date;

            if (request.Filter != null && request.Filter.Filters != null)
            {
                foreach (var item in request.Filter.Filters)
                {
                    if (item.Field == "Fecha")
                    {
                        if (item.Operator == "gte")
                        {
                            cargaDesde = (DateTime)item.Value;
                        }
                        else
                        {
                            cargaHasta = (DateTime)item.Value;
                        }
                    }
                }
            }

            List<PrecioPizarra> listaPizarra = contexto.Set<PrecioPizarra>().Where(a => ((a.FechaDesde >= cargaDesde && a.FechaDesde <= cargaHasta)
                                                                                      || (a.FechaHasta >= cargaDesde && a.FechaHasta <= cargaHasta)
                                                                                      || (a.FechaDesde <= cargaDesde && a.FechaHasta >= cargaHasta))
                                                                                      && a.PizarraId == 1).ToList();
            foreach (var item in result.Data)
            {
                if (((item as BasicoContrato).TipoNegocioId == 3 || (item as BasicoContrato).TipoNegocioId == 2) && (item as BasicoContrato).Pizarra == true && (item as BasicoContrato).Fecha.HasValue)
                {
                    var pizarra = listaPizarra.Where(a => a.FechaDesde <= (item as BasicoContrato).Fecha.Value && a.FechaHasta >= (item as BasicoContrato).Fecha.Value && a.MaterialId == (item as BasicoContrato).MaterialId).SingleOrDefault();
                    if (pizarra != null)
                    {
                        (item as BasicoContrato).Precio = pizarra.Precio;
                        (item as BasicoContrato).Moneda = pizarra.MonedaId;
                    }
                }
            }
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
