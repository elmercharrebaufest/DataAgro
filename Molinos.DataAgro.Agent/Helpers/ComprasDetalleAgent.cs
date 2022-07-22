using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ComprasDetalle;
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
    public class ComprasDetalleAgent : IComprasDetalleAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        public ComprasDetalleAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
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
                //var log = new Log
                //{
                //    Fecha = DateTime.Now,
                //    Xml = rq.ToXml()
                //};
                //var logId = repositorio.Agregar(log);
                //repositorio.GuardarCambios();
                //logger.Debug(rq.ToXml());
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
                logger.Error("Error obtener compras detalle con el usuario " + UsuarioComercial);
                logger.Error(e);
                return compra;
            }


        }
        private CompraDetalleAgentDto ConvertirADto(ZMPES5620 dev)
        {
            var compraAgent = new CompraDetalleAgentDto
            {               
                COSECHA = dev.COSECHA,
                MATERIAL = dev.MATERIAL,
                VENDEDOR = dev.VENDEDOR,

                CLASE_DOC = dev.CLASE_DOC,
                CLASIFICACION = dev.CLASIFICACION,
                CONTRATO = dev.CONTRATO,
                CORREDOR = dev.CORREDOR,
                FECHA = dev.FECHA,               
                PEND_APLICAR = dev.PEND_APLICAR,
                PEND_FIJAR = dev.PEND_FIJAR,
                TN_AMPLIADAS = dev.TN_AMPLIADAS,
                TN_ANULADAS = dev.TN_ANULADAS,
                TN_APLICADAS = dev.TN_APLICADAS,
                TN_CONTRATO = dev.TN_CONTRATO,
                TN_FIJADAS = dev.TN_FIJADAS,
               FECHA_HASTA = dev.FECHA_HASTA,
               FECHA_DESDE = dev.FECHA_DESDE,
                
            };
            return compraAgent;
        }
    }
}
