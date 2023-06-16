using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsAgenteCompra : ProcesadorCriterio<CriterioEsAgenteCompra>
    {
        public ProcesadorCriterioEsAgenteCompra(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsAgenteCompra criterio)
        {
            return criterio.Dto.TipoAgenteCompraId == null ? 0 : 1;
        }
    }
}