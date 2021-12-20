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
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class FormulaController : Controller
    {


        private IFormulaManager mobjFormulaManager;
        private IMaterialManager materialManager;

        public FormulaController(IFormulaManager oFormulaManager, IMaterialManager materialManager)
        {
            mobjFormulaManager = oFormulaManager;
            this.materialManager = materialManager;
        }
        [Autorizacion(PermisosDataAgro.AlgoritimoDeCupos)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Inicializar(int? MaterialId)
        {
            return new JsonResult()
            {
                Data = new ResultIniFormulaModel
                {
                    Datos = new DatosIniAbmFormula
                    {

                        Criterios = new ResultIniCriterio
                        {
                            Criterios = mobjFormulaManager.TodosLosCriterios()
                        },

                        ultimaFormulaTraida = mobjFormulaManager.UltimaFormula(MaterialId ?? 3),
                        materiales = materialManager.TraerTodoMaterial().Material.OrderBy(a => a.MaterialId).ToList()
                    }
                },
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult Buscar(int? MaterialId)
        {

            var model = new ResultIniCriterioModel();

            var result = mobjFormulaManager.TraerCriteriosGuardados(MaterialId ?? 3);

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
        [HttpPost]
        public ActionResult Eliminar(CriterioIni criterio)
        {
            var model = new AbmCriterioResult();

            var entityErrors = mobjFormulaManager.EliminarCriterio(criterio);
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

            var entityErrors = mobjFormulaManager.ActualizarDias(formulaDias);
            modelo.Errores = entityErrors.Errores;

            if (modelo.HayErrores)
            {
                modelo.Formula = new FormulaDto
                {
                    CuposDesde = formulaDias.CuposDesde,
                    CuposHasta = formulaDias.CuposHasta,
                    NegociosDesde = formulaDias.NegociosDesde,
                    NegociosHasta = formulaDias.NegociosHasta
                };
            }

            return new JsonResult()
            {
                Data = modelo,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult ActualizarCierre(FormulaIni formulaDias)
        {
            var modelo = new AbmFormulaResult();

            var entityErrors = mobjFormulaManager.ActualizarCierre(formulaDias);
            modelo.Errores = entityErrors.Errores;

            if (modelo.HayErrores)
            {
                modelo.Formula = new FormulaDto
                {
                    CuposDesde = formulaDias.CuposDesde,
                    CuposHasta = formulaDias.CuposHasta,
                    NegociosDesde = formulaDias.NegociosDesde,
                    NegociosHasta = formulaDias.NegociosHasta
                };
            }

            return new JsonResult()
            {
                Data = modelo,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
