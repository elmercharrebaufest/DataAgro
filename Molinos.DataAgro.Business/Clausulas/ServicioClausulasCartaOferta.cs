using Autofac.Extras.NLog;
using Molinos.DataAgro.Business.ClausulasBoleto;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.ClausulasBoleto;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Clausulas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Clausulas
{
    public class ServicioClausulasCartaOferta : IServicioClausulasCartaOferta
    {
        private readonly ILogger log;
        private IDictionary<Type, Type> procesadores;

        public ServicioClausulasCartaOferta(ILogger log)
        {
            this.log = log;
            RegistrarProcesadores();
        }

        public ResultadoClausula DevolverClausulas(ClausulaCartaOferta clausula)
        {
            var tipoProcesador = procesadores[clausula.GetType()];
            var procesador = (IProcesadorClausulaCartaOferta)System.Web.Mvc.DependencyResolver.Current.GetService(tipoProcesador);
            return procesador.DevolverClausulas(clausula);
        }

        private void RegistrarProcesadores()
        {
            //log.Info("Registrando procesadores de comandos...");
            procesadores = new Dictionary<Type, Type>();

            var clausulas = ClausulaCartaOferta.TiposDeComandos().Where(t => !t.IsAbstract);

            foreach (var clausula in clausulas)
            {
                var procesador = ObtenerProcesador(clausula);
                procesadores.Add(clausula, procesador);
            }
        }

        private Type ObtenerProcesador(Type clausula)
        {
            try
            {
                return typeof(IProcesadorClausulaCartaOferta<>)
                    .Assembly
                    .GetExportedTypes()
                    .Single(
                        x => !x.IsAbstract && x.GetInterfaces().Any(i => i.IsGenericType
                                                        && i.GetGenericTypeDefinition() == typeof(IProcesadorClausulaCartaOferta<>)
                                                        && i.GetGenericArguments().Single() == clausula));
            }
            catch (InvalidOperationException e)
            {
                log.Error(String.Format("No existe procesador para el clausula {0}", clausula.Name), e);
                throw;
            }
        }
    }
}
