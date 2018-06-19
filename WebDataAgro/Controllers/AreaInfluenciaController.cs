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
    public class AreaInfluenciaController : Controller
    {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private IAreaInfluenciaManager mobjAreaInfluenciaManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public AreaInfluenciaController(IMSContextProvider oMSContextProvider, IAreaInfluenciaManager oAreaInfluenciaManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjAreaInfluenciaManager = oAreaInfluenciaManager;

            mobjAreaInfluenciaManager.Inicializar(mobjMSContext);

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


        public ActionResult Inicializar()
        {
            var model = new DatosIniAbmAreaInfluenciaModel();

            model.AreaInfluencia = new AreaInfluencia();

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


