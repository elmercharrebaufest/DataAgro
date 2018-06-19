
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;

using WebDataAgro.Core;
using WebDataAgro.Models;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Business;

namespace WebDataAgro.Controllers
{
    public class CondicionController : Controller
    {
        //-----------------------------------------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------------------------------------

        private MSContext mobjMSContext;

        private ICondicionManager mobjCondicionManager;

        private string idActiveDirectory;

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public CondicionController(IMSContextProvider oMSContextProvider, ICondicionManager oCondicionManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjCondicionManager = oCondicionManager;

            mobjCondicionManager.Inicializar(mobjMSContext);

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);


            if (mobcomercialmanager.EsPerfilAdministrativo(idActiveDirectory) || mobcomercialmanager.EsPerfilVisualizador(idActiveDirectory))
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
            var model = new ResultIniCondicionModel();

            var result = await mobjCondicionManager.TraerTodoCondicionAsync();

            if (result != null)
            {
                model.Datos = result.Condicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Aplicar(AbmCondicionParam oParam)
        {
            var model = new AbmCondicionResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Condicion = await mobjCondicionManager.TraerCondicionAsync(oParam.CondicionId);
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


        public async Task<ActionResult> Grabar(Condicion oCondicion)
        {
            var model = new AbmCondicionResult();

            var entityErrors = await mobjCondicionManager.GrabarCondicionAsync(oCondicion);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Condicion = oCondicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



        public async Task<ActionResult> Eliminar(AbmCondicionParam oParam)
        {
            var model = new AbmCondicionResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjCondicionManager.EliminarCondicionAsync(oParam.CondicionId);
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
            var model = new AbmCondicionResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }



    }
}


