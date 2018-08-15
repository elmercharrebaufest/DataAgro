using Autofac.Extras.NLog;
using KendoGridBinder.Containers;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class CompraNetController : Controller
    {
        private IHomeManager mobjHomeManager;

        private ICompraNetManager mobjCompraNetManager;

        private IContratoManager mobjContratoManager;

        private IFijacionDePrecioContratoManager mobjFijacionDePrecioContratoManager;

        private IComercialManager mobjComercialManager;

        private ICampañaManager mobjCampañaManager;

        private ILocalidadManager mobjLocalidadManager;

        private IMaterialManager mobjMaterialManager;

        private IProveedorManager mobjProveedorManager;

        private ILogger mobjLogger;

        

        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public CompraNetController(IHomeManager oHomeManager, ILocalidadManager ojLocalidadManager, IProveedorManager oProveedorManager, IMaterialManager oMaterialManager, IContratoManager oContratoManager, IFijacionDePrecioContratoManager oFijacionDePrecioContratoManager, ICompraNetManager oCompraNetManager, IComercialManager oComercialManager, ICampañaManager oCampañaManager, ILogger oLogger)
        {
            mobjHomeManager = oHomeManager;
            mobjComercialManager = oComercialManager;
            mobjCompraNetManager = oCompraNetManager;
            mobjContratoManager = oContratoManager;
            mobjFijacionDePrecioContratoManager = oFijacionDePrecioContratoManager;
            mobjCampañaManager = oCampañaManager;
            mobjMaterialManager = oMaterialManager;
            mobjProveedorManager = oProveedorManager;
            mobjLocalidadManager = ojLocalidadManager;
            mobjLogger = oLogger;
            
        }

        //-----------------------------------------------------
        // Metodos Publicos
        //-----------------------------------------------------

        public ActionResult Index()
        {
            ViewBag.perfil = GlobalVariables.Perfil.DisplayEnum();
            ViewBag.TieneEmpleadosACargo = GlobalVariables.TieneEmpleadosACargo;
            return View();
        }

        public ActionResult CrearContrato(int? id)
        {
            ViewBag.ContratoId = id;
            return View();
        }

        public ActionResult CrearFijacion(int? id)
        {
            ViewBag.FijacionId = id;
            return View("CrearContrato");
        }

        public ActionResult Inicializar()
        {
            return new JsonResult()
            {
                Data = new DatosIniCompraNetModel
                {
                    Datos = mobjCompraNetManager.TraerDatosIniciales(GlobalVariables.Equipo)
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarContrato()
        {
            return new JsonResult()
            {
                Data = new ContratoModel_prueba
                {
                    Datos = mobjContratoManager.TraerDatosCombo()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult InicializarFijacion()
        {
            return new JsonResult()
            {
                Data = new FijacionDePrecioContratoModel
                {
                    Datos = mobjFijacionDePrecioContratoManager.TraerDatosIniciales()
                },
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarContrato(Contrato oParam)
        {
            if (oParam.Base == null) oParam.Base = false;
            if (oParam.NoInformaSio == null) oParam.NoInformaSio = false;
            if (oParam.TrigoEspecial == null) oParam.TrigoEspecial = false;

            if (oParam.ComercialId.HasValue)
            {
                var comercial = mobjComercialManager.TraerComercial(oParam.ComercialId.Value);
                oParam.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }

            oParam.UsuarioId = GlobalVariables.IdActiveDirectory;

            var model = mobjContratoManager.GrabarContrato(oParam);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FinalizarContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.FinalizarContrato(oParam, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ConfirmarContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ConfirmarContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.BorrarContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult BorrarFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.BorrarFijacion(oParam),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult ConfirmarFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.ConfirmarFijacion(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult FinalizarFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.FinalizarFijacion(oParam, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ReenviarMails(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = new GrabarContratoResult(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionContrato(Contrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.GrabarAmpliacionContrato(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.GrabarAmpliacionFijacion(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarFijacion(FijacionDePrecioContrato oParam)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.GrabarFijacionDePrecio(oParam),
                MaxJsonLength = Int32.MaxValue
            };
        }

        [HttpPost]
        public ActionResult BuscaDatosTabla(KendoGridMvcRequest request)
        {
            if (request.SortObjects != null)
            {
                request.SortObjects = request.SortObjects.Concat(new[] { new SortObject("Estado_Order", "asc") });
            }
            else
            {
                request.SortObjects = new List<SortObject> { new SortObject("Estado_Order", "asc") };
            }
            request.SortObjects = request.SortObjects.Concat(new[] { new SortObject("Fecha_Order", "desc") });

            var model = mobjContratoManager.TraerTodosContratos(request, GlobalVariables.Equipo);

            return Json(model);
        }

        public ActionResult TraerCampanaPorMaterial(int? materialId)
        {
            if (materialId == null)
            {
                materialId = 0;
            }

            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCampañaPorMaterial(Convert.ToInt32(materialId)),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerLocalidadPorProvincia(int? provinciaId)
        {
            if (provinciaId == null)
            {
                provinciaId = 0;
            }

            return new JsonResult()
            {
                Data = mobjLocalidadManager.TraerLocalidadPorProvincia(provinciaId.Value),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public int? TraerCampanaActualMaterial(int MaterialId)
        {
            return mobjMaterialManager.TraerMaterial(MaterialId).CampañaId;
        }

        public ActionResult TraerCalidadesPorMaterial(int? MaterialId)
        {
            if (MaterialId == null)
            {
                MaterialId = 0;
            }

            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCalidadPorMaterial(Convert.ToInt32(MaterialId)),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerComercialId()
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.ObtenerComercialId(GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public int ObtenerProveedorId(string Cuit)
        {
            return mobjProveedorManager.TraerProveedorPorCuit(Cuit).ProveedorId;
        }

        public ActionResult ObtenerProvinciaLocalidad(string Cuit)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerLocalidadProveedorPorCuit(Cuit),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerProvinciaLocalidadProv(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerLocalidadProveedorPorCuit(oDatosLocalidadProvinciaFiltro),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BuscarCuitPorId(int? ProveedorId)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerProveedor(ProveedorId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ListarProveedor(string text = "")
        {
            var proveedores = mobjProveedorManager.ListarProveedor(text);
            return Json(proveedores.Select(x => new { x.ProveedorId, Proveedor = x.RazonSocial }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult ListarComercial(string text = "")
        {
            var comerciales = mobjComercialManager.ListarComercial(text, GlobalVariables.Equipo);
            return Json(comerciales.Select(x => new { x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult TraerDescuentosPorContrato(int contratoId = 0)
        {
            var model = mobjContratoManager.TraerDescuentosPorContrato(contratoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult TraerCalidadesPorContrato(int contratoId = 0)
        {
            var model = mobjContratoManager.TraerCalidadesPorContrato(contratoId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        public ActionResult  TraerContratoCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjContratoManager.TraerContrato(id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerFijacionCompleto(int id)
        {
            return new JsonResult()
            {
                Data = mobjFijacionDePrecioContratoManager.TraerFijacion(id),
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}