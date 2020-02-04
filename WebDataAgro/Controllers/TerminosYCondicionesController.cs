using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class TerminosYCondicionesController : Controller
    {
        private readonly IUsuarioManager usuarioManager;

        public TerminosYCondicionesController(IUsuarioManager usuarioManager)
        {
            this.usuarioManager = usuarioManager;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Aceptar()
        {
            usuarioManager.Aceptar(PermisosHelper.ObtenerUsuario(),PermisosHelper.ObtenerCuit());
            if (PermisosHelper.Is(PermisosDataAgro.VisualizarCompraNet))
            {
                return RedirectToAction("Index", "CompraNet");
            }
            else
            {
                return RedirectToAction("Index", "Error");
            }
        }
    }
}