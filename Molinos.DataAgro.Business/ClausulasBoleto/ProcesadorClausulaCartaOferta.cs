using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using Autofac.Extras.NLog;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Business.ClausulasBoleto
{
    public abstract class ProcesadorClausulaCartaOferta<TClausula> : IProcesadorClausulaCartaOferta<TClausula> where TClausula : ClausulaCartaOferta
    {
        protected IRepositorio Repositorio { get; private set; }
        protected ILogger Log { get; private set; }
        public IConsultarEstadoBoletoAgent EstadoBoleto { get; }

        protected ProcesadorClausulaCartaOferta(IRepositorio repositorio, ILogger log, IConsultarEstadoBoletoAgent estadoBoleto)
        {
            Log = log;
            EstadoBoleto = estadoBoleto;
            Repositorio = repositorio;
        }

        public virtual ResultadoClausula DevolverClausulas(TClausula comando) { return new ResultadoClausula(); }


        public ResultadoClausula DevolverClausulas(ClausulaCartaOferta comando)
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
                    Log.Error(String.Format("Ocurrió un error al ejecutar la ClausulaCartaOferta - Intento número:" + count), e);
                    if (++count == maxTries)
                    {
                        throw;
                    }
                }
            }
        }

    }
}
