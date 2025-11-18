using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioConDescarga : ProcesadorCriterio<CriterioConDescarga>
    {
        public ProcesadorCriterioConDescarga(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioConDescarga criterio)
        {
            return criterio.Dto.ConDescarga ? 1 : 0;
        }
    }
}