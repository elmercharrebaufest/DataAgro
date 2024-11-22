using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerToneladasSojaEPAyEUDR : IConsultaEscalar<ReporteSojaEPAyEUDRDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int centroId;

        public TraerToneladasSojaEPAyEUDR(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.centroId = centroId;
        }

        public ReporteSojaEPAyEUDRDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;

            var contratos = contexto.Set<Contrato>().Where(x =>
            x.OcultarEnTablero == false &&
            DbFunctions.TruncateTime(x.FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaManana &&
            (x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error || x.EstadoId == (int)EnumEstadoContrato.Finalizado) &&
            x.ImporteSustentable != null && x.MonedaSustentableId != null && (x.EPA || x.EUDR) &&
            (centroId == 0 || x.DestinoId == centroId) &&
            x.ContratoAcuerdo == null &&
            x.TipoAgenteCompraId == null &&
            x.Venta != true &&
            x.AnulaYReemplazaContratoId == null
            ).ToList();

            var result = new ReporteSojaEPAyEUDRDto();
            if (contratos.Any())
            {
                result = contratos.GroupBy(x => x.Material.MaterialId).DefaultIfEmpty().Select(x => new ReporteSojaEPAyEUDRDto()
                {
                    Fijar = x.Where(y => y.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Precio = x.Where(y => y.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO).Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Total = x.Select(y => Math.Round(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    Ids = x.Select(a => new KeyValuePair<int, int>(a.TipoNegocioId, a.Id))
                }).First();
            }

            return result;
        }
    }
}
