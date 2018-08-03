using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class CubProveedoresController : Controller
    {
        private ICubProveedoresManager mobjCubProveedoresManager;

        private IHomeManager mobjHomeManager;

        

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CubProveedoresController(ICubProveedoresManager oCubProveedoresManager, IHomeManager oHomeManager, IComercialManager oComercialManager)
        {
            
            mobjCubProveedoresManager = oCubProveedoresManager;
            mobjHomeManager = oHomeManager;
            mobjComercialManager = oComercialManager;
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


        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniCubProveedoresModel
                {
                    Datos = mobjCubProveedoresManager.TraerDatosIniciales(GlobalVariables.Equipo),

                    Param = mobjCubProveedoresManager.TraerParam()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Validar(ParamCubProveedores oParam)
        {
            return Json(new CubProveedoresModel());
        }


        public ActionResult Listar(ParamCubProveedores oParam)
        {
            var model = new CubProveedoresModel();

            oParam.ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);

            var datos = mobjCubProveedoresManager.TraerDatos(oParam);

            model.Errores = datos.Errores;

            if (model.HayErrores)
            {
                if (datos.Proveedores.Count == 0)
                {
                    model.Error("aviso", "No hay datos para listar");
                }
            }
            else
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

