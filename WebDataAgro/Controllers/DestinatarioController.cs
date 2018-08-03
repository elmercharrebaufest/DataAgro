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
    public class DestinatarioController : Controller
    {
        private IDestinatarioManager mobjDestinatarioManager;
        
        

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public DestinatarioController(IDestinatarioManager oDestinatarioManager)
        {
            mobjDestinatarioManager = oDestinatarioManager;
            
            
            
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
        
        public ActionResult Buscar()
        {
            var model = new ResultIniDestinatarioModel();

            var result = mobjDestinatarioManager.TraerTodoDestinatario();

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
        
        public ActionResult Aplicar(AbmDestinatarioParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmDestinatarioResult
                {
                    Destinatario = mobjDestinatarioManager.TraerDestinatario(oParam.DestinatarioId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Grabar(Destinatario oDestinatario)
        {
            var model = new AbmDestinatarioResult();

            var entityErrors = mobjDestinatarioManager.GrabarDestinatario(oDestinatario);
            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.Destinatario = oDestinatario;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Eliminar(AbmDestinatarioParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjDestinatarioManager.EliminarDestinatario(oParam.DestinatarioId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmDestinatarioResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


