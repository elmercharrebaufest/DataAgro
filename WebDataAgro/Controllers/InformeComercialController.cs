
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;

using WebDataAgro.Core;
using WebDataAgro.Models;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using Molinos.DataAgro.Entities.Domain;
using Molinos.DataAgro.Business;

namespace WebDataAgro.Controllers
{
    public class InformeComercialController : Controller
    {
        //-----------------------------------------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------------------------------------

        private MSContext mobjMSContext;

        private ICondicionManager mobjCondicionManager;

        private IInformeComercialManager mobjInformeComercialManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        private IHomeManager mobjHomeManager;

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public InformeComercialController(IMSContextProvider oMSContextProvider, ICondicionManager oCondicionManager, IInformeComercialManager oInformeComercialManager, IComercialManager oComercialManager, IHomeManager oHomeManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            mobjCondicionManager = oCondicionManager;

            mobjCondicionManager.Inicializar(mobjMSContext);

            mobjInformeComercialManager = oInformeComercialManager;

            mobjInformeComercialManager.Inicializar(mobjMSContext);

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();

            mobjComercialManager = oComercialManager;
            mobjComercialManager.Inicializar(mobjMSContext);

            IComercialManager mobcomercialmanager = new ComercialManager();
            mobcomercialmanager.Inicializar(mobjMSContext);

            mobjHomeManager = oHomeManager;
            mobjHomeManager.Inicializar(mobjMSContext);

            if (mobcomercialmanager.EsPerfilAdministrativo(idActiveDirectory) || mobcomercialmanager.EsPerfilVisualizador(idActiveDirectory))
            {
                ViewBag.edita = false;
            }
        }

        //-----------------------------------------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InformeAdministrativo()
        {
            return View();
        }

        public ActionResult Reporte()
        {
            return View();
        }

        public async Task<ActionResult> Buscar()
        {
            var model = new ResultIniCondicionModel();

            var result = await mobjCondicionManager.TraerTodoCondicionAsync();

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
              
        public async Task<ActionResult> GrabarInformeComercial(Condicion oCondicion)
        {
            var model = new AbmCondicionResult();

            var entityErrors = await mobjCondicionManager.GrabarCondicionAsync(oCondicion);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            if (model.Errores.Count > 0)
            {
                model.Condicion = oCondicion;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }       

        public async Task<ActionResult> Listar(ParamInformeComercial oParam)
        {
            var model = new ReportesModel();

            var ComercialId = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var entityError = await mobjInformeComercialManager.GrabarInformeComercial(oParam, ComercialId);

            if (!entityError.errores.HayError)
            {
                var oLstInformeComercial = new LstInformeComercial(mobjMSContext);

                var datos = await mobjInformeComercialManager.GenerarInformeComercial(oParam, (int)entityError.InformeId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);
            }
            else
            {
                var list = new List<MSErrorMessage>();
                entityError.errores.ListaErrores.ForEach(x => list.Add(new MSErrorMessage(x)));
                model.Errores = list;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ListarMateriales(oParamInforme oParam)
        {

            var model = new InformesModel();

            model.materiales = await  mobjInformeComercialManager.TraerInformeComercialAsync(oParam.filtro);
            model.InformeGenerado = await mobjInformeComercialManager.TraerInformeComercialGeneradoAsync(oParam.filtro);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ListarInformes()
        {

            var model = await mobjInformeComercialManager.TraerInformesGeneradosAsync();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> GenerarExcel(oParamExcel oParamReportes)
        {
            var model = new ReportesModel();

            var oLstIndicadores = new LstInformeComercial(mobjMSContext);

            var odatos = await mobjInformeComercialManager.TraerCapacidadProductivaAsync(oParamReportes.Informes);

            var identif = await oLstIndicadores.GenerarInformesExcelAsync(odatos);

            var d = await mobjInformeComercialManager.GrabarCapacidadProductivaAsync(oParamReportes.Informes);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> GenerarExcelIA(ParamReportesIC oParamReportes)
        {
            var model = new ReportesModel();

            /*if (oParamReportes.ComercialID == null)
            {
                oParamReportes.ComercialID = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            }*/

            oParamReportes.ComercialIDGenerador = (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var oLstIndicadores = new LstInformeComercial(mobjMSContext);

            var odatos = await mobjInformeComercialManager.ListarReportes(oParamReportes);

            var identif = await oLstIndicadores.GenerarInformesExcelICAsync(odatos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);

        }

        public async Task<ActionResult> ReImprimirPDF(int InformeComercialId)
        {
            var model = new ReportesModel();

            var informe = await mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId);

            if (informe != null)
            {
                var oLstInformeComercial = new LstInformeComercial(mobjMSContext);

                var datos = await mobjInformeComercialManager.GenerarInformeComercial(informe, InformeComercialId);

                var identif = await oLstInformeComercial.GenerarListadoAsync(datos);

                model.DownloadKey = Util.GetDownloadKey(identif);
            }
            else
            {
                var list = new List<MSErrorMessage>();
                model.Errores = list;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> EliminarInformeComercial(int InformeComercialId)
        {
            var model = InformeComercialId;

            var informe = await mobjInformeComercialManager.EliminarInformes(InformeComercialId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> ModificarInformeComercial(int InformeComercialId)
        {
            var model = new ReportesModificacionModel();

            model.parametros = await mobjInformeComercialManager.ReimprimirInformeComercial(InformeComercialId);

            model.materiales = await mobjInformeComercialManager.TraerInformeMaterialesAsync(InformeComercialId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public async Task<ActionResult> ListarReportes(ParamReportesIC oParam)
        {
            var model = new List<ReportesList>();

            if (oParam.ComercialIDGenerador == null)
            {
                oParam.ComercialIDGenerador= (int)await mobjHomeManager.TraerIdComercial(idActiveDirectory);
            }

            model = await mobjInformeComercialManager.ListarReportes(oParam);
            

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };


        }


    }
}


