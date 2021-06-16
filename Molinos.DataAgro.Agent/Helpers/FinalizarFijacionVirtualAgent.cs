using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.InsertarFijacionesVirtuales;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class FinalizarFijacionVirtualAgent : IFinalizarFijacionVirtualAgent
    {
        public FinalizarFijacionVirtualAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        string UserSap = ConfigurationManager.AppSettings["SapUser"];
        string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public string FinalizarFijacionVirtual(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {               
                var numeroSAP = "05";                
                return (int.Parse(numeroSAP) + 1).ToString();
            }
            else
            {
                try
                {
                    var agent = new SI_ZMPWS_DATAAGRO_INSERTAR_FIJ_VIR_CANJEClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;


                    var rq = new Z_MPRFC_INSERTAR_FIJ_VIR_CANJE()
                    {
                        IM_A_FIJAR = fijacion.Cantidad.ToString(),
                        IM_COMERCIAL = fijacion.Comercial.IdActiveDirectory,
                        IM_CONTRATO = fijacion.ContratoSAP,
                        IM_FECHA = fijacion.Fecha.ToString("yyyy-MM-dd"),
                        IM_HORA = fijacion.Fecha.ToString("HH:mm:ss"),
                        IM_PRECIO = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? fijacion.Precio : 0 : 0,
                        IM_MONEDA = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? fijacion.MonedaId.TrimEnd() : "" : "",
                        IM_UNIME = "KG",
                        IM_FECHA_OPERACION = fijacion.FechaOperacion.ToString("yyyy-MM-dd"),
                        
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.SI_ZMPWS_DATAAGRO_INSERTAR_FIJ_VIR_CANJE(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return valor.EX_NROFIJO;
                }
                catch (Exception e)
                {
                    logger.Error(e.Message);
                    throw;
                }
            }
        }

    }
}
