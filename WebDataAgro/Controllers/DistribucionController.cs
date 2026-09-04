using Molinos.DataAgro.Entities.Seguridad;
using System.Web.Mvc;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.SugerenciaDeCupos)]
    public class DistribucionController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}
