using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
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
        private IRepositorio repositorio;
        private IComercialManager mobComercial;

        private ILogger logger;

        public ProveedorManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial)
        {
            this.logger = logger;
            mobComercial = oComercial;
            this.repositorio = repositorio;
        }

        public StoredHistorialResult TraerHistorialActividad(HistorialActiviad oParam, int ProveedorId, string TipoActividadId)
        {
            return new StoredHistorialResult
            {
                ActividadHistoriaTraerPorProveedores = repositorio.SelStore<HistorialTraer>("DataAgro_ActividadHistoriaTraerPorProveedorId", 0, ProveedorId, oParam.detalle, TipoActividadId) 
            };
        }

        public StoredPorProveedorResult TraerProveedor(int ProveedorId, string UsuarioDirectory, List<int> equipo)
        {
            var res = new StoredPorProveedorResult();
            try
            {
                var oComerciales = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == UsuarioDirectory);
                res.BasicoProveedorTraerPorProveedores = TraerDatosBasicosProveedor(ProveedorId, oComerciales.ComercialId, equipo);
                res.ActividadTraerPorProveedores = repositorio.SelStore<ActividadTraer>("DataAgro_ActividadTraerPorProveedorId", 0, ProveedorId);
                res.ContactosComercialesTraerPorProveedores = repositorio.SelStore<ContactosComerciales>("DataAgro_ContactosComercialesTraerPorProveedorId", 0, ProveedorId);
                res.ActividadHistoriaTraerPorProveedores = repositorio.SelStore<ActividadTraer>("DataAgro_ActividadHistoriaTraerPorProveedorId", 0, ProveedorId, null, null);
                res.ObjetivosTraerPorProveedorId = repositorio.SelStore<ObjetivosTraer>("DataAgro_ObjetivosTraerPorProveedorId", 0, ProveedorId);
                res.AcopioMaterialPorProveedores = repositorio.SelStore<AcopioMaterialPorProveedor>("DataAgro_AcopioMaterialPorProveedorId", 0, ProveedorId);
                var campoacopio = repositorio.SelStore<CampoProduccionAcopio>("DataAgro_CampoProduccionAcopioPorProveedorId", 0, ProveedorId);
                res.CampoProduccionAcopioPorProveedores = campoacopio.Where(z => z.EsCampoProduccion == true).ToList();
                res.Acopio = campoacopio.Where(z => z.EsCampoProduccion == false).ToList();

                res.DatosContacto = DevolverDatosContacto(ProveedorId);
                res.Historial = Comprar(ProveedorId, UsuarioDirectory);
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

                if (!String.IsNullOrEmpty(aux.Situacion))
                {
                    if ((aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitNoIncluida"])
                         || (aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitExcluido"])
                             || (aux.Situacion.ToLower() == ConfigurationManager.AppSettings["SitSuspendido"]))
                    {
                        aux.NoOperable = true;
                        aux.Operando = false;
                        aux.TooltipNoOperable = aux.Situacion;
                    }
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
                    valor = repositorio.Obtener<Localidad, DatosLocalidadProvincia>(x => x.LocalidadId == oProveedor.LocalidadCompraNet.LocalidadId,
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
                valor.BoletoId = oProveedor.BoletoCompraNetId ?? 0;
                valor.BolsaId = oProveedor.BolsaCompraNetId ?? 0;
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

            Actividad oActividadSave;

            if (oParam.ActividadId == 0)
            {
                oActividadSave = new Actividad();
            }
            else
            {
                oActividadSave = repositorio.Obtener<Actividad>(x => x.ActividadId == oParam.ActividadId) ?? new Actividad();
            }

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

                if ((oParam.tipoactividad == Convert.ToInt32(ConfigurationManager.AppSettings["AgendaCita"])))
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
                    oMensaje.To.Add(ConfigurationManager.AppSettings["maildeUsuarios"]);
                else
                    oMensaje.To.Add(GetEmailUserActiveDirectory(oParam.UserName));


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

                oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";
                oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);

                oCliente.Send(oMensaje);

            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void EnviarEmail(Contrato oContrato, string idActiveDirectory)
        {
            try
            {
                var proveedorContacto = repositorio.Obtener<ContactoComercial>(x => x.ProveedorId == oContrato.ProveedorId);
                
                string emailComercial = "";
                string emailJefe = "";

                if (oContrato.Comercial != null)
                {
                    try { emailComercial = GetEmailUserActiveDirectory(oContrato.Comercial.IdActiveDirectory); } catch (Exception e){ logger.Error(e); }

                    try { emailJefe = GetEmailUserActiveDirectory(oContrato.Comercial.EmpleadorACargo.IdActiveDirectory); } catch (Exception e){ logger.Error(e); }
                }

                if ((proveedorContacto != null && proveedorContacto.Email1 != "" && proveedorContacto.Email1 != null) || (emailJefe != null && emailJefe != "") || (emailComercial != null && emailComercial == ""))
                {
                    var oMensaje = new MailMessage
                    {
                        From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                    };

                    oMensaje.To.Add(proveedorContacto.Email1);
                    if (emailJefe != "" && emailJefe != null) oMensaje.CC.Add(emailJefe);
                    if (emailComercial != "" && emailComercial != null) oMensaje.CC.Add(emailComercial);
                    
                    oMensaje.AlternateViews.Add(CuerpoMailContrato(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), oContrato, emailComercial));

                    oMensaje.Subject = "Nuevo negocio Molinos Agro S.A. - " + oContrato.Proveedor.RazonSocial;

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

                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";                    
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                            ConfigurationManager.AppSettings["CredentialPassword"]);
                    oCliente.Send(oMensaje);
                }
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
                var proveedorContacto = repositorio.Obtener<ContactoComercial>(x => x.ProveedorId == oFijacionDePrecioContrato.ProveedorId);
                
                string emailComercial = "";
                string emailJefe = "";

                if (oFijacionDePrecioContrato.Comercial != null)
                {
                    try { emailComercial = GetEmailUserActiveDirectory(oFijacionDePrecioContrato.Comercial.IdActiveDirectory); } catch (Exception e) { logger.Error(e); }
                    try { emailJefe = GetEmailUserActiveDirectory(oFijacionDePrecioContrato.Comercial.EmpleadorACargo.IdActiveDirectory); } catch (Exception e) { logger.Error(e); }
                }

                if ((proveedorContacto != null && proveedorContacto.Email1 != "" && proveedorContacto.Email1 != null) || (emailJefe != null && emailJefe != "") || (emailComercial != null && emailComercial == ""))
                {
                    MailMessage oMensaje = new MailMessage();

                    oMensaje.From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]);

                    oMensaje.To.Add(proveedorContacto.Email1);
                    if (emailJefe != "" && emailJefe != null) oMensaje.CC.Add(emailJefe);
                    if (emailComercial != "" && emailComercial != null) oMensaje.CC.Add(emailComercial);

                    oMensaje.AlternateViews.Add(CuerpoMailFijacion(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), oFijacionDePrecioContrato, emailComercial));

                    oMensaje.Subject = "Nuevo negocio Molinos Agro S.A. - DataAgro";
                    
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

                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                            ConfigurationManager.AppSettings["CredentialPassword"]);

                    oCliente.Send(oMensaje);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private AlternateView CuerpoMailContrato(String filePath, Contrato oContrato, string emailComercial)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody = "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />  ";
            htmlBody += oContrato.Fecha.ToShortDateString() + " - ";
            htmlBody += oContrato.Material.Descripcion + " - ";
            htmlBody += "Contrato Nro " + oContrato.ContratoSAP + "<br /> ";
            htmlBody += oContrato.Proveedor.CUIT+ " - " + oContrato.Proveedor.RazonSocial + " - ";
            if (oContrato.ClasificacionId != null) htmlBody += oContrato.Clasificacion.Descripcion + "<br />";
            if (oContrato.DestinoId != null) htmlBody += oContrato.Destino.Descripcion + " - ";
            htmlBody += oContrato.Cantidad.ToString("N0",CultureInfo.CreateSpecificCulture("es-AR")) 
                + " Kg." + " <br />  ";
            if (oContrato.TipoNegocioId == 2) htmlBody += oContrato.Precio.ToString("N2",CultureInfo.CreateSpecificCulture("es-AR")) 
                + " " + oContrato.Moneda.Descripcion + "<br /> ";
            if (oContrato.TipoNegocioId == 1) htmlBody += "A fijar - " + oContrato.FechaHasta.ToShortDateString() + "<br />  ";
            htmlBody += oContrato.Localidad.Nombre + " - " + oContrato.Provincia.Nombre + "<br />  ";
            htmlBody += "Entrega Desde " + oContrato.FechaDesde.ToShortDateString() + " - Hasta " + oContrato.FechaHasta.ToShortDateString() + "<br />  ";
            htmlBody += oContrato.Campana.Descripcion + " - ";
            if (oContrato.BoletoId != null) htmlBody += "Boleto " + oContrato.Boleto.Descripcion + "  ";
            if (oContrato.BoletoId != 3) htmlBody += oContrato.Bolsa.Descripcion + "  <br />  ";
            if (oContrato.Observacion != null) htmlBody += oContrato.Observacion + "  <br />  ";

            htmlBody += "<br /><br />  Por consultas, contactarse con " + (oContrato.Comercial != null ? oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
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
            string htmlBody = "";
            htmlBody = "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />";
            htmlBody += oFijacionDePrecioContrato.Fecha.ToShortDateString() + "<br />";
            htmlBody += oFijacionDePrecioContrato.Material.Descripcion + "<br />";
            htmlBody += oFijacionDePrecioContrato.ContratoId + "<br />";
            htmlBody += oFijacionDePrecioContrato.Proveedor.CUIT + " - " + oFijacionDePrecioContrato.Proveedor.RazonSocial + "<br />";
            if (oFijacionDePrecioContrato.Proveedor.ClasificacionCompraNet != null) htmlBody += oFijacionDePrecioContrato.Proveedor.ClasificacionCompraNet.Descripcion + " <br />";
            htmlBody += oFijacionDePrecioContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "<br />";
            htmlBody += oFijacionDePrecioContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + " " + oFijacionDePrecioContrato.Moneda.Descripcion + "<br />";
            htmlBody += oFijacionDePrecioContrato.Material.Campaña.Descripcion + " ";

            htmlBody += "<br />  Por consultas, contactarse con " + (oFijacionDePrecioContrato.Comercial != null ? oFijacionDePrecioContrato.Comercial.Nombres + " " + oFijacionDePrecioContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.   <br /><br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        private string GetEmailUserActiveDirectory(string UserName)
        {
            DirectoryEntry entry = new DirectoryEntry();
            string userName = UserName;

            try { userName = UserName.Split('\\')[1]; } catch { }


            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = String.Format("(sAMAccountName={0})", userName);


            search.PropertiesToLoad.Add("givenName");   // first name
            search.PropertiesToLoad.Add("sn");          // last name
            search.PropertiesToLoad.Add("mail");        // smtp mail address

            // perform the search
            SearchResult result = search.FindOne();
            string email = result.Properties["mail"][0].ToString();


            return email;
        }

        public Resultado EliminarRecordatorio(int Id)
        {
            var oEntityErrors = new Resultado();
            repositorio.Remover<Actividad>(Id);
            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public DatosIniProveedor TraerDatosCombo(int ProveedorId)
        {
            var DatosCombo = new DatosIniProveedor
            {
                segm = repositorio.Listar<Segmentacion, SegmentacionQry>(x => new SegmentacionQry() { SegmentacionId = x.SegmentacionId, Descripcion = x.Descripcion, Grupo = x.Grupo }),
                tiptel = repositorio.Listar<TipoTelefono, TipoTelefonoQry>(x => new TipoTelefonoQry() { TipoTelefonoId = x.TipoTelefonoId, Descripcion = x.Descripcion }),
                prov = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry() { Provinciaid = x.ProvinciaId, Nombre = x.Nombre }),
                loc = new List<LocalidadQry>(),
                cope = repositorio.Listar<CanalOperacion, CanalOperacionQry>(x => new CanalOperacionQry() { CanalOperacionId = x.CanalOperacionId, Descripcion = x.Descripcion, Inhabilitado = false }),
                gran = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Codigo = x.Codigo, Descripcion = x.Descripcion/*, CampañaIdActual = x.CampañaIdActual*/ }),
                dest = repositorio.Listar<Destinatario, DestinatarioQry>(x => new DestinatarioQry() { DestinatarioId = x.DestinatarioId, Descripcion = x.Descripcion, Inhabilitado = false }),
                cond = repositorio.Listar<Condicion, CondicionQry>(x => new CondicionQry() { CondicionId = x.CondicionId, Descripcion = x.Descripcion, Inhabilitado = false }),
                inte = repositorio.Listar<Interes, InteresQry>(x => new InteresQry() { InteresId = x.InteresId, Descripcion = x.Descripcion }),
                tipoact = repositorio.Listar<TipoActividad, TipoActividadQry>(x => new TipoActividadQry() { TipoActividadId = x.TipoActividadId, Descripcion = x.Descripcion }),
                concom = repositorio.Listar<ContactoComercial, ContactoComercialQry>(x => new ContactoComercialQry() { ContactoComercialId = x.ContactoComercialId, Nombres = x.Nombres }),
                ClasComNet = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion }),
                BoleComNet = repositorio.Listar<BoletoCompraNet, BoletoCompraNetQry>(x => new BoletoCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion }),
                BolsComNet = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion })
            };

            return DatosCombo;
        }

        public List<LocalidadDto> TraerLocalidad(int Id)
        {
            return repositorio.Listar<Localidad, LocalidadDto>(x => new LocalidadDto { LocalidadId = x.LocalidadId, Nombre = x.Nombre },x => x.ProvinciaId == Id);
        }

        public ProveedorNuevo TraerRazonSocial(string cuit)
        {
            var razonsocial = repositorio.Obtener<RG2300, ProveedorNuevo>(x => x.CUIT == cuit,
                x => new ProveedorNuevo
                {
                    CUIT = x.CUIT,
                    Operable = 1,
                    Condicion = x.Situacion,
                    razonSocial = x.RazonSocial
                });
            if (razonsocial == null)
            {
                razonsocial = new ProveedorNuevo() { Condicion = "no incluido", CUIT = cuit, Operable = 0, razonSocial = "No existe Razon Social" };
            }
            else
            {

                razonsocial.Existe = repositorio.Existe<Proveedor>(x => x.CUIT == cuit) ? 1 : 0;
            }

            TraerEstado(razonsocial);

            return razonsocial;
        }

        public ProveedorQry TraerProveedorPorCuit(string cuit)
        {
            return repositorio.Obtener<Proveedor, ProveedorQry>(x => x.CUIT == cuit, x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial });
        }

        public void TraerEstado(ProveedorNuevo prov)
        {
            var oFacacop = repositorio.Existe<FACACOP>(x => x.CUIT == prov.CUIT);

            if (oFacacop)
            {
                prov.Operable = 0;
                prov.Condicion = "Apocrifos";
                return;
            }
            else if (!String.IsNullOrEmpty(prov.Condicion))
            {
                if ((prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitNoIncluida"])
                     || (prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitExcluido"])
                         || (prov.Condicion.ToLower() == ConfigurationManager.AppSettings["SitSuspendido"]))
                {
                    prov.Operable = 0;
                }
            }

            if (ConfigurationManager.AppSettings["SinConexionSap"] != "1")
            {
                var riesgo = new Agent.RiesgoComerciales();

                prov.RiesgoComercial = riesgo.ObtenerRiesgoComercial(prov.CUIT);

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

            if (repositorio.Existe<Proveedor>(x => x.CUIT == oParam.basicos.cuit))
            {
                oEntityErrors.Error("Proveedor", "Ya existe un proveedor con ese CUIT");
                return oEntityErrors;
            }

            if (!repositorio.Existe<RG2300>(x => x.CUIT == oParam.basicos.cuit))
            {
                oEntityErrors.Error("RG2300", "No existe el CUIT");
                return oEntityErrors;
            }

            if (repositorio.Existe<FACACOP>(x => x.CUIT == oParam.basicos.cuit))
            {
                oEntityErrors.Error("FACACOP", "El CUIT es Apocrifo");
                return oEntityErrors;
            }

            var proveedor = new Proveedor
            {
                AlmacHabilitadoSojaSust = oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null" ? Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)) : (bool?)null,
                AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust,
                AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust,
                AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns,
                AreaInfluencia = oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null" ? repositorio.Obtener<AreaInfluencia>(Convert.ToInt32(oParam.contacto.areaDeInfluencia)) : null,
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
                FechaAlta = DateTime.Now
            };
            var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
            var oEstados = repositorio.Listar<Estado>();
            if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
            {
                var list = new DatosProveedor(logger).ObtenerDatosDeProveedor(new List<Datos> { new Datos { CUIT = oParam.basicos.cuit, UsuarioDirectory = idActiveDirectory } });

                if (list.Count > 0)
                {
                    foreach (var lista in list)
                    {
                        var comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == lista.USUARIO);

                        if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                        {
                            string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                            comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == aux);
                        }

                        if (comercialLista != null)
                        {
                            repositorio.Agregar(new ProveedorEstado
                            {
                                Comercial = comercialLista,
                                Estado = oEstados.Where(x => x.Descripcion.ToLower() == lista.STATUS.ToLower()).FirstOrDefault() ?? oEstados.Where(x => x.EstadoId == 1).FirstOrDefault(),
                                Proveedor = proveedor
                            });
                        }
                        proveedor.ClienteMOA = (!String.IsNullOrEmpty(lista.CLIENTE_MOA) ? true : false);
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
                        TipoTelefono3Id = (param.telefonos[2].tipoTelefono.HasValue ? (int?)param.telefonos[2].tipoTelefono.Value : null)
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

        public GrabarProveedorResult UpdateProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {
            var resultado = UpdateDatosBasicosProveedor(oParam, idActiveDirectory);

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

        public ProveedorDto TraerProveedor(int? proveedorId)
        {
            return proveedorId.HasValue ? repositorio.Obtener<Proveedor, ProveedorDto>(x => x.ProveedorId == proveedorId, 
                x => new ProveedorDto {
                    CUIT = x.CUIT,
                    ProveedorId = x.ProveedorId,
                    RazonSocial = x.RazonSocial
                }) : null;
        }

        public GrabarProveedorResult UpdateDatosBasicosProveedor(NuevoProveedor oParam, string idActiveDirectory)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oProveedorSave = repositorio.Obtener<Proveedor>(oParam.ProveedorId);

                var clasificacion = oParam.basicos.ClasificacionCompraNet != null? oParam.basicos.ClasificacionCompraNet : (oParam.produccion != null && oParam.produccion.CamposProduccion != null && oParam.produccion.CamposProduccion.Any()) ? 1 : (oParam.almacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento != null && oParam.almacenamiento.CamposAlmacenamiento.Any()) ? 2 : oProveedorSave.ClasificacionCompraNetId;
                oProveedorSave.AlmacHabilitadoSojaSust = oParam.produccion.habilitaoSojaSust != null && oParam.produccion.habilitaoSojaSust != "null" ? Convert.ToBoolean(Convert.ToInt32(oParam.produccion.habilitaoSojaSust)) : (bool?)null;
                oProveedorSave.AlmacVolAnualTotal = oParam.produccion.volumenAnualTotalTns;
                oProveedorSave.AlmacHectSojaSust = oParam.produccion.hasAprobSojaSust;
                oProveedorSave.AlmacTonsMaxSojaSust = oParam.produccion.TonsMaxAprobSojaSust;
                oProveedorSave.Calificacion = oParam.basicos.calificacion;
                oProveedorSave.Observaciones = oParam.basicos.comentario;
                oProveedorSave.SegmentacionId = oParam.basicos.segmentacion;
                oProveedorSave.CodigoPostal = oParam.contacto.codpost;
                oProveedorSave.Direccion = oParam.contacto.direccion;
                oProveedorSave.Intermediario = oParam.contacto.intermediario;
                oProveedorSave.LocalidadId = oParam.contacto.localidad;
                oProveedorSave.ProvinciaId = oParam.contacto.provincia;
                oProveedorSave.AreaInfluenciaId = oParam.contacto.areaDeInfluencia != null && oParam.contacto.areaDeInfluencia != "null" ? Convert.ToInt32(oParam.contacto.areaDeInfluencia) : (int?)null;
                oProveedorSave.ClasificacionCompraNetId = clasificacion;
                oProveedorSave.BoletoCompraNetId = oParam.basicos.BoletoCompraNet;
                oProveedorSave.BolsaCompraNetId = oParam.basicos.BolsaCompraNet;
                oProveedorSave.LocalidadCompraNetId = oParam.basicos.LocalidadCompraNet;
                oProveedorSave.ProvinciaCompraNetId = oParam.basicos.ProvinciaCompraNet;

                var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == oParam.basicos.cuit);
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
                var oEstados = repositorio.Listar<Estado>();
                if (ConfigurationManager.AppSettings["SinConexionSap"].ToString() != "1")
                {
                    var listaDeCuit = repositorio.SelStore<Datos>("DataAgro_ActualizarComercialHome", 0, comercial.ComercialId).Where(x => x.CUIT == oParam.basicos.cuit).ToList();

                    if (listaDeCuit.Any() && ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                    {
                        foreach (var x in listaDeCuit)
                        {
                            x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"].ToString();
                        }
                    }

                    var list = new DatosProveedor(logger).ObtenerDatosDeProveedor(listaDeCuit);

                    if (list.Count > 0)
                    {
                        foreach (var lista in list)
                        {
                            var comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == lista.USUARIO.ToLower());

                            if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && idActiveDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                            {
                                string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                                comercialLista = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory.ToLower() == aux);
                            }

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

        public GrabarProveedorResult UpdateDatosContacto(NuevoProveedor oParam)
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

        public GrabarProveedorResult UpdateCanalOperacion(NuevoProveedor oParam)
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

        public GrabarProveedorResult UpdateDestinatario(NuevoProveedor oParam)
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

        public GrabarProveedorResult UpdateCondicion(NuevoProveedor oParam)
        {
            var resultado = new GrabarProveedorResult();
            try
            {
                var oCondicionSave = repositorio.Listar<ProveedorCondicion>(x => x.ProveedorId == oParam.ProveedorId);

                #region Eliminar

                foreach (var cond in oCondicionSave)
                {
                    if (!oParam.contacto.condPreferentes.Contains(cond.Condicion.CondicionId))
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

        public GrabarProveedorResult UpdateObjetivos(NuevoProveedor oParam)
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
                if (oParam.produccion.objetivos != null && oParam.produccion.objetivos.Any(x => x.campañaId == camp.Campaña.CampañaId && x.granoId == camp.Material.MaterialId))
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

        public GrabarProveedorResult UpdateContactoComerciales(NuevoProveedor oParam)
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
                                ProveedorId = (int)oParam.ProveedorId
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

                        #region Eliminar Intereses Contactos
                        var oContactosInteresesSave = repositorio.Listar<ContactoComercialInteres>(x => x.ContactoComercial.ContactoComercialId == mod.contactoComercialId);

                        foreach (var can in oContactosInteresesSave)
                        {
                            if (!mod.intereses.Contains(can.Interes.InteresId))
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

        public GrabarProveedorResult UpdateProduccion(NuevoProveedor oParam)
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

        public GrabarProveedorResult UpdateAlmacenamiento(NuevoProveedor oParam)
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
                                    if (!oCampoMaterialSave.Any(x => x.Campaña.CampañaId == gra.campañaId && x.MaterialId == gra.granoId))
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

        public Historial Comprar(int proveedorId, string UsuarioDirectory)
        {
            var historial = new Historial();
            
            try
            {
                List<ZMPES5130> hist = new List<ZMPES5130>();

                var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
                var listMateriales = repositorio.Listar<CampañaMaterial,Material>(x => x.Material,x => x.Proveedor.ProveedorId == proveedorId);
                foreach (var material in listMateriales)
                {
                    Comercial comercial = null;
                    if (ConfigurationManager.AppSettings["usuarioLaura"].ToString() == "1" && UsuarioDirectory.ToLower() == ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower())
                    {
                        string aux = ConfigurationManager.AppSettings["usuarioLaurastring"].ToString().ToLower();
                        comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == aux);
                    }
                    else
                    {
                        comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == UsuarioDirectory);
                    }
                    
                    var campaniaActual = material.Campaña.CampañaId;
                    var oCampañaMaterialAnteriorActualNueva = repositorio.Listar<CampañaMaterial>(x => x.Proveedor.ProveedorId == proveedorId && (x.Campaña.CampañaId == (campaniaActual - 1) || x.Campaña.CampañaId == campaniaActual || x.Campaña.CampañaId == (campaniaActual + 1)) && x.Material.MaterialId == material.MaterialId);

                    foreach (var cmaux in oCampañaMaterialAnteriorActualNueva)
                    {
                        var oCampañaMaterialPorMes = repositorio.SelStore<CampañaMaterialPorMes>("DataAgro_ComprasPorComercialId", 0, comercial.ComercialId, cmaux.CampañaMaterialId);

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

        public List<BusquedaHome> DevolverProveedores(string filtro)
        {
            return repositorio.SelStore<BusquedaHome>("DataAgro_BusquedaProveedores", 15, filtro);
        }

        public List<ProveedorDto> ListarProveedor(string proveedor)
        {
            return repositorio.Listar<Proveedor, ProveedorDto>(x => new ProveedorDto { CUIT = x.CUIT, ProveedorId = x.ProveedorId, RazonSocial = x.RazonSocial}, x => proveedor == "" || (x.RazonSocial.Contains(proveedor) || x.CUIT.Contains(proveedor)), 15);
        }

        public class ZMPES5130
        {
            public string VENDEDOR;

            public string MATERIAL;

            public string COSECHA;

            public string MES;

            public string ANIO;

            public decimal TN_COMPRADAS;
        }

        public List<BasicoProveedor> TraerDatosBasicosProveedor(int proveedorId, int comercialId, List<int> equipo)
        {
            return repositorio.ListarConsulta(new TraerDatosBasicosProveedor(proveedorId, comercialId, equipo));
        }
    }
}
