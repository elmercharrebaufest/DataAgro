using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
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

        private string idActiveDirectory;

        private IComercialManager mobjComercialManager;

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public MaterialController(IMSContextProvider oMSContextProvider, IMaterialManager oMaterialManager, IComercialManager oComercialManager)
        {
            mobjMaterialManager = oMaterialManager;
            idActiveDirectory = oMSContextProvider.GetIdActiveDirectory();
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
               

        public async Task<ActionResult> Filtrar(ParamAbmMaterial oParam)
        {
            var model = new ResultIniMaterialModel();

            oParam.Codigo = oParam.Codigo ?? "";
            oParam.Descripcion = oParam.Descripcion ?? "";

            var result = await mobjMaterialManager.TraerFiltroMaterialAsync(oParam);

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


        public async Task<ActionResult> Aplicar(AbmMaterialParam oParam)
        {
            var model = new AbmMaterialResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                model.Material = await mobjMaterialManager.TraerMaterialAsync(oParam.MaterialId);
            }
            else
            {
                model.Errores = Util.EntityErrorsToMSErrorMessage(errors);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Grabar(Material oMaterial)
        {
            var model = new AbmMaterialResult();

            var entityErrors = await mobjMaterialManager.GrabarMaterialAsync(oMaterial);

            model.Errores = Util.EntityErrorsToMSErrorMessage(entityErrors);
                        
            if (model.Errores.Count > 0)
            {
                model.Material = oMaterial;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmMaterialParam oParam)
        {
            var model = new AbmMaterialResult();

            var errors = new EntityErrors();

            if (oParam.Validate(errors.ListaErrores))
            {
                await mobjMaterialManager.EliminarMaterialAsync(oParam.MaterialId);
            }
            else
            {
                model.Errores = Util.EntityErrorsToMSErrorMessage(errors);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Cancelar()
        {
            var model = new AbmMaterialResult();
                      
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


