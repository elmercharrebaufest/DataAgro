using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ProvinciaController : Controller
    {
        private IProvinciaManager mobjProvinciaManager;
        private IComercialManager mobjComercialManager;

        public ProvinciaController(IProvinciaManager oProvinciaManager, IComercialManager oComercialManager)
        {
            mobjProvinciaManager = oProvinciaManager;
            mobjComercialManager = oComercialManager;
            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Buscar()
        {
            var model = new ResultIniProvinciaModel();

            var result = mobjProvinciaManager.TraerTodoProvincia();

            if (result != null)
            {
                model.Datos = result.Provincia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Aplicar(AbmProvinciaParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmProvinciaResult
                {
                    Provincia = mobjProvinciaManager.TraerProvincia(oParam.ProvinciaId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Grabar(Provincia oProvincia)
        {
            var model = new AbmProvinciaResult();

            var entityErrors = mobjProvinciaManager.GrabarProvincia(oProvincia);

            if (entityErrors.HayErrores)
            {
                model.Errores = entityErrors.Errores;
                model.Provincia = oProvincia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Eliminar(AbmProvinciaParam oParam)
        {
            var model = new AbmProvinciaResult();

            var entityErrors = mobjProvinciaManager.EliminarProvincia(oParam.ProvinciaId);

            return new JsonResult()
            {
                Data = entityErrors,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Cancelar()
        {    
            return new JsonResult()
            {
                Data = new AbmProvinciaResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
        
    }
}


