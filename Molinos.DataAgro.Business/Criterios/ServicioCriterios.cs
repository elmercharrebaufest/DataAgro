using Autofac;
using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Criterios;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Criterios
{
    public class ServicioCriterios : IServicioCriterios
    {
        private readonly ILogger log;
        private IDictionary<Type, Type> procesadores;
        private readonly ILifetimeScope _lifetimeScope;

        public ServicioCriterios(ILogger log, ILifetimeScope lifetimeScope)
        {
            this.log = log;
            this._lifetimeScope = lifetimeScope;
            RegistrarProcesadores();
        }

        public decimal Calcular(Criterio criterio)
        {
            if (criterio.Hijos != null && criterio.Hijos.Count > 0)
            {
                decimal resultado = 0;
                foreach (var hijo in criterio.Hijos)
                {
                    hijo.Dto = criterio.Dto;
                    var result = Calcular(hijo);
                    var resultCalcular = result * hijo.Prioridad;
                    hijo.Puntuacion = resultCalcular;
                    resultado += resultCalcular;
                }


                if (criterio.GetType().BaseType.ToString() != typeof(CriterioRaiz).ToString())
                {
                    criterio.Puntuacion = resultado / 100;
                }
                else
                {
                    criterio.Puntuacion = resultado;
                }
                return criterio.Puntuacion;
            }
            else
            {
                var tipoProcesador = procesadores[criterio.GetType().BaseType];
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    var procesador = (IProcesadorCriterio)scope.Resolve(tipoProcesador);
                    return procesador.Calcular(criterio);
                }
            }

        }

        private void RegistrarProcesadores()
        {
            //log.Info("Registrando procesadores de comandos...");
            procesadores = new Dictionary<Type, Type>();

            var criterios = Criterio.TiposDeComandos().Where(t => !t.IsAbstract);

            foreach (var criterio in criterios)
            {
                var procesador = ObtenerProcesador(criterio);
                procesadores.Add(criterio, procesador);
                //log.Debug("Criterio: {0} Procesador: {1}", criterio.Name, procesador.Name);
            }
        }

        private Type ObtenerProcesador(Type criterio)
        {
            try
            {
                return typeof(IProcesadorCriterio<>)
                    .Assembly
                    .GetExportedTypes()
                    .Single(
                        x => !x.IsAbstract && x.GetInterfaces().Any(i => i.IsGenericType
                                                        && i.GetGenericTypeDefinition() == typeof(IProcesadorCriterio<>)
                                                        && i.GetGenericArguments().Single() == criterio));
            }
            catch (InvalidOperationException e)
            {
                log.Error(String.Format("No existe procedador para el criterio {0}", criterio.Name), e);
                throw;
            }
        }

    }
}
