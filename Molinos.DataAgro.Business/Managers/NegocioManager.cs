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


        public NegocioManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
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
            var contratos = repositorio.Listar<Negocio>(x => DbFunctions.TruncateTime(x.Fecha) != DbFunctions.TruncateTime(x.FechaOperacion) && x.Fecha >= hoy && x.Canje != true && x.PrestamoDevolucion != true && x.Venta != true && x.TipoNegocioId != 5);
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
                var idJefes = repositorio.Listar<Comercial,string>(x=>x.IdActiveDirectory, 
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
                var cuerpoMail = CuerpoMailNegociosConDiaAnterior(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), contratos);
                mailManager.EnviarMail(lista, "Negocios con fecha anterior", "", listaJefes, cuerpoMail);
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
                    th + "Precio" + "</td>" +
                    th + "Moneda" + "</td>" +
                    th + "Cantidad" + "</td>" +
                    th + "Fecha de Carga" + "</td>" +
                    th + "Fecha de Operacion" + "</td>" +
                    th + "Comercial" + "</td>" +
                    th + "Corredor" + "</td>" +
                    th + "Proveedor" + "</td>" +
                    th + "Tipo" + "</td>" +
                    th + "Estado:" + "</td>" +
                    th + "Motivo:" + "</td>" +
                    "</tr>";

            foreach (var c in contratos)
            {
                string style1 = "";
                string style2 = "";
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                    style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                }
                else
                {
                    style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                    style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                }
                logger.Debug($"contrato numero: {c.Id}");
                linea += 1;
                if (linea % 2 == 0)
                {
                    htmlBody += "<tr>" +
                         "<td " + style1 + (c.TipoNegocioId != 3 ? c.ContratoSAP : c is FijacionDePrecioContrato ? (c as FijacionDePrecioContrato).FijacionSAP : "") + "</td>" +
                         "<td " + style1 + c.Material.Descripcion + "</td>" +
                         "<td " + style1 + (c.Precio == 0 ? "" : c.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td>" +
                         "<td " + style1 + (c.MonedaId == null ? "" : c.Moneda.Descripcion) + "</td>" +
                         "<td " + style1 + c.Cantidad + "</td>" +
                         "<td " + style1 + c.Fecha.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                         "<td " + style1 + c.FechaOperacion.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                         "<td " + style1 + c.Comercial.Nombres + " " + c.Comercial.Apellido + "</td>" +
                         (c.Corredor != null ? "<td " + style1 + c.Corredor.RazonSocial + "</td>" : "<td " + style1 + "</td>") +
                         "<td " + style1 + (c.ProveedorId == null ? "" : c.Proveedor.RazonSocial) + "</td>" +
                         "<td " + style1 + c.TipoNegocio.Descripcion + "</td>" +
                         "<td " + style1 + c.Estado.Descripcion + "</td>" +
                         "<td " + style1 + (c.MotivoOperacionAnterior ?? "") + "</td> </tr> ";
                }
                else
                {
                    htmlBody += "<tr>" +
                         "<td " + style2 + (c.TipoNegocioId != 3 ? c.ContratoSAP : c is FijacionDePrecioContrato ? (c as FijacionDePrecioContrato).FijacionSAP : "") + "</td>" +
                         "<td " + style2 + c.Material.Descripcion + "</td>" +
                         "<td " + style2 + (c.Precio == 0 ? "" : c.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td>" +
                         "<td " + style2 + (c.MonedaId == null ? "" : c.Moneda.Descripcion) + "</td>" +
                         "<td " + style2 + c.Cantidad + "</td>" +
                         "<td " + style2 + c.Fecha.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                         "<td " + style2 + c.FechaOperacion.ToString("dd/MM/yyyy hh:mm:ss") + "</td>" +
                         "<td " + style2 + c.Comercial.Nombres + " " + c.Comercial.Apellido + "</td>" +
                         (c.Corredor != null ? "<td " + style2 + c.Corredor.RazonSocial + "</td>" : "<td " + style2 + "</td>") +
                         "<td " + style2 + (c.ProveedorId == null ? "" : c.Proveedor.RazonSocial) + "</td>" +
                         "<td " + style2 + c.TipoNegocio.Descripcion + "</td>" +
                         "<td " + style2 + c.Estado.Descripcion + "</td>" +
                         "<td " + style2 + (c.MotivoOperacionAnterior ?? "") + "</td> </tr> ";
                }
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
    }
}
