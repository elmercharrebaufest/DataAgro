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
            var externo = PermisosHelper.Is(PermisosDataAgro.IngresoExterno);
            var nombreUsuario = PermisosHelper.ObtenerUsuario();
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryCupos =
                from cupo in contexto.Set<Cupo>()
                where (equipo.Contains(cupo.ComercialId != null ? cupo.ComercialId.Value : 0) && !externo ) ||
                (externo && cupo.UsuarioCreador == nombreUsuario)

                select new CupoDto
                {

                    Id = cupo.Id,
                    ComercialId = cupo.ComercialId,
                    FechaIngreso = cupo.FechaIngreso,
                    FechaGeneracion = cupo.FechaGeneracion,
                    FechaRegistro = DbFunctions.TruncateTime(cupo.FechaGeneracion),
                    Hora = SqlFunctions.DateName("hh", cupo.FechaGeneracion) + ":" + DbFunctions.Right("00" + SqlFunctions.DateName("n", cupo.FechaGeneracion), 2),
                    Comercial = cupo.Comercial.Nombres + " " + cupo.Comercial.Apellido,
                    CupoSap = cupo.CupoSap,
                    CupoStop = cupo.CupoStop.ToString(),
                    Material = cupo.Material.Descripcion,
                    MaterialId = cupo.Material.MaterialId,
                    Proveedor = !string.IsNullOrEmpty(cupo.Proveedor.Alias) ? cupo.Proveedor.Alias + " - " + cupo.Proveedor.RazonSocial : cupo.Proveedor.RazonSocial,
                    ProveedorId = cupo.ProveedorId,
                    Destinatario = cupo.Destinatario,
                    Centro = cupo.Centro.Descripcion,
                    CentroId = cupo.CentroId,
                    Calidad = cupo.Calidad,
                    ZonaCupo = cupo.ZonaCupo.Descripcion,
                    ZonaCupoSap = cupo.ZonaCupo.CodigoSap,
                    ZonaCupoId = cupo.ZonaCupoId,
                    FleteProcedencia = cupo.FleteProcedencia,
                    Observaciones = cupo.Observaciones,
                    EstadoCupoId = cupo.EstadoCupoId,
                    EstadoCupo = externo ? (cupo.EstadoCupo.Descripcion == "Sin STOP") ? "Pendiente" : (cupo.EstadoCupo.Descripcion == "Sin CTG") ? "Aceptado" : cupo.EstadoCupo.Descripcion : cupo.EstadoCupo.Descripcion,
                    MensajeError = cupo.ErrorStop,
                    Acopio = cupo.Centro.Acopio,
                    UsuarioCreador = cupo.UsuarioCreador,
                    
                    CartaPorte = cupo.CartaPorte,
                    Chofer = cupo.Chofer,
                    CorredorComprador = cupo.CorredorComprador,
                    CorredorVendedor = cupo.CorredorVendedor,
                    Cosecha = cupo.Cosecha,
                    CTG = cupo.CTG,
                    CTGFechaDesde = cupo.CTGFechaDesde.Value,
                    CTGFechaHasta = cupo.CTGFechaHasta.Value,
                    EstadoPlanta = cupo.EstadoPlanta,
                    IntermediarioFlete = cupo.IntermediarioFlete,
                    Km = cupo.Km,
                    MercadoATermino = cupo.MercadoATermino,
                    Peso = cupo.Peso,
                    CuitOrigen = cupo.CuitOrigen,
                    RemitenteComercial = cupo.RemitenteComercial,
                    Transportista = cupo.Transportista,
                    NroEstablecimientoOrigen = cupo.NroEstablecimientoOrigen,
                    CodLocalidadOrigen = cupo.CodLocalidadOrigen,
                    CuitOrigenAfip = cupo.CuitOrigenAfip,
                    MotivoRechazo = cupo.MotivoRechazo,
                    //Fecha = SqlFunctions.DateName("day", cupo.FechaIngreso) + "/" + SqlFunctions.DatePart("month", cupo.FechaIngreso) + "/" + SqlFunctions.DateName("year", cupo.FechaIngreso),
                    EstadoOrden = cupo.EstadoCupo.Orden,
                    Sustentable = cupo.Sustentable,
                    EPA = cupo.EPA,
                    ConDescarga = cupo.ConDescarga,
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
