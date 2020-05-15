using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarFijacion;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarFijacionAgent : IFinalizarFijacionAgent
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
                var numeroSAP = repositorio.Listar<FijacionDePrecioContrato, string>(x => x.FijacionSAP, x => x.FijacionSAP != null).Last();
                return (int.Parse(numeroSAP) + 1).ToString();
            }
            else
            {
                try
                {
                    SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient agent = new SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaApertura = new List<ZMPES5440>();
                    foreach (AperturaPrecio apertura in fijacion.AperturaPrecio)
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            listaApertura.Add(new ZMPES5440
                            {
                                CONCEPTO = apertura.ConceptoAperturaPrecio.CodigoSap,
                                IMPORTE = apertura.Importe,
                                MONEDA = fijacion.Moneda != null && apertura.Porcentaje == 0 ? fijacion.Moneda.MonedaId : null,
                                PORC = apertura.Porcentaje
                            });
                        }
                    }
                    decimal precioImportFinanciero = 0;
                    var ImportFinanciero = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 1).SingleOrDefault();
                    if (ImportFinanciero != null)
                    {
                        precioImportFinanciero = ImportFinanciero.Importe;
                    }
                    decimal im_precio = (fijacion.Precio + precioImportFinanciero);
                    var oContrato = repositorio.Obtener<Contrato>(x => x.ContratoSAP == fijacion.ContratoSAP);

                    if (listaApertura.Count > 0 && oContrato.Descuentos.Count == 0)
                        im_precio = fijacion.PrecioNeto.Value;

                    var rq = new Z_MPRFC_REGISTRAR_FIJACION()
                    {
                        IM_PROVEEDOR = fijacion.Proveedor.CUIT,
                        IM_MATERIAL = fijacion.Material.Codigo,
                        IM_KILOS = (decimal)fijacion.Cantidad,
                        IM_PRECIO = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? im_precio : 0 : 0,
                        IM_MONEDA = fijacion.Pizarra.HasValue ? !fijacion.Pizarra.Value ? fijacion.MonedaId.TrimEnd() : "" : "",
                        IM_CONTRATO = fijacion.ContratoSAP.ToString(),
                        IM_CORREDOR = fijacion.Corredor != null ? fijacion.Corredor.CUIT : "",
                        IM_APERTURA = listaApertura.ToArray(),
                        IM_PAGO_DIF_ARP = fijacion.PagoDiferido.HasValue && fijacion.PagoDiferido.Value ? "X" : "",
                        IM_DIAS_DIFERIM = fijacion.DiasPesificado.HasValue ? fijacion.DiasPesificado.Value.ToString() : "",
                        IM_FECHA = fijacion.Fecha.ToString("yyyy-MM-dd")
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACION(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

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
