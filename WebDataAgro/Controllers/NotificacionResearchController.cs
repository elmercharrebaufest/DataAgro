using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Models;

namespace WebDataAgro.Controllers
{
    public class NotificacionResearchController : Controller
    {
        private readonly IMaterialManager materialManager;
        private readonly ICampañaManager campanaManager;
        private readonly IResearchManager researchManager;

        public NotificacionResearchController(IMaterialManager materialManager,ICampañaManager campanaManager, IResearchManager researchManager)
        {
            this.materialManager = materialManager;
            this.campanaManager = campanaManager;
            this.researchManager = researchManager;
        }

        // GET: NotificacionResearch
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult NotificacionesPartial(NotificacionModel model)
        {
            CargarViewBag();
            model.Notificaciones = researchManager.TraerTodasNotificaciones();            
            return PartialView("_NotificacionesPartial",model);
        }
        [HttpPost]
        public ActionResult GrabarNotificacion(NotificacionModel model)
        {
            var researchAvanceSiembra = TransformarAEntidad(model);
            var resultado = researchManager.GrabarNotificacion(researchAvanceSiembra);
            if (resultado.HayError)
            {
                model.Resultado = resultado;                
            }
            else
            {
                resultado.Errores.Add(new ErrorMessage(200, "Se Guardo correctamente"));
                model = new NotificacionModel { Resultado = resultado };
            }
            return NotificacionesPartial(model);
        }
        public ActionResult EliminarNotificacion(int id)
        {
            var resultado = researchManager.EliminarNotificacion(id);
            
            return NotificacionesPartial(new NotificacionModel { Resultado = resultado });
        }

        private void CargarViewBag()
        {
            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x=>x.Value);
            ViewBag.Material = materialesListItems;
            var campana = campanaManager.TraerCampañasActivas();
            var campanaListItems = campana.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.CampañaId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Campana = campanaListItems;
            var research = researchManager.TraerResearch();
            var researchListItems = research.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.Id.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Research = researchListItems;
        }
        private NotificacionResearch TransformarAEntidad(NotificacionModel model)
        {
            return new NotificacionResearch
            {
                Id=model.Id,
                MaterialId = model.MaterialId,
                CampanaId = model.CampanaId,
                TipoResearchId = model.TipoResearchId,
                FechaDesde = model.FechaDesde,
                FechaHasta = model.FechaHasta,
                Mensaje = model.Mensaje
            };
        }
    }
}