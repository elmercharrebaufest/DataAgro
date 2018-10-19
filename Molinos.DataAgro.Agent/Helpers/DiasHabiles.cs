using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DiasHabiles
    {
        public List<DateTime> ObtenerDiasHabiles(IRepositorio repositorio)
        {
            var fecha = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                DateTime.TryParse(i.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString(), out DateTime dia);
                if (dia.DayOfWeek != DayOfWeek.Saturday && dia.DayOfWeek != DayOfWeek.Sunday && !repositorio.Listar<FechaFeriado>().Select(x=>x.Feriado).Contains(dia))
                {
                    fecha.Add(dia);
                }
            }


            return fecha;
        }

    }
}
