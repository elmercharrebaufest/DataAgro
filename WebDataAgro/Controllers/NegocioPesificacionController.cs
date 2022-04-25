using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class NegocioPesificacionController : Controller
    {
        private readonly IProveedorManager proveedorManager;
        private readonly IReportesManager reportesManager;
        private readonly ICentroManager centroManager;
        private readonly IMaterialManager materialManager;
        private readonly IComercialManager comercialManager;
        private readonly IContratoManager contratoManager;
        private readonly ICampañaManager campañaManager;
        private readonly ILogger logger;

        public NegocioPesificacionController(IProveedorManager proveedorManager, IReportesManager reportesManager, ICentroManager centroManager,
            IMaterialManager materialManager, IComercialManager comercialManager, IContratoManager contratoManager,
            ICampañaManager campañaManager, ILogger logger)
        {
            this.proveedorManager = proveedorManager;
            this.reportesManager = reportesManager;
            this.centroManager = centroManager;
            this.materialManager = materialManager;
            this.comercialManager = comercialManager;
            this.contratoManager = contratoManager;
            this.campañaManager = campañaManager;
            this.logger = logger;
        }

        // GET: ReportePesificados
        public ActionResult Index()
        {
            CargarView();
            return View();
        }

        private void CargarView()
        {
            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var comercial = comercialManager.TraerTodoComercial();
            comercial.Comercial = comercial.Comercial.Where(a => (a.Rol.ToUpper().Contains("Comercial".ToUpper()) || a.Rol.ToUpper().Contains("Comercial corredor".ToUpper()) || a.Rol.ToUpper().Contains("Mesa".ToUpper())) && a.Deshabilitado != true).ToList();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Nombres + " " + x.Apellido,
                   Value = x.ComercialId.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Comercial = comercialListItems;

            var clasificacion = contratoManager.TraerDatosCombo();
            var clasificacionListItems = clasificacion.Clasificacion.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Clasificacion = clasificacionListItems;

        }
        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field= "MaterialDesc", Dir="desc" }
                };
            }
            if(filtro.Filter != null)
            {
                foreach(var item in filtro.Filter.Filters)
                {
                    if(item.Field == "Clasificacion")
                    {
                        if(item.Value.ToString() == "1")
                        {
                            item.Value = "PRODUCTOR";
                        }
                        if (item.Value.ToString() == "2")
                        {
                            item.Value = "ACOPIADOR";
                        }
                        if (item.Value.ToString() == "3")
                        {
                            item.Value = "OTROS";
                        }
                    }
                    if(item.Field == "Cesion")
                    {
                        if (item.Value.ToString() == "1")
                        {
                            item.Value = true;
                        }
                        if (item.Value.ToString() == "0")
                        {
                            item.Value = false;
                        }
                        if (item.Value.ToString() == "")
                        {
                            item.Value = true;                            
                        }
                    }
                    if (item.Field == "Excepcion")
                    {
                        if (item.Value.ToString() == "1")
                        {
                            item.Value = true;
                        }
                        if (item.Value.ToString() == "0")
                        {
                            item.Value = true;
                            item.Operator = "neq";
                        }
                    }
                    if (item.Field == "ConFechaInstruccion")
                    {
                        if (item.Value.ToString() == "1")
                        {
                            item.Value = true;
                        }
                        if (item.Value.ToString() == "0")
                        {
                            item.Value = false;
                        }
                    }
                }
            }


            var equipo = GlobalVariables.EquipoReal;
            var model = reportesManager.BuscarDatosNegocioPesificacion(filtro, equipo);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult ConfigurarExcedente(int id, bool excedente)
        {
            var model = reportesManager.ConfigurarExcedente(id, excedente, GlobalVariables.ComercialId);
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult EnviarMail(List<int> ids, DateTime fecha)
        {
            var mensaje = "OK";
            if (ids != null && ids.Count > 0)
            {
                reportesManager.EnviarMail(ids, fecha, GlobalVariables.ComercialId);
            }
            else
            {
                mensaje = "Error";
            }           
            return new JsonResult()
            {
                Data = mensaje,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
