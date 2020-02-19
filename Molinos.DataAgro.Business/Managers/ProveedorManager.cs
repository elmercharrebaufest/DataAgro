using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Molinos.DataAgro.Business.Managers
{
    public class ProveedorManager : IProveedorManager
    {
        private readonly IRepositorio repositorio;
        private readonly IComercialManager mobComercial;
        private readonly IRiesgoComercialAgent oRiesgoComercialAgent;
        private readonly IDatosProveedorAgent oDatosProveedorAgent;
        private readonly IMailManager mailManager;
        private readonly ILogger logger;

        public ProveedorManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, 
            IRiesgoComercialAgent oRiesgoComercialAgent, IDatosProveedorAgent oDatosProveedorAgent,
            IMailManager mailManager)
        {
            this.logger = logger;
            mobComercial = oComercial;
            this.oRiesgoComercialAgent = oRiesgoComercialAgent;
            this.oDatosProveedorAgent = oDatosProveedorAgent;
            this.mailManager = mailManager;
            this.repositorio = repositorio;
        }

        public StoredHistorialResult TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string actividadId)
        {
            return new StoredHistorialResult
            {
                ActividadHistoriaTraerPorProveedores = repositorio.ListarConsulta(new ConsultaActividadHistoriaTraerPorProveedorId(ProveedorId, false, oParam.detalle, actividadId))
                //ActividadHistoriaTraerPorProveedores = repositorio.SelStore<HistorialTraer>("DataAgro_ActividadHistoriaTraerPorProveedorId", 0, ProveedorId, oParam.detalle, TipoActividadId)
            };
        }

        public StoredPorProveedorResult TraerProveedor(int ProveedorId, string UsuarioDirectory, List<int> equipo)
        {
            var res = new StoredPorProveedorResult();
            try
            {
                var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == UsuarioDirectory);
                res.BasicoProveedorTraerPorProveedores = TraerDatosBasicosProveedor(ProveedorId, oComerciales.ComercialId, equipo);
                res.ActividadTraerPorProveedores = repositorio.ListarConsulta(new ConsultaActividadHistoriaTraerPorProveedorId(ProveedorId, true));
                res.ContactosComercialesTraerPorProveedores = repositorio.SelStore<ContactosComerciales>("DataAgro_ContactosComercialesTraerPorProveedorId", 0, ProveedorId);
                res.ActividadHistoriaTraerPorProveedores = repositorio.ListarConsulta(new ConsultaActividadHistoriaTraerPorProveedorId(ProveedorId, false));
                res.ObjetivosTraerPorProveedorId = repositorio.SelStore<ObjetivosTraer>("DataAgro_ObjetivosTraerPorProveedorId", 0, ProveedorId);
                res.AcopioMaterialPorProveedores = repositorio.Listar<AcopioMaterial, AcopioMaterialPorProveedor>(x => new AcopioMaterialPorProveedor
                {
                    AcopioId = x.AcopioId,
                    AcopioMaterialId = x.AcopioMaterialId,
                    CampañaId = x.CampañaId,
                    Campaña = x.Campaña.Descripcion,
                    MaterialId = x.MaterialId,
                    NroItem = x.NroItem,
                    Toneladas = x.Toneladas,
                    Material = x.Material.Descripcion,
                    LocalidadId = x.Acopio.LocalidadId.Value,
                    Localidad = x.Acopio.Localidad.Nombre,
                    ProvinciaId = x.Acopio.Localidad.ProvinciaId,
                    Provincia = x.Acopio.Localidad.Provincia.Nombre
                }, x => x.Acopio.ProveedorId == ProveedorId);
                var campoacopio = repositorio.SelStore<CampoProduccionAcopio>("DataAgro_CampoProduccionAcopioPorProveedorId", 0, ProveedorId);
                res.CampoProduccionAcopioPorProveedores = campoacopio.Where(z => z.EsCampoProduccion == true).ToList();
                res.Acopio = campoacopio.Where(z => z.EsCampoProduccion == false).ToList();

                res.DatosContacto = DevolverDatosContacto(ProveedorId);
                res.Historial = Comprar(ProveedorId, UsuarioDirectory, equipo);
                res.CanalesDeOperacion = repositorio.Listar<ProveedorCanalOperacion, CanalOperacion>(x => x.CanalOperacion, x => x.ProveedorId == ProveedorId);
                res.ProveedorCondicion = repositorio.Listar<ProveedorCondicion, Condicion>(x => x.Condicion, x => x.ProveedorId == ProveedorId);
                res.ProveedorDestinatario = repositorio.Listar<ProveedorDestinatario, Destinatario>(x => x.Destinatario, x => x.ProveedorId == ProveedorId);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }


            foreach (var aux in res.BasicoProveedorTraerPorProveedores)
            {
                aux.NoOperable = false;
                aux.Operando = true;


                if (!String.IsNullOrEmpty(aux.RiesgoComercialSap))
                {
                    if (aux.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                    {
                        aux.NoOperable = true;
                        aux.Operando = false;
                        aux.TooltipNoOperable = "Riesgo Comercial Alto";
                    }
                }

                if (aux.EstadoCuit == 0)
                {
                    aux.NoOperable = true;
                    aux.Operando = false;
                    aux.TooltipNoOperable = "Inactivo";
                }
                if (aux.EstadoCuit == 3)
                {
                    aux.NoOperable = true;
                    aux.Operando = false;
                    aux.TooltipNoOperable = "Estado 3";
                }
                if (aux.Facacop == 1)
                {
                    aux.Operando = false;
                    aux.NoOperable = true;
                    aux.TooltipNoOperable = "Apocrifos";
                }
            }

            return res;
        }
        public DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(string CUIT)
        {
            return repositorio.Obtener<Proveedor, DatosLocalidadProvincia>(x => x.CUIT == CUIT && x.Localidad != null && x.Provincia != null,
                x => new DatosLocalidadProvincia
                {
                    ProveedorId = x.ProveedorId,
                    CUIT = x.CUIT,
                    RazonSocial = x.RazonSocial,
                    LocalidadId = x.LocalidadId.Value,
                    ProvinciaId = x.ProvinciaId.Value,
                    Localidad = x.Localidad.Nombre,
                    Provincia = x.Provincia.Nombre
                });
        }
        public DatosLocalidadProvincia TraerLocalidadProveedorPorCuit(DatosLocalidadProvinciaFiltro oDatosLocalidadProvinciaFiltro)
        {
            var valor = new DatosLocalidadProvincia();
            try
            {

                var oProveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == oDatosLocalidadProvinciaFiltro.CUIT);

                if (oProveedor.ProvinciaCompraNet != null && oProveedor.LocalidadCompraNet != null)
                {
                    valor = repositorio.Obtener<Localidad, DatosLocalidadProvincia>(
                        x => x.LocalidadId == oProveedor.LocalidadCompraNet.LocalidadId,
                        x => new DatosLocalidadProvincia
                        {
                            Localidad = x.Nombre,
                            LocalidadId = x.LocalidadId,
                            Provincia = x.Provincia.Nombre,
                            ProvinciaId = x.Provincia.ProvinciaId,
                        });
                }
                else
                {
                    valor = repositorio.Obtener<CampoMaterial, DatosLocalidadProvincia>(x => x.Campo.Proveedor.ProveedorId == oProveedor.ProveedorId && x.CampañaId == oDatosLocalidadProvinciaFiltro.CampanaId && x.MaterialId == oDatosLocalidadProvinciaFiltro.MaterialId, x => new DatosLocalidadProvincia
                    {
                        Localidad = x.Campo.Localidad.Nombre,
                        LocalidadId = x.Campo.Localidad.LocalidadId,
                        Provincia = x.Campo.Localidad.Provincia.Nombre,
                        ProvinciaId = x.Campo.Localidad.Provincia.ProvinciaId,
                    });
                }

                valor = valor ?? new DatosLocalidadProvincia();
                valor.CUIT = oProveedor.CUIT;
                valor.ProveedorId = oProveedor.ProveedorId;
                valor.RazonSocial = oProveedor.RazonSocial;
                valor.ClasificacionId = oProveedor.ClasificacionCompraNetId ?? 0;
                valor.Consignatario = oProveedor.Consignatario ?? false;
                valor.BoletoId = oProveedor.BoletoCompraNetId ?? 0;
                valor.BolsaId = oProveedor.BolsaCompraNetId ?? 0;
                valor.Corredor = oProveedor.SegmentacionId == 5 || oProveedor.SegmentacionId == 7 ? true : false;
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return valor;
        }
        private DatosContacto DevolverDatosContacto(int proveedorId)
        {
            return repositorio.Obtener<Proveedor, DatosContacto>(x => x.ProveedorId == proveedorId, x => new DatosContacto()
            {
                Direccion = x.Direccion,
                CodigoPostal = x.CodigoPostal,
                Intermediario = x.Intermediario
            });
        }
        public Resultado GrabarRecordatorio(ActividadInsetarIni oParam)
        {
            var oEntityErrors = new Resultado();

            Actividad oActividadSave = oParam.ActividadId == 0
                ? new Actividad()
                : repositorio.Obtener<Actividad>(x => x.ActividadId == oParam.ActividadId) ?? new Actividad();
            oActividadSave.ComercialId = oParam.ComercialId;
            oActividadSave.ContactoComercialId = oParam.contacto;
            oActividadSave.Detalle = oParam.detalle;
            oActividadSave.FechaHoraActividad = oParam.fechaYHoraActividad;
            oActividadSave.FechaHoraRecordatorio = oParam.fechaYHoraRecordatorio;
            oActividadSave.ProveedorId = oParam.ProveedorId;
            oActividadSave.TipoActividadId = oParam.tipoactividad;
            oActividadSave.asunto = oParam.asunto;
            oActividadSave.FechaHoraRecordatorioFin = oParam.fechaYHoraRecordatorioFin;

            repositorio.Agregar(oActividadSave);

            var proveedor = repositorio.Obtener<Proveedor>(oParam.ProveedorId);
            proveedor.FechaUltimoContacto = DateTime.Now;

            try
            {
                repositorio.GuardarCambios();

                if (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                {
                    EnviarCita(oParam);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }
        private void EnviarCita(ActividadInsetarIni oParam)
        {
            try
            {
                var oMensaje = new MailMessage();

                oMensaje.From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]);

                if (ConfigurationManager.AppSettings["EmailAgenda"] == "1")
                {
                    oMensaje.To.Add(ConfigurationManager.AppSettings["maildeUsuarios"]);
                }
                else
                {
                    oMensaje.To.Add(mailManager.GetEmailUserActiveDirectory(oParam.UserName));
                }

                if ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                    || ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaTareas"]))))
                {
                    oMensaje.Subject = oParam.asunto;
                }

                var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
                var comercial = repositorio.Obtener<ContactoComercial>(x => x.ProveedorId == oParam.ProveedorId && x.ContactoComercialId == oParam.contacto);

                oMensaje.Body = "CUIT: " + proveedor.CUIT + "\r\nRazón Social: " + proveedor.RazonSocial;
                if (comercial != null)
                {
                    oMensaje.Body = oMensaje.Body + "\r\nContacto: " + (!string.IsNullOrEmpty(comercial.Nombres) ? comercial.Nombres : "") + " " + (!string.IsNullOrEmpty(comercial.Apellido) ? comercial.Apellido : "");
                    oMensaje.Body = oMensaje.Body + (comercial.Telefono1 != null ? " Tel: " + comercial.Telefono1 : "");
                    oMensaje.Body = oMensaje.Body + (!string.IsNullOrEmpty(comercial.Email1) ? " Email: " + comercial.Email1 : "");
                }
                oMensaje.Body = oMensaje.Body + "\r\nDetalle: " + oParam.detalle;

                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                // Now Contruct the ICS file using string builder
                StringBuilder str = new StringBuilder();
                str.AppendLine("BEGIN:VCALENDAR");
                str.AppendLine("PRODID:-//Schedule a Meeting");
                str.AppendLine("VERSION:2.0");
                str.AppendLine("METHOD:REQUEST");
                str.AppendLine("BEGIN:VEVENT");
                //validar para las llamadas
                if (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"]))
                {
                    str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorio.Value.ToUniversalTime()));
                    str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorio.Value.ToUniversalTime()));
                    str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", oParam.fechaYHoraRecordatorioFin.Value.ToUniversalTime().AddMinutes(+30)));
                }
                else if (oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaTareas"]))
                {
                    str.AppendLine(string.Format("DTSTART:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime()));
                    str.AppendLine(string.Format("DTSTAMP:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime()));
                    str.AppendLine(string.Format("DTEND:{0:yyyyMMddTHHmmssZ}", DateTime.Now.ToUniversalTime().AddMinutes(+30)));
                }

                str.AppendLine("LOCATION: " + "");
                str.AppendLine(string.Format("UID:{0}", Guid.NewGuid()));
                str.AppendLine(string.Format("DESCRIPTION:{0}", oMensaje.Body));
                str.AppendLine(string.Format("X-ALT-DESC;FMTTYPE=text/html:{0}", oMensaje.Body));
                str.AppendLine(string.Format("SUMMARY:{0}", oMensaje.Subject));
                str.AppendLine(string.Format("ORGANIZER:MAILTO:{0}", oMensaje.From.Address));

                str.AppendLine(string.Format("ATTENDEE;CN=\"{0}\";RSVP=TRUE:mailto:{1}", oMensaje.To[0].DisplayName, oMensaje.To[0].Address));

                str.AppendLine("BEGIN:VALARM");
                str.AppendLine("TRIGGER:-PT15M");
                str.AppendLine("ACTION:DISPLAY");
                str.AppendLine("DESCRIPTION:Reminder");
                str.AppendLine("END:VALARM");
                str.AppendLine("END:VEVENT");
                str.AppendLine("END:VCALENDAR");

                System.Net.Mime.ContentType contype = new System.Net.Mime.ContentType("text/calendar");
                contype.Parameters.Add("method", "REQUEST");
                contype.Parameters.Add("name", "Recordatorio.ics");
                AlternateView avCal = AlternateView.CreateAlternateViewFromString(str.ToString(), contype);
                oMensaje.AlternateViews.Add(avCal);

                SmtpClient oCliente = default(SmtpClient);

                int Condicion = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                }
                else
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                }

                if (ConfigurationManager.AppSettings["SmtpAnonimo"] != "S")
                {
                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);
                }

                oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";
                oCliente.Send(oMensaje);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void EnviarEmail(Contrato oContrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string idActiveDirectory, bool? eliminar)
        {
            try
            {
                var id = oContrato.CorredorId.HasValue ? oContrato.CorredorId : oContrato.ProveedorId;
                var proveedorContacto = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == id && x.CompraNet == true);

                string emailComercial = "";

                if (oContrato.Comercial != null)
                {
                    try { emailComercial = mailManager.GetEmailUserActiveDirectory(oContrato.Comercial.IdActiveDirectory); } catch (Exception e) { logger.Error(e); }
                }

                var oMensaje = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                };

                if (proveedorContacto.Count > 0)
                {
                    foreach (var contacto in proveedorContacto)
                    {
                        if (!string.IsNullOrEmpty(contacto.Email1))
                        {
                            oMensaje.To.Add(contacto.Email1);
                        }
                    }
                    if (!string.IsNullOrEmpty(emailComercial)) oMensaje.CC.Add(emailComercial);
                }
                else if (!string.IsNullOrEmpty(emailComercial))
                {
                    oMensaje.To.Add(emailComercial);
                }
                else
                {
                    logger.Debug($"El contrato {oContrato.ContratoId} no tiene ContactoComercial para el proveedor {oContrato.ProveedorId} ni email comercial");
                    return;
                }
                var mailCreador = "";
                try { mailCreador = mailManager.GetEmailUserActiveDirectory(oContrato.ComercialCreador.IdActiveDirectory); } catch (Exception e) { logger.Error(e); }

                if (!string.IsNullOrEmpty(mailCreador) && mailCreador != emailComercial)
                {
                    oMensaje.CC.Add(mailCreador);
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);
                var emailComerciales = "";
                if (PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
                {
                    var corredoresComerciales = mobComercial.ListarComercialesCorredor();
                    corredoresComerciales.Remove(oContrato.Comercial);
                    logger.Debug("Enviando mail a " + string.Join(", ", corredoresComerciales));
                    foreach (Comercial corredorComercialCopia in corredoresComerciales)
                    {
                        try
                        {
                            emailComerciales = mailManager.GetEmailUserActiveDirectory(corredorComercialCopia.IdActiveDirectory);
                            oMensaje.To.Add(emailComerciales);
                        }
                        catch (Exception e) { logger.Error(e); }
                    }
                }

                oMensaje.AlternateViews.Add(CuerpoMailContrato(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), oContrato, objDescuento, objCalidad, emailComercial, eliminar));
                var subject = "";

                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                {
                    subject += "Mail Pruebas - ";
                }
                if (eliminar != null && eliminar == true)
                {
                    subject += "Anulación negocio Molinos Agro S.A. - ";
                }
                else
                {
                    subject += "Nuevo negocio Molinos Agro S.A. - ";
                }
                subject += oContrato.Corredor != null ? oContrato.Corredor.RazonSocial : oContrato.Proveedor.RazonSocial;
                oMensaje.Subject = subject;
                oMensaje.BodyEncoding = Encoding.UTF8;

                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                SmtpClient oCliente = default(SmtpClient);

                int Condicion = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                }
                else
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                }

                if (ConfigurationManager.AppSettings["SmtpAnonimo"] != "S")
                {
                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);
                }

                oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";

                oCliente.Send(oMensaje);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        public void EnviarEmailFijacion(FijacionDePrecioContrato oFijacionDePrecioContrato, string idActiveDirectory)
        {
            try
            {
                var id = oFijacionDePrecioContrato.CorredorId.HasValue ? oFijacionDePrecioContrato.CorredorId : oFijacionDePrecioContrato.ProveedorId;
                var proveedorContacto = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == id && x.CompraNet == true);

                string emailComercial = "";

                if (oFijacionDePrecioContrato.Comercial != null)
                {
                    try { emailComercial = mailManager.GetEmailUserActiveDirectory(oFijacionDePrecioContrato.Comercial.IdActiveDirectory); } catch (Exception e) { logger.Error(e); }
                }

                var oMensaje = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                };

                if (proveedorContacto.Count > 0)
                {
                    foreach (var contacto in proveedorContacto)
                    {
                        if (!string.IsNullOrEmpty(contacto.Email1))
                        {
                            oMensaje.To.Add(contacto.Email1);
                        }
                    }
                    if (!string.IsNullOrEmpty(emailComercial)) oMensaje.CC.Add(emailComercial);
                }
                else if (!string.IsNullOrEmpty(emailComercial))
                {
                    oMensaje.To.Add(emailComercial);
                }
                else
                {
                    logger.Debug($"La fijación {oFijacionDePrecioContrato.ContratoId} no tiene ContactoComercial para el proveedor {oFijacionDePrecioContrato.ProveedorId} ni email comercial");
                    return;
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);
                var emailComerciales = "";
                if (PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial))
                {
                    var corredoresComerciales = mobComercial.ListarComercialesCorredor();
                    corredoresComerciales.Remove(oFijacionDePrecioContrato.Comercial);

                    foreach (Comercial corredorComercialCopia in corredoresComerciales)
                    {
                        try
                        {
                            emailComerciales = mailManager.GetEmailUserActiveDirectory(corredorComercialCopia.IdActiveDirectory);
                            oMensaje.To.Add(emailComerciales);
                        }
                        catch (Exception e) { logger.Error(e); }
                    }
                }
                oMensaje.AlternateViews.Add(CuerpoMailFijacion(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), oFijacionDePrecioContrato, emailComercial));
                var subject = "";
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    subject += "Nueva fijación Molinos Agro S.A. –  " + oFijacionDePrecioContrato.Proveedor.RazonSocial;
                }
                else
                {
                    subject += "Mail Pruebas - Nueva fijación Molinos Agro S.A. –  ";
                }
                subject += oFijacionDePrecioContrato.Corredor != null ? oFijacionDePrecioContrato.Corredor.RazonSocial : oFijacionDePrecioContrato.Proveedor.RazonSocial;
                oMensaje.Subject = subject;
                var tipoNegocio = repositorio.Obtener<TipoNegocio>(3);

                oMensaje.BodyEncoding = Encoding.UTF8;

                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                SmtpClient oCliente = default(SmtpClient);

                int Condicion = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                }
                else
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                }

                if (ConfigurationManager.AppSettings["SmtpAnonimo"] != "S")
                {
                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);
                }

                oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";
                oCliente.Send(oMensaje);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
        private AlternateView CuerpoMailContrato(String filePath, Contrato oContrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string emailComercial, bool? eliminar)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;

            string htmlBody = "";
            if (eliminar.HasValue && eliminar.Value)
            {
                htmlBody += "En el presente mail, se detalla el negocio eliminado con Molinos Agro S.A.: <br /><br />  ";
            }
            else
            {
                htmlBody += "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />  ";
            }
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA</th>" + Td(ref linea);
            if (oContrato.ContratoAcuerdoId != null)
            {
                htmlBody += Split(oContrato.ContratoAcuerdo.Fecha.ToShortDateString()) + "</td></tr>";
            }
            else
            {
                htmlBody += Split(oContrato.Fecha.ToShortDateString()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "GRANO</th>" + Td(ref linea) + oContrato.Material.Descripcion.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "CONTRATO</th>" + Td(ref linea) + Split(oContrato.ContratoSAP.TrimStart('0')) + "</td></tr>";
            if (oContrato.DestinoId != null)
            {
                htmlBody += "<tr>" + th + "DESTINO</th>" + Td(ref linea) + oContrato.Destino.Descripcion.ToUpper() + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "PROVEEDOR</th>" + Td(ref linea) + oContrato.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "CUIT</th>" + Td(ref linea) + Split(oContrato.Proveedor.CUIT.ToString()) + "</td></tr>";
            if (oContrato.Corredor != null)
            {
                htmlBody += "<tr>" + th + "CORREDOR</th>" + Td(ref linea) + oContrato.Corredor.RazonSocial.ToUpper() + "</td></tr>";
                htmlBody += "<tr>" + th + "CUIT CORREDOR</th>" + Td(ref linea) + Split(oContrato.Corredor.CUIT.ToString()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "FIGURA</th>" + Td(ref linea) + oContrato.Clasificacion.Descripcion.ToUpper();
            if (oContrato.Consignatario == true)
            {
                htmlBody += " CONSIG";
            }
            htmlBody += "</td></tr>";

            htmlBody += "<tr>" + th + "CANTIDAD</th>" + Td(ref linea) + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))
                + " Kg.";
            if (oContrato.CantidadCamiones != null)
            {
                htmlBody += " (" + oContrato.CantidadCamiones + " camiones)<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea);
            if (oContrato.TipoNegocioId == 2)
            {
                if (oContrato.Pizarra.HasValue && oContrato.Pizarra.Value)
                {
                    htmlBody += "Pizarra</td></tr>";
                }
                else
                {
                    if (oContrato.PrecioNeto.HasValue)
                    {
                        htmlBody += Split(oContrato.PrecioNeto.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                    }
                    else
                    {
                        htmlBody += Split(oContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                    }
                }
            }
            else if (oContrato.TipoNegocioId == 1)
            {
                htmlBody += "A FIJAR HASTA: <br />" + Split(oContrato.HastaFijacion.Value.ToShortDateString()) + "<br />" + Split(oContrato.CondicionFijacion.Descripcion.ToUpper()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "PROCEDENCIA</th>" + Td(ref linea) + oContrato.Localidad.Nombre.ToUpper() + " - " + oContrato.Provincia.Nombre.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "ENT. DESDE</th>" + Td(ref linea) + Split(oContrato.FechaDesde.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "ENT. HASTA</th>" + Td(ref linea) + Split(oContrato.FechaHasta.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "COSECHA</th>" + Td(ref linea) + oContrato.Campana.Descripcion.ToUpper() + "</td></tr>";
            if (oContrato.BoletoId != null && oContrato.BoletoId != 3)
            {
                htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + " " + oContrato.Bolsa.Descripcion.ToUpper() + "</td></tr>";
            }
            else if (oContrato.BoletoId == 3)
            {
                htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "OBSERVACIÓN</th>" + Td(ref linea);
            if (oContrato.EstablecimientoPropio == true)
            {
                htmlBody += "ESTABLECIMIENTO PROPIO<br />";
            }
            else if (oContrato.EstablecimientoPropio == false)
            {
                htmlBody += "ESTABLECIMIENTO ARRENDADO<br />";
            }
            if (oContrato.ImporteSustentable != null)
            {
                htmlBody += "SUSTENTABLE " + oContrato.ImporteSustentable + " " + oContrato.MonedaSustentable.Descripcion.ToUpper() + "<br />";
            }
            if (oContrato.ClasificacionId == 1 && oContrato.CorredorId == null && oContrato.Dolarizado.Value)
            {
                htmlBody += "DOLARIZADO MÍNIMO 30 DÍAS<br />";
            }
            if (oContrato.FechaDolarizado != null)
            {
                htmlBody += "FECHA DOLARIZADO " + Split(oContrato.FechaDolarizado.Value.ToShortDateString()) + "<br />";
            }
            if (oContrato.DiasPesificado != null)
            {
                htmlBody += "PAGO DIFERIDO <br />";
                htmlBody += "DÍAS DE DIFERIMIENTO " + oContrato.DiasPesificado + "<br />";
            }
            if (oContrato.CD == true)
            {
                htmlBody += "PAGO CD<br />";
            }
            else if (oContrato.Warrant == true)
            {
                htmlBody += "PAGO WARRANT<br />";
            }
            if (oContrato.PagoDirectoVendedor == true)
            {
                htmlBody += "PAGO DIRECTO<br /> ";
            }
            if (oContrato.MercsDeposito == true)
            {
                htmlBody += "MERCADERIA EN DEPOSITO<br /> ";
            }
            if (objDescuento != null)
            {
                foreach (var desc in objDescuento)
                {
                    if (desc.Importe > 0 || desc.Porcentaje > 0)
                    {
                        htmlBody += "BONIFICACIONES " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                    }
                    else if (desc.Importe < 0 || desc.Porcentaje < 0)
                    {
                        htmlBody += "DESCUENTOS " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                    }
                    if (desc.Importe != 0)
                    {
                        htmlBody += desc.Importe + " " + desc.Moneda.Descripcion + "<br />";
                    }

                    if (desc.Porcentaje != 0)
                    {
                        htmlBody += desc.Porcentaje + "%<br />";
                    }
                }
            }
            if (oContrato.StandardDeCalidadId == 7)
            {
                htmlBody += "CALIDAD GRADO 2<br />";
            }
            else if (oContrato.TrigoEspecial == true)
            {
                htmlBody += "CALIDAD ESPECIAL ";
            }
            if (objCalidad != null && oContrato.StandardDeCalidadId != 7)
            {
                foreach (var cal in objCalidad)
                {
                    htmlBody += cal.CalidadEspecial.Descripcion.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "<br />";
                    if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                    {
                        htmlBody += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "%<br />";
                    }
                }
            }
            if (oContrato.PlanCanje == true)
            {
                htmlBody += "PLAN CANJE" + "<br />";
            }
            if (oContrato.ContratoVendedor != null)
            {
                htmlBody += "Contrato Vendedor: " + oContrato.ContratoVendedor + "<br />";
            }
            if (oContrato.ContratoCorredor != null)
            {
                htmlBody += "Contrato Corredor: " + oContrato.ContratoCorredor + "<br />";
            }
            if (oContrato.SelCargoMOA == true)
            {
                htmlBody += " Sellado 100% a Cargo MOA " + "<br />";
            }
            if (oContrato.SelCargoVendedor == true)
            {
                htmlBody += " Sellado 100% a Cargo vendedor " + "<br />";
            }
            if (oContrato.Compensacion == true)
            {
                htmlBody += " NEGOCIO COMPENSACIÓN " + "<br />";
            }
            if ((oContrato.NivelTarifa != null) && oContrato.TarifaFlete > 0)
            {
                htmlBody += " NIVEL DE TARIFA: " + oContrato.NivelTarifa.Descripcion.ToUpper() + "<br />" +
                    "TARIFA DE FLETE: " + oContrato.TarifaFlete + "<br />";
            }
            if (oContrato.Observacion != null)
            {
                htmlBody += oContrato.Observacion + "<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /><br /> Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + (oContrato.Comercial != null ? oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        private AlternateView CuerpoMailFijacion(String filePath, FijacionDePrecioContrato oFijacionDePrecioContrato, string emailComercial)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";

            //htmlBody += "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA</th>" + Td(ref linea) + oFijacionDePrecioContrato.Fecha.ToShortDateString() + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO</th>" + Td(ref linea) + oFijacionDePrecioContrato.Material.Descripcion + "</td></tr>";
            htmlBody += "<tr>" + th + "CONTRATO</th>" + Td(ref linea) + oFijacionDePrecioContrato.ContratoSAP.TrimStart('0') + " - " + oFijacionDePrecioContrato.FijacionSAP.Substring(oFijacionDePrecioContrato.FijacionSAP.Length - 2) + "</td></tr>";
            htmlBody += "<tr>" + th + "PROVEEDOR</th>" + Td(ref linea) + oFijacionDePrecioContrato.Proveedor.RazonSocial + "</td></tr>";
            htmlBody += "<tr>" + th + "CUIT</th>" + Td(ref linea) + oFijacionDePrecioContrato.Proveedor.CUIT + "</td></tr>";
            if (oFijacionDePrecioContrato.Corredor != null)
            {
                htmlBody += "<tr>" + th + "CORREDOR</th>" + Td(ref linea) + oFijacionDePrecioContrato.Corredor.RazonSocial.ToUpper() + "</td></tr>";
                htmlBody += "<tr>" + th + "CUIT CORREDOR</th>" + Td(ref linea) + Split(oFijacionDePrecioContrato.Corredor.CUIT.ToString()) + "</td></tr>";
            }

            htmlBody += "<tr>" + th + "CANTIDAD</th>" + Td(ref linea) + oFijacionDePrecioContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "</td></tr>";

            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea);

            if (oFijacionDePrecioContrato.Pizarra.HasValue && oFijacionDePrecioContrato.Pizarra.Value)
            {
                htmlBody += "Pizarra";
            }
            else if (oFijacionDePrecioContrato.PrecioNeto.HasValue)
            {
                htmlBody += Split(oFijacionDePrecioContrato.PrecioNeto.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oFijacionDePrecioContrato.Moneda.Descripcion.ToUpper();
            }
            else
            {
                htmlBody += Split(oFijacionDePrecioContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oFijacionDePrecioContrato.Moneda.Descripcion.ToUpper();
            }
            htmlBody += "<tr>" + th + "OBSERVACIONES</th>" + Td(ref linea);
            if (oFijacionDePrecioContrato.PagoDiferido.HasValue && oFijacionDePrecioContrato.PagoDiferido.Value)
            {
                htmlBody += "PAGO DIFERIDO <br /> ";
            }
            if (oFijacionDePrecioContrato.DiasPesificado.HasValue)
            {
                htmlBody += "DÍAS DE DIFERIMIENTO: " + oFijacionDePrecioContrato.DiasPesificado.ToString() + "<br /> ";
            }
            //var conceptoApertura = oFijacionDePrecioContrato.AperturaPrecio;
            //if ((oFijacionDePrecioContrato.AperturaPrecio.Count >0 || oFijacionDePrecioContrato.AperturaPrecio != null) && oFijacionDePrecioContrato.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == 1).Select(x => x.Importe).First() > 0)
            //{
            //    htmlBody += "COSTO FINANCIERO: " + conceptoApertura.Select(x => x.Importe).First().ToString()+ "<br />";
            //}
            var contrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP.Contains(oFijacionDePrecioContrato.ContratoSAP));
            if (contrato != null)
            {
                if (contrato.Sustentable.HasValue && contrato.Sustentable.Value)
                {
                    htmlBody += "SUSTENTABLE <br />";
                }
                if (contrato.StandardDeCalidadId != null && (contrato.StandardDeCalidad.Descripcion == "Grado 2" ||
                    (contrato.MaterialId == 1 && contrato.StandardDeCalidadId == 2 &&
                    repositorio.Existe<Calidad>(x => x.ContratoId == contrato.ContratoId && x.CalidadEspecialId == 4 && x.Valor == 2))))
                {
                    htmlBody += "CALIDAD GRADO 2<br />";
                }
                else if (contrato.StandardDeCalidadId != null && (contrato.StandardDeCalidad.Descripcion == "Especial"
                    || contrato.StandardDeCalidad.Descripcion == "Materia Extraña"))
                {
                    htmlBody += "CALIDAD ESPECIAL<br />";
                }
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br />  En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A. Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + (oFijacionDePrecioContrato.Comercial != null ? oFijacionDePrecioContrato.Comercial.Nombres + " " + oFijacionDePrecioContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.   <br /><br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            htmlBody += "<style> table, th, td{ }</style>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        private string Split(string str)
        {
            var enumNumero = Enumerable.Range(0, str.Length / 2)
                .Select(i => str.Substring(i * 2, 2)).ToList();
            if (str.Length % 2 == 1)
            {
                enumNumero.Add(str[str.Length - 1].ToString());
            }
            var nuevoString = "";

            for (int i = 0; i < enumNumero.Count(); i++)
            {
                nuevoString += "<span>" + enumNumero[i] + "</span>";
            }
            return nuevoString;
        }
        private string Td(ref int linea)
        {
            string td1 = "";
            string td2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
            }
            linea += 1;
            if (linea % 2 == 0)
            {
                return td1;
            }
            else
            {
                return td2;
            }
        }
        
        public Resultado EliminarRecordatorio(int Id)
        {
            var oEntityErrors = new Resultado();
            repositorio.Remover<Actividad>(Id);
            repositorio.GuardarCambios();
            return oEntityErrors;
        }
        public DatosIniProveedor TraerDatosCombo(int ProveedorId, bool noFiltrarAdministrativo)
        {
            var DatosCombo = new DatosIniProveedor
            {
                segm = repositorio.Listar<Segmentacion, SegmentacionQry>(
                    x => new SegmentacionQry { SegmentacionId = x.SegmentacionId,
                        Descripcion = x.Descripcion, Grupo = x.Grupo },
                    x => (!noFiltrarAdministrativo || x.Grupo == "Corredores")
                    && x.Grupo != "Grandes Cuentas" && x.Grupo != "Canjeadores"),
                tiptel = repositorio.Listar<TipoTelefono, TipoTelefonoQry>(
                    x => new TipoTelefonoQry { TipoTelefonoId = x.TipoTelefonoId, Descripcion = x.Descripcion }),
                prov = repositorio.Listar<Provincia, ProvinciaQry>(
                    x => new ProvinciaQry { Provinciaid = x.ProvinciaId, Nombre = x.Nombre }),
                loc = new List<LocalidadQry>(),
                cope = repositorio.Listar<CanalOperacion, CanalOperacionQry>(
                    x => new CanalOperacionQry { CanalOperacionId = x.CanalOperacionId, Descripcion = x.Descripcion, Inhabilitado = false }),
                gran = repositorio.Listar<Material, MaterialQry>(
                    x => new MaterialQry { MaterialId = x.MaterialId, Codigo = x.Codigo, Descripcion = x.Descripcion }),
                dest = repositorio.Listar<Destinatario, DestinatarioQry>(
                    x => new DestinatarioQry { DestinatarioId = x.DestinatarioId, Descripcion = x.Descripcion, Inhabilitado = false }),
                cond = repositorio.Listar<Condicion, CondicionQry>(
                    x => new CondicionQry { CondicionId = x.CondicionId, Descripcion = x.Descripcion, Inhabilitado = false }),
                inte = repositorio.Listar<Interes, InteresQry>(
                    x => new InteresQry { InteresId = x.InteresId, Descripcion = x.Descripcion }),
                tipoact = repositorio.Listar<TipoActividad, TipoActividadQry>(
                    x => new TipoActividadQry { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }),
                concom = repositorio.Listar<ContactoComercial, ContactoComercialQry>(
                    x => new ContactoComercialQry { ContactoComercialId = x.ContactoComercialId, Nombres = x.Nombres + " " + x.Apellido }, x => x.ProveedorId == ProveedorId),
                ClasComNet = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(
                    x => new ClasificacionCompraNetQry { Id = x.Id, Descripcion = x.Descripcion }),
                BoleComNet = repositorio.Listar<BoletoCompraNet, BoletoCompraNetQry>(
                    x => new BoletoCompraNetQry { Id = x.Id, Descripcion = x.Descripcion }),
                BolsComNet = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(
                    x => new BolsaCompraNetQry { Id = x.Id, Descripcion = x.Descripcion })
            };

            return DatosCombo;
        }
        public List<LocalidadDto> TraerLocalidad(int Id)
        {
            return repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre }, x => x.ProvinciaId == Id);
        }
        public ProveedorNuevo TraerRazonSocial(string cuit)
        {
            var sisa = repositorio.Listar<SISA>(x => x.CUIT == cuit).FirstOrDefault();
            var razonsocial = new ProveedorNuevo();
            if (sisa != null)
            {
                razonsocial = new ProveedorNuevo
                {
                    CUIT = sisa.CUIT,
                    Operable = 1,
                    EstadoCuit = sisa.EstadoCuit,
                    razonSocial = sisa.RazonSocial,
                    FechaVigenciaEstado = sisa.FechaVigenciaEstado ?? DateTime.Parse(null)
                };

                if (razonsocial != null)
                {
                    razonsocial.Existe = repositorio.Existe<Proveedor>(x => x.CUIT == cuit) ? 1 : 0;
                }
                TraerEstado(razonsocial);
            }
            else
            {
                razonsocial = new ProveedorNuevo() { Condicion = "no incluido", CUIT = cuit, Operable = 0, razonSocial = "No existe Razon Social" };
            }
            return razonsocial;
        }
        public ProveedorQry TraerProveedorPorCuit(string cuit, bool corredor)
        {
            var proveedor = new ProveedorQry();
            if (corredor)
            {
                proveedor = repositorio.Obtener<CorredorProveedor, ProveedorQry>(x => x.Corredor.CUIT == cuit, x => new ProveedorQry() { ProveedorId = x.CorredorId, Descripcion = x.Corredor.RazonSocial });
            }
            else
            {
                proveedor = repositorio.Obtener<Proveedor, ProveedorQry>(x => x.CUIT == cuit && (x.SegmentacionId != 5 && x.SegmentacionId != 7), x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial });
            }
            return proveedor;
        }
        private void TraerEstado(ProveedorNuevo prov)
        {
            var oFacacop = repositorio.Existe<FACACOP>(x => x.CUIT == prov.CUIT);

            if (oFacacop)
            {
                prov.Operable = 0;
                prov.Condicion = "Apocrifos";
                return;
            }
            else if (prov.EstadoCuit == 0)
            {
                prov.Condicion = "Inactivo";
                prov.Operable = 0;
            }
            else if (prov.EstadoCuit == 3)
            {
                prov.Condicion = "Estado 3";
                prov.Operable = 0;
            }

            if (ConfigurationManager.AppSettings["SinConexionSap"] != "1")
            {
                prov.RiesgoComercial = oRiesgoComercialAgent.ObtenerRiesgoComercial(prov.CUIT);

                if (prov.RiesgoComercial == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    prov.Operable = 0;
                    prov.Condicion = "Riesgo Comercial Alto";
                    return;
                }
            }
        }
        public GrabarProveedorResult GrabarNuevoProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarProveedorResult();
            oEntityErrors = ValidarProveedor(oParam, oEntityErrors, false);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var proveedor = new Proveedor
            {
                AlmacHabilitadoSojaSust = oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null" ?
                    Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)) : (bool?)null,
                AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust,
                AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust,
                AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns,
                AreaInfluenciaId = oParam.contacto.areaDeInfluencia != null ? Convert.ToInt32(oParam.contacto.areaDeInfluencia) : (int?)null,
                CUIT = oParam.basicos.cuit,
                RazonSocial = oParam.basicos.RazonSocial,
                SegmentacionId = oParam.basicos.segmentacion,
                Calificacion = oParam.basicos.calificacion,
                ProvinciaCompraNetId = oParam.basicos.ProvinciaCompraNet,
                LocalidadCompraNetId = oParam.basicos.LocalidadCompraNet,
                ClasificacionCompraNetId = oParam.basicos.ClasificacionCompraNet,
                BoletoCompraNetId = oParam.basicos.BoletoCompraNet,
                BolsaCompraNetId = oParam.basicos.BolsaCompraNet,
                Observaciones = oParam.basicos.comentario,
                CodigoPostal = oParam.contacto.codpost,
                Direccion = oParam.contacto.direccion,
                Intermediario = oParam.contacto.intermediario,
                LocalidadId = oParam.contacto.localidad,
                EstadoId = 1,
                FechaAlta = DateTime.Now,
                Consignatario = oParam.basicos.Consignatario,
                ComisionPorcentaje = oParam.basicos.Comision
            };
            var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
            var oEstados = repositorio.Listar<Estado>();
            if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
            {
                var list = oDatosProveedorAgent.ObtenerDatosDeProveedor(new List<Datos> { new Datos { CUIT = oParam.basicos.cuit, UsuarioDirectory = idActiveDirectory } });

                if (list.Count > 0)
                {
                    foreach (var lista in list)
                    {
                        var comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == lista.USUARIO);

                        if (comercialLista != null)
                        {
                            repositorio.Agregar(new ProveedorEstado
                            {
                                Comercial = comercialLista,
                                Estado = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault() ?? oEstados.Where(x => x.EstadoId == 1).FirstOrDefault(),
                                Proveedor = proveedor
                            });
                        }
                        proveedor.ClienteMOA = !string.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false;
                    }
                }
                else
                {
                    repositorio.Agregar(new ProveedorEstado
                    {
                        Comercial = comercial,
                        Proveedor = proveedor,
                        Estado = oEstados.Where(x => x.EstadoId == int.Parse(ConfigurationManager.AppSettings[oParam.basicos.nocliente == 1 ? "NoCliente" : "PotencialCliente"])).FirstOrDefault()
                    });
                }
            }
            else
            {
                proveedor.Estado = oEstados.Where(x => x.EstadoId == int.Parse(ConfigurationManager.AppSettings[oParam.basicos.nocliente == 1 ? "NoCliente" : "PotencialCliente"])).FirstOrDefault();
            }
            repositorio.Agregar(proveedor);

            repositorio.Agregar(new ProveedorComercial()
            {
                Comercial = comercial,
                NroItem = 1,
                Proveedor = proveedor
            });

            if (oParam.contactocomercial != null && oParam.contactocomercial.Count > 0)
            {
                foreach (var param in oParam.contactocomercial)
                {
                    var contactoComercial = new ContactoComercial
                    {
                        Apellido = param.apellido,
                        Cargo = param.cargo,
                        Email1 = param.emails[0],
                        Email2 = param.emails[1],
                        Email3 = param.emails[2],
                        EsPrincipal = param.principal,
                        FechaNacimiento = param.fechaNacimiento,
                        Nombres = param.nombre,
                        OtrosIntereses = param.otrosIntereses,
                        Proveedor = proveedor,
                        Puesto = param.puesto,
                        Telefono1 = param.telefonos[0].telefono,
                        Telefono2 = param.telefonos[1].telefono,
                        Telefono3 = param.telefonos[2].telefono,
                        TipoTelefono1Id = (param.telefonos[0].tipoTelefono.HasValue ? (int?)param.telefonos[0].tipoTelefono.Value : null),
                        TipoTelefono2Id = (param.telefonos[1].tipoTelefono.HasValue ? (int?)param.telefonos[1].tipoTelefono.Value : null),
                        TipoTelefono3Id = (param.telefonos[2].tipoTelefono.HasValue ? (int?)param.telefonos[2].tipoTelefono.Value : null),
                        CompraNet = param.CompraNet,
                        Cupo = param.Cupo
                    };
                    repositorio.Agregar(contactoComercial);

                    int cant = 1;
                    foreach (var interes in param.intereses)
                    {
                        repositorio.Agregar(new ContactoComercialInteres
                        {
                            ContactoComercial = contactoComercial,
                            Interes = repositorio.Obtener<Interes>(interes),
                            NroItem = cant++,
                        });
                    }
                }
            }

            if (oParam.produccion.CamposProduccion != null && oParam.produccion.CamposProduccion.Count > 0)
            {
                int item = 1;
                foreach (var cmp in oParam.produccion.CamposProduccion)
                {
                    var campo = new Campo
                    {
                        ArrendaPropia = cmp.hectareas,
                        Coordenadas = cmp.coordenadas,
                        HabilitadoSojaSustentable = Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)),
                        KMZfile = cmp.archivoFileResult,
                        KMZnombre = cmp.archivo,
                        LocalidadId = cmp.localidad,
                        NroItem = item++,
                        Proveedor = proveedor
                    };
                    repositorio.Agregar(campo);

                    int itemCM = 1;
                    foreach (var grn in cmp.granos)
                    {
                        repositorio.Agregar(new CampoMaterial
                        {
                            CampañaId = grn.campañaId,
                            Campo = campo,
                            Hectareas = grn.hectareas.HasValue ? grn.hectareas : null,
                            MaterialId = grn.granoId,
                            NroItem = itemCM++,
                            Toneladas = grn.toneladas.HasValue ? grn.toneladas : null
                        });
                    }

                }
            }

            if (oParam.almacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento.Count > 0)
            {
                int item = 1;
                foreach (var cmp in oParam.almacenamiento.CamposAlmacenamiento)
                {
                    var acopio = new Acopio
                    {
                        Coordenadas = cmp.coordenadasAlmacenamiento,
                        KMZfile = cmp.archivoFileResult,
                        KMZnombre = cmp.archivo,
                        LocalidadId = cmp.localidad,
                        NroItem = item++,
                        Proveedor = proveedor
                    };
                    repositorio.Agregar(acopio);

                    int itemCM = 1;
                    foreach (var grn in cmp.granosAlmacenamiento)
                    {
                        repositorio.Agregar(new AcopioCampaña
                        {
                            Acopio = acopio,
                            CampañaId = grn.campañaId,
                            HasArrendadas = grn.hasArrendadas,
                            NroItem = itemCM++,
                            Toneladas = grn.toneladasAlmacenamiento
                        });
                    }

                    itemCM = 1;
                    foreach (var grn in cmp.granosAlmacenamientoGrano)
                    {
                        repositorio.Agregar(new AcopioMaterial
                        {
                            Acopio = acopio,
                            CampañaId = grn.campañaId,
                            MaterialId = grn.granoId,
                            NroItem = itemCM++,
                            Toneladas = grn.toneladasAlmacenamiento
                        });
                    }
                }
            }

            if (oParam.contacto.canalesOperacion != null && oParam.contacto.canalesOperacion.Count > 0)
            {
                int itemPCO = 1;

                foreach (var param in oParam.contacto.canalesOperacion)
                {
                    repositorio.Agregar(new ProveedorCanalOperacion
                    {
                        CanalOperacionId = param,
                        NroItem = itemPCO.ToString(),
                        Proveedor = proveedor
                    });
                    itemPCO++;
                }
            }

            if (oParam.contacto.condPreferentes != null && oParam.contacto.condPreferentes.Count > 0)
            {
                int itemPCO = 1;
                foreach (var param in oParam.contacto.condPreferentes)
                {
                    repositorio.Agregar(new ProveedorCondicion
                    {
                        CondicionId = param,
                        NroItem = itemPCO++,
                        Proveedor = proveedor
                    });
                }
            }

            if (oParam.contacto.entregaA != null && oParam.contacto.entregaA.Count > 0)
            {
                int itemPCO = 1;
                foreach (var param in oParam.contacto.entregaA)
                {
                    repositorio.Agregar(new ProveedorDestinatario
                    {
                        DestinatarioId = param,
                        NroItem = itemPCO++,
                        Proveedor = proveedor
                    });
                }
            }

            if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Count > 0)
            {
                oParam.produccion.objetivos.OrderBy(z => z.campañaId);

                int itemCampañaActual = 0;
                foreach (var param in oParam.produccion.objetivos)
                {
                    repositorio.Agregar(new Objetivo
                    {
                        CampañaId = param.campañaId,
                        MaterialId = param.granoId,
                        NroItem = itemCampañaActual++,
                        Proveedor = proveedor,
                        ToneladasObjetivos = Convert.ToDouble(param.toneladasObjetivo)
                    });
                }
            }

            repositorio.Agregar(new LogProveedor()
            {
                Comercial = comercial,
                Fecha = DateTime.Now,
                Proveedor = proveedor
            });
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                oEntityErrors.Error("", ex.Message);
                return oEntityErrors;
            }
            oEntityErrors.ProveedorId = proveedor.ProveedorId;
            return oEntityErrors;
        }

        public GrabarProveedorResult UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory, List<int> equipo, int comercialId)
        {
            var resultado = UpdateDatosBasicosProveedor(oParam, idActiveDirectory, equipo);

            if (resultado.HayErrores)
            {
                return resultado;
            }
            resultado = UpdateDatosContacto(oParam);

            if (resultado.HayErrores)
            {
                return resultado;
            }
            resultado = UpdateContactoComerciales(oParam);

            if (resultado.HayErrores)
            {
                return resultado;
            }
            resultado = UpdateProduccion(oParam);

            if (resultado.HayErrores)
            {
                return resultado;
            }
            resultado = UpdateAlmacenamiento(oParam);

            if (resultado.HayErrores)
            {
                return resultado;
            }
            var log = new LogProveedor { Fecha = DateTime.Now, ComercialId = comercialId, ProveedorId = oParam.ProveedorId.Value };
            repositorio.Agregar(log);
            if (resultado.HayErrores)
            {
                return resultado;
            }

            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
                return resultado;
            }

            resultado.ProveedorId = oParam.ProveedorId;

            return resultado;

        }
        private GrabarProveedorResult ValidarProveedor(NuevoProveedor oParam, GrabarProveedorResult oEntityErrors, bool paraCorredor)
        {
            if (!paraCorredor)
            {
                if (repositorio.Existe<Proveedor>(x => x.CUIT == oParam.basicos.cuit && (x.SegmentacionId != 5 && x.SegmentacionId != 7)) && (oParam.basicos.segmentacion != 5 && oParam.basicos.segmentacion != 7))
                {
                    oEntityErrors.Error("Proveedor", "Ya existe un proveedor con CUIT " + oParam.basicos.cuit);
                    return oEntityErrors;
                }
                if (repositorio.Existe<Proveedor>(x => x.CUIT == oParam.basicos.cuit && (x.SegmentacionId == 5 || x.SegmentacionId == 7)) && (oParam.basicos.segmentacion == 5 || oParam.basicos.segmentacion == 7))
                {
                    oEntityErrors.Error("Proveedor", "Ya existe un corredor con CUIT " + oParam.basicos.cuit);
                    return oEntityErrors;
                }
            }
            if (!repositorio.Existe<SISA>(x => x.CUIT == oParam.basicos.cuit))
            {
                oEntityErrors.Error("SISA", "No existe el CUIT" + oParam.basicos.cuit);
                return oEntityErrors;
            }

            if (repositorio.Existe<FACACOP>(x => x.CUIT == oParam.basicos.cuit))
            {
                oEntityErrors.Error("FACACOP", "El CUIT " + oParam.basicos.cuit + " es Apocrifo");
                return oEntityErrors;
            }
            return oEntityErrors;
        }
        public ProveedorDto TraerProveedor(int? proveedorId)
        {
            return proveedorId.HasValue ? repositorio.Obtener<Proveedor, ProveedorDto>(x => x.ProveedorId == proveedorId,
                x => new ProveedorDto
                {
                    CUIT = x.CUIT,
                    ProveedorId = x.ProveedorId,
                    RazonSocial = x.RazonSocial
                }) : null;
        }

        private GrabarProveedorResult UpdateDatosBasicosProveedor(NuevoProveedor oParam, string idActiveDirectory, List<int> equipo)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oProveedorSave = repositorio.Obtener<Proveedor>(oParam.ProveedorId);

                var clasificacion = oParam.basicos.ClasificacionCompraNet != null ?
                    oParam.basicos.ClasificacionCompraNet :
                    (oParam.produccion != null && oParam.produccion.CamposProduccion != null && oParam.produccion.CamposProduccion.Any()) ? 1 :
                    (oParam.almacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento.Any()) ? 2 :
                    oProveedorSave.ClasificacionCompraNetId;
                oProveedorSave.AlmacHabilitadoSojaSust = oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null" ?
                    Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)) : (bool?)null;
                oProveedorSave.AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns;
                oProveedorSave.AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust;
                oProveedorSave.AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust;
                oProveedorSave.Calificacion = oParam.basicos.calificacion;
                oProveedorSave.Observaciones = oParam.basicos.comentario;
                oProveedorSave.SegmentacionId = oParam.basicos.segmentacion != 0 ? oParam.basicos.segmentacion : 2;
                oProveedorSave.CodigoPostal = oParam.contacto.codpost;
                oProveedorSave.Direccion = oParam.contacto.direccion;
                oProveedorSave.Intermediario = oParam.contacto.intermediario;
                oProveedorSave.LocalidadId = oParam.contacto.localidad;
                oProveedorSave.ProvinciaId = oParam.contacto.provincia;
                oProveedorSave.AreaInfluenciaId = oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null" ?
                    Convert.ToInt32(oParam.contacto.areaDeInfluencia) : (int?)null;
                oProveedorSave.ClasificacionCompraNetId = clasificacion;
                oProveedorSave.BoletoCompraNetId = oParam.basicos.BoletoCompraNet;
                oProveedorSave.BolsaCompraNetId = oParam.basicos.BolsaCompraNet;
                oProveedorSave.LocalidadCompraNetId = oParam.basicos.LocalidadCompraNet;
                oProveedorSave.ProvinciaCompraNetId = oParam.basicos.ProvinciaCompraNet;
                oProveedorSave.Consignatario = oParam.basicos.Consignatario;
                oProveedorSave.ComisionPorcentaje = oParam.basicos.Comision;

                var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == oParam.basicos.cuit);
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
                var oEstados = repositorio.Listar<Estado>();
                if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
                {
                    var cuit = repositorio.Obtener<Proveedor, string>(x => x.CUIT == oParam.basicos.cuit, x => x.CUIT);
                    List<string> comerciales = repositorio.Listar<Comercial, string>(y => y.IdActiveDirectory, y => equipo.Contains(y.ComercialId));
                    var listaDeCuit = new List<Datos>();
                    foreach (var com in comerciales)
                    {
                        listaDeCuit.Add(new Datos { CUIT = cuit, UsuarioDirectory = com });
                    }

                    var list = oDatosProveedorAgent.ObtenerDatosDeProveedor(listaDeCuit);

                    if (list.Count > 0)
                    {
                        foreach (var lista in list)
                        {
                            var comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());

                            if (comercialLista != null)
                            {

                                var provEstados = repositorio.Listar<ProveedorEstado>(x => x.ProveedorId == proveedor.ProveedorId && x.ComercialId == comercialLista.ComercialId);
                                var est = oEstados.FirstOrDefault(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()) ?? oEstados.FirstOrDefault(x => x.EstadoId == 1);
                                if (provEstados.Count == 0)
                                {
                                    repositorio.Agregar(new ProveedorEstado
                                    {
                                        Comercial = comercial,
                                        Estado = est,
                                        Proveedor = proveedor
                                    });
                                }
                                else
                                {
                                    foreach (var pe in provEstados)
                                    {
                                        pe.Estado = est;
                                    }
                                }

                                oProveedorSave.ClienteMOA = (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false);
                            }
                        }
                    }
                    else
                    {
                        oProveedorSave.Estado = oEstados.Where(x => x.EstadoId == int.Parse(ConfigurationManager.AppSettings[oParam.basicos.nocliente == 1 ? "NoCliente" : "PotencialCliente"])).FirstOrDefault();

                        if (comercial != null)
                        {
                            var provEstados = repositorio.Listar<ProveedorEstado>(x => x.ProveedorId == proveedor.ProveedorId && x.ComercialId == comercial.ComercialId);
                            if (provEstados.Count == 0)
                            {
                                repositorio.Agregar(new ProveedorEstado
                                {
                                    Comercial = comercial,
                                    Estado = oProveedorSave.Estado,
                                    Proveedor = proveedor
                                });
                            }
                            else
                            {
                                foreach (var pe in provEstados)
                                {
                                    pe.Estado = oProveedorSave.Estado;
                                }
                            }
                        }
                    }
                }
                else
                {
                    oProveedorSave.Estado = oEstados.Where(x => x.EstadoId == int.Parse(ConfigurationManager.AppSettings[oParam.basicos.nocliente == 1 ? "NoCliente" : "PotencialCliente"])).FirstOrDefault();
                    if (comercial != null)
                    {
                        var provEstados = repositorio.Listar<ProveedorEstado>(x => x.ProveedorId == proveedor.ProveedorId && x.ComercialId == comercial.ComercialId);
                        if (provEstados.Count == 0)
                        {
                            repositorio.Agregar(new ProveedorEstado
                            {
                                Comercial = comercial,
                                Estado = oProveedorSave.Estado,
                                Proveedor = proveedor
                            });
                        }
                        else
                        {
                            foreach (var pe in provEstados)
                            {
                                pe.Estado = oProveedorSave.Estado;
                            }
                        }
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateDatosContacto(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var ErrorCanal = UpdateCanalOperacion(oParam);
                if (ErrorCanal.HayErrores)
                {
                    return ErrorCanal;
                }

                var ErrorDestinatario = UpdateDestinatario(oParam);
                if (ErrorDestinatario.HayErrores)
                {
                    return ErrorDestinatario;
                }

                var ErrorCondicion = UpdateCondicion(oParam);
                if (ErrorCondicion.HayErrores)
                {
                    return ErrorCondicion;
                }

                var ErrorObjetivos = UpdateObjetivos(oParam);
                if (ErrorObjetivos.HayErrores)
                {
                    return ErrorObjetivos;
                }
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateCanalOperacion(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oCanalSave = repositorio.Listar<ProveedorCanalOperacion>(x => x.ProveedorId == oParam.ProveedorId);


                #region Eliminar

                foreach (var can in oCanalSave)
                {
                    if (!oParam.contacto.canalesOperacion.Contains(can.CanalOperacionId))
                    {
                        repositorio.Remover(can);
                    }
                }

                #endregion
                foreach (var can in oParam.contacto.canalesOperacion)
                {
                    if (!oCanalSave.Any(x => x.CanalOperacionId == can))
                    {
                        repositorio.Agregar(new ProveedorCanalOperacion()
                        {
                            NroItem = "1",
                            ProveedorId = (int)oParam.ProveedorId,
                            CanalOperacionId = can
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateDestinatario(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oDestinatarioSave = repositorio.Listar<ProveedorDestinatario>(x => x.ProveedorId == oParam.ProveedorId);

                #region Eliminar

                foreach (var dest in oDestinatarioSave)
                {
                    if (!oParam.contacto.entregaA.Contains(dest.DestinatarioId))
                    {
                        repositorio.Remover(dest);
                    }
                }

                #endregion
                foreach (var can in oParam.contacto.entregaA)
                {
                    if (!oDestinatarioSave.Any(x => x.DestinatarioId == can))
                    {
                        repositorio.Agregar(new ProveedorDestinatario()
                        {
                            NroItem = 1,
                            ProveedorId = (int)oParam.ProveedorId,
                            DestinatarioId = can
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateCondicion(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oCondicionSave = repositorio.Listar<ProveedorCondicion>(x => x.ProveedorId == oParam.ProveedorId);

                #region Eliminar

                foreach (var cond in oCondicionSave)
                {
                    if (!oParam.contacto.condPreferentes.Contains(cond.CondicionId))
                    {
                        repositorio.Remover(cond);
                    }
                }

                #endregion
                if (oParam.contacto != null)
                {
                    foreach (var cond in oParam.contacto.condPreferentes)
                    {
                        if (!oCondicionSave.Any(x => x.CondicionId == cond))
                        {

                            repositorio.Agregar(new ProveedorCondicion()
                            {
                                CondicionId = cond,
                                NroItem = 1,
                                ProveedorId = (int)oParam.ProveedorId
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateObjetivos(NuevoProveedor oParam)
        {
            var oCampañaSave = repositorio.Listar<Objetivo>(x => x.ProveedorId == oParam.ProveedorId && x.CampañaId >= x.Material.CampañaId);
            #region Eliminar

            if (oParam.produccion.eliminarobjetivos.Count > 0)
            {
                foreach (var camp in oCampañaSave)
                {
                    if (oParam.produccion.eliminarobjetivos.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                    {
                        repositorio.Remover(camp);
                    }
                }
            }

            #endregion

            #region Agregar

            if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Count > 0)
            {
                foreach (var obj in oParam.produccion.objetivos)
                {
                    if (!oCampañaSave.Any(x => x.CampañaId == obj.campañaId && x.MaterialId == obj.granoId))
                    {
                        repositorio.Agregar(new Objetivo()
                        {
                            NroItem = 1,
                            CampañaId = obj.campañaId,
                            MaterialId = obj.granoId,
                            ProveedorId = (int)oParam.ProveedorId,
                            ToneladasObjetivos = Convert.ToDouble(obj.toneladasObjetivo)
                        });
                    }
                }
            }
            #endregion

            #region Modificar

            foreach (var camp in oCampañaSave)
            {
                if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                {
                    var mod = oParam.produccion.objetivos.Where(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId).First();
                    camp.CampañaId = mod.campañaId;
                    camp.MaterialId = mod.granoId;
                    camp.ToneladasObjetivos = Convert.ToDouble(mod.toneladasObjetivo);
                }
            }

            #endregion
            return new GrabarProveedorResult();

        }

        private GrabarProveedorResult UpdateContactoComerciales(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oContactoSave = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == oParam.ProveedorId);
                #region Eliminar

                foreach (var can in oContactoSave)
                {
                    if ((oParam.contactocomercial != null && !oParam.contactocomercial.Any(x => x.contactoComercialId == can.ContactoComercialId)) || oParam.contactocomercial == null)
                    {
                        var oContactoComercialInteresEliminar = repositorio.Listar<ContactoComercialInteres>(x => x.ContactoComercialId == can.ContactoComercialId);

                        foreach (var interes in oContactoComercialInteresEliminar)
                        {
                            repositorio.Remover(interes);
                        }
                        repositorio.Remover(can);
                    }
                }

                #endregion

                #region Agregar
                if (oParam.contactocomercial != null)
                {
                    foreach (var can in oParam.contactocomercial)
                    {
                        if (can.contactoComercialId == 0)
                        {
                            var contacto = new ContactoComercial()
                            {
                                Nombres = can.nombre,
                                Apellido = can.apellido,
                                Cargo = can.cargo,
                                FechaNacimiento = can.fechaNacimiento,
                                Puesto = can.puesto,
                                EsPrincipal = can.principal,
                                OtrosIntereses = can.otrosIntereses,
                                Telefono1 = can.telefonos[0].telefono,
                                Telefono2 = can.telefonos[1].telefono,
                                Telefono3 = can.telefonos[2].telefono,
                                TipoTelefono1Id = can.telefonos[0].tipoTelefono,
                                TipoTelefono2Id = can.telefonos[1].tipoTelefono,
                                TipoTelefono3Id = can.telefonos[2].tipoTelefono,
                                Email1 = can.emails[0],
                                Email2 = can.emails[1],
                                Email3 = can.emails[2],
                                ProveedorId = (int)oParam.ProveedorId,
                                CompraNet = can.CompraNet,
                                Cupo = can.Cupo
                            };
                            repositorio.Agregar(contacto);

                            var nroItem = 1;
                            foreach (var valor in can.intereses)
                            {
                                repositorio.Agregar(new ContactoComercialInteres()
                                {
                                    NroItem = nroItem++,
                                    InteresId = valor,
                                    ContactoComercial = contacto
                                });
                            }
                        }
                    }
                }

                #endregion

                #region Modificar

                foreach (var con in oContactoSave)
                {
                    if (oParam.contactocomercial != null && oParam.contactocomercial.Any(x => x.contactoComercialId == con.ContactoComercialId))
                    {
                        //Todo Agregar los objetos que faltan
                        var mod = oParam.contactocomercial.Where(x => x.contactoComercialId == con.ContactoComercialId).First();
                        con.Apellido = mod.apellido;
                        con.Nombres = mod.nombre;
                        con.ProveedorId = (int)oParam.ProveedorId;
                        con.OtrosIntereses = mod.otrosIntereses;
                        con.Puesto = mod.puesto;
                        con.Telefono1 = mod.telefonos[0].telefono;
                        con.Telefono2 = mod.telefonos[1].telefono;
                        con.Telefono3 = mod.telefonos[2].telefono;
                        con.TipoTelefono1Id = mod.telefonos[0].tipoTelefono;
                        con.TipoTelefono2Id = mod.telefonos[1].tipoTelefono;
                        con.TipoTelefono3Id = mod.telefonos[2].tipoTelefono;
                        con.Email1 = mod.emails[0];
                        con.Email2 = mod.emails[1];
                        con.Email3 = mod.emails[2];
                        con.EsPrincipal = mod.principal;
                        con.FechaNacimiento = mod.fechaNacimiento;
                        con.Cargo = mod.cargo;
                        con.CompraNet = mod.CompraNet;
                        con.Cupo = mod.Cupo;

                        #region Eliminar Intereses Contactos
                        var oContactosInteresesSave = repositorio.Listar<ContactoComercialInteres>(x => x.ContactoComercial.ContactoComercialId == mod.contactoComercialId);

                        foreach (var can in oContactosInteresesSave)
                        {
                            if (!mod.intereses.Contains(can.InteresId))
                            {
                                repositorio.Remover(can);
                            }
                        }

                        #endregion

                        #region Agregar Intereses Contactos
                        if (mod.intereses != null)
                        {
                            foreach (var can in mod.intereses)
                            {
                                if (!oContactosInteresesSave.Any(x => x.InteresId == can))
                                {
                                    repositorio.Agregar(new ContactoComercialInteres()
                                    {
                                        ContactoComercial = con,
                                        NroItem = 1,
                                        InteresId = can
                                    });
                                }
                            }
                        }
                        #endregion
                    }
                }



                #endregion
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateProduccion(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oCampoSave = repositorio.Listar<Campo>(x => x.ProveedorId == oParam.ProveedorId);
                #region Eliminar

                foreach (var cam in oCampoSave)
                {
                    if ((oParam.produccion.CamposProduccion != null && !oParam.produccion.CamposProduccion.Any(x => x.CampoId == cam.CampoId)) || oParam.produccion.CamposProduccion == null)
                    {
                        var oCampoEliminar = repositorio.Listar<CampoMaterial>(x => x.CampoId == oParam.produccion.CampoId);
                        foreach (var campmat in oCampoEliminar)
                        {
                            if (!oParam.produccion.objetivos.Any(x => x.campañaId == campmat.CampañaId && x.granoId == campmat.MaterialId))
                            {
                                repositorio.Remover(campmat);
                            }
                        }
                        repositorio.Remover(cam);

                    }
                }
                #endregion

                #region Agregar
                if (oParam.produccion.CamposProduccion != null)
                {
                    foreach (var cam in oParam.produccion.CamposProduccion)
                    {
                        if (cam.CampoId.GetValueOrDefault(0) == 0)
                        {
                            var produccion = repositorio.Agregar(new Campo()
                            {
                                Coordenadas = cam.coordenadas,
                                KMZfile = cam.archivoFileResult,
                                KMZnombre = cam.archivo,
                                LocalidadId = cam.localidad,
                                ArrendaPropia = cam.hectareas,
                                NroItem = 1,
                                ProveedorId = (int)oParam.ProveedorId
                            });

                            if (cam.granos != null)
                            {
                                foreach (var valor in cam.granos)
                                {
                                    repositorio.Agregar(new CampoMaterial()
                                    {
                                        NroItem = 1,
                                        MaterialId = valor.granoId,
                                        CampañaId = valor.campañaId,
                                        Campo = produccion,
                                        Hectareas = valor.hectareas.HasValue ? valor.hectareas : null,
                                        Toneladas = valor.toneladas.HasValue ? valor.toneladas : null,
                                    });
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Modificar

                foreach (var campo in oCampoSave)
                {
                    if (oParam.produccion.CamposProduccion != null)
                    {
                        if (oParam.produccion.CamposProduccion.Any(x => x.CampoId == campo.CampoId))
                        {
                            var mod = oParam.produccion.CamposProduccion.Where(x => x.CampoId == campo.CampoId).First();

                            #region Set Campos
                            campo.Coordenadas = mod.coordenadas;
                            campo.KMZfile = mod.archivoFileResult;
                            campo.KMZnombre = mod.archivo;
                            campo.LocalidadId = mod.localidad;
                            campo.ArrendaPropia = mod.hectareas;
                            campo.Coordenadas = mod.coordenadas;
                            campo.ProveedorId = (int)oParam.ProveedorId;
                            #endregion

                            #region Eliminar Campo Material

                            var oCampoMaterialSave = repositorio.Listar<CampoMaterial>(x => x.CampoId == mod.CampoId);
                            foreach (var can in oParam.produccion.CamposProduccion)
                            {
                                if (can.eliminarproduccion != null && can.eliminarproduccion.Count > 0)
                                {
                                    foreach (var camp in oCampoMaterialSave)
                                    {
                                        if (can.eliminarproduccion.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                                        {
                                            repositorio.Remover(camp);
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Agregar Campo material
                            if (mod.granos != null)
                            {
                                foreach (var gra in mod.granos)
                                {
                                    if (!oCampoMaterialSave.Any(x => x.MaterialId == gra.granoId && x.CampañaId == gra.campañaId))
                                    {
                                        repositorio.Agregar(new CampoMaterial()
                                        {
                                            NroItem = 1,
                                            MaterialId = gra.granoId,
                                            CampañaId = gra.campañaId,
                                            Campo = campo,
                                            Hectareas = gra.hectareas,
                                            Toneladas = gra.toneladas
                                        });
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Material

                            foreach (var campmaterial in oCampoMaterialSave)
                            {
                                if (mod.granos != null && mod.granos.Any(x => x.granoId == campmaterial.MaterialId && x.campañaId == campmaterial.CampañaId))
                                {
                                    var grano = mod.granos.Where(x => x.granoId == campmaterial.MaterialId && x.campañaId == campmaterial.CampañaId).First();
                                    campmaterial.Campo = campo;
                                    campmaterial.CampañaId = grano.campañaId;
                                    campmaterial.MaterialId = grano.granoId;
                                    campmaterial.Hectareas = grano.hectareas;
                                    campmaterial.Toneladas = grano.toneladas;
                                }
                            }
                            #endregion
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private GrabarProveedorResult UpdateAlmacenamiento(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oAcopioSave = repositorio.Listar<Acopio>(x => x.ProveedorId == oParam.ProveedorId);

                #region Eliminar Almacenamiento

                foreach (var cam in oAcopioSave)
                {
                    if ((oParam.almacenamiento != null && !oParam.almacenamiento.CamposAlmacenamiento.Any(x => x.CampoId == cam.AcopioId)) || oParam.almacenamiento == null)
                    {
                        var oCampoEliminar = repositorio.Listar<AcopioMaterial>(x => x.AcopioId == cam.AcopioId);
                        var oAcopioCampañaEliminar = repositorio.Listar<AcopioCampaña>(x => x.AcopioId == cam.AcopioId);

                        repositorio.RemoverTodos(oCampoEliminar);
                        repositorio.RemoverTodos(oAcopioCampañaEliminar);
                        repositorio.Remover(cam);
                    }
                }
                #endregion

                #region Agregar
                if (oParam.almacenamiento != null)
                {
                    foreach (var cam in oParam.almacenamiento.CamposAlmacenamiento)
                    {
                        if (cam.CampoId.GetValueOrDefault(0) == 0)
                        {
                            var acopio = repositorio.Agregar(new Acopio()
                            {
                                Coordenadas = cam.coordenadasAlmacenamiento,
                                KMZfile = cam.archivoFileResult,
                                KMZnombre = cam.archivo,
                                LocalidadId = cam.localidad,
                                NroItem = 1,
                                ProveedorId = (int)oParam.ProveedorId
                            });

                            if (cam.granosAlmacenamientoGrano != null)
                            {
                                foreach (var valor in cam.granosAlmacenamientoGrano)
                                {
                                    repositorio.Agregar(new AcopioMaterial()
                                    {
                                        NroItem = 1,
                                        CampañaId = valor.campañaId,
                                        Acopio = acopio,
                                        MaterialId = valor.granoId,
                                        Toneladas = valor.toneladasAlmacenamiento
                                    });

                                }
                            }
                            if (cam.granosAlmacenamiento != null)
                            {
                                foreach (var acopiocamp in cam.granosAlmacenamiento)
                                {
                                    repositorio.Agregar(new AcopioCampaña()
                                    {
                                        NroItem = 1,
                                        CampañaId = acopiocamp.campañaId,
                                        Acopio = acopio,
                                        HasArrendadas = acopiocamp.hasArrendadas,
                                        Toneladas = acopiocamp.toneladasAlmacenamiento
                                    });
                                }
                            }
                        }
                    }
                }
                #endregion

                #region Modificar Almacenamiento

                foreach (var acopio in oAcopioSave)
                {
                    if (oParam.almacenamiento != null)
                    {
                        if (oParam.almacenamiento.CamposAlmacenamiento.Any(x => x.CampoId == acopio.AcopioId))
                        {
                            var mod = oParam.almacenamiento.CamposAlmacenamiento.Where(x => x.CampoId == acopio.AcopioId).First();

                            #region Set Campos
                            acopio.AcopioId = (int)mod.CampoId;
                            acopio.Coordenadas = mod.coordenadasAlmacenamiento;
                            acopio.KMZfile = mod.archivoFileResult;
                            acopio.KMZnombre = mod.archivo;
                            acopio.LocalidadId = mod.localidad;
                            acopio.ProveedorId = (int)oParam.ProveedorId;
                            #endregion

                            var oCampoMaterialSave = repositorio.Listar<AcopioMaterial>(x => x.Acopio.AcopioId == mod.CampoId);
                            var oCampoCampañaSave = repositorio.Listar<AcopioCampaña>(x => x.Acopio.AcopioId == mod.CampoId);

                            #region Eliminar Campo Material

                            foreach (var can in oParam.almacenamiento.CamposAlmacenamiento)
                            {
                                if (can.eliminargranoalmacenamientograno != null && can.eliminargranoalmacenamientograno.Count > 0)
                                {
                                    foreach (var camp in oCampoMaterialSave)
                                    {
                                        if (can.eliminargranoalmacenamientograno.Any(x => x.campañaId == camp.CampañaId && x.granoId == camp.MaterialId))
                                        {
                                            repositorio.Remover(camp);
                                        }
                                    }
                                }

                                if (can.eliminargranoalmacenamiento != null && can.eliminargranoalmacenamiento.Count > 0)
                                {
                                    foreach (var camp in oCampoCampañaSave)
                                    {
                                        if (can.eliminargranoalmacenamiento.Any(x => x.campañaId == camp.CampañaId && x.hasArrendadas == camp.HasArrendadas))
                                        {
                                            repositorio.Remover(camp);
                                        }
                                    }
                                }

                            }

                            #endregion

                            #region Agregar Campo material
                            if (mod.granosAlmacenamientoGrano != null)
                            {
                                foreach (var gra in mod.granosAlmacenamientoGrano)
                                {
                                    if (!oCampoMaterialSave.Any(x => x.CampañaId == gra.campañaId && x.MaterialId == gra.granoId))
                                    {
                                        repositorio.Agregar(new AcopioMaterial()
                                        {
                                            NroItem = 1,
                                            CampañaId = gra.campañaId,
                                            MaterialId = gra.granoId,
                                            Acopio = acopio,
                                            Toneladas = gra.toneladasAlmacenamiento
                                        });
                                    }
                                }
                            }
                            #endregion

                            #region Agregar Campo Campaña
                            if (mod.granosAlmacenamiento != null)
                            {
                                foreach (var camp in mod.granosAlmacenamiento)
                                {
                                    if (!oCampoCampañaSave.Any(x => x.CampañaId == camp.campañaId))
                                    {
                                        repositorio.Agregar(new AcopioCampaña()
                                        {
                                            NroItem = 1,
                                            CampañaId = camp.campañaId,
                                            HasArrendadas = camp.hasArrendadas,
                                            Acopio = acopio,
                                            Toneladas = camp.toneladasAlmacenamiento,
                                        });
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Material
                            foreach (var campmaterial in oCampoMaterialSave)
                            {
                                if (mod.granosAlmacenamientoGrano != null)
                                {
                                    if (mod.granosAlmacenamientoGrano.Any(x => x.campañaId == campmaterial.CampañaId && x.granoId == campmaterial.MaterialId))
                                    {
                                        var grano = mod.granosAlmacenamientoGrano.Where(x => x.campañaId == campmaterial.CampañaId && x.granoId == campmaterial.MaterialId).First();
                                        campmaterial.Acopio = acopio;
                                        campmaterial.CampañaId = grano.campañaId;
                                        campmaterial.Toneladas = grano.toneladasAlmacenamiento;
                                        campmaterial.MaterialId = grano.granoId;
                                    }
                                }
                            }
                            #endregion

                            #region Modificar Campo Campaña
                            foreach (var campcampaña in oCampoCampañaSave)
                            {
                                if (mod.granosAlmacenamiento != null)
                                {
                                    if (mod.granosAlmacenamiento.Any(x => x.campañaId == campcampaña.CampañaId))
                                    {
                                        var campaña = mod.granosAlmacenamiento.Where(x => x.campañaId == campcampaña.CampañaId).First();
                                        campcampaña.Acopio = acopio;
                                        campcampaña.CampañaId = campaña.campañaId;
                                        campcampaña.Toneladas = campaña.toneladasAlmacenamiento;
                                        campcampaña.HasArrendadas = campaña.hasArrendadas;
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        private Historial Comprar(int proveedorId, string UsuarioDirectory, List<int> equipo)
        {
            var historial = new Historial();

            try
            {
                List<ZMPES5130> hist = new List<ZMPES5130>();

                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
                var listMateriales = repositorio.Listar<CampañaMaterial, Material>(x => x.Material, x => x.Proveedor.ProveedorId == proveedorId);
                foreach (var material in listMateriales)
                {
                    var campaniaActual = material.Campaña.CampañaId;
                    var oCampañaMaterialAnteriorActualNueva = repositorio.Listar<CampañaMaterial>(x => x.Proveedor.ProveedorId == proveedorId && (x.Campaña.CampañaId == (campaniaActual - 1) || x.Campaña.CampañaId == campaniaActual || x.Campaña.CampañaId == (campaniaActual + 1)) && x.Material.MaterialId == material.MaterialId);

                    foreach (var cmaux in oCampañaMaterialAnteriorActualNueva)
                    {
                        var oCampañaMaterialPorMes = repositorio.SelStore<CampañaMaterialPorMes>("DataAgro_ComprasPorComercialId", 0, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), cmaux.CampañaMaterialId);

                        foreach (var cmpm in oCampañaMaterialPorMes)
                        {
                            hist.Add(new ZMPES5130
                            {
                                ANIO = cmpm.Año.Value.ToString(),
                                COSECHA = cmaux.Campaña.Descripcion,
                                MATERIAL = material.MaterialId.ToString(),
                                MES = cmpm.Mes.Value.ToString(),
                                TN_COMPRADAS = decimal.Parse(cmpm.Toneladas.Value.ToString()),
                                VENDEDOR = proveedor.CUIT
                            });
                        }

                    }
                }

                var listMaterial = hist.GroupBy(z => z.MATERIAL).ToList();

                var ct = new campañaTotal();
                ct.Nombre = "0";

                var list = new List<GranoHistorial>();
                foreach (var mat in listMaterial)
                {
                    ct.grano.Add(new GranoTotales()
                    {
                        Grano = listMateriales.Where(x => x.MaterialId.ToString() == mat.Key).First().Descripcion,
                        Total = hist.Where(x => x.MATERIAL == mat.Key).Sum(x => x.TN_COMPRADAS).ToString()
                    });

                    var Camp = hist.Where(x => x.MATERIAL == mat.Key).GroupBy(z => z.COSECHA);
                    var gr = new GranoHistorial() { Nombre = listMateriales.Where(x => x.MaterialId.ToString() == mat.Key).First().Descripcion };
                    foreach (var camp in Camp)
                    {
                        var granosPorCampaña = hist.Where(x => x.MATERIAL == mat.Key && x.COSECHA == camp.Key)
                            .Select(x => new CampañaPorMes() { Mes = int.Parse(x.MES), Año = int.Parse(x.ANIO), Total = (double)x.TN_COMPRADAS })
                            .OrderBy(x => x.Año).ThenBy(x => x.Mes).ToList();

                        gr.campañas.Add(new CampañaHistorial() { Nombre = camp.Key, Campañas = granosPorCampaña });
                    }

                    list.Add(gr);
                }

                historial.camp.Add(ct);

                historial.HistorialGrano = list;

                var listCampaña = hist.GroupBy(z => z.COSECHA).ToList();

                foreach (var ca in listCampaña)
                {
                    ct = new campañaTotal();
                    ct.Nombre = ca.Key;
                    var materiales = hist.Where(x => x.COSECHA == ca.Key).GroupBy(z => z.MATERIAL);

                    foreach (var m in materiales)
                    {
                        var total = hist.Where(x => x.MATERIAL == m.Key && x.COSECHA == ca.Key).Sum(x => x.TN_COMPRADAS);
                        ct.grano.Add(new GranoTotales() { Grano = listMateriales.Where(x => x.MaterialId.ToString() == m.Key).First().Descripcion, Total = total.ToString() });
                    }
                    historial.camp.Add(ct);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return historial;
        }

        public List<ReporteProveedor> ObtenerReporteProveedor(string Valor, string idActiveDirectory)
        {

            var comercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory.ToLower() == idActiveDirectory.ToLower(), x => x.ComercialId);

            return repositorio.SelStore<ReporteProveedor>("DataAgro_ReporteProveedor", 0, Valor, comercialId);
        }
        public List<BusquedaHome> DevolverProveedoresConCorredor(string filtro, string cuitCorredor)
        {
            var resultado = repositorio.ListarConsulta(new DevolverProveedoresConCorredor(filtro, cuitCorredor));

            return resultado;
        }
        public List<BusquedaHome> DevolverProveedores(string filtro, int corredor, List<int> equipo)
        {
            var resultado = repositorio.ListarConsulta(new DevolverProveedores(filtro, corredor, equipo));
            var lista = resultado.GroupBy(x => new { x.Cuit, x.Filtro }).ToList();
            resultado = lista.Select(x => new BusquedaHome
            {
                Cuit = x.Key.Cuit,
                Filtro = x.Key.Filtro,
                RazonSocial = resultado.FirstOrDefault(y => y.Cuit == x.Key.Cuit).RazonSocial,
                Id = resultado.FirstOrDefault(y => y.Cuit == x.Key.Cuit).Id
            }).ToList();
            return resultado;
        }
        public List<BusquedaHome> DevolverProveedoresCorredores(string filtro)
        {
            var resultado = repositorio.ListarConsulta(new DevolverProveedoresCorredores(filtro));
            //var lista = resultado.GroupBy(x => new { x.Cuit, x.Filtro }).ToList();
            //resultado = lista.Select(x => new BusquedaHome
            //{
            //    Cuit = x.Key.Cuit,
            //    Filtro = x.Key.Filtro,
            //    RazonSocial = resultado.FirstOrDefault(y => y.Cuit == x.Key.Cuit).RazonSocial,
            //    Id = resultado.FirstOrDefault(y => y.Cuit == x.Key.Cuit).Id
            //}).ToList();
            return resultado;
        }
        public List<ProveedorDto> ListarProveedor(string proveedor)
        {
            return repositorio.Listar<Proveedor, ProveedorDto>(x => new ProveedorDto { CUIT = x.CUIT, ProveedorId = x.ProveedorId, RazonSocial = x.RazonSocial }, x => proveedor == "" || (x.RazonSocial.Contains(proveedor) || x.CUIT.Contains(proveedor)), 15);
        }
        public List<ProveedorDto> ListarCorredor(string proveedor)
        {
            return repositorio.Listar<Proveedor, ProveedorDto>(x => new ProveedorDto { CUIT = x.CUIT, ProveedorId = x.ProveedorId, RazonSocial = x.RazonSocial }, x => (proveedor == "" || x.RazonSocial.Contains(proveedor) || x.CUIT.Contains(proveedor)) && x.Segmentacion.Grupo == "Corredores", 15);
        }
        public List<ProveedorCorredorDto> ListarProveedorCorredor(int corredorId)
        {
            var proveedores = repositorio.Listar<CorredorProveedor, ProveedorCorredorDto>(x => new ProveedorCorredorDto
            {
                CUIT = x.Proveedor.CUIT,
                ProveedorId = x.ProveedorId,
                RazonSocial = x.Proveedor.RazonSocial,
                ClasificacionCompraNetId = x.Proveedor.ClasificacionCompraNetId,
                ClasificacionDescripcion = x.Proveedor.ClasificacionCompraNet.Descripcion,
                Localidad = x.Proveedor.Localidad.Nombre,
                LocalidadId = x.Proveedor.LocalidadId,
                ProvinciaId = x.Proveedor.ProvinciaId,
                Provincia = x.Proveedor.Provincia.Nombre,
                LocalidadCompraNet = x.Proveedor.LocalidadCompraNet.Nombre,
                LocalidadCompraNetId = x.Proveedor.LocalidadCompraNetId,
                ProvinciaCompraNet = x.Proveedor.ProvinciaCompraNet.Nombre,
                ProvinciaCompraNetId = x.Proveedor.ProvinciaCompraNetId,
                Direccion = x.Proveedor.Direccion,
                CodigoPostal = x.Proveedor.CodigoPostal,
                ProveedorCorredorId = x.Id,
                Consignatario = x.Proveedor.Consignatario,
                RiesgoComercialSap = x.Proveedor.RiesgoComercialSap,
            }, x => x.CorredorId == corredorId); ;

            foreach (var aux in proveedores)
            {
                aux.NoOperable = false;
                aux.Operando = true;
                aux.EstadoCuit = repositorio.Obtener<SISA, int>(y => y.CUIT == aux.CUIT, y => y.EstadoCuit);
                aux.Facacop = repositorio.Existe<FACACOP>(x => x.CUIT == aux.CUIT);

                if (!String.IsNullOrEmpty(aux.RiesgoComercialSap))
                {
                    if (aux.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                    {
                        aux.NoOperable = true;
                        aux.Operando = false;
                        aux.TooltipNoOperable = "Riesgo Comercial Alto";
                    }
                }

                if (aux.EstadoCuit == 0)
                {
                    aux.NoOperable = true;
                    aux.Operando = false;
                    aux.TooltipNoOperable = "Inactivo";
                }
                if (aux.EstadoCuit == 3)
                {
                    aux.NoOperable = true;
                    aux.Operando = false;
                    aux.TooltipNoOperable = "Estado 3";
                }
                if (aux.Facacop)
                {
                    aux.Operando = false;
                    aux.NoOperable = true;
                    aux.TooltipNoOperable = "Apocrifos";
                }
            }

            return proveedores;
        }
        public TraerProveedorResult TraerProveedorParaCorredor(string cuit)
        {
            var proveedorResult = new TraerProveedorResult();
            var proveedor = repositorio.Obtener<Proveedor, ProveedorDto>(x => x.CUIT == cuit && (x.SegmentacionId != 5 && x.SegmentacionId != 7), x => new ProveedorDto
            {
                CUIT = x.CUIT,
                ProveedorId = x.ProveedorId,
                RazonSocial = x.RazonSocial,
                ClasificacionCompraNetId = x.ClasificacionCompraNetId,
                ClasificacionDescripcion = x.ClasificacionCompraNet.Descripcion,
                Localidad = x.Localidad.Nombre,
                LocalidadId = x.LocalidadId,
                ProvinciaId = x.Localidad.ProvinciaId,
                Provincia = x.Localidad.Provincia.Nombre,
                LocalidadCompraNet = x.LocalidadCompraNet.Nombre,
                LocalidadCompraNetId = x.LocalidadCompraNetId,
                ProvinciaCompraNet = x.ProvinciaCompraNet.Nombre,
                ProvinciaCompraNetId = x.ProvinciaCompraNetId,
                Direccion = x.Direccion,
                CodigoPostal = x.CodigoPostal,
                Consignatario = x.Consignatario
            });
            if (proveedor != null)
            {
                proveedorResult.Proveedor = proveedor;
            }
            else
            {
                proveedorResult.Error("", "El Proveedor no existe");
            }
            return proveedorResult;
        }
        private class ZMPES5130
        {
            public string VENDEDOR;

            public string MATERIAL;

            public string COSECHA;

            public string MES;

            public string ANIO;

            public decimal TN_COMPRADAS;
        }

        private List<BasicoProveedor> TraerDatosBasicosProveedor(int proveedorId, int comercialId, List<int> equipo)
        {
            return repositorio.ListarConsulta(new TraerDatosBasicosProveedor(proveedorId, comercialId, equipo));
        }

        public GrabarProveedorResult GrabarNuevoCorredor(NuevoCorredor oParam, string idActiveDirectory)
        {
            var result = GrabarNuevoProveedor(new NuevoProveedor()
            {
                basicos = oParam.basicos,
                contacto = oParam.contacto,
                contactocomercial = oParam.contactocomercial,
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento(),
            }, idActiveDirectory);
            int provId;
            if (result.HayError)
            {
                return result;
            }
            else
            {
                provId = result.ProveedorId.Value;
            }
            if (oParam.proveedorCorredor != null)
            {
                result = UpdateProveedorCorredor(oParam.proveedorCorredor, result.ProveedorId.Value, idActiveDirectory);
            }
            result.ProveedorId = provId;
            return result;
        }
        public GrabarProveedorResult UpdateCorredor(NuevoCorredor oParam, string idActiveDirectory, List<int> equipo, int comercialId)
        {
            var corredor = new NuevoProveedor()
            {
                ProveedorId = oParam.CorredorId,
                basicos = oParam.basicos,
                contacto = oParam.contacto,
                contactocomercial = oParam.contactocomercial,
                produccion = new Produccion(),
                almacenamiento = new Almacenamiento()

            };
            var resultado = UpdateDatosBasicosProveedor(corredor, idActiveDirectory, equipo);

            if (resultado.HayErrores)
            {
                resultado.ProveedorId = oParam.CorredorId;
                return resultado;
            }
            resultado = UpdateDatosContacto(corredor);

            if (resultado.HayErrores)
            {
                resultado.ProveedorId = oParam.CorredorId;
                return resultado;
            }
            resultado = UpdateContactoComerciales(corredor);

            if (resultado.HayErrores)
            {
                resultado.ProveedorId = oParam.CorredorId;
                return resultado;
            }
            resultado = UpdateProveedorCorredor(oParam.proveedorCorredor, oParam.CorredorId.Value, idActiveDirectory);
            if (resultado.HayErrores)
            {
                resultado.ProveedorId = oParam.CorredorId;
                return resultado;
            }
            try
            {
                resultado.ProveedorId = oParam.CorredorId;
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
                return resultado;
            }

            resultado.ProveedorId = oParam.CorredorId;

            return resultado;
        }

        private GrabarProveedorResult UpdateProveedorCorredor(List<NuevoProveedor> proveedores, int corredorId, string idActiveDirectory)
        {
            var resultado = new GrabarProveedorResult();
            var sinError = new List<NuevoProveedor>();
            if (proveedores != null)
            {
                foreach (var proveedor in proveedores)
                {
                    try
                    {

                        var nuevoProveedorParaCorredor = new Proveedor
                        {
                            CUIT = proveedor.basicos.cuit,
                            RazonSocial = proveedor.basicos.RazonSocial,
                            SegmentacionId = proveedor.basicos.segmentacion != 0 ? proveedor.basicos.segmentacion : 2,
                            ProvinciaCompraNetId = proveedor.basicos.ProvinciaCompraNet,
                            LocalidadCompraNetId = proveedor.basicos.LocalidadCompraNet,
                            ClasificacionCompraNetId = proveedor.basicos.ClasificacionCompraNet,
                            BoletoCompraNetId = proveedor.basicos.BoletoCompraNet,
                            BolsaCompraNetId = proveedor.basicos.BolsaCompraNet,
                            Consignatario = proveedor.basicos.Consignatario,
                            CodigoPostal = proveedor.contacto.codpost,
                            Direccion = proveedor.contacto.direccion,
                            LocalidadId = proveedor.contacto.localidad,
                            ProvinciaId = proveedor.contacto.provincia,
                            EstadoId = 1,
                            FechaAlta = DateTime.Now
                        };
                        var res = new GrabarProveedorResult();
                        res = ValidarProveedor(proveedor, res, true);
                        if (res.HayError)
                        {
                            resultado.Errores.AddRange(res.Errores);
                            continue;
                        }
                        else
                        {
                            sinError.Add(proveedor);
                        }
                        if (proveedor.ProveedorId != 0 && proveedor.ProveedorId != null)
                        {
                            var proveedorUpdate = repositorio.Obtener<Proveedor>(x => x.ProveedorId == proveedor.ProveedorId);
                            proveedorUpdate.SegmentacionId = proveedor.basicos.segmentacion != 0 ? proveedor.basicos.segmentacion : proveedorUpdate.SegmentacionId;
                            proveedorUpdate.ProvinciaCompraNetId = proveedor.basicos.ProvinciaCompraNet != null ? proveedor.basicos.ProvinciaCompraNet : proveedorUpdate.ProvinciaCompraNetId;
                            proveedorUpdate.LocalidadCompraNetId = proveedor.basicos.LocalidadCompraNet != null ? proveedor.basicos.LocalidadCompraNet : proveedorUpdate.LocalidadCompraNetId;
                            proveedorUpdate.ClasificacionCompraNetId = proveedor.basicos.ClasificacionCompraNet != null ? proveedor.basicos.ClasificacionCompraNet : proveedorUpdate.ClasificacionCompraNetId;
                            proveedorUpdate.BoletoCompraNetId = proveedor.basicos.BoletoCompraNet != null ? proveedor.basicos.BoletoCompraNet : proveedorUpdate.BoletoCompraNetId;
                            proveedorUpdate.BolsaCompraNetId = proveedor.basicos.BolsaCompraNet != null ? proveedor.basicos.BolsaCompraNet : proveedorUpdate.BolsaCompraNetId;
                            proveedorUpdate.Consignatario = proveedor.basicos.Consignatario;
                            proveedorUpdate.CodigoPostal = proveedor.contacto.codpost != null ? proveedor.contacto.codpost : proveedorUpdate.CodigoPostal;
                            proveedorUpdate.Direccion = proveedor.contacto.direccion != null ? proveedor.contacto.direccion : proveedorUpdate.Direccion;
                            proveedorUpdate.ProvinciaId = proveedor.contacto.provincia != null ? proveedor.contacto.provincia : proveedorUpdate.ProvinciaId;
                            proveedorUpdate.LocalidadId = proveedor.contacto.localidad != null ? proveedor.contacto.localidad : proveedorUpdate.LocalidadId;

                        }
                        else
                        {
                            var entidad = repositorio.Agregar(nuevoProveedorParaCorredor);
                            repositorio.GuardarCambios();
                            proveedor.ProveedorId = entidad.ProveedorId;
                        }
                    }
                    catch (Exception ex)
                    {
                        resultado.Error("", ex.Message);
                        return resultado;
                    }
                }
            }
            try
            {
                var oCorredorSave = repositorio.Listar<CorredorProveedor>(x => x.CorredorId == corredorId);
                #region Eliminar

                foreach (var cor in oCorredorSave)
                {
                    if ((proveedores != null && !proveedores.Any(x => x.ProveedorCorredorId == cor.Id)) || proveedores == null)
                    {
                        var corProv = repositorio.Obtener<CorredorProveedor>(x => x.Id == cor.Id);
                        repositorio.Remover(corProv);
                    }
                }

                #endregion

                #region Agregar
                if (sinError != null)
                {
                    foreach (var cor in sinError)
                    {
                        if (cor.ProveedorCorredorId == null)
                        {
                            var proveedorCorredor = new CorredorProveedor()
                            {
                                ProveedorId = cor.ProveedorId.HasValue ? cor.ProveedorId.Value : repositorio.Obtener<Proveedor, int>(x => x.CUIT == cor.basicos.cuit && x.RazonSocial == cor.basicos.RazonSocial && x.Segmentacion.Grupo != "Corredores", x => x.ProveedorId),
                                CorredorId = corredorId,
                            };
                            repositorio.Agregar(proveedorCorredor);
                        }
                    }
                }
                #endregion
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Error("", ex.Message);
            }
            return resultado;
        }

        public DatosCompraNetDto TraerBoletoBolsa(int id)
        {
            return repositorio.Obtener<Proveedor, DatosCompraNetDto>(x => x.ProveedorId == id, x => new DatosCompraNetDto()
            {
                BoletoCompraNetId = x.BoletoCompraNetId,
                BolsaCompraNetId = x.BolsaCompraNetId,
                LocalidadId = x.LocalidadId,
                ProvinciaId = x.ProvinciaId,
                ProveedorId = x.ProveedorId,
                ClasificacionCompraNetId = x.ClasificacionCompraNetId
            });
        }

        public bool ValidarProveedorEsCorredor(int idproveedor)
        {
            return repositorio.Obtener<Proveedor>(x => x.ProveedorId == idproveedor).Segmentacion.Grupo == "Corredores";
        }

        public string TraerCuit(int id)
        {
            return repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == id, x => x.CUIT);
        }
        public GrabarProveedorResult GrabarRol(int id, List<Rol> roles)
        {
            var resultado = new GrabarProveedorResult();
            var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId ==id, x => x.CUIT);
            var proveedores = repositorio.Listar<Proveedor>(x => x.CUIT == cuit);

            foreach (var proveedor in proveedores)
            {
                var listaRoles = roles.Select(y => y.Id).ToList();
                if (proveedor.RolesAsociados != null)
                {
                    proveedor.RolesAsociados.Clear();
                }
                else
                {
                    proveedor.RolesAsociados = new List<Rol>();
                }
                proveedor.RolesAsociados = repositorio.Listar<Rol>(x => listaRoles.Any(y => y == x.Id));
                try
                {
                    repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    resultado.Error("Roles", e.Message);
                }
            }
            return resultado;
        }

        public List<RolBasicoDto> TraerRolesProveedor(int id)
        {
            var roles = repositorio.Obtener<Proveedor, ICollection<Rol>>(x => x.ProveedorId == id, x => x.RolesAsociados);
            return roles.Select(y => new RolBasicoDto { Descripcion = y.Descripcion, Id = y.Id }).ToList();
        }
        public bool ValidarDirecto(string cuit){
            return !repositorio.Existe<CorredorProveedor>(x => x.Corredor.CUIT == cuit);
        }
    }
}
