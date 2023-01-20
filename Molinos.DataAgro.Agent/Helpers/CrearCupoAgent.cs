using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.CrearCupo;
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
    public class CrearCupoAgent : ICrearCupoAgent
    {
        public CrearCupoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        public List<string> Crear(Cupo cupo, int cantidadCupos)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var listaCupos = new List<string>();
                var random = new Random();

                for (var i = 0; i < cantidadCupos; i++)
                {
                    int randomNumber = random.Next(0, 1000);
                    var n = randomNumber.ToString().PadLeft(4, '0');
                    listaCupos.Add("MOL" + n + "/" + cupo.FechaIngreso.ToString("ddMMyyyy"));
                }
                return listaCupos;
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_CREAR_CUPOSClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    var corredor = repositorio.Existe<CorredorProveedor>(x => x.CorredorId == cupo.ProveedorId) ? "C" : "00";
                    var rq = new Z_MPRFC_CREAR_CUPOS
                    {
                        IM_CANTIDAD_CUPOS = cantidadCupos.ToString(),
                        IM_COMERCIAL = cupo.Comercial.IdActiveDirectory,
                        IM_DATAAGRO = "X",
                        IM_CUPO = new ZMPES5500()
                        {
                            FECHA_INGRESO = cupo.FechaIngreso.ToString("yyyy-MM-dd"),
                            MATNR = cupo.Material.Codigo,
                            PROVEEDOR = corredor + cupo.Proveedor.CUIT.Remove(cupo.Proveedor.CUIT.Length - 1).Remove(0, 2),
                            DESCPROV = cupo.Proveedor.RazonSocial.Length > 35 ? cupo.Proveedor.RazonSocial.Substring(0, 35) : cupo.Proveedor.RazonSocial,
                            PLANTA = cupo.Centro.CodigoSap == "1029" && cupo.Sustentable == true ? "1600" : cupo.Centro.CodigoSap,
                            ZONA = cupo.ZonaCupo.CodigoSap,
                            OBSERVACIONES = cupo.Observaciones,
                            DESTINATARIO = cupo.Destinatario,
                            FLETE_PROC = cupo.FleteProcedencia == true ? "S" : "N",
                            CALIDAD = cupo.Calidad == "Camara" ? "01" : cupo.Calidad == "Fabrica" ? "03" : ""
                        },
                        IM_PROPUESTA = cupo.TipoNegocioId != null || cupo.NegocioId != null || cupo.ConfiguracionEspacioDinamicoId != null ? "X" : "",
                        IM_EXCEPCION = cupo.Proveedor.CuposConRiesgo == true ? "X" : ""
                    };

                    var logId = repositorio.Agregar(new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    });
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_CREAR_CUPOS(rq);

                    logger.Debug(devolucion.ToXml());
                    var log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    logger.Debug("Guardado en la base");

                    if (devolucion.EX_MENSAJE_ERROR != "")
                    {
                        throw new Exception(devolucion.EX_MENSAJE_ERROR);
                    }
                    logger.Debug("Sin Error");

                    return devolucion.EX_N_CUPO.Select(x => x.CODIGO_CUPO).ToList();
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
        }
    }
}
