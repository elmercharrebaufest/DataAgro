using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro, PermisosDataAgro.ConfiguracionCanalOperacion)]
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

        public ActionResult Buscar()
        {
            var model = new ResultIniCanalOperacionModel();

            var result = mobjCanalOperacionManager.TraerTodoCanalOperacion();

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

        public ActionResult Aplicar(AbmCanalOperacionParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmCanalOperacionResult
                {
                    CanalOperacion = mobjCanalOperacionManager.TraerCanalOperacion(oParam.CanalOperacionId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(CanalOperacion oCanalOperacion)
        {
            var model = new AbmCanalOperacionResult();

            var entityErrors = mobjCanalOperacionManager.GrabarCanalOperacion(oCanalOperacion);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.CanalOperacion = new CanalOperacionDto { CanalOperacionId = oCanalOperacion.CanalOperacionId, Descripcion = oCanalOperacion.Descripcion, Inhabilitado = oCanalOperacion.Inhabilitado};
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Eliminar(AbmCanalOperacionParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjCanalOperacionManager.EliminarCanalOperacion(oParam.CanalOperacionId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmCanalOperacionResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


