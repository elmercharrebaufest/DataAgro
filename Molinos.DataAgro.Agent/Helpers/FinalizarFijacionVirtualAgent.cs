using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.InsertarFijacionesVirtuales;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
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
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        logger.Info("SAP sin PI - RFC ZMprfcInsertarFijVirCanje");
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
                        logger.Info("SAP sin PI - RFC ZMprfcInsertarFijVirCanje");
                        logger.Debug(valor.ToXml());
                        log = repositorio.Obtener<Log>(logId.Id);
                        log.Xml += valor.ToXml();
                        repositorio.GuardarCambios();

                        return valor.ExNrofijo;
                    }
                    else
                    {
                        var agent = new SI_ZMPWS_DATAAGRO_INSERTAR_FIJ_VIR_CANJEClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;
                        //var conceptosCargados = new List<int>() { };
                        //var oContrato = contratosParaFijacionVirtualAgent.ObtenerContratosCanje(fijacion.Proveedor.CUIT, fijacion.Corredor == null ? "" : fijacion.Corredor.CUIT, fijacion.MaterialId, fijacion.ContratoSAP.TrimStart('0'), fijacion.Id).SingleOrDefault();

                        //var importeComisiones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones).FirstOrDefault().Importe;
                        //var modificoImporteComisionesAFijarViejo = (oContrato.Aperturas == null || oContrato.Aperturas.Count == 0)
                        //                                        && importeComisiones != 0
                        //                                        && oContrato.ImporteSobrePrecio != 0
                        //                                        && importeComisiones != oContrato.ImporteSobrePrecio;

                        //if ((oContrato != null && oContrato.ImporteSobrePrecio == 0)
                        //    || fijacion.AperturaPrecio.Any(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && a.Porcentaje > 0)
                        //    || modificoImporteComisionesAFijarViejo
                        //    )
                        //{
                        //    conceptosCargados.Add((int)EnumConceptoApertura.Comisiones);
                        //}

                        //if (oContrato != null && oContrato.Aperturas != null
                        //    && !oContrato.Aperturas.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0 || x.Porcentaje > 0)))
                        //{
                        //    conceptosCargados.Add((int)EnumConceptoApertura.Bonificaciones);
                        //}                   


                        //decimal precioApertura = 0;
                        //var fechaDolarizadoString = fijacion.FechaDolarizado?.ToString("yyyy-MM-dd");
                        //var ImportFinanciero = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                        //if (ImportFinanciero != null)
                        //{
                        //    precioApertura += ImportFinanciero.Importe;
                        //}
                        //var ImportBonificaciones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                        //if (ImportBonificaciones != null)
                        //{
                        //    precioApertura += ImportBonificaciones.Importe;
                        //}
                        //var Comisiones = fijacion.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && conceptosCargados.Contains(a.ConceptoAperturaPrecioId)).SingleOrDefault();
                        //if (Comisiones != null)
                        //{
                        //    precioApertura += Comisiones.Importe;
                        //}

                        //decimal im_precio = fijacion.Precio + precioApertura;
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
                        var rq = new Z_MPRFC_INSERTAR_FIJ_VIR_CANJE()
                        {
                            IM_A_FIJAR = fijacion.Cantidad.ToString(),
                            IM_COMERCIAL = fijacion.ComercialCreador.IdActiveDirectory,// se informa el creador
                            IM_CONTRATO = fijacion.ContratoSAP,
                            IM_FECHA = fijacion.Fecha.ToString("yyyy-MM-dd"),
                            IM_HORA = fijacion.Fecha.ToString("HH:mm:ss"),
                            IM_PRECIO = comision > 0 ? fijacion.Precio : decimal.Parse(precioNeto.ToString("n2")),
                            IM_MONEDA = fijacion.MonedaId.TrimEnd(),
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
