using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Molinos.DataAgro.Agent.DatosDelProveedor;
using System.Configuration;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Dto;
using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Agent
{
    public class DatosProveedor
    {
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];

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

                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);

                return valor1.EX_DATOS.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<ZMPES5140> ObtenerDatosDeProveedorEstado(List<String> CUIT, List<String> usuarios)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                //datos.ForEach(x => x.UsuarioDirectory = ConfigurationManager.AppSettings["SapPruebaUser"]);
            }

            try
            {
                //var CUIT = new List<String>();
                List<ZMPES5150> valor = new List<ZMPES5150>();

                foreach (var item in usuarios)
                {
                    ZMPES5150 us = new ZMPES5150();
                    us.USUARIO = item;
                    valor.Add(us);
                }

                SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient agent = new SI_ZMPWS_DATAAGRO_DATOS_PROVEEDORClient();

                agent.ClientCredentials.UserName.UserName = UserSap;

                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_DATOS_PROVEEDOR() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = valor.ToArray() };

                var valor1 = agent.SI_ZMPWS_DATAAGRO_DATOS_PROVEEDOR(rq);

                return valor1.EX_DATOS.ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
