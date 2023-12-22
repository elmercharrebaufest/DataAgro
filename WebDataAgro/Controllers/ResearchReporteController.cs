using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Report;
using System;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ResearchReporteController : Controller
    {
        private readonly IResearchManager researchManager;
        private readonly IMaterialManager materialManager;
        private readonly ICampañaManager campañaManager;
        private readonly IComercialManager comercialManager;
        private readonly IProvinciaManager provinciaManager;

        public ResearchReporteController(IResearchManager researchManager, IMaterialManager materialManager, ICampañaManager campañaManager, IComercialManager comercialManager, IProvinciaManager provinciaManager)
        {
            this.researchManager = researchManager;
            this.materialManager = materialManager;
            this.campañaManager = campañaManager;
            this.comercialManager = comercialManager;
            this.provinciaManager = provinciaManager;
        }

        [Autorizacion(PermisosDataAgro.ReporteResearch)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field= "Material",Dir="desc" }
                };
            }

            DataSourceResult model = researchManager.BuscaDatosTabla(filtro);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }
        
        private void FillViewBag()
        {

            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);

            var campania = campañaManager.TraerTodoCampania().Where(x => x.CampañaId >= 6).ToList();
            var campaniaListItems = campania.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.CampañaId.ToString(),
                Selected = false
            }).OrderBy(x => x.Text);

            var comercial = comercialManager.TraerTodoComercial();
            comercial.Comercial = comercial.Comercial.Where(a => (a.Rol.ToUpper().Contains("Comercial".ToUpper()) || a.Rol.ToUpper().Contains("Comercial corredor".ToUpper()) || a.Rol.ToUpper().Contains("Mesa".ToUpper())) && a.Deshabilitado != true).ToList();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Nombres + " " + x.Apellido,
                   Value = x.ComercialId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);

            var condicion = researchManager.TraerResearchCondicion();
            var condicionListItems = condicion.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.CondicionId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);

            var estadio = researchManager.TraerResearchEstadio();
            var estadioListItems = estadio.Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.EstadioId.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);

            var tipoCarga = researchManager.TraerResearchTipoCarga();
            var tipoCargaListItems = tipoCarga.Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.TipoCargaId.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);

            var tipoMuestra = researchManager.TraerResearchTipoMuestra();
            var tipoMuestraListItems = tipoMuestra.Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.TipoMuestraId.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);

            var humedadSuelo = researchManager.TraerResearchHumedadSuelo();
            var humedadSueloListItems = humedadSuelo.Select(
                   x => new SelectListItem
                   {
                       Text = x.Descripcion,
                       Value = x.HumedadSueloId.ToString(),
                       Selected = false
                   }).OrderBy(x => x.Value);

            var provincia = provinciaManager.ListarProvincia("");
            var provinciaListItems = provincia.Select(
                   x => new SelectListItem
                   {
                       Text = x.Nombre,
                       Value = x.ProvinciaId.ToString(),
                       Selected = false
                   });


            ViewBag.Condicion = condicionListItems;
            ViewBag.Estadio = estadioListItems;
            ViewBag.TipoCarga = tipoCargaListItems;
            ViewBag.TipoMuestra = tipoMuestraListItems;
            ViewBag.HumedadSuelo = humedadSueloListItems;
            ViewBag.Material = materialesListItems;
            ViewBag.Campania = campaniaListItems;
            ViewBag.Comercial = comercialListItems;
            ViewBag.Provincia = provinciaListItems;

        }
        
        public ActionResult Export(DataSourceRequest filtro)
        {
            var model = new ReportesModel();

            //if (filtro.Sort == null)
            //{
            //    filtro.Sort = new List<Sort> {
            //        new Sort {Field= "Material",Dir="desc" }
            //    };
            //}
            //filtro.Skip = 0;
            //filtro.Take = 0;
            //var equipo = GlobalVariables.EquipoReal;
            //List<BasicoContrato> datos = (List<BasicoContrato>)researchManager.TraerContratosFiltrados(filtro, equipo).Data;


            //var oLstContacto = new LstContrato(reportesManager);
            //reportesManager.TraerPosicionNegocios(datos);
            //foreach (var negocio in datos)
            //{
            //    if (negocio.TipoNegocioId == 1 || negocio.TipoNegocioId == 2)
            //    {
            //        negocio.Descuentos = mobjContratoManager.TraerDescuentosPorContrato(negocio.Id);
            //    }
            //    if (negocio.TipoNegocioId == 3 && (negocio.ImporteComision ?? 0) == 0 && (negocio.PorcentajeComision ?? 0) == 0)
            //    {
            //        fijacionDePrecioContratoManager.BuscarComision(negocio);
            //    }
            //}
            //var identif = oLstContacto.GenerarExcel(datos);

            //model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        [HttpPost]
        public ActionResult BorrarRegistro(int id)
        {
            return new JsonResult()
            {
                Data = researchManager.BorrarResearch(id),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}