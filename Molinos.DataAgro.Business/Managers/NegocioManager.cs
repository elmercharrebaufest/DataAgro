using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
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
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace Molinos.DataAgro.Business.Managers
{
    public class NegocioManager : INegocioManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        private readonly IClientePrimariAPIAgent clientePrimariAPI;
        private readonly IHttpContextManager httpContextManager;


        public NegocioManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IClientePrimariAPIAgent clientePrimariAPI,
            IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.clientePrimariAPI = clientePrimariAPI;
            this.httpContextManager = httpContextManager;
        }

        public Resultado OcultarEnTablero(Negocio negocio)
        {
            var error = new Resultado();
            try
            {
                var negocioAMarcar = repositorio.Obtener<Negocio>(x => x.Id == negocio.Id);
                negocioAMarcar.OcultarEnTablero = negocio.OcultarEnTablero;
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                error.Error("", e.Message);
            }
            return error;
        }
        public void EnvioMailNegociosConDiaAnterior()
        {
            DateTime hoy = DateTime.Now.Date;
            var contratos = repositorio.Listar<Negocio>(x => DbFunctions.TruncateTime(x.Fecha) != DbFunctions.TruncateTime(x.FechaOperacion) && x.Fecha >= hoy && x.Canje != true && x.PrestamoDevolucion != true && x.Venta != true && x.TipoNegocioId != 5 && x.EstadoId == 5);
            var lista = new List<string>();
            var listaJefes = new List<string>();
            if (contratos.Count > 0)
            {
                foreach (var p in contratos)
                {
                    var comercial = repositorio.Obtener<Comercial>(p.ComercialId);
                    var email = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
                    lista.Add(email);

                }
                var idJefes = repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory,
                    a => a.RolesAsociados.Any(b => b.PermisosAsociados.Any(c => c.Permiso == PermisosDataAgro.JefeEnvioMailNegociosConDiaAnterior)));

                foreach (var item in idJefes)
                {
                    var email = mailManager.GetEmailUserActiveDirectory(item);
                    listaJefes.Add(email);
                }
                lista = lista.Distinct().ToList();
                if (listaJefes.Count == 0)
                {
                    listaJefes = null;
                }
                var subject = "";
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    subject = "Negocios con fecha anterior";
                }
                else
                {
                    subject = "Prueba Mail - Negocios con fecha anterior";
                }
                var rutaMolinos = httpContextManager.ObtenerPathLogoMail();

                var cuerpoMail = CuerpoMailNegociosConDiaAnterior(rutaMolinos, contratos);
                mailManager.EnviarMail(lista, subject, "", listaJefes, cuerpoMail);
            }


        }
        private AlternateView CuerpoMailNegociosConDiaAnterior(string filePath, List<Negocio> contratos)
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
            htmlBody += "En el presente mail, se detalla los Negocios creados con fecha anterior a la actual: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" +
                    th + "Contrato" + "</td>" +
                    th + "Material" + "</td>" +
                    th + "Precio Base" + "</td>" +
                    th + "Moneda" + "</td>" +
                    th + "Cantidad (Kg)" + "</td>" +
                    th + "Fecha de Carga" + "</td>" +
                    th + "Fecha de Operacion" + "</td>" +
                    th + "Comercial" + "</td>" +
                    th + "Corredor" + "</td>" +
                    th + "Proveedor" + "</td>" +
                    th + "Tipo" + "</td>" +
                    //th + "Estado:" + "</td>" +
                    th + "Anula y reemplaza" + "</td>" +
                    th + "Motivo" + "</td>" +
                    "</tr>";

            foreach (var c in contratos)
            {
                string style = "";
                string style1 = "";
                string style2 = "";
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                    style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                }
                else
                {
                    style1 = "style =\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
                    style2 = "style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                }
                logger.Debug($"contrato numero: {c.Id}");
                linea += 1;
                if (linea % 2 == 0)
                {
                    style = style1;
                }
                else
                {
                    style = style2;
                }
                htmlBody += "<tr>" +
                      "<td " + style + (c.TipoNegocioId != 3 ? c.ContratoSAP : c is FijacionDePrecioContrato ? (c as FijacionDePrecioContrato).FijacionSAP : "") + "</td>" +
                      "<td " + style + c.Material.Descripcion + "</td>" +
                      "<td " + style + (c.Precio == 0 ? "" : c.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td>" +
                      "<td " + style + (c.MonedaId == null ? "" : c.Moneda.Descripcion) + "</td>" +
                      "<td " + style + c.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "</td>" +
                      "<td " + style + c.Fecha.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                      "<td " + style + c.FechaOperacion.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                      "<td " + style + c.Comercial.Nombres + " " + c.Comercial.Apellido + "</td>" +
                      (c.Corredor != null ? "<td " + style + c.Corredor.RazonSocial + "</td>" : "<td " + style + "</td>") +
                      "<td " + style + (c.ProveedorId == null ? "" : c.Proveedor.RazonSocial) + "</td>" +
                      "<td " + style + (c.Virtual == true ? "FIJACION VIRTUAL" : c.TipoNegocio.Descripcion) + "</td>" +
                      "<td " + style + (c is Contrato && (c as Contrato).AnulaYReemplazaContrato != null ? (c as Contrato).AnulaYReemplazaContrato.ContratoSAP : "") + "</td>" +
                      "<td " + style + (c is Contrato && (c as Contrato).AnulaYReemplazaContrato != null ? (c as Contrato).MotivoReemplazo : (!String.IsNullOrEmpty(c.MotivoOperacionAnterior) ? c.MotivoOperacionAnterior + " - " + c.DescripcionOperacionAnterior : "")) + "</td> </tr> ";
            }

            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        
        public void ConsultarContratosPrimary()
        {
            clientePrimariAPI.ObtenerNegocios();
        }


        public void EnvioMailNegociosAnulaYReemplaza()
        {
            DateTime hoy = DateTime.Now.Date;
            var contratos = repositorio.Listar<Contrato>(x =>
                DbFunctions.TruncateTime(x.Fecha) == hoy &&
                x.AnulaYReemplazaContratoId != null && x.EstadoId == 5
            );
            var listaJefes = new List<string>();

            var idJefes = repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory,
                a => a.RolesAsociados.Any(b => b.PermisosAsociados.Any(c => c.Permiso == PermisosDataAgro.EnvioMailNegociosAnulaYReemplaza)));

            foreach (var item in idJefes)
            {
                var email = mailManager.GetEmailUserActiveDirectory(item);
                listaJefes.Add(email);
            }

            var subject = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                subject = "Negocios con Anula y Reemplaza";
            }
            else
            {
                subject = "Prueba Mail - Negocios con Anula y Reemplaza";
            }
            var rutaMolinos = httpContextManager.ObtenerPathLogoMail();

            var cuerpoMail = CuerpoMailNegociosAnulaYReemplaza(rutaMolinos, contratos);
            mailManager.EnviarMail(listaJefes, subject, "", null, cuerpoMail);
        }

        private AlternateView CuerpoMailNegociosAnulaYReemplaza(string filePath, List<Contrato> contratos)
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
            if (contratos.Count() == 0)
            {
                htmlBody += "El dia " + DateTime.Now.ToString("dd/MM/yyyy") + " no se realizaron negocios Anula y Reemplaza. <br /><br />  ";
            }
            else
            {
                htmlBody += "En el presente mail, se detalla los negocios Anula y Reemplaza creados  el " + DateTime.Now.ToString("dd/MM/yyyy") + " <br /><br />  ";
                htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
                htmlBody += "<tr>" +
                        th + "Contrato" + "</td>" +
                        th + "Material" + "</td>" +
                        th + "Precio Base" + "</td>" +
                        th + "Moneda" + "</td>" +
                        th + "Cantidad (Kg)" + "</td>" +
                        th + "Fecha de Carga" + "</td>" +
                        th + "Fecha de Operacion" + "</td>" +
                        th + "Comercial" + "</td>" +
                        th + "Corredor" + "</td>" +
                        th + "Proveedor" + "</td>" +
                        th + "Tipo" + "</td>" +
                        th + "Anula y reemplaza" + "</td>" +
                        th + "Motivo" + "</td>" +
                        "</tr>";

                foreach (var c in contratos)
                {
                    string style = "";
                    string style1 = "";
                    string style2 = "";
                    if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    else
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                    }
                    logger.Debug($"contrato numero: {c.Id}");
                    linea += 1;
                    if (linea % 2 == 0)
                    {
                        style = style1;
                    }
                    else
                    {
                        style = style2;
                    }
                    htmlBody += "<tr>" +
                          "<td " + style + c.ContratoSAP + "</td>" +
                          "<td " + style + c.Material.Descripcion + "</td>" +
                          "<td " + style + (c.Precio == 0 ? "" : c.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td>" +
                          "<td " + style + (c.MonedaId == null ? "" : c.Moneda.Descripcion) + "</td>" +
                          "<td " + style + c.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "</td>" +
                          "<td " + style + c.Fecha.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                          "<td " + style + c.FechaOperacion.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                          "<td " + style + c.Comercial.Nombres + " " + c.Comercial.Apellido + "</td>" +
                          (c.Corredor != null ? "<td " + style + c.Corredor.RazonSocial + "</td>" : "<td " + style + "</td>") +
                          "<td " + style + (c.ProveedorId == null ? "" : c.Proveedor.RazonSocial) + "</td>" +
                          "<td " + style + c.TipoNegocio.Descripcion + "</td>" +
                          "<td " + style + c.AnulaYReemplazaContrato.ContratoSAP + "</td>" +
                          "<td " + style + c.MotivoReemplazo + "</td> </tr> ";
                }

                htmlBody += " </td></tr>";
                htmlBody += "</td></tr></table>";
            }

            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }



        public void EnviarMailErrorFinalizarNegocio(int negocioId)
        {
            var lista = new List<string>();
            var email = "";
            var negocio = repositorio.Obtener<Negocio>(negocioId);

            email = mailManager.GetEmailUserActiveDirectory(negocio.Comercial.IdActiveDirectory);
            lista.Add(email);
            if (negocio.ComercialCreadorId != negocio.ComercialId)
            {
                email = mailManager.GetEmailUserActiveDirectory(negocio.ComercialCreador.IdActiveDirectory);
                lista.Add(email);
            }

            var subject = "Error Finalizacion negocio Molinos Agro S.A. – " + negocio.ContratoSAP;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
            {
                subject = "Mail de Pruebas - " + subject;
            }
            mailManager.EnviarMail( ConfigurationManager.AppSettings["EmailDASoporte"].ToString().Split(';').ToList(), subject, "", lista, CuerpoMailContrato(httpContextManager.ObtenerPathLogoMail(), negocio));
        }

        private AlternateView CuerpoMailContrato(string filePath, Negocio negocio)
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

            htmlBody += "En el presente mail se detalla el negocio finalizado con error y contrato sap asignado: " + negocio.ContratoSAP + " <br /><br />  ";
            htmlBody += "Por favor revisarlo con prioridad ALTA. <br /> <br /> ";

            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" +
                    th + "Contrato" + "</td>" +
                    th + "Material" + "</td>" +
                    th + "Precio Base" + "</td>" +
                    th + "Moneda" + "</td>" +
                    th + "Cantidad (Kg)" + "</td>" +
                    th + "Fecha de Carga" + "</td>" +
                    th + "Fecha de Operacion" + "</td>" +
                    th + "Comercial" + "</td>" +
                    th + "Corredor" + "</td>" +
                    th + "Proveedor" + "</td>" +
                    th + "Tipo" + "</td>" +
                    "</tr>";
            string style = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                style = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                style = "style =\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
            }

            htmlBody += "<tr>" +
                  "<td " + style + negocio.ContratoSAP + "</td>" +
                  "<td " + style + negocio.Material.Descripcion + "</td>" +
                  "<td " + style + (negocio.Precio == 0 ? "" : negocio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td>" +
                  "<td " + style + (negocio.MonedaId == null ? "" : negocio.Moneda.Descripcion) + "</td>" +
                  "<td " + style + negocio.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "</td>" +
                  "<td " + style + negocio.Fecha.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                  "<td " + style + negocio.FechaOperacion.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                  "<td " + style + negocio.Comercial.Nombres + " " + negocio.Comercial.Apellido + "</td>" +
                  (negocio.Corredor != null ? "<td " + style + negocio.Corredor.RazonSocial + "</td>" : "<td " + style + "</td>") +
                  "<td " + style + (negocio.ProveedorId == null ? "" : negocio.Proveedor.RazonSocial) + "</td>" +
                  "<td " + style + negocio.TipoNegocio.Descripcion + "</td>";

            htmlBody += "</tr>";
            htmlBody += "</td></tr></table>";


            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}
