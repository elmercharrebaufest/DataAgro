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
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int centroId;
        public TraerToneladasSojaSustentable(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.centroId = centroId;
        }

        public ReporteSojaSustDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;

            var contratos = contexto.Set<Contrato>().Where(x =>
            x.OcultarEnTablero == false &&
            DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy &&
            DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana &&
            (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) &&
            /*x.MaterialId == 3 &&*/
            (x.ImporteSustentable != null && x.MonedaSustentableId != null) &&
            (centroId == 0 || x.DestinoId == centroId) &&
            x.ContratoAcuerdo == null &&
            x.TipoAgenteCompraId == null && 
            x.Venta != true &&
            x.Canje != true 
            ).ToList();
            var result = new ReporteSojaSustDto();
            if (contratos.Count > 0)
            {
                result = contratos.GroupBy(x => x.Material.MaterialId).DefaultIfEmpty().Select(x => new ReporteSojaSustDto()
                {
                    Fijar = x.Where(y => y.TipoNegocioId == 1).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Precio = x.Where(y => y.TipoNegocioId == 2).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Total = x.Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Ids = x.Select(a => new KeyValuePair<int, int>(a.TipoNegocioId, a.Id))
                }).First();
            }

            return result;
        }
    }
}
