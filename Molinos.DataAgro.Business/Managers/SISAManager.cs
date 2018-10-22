using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class SISAManager : ISISAManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComercialManager oComercial;

        public SISAManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComercial = oComercial;
        }

        public bool InsertarSISA(List<SISA> oDatos)
        {
            try
            {
                repositorio.RemoverTodos<SISA>(x => true);
                repositorio.AgregarTodos<SISA>(oDatos);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return false;
        }
    }
}