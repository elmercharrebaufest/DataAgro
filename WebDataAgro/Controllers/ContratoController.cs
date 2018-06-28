using KendoGridBinder.ModelBinder.Mvc;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
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
        private IComercialManager mobjComercialManager;
        private IProvinciaManager mobjProvinciaManager;
        private ILocalidadManager mobjLocalidadManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ContratoController(IMSContextProvider oMSContextProvider, IHomeManager oHomeManager,  ICampañaManager oCampañaManager, IProveedorManager oProveedorManager, IContratoManager ocontratoManager, IComercialManager oComercialManager, IProvinciaManager oProvinciaManager, ILocalidadManager oLocalidadManager)
        {
            mobjMSContext = oMSContextProvider.GetMSContext();
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
            mobjHomeManager = oHomeManager;
            mobjHomeManager.Inicializar(mobjMSContext);
            mobjCampañaManager = oCampañaManager;
            mobjCampañaManager.Inicializar(mobjMSContext);
            mobjProveedorManager = oProveedorManager;
            mobjProveedorManager.Inicializar(mobjMSContext);
            mobjComercialManager = oComercialManager;
            mobjComercialManager.Inicializar(mobjMSContext);
            mobjContratoManager = ocontratoManager;
            mobjContratoManager.Inicializar(mobjMSContext);
            mobjProvinciaManager = oProvinciaManager;
            mobjProvinciaManager.Inicializar(mobjMSContext);
            mobjLocalidadManager = oLocalidadManager;
            mobjLocalidadManager.Inicializar(mobjMSContext);
        }

        public ActionResult Index()
        {
            if (mobjComercialManager.EsPerfilAdministrativo(idActiveDirectory) || mobjComercialManager.EsPerfilVisualizador(idActiveDirectory))
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
            int comercialId = mobjContratoManager.ObtenerComercialId(idActiveDirectory);
            var perfil = mobjComercialManager.ObtenerPerfil(idActiveDirectory);
            List<ComercialQry> listComercial = new List<ComercialQry>();

            ComercialQry comercial = new ComercialQry();

            List<ComercialQry> listComercialAux = await mobjContratoManager.TraerComerciales();

            if (perfil == EnumPerfil.Comercial)
            {
                comercial.ComercialId = comercialId;

                listComercial.Add(comercial);
            }
            else if (perfil == EnumPerfil.Jefe)
            {
                listComercialAux.RemoveAll(x => comercialId != x.EmpleadorACargo);
                foreach (ComercialQry comerciallista in listComercialAux)
                {
                    listComercial.Add(comerciallista);
                }
                comercial.ComercialId = comercialId;

                listComercial.Add(comercial);
            }
            else if (perfil == EnumPerfil.Analista)
            {
                listComercialAux.RemoveAll(x => comercialId != x.EmpleadorACargo);
                foreach (ComercialQry comerciallista in listComercialAux)
                {
                    listComercial.Add(comerciallista);
                }
                comercial.ComercialId = comercialId;

                listComercial.Add(comercial);
            }
            else if (perfil == EnumPerfil.Mesa)
            {
                listComercial = listComercialAux;
            }

            return listComercial;
        }

        public ActionResult ListarComercial(string text)
        {
            var comerciales = mobjComercialManager.ListarComercial(text);
                                                     //tiene que coincidir ComercialId y Comercial con los campos configurados en el js linea 291
            return Json(comerciales.Select(x => new { ComercialId = x.ComercialId, Comercial = x.Nombres }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarProvincia(string text)
        {
            var provincias = mobjProvinciaManager.ListarProvincia(text);            
            return Json(provincias.Select(x => new { ProvinciaId = x.ProvinciaId, Provincia = x.Nombre }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult ListarLocalidad(string text)
        {
            var localidades = mobjLocalidadManager.ListarLocalidad(text);
            return Json(localidades.Select(x => new { LocalidadId = x.LocalidadId, Localidad = x.Nombre }), JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult ListarProveedor(string text)
        {
            var proveedores = mobjProveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { ProveedorId = x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
    }
}
