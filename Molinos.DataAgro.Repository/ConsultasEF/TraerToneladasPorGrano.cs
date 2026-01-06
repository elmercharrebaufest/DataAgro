using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Linq;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerToneladasPorGrano : IConsultaEscalar<ToneladasGranoTipoDto>
    {
        private readonly int materialId;
        private readonly DateTime fechaDesde;
        private readonly DateTime fechaHasta;
        private readonly int? calidad;
        private readonly int centroId;

        public TraerToneladasPorGrano(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad = null, int centroId = 0)
        {
            this.materialId = materialId;
            this.fechaDesde = fechaDesde;
            this.fechaHasta = fechaHasta;
            this.calidad = calidad;
            this.centroId = centroId;
        }

        public ToneladasGranoTipoDto Ejecutar(DbContext contexto)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            // Ajustamos las fechas para el rango del día completo
            var fechaInicio = fechaDesde.Date;
            var fechaFin = fechaHasta.Date.AddDays(1).AddTicks(-1);

            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(+1).Month, 1);
            var toneladasPorGrano = new ToneladasGranoTipoDto
            {
                ListNewAgente = new List<int>(),
                ListDispAgente = new List<int>(),
                ListFrwAgente = new List<int>()
            };

            var agente = contexto.Set<AgenteCompra>().Where(x => x.OcultarEnTablero == false
                && x.FechaOperacion >= fechaInicio
                && x.FechaOperacion <= fechaFin
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
                && x.MaterialId == materialId && (centroId == 0 || centroId == 1)
                && (calidad == null || calidad == 3))
                .Select(x => new NegocioToneladasPosicionDto
                {
                    Id = x.Id,
                    Fecha = x.FechaOperacion,
                    TipoNegocioId = 5,
                    Cantidad = x.Cantidad,
                    PosicionString = x.Posicion,
                    CampanaId = x.CampanaId ?? 0,
                    MaterialCampanaId = x.Material.CampaniaTableroId ?? x.Material.CampañaId ?? 0
                }).ToList();

            foreach (var age in agente)
            {
                var pos = age.PosicionString.Split('.');
                var fecha = new DateTime(int.Parse(pos[1]), int.Parse(pos[0]), 1);
                var fechaAñoSiguiente = new DateTime(DateTime.Now.AddYears(1).Year, materialId == 3 ? 4 : materialId == 1 ? 3 : 11, 1);
                var fechaMesSiguiente = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(1).Month, 1);

                if (age.CampanaId > age.MaterialCampanaId)
                {
                    toneladasPorGrano.NewAgente += Math.Round(age.Cantidad / 1000);
                    toneladasPorGrano.ListNewAgente.Add(age.Id);
                }
                else if (age.CampanaId <= age.MaterialCampanaId && (fechaMesSiguiente >= fecha))
                {
                    toneladasPorGrano.DispAgente += Math.Round(age.Cantidad / 1000);
                    toneladasPorGrano.ListDispAgente.Add(age.Id);
                }
                else
                {
                    toneladasPorGrano.FrwAgente += Math.Round(age.Cantidad / 1000);
                    toneladasPorGrano.ListFrwAgente.Add(age.Id);
                }
            }

            toneladasPorGrano.Total += toneladasPorGrano.NewAgente + toneladasPorGrano.DispAgente + toneladasPorGrano.FrwAgente;

            return toneladasPorGrano;
        }
    }
}
