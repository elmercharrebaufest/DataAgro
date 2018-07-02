using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
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
        private string idActiveDirectory;
        private IComercialManager mobjComercialManager;
        private readonly IReportesManager reportesManager;

        public AgendaController(IMSContextProvider oMSContextProvider, IAgendaManager oAgendaManager, IComercialManager oComercialManager, IReportesManager reportesManager)
        {
            mobjAgendaManager = oAgendaManager;

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
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
   
        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAgendaActividadModel
            {
                Datos = await mobjAgendaManager.TraerDatosInicialesAsync(Util.GetIdActiveDirectory())
            };
            
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ExportarAgenda(RptActividadAgendaParam oParam)
        {
            var model = new ReportesModel();

            oParam.ActiveDirectoryId = Util.GetIdActiveDirectory();

            var datos = await mobjAgendaManager.ExportarAgenda(oParam);

            var oLstAgenda = new LstAgendaActividad(reportesManager);                           
            
            var identif = await oLstAgenda.GenerarListadoAsync(datos);

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

        public async Task<ActionResult> VistaPreviaAgenda(RptActividadAgendaParam oParam)
        {
           

            oParam.ActiveDirectoryId = Util.GetIdActiveDirectory();

           var model = await mobjAgendaManager.VistaPreviaAgenda(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

    }
}