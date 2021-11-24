using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarFijacion;
using Molinos.DataAgro.Entities.Common.Enums;
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
        private readonly IContratosParaFijacionAgent contratosParaFijacionAgent;
        private readonly ITipoDeCambioAgent tipoCambioAgent;

        public FinalizarFijacionAgent(ILogger logger, IRepositorio repositorio, IContratosParaFijacionAgent contratosParaFijacionAgent, ITipoDeCambioAgent tipoCambioAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratosParaFijacionAgent = contratosParaFijacionAgent;
            this.tipoCambioAgent = tipoCambioAgent;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        public string Finalizar(FijacionDePrecioContrato fijacion)
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
                    SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient agent = new SI_ZMPWS_DATAAGRO_REGISTRAR_FIJACIONClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaApertura = new List<ZMPES5440>();
                    var conceptosCargados = new List<int>() { (int)EnumConceptoApertura.Financiero };
                    var oContrato = contratosParaFijacionAgent.ObtenerContratos(fijacion.Proveedor.CUIT, fijacion.Corredor == null ? "" : fijacion.Corredor.CUIT, fijacion.MaterialId, fijacion.ContratoSAP.TrimStart('0'), fijacion.Id).SingleOrDefault();
                    if (oContrato.ImporteSobrePrecio != 0 && !string.IsNullOrEmpty(fijacion.MonedaId) && oContrato.MonedaSobrePrecio?.Trim() != fijacion.MonedaId.Trim())
                    {
                        var cotizacion = decimal.Round(tipoCambioAgent.TraerTipoDeCambio(DateTime.Now.AddDays(-1).Date), 2, MidpointRounding.AwayFromZero);
                        
                        if (fijacion.MonedaId.Trim() == "ARP")
                        {
                            oContrato.ImporteSobrePrecio = oContrato.ImporteSobrePrecio * cotizacion;
                        }
                        if (fijacion.MonedaId.Trim() == "USMD")
                        {
                            oContrato.ImporteSobrePrecio = oContrato.ImporteSobrePrecio / cotizacion;
                        }
                    }
                    var importeComisiones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).FirstOrDefault().Importe;
                    var modificoImporteComisionesAFijarViejo = (oContrato != null && oContrato.Aperturas == null || oContrato.Aperturas.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).ToList().Count == 0)
                                                            && importeComisiones != 0
                                                            && oContrato.ImporteSobrePrecio != 0
                                                            && importeComisiones != oContrato.ImporteSobrePrecio;

                    if ((oContrato != null && oContrato.ImporteSobrePrecio == 0)
                        || fijacion.AperturaPrecio.Any(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && a.Porcentaje > 0)
                        || modificoImporteComisionesAFijarViejo
                        )
                    {
                        conceptosCargados.Add((int)EnumConceptoApertura.Comisiones);
                    }

                    if (oContrato != null && oContrato.Aperturas != null
                        && !oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0 || x.Porcentaje > 0)))
                    {
                        conceptosCargados.Add((int)EnumConceptoApertura.Bonificaciones);
                    }

                    //if (fijacion.Pizarra != true)
                    //{
                        foreach (AperturaPrecio apertura in fijacion.AperturaPrecio.Where(x => conceptosCargados.Contains(x.ConceptoAperturaPrecioId)))
                        {
                            if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                            {
                                var a = new ZMPES5440
                                {
                                    CONCEPTO = apertura.ConceptoAperturaPrecio.CodigoSap,
                                    IMPORTE = apertura.Importe,
                                    MONEDA = apertura.MonedaId,
                                    PORC = apertura.Porcentaje
                                };
                                listaApertura.Add(a);
                            }
                        }
                    //}


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
                        IM_FECHA = fijacion.FechaOperacion.ToString("yyyy-MM-dd"),
                        IM_ZLSCH = fijacion.ChequeElectronico == true ? "=" : "",
                        IM_CUENTA_MRP = fijacion.PagoCBU != null ? fijacion.PagoCBU.Split('-')[0] : "",
                        IM_DOLARIZADO = fijacion.Dolarizado == true ? "X" : "",
                        IM_DOL_EXPRESS = fijacion.DolarizadoExpress == true ? "X" : "",
                        IM_FECHA_LIMITE = !String.IsNullOrEmpty(fechaDolarizadoString) ? fechaDolarizadoString : "",
                        IM_DOL_CORREDOR = fijacion.DolarizadoCorredor == true ? "X" : "",
                        IM_FECHA_CIERTA = fijacion.FechaCierta.HasValue ? fijacion.FechaCierta.Value.ToString("yyyy-MM-dd") : "",
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
