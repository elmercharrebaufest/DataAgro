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
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business.Managers
{



    public class FormulaManager : IFormulaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;



        public FormulaManager(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
        }
        public ResultIniCriterio TraerCriteriosGuardados(int MaterialId)
        {
            if (repositorio.Listar<Formula>(x => x.MaterialId == MaterialId).Count() == 0)
            {
                var hoy = DateTime.Now.Date;
                var material = repositorio.Obtener<Material>(MaterialId);
                repositorio.Agregar<Formula>(new Formula { Material = material, Criterio = new CriterioRaiz { Prioridad = 100 }, Fecha = DateTime.Now, CentroId = 1, CuposDesde = hoy, CuposHasta = hoy, MaterialId = MaterialId, NegociosDesde = hoy, NegociosHasta = hoy });
                repositorio.GuardarCambios();
            }
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(MaterialId));

            //var todosLosCriterios = repositorio.Listar<Criterio>(x => x.Id == formulaDeBase.CriterioId || x.PadreId == formulaDeBase.CriterioId);
            ResultIniCriterio criteriosTodosDto = new ResultIniCriterio();
            criteriosTodosDto.Criterios = new List<CriterioIni>();
            ObtenerTodosLosCriterios(formula.Criterio, criteriosTodosDto.Criterios);

            return criteriosTodosDto;

        }

        private void ObtenerTodosLosCriterios(Criterio criterio, ICollection<CriterioIni> lista)
        {
            lista.Add(new CriterioIni
            {
                Id = criterio.Id,
                Descripcion = criterio.Descripcion,
                PadreId = criterio.PadreId,
                Prioridad = criterio.Prioridad,
                Concreta = criterio.Concreta,
                DisplayName = criterio.DisplayName
            });
            if (criterio.Hijos.Count > 0)
            {
                foreach (var hijo in criterio.Hijos)
                {
                    ObtenerTodosLosCriterios(hijo, lista);
                }
            }
        }

        public Resultado GrabarCriterio(CriterioIni criterio)
        {
            Criterio criterioRaiz = ObtenerCriterioRaiz(criterio.PadreId.Value);
            var MaterialId = repositorio.Obtener<Formula, int>(a => a.CriterioId == criterioRaiz.Id, a => a.MaterialId);
            var formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(MaterialId));
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
                AgregarCriterio(formula.Criterio, criterioAsd);
            }
            else
            {
                ActualizarCriterio(formula.Criterio, criterioAsd);
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

        private Criterio ObtenerCriterioRaiz(int CriterioId)
        {
            var criterio = repositorio.Obtener<Criterio>(CriterioId);
            if (criterio.PadreId == null)
            {
                return criterio;
            }
            else
            {
                return ObtenerCriterioRaiz(criterio.PadreId.Value);
            }
        }

        public Resultado EliminarCriterio(CriterioIni criterio)
        {
            var oEntityErrors = new Resultado();
            Criterio criterioRaiz = ObtenerCriterioRaiz(criterio.Id);
            var MaterialId = repositorio.Obtener<Formula, int>(a => a.CriterioId == criterioRaiz.Id, a => a.MaterialId);
            var formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(MaterialId));

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
            formula.Fecha = DateTime.Now;
            formula.Usada = null;

            EliminarCriterio(formula.Criterio, criterioAsd);

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
        public ResultIniFormula UltimaFormula(int MaterialId)
        {

            if (repositorio.Listar<Formula>(x => x.MaterialId == MaterialId).Count() == 0)
            {
                var hoy = DateTime.Now.Date;
                var material = repositorio.Obtener<Material>(MaterialId);
                repositorio.Agregar<Formula>(new Formula { Material = material, Criterio = new CriterioRaiz { Prioridad = 100 }, Fecha = DateTime.Now, CentroId = 1, CuposDesde = hoy, CuposHasta = hoy, MaterialId = MaterialId, NegociosDesde = hoy, NegociosHasta = hoy });
                repositorio.GuardarCambios();

            }

            Formula formulaDeBase = repositorio.Listar<Formula>(x => x.MaterialId == MaterialId).OrderBy(f => f.Id).ToList().Last();

            return new ResultIniFormula
            {
                Formula = new FormulaIni
                {
                    CuposDesde = formulaDeBase.CuposDesde,
                    CuposHasta = formulaDeBase.CuposHasta,
                    NegociosDesde = formulaDeBase.NegociosDesde,
                    NegociosHasta = formulaDeBase.NegociosHasta,
                    MaterialId = formulaDeBase.MaterialId,
                    Material = formulaDeBase.Material == null ? "" : formulaDeBase.Material.Descripcion,
                    Cierre = cupoManager.DevolverTodoCierreCupera().FirstOrDefault() != null ? cupoManager.DevolverTodoCierreCupera().FirstOrDefault().Cierre : false,
                }
            };
        }
        public Resultado ActualizarDias(FormulaIni formulaDias)
        {
            var oEntityErrors = new Resultado();

            if (formulaDias.CuposDesde < DateTime.Now.Date)
            {
                oEntityErrors.Error("", "La fecha Cupo Desde no puede ser menor a la fecha Actual.");
            }

            if (formulaDias.CuposHasta < formulaDias.CuposDesde)
            {
                oEntityErrors.Error("", "La fecha Cupo Hasta no puede ser menor a la fecha Desde.");
            }
            if (formulaDias.NegociosHasta < formulaDias.NegociosDesde)
            {
                oEntityErrors.Error("", "La fecha Negocio Hasta no puede ser menor a la fecha Desde.");
            }
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }

            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(formulaDias.MaterialId));

            formula.CuposDesde = formulaDias.CuposDesde;
            formula.CuposHasta = formulaDias.CuposHasta;
            formula.NegociosDesde = formulaDias.NegociosDesde;
            formula.NegociosHasta = formulaDias.NegociosHasta;
            formula.Fecha = DateTime.Now;
            formula.Usada = null;

            GrabarCierreCuperaAlgoritmo(formulaDias);

            repositorio.Agregar(formula);

            repositorio.GuardarCambios();

            return oEntityErrors;
        }
        public Resultado ActualizarCierre(FormulaIni formulaDias)
        {
            var oEntityErrors = new Resultado();

            GrabarCierreCuperaAlgoritmo(formulaDias);

            repositorio.GuardarCambios();

            return oEntityErrors;
        }
        private void GrabarCierreCuperaAlgoritmo(FormulaIni formulaDias)
        {
            var cierre = repositorio.Obtener<CierreCupera>(1);
            if (cierre != null)
            {
                cierre.Cierre = formulaDias.Cierre;
                cierre.MaterialId = 1; //cambiar cuando este la mejora,
            }
            else
            {
                var nuevoCierre = new CierreCupera
                {
                    MaterialId = 1, //cambiar cuando este la mejora,
                    Cierre = formulaDias.Cierre,
                };
                repositorio.Agregar(nuevoCierre);
            }
        }

        public List<CriterioIni> TodosLosCriterios()
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
        private void AgregarCriterio(Criterio criterio, Criterio agregar)
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
                    AgregarCriterio(hijo, agregar);
                }
            }
        }
        private void EliminarCriterio(Criterio criterio, Criterio eliminar)
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
                    EliminarCriterio(hijo, eliminar);
                }
            }
        }
        private void ActualizarCriterio(Criterio criterio, Criterio actualizar)
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
                    ActualizarCriterio(hijo, actualizar);
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
