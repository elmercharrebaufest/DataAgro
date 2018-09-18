using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class RG2300Manager : IRG2300Manager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComercialManager oComercial;

        public RG2300Manager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComercial = oComercial;
        }

        public bool InsetarRG2300(List<RG2300> oDatos)
        {
            try
            {
                repositorio.RemoverTodos<RG2300>(x => true);
                repositorio.AgregarTodos<RG2300>(oDatos);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }
    }
}
