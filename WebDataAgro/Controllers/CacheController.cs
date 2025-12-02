using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NLog;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro, PermisosDataAgro.ConfiguracionConstantes)]
    public class CacheController : Controller
    {
        private readonly ILogger logger;
        private readonly ICache cache;
        public CacheController(ILogger logger, ICache cache)
        {
            this.logger = logger;
            this.cache = cache;
        }

        public ActionResult Index()
        {
            var listado = cache.ListAllCacheItems().Where(a=>!a.Key.Contains("MetadataPrototypes")).ToList();
            return View(listado);
        }
    }
}
