using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsFijacionDePrecioContrato : ProcesadorCriterio<CriterioEsFijacionDePrecioContrato>
    {
        public ProcesadorCriterioEsFijacionDePrecioContrato(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsFijacionDePrecioContrato criterio)
        {
            return criterio.Dto.TipoNegocioId == 3 ? 1 : 0;
        }

    }
}
