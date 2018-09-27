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
        public TraerMonedaKilo()
        {
        }

        public List<PrecioCantidadDto> Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaHoy = DateTime.Now.Date;

            return contexto.Set<Contrato>().Where(x => DbFunctions.TruncateTime(x.Fecha) == fechaHoy && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5))
                .GroupBy(x => x.MonedaId).DefaultIfEmpty()
                .Select(x => new PrecioCantidadDto()
                {
                    Moneda = x.Key,
                    Cantidad= x.Sum(y => (double)y.Precio)
                }).ToList();
        }
    }
}
