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
    public class ComercialController : Controller
    {
        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ComercialController(IMSContextProvider oMSContextProvider, IComercialManager oComercialManager)
        {
            mobjComercialManager = oComercialManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            } else if (!mobjComercialManager.ComercialExiste(idActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            return View(ActionView);

        }


        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAbmComercialModel
            {
                Datos = await mobjComercialManager.TraerDatosInicialesAsync(),

                Comercial = new Comercial()
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniComercialModel();

            var result = await mobjComercialManager.TraerTodoComercialAsync();

            if (result != null)
            {
                model.Datos = result.Comercial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> ComercialCombo(AbmComercialParam oParam)
        {
            var model = new DataAbmComercial
            {
                Comercial = await mobjComercialManager.ObtenerComerciales(oParam.ComercialId)
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        

        public async Task<ActionResult> Aplicar(AbmComercialParam oParam)
        {
            var model = new AbmComercialResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Comercial = await mobjComercialManager.TraerComercialAsync(oParam.ComercialId);
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


        public async Task<ActionResult> Grabar(Comercial oComercial)
        {
            var model = new AbmComercialResult();

            var entityErrors = await mobjComercialManager.GrabarComercialAsync(oComercial);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Comercial = oComercial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmComercialParam oParam)
        {
            var model = new AbmComercialResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                errors = await mobjComercialManager.EliminarComercialAsync(oParam.ComercialId);
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
            var model = new AbmComercialResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


