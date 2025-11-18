using NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
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
        private readonly IAltaTempranaAgent altaTempranaAgent;
        private readonly ICartasDePortePendienteAplicarAgent ccppPendientesAgent;

        public NegocioManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IClientePrimaryAPIAgent clientePrimaryAPI, IHttpContextManager httpContextManager,
            IAltaTempranaAgent altaTempranaAgent, ICartasDePortePendienteAplicarAgent ccppPendientesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.clientePrimaryAPI = clientePrimaryAPI;
            this.httpContextManager = httpContextManager;
            this.altaTempranaAgent = altaTempranaAgent;
            this.ccppPendientesAgent = ccppPendientesAgent;
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
            var contratos = repositorio.Listar<Negocio>(x => DbFunctions.TruncateTime(x.Fecha) != DbFunctions.TruncateTime(x.FechaOperacion) &&
                                                             x.Fecha >= hoy &&
                                                             x.Canje != true &&
                                                             x.PrestamoDevolucion != true &&
                                                             x.Venta != true &&
                                                             x.TipoNegocioId != (int)EnumTipoNegocio.AGENTE_DE_COMPRAS &&
                                                             x.EstadoId == (int)EnumEstadoContrato.Finalizado);
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
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };
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

                var negociosConAgenteId = repositorio.Listar<Negocio, int>(x => x.Id,
                                                                           x => x.FechaOperacion == fecha &&
                                                                                x.TipoNegocioId == (int)EnumTipoNegocio.AGENTE_DE_COMPRAS &&
                                                                                (x.EstadoId == (int)EnumEstadoContrato.Confirmado ||
                                                                                x.EstadoId == (int)EnumEstadoContrato.Finalizado)).ToList();

                var negociosDA = repositorio.Listar<AgenteCompra>(x => negociosConAgenteId.Contains(x.Id));
                var negociosDAJson = negociosDA.Select(a => new { a.Id, a.ComercialId, a.Fecha, a.Cantidad, a.Precio, a.MaterialId, a.MonedaId, a.DolarExportador, a.OperadorId, a.EstadoId }).ToJson();
                logger.Debug("Lista negocios AgenteCompra DataAgro: " + negociosDAJson);

                var agrupadoMAT = negociosMAT.GroupBy(item => new { item.MaterialId, item.Posicion, item.OperadorId, item.MonedaId, item.DolarExportador });
                var agrupadoDA = negociosDA.GroupBy(item => new { item.MaterialId, item.Posicion, item.OperadorId, item.MonedaId, item.DolarExportador });
                var listaMAT = new List<AgenteCompra>();
                var listaDA = new List<AgenteCompra>();
                List<string> errores = new List<string>();

                foreach (var mat in agrupadoMAT)
                {
                    var negocio = new AgenteCompra()
                    {
                        MaterialId = mat.Key.MaterialId,
                        Material = mat.First().Material,
                        DolarExportador = mat.Key.DolarExportador,
                        MonedaId = mat.Key.MonedaId,
                        Posicion = mat.Key.Posicion,
                        Operador = mat.First().Operador,
                        OperadorId = mat.Key.OperadorId,
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
                        DolarExportador = dataAgro.Key.DolarExportador == true,
                        MonedaId = dataAgro.Key.MonedaId,
                        Posicion = dataAgro.Key.Posicion,
                        Operador = dataAgro.First().Operador,
                        OperadorId = dataAgro.Key.OperadorId,
                        PrecioPonderado = dataAgro.Sum(x => x.Precio * (decimal)Math.Abs(x.Cantidad)) / dataAgro.Sum(x => (decimal)Math.Abs(x.Cantidad))
                    };
                    listaDA.Add(negocio);
                }

                foreach (var item in listaMAT)
                {
                    double cantidadMAT = negociosMAT.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador != true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                    double cantidadDA = negociosDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador != true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);

                    if (item.DolarExportador.Value)
                    {
                        cantidadMAT = negociosMAT.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador == true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                        cantidadDA = negociosDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador == true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                        var elem = listaDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador == true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                        if (elem != null)
                        {
                            var elemPrecioRound = elem.PrecioPonderado?.ToString("N", new CultureInfo("es-AR"));
                            var itemPrecioRound = item.PrecioPonderado?.ToString("N", new CultureInfo("es-AR"));
                            if (elemPrecioRound != itemPrecioRound)
                            {
                                errores.Add($"• Diferencia de precios ponderados para negocios de {elem.Material.Descripcion} en {elem.MonedaId} EXPORTADOR para la posición {elem.Posicion} y operador {elem.Operador.Descripcion} - en Data Agro: {elemPrecioRound} y en MAT: {itemPrecioRound}");
                            }

                            if (cantidadDA != cantidadMAT)
                            {
                                errores.Add($"• Diferencia en los kilos totales de negocios de {elem.Material.Descripcion} en {elem.MonedaId} EXPORTADOR para la posición {elem.Posicion} y operador {elem.Operador.Descripcion} - en Data Agro: {cantidadDA.ToString("N", new CultureInfo("es-AR"))} Kg. y en MAT: {cantidadMAT.ToString("N", new CultureInfo("es-AR"))} Kg.");
                            }
                        }
                        else
                        {
                            errores.Add($"• No se encontraron en Data Agro negocios de {item.Material.Descripcion} en {item.MonedaId} EXPORTADOR para la posición {item.Posicion} y operador {item.Operador.Descripcion}, pero figuran {cantidadMAT.ToString("N", new CultureInfo("es-AR"))} Kg. en el MAT.");
                        }
                    }
                    else
                    {
                        var elem = listaDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador != true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                        if (elem != null)
                        {
                            var elemPrecioRound = elem.PrecioPonderado?.ToString("N", new CultureInfo("es-AR"));
                            var itemPrecioRound = item.PrecioPonderado?.ToString("N", new CultureInfo("es-AR"));
                            if (elemPrecioRound != itemPrecioRound)
                            {
                                errores.Add($"• Diferencia de precios ponderados para negocios de {elem.Material.Descripcion} en {elem.MonedaId} para la posición {elem.Posicion} y operador {elem.Operador.Descripcion} - en Data Agro: {elemPrecioRound} y en MAT: {itemPrecioRound}");
                            }

                            if (cantidadDA != cantidadMAT)
                            {
                                errores.Add($"• Diferencia en los kilos totales de negocios de {elem.Material.Descripcion} en {elem.MonedaId} para la posición {elem.Posicion} y operador {elem.Operador.Descripcion} - en Data Agro: {cantidadDA.ToString("N", new CultureInfo("es-AR"))} Kg. y en MAT: {cantidadMAT.ToString("N", new CultureInfo("es-AR"))} Kg.");
                            }
                        }
                        else
                        {
                            errores.Add($"• No se encontraron en Data Agro negocios de {item.Material.Descripcion} en {item.MonedaId} para la posición {item.Posicion} y operador {item.Operador.Descripcion}, pero figuran {cantidadMAT.ToString("N", new CultureInfo("es-AR"))} Kg. en el MAT.");
                        }
                    }
                }
                foreach (var item in listaDA)
                {
                    if (item.DolarExportador.Value)
                    {
                        var cantidadDA = negociosDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador == true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                        var elem = listaMAT.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador == true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                        if (elem == null)
                        {
                            errores.Add($"• No se encontraron en el MAT negocios de {item.Material.Descripcion} en {item.MonedaId} EXPORTADOR para la posición {item.Posicion} y operador {item.Operador.Descripcion}, pero figuran {cantidadDA.ToString("N", new CultureInfo("es-AR"))} Kg. en Data Agro.");
                        }
                    }
                    else
                    {
                        var cantidadDA = negociosDA.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador != true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.Operador.Id == item.Operador.Id).Sum(x => x.Cantidad);
                        var elem = listaMAT.Where(x => x.MaterialId == item.MaterialId && x.DolarExportador != true && x.MonedaId == item.MonedaId && x.Posicion == item.Posicion && x.OperadorId == item.OperadorId).SingleOrDefault();
                        if (elem == null)
                        {
                            errores.Add($"• No se encontraron en el MAT negocios de {item.Material.Descripcion} en {item.MonedaId} para la posición {item.Posicion} y operador {item.Operador.Descripcion}, pero figuran {cantidadDA.ToString("N", new CultureInfo("es-AR"))} Kg. en Data Agro.");
                        }
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

                repositorio.GuardarCambios();
                if (errores.Count == 0)
                {
                    var asunto = "No hay diferencias entre Data Agro y posición MATBA - " + fecha.ToString("dd/MM/yyyy");
                    errores.Add("• No se encontraron diferencias entre Data Agro y posición MATBA.");
                    EnviarMailMATPrimay(asunto, errores);
                }
                else
                {
                    var asunto = "Error - Diferencias entre Data Agro y posición MATBA - " + fecha.ToString("dd/MM/yyyy");
                    EnviarMailMATPrimay(asunto, errores);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error MigrarContratosPrimary: ", e.Message);
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
            List<string> copia = new List<string> { ConfigurationManager.AppSettings["EmailSoporte"] };
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
            mailManager.EnviarMail(destinatarios, asunto, "", copia, alternateView);
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
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };
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
            var negocio = repositorio.Obtener<Negocio>(negocioId);

            string email = mailManager.GetEmailUserActiveDirectory(negocio.Comercial.IdActiveDirectory);
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
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };
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
            string style;
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

        public BasicoContrato TraerAcuerdo(int id)
        {
            var contrato = repositorio.Obtener<ContratoAcuerdo, BasicoContrato>(x => x.Id == id, x => new BasicoContrato
            {
                Id = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                CorredorId = x.CorredorId ?? 0,
                Proveedor = (x.ProveedorId != null && x.ProveedorId > 0) ? x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")" : "",
                Corredor = (x.CorredorId != null && x.CorredorId > 0) ? x.Corredor.RazonSocial + " (" + x.Corredor.CUIT + ")" : "",
                ComercialId = x.ComercialId,
                Cantidad = x.Cantidad,
                TipoNegocioId = 6,
                MaterialId = x.MaterialId,
                CampanaId = x.CampanaId ?? 0,
                Campania = x.CampanaId == null ? "" : x.Campana.Descripcion,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion,
                ProvinciaId = x.ProvinciaId,
                Provincia = x.Provincia.Nombre,
                LocalidadId = x.LocalidadId,
                Localidad = x.Localidad.Nombre,
                ContratoSAP = x.ContratoSAP,
                Base = x.Base,
                Observacion = x.Observacion,
                Estado = x.EstadoId,
                Estado_Contrato = x.Estado.Descripcion,
                Sustentable = x.Sustentable,
                EPA = x.EPA,
                EUDR = x.EUDR,
                SustentableTipoDBId = x.SustentableTipoDBId,
                SustentableTipoDB = x.SustentableTipoDB != null ? x.SustentableTipoDB.Descripcion : "",
                Importe_Sustentable = x.ImporteSustentable,
                Moneda_Sustentable = x.MonedaSustentableId,
                FechaFormateado = DbFunctions.Right("0" + x.Fecha.Day, 2) + "-" + DbFunctions.Right("0" + x.Fecha.Month, 2) + "-" + x.Fecha.Year,
                FechaDesdeFormateado = DbFunctions.Right("0" + x.FechaDesde.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDesde.Month, 2) + "-" + x.FechaDesde.Year,
                FechaHastaFormateado = DbFunctions.Right("0" + x.FechaHasta.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaHasta.Month, 2) + "-" + x.FechaHasta.Year,
                DesdeFijacionFormateado = x.DesdeFijacion.HasValue ? DbFunctions.Right("0" + x.DesdeFijacion.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.DesdeFijacion.Value.Month, 2) + "-" + x.DesdeFijacion.Value.Year : "",
                HastaFijacionFormateado = x.HastaFijacion.HasValue ? DbFunctions.Right("0" + x.HastaFijacion.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.HastaFijacion.Value.Month, 2) + "-" + x.HastaFijacion.Value.Year : "",
                Fecha_DolarizadoFormateado = x.FechaDolarizado.HasValue ? DbFunctions.Right("0" + x.FechaDolarizado.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDolarizado.Value.Month, 2) + "-" + x.FechaDolarizado.Value.Year : "",
                FechaDolarizadoOriginalFormateado = x.FechaDolarizadoOriginal.HasValue ? DbFunctions.Right("0" + x.FechaDolarizadoOriginal.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDolarizadoOriginal.Value.Month, 2) + "-" + x.FechaDolarizadoOriginal.Value.Year : "",
                FechaCierta = x.FechaCierta,
                FechaCiertaFormateado = x.FechaCierta.HasValue ? DbFunctions.Right("0" + x.FechaCierta.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaCierta.Value.Month, 2) + "-" + x.FechaCierta.Value.Year : "",
                FechaDesde_Sustentable = x.FechaDesdeSustentable,
                FechaDesde_SustentableFormateado = x.FechaDesdeSustentable.HasValue ? DbFunctions.Right("0" + x.FechaDesdeSustentable.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDesdeSustentable.Value.Month, 2) + "-" + x.FechaDesdeSustentable.Value.Year : "",
                FechaHasta_Sustentable = x.FechaHastaSustentable,
                FechaHasta_SustentableFormateado = x.FechaHastaSustentable.HasValue ? DbFunctions.Right("0" + x.FechaHastaSustentable.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaHastaSustentable.Value.Month, 2) + "-" + x.FechaHastaSustentable.Value.Year : "",
                FechaOperacion = x.FechaOperacion,
                FechaOperacionFormateado = DbFunctions.Right("0" + x.FechaOperacion.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaOperacion.Month, 2) + "-" + x.FechaOperacion.Year,
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                CondicionFijacion = x.CondicionFijacionId,
                CondicionFijacionDescripcion = x.CondicionFijacion.Descripcion,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                Dolarizado = x.Dolarizado,
                DolarizadoCorredor = x.DolarizadoCorredor,
                Fecha_Dolarizado = x.FechaDolarizado,
                Dias_Pesificado = x.DiasPesificado,
                NoInformaSIO = x.NoInformaSio,
                TrigoEspecial = x.TrigoEspecial,
                ClasificacionId = x.ClasificacionId,
                DestinoId = x.DestinoId,
                PlanCanje = x.PlanCanje,
                Consignatario = x.Consignatario,
                CantidadCamiones = x.CantidadCamiones,
                BoletoId = x.BoletoId,
                BolsaId = x.BolsaId,
                CD = x.CD,
                Warrant = x.Warrant,
                PagoDirectoVendedor = x.PagoDirectoVendedor,
                EstablecimientoPropio = x.EstablecimientoPropio,
                MercsDeposito = x.MercsDeposito,
                CantidadDeposito = x.CantidadDeposito,
                PorcentajeComision = x.PorcentajeComision,
                ContratoCorredor = x.ContratoCorredor,
                ContratoVendedor = x.ContratoVendedor,
                SelCargoMOA = x.SelCargoMOA,
                SelCargoVendedor = x.SelCargoVendedor,
                Madre = x.Madre,
                EsFason = x.EsFason,
                ContratoMadre = x.ContratoMadre,
                Pizarra = x.Pizarra ?? false,
                StandardDeCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                PagoDiferido = x.PagoDiferido,
                PagoDiferidoTerceroId = x.PagoDiferidoTerceroId,
                ZonaId = x.ZonaId,
                ZonaDescripcion = x.Zona.Descripcion,
                Compensacion = x.Compensacion,
                NivelTarifaId = x.NivelTarifaId,
                TarifaFlete = x.TarifaFlete,
                ObligatoriedadCostoFinanciero = x.ObligatoriedadCostoFinanciero,
                ObligatoriedadBonificacion = x.ObligatoriedadBonificacion,
                Material = x.Material.Descripcion,
                Fecha = x.Fecha,
                DestinoDescripcion = x.Destino.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
                ChequeElectronico = x.ChequeElectronico,
                PagoCBU = x.PagoCBU,
                PosicionCBOT = x.PosicionCBOT,
                TipoPosicionCBOTId = x.TipoPosicionCBOTId,
                TipoPosicionCBOT = x.TipoPosicionCBOT.Descripcion,
                ProveedorCreador = x.ProveedorCreadorId,
                UsuarioId = x.UsuarioId,
                UsuarioTercero = x.UsuarioTercero,
                DolarizadoExpress = x.DolarizadoExpress,
                CalidadTercero = x.CalidadTercero,
                DolarizadoTercero = x.DolarizadoTercero,
                PagoDiferidoTercero = x.PagoDiferidoTercero,
                ObservacionTercero = x.ObservacionTercero,
                Canje = x.Canje,
                Monto = x.Monto,
                MonedaCanjeId = x.MonedaCanjeId,
                Insumo = x.Insumo,
                PrestamoDevolucion = x.PrestamoDevolucion ?? false,
                PlantaDestinoId = x.PlantaDestinoId ?? 0,
                PlantaDestinoDescripcion = !x.PlantaDestinoId.HasValue ? "" : x.Destino.Descripcion,
                SustentableTercero = x.SustentableTercero,
                Venta = x.Venta,
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                TipoAgenteCompra = x.TipoAgenteCompraId == null ? "" : x.TipoAgenteCompra.Descripcion,
                CaratulaMAT = x.CaratulaMAT,
                PrecioAjusteComision = x.PrecioAjusteComision,
                MonedaAjusteComisionId = x.MonedaAjusteComisionId,
                ProveedorComisionistaId = x.ProveedorComisionistaId,
                KgMinimo = x.KgMinimo ?? 0,
                KgMaximo = x.KgMaximo ?? 0,
                ProcedenciaVentaId = x.ProcedenciaVentaId,
                ProvinciaVentaId = x.ProcedenciaVenta.ProvinciaId,
                ProvinciaVenta = x.ProcedenciaVenta.Provincia.Nombre,
                LocalidadVenta = x.ProcedenciaVenta.Nombre,
                CamaraId = x.CamaraId,
                ComisionAFavorId = x.ComisionAFavorId,
                PorcentajeComisionVenta = x.PorcentajeComisionVenta,
                FleteACargo = x.FleteACargo,
                KgBalanza = x.KgBalanza,
                CondicionDePagoDiaPesificado = x.CondicionDePagoDiaPesificado,
                CondicionDePagoTipoPesificado = x.CondicionDePagoTipoPesificado,
                CondicionDePagoPesificadoVentaId = x.CondicionDePagoPesificadoVentaId,
                Pago = x.Pago,
                CondicionDePagoDiaFijacion = x.CondicionDePagoDiaFijacion,
                CondicionDePagoTipoFijacion = x.CondicionDePagoTipoFijacion,
                CondicionDePagoFijacionVentaId = x.CondicionDePagoFijacionVentaId,
                BoletoVentaId = x.BoletoVentaId,
                MailVentaBoleto = x.MailVentaBoleto,
                CreditoDisponible = x.CreditoDisponible,
                Cesion = x.Cesion,
                TarifaAConvenir = x.TarifaAConvenir,
                ConDescarga = x.ConDescarga,
                DolarExportador = x.DolarExportador,
                Descuentos = x.Descuentos.Select(y => new DescuentoBonificacionDto
                {
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? DbFunctions.Right("00" + SqlFunctions.DateName("day", y.FechaDesde).Trim(), 2) + "-" +
                                            DbFunctions.Right("00" + SqlFunctions.StringConvert((double)y.FechaDesde.Value.Month).TrimStart(), 2) + "-" +
                                           SqlFunctions.DateName("year", y.FechaDesde) : "",
                    FechaHasta = y.FechaHasta != null ? DbFunctions.Right("00" + SqlFunctions.DateName("day", y.FechaHasta).Trim(), 2) + "-" +
                                            DbFunctions.Right("00" + SqlFunctions.StringConvert((double)y.FechaHasta.Value.Month).TrimStart(), 2) + "-" +
                                           SqlFunctions.DateName("year", y.FechaHasta) : "",
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Moneda = y.MonedaId,
                    Id = y.Id,
                    Porcentaje = y.Porcentaje,
                    TipoDBDesc = y.TipoDB.Descripcion,
                    TipoDBId = y.TipoDBId,
                    TipoPeriodoDBDesc = y.TipoPeriodoDB.Descripcion,
                    TipoPeriodoDBId = y.TipoPeriodoDBId
                }).ToList(),
                Calidades = x.Calidad.Select(y => new CalidadDto
                {
                    Id = y.Id,
                    Valor = y.Valor,
                    CalidadEspecialId = y.CalidadEspecialId,
                    CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                    PorcentajeDesde = y.PorcentajeDesde,
                    PorcentajeHasta = y.PorcentajeHasta
                }).ToList(),
                AperturaPrecios = x.AperturaPrecio.Select(y => new AperturaPrecioDto
                {
                    contratoId = y.NegocioId,
                    Id = y.Id,
                    ConceptoAperturaPrecio = y.ConceptoAperturaPrecio.Descripcion,
                    ConceptoAperturaPrecioId = y.ConceptoAperturaPrecioId,
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Porcentaje = y.Porcentaje
                }).ToList(),
                PreciosPactados = x.PrecioPactado.Select(y => new PrecioPactadosDto
                {
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? SqlFunctions.DateName("day", y.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)y.FechaDesde.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", y.FechaDesde) : "",
                    FechaHasta = y.FechaHasta != null ? SqlFunctions.DateName("day", y.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)y.FechaHasta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", y.FechaHasta) : "",
                    Id = y.Id,
                    ImportePactado = y.ImportePactado,
                    MonedaImportePactadoDesc = y.MonedaImportePactado.Descripcion,
                    MonedaImportePactadoId = y.MonedaImportePactadoId,
                    MonedaPactadoDesc = y.MonedaPactado.Descripcion,
                    MonedaPactadoId = y.MonedaPactadoId,
                    Porcentaje = y.Porcentaje,
                    Precio = y.Precio
                }).ToList(),
                Servicios = x.Servicios.Select(y => new ServicioValorDto
                {
                    Id = y.Id,
                    ServicioValorId = y.ServicioValor.Id,
                    Descripcion = y.ServicioValor.TipoServicio.Descripcion,
                    CodigoSAP = y.ServicioValor.TipoServicio.CodigoSAP,
                    Importe = y.Importe,
                    MonedaDescripcion = y.Moneda.Descripcion,
                    MonedaId = y.Moneda.MonedaId,
                    Desde = y.Desde,
                    Hasta = y.Hasta,
                    TipoServicioId = y.ServicioValor.TipoServicio.Id,
                    Modificado = y.Modificado
                }).ToList()
            });
            return contrato;
        }

        public Resultado ControlesAccesoConDescarga(Negocio contrato)
        {
            var oErrorMessages = new Resultado();

            contrato.ConDescarga = contrato.ConDescarga ?? false;

            if ((bool)contrato.ConDescarga)
            {
                var diasParametro = repositorio.Obtener<Configuracion>(1).CantidadMaximaDiasNegocioConDescarga;
                if (contrato.Id != 0)
                {
                    var contratoGuardado = repositorio.Obtener<Negocio>(contrato.Id);
                    if (contrato.FechaHasta != contratoGuardado.FechaHasta) oErrorMessages.Error("FechaHasta", "No se permite modificar la fecha hasta en negocios con descarga.\n\n");
                    if (contrato.Cantidad < contratoGuardado.Cantidad) oErrorMessages.Error("Cantidad", "No se permite reducir la cantidad de kilos en negocios con descarga.\n\n");
                }

                var hoy = DateTime.Today;
                var fechaLimite = hoy.AddDays(diasParametro);

                var fechasConDescarga = new List<DateTime>();
                var fechasEntrega = new List<DateTime>();

                for (var dt = hoy; dt <= fechaLimite; dt = dt.AddDays(1))
                {
                    fechasConDescarga.Add(dt);
                }
                for (var dt = contrato.FechaDesde; dt <= contrato.FechaHasta; dt = dt.AddDays(1))
                {
                    fechasEntrega.Add(dt);
                }
                var fechasAmbos = fechasEntrega.Intersect(fechasConDescarga);

                if (fechasAmbos.Count() == 0)
                {
                    oErrorMessages.Error("FechaDesdeHasta", "El rango de entrega del negocio imposibilita la configuración de Cupos Con Descarga.\n\n");
                }

                if (contrato.ComercialId == null) oErrorMessages.Error("Zona", "Debe seleccionar un comercial.");
                else
                {
                    var grupoDeCompras = repositorio.Listar<Comercial>(x => x.ComercialId == contrato.ComercialId).First().GrupoDeCompras.Descripcion;
                    var zona = repositorio.Listar<ZonaCupo>(x => x.Descripcion == grupoDeCompras).FirstOrDefault();
                    if (zona == null) oErrorMessages.Error("Zona", "El comercial seleccionado no tiene zona cupo asignada.\n\n");
                }
            }

            return oErrorMessages;
        }

        public Cupo TransformarContratoACupo(Negocio contrato)
        {
            var cuitProveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == contrato.ProveedorId, x => x.CUIT);
            var comercial = repositorio.Obtener<Comercial>(x => x.ComercialId == contrato.ComercialId);
            var comercialId = contrato.ComercialId;
            var grupoDeCompras = comercial.GrupoDeCompras.Descripcion;
            var zonaComercial = repositorio.Listar<ZonaCupo>(x => x.Descripcion == grupoDeCompras).First();

            var cupoNuevo = new Cupo
            {
                Id = 0,
                ProveedorId = contrato.CorredorId == null ? contrato.ProveedorId.Value : contrato.CorredorId.Value,
                MaterialId = contrato.MaterialId,
                FechaIngreso = contrato.FechaEntrega.Value,
                CentroId = contrato.DestinoId.Value,
                FleteProcedencia = contrato.FleteACargo == "true",
                Calidad = contrato.MaterialId == 3 ? contrato.StandardDeCalidadId == 4 ? "Camara" : "Fabrica" : "",
                Observaciones = contrato.Observacion,
                Fason = contrato.EsFason,
                Destinatario = "30715118773",
                ComercialId = comercialId,
                FechaGeneracion = DateTime.Now,
                NegocioId = contrato.Id,
                ZonaCupoId = zonaComercial.Id,
                ConDescarga = contrato.ConDescarga,
                Sustentable = contrato.Sustentable,
                EPA = contrato.EPA,
                EUDR = contrato.EUDR,
                ComercialCreadorId = contrato.ComercialCreadorId,
            };

            return cupoNuevo;
        }

        public Resultado ValidarAltaTemprana(Negocio negocio, Proveedor proveedor)
        {
            var resultado = new Resultado();
            string tipoProv = proveedor.SegmentacionId == 5 || proveedor.SegmentacionId == 7 ? "CORR" : "PROV";
            var alta = altaTempranaAgent.ObtenerAlta(proveedor.CUIT, tipoProv);
            if (string.IsNullOrEmpty(alta.Mensaje))
            {
                if (negocio.Consignatario.HasValue && negocio.Consignatario.Value && alta.Consignatario == "NO")
                {
                    resultado.Error("Consignatario", "El proveedor no está habilitado como Consignatario.\n\n");
                }
                if (negocio.PlanCanje.HasValue && negocio.PlanCanje.Value && alta.PlanCanje == "NO")
                {
                    resultado.Error("PlanCanje", "El proveedor no está habilitado para Plan Canje.\n\n");
                }
                if (negocio.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA)
                    resultado.Errores.AddRange(ValidarCartaOferta(negocio, alta.Carta).Errores);

                if (alta.AltaTemprana == "SI")
                {
                    if (alta.Bolsa == "NO" && alta.Nosis == "NO")
                    {
                        resultado.Error("", "El vendedor de alta temprana no tiene informe Nosis aprobado ni legajo de la bolsa.\n\n");
                    }
                    if (alta.Bolsa == "NO" && alta.Nosis == "SI")
                    {
                        resultado.Error("", "El vendedor de alta temprana no tiene legajo de la bolsa.\n\n");
                    }
                    if (alta.Bolsa == "SI" && alta.Nosis == "NO")
                    {
                        resultado.Error("", "El vendedor de alta temprana no tiene informe Nosis aprobado.\n\n");
                    }
                }
                if (alta.ProveedorGrano == "SI")
                {
                    resultado.Error("MateriasPrimas", "El proveedor es un vendedor eventual.\n\n");
                }
                if (alta.BoletoFisico == "NO" && negocio.BoletoId == (int)EnumBoletoCompraNet.FISICO)
                {
                    resultado.Error("BoletoFisico", "No está habilitado para operar con boleto físico.\n\n");
                }
            }
            else
            {
                resultado.Error("", alta.Mensaje);
            }
            return resultado;
        }

        private Resultado ValidarCartaOferta(Negocio negocio, string altaTempranaCO)
        {
            var resultado = new Resultado();
            var boletoCompraNetProvincias = repositorio.Listar<BoletoCompraNetProvincia>(x => x.BoletoCompraNetId == (int)EnumBoletoCompraNet.CARTA_OFERTA);
            bool esProductor = negocio.ClasificacionId == (int)EnumClasificacionCompraNet.Productor;
            if (!boletoCompraNetProvincias.Any(x => x.ProvinciaId == negocio.ProvinciaId))
            {
                resultado.Error("Carta Oferta", "La provincia de procedencia no está habilitada para operar con carta oferta.\n\n");
                return resultado;
            }
            else if (negocio.ProvinciaId == 12 && esProductor)
            {
                resultado.Error("Carta Oferta", "La provincia de Santa Fe no está habilitada para que los productores operen con carta oferta.\n\n");
                return resultado;
            }
            else if (!negocio.Destino.CentroPropio)
            {
                resultado.Error("Carta Oferta", "Solo se puede operar con carta oferta en destinos propios de MOA. " + negocio.Destino.Descripcion + " no lo es.\n\n");
            }
            else if (altaTempranaCO == "NO")
            {
                resultado.Error("Carta Oferta", "No está habilitado para operar con carta oferta.\n\n");
                return resultado;
            }
            else if (negocio.BolsaId != (int)EnumBolsaCompraNet.BS_AS)
            {
                resultado.Error("Carta Oferta", "La bolsa debe ser Buenos Aires cuando el boleto es Carta Oferta.\n\n");
            }
            return resultado;
        }

        public Resultado ValidarSinBoleto(Negocio contrato)
        {
            var resultado = new Resultado();
            var materialesHabilitados = repositorio.Listar<MaterialHabilitadoSinBoleto, int>(x => x.MaterialId);
            var centrosHabilitados = repositorio.Listar<CentroHabilitadoSinBoleto, CentroHabilitadoSinBoletoDto>(x =>
                new CentroHabilitadoSinBoletoDto() { Id = x.Id, Centro = x.Centro.Descripcion, CentroId = x.CentroId, TipoNegocioId = x.TipoNegocioId });
            var clasificacionHabilitados = repositorio.Listar<ClasificacionHabilitadoSinBoleto, int>(x => x.ClasificacionId);
            var operacionHabilitada = repositorio.Listar<TipoOperacionHabilitadoSinBoleto, TipoOperacionHabilitadoSinBoletoDto>(x =>
                new TipoOperacionHabilitadoSinBoletoDto() { Id = x.Id, Corredor = x.Corredor, OperacionDirecta = x.Directo }).FirstOrDefault();
            var tipoNegocioHabilitados = repositorio.Listar<TipoNegocioHabilitadoSinBoleto, int>(x => x.TipoNegocioId);
            var provinciaNoHabilitados = repositorio.Listar<ProvinciaNoHabilitadoSinBoleto, int>(x => x.ProvinciaId); //no hay ABM para que los usuarios gestionen la tabla y BoletoCompraNetProvincia cubre la misma función
            var provinciasHabilitadas = repositorio.Listar<BoletoCompraNetProvincia, int>(a => a.ProvinciaId, x => x.BoletoCompraNetId == (int)EnumBoletoCompraNet.SIN_BOLETO);
            bool esProductor = contrato.ClasificacionId == (int)EnumClasificacionCompraNet.Productor;

            if (!materialesHabilitados.Any(x => x == contrato.MaterialId))
            {
                resultado.Error("Material", "El material seleccionado no está habilitado para la carga de contratos sin boleto.\n\n");
                return resultado;
            }
            if (contrato.DestinoId != null && !centrosHabilitados.Any(x => x.CentroId == contrato.DestinoId && x.TipoNegocioId == contrato.TipoNegocioId))
            {
                if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    resultado.Error("Centro", "El destino seleccionado no está habilitado para la carga de contratos a precio sin boleto.\n\n");
                }
                else if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                {
                    resultado.Error("Centro", "El destino seleccionado no está habilitado para la carga de contratos a fijar sin boleto.\n\n");
                }
                return resultado;
            }
            if (contrato.ProvinciaId != null && !provinciasHabilitadas.Any(x => x == contrato.ProvinciaId))
            {
                resultado.Error("Provincia", "La provincia de procedencia no está habilitada para la carga de contratos sin boleto.\n\n");
                return resultado;
            }
            if (contrato.ClasificacionId != null && !clasificacionHabilitados.Any(x => x == contrato.ClasificacionId))
            {
                resultado.Error("Clasificacion", "La clasificación seleccionada no está habilitada para la carga de contratos sin boleto.\n\n");
                return resultado;
            }

            if (!tipoNegocioHabilitados.Any(x => x == contrato.TipoNegocioId))
            {
                resultado.Error("TipoNegocio", "El tipo de negocio seleccionado no está habilitado para la carga de contratos sin boleto.\n\n");
                return resultado;
            }
            bool conCorredor = contrato.CorredorId != null && contrato.CorredorId > 0;
            if (conCorredor && operacionHabilitada.Corredor != true)
            {
                resultado.Error("Operacion", "Las operaciones con corredor no están habilitadas sin boleto.\n");
                return resultado;
            }
            if (!conCorredor && operacionHabilitada.OperacionDirecta != true)
            {
                resultado.Error("Operacion", "Las operaciones directas (sin corredor) no están habilitadas sin boleto.\n\n");
                return resultado;
            }
            if (contrato.ProvinciaId == 12 && esProductor)
            {
                resultado.Error("Productor Santa Fe", "La provincia de Santa Fe no está habilitada para que los productores operen sin boleto.\n\n");
                return resultado;
            }
            if (contrato.Warrant == true || contrato.CD == true)
            {
                resultado.Error("SinBoleto", "Los contratos sin boleto no pueden tener la tilde CD o Warrant.\n\n");
            }
            if (!string.IsNullOrEmpty(contrato.ContratoMadre))
            {
                resultado.Error("Convenir", "Los contratos sin boleto no pueden tener la tilde Fij. Convenio.\n\n");
            }
            if (contrato.DestinoId != null && contrato.ProveedorId != null)
            {
                var centroCodigo = repositorio.Obtener<Centro, string>(x => x.Id == contrato.DestinoId, x => x.CodigoSap);
                var materialCodigo = repositorio.Obtener<Material, string>(x => x.MaterialId == contrato.MaterialId, x => x.Codigo);
                var cuitProveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == contrato.ProveedorId, x => x.CUIT);
                var contratosPendientes = repositorio.Listar<Contrato>(x => x.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO && x.Id != contrato.Id && x.ProveedorId == contrato.ProveedorId && x.DestinoId == contrato.DestinoId &&
                x.MaterialId == contrato.MaterialId && x.EstadoId != (int)EnumEstadoContrato.Rechazado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.ConfirmadoSAP != true && (x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO));

                var pendienteDto = new CcPpPendienteAplicarDto()
                {
                    Centro = centroCodigo,
                    Material = materialCodigo,
                    Proveedor = cuitProveedor,
                    Corredor = null,
                };
                logger.Debug($"Datos ingresados para CartasDePortePendienteAplicar: {pendienteDto.ToJson()}");
                var pendientes = ccppPendientesAgent.ListarCartasDePortePendienteAplicar(pendienteDto);
                var sustentable = contrato.Sustentable | contrato.EPA | contrato.EUDR;
                double cantidadDisponible = DevolverCantidadDisponible(sustentable, contratosPendientes, pendientes, true);
                logger.Debug($"Cantidad Disponible: {cantidadDisponible}");

                if (contrato.Cantidad > cantidadDisponible)
                {
                    if (cantidadDisponible < 0) { cantidadDisponible = 0; }
                    resultado.Error("Cantidad", $"La cantidad del contrato sin boleto no puede exceder la cantidad de {cantidadDisponible:N2} kg. disponibles en depósito.\n\n");
                    return resultado;
                }
            }
            return resultado;
        }

        public double DevolverCantidadDisponible(bool sustentable, List<Contrato> contratosPendientes, List<CcPpPendienteAplicarDto> ccppPendientes, bool sinBoleto)
        {
            if (sinBoleto) ccppPendientes.RemoveAll(x => x.Region == "3");
            var cantidadCartaDePorte = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.CartasPorte)).Sum(x => x.Cantidad);
            var cantidadContratoDeSAP = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && (x.Canje || x.CD || x.Warrant)).Sum(x => x.KgContrato);
            logger.Debug($"cantidadContratoDeSAPCanje {ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && (x.Canje || x.CD || x.Warrant)).ToJson()}");

            var contratosPendientesAplicar = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && x.Canje == false && x.CD == false && x.Warrant == false).Select(x => x.Contrato).ToList();
            logger.Debug($"contratosPendientesAplicar {contratosPendientesAplicar.ToJson()}");

            var contratosFinalizados = repositorio.Listar<Contrato, BasicoContrato>(x => new BasicoContrato
            {
                ContratoSAP = x.ContratoSAP,
                BoletoId = x.BoletoId,
                Cantidad = x.Cantidad
            }, x => contratosPendientesAplicar.Contains(x.ContratoSAP));

            contratosFinalizados = sinBoleto ? contratosFinalizados.Where(x => x.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO && x.FechaDesde <= DateTime.Today.AddDays(1)).ToList() : contratosFinalizados.Where(x => x.FechaDesde <= DateTime.Today.AddDays(1)).ToList();

            var cantidadContratosKilosPendientesAplicar = ccppPendientes.Where(x => contratosFinalizados.Select(y => y.ContratoSAP).Contains(x.Contrato)).Sum(x => x.KgContrato);
            var cantidadNegocioPendiente = contratosPendientes.Where(x => !ccppPendientes.Any(y => y.Contrato.Contains(x.ContratoSAP))).Sum(x => x.Cantidad);

            logger.Debug($"cantidadContratosKilosPendientesAplicar {cantidadContratosKilosPendientesAplicar}");

            //Soja Comun
            if (!sustentable)
            {
                cantidadCartaDePorte = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.CartasPorte) && !x.Sustentable && !x.EPA).Sum(x => x.Cantidad);
                cantidadContratoDeSAP = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && !x.Sustentable && !x.EPA && (x.Canje || x.CD || x.Warrant)).Sum(x => x.KgContrato);
                logger.Debug($"soja comun {ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && !x.Sustentable && !x.EPA && (x.Canje || x.CD || x.Warrant)).ToJson()}");
                cantidadNegocioPendiente = contratosPendientes.Where(x => x.Sustentable != true && x.EPA != true).Sum(x => x.Cantidad);
                cantidadContratosKilosPendientesAplicar = ccppPendientes.Where(x => x.Sustentable != true && x.EPA != true && contratosFinalizados.Select(y => y.ContratoSAP).Contains(x.Contrato)).Sum(x => x.KgContrato);
            }
            logger.Debug($"soja sustentable {ccppPendientes.Where(x => !string.IsNullOrEmpty(x.Contrato) && (x.Canje || x.CD || x.Warrant)).ToJson()}");
            logger.Debug($"CantidadCartaDePorte {cantidadCartaDePorte}, cantidadContratoDeSAP {cantidadContratoDeSAP}, cantidadNegocioPendiente {cantidadNegocioPendiente}, cantidadContratosKilosPendientesAplicar {cantidadContratosKilosPendientesAplicar}");
            var cantidadDisponible = cantidadCartaDePorte - cantidadContratoDeSAP - (decimal)cantidadNegocioPendiente - cantidadContratosKilosPendientesAplicar;
            return decimal.ToDouble(cantidadDisponible);
        }
    }
}