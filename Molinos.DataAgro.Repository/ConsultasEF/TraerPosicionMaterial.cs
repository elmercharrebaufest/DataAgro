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
    public class TraerPosicionMaterial : IConsulta<PosicionKilos>
    {
        private readonly int materialId;
        private readonly bool? calidad;
        private readonly DateTime fecha;

        public TraerPosicionMaterial(int materialId,DateTime fecha, bool? calidad = null )
        {
            this.materialId = materialId;
            this.calidad = calidad;
            this.fecha = fecha;
        }
        
        private static List<PosicionKilos> Query(DbContext contexto, int materialId, DateTime fecha, bool? calidad)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fecha.Date;

            return contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)))
                .GroupBy(x => SqlFunctions.DatePart("Month", x.FechaHasta) ?? 11)
                       .Select(x => new PosicionKilos()
                       {
                           Mes = x.Key != 0 ? (EnumMeses)x.Key : (EnumMeses)11,
                           Kilos = x.Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum()
                       }
                       ).ToList();
        }

        public virtual List<PosicionKilos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, materialId, fecha, calidad);
            }
        }
    }
}
