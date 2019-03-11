using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;
using Molinos.DataAgro.Entities.Extensions;

namespace WebDataAgro.Controllers
{
    public class ContratoAcuerdoController : Controller
    {
        private IContratoAcuerdoManager mobjContratoAcuerdoManager;

        public ContratoAcuerdoController(IContratoAcuerdoManager oContratoAcuerdoManager)
        {
            mobjContratoAcuerdoManager = oContratoAcuerdoManager;
        }

        public ActionResult Index()
        {
            string ActionView = "";

            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Mesa)
            {
                ViewBag.edita = false;
            }
            ViewBag.perfil = GlobalVariables.Perfil.DisplayEnum();

            return View(ActionView);
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniContratoAcuerdoModel();

            var result = mobjContratoAcuerdoManager.TraerTodoContratoAcuerdo();

            if (result != null)
            {
                model.Datos = result.ContratoAcuerdo;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmContratoAcuerdoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarContratoAcuerdo()
        {
            return new JsonResult()
            {
                Data = new ContratoAcuerdoModel_prueba
                {
                    Datos = mobjContratoAcuerdoManager.TraerDatosCombo((int)GlobalVariables.Perfil)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.EliminarContratoAcuerdo(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult Confirmar(AbmOperadorParam oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoAcuerdoManager.ConfirmarContratoAcuerdo(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }


        public ActionResult ContratoAcuerdoCombo(AbmContratoAcuerdoParam oParam)
        {
            return new JsonResult()
            {
                Data = new DataAbmContratoAcuerdo
                {
                    ContratoAcuerdo = mobjContratoAcuerdoManager.TraerContratoAcuerdo(oParam.Id)

                },
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult Aplicar(AbmContratoAcuerdoParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmContratoAcuerdoResult
                {
                    ContratoAcuerdo = mobjContratoAcuerdoManager.TraerContratoAcuerdo(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(ContratoAcuerdo oContratoAcuerdo)
        {
            var model = new AbmContratoAcuerdoResult();

            var entityErrors = mobjContratoAcuerdoManager.GrabarContratoAcuerdo(oContratoAcuerdo, GlobalVariables.Perfil);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.ContratoAcuerdo = new ContratoAcuerdoDto { Cantidad = oContratoAcuerdo.Cantidad, Comercial = oContratoAcuerdo.Comercial.ToString(), Destino = oContratoAcuerdo.Destino.ToString() };
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
