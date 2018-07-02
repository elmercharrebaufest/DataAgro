
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class CanalOperacionController : Controller
    {
        private ICanalOperacionManager mobjCanalOperacionManager;

        public CanalOperacionController(ICanalOperacionManager oCanalOperacionManager)
        {
            mobjCanalOperacionManager = oCanalOperacionManager;
        }
        
        public ActionResult Index()
        {
            ViewBag.edita = false;
            return View();
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniCanalOperacionModel();

            var result = await mobjCanalOperacionManager.TraerTodoCanalOperacionAsync();

            if (result != null)
            {
                model.Datos = result.CanalOperacion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Aplicar(AbmCanalOperacionParam oParam)
        {
            var model = new AbmCanalOperacionResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.CanalOperacion = await mobjCanalOperacionManager.TraerCanalOperacionAsync(oParam.CanalOperacionId);
            }
            else
            {
                model.Errores = Util.EntityErrorsToMSErrorMessage(errors);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Grabar(CanalOperacion oCanalOperacion)
        {
            var model = new AbmCanalOperacionResult();

            var entityErrors = await mobjCanalOperacionManager.GrabarCanalOperacionAsync(oCanalOperacion);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.CanalOperacion = oCanalOperacion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



        public async Task<ActionResult> Eliminar(AbmCanalOperacionParam oParam)
        {
            var model = new AbmCanalOperacionResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjCanalOperacionManager.EliminarCanalOperacionAsync(oParam.CanalOperacionId);
            }
            else
            {
                model.Errores = Util.EntityErrorsToMSErrorMessage(errors);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Cancelar()
        {
            var model = new AbmCanalOperacionResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



    }
}


