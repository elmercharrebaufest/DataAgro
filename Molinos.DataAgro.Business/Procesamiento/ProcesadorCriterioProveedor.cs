using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class ProcesadorCriterioProveedor : ProcesadorCriterio<CriterioProveedor>
    {
        public ProcesadorCriterioProveedor(IRepositorio repositorio, ILogger log)
           : base(repositorio, log)
        {

        }
      
    }
}
