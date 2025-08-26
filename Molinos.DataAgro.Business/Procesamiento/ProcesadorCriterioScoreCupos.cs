using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;

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
                Proveedor proveedor = Repositorio.Obtener<Proveedor>(x => x.ProveedorId == criterio.Dto.ProveedorId.Value);
                double score = proveedor?.Score ?? 0;
                return Math.Round((decimal)score, 2);
            }

            return 1;
        }
    }
}
