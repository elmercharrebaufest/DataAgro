using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class AnularFijacionAgent : IAnularFijacionAgent
    {
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public AnularFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public string AnularFijacion(FijacionDePrecioContrato fijacion)
        {
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var numeroSAP = repositorio.Listar<FijacionDePrecioContrato, string>(x => x.FijacionSAP, x => x.FijacionSAP != null && x.ContratoSAP == fijacion.ContratoSAP).LastOrDefault();
                if (string.IsNullOrEmpty(numeroSAP))
                {
                    numeroSAP = fijacion.ContratoSAP + "00";
                }
                return (int.Parse(numeroSAP) + 1).ToString();
            }
            else
            {
                try
                {
                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaApertura = new List<Zmpes5440>();
                    var conceptosCargados = new List<int>() { (int)EnumConceptoApertura.Financiero };

                    decimal precioApertura = 0;
                    var fechaDolarizadoString = fijacion.FechaDolarizado?.ToString("yyyy-MM-dd");
                    var ImportFinanciero = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                    if (ImportFinanciero != null)
                    {
                        precioApertura += ImportFinanciero.Importe;
                    }

                    var ImportBonificaciones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                    if (ImportBonificaciones != null)
                    {
                        precioApertura += ImportBonificaciones.Importe;
                    }

                    var Comisiones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                    if (Comisiones != null)
                    {
                        precioApertura += Comisiones.Importe;
                    }

                    decimal im_precio = fijacion.Precio + precioApertura;

                    var rq = new ZMprfcRegistrarFijacion()
                    {
                        ImProveedor = fijacion.Proveedor.CUIT,
                        ImMaterial = fijacion.Material.Codigo,
                        ImKilos = (decimal)fijacion.Cantidad,
                        ImPrecio = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? im_precio : 0 : 0,
                        ImMoneda = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? fijacion.MonedaId.TrimEnd() : "" : "",
                        ImContrato = fijacion.ContratoSAP.ToString(),
                        ImCorredor = fijacion.Corredor != null ? fijacion.Corredor.CUIT : "",
                        ImApertura = listaApertura.ToArray(),
                        ImPagoDifArp = "",
                        ImDiasDiferim = "",
                        ImFecha = DateTime.Now.ToString("yyyy-MM-dd"),
                        ImZlsch = "",
                        ImCuentaMrp = "",
                        ImDolarizado = "",
                        ImDolExpress = "",
                        ImFechaLimite = "",
                        ImDolCorredor = "",
                        ImFechaCierta = "",
                        ImAnulacion = "X",
                        ImPedido = fijacion.FijacionSAP

                    };
                    logger.Debug(rq.ToXml());
                    logger.Debug("Anular fijacion log 10" + im_precio);
                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();
                    logger.Debug("Anular fijacion log 11" + im_precio);
                    var devolucion = agent.ZMprfcRegistrarFijacion(rq);

                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();
                    return devolucion.ExSalida;

                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al anular fijación.");
                    throw;
                }
            }
        }
    }
}
