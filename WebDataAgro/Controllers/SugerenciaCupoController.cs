
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using System;
using static WebDataAgro.MvcApplication;
using System.Web.Script.Serialization;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using KendoGridBinder.ModelBinder.Mvc;
using KendoGridBinder;
using Molinos.DataAgro.Entities.Dto;
using System.ComponentModel;
using System.Linq;
using WebDataAgro.Atributos;
using Molinos.DataAgro.Entities.Seguridad;
using System.Web.UI.HtmlControls;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class SugerenciaCupoController : Controller
    {
        private readonly ILogger logger;
        private readonly ICupoManager cupoManager;
        private readonly IMaterialManager oMaterialManager;
        private readonly ICentroManager centroManager;

        public SugerenciaCupoController(ILogger logger, ICentroManager centroManager, ICupoManager cupoManager, IMaterialManager oMaterialManager)
        {
            this.centroManager = centroManager;
            this.logger = logger;
            this.cupoManager = cupoManager;
            this.oMaterialManager = oMaterialManager;
        }

        [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
        public ActionResult Index(int materialId = 2, string centroId = "1029")
        {
            CargarVista(materialId, centroId);
            return View();
        }
        public ActionResult PartialTabla(int materialId, string centroId)
        {
            CargarVista(materialId, centroId);
            return PartialView();
        }

        private void CargarVista(int materialId = 2, string centroId = "1029")
        {
            var lista = cupoManager.ObtenerSugerenciaCupoAgrupadasPorProveedor(GlobalVariables.ComercialId, materialId, centroId);
            ViewBag.Lista = lista;
            ViewBag.Fechas = cupoManager.FechasComprendidas();
            var material = oMaterialManager.TraerTodoMaterial();

            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = x.MaterialId == materialId
                    }).OrderBy(x => x.Value);
            ViewBag.MatLista = new SelectList(materialesListItems, "Value", "Text", materialId);

            var centros = centroManager.TraerTodoCentro();
            var listaCentro = new List<SelectListItem>();
            foreach (var i in centros.Centro)
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == centroId ? true : false
                });
            }
            ViewBag.CenLista = new SelectList(listaCentro.OrderBy(x => x.Value),"Value","Text", centroId);
        }
        public ActionResult DatosConfiguracion(KendoGridMvcRequest request)
        {
            var lista = cupoManager.ObtenerSugerenciaCupo(GlobalVariables.ComercialId);
            ViewBag.Lista = lista;
            ViewBag.Fechas = cupoManager.FechasComprendidas();
            var result = new KendoGrid<SugerenciaCupoDto>(request, lista);

            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public JsonResult Aceptar(List<SugerenciaCupoDto> sugerencias)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (sugerencias != null)
            {
                resultado = cupoManager.AceptarSugerenciaCupo(sugerencias);
            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Sugerencia."));
                resultado.Add(error);
            }
            return Json(resultado);
        }

        public JsonResult Rechazar(List<int> ids,string motivo)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (ids != null)
            {
                cupoManager.RechazarSugerenciaCupo(ids, motivo);

            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Sugerencia."));
                resultado.Add(error);
            }
            return Json(resultado);
        }
        public JsonResult DatosConfirmar(List<ConfirmacionSugerenciaCupoDto> datosTabla, int materialId, string centroId)
        {            
            CupoResult resultado = new CupoResult();
            if (datosTabla != null)
            {
                resultado = cupoManager.ConfirmarSugerencia(datosTabla, materialId, centroId);
            }
            else
            {
                resultado.Errores.Add(new ErrorMessage(400, "No se seleccionó ninguna sugerencia"));
            }
            return Json(resultado);
        }        
    }
} 