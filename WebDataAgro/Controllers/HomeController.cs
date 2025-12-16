using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Web.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Report;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using WebDataAgro.Atributos;
using WebDataAgro.Core;
using WebDataAgro.Models;
using static WebDataAgro.MvcApplication;

namespace WebDataAgro.Controllers
{
    //[Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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

        public async Task<ActionResult> Index(string code, string error)
        {
            logger.Debug("Home/Index called with error: " + error);
            if (!string.IsNullOrEmpty(error))
                return Content("Error: " + error);

            // Si viene el "code" desde Azure → canjeamos por token
            if (!string.IsNullOrEmpty(code))
            {
                var token = await ExchangeCodeForToken(code);
                if (token == null)
                    return Content("Error al obtener el token.");

                var email = GetEmailFromAccessToken(token.AccessToken);
                logger.Debug("Create session for email: " + email);
                CrearOActualizarSesion(email);

                logger.Debug("User session created for email: " + email);
                return RedirectToAction("Index");
            }

            return View();
        }

        private void CrearOActualizarSesion(string email)
        {
            // Tus roles desde DB o servicio
            var roles = comercialManager.ObtenerPermisosPorEmail(email);
            var comercial = comercialManager.TraerComercial(email);
            // Crear identidad con claims
            var identity = new ClaimsIdentity(
                CookieAuthenticationDefaults.AuthenticationType,
                System.IdentityModel.Claims.ClaimTypes.Email,
                ClaimTypes.Role
            );

            identity.AddClaim(new Claim(ClaimTypes.Email, email));
            identity.AddClaim(new Claim(ClaimTypes.Name, email));

            foreach (var rol in roles)
                identity.AddClaim(new Claim(ClaimTypes.Role, rol));

            var equipo = comercialManager.ListarEquipo(comercial.IdActiveDirectory, roles);
            identity.AddClaim(new Claim("perfil", JsonConvert.SerializeObject(comercialManager.ObtenerPerfilDeUsuario(comercial.IdActiveDirectory))));
            identity.AddClaim(new Claim("esAdministrador", JsonConvert.SerializeObject(comercialManager.EsAdministrador(comercial.IdActiveDirectory))));
            identity.AddClaim(new Claim("EsCupera", JsonConvert.SerializeObject(comercialManager.EsCupera(comercial.IdActiveDirectory))));
            identity.AddClaim(new Claim("comercialId", JsonConvert.SerializeObject(comercial.ComercialId)));
            identity.AddClaim(new Claim("equipo", JsonConvert.SerializeObject(equipo.Equipo)));
            identity.AddClaim(new Claim("equipoReal", JsonConvert.SerializeObject(equipo.EquipoReal)));
            identity.AddClaim(new Claim("IdActiveDirectory", JsonConvert.SerializeObject(comercial.IdActiveDirectory)));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, comercial.IdActiveDirectory));
            identity.AddClaim(new Claim("IdActiveDirectoryCompleto", JsonConvert.SerializeObject("molinosagro\\" + comercial.IdActiveDirectory)));
            identity.AddClaim(new Claim("corredoresComercial", JsonConvert.SerializeObject(comercialManager.ListarCorredoresComercial(roles))));

            // Loguear: emitir cookie OWIN
            var ctx = HttpContext.GetOwinContext();
            var auth = ctx.Authentication;

            auth.SignIn(new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            }, identity);
        }

        private string GetEmailFromAccessToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);

            // 1) Email si existe
            var email = jwt.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
            if (!string.IsNullOrEmpty(email))
                return email;

            // 2) preferred_username (muy común)
            email = jwt.Claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value;
            if (!string.IsNullOrEmpty(email))
                return email;

            // 3) upn como fallback
            email = jwt.Claims.FirstOrDefault(c => c.Type == "upn")?.Value;
            return email;
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            string tenantId = ConfigurationManager.AppSettings["AzureAd_TenantId"];
            string clientId = ConfigurationManager.AppSettings["AzureAd_ClientId"];
            var request = HttpContext.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority;

            string url =
                $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/authorize" +
                $"?client_id={clientId}" +
                $"&response_type=code" +
                $"&redirect_uri={HttpUtility.UrlEncode(baseUrl)}" +
                $"&response_mode=query" +
                $"&scope=openid%20email%20profile%20offline_access" +
                $"&prompt=select_account";

            return Redirect(url);
        }

        private async Task<TokenResponse> ExchangeCodeForToken(string code)
        {
            var request = HttpContext.Request;
            string baseUrl = request.Url.Scheme + "://" + request.Url.Authority;
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                    { "client_id", ConfigurationManager.AppSettings["AzureAd_ClientId"] },
                    { "client_secret", ConfigurationManager.AppSettings["AzureAd_ClientSecret"] },
                    { "grant_type", "authorization_code" },
                    { "code", code },
                    { "redirect_uri", baseUrl }
                };

                var response = await client.PostAsync(
                    $"https://login.microsoftonline.com/{ConfigurationManager.AppSettings["AzureAd_TenantId"]}/oauth2/v2.0/token",
                    new FormUrlEncodedContent(values));

                var json = await response.Content.ReadAsStringAsync();

                // Log full response details
                logger.Debug("Token response StatusCode: " + response.StatusCode);
                logger.Debug("Token response ReasonPhrase: " + response.ReasonPhrase);
                logger.Debug("Token response Headers: " + JsonConvert.SerializeObject(response.Headers.ToDictionary(h => h.Key, h => h.Value)));
                logger.Debug("Token response ContentHeaders: " + JsonConvert.SerializeObject(response.Content.Headers.ToDictionary(h => h.Key, h => h.Value)));


                return JsonConvert.DeserializeObject<TokenResponse>(json);
            }
        }

        public class TokenResponse
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
        }

        public class GraphUser
        {
            public string Id { get; set; }
            public string UserPrincipalName { get; set; }
            public string Mail { get; set; }
            public string DisplayName { get; set; }
        }
        public ActionResult Logout()
        {
            //Cerrar cookie OWIN de autenticación
            var auth = HttpContext.GetOwinContext().Authentication;
            auth.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return RedirectToAction("Login", "Home");
        }
        public ActionResult ErrorDePermisos()
        {

            return View("ErrorDePermisos");
        }

        public ActionResult ErrorUsuarioSinDerechos()
        {

            return View("ErrorUsuarioSinDerechos");
        }
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
        public ActionResult BusquedaHome(string filtro)
        {
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? GlobalVariables.EquipoReal : PermisosHelper.Is(PermisosDataAgro.ProveedorZonaPropia) ? mobjHomeManager.ListarTodosLosComercialesConMismaZona(GlobalVariables.ComercialId) : GlobalVariables.Equipo;
            return new JsonResult()
            {
                Data = mobjHomeManager.BusquedaHome(filtro, GlobalVariables.ComercialId, equipo, GlobalVariables.CorredoresComercial),
                MaxJsonLength = Int32.MaxValue
            };

        }
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
        public ActionResult EliminarObjetivo(int id)
        {
            var resultado = objetivoManager.EliminarObjetivo(id);
            return new JsonResult()
            {
                Data = resultado,
                MaxJsonLength = Int32.MaxValue
            };
        }
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
        public ActionResult BorrarCookie()
        {
            return View();
        }

        [AjaxOnly]
        public void BorrarCookies()
        {
            string Email = HttpContext.User.Identity.Name;

            CrearOActualizarSesion(Email);
        }
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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
        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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

        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
        public ActionResult ExportarCapProdDesactualizadas()
        {
            bool esAdministrador = PermisosHelper.Is(PermisosDataAgro.Administracion_Proveedores);
            List<CapacidadProductivaDesactualizadaDto> ProveedoresConCapProdDesactualizada = mobjHomeManager.ProveedoresConCapProdDesactualizada(GlobalVariables.ComercialId, esAdministrador);
            byte[] archivoBytes = mobjHomeManager.ExportarListadoAXls(ProveedoresConCapProdDesactualizada);
            return File(archivoBytes, "application/vnd.ms-excel", "Informes comerciales faltantes.xls");
        }

        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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

        [Autorizacion(PermisosDataAgro.IngresoDataAgro)]
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