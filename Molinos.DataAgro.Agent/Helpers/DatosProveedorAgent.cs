using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelProveedor;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
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
    public class DatosProveedorAgent : IDatosProveedorAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatosProveedorAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public List<DatosProveedorAgentDto> ObtenerDatosDeProveedor(List<Datos> datos)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                datos.ForEach(x => x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"]);
            }

            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    logger.Info("SAP sin PI - RFC ZMprfcDatosProveedor");
                    var CUIT = new List<String>();
                    List<Zmpes5150> valor = new List<Zmpes5150>();
                    var users = new List<string>();
                    foreach (var item in datos)
                    {
                        if (!users.Contains(item.UsuarioDirectory))
                        {
                            users.Add(item.UsuarioDirectory);
                        }
                        CUIT.Add(item.CUIT);
                    }
                    foreach (var user in users)
                    {
                        Zmpes5150 us = new Zmpes5150() { Usuario = user };
                        valor.Add(us);
                    }
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    valor = valor.Distinct().ToList();
                    var rq = new ZMprfcDatosProveedor() { ImCuit = CUIT.ToArray(), ImUsuario = valor.ToArray() };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now.Date,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var valor1 = agent.ZMprfcDatosProveedor(rq);
                    logger.Info("SAP sin PI - RFC ZMprfcDatosProveedor");
                    logger.Debug(valor1.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor1.ToXml();
                    repositorio.GuardarCambios();

                    var respuesta = new List<DatosProveedorAgentDto>();
                    if (valor1.ExDatos != null)
                    {
                        respuesta = valor1.ExDatos.Select(x => new DatosProveedorAgentDto
                        {
                            CLIENTE_MOA = x.ClienteMoa,
                            CUIT = x.Cuit,
                            STATUS = x.Status,
                            USUARIO = x.Usuario
                        }).ToList();
                    }
                    return respuesta;

                }
                else
                {
                    var CUIT = new List<String>();
                    List<ZMPES5150> valor = new List<ZMPES5150>();
                    var users = new List<string>();
                    foreach (var item in datos)
                    {
                        if (!users.Contains(item.UsuarioDirectory))
                        {
                            users.Add(item.UsuarioDirectory);
                        }
                        CUIT.Add(item.CUIT);
                    }
                    foreach (var user in users)
                    {
                        ZMPES5150 us = new ZMPES5150() { USUARIO = user };
                        valor.Add(us);
                    }
                    SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient agent = new SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;
                    valor = valor.Distinct().ToList();
                    var rq = new Z_MPRFC_DATOS_PROVEEDOR() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = valor.ToArray() };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now.Date,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);
                    logger.Debug(valor1.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor1.ToXml();
                    repositorio.GuardarCambios();

                    var respuesta = new List<DatosProveedorAgentDto>();
                    if (valor1.EX_DATOS != null)
                    {
                        respuesta = valor1.EX_DATOS.Select(x => new DatosProveedorAgentDto
                        {
                            CLIENTE_MOA = x.CLIENTE_MOA,
                            CUIT = x.CUIT,
                            STATUS = x.STATUS,
                            USUARIO = x.USUARIO
                        }).ToList();
                    }
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public List<DatosProveedorAgentDto> ObtenerDatosDeProveedorEstado(List<string> CUIT, List<string> usuarios)
        {
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    var valor = new List<Zmpes5150>();

                    foreach (var item in usuarios)
                    {
                        valor.Add(new Zmpes5150
                        {
                            Usuario = item
                        });
                    }

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new ZMprfcDatosProveedor()
                    {
                        ImCuit = CUIT.ToArray(),
                        ImUsuario = valor.ToArray()
                    };
                    //logger.Debug(rq.ToXml());
                    var valor1 = agent.ZMprfcDatosProveedor(rq);

                    var respuesta = new List<DatosProveedorAgentDto>();
                    if (valor1.ExDatos != null)
                    {
                        respuesta = valor1.ExDatos.Select(x => new DatosProveedorAgentDto
                        {
                            CLIENTE_MOA = x.ClienteMoa,
                            CUIT = x.Cuit,
                            STATUS = x.Status,
                            USUARIO = x.Usuario
                        }).ToList();
                    }
                    return respuesta;
                }
                else
                {
                    var valor = new List<ZMPES5150>();

                    foreach (var item in usuarios)
                    {
                        valor.Add(new ZMPES5150
                        {
                            USUARIO = item
                        });
                    }

                    SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient agent = new SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;

                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_DATOS_PROVEEDOR() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = valor.ToArray() };
                    //logger.Debug(rq.ToXml());
                    var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);

                    var respuesta = new List<DatosProveedorAgentDto>();
                    if (valor1.EX_DATOS != null)
                    {
                        respuesta = valor1.EX_DATOS.Select(x => new DatosProveedorAgentDto
                        {
                            CLIENTE_MOA = x.CLIENTE_MOA,
                            CUIT = x.CUIT,
                            STATUS = x.STATUS,
                            USUARIO = x.USUARIO
                        }).ToList();
                    }
                    return respuesta;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
