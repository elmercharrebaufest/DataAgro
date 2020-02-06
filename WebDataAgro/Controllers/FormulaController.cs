using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class FormulaController : Controller
    {


        private IFormulaManager mobjFormulaManager;

        public FormulaController(IFormulaManager oFormulaManager)
        {
            mobjFormulaManager = oFormulaManager;
        }
        [Autorizacion(PermisosDataAgro.Formula)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new ResultIniFormulaModel
                {
                    Datos = new DatosIniAbmFormula
                    {

                        Criterios = new ResultIniCriterio
                        {
                            Criterios = mobjFormulaManager.todosLosCriterios()
                        },

                        ultimaFormulaTraida = mobjFormulaManager.ultimaFormula()
                    }
                },
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult Buscar()
        {

            var model = new ResultIniCriterioModel();

            var result = mobjFormulaManager.TraerCriteriosGuardados();

            if (result != null)
            {
                model.Datos = result.Criterios;
            }

            return Json(model.Datos, JsonRequestBehavior.AllowGet);

        }
        public ActionResult Update(CriterioIni criterio)
        {
            var model = new AbmCriterioResult();

            var entityErrors = mobjFormulaManager.GrabarCriterio(criterio);
            model.Errores = entityErrors.Errores;


            if (model.HayErrores)
            {
                model.Criterio = new CriterioDto { Prioridad = criterio.Prioridad, PadreId = criterio.PadreId };
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Eliminar(CriterioIni criterio)
        {
            var model = new AbmCriterioResult();

            var entityErrors = mobjFormulaManager.eliminarCriterio(criterio);
            model.Errores = entityErrors.Errores;


            if (model.HayErrores)
            {
                model.Criterio = new CriterioDto { Prioridad = criterio.Prioridad, PadreId = criterio.PadreId };
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult ActualizarDiasFormula(FormulaIni formulaDias)
        {
            var modelo = new AbmFormulaResult();

            var entityErrors = mobjFormulaManager.actualizarDias(formulaDias);
            modelo.Errores = entityErrors.Errores;

            if (modelo.HayErrores)
            {
                modelo.Formula = new FormulaDto { Inicio = formulaDias.Inicio, CantDias = formulaDias.CantDias };
            }

            return new JsonResult()
            {
                Data = modelo,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
