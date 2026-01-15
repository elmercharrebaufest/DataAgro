using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebDataAgro.Controllers
{
    public class ControlDeBoletosController : Controller
    {
        private readonly IConfirmaManager _confirmaManager;
        private readonly IMaterialManager _materialManager;
        private readonly IContratoManager _contratoManager;

        public ControlDeBoletosController(IConfirmaManager confirmaManager, IMaterialManager materialManager, IContratoManager contratoManager)
        {
            this._confirmaManager = confirmaManager;
            this._materialManager = materialManager;
            this._contratoManager = contratoManager;
        }

        // GET: ControlDeBoletos
        public ActionResult Index()
        {
            this.CargarSeleccionables();
            return View();
        }

        // GET: ControlDeBoletos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ControlDeBoletos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ControlDeBoletos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(object model)
        {
            try
            {
                // TODO: Add insert logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ControlDeBoletos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ControlDeBoletos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, object model)
        {
            try
            {
                // TODO: Add update logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: ControlDeBoletos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ControlDeBoletos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                // TODO: Add delete logic here
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #region Metodos Privados
        private void CargarSeleccionables()
        {
            var datosCombos = _confirmaManager.TraerDatosCombos();
            var claseListItems = datosCombos.clasenegocio.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.ClaseNegocioId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.ClaseNegocio = claseListItems;
            var material = _materialManager.TraerTodoMaterial();

            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var boleto = _contratoManager.TraerTodosLosBoletos();
            var boletoListItems = boleto.Select(
               x => new SelectListItem
               {
                   Text = x.Descripcion,
                   Value = x.Id.ToString(),
                   Selected = false
               }).OrderBy(x => x.Value);
            ViewBag.Boleto = boletoListItems;
        }
        #endregion
    }
}
