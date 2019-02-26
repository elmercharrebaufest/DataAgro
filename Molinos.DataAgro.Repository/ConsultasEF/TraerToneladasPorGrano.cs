using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerToneladasPorGrano : IConsultaEscalar<ToneladasGranoTipoDto>
    {
        private readonly int materialId;
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly bool? calidad;
        public TraerToneladasPorGrano(int materialId, DateTime fechaDesde, DateTime fechaHasta, bool? calidad = null)
        {
            this.materialId = materialId;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.calidad = calidad;
        }

        public ToneladasGranoTipoDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var toneladasPorGrano = new ToneladasGranoTipoDto();
            var query = contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)))
                .GroupBy(x => x.Material.MaterialId).DefaultIfEmpty()
                .Select(x => new ToneladasGranoTipoDto()
                {
                    DispAFijar = x.Where(y => (DbFunctions.TruncateTime(y.FechaDesde) <= DbFunctions.TruncateTime(y.Fecha) && y.TipoNegocioId == 1 && y.Material.CampañaId == y.CampanaId) || (y.Material.CampañaId > y.CampanaId && y.TipoNegocioId == 1)).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    DispAPrecio = x.Where(y => (DbFunctions.TruncateTime(y.FechaDesde) <= DbFunctions.TruncateTime(y.Fecha) && y.TipoNegocioId == 2 && y.Material.CampañaId == y.CampanaId) || (y.Material.CampañaId > y.CampanaId && y.TipoNegocioId == 2)).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    FrwAFijar = x.Where(y => DbFunctions.TruncateTime(y.FechaDesde) > DbFunctions.TruncateTime(y.Fecha) && y.TipoNegocioId == 1 && y.Material.CampañaId == y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    FrwAPrecio = x.Where(y => DbFunctions.TruncateTime(y.FechaDesde) > DbFunctions.TruncateTime(y.Fecha) && y.TipoNegocioId == 2 && y.Material.CampañaId == y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    NewAFijar = x.Where(y => y.TipoNegocioId == 1 && y.Material.CampañaId < y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    NewAPrecio = x.Where(y => y.TipoNegocioId == 2 && y.Material.CampañaId < y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                }).First();

            toneladasPorGrano.DispAFijar = query.DispAFijar;
            toneladasPorGrano.DispAPrecio = query.DispAPrecio;
            toneladasPorGrano.FrwAFijar = query.FrwAFijar;
            toneladasPorGrano.FrwAPrecio = query.FrwAPrecio;
            toneladasPorGrano.NewAFijar = query.NewAFijar;
            toneladasPorGrano.NewAPrecio = query.NewAPrecio;

            query = contexto.Set<FijacionDePrecioContrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.TrigoEspecial == calidad)))
                .GroupBy(x => x.Material.MaterialId).DefaultIfEmpty()
                .Select(x => new ToneladasGranoTipoDto()
                {
                    DispFijac = x.Where(y => (DbFunctions.TruncateTime(y.FechaDesde) <= DbFunctions.TruncateTime(y.Fecha) && y.Material.CampañaId == y.CampanaId) || (y.Material.CampañaId > y.CampanaId )).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    FrwFijac = x.Where(y => DbFunctions.TruncateTime(y.FechaDesde) > DbFunctions.TruncateTime(y.Fecha) && y.Material.CampañaId == y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    NewFijac = x.Where(y => y.Material.CampañaId < y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                }).First();

            toneladasPorGrano.DispFijac = query.DispFijac;
            toneladasPorGrano.FrwFijac = query.FrwFijac;
            toneladasPorGrano.NewFijac = query.NewFijac;

            query = contexto.Set<Fason>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == materialId && (calidad == null || (calidad != null && x.Especial == calidad)))
                .GroupBy(x => x.Material.MaterialId).DefaultIfEmpty()
                .Select(x => new ToneladasGranoTipoDto()
                {
                    DispFason = x.Where(y => (DbFunctions.TruncateTime(y.FechaDesde) <= DbFunctions.TruncateTime(y.Fecha) && y.Material.CampañaId == y.CampanaId) || (y.Material.CampañaId > y.CampanaId)).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    FrwFason = x.Where(y => DbFunctions.TruncateTime(y.FechaDesde) > DbFunctions.TruncateTime(y.Fecha) && y.Material.CampañaId == y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                    NewFason = x.Where(y => y.Material.CampañaId < y.CampanaId).Select(y => Math.Ceiling(y.Cantidad / 1000)).DefaultIfEmpty(0).Sum(),
                }).First();

            toneladasPorGrano.DispFason = query.DispFason;
            toneladasPorGrano.FrwFason = query.FrwFason;
            toneladasPorGrano.NewFason = query.NewFason;

            toneladasPorGrano.Total = toneladasPorGrano.DispAFijar + toneladasPorGrano.DispAPrecio + toneladasPorGrano.DispFijac + toneladasPorGrano.DispFason +
               toneladasPorGrano.FrwAFijar + toneladasPorGrano.FrwAPrecio + toneladasPorGrano.FrwFijac + toneladasPorGrano.FrwFason +
               toneladasPorGrano.NewAFijar + toneladasPorGrano.NewAPrecio + toneladasPorGrano.NewFijac + toneladasPorGrano.NewFason;

            return toneladasPorGrano;
        }
    }
}
