using Molinos.DataAgro.Entities.Seguridad;
using WebDataAgro.Atributos;
using System.Web.Mvc;
using NLog;
using Molinos.DataAgro.Entities.Entities;
using System;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Dto;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class CupoTerceroController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICupoManager _cupoManager;

        public CupoTerceroController(ILogger logger, ICupoManager cupoManager) 
        {
            _logger = logger;
            _cupoManager = cupoManager;
        }

        //[Autorizacion(PermisosDataAgro.IngresoDataAgro)]
        //public ActionResult DatosCupoSap(string cupoStop)
        //{
        //    CupoSapTerceroDto response = _cupoManager.DatosCupoSap(cupoStop);

        //    return new JsonResult()
        //    {
        //        Data = response,
        //        MaxJsonLength = Int32.MaxValue,
        //        JsonRequestBehavior = JsonRequestBehavior.AllowGet
        //    };
        //}
    }
}