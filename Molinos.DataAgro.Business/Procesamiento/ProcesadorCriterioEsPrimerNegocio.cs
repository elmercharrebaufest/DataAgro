using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Business.Procesamiento
{
    public class ProcesadorCriterioEsPrimerNegocio : ProcesadorCriterio<CriterioEsPrimerNegocio>
    {
        public ProcesadorCriterioEsPrimerNegocio(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioEsPrimerNegocio criterio)
        {
            if (!criterio.Dto.ProveedorId.HasValue)
            {
                return 0;
            }
            bool existeCompra = Repositorio.Existe<CampanaMaterialDetallePorMes>(a => a.ProveedorId == criterio.Dto.ProveedorId);
            return existeCompra ? 0 : 1;
        }

    }
}
