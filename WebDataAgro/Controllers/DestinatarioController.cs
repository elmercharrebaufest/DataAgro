
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
    public class DestinatarioController : Controller
    {
        private IDestinatarioManager mobjDestinatarioManager;
        
        private string idActiveDirectory;

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public DestinatarioController(IMSContextProvider oMSContextProvider, IDestinatarioManager oDestinatarioManager)
        {
            mobjDestinatarioManager = oDestinatarioManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            
            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
        }

        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniDestinatarioModel();

            var result = await mobjDestinatarioManager.TraerTodoDestinatarioAsync();

            if (result != null)
            {
                model.Datos = result.Destinatario;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Aplicar(AbmDestinatarioParam oParam)
        {
            var model = new AbmDestinatarioResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Destinatario = await mobjDestinatarioManager.TraerDestinatarioAsync(oParam.DestinatarioId);
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


        public async Task<ActionResult> Grabar(Destinatario oDestinatario)
        {
            var model = new AbmDestinatarioResult();

            var entityErrors = await mobjDestinatarioManager.GrabarDestinatarioAsync(oDestinatario);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Destinatario = oDestinatario;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



        public async Task<ActionResult> Eliminar(AbmDestinatarioParam oParam)
        {
            var model = new AbmDestinatarioResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjDestinatarioManager.EliminarDestinatarioAsync(oParam.DestinatarioId);
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
            var model = new AbmDestinatarioResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



    }
}


