using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Criterios;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;






namespace Molinos.DataAgro.Business.Managers
{
    public class CupoManager : ICupoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ICrearCupoAgent crearCupoAgent;
        private readonly IEliminarCupoAgent eliminarCupoAgent;
        private readonly IClienteStopAgent clienteStopAgent;
        private readonly IModificarCupoAgent modificarCupoAgent;
        private readonly IProveedorManager proveedorManager;
        private readonly IMailManager mailManager;
        private readonly IServicioCriterios servicioCriterios;
        private readonly IDisponibilidadCuposAgent disponibilidadCuposAgent;
        private readonly ICriterioCDWarrantAgent cdWarrant;
        private readonly ILogDataAgroManager logDataAgroManager;


        public CupoManager(IRepositorio repositorio, ILogger logger, ICrearCupoAgent crearCupoAgent,
            IEliminarCupoAgent eliminarCupoAgent, IClienteStopAgent clienteStopAgent, IModificarCupoAgent modificarCupoAgent,
            IProveedorManager proveedorManager, IMailManager mailManager, IServicioCriterios servicioCriterios,
            IDisponibilidadCuposAgent disponibilidadCuposAgent, ICriterioCDWarrantAgent cdWarrant, ILogDataAgroManager logDataAgroManager)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.crearCupoAgent = crearCupoAgent;
            this.eliminarCupoAgent = eliminarCupoAgent;
            this.clienteStopAgent = clienteStopAgent;
            this.modificarCupoAgent = modificarCupoAgent;
            this.proveedorManager = proveedorManager;
            this.mailManager = mailManager;
            this.servicioCriterios = servicioCriterios;
            this.disponibilidadCuposAgent = disponibilidadCuposAgent;
            this.cdWarrant = cdWarrant;
            this.logDataAgroManager = logDataAgroManager;
        }
        public CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias)
        {
            var error = new CupoResult { ListaCupos = new List<string>() };
            try
            {
                var comercial = repositorio.Obtener<Comercial>(cupo.ComercialId); ;
                cupo.Proveedor = repositorio.Obtener<Proveedor>(cupo.ProveedorId);
                cupo.Comercial = comercial;
                cupo.Material = repositorio.Obtener<Material>(cupo.MaterialId);
                cupo.Centro = repositorio.Obtener<Centro>(cupo.CentroId);
                cupo.ZonaCupo = repositorio.Obtener<ZonaCupo>(cupo.ZonaCupoId);
                if (cupo.Id == 0)
                {
                    foreach (var d in dias)
                    {
                        if (d.Cantidad > 0)
                        {
                            if (d.Fecha < DateTime.Today)
                            {
                                error.Error("CantidadCuposSAP", d.Fecha.ToShortDateString() + ": La Fecha de Ingreso no debe ser una fecha menor al día de hoy");
                                continue;
                            }
                            cupo.FechaIngreso = d.Fecha;
                            var listaCupos = new List<string>();
                            var errorSap = new Resultado();
                            try
                            {
                                listaCupos = crearCupoAgent.Crear(cupo, d.Cantidad.Value);
                            }
                            catch (Exception e)
                            {
                                errorSap.Error("CantidadCuposSAP", cupo.FechaIngreso.ToShortDateString() + ": " + e.Message);
                            }
                            if (errorSap.HayError)
                            {
                                error.Errores.AddRange(errorSap.Errores);
                                continue;
                            }
                            var cuposConSap = new List<Cupo>();

                            cupo.EstadoCupoId = cupo.Centro.CodigoSap == "1600" || cupo.Centro.CodigoSap == "1029" ? 6 : 8;
                            foreach (var cupoSap in listaCupos)
                            {
                                var nuevoCupo = (Cupo)cupo.Clone();
                                nuevoCupo.CupoSap = cupoSap;
                                cuposConSap.Add(nuevoCupo);
                            }
                            repositorio.AgregarTodos(cuposConSap);
                            repositorio.GuardarCambios();

                            foreach (var cupoNuevo in cuposConSap)
                            {
                                if (cupo.Id == 0)
                                {
                                    var cupoConId = repositorio.Obtener<Cupo>(x => x.CupoSap == cupoNuevo.CupoSap);
                                    logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoConId.Id), TipoAccionLogDataAgro.Crear);
                                }
                            }



                            if (listaCupos.Count < d.Cantidad.Value)
                            {
                                error.Error("CantidadCuposSAP", "Se generaron " + listaCupos.Count + " de " + d.Cantidad.Value + " cupos solicitados para el dia " + cupo.FechaIngreso.ToShortDateString());
                            }
                            error.ListaCupos.AddRange(listaCupos);
                        }
                    }
                    if (error.ListaCupos.Count > 0)
                    {

                        EnviarEmail(cupo, error.ListaCupos);
                    }
                    return error;
                }
                else
                {
                    var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                    var cupoSave = repositorio.Obtener<Cupo>(cupo.Id);
                    cupoSave.ProveedorId = cupo.ProveedorId;
                    cupoSave.Proveedor = cupo.Proveedor;
                    cupoSave.Calidad = cupo.Calidad;
                    cupoSave.Observaciones = cupo.Observaciones;
                    cupoSave.Destinatario = cupo.Destinatario;
                    cupoSave.Fason = cupo.Fason;
                    cupoSave.FleteProcedencia = cupo.FleteProcedencia;
                    var res = modificarCupoAgent.Modificar(cupoSave);
                    if (res != "Ok")
                    {
                        error.Error("SAP", $"Error al grabar en SAP: {res}");
                    }
                    if (!cupoSave.Centro.Acopio)
                    {
                        if (datosConfiguracion.ConexionABMStop.HasValue && datosConfiguracion.ConexionABMStop.Value)
                        {
                            if (cupoSave.CupoStop != null)
                            {
                                clienteStopAgent.ModificarCupo(cupoSave);
                            }
                        }
                        else
                        {
                            error.Error("Stop", "Sin Conexión a Stop. Modificado en SAP");
                        }
                    }
                    repositorio.GuardarCambios();
                    var asd = ObtenerCupo(cupoSave.Id);
                    logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSave.Id), TipoAccionLogDataAgro.Modificar);

                    return error;
                }
            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Errores.Add(new ErrorMessage(400, e.Message));
                return error;
            }
        }
        public Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta)
        {
            var error = new Resultado();
            if (cupo.ProveedorId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El Proveedor no debe estar vacío"));
            }
            if (cupo.ProveedorId != 0)
            {
                var cuit = repositorio.Obtener<Proveedor, string>(y => y.ProveedorId == cupo.ProveedorId, y => y.CUIT);
                var sisa = repositorio.Obtener<SISA>(x => x.CUIT == cuit);
                if ((sisa != null && (sisa.EstadoCuit == 0 || sisa.EstadoCuit == 3)) || sisa == null)
                {
                    error.Errores.Add(new ErrorMessage(400, "Proveedor con CUIT en estado No Operable"));
                }
            }
            if (cupo.Id == 0 && cantidadCupos == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "La Cantidad no debe estar vacía"));
            }
            if (cupo.MaterialId == 3 && (cupo.Calidad == "" || cupo.Calidad == null))
            {
                error.Errores.Add(new ErrorMessage(400, "La calidad no debe estar vacia para Soja"));
            }
            if (cupo.Fason == true && (cupo.Destinatario == "" || cupo.Destinatario == null))
            {
                error.Errores.Add(new ErrorMessage(400, "CUIT Destinatario no debe estar vacío cuando elige Fasón/Préstamo Devolución"));
            }
            if (cupo.FechaIngreso.Date < DateTime.Today && fechaHasta.HasValue && fechaHasta < DateTime.Today)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha de Ingreso no debe ser una fecha menor al día de hoy"));
            }
            if (cupo.Id == 0 && fechaHasta.HasValue && fechaHasta < cupo.FechaIngreso)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha Hasta de entrega no puede ser menor a la Fecha Desde"));
            }

            return error;
        }
        public DataSourceResult TraerCuposTabla(DataSourceRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosCupos(request, equipo));
        }

        public Resultado EliminarCupo(int id, string comercial)
        {
            try
            {
                var nuevoResultado = new Resultado();

                var cupoSap = repositorio.Obtener<Cupo>(id);
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                var resultado = eliminarCupoAgent.Eliminar(cupoSap.CupoSap, comercial);
                if (resultado == "OK")
                {
                    cupoSap.EstadoCupoId = 4;

                    repositorio.GuardarCambios();

                    logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSap.Id), TipoAccionLogDataAgro.Eliminar);


                }
                else
                {
                    nuevoResultado.Error("", $"Error al anular cupo en SAP: {resultado}");
                }
                if (!cupoSap.Centro.Acopio)
                {
                    if (cupoSap.EstadoCupoId == 4 && cupoSap.CupoStop != null)
                    {
                        if (datosConfiguracion.ConexionABMStop.HasValue && !datosConfiguracion.ConexionABMStop.Value)
                        {
                            nuevoResultado.Error("Stop", "Error al anular cupo en STOP: Sin conexión a STOP. Anulado en SAP Correctamente");
                            return nuevoResultado;
                        }
                        var resultadoStop = clienteStopAgent.EliminarCupo(cupoSap);
                        if (nuevoResultado.HayError)
                        {
                            foreach (var e in resultadoStop.Errores)
                            {
                                nuevoResultado.Error("", $"Error al anular cupo en STOP: {e.Message}. Anulado en SAP Correctamente"); ;
                            }
                            return nuevoResultado;
                        }
                    }
                }
                return nuevoResultado;
            }
            catch (Exception e)
            {
                var nuevoResultado = new Resultado();
                nuevoResultado.Error("eliminar", e.Message);
                return nuevoResultado;
            }
        }

        public Resultado TransmitirCupos(List<string> cupos)
        {
            var result = new Resultado();
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionABMStop.HasValue && !datosConfiguracion.ConexionABMStop.Value)
            {
                result.Error("Stop", "Sin conexión a STOP");
                return result;
            }
            try
            {
                clienteStopAgent.CrearCupo(cupos);
            }
            catch (Exception e)
            {
                result.Error("", e.Message);
            }
            return result;
        }
        public void TransmitirCupos()
        {
            clienteStopAgent.TransmitirJobCupos();
        }
        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            return clienteStopAgent.ConsultarCuposDiarios();
        }
        public CupoDto ObtenerCupo(int id)
        {
            return repositorio.Obtener<Cupo, CupoDto>(x => x.Id == id, x => new CupoDto
            {
                Id = x.Id,
                Calidad = x.Calidad,
                Centro = x.Centro.Descripcion,
                CentroId = x.CentroId,
                ComercialId = x.ComercialId,
                Destinatario = x.Destinatario,
                Fason = x.Fason,
                FleteProcedencia = x.FleteProcedencia,
                FechaIngreso = x.FechaIngreso,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                Observaciones = x.Observaciones,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap
            });
        }
        public Resultado EliminarVarios(List<int> cupos, string comercial)
        {
            var resultado = new Resultado();
            foreach (var c in cupos)
            {
                var resCupos = EliminarCupo(c, comercial);
                if (resCupos.HayError)
                {
                    resultado.Errores.AddRange(resCupos.Errores);
                }
            }
            return resultado;
        }

        public string ObtenerCodigoSap(int id)
        {
            return repositorio.Obtener<Cupo, string>(x => x.Id == id, x => x.CupoSap);
        }

        private void EnviarEmail(Cupo cupo, List<string> listaCupos)
        {
            try
            {
                var id = cupo.ProveedorId;
                var proveedorContacto = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == id && x.Cupo == true);
                if (proveedorContacto.Count == 0)
                {
                    return;
                }
                string emailComercial = "";

                if (cupo.Comercial != null)
                {
                    try
                    {
                        emailComercial = mailManager.GetEmailUserActiveDirectory(cupo.Comercial.IdActiveDirectory);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
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
                    if (!string.IsNullOrEmpty(emailComercial))
                        oMensaje.CC.Add(emailComercial);
                }
                else if (!string.IsNullOrEmpty(emailComercial))
                {
                    oMensaje.To.Add(emailComercial);
                }
                else
                {
                    logger.Debug($"El contrato {cupo.Id} no tiene ContactoComercial para el proveedor {cupo.ProveedorId} ni email comercial");
                    return;
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);


                oMensaje.AlternateViews.Add(CuerpoMail(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), listaCupos, cupo, emailComercial));
                var subject = "";

                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                {
                    subject += "Mail Pruebas - ";
                }
                subject += "Cupos Molinos Agro S.A. - ";
                subject += cupo.Proveedor.RazonSocial;
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

        private AlternateView CuerpoMail(String filePath, List<string> listaCupos, Cupo cupo, string emailComercial)
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

            htmlBody += "En el presente mail, se detalla los cupos generados con Molinos Agro S.A.: <br /><br />  ";

            foreach (var c in listaCupos)
            {
                htmlBody += c + "<br />";
            }
            htmlBody += "<br />";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + Td(ref linea, 2) + "Con destino SAN LORENZO - SANTA FE - BENIELLI 398" + "</td></tr>";
            htmlBody += "<tr>" + th + "FECHA DESCARGA: </th>" + Td(ref linea) + Split(cupo.FechaIngreso.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + Td(ref linea) + cupo.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINATARIO: </th>" + Td(ref linea) + (cupo.Destinatario.ToUpper() == "30715118773" ? "MOLINOS AGRO S.A.-30715118773" : cupo.Destinatario.ToUpper()) + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINO: </th>" + Td(ref linea) + "MOLINOS AGRO S.A.-30715118773" + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + Td(ref linea) + cupo.Material.Descripcion.ToUpper() + "</td></tr>";

            if (cupo.Centro.CodigoSap == "1600" && (cupo.MaterialId == 1 || cupo.MaterialId == 2 || cupo.MaterialId == 3))
            {
                htmlBody += "<tr>" + th + "OBSERVACIÓN</th>" + Td(ref linea);
                if (cupo.MaterialId == 1)
                {
                    htmlBody += "ESPECIAL<br />";
                }
                if (cupo.MaterialId == 2)
                {
                    htmlBody += "ESPECIAL<br />";
                }
                if (cupo.MaterialId == 3)
                {
                    htmlBody += "SUSTENTABLE<br />";
                }
            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /> Recordamos que el cupo tiene validez desde las 0 hrs hasta las 23:59 hrs del mismo día para el cual fue otorgado el cupo. Evitar el arribo previo o posterior a dicha fecha, ya que perjudican la operatoria, haciendo más lento el circuito de descarga y por ende mayores demoras para los transportes. A su vez, aquellos que no cumplan con la franja que corresponde al cupo podrán sufrir sanciones.";
            htmlBody += "<br /><br /> Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + cupo.Comercial.Nombres + " " + cupo.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
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
        private string Td(ref int linea, int largo = 1)
        {
            string td1 = "";
            string td2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                td1 = "<td colspan=\"" + largo + "\" style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                td2 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                td2 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
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
        private string TrEncabezado(Cupo c, ref int linea)
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
            logger.Debug($"cupo numero: {c.Id}");
            linea += 1;
            if (linea % 2 == 0)
            {
                return "<tr>" +
                    "<td " + style1 + c.Material.Descripcion + "</td>" +
                     "<td " + style1 + Split(c.FechaIngreso.ToShortDateString()) + "</td>" +
                     "<td " + style1 + c.Proveedor.RazonSocial + "</td>" +
                     "<td " + style1 + c.CupoSap + "</td>" +
                     "<td " + style1 + "Sin CTG" + "</td></ tr>";
            }
            else
            {
                return "<tr>" +
                        "<td " + style2 + c.Material.Descripcion + "</td>" +
                        "<td " + style2 + Split(c.FechaIngreso.ToShortDateString()) + "</td>" +
                        "<td " + style2 + c.Proveedor.RazonSocial + "</td>" +
                        "<td " + style2 + c.CupoSap + "</td>" +
                        "<td " + style2 + "Sin CTG" + "</td></ tr>";
            }

        }

        public void CrearSugerenciaCupo()
        {
            try
            {
                DateTime hoy = DateTime.Now.Date;

                Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
                logger.Debug("CrearSugerenciaCupo - se obtuvo la formula: ");
                var formulaSave = repositorio.Obtener<Formula>(formula.Id);
                formulaSave.Inicio = formula.Inicio;
                formulaSave.CantDias = formula.CantDias;
                formulaSave.CriterioId = formula.CriterioId;
                formulaSave.CentroId = formula.CentroId;
                formulaSave.Fecha = formula.Fecha;
                formulaSave.Usada = true;

                //cupos en rango de fecha
                List<Cupo> cupos = repositorio.Listar<Cupo>(x => x.FechaIngreso >= formula.FechaDesde && x.FechaIngreso <= formula.FechaHasta && x.CentroId == formula.CentroId);
                logger.Debug("CrearSugerenciaCupo - se obtuvieron " + cupos.Count + " cupos.");

                logger.Debug("CrearSugerenciaCupo - inicio de disponibilidad en planta.");
                List<ConfiguracionCupoDto> disponibilidadEnPlantas = ObtenerDisponibilidadEnPlantas(formula, cupos);
                logger.Debug("CrearSugerenciaCupo - fin de disponibilidad en planta.");

                List<SugerenciaCupoDto> negocios = new List<SugerenciaCupoDto>();

                logger.Debug("CrearSugerenciaCupo - inicio de obtener negocios.");
                ObtenerNegocios(hoy, formula, cupos, negocios);
                logger.Debug("CrearSugerenciaCupo - fin de obtener negocios.");

                logger.Debug("CrearSugerenciaCupo - inicio de ObtenerPuntajes.");
                ObtenerPuntajes(formula, negocios);
                logger.Debug("CrearSugerenciaCupo - fin de ObtenerPuntajes.");

                logger.Debug("CrearSugerenciaCupo - inicio de PriorizarSegunDisponibilidad.");
                PriorizarSegunDisponibilidad(disponibilidadEnPlantas, negocios);
                logger.Debug("CrearSugerenciaCupo - negocios priorizados: " + negocios.Where(a => a.Priorizado).Count());
                logger.Debug("CrearSugerenciaCupo - fin de PriorizarSegunDisponibilidad.");

                List<SugerenciaCupo> sugerencias = negocios.Where(a => a.Priorizado).Select(a => new SugerenciaCupo
                {
                    //AgenteCompraId = a.AgenteCompraId,
                    ZonaCupoId = a.ZonaCupoId ?? 0,
                    CantidadDeCupos = a.CantidadDeCupos,
                    CentroId = a.DestinoId,
                    NegocioId = a.NegocioId,
                    //ContratoId = a.ContratoId,
                    //FasonId = a.FasonId,
                    FechaSugerida = a.FechaSugerida,
                    //FijacionDePrecioContratoId = a.FijacionDePrecioContratoId,
                    ConfiguracionEspacioDinamicoId = a.ConfiguracionEspacioDinamicoId,
                    MaterialId = a.MaterialId,
                    MonedaId = a.MonedaId,
                    Precio = a.Precio,
                    TipoNegocioId = a.TipoNegocioId,
                    Puntuacion = a.PuntuacionTotal,
                    ProveedorId = a.ProveedorId,
                    Destinatario = a.Destinatario,
                    ComercialId = a.ComercialId,
                    StandardDeCalidad = a.StandardDeCalidad,
                    Aceptado = null,
                    Puntuaciones = JsonConvert.SerializeObject(a.Puntuaciones),
                    ContratoSAP = a.ContratoSAP,
                    CDWarrant = a.CDWarrant
                }).ToList();
                repositorio.RemoverTodos<SugerenciaCupo>(a => a.Aceptado != false);
                repositorio.AgregarTodos(sugerencias);
                repositorio.GuardarCambios();

                logger.Debug("CrearSugerenciaCupo - GuardarCambios.");
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }

        private List<ConfiguracionCupoDto> ObtenerDisponibilidadEnPlantas(Formula formula, List<Cupo> cupos)
        {
            List<ConfiguracionCupo> configuracionCupo = repositorio.Listar<ConfiguracionCupo>(x => x.Fecha >= formula.FechaDesde && x.Fecha <= formula.FechaHasta && x.CentroId == formula.CentroId);

            List<ConfiguracionCupoDto> disponibilidadEnPlantas = configuracionCupo.Select(x => new ConfiguracionCupoDto { CentroId = x.CentroId, LimiteCupo = x.LimiteCupo, MaterialId = x.MaterialId, Fecha = x.Fecha, CantidadCupo = x.CantidadCupo.Select(b => new LimiteCupoDto { ZonaCupo = b.ZonaCupo.Descripcion, ZonaCupoId = b.ZonaCupoId, CantidadCupo = b.CantidadCupo }).ToList() }).ToList();
            foreach (var cupo in cupos)
            {
                var configuracion = disponibilidadEnPlantas.Where(a => a.CentroId == cupo.CentroId && a.MaterialId == cupo.MaterialId && a.Fecha == cupo.FechaIngreso).SingleOrDefault();
                if (configuracion != null)
                {
                    configuracion.LimiteCupo -= 1;
                    var zonaCupo = configuracion.CantidadCupo.Where(a => a.ZonaCupoId == cupo.ZonaCupoId).SingleOrDefault();
                    if (zonaCupo != null)
                    {
                        zonaCupo.CantidadCupo -= 1;
                    }
                }
            }

            foreach (var item in disponibilidadEnPlantas)
            {
                logger.Debug("CrearSugerenciaCupo - DisponibilidadEnPlanta:" + item.Fecha.ToString("dd/MM/yyyy") + ",cantidad:" + item.LimiteCupo + "materialid:" + item.MaterialId);
            }
            return disponibilidadEnPlantas;
        }

        private void ObtenerPuntajes(Formula formula, List<SugerenciaCupoDto> negocios)
        {
            foreach (var item in negocios.ToList())
            {
                formula.Criterio.Dto = item;
                formula.Criterio.Dto.formula = formula;
                item.PuntuacionTotal = Math.Round(servicioCriterios.Calcular(formula.Criterio), 2);

                ArmarPuntuaciones(formula.Criterio, item.Puntuaciones, 0);

                item.formula = null;
            }
            negocios = negocios.OrderByDescending(a => a.PuntuacionTotal).ToList();
        }

        private static void PriorizarSegunDisponibilidad(List<ConfiguracionCupoDto> disponibilidadEnPlantas, List<SugerenciaCupoDto> negocios)
        {
            List<SugerenciaCupoDto> newNegocios = new List<SugerenciaCupoDto>();
            foreach (var negocio in negocios.OrderByDescending(a => a.PuntuacionTotal))
            {
                var disponibles = disponibilidadEnPlantas.Where(a => a.MaterialId == negocio.MaterialId && a.LimiteCupo > 0 && a.Fecha >= negocio.FechaDesde && a.Fecha <= negocio.FechaHasta).OrderBy(a => a.Fecha).ToList();

                foreach (var disponible in disponibles)
                {

                    if (disponible.LimiteCupo > 0 && !negocio.Priorizado)
                    {
                        var disponiblezona = disponible.CantidadCupo.Where(a => a.ZonaCupo == negocio.ZonaDescrip).SingleOrDefault();
                        if (disponiblezona != null)
                        {
                            if (disponiblezona.CantidadCupo >= negocio.CantidadDeCupos)
                            {
                                disponiblezona.CantidadCupo -= negocio.CantidadDeCupos;
                                disponible.LimiteCupo -= negocio.CantidadDeCupos;
                                negocio.Priorizado = true;
                                negocio.FechaSugerida = disponible.Fecha.Date;
                            }
                            else
                            {
                                if (disponiblezona.CantidadCupo > 0)
                                {
                                    var newNegocio = (SugerenciaCupoDto)negocio.Clone();
                                    newNegocio.CantidadDeCupos = disponiblezona.CantidadCupo;
                                    newNegocio.Priorizado = true;
                                    newNegocio.FechaSugerida = disponible.Fecha.Date;
                                    newNegocios.Add(newNegocio);

                                    negocio.CantidadDeCupos -= disponiblezona.CantidadCupo;
                                    disponible.LimiteCupo -= disponiblezona.CantidadCupo;
                                    disponiblezona.CantidadCupo = 0;
                                    negocio.Priorizado = false;
                                }

                            }
                        }
                        else
                        {
                            if (disponible.LimiteCupo >= negocio.CantidadDeCupos)
                            {
                                disponible.LimiteCupo -= negocio.CantidadDeCupos;
                                negocio.Priorizado = true;
                                negocio.FechaSugerida = disponible.Fecha.Date;
                            }
                            else
                            {
                                if (disponible.LimiteCupo > 0)
                                {
                                    var newNegocio = (SugerenciaCupoDto)negocio.Clone();
                                    newNegocio.CantidadDeCupos = disponible.LimiteCupo;
                                    newNegocio.Priorizado = true;
                                    newNegocio.FechaSugerida = disponible.Fecha.Date;
                                    newNegocios.Add(newNegocio);

                                    negocio.CantidadDeCupos -= disponible.LimiteCupo;
                                    disponible.LimiteCupo = 0;
                                    negocio.Priorizado = false;
                                }

                            }

                        }


                    }
                }
            }
            if (newNegocios.Count > 0)
            {
                negocios.AddRange(newNegocios);
            }
        }

        private void ObtenerNegocios(DateTime hoy, Formula formula, List<Cupo> cupos, List<SugerenciaCupoDto> negocios)
        {

            //negocios disponibles

            var pizarraLista = repositorio.Listar<PrecioPizarra>(x => x.FechaDesde >= formula.FechaDesde && x.FechaHasta <= formula.FechaHasta);


            //if (formula.CentroId == 1)//centro 1 = san lorenso
            //{
            //    Dictionary<int, int> agenteCompraUsados = cupos.Where(x => x.AgenteCompraId != null).GroupBy(x => x.AgenteCompraId.Value).ToDictionary(a => a.Key, a => a.Count());
            //    var agentes = repositorio.Listar<AgenteCompra, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId=x.ComercialId, Destinatario = "", ProveedorId = null, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = 1, AgenteCompraId = x.Id, MaterialId = x.MaterialId, TipoNegocioId = 5, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.Fecha, FechaHasta = x.Fecha, FechaDesdeString = x.Fecha.ToString(), FechaHastaString = x.Fecha.ToString() }, x => x.Fecha >= formula.FechaDesde && x.Fecha <= formula.FechaHasta && x.EstadoId == 5);
            //    foreach (var item in agentes)
            //    {
            //        if (agenteCompraUsados.Any(a => a.Key == item.AgenteCompraId))
            //        {
            //            item.CantidadCupos -= agenteCompraUsados.Where(a => a.Key == item.AgenteCompraId).Single().Value;
            //        }
            //    }
            //    agentes = agentes.Where(a => a.CantidadCupos > 0).ToList();
            //    negocios.AddRange(agentes);

            //    Dictionary<int, int> fasonUsados = cupos.Where(x => x.FasonId != null).GroupBy(x => x.FasonId.Value).ToDictionary(a => a.Key, a => a.Count());
            //    var fasones = repositorio.Listar<Fason, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId = x.ComercialId, Destinatario = x.Fasonero.CUIT, ProveedorId = x.FasoneroId, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = 1, FasonId = x.Id, MaterialId = x.MaterialId, TipoNegocioId = 4, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, FechaDesdeString = x.FechaDesde.ToString(), FechaHastaString = x.FechaHasta.ToString() }, x => ((x.FechaDesde >= formula.FechaDesde && x.FechaDesde <= formula.FechaHasta) || (x.FechaHasta >= formula.FechaDesde && x.FechaHasta <= formula.FechaHasta)) && x.EstadoId == 5);
            //    foreach (var item in fasones)
            //    {
            //        if (fasonUsados.Any(a => a.Key == item.FasonId))
            //        {
            //            item.CantidadCupos -= fasonUsados.Where(a => a.Key == item.FasonId).Single().Value;
            //        }
            //    }
            //    fasones = fasones.Where(a => a.CantidadCupos > 0).ToList();
            //    negocios.AddRange(fasones);
            //    List<ZonaCupo> zonaCupos = repositorio.Listar<ZonaCupo>();
            //    foreach (var item in negocios)
            //    {
            //        var zona = zonaCupos.Where(a => a.Descripcion == item.ZonaDescrip).SingleOrDefault();
            //        if (zona != null)
            //        {
            //            item.ZonaCupoId = zona.Id;
            //        }
            //    }
            //}

            //Dictionary<int, int> fijacionDePrecioContratoUsados = cupos.Where(x => x.FijacionDePrecioContratoId != null).GroupBy(x => x.FijacionDePrecioContratoId.Value).ToDictionary(a => a.Key, a => a.Count());
            //var fijaciones = repositorio.Listar<FijacionDePrecioContrato, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId = x.ComercialId, Destinatario = x.Proveedor.CUIT, ProveedorId = x.ProveedorId, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = x.DestinoId.Value, FijacionDePrecioContratoId = x.FijacionDePrecioContratoId, MaterialId = x.MaterialId.Value, TipoNegocioId = 3, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, FechaDesdeString = x.FechaDesde.ToString(), FechaHastaString = x.FechaHasta.ToString() }, x => x.MaterialId != null && ((x.FechaDesde >= formula.FechaDesde && x.FechaDesde <= formula.FechaHasta) || (x.FechaHasta >= formula.FechaDesde && x.FechaHasta <= formula.FechaHasta)) && x.EstadoId == 5 && x.DestinoId == formula.CentroId);
            //foreach (var item in fijaciones)
            //{
            //    if (fijacionDePrecioContratoUsados.Any(a => a.Key == item.FijacionDePrecioContratoId))
            //    {
            //        item.CantidadCupos -= fijacionDePrecioContratoUsados.Where(a => a.Key == item.FijacionDePrecioContratoId).Single().Value;
            //    }
            //}
            //fijaciones = fijaciones.Where(a => a.CantidadCupos > 0).ToList();
            //negocios.AddRange(fijaciones);
            //logger.Debug("CrearSugerenciaCupo - fijaciones obtenidos: " + fijaciones.Count());


            //foreach (var fijacion in negocios.Where(a => a.TipoNegocioId == 3 && a.Precio == 0).ToList())
            //{
            //    var pizarra = pizarraLista.Where(a => a.MaterialId == fijacion.MaterialId).SingleOrDefault();
            //    if (pizarra == null)
            //    {
            //        pizarra = repositorio.Listar<PrecioPizarra>(a => a.MaterialId == fijacion.MaterialId ).OrderByDescending(a => a.FechaDesde).Take(1).Single();
            //        pizarraLista.Add(pizarra);
            //    }
            //    fijacion.MonedaId = pizarra.MonedaId;
            //    fijacion.Precio = pizarra.Precio;

            //}
            //logger.Debug("CrearSugerenciaCupo - pongo precio pizarra a FijacionDePrecioContrato que no tienen precio");

            Dictionary<int, int> negociosUsados = cupos.Where(x => x.NegocioId != null && x.ComercialId != null).GroupBy(x => x.NegocioId.Value).ToDictionary(a => a.Key, a => a.Count());
            var contratos = repositorio.Listar<Contrato, SugerenciaCupoDto>(x =>
                new SugerenciaCupoDto
                {
                    StandardDeCalidad = x.StandardDeCalidadId.HasValue ? x.StandardDeCalidad.Descripcion : "",
                    ComercialId = x.ComercialId.Value,
                    Destinatario = x.Proveedor.CUIT,
                    ProveedorId = x.ProveedorId,
                    ZonaDescrip = x.GrupoDeCompras.Descripcion,
                    CantidadDeCupos = (int)Math.Ceiling(x.Cantidad / 30000),
                    DestinoId = x.DestinoId.Value,
                    NegocioId = x.Id,
                    MaterialId = x.MaterialId,
                    MonedaId = x.MonedaId,
                    Precio = x.Precio,
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    TipoNegocioId = x.TipoNegocioId,
                    ContratoSAP = x.ContratoSAP
                },
                    x => ((formula.FechaDesde >= x.FechaDesde && formula.FechaHasta < x.FechaHasta) || (formula.FechaHasta <= x.FechaHasta && formula.FechaHasta > x.FechaDesde) || (formula.FechaDesde <= x.FechaDesde && formula.FechaHasta >= x.FechaHasta)) && x.EstadoId == 5 && x.DestinoId == formula.CentroId);
            var zonas = repositorio.Listar<ZonaCupo>();
            foreach (var item in contratos)
            {
                if (negociosUsados.Any(a => a.Key == item.NegocioId))
                {
                    item.CantidadDeCupos -= negociosUsados.Where(a => a.Key == item.NegocioId).Single().Value;
                }
                item.ZonaCupoId = zonas.Where(a => a.Descripcion == item.ZonaDescrip).Select(a => a.Id).SingleOrDefault();
            }
            contratos = contratos.Where(a => a.CantidadDeCupos > 0).ToList();
            negocios.AddRange(contratos);
            logger.Debug("CrearSugerenciaCupo - Contratos obtenidos: " + contratos.Count());

            var warrant = cdWarrant.ConsultarContratoWarrant(formula.FechaDesde, formula.FechaHasta);
            foreach (var c in contratos)
            {
                var item = warrant.Where(x => x.ContratoSAP == c.ContratoSAP).SingleOrDefault();
                if (item != null)
                {
                    c.CDWarrant = true;
                }
            }
            Dictionary<int, int> espacioDinamicoUsados = cupos.Where(x => x.ConfiguracionEspacioDinamicoId != null).GroupBy(x => x.ConfiguracionEspacioDinamicoId.Value).ToDictionary(a => a.Key, a => a.Count());
            TipoNegocio tipoNegocioEspacioDinamico = repositorio.ObtenerPrimero<TipoNegocio>(a => a.Descripcion == "ESPACIO DINAMICO");
            var espacioDinamicoLista = repositorio.Listar<ConfiguracionEspacioDinamico, SugerenciaCupoDto>(x =>
            new SugerenciaCupoDto
            {
                StandardDeCalidad = x.Calidad,
                ComercialId = x.ComercialId,
                Destinatario = x.Proveedor.CUIT,
                ProveedorId = x.ProveedorId,
                ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion,
                ZonaCupoId = null,
                CantidadDeCupos = x.CantidadDeCupo,
                DestinoId = x.CentroId,
                ConfiguracionEspacioDinamicoId = x.Id,
                MaterialId = x.MaterialId,
                MonedaId = "",
                Precio = null,
                FechaDesde = x.Fecha,
                FechaHasta = x.Fecha,
                TipoNegocioId = tipoNegocioEspacioDinamico.TipoNegocioId,
                ContratoSAP = null
            }, x => ((x.Fecha >= formula.FechaDesde && x.Fecha <= formula.FechaHasta) || (x.Fecha >= formula.FechaDesde && x.Fecha <= formula.FechaHasta)) && x.CentroId == formula.CentroId);

            foreach (var item in espacioDinamicoLista)
            {
                if (espacioDinamicoUsados.Any(a => a.Key == item.ConfiguracionEspacioDinamicoId))
                {
                    item.CantidadDeCupos -= espacioDinamicoUsados.Where(a => a.Key == item.ConfiguracionEspacioDinamicoId).Single().Value;
                }
                //if (item.CantidadDeCupos > 0)
                //{
                //    var preciopizarra = pizarraLista.Where(a => a.MaterialId == item.MaterialId && a.FechaDesde >= item.FechaDesde && a.FechaHasta <= item.FechaDesde).SingleOrDefault();
                //    if (preciopizarra == null)
                //    {
                //        preciopizarra = repositorio.Listar<PrecioPizarra>(a => a.MaterialId == item.MaterialId).OrderByDescending(a => a.FechaDesde).Take(1).Single();
                //    }
                //    item.Precio = preciopizarra.Precio;
                //    item.MonedaId = preciopizarra.MonedaId;
                //}
                item.ZonaCupoId = zonas.Where(a => a.Descripcion == item.ZonaDescrip).Select(a => a.Id).SingleOrDefault();
            }
            espacioDinamicoLista = espacioDinamicoLista.Where(a => a.CantidadDeCupos > 0).ToList();
            negocios.AddRange(espacioDinamicoLista);

            logger.Debug("CrearSugerenciaCupo - Espacio Dinamico obtenidos: " + espacioDinamicoLista.Count());
        }


        private void ArmarPuntuaciones(Criterio criterio, Dictionary<string, decimal> puntuaciones, int guiones)
        {
            string guion = new String('-', guiones);
            puntuaciones.Add(guion + criterio.DisplayName, Math.Round(criterio.Puntuacion, 2));
            if (criterio.Hijos != null && criterio.Hijos.Count > 0)
            {
                foreach (var hijo in criterio.Hijos)
                {
                    ArmarPuntuaciones(hijo, puntuaciones, guiones + 1);
                }
            }
        }

        public List<SugerenciaCupoDto> ObtenerSugerenciaCupo(int ComercialId)
        {
            List<SugerenciaCupoDto> resultado = repositorio.Listar<SugerenciaCupo, SugerenciaCupoDto>(a => new SugerenciaCupoDto
            {
                Id = a.Id,
                Precio = a.Precio,
                TipoNegocioId = a.TipoNegocioId,
                TipoNegocioDesc = a.TipoNegocio.Descripcion,
                CentroId = a.CentroId,
                //AgenteCompraId = a.AgenteCompraId,
                NegocioId = a.NegocioId,
                CantidadDeCupos = a.CantidadDeCupos,
                //ContratoId = a.ContratoId,
                DestinoId = 1,
                //FasonId = a.FasonId,
                FechaSugerida = a.FechaSugerida,
                //FijacionDePrecioContratoId = a.FijacionDePrecioContratoId,
                MaterialId = a.MaterialId,
                MaterialDesc = a.Material.Descripcion,
                MonedaId = a.MonedaId == null ? "" : a.MonedaId,
                MonedaDesc = a.MonedaId == null ? "" : a.Moneda.Descripcion,
                PuntuacionTotal = a.Puntuacion,
                ProveedorId = a.ProveedorId,
                ProveedorDesc = a.ProveedorId.HasValue ? a.Proveedor.RazonSocial : "",
                ProveedorCUIT = a.ProveedorId.HasValue ? a.Proveedor.CUIT : "",
                PuntuacionesString = a.Puntuaciones,
                ContratoSAP = a.ContratoSAP,
                ZonaCupoId = a.ZonaCupoId,
                ZonaDescrip = a.ZonaCupo.Descripcion,
                ConfiguracionEspacioDinamicoId = a.ConfiguracionEspacioDinamicoId
            }, a => a.Aceptado == null && a.ComercialId == ComercialId);
            foreach (var item in resultado)
            {
                item.Puntuaciones = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(item.PuntuacionesString);
            }
            return resultado;
        }
        private List<SugerenciaCupo> ObtenerSugerenciaCupoPorProveedor(List<int> proveedor, int materialId)
        {
            var fecha = DateTime.Now.Date;
            List<SugerenciaCupo> resultado = repositorio.Listar<SugerenciaCupo>(a => a.Aceptado == null && a.MaterialId == materialId && proveedor.Contains(a.ProveedorId.Value)
             && a.FechaSugerida >= fecha);

            return resultado;
        }
        public IList<SugerenciaCupoDto> ObtenerSugerenciaCupoAgrupadasPorProveedor(int comercialId, int materialId, string centroId)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            return repositorio.ObtenerConsultaEscalar(new ObtenerSugerenciaAgrupadasPorProveedor(formula.FechaDesde, formula.FechaHasta, comercialId, materialId, centroId));
        }

        public List<CupoResult> AceptarSugerenciaCupo(List<SugerenciaCupoDto> sugerenciasAceptadas)
        {
            List<CupoResult> resultado = new List<CupoResult>();
            //CupoResult validacionDisponibilidad = ValidarDisponibilidad(sugerenciasAceptadas);
            //if (!validacionDisponibilidad.HayError)
            //{
            var ids = sugerenciasAceptadas.Select(b => b.Id).ToArray();
            List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a => ids.Contains(a.Id) && a.Aceptado == null);

            foreach (var sugerencia in sugerencias)
            {
                sugerencia.CantidadDeCupos = sugerenciasAceptadas.Where(a => a.Id == sugerencia.Id).Single().CantidadDeCupos;
                Cupo cupo = new Cupo
                {
                    ProveedorId = sugerencia.ProveedorId.Value,//---Agentecompra no tiene proveedor
                    CentroId = sugerencia.CentroId,
                    MaterialId = sugerencia.MaterialId,
                    FechaIngreso = sugerencia.FechaSugerida,
                    ZonaCupoId = sugerencia.ZonaCupoId.Value,
                    ComercialId = sugerencia.ComercialId,
                    Calidad = sugerencia.StandardDeCalidad,
                    Fason = sugerencia.TipoNegocioId == 4,
                    Destinatario = sugerencia.Destinatario,
                    FechaGeneracion = DateTime.Now,
                    Observaciones = null,//---
                    CupoSap = "",//---
                    FleteProcedencia = false,//---
                    EstadoCupoId = 1,//---
                    CupoStop = null,//---
                    CreacionStop = "",//---
                    ErrorStop = "",//---
                    NegocioId = sugerencia.NegocioId,
                    ConfiguracionEspacioDinamicoId = sugerencia.ConfiguracionEspacioDinamicoId,
                    TipoNegocioId = sugerencia.TipoNegocioId,
                };

                CupoResult result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerencia.CantidadDeCupos, Fecha = sugerencia.FechaSugerida } });

                if (sugerenciasAceptadas.Where(a => a.Id == sugerencia.Id).Single().CantidadFleteProcedencia > 0)
                {
                    cupo.FleteProcedencia = true;
                    CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerenciasAceptadas.Where(a => a.Id == sugerencia.Id).Single().CantidadFleteProcedencia, Fecha = sugerencia.FechaSugerida } });

                    result.Errores.AddRange(result2.Errores);
                    result.ListaCupos.AddRange(result2.ListaCupos);
                }
                if (!result.HayError)
                {
                    sugerencia.Aceptado = true;
                }
                else
                {
                    if (result.ListaCupos.Count > 0)
                    {
                        sugerencia.CantidadDeCupos -= result.ListaCupos.Count;
                    }
                }
                resultado.Add(result);
            }
            repositorio.GuardarCambios();
            //}
            //else
            //{
            //    resultado.Add(validacionDisponibilidad);
            //}
            return resultado;
        }

        public CupoResult AceptarCupoExcedente(int administracionId)
        {
            try
            {
                var sugerencia = repositorio.Obtener<AdministracionCupo>(administracionId);
                CupoResult resultado = new CupoResult();
                Cupo cupo = new Cupo
                {
                    ProveedorId = sugerencia.ProveedorId,//---Agentecompra no tiene proveedor
                    CentroId = sugerencia.CentroId,
                    MaterialId = sugerencia.MaterialId,
                    FechaIngreso = sugerencia.Fecha,
                    ZonaCupoId = sugerencia.ZonaId,
                    ComercialId = sugerencia.ComercialId,
                    Calidad = "",
                    Fason = false,
                    Destinatario = "",
                    FechaGeneracion = DateTime.Now,
                    Observaciones = null,//---
                    CupoSap = "",//---
                    FleteProcedencia = false,//---
                    EstadoCupoId = 1,//---
                    CupoStop = null,//---
                    CreacionStop = "",//---
                    ErrorStop = "",//---
                    NegocioId = 7,
                    ConfiguracionEspacioDinamicoId = null,
                    TipoNegocioId = null,
                };

                CupoResult result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerencia.CantidadCupo, Fecha = sugerencia.Fecha } });

                if (sugerencia.CantidadFleteProcedencia > 0)
                {
                    cupo.FleteProcedencia = true;
                    CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerencia.CantidadFleteProcedencia, Fecha = sugerencia.Fecha } });

                    result.Errores.AddRange(result2.Errores);
                    result.ListaCupos.AddRange(result2.ListaCupos);
                }
                if (!result.HayError)
                {

                    sugerencia.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo;
                }
                else
                {
                    if (result.ListaCupos.Count > 0)
                    {
                        sugerencia.CantidadCupo -= result.ListaCupos.Count;
                    }
                }
                resultado.Errores.AddRange(result.Errores);
                resultado.ListaCupos.AddRange(result.ListaCupos);
                repositorio.GuardarCambios();

                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }


        public List<DateTime> FechasComprendidas()
        {
            var fechas = new List<DateTime>();
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            var fechaInicio = formula.FechaDesde;
            for (var i = 0; i <= formula.CantDias; i++)
            {
                fechas.Add(fechaInicio.AddDays(i));
            }
            return fechas;
        }
        private CupoResult ValidarDisponibilidad(List<SugerenciaCupo> sugerenciasAceptadas)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            List<Cupo> cupos = repositorio.Listar<Cupo>(x => x.FechaIngreso >= formula.FechaDesde && x.FechaIngreso <= formula.FechaHasta && x.CentroId == formula.CentroId);
            List<ConfiguracionCupoDto> disponibilidadEnPlantas = ObtenerDisponibilidadEnPlantas(formula, cupos);
            CupoResult result = new CupoResult();
            foreach (var sugerencia in sugerenciasAceptadas)
            {
                var disponibles = disponibilidadEnPlantas.Where(a => a.MaterialId == sugerencia.MaterialId && a.LimiteCupo > 0 && a.Fecha.Date == sugerencia.FechaSugerida.Date).ToList();
                if (disponibles.Count == 0)
                {
                    result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en planta.");
                    return result;
                }
                foreach (var disponible in disponibles)
                {

                    if (disponible.CantidadCupo.Count() > 0)
                    {
                        var disponiblezona = disponible.CantidadCupo.Where(a => a.ZonaCupo == sugerencia.ZonaCupo.Descripcion).SingleOrDefault();
                        if (disponiblezona != null)
                        {
                            if (disponiblezona.CantidadCupo >= sugerencia.CantidadDeCupos)
                            {
                                disponiblezona.CantidadCupo -= sugerencia.CantidadDeCupos;
                                disponible.LimiteCupo -= sugerencia.CantidadDeCupos;
                            }
                            else
                            {
                                result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en la zona.");
                                return result;
                            }
                        }
                        else
                        {
                            result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en la zona.");
                            return result;
                        }
                    }
                    else
                    {
                        disponible.LimiteCupo -= sugerencia.CantidadDeCupos;
                    }
                }
            }

            return result;
        }

        public CupoResult RechazarSugerenciaCupo(List<int> ids, string motivo)
        {
            var resultado = new CupoResult();
            try
            {
                List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a => ids.Contains(a.Id) && a.Aceptado == null);
                List<Cupo> cupos = new List<Cupo>();
                foreach (var sugerencia in sugerencias)
                {
                    sugerencia.Aceptado = false;
                    sugerencia.MotivoRechazo = motivo;
                }
                repositorio.AgregarTodos(cupos);
                repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                var nuevoResultado = new CupoResult();
                nuevoResultado.Error("eliminar", e.Message);
                return nuevoResultado;
            }

        }

        private void darFormatoAlFiltro(IEnumerable<Filter> filtros)
        {
            filtros.ToList().ForEach(p =>
            {
                if (p.Value == null)
                {
                    darFormatoAlFiltro(p.Filters);
                }
                else
                {
                    if (p.Value.GetType().IsArray)
                    {
                        var unicoValueDelArray = p.Value as object[];
                        p.Value = unicoValueDelArray.First();
                    }
                }
            });
            convertirFechaDe<CupoDto>(filtros);
            convertirBool<CupoDto>(filtros);
        }

        private void convertirFechaDe<TAlgunDto>(IEnumerable<Filter> filtros)
        {
            var propiedadesConFechas = typeof(TAlgunDto).GetProperties().Where(x => x.PropertyType == typeof(DateTime)).ToList();

            var filtrosConFechas = filtros.Where(y => propiedadesConFechas.Any(x => x.Name == y.Field));
            filtrosConFechas.ToList().ForEach(x =>
            {
                x.Value = DateTime.Parse(x.Value.ToString());
            });
        }

        private void convertirBool<TAlgunDto>(IEnumerable<Filter> filtros)
        {

            var propiedadesConBool = typeof(TAlgunDto).GetProperties().Where(x => x.PropertyType == typeof(bool)).ToList();

            var filtrosConFechas = filtros.Where(y => propiedadesConBool.Any(x => x.Name == y.Field));
            filtrosConFechas.ToList().ForEach(x =>
            {

                x.Value = bool.Parse(x.Value.ToString());
            });
        }

        //==============view

        public List<EstadoCupoDto> TraerTodoLosEstados()
        {
            return repositorio.Listar<EstadoCupo, EstadoCupoDto>(x => new EstadoCupoDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Orden = x.Orden,
            });
        }

        public SugerenciaCupoDto ObtenerSugerencia(int sugerenciaId)
        {
            return repositorio.Obtener<SugerenciaCupo, SugerenciaCupoDto>(a => a.Id == sugerenciaId, a => new SugerenciaCupoDto
            {
                Id = a.Id,
                Precio = a.Precio,
                TipoNegocioId = a.TipoNegocioId,
                TipoNegocioDesc = a.TipoNegocio.Descripcion,
                CentroId = a.CentroId,
                NegocioId = a.NegocioId,
                CantidadDeCupos = a.CantidadDeCupos,
                DestinoId = 1,
                FechaSugerida = a.FechaSugerida,
                MaterialId = a.MaterialId,
                MaterialDesc = a.Material.Descripcion,
                MonedaId = a.MonedaId == null ? "" : a.MonedaId,
                MonedaDesc = a.MonedaId == null ? "" : a.Moneda.Descripcion,
                PuntuacionTotal = a.Puntuacion,
                ProveedorId = a.ProveedorId,
                ProveedorDesc = a.ProveedorId.HasValue ? a.Proveedor.RazonSocial : "",
                ProveedorCUIT = a.ProveedorId.HasValue ? a.Proveedor.CUIT : "",
                PuntuacionesString = a.Puntuaciones,
                ContratoSAP = a.ContratoSAP,
                ZonaCupoId = a.ZonaCupoId,
                ZonaDescrip = a.ZonaCupo.Descripcion,
                ConfiguracionEspacioDinamicoId = a.ConfiguracionEspacioDinamicoId
            });
        }

        private CupoResult CrearCupos(DiaCupo detalle, SugerenciaCupo sugerencia, Dictionary<DateTime, int> cuposDevueltos, bool esLaUltimaSugerencia)
        {
            var resultado = new CupoResult();
            int cuposDevueltosParaLaFecha = 0;
            if (esLaUltimaSugerencia)
            {
                cuposDevueltos.TryGetValue(sugerencia.FechaSugerida, out cuposDevueltosParaLaFecha);
            }

            var cantidadFleteProcedencia = detalle.CantidadFleteProcedencia ?? 0;
            cantidadFleteProcedencia = sugerencia.CantidadDeCupos < cantidadFleteProcedencia ? sugerencia.CantidadDeCupos : cantidadFleteProcedencia;
            var cantidadSugerencia = sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha > detalle.CantidadSugerencia
                                        ? detalle.CantidadSugerencia : sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha;

            if (cantidadSugerencia == sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha && cuposDevueltos.ContainsKey(sugerencia.FechaSugerida.Date))
            {
                cuposDevueltos[sugerencia.FechaSugerida] -= cantidadSugerencia - sugerencia.CantidadDeCupos;
            }

            var cupo = new Cupo
            {
                ProveedorId = sugerencia.ProveedorId.Value,//---Agentecompra no tiene proveedor
                CentroId = sugerencia.CentroId,
                MaterialId = sugerencia.MaterialId,
                FechaIngreso = sugerencia.FechaSugerida,
                ZonaCupoId = sugerencia.ZonaCupoId.Value,
                ComercialId = sugerencia.ComercialId,
                Calidad = sugerencia.StandardDeCalidad,
                Fason = sugerencia.TipoNegocioId == 4,
                Destinatario = sugerencia.Destinatario,
                FechaGeneracion = DateTime.Now,
                Observaciones = null,//---
                CupoSap = "",//---
                FleteProcedencia = false,//---
                EstadoCupoId = 1,//---
                CupoStop = null,//---
                CreacionStop = "",//---
                ErrorStop = "",
                NegocioId = sugerencia.NegocioId,
                ConfiguracionEspacioDinamicoId = sugerencia.ConfiguracionEspacioDinamicoId,
                TipoNegocioId = sugerencia.TipoNegocioId,
            };

            logger.Debug($"Cupos {cuposDevueltos} ");
            //si tengo al menos un cupo normal para crear...
            if (cantidadSugerencia - cantidadFleteProcedencia > 0)
            {
                var result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadSugerencia - cantidadFleteProcedencia, Fecha = sugerencia.FechaSugerida } });
                detalle.CantidadSugerencia -= cantidadSugerencia;
                //sugerencia.CantidadDeCupos = cantidadSugerencia;
                resultado.Errores.AddRange(result.Errores);
                resultado.ListaCupos.AddRange(result.ListaCupos);
            }

            //si tengo al menos un cupo flete proc para crear...
            if (cantidadFleteProcedencia > 0)
            {
                cupo.FleteProcedencia = true;
                CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadFleteProcedencia, Fecha = sugerencia.FechaSugerida } });
                detalle.CantidadFleteProcedencia -= cantidadFleteProcedencia;
                resultado.Errores.AddRange(result2.Errores);
                resultado.ListaCupos.AddRange(result2.ListaCupos);
            }
            return resultado;
        }

        public CupoResult ConfirmarSugerencia(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, int materialId, string centroId)
        {
            //datosTabla un elemento por cada proveedor a confirmar
            //datosTabla.Detalle un elemento por cada fecha de cada proveedor

            var sugerenciaTodosLosProveedores = ObtenerSugerenciaCupoPorProveedor(datosTablaPorProveedor.Select(x => x.ProveedorId).ToList(), materialId);

            CupoResult resultado = new CupoResult();

            //Calculo los cupos que va a devolver
            var cuposDevueltos = CalcularCuposDevueltos(datosTablaPorProveedor, sugerenciaTodosLosProveedores);
            logger.Debug($"ConfirmarSugerencia {datosTablaPorProveedor.Count} ");
            var centro = repositorio.Obtener<Centro, int>(x => x.CodigoSap == centroId, x => x.Id);
            foreach (var detalle in datosTablaPorProveedor)
            {
                //Sugerencia para un día del proveedor agrupadas en un solo objeto
                foreach (var fechaProveedor in detalle.Detalles)
                {
                    var comercial = repositorio.Obtener<Comercial>(detalle.ComercialId);
                    var sugerenciasProveedorFecha = sugerenciaTodosLosProveedores.Where(a => a.FechaSugerida == fechaProveedor.Fecha && a.ProveedorId == detalle.ProveedorId).ToList();
                    var totalDeCuposIngresadosEnPantalla = fechaProveedor.CantidadSugerencia;
                    var totalDeCuposEnSugerenciasExistentes = sugerenciasProveedorFecha.Sum(x => x.CantidadDeCupos);
                    //if (sugerenciasProveedorFecha.Count() <= 0)//tendria que generar solicitud en lugar de sugerencia
                    //{

                    //    var nuevaSugerencia = new SugerenciaCupo
                    //    {
                    //        FechaSugerida = fechaProveedor.Fecha,
                    //        CantidadDeCupos = fechaProveedor.CantidadSugerencia,
                    //        ProveedorId = detalle.ProveedorId,
                    //        CentroId = centro,
                    //        MaterialId = materialId,
                    //        ComercialId = detalle.ComercialId,
                    //        ZonaCupoId = repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial.GrupoDeCompras.Descripcion, x => x.Id),
                    //        Puntuaciones = "",
                    //        StandardDeCalidad = "",
                    //        TipoNegocioId = 7
                    //    };
                    //    sugerenciasProveedorFecha.Add(nuevaSugerencia);
                    //    repositorio.Agregar(nuevaSugerencia);
                    //}

                    foreach (var s in sugerenciasProveedorFecha)
                    {

                        s.Aceptado = true;
                        var res = CrearCupos(fechaProveedor, s, cuposDevueltos, s == sugerenciasProveedorFecha.Last());
                        resultado.Errores.AddRange(res.Errores);
                        resultado.ListaCupos.AddRange(res.ListaCupos);
                    }

                    if (totalDeCuposIngresadosEnPantalla - totalDeCuposEnSugerenciasExistentes > 0)
                    {
                        logger.Debug($"CrearSugerencia  {detalle.ProveedorId} {fechaProveedor.Fecha} {fechaProveedor.CantidadSugerencia} ");

                        var autorizacion = new AdministracionCupo()
                        {
                            CantidadCupo = totalDeCuposIngresadosEnPantalla - totalDeCuposEnSugerenciasExistentes,
                            ComercialId = detalle.ComercialId,
                            Fecha = fechaProveedor.Fecha,
                            ProveedorId = detalle.ProveedorId,
                            CantidadFleteProcedencia = fechaProveedor.CantidadFleteProcedencia.Value,
                            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                            MaterialId = materialId,
                            ZonaId = repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial.GrupoDeCompras.Descripcion, x => x.Id),
                            CentroId = centro,
                            Excedente = true
                        };
                        repositorio.Agregar(autorizacion);
                    }
                    if (cuposDevueltos.ContainsKey(fechaProveedor.Fecha.Date) && cuposDevueltos[fechaProveedor.Fecha.Date] != 0)
                    {
                        var autorizacion = new AdministracionCupo()
                        {
                            CantidadCupo = cuposDevueltos[fechaProveedor.Fecha],
                            ComercialId = detalle.ComercialId,
                            Fecha = fechaProveedor.Fecha,
                            ProveedorId = detalle.ProveedorId,
                            CantidadFleteProcedencia = fechaProveedor.CantidadFleteProcedencia.Value,
                            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                            MaterialId = materialId,
                            ZonaId = repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial.GrupoDeCompras.Descripcion, x => x.Id),
                            CentroId = centro,
                            Excedente = false
                        };
                        repositorio.Agregar(autorizacion);
                    }
                }
            }
            repositorio.GuardarCambios();
            return resultado;
        }

        private Dictionary<DateTime, int> CalcularCuposDevueltos(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, List<SugerenciaCupo> sugerenciaTodosLosProveedores)
        {
            var cuposDevueltos = new Dictionary<DateTime, int>();
            foreach (var detalle in datosTablaPorProveedor)
            {
                foreach (var fechaProveedor in detalle.Detalles)
                {
                    var sugerenciasProveedorFecha = sugerenciaTodosLosProveedores.Where(a => a.FechaSugerida == fechaProveedor.Fecha && a.ProveedorId == detalle.ProveedorId);
                    var totalDeCuposIngresadosEnPantalla = fechaProveedor.CantidadSugerencia;
                    var totalDeCuposEnSugerenciasExistentes = sugerenciasProveedorFecha.Sum(x => x.CantidadDeCupos);
                    if (totalDeCuposIngresadosEnPantalla < totalDeCuposEnSugerenciasExistentes)
                    {
                        var cuposDevueltosDelDia = totalDeCuposEnSugerenciasExistentes - totalDeCuposIngresadosEnPantalla;
                        if (cuposDevueltos.ContainsKey(fechaProveedor.Fecha))
                        {
                            cuposDevueltos[fechaProveedor.Fecha] += cuposDevueltosDelDia;
                        }
                        else
                        {
                            cuposDevueltos.Add(fechaProveedor.Fecha, cuposDevueltosDelDia);
                        }
                    }
                }
            }
            return cuposDevueltos;
        }
        public List<DiaCupo> Panel()
        {
            var lista = new List<DiaCupo>();
            try
            {
                Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
                List<ConfiguracionCupo> configuracionCupo = repositorio.Listar<ConfiguracionCupo>(x => x.Fecha >= formula.FechaDesde && x.Fecha <= formula.FechaHasta && x.CentroId == formula.CentroId);
                List<ConfiguracionCupoDto> disponibilidadEnPlanta = configuracionCupo.Select(x => new ConfiguracionCupoDto { CentroId = x.CentroId, LimiteCupo = x.LimiteCupo, MaterialId = x.MaterialId, Fecha = x.Fecha, CantidadCupo = x.CantidadCupo.Select(b => new LimiteCupoDto { ZonaCupo = b.ZonaCupo.Descripcion, ZonaCupoId = b.ZonaCupoId, CantidadCupo = b.CantidadCupo }).ToList() }).ToList();
                var sugerencias = repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida >= formula.FechaDesde && x.FechaSugerida <= formula.FechaHasta && x.CentroId == formula.CentroId && x.Aceptado == true);
                var fechasComprendidas = FechasComprendidas();
                for (var i = 0; i <= formula.CantDias; i++)
                {
                    var fecha = fechasComprendidas[i];
                    var cupo = new DiaCupo()
                    {
                        Fecha = fecha,
                        CantidadCuposDevueltos = repositorio.Existe<AdministracionCupo>(x => x.Fecha == fecha && !x.Excedente) ? (int)repositorio.Sumar<AdministracionCupo>(x => x.CantidadCupo, x => x.Fecha == fecha && !x.Excedente) : 0,
                        CantidadDisponibilidadDia = disponibilidadEnPlanta.Where(x => x.Fecha == fecha).Count() > 0 ? disponibilidadEnPlanta.Where(x => x.Fecha == fecha).Sum(y => y.LimiteCupo) : 0,
                        CantidadSugerenciaPendiente = repositorio.Existe<SugerenciaCupo>(x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == null) ?
                        (int)repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == null) : 0,
                        CantidadSugerenciaAceptadaDia = repositorio.Existe<SugerenciaCupo>(x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == true) ?
                        (int)repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == true) : 0
                    };
                    cupo.CantidadCuposLibres = (cupo.CantidadDisponibilidadDia -cupo.CantidadSugerenciaAceptadaDia - cupo.CantidadSugerenciaPendiente + cupo.CantidadCuposDevueltos);
                    lista.Add(cupo);
                }
            }
            catch (Exception e)
            {
                return lista;
            }
            return lista;
        }
        public List<SugerenciaNoAceptada> SugerenciasNoAceptadas()
        {
            var lista = new List<SugerenciaNoAceptada>();
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
            var sugerenciaComercialDia = repositorio.Listar<SugerenciaCupo>(x => x.FechaSugerida >= formula.FechaDesde && x.FechaSugerida <= formula.FechaHasta && x.CentroId == formula.CentroId && x.Aceptado == null).GroupBy(y => y.Comercial);
            foreach (var sug in sugerenciaComercialDia)
            {
                var dia = new SugerenciaNoAceptada()
                {
                    NombreComercial = sug.Key.Nombres + " " + sug.Key.Apellido,
                    Diacupo = new List<DiaCupo>()
                };
                foreach (var d in sug.GroupBy(x => x.FechaSugerida))
                {
                    dia.Diacupo.Add(new DiaCupo
                    {
                        Fecha = d.Key,
                        Cantidad = d.Sum(x => x.CantidadDeCupos)
                    });
                }
                lista.Add(dia);
            }
            return lista;
        }


        public void EnviarMailSinCtg()
        {
            var cupos = repositorio.Listar<Cupo>(x => x.EstadoCupoId == 1 && x.FechaIngreso == DateTime.Today, 0, "CupoSap").GroupBy(x => new { ProveedorId = x.ProveedorId, ComercialId = x.ComercialId });

            foreach (var p in cupos)
            {
                var lista = new List<string>();
                var comercial = repositorio.Obtener<Comercial>(p.Key.ComercialId);
                var email = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
                var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == p.Key.ProveedorId && x.Cupo == true);
                if (emailproveedor.Count <= 0)
                {
                    continue;
                }
                lista.Add(email);
                mailManager.EnviarMail(emailproveedor,
                   "Estado de cupos", "", lista, CuerpoMailSinCtg(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"),
                    p.ToList(), comercial));

            }
        }

        private AlternateView CuerpoMailSinCtg(String filePath, List<Cupo> cupos, Comercial comercial)
        {
            var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
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
            htmlBody += "En el presente mail, se detalla los cupos sin activar con Molinos Agro S.A: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "Material" + "</td>" +
                    th + "Fecha de Cupo" + "</td>" +
                    th + "Productor/Corredor" + "</td>" +
                    th + "Cupos Generados" + "</td>" +
                    th + "Estado de cupo:" + "</td>" +
                    "</tr>";

            foreach (var c in cupos)
            {
                htmlBody += TrEncabezado(c, ref linea);
            }

            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /><br /> En el caso que sea necesario, comuníquese con  " + comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<DisponibilidadCuposDto> TraerCupoDisponibilidad(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId)
        {
            return disponibilidadCuposAgent.TraerDisponibilidadCupos(fechaDesde, fechaHasta, zonaId, centroId, materialId);
        }
    }
}