using System;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System.Collections.Generic;
using System.Linq;
using WebDataAgro.Models;
using System.Web;
using System.Web.Mvc;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ZonaCupoController: Controller
    {

        private IZonaCupoManager mobjZonaCupoManager;
        private IComercialManager mobjComercialManager;

        public ZonaCupoController(IZonaCupoManager oZonaCupoManager, IComercialManager oComercialManager)
        {
            mobjZonaCupoManager = oZonaCupoManager;
            mobjComercialManager = oComercialManager;
        }
        public ActionResult Index()
        {
            string ActionView = "";
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }
            else if (!mobjComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }
            return View(ActionView);
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