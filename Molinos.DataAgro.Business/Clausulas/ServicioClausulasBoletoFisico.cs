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
    public class ServicioClausulasBoletoFisico : IServicioClausulasBoletoFisico
    {
        private readonly ILogger log;
        private IDictionary<Type, Type> procesadores;

        public ServicioClausulasBoletoFisico(ILogger log)
        {
            this.log = log;
            RegistrarProcesadores();
        }

        public ResultadoClausula DevolverClausulas(ClausulaBoletoFisico clausula)
        {
            var tipoProcesador = procesadores[clausula.GetType()];
            var procesador = (IProcesadorClausulaBoletoFisico)System.Web.Mvc.DependencyResolver.Current.GetService(tipoProcesador);
            return procesador.DevolverClausulas(clausula);
        }

        private void RegistrarProcesadores()
        {
            //log.Info("Registrando procesadores de comandos...");
            procesadores = new Dictionary<Type, Type>();

            var clausulas = ClausulaBoletoFisico.TiposDeComandos().Where(t => !t.IsAbstract);

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
                return typeof(IProcesadorClausulaBoletoFisico<>)
                    .Assembly
                    .GetExportedTypes()
                    .Single(
                        x => !x.IsAbstract && x.GetInterfaces().Any(i => i.IsGenericType
                                                        && i.GetGenericTypeDefinition() == typeof(IProcesadorClausulaBoletoFisico<>)
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
