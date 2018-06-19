using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;

using WebDataAgro.Core;
using WebDataAgro.Models;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Business;

namespace WebDataAgro.Controllers
{
    public class ProvinciaController : Controller
    {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private IProvinciaManager mobjProvinciaManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ProvinciaController(IMSContextProvider oMSContextProvider, IProvinciaManager oProvinciaManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjProvinciaManager = oProvinciaManager;

            mobjProvinciaManager.Inicializar(mobjMSContext);

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjComercialManager = oComercialManager;
            mobjComercialManager.Inicializar(mobjMSContext);

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);


            if (mobcomercialmanager.EsPerfilAdministrativo(idActiveDirectory) || mobcomercialmanager.EsPerfilVisualizador(idActiveDirectory))
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


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniProvinciaModel();

            var result = await mobjProvinciaManager.TraerTodoProvinciaAsync();

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
        

        public async Task<ActionResult> Aplicar(AbmProvinciaParam oParam)
        {
            var model = new AbmProvinciaResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Provincia = await mobjProvinciaManager.TraerProvinciaAsync(oParam.ProvinciaId);
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


        public async Task<ActionResult> Grabar(Provincia oProvincia)
        {
            var model = new AbmProvinciaResult();

            var entityErrors = await mobjProvinciaManager.GrabarProvinciaAsync(oProvincia);

            if (model.Errores.Count == 0)
            {
                model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);
            }

            if (model.Errores.Count > 0)
            {
                model.Provincia = oProvincia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmProvinciaParam oParam)
        {
            var model = new AbmProvinciaResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjProvinciaManager.EliminarProvinciaAsync(oParam.ProvinciaId);
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
            var model = new AbmProvinciaResult();
                      
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
    }
}


