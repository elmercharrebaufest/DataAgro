using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.PrecioPizarra;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class PrecioPizarraAgent : IPrecioPizarraAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public PrecioPizarraAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string Crear(Entities.Entities.PrecioPizarra precioPizarra)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                var comercial = repositorio.Obtener<Comercial>(precioPizarra.ComercialId);
                var material = repositorio.Obtener<Material>(precioPizarra.MaterialId);
                var pizarra = repositorio.Obtener<Pizarra>(precioPizarra.PizarraId);

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var rq = new ZMprfcPrecioPizarra
                    {
                        ImFechaDesde = precioPizarra.FechaDesde.ToString("yyyy-MM-dd"),
                        ImFechaHasta = precioPizarra.FechaHasta.ToString("yyyy-MM-dd"),
                        ImMatnr = material.Codigo,
                        ImPizarra = pizarra.Codigo,
                        ImPrecio = Convert.ToDecimal(precioPizarra.Precio.ToString()),
                        ImPrecioSpecified = true,
                        ImUdate = DateTime.Now.ToString("yyyy-MM-dd"),
                        ImUnimed = precioPizarra.UnidadMedida,
                        ImUsuario = comercial.IdActiveDirectory,
                        ImUtime = DateTime.Now.ToString("HH:mm:ss"),
                        ImWaers = precioPizarra.MonedaId

                    };
                    return EjecutarSinPi(rq);
                }
                else
                {
                    var rq = new Z_MPRFC_PRECIO_PIZARRA
                    {
                        IM_FECHA_DESDE = precioPizarra.FechaDesde.ToString("yyyy-MM-dd"),
                        IM_FECHA_HASTA = precioPizarra.FechaHasta.ToString("yyyy-MM-dd"),
                        IM_MATNR = material.Codigo,
                        IM_PIZARRA = pizarra.Codigo,
                        IM_PRECIO = precioPizarra.Precio,
                        IM_UDATE = DateTime.Now.ToString("yyyy-MM-dd"),
                        IM_UNIMED = precioPizarra.UnidadMedida,
                        IM_USUARIO = comercial.IdActiveDirectory,
                        IM_UTIME = DateTime.Now.ToString("HH:mm:ss"),
                        IM_WAERS = precioPizarra.MonedaId

                    };
                    return Ejecutar(rq);
                }

            }
        }

        public string Anular(Molinos.DataAgro.Entities.Entities.PrecioPizarra precioPizarra)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                var material = repositorio.Obtener<Material>(precioPizarra.MaterialId);
                var pizarra = repositorio.Obtener<Pizarra>(precioPizarra.PizarraId);

                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var rq = new ZMprfcPrecioPizarra
                    {
                        ImFechaDesde = precioPizarra.FechaDesde.ToString("yyyy-MM-dd"),
                        ImFechaHasta = precioPizarra.FechaHasta.ToString("yyyy-MM-dd"),
                        ImMatnr = material.Codigo,
                        ImPizarra = pizarra.Codigo,
                        ImAnulacion = "X"
                    };
                    return EjecutarSinPi(rq);
                }
                else
                {
                    var rq = new Z_MPRFC_PRECIO_PIZARRA
                    {
                        IM_FECHA_DESDE = precioPizarra.FechaDesde.ToString("yyyy-MM-dd"),
                        IM_FECHA_HASTA = precioPizarra.FechaHasta.ToString("yyyy-MM-dd"),
                        IM_MATNR = material.Codigo,
                        IM_PIZARRA = pizarra.Codigo,
                        IM_ANULACION = "X"
                    };
                    return Ejecutar(rq);
                }


            }
        }

        string Ejecutar(Z_MPRFC_PRECIO_PIZARRA request)
        {
            try
            {
                var agent = new SI_ZMPWS_DATAAGRO_PRECIO_PIZARRAClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var logId = repositorio.Agregar(new Log
                {
                    Fecha = DateTime.Now,
                    Xml = request.ToXml()
                });
                repositorio.GuardarCambios();
                logger.Debug(request.ToXml());

                Z_MPRFC_PRECIO_PIZARRAResponse devolucion = agent.SI_ZMPWS_DATAAGRO_PRECIO_PIZARRA(request);

                logger.Debug(devolucion.ToXml());
                var log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();
                logger.Debug("Guardado en la base");

                if (devolucion.EX_MENSAJE != "OK")
                {
                    throw new Exception(devolucion.EX_MENSAJE);
                }
                logger.Debug("Sin Error");

                return devolucion.EX_MENSAJE;
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw;
            }
        }

        string EjecutarSinPi(ZMprfcPrecioPizarra request)
        {
            try
            {
                logger.Info("SAP sin PI - RFC ZMprfcPrecioPizarra");
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var logId = repositorio.Agregar(new Log
                {
                    Fecha = DateTime.Now,
                    Xml = request.ToXml()
                });
                repositorio.GuardarCambios();
                logger.Debug(request.ToXml());

                ZMprfcPrecioPizarraResponse devolucion = agent.ZMprfcPrecioPizarra(request);
                logger.Info("SAP sin PI - RFC ZMprfcPrecioPizarra");
                logger.Debug(devolucion.ToXml());
                var log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();
                logger.Debug("Guardado en la base");

                if (devolucion.ExMensaje != "OK")
                {
                    throw new Exception(devolucion.ExMensaje);
                }
                logger.Debug("Sin Error");

                return devolucion.ExMensaje;
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw;
            }
        }

    }
}
