using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ComprasAgent : IComprasAgent
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public List<CompraAgentDto> Comprar(string CUIT, string UsuarioComercial)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient agent = new Compras.SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS() { IM_CUIT = new List<String>() { CUIT }.ToArray(), IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS(rq);
            var compra = new List<CompraAgentDto>();
            if (devolucion.EX_COMPRAS != null)
            {
                foreach (var dev in devolucion.EX_COMPRAS)
                {
                    compra.Add(ConvertirADto(dev));
                }
            }
            return compra;
        }

        public List<CompraAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial)
        {

            //if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            //{
            //    UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            //}
            SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient agent = new SI_ZMPWS_DATAAGRO_DATOS_COMPRASClient();

            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var rq = new Z_MPRFC_DATOS_COMPRAS() { IM_CUIT = CUIT.ToArray(), IM_USUARIO = UsuarioComercial };

            var devolucion = agent.SI_ZMPWS_DATAAGRO_DATOS_COMPRAS(rq);
            var compra = new List<CompraAgentDto>();
            if (devolucion.EX_COMPRAS != null)
            {
                foreach (var dev in devolucion.EX_COMPRAS)
                {
                    compra.Add(ConvertirADto(dev));
                }
            }
            return compra;
        }

        private CompraAgentDto ConvertirADto(ZMPES5130 dev)
        {
            var compraAgent = new CompraAgentDto
            {
                VENDEDOR = dev.VENDEDOR,
                TN_COMPRADAS = dev.TN_COMPRADAS,
                MES = dev.MES,
                ANIO = dev.ANIO,
                MATERIAL = dev.MATERIAL,
                COSECHA = dev.COSECHA
            };
            return compraAgent;
        }
    }
}
