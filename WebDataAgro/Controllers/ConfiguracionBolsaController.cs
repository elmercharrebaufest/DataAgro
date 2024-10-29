using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ConfiguracionBolsaController : Controller
    {

        private readonly IContratoManager contratoManager;
        private readonly IConfiguracionBolsaManager configuracionBolsaManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ConfiguracionBolsaController(IContratoManager contratoManager,
            IConfiguracionBolsaManager configuracionBolsaManager)
        {
            this.contratoManager = contratoManager;
            this.configuracionBolsaManager = configuracionBolsaManager;
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------
        [Autorizacion(PermisosDataAgro.ConfiguracionBolsa)]
        public ActionResult Index()
        {

            ViewBag.comercialId = GlobalVariables.ComercialId;
            CargarViewBag();
            return View();
        }
        public ActionResult TablaBolsaPartial()
        {
            var model = new ConfiguracionBolsaModel { Resultado = new Resultado() };
            return PartialView("_ListaBolsa", model);
        }
        [HttpPost]
        public ActionResult GrabarConfiguracionBolsa(ConfiguracionBolsaModel bolsa)
        {
            CargarViewBag();
            if (!ModelState.IsValid)
            {
                return View("Index", bolsa);
            }
            var resultado = configuracionBolsaManager.GrabarConfiguracionBolsa(new ConfiguracionBolsa
            {
                Id = bolsa.Id,
                ProvinciaId = bolsa.ProvinciaId,
                DestinoId = bolsa.DestinoId,
                BolsaId = bolsa.BolsaId
            });
            if (resultado.HayError)
            {
                foreach (var e in resultado.Errores)
                {
                    ModelState.AddModelError("400", e.Message);
                }
                return View("Index", bolsa);
            }
            return RedirectToAction("Index");
        }
        public ActionResult DatosConfiguracion(KendoGridMvcRequest request)
        {
            var model = configuracionBolsaManager.TraerTodaConfiguracionBolsa(request);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        private void CargarViewBag()
        {
            var datosCombo = contratoManager.TraerDatosCombo();

            var destino = datosCombo.Destino;
            var destinoListItems = destino.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Destino = destinoListItems;
            var provincia = datosCombo.prov;
            var provinciaListItems = provincia.Select(
                    x => new SelectListItem
                    {
                        Text = x.Nombre,
                        Value = x.Provinciaid.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Provincia = provinciaListItems;
            var bolsa = datosCombo.Bolsa;
            var bolsaListItems = bolsa.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Bolsa = bolsaListItems;
        }

        public ActionResult EditarConfiguracionBolsa(int id)
        {
            var bolsa = configuracionBolsaManager.TraerConfiguracionBolsa(id);
            return new JsonResult() { Data = bolsa, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult EliminarConfiguracionBolsa(int id)
        {
            var bolsa = configuracionBolsaManager.EliminarConfiguracionBolsa(id);
            return new JsonResult() { Data = bolsa, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult TraerConfiguracionBolsaConDestinoYProcedencia(int destinoId, int provinciaId)
        {
            var bolsa = configuracionBolsaManager.TraerConfiguracionBolsaConDestinoYProcedencia(destinoId, provinciaId);
            if (bolsa != null)
            {
                return new JsonResult() { Data = bolsa, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
            }
            return new JsonResult() { Data = 0, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

    }
}