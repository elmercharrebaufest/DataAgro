using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DisponibilidadCupos;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DisponibilidadCuposAgent : IDisponibilidadCuposAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public DisponibilidadCuposAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
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
                    fecha = fecha == null ? DateTime.Now.Date : fecha.Value.Date;

                    var rq = new Z_MPRFC_DISPONIBILIDAD_CUPOS() { IM_FECHA = fecha.Value.ToString("yyyy-MM-dd"), IM_CENTRO = centroId, IM_MATERIAL = materialId, IM_ZONA = zonaId };
                    logger.Debug(rq.ToXml());
                    Z_MPRFC_DISPONIBILIDAD_CUPOSResponse devolucion = agent.SI_ZMPWS_DATAAGRO_DISPONIBILIDAD_CUPOS(rq);
                    var result = new List<DisponibilidadCuposDto>();
                    List<Material> materiales = repositorio.Listar<Material>();
                    foreach (var item in devolucion.EX_SALIDA)
                    {
                        result.Add(new DisponibilidadCuposDto
                        {
                            Fecha = DateTime.Parse(item.FECHA),
                            MaterialCodigo = item.MATERIAL,
                            MaterialNombre = materiales.FirstOrDefault(a => a.Codigo == item.MATERIAL) == null ? item.MATERIAL : materiales.FirstOrDefault(a => a.Codigo == item.MATERIAL).Descripcion,
                            ZonaId = item.ZONA,
                            Disponibles = item.DISPONIBLES.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.DISPONIBLES.TrimStart(new Char[] { '0' }),
                            Consumidos = item.CONSUMIDOS.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.CONSUMIDOS.TrimStart(new Char[] { '0' }),
                            Limite = item.LIMITE_CUPOS.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.LIMITE_CUPOS.TrimStart(new Char[] { '0' })
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
