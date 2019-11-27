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
    public class LocalidadController : Controller
    {
        private ILocalidadManager mobjLocalidadManager;

        public LocalidadController(ILocalidadManager oLocalidadManager)
        {
            mobjLocalidadManager = oLocalidadManager;
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
            return new JsonResult()
            {
                Data = new DatosIniAbmLocalidadModel
                {
                    Datos = mobjLocalidadManager.TraerDatosIniciales(),
                    Localidad = new Localidad()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Filtrar(ParamAbmLocalidad oParam)
        {
            var model = new ResultIniLocalidadModel();

            oParam.Nombre = oParam.Nombre ?? "";

            var result = mobjLocalidadManager.TraerFiltroLocalidad(oParam);

            if (result != null)
            {
                model.Datos = result.Localidad;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Aplicar(AbmLocalidadParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmLocalidadCrearResult
                {
                    Localidad = mobjLocalidadManager.TraerLocalidad(oParam.LocalidadId)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Grabar(Localidad oLocalidad)
        {
            var model = new AbmLocalidadResult();

            var entityErrors = mobjLocalidadManager.GrabarLocalidad(oLocalidad);

            model.Errores = entityErrors.Errores;

            if (model.Errores.Count > 0)
            {
                model.Localidad = oLocalidad;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }


        public async Task<ActionResult> Eliminar(AbmLocalidadParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjLocalidadManager.EliminarLocalidad(oParam.LocalidadId),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmLocalidadResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}


