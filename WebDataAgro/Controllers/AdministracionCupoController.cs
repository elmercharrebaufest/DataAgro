using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class AdministracionCupoController : Controller
    {
        private readonly IAdministracionCupoManager administracionCupoManager;
        private readonly ICupoManager cupoManager;

        public AdministracionCupoController(IAdministracionCupoManager administracionCupoManager, ICupoManager cupoManager)
        {
            this.administracionCupoManager = administracionCupoManager;
            this.cupoManager = cupoManager;
        }
        [Autorizacion(PermisosDataAgro.AdministracionCupos)]
        public ActionResult Index()
        {
            ViewBag.Panel = cupoManager.Panel();
            ViewBag.Fechas = cupoManager.FechasComprendidas(null);
            ViewBag.SugerenciasNoAceptadas = cupoManager.SugerenciasNoAceptadas();
            ViewBag.comercialId = GlobalVariables.ComercialId;
            return View();
        }
        public ActionResult DatosAdministracion(KendoGridMvcRequest request)
        {
            //ViewBag.Panel = cupoManager.Panel();
            //ViewBag.Fechas = cupoManager.FechasComprendidas(null);
            //ViewBag.SugerenciasNoAceptadas = cupoManager.SugerenciasNoAceptadas();
            var model = administracionCupoManager.TraerTodaAdministracionCupo(request, null);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        public ActionResult PartialPanel()
        {
            ViewBag.Panel = cupoManager.Panel();
            ViewBag.Fechas = cupoManager.FechasComprendidas(null);
            ViewBag.SugerenciasNoAceptadas = cupoManager.SugerenciasNoAceptadas();
            ViewBag.comercialId = GlobalVariables.ComercialId;
            return PartialView("PartialPanel");
        }
        public JsonResult Aceptar(int administracionId, int cantidadCupo, int cantidadFleteProcedencia)
        {
            CupoResult resultado = new CupoResult();
            if (administracionId != 0)
            {
                resultado = administracionCupoManager.AceptarCupoExcedente(administracionId, cantidadCupo, cantidadFleteProcedencia, GlobalVariables.IdActiveDirectory);
            }
            else
            {
                resultado.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna sugerencia"));
            }
            return Json(resultado);
        }

        public JsonResult Rechazar(int administracionId)
        {
            var resultado = administracionCupoManager.CambiarEstadoRechazado(administracionId, GlobalVariables.IdActiveDirectory);
            return Json(resultado);
        }

        public JsonResult AceptarMasivo(List<AdministracionCupoDto> solicitudes)
        {
            CupoResult resultados = new CupoResult();
            if (solicitudes.Count > 0)
            {
                foreach (var adm in solicitudes)
                {
                    var resultado = administracionCupoManager.AceptarCupoExcedente(adm.Id, adm.CantidadDeCupo, adm.CantidadFleteProcedencia, GlobalVariables.IdActiveDirectory);
                    if (resultado.HayError)
                        resultados.Errores.AddRange(resultado.Errores);
                }
            }
            else
            {
                resultados = new CupoResult { Errores = new List<ErrorMessage> { new ErrorMessage(400, "No se seleccionó ninguna solicitud") } };
            }
            return Json(resultados);
        }

        public JsonResult RechazarMasivo(List<AdministracionCupoDto> solicitudes)
        {
            var resultado = new Resultado();
            if (solicitudes.Count > 0)
            {
                foreach (var adm in solicitudes)
                {
                    resultado = administracionCupoManager.CambiarEstadoRechazado(adm.Id, GlobalVariables.IdActiveDirectory);
                }
            }
            else
            {
                resultado.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna solicitud"));
            }
            return Json(resultado);
        }

        public ActionResult BuscarDatosSolicitudCupo(KendoGridMvcRequest request, int? ComercialId)
        {
            ComercialId = ComercialId ?? GlobalVariables.ComercialId;
            var model = administracionCupoManager.TraerTodaAdministracionCupo(request, ComercialId);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}