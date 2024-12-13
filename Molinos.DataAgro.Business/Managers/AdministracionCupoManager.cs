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
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ICupoManager cupoManager;
        private readonly IMailManager mailManager;
        private readonly IHttpContextManager httpContextManager;

        public AdministracionCupoManager(ILogger logger, IRepositorio repositorio, ICupoManager cupoManager,
            IMailManager mailManager, IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.cupoManager = cupoManager;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
        }

        public KendoGrid<AdministracionCupoDto> TraerTodaAdministracionCupo(KendoGridMvcRequest request, int? comercialId)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerAdministracionCupoExcedente(request, comercialId));
        }

        public AdministracionCupoDto TraerAdministracionCupo(int id)
        {
            return repositorio.Obtener<AdministracionCupo, AdministracionCupoDto>(x => x.Id == id, x => new AdministracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha
            });
        }

        public CupoResult AceptarCupoExcedente(int administracionId, int cantidad, int cantidadFp, int cantidadOriginal, int cantidadFleteOriginal, string active, string motivo)
        {
            try
            {
                bool activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] != null && ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";

                CupoResult resultado = new CupoResult();
                var solicitud = repositorio.Obtener<AdministracionCupo>(administracionId);
                if (solicitud.EstadoId != (int)EnumEstadoAdministracionCupo.Pendiente)
                {
                    resultado.Error("Solicitud", "La solicitud no puede ser editada porque no se encuentra en estado pendiente");
                    return resultado;
                }
                if (solicitud.CantidadCupo != cantidadOriginal || solicitud.CantidadFleteProcedencia != cantidadFleteOriginal)
                {
                    resultado.Error("Solicitud", $"- La solicitud con fecha {solicitud.Fecha.ToString("dd-MM-yyyy")}, proveedor {solicitud.Proveedor.RazonSocial} y destino {solicitud.Centro.Descripcion} no puede ser confirmada/aceptada porque ha sido editada por el usuario.<br /><br />");
                    return resultado;
                }

                if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA ValidarDisponibilidadCupera() ");
                var error = cupoManager.ValidarDisponibilidadCupera(solicitud.MaterialId, solicitud.CentroId, solicitud.Fecha, cantidad + cantidadFp);
                if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA ValidarDisponibilidadCupera() ");

                if (error.HayError)
                {
                    return error;
                }
                if (solicitud.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Algoritmo)
                {
                    //var sugerenciasParaAceptar = cupoManager.SugerenciasParaAceptar(solicitud.ProveedorId.Value, solicitud.ComercialId.Value, solicitud.Centro.CodigoSap, solicitud.MaterialId, null);

                    //var sugerenciaPorComercial = cupoManager.ObtenerSugerenciaPorComercial(solicitud.Fecha, solicitud.ComercialId.Value, solicitud.MaterialId, solicitud.Centro.CodigoSap);
                    var cantidadIngresada = cantidad + cantidadFp;

                    if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA AceptarSugerencia() - Algoritmo ");
                    AceptarSugerencia(solicitud, cantidadIngresada);
                    if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA AceptarSugerencia() - Algoritmo ");

                    //foreach (var s in sugerenciasParaAceptar.Where(x => x.FechaSugerida == solicitud.Fecha))
                    //{
                    //    cantidadIngresada = AceptarSugerencia(solicitud, cantidadIngresada, s);
                    //}

                    //if (cantidadIngresada > 0)
                    //{
                    //    foreach (var s in sugerenciasParaAceptar)
                    //    {
                    //        cantidadIngresada = AceptarSugerencia(solicitud, cantidadIngresada, s);
                    //    }
                    //}
                    CupoResult result = new CupoResult();
                    Cupo cupo = new Cupo
                    {
                        ProveedorId = solicitud.SugerenciaCupo.Negocio != null && solicitud.SugerenciaCupo.Negocio.ProveedorComisionistaId != null ? solicitud.SugerenciaCupo.Negocio.ProveedorComisionistaId.Value : solicitud.ProveedorId.Value,
                        CentroId = solicitud.CentroId,
                        MaterialId = solicitud.MaterialId,
                        FechaIngreso = solicitud.Fecha,
                        ZonaCupoId = solicitud.ZonaId,
                        ComercialId = solicitud.ComercialId,
                        Calidad = solicitud.SugerenciaCupo.StandardDeCalidad,
                        Fason = false,
                        Destinatario = solicitud.SugerenciaCupo.Destinatario,
                        FechaGeneracion = DateTime.Now,
                        Observaciones = null,
                        CupoSap = "",
                        FleteProcedencia = false,
                        EstadoCupoId = 1,
                        CupoStop = null,
                        CreacionStop = "",
                        ErrorStop = "",
                        NegocioId = solicitud.NegocioId ?? solicitud.SugerenciaCupo.NegocioId,
                        ConfiguracionEspacioDinamicoId = solicitud.SugerenciaCupo.ConfiguracionEspacioDinamicoId,
                        TipoNegocioId = solicitud.SugerenciaCupo.TipoNegocioId,
                        ConDescarga = solicitud.ConDescarga,
                        AdministracionCupoId = administracionId,
                        ComercialCreadorId = solicitud.ComercialCreadorId,
                        Sustentable = solicitud.Sustentable,
                        EPA = solicitud.EPA,
                        EUDR = solicitud.EUDR
                    };
                    if (cantidad > 0)
                    {
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA GrabarCupo() - 1 - Algoritmo ");
                        result = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidad, Fecha = solicitud.Fecha } });
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA GrabarCupo() - 1 - Algoritmo ");
                    }


                    if (cantidadFp > 0)
                    {
                        cupo.FleteProcedencia = true;
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA GrabarCupo() - 2 - Algoritmo ");
                        CupoResult result2 = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadFp, Fecha = solicitud.Fecha } });
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA GrabarCupo() - 2 - Algoritmo ");

                        result.Errores.AddRange(result2.Errores);
                        for (int i = 0; i < result2.ListaCupos.Count; i++)
                        {
                            result2.ListaCupos[i] = "<strong>* " + result2.ListaCupos[i] + " *</strong>";
                        }
                        result.ListaCupos.AddRange(result2.ListaCupos);

                    }
                    if (!result.HayError)
                    {
                        solicitud.EstadoId = (int)EnumEstadoAdministracionCupo.Aceptado;
                        solicitud.FechaDecision = DateTime.Now;
                        solicitud.Motivo = motivo;
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA EnviarMailSolicitudAceptada() - 1 - Algoritmo ");
                        EnviarMailSolicitudAceptada(solicitud, cantidad, cantidadFp, result.ListaCupos, active);
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA EnviarMailSolicitudAceptada() - 1 - Algoritmo ");
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
                }
                else
                {
                    Cupo cupo = new Cupo
                    {
                        ProveedorId = solicitud.ProveedorId.Value,//---Agentecompra no tiene proveedor
                        CentroId = solicitud.CentroId,
                        MaterialId = solicitud.MaterialId,
                        FechaIngreso = solicitud.Fecha,
                        ZonaCupoId = solicitud.ZonaId,
                        ComercialId = solicitud.ComercialId,
                        Calidad = solicitud.Calidad,
                        Fason = solicitud.Fason ?? false,
                        Destinatario = solicitud.Destinatario,
                        FechaGeneracion = DateTime.Now,
                        Observaciones = null,
                        CupoSap = "",
                        FleteProcedencia = false,
                        EstadoCupoId = 1,
                        CupoStop = null,
                        CreacionStop = "",
                        ErrorStop = "",
                        NegocioId = solicitud.NegocioId,
                        ConfiguracionEspacioDinamicoId = null,
                        TipoNegocioId = 7,//para que lo envie a SAP como cupo con marca de propuesta y no valide limites en SAP
                        AdministracionCupoId = administracionId,
                        ComercialCreadorId = solicitud.ComercialCreadorId,
                        Sustentable = solicitud.Sustentable,
                        EPA = solicitud.EPA,
                        EUDR = solicitud.EUDR
                    };

                    if (cantidad > 0)
                    {
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA GrabarCupo() - 3 - NO ES Algoritmo ");
                        CupoResult result = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidad, Fecha = solicitud.Fecha } });
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA GrabarCupo() - 3 - NO ES Algoritmo ");
                        if (result.HayError)
                            resultado.Errores.AddRange(result.Errores);
                        if (result.ListaCupos.Count > 0)
                            resultado.ListaCupos.AddRange(result.ListaCupos);
                    }

                    if (cantidadFp > 0)
                    {
                        cupo.FleteProcedencia = true;
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA GrabarCupo() - 4 - NO ES Algoritmo ");
                        CupoResult result = cupoManager.GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadFp, Fecha = solicitud.Fecha } });
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA GrabarCupo() - 4 - NO ES Algoritmo ");
                        if (result.HayError)
                            resultado.Errores.AddRange(result.Errores);
                        if (result.ListaCupos.Count > 0)
                        {
                            for (int i = 0; i < result.ListaCupos.Count; i++)
                            {
                                result.ListaCupos[i] = "<strong>* " + result.ListaCupos[i] + " *</strong>";
                            }
                            resultado.ListaCupos.AddRange(result.ListaCupos);
                        }
                    }

                    if (resultado.ListaCupos.Count > 0)
                        solicitud.Motivo = motivo;
                    if (resultado.ListaCupos.Count > 0)
                    {
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA EnviarMailSolicitudAceptada() - 2 - NO ES Algoritmo ");
                        EnviarMailSolicitudAceptada(solicitud, cantidad, cantidadFp, resultado.ListaCupos, active);
                        if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA EnviarMailSolicitudAceptada() - 2 - NO ES Algoritmo ");
                    }

                    if (!resultado.HayError)
                    {
                        solicitud.EstadoId = (int)EnumEstadoAdministracionCupo.Aceptado;
                        solicitud.FechaDecision = DateTime.Now;
                        repositorio.GuardarCambios();
                    }
                }

                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }

        private void AceptarSugerencia(AdministracionCupo solicitud, int cantidadIngresada)
        {

            if (cantidadIngresada > 0)
            {
                solicitud.SugerenciaCupo.Aceptado = true;
                if (cantidadIngresada < solicitud.SugerenciaCupo.CantidadDeCupos)
                {
                    var nuevaSugerencia = cupoManager.ClonarSugerencia(solicitud.SugerenciaCupo);
                    solicitud.SugerenciaCupo.CantidadDeCupos = cantidadIngresada;
                    nuevaSugerencia.FechaSugerida = solicitud.Fecha;
                    nuevaSugerencia.CantidadDeCupos -= cantidadIngresada;
                    nuevaSugerencia.MonedaId = !String.IsNullOrWhiteSpace(solicitud.SugerenciaCupo.MonedaId) ? solicitud.SugerenciaCupo.MonedaId : null;
                    nuevaSugerencia.Aceptado = null;
                    repositorio.Agregar(nuevaSugerencia);
                }
            }
        }

        private void EnviarMailSolicitudAceptada(AdministracionCupo solicitud, int cantidad, int cantidadFp, List<string> listaCupo, string active)
        {
            try
            {

                var lista = new List<string>();
                var comercial = new List<string>();
                var c = repositorio.Obtener<Comercial, string>(x => x.ComercialId == solicitud.ComercialCreadorId, x => x.IdActiveDirectory);

                comercial.Add(c);
                if (solicitud.Comercial != null)
                {
                    comercial.Add(solicitud.Comercial.IdActiveDirectory);
                }
                else
                {
                    if (solicitud.ComercialId != null)
                    {
                        var comercial1 = repositorio.Obtener<Comercial>(x => x.ComercialId == solicitud.ComercialId);
                        comercial.Add(comercial1.IdActiveDirectory);
                    }
                }

                if (solicitud.ComercialCreador != null)
                {
                    comercial.Add(solicitud.ComercialCreador.IdActiveDirectory);
                }
                else
                {
                    if (solicitud.ComercialCreadorId != null)
                    {
                        var comercial1 = repositorio.Obtener<Comercial>(x => x.ComercialId == solicitud.ComercialCreadorId);
                        comercial.Add(comercial1.IdActiveDirectory);
                    }
                }

                if (comercial.Count <= 0)
                {
                    return;
                }
                var administrador = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == active);
                //lista.Add(active);
                var alterView = CuerpoMailSolicitudAceptada(httpContextManager.ObtenerPathLogoMail(), solicitud, cantidad, cantidadFp, listaCupo, administrador);
                mailManager.EnviarMail(comercial, "Solicitud de cupos Aceptada", "", lista, alterView);

            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailSolicitudAceptada", e.Message);
            }
        }

        private AlternateView CuerpoMailSolicitudAceptada(String filePath, AdministracionCupo solicitud, int cantidad, int cantidadFp, List<string> listaCupo, Comercial comercial)
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
            htmlBody += "En el presente mail se detalla la solicitud de cupos aceptada por Molinos Agro S.A: <br /><br />  ";
            if (!string.IsNullOrEmpty(solicitud.Motivo))
            {
                htmlBody += $"Motivo de confirmación: {solicitud.Motivo} <br /><br />  ";
            }
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA SOLICITUD: </th>" + cupoManager.Td(ref linea) + cupoManager.Split(solicitud.Fecha.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS SIN FLETE: </th>" + cupoManager.Td(ref linea) + cantidad + " de " + solicitud.CantidadCupo + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS CON FLETE: </th>" + cupoManager.Td(ref linea) + cantidadFp + " de " + solicitud.CantidadFleteProcedencia + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + cupoManager.Td(ref linea) + solicitud.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + cupoManager.Td(ref linea) + solicitud.Material.Descripcion.ToUpper() + (solicitud.Sustentable ? " (Sustentable)"
                : solicitud.EPA && solicitud.EUDR ? " (EPA/EUDR)" : solicitud.EPA && !solicitud.EUDR ? " (EPA)" : !solicitud.EPA && solicitud.EUDR ? " (EUDR)" : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS GENERADOS: </th>" + cupoManager.Td(ref linea);

            foreach (var cupo in listaCupo)
            {
                htmlBody += cupo.ToUpper() + "<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            if (solicitud.CantidadFleteProcedencia > 0)
            {
                htmlBody += "(*)<strong> Cupos con flete procedencia </strong> <br />";
            }
            htmlBody += "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public Resultado CambiarEstadoRechazado(int idAdministracion, string motivo, string active)
        {
            var resultado = new Resultado();
            try
            {
                var solicitud = repositorio.Obtener<AdministracionCupo>(idAdministracion);
                solicitud.EstadoId = (int)EnumEstadoAdministracionCupo.Rechazado;
                solicitud.FechaDecision = DateTime.Now;
                solicitud.Motivo = motivo;
                //var sugerenciasParaAceptar = cupoManager.SugerenciasParaAceptar(solicitud.ProveedorId.Value, solicitud.ComercialId.Value, solicitud.Centro.CodigoSap, solicitud.MaterialId, null);

                //foreach (var s in sugerenciasParaAceptar.Where(x => x.FechaSugerida == solicitud.Fecha))
                //{
                //    s.Aceptado = false;
                //}
                repositorio.GuardarCambios();
                EnviarMailSolicitudRechazo(solicitud, active);
                return resultado;

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                resultado.Error("", e.Message);
                return resultado;
            }

        }

        private void EnviarMailSolicitudRechazo(AdministracionCupo solicitud, string active)
        {
            try
            {
                var lista = new List<string>();
                var comercial = new List<string>();
                var c = repositorio.Obtener<Comercial, string>(x => x.ComercialId == solicitud.ComercialCreadorId, x => x.IdActiveDirectory);

                comercial.Add(c);
                if (solicitud.Comercial != null)
                {
                    comercial.Add(solicitud.Comercial.IdActiveDirectory);
                }
                else
                {
                    if (solicitud.ComercialId != null)
                    {
                        var comercial1 = repositorio.Obtener<Comercial>(x => x.ComercialId == solicitud.ComercialId);
                        comercial.Add(comercial1.IdActiveDirectory);
                    }
                }

                if (solicitud.ComercialCreador != null)
                {
                    comercial.Add(solicitud.ComercialCreador.IdActiveDirectory);
                }
                else
                {
                    if (solicitud.ComercialCreadorId != null)
                    {
                        var comercial1 = repositorio.Obtener<Comercial>(x => x.ComercialId == solicitud.ComercialCreadorId);
                        comercial.Add(comercial1.IdActiveDirectory);
                    }
                }

                if (comercial.Count <= 0)
                {
                    return;
                }
                var administrador = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == active);
                var alterView = CuerpoMailSolicitudRechazo(httpContextManager.ObtenerPathLogoMail(), solicitud, administrador);
                mailManager.EnviarMail(comercial, "Solicitud de cupos Rechazada", "", lista, alterView);
            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailSolicitudRechazo", e.Message);
            }
        }

        private AlternateView CuerpoMailSolicitudRechazo(String filePath, AdministracionCupo solicitud, Comercial comercial)
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
            htmlBody += "En el presente mail se detalla la solicitud de cupos rechazada por Molinos Agro S.A: <br /><br /> ";
            if (!string.IsNullOrEmpty(solicitud.Motivo))
            {
                htmlBody += $"Motivo de rechazo: {solicitud.Motivo} <br /><br />  ";
            }
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA SOLICITUD: </th>" + cupoManager.Td(ref linea) + cupoManager.Split(solicitud.Fecha.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS SIN FLETE: </th>" + cupoManager.Td(ref linea) + solicitud.CantidadCupo + "</td></tr>";
            htmlBody += "<tr>" + th + "CUPOS CON FLETE: </th>" + cupoManager.Td(ref linea) + solicitud.CantidadFleteProcedencia + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + cupoManager.Td(ref linea) + solicitud.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + cupoManager.Td(ref linea) + solicitud.Material.Descripcion.ToUpper() + (solicitud.Sustentable ? " (Sustentable)"
                : solicitud.EPA && solicitud.EUDR ? " (EPA/EUDR)" : solicitud.EPA && !solicitud.EUDR ? " (EPA)" : !solicitud.EPA && solicitud.EUDR ? " (EUDR)" : "") + "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /> <br />  Saludos Cordiales," +
              " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
              @"<img src='cid:" + res.ContentId + @"'/>" +
              "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void RechazarSolicitudesVencidas()
        {
            var administacion = repositorio.Listar<AdministracionCupo>(x => x.Fecha < DateTime.Now && x.EstadoId == (int)EnumEstadoAdministracionCupo.Pendiente);
            foreach (var item in administacion)
            {
                CambiarEstadoRechazado(item.Id, "", "");
            }
        }

        public string ActualizarSolicitud(int id, int cantidadCupo, int cantidadFlete, bool estado)
        {
            var resultado = "Error";
            try
            {
                var solicitud = repositorio.Obtener<AdministracionCupo>(id);
                if (solicitud != null && solicitud.EstadoId == (int)EnumEstadoAdministracionCupo.Pendiente)
                {
                    if (!estado)
                    {
                        solicitud.CantidadCupo = cantidadCupo;
                        solicitud.CantidadFleteProcedencia = cantidadFlete;
                    }

                    solicitud.EstadoId = estado ? (int)EnumEstadoAdministracionCupo.Anulado : solicitud.EstadoId;
                    repositorio.GuardarCambios();
                    resultado = "Ok";
                }

            }
            catch (Exception e)
            {
                logger.Error("Error al actualizar la solicitud", e.Message);
            }
            return resultado;
        }

        public CupoResult AceptarCupoExcedenteMasivo(List<AdministracionCupoDto> solicitudes, string motivo, string IdActiveDirectory)
        {
            bool activarLogDebug = false;
            if (ConfigurationManager.AppSettings["ActivarLogDebug"] != null)
            {
                activarLogDebug = ConfigurationManager.AppSettings["ActivarLogDebug"] == "1";
            }

            CupoResult resultados = new CupoResult();

            List<AdministracionCupoDto> keys = new List<AdministracionCupoDto>();
            foreach (var adm in solicitudes.GroupBy(a => new { a.Fecha, a.MaterialId, a.CentroId }))
            {
                int cantidad = adm.Sum(a => a.CantidadDeCupo + a.CantidadFleteProcedencia);

                if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA ValidarDisponibilidadCupera() ");
                var error = cupoManager.ValidarDisponibilidadCupera(adm.Key.MaterialId, adm.Key.CentroId, adm.Key.Fecha, cantidad);
                if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA ValidarDisponibilidadCupera() ");

                if (error.HayError)
                {
                    keys.Add(new AdministracionCupoDto { MaterialId = adm.Key.MaterialId, CentroId = adm.Key.CentroId, Fecha = adm.Key.Fecha });
                    resultados.Errores.AddRange(error.Errores);
                }
            }


            foreach (var adm in solicitudes)
            {
                if (keys.Any(a => a.Fecha == adm.Fecha && a.MaterialId == adm.MaterialId && a.CentroId == adm.CentroId))
                {
                    continue;
                }

                if (activarLogDebug) logger.Debug(DateTime.Now + " - INICIA AceptarCupoExcedente() ");
                var resultado = AceptarCupoExcedente(adm.Id, adm.CantidadDeCupo, adm.CantidadFleteProcedencia, adm.CantidadDeCupoOriginal, adm.CantidadFleteProcedenciaOriginal, IdActiveDirectory, motivo);
                if (activarLogDebug) logger.Debug(DateTime.Now + " - FINALIZA AceptarCupoExcedente() ");

                if (resultado.HayError)
                    resultados.Errores.AddRange(resultado.Errores);
            }

            return resultados;
        }
    }
}
