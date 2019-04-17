using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    public class ProveedorController : Controller
    {
        private IProveedorManager mobjProveedorManager;
        private IHomeManager mobjHomeManager;
        private ICampañaManager mobjCampañaManager;
        private ILocalidadManager mobjLocalidadManager;

        private IComercialManager mobComercialManager;
        private IReportesManager mobjreportesManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ProveedorController(IProveedorManager oProveedorManager, IHomeManager oHomeManager, ICampañaManager oCampañaManager, IComercialManager oComercialManager, IReportesManager oReportesManager, ILocalidadManager oLocalidadManager)
        {

            mobjProveedorManager = oProveedorManager;
            mobjHomeManager = oHomeManager;
            mobjCampañaManager = oCampañaManager;
            mobComercialManager = oComercialManager;
            mobjreportesManager = oReportesManager;
            mobjLocalidadManager = oLocalidadManager;
        }


        // GET: Contactos
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReporteProveedor()
        {
            if (GlobalVariables.Perfil == EnumPerfil.Administrativo || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                ViewBag.edita = false;
            }

            return View();
        }


        public ActionResult Agregar(int? ProveedorId)
        {
            if (GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                return RedirectToAction("Index", "Error");
            }
            else
            {
                ViewBag.ProveedorId = ProveedorId;
                return View();
            }

        }

        // GET: Contactos/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Contactos/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Contactos/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
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

        // GET: Contactos/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Contactos/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
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

        // GET: Contactos/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Contactos/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
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

        public ActionResult Detalle(int ProveedorId, bool? Agenda)
        {
            string ActionView = "";
            bool mostrarEditar = true;


            if (!mobComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }


            if ((GlobalVariables.Perfil == EnumPerfil.Administrativo && !mobjProveedorManager.ValidarProveedorEsCorredor(ProveedorId)) || GlobalVariables.Perfil == EnumPerfil.Visualizador)
            {
                mostrarEditar = false;
                ViewBag.edita = false;
            }
            else if (!mobComercialManager.ComercialPerteneceProveedor(GlobalVariables.Equipo, ProveedorId, (int)GlobalVariables.Perfil, GlobalVariables.CorredoresComercial))
            {
                ActionView = "ErrorUsuarioSinDerechos";
            }


            ViewBag.MostrarEditar = mostrarEditar;
            ViewBag.MostrarAgenda = Agenda;
            ViewBag.ProveedorId = ProveedorId;
            return View(ActionView);
        }

        public ActionResult TraerProveedor(int ProveedorId)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerProveedor(ProveedorId, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult Iniciliazar(int proveedorId)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerDatosCombo(proveedorId, GlobalVariables.Perfil == EnumPerfil.Administrativo),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerLocalidad(int Id)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerLocalidad(Id),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult CrearActividad(ActividadInsetarIni oParam)
        {
            oParam.ComercialId = mobjHomeManager.TraerIdComercial(GlobalVariables.IdActiveDirectory);
            oParam.UserName = GlobalVariables.IdActiveDirectoryCompleto;

            mobjProveedorManager.GrabarRecordatorio(oParam);

            return new JsonResult()
            {
                Data = new Actividad(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult EliminarRecordatorio(int id)
        {
            mobjProveedorManager.EliminarRecordatorio(id);

            return new JsonResult()
            {
                Data = new Actividad(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerRazonSocial(string cuit)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerRazonSocial(cuit),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarProveedor(NuevoProveedor oParam)
        {

            GrabarProveedorResult model = new GrabarProveedorResult();

            if (oParam.ProveedorId != null && oParam.ProveedorId != 0)
            {
                model = mobjProveedorManager.UpdateProveedor(oParam, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo,GlobalVariables.ComercialId);
            }
            else
            {
                model = mobjProveedorManager.GrabarNuevoProveedor(oParam, GlobalVariables.IdActiveDirectory);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerCampañasActivas()
        {
            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCampañasActivas(),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerMaterialPorCampaña(int campañaId)
        {
            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerMaterialPorCampaña(campañaId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerCampañaPorMaterial(int materialId)
        {
            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCampañaPorMaterial(materialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerFiltros(string TipoActividadId, int ProveedorId, HistorialActiviad oParam)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerHistorialActividad(oParam, ProveedorId, TipoActividadId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public async Task<ActionResult> ImprimirReporteProveedor(int ProveedorId)
        {
            var model = new ReportesModel();

            var datos = mobjProveedorManager.TraerProveedor(ProveedorId, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo);

            var oLstProveedor = new LstProveedor(mobjreportesManager);

            var identif = oLstProveedor.GenerarListado(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerCampañasPorGrano(int MaterialId)
        {
            return new JsonResult()
            {
                Data = mobjCampañaManager.TraerCampañasPorGrano(MaterialId),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ObtenerReporteProveedor(string Valor)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.ObtenerReporteProveedor(Valor, GlobalVariables.IdActiveDirectory),
                MaxJsonLength = Int32.MaxValue
            };
        }
        public JsonResult BuscarCorredores(string filtro, bool corredor)
        {
            return Json(mobjProveedorManager.DevolverProveedores(filtro, corredor, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarProveedoresConCorredor(string filtroProveedor, string filtro, bool corredor)
        {
            if (filtro == "")
            {
                return Json(mobjProveedorManager.DevolverProveedores(filtroProveedor, corredor, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(mobjProveedorManager.DevolverProveedoresConCorredor(filtroProveedor, filtro), JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult BuscarLocalidades(string filtro)
        {
            return Json(mobjLocalidadManager.DevolverLocalidades(filtro), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TraerProveedoresCorredor(int ProveedorId)
        {
            return Json(mobjProveedorManager.ListarProveedorCorredor(ProveedorId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult TraerProveedorParaCorredor(string cuit)
        {
            return Json(mobjProveedorManager.TraerProveedorParaCorredor(cuit), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GrabarCorredor(NuevoCorredor oParam)
        {

            GrabarProveedorResult model = new GrabarProveedorResult();

            if (oParam.CorredorId != null && oParam.CorredorId != 0)
            {
                model = mobjProveedorManager.UpdateCorredor(oParam, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo, GlobalVariables.ComercialId);
            }
            else
            {
                model = mobjProveedorManager.GrabarNuevoCorredor(oParam, GlobalVariables.IdActiveDirectory);
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
    }
}
