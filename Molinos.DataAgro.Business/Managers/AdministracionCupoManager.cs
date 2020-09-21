using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
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

namespace Molinos.DataAgro.Business.Managers
{
    public class AdministracionCupoManager : IAdministracionCupoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;
        private readonly IMailManager mailManager;
        private readonly IComercialManager comercialManager;
        private readonly IHttpContextManager httpContextManager;

        public AdministracionCupoManager(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager, 
            IMailManager mailManager, IComercialManager comercialManager, IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
            this.mailManager = mailManager;
            this.comercialManager = comercialManager;
            this.httpContextManager = httpContextManager;
        }

        public KendoGrid<AdministracionCupoDto> TraerTodaAdministracionCupo(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerAdministracionCupoExcedente(request));
        }
        public AdministracionCupoDto TraerAdministracionCupo(int id)
        {
            return repositorio.Obtener<AdministracionCupo, AdministracionCupoDto>(x => x.Id == id, x => new AdministracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha
            });
        }
        public CupoResult AceptarCupoExcedente(int administracionId, int cantidad, int cantidadFp, string active)
        {
            try
            {
                var solicitud = repositorio.Obtener<AdministracionCupo>(administracionId);
               
                CupoResult resultado = new CupoResult();
                Cupo cupo = new Cupo
                {
                    ProveedorId = solicitud.ProveedorId,//---Agentecompra no tiene proveedor
                    CentroId = solicitud.CentroId,
                    MaterialId = solicitud.MaterialId,
                    FechaIngreso = solicitud.Fecha,
                    ZonaCupoId = solicitud.ZonaId,
                    ComercialId = solicitud.ComercialId,
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

                CupoResult result = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidad, Fecha = solicitud.Fecha } });

                if (cantidadFp > 0)
                {
                    cupo.FleteProcedencia = true;
                    CupoResult result2 = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadFp, Fecha = solicitud.Fecha } });

                    result.Errores.AddRange(result2.Errores);
                    for (int i = 0; i < result2.ListaCupos.Count; i++)
                    {
                        result2.ListaCupos[i] = "<strong>" + result2.ListaCupos[i] + " *</strong>";
                    }
                    result.ListaCupos.AddRange(result2.ListaCupos);
                   
                }
                if (!result.HayError)
                {
                    solicitud.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo;                   
                    EnviarMailSolicitudAceptada(solicitud, cantidad, cantidadFp, result.ListaCupos, active);
                    solicitud.CantidadCupo = cantidad;
                    solicitud.CantidadFleteProcedencia = cantidadFp;
                }
                else
                {
                    if (result.ListaCupos.Count > 0)
                    {
                        solicitud.CantidadCupo -= result.ListaCupos.Count;                      
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

        private void EnviarMailSolicitudAceptada(AdministracionCupo solicitud, int cantidad, int cantidadFp, List<string> listaCupo, string active)
        {

            var lista = new List<string>();
            var comercial = new List<string>();
            var c = repositorio.Obtener<Comercial>(comercialManager.ComercialAsociado(solicitud.ProveedorId));
            comercial.Add(c.IdActiveDirectory);
            if (comercial.Count <= 0)
            {
                return;
            }
            var administrador = repositorio.Obtener<Comercial>(x=> x.IdActiveDirectory == active);
            //lista.Add(active);
            var alterView = CuerpoMailSolicitudAceptada(httpContextManager.ObtenerPathLogoMail(), solicitud, cantidad, cantidadFp, listaCupo, administrador);
            mailManager.EnviarMail(comercial, "Solicitud de cupos Aceptada", "", lista, alterView);
        }

        private AlternateView CuerpoMailSolicitudAceptada(String filePath, AdministracionCupo solicitud, int cantidad, int cantidadFp, List<string> listaCupo, Comercial comercial)
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
            htmlBody += "En el presente mail, se detalla las solicitud aceptada por Molinos Agro S.A: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA SOLICITUD: </th>" + cupoManager.Td(ref linea) + cupoManager.Split(solicitud.Fecha.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS SIN FLETE: </th>" + cupoManager.Td(ref linea) + cantidad + " de " + solicitud.CantidadCupo + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS CON FLETE: </th>" + cupoManager.Td(ref linea) + cantidadFp + " de " + solicitud.CantidadFleteProcedencia + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + cupoManager.Td(ref linea) + solicitud.Proveedor.RazonSocial.ToUpper() + "</td></tr>";           
            htmlBody += "<tr>" + th + "GRANO: </th>" + cupoManager.Td(ref linea) + solicitud.Material.Descripcion.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS GENERADOS: </th>" + cupoManager.Td(ref linea);

            foreach (var cupo in listaCupo)
            {
                htmlBody += cupo.ToUpper() + "<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "(*)<strong> Cupos con flete procedencia </strong> <br />";
            //"<br /><br /> En el caso que sea necesario, comuníquese con  " + comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public Resultado CambiarEstadoRechazado(int idAdministracion, string active)
        {
            var resultado = new Resultado();
            try
            {
                var administacion = repositorio.Obtener<AdministracionCupo>(idAdministracion);
                administacion.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoRechazadoAdministracionCupo;
                EnviarMailSolicitudRechazo(administacion, active);
                repositorio.GuardarCambios();
                return resultado;

            }catch(Exception e)
            {
                logger.Error(e.Message);
                resultado.Error("", e.Message);
                return resultado;
            }

        }

        private void EnviarMailSolicitudRechazo(AdministracionCupo solicitud, string active)
        {

            var lista = new List<string>();
            var comercial = new List<string>();
            var c = repositorio.Obtener<Comercial>(comercialManager.ComercialAsociado(solicitud.ProveedorId));
            comercial.Add(c.IdActiveDirectory);
            if (comercial.Count <= 0)
            {
                return;
            }
            var administrador = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == active);
            if(administrador == null)
            {
                administrador = new Comercial();
            }
            var alterView = CuerpoMailSolicitudRechazo(httpContextManager.ObtenerPathLogoMail(), solicitud, administrador);
            mailManager.EnviarMail(comercial, "Solicitud de cupos Rechazada", "", lista, alterView);
        }

        private AlternateView CuerpoMailSolicitudRechazo(String filePath, AdministracionCupo solicitud, Comercial comercial)
        {
            var emailAdmin = "";
            if(comercial.IdActiveDirectory != "")
            {
                emailAdmin = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            }            
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
            htmlBody += "En el presente mail, se detalla las solicitud rechazada por Molinos Agro S.A: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA SOLICITUD: </th>" + cupoManager.Td(ref linea) + cupoManager.Split(solicitud.Fecha.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS SIN FLETE: </th>" + cupoManager.Td(ref linea) +  solicitud.CantidadCupo + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS CON FLETE: </th>" + cupoManager.Td(ref linea) +  solicitud.CantidadFleteProcedencia + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + cupoManager.Td(ref linea) + solicitud.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + cupoManager.Td(ref linea) + solicitud.Material.Descripcion.ToUpper() + "</td></tr>";            
            htmlBody += "</table>";
           //"<br /><br /> En el caso que sea necesario, comuníquese con  " + (comercial.IdActiveDirectory != ""? (comercial.Nombres + " " + comercial.Apellido + (emailAdmin != "" && emailAdmin != null ? "(" + emailAdmin + ")." : ".")): "un administrador" )+
              htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void RechazarSolicitudesVencidas()
        {          
            var administacion = repositorio.Listar<AdministracionCupo>(x=> x.Fecha < DateTime.Now && x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo);
            foreach (var item in administacion)
            {
                CambiarEstadoRechazado(item.Id, "");
            }
        }
    }
}
