using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class ProcesadorCriterioFechaDeEntrega : ProcesadorCriterio<CriterioFechaDeEntrega>
    {
        public ProcesadorCriterioFechaDeEntrega(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
        public override decimal Calcular(CriterioFechaDeEntrega criterio)
        {
            int dias = (int)(criterio.Dto.FechaHasta.Date - DateTime.Now.Date).TotalDays;
            if (dias==0)
            {
                return 1;
            }
            decimal result = (decimal)1 / (decimal)dias;
            return result;
        }
    }
}
