using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class FormulaController : Controller
    {
        
        
        private IFormulaManager mobjFormulaManager;
        private IMaterialManager materialManager;
        private ICupoManager cupoManager;
        private readonly IHttpContextManager httpContextManager;
        private ITipoNegocioManager tipoNegocioManager;
        private readonly IConfiguracionManager configuracionManager;

        public FormulaController(IFormulaManager oFormulaManager, IMaterialManager materialManager, ICupoManager cupoManager, IHttpContextManager httpContextManager, ITipoNegocioManager tipoNegocioManager, IConfiguracionManager configuracionManager)
        {
            mobjFormulaManager = oFormulaManager;
            this.materialManager = materialManager;
            this.cupoManager = cupoManager;
            this.httpContextManager = httpContextManager;
            this.tipoNegocioManager = tipoNegocioManager;
            this.configuracionManager = configuracionManager;
        }

        [Autorizacion(PermisosDataAgro.AlgoritimoDeCupos)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }

        private void FillViewBag()
        {
            var tipoNegocio = tipoNegocioManager.TraerTodoTipoNegocio().FindAll(x => x.Descripcion.Contains("A FIJAR") || x.Descripcion.Contains("A PRECIO") || x.Descripcion.Contains("ESPACIO") || x.Descripcion.Contains("AGENTE DE COMPRAS"));
            
            var tipoNegocioListItems = tipoNegocio.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.TipoNegocioId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Text);
            ViewBag.TipoNegocio = tipoNegocioListItems;
            var config = configuracionManager.TraerConfiguraciones();
            ViewBag.Vincular = config.ExigirNegocioEnSolExt.GetValueOrDefault();
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

                        ultimaFormulaTraida = mobjFormulaManager.UltimaFormula(MaterialId ?? (int)EnumMateriales.SOJA),
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

        //static readonly object _lockEjecutarFormula = new object();

        //private async Task<string> EjecutarFormulaAsync(int materialId,FormulaDto formula, string path)
        //{
        //    await Task.Run(() =>
        //    {
        //        //lock (_lockEjecutarFormula)
        //        //{
        //        //Thread.Sleep(5000);
        //        cupoManager.EjecutarAlgoritmoManual(materialId, formula, null, path);
        //        //}
        //    });
        //    return "";
        //}

        //public ActionResult EjecutarFormula(int materialId)
        //{
        //   var formula= cupoManager.ObtenerFormulaDto(materialId);
        //    var path = httpContextManager.ObtenerPathLogoMail();
        //    var a = EjecutarFormulaAsync(materialId, formula, path);
        //    return Json("Estamos procesando tu solicitud, en breve te enviaremos un mail con el resultado del algoritmo.");
        //}

        public ActionResult BuscarTiposNegociosExcluidos(int? MaterialId)
        {
            var modelo = new ResultIniTipoNegocioExcluidoModel();
            var result = mobjFormulaManager.TraerTiposNegociosExcluidosGuardados(MaterialId ?? 3);

            if (result != null)
            {
                modelo.Datos = result.TiposNegocioExcluidos;
            }

            return new JsonResult()
            {
                Data = modelo,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ActualizarTiposNegociosExcluidos(int materialId, List<TipoNegocioDto> tiposNegociosExcluidos, FormulaIni formulaDias)
        {
            var modelo = new AbmFormulaResult();

            var entityErrors = mobjFormulaManager.ActualizarTiposNegociosExcluidos(materialId, tiposNegociosExcluidos, formulaDias);
            modelo.Errores = entityErrors.Errores;

            return new JsonResult()
            {
                Data = modelo,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult VincularSolicitudExtraordinaria(bool valor)
        {
            var resultado = configuracionManager.SetExigirNegocioEnSolExt(valor);

            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
