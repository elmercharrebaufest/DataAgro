using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Threading.Tasks;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;

using WebDataAgro.Core;
using WebDataAgro.Models;

using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Business;
using static WebDataAgro.MvcApplication;
using Molinos.DataAgro.Entities.Common.Enums;

namespace WebDataAgro.Controllers
{
    public class CubProveedoresController : Controller
    {
        //-----------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------

        private MSContext mobjMSContext;

        private ICubProveedoresManager mobjCubProveedoresManager;

        private IHomeManager mobjHomeManager;

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CubProveedoresController(IMSContextProvider oMSContextProvider, ICubProveedoresManager oCubProveedoresManager, IHomeManager oHomeManager, IComercialManager oComercialManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();

            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();

            mobjCubProveedoresManager = oCubProveedoresManager;

            mobjCubProveedoresManager.Inicializar(mobjMSContext);

            mobjHomeManager = oHomeManager;

            mobjHomeManager.Inicializar(mobjMSContext);

            mobjComercialManager = oComercialManager;

            mobjComercialManager.Inicializar(mobjMSContext);
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            return View();
        }


        public async Task<ActionResult> Inicializar()
        {
            var model = new DatosIniCubProveedoresModel();

            var ActiveDirectory = Util.GetIdActiveDirectory();
            model.Datos = await mobjCubProveedoresManager.TraerDatosInicialesAsync(ActiveDirectory);

            model.Param = mobjCubProveedoresManager.TraerParam();

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Validar(ParamCubProveedores oParam)
        {
            var model = new CubProveedoresModel();

            var entityErrors = mobjCubProveedoresManager.Validar(oParam);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);

            return Json(model);
        }


        public async Task<ActionResult> Listar(ParamCubProveedores oParam)
        {
            var model = new CubProveedoresModel();

            oParam.ComercialId = await mobjHomeManager.TraerIdComercial(idActiveDirectory);

            var datos = await mobjCubProveedoresManager.TraerDatosAsync(oParam);

            model.Errores = Util.EntityErrorsToMSErrorMessage(datos);

            if (model.Errores.Count == 0)
            {
                if (datos.Proveedores.Count == 0)
                {
                    model.Errores.Add(new MSErrorMessage() { Message = "No hay datos para listar", Source = "aviso" });
                }
            }

            if (model.Errores.Count == 0)
            {
                model.Proveedores = datos.Proveedores;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}

