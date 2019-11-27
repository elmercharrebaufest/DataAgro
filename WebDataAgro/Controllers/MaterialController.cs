using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class MaterialController : Controller
    {
        private readonly IMaterialManager materialManager;

        public MaterialController(IMaterialManager materialManager) 
        {
            this.materialManager = materialManager;
        }
        // GET: Material
        [Autorizacion(PermisosDataAgro.ConfiguracionMaterial)]
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmMaterialModel
                {
                    Datos = materialManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniMaterialModel();

            var result = materialManager.TraerTodoMaterial();

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

        public ActionResult MaterialCombo(AbmMaterialParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmMaterial
                {
                    Material = materialManager.TraerMaterial(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmMaterialParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmMaterialResult
                {
                    Material = materialManager.TraerMaterial(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(Material oMaterial)
        {
            var model = new AbmMaterialResult();

            var entityErrors = materialManager.GrabarMaterial(oMaterial);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.Material = new MaterialDto { Codigo = oMaterial.Codigo, Descripcion = oMaterial.Descripcion, MaterialId = oMaterial.MaterialId };
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
                Data = materialManager.EliminarMaterial(oParam.Id),
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