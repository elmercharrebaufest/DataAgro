using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class FacacopManager : IFacacopManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public FacacopManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public bool InsetarFacacop(List<FACACOP> oDatos)
        {
            try
            {
                repositorio.RemoverTodos<FACACOP>(x => true);
                repositorio.AgregarTodos(oDatos);
                return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return false;
        }
    }
}
