using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace Molinos.DataAgro.Business
{
    public class FAQManager : IFAQManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IComercialManager mobjComercial;
        private readonly IMailManager mailManager;
        private readonly IHttpContextManager httpContextManager;

        public FAQManager(ILogger logger, IRepositorio repositorio, IComercialManager mobjComercial, IMailManager mailManager, IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mobjComercial = mobjComercial;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
        }

        public List<ManualesDto> TraerManuales()
        {
            List<ManualesDto> manuales = repositorio.Listar<Manuales, ManualesDto>(x => new ManualesDto()
            {
                Id = x.Id,
                Titulo = x.Titulo,
                Descripcion = x.Descripcion,
                Path = x.Path,
                FechaUltimaActualizacion = x.FechaUltimaActualizacion,
                Version = x.Version,
                CantidadVisitas = x.CantidadVisitas
            });

            return manuales;
        }

        public ManualResult EnviarSugerencia(ManualesDto manual, string sugerencia, string IdActiveDirectory)
        {
            var result = new ManualResult();

            try
            {
                // CC al Comercial que realizó la sugerencia
                List<string> emailComercialSugerencia = new List<string>();
                var emailComercial = mailManager.GetEmailUserActiveDirectory(IdActiveDirectory.ToUpper());
                if (!String.IsNullOrEmpty(emailComercial))
                {
                    emailComercialSugerencia.Add(emailComercial);
                }

                var subject = "Nueva sugerencia de Manuales de Originación";

                Comercial comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == IdActiveDirectory);

                var lista = new List<string>();
                lista.AddRange(BuscarMailReceptoresSugerenciasFAQ(comercial));

                // Cuerpo del mail
                LinkedResource resource = new LinkedResource(httpContextManager.ObtenerPathLogoMail())
                {
                    ContentId = Guid.NewGuid().ToString()
                };
                string htmlBody = "";
                htmlBody += "<b>Manual:</b> " + manual.Titulo;
                htmlBody += "<br>";
                htmlBody += "<b>Sugerencia realizada por " + comercial.Nombres + ' ' + comercial.Apellido + ":</b> <br>";
                htmlBody += sugerencia;
                htmlBody += "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + resource.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
                AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                alternateView.LinkedResources.Add(resource);

                mailManager.EnviarMail(new Comercial(), lista, subject, "", emailComercialSugerencia, alternateView);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return result;
        }

        private List<string> BuscarMailReceptoresSugerenciasFAQ(Comercial comercial)
        {
            var lista = new List<string>();
            var recibirSugerenciaFAQ = this.mobjComercial.ListarComercialesRecibirSugerenciaFAQ();
            if (recibirSugerenciaFAQ == null) return lista;
            
            recibirSugerenciaFAQ.Remove(comercial);

            logger.Debug("Enviando mail Sugerencia FAQ a " + string.Join(", ", recibirSugerenciaFAQ));

            foreach (Comercial recibirSugerenciaFAQCopia in recibirSugerenciaFAQ)
            {
                try
                {
                    var emailComerciales = mailManager.GetEmailUserActiveDirectory(recibirSugerenciaFAQCopia.IdActiveDirectory);
                    if (!String.IsNullOrEmpty(emailComerciales))
                    {
                        lista.Add(emailComerciales);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }

            return lista;
        }

        public ManualResult RegistrarVisita(int idManual, string IdActiveDirectory)
        {
            var result = new ManualResult();

            try
            {
                Manuales manual = repositorio.Obtener<Manuales>(x => x.Id == idManual);
                manual.CantidadVisitas += 1;

                Comercial comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == IdActiveDirectory);

                Entities.Entities.ManualesLogVisitas manualesLogVisitas = new Entities.Entities.ManualesLogVisitas()
                {
                    ManualId = idManual,
                    ComercialId = comercial.ComercialId,
                    FechaVisita = DateTime.Now,
                };

                repositorio.Agregar(manualesLogVisitas);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }

            return result;
        }

        public void ActualizarFechaUltimaActualizacionManualesFAQ()
        {
            try
            {
                List<Manuales> manuales = repositorio.Listar<Manuales>();
                string format = "ddd, dd MMM yyyy HH:mm:ss 'GMT'";

                manuales.ForEach(x =>
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                    // Send a request to the server
                    WebRequest request = WebRequest.Create(x.Path);
                    WebResponse response = request.GetResponse();
                    // Retrieve the creation date of the file
                    string lastModified = response.Headers.Get("Last-Modified");

                    // Intenta convertir la cadena en un objeto DateTime
                    if (DateTime.TryParseExact(lastModified, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTime result))
                    {
                        if (x.FechaUltimaActualizacion < result)
                        {
                            x.FechaUltimaActualizacion = result;
                            x.Version += 1;
                        }
                    }
                });

                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
            }
        }
    }
}
