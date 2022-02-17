using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class HabilitacionBoletoController : Controller
    {
        private readonly IHabilitacionBoletoManager habilitacionBoletoManager;
        private readonly ITipoNegocioManager tipoNegocioManager;

        public HabilitacionBoletoController(IHabilitacionBoletoManager habilitacionBoletoManager, ITipoNegocioManager tipoNegocioManager)
        {
            this.habilitacionBoletoManager = habilitacionBoletoManager;
            this.tipoNegocioManager = tipoNegocioManager;
        }
        // GET: HabilitacionBoleto
        public ActionResult Index()
        {
            CompletarVista();
            return View("Index", new HabilitacionBoletoModel());
        }
        [HttpPost]
        public ActionResult AgregarTipoNegocioDetalle(HabilitacionBoletoModel modelo)
        {
            CompletarVista();
            if (!ModelState.IsValid)
            {
                return PartialView("AgregarModal", modelo);
            }
            habilitacionBoletoManager.AgregarHabilitacionBoleto(ConvertirModeloADto(modelo));
            return Json(new { success = true });
        }
        [HttpPost]
        public ActionResult ActualizarTipoNegocioDetalle(HabilitacionBoletoModel modelo)
        {
            if (modelo != null)
            {
                habilitacionBoletoManager.ActualizarTipoNegocioDetalle(modelo.TipoNegocioDetalles);
            }
            CompletarVista();
            return PartialView("_ListaHabilitados", modelo);
        }

        [HttpPost]
        public ActionResult RecargarPantalla(HabilitacionBoletoModel modelo)
        {
            CompletarVista();
            return PartialView("_ListaHabilitados", modelo);
        }

        private void CompletarVista()
        {
            ViewBag.ListaHabilitacion = habilitacionBoletoManager.ListarTipoNegociosDetalle();

            var negocios = tipoNegocioManager.TraerTodoTipoNegocio();
            var negocioListItems = negocios.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.TipoNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.TipoNegocio = negocioListItems;
        }

        private TipoNegocioDetalleDto ConvertirModeloADto(HabilitacionBoletoModel modelo)
        {
            return new TipoNegocioDetalleDto()
            {
                TipoNegocioId = modelo.TipoNegocioId,
                Descripcion = modelo.Descripcion
            };


        }
    }
}
