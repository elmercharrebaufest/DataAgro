using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.PrecioPizarra;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class PrecioPizarraAgent : IPrecioPizarraAgent
    {
        public PrecioPizarraAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        public string Crear(Molinos.DataAgro.Entities.Entities.PrecioPizarra precioPizarra)
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
                throw e;
            }

        }
    }
}
