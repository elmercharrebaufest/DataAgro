using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DiasHabilesAgent : IDiasHabilesAgent
    {
        private readonly IRepositorio repositorio;
        private readonly HashSet<DateTime> feriados;

        public DiasHabilesAgent(IRepositorio repositorio)
        {
            this.repositorio = repositorio;
            feriados = new HashSet<DateTime>(repositorio.Listar<FechaFeriado>().Select(x => x.Feriado));
        }

        public List<DateTime> ObtenerDiasHabiles()
        {
            var fecha = new List<DateTime>();
            var anterior = DateTime.Today.AddMonths(-12);
            
            DateTime diaAnterior = anterior;
            while (diaAnterior <= DateTime.Now.Date)
            {
                if (diaAnterior.DayOfWeek != DayOfWeek.Saturday && diaAnterior.DayOfWeek != DayOfWeek.Sunday && !feriados.Contains(diaAnterior))
                {
                    fecha.Add(diaAnterior);
                }
                diaAnterior = diaAnterior.AddDays(1);
            }
            for (int i = 1; i <= DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month); i++)
            {
                DateTime.TryParse(i.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString(), out DateTime dia);
                if (dia.DayOfWeek != DayOfWeek.Saturday && dia.DayOfWeek != DayOfWeek.Sunday && !feriados.Contains(dia))
                {
                    fecha.Add(dia);
                }
            }
            return fecha;
        }

        public List<DateTime> ObtenerDiasHabilesDelMes(DateTime? fechaActual = null)
        {
            var fechas = new List<DateTime>();
            var fecha = fechaActual == null ? DateTime.Now : fechaActual.Value;
            for (int i = 1; i <= DateTime.DaysInMonth(fecha.Year, fecha.Month); i++)
            {
                DateTime.TryParse(i.ToString() + "/" + fecha.Month.ToString() + "/" + fecha.Year.ToString(), out DateTime dia);
                if (dia.DayOfWeek != DayOfWeek.Saturday && dia.DayOfWeek != DayOfWeek.Sunday && !repositorio.Listar<FechaFeriado>().Select(x => x.Feriado).Contains(dia))
                {
                    fechas.Add(dia);
                }
            }
            return fechas;
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

        public bool EsDiaHabil(DateTime fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                return false;
            else
                return !feriados.Contains(fecha);
        }
    }
}
