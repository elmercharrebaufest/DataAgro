using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsSojaSustentable : ProcesadorCriterio<CriterioEsSojaSustentable>
    {
        public ProcesadorCriterioEsSojaSustentable(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsSojaSustentable criterio)
        {
            return criterio.Dto.Sustentable ? 1 : 0;
        }

    }
}
