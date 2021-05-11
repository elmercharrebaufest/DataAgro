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
        public FinalizarFijacionAgent(ILogger logger, IRepositorio repositorio, IContratosParaFijacionAgent contratosParaFijacionAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.contratosParaFijacionAgent = contratosParaFijacionAgent;
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

                    //•	Contrato 2699085 – No tiene descuentos y bonificaciones: el comercial podrá completar los conceptos que serían financiero, comisiones y bonificaciones:
                    if (oContrato != null && oContrato.PorcentajeSobrePrecio == 0 && oContrato.ImporteSobrePrecio == 0 &&
                        (oContrato.Aperturas == null || !oContrato.Aperturas.Any(x => x.Importe > 0 || x.Porcentaje > 0))
                        )
                    {
                        conceptosCargados.Add((int)EnumConceptoApertura.Comisiones);
                        conceptosCargados.Add((int)EnumConceptoApertura.Bonificaciones);
                    }
                    //•	Contrato 2699086 – Tiene bonificaciones: el comercial podrá completar los conceptos que serían financiero y comisiones:
                    if (oContrato != null && oContrato.Aperturas != null
                        //&& oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0 || x.Porcentaje > 0))
                        && !oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && (x.Importe > 0 || x.Porcentaje > 0))
                        )
                    {
                        conceptosCargados.Add((int)EnumConceptoApertura.Comisiones);
                    }
                    else
                    {
                        if (oContrato.Aperturas.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje !=
                            fijacion.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje
                            )
                        {
                            conceptosCargados.Add((int)EnumConceptoApertura.Comisiones);
                        }
                    }
                    //•	Contrato 2699087 – Tiene descuentos: el comercial podrá completar los conceptos que serían financiero, comisiones y bonificaciones:
                    // no afecta, seria lo mismo que el caso 2699085
                    //•	Contrato 2699088 – Tiene 1 de comisión o porcentaje sobre precio: el comercial podrá completar los conceptos que serían financiero y bonificaciones:
                    if (oContrato != null && oContrato.Aperturas != null
                        //&& oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && (x.Importe > 0 || x.Porcentaje == 1))
                        && !oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0 || x.Porcentaje > 0))
                        )
                    {
                        conceptosCargados.Add((int)EnumConceptoApertura.Bonificaciones);
                    }
                    //•	Contrato 2699089 – Tiene comisiones o porcentaje sobre precio y bonificaciones: el comercial podrá completar el costo financiero:


                    foreach (AperturaPrecio apertura in fijacion.AperturaPrecio)//.Where(x => listaAperturaConceptos.Contains(x.ConceptoAperturaPrecioId)))
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            var a = new ZMPES5440
                            {
                                CONCEPTO = apertura.ConceptoAperturaPrecio.CodigoSap,
                                IMPORTE = apertura.Importe,
                                MONEDA = fijacion.MonedaId/* != null && apertura.Porcentaje == 0 ? fijacion.Moneda.MonedaId : null*/,
                                PORC = apertura.Porcentaje
                            };
                            if (apertura.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones)
                            {
                                if (oContrato != null && oContrato.Aperturas != null && oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones))
                                {
                                    if (oContrato.Aperturas.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje !=
                                                                        fijacion.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje)
                                    {
                                        a.PORC = fijacion.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje -
                                            oContrato.Aperturas.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).First().Porcentaje;
                                    }
                                }

                            }
                            listaApertura.Add(a);
                        }
                    }
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
                    //decimal porcentajeComision = 0;
                    //if (Comisiones != null)
                    //{
                    //    precioApertura += Comisiones.Importe;
                    //    porcentajeComision = Comisiones.Porcentaje / 100;
                    //}

                    decimal im_precio = (fijacion.Precio + precioApertura);
                    //if (porcentajeComision > 0)
                    //{
                    //    im_precio = im_precio + (im_precio * porcentajeComision);
                    //    im_precio = Decimal.Round(im_precio, 2);
                    //}
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
                        IM_FECHA_LIMITE = fechaDolarizadoString,
                        IM_DOL_CORREDOR = fijacion.DolarizadoCorredor == true ? "X" : "",
                        IM_FECHA_CIERTA = "",


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
