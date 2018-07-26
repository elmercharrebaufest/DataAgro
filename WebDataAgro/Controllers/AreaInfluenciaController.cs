using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Common.Enums;
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
    public class AreaInfluenciaController : Controller
    {
        private IAreaInfluenciaManager mobjAreaInfluenciaManager;

        private string idActiveDirectory;

        public AreaInfluenciaController(IMSContextProvider oMSContextProvider, IAreaInfluenciaManager oAreaInfluenciaManager)
        {
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjAreaInfluenciaManager = oAreaInfluenciaManager;
            
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


        public ActionResult Inicializar()
        {
            var model = new DatosIniAbmAreaInfluenciaModel
            {
                AreaInfluencia = new AreaInfluencia()
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniAreaInfluenciaModel();

            var result = await mobjAreaInfluenciaManager.TraerTodoAreaInfluenciaAsync();

            if (result != null)
            {
                model.Datos = result.AreaInfluencia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


       


        public async Task<ActionResult> Aplicar(AbmAreaInfluenciaParam oParam)
        {
            var model = new AbmAreaInfluenciaResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.AreaInfluencia = await mobjAreaInfluenciaManager.TraerAreaInfluenciaAsync(oParam.AreaInfluenciaId);
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


        public async Task<ActionResult> Grabar(AreaInfluencia oAreaInfluencia)
        {
            var model = new AbmAreaInfluenciaResult();

            var entityErrors = await mobjAreaInfluenciaManager.GrabarAreaInfluenciaAsync(oAreaInfluencia);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.AreaInfluencia = oAreaInfluencia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmAreaInfluenciaParam oParam)
        {
            var model = new AbmAreaInfluenciaResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjAreaInfluenciaManager.EliminarAreaInfluenciaAsync(oParam.AreaInfluenciaId);
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
            var model = new AbmAreaInfluenciaResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


