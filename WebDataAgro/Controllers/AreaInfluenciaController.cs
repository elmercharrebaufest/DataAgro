using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using System;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class AreaInfluenciaController : Controller
    {
        private IAreaInfluenciaManager mobjAreaInfluenciaManager;

        public AreaInfluenciaController(IAreaInfluenciaManager oAreaInfluenciaManager)
        {
            mobjAreaInfluenciaManager = oAreaInfluenciaManager;            
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Inicializar()
        {
            var model = new DatosIniAbmAreaInfluenciaModel
            {
                AreaInfluencia = new AreaInfluencia()
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Buscar()
        {
            var model = new ResultIniAreaInfluenciaModel();

            var result = mobjAreaInfluenciaManager.TraerTodoAreaInfluencia();

            if (result != null)
            {
                model.Datos = result.AreaInfluencia;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        
        public ActionResult Aplicar(AbmAreaInfluenciaParam oParam)
        {
            var model = new AbmAreaInfluenciaResult
            {
                AreaInfluencia = mobjAreaInfluenciaManager.TraerAreaInfluencia(oParam.AreaInfluenciaId)
            };

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Grabar(AreaInfluencia oAreaInfluencia)
        {
            var model = new AbmAreaInfluenciaResult();

            var entityErrors = mobjAreaInfluenciaManager.GrabarAreaInfluencia(oAreaInfluencia);
            model.Errores = entityErrors.Errores;

            if (model.HayErrores)
            {
                model.AreaInfluencia = new AreaInfluenciaDto { AreaInfluenciaId = oAreaInfluencia.AreaInfluenciaId, Descripcion = oAreaInfluencia.Descripcion};
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Eliminar(AbmAreaInfluenciaParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjAreaInfluenciaManager.EliminarAreaInfluencia(oParam.AreaInfluenciaId),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmAreaInfluenciaResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }


    }
}


