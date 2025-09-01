using Autofac.Extras.NLog;
using iTextSharp.text;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioScoreCupos : ProcesadorCriterio<CriterioScoreCupos>
    {
        public ProcesadorCriterioScoreCupos(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }

        public override decimal Calcular(CriterioScoreCupos criterio)
        {
            if (criterio.Dto.ProveedorId.HasValue)
            {
                List<ProveedorScoringDto> proveedores = Repositorio.Listar<Proveedor>()
                    .Select(x => new ProveedorScoringDto
                    {
                        ProveedorId = x.ProveedorId,
                        Score = x.Score
                    }).ToList();

                double scoreMax = proveedores.Max(x => x.Score).Value;
                double scoreMin = proveedores.Min(x => x.Score).Value;

                //Proveedor proveedor = Repositorio.Obtener<Proveedor>(x => x.ProveedorId == criterio.Dto.ProveedorId.Value);
                ProveedorScoringDto proveedor = proveedores.Single(x => x.ProveedorId == criterio.Dto.ProveedorId.Value);
                double score = proveedor?.Score ?? 0;

                var puntos = Normalizar(score, scoreMin, scoreMax) * 100; // escala 0-100

                return Math.Round((decimal)puntos, 2);
            }

            return 1;
        }

        private double Normalizar(double score, double min, double max)
        {
            if (max == min) return 0; // evita división por cero
            return (score - min) / (max - min);
        }
    }

    public class ProveedorScoringDto
    {
        public int ProveedorId { get; set; }
        public double? Score { get; set; }
    }
}
