using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class FAQController : Controller
    {
        private readonly IFAQManager mobjFAQManager;

        public FAQController(IFAQManager faqManager) {
            this.mobjFAQManager = faqManager;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TraerManuales()
        {
            List<ManualesDto> result;
            result = mobjFAQManager.TraerManuales();
            
            return new JsonResult()
            {
                Data = result,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public JsonResult EnviarSugerencia(ManualesDto manual, string sugerencia)
        {
            var resultado = mobjFAQManager.EnviarSugerencia(manual, sugerencia, GlobalVariables.IdActiveDirectory);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ContarVisita(int idManual)
        {
            var resultado = mobjFAQManager.RegistrarVisita(idManual, GlobalVariables.IdActiveDirectory);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

    }
}