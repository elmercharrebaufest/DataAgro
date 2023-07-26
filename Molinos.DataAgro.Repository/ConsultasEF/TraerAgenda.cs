using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerAgenda : IConsulta<AgendaStore>
    {
        private readonly List<int> equipo;
        private readonly RptActividadAgendaParam rptActividadAgendaParam;

        public TraerAgenda(List<int> equipo, RptActividadAgendaParam rptActividadAgendaParam)
        {
            this.equipo = equipo;
            this.rptActividadAgendaParam = rptActividadAgendaParam;
        }

        private static List<AgendaStore> Query(DbContext contexto, List<int> equipo, RptActividadAgendaParam rpt)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from proveedorComercial in contexto.Set<ProveedorComercial>()
                join actividad in contexto.Set<Actividad>() on proveedorComercial.Proveedor.ProveedorId equals actividad.Proveedor.ProveedorId into facs
                from actividad in facs.DefaultIfEmpty()
                join contactoComercial in contexto.Set<ContactoComercial>() on proveedorComercial.Proveedor.ProveedorId equals contactoComercial.Proveedor.ProveedorId into cons
                from contactoComercial in cons.DefaultIfEmpty()
                where equipo.Contains(proveedorComercial.Comercial.ComercialId) &&
                    (rpt.ActividadDetalle == null || actividad.Detalle.Contains(rpt.ActividadDetalle)) &&
                    (rpt.TipoActividad == null || actividad.TipoActividad.TipoActividadId == rpt.TipoActividad) &&
                    (rpt.ProveedorId == null || actividad.Proveedor.ProveedorId == rpt.ProveedorId) &&
                    (rpt.FechaDesde == null || DbFunctions.TruncateTime(actividad.FechaHoraActividad) >= DbFunctions.TruncateTime(rpt.FechaDesde)) &&
                    (rpt.FechaHasta == null || DbFunctions.TruncateTime(actividad.FechaHoraActividad) <= DbFunctions.TruncateTime(rpt.FechaHasta))
                orderby actividad.FechaHoraRecordatorio == null ? 1 : 0, actividad.FechaHoraRecordatorio
                select new AgendaStore
                {
                    ActividadId = (int?)actividad.ActividadId ?? 0,
                    Apellido = proveedorComercial.Comercial.Apellido ?? "",
                    Detalle = actividad.Detalle ?? "",
                    FechaHoraActividad = actividad.FechaHoraActividad,
                    FechaHoraRecordatorio = actividad.FechaHoraRecordatorio.HasValue ? actividad.FechaHoraRecordatorio : actividad.FechaHoraActividad,
                    NombreContacto = contactoComercial.Apellido + " " + contactoComercial.Nombres,
                    RazonSocial = proveedorComercial.Proveedor.RazonSocial ?? "",
                    TipoDeAcividad = actividad.TipoActividad.Descripcion ?? ""
                };

            return resultado.ToList();
        }

        public virtual List<AgendaStore> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, equipo, rptActividadAgendaParam);
            }
        }
    }
}
