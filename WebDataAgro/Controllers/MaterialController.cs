using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class MaterialController : Controller
    {
        private IMaterialManager mobjMaterialManager;

        

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public MaterialController(IMaterialManager oMaterialManager, IComercialManager oComercialManager)
        {
            mobjMaterialManager = oMaterialManager;
            
            mobjComercialManager = oComercialManager;
            
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Filtrar(ParamAbmMaterial oParam)
        {
            var model = new ResultIniMaterialModel();

            oParam.Codigo = oParam.Codigo ?? "";
            oParam.Descripcion = oParam.Descripcion ?? "";

            var result = mobjMaterialManager.TraerFiltroMaterial(oParam);

            if (result != null)
            {
                model.Datos = result.Material;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Aplicar(AbmMaterialParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmMaterialCrearResult
                {
                    Material = mobjMaterialManager.TraerMaterial(oParam.MaterialId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Grabar(Material oMaterial)
        {
            var model = new AbmMaterialResult();

            var entityErrors = mobjMaterialManager.GrabarMaterial(oMaterial);

            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.Material = oMaterial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Eliminar(AbmMaterialParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjMaterialManager.EliminarMaterial(oParam.MaterialId),
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmMaterialResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


