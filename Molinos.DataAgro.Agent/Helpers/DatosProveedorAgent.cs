using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelProveedor;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DatosProveedorAgent : IDatosProveedorAgent
    {
        private readonly ILogger logger;
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatosProveedorAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public List<DatosProveedorAgentDto> ObtenerDatosDeProveedor(List<Datos> datos)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                datos.ForEach(x => x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"]);
            }

            try
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
                foreach(var user in users)
                {
                    ZMPES5150 us = new ZMPES5150();
                    us.USUARIO = user;
                    valor.Add(us);
                }
                SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient agent = new SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;
                valor = valor.Distinct().ToList();
                var rq = new Z_MPRFC_DATOS_PROVEEDOR() { IM_CUIT = CUIT.ToArray() , IM_USUARIO = valor.ToArray() };
                logger.Debug(rq.ToXml());
                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);
                logger.Debug(valor1.ToXml());

                var respuesta = new List<DatosProveedorAgentDto>();
                if (valor1.EX_DATOS != null)
                {
                    foreach(var val in valor1.EX_DATOS)
                    {
                        respuesta.Add(ConvertirADto(val));
                    }
                }
                return respuesta;
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
                    foreach (var val in valor1.EX_DATOS)
                    {
                        respuesta.Add(ConvertirADto(val));
                    }
                }
                return respuesta;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
        private DatosProveedorAgentDto ConvertirADto(ZMPES5140 dev)
        {
            var datos = new DatosProveedorAgentDto();
            datos.CLIENTE_MOA = dev.CLIENTE_MOA;
            datos.CUIT = dev.CUIT;
            datos.STATUS = dev.STATUS;
            datos.USUARIO = dev.USUARIO;
            return datos;
        }
    }
}
