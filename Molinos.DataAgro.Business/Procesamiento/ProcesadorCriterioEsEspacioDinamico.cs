using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsEspacioDinamico : ProcesadorCriterio<CriterioEsEspacioDinamico>
    {

        public ProcesadorCriterioEsEspacioDinamico(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsEspacioDinamico criterio)
        {
            TipoNegocio tipoNegocioEspacioDinamico = Repositorio.ObtenerPrimero<TipoNegocio>(a => a.Descripcion == "ESPACIO DINAMICO");
            return criterio.Dto.TipoNegocioId == tipoNegocioEspacioDinamico.TipoNegocioId ? 1 : 0;
        }

    }
}
