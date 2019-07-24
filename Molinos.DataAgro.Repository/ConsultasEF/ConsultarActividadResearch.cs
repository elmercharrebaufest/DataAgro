using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ConsultarActividadResearch : IConsulta<ActividadRecordatorio>
    {
        private readonly int comercialId;

        public ConsultarActividadResearch(int comercialId)
        {
            this.comercialId = comercialId;
        }

        private static List<ActividadRecordatorio> Query(DbContext contexto, int comercialId)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var hoy = DateTime.Now.Date;
            var notiSiembra =
                from notif in contexto.Set<NotificacionResearch>()                
                where notif.FechaDesde <= hoy && notif.FechaHasta >= hoy && notif.TipoResearchId == 1 && 
                !(from siembra in contexto.Set<ResearchAvanceSiembra>()
                 where siembra.ComercialId == comercialId && siembra.MaterialId == notif.MaterialId && (notif.FechaDesde < siembra.FechaHora && siembra.FechaHora<notif.FechaHasta) select 1 ).Any()
                select new ActividadRecordatorio {
                    Tema = notif.TipoResearch.Descripcion,
                    ActividadId = notif.Id,
                    Comentarios = notif.Mensaje,
                    Dia = SqlFunctions.DateName("day", notif.FechaHasta) + "/" + SqlFunctions.DatePart("month", notif.FechaHasta) + "/" + SqlFunctions.DateName("year", notif.FechaHasta) ,
            };
            var notiCosecha =
                from notif in contexto.Set<NotificacionResearch>()
                where notif.FechaDesde <= hoy && notif.FechaHasta >= hoy && notif.TipoResearchId == 2 &&
                !(from siembra in contexto.Set<ResearchAvanceCosecha>()
                  where siembra.ComercialId == comercialId && siembra.MaterialId == notif.MaterialId && (notif.FechaDesde < siembra.FechaHora && siembra.FechaHora < notif.FechaHasta)
                  select 1).Any()
                select new ActividadRecordatorio
                {
                    Tema = notif.TipoResearch.Descripcion,
                    ActividadId = notif.Id,
                    Comentarios = notif.Mensaje,
                    Dia = SqlFunctions.DateName("day", notif.FechaHasta) + "/" + SqlFunctions.DatePart("month", notif.FechaHasta) + "/" + SqlFunctions.DateName("year", notif.FechaHasta),
                };
            var notiCultivo =
                from notif in contexto.Set<NotificacionResearch>()
                where notif.FechaDesde <= hoy && notif.FechaHasta >= hoy && notif.TipoResearchId == 3 &&
                !(from siembra in contexto.Set<ResearchSituacionCultivo>()
                  where siembra.ComercialId == comercialId && siembra.MaterialId == notif.MaterialId && (notif.FechaDesde < siembra.FechaHora && siembra.FechaHora < notif.FechaHasta)
                  select 1).Any()
                select new ActividadRecordatorio
                {
                    Tema = notif.TipoResearch.Descripcion,
                    ActividadId = notif.Id,
                    Comentarios = notif.Mensaje,
                    Dia = SqlFunctions.DateName("day", notif.FechaHasta) + "/" + SqlFunctions.DatePart("month", notif.FechaHasta) + "/" + SqlFunctions.DateName("year", notif.FechaHasta),
                };
            var notiStock =
                from notif in contexto.Set<NotificacionResearch>()
                where notif.FechaDesde <= hoy && notif.FechaHasta >= hoy && notif.TipoResearchId == 4 &&
                !(from siembra in contexto.Set<ResearchVentaStock>()
                  where siembra.ComercialId == comercialId && siembra.MaterialId == notif.MaterialId && (notif.FechaDesde < siembra.FechaHora && siembra.FechaHora < notif.FechaHasta)
                  select 1).Any()
                select new ActividadRecordatorio
                {
                    Tema = notif.TipoResearch.Descripcion,
                    ActividadId = notif.Id,
                    Comentarios = notif.Mensaje,
                    Dia = SqlFunctions.DateName("day", notif.FechaHasta) + "/" + SqlFunctions.DatePart("month", notif.FechaHasta) + "/" + SqlFunctions.DateName("year", notif.FechaHasta),
                };
            
            return notiSiembra.Union(notiCosecha).Union(notiCultivo).Union(notiStock).ToList();
        }

        public virtual List<ActividadRecordatorio> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, comercialId);
            }
        }
    }
}
