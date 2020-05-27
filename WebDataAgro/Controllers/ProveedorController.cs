using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.Clases;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;
using Molinos.DataAgro.Entities.Helpers;
using System.Data;
using System.Collections.Generic;
using WebDataAgro.Helpers;
using System.Text;
using System.Globalization;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class ProveedorController : Controller
    {
        private IProveedorManager mobjProveedorManager;
        private IHomeManager mobjHomeManager;
        private ICampañaManager mobjCampañaManager;
        private ILocalidadManager mobjLocalidadManager;

        private IComercialManager mobComercialManager;
        private IReportesManager mobjreportesManager;
        private IProvinciaManager mobjProvinciaManager;
        //-----------------------------------------------------
        //  Constructor
        //-----------------------------------------------------

        public ProveedorController(IProveedorManager oProveedorManager, IHomeManager oHomeManager, ICampañaManager oCampañaManager, IComercialManager oComercialManager, IReportesManager oReportesManager, ILocalidadManager oLocalidadManager, IProvinciaManager oProvinciaManager)
        {

            mobjProveedorManager = oProveedorManager;
            mobjHomeManager = oHomeManager;
            mobjCampañaManager = oCampañaManager;
            mobComercialManager = oComercialManager;
            mobjreportesManager = oReportesManager;
            mobjLocalidadManager = oLocalidadManager;
            mobjProvinciaManager = oProvinciaManager;
        }


        // GET: Contactos
        public ActionResult Index()
        {
            return View();
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteProveedor)]
        public ActionResult ReporteProveedor()
        {
            ViewBag.edita = false;
            return View();
        }


        [Autorizacion(PermisosDataAgro.AltaDatosProveedor, PermisosDataAgro.ModificarDatosProveedor)]
        public ActionResult Agregar(int? ProveedorId)
        {
            ViewBag.ProveedorId = ProveedorId;
            return View();
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

        [Autorizacion(PermisosDataAgro.VisualizarDatosProveedor)]
        public ActionResult Detalle(int ProveedorId, bool? Agenda)
        {
            string ActionView = "";
            bool mostrarEditar = true;


            if (!mobComercialManager.ComercialExiste(GlobalVariables.IdActiveDirectory))
            {
                ActionView = "ErrorDePermisos";
            }

            ViewBag.MostrarEditar = mostrarEditar;
            ViewBag.MostrarAgenda = Agenda;
            ViewBag.ProveedorId = ProveedorId;
            return View(ActionView);
        }

        public ActionResult TraerProveedor(int ProveedorId)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            return new JsonResult()
            {
                Data = mobjProveedorManager.TraerProveedor(ProveedorId, GlobalVariables.IdActiveDirectory, equipo),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult Iniciliazar(int proveedorId)
        {
            var datos = mobjProveedorManager.TraerDatosCombo(proveedorId, PermisosHelper.Is(PermisosDataAgro.FiltrarAdministrativo));

            if (!PermisosHelper.Is(PermisosDataAgro.SubidaArchivosKMZ))
            {
                datos.comercial = datos.comercial.Where(a => a.ComercialId == GlobalVariables.ComercialId).ToList();
            }
            return new JsonResult()
            {
                Data = datos,
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
                model = mobjProveedorManager.UpdateProveedor(oParam, GlobalVariables.IdActiveDirectory, GlobalVariables.Equipo, GlobalVariables.ComercialId);
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
        public JsonResult BuscarCorredores(string filtro, int corredor)
        {
            return Json(mobjProveedorManager.DevolverProveedores(filtro, 1, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarProveedoresConCorredor(string filtroProveedor, string filtro)
        {
            if (filtro == "")
            {
                return Json(mobjProveedorManager.DevolverProveedores(filtroProveedor, 0, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);
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
        public JsonResult BuscarProveedores(string filtroProveedor)
        {

            return Json(mobjProveedorManager.DevolverProveedores(filtroProveedor, 2, GlobalVariables.Equipo), JsonRequestBehavior.AllowGet);

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

        [Autorizacion(PermisosDataAgro.SubidaArchivosKMZ)]
        public ActionResult ImportarEstablecimientos()
        {
            return View();
        }

        [HttpPost]
        [Autorizacion(PermisosDataAgro.SubidaArchivosKMZ)]
        public ActionResult ImportarEstablecimientos(string file)
        {
            var dsExcel = ExcelHelper.LeerExcelDesdeHttpRequest(Request);
            if (dsExcel != null && dsExcel.Tables.Count > 0 && dsExcel.Tables[0].Rows.Count > 0)
            {
                var dtExcel = dsExcel.Tables[0];
                var resultado = new Resultado();
                var campos = ValidarExcel(dtExcel, resultado);
                if (campos.Count > 0)
                {
                    mobjProveedorManager.ImportarEstablecimientos(campos, resultado);
                }

                ViewBag.resultado = resultado;
            }
            return View();
        }

        private List<CampoDetalleDto> ValidarExcel(DataTable dtExcel, Resultado resultado)
        {
            var proveedores = mobjProveedorManager.ListarProveedorTodos();
            var comerciales = mobComercialManager.TraerTodoComercial();
            var localidades = mobjLocalidadManager.ListarLocalidadTodas();
            var campos = new List<CampoDetalleDto>();
            var rows = dtExcel.AsEnumerable().Select(x => x.ItemArray);
            var i = 0;
            foreach (var r in rows.AsEnumerable().Skip(1))
            {
                string cuit2 = r[3].ToString();

                bool error = false;
                i++;
                CampoDetalleDto campo = new CampoDetalleDto();

                if (r[0] != null && r[0].GetType().Equals(typeof(double)))
                {
                    campo.ImportId = int.Parse(r[0].ToString());
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna ID fila: " + i + ". El ID no es numerico. " + r[0].ToString() });
                    error = true;
                }
                if (error == false)
                {
                    if (!String.IsNullOrEmpty(r[3].ToString()) && r[3].ToString().GetType().Equals(typeof(System.String)))
                    {
                        string cuit = r[3].ToString().Replace("-", "").Trim();
                        var proveedor = proveedores.Where(a => a.CUIT == cuit).OrderByDescending(a => a.SegmentacionId).FirstOrDefault();
                        if (proveedor != null)
                        {
                            campo.proveedorId = proveedor.ProveedorId;
                        }
                        else
                        {
                            resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: CUIT fila: " + i + ". No se encontro el cuit. " + r[3].ToString() });
                            error = true;
                        }
                    }
                    else
                    {
                        resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: CUIT fila: " + i + ". " + r[3].ToString() });
                        error = true;
                    }
                }


                campo.nombre = r[4].ToString();

                string provinciaNom = r[5].ToString().ToUpper().RemoveDiacritics().Trim();
                string departamentoNom = r[6].ToString().ToUpper().RemoveDiacritics().Trim();
                string localidadNom = r[7].ToString().ToUpper().RemoveDiacritics().Trim();

                if (error == false)
                {
                    var localidad = localidades.Where(a => a.Provincia_Nombre.ToUpper().Trim() == provinciaNom && a.Nombre.ToUpper().Trim() == localidadNom && a.Partido_Nombre == departamentoNom).FirstOrDefault();
                    if (localidad != null)
                    {
                        campo.localidad = localidad.LocalidadId;
                    }
                    else
                    {
                        localidad = localidades.Where(a => a.Provincia_Nombre.ToUpper().Trim() == provinciaNom && a.Nombre.ToUpper().Trim() == localidadNom).FirstOrDefault();
                        if (localidad != null)
                        {
                            campo.localidad = localidad.LocalidadId;
                        }
                        else
                        {
                            resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Localidad fila: " + i + ". No se encontro la Localidad. " + provinciaNom + " - " + localidadNom });
                            error = true;
                        }

                    }
                }

                campo.latitud = r[8].ToString().Trim();
                campo.longitud = r[9].ToString().Trim();


                if (!String.IsNullOrEmpty(r[10].ToString()) && r[10].ToString().GetType().Equals(typeof(System.String)))
                {
                    string comercialNom = r[10].ToString().ToUpper().RemoveDiacritics().Trim();
                    var comercial = comerciales.Comercial.Where(a => a.Apellido.ToUpper().Trim() == comercialNom).FirstOrDefault();
                    if (comercial != null)
                    {
                        campo.comercialId = comercial.ComercialId;
                    }
                    else
                    {
                        resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Comercial fila: " + i + ". No se encontro el Comercial. " + r[10].ToString() });
                        //error = true;
                    }
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Comercial fila: " + i + ". " + r[10].ToString() });
                    //error = true;
                }

                //if (r[12] != null && r[12].GetType().Equals(typeof(double)))
                //{
                //    campo.rinde = decimal.Parse(r[12].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Message = "Error en la Columna Rinde  fila: " + i + ". No es numerico. " + r[12].ToString() });
                //    //error = true;
                //}

                //if (r[13] != null && r[13].GetType().Equals(typeof(double)))
                //{
                //    campo.rinde = decimal.Parse(r[13].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Rinde  fila: " + i + ". No es numerico. " + r[13].ToString() });
                //    //error = true;
                //}

                if (r[12] != null && r[12].GetType().Equals(typeof(double)))
                {
                    campo.htotales = decimal.Parse(r[12].ToString());
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Hectáreas Totales fila: " + i + ". No es numerico. " + r[12].ToString() });
                    //error = true;
                }

                //if (r[15] != null && r[15].GetType().Equals(typeof(double)))
                //{
                //    campo.hcultivables = decimal.Parse(r[15].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Hectáreas Cultivables fila: " + i + ". No es numerico. " + r[15].ToString() });
                //    //error = true;
                //}


                if (error == false)
                {
                    campos.Add(campo);
                }

            }
            if (campos.Count <= 0)
            {
                resultado.Errores.Add(new ErrorMessage { Message = "SinDatos" });
            }
            return campos;
        }

    }
}
