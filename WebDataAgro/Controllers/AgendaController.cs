using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class AgendaController : Controller
    {
        private IAgendaManager mobjAgendaManager;
        private IComercialManager mobjComercialManager;
        private readonly IReportesManager reportesManager;

        public AgendaController(IAgendaManager oAgendaManager, IComercialManager oComercialManager, IReportesManager reportesManager)
        {
            mobjAgendaManager = oAgendaManager;
            mobjComercialManager = oComercialManager;
            this.reportesManager = reportesManager;
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
        }
        
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

        public async Task<ActionResult> ExportarAgenda(RptActividadAgendaParam oParam)
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