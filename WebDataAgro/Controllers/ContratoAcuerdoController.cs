using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ContratoAcuerdoController : Controller
    {
        private IContratoAcuerdoManager mobjContratoAcuerdoManager;

        public ContratoAcuerdoController(IContratoAcuerdoManager oContratoAcuerdoManager)
        {
            mobjContratoAcuerdoManager = oContratoAcuerdoManager;
        }

        public ActionResult Index()
        {
            string ActionView = "";
            
            ViewBag.ComercialId = GlobalVariables.ComercialId;

            return View(ActionView);
        }
        
        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmContratoAcuerdoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarContratoAcuerdo()
        {
            return new JsonResult()
            {
                Data = new ContratoAcuerdoModel_prueba
                {
                    Datos = mobjContratoAcuerdoManager.TraerDatosCombo()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(ContratoAcuerdo oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.BorrarAcuerdo(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Confirmar(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.ConfirmarContratoAcuerdo(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }



        public ActionResult Grabar(ContratoAcuerdo oContratoAcuerdo)
        {
            var model = new AbmContratoAcuerdoResult();

            var entityErrors = mobjContratoAcuerdoManager.GrabarAcuerdo(oContratoAcuerdo);
            model.Errores = entityErrors.Errores;           

            return new JsonResult()
            {
                Data = entityErrors,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
