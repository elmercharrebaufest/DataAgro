using Autofac.Extras.NLog;
using Molinos.DataAgro.Interfaces;
using System.Web.Mvc;
using System;
using static WebDataAgro.MvcApplication;
using System.Collections.Generic;
using KendoGridBinder.ModelBinder.Mvc;
using KendoGridBinder;
using Molinos.DataAgro.Entities.Dto;
using System.Linq;
using WebDataAgro.Atributos;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Entities.Entities;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class SugerenciaCupoController : Controller
    {
        private readonly ILogger logger;
        private readonly ICupoManager cupoManager;
        private readonly IMaterialManager oMaterialManager;
        private readonly ICentroManager centroManager;
        private readonly IFormulaManager formulaManager;
        private readonly IComercialManager comercialManager;

        public SugerenciaCupoController(ILogger logger, ICentroManager centroManager, ICupoManager cupoManager, IMaterialManager oMaterialManager,
            IFormulaManager formulaManager, IComercialManager comercialManager)
        {
            this.centroManager = centroManager;
            this.logger = logger;
            this.cupoManager = cupoManager;
            this.oMaterialManager = oMaterialManager;
            this.formulaManager = formulaManager;
            this.comercialManager = comercialManager;
        }

        [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
        public ActionResult Index(int? materialId = null, string centroId = "1029", int? ComercialSeleccionado = null, bool muestraModal = false)
        {
            ComercialSeleccionado = ComercialSeleccionado ?? GlobalVariables.ComercialId;
            CargarVista(materialId, centroId, ComercialSeleccionado.Value, muestraModal);
            return View();
        }
        public ActionResult PartialTabla(int materialId, string centroId, int? ComercialSeleccionado2, bool muestraModal = false)
        {
            ComercialSeleccionado2 = ComercialSeleccionado2 ?? GlobalVariables.ComercialId;
            CargarVista(materialId, centroId, ComercialSeleccionado2.Value);
            return PartialView();
        }

        private void CargarVista(int? materialId = null, string centroId = "1029", int ComercialSeleccionado = 0, bool muestraModal = false)
        {
            if (!PermisosHelper.Is(PermisosDataAgro.VerTodasLasSugerencias))
            {
                ComercialSeleccionado = GlobalVariables.ComercialId;
            }
            //var lista = cupoManager.ObtenerSugerenciaCupoAgrupadasPorProveedor(ComercialSeleccionado, materialId, centroId);

            var material = oMaterialManager.TraerTodoMaterial();

            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = x.MaterialId == materialId
                    }).OrderBy(x => x.Value);
            ViewBag.MatLista = new SelectList(materialesListItems, "Value", "Text", materialId);
            ViewBag.MuestraModal = muestraModal;
            var centros = centroManager.TraerTodoCentro();
            var listaCentro = new List<SelectListItem>();
            foreach (var i in centros.Centro.Where(x => x.CargaCupos == true && x.Orden != null && x.Descripcion.Contains("SUSTENTABLE") == false).OrderBy(y => y.Orden))
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == centroId
                });
            }
            foreach (var i in centros.Centro.Where(x => x.CargaCupos == true && x.Orden == null && x.Descripcion.Contains("SUSTENTABLE") == false).OrderBy(y => y.Descripcion))
            {
                listaCentro.Add(new SelectListItem
                {
                    Text = i.Descripcion,
                    Value = i.CodigoSap.ToString(),
                    Selected = i.CodigoSap == centroId
                });
            }
            ViewBag.CenLista = new SelectList(listaCentro, "Value", "Text", centroId);
            var primerCierre = cupoManager.DevolverTodoCierreCupera().FirstOrDefault();
            ViewBag.Cierre = primerCierre != null ? primerCierre.Cierre : false;

            var equipo = GlobalVariables.EquipoReal;
            var comerciales = comercialManager.ListarComercialesAsignanNegocios();
            var comercialesListItems = comerciales.Select(
                    x => new SelectListItem
                    {
                        Text = x.Comercial,
                        Value = x.ComercialId.ToString(),
                        Selected = x.ComercialId == ComercialSeleccionado
                    }).OrderBy(x => x.Text).ToList();
            ViewBag.Comercial = new SelectList(comercialesListItems, "Value", "Text", ComercialSeleccionado);

            ViewBag.ComercialSeleccionado = new SelectList(comercialesListItems, "Value", "Text", ComercialSeleccionado);
            if (!PermisosHelper.Is(PermisosDataAgro.VerTodasLasSugerencias))
            {
                ViewBag.ComercialSeleccionado = new SelectList(comercialesListItems.Where(a => a.Value == GlobalVariables.ComercialId.ToString()).ToList(), "Value", "Text", ComercialSeleccionado);
            }

            var calidades = new List<SelectListItem>() { new SelectListItem { Text = "Camara", Value = "1",Selected =false},
                new SelectListItem { Text = "Fabrica", Value = "2",Selected =true } };
            ViewBag.Calidad = new SelectList(calidades, "Value", "Text", 2);
            ViewBag.Fechas = new List<DateTime>();

            if (materialId.HasValue)
            {
                var lista = cupoManager.ObtenerSugerenciaCupo(ComercialSeleccionado, materialId);
                var sugerenciaPorComercial = cupoManager.ObtenerSugerenciaPorComercialFecha(ComercialSeleccionado, materialId.Value, centroId);
                ViewBag.Sugerencia = sugerenciaPorComercial;
                ViewBag.Lista = lista;
                ViewBag.Material = materialId;
                ViewBag.Fechas = cupoManager.FechasComprendidas(materialId);
                ViewBag.Mensajes = cupoManager.MostrarDetalle(ComercialSeleccionado, centroId, materialId.Value);
            }
        }
        public ActionResult DatosConfiguracion(KendoGridMvcRequest request, int? ComercialId)
        {
            ComercialId = ComercialId ?? GlobalVariables.ComercialId;
            var lista = cupoManager.ObtenerSugerenciaCupo(ComercialId.Value, null);
            ViewBag.Lista = lista;
            ViewBag.Fechas = cupoManager.FechasComprendidas(null);
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

        public JsonResult Rechazar(List<int> ids, string motivo)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (ids != null)
            {
                resultado.Add(cupoManager.RechazarSugerenciaCupo(ids, motivo));
            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Sugerencia."));
                resultado.Add(error);
            }
            return Json(resultado);
        }
        public JsonResult DatosConfirmar(List<ConfirmacionSugerenciaCupoDto> datosTabla, List<DiaCupo> devoluciones, int materialId, string centroId, int? comercialId)
        {
            var comercialSeleccionado = comercialId ?? GlobalVariables.ComercialId;
            //CupoResult resultado = new CupoResult();
            if (datosTabla != null || devoluciones != null)
            {
                cupoManager.ConfirmarSugerencia(datosTabla, devoluciones, materialId, centroId, comercialSeleccionado);
            }
            return Json("Ok", JsonRequestBehavior.AllowGet);
        }

        public JsonResult GenerarSolicitudExtraordinaria(AdministracionCupoDto solicitud)
        {
            var resultado = cupoManager.GenerarSolicitudExtraordinaria(solicitud);

            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AceptarSugerenciaCupo(AceptarSugerenciaCupoDto item)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            if (item.cantidad > 0 || item.cantidadFleteProcedencia > 0)
            {
                if (item.idSugerencia > 0)
                {
                    resultado = cupoManager.AceptarSugerenciaCupo(item.idSugerencia, item.cantidad, item.cantidadFleteProcedencia);
                }
                else if (item.materialId != null)
                {
                    resultado = cupoManager.AceptarSugerenciaCupoPorProveedor(item.proveedorId, item.materialId.Value, item.fecha.Value, item.cantidad, item.cantidadFleteProcedencia, item.comercialId, item.centroId);
                }
            }
            else
            {
                CupoResult error = new CupoResult();
                error.Errores.Add(new ErrorMessage(400, "No selecciono ninguna Cantidad."));
                resultado.Add(error);
            }
            return Json(resultado);
        }

        public JsonResult ModificarSugerenciaCupo(List<AceptarSugerenciaCupoDto> items)
        {
            List<CupoResult> resultado = new List<CupoResult>();

            if (items.First().idSugerencia > 0)
            {
                resultado = cupoManager.ModificarSugerenciaCupo(items.OrderBy(a => a.fecha).ToList());
            }
            else
            {
                resultado = cupoManager.ModificarSugerenciaCupoPorProveedor(items.OrderBy(a => a.fecha).ToList());
            }

            return Json(resultado);
        }

        public JsonResult DevolverSugerenciasMasivo(List<DevolucionSugerenciaCupoDto> sugerenciasADevolver)
        {
            CupoResult resultado = cupoManager.DevolverSugerenciasMasivo(sugerenciasADevolver);
            return Json(resultado);
        }

    }
}