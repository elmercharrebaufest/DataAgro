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
    public class FinalizarFijacionAgent : IFinalizarFijacionAgent
    {
        private readonly IContratosParaFijacionAgent contratosParaFijacionAgent;
        private readonly ITipoDeCambioAgent tipoCambioAgent;
        private readonly IFinalizarContratoAgent finalizarContratoAgent;
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public FinalizarFijacionAgent(ILogger logger, IRepositorio repositorio, IContratosParaFijacionAgent contratosParaFijacionAgent, ITipoDeCambioAgent tipoCambioAgent, IFinalizarContratoAgent finalizarContratoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratosParaFijacionAgent = contratosParaFijacionAgent;
            this.tipoCambioAgent = tipoCambioAgent;
            this.finalizarContratoAgent = finalizarContratoAgent;
        }

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

                    Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaApertura = new List<Zmpes5440>();
                    var conceptosCargados = new List<int>() { (int)EnumConceptoApertura.Financiero };
                    var oContrato = contratosParaFijacionAgent.ObtenerContratos(fijacion.Proveedor.CUIT, fijacion.Corredor == null ? "" : fijacion.Corredor.CUIT, fijacion.MaterialId, fijacion.ContratoSAP.TrimStart('0'), fijacion.Id).SingleOrDefault();
                    if (oContrato.ImporteSobrePrecio != 0 && !string.IsNullOrEmpty(fijacion.MonedaId) && oContrato.MonedaSobrePrecio?.Trim() != fijacion.MonedaId.Trim())
                    {
                        string codigoTC = finalizarContratoAgent.DevolverTipoCambioSAP(fijacion.TipoNegocioId, fijacion.MonedaId, fijacion.TipoAgenteCompraId, fijacion.Fecha);
                        string typeOfRate = codigoTC == "04" ? "Z" : "M";
                        var cotizacion = decimal.Round(tipoCambioAgent.TraerTipoDeCambio(DateTime.Now.AddDays(-1).Date, typeOfRate), 2, MidpointRounding.AwayFromZero);

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
                            var a = new Zmpes5440
                            {
                                Concepto = apertura.ConceptoAperturaPrecio.CodigoSap,
                                Importe = apertura.Importe,
                                Moneda = apertura.MonedaId,
                                Porc = apertura.Porcentaje
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
                        ImPagoDifArp = fijacion.PagoDiferido.HasValue && fijacion.PagoDiferido.Value ? "X" : "",
                        ImDiasDiferim = fijacion.DiasPesificado.HasValue ? fijacion.DiasPesificado.Value.ToString() : "",
                        ImFecha = fijacion.FechaOperacion.ToString("yyyy-MM-dd"),
                        ImZlsch = fijacion.ChequeElectronico == true ? "=" : "",
                        ImCuentaMrp = fijacion.PagoCBU != null ? fijacion.PagoCBU.Split('-')[0] : "",
                        ImDolarizado = fijacion.Dolarizado == true ? "X" : "",
                        ImDolExpress = fijacion.DolarizadoExpress == true ? "X" : "",
                        ImFechaLimite = !String.IsNullOrEmpty(fechaDolarizadoString) ? fechaDolarizadoString : "",
                        ImDolCorredor = fijacion.DolarizadoCorredor == true ? "X" : "",
                        ImFechaCierta = fijacion.FechaCierta.HasValue ? fijacion.FechaCierta.Value.ToString("yyyy-MM-dd") : "",
                        ImAnulacion = "",
                        ImPedido = ""
                    };
                    logger.Debug(rq.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.ZMprfcRegistrarFijacion(rq);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    if (devolucion.ExMensaje != null && devolucion.ExMensaje != "")
                    {
                        throw new Exception(devolucion.ExMensaje);
                    }

                    return devolucion.ExSalida;
                }
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al finalizar fijación.");
                    throw;
                }
            }
        }
    }
}
