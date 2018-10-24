using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace WebDataAgro.Controllers
{
    public class ErrorController : Controller
    {
        public ActionResult Index(string jsonx = "",bool esJson = false ,bool esMsj = false )
        {
          if ( jsonx == "")
            {
                ViewBag.Title = "Error-";
                ViewBag.Description = "La Peticion No Ha Podido Ser Procesada";                 
            }
          else if (esJson)
            {
                jsonx = jsonx.Replace(((Char)34).ToString(), "'");
                jsonx = jsonx.Replace(((Char)13).ToString(), "");
                LErrores Mierror = JsonConvert.DeserializeObject<LErrores>(jsonx);
                ViewBag.Title = Mierror.Errores[0].Source;
                ViewBag.Description = Mierror.Errores[0].Message;
            }
          else if (esMsj)
            {
                ViewBag.Title = "Error-";
                ViewBag.Description = "La Peticion No Ha Podido Ser Procesada";
            }
          else
            {
                ViewBag.Title = "Error-";
                ViewBag.Description = jsonx;
            }
            return View();
        }
    }

    public class Errores
    {
        public string Source { get; set; }
        public string Message { get; set; }
    }

    public class LErrores
    {
        public List<Errores> Errores { get; set; }
    }
}