using System.Linq;
using System.Web.Mvc;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using WebDataAgro.Atributos;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ResearchMapaController : Controller
    {
        private readonly ICampañaManager campañaManager;
        private readonly IMaterialManager materialManager;

        public ResearchMapaController(ICampañaManager campañaManager, IMaterialManager materialManager)
        {
            this.campañaManager = campañaManager;
            this.materialManager = materialManager;
        }

        // GET: ResearchMapa
        [Autorizacion(PermisosDataAgro.MapaResearch)]
        public ActionResult Index()
        {
            FillViewBag();
            return View();
        }

        private void FillViewBag()
        {
            var material = materialManager.TraerTodoMaterial();
            var materialesListItems = material.Material.Select(
                    x => new SelectListItem
                    {
                        Text = x.Descripcion,
                        Value = x.MaterialId.ToString(),
                        Selected = false
                    }).OrderBy(x => x.Value);
            ViewBag.Material = materialesListItems;

            var campania = campañaManager.TraerTodoCampania().Where(x => x.CampañaId >= 6).ToList();
            var campaniaListItems = campania.Select(x => new SelectListItem
            {
                Text = x.Descripcion,
                Value = x.CampañaId.ToString(),
                Selected = false
            }).OrderBy(x => x.Text);

            ViewBag.Campania = campaniaListItems;

        }
    }
}