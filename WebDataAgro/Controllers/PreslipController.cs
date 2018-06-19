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

namespace WebDataAgro.Controllers
{
    public class PreslipController : Controller
    {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private IPreslipManager mobjPreslipManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public PreslipController(IMSContextProvider oMSContextProvider, IPreslipManager oPreslipManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjPreslipManager = oPreslipManager;

            mobjPreslipManager.Inicializar(mobjMSContext);
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
            var model = new DatosIniAbmPreslipModel();

            model.Datos = await mobjPreslipManager.TraerDatosInicialesAsync();

            model.Preslip = new Preslip();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniPreslipModel();

            var result = await mobjPreslipManager.TraerTodoPreslipAsync();

            if (result != null)
            {
                model.Datos = result.Preslip;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Aplicar(AbmPreslipParam oParam)
        {
            var model = new AbmPreslipResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Preslip = await mobjPreslipManager.TraerPreslipAsync(oParam.PreslipId);
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


        public async Task<ActionResult> Grabar(Preslip oPreslip)
        {
            var model = new AbmPreslipResult();

            var entityErrors = await mobjPreslipManager.GrabarPreslipAsync(oPreslip);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Preslip = oPreslip;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmPreslipParam oParam)
        {
            var model = new AbmPreslipResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                errors = await mobjPreslipManager.EliminarPreslipAsync(oParam.PreslipId);
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
            var model = new AbmPreslipResult();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


