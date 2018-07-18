using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class CentroController : Controller
    {
        private string idActiveDirectory;

        private ICentroManager mobjCentroManager;
        private IComercialManager mobjComercialManager;

        public CentroController(IMSContextProvider oMSContextProvider, ICentroManager oCentroManager, IComercialManager oComercialManager)
        {
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjCentroManager = oCentroManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
            else if (!mobjComercialManager.ComercialExiste(idActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }
            return View(ActionView);
        }

        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAbmCentroModel
            {
                Datos = await mobjCentroManager.TraerDatosInicialesAsync()
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniCentroModel();

            var result = await mobjCentroManager.TraerTodoCentroAsync();

            if (result != null)
            {
                model.Datos = result.Centro;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> CentroCombo(AbmCentroParam oParam)
        {
            var model = new DataAbmCentro
            {
                Centro = await mobjCentroManager.TraerCentroAsync(oParam.Id)
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public async Task<ActionResult> Aplicar(AbmCentroParam oParam)
        {
            var model = new AbmCentroResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Centro = await mobjCentroManager.TraerCentroAsync(oParam.Id);
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

        public async Task<ActionResult> Grabar(Centro oCentro)
        {
            var model = new AbmCentroResult();

            var entityErrors = await mobjCentroManager.GrabarCentroAsync(oCentro);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Centro = oCentro;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> Eliminar(AbmCentroParam oParam)
        {
            var model = new AbmCentroResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                errors = await mobjCentroManager.EliminarCentroAsync(oParam.Id);
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
            var model = new AbmCentroResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}