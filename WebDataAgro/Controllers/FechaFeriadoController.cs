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
    public class FechaFeriadoController : Controller
    {
        private readonly IFechaFeriadoManager fechaFeriadoManager;

        public FechaFeriadoController(IFechaFeriadoManager fechaFeriadoManager)
        {
            this.fechaFeriadoManager = fechaFeriadoManager;
        }
        // GET: Material
        [Autorizacion(PermisosDataAgro.ConfiguracionFeriado)]
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Buscar()
        {
            var model = new ResultIniFechaFeriadoModel();

            var result = fechaFeriadoManager.TraerTodo();

            if (result != null)
            {
                model.Datos = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Aplicar(AbmFechaFeriadoParam oParam)
        {
            return new JsonResult()
            {
                Data = new AbmFechaFeriadoResult
                {
                    FechaFeriado = fechaFeriadoManager.Traer(oParam.Id)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Grabar(FechaFeriado oFechaFeriado)
        {
            var model = new AbmFechaFeriadoResult();

            var entityErrors = fechaFeriadoManager.Grabar(oFechaFeriado);
            model.Errores = entityErrors.Errores;
            if (model.HayErrores)
            {
                model.FechaFeriado = new FechaFeriadoDto { Id = oFechaFeriado.Id, Feriado = oFechaFeriado.Feriado };
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Eliminar(AbmFechaFeriadoParam oParam)
        {
            return new JsonResult()
            {
                Data = fechaFeriadoManager.Eliminar(oParam.Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult Cancelar()
        {
            return new JsonResult()
            {
                Data = new AbmFechaFeriadoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}