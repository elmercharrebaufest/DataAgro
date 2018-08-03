using Autofac.Extras.NLog;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business.Managers
{
    public class RG2300Manager : IRG2300Manager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public RG2300Manager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

    }

}
