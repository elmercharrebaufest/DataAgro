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
    public class LocalidadController : Controller
    {
        private ILocalidadManager mobjLocalidadManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        public LocalidadController(IMSContextProvider oMSContextProvider, ILocalidadManager oLocalidadManager, IComercialManager oComercialManager)
        {
            mobjLocalidadManager = oLocalidadManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();

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


        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAbmLocalidadModel
            {
                Datos = await mobjLocalidadManager.TraerDatosInicialesAsync(),

                Localidad = new Localidad()
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        

        public async Task<ActionResult> Filtrar(ParamAbmLocalidad oParam)
        {
            var model = new ResultIniLocalidadModel();

            oParam.Nombre = oParam.Nombre ?? "";

            var result = await mobjLocalidadManager.TraerFiltroLocalidadAsync(oParam);

            if (result != null)
            {
                model.Datos = result.Localidad;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Aplicar(AbmLocalidadParam oParam)
        {
            var model = new AbmLocalidadResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Localidad = await mobjLocalidadManager.TraerLocalidadAsync(oParam.LocalidadId);
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


        public async Task<ActionResult> Grabar(Localidad oLocalidad)
        {
            var model = new AbmLocalidadResult();

            var entityErrors = await mobjLocalidadManager.GrabarLocalidadAsync(oLocalidad);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);
                      
            if (model.Errores.Count > 0)
            {
                model.Localidad = oLocalidad;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmLocalidadParam oParam)
        {
            var model = new AbmLocalidadResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjLocalidadManager.EliminarLocalidadAsync(oParam.LocalidadId);
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
            var model = new AbmLocalidadResult();
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


