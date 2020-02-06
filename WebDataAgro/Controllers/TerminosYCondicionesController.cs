using Autofac.Extras.NLog;
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
        private readonly ILogger logger;

        public TerminosYCondicionesController(IUsuarioManager usuarioManager, ILogger logger)
        {
            this.usuarioManager = usuarioManager;
            this.logger = logger;
        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Aceptar()
        {
            try
            {
                usuarioManager.Aceptar(PermisosHelper.ObtenerUsuario(), PermisosHelper.ObtenerCuit());
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return RedirectToAction("Index", "Error");
            }
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