using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioCDWarrant : ProcesadorCriterio<CriterioCDWarrant>
    {
        public ProcesadorCriterioCDWarrant(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioCDWarrant criterio)
        {
            return criterio.Dto.CDWarrant ? 1 : 0;
        }

    }
}
