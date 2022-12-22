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
        public JsonResult Aceptar(int administracionId, int cantidadCupo, int cantidadFleteProcedencia, int cantidadOriginal, int cantidadFleteOriginal, string motivo)
        {
            CupoResult resultado = new CupoResult();
            if (administracionId != 0)
            {
                resultado = administracionCupoManager.AceptarCupoExcedente(administracionId, cantidadCupo, cantidadFleteProcedencia, cantidadOriginal, cantidadFleteOriginal, GlobalVariables.IdActiveDirectory, motivo);
            }
            else
            {
                resultado.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna sugerencia"));
            }
            return Json(resultado);
        }

        public JsonResult Rechazar(int administracionId, string motivo)
        {
            var resultado = administracionCupoManager.CambiarEstadoRechazado(administracionId, motivo, GlobalVariables.IdActiveDirectory);
            return Json(resultado);
        }

        public JsonResult AceptarMasivo(List<AdministracionCupoDto> solicitudes, string motivo)
        {
            CupoResult resultados = new CupoResult();
            if (solicitudes.Count > 0)
            {
                resultados = administracionCupoManager.AceptarCupoExcedenteMasivo(solicitudes, motivo, GlobalVariables.IdActiveDirectory);
            }
            else
            {
                resultados = new CupoResult { Errores = new List<ErrorMessage> { new ErrorMessage(400, "No se seleccionó ninguna solicitud") } };
            }
            return Json(resultados);
        }

        public JsonResult RechazarMasivo(List<AdministracionCupoDto> solicitudes, string motivo)
        {
            var resultado = new Resultado();
            if (solicitudes.Count > 0)
            {
                foreach (var adm in solicitudes)
                {
                    resultado = administracionCupoManager.CambiarEstadoRechazado(adm.Id, motivo, GlobalVariables.IdActiveDirectory);
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

        public ActionResult ActualizarSolicitud(int id, int cantidadCupo, int cantidadFlete, bool estado)
        {
            var model = administracionCupoManager.ActualizarSolicitud(id, cantidadCupo, cantidadFlete, estado);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
    }
}