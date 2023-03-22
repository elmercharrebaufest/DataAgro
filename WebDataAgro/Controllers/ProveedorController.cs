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
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Report;

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
        private readonly IInformeComercialManager mobjInformeComercialManager;
        private readonly IRepositorio repositorio;

        public ProveedorController(IProveedorManager oProveedorManager, IHomeManager oHomeManager,
            ICampañaManager oCampañaManager, IComercialManager oComercialManager,
            IReportesManager oReportesManager, ILocalidadManager oLocalidadManager,
            IProvinciaManager oProvinciaManager, IInformeComercialManager oInformeComercialManager, IRepositorio repositorio)
        {

            mobjProveedorManager = oProveedorManager;
            mobjHomeManager = oHomeManager;
            mobjCampañaManager = oCampañaManager;
            mobComercialManager = oComercialManager;
            mobjreportesManager = oReportesManager;
            mobjLocalidadManager = oLocalidadManager;
            mobjProvinciaManager = oProvinciaManager;
            mobjInformeComercialManager = oInformeComercialManager;
            this.repositorio = repositorio;
        }


        // GET: Contactos
        public ActionResult Index()
        {
            return View();
        }

        [Autorizacion(PermisosDataAgro.VisualizarReporteProveedor)]
        public ActionResult ReporteProveedor(string valor)
        {
            ViewBag.filtro = valor;
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
            ViewBag.Deshabilitado = ProveedorId != null ? mobjProveedorManager.MostrarProveedorDeshabilitado(ProveedorId) : false;
            ViewBag.MostrarEditar = mostrarEditar;
            ViewBag.MostrarAgenda = Agenda;
            ViewBag.ProveedorId = ProveedorId;
            return View(ActionView);
        }

        public ActionResult TraerProveedor(int ProveedorId)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            var data = mobjProveedorManager.TraerProveedor(ProveedorId, GlobalVariables.IdActiveDirectory, equipo);

            return new JsonResult()
            {
                Data = data,
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

        public ActionResult ValidarCategoriaSISA(CuitSegmentacion cuitSegmentacion)
        {
            return new JsonResult()
            {
                Data = mobjProveedorManager.ValidarCategoriaSISA(cuitSegmentacion),
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult GrabarProveedor(NuevoProveedor oParam, CampaniaDto modificados)
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

            if (modificados != null && modificados.ComercialId != null)
            {
                var informeId = repositorio.Listar<InformeComercial, int>(x => x.InformeComercialId, x => x.CampañaId != null && x.ProveedorId == model.ProveedorId && modificados.CampaniaId.Contains((int)x.CampañaId));
                var produccionPorProve = repositorio.Listar<Campo, int>(x => x.CampoId, x => x.ProveedorId == model.ProveedorId);
                var acopiosPorProve = repositorio.Listar<Acopio, int>(x => x.AcopioId, x => x.ProveedorId == model.ProveedorId);
                var campoMaterial = repositorio.Listar<CampoMaterial, CampoMaterialDto>(x => new CampoMaterialDto { MaterialId = x.MaterialId, CampoId = (int)x.CampoId }, x => x.CampoId.HasValue);
                var acopioMaterial = repositorio.Listar<AcopioMaterial>();
                var materialesCampania = new List<int>();
                for (int i = 0; i < produccionPorProve.Count(); i++)
                {
                    if (campoMaterial != null && campoMaterial.Count() > 0)
                    {
                        materialesCampania.AddRange(campoMaterial.Where(x => x.CampoId == produccionPorProve[i]).Select(x => x.MaterialId));
                    }
                }
                for (int i = 0; i < acopiosPorProve.Count(); i++)
                {
                    if (acopioMaterial != null)
                    {
                        materialesCampania.AddRange(acopioMaterial.Where(x => x.AcopioId == acopiosPorProve[i]).Select(x => x.MaterialId));
                    }
                }

                var matCampania = materialesCampania.Distinct().ToList();

                //borrar los informes que existan para las campañas que estoy recibiendo
                foreach (var i in informeId)
                {
                    var resultEliminar = mobjInformeComercialManager.EliminarInformes(i);
                }

                //generar nuevos informes para las campañas que estoy recibiendo
                for (var i = 0; i < modificados.CampaniaId.Distinct().Count(); i++)
                {
                    var paramGrabar = new ParamInformeComercial
                    {
                        Campaña = modificados.CampaniaDesc[i],
                        CampañaId = modificados.CampaniaId[i],
                        ProveedorId = (int)model.ProveedorId,
                        Materiales = new List<ParamInformeComercialMaterial>(),
                        ComercialId = (int)modificados.ComercialId
                    };
                    for (var j = 0; j < matCampania.Count(); j++)
                    {
                        paramGrabar.Materiales.Add(new ParamInformeComercialMaterial { MaterialId = matCampania[j] });
                    }

                    var resultGrabar = mobjInformeComercialManager.GrabarInformeComercial(paramGrabar, (int)modificados.ComercialId, null, null, null, "", "", 0);

                    if (!resultGrabar.HayErrores)
                    {
                        var oLstInformeComercial = new LstInformeComercial(mobjreportesManager);

                        var datos = mobjInformeComercialManager.GenerarInformeComercial(paramGrabar, (int)resultGrabar.InformeId);

                        var identif = oLstInformeComercial.GenerarListadoAsync(datos).Result;

                        model.DownloadKey.Add(Util.GetDownloadKey(identif));

                        mobjInformeComercialManager.EnviarMailInformeComercial(identif);
                    }
                    else
                    {
                        model.Errores = resultGrabar.Errores;
                    }
                }
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
                MaxJsonLength = Int32.MaxValue,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }
        public JsonResult BuscarCorredores(string filtro, int corredor, int? agenteCompraId)
        {
            return Json(mobjProveedorManager.DevolverProveedores(filtro, 1, GlobalVariables.Equipo, agenteCompraId), JsonRequestBehavior.AllowGet);
        }
        public JsonResult BuscarProveedoresConCorredor(string filtroProveedor, string filtro, int? agenteCompraId)
        {
            if (filtro == "")
            {
                return Json(mobjProveedorManager.DevolverProveedores(filtroProveedor, 0, GlobalVariables.Equipo, agenteCompraId), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(mobjProveedorManager.DevolverProveedoresConCorredor(filtroProveedor, filtro), JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult BuscarProveedoresEnSugerencia(string filtro, string filtroProveedor, int? agenteCompraId)
        {
            var resultado = Json(mobjHomeManager.BusquedaHome(filtroProveedor, GlobalVariables.ComercialId, GlobalVariables.Equipo, GlobalVariables.CorredoresComercial), JsonRequestBehavior.AllowGet);
            return resultado;
        }

        public JsonResult BuscarProveedor(string filtroProveedor, bool esComisionista = false, string cuitProveedor = "")
        {
            return Json(mobjProveedorManager.DevolverProveedoresCorredores(filtroProveedor, esComisionista, cuitProveedor), JsonRequestBehavior.AllowGet);
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

            return Json(mobjProveedorManager.DevolverProveedores(filtroProveedor, 2, GlobalVariables.Equipo, null), JsonRequestBehavior.AllowGet);

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
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna ID fila: <b>" + i + "</b>. El ID no es numerico. " + r[0].ToString() });
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
                            resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: CUIT fila: <b>" + i + "</b>. No se encontro el cuit. " + r[3].ToString() });
                            error = true;
                        }
                    }
                    else
                    {
                        resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: CUIT fila: <b>" + i + "</b>. " + r[3].ToString() });
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
                        campo.localidadId = localidad.LocalidadId;
                    }
                    else
                    {
                        localidad = localidades.Where(a => a.Provincia_Nombre.ToUpper().Trim() == provinciaNom && a.Nombre.ToUpper().Trim() == localidadNom).FirstOrDefault();
                        if (localidad != null)
                        {
                            campo.localidadId = localidad.LocalidadId;
                        }
                        else
                        {
                            resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Localidad fila: <b>" + i + "</b>. No se encontro la Localidad. " + provinciaNom + " - " + localidadNom });
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
                        resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Comercial fila: <b>" + i + "</b>. No se encontro el Comercial. " + r[10].ToString() });
                        //error = true;
                    }
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna: Comercial fila: <b>" + i + "</b>. " + r[10].ToString() });
                    //error = true;
                }

                //if (r[12] != null && r[12].GetType().Equals(typeof(double)))
                //{
                //    campo.rinde = decimal.Parse(r[12].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Message = "Error en la Columna Rinde  fila: <b>" + i + "</b>. No es numerico. " + r[12].ToString() });
                //    //error = true;
                //}

                //if (r[13] != null && r[13].GetType().Equals(typeof(double)))
                //{
                //    campo.rinde = decimal.Parse(r[13].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Rinde  fila: <b>" + i + "</b>. No es numerico. " + r[13].ToString() });
                //    //error = true;
                //}

                if (r[12] != null && r[12].GetType().Equals(typeof(double)))
                {
                    campo.htotales = decimal.Parse(r[12].ToString());
                }
                else
                {
                    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Hectáreas Totales fila: <b>" + i + "</b>. No es numerico. " + r[12].ToString() });
                    //error = true;
                }

                //if (r[15] != null && r[15].GetType().Equals(typeof(double)))
                //{
                //    campo.hcultivables = decimal.Parse(r[15].ToString());
                //}
                //else
                //{
                //    resultado.Errores.Add(new ErrorMessage { Source = cuit2, Message = "Error en la Columna Hectáreas Cultivables fila: <b>" + i + "</b>. No es numerico. " + r[15].ToString() });
                //    //error = true;
                //}


                if (error == false)
                {
                    campos.Add(campo);
                }

            }
            if (campos.Count <= 0)
            {
                resultado.Errores.Add(new ErrorMessage { Message = "No se pudo agregar ningun Establecimiento" });
            }
            return campos;
        }

        public ActionResult ExportarActividadesExcel(HistorialActiviad filtro)
        {
            var model = new ReportesModel();

            List<ActividadExportar> datos = mobjProveedorManager.ExportarActividades(filtro, GlobalVariables.IdActiveDirectory);

            var oLstAgendaActividad = new LstAgendaActividad(mobjreportesManager);

            var identif = oLstAgendaActividad.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }
    }
}
