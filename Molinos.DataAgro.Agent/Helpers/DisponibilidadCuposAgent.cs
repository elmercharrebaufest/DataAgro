using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DisponibilidadCupos;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DisponibilidadCuposAgent : IDisponibilidadCuposAgent
    {
        private readonly ILogger logger;
        public DisponibilidadCuposAgent(ILogger logger)
        {
            this.logger = logger;
        }

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public List<DisponibilidadCuposDto> TraerDisponibilidadCupos(DateTime? fecha, string zonaId, string centroId, string materialId)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<DisponibilidadCuposDto>() { new DisponibilidadCuposDto { Consumidos = "1", Fecha = DateTime.Now.Date, Disponibles = "9", Limite = "10", MaterialCodigo = "000000000019908017", MaterialId = 3, MaterialNombre = "Soja", ZonaId = "CBA" } };
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_DISPONIBILIDAD_CUPOSClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_DISPONIBILIDAD_CUPOS() { IM_FECHA = DateTime.Now.Date.ToString("yyyy-MM-dd"), IM_CENTRO = centroId, IM_MATERIAL = materialId, IM_ZONA = zonaId };
                    logger.Debug(rq.ToXml());

                    Z_MPRFC_DISPONIBILIDAD_CUPOSResponse devolucion = agent.SI_ZMPWS_DATAAGRO_DISPONIBILIDAD_CUPOS(rq);
                    var result = new List<DisponibilidadCuposDto>();
                    foreach (var item in devolucion.EX_SALIDA)
                    {
                        result.Add(new DisponibilidadCuposDto
                        {
                            Fecha = DateTime.Parse(item.FECHA),
                            MaterialCodigo = item.MATERIAL,
                            ZonaId = item.ZONA,
                            Disponibles = item.DISPONIBLES,
                            Consumidos = item.CONSUMIDOS,
                            Limite = item.LIMITE_CUPOS
                        });
                    }
                    logger.Debug(devolucion.ToXml());
                    return result;
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    return new List<DisponibilidadCuposDto>();
                }
            }
        }
    }
}
