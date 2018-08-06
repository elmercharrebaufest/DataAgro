using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class FijacionDePrecioController : Controller
    {
        private IFijacionDePrecioManager mobjFijacionDePrecioManager;
        
        
        public FijacionDePrecioController(IFijacionDePrecioManager oFijacionDePrecioManager)
        {
            mobjFijacionDePrecioManager = oFijacionDePrecioManager;
            
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmFijacionDePrecioModel
                {
                    Datos = mobjFijacionDePrecioManager.TraerDatosIniciales(),
                    FijacionDePrecio = new FijacionDePrecio()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Buscar()
        {
            var model = new ResultIniFijacionDePrecioModel();

            var result = mobjFijacionDePrecioManager.TraerTodoFijacionDePrecio();

            if (result != null)
            {
                model.Datos = result.FijacionDePrecio;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Aplicar(AbmFijacionDePrecioParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmFijacionDePrecioCrearResult
                {
                    FijacionDePrecio = mobjFijacionDePrecioManager.TraerFijacionDePrecio(oParam.FijacionId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Grabar(FijacionDePrecio oFijacionDePrecio)
        {
            var model = new AbmFijacionDePrecioResult();

            var entityErrors = mobjFijacionDePrecioManager.GrabarFijacionDePrecio(oFijacionDePrecio, GlobalVariables.IdActiveDirectory);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.FijacionDePrecio = oFijacionDePrecio;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Eliminar(AbmFijacionDePrecioParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioManager.EliminarFijacionDePrecio(oParam.FijacionId),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmFijacionDePrecioResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


