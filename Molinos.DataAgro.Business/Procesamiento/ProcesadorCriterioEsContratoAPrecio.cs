using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsContratoAPrecio : ProcesadorCriterio<CriterioEsContratoAPrecio>
    {
        public ProcesadorCriterioEsContratoAPrecio(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsContratoAPrecio criterio)
        {
            return criterio.Dto.TipoNegocioId == 2 ? 1 : 0;
        }

    }
}
