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

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerTodosCupos : IConsultaEscalar<DataSourceResult>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;

        public TraerTodosCupos(DataSourceRequest request, List<int> equipo)
        {
            this.request = request;
            this.equipo = equipo;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> equipo)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryCupos =
                from cupo in contexto.Set<Cupo>()
                where equipo.Contains(cupo.ComercialId != null ? cupo.ComercialId.Value : 0)

                select new CupoDto
                {

                    Id = cupo.Id,
                    ComercialId = cupo.ComercialId,
                    FechaIngreso = cupo.FechaIngreso,
                    FechaGeneracion = cupo.FechaGeneracion,
                    FechaRegistro = DbFunctions.Right("00" + cupo.FechaGeneracion.Day, 2) + "/" + DbFunctions.Right("00" + cupo.FechaGeneracion.Month, 2) + "/" + DbFunctions.Right("0000" + cupo.FechaGeneracion.Year, 4),
                    Hora = SqlFunctions.DateName("hh", cupo.FechaGeneracion) + ":" + SqlFunctions.DateName("mi", cupo.FechaGeneracion),
                    Comercial = cupo.Comercial.Nombres + " " + cupo.Comercial.Apellido,
                    CupoSap = cupo.CupoSap,
                    CupoStop = cupo.CupoStop.ToString(),
                    Material = cupo.Material.Descripcion,
                    MaterialId = cupo.Material.MaterialId,
                    Proveedor = cupo.Proveedor.RazonSocial,
                    ProveedorId = cupo.ProveedorId,
                    Destinatario = cupo.Destinatario,
                    Centro = cupo.Centro.Descripcion,
                    CentroId = cupo.CentroId,
                    Calidad = cupo.Calidad,
                    ZonaCupo = cupo.ZonaCupo.Descripcion,
                    ZonaCupoId = cupo.ZonaCupoId,
                    FleteProcedencia = cupo.FleteProcedencia,
                    Observaciones = cupo.Observaciones,
                    EstadoCupoId = cupo.EstadoCupoId,
                    EstadoCupo = cupo.EstadoCupo.Descripcion,
                    MensajeError = cupo.ErrorStop,
                    Acopio = cupo.Centro.Acopio,
                    //Fecha = SqlFunctions.DateName("day", cupo.FechaIngreso) + "/" + SqlFunctions.DatePart("month", cupo.FechaIngreso) + "/" + SqlFunctions.DateName("year", cupo.FechaIngreso),

                    EstadoOrden = cupo.EstadoCupo.Orden
                };

            GridHelper.TruncateTime(request.Filter, ref queryCupos);
            return queryCupos.ToDataSourceResult<CupoDto>(request);
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
