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
    public class LocalidadController : Controller
    {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private ILocalidadManager mobjLocalidadManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public LocalidadController(IMSContextProvider oMSContextProvider, ILocalidadManager oLocalidadManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjLocalidadManager = oLocalidadManager;

            mobjLocalidadManager.Inicializar(mobjMSContext);

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


        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAbmLocalidadModel();

            model.Datos = await mobjLocalidadManager.TraerDatosInicialesAsync();

            model.Localidad = new Localidad();

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


