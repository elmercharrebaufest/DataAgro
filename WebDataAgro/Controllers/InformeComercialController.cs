using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class InformeComercialController : Controller
    {
        private readonly ICondicionManager mobjCondicionManager;
        private readonly IInformeComercialManager mobjInformeComercialManager;
        private readonly IHomeManager mobjHomeManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly IMaterialManager mobjMaterialManager;
        private readonly ICampañaManager mobjCampaniaManager;
        private readonly IReportesManager reportesManager;

        public InformeComercialController(ICondicionManager oCondicionManager, IInformeComercialManager oInformeComercialManager, IHomeManager oHomeManager, IReportesManager reportesManager,
            IComercialManager mobjComercialManager, IMaterialManager mobjMaterialManager, ICampañaManager mobjCampaniaManager)
        {
            mobjCondicionManager = oCondicionManager;
            mobjInformeComercialManager = oInformeComercialManager;
            mobjHomeManager = oHomeManager;
            this.mobjMaterialManager = mobjMaterialManager;
            this.mobjCampaniaManager = mobjCampaniaManager;
            this.mobjComercialManager = mobjComercialManager;
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

        [Autorizacion(PermisosDataAgro.VisualizarInformeComercial)]
        public ActionResult Index()
        {
            return View();
        }

        private void FillViewBag()
        {
            var material = mobjMaterialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var campania = mobjCampaniaManager.TraerTodoCampania().Where(x => x.CampañaId >= 6).ToList();
            var campaniaListItems = campania.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.Descripcion,
                Selected = false
            }).OrderByDescending(x => x.Text);
            ViewBag.Campania = campaniaListItems;

            var comercial = mobjComercialManager.TraerTodoComercial();
            comercial.Comercial = comercial.Comercial.Where(a => (a.Rol.ToUpper().Contains("Comercial".ToUpper()) || 
                                                                  a.Rol.ToUpper().Contains("Comercial corredor".ToUpper()) || 
                                                                  a.Rol.ToUpper().Contains("Mesa".ToUpper())) && a.Deshabilitado != true).ToList();
            var comercialListItems = comercial.Comercial.Select(
               x => new SelectListItem
               {
                   Text = x.Apellido + " " + x.Nombres,
                   Value = x.ComercialId.ToString(),
               }).OrderBy(x => x.Text);
            ViewBag.Comercial = comercialListItems;

        }

        [Autorizacion(PermisosDataAgro.VisualizarInformeAdministrativo)]
        public ActionResult InformeAdministrativo()
        {
            FillViewBag();
            return View();
        }

        public ActionResult Reporte()
        {
            return View();
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniCondicionModel();

            var result = mobjCondicionManager.TraerTodoCondicion();

            if (result != null)
            {
                model.Datos = result.Condicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarInformeComercial(Condicion oCondicion)
        {
            var model = new AbmCondicionResult();

            var entityErrors = mobjCondicionManager.GrabarCondicion(oCondicion);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.Condicion = oCondicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> Listar(ParamInformeComercial oParam, int? ComercialId, List<NuevoProduccion> nuevosCampos,
            List<NuevoAcopio> nuevosAcopios, ContactoComercial contactoComercial, string direccion, string codigoPostal, int? localidadId)
        {
            var model = new ReportesModel();
            if (!ComercialId.HasValue)
            {
                ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            }
            oParam.ComercialId = ComercialId.Value;
            var entityError = mobjInformeComercialManager.GrabarInformeComercial(oParam, ComercialId.Value, nuevosCampos, nuevosAcopios, 
                contactoComercial, direccion, codigoPostal, localidadId);

            if (!entityError.HayErrores)
            {
                var oLstInformeComercial = new LstInformeComercial(reportesManager);

                var datos = mobjInformeComercialManager.GenerarInformeComercial(oParam, (int)entityError.InformeId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);

                mobjInformeComercialManager.EnviarMailInformeComercial(identif);
            }
            else
            {
                model.Errores = entityError.Errores;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarMateriales(oParamInforme oParam)
        {
            var model = new InformesModel
            {
                materiales = mobjInformeComercialManager.TraerInformeComercial(oParam.filtro),
                InformeGenerado = mobjInformeComercialManager.TraerInformeComercialGenerado(oParam.filtro)
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarInformes()
        {
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.TraerInformesGenerados(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> GenerarExcel(oParamExcel oParamReportes)
        {
            var model = new ReportesModel();

            var oLstIndicadores = new LstInformeComercial(reportesManager);

            var odatos = mobjInformeComercialManager.TraerCapacidadProductiva(oParamReportes.Informes);

            var identif = oLstIndicadores.GenerarInformesExcel(odatos);

            var d = mobjInformeComercialManager.GrabarCapacidadProductiva(oParamReportes.Informes);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> GenerarExcelIA(ParamReportesIC oParamReportes)
        {
            var model = new ReportesModel();

            oParamReportes.ComercialIDGenerador = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var oLstIndicadores = new LstInformeComercial(reportesManager);

            var odatos = mobjInformeComercialManager.ListarReportes(oParamReportes, GlobalVariables.Equipo);

            var identif = await oLstIndicadores.GenerarInformesExcelICAsync(odatos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ReImprimirPDF(int InformeComercialId)
        {
            var model = new ReportesModel();

            var informe = mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId);

            if (informe != null)
            {
                var oLstInformeComercial = new LstInformeComercial(reportesManager);

                var datos = mobjInformeComercialManager.GenerarInformeComercial(informe, InformeComercialId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult EliminarInformeComercial(int informeComercialId)
        {
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.EliminarInformes(informeComercialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ModificarInformeComercial(int InformeComercialId)
        {
            return new JsonResult()
            {
                Data = new ReportesModificacionModel
                {
                    parametros = mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId),
                    materiales = mobjInformeComercialManager.TraerInformeMateriales(InformeComercialId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarReportes(ParamReportesIC oParam)
        {
            var model = new List<ReportesList>();

            if (oParam.ComercialIDGenerador == null)
            {
                oParam.ComercialIDGenerador = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            }
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.ListarReportes(oParam, GlobalVariables.Equipo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(DataSourceRequest filtro)
        {
            if (filtro.Sort == null)
            {
                filtro.Sort = new List<Sort> {
                    new Sort {Field = "RazonSocial", Dir = "desc" }
                };
            }

            var equipo = GlobalVariables.EquipoReal;
            var model = mobjInformeComercialManager.TraerInformesFiltrados(filtro);

            return new JsonResult() { Data = model, JsonRequestBehavior = JsonRequestBehavior.AllowGet, MaxJsonLength = Int32.MaxValue };
        }

        public ActionResult ActualizarFechaDescargaInformeComercial(List<int> ids)
        {
            mobjInformeComercialManager.GuardarFechaDescargaInformeComercial(ids);
            return new JsonResult()
            {
                Data = "Ok",
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult EnviarCapacidadProductivaSAP(List<EnviarCapacidadProductivaSAPDto> enviar)
        {
            return new JsonResult()
            {
                Data = mobjInformeComercialManager.EnviarCapacidadProductivaSAP(enviar),
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}


