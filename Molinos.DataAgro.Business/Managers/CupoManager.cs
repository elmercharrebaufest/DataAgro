using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

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

        public CupoManager(IRepositorio repositorio, ILogger logger, ICrearCupoAgent crearCupoAgent,
            IEliminarCupoAgent eliminarCupoAgent, IClienteStopAgent clienteStopAgent, IModificarCupoAgent modificarCupoAgent,
            IProveedorManager proveedorManager,IMailManager mailManager)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.crearCupoAgent = crearCupoAgent;
            this.eliminarCupoAgent = eliminarCupoAgent;
            this.clienteStopAgent = clienteStopAgent;
            this.modificarCupoAgent = modificarCupoAgent;
            this.proveedorManager = proveedorManager;
            this.mailManager = mailManager;
        }
        public CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias)
        {
            var error = new CupoResult { ListaCupos = new List<string>()};
            try
            {
                cupo.Proveedor = repositorio.Obtener<Proveedor>(cupo.ProveedorId);
                cupo.Comercial = repositorio.Obtener<Comercial>(cupo.ComercialId);
                cupo.Material = repositorio.Obtener<Material>(cupo.MaterialId);
                cupo.Centro = repositorio.Obtener<Centro>(cupo.CentroId);
                cupo.ZonaCupo = repositorio.Obtener<ZonaCupo>(cupo.ZonaCupoId);
                if (cupo.Id == 0)
                {
                    foreach (var d in dias)
                    {
                        if (d.Cantidad > 0)
                        {
                            if (d.Fecha<DateTime.Today)
                            {
                                error.Error("CantidadCuposSAP", d.Fecha.ToShortDateString() +": La Fecha de Ingreso no debe ser una fecha menor al día de hoy");
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
        public Resultado Validar(Cupo cupo,int cantidadCupos, DateTime? fechaHasta)
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
            if(cupo.Id == 0 && cantidadCupos == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "La Cantidad no debe estar vacía"));
            }
            if (cupo.MaterialId == 3 && (cupo.Calidad == ""|| cupo.Calidad == null))
            {
                error.Errores.Add(new ErrorMessage(400, "La calidad no debe estar vacia para Soja"));
            }
            if (cupo.Fason == true && (cupo.Destinatario == ""|| cupo.Destinatario == null))
            {
                error.Errores.Add(new ErrorMessage(400, "CUIT Destinatario no debe estar vacío cuando elige Fasón/Préstamo Devolución"));
            }
            if (cupo.FechaIngreso.Date < DateTime.Today && fechaHasta.HasValue && fechaHasta < DateTime.Today)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha de Ingreso no debe ser una fecha menor al día de hoy"));
            }
            if (cupo.Id == 0 && fechaHasta.HasValue && fechaHasta< cupo.FechaIngreso)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha Hasta de entrega no puede ser menor a la Fecha Desde"));
            }

            return error;
        }
        public KendoGrid<CupoDto> TraerCuposTabla(KendoGridMvcRequest request, List<int> equipo)
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
                }
                else
                {
                    nuevoResultado.Error("", $"Error al anular cupo en SAP: {resultado}");
                }
                if (!cupoSap.Centro.Acopio)
                {
                    if (cupoSap.EstadoCupoId == 4 && cupoSap.CupoStop!= null)
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
                nuevoResultado.Error("eliminar",e.Message);
                return nuevoResultado;
            }
        }
        
        public Resultado TransmitirCupos(List<string> cupos)
        {
            var result = new Resultado();
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if(datosConfiguracion.ConexionABMStop.HasValue&& !datosConfiguracion.ConexionABMStop.Value)
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
            return repositorio.Obtener<Cupo, CupoDto>(x => x.Id == id, x=> new CupoDto
            { 
                Id= x.Id,
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
                Proveedor = x.Proveedor.RazonSocial+ " (" + x.Proveedor.CUIT+ ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap
            });
        }
        public Resultado EliminarVarios(List<int> cupos, string comercial)
        {
            var resultado = new Resultado();
            foreach(var c in cupos)
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

        private void EnviarEmail(Cupo cupo,List<string> listaCupos)
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
            
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            foreach (var c in listaCupos)
            {
                htmlBody += "<tr>" + Td(ref linea) + c + "</td></tr>";
            }
            htmlBody += "<tr>" + Td(ref linea) + "Con destino "+ cupo.Centro.Descripcion + "</td></tr>";
            if (cupo.Centro.CodigoSap == "1600" &&(cupo.MaterialId == 1 || cupo.MaterialId == 2 || cupo.MaterialId == 3))
                htmlBody += "<tr>" + Td(ref linea)+"Observaciones: " + (cupo.MaterialId == 1? "Maíz Especial": cupo.MaterialId == 2 ? "Trigo Especial " : "Soja Sustentable")+ "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /><br /> Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + cupo.Comercial.Nombres +" "+ cupo.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
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

    }
}
