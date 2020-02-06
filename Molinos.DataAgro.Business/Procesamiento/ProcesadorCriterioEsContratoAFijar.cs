using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsContratoAFijar : ProcesadorCriterio<CriterioEsContratoAFijar>
    {
        public ProcesadorCriterioEsContratoAFijar(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsContratoAFijar criterio)
        {
            return criterio.Dto.TipoNegocioId == 1 ? 1 : 0;
        }

    }
}
