using Molinos.DataAgro.Agent.Compras;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Molinos.DataAgro.Agent
{
    public class ComprasAgent : IComprasAgent
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public ComprasAgent(ILogger logger)
        {
            this.logger = logger;
        }

        public List<CompraAgentDto> Comprar(string CUIT, string UsuarioComercial)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                UsuarioComercial = ConfigurationManager.AppSettings["SapPruebaUser"];
            }
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                var rq = new ZMprfcDatosCompras() { ImCuit = new List<String>() { CUIT }.ToArray(), ImUsuario = UsuarioComercial };

                var devolucion = agent.ZMprfcDatosCompras(rq);
                var compra = new List<CompraAgentDto>();
                if (devolucion.ExCompras != null)
                {
                    foreach (var dev in devolucion.ExCompras)
                    {
                        compra.Add(ConvertirADtoSinPi(dev));
                    }
                }
                return compra;
            }
            else
            {
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
        }

        public List<CompraAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial)
        {

            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                var rq = new ZMprfcDatosCompras() { ImCuit = CUIT.ToArray(), ImUsuario = UsuarioComercial };
                var devolucion = agent.ZMprfcDatosCompras(rq);

                var compra = new List<CompraAgentDto>();
                if (devolucion.ExCompras != null)
                {
                    foreach (var dev in devolucion.ExCompras)
                    {
                        compra.Add(ConvertirADtoSinPi(dev));
                    }
                }
                return compra;
            }
            else
            {
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

        private CompraAgentDto ConvertirADtoSinPi(Zmpes5130 dev)
        {
            var compraAgent = new CompraAgentDto
            {
                VENDEDOR = dev.Vendedor,
                TN_COMPRADAS = dev.TnCompradas,
                MES = dev.Mes,
                ANIO = dev.Anio,
                MATERIAL = dev.Material,
                COSECHA = dev.Cosecha
            };
            return compraAgent;
        }
    }
}
