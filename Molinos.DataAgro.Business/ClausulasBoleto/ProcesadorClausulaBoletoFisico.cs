using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.ClausulasBoleto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Globalization;
using System.Threading;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Business.ClausulasBoleto
{
    public abstract class ProcesadorClausulaBoletoFisico<TClausula> : IProcesadorClausulaBoletoFisico<TClausula> where TClausula : ClausulaBoletoFisico
    {
        protected IRepositorio Repositorio { get; private set; }
        protected ILogger Log { get; private set; }
        public IConsultarEstadoBoletoAgent EstadoBoleto { get; }

        protected ProcesadorClausulaBoletoFisico(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
        {
            Log = log;
            EstadoBoleto = estadoBoleto;
            Repositorio = repositorio;
        }

        public virtual ResultadoClausula DevolverClausulas(TClausula comando) { return new ResultadoClausula(); }


        public ResultadoClausula DevolverClausulas(ClausulaBoletoFisico comando)
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
                    Log.Error(e, String.Format("Ocurrió un error al ejecutar la ClausulaBoletoFisico - Intento número:" + count));
                    if (++count == maxTries)
                    {
                        throw;
                    }
                }
            }
        }

    }
}
