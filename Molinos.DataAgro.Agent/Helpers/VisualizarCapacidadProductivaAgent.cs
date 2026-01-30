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
    public class VisualizarCapacidadProductivaAgent : IVisualizarCapacidadProductivaAgent
    {
        public VisualizarCapacidadProductivaAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public List<CapacidadProductivaDto> VisualizarCapacidadProductiva(int proveedorID, ProveedorDto proveedorDto, List<Material> materialesList, List<Campaña> campaniasList)
        {
            try
            {
                ProveedorDto proveedor = null;
                List<Material> materiales = null;
                List<Campaña> cosecha = null;

                if (proveedorDto != null && materialesList != null && campaniasList != null)
                {
                    proveedor = proveedorDto;
                    materiales = materialesList;
                    cosecha = campaniasList;
                }
                else
                {
                    proveedor = repositorio.Obtener<Proveedor, ProveedorDto>(x => proveedorID == x.ProveedorId, x => new ProveedorDto { ProveedorId = x.ProveedorId, CUIT = x.CUIT, RazonSocial = x.RazonSocial });
                    materiales = repositorio.Listar<Material>();
                    cosecha = repositorio.Listar<Campaña>();
                }


                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new ZMprfcVisuCapProductiva()
                {
                    ImCuit = proveedor.CUIT
                };

                var response = agent.ZMprfcVisuCapProductiva(rq);
                List<CapacidadProductivaDto> lista = new List<CapacidadProductivaDto>();

                foreach (var item in response.ExSalida)
                {
                    if (materiales.Where(x => x.Codigo == item.Matnr).FirstOrDefault() == null ||
                        cosecha.Where(x => x.Descripcion == item.Cosecha).FirstOrDefault() == null)
                    {
                        logger.Error($"VisualizarCapacidadProductivaAgent - No se pudo agregar para el CUIT {proveedor.CUIT}: {item.ToJson()}");
                    }
                    else
                    {
                        lista.Add(new CapacidadProductivaDto
                        {
                            ProveedorId = proveedorID,
                            MaterialId = materiales.Where(x => x.Codigo == item.Matnr).FirstOrDefault().MaterialId,
                            CampaniaId = cosecha.Where(x => x.Descripcion == item.Cosecha).FirstOrDefault().CampañaId,
                            Cantidad = item.Cantidad,
                            UnidadMedida = item.Unime,
                            Porcentaje = item.Porc,
                            Material = materiales.Where(x => x.Codigo == item.Matnr).FirstOrDefault().Descripcion,
                            Campania = cosecha.Where(x => x.Descripcion == item.Cosecha).FirstOrDefault().Descripcion,
                            FechaActualizacion = !string.IsNullOrEmpty(item.FechaAct) ? DateTime.Parse(item.FechaAct) <= new DateTime(1900, 1, 1) ? (DateTime?)null : DateTime.Parse(item.FechaAct) : null
                        });
                    }
                }
                return lista;


            }
            catch (Exception e)
            {
                logger.Error(e, $"No se pudo obtener la capacidad productiva para el proveedor con ID {proveedorID}. ");
                return new List<CapacidadProductivaDto> { };
            }

        }
    }
}
