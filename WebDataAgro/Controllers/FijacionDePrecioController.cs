using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class FijacionDePrecioController : Controller
    {
        private IFijacionDePrecioManager mobjFijacionDePrecioManager;
        private string idActiveDirectory;
        
        public FijacionDePrecioController(IMSContextProvider oMSContextProvider, IFijacionDePrecioManager oFijacionDePrecioManager)
        {
            mobjFijacionDePrecioManager = oFijacionDePrecioManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAbmFijacionDePrecioModel();

            model.Datos = await mobjFijacionDePrecioManager.TraerDatosInicialesAsync();

            model.FijacionDePrecio = new FijacionDePrecio();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniFijacionDePrecioModel();

            var result = await mobjFijacionDePrecioManager.TraerTodoFijacionDePrecioAsync();

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


        public async Task<ActionResult> Aplicar(AbmFijacionDePrecioParam oParam)
        {
            var model = new AbmFijacionDePrecioResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.FijacionDePrecio = await mobjFijacionDePrecioManager.TraerFijacionDePrecioAsync(oParam.FijacionId);
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


        public async Task<ActionResult> Grabar(FijacionDePrecio oFijacionDePrecio)
        {
            var model = new AbmFijacionDePrecioResult();

            var entityErrors = await mobjFijacionDePrecioManager.GrabarFijacionDePrecioAsync(oFijacionDePrecio, idActiveDirectory);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.FijacionDePrecio = oFijacionDePrecio;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmFijacionDePrecioParam oParam)
        {
            var model = new AbmFijacionDePrecioResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                errors = await mobjFijacionDePrecioManager.EliminarFijacionDePrecioAsync(oParam.FijacionId);
            }

            if (errors.ListaErrores.Count > 0)
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
            var model = new AbmFijacionDePrecioResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


