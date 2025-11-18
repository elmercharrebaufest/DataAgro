using Microsoft.Web.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Report;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Services;
using System.Linq;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
    public class HomeController : Controller
    {
        private readonly IHomeManager mobjHomeManager;
        private readonly IInformeComercialAperturaManager mobjInformeComercialAperturaManager;
        private readonly IObjetivoManager objetivoManager;
        private readonly ILogger logger;
        private readonly IComercialManager comercialManager;
        private readonly IReportesManager reportesManager;

        public HomeController(IComercialManager comercialManager, IReportesManager reportesManager,
            IHomeManager homeManager, IObjetivoManager objetivoManager, IInformeComercialAperturaManager mobjInformeComercialAperturaManager, ILogger logger)
        {
            this.comercialManager = comercialManager;
            this.reportesManager = reportesManager;
            this.mobjHomeManager = homeManager;
            this.objetivoManager = objetivoManager;
            this.mobjInformeComercialAperturaManager = mobjInformeComercialAperturaManager;
            this.logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ErrorDePermisos()
        {

            return View("ErrorDePermisos");
        }

        public ActionResult ErrorUsuarioSinDerechos()
        {

            return View("ErrorUsuarioSinDerechos");
        }

        public ActionResult Inicializar(int? comercialId, int? zonaId)
        {
            var model = new ResultIniContactoModel();
            var filtro = new oParamBusqueda
            {
                ComercialId = GlobalVariables.ComercialId,
                Equipo = GlobalVariables.Equipo
            };
            var verTodos = PermisosHelper.Is(PermisosDataAgro.VerTodos);
            var proveedorZonaPropia = PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia);
            var comercialesConMismaZona = mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId);

            logger.Debug($"Inicializar GlobalVariables.ComercialId:{GlobalVariables.ComercialId}, usuario: {GlobalVariables.IdActiveDirectoryCompleto}");
            logger.Debug($"PermisosHelper.Is(PermisosDataAgro.VerTodos): {verTodos}");
            logger.Debug($"GlobalVariables.EquipoReal: {GlobalVariables.EquipoReal.ToJson()}");
            logger.Debug($"GlobalVariables.Equipo: {GlobalVariables.Equipo.ToJson()}");
            logger.Debug($"PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia): {proveedorZonaPropia}");
            logger.Debug($"ListarTodosLosComercialesConMismaZona: {comercialesConMismaZona.ToJson()}");

            var equipo = verTodos ? GlobalVariables.EquipoReal :
                proveedorZonaPropia ? comercialesConMismaZona :
                GlobalVariables.Equipo;

            logger.Debug($"Equipo final tomado según permisos: {equipo.ToJson()}");

            var result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, equipo);
            model.Objetivo = mobjHomeManager.TraerInfoObjetivo(comercialId, equipo, zonaId, GlobalVariables.ComercialId);
            model.Datos = mobjHomeManager.TraerInfoIniciales(equipo);
            model.Detalle = mobjHomeManager.TraerTodoCompraDetalle(equipo, comercialId, zonaId);
            //model.Campaña = mobjHomeManager.TraerInfoCampaña(GlobalVariables.ComercialId, equipo);
            model.Campaña = new CampañaHome();
            if (result != null)
            {
                model.Contactos = result;
            }
            if (model.Detalle != null)
            {
                foreach (var item in model.Detalle.GroupBy(a => a.Material))
                {
                    model.Campaña.Materiales.Add(
                        new MaterialCampaña
                        {
                            Campaña = item.First().Campana,
                            Nombre = item.First().Material,
                            Toneladas = item.First().TotalCompra
                        });
                }
            }
            //if (model.Campaña.Materiales != null)
            //{
            //    foreach (var item in model.Campaña.Materiales)
            //    {
            //        var det = model.Detalle.Find(x => x.Campana == item.Campaña && x.Material == item.Nombre);
            //        item.Toneladas = det.ConCorredor.ARecibirAFijar + det.ConCorredor.ComprasConPrecio + det.ConCorredor.FasonFas + det.ConCorredor.RecibidoSinPrecio
            //            + det.DirectoAcopiador.ARecibirAFijar + det.DirectoAcopiador.ComprasConPrecio + det.DirectoAcopiador.FasonFas + det.DirectoAcopiador.RecibidoSinPrecio
            //            + det.DirectoProductor.ARecibirAFijar + det.DirectoProductor.ComprasConPrecio + det.DirectoProductor.FasonFas + det.DirectoProductor.RecibidoSinPrecio;
            //    }
            //}

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BusquedaHome(string filtro)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;
            return new JsonResult()
            {
                Data = mobjHomeManager.BusquedaHome(filtro, GlobalVariables.ComercialId, equipo, GlobalVariables.CorredoresComercial),
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerBusquedaContacto(oParamBusqueda filtro, int pagina)
        {
            var model = new ResultIniContactoModel();

            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;


            ResultIniContacto result;
            if (!PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, filtro.Equipo);
            }
            else
            {
                result = mobjHomeManager.TraerBusquedaContacto(filtro, 1, GlobalVariables.CorredoresComercial);
            }

            if (result != null)
            {
                model.Contactos = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult TraerActividadesPorComercialId()
        {
            var model = new ResultActividadesModel();

            var result = mobjHomeManager.TraerActividadesPorComercialId(GlobalVariables.ComercialId);

            if (result != null)
            {
                model.Actividades = result;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        [Autorizacion(PermisosDataAgro.DescargaPdf)]
        public ActionResult ExportarContactosPDF(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;


            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory, filtro.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarListado(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }

        [Autorizacion(PermisosDataAgro.DescargaExcel)]
        public ActionResult ExportarContactosExcel(oParamBusqueda filtro)
        {
            var model = new ReportesModel();
            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;

            var datos = mobjHomeManager.ExportarContactos(filtro, GlobalVariables.IdActiveDirectory, filtro.Equipo);

            var oLstContacto = new LstContacto(reportesManager);

            var identif = oLstContacto.GenerarExcel(datos);

            model.DownloadKey = Util.GetDownloadKey(identif);

            return Json(model);
        }
        [Autorizacion(PermisosDataAgro.DescargaExportAllComercial, PermisosDataAgro.DescargaExportAllVisualizador)]
        public ActionResult ExportarAll(oParamBusqueda filtro)
        {
            logger.Info("inicio export all");
            var model = new ReportesModel();

            filtro.ComercialId = GlobalVariables.ComercialId;
            filtro.Equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;
            filtro.Estado = null;
            try
            {
                var datos = mobjHomeManager.ExportarAll(filtro, GlobalVariables.IdActiveDirectory, filtro.Equipo);
                var oLstContacto = new LstContacto(reportesManager);
                logger.Info("inicio export idnetif");
                var identif = oLstContacto.GenerarExcelExportAll(datos);
                logger.Info("inicio export download");
                model.DownloadKey = Util.GetDownloadKey(identif);
                logger.Info("inicio export fin try");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
            logger.Info("fin export all");
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult TraerPostIt()
        {
            var model = new ResultIniPostItModel();

            var result = mobjHomeManager.TraerTexto(GlobalVariables.ComercialId);

            if (result != null)
            {
                model.Texto = result.Texto;
            }

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }

        public ActionResult GuardarPostIt(PostIt post)
        {
            var model = new GrabarPostItResult();

            post.ComercialId = GlobalVariables.ComercialId;

            if (post.ComercialId != 0)
            {
                model = mobjHomeManager.GuardarPostIt(post);
            }


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult GuardarObjetivoComercial(ObjetivoComercial objetivo)
        {
            var model = new Resultado();

            objetivo.ComercialId = GlobalVariables.ComercialId;

            if (objetivo.ComercialId != 0)
            {
                model = objetivoManager.GuardarObjetivo(objetivo);
            }


            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };

        }
        public ActionResult TraerObjetivos(int? comercialId, int? zonaId)
        {
            var model = new ResultIniContactoModel();
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            model.Objetivo = mobjHomeManager.TraerInfoObjetivo(comercialId, equipo, zonaId, GlobalVariables.ComercialId);

            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult EliminarObjetivo(int id)
        {
            var resultado = objetivoManager.EliminarObjetivo(id);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult BorrarCookie()
        {
            return View();
        }

        [AjaxOnly]
        public void BorrarCookies()
        {
            if (FederatedAuthentication.SessionAuthenticationModule != null)
            {
                FederatedAuthentication.SessionAuthenticationModule.DeleteSessionTokenCookie();
            }
        }

        public ActionResult TraerCompras(int? comercialId, int? zonaId)
        {
            var model = new ResultIniContactoModel();
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : GlobalVariables.Equipo;
            model.Detalle = mobjHomeManager.TraerTodoCompraDetalle(equipo, comercialId, zonaId);
            model.Campaña = new CampañaHome();
            if (model.Detalle != null)
            {
                foreach (var item in model.Detalle.GroupBy(a => a.Material))
                {
                    model.Campaña.Materiales.Add(
                        new MaterialCampaña
                        {
                            Campaña = item.First().Campana,
                            Nombre = item.First().Material,
                            Toneladas = item.First().TotalCompra
                        });
                }
            }
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        #region Informes Comerciales
        public ActionResult VerificarInformesComerciales()
        {
            bool esAdministrador = PermisosHelper.Is(PermisosDataAgro.Administracion_Proveedores);
            List<CapacidadProductivaDesactualizadaDto> ProveedoresConCapProdDesactualizada = mobjHomeManager.ProveedoresConCapProdDesactualizada(GlobalVariables.ComercialId, esAdministrador);
            return new JsonResult()
            {
                Data = ProveedoresConCapProdDesactualizada,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult ExportarCapProdDesactualizadas()
        {
            bool esAdministrador = PermisosHelper.Is(PermisosDataAgro.Administracion_Proveedores);
            List<CapacidadProductivaDesactualizadaDto> ProveedoresConCapProdDesactualizada = mobjHomeManager.ProveedoresConCapProdDesactualizada(GlobalVariables.ComercialId, esAdministrador);
            byte[] archivoBytes = mobjHomeManager.ExportarListadoAXls(ProveedoresConCapProdDesactualizada);
            return File(archivoBytes, "application/vnd.ms-excel", "Informes comerciales faltantes.xls");
        }

        public ActionResult GrabarInformeComercialApertura()
        {
            InformeComercialApertura informeComercialApertura = new InformeComercialApertura();
            informeComercialApertura.ComercialId = GlobalVariables.ComercialId;
            informeComercialApertura.FechaApertura = DateTime.Now.Date.AddDays(1);
            var model = new Resultado();
            model = mobjInformeComercialAperturaManager.GrabarInformeComercialApertura(informeComercialApertura);
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }

        public ActionResult TraerInformeComercialApertura()
        {
            var model = new InformeComercialAperturaDto();
            model = mobjInformeComercialAperturaManager.TraerInformeComercialApertura(GlobalVariables.ComercialId);
            return new JsonResult()
            {
                Data = model,
                MaxJsonLength = Int32.MaxValue
            };
        }
        #endregion

    }
}