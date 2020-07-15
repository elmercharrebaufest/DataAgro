using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ComprasDetalle;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class ComprasDetalleAgent : IComprasDetalleAgent
    {
        private readonly ILogger logger;

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        public ComprasDetalleAgent(ILogger logger)
        {
            this.logger = logger;
        }
        public List<CompraDetalleAgentDto> Comprar(string CUIT, string UsuarioComercial)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS_DETALLE() { IM_CUIT = new List<String>() { CUIT }.ToArray(), IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLE(rq);
            var compra = new List<CompraDetalleAgentDto>();
            if (devolucion.EX_SALIDA != null)
            {
                foreach (var dev in devolucion.EX_SALIDA)
                {
                    compra.Add(ConvertirADto(dev));
                }
            }
            return compra;
        }

        public List<CompraDetalleAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial)
        {
            var compra = new List<CompraDetalleAgentDto>();

            try
            {
                //if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
                //{
                //    UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
                //}
                SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLEClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var rq = new Z_MPRFC_DATOS_COMPRAS_DETALLE() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = UsuarioComercial };

                var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS_DETALLE(rq);
                if (devolucion.EX_SALIDA != null)
                {
                    foreach (var dev in devolucion.EX_SALIDA)
                    {
                        compra.Add(ConvertirADto(dev));
                    }
                }
                return compra;
            }
            catch (Exception e)
            {
                logger.Error("Error obtener compras detalle para el cuit " + CUIT + "con el usuario " + UsuarioComercial);
                logger.Error(e);
                return compra;
            }


        }
        private CompraDetalleAgentDto ConvertirADto(ZMPES5620 dev)
        {
            var compraAgent = new CompraDetalleAgentDto();
            compraAgent.CLASE_DOC = dev.CLASE_DOC;
            compraAgent.CLASIFICACION = dev.CLASIFICACION;
            compraAgent.CONTRATO = dev.CONTRATO;
            compraAgent.CORREDOR = dev.CORREDOR;
            compraAgent.COSECHA = dev.COSECHA;
            compraAgent.FECHA = dev.FECHA;
            compraAgent.MATERIAL = dev.MATERIAL;
            compraAgent.PEND_APLICAR = dev.PEND_APLICAR;
            compraAgent.PEND_FIJAR = dev.PEND_FIJAR;
            compraAgent.TN_AMPLIADAS = dev.TN_AMPLIADAS;
            compraAgent.TN_ANULADAS = dev.TN_ANULADAS;
            compraAgent.TN_APLICADAS = dev.TN_APLICADAS;
            compraAgent.TN_CONTRATO = dev.TN_CONTRATO;
            compraAgent.TN_FIJADAS = dev.TN_FIJADAS;
            compraAgent.VENDEDOR = dev.VENDEDOR;
            return compraAgent;
        }
    }
}
