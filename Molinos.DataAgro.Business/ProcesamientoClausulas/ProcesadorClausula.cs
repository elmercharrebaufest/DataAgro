using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;
using System.Threading;

namespace Molinos.DataAgro.Business
{
    public abstract class ProcesadorClausula<TClausula> : IProcesadorClausula<TClausula> where TClausula : Clausula
    {
        protected IRepositorio Repositorio { get; private set; }
        protected ILogger Log { get; private set; }
        public IConsultarEstadoBoletoAgent EstadoBoleto { get; }

        protected ProcesadorClausula(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
        {
            Log = log;
            EstadoBoleto = estadoBoleto;
            Repositorio = repositorio;
        }

        public virtual ResultadoClausula DevolverClausulas(TClausula comando) { return new ResultadoClausula(); }


        public ResultadoClausula DevolverClausulas(Clausula comando)
        {
            var count = 1;
            const int maxTries = 3;
            Humanizer.Configuration.Configurator.Ordinalizers.ResolveForCulture(CultureInfo.GetCultureInfo("es-AR"));
            while (true)
            {
                try
                {
                    return DevolverClausulas((TClausula)comando);
                }
                catch (Exception e)
                {
                    Thread.Sleep(count * 1500);
                    Log.Error(String.Format("Ocurrió un error al ejecutar la Clausula - Intento número:" + count), e);
                    if (++count == maxTries)
                    {
                        throw;
                    }
                }
            }
        }

    }
}
