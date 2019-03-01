using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.DatosDelProveedor;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class DatosProveedor
    {
        private readonly ILogger logger;
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

        public DatosProveedor(ILogger logger)
        {
            this.logger = logger;
        }

        public List<ZMPES5140> ObtenerDatosDeProveedor(List<Datos> datos)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                datos.ForEach(x => x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"]);
            }

            try
            {
                var CUIT = new List<String>();
                List<ZMPES5150> valor = new List<ZMPES5150>();

                foreach (var item in datos)
                {
                    ZMPES5150 us = new ZMPES5150();
                    us.USUARIO = item.UsuarioDirectory;
                    valor.Add(us);
                    CUIT.Add(item.CUIT);
                }

                SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient agent = new SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_DATOS_PROVEEDOR() { IM_CUIT = CUIT.ToArray() , IM_USUARIO = valor.ToArray() };
                logger.Debug(rq.ToXml());
                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);
                logger.Debug(valor1.ToXml());
                return valor1.EX_DATOS.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }

        public List<ZMPES5140> ObtenerDatosDeProveedorEstado(List<string> CUIT, List<string> usuarios)
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

                return valor1.EX_DATOS.ToList();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
        }
    }
}
