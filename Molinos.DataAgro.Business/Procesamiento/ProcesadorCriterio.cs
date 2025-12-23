using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Threading;

namespace Molinos.DataAgro.Business
{
    public abstract class ProcesadorCriterio<TCriterio> : IProcesadorCriterio<TCriterio> where TCriterio : Criterio
    {
        protected IRepositorio Repositorio { get; private set; }
        protected ILogger Log { get; private set; }

        protected ProcesadorCriterio(IRepositorio repositorio, ILogger log)
        {
            Log = log;
            Repositorio = repositorio;
        }

        public virtual decimal Calcular(TCriterio comando) { return 0; }


        public decimal Calcular(Criterio comando)
        {
            var count = 1;
            const int maxTries = 3;
            while (true)
            {
                try
                {
                    return Calcular((TCriterio)comando);
                }
                catch (Exception e)
                {
                    Thread.Sleep(count * 1500);
                    Log.Error(e, String.Format("Ocurrió un error el ejecutar el criterio - Intento numero:" + count));
                    if (++count == maxTries)
                    {
                        throw;
                    }
                }
            }
        }
    }
}