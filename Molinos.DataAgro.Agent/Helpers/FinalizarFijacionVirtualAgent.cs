using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class FinalizarFijacionVirtualAgent : IFinalizarFijacionVirtualAgent
    {
        private readonly IContratosParaFijacionVirtualAgent contratosParaFijacionVirtualAgent;
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public FinalizarFijacionVirtualAgent(ILogger logger, IRepositorio repositorio, IContratosParaFijacionVirtualAgent contratosParaFijacionVirtualAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratosParaFijacionVirtualAgent = contratosParaFijacionVirtualAgent;
        }

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
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var contrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP == fijacion.ContratoSAP);
                    var descuentoGeneralSobrePrecio = contrato.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                    var comision = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                    var precioNeto = fijacion.Precio;
                    if (comision <= 0)
                    {
                        var fijacionComision = fijacion.AperturaPrecio.Count() > 0 ? fijacion.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Porcentaje : 0;
                        if (fijacionComision > 0)
                        {
                            fijacionComision = fijacionComision / 100;
                            precioNeto = precioNeto + (precioNeto * fijacionComision);
                        }
                    }
                    var rq = new ZMprfcInsertarFijVirCanje()
                    {
                        ImAFijar = fijacion.Cantidad.ToString(),
                        ImComercial = fijacion.ComercialCreador.IdActiveDirectory,// se informa el creador
                        ImContrato = fijacion.ContratoSAP,
                        ImFecha = fijacion.Fecha.ToString("yyyy-MM-dd"),
                        ImHora = fijacion.Fecha.ToString("HH:mm:ss"),
                        ImPrecio = comision > 0 ? fijacion.Precio : decimal.Parse(precioNeto.ToString("n2")),
                        ImMoneda = fijacion.MonedaId.TrimEnd(),
                        ImUnime = "KG",
                        ImFechaOperacion = fijacion.FechaOperacion.ToString("yyyy-MM-dd"),
                    };

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };
                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug(rq.ToXml());

                    var valor = agent.ZMprfcInsertarFijVirCanje(rq);
                    logger.Debug(valor.ToXml());
                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += valor.ToXml();
                    repositorio.GuardarCambios();

                    return valor.ExNrofijo;
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
