using DiffPlex.DiffBuilder;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections;
using System.Text;
using System.Text.RegularExpressions;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.LogDataAgro)]
    public class LogDataAgroController : Controller
    {
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IReportesManager reportesManager;       
        private readonly IComercialManager mobjComercialManager;
        private readonly IProveedorManager proveedorManager;


        public LogDataAgroController(ILogDataAgroManager logDataAgroManager, IReportesManager reportesManager, IComercialManager mobjComercialManager, IProveedorManager proveedorManager)
        {
            this.logDataAgroManager = logDataAgroManager;
            this.reportesManager = reportesManager;
            this.mobjComercialManager = mobjComercialManager;
            this.proveedorManager = proveedorManager;
        }

        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }
        [HttpPost]
        public ActionResult BuscarDatosLogDataAgro(DataSourceRequest request)
        {
            if (request.Sort == null)
            {
                request.Sort = new List<Sort> {
                    new Sort {Field= "Fecha",Dir="desc" }
                    };
            }
            if (request.Filter != null && request.Filter.Filters != null)
            {
                foreach (var item in request.Filter.Filters)
                {
                    if (item.Field == "Fecha" && item.Operator == "lte")
                    {
                        item.Value = Convert.ToDateTime(item.Value).AddDays(1);
                    }
                }
            }
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var model = logDataAgroManager.ListarDatosLogDataAgro(request, equipo);
            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        [HttpPost]
        public ActionResult MostrarDiferencias(int idLogDataAgro)
        {
            SideBySideDiffBuilder diffBuilder = new SideBySideDiffBuilder();
            LogDataAgroDto actual = logDataAgroManager.Obtener(idLogDataAgro);
            LogDataAgroDto anterior = logDataAgroManager.Obtener(idLogDataAgro, true);
            var model = diffBuilder.BuildDiffModel(anterior.DatoModificado ?? string.Empty, actual.DatoModificado ?? string.Empty);

            foreach (var diffLine in model.NewText.Lines)
            {
                FormatearTexto(diffLine);
            }
            foreach (var diffLine in model.OldText.Lines)
            {
                FormatearTexto(diffLine);
            }
            List<string> tipos = new List<string> { "BasicoContrato", "StoredPorProveedorResult", "CupoDto" };
            if (tipos.Contains(actual.Tipo))
            {
                ViewBag.Mensaje = actual.AccionRealizada + " " + actual.Clase + ": " + actual.Descripcion;

            }
            return PartialView("Diff", model);
        }
        [HttpPost]
        public ActionResult MostrarDiferenciasTabla(int idLogDataAgro)
        {
            LogDataAgroDto actual = logDataAgroManager.Obtener(idLogDataAgro);
            DatosModificadosLogDataAgroDto CamposCambiados = logDataAgroManager.TraerDatosModificadosPorId(idLogDataAgro);

            List<string> tipos = new List<string> { "BasicoContrato", "StoredPorProveedorResult", "CupoDto" };
            if (tipos.Contains(actual.Tipo))
            {
                ViewBag.Mensaje = actual.AccionRealizada + " " + actual.Clase + ": " + actual.Descripcion;

            }
            return PartialView("_MostrarDiferenciasTabla", CamposCambiados);
        }

        public JsonResult BuscarProveedor(string text)
        {
            var proveedores = proveedorManager.DevolverProveedoresCorredores(text);

            return Json(proveedores.Select(x => new { ProveedorId = x.Id, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Export(DataSourceRequest filtro)
        {
            var model = new ReportesModel();

            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field= "Id",Dir="desc" }
                };
            }
            if (filtro.Filter != null && filtro.Filter.Filters != null)
            {
                foreach (var item in filtro.Filter.Filters)
                {
                    if (item.Field == "Fecha" && item.Operator == "lte")
                    {
                        item.Value = Convert.ToDateTime(item.Value).AddDays(1);
                    }
                }
            }
            filtro.Skip = 0;
            filtro.Take = 0;
            var equipo = GlobalVariables.EquipoReal;
            List<LogDataAgroDto> datos = (List<LogDataAgroDto>)logDataAgroManager.ListarDatosLogDataAgro(filtro, equipo).Data;
            foreach (var dato in datos)
            {
                dato.CamposCambiados = logDataAgroManager.TraerDatosModificadosPorId(dato.Id).CamposCambiados;
            }
            var oLstContacto = new LstLogDataAgro(reportesManager);

            var identif = oLstContacto.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        private void FillViewBag()
        {
            var accionesListItems = new List<SelectListItem>();
            foreach (var nombreDelEnum in Enum.GetNames(typeof(TipoAccionLogDataAgro)))
            {
                accionesListItems.Add(new SelectListItem
                {
                    Text = nombreDelEnum,
                    Value = nombreDelEnum,
                    Selected = false
                });
            }
            accionesListItems.OrderBy(x => x.Text);
            ViewBag.Accion = accionesListItems;

            var clasesListItems = new List<SelectListItem>{
                new SelectListItem
                    {
                        Text = "Cupo",
                        Value = "Cupo",
                        Selected = false
                    },
                new SelectListItem
                    {
                        Text = "Proveedor",
                        Value = "Proveedor",
                        Selected = false
                    },
                new SelectListItem
                    {
                        Text = "Negocio",
                        Value = "Negocio",
                        Selected = false
                    },
                new SelectListItem
                    {
                        Text = "RangoConfirmacionAutomatica",
                        Value = "RangoConfirmacionAutomatica",
                        Selected = false
                    },
                 new SelectListItem
                    {
                        Text = "PrecioMoa",
                        Value = "PrecioMoa",
                        Selected = false
                    },
                 //new SelectListItem
                 //   {
                 //       Text = "HabilitacionFijacion",
                 //       Value = "HabilitacionFijacion",
                 //       Selected = false
                 //   },
                 new SelectListItem
                    {
                        Text = "HabilitacionPizarra",
                        Value = "HabilitacionPizarra",
                        Selected = false
                    },new SelectListItem
                    {
                        Text = "HabilitacionCampaña",
                        Value = "HabilitacionCampaña",
                        Selected = false
                    },
            }.OrderBy(x => x.Text);
            ViewBag.Clase = clasesListItems;


            var comercial = mobjComercialManager.TraerTodoComercial();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Nombres + " " + x.Apellido,
                   Value = x.Nombres + " " + x.Apellido,
                   Selected = false
               }).OrderBy(x => x.Text);
            ViewBag.Comercial = comercialListItems;
        }

        public void FormatearTexto(DiffPlex.DiffBuilder.Model.DiffPiece diffLine)
        {
            if (diffLine.Text != null)
            {
                //diffLine.Text = diffLine.Text.Replace("\"", "").Replace("_", "");
                diffLine.Text = logDataAgroManager.BuscaFechaYFormatea(diffLine.Text,"");
                diffLine.Text = logDataAgroManager.AddSpacesToSentence(diffLine.Text, ':');

                foreach (var character in diffLine.SubPieces)
                {
                    if (character.Text != null)
                    {
                        character.Text = logDataAgroManager.BuscaFechaYFormatea(character.Text,"");
                        character.Text = logDataAgroManager.AddSpacesToSentence(character.Text, ':');
                    }

                }
            }

        }

        public JsonResult ObtenerNegociosId(List<string> contratosSap)
        {
           
            List<int> negocios = logDataAgroManager.ObtenerNegociosId(contratosSap);

            return Json(negocios, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ObtenerCuposId(List<string> cupoSap)
        {

            List<int> cupos = logDataAgroManager.ObtenerCuposId(cupoSap);

            return Json(cupos, JsonRequestBehavior.AllowGet);
        }
        
    }
}