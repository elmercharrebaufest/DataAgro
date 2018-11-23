using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarFijacion;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarFijacionAgent
    {
        public FinalizarFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        public string Finalizar(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                return repositorio.Listar<FijacionDePrecioContrato>().Select(x => x.FijacionSAP).Last() + 1.ToString();
            }
            else
            {
                try
                {
                    SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient agent = new SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var rq = new Z_MPRFC_REGISTRAR_FIJACION()
                    {
                        IM_PROVEEDOR = fijacion.Proveedor.CUIT,
                        IM_MATERIAL = fijacion.Material.Codigo,
                        IM_KILOS = fijacion.Cantidad,
                        IM_PRECIO = fijacion.Precio,
                        IM_MONEDA = fijacion.Moneda.MonedaId,
                        IM_CONTRATO = fijacion.ContratoSAP != null ? fijacion.ContratoSAP.ToString() : "",
                        IM_CORREDOR = fijacion.Corredor != null ? fijacion.Corredor.CUIT : ""
                    };

                    logger.Debug(rq.ToXml());
                    var devolucion = agent.SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACION(rq);
                    logger.Debug(devolucion.ToXml());
                    if (devolucion.EX_MENSAJE != null && devolucion.EX_MENSAJE != "")
                    {
                        throw new Exception(devolucion.EX_MENSAJE);
                    }

                    return devolucion.EX_SALIDA;
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
        }
    }
}
