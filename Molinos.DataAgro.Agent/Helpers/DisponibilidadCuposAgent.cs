using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
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
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DisponibilidadCuposAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<DisponibilidadCuposDto> TraerDisponibilidadCupos(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return new List<DisponibilidadCuposDto>() { new DisponibilidadCuposDto { Consumidos = 1, Fecha = DateTime.Now.Date, Disponibles = 9, Limite = 10, MaterialCodigo = "000000000019908017", MaterialId = 3, MaterialNombre = "Soja", ZonaId = "CBA", CentroCodigo = "1029", CentroNombre = "San Lorenzo" } };
            }
            else
            {
                try
                {
                    fechaDesde = fechaDesde == null ? DateTime.Now.Date : fechaDesde.Value.Date;
                    fechaHasta = fechaHasta == null ? DateTime.Now.Date : fechaHasta.Value.Date;
                    DateTime fecha = fechaDesde.Value;
                    var result = new List<DisponibilidadCuposDto>();
                    List<Material> materiales = repositorio.Listar<Material>();
                    List<ZonaCupo> zonas = repositorio.Listar<ZonaCupo>();
                    List<Centro> centros = repositorio.Listar<Centro>();
                    centros = centros.Where(a => a.NoPropio == false).ToList();
                    if (centroId != null && centroId.Count == 0)
                    {
                        centroId.AddRange(centros.Select(x => x.CodigoSap).ToList());
                    }

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    while (fecha <= fechaHasta.Value)
                    {
                        foreach (var centro in centroId)
                        {
                            var rq = new ZMprfcDisponibilidadCupos() { ImFecha = fecha.ToString("yyyy-MM-dd"), ImCentro = centro, ImMaterial = materialId, ImZona = zonaId };
                            logger.Debug(rq.ToXml());
                            ZMprfcDisponibilidadCuposResponse devolucion = agent.ZMprfcDisponibilidadCupos(rq);
                            foreach (var item in devolucion.ExSalida)
                            {
                                result.Add(new DisponibilidadCuposDto
                                {
                                    Fecha = DateTime.Parse(item.Fecha),
                                    MaterialCodigo = item.Material,
                                    MaterialNombre = materiales.FirstOrDefault(a => a.Codigo == item.Material) == null ? item.Material : materiales.FirstOrDefault(a => a.Codigo == item.Material).Descripcion,
                                    ZonaId = item.Zona,
                                    ZonaNombre = zonas.FirstOrDefault(a => a.CodigoSap == item.Zona) == null ? item.Zona : zonas.FirstOrDefault(a => a.CodigoSap == item.Zona).Descripcion,
                                    Disponibles = int.Parse(item.Disponibles.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.Disponibles.TrimStart(new Char[] { '0' })),
                                    Consumidos = int.Parse(item.Consumidos.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.Consumidos.TrimStart(new Char[] { '0' })),
                                    Limite = int.Parse(item.LimiteCupos.TrimStart(new Char[] { '0' }).Length == 0 ? "0" : item.LimiteCupos.TrimStart(new Char[] { '0' })),
                                    CentroCodigo = centro,
                                    CentroNombre = centros.FirstOrDefault(a => a.CodigoSap == centro) == null ? centro : centros.FirstOrDefault(a => a.CodigoSap == centro).Descripcion,

                                });
                            }
                        }

                        fecha = fecha.AddDays(1);
                    }
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
