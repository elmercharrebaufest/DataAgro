using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DiasHabilesAgent : IDiasHabilesAgent
    {
        private readonly IRepositorio repositorio;
        public DiasHabilesAgent(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }
        public List<DateTime> ObtenerDiasHabiles()
        {
            var fecha = new List<DateTime>();
            var anterior = DateTime.Today.AddMonths(-1);
            for (int i = 1; i <= DateTime.DaysInMonth(anterior.Year, anterior.Month); i++)
            {
                DateTime.TryParse(i.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString(), out DateTime dia);
                if (dia.DayOfWeek != DayOfWeek.Saturday && dia.DayOfWeek != DayOfWeek.Sunday && !repositorio.Listar<FechaFeriado>().Select(x=>x.Feriado).Contains(dia))
                {
                    fecha.Add(dia);
                }
            }
            for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                DateTime.TryParse(i.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString(), out DateTime dia);
                if (dia.DayOfWeek != DayOfWeek.Saturday && dia.DayOfWeek != DayOfWeek.Sunday && !repositorio.Listar<FechaFeriado>().Select(x => x.Feriado).Contains(dia))
                {
                    fecha.Add(dia);
                }
            }
            return fecha;
        }

        public DateTime UltimoDiaHabil(DateTime? fecha)
        {

            var diaAnterior = fecha != null ? fecha.Value.Date : DateTime.Now.Date;
            var diasHabiles = ObtenerDiasHabiles();
            for (var i = 1; i < diasHabiles.Count; i++)
            {
                if (diasHabiles.Contains(diaAnterior.Date.AddDays(-i)))
                {
                    diaAnterior = diaAnterior.AddDays(-i);
                    break;
                }
            }
            return diaAnterior;
        }
    }
}
