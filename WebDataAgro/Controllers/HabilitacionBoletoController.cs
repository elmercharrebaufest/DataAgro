using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class HabilitacionBoletoController : Controller
    {
        private readonly IHabilitacionBoletoManager habilitacionBoletoManager;
        private readonly ITipoNegocioManager tipoNegocioManager;
        private readonly IProvinciaManager provinciaManager;

        public HabilitacionBoletoController(IHabilitacionBoletoManager habilitacionBoletoManager, ITipoNegocioManager tipoNegocioManager, IProvinciaManager provinciaManager)
        {
            this.habilitacionBoletoManager = habilitacionBoletoManager;
            this.tipoNegocioManager = tipoNegocioManager;
            this.provinciaManager = provinciaManager;
        }

        public ActionResult Index()
        {
            var viewModel = CompletarViewModel();

            return View("Index", viewModel);
        }

        private HabilitacionBoletoViewModel CompletarViewModel()
        {
            return new HabilitacionBoletoViewModel
            {
                TipoNegocioDetalle = habilitacionBoletoManager.ListarTipoNegociosDetalle().OrderBy(x => x.TipoNegocioDescripcion).ThenBy(x => x.Descripcion).ToList(),
                BoletoCompraNetProvincia = habilitacionBoletoManager.ListarBoletoCompraNetProvincia().OrderBy(x => x.ProvinciaNombre).ThenBy(x => x.BoletoDescripcion).ToList(),
                TipoNegocio = tipoNegocioManager.TraerTodoTipoNegocio().Select(x => new SelectListItem
                {
                    Text = x.Descripcion,
                    Value = x.TipoNegocioId.ToString(),
                    Selected = false
                }).OrderBy(x => x.Value),
                ProvinciaList = provinciaManager.ListarProvincia("").Select(x => new SelectListItem
                {
                    Value = x.ProvinciaId.ToString(),
                    Text = x.Nombre
                }).OrderBy(x => x.Text),
                TipoBoleto = habilitacionBoletoManager.ListarBoletoCompraNet().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Descripcion
                })
            };
        }

        #region TipoNegocioDetalle
        [HttpPost]
        public ActionResult AgregarTipoNegocioDetalle(HabilitacionBoletoModel modelo)
        {
            var resultado = habilitacionBoletoManager.AgregarTipoNegocioDetalle(ConvertirModeloADto(modelo));

            if (resultado.HayError)
            {
                return Json(new { success = false, message = resultado.Errores.First().Message });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public ActionResult ActualizarTipoNegocioDetalle(HabilitacionBoletoModel modelo)
        {
            if (modelo != null)
            {
                habilitacionBoletoManager.ActualizarTipoNegocioDetalle(modelo.TipoNegocioDetalles);
            }

            var viewModel = CompletarViewModel();

            return PartialView("_ListaTipoNegocioDetalle", viewModel.TipoNegocioDetalle);
        }

        [HttpPost]
        public ActionResult RecargarTipoNegocioDetalle()
        {
            var viewModel = CompletarViewModel();
            return PartialView("_ListaTipoNegocioDetalle", viewModel.TipoNegocioDetalle);
        }

        private TipoNegocioDetalleDto ConvertirModeloADto(HabilitacionBoletoModel modelo)
        {
            return new TipoNegocioDetalleDto()
            {
                TipoNegocioId = modelo.TipoNegocioId,
                Descripcion = modelo.Descripcion
            };
        }
        #endregion

        #region BoletoCompraNetProvincia
        public ActionResult ActualizarBoletoCompraNetProvincia()
        {
            var viewModel = CompletarViewModel();
            return PartialView("_ListaBoletoCompraNetProvincia", viewModel.BoletoCompraNetProvincia);
        }

        [HttpPost]
        public ActionResult AgregarBoletoCompraNetProvincia(BoletoCompraNetProvinciaModel model)
        {
            BoletoCompraNetProvinciaDto dto = new BoletoCompraNetProvinciaDto { BoletoCompraNetId = model.BoletoCompraNetId, ProvinciaId = model.ProvinciaId };
            var resultado = habilitacionBoletoManager.AgregarBoletoCompraNetProvincia(dto);

            if (resultado.HayError)
            {
                return Json(new { success = false, message = resultado.Errores.First().Message });
            }
            else
            {
                return Json(new { success = true });
            }
        }

        [HttpPost]
        public ActionResult EliminarBoletoCompraNetProvincia(int id)
        {
            var resultado = habilitacionBoletoManager.EliminarBoletoCompraNetProvincia(id);

            if (resultado.HayError)
            {
                return Json(new { success = false, message = resultado.Errores.First().Message });
            }
            else
            {
                return Json(new { success = true });
            }
        }
        #endregion
    }
}
