using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class AdministracionCupoController : Controller
    {
        private readonly IAdministracionCupoManager administracionCupoManager;
        private readonly ICupoManager cupoManager;
        private readonly ICentroManager centroManager;

        public AdministracionCupoController(IAdministracionCupoManager administracionCupoManager, ICupoManager cupoManager, ICentroManager centroManager)
        {
            this.administracionCupoManager = administracionCupoManager;
            this.cupoManager = cupoManager;
            this.centroManager = centroManager;
        }

        private JsonResult CreateJsonResult(object data)
        {
            return new JsonResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                MaxJsonLength = Int32.MaxValue
            };
        }

        private void InitializeAdministrationViewBags()
        {
            ViewBag.Panel = cupoManager.Panel();
            ViewBag.Fechas = cupoManager.FechasComprendidas(null);
            ViewBag.SugerenciasNoAceptadas = cupoManager.SugerenciasNoAceptadas();
            ViewBag.comercialId = GlobalVariables.ComercialId;
        }

        [Autorizacion(PermisosDataAgro.AdministracionCupos)]
        public ActionResult Index()
        {
            InitializeAdministrationViewBags();
            ViewBag.CentrosTodos = centroManager.TraerTodoCentro().Centro
                .Where(x => x.CargaCupos)
                .Select(x => x.Descripcion)
                .OrderByDescending(x => x)
                .ToList();
            return View();
        }

        public ActionResult DatosAdministracion(KendoGridMvcRequest request)
        {
            var model = administracionCupoManager.TraerTodaAdministracionCupo(request, null);
            return CreateJsonResult(model);
        }

        public ActionResult PartialPanel()
        {
            InitializeAdministrationViewBags();
            return PartialView("PartialPanel");
        }

        public JsonResult Aceptar(int administracionId, int cantidadCupo, int cantidadFleteProcedencia, int cantidadOriginal, int cantidadFleteOriginal, string motivo)
        {
            if (administracionId <= 0)
            {
                var errorResult = new CupoResult();
                errorResult.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna sugerencia"));
                return Json(errorResult);
            }

            var resultado = administracionCupoManager.AceptarCupoExcedente(administracionId, cantidadCupo, cantidadFleteProcedencia, cantidadOriginal, cantidadFleteOriginal, GlobalVariables.IdActiveDirectory, motivo);
            return Json(resultado);
        }

        public JsonResult Rechazar(int administracionId, string motivo)
        {
            var resultado = administracionCupoManager.CambiarEstadoRechazado(administracionId, motivo, GlobalVariables.IdActiveDirectory);
            return Json(resultado);
        }

        public JsonResult AceptarMasivo(List<AdministracionCupoDto> solicitudes, string motivo)
        {
            if (solicitudes == null || solicitudes.Count == 0)
            {
                return Json(new CupoResult
                {
                    Errores = new List<ErrorMessage>
                    {
                        new ErrorMessage(400, "No se seleccionó ninguna solicitud")
                    }
                });
            }

            var resultados = administracionCupoManager.AceptarCupoExcedenteMasivo(solicitudes, motivo, GlobalVariables.IdActiveDirectory);
            return Json(resultados);
        }

        public JsonResult RechazarMasivo(List<AdministracionCupoDto> solicitudes, string motivo)
        {
            if (solicitudes == null || solicitudes.Count == 0)
            {
                var errorResult = new Resultado();
                errorResult.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna solicitud"));
                return Json(errorResult);
            }

            var resultados = new List<Resultado>();
            foreach (var adm in solicitudes)
            {
                var resultado = administracionCupoManager.CambiarEstadoRechazado(adm.Id, motivo, GlobalVariables.IdActiveDirectory);
                resultados.Add(resultado);
            }

            return Json(resultados);
        }

        public ActionResult BuscarDatosSolicitudCupo(KendoGridMvcRequest request, int? ComercialId)
        {
            ComercialId = ComercialId ?? GlobalVariables.ComercialId;
            var model = administracionCupoManager.TraerTodaAdministracionCupo(request, ComercialId);
            return CreateJsonResult(model);
        }

        public ActionResult ActualizarSolicitud(int id, int cantidadCupo, int cantidadFlete, bool estado)
        {
            var model = administracionCupoManager.ActualizarSolicitud(id, cantidadCupo, cantidadFlete, estado);
            return CreateJsonResult(model);
        }
    }
}
