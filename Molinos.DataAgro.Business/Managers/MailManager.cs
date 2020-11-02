using Autofac.Extras.NLog;
using MailKit;
//using GemBox.Email.Imap;
//using GemBox.Email.Mime;
using MailKit.Search;
using MimeKit;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Extensions;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.DirectoryServices;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Molinos.DataAgro.Business
{

    public class MailManager : IMailManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public MailManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public void EnviarMail(Comercial desde, List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null)
        {
            try
            {
                var oMensaje = CrearMailBase(cuerpo, asunto, enviarA);
                if (copia != null)
                {
                    foreach (string mail in copia)
                    {
                        oMensaje.CC.Add(mail);
                    }
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);
                if (vistaAlternativa != null)
                {
                    oMensaje.AlternateViews.Add(vistaAlternativa);
                }

                oMensaje.BodyEncoding = Encoding.UTF8;
                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");
                if (archivo != null)
                {
                    Attachment data = new Attachment(new MemoryStream(archivo), nombreArchivo);
                    oMensaje.Attachments.Add(data);
                }
                EnviarMailClient(oMensaje);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public string GetEmailUserActiveDirectory(string UserName)
        {
            DirectoryEntry entry = new DirectoryEntry();
            string userName = UserName;
            try
            {
                var userNameArray = UserName.Split('\\');
                userName = userNameArray.Length == 1 ? userNameArray[0] : userNameArray[1];

            }
            catch { }

            DirectorySearcher search = new DirectorySearcher(entry);
            search.Filter = String.Format("(sAMAccountName={0})", userName);
            search.PropertiesToLoad.Add("givenName");   // first name
            search.PropertiesToLoad.Add("sn");          // last name
            search.PropertiesToLoad.Add("mail");        // smtp mail address

            // perform the search
            SearchResult result = search.FindOne();
            try
            {
                return result.Properties.Contains("mail") ? result.Properties["mail"][0].ToString() : result.Properties["userPrincipalName"][0].ToString();
            }
            catch
            {
                logger.Error($"No se encontró el mail en AD para el usuario {userName}");
            }
            return string.Empty;
        }

        private MailMessage CrearMailBase(string cuerpo, string asunto, List<string> enviarA)
        {
            MailMessage oMensaje = new MailMessage
            {
                From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]),
                Body = cuerpo,
                Subject = asunto
            };
            foreach (string mail in enviarA)
            {
                if (!string.IsNullOrEmpty(mail))
                {
                    oMensaje.To.Add(mail);
                }
            }
            if (enviarA == null || enviarA.Count() == 0)
            {
                oMensaje.To.Add(ConfigurationManager.AppSettings["CredentialUserName"]);
            }
            return oMensaje;
        }

        private void EnviarMailClient(MailMessage oMensaje)
        {
            try
            {
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
                logger.Error($"Fallo el SmtpClient con error:  {ex.Message}");
            }
        }
        public void EnviarMail(Comercial desde, List<Comercial> enviarA, string asunto, string cuerpo, List<Comercial> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null)
        {
            List<string> enviarAstring = new List<string>();
            if (enviarA != null)
            {
                foreach (Comercial comercial in enviarA)
                {
                    try
                    {
                        var mail = GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
                        enviarAstring.Add(mail);
                        logger.Info("Se envia el mail cierre del dia a {0}, con idad {1}", mail, comercial.IdActiveDirectory);
                    }
                    catch (Exception e) { logger.Error(e); }
                }
            }
            List<string> copiaAstring = new List<string>();
            if (copia != null)
            {
                foreach (Comercial comercial in copia)
                {
                    try { copiaAstring.Add(GetEmailUserActiveDirectory(comercial.IdActiveDirectory)); } catch (Exception e) { logger.Error(e); }
                }
            }
            this.EnviarMail(enviarAstring, asunto, cuerpo, copiaAstring, vistaAlternativa, archivo, nombreArchivo);
        }
        public void ReenviarMailCierreDia(string asuntoABuscar, string asuntoNuevoMail, string cuerpo)
        {
            try
            {
                MimeMessage originalMessage = new MimeMessage();
                UniqueId uid = new UniqueId();

                logger.Debug("ReenviarMailCierreDia Obtener mail IMAP");
                using (var client = new MailKit.Net.Imap.ImapClient())
                {
                    client.Connect(ConfigurationManager.AppSettings["ImapServer"], int.Parse(ConfigurationManager.AppSettings["ImapServerPort"]), true);

                    client.Authenticate(ConfigurationManager.AppSettings["CredentialUserName"], ConfigurationManager.AppSettings["CredentialPassword"]);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(MailKit.FolderAccess.ReadOnly);

                    Console.WriteLine("Total messages: {0}", inbox.Count);
                    Console.WriteLine("Recent messages: {0}", inbox.Recent);

                    try
                    {
                        var query = SearchQuery.SubjectContains(asuntoNuevoMail);

                        uid = inbox.Search(query).LastOrDefault();
                        if (uid.ToString() != "0")
                        {
                            originalMessage = inbox.GetMessage(uid);
                        }
                        else
                        {
                            query = SearchQuery.SubjectContains(asuntoABuscar);
                            uid = inbox.Search(query).Last();
                            originalMessage = inbox.GetMessage(uid);
                        }
                    }
                    catch (Exception e)
                    {

                    }
                    client.Disconnect(true);
                }

                var comercial = repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailHedge)));
                logger.Debug("ReenviarMailCierreDia comerciales " + comercial.ToJson());


                var lista = new List<string>();

                var email = "";
                foreach (var item in comercial)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        email = this.GetEmailUserActiveDirectory(item);
                        if (!String.IsNullOrEmpty(email))
                        {
                            lista.Add(email);
                        }
                    }
                }

                var replyMessage = new MimeMessage();
                replyMessage.From.Add(new MailboxAddress(ConfigurationManager.AppSettings["CredentialUserName"], ConfigurationManager.AppSettings["CredentialUserName"]));
                foreach (var item in lista)
                {
                    replyMessage.To.Add(new MailboxAddress(item, item));

                }

                replyMessage.Subject = asuntoNuevoMail;
                replyMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = cuerpo +
                        $"<div>{originalMessage.Date:G}, {originalMessage.From[0].Name} Escribió:</div>" +
                        $"<blockquote>{originalMessage.HtmlBody}</blockquote>"
                };
                var Text = cuerpo +
                        $"<div>{originalMessage.Date:G}, {originalMessage.From[0].Name} Escribió:</div>" +
                        $"<blockquote>{originalMessage.HtmlBody}</blockquote>";

                logger.Debug("ReenviarMailCierreDia SMTP send mail");
                // Send reply email.                 
                //using (var client = new MailKit.Net.Smtp.SmtpClient())
                //{
                //    client.Connect(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]), ConfigurationManager.AppSettings["EnableSSL"] == "S");

                //    // Note: only needed if the SMTP server requires authentication
                //    client.Authenticate(ConfigurationManager.AppSettings["CredentialUserName"], ConfigurationManager.AppSettings["CredentialPassword"]);

                //    client.Send(replyMessage);
                //    client.Disconnect(true);
                //}
                EnviarMail(
                    new List<string> { "emartin@baufest.com" },//replyMessage.To.Select(a => a.Name.ToString()).ToList(),
                    replyMessage.Subject,
                    "",
                    null,
                    AlternateView.CreateAlternateViewFromString(Text, null, MediaTypeNames.Text.Html),
                    null,null
                    );

                //MailMessage mensaje = MimeKitExtensions.ConvertToMailMessage(originalMessage);
                //EnviarMailClient(mensaje);

            }
            catch (Exception e)
            {
                logger.Error("Error reenvio de mail", e);
            }
        }

        public void EnviarMail(List<string> enviarA, string asunto, string cuerpo, List<string> copia = null, AlternateView vistaAlternativa = null, byte[] archivo = null, string nombreArchivo = null)
        {
            List<string> enviarAstring = new List<string>();
            List<string> copiaAstring = new List<string>();
            foreach (var enviar in enviarA)
            {
                if (enviar.Split('@').Length == 2)
                {
                    enviarAstring.Add(enviar);
                }
                else
                {
                    try
                    {
                        var mail = GetEmailUserActiveDirectory(enviar);
                        enviarAstring.Add(mail);
                        logger.Info("Se envia en copia el mail {0} dia a {1}, con idad {2}", asunto, mail, enviar);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                }
            }
            if (copia != null)
            {
                foreach (var cc in copia)
                {
                    if (cc.Split('@').Length == 2)
                    {
                        copiaAstring.Add(cc);
                    }

                    else
                    {
                        try
                        {
                            var mail = GetEmailUserActiveDirectory(cc);
                            copiaAstring.Add(mail);
                            logger.Info("Se envia en copia el mail {0} dia a {1}, con idad {2}", asunto, mail, cc);
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                        }
                    }
                }
            }
            this.EnviarMail(new Comercial(), enviarAstring, asunto, cuerpo, copiaAstring, vistaAlternativa, archivo, nombreArchivo);
        }
    }
}

