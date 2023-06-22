using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;

namespace Molinos.DataAgro.Business.Managers
{
    public class NegocioManager : INegocioManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        private readonly IClientePrimaryAPIAgent clientePrimaryAPI;
        private readonly IHttpContextManager httpContextManager;


        public NegocioManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IClientePrimaryAPIAgent clientePrimaryAPI,
            IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.clientePrimaryAPI = clientePrimaryAPI;
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
                var subject = "Negocios con fecha anterior";

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
            htmlBody += "En el presente mail se detallan los negocios creados con fecha anterior a la actual: <br /><br />  ";
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
            htmlBody += "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void MigrarContratosPrimary(DateTime fecha)
        {
            try
            {
                var negociosMAT = clientePrimaryAPI.ObtenerNegocios(fecha);
                var negociosDA = repositorio.Listar<AgenteCompra>(x =>
                    DbFunctions.TruncateTime(x.Fecha) == fecha && x.TipoNegocioId == 5 && (x.EstadoId == 2 || x.EstadoId == 5));
                var agrupadoMAT = negociosMAT.GroupBy(item => new { item.MaterialId, item.Posicion, item.OperadorId, item.MonedaId });
                var agrupadoDA = negociosDA.GroupBy(item => new { item.MaterialId, item.Posicion, item.OperadorId, item.MonedaId });
                var listaMAT = new List<AgenteCompra>();
                var listaDA = new List<AgenteCompra>();
                foreach (var mat in agrupadoMAT)
                {
                    var negocio = new AgenteCompra()
                    {
                        MaterialId = mat.Key.MaterialId,
                        Material = mat.First().Material,
                        MonedaId = mat.Key.MonedaId,
                        Posicion = mat.Key.Posicion,
                        Operador = mat.First().Operador,
                        OperadorId = mat.First().OperadorId,
                        PrecioPonderado = mat.Sum(x => x.Precio * (decimal)Math.Abs(x.Cantidad)) / mat.Sum(x => (decimal)Math.Abs(x.Cantidad))
                    };
                    listaMAT.Add(negocio);
                }
                foreach (var dataAgro in agrupadoDA)
                {
                    var negocio = new AgenteCompra()
                    {
                        MaterialId = dataAgro.Key.MaterialId,
                        Material = dataAgro.First().Material,
                        MonedaId = dataAgro.Key.MonedaId,
                        Posicion = dataAgro.Key.Posicion,
                        Operador = dataAgro.First().Operador,
                        OperadorId = dataAgro.First().OperadorId,
                        PrecioPonderado = dataAgro.Sum(x => x.Precio * (decimal)Math.Abs(x.Cantidad)) / dataAgro.Sum(x => (decimal)Math.Abs(x.Cantidad))
                    };
                    listaDA.Add(negocio);
                }
                List<string> errores = new List<string>();
                foreach (var item in listaMAT)
                {
                    var elem = listaDA.Where(x => x.MaterialId == item.MaterialId && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                    if (elem != null)
                    {
                        var cantidadMAT = negociosMAT.Where(x => x.MaterialId == item.MaterialId && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                        var cantidadDA = negociosDA.Where(x => x.MaterialId == item.MaterialId && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);

                        if (elem.PrecioPonderado != item.PrecioPonderado)
                        {
                            errores.Add("Diferencia de precios ponderados para negocios de " + elem.Material.Descripcion + " en " + elem.MonedaId +
                                " para la posición " + elem.Posicion + " y operador " + elem.Operador.Descripcion + " - en Data Agro: " + elem.PrecioPonderado?.ToString("N", new CultureInfo("es-AR")) + " y en MAT: " + item.PrecioPonderado?.ToString("N", new CultureInfo("es-AR")));
                        }

                        if (cantidadDA != cantidadMAT)
                        {
                            errores.Add("Diferencia en los kilos totales de negocios de " + elem.Material.Descripcion + " en " + elem.MonedaId +
                                " para la posición " + elem.Posicion + " y operador " + elem.Operador.Descripcion + " - en Data Agro: " + cantidadDA.ToString("N", new CultureInfo("es-AR")) + " Kg. y en MAT: " + cantidadMAT.ToString("N", new CultureInfo("es-AR")) + " Kg.");
                        }
                    }
                    else
                    {
                        errores.Add("No se encontraron en Data Agro negocios de " + item.Material.Descripcion + " en " + item.MonedaId + " para la posición " + item.Posicion + " y operador " + item.Operador.Descripcion + ", pero sí en el MAT.");
                    }
                }
                foreach (var item in listaDA)
                {
                    var elem = listaMAT.Where(x => x.MaterialId == item.MaterialId && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                    if (elem == null)
                    {
                        errores.Add("No se encontraron en el MAT negocios de " + item.Material.Descripcion + " en " + item.MonedaId + " para la posición " + item.Posicion + " y operador " + item.Operador.Descripcion + ", pero sí en Data Agro.");
                    }
                }
                foreach (var negocio in negociosDA)
                {
                    negocio.EstadoId = 8; //Eliminado
                }

                foreach (var item in negociosMAT)
                {
                    repositorio.Agregar(item);
                }

                if (errores.Count == 0)
                {
                    var asunto = "No hay diferencias entre Data Agro y posición MATBA - " + fecha.ToString("dd/MM/yyyy");
                    errores.Add("No se encontraron diferencias entre Data Agro y posición MATBA.");
                    EnviarMailMATPrimay(asunto, errores);
                }
                else
                {
                    var asunto = "Error - Diferencias entre Data Agro y posición MATBA - " + fecha.ToString("dd/MM/yyyy");
                    EnviarMailMATPrimay(asunto, errores);
                }
                repositorio.GuardarCambios();
            }
            catch
            {
                var asunto = "Error al obtener la posición MATBA - " + fecha.ToString("dd/MM/yyyy");
                List<string> errores = new List<string>
                {
                    "No se pudo procesar la sincronización con el MAT. Informar a sistemas."
                };
                EnviarMailMATPrimay(asunto, errores);
                throw;
            }

        }

        private void EnviarMailMATPrimay(string asunto, List<string> cuerpo)
        {
            LinkedResource resource = new LinkedResource(httpContextManager.ObtenerPathLogoMail())
            {
                ContentId = Guid.NewGuid().ToString()
            };
            var destinatarios = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailMATPrimary))).Select(x => x.IdActiveDirectory).ToList();
            string htmlBody = "En el presente mail se detalla el resultado de la comparación automática entre los negocios registrados en Data Agro y los obtenidos del MAT.<br /><br />";
            foreach (var mensaje in cuerpo)
            {
                htmlBody += mensaje + "<br /><br />";
            }
            htmlBody += "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + resource.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(resource);
            mailManager.EnviarMail(destinatarios, asunto, "", null, alternateView);
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

            var subject = "Negocios con Anula y Reemplaza";

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
                htmlBody += "En el presente mail se detallan los negocios Anula y Reemplaza creados  el " + DateTime.Now.ToString("dd/MM/yyyy") + " <br /><br />  ";
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

            htmlBody += "<br /> <br />  Saludos Cordiales," +
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

            var subject = "Error finalización de negocio Molinos Agro S.A. – " + negocio.ContratoSAP;

            mailManager.EnviarMail(ConfigurationManager.AppSettings["EmailDASoporte"].ToString().Split(';').ToList(), subject, "", lista, CuerpoMailContrato(httpContextManager.ObtenerPathLogoMail(), negocio));
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

            string htmlBody = "";

            htmlBody += "En el presente mail se detalla el negocio finalizado con error y contrato SAP asignado: " + negocio.ContratoSAP + " <br /><br />  ";
            htmlBody += "Por favor, revisarlo con prioridad ALTA. <br /> <br /> ";

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


            htmlBody += "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}