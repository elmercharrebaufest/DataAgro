using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerToneladasSojaSustentable : IConsultaEscalar<ReporteSojaSustDto>
    {
        private readonly DateTime fecha;
        public TraerToneladasSojaSustentable(DateTime fecha)
        {
            this.fecha = fecha;
        }

        public ReporteSojaSustDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fecha.Date;

            return contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && /*x.MaterialId == 3 &&*/ (x.ImporteSustentable != null && x.MonedaSustentableId != null))
                .GroupBy(x => x.Material.MaterialId).DefaultIfEmpty()
                .Select(x => new ReporteSojaSustDto()
                {
                    Fijar = x.Where(y => y.TipoNegocioId == 1).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Precio = x.Where(y => y.TipoNegocioId == 2).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Total = x.Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                }).First();
        }
    }
}
