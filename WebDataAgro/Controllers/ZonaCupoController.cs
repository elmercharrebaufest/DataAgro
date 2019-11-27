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
    [Autorizacion(PermisosDataAgro.IngresoDataAgro )]
    public class ZonaCupoController: Controller
    {

        private IZonaCupoManager mobjZonaCupoManager;

        public ZonaCupoController(IZonaCupoManager oZonaCupoManager)
        {
            mobjZonaCupoManager = oZonaCupoManager;
        }
        [Autorizacion(PermisosDataAgro.ConfiguracionZonaCupos )]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniAbmZonaCupoModel
                {
                    Datos = mobjZonaCupoManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniZonaCupoModel();

            var result = mobjZonaCupoManager.TraerTodoZonaCupo();

            if (result != null)
            {
                model.Datos = result.ZonaCupo;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ZonaCupoCombo(AbmZonaCupoParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmZonaCupo
                {
                    ZonaCupo = mobjZonaCupoManager.TraerZonaCupo(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult Aplicar(AbmZonaCupoParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmZonaCupoResult
                {
                    ZonaCupo = mobjZonaCupoManager.TraerZonaCupo(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(ZonaCupo oZonaCupo)
        {
            var model = new AbmZonaCupoResult();

            var entityErrors = mobjZonaCupoManager.GrabarZonaCupo(oZonaCupo);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.ZonaCupo = new ZonaCupoDto { CodigoSap = oZonaCupo.CodigoSap, Descripcion = oZonaCupo.Descripcion, Id = oZonaCupo.Id };
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmZonaCupoParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjZonaCupoManager.EliminarZonaCupo(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmZonaCupoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}