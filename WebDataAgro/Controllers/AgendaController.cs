using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class AgendaController : Controller
    {
        private IAgendaManager mobjAgendaManager;
        private readonly IReportesManager reportesManager;

        public AgendaController(IAgendaManager oAgendaManager, IReportesManager reportesManager)
        {
            mobjAgendaManager = oAgendaManager;
            this.reportesManager = reportesManager;            
        }
        
        [Autorizacion( PermisosDataAgro.VisualizarReporteAgenda)]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAgendaActividadModel
                {
                    Datos = mobjAgendaManager.TraerDatosIniciales(GlobalVariables.Equipo)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ExportarAgenda(RptActividadAgendaParam oParam)
        {
            var model = new ReportesModel();

            oParam.ActiveDirectoryId = GlobalVariables.IdActiveDirectory;

            var datos = mobjAgendaManager.ExportarAgenda(oParam, GlobalVariables.Equipo);

            var oLstAgenda = new LstAgendaActividad(reportesManager);

            var identif = oLstAgenda.GenerarListado(datos);

            if (datos.Count == 0)
            {
                model.DownloadKey = "";
            }
            else
            {
                model.DownloadKey = Util.GetDownloadKey(identif);
            }
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult VistaPreviaAgenda(RptActividadAgendaParam oParam)
        {
            oParam.ActiveDirectoryId = GlobalVariables.IdActiveDirectory;
            return new JsonResult()
            {
                Data = mobjAgendaManager.VistaPreviaAgenda(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}