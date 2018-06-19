using Mastersoft.Framework.DataRepository;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Business;

namespace WebDataAgro.Controllers
{
    public class AgendaController : Controller
    {

        private MSContext mobjMSContext;

        private IAgendaManager mobjAgendaManager;       

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------
        public ActionResult Index()
        {
            return View();
        }



        public AgendaController(IMSContextProvider oMSContextProvider, IAgendaManager oAgendaManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();            
            mobjAgendaManager = oAgendaManager;
            mobjAgendaManager.Inicializar(mobjMSContext);

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjComercialManager = oComercialManager;
            mobjComercialManager.Inicializar(mobjMSContext);

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);


            if (mobcomercialmanager.EsPerfilAdministrativo(idActiveDirectory) || mobcomercialmanager.EsPerfilVisualizador(idActiveDirectory))
            {
                ViewBag.edita = false;
            }
        }
               
        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniAgendaActividadModel();           

            model.Datos = await mobjAgendaManager.TraerDatosInicialesAsync(Util.GetIdActiveDirectory());

            

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

            var oLstAgenda = new LstAgendaActividad(mobjMSContext);                           
            
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