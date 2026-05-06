using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
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

        public string Crear(PrecioPizarra precioPizarra)
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
        }

        public string Anular(PrecioPizarra precioPizarra)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return "OK";
            }
            else
            {
                var material = repositorio.Obtener<Material>(precioPizarra.MaterialId);
                var pizarra = repositorio.Obtener<Pizarra>(precioPizarra.PizarraId);


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
        }

        string EjecutarSinPi(ZMprfcPrecioPizarra request)
        {
            try
            {
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
                logger.Error(e, "Error comunicacion SAP");
                throw;
            }
        }

    }
}
