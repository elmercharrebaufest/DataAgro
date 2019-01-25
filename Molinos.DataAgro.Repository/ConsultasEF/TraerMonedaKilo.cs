using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerMonedaKilo : IConsulta<PrecioCantidadDto>
    {
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        public TraerMonedaKilo(DateTime fechaDesde, DateTime fechaHasta)
        {
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
        }

        public List<PrecioCantidadDto> Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;

            var cont = contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5))
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad= x.Sum(y => (double)y.Precio * y.Cantidad/1000)
                }).ToList();

            var fij = contexto.Set<FijacionDePrecioContrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5))
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => (double)y.Precio * y.Cantidad / 1000)
                }).ToList();

            var fas = contexto.Set<Fason>().Where(x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5))
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => (double)y.Precio * y.Cantidad / 1000)
                }).ToList();
            var res = cont.Union(fij).Union(fas).GroupBy(x=>x.Moneda)
                .Select(x=> new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad = x.Sum(y => y.Cantidad)
                }).ToList();
            return res;
        }
    }
}
