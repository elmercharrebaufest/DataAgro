using System.Web.Mvc;
using Molinos.DataAgro.Interfaces;

namespace WebDataAgro.Controllers
{
    [Authorize]
    public class ContactoComercialAuditoriaController : Controller
    {
        private readonly IContactoComercialAuditoriaManager _auditoriaManager;
        
        public ContactoComercialAuditoriaController(IContactoComercialAuditoriaManager auditoriaManager)
        {
            _auditoriaManager = auditoriaManager;
        }
        
        /// <summary>
        /// Visualiza el panel de auditoría con todos los métodos disponibles
        /// </summary>
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        
        /// <summary>
        /// Visualiza el historial de cambios de un ContactoComercial
        /// </summary>
        [HttpGet]
        public ActionResult HistorialContacto(int contactoComercialId)
        {
            var historial = _auditoriaManager.ObtenerHistorial(contactoComercialId);
            ViewBag.ContactoComercialId = contactoComercialId;
            return View(historial);
        }
        
        /// <summary>
        /// API para obtener el historial en JSON
        /// </summary>
        [HttpGet]
        public JsonResult ObtenerHistorialJson(int contactoComercialId)
        {
            var historial = _auditoriaManager.ObtenerHistorial(contactoComercialId);
            return Json(new { data = historial }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Obtiene cambios recientes
        /// </summary>
        [HttpGet]
        public JsonResult CambiosRecientes(int dias = 7)
        {
            var cambios = _auditoriaManager.ObtenerCambiosRecientes(dias);
            return Json(new { data = cambios }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Obtiene cambios por usuario
        /// </summary>
        [HttpGet]
        public JsonResult CambiosPorUsuario(string usuario)
        {
            var cambios = _auditoriaManager.ObtenerCambiosPorUsuario(usuario);
            return Json(new { data = cambios }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Obtiene el último cambio realizado
        /// </summary>
        [HttpGet]
        public JsonResult UltimoCambio(int contactoComercialId)
        {
            var cambio = _auditoriaManager.ObtenerUltimoCambio(contactoComercialId);
            return Json(new { data = cambio }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Obtiene resumen de cambios por tipo de operación
        /// </summary>
        [HttpGet]
        public JsonResult ResumenPorTipoOperacion(int contactoComercialId)
        {
            var resumen = _auditoriaManager.ObtenerResumenPorTipoOperacion(contactoComercialId);
            return Json(new { data = resumen }, JsonRequestBehavior.AllowGet);
        }
        
        /// <summary>
        /// Obtiene historial de auditoría para un ContactoComercial específico
        /// </summary>
        [HttpGet]
        public JsonResult Historial(int contactoComercialId)
        {
            var historial = _auditoriaManager.ObtenerHistorial(contactoComercialId);
            return Json(new { data = historial }, JsonRequestBehavior.AllowGet);
        }
    }
}
