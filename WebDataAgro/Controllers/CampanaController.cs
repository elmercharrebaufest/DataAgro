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
    public class CampanaController : Controller
    {
        private readonly ICampañaManager campañaManager;

        public CampanaController(ICampañaManager campañaManager) 
        {
            this.campañaManager = campañaManager;
        }

        public ActionResult Buscar()
        {
            var model = new ResultIniCampanaModel();

            var result = campañaManager.TraerTodoCampania();

            if (result != null)
            {
                model.Datos = result;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}