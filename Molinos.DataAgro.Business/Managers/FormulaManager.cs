using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Repository.ConsultasEF;
using System.ComponentModel;

namespace Molinos.DataAgro.Business.Managers
{



    public class FormulaManager : IFormulaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;


        public FormulaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public ResultIniCriterio TraerCriteriosGuardados()
        {
            var criterioss = repositorio.Listar<Criterio>();
            if (repositorio.Listar<Criterio>().Count == 0)
            {
                repositorio.Agregar<Criterio>(new CriterioRaiz { Prioridad = 100 });
            }
            var criteriosRaiz = repositorio.Listar<Criterio>().Where(x => x.DisplayName == "Criterios").OrderBy(c => c.Id).Last();
            var todosLosCriterios = repositorio.Listar<Criterio>().Where(x => x.Id >= criteriosRaiz.Id);



            ResultIniCriterio criteriosTodosDto = new ResultIniCriterio
            {
                Criterios = todosLosCriterios.Select(p => new CriterioIni
                {
                    Id = p.Id,
                    Descripcion = p.Descripcion,
                    PadreId = p.PadreId,
                    Prioridad = p.Prioridad,
                    Concreta = p.Concreta,
                    DisplayName = p.DisplayName

                }).ToList()
            };

            return criteriosTodosDto;

        }
        public Resultado GrabarCriterio(CriterioIni criterio)

        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(criterio, oEntityErrors);
            var criterioss = repositorio.Listar<Criterio>();


            if (criterio.Id != 0)
            {

                Criterio criterioPadre = criterioss.Where(c => c.Id == criterio.PadreId).First();
                Criterio criterioViejo = criterioss.Where(c => c.Id == criterio.Id).First();

                int sumaPrioridadDisponible = 100 - criterioss.Where(x => x.PadreId == criterioPadre.Id).Sum(c => c.Prioridad) + criterioViejo.Prioridad;

                //int sumaPrioridadDisponible = 100 - criterioPadre.Hijos.Sum(x => x.Prioridad) + criterioViejo.Prioridad;
                int prioridadNueva = criterio.Prioridad;


                if (sumaPrioridadDisponible < prioridadNueva)
                {
                    if (sumaPrioridadDisponible == 0)
                    {
                        oEntityErrors.Error("", "No se pueden Agregar mas Criterios");
                    }
                    else
                    {
                        oEntityErrors.Error("", "La Prioridad Debe ser menor a : " + sumaPrioridadDisponible.ToString());
                    }

                }
            }
            if (criterio.Prioridad == 0)
            {
                oEntityErrors.Error("", "La Prioridad debe ser mayor a 0");
            }
            if (criterio.Descripcion == "" || criterio.Descripcion == null)
            {
                oEntityErrors.Error("", "Debe seleccionar un Criterio");
            }
            if (formula == null)
            {
                oEntityErrors.Error("", "Error Cargando la Formula");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }





            Criterio criterioAsd = (Criterio)GetInstance("Molinos.DataAgro.Entities.Entities." + criterio.Descripcion);

            criterioAsd.PadreId = criterio.PadreId;
            criterioAsd.Prioridad = criterio.Prioridad;
            criterioAsd.Id = criterio.Id;

            formula.Fecha = DateTime.Now;
            formula.Usada = null;
            //formula.Inicio = formulaDias.Inicio;
            //formula.CantDias = formulaDias.CantDias;


            if (criterioAsd.Id == 0)
            {
                agregarCriterio(formula.Criterio, criterioAsd);
            }
            else
            {
                actualizarCriterio(formula.Criterio, criterioAsd);
            }

            try
            {
                repositorio.Agregar(formula);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }
        public Resultado eliminarCriterio(CriterioIni criterio)
        {
            var oEntityErrors = new Resultado();
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            Criterio criterioAEliminar = repositorio.Obtener<Criterio>(criterio.Id);

            if (formula == null)
            {
                oEntityErrors.Error("", "Error Cargando la Formula");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }


            var criterioAsd = (Criterio)GetInstance("Molinos.DataAgro.Entities.Entities." + criterio.Descripcion);

            criterioAsd.PadreId = criterio.PadreId;
            criterioAsd.Prioridad = criterio.Prioridad;
            criterioAsd.Id = criterio.Id;
            //criterioAsd.Hijos = criterioAEliminar.Hijos;
            formula.Fecha = DateTime.Now;
            formula.Usada = null;
            //formula.Inicio = formulaDias.Inicio;
            //formula.CantDias = formulaDias.CantDias;

            eliminarCriterio(formula.Criterio, criterioAsd);

            try
            {
                repositorio.Agregar(formula);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.Debug("Guardando el criterio:" + criterio.Descripcion);


            return oEntityErrors;
        }
        public ResultIniFormula ultimaFormula()
        {

            if (repositorio.Listar<Formula>().Count() == 0)
            {
                repositorio.Agregar<Formula>(new Formula { Criterio = new CriterioRaiz { Prioridad = 100 }, Fecha = DateTime.Now, CentroId = 1 });
                repositorio.GuardarCambios();

            }

            Formula formulaDeBase = repositorio.Listar<Formula>().OrderBy(f => f.Id).ToList().Last();

            return new ResultIniFormula
            {
                Formula = new FormulaIni
                {
                    Inicio = formulaDeBase.Inicio,
                    CantDias = formulaDeBase.CantDias
                }
            };
        }
        public Resultado actualizarDias(FormulaIni formulaDias)
        {
            var oEntityErrors = new Resultado();


            if (formulaDias.Inicio < 0)
            {
                oEntityErrors.Error("", "El dia de inicio debe ser mayor o igual a 0");
            }
            if (formulaDias.CantDias < 0)
            {
                oEntityErrors.Error("", "La cantidad de Dias a calcular debe ser mayor o igual a 0");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }


            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());

            formula.Inicio = formulaDias.Inicio;
            formula.CantDias = formulaDias.CantDias;
            formula.Fecha = DateTime.Now;
            formula.Usada = null;
            repositorio.Agregar(formula);
            repositorio.GuardarCambios();

            return oEntityErrors;
        }
        public List<CriterioIni> todosLosCriterios()
        {
            var results = Criterio.TiposDeComandos();

            List<CriterioIni> arrayDeCriterios = new List<CriterioIni>();

            foreach (var item in results)
            {
                if (item.Name != "Criterio" && item.Name != "CriterioRaiz")
                {
                    CriterioIni criterioNuevo = new CriterioIni();

                    object[] myAttributes = item.GetCustomAttributes(true);

                    DisplayNameAttribute atributoDisplay = (DisplayNameAttribute)myAttributes[1];
                    string nombreEnDisplay = atributoDisplay.DisplayName.ToString();


                    criterioNuevo.Concreta = ((Molinos.DataAgro.Entities.CustomAtributte.Concreta)myAttributes[0]).result;
                    criterioNuevo.DisplayName = nombreEnDisplay;
                    criterioNuevo.Descripcion = item.Name;

                    arrayDeCriterios.Add(criterioNuevo);
                }
            }

            return arrayDeCriterios;
        }

        //----------------------------------
        private void agregarCriterio(Criterio criterio, Criterio agregar)
        {
            if (criterio.Id == agregar.PadreId)
            {
                criterio.Hijos.Add(agregar);
                return;
            }
            foreach (var hijo in criterio.Hijos)
            {
                if (hijo.Id == agregar.PadreId)
                {
                    hijo.Hijos.Add(agregar);
                    break;
                }
                else
                {
                    agregarCriterio(hijo, agregar);
                }
            }
        }
        private void eliminarCriterio(Criterio criterio, Criterio eliminar)
        {
            foreach (var hijo in criterio.Hijos)
            {
                if (hijo.Id == eliminar.Id)
                {
                    criterio.Hijos.Remove(hijo);
                    break;
                }
                else
                {
                    eliminarCriterio(hijo, eliminar);
                }
            }
        }
        private void actualizarCriterio(Criterio criterio, Criterio actualizar)
        {
            foreach (var hijo in criterio.Hijos)
            {
                if (hijo.Id == actualizar.Id)
                {
                    hijo.Prioridad = actualizar.Prioridad;
                    break;
                }
                else
                {
                    actualizarCriterio(hijo, actualizar);
                }
            }
        }
        private object GetInstance(string strFullyQualifiedName)
        {
            Type type = Type.GetType(strFullyQualifiedName);
            if (type != null)
                return Activator.CreateInstance(type);
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = asm.GetType(strFullyQualifiedName);
                if (type != null)
                    return Activator.CreateInstance(type);
            }
            return null;
        }
        private ICollection<CriterioIni> cargarHijos(Criterio criterio)
        {

            ICollection<CriterioIni> HijosCargados = new List<CriterioIni>();


            if (criterio.Hijos != null && criterio.Hijos.Count > 0)
            {
                foreach (var hijo in criterio.Hijos)
                {

                    if (hijo.PadreId == 8)
                    {

                        HijosCargados.Add(
                        new CriterioIni
                        {
                            Id = hijo.Id,
                            Descripcion = hijo.Descripcion,

                            Prioridad = hijo.Prioridad,
                            Concreta = hijo.Concreta,
                            Hijos = cargarHijos(hijo)
                        });

                    }
                    else
                    {
                        HijosCargados.Add(
                            new CriterioIni
                            {
                                Id = hijo.Id,
                                Descripcion = hijo.Descripcion,
                                PadreId = hijo.PadreId,
                                Prioridad = hijo.Prioridad,
                                Concreta = hijo.Concreta,
                                Hijos = cargarHijos(hijo)
                            });
                    }


                }
            }

            return HijosCargados;
        }
        //private int totalrioridadRestante()
        //{
        //    var criteriosRaiz = repositorio.Listar<Criterio>().Where(x => x.Descripcion == "CriterioRaiz").OrderBy(c => c.Id).Last();
        //    var todosLosCriterios = repositorio.Listar<Criterio>().Where(x => x.Id >= criteriosRaiz.Id && x.Descripcion != "CriterioRaiz").ToList();

        //    if (todosLosCriterios.Count() == 0)
        //    {
        //        return 100;
        //    }

        //    var sumaPrioridades = todosLosCriterios.Sum(x => x.Prioridad);

        //    if (100 - sumaPrioridades < 0)
        //    {
        //        return 0;
        //    }

        //    return 100 - sumaPrioridades;

        //}



    }
}
