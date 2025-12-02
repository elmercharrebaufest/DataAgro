using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsFason : ProcesadorCriterio<CriterioEsFason>
    {
        public ProcesadorCriterioEsFason(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsFason criterio)
        {
            return criterio.Dto.TipoNegocioId == 4 ? 1 : 0;
        }

    }
}
