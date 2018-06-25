using KendoGridBinder.ModelBinder.Mvc;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities;
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

namespace WebDataAgro.Controllers
{
    public class ContratoController : Controller
    {
        private MSContext mobjMSContext;

        private IHomeManager mobjHomeManager;

        private ICampañaManager mobjCampañaManager;

        private string idActiveDirectory;
        private IContratoManager mobjContratoManager;
        private IProveedorManager mobjProveedorManager;
        
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ContratoController(IMSContextProvider oMSContextProvider, IHomeManager oHomeManager,  ICampañaManager oCampañaManager, IProveedorManager oProveedorManager, IContratoManager ocontratoManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjHomeManager = oHomeManager;
            mobjHomeManager.Inicializar(mobjMSContext);
            mobjCampañaManager = oCampañaManager;
            mobjCampañaManager.Inicializar(mobjMSContext);
            mobjProveedorManager = oProveedorManager;
            mobjProveedorManager.Inicializar(mobjMSContext);
        }

        public ActionResult Index()
        {
            IComercialManager mobComercialManager = new Molinos.DataAgro.Business.ComercialManager();
            mobComercialManager.Inicializar(mobjMSContext);

            if (mobComercialManager.EsPerfilAdministrativo(idActiveDirectory) || mobComercialManager.EsPerfilVisualizador(idActiveDirectory))
            {
                ViewBag.edita = false;
            }

            return View();
        }
        
        [HttpPost]
        public async Task<ActionResult> BuscaDatosTabla(KendoGridMvcRequest request)
        {
            List<ComercialQry> listComercial = await RecuperaEquipo(idActiveDirectory);

            var model = mobjContratoManager.TraerTodosContratos(request, listComercial);

            return Json(model);
        }

        private async Task<List<ComercialQry>> RecuperaEquipo(string idActiveDirectory)
        {

            int ComercialId = await mobjContratoManager.ObtenerComercialId(idActiveDirectory);

            IComercialManager mObjcomercialmanager = new ComercialManager();

            mObjcomercialmanager.Inicializar(mobjMSContext);

            List<ComercialQry> listComercial = new List<ComercialQry>();

            ComercialQry comercial = new ComercialQry();

            List<ComercialQry> listComercialAux = await mobjContratoManager.TraerComerciales();

            if (mObjcomercialmanager.EsPerfilComercial(idActiveDirectory))
            {
                comercial.ComercialId = ComercialId;

                listComercial.Add(comercial);
            }
            else if (mObjcomercialmanager.EsPerfilJefe(idActiveDirectory))
            {
                listComercialAux.RemoveAll(x => ComercialId != x.EmpleadorACargo);
                foreach (ComercialQry comerciallista in listComercialAux)
                {
                    listComercial.Add(comerciallista);
                }
                comercial.ComercialId = ComercialId;

                listComercial.Add(comercial);
            }
            else if (mObjcomercialmanager.EsPerfilMesa(idActiveDirectory))
            {
                listComercial = listComercialAux;
            }

            return listComercial;
        }

    }
}
