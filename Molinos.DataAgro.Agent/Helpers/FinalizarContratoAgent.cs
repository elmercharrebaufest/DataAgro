using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarContrato;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarContratoAgent : IFinalizarContratoAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ITipoDeCambioAgent tipoCambioAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        public FinalizarContratoAgent(ILogger logger, IRepositorio repositorio, ITipoDeCambioAgent tipoCambioAgent, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.tipoCambioAgent = tipoCambioAgent;
            this.diasHabilesAgent = diasHabilesAgent;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Finalizar(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {
            descuentoBonificacion = descuentoBonificacion ?? new List<DescuentoBonificacion>();
            var servicios = contrato.Servicios ?? new List<Servicio>();
            calidad = calidad ?? new List<Calidad>();
            logger.Debug("Finalizando Contrato Nro: " + contrato.Id);
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                var resp = "";
                resp = repositorio.ObtenerMayor<Negocio, int>(x => x.ContratoSAP != null && x.ContratoSAP != "", x => x.Id).ContratoSAP;
                long numsap = 0;
                long.TryParse(resp, out numsap);
                numsap = numsap + 1;
                resp = numsap.ToString().PadLeft(10, '0');
                return resp;
            }
            try
            {
                //using (var transaction = new TransactionScope())
                //{
                    SI_ZMPWS_DATAAGRO_PRE_SLIPClient agent = new SI_ZMPWS_DATAAGRO_PRE_SLIPClient();

                    agent.ClientCredentials.UserName.UserName = UserSap;
                    agent.ClientCredentials.UserName.Password = PassSap;

                    var listaDescuentos = new List<ZMPES5290>();
                    var topesFijacion = new List<ZMPES5280>();
                    var servicioSap = new List<ZMPES6620>();
                    foreach (var descBon in descuentoBonificacion)
                    {
                        if (descBon.TipoPeriodoDBId != 1)
                        {
                            listaDescuentos.Add(new ZMPES5290
                            {
                                TIPO_PERIODO = descBon.TipoPeriodoDB.CodigoSap,
                                TIPO_DB = descBon.TipoDB.CodigoSap,
                                FEDESDE = descBon.FechaDesde != null ? descBon.FechaDesde.Value.ToString("yyyy-MM-dd") : null,
                                FEHASTA = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                                IMPORTE_DB = descBon.Importe * (1 + (descBon.Porcentaje / 100)),
                                MONEDA_DB = descBon.MonedaId ?? "",
                                MONEDA = descBon.MonedaId ?? "",
                                PORC_DB = descBon.Porcentaje
                            }
                            );
                        };
                    }
                    decimal? precioNetoSustentable = null;

                    if ((contrato.EPA == true || contrato.Sustentable == true) && contrato.SustentableTipoDBId.HasValue)
                    {
                        listaDescuentos.Add(new ZMPES5290
                        {
                            TIPO_PERIODO = "I",
                            TIPO_DB = "B",
                            FEDESDE = contrato.FechaDesdeSustentable.HasValue ? contrato.FechaDesdeSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaDesde != null ? contrato.FechaDesde.ToString("yyyy-MM-dd") : null,
                            FEHASTA = contrato.FechaHastaSustentable.HasValue ? contrato.FechaHastaSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaHasta != null ? contrato.FechaHasta.ToString("yyyy-MM-dd") : null,
                            IMPORTE_DB = (contrato.TarifaAConvenir == true && contrato.Sustentable == true) ? -1 : contrato.SustentableTipoDBId == 1 ? 0 : contrato.ImporteSustentable.Value,
                            MONEDA_DB = contrato.MonedaSustentableId,
                            PORC_DB = 0,
                            PRECIO = 0
                        });
                        if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                        {
                            precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                        }
                    }

                    if (contrato.PrecioPactado != null && contrato.PrecioPactado.Count > 0)
                    {
                        var tipoCambio = decimal.Round(tipoCambioAgent.TraerTipoDeCambio(DateTime.Now.Date), 2, MidpointRounding.AwayFromZero);
                        foreach (var p in contrato.PrecioPactado)
                        {
                            decimal importe = 0;
                            if (p.ImportePactado.HasValue && p.ImportePactado.Value > 0)
                            {
                                if (p.MonedaImportePactadoId == p.MonedaPactadoId)
                                {
                                    importe = p.ImportePactado.Value;
                                }
                                else
                                {
                                    if (p.MonedaImportePactadoId.Replace(" ", "") == "USDM")
                                    {
                                        importe = p.ImportePactado.Value * tipoCambio;
                                    }
                                    else
                                    {
                                        importe = p.ImportePactado.Value / tipoCambio;
                                    }
                                }
                            }
                            listaDescuentos.Add(new ZMPES5290
                            {
                                TIPO_PERIODO = "E",
                                TIPO_DB = "",
                                FEDESDE = p.FechaDesde?.ToString("yyyy-MM-dd"),
                                FEHASTA = p.FechaHasta?.ToString("yyyy-MM-dd"),
                                IMPORTE_DB = 0, //p.ImportePactado ?? 0,
                                MONEDA_DB = "",//p.MonedaImportePactado != null ? p.MonedaImportePactadoId : "",
                                PORC_DB = 0, //p.Porcentaje ?? 0,
                                PRECIO = Math.Round(p.Precio + importe + (p.Porcentaje.HasValue ? p.Precio * (p.Porcentaje.Value / 100) : 0), 2),
                                MONEDA = p.MonedaPactadoId
                            });
                        }
                    }
                    logger.Debug("Descuentos: " + descuentoBonificacion);
                    var listaCalidades = new List<ZMPES5300>();
                    foreach (var cal in calidad)
                    {
                        if (cal.StandardDeCalidadId == 2)
                        {
                            if (((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecial.Id == 4 || cal.CalidadEspecial.Id == 5)) || cal.CalidadEspecial.Id == 10)
                            {
                                listaCalidades.Add(new ZMPES5300
                                {
                                    CODIGO = cal.CalidadEspecial.CodigoSap,
                                    VALOR = 0,
                                    PORC_DESDE = 1,
                                    PORC_HASTA = 1
                                }
                            );
                            }
                            else
                            {
                                if (cal.CalidadEspecialId == 1 && cal.PorcentajeHasta == 40)
                                {
                                    listaCalidades.Add(new ZMPES5300
                                    {
                                        CODIGO = cal.CalidadEspecial.CodigoSap,
                                        VALOR = cal.Valor,
                                        PORC_DESDE = cal.PorcentajeDesde ?? 0,
                                        PORC_HASTA = 51
                                    }
                                    );
                                }
                                else
                                {
                                    listaCalidades.Add(new ZMPES5300
                                    {
                                        CODIGO = cal.CalidadEspecial.CodigoSap,
                                        VALOR = cal.Valor,
                                        PORC_DESDE = cal.PorcentajeDesde ?? 0,
                                        PORC_HASTA = cal.PorcentajeHasta ?? 0
                                    }
                                    );
                                }
                            }
                        }
                        else if (cal.StandardDeCalidadId == 7)
                        {
                            listaCalidades.Add(new ZMPES5300
                            {
                                CODIGO = cal.CalidadEspecial.CodigoSap,
                                VALOR = 0,
                                PORC_DESDE = 1,
                                PORC_HASTA = 1
                            });
                        }
                    }
                    logger.Debug("Calidades: " + calidad);

                    var listaApertura = new List<ZMPES5440>();
                    if ((contrato.EPA == true || (contrato.Sustentable == true && contrato.TarifaAConvenir != true)) && contrato.SustentableTipoDBId == 1) //bonificación sobre precio
                    {
                        listaApertura.Add(new ZMPES5440
                        {
                            CONCEPTO = "BO",
                            IMPORTE = (decimal)contrato.ImporteSustentable,
                            MONEDA = contrato.MonedaSustentableId,
                        });
                    }
                    foreach (AperturaPrecio apertura in contrato.AperturaPrecio)
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            listaApertura.Add(new ZMPES5440
                            {
                                CONCEPTO = apertura.ConceptoAperturaPrecio.CodigoSap,
                                IMPORTE = apertura.Importe,
                                MONEDA = contrato.TipoNegocioId == 2 ? (contrato.Moneda != null ? contrato.Moneda.MonedaId : null) : apertura.MonedaId,
                                PORC = apertura.Porcentaje
                            });
                        }
                    }
                    
                    logger.Debug("Apertura: " + contrato.AperturaPrecio);
                    var descuentoGeneralSobrePrecio = descuentoBonificacion.AsQueryable()
                        .Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1)
                        .Select(a => new DescuentoBonificacion
                        {
                            TipoPeriodoDBId = a.TipoPeriodoDBId,
                            TipoDBId = a.TipoDBId,
                            Importe = a.Importe,
                            Porcentaje = a.Porcentaje,
                            MonedaId = a.MonedaId,
                        })
                        .FirstOrDefault();
                    if (contrato.Pizarra == true &&
                        (contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho && x.Importe != 0)
                            || contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && x.Porcentaje > 0)
                            )
                        )
                    {
                        descuentoGeneralSobrePrecio = new DescuentoBonificacion
                        {
                            TipoPeriodoDBId = 1,
                            TipoDBId = 1,
                            Importe = 0,
                            Porcentaje = 0,
                            MonedaId = "ARP  "
                        };
                        var red = contrato.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho && x.Importe != 0).SingleOrDefault();
                        if (red != null)
                        {
                            descuentoGeneralSobrePrecio.Importe = red.Importe;
                        }
                        var com = contrato.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && x.Porcentaje > 0).SingleOrDefault();
                        if (com != null)
                        {
                            descuentoGeneralSobrePrecio.Porcentaje = com.Porcentaje;
                        }

                    }
                    if ((contrato.EPA == true || contrato.Sustentable == true) && contrato.SustentableTipoDBId == 1)
                    {
                        descuentoGeneralSobrePrecio = descuentoGeneralSobrePrecio ?? new DescuentoBonificacion
                        {
                            TipoPeriodoDBId = 1,
                            TipoDBId = 1,
                            Importe = 0,
                            Porcentaje = 0,
                        };

                    descuentoGeneralSobrePrecio.Importe += contrato.ImporteSustentable ?? 0;
                    descuentoGeneralSobrePrecio.MonedaId = contrato.MonedaSustentableId;
                }
                logger.Debug("AperturaPrecio: " + contrato.AperturaPrecio);
                logger.Debug("Servicios: " + contrato.Servicios.ToJson());
                foreach (var servicio in servicios)
                {
                    servicioSap.Add(new ZMPES6620
                    {
                        CODIGO = servicio.ServicioValor.TipoServicio.CodigoSAP,
                        PORC_DESDE = servicio.Desde,
                        PORC_HASTA = servicio.Hasta,
                        VALOR = servicio.Importe,
                        MONEDA = servicio.MonedaId,
                        FECHAACT = contrato.Fecha.ToString("yyyy-MM-dd"),
                        HORAACT = contrato.Fecha.ToString("HH:mm:ss"),
                    }
                    );
                }
                logger.Debug("Servicios a enviar a SAP para NegocioID "+ contrato.Id + ": " + servicioSap.ToXml());

                var descuentoGeneralFueraPrecio = descuentoBonificacion.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado?.ToString("yyyy-MM-dd");
                //string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = calidad.Count > 0 ? "4" : "1";

                    string localidadString = RellenarEspaciosSAP(contrato.Localidad.CodLocalidad, 5);
                    decimal cantidadCamiones = Convert.ToDecimal(contrato.CantidadCamiones ?? 0);

                    topesFijacion.Add(new ZMPES5280
                    {
                        FE_DESDE = contrato.TipoNegocioId == 1 && contrato.DesdeFijacion.HasValue ? contrato.DesdeFijacion.Value.ToString("yyyy-MM-dd") : "",
                        FE_HASTA = contrato.TipoNegocioId == 1 && contrato.HastaFijacion.HasValue ? contrato.HastaFijacion.Value.ToString("yyyy-MM-dd") : "",
                        HORAACT = "00:00:00",
                        CANT_MAX = contrato.TipoNegocioId == 1 ?
                           contrato.Cantidad < 30000 ?
                           Convert.ToDecimal(contrato.Cantidad) :
                           (contrato.Cantidad >= 30000 && contrato.Cantidad <= 100000) ? 30000
                           : Convert.ToDecimal(contrato.KgMaximo) : 0,
                        CANT_MIN = contrato.TipoNegocioId == 1 ? contrato.Cantidad < 30000 ? Convert.ToDecimal(contrato.Cantidad) : 30000 : 0,
                        FECHAACT = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.Fecha.ToString("yyyy-MM-dd") :
                           repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd"),
                        VALOR = "KG"

                    });
                    logger.Debug("Cargando contrato");
                    var rq2 = new Z_MPRFC_PRE_SLIP();
                    rq2.IM_CONTRATO = new ZMPES5270();
                    rq2.IM_CONTRATO.CANTIDAD = Convert.ToDecimal(contrato.Cantidad);
                    rq2.IM_CONTRATO.CONTR_DATAAGRO = contrato.Id.ToString();
                    rq2.IM_CONTRATO.COSECHA = contrato.Campana.Descripcion;
                    rq2.IM_CONTRATO.DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0";
                    rq2.IM_CONTRATO.FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd");
                    rq2.IM_CONTRATO.FECHA_ENTREGA = contrato.FechaEntrega.ToString("yyyy-MM-dd");
                    rq2.IM_CONTRATO.FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd");
                    rq2.IM_CONTRATO.FECHA_LIMITE = fechaDolarizadoString;
                    rq2.IM_CONTRATO.GRUPO_COMPRAS = contrato.TipoAgenteCompraId == 1 ? "902" : "";
                    rq2.IM_CONTRATO.MONEDA = contrato.Pizarra == true ? "ARP  " : contrato.MonedaId;
                    rq2.IM_CONTRATO.NO_INFORMAR_SIO = noInformaSioString;
                    rq2.IM_CONTRATO.PAGO_DIFERIDO = (contrato.Dolarizado == true || contrato.DolarizadoCorredor == true) ? "X" : "";
                    rq2.IM_CONTRATO.MATERIAL = contrato.Material.Codigo;
                    rq2.IM_CONTRATO.PAGO_DIF_ARP = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "";
                    rq2.IM_CONTRATO.PRECIO_PIZARRA = contrato.Precio;
                    rq2.IM_CONTRATO.PRECIO = (contrato.EPA == true || contrato.Sustentable == true) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio;
                    rq2.IM_CONTRATO.PROVEEDOR = contrato.Proveedor.CUIT;
                    rq2.IM_CONTRATO.PROVINCIA = contrato.ProvinciaId.ToString();
                    rq2.IM_CONTRATO.SUSTENTABLE = contrato.Sustentable == true || contrato.EPA == true ? "X" : "";
                    rq2.IM_CONTRATO.EPA = contrato.EPA == true ? "X" : "";
                    rq2.IM_CONTRATO.ESPECIAL = contrato.StandardDeCalidad != null ? contrato.StandardDeCalidad.CodigoSap : especialString;
                    rq2.IM_CONTRATO.FECHA = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.FechaOperacion.ToString("yyyy-MM-dd") : repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd");
                    rq2.IM_CONTRATO.USUARIO = contrato.Comercial.IdActiveDirectory;
                    rq2.IM_CONTRATO.HORAACT = contrato.Fecha.ToString("HH:mm:ss");
                    rq2.IM_CONTRATO.PROCEDENCIA = localidadString;
                    rq2.IM_CONTRATO.CENTRO = contrato.Destino.CodigoSap;
                    rq2.IM_CONTRATO.CLASIFICACION = contrato.Clasificacion.Descripcion;
                    rq2.IM_CONTRATO.IND_OP_CANJE = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "";
                    rq2.IM_CONTRATO.CONSIGNATARIO = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "";
                    rq2.IM_CONTRATO.COND_FIJACION = contrato.CondicionFijacion?.CodigoSap;
                    rq2.IM_CONTRATO.CAMIONES = cantidadCamiones;
                    rq2.IM_CONTRATO.CONFIRMA = contrato.BoletoId == 1 ? "X" : "";
                    rq2.IM_CONTRATO.BOLSA = contrato.BoletoId == 1 || contrato.BoletoId == 2 || contrato.BoletoId == 4 ? contrato.Bolsa.CodigoSap : null;
                    rq2.IM_CONTRATO.BOL_FISICO = contrato.BoletoId == 2 ? "X" : "";
                    rq2.IM_CONTRATO.CARTA_OFERTA = contrato.BoletoId == 4 ? "X" : "";
                    rq2.IM_CONTRATO.NINGUNO = contrato.BoletoId == 3 ? "X" : "";
                    rq2.IM_CONTRATO.SIN_BOLETO = contrato.BoletoId == 5 ? "X" : "";
                    rq2.IM_CONTRATO.AUT_CG = contrato.Warrant == true ? "X" : "";
                    rq2.IM_CONTRATO.AUR_CD = contrato.CD == true ? "X" : "";
                    rq2.IM_CONTRATO.PAGO_DIR_VEND = contrato.PagoDirectoVendedor == true ? "X" : "";
                    rq2.IM_CONTRATO.ESTAB_PROPIO = contrato.EstablecimientoPropio == true ? "X" : "";
                    rq2.IM_CONTRATO.ESTAB_ARRENDADO = contrato.EstablecimientoPropio == false ? "X" : "";
                    rq2.IM_CONTRATO.IMPORTE_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                    rq2.IM_CONTRATO.MONEDA_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                    rq2.IM_CONTRATO.PORC_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                    rq2.IM_CONTRATO.IMPORTE_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                    rq2.IM_CONTRATO.MONEDA_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                    rq2.IM_CONTRATO.PORC_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                    rq2.IM_CONTRATO.MERC_DESCARGADA = contrato.MercsDeposito == true ? "X" : "";
                    rq2.IM_CONTRATO.OBSERVACION_CAL1 = contrato.Observacion;
                    rq2.IM_CONTRATO.CUIT_CORREDOR = contrato.Corredor != null ? contrato.Corredor.CUIT : "";
                    rq2.IM_CONTRATO.PORC_COMISION = contrato.Corredor != null ? contrato.PorcentajeComision.Value : 0;
                    rq2.IM_CONTRATO.CONTRCORR = contrato.ContratoCorredor ?? "";
                    rq2.IM_CONTRATO.CONTRVEND = contrato.ContratoVendedor ?? "";
                    rq2.IM_CONTRATO.SEL_CARGO_MOA = contrato.SelCargoMOA == true ? "X" : "";
                    rq2.IM_CONTRATO.SEL_CARGO_VEND = contrato.SelCargoVendedor == true ? "X" : "";
                    rq2.IM_CONTRATO.CONTRATO_MADRE = contrato.ContratoMadre ?? "";
                    rq2.IM_CONTRATO.CREADOR = contrato.ComercialCreador != null ? contrato.ComercialCreador.IdActiveDirectory : "";
                    rq2.IM_CONTRATO.ZONA = contrato.Zona != null ? contrato.Zona.CodigoSap : "";
                    rq2.IM_CONTRATO.COMPENSACION = contrato.Compensacion == true ? "X" : "";
                    rq2.IM_CONTRATO.FLETE_NIVEL = contrato.NivelTarifa != null ? contrato.NivelTarifa.CodigoSap : "";
                    rq2.IM_CONTRATO.FLETE_TARIFA = contrato.TarifaFlete ?? 0;
                    rq2.IM_CONTRATO.FECHA_CIERTA = contrato.FechaCierta.HasValue ? contrato.FechaCierta.Value.ToString("yyyy-MM-dd") : null;
                    rq2.IM_CONTRATO.PORCPARCIAL = contrato.PorcentajeDePago ?? (decimal)97.5;
                    rq2.IM_CONTRATO.AGENTE_COMPRA = contrato.TipoAgenteCompraId == 1 ? "9952569841" : "";
                    rq2.IM_CONTRATO.CARATULA = contrato.CaratulaMAT;
                    rq2.IM_CONTRATO.CARATULA_EXT = contrato.CaratulaExtension;
                    rq2.IM_CONTRATO.PRECIO_COM_MAT = contrato.PrecioAjusteComision ?? 0;
                    rq2.IM_CONTRATO.MONEDA_COM_MAT = contrato.MonedaAjusteComisionId;
                    rq2.IM_CONTRATO.FECHA_CREACION = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.Fecha.ToString("yyyy-MM-dd") : repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd");
                    rq2.IM_CONTRATO.ZLSCH = contrato.ChequeElectronico == true ? "=" : "";
                    rq2.IM_CONTRATO.DOL_EXPRESS = contrato.DolarizadoExpress == true ? "X" : "";
                    rq2.IM_CONTRATO.CUENTA_MRP = contrato.PagoCBU != null ? contrato.PagoCBU.Split('-')[0] : "";
                    rq2.IM_CONTRATO.PLANTA_DEST = contrato.PlantaDestino != null ? contrato.PlantaDestino.CodigoSap : contrato.Destino.CodigoSap;
                    rq2.IM_CONTRATO.CANJE = contrato.Canje == true ? "X" : "";
                    rq2.IM_CONTRATO.DESC_INSUMOS = contrato.Insumo;
                    rq2.IM_CONTRATO.MONEDA_DEUDA = contrato.MonedaCanjeId == "USDM " ? "USD" : contrato.MonedaCanjeId;
                    rq2.IM_CONTRATO.MONTO_DEUDA = contrato.Monto.HasValue ? contrato.Monto.Value : 0;
                    rq2.IM_CONTRATO.POSICION_CBOT = contrato.PosicionCBOT ?? "";
                    rq2.IM_CONTRATO.FIJ_CBOT_MAT = contrato.TipoPosicionCBOTId.HasValue ? contrato.TipoPosicionCBOTId.ToString() : "";
                    rq2.IM_CONTRATO.TERCERO = contrato.ProveedorCreadorId != null ? "X" : "";
                    rq2.IM_CONTRATO.ANULA_Y_REEMP = contrato.AnulaYReemplazaContratoId == null ? "" : contrato.AnulaYReemplazaContrato.ContratoSAP;
                    rq2.IM_CONTRATO.CONDICIONAL = contrato.Condicional == true ? "X" : "";
                    rq2.IM_CONTRATO.FECHA_COND = contrato.CondicionalFecha != null ? contrato.CondicionalFecha.Value.ToString("yyyy-MM-dd") : "";
                    rq2.IM_CONTRATO.MES_COND_MAT = contrato.CondicionalPosicion != null ? contrato.CondicionalPosicion : "";
                    rq2.IM_CONTRATO.MONEDA_COND = contrato.CondicionalMonedaId != null ? contrato.CondicionalMonedaId : "";
                    rq2.IM_CONTRATO.PRECIO_COND = contrato.CondicionalPrecio != null ? contrato.CondicionalPrecio.Value : 0;
                    rq2.IM_CONTRATO.CONTRATO_COND = contrato.CondicionalContrato != null ? contrato.CondicionalContrato.ContratoSAP : "";
                    rq2.IM_CONTRATO.CANTIDAD_COND = contrato.CondicionalCantidad != null ? Convert.ToDecimal(contrato.CondicionalCantidad.Value) : 0;
                    rq2.IM_CONTRATO.COND_PAGO = contrato.TipoNegocioId == 1 ? "04" : "";
                    rq2.IM_CONTRATO.PORC_MULTA = contrato.TipoNegocioId == 1 ? "10" : "";
                    rq2.IM_CONTRATO.TOL_INF = contrato.CantidadCamiones > 0 ? 0 : 3;
                    rq2.IM_CONTRATO.TOL_SUP = contrato.CantidadCamiones > 0 ? 0 : 3;
                    rq2.IM_CONTRATO.PIZARRA = contrato.TipoNegocioId == 1 ? "ROS" : "";
                    rq2.IM_CONTRATO.CODIGO_TC = contrato.TipoNegocioId == 2 && contrato.MonedaId == "USDM " && contrato.TipoAgenteCompraId == null ? "02" : contrato.TipoNegocioId == 2 && contrato.MonedaId == "USDM " && contrato.TipoAgenteCompraId != null ? "03" : "";
                    rq2.IM_CONTRATO.BLOQUEO = "";
                    rq2.IM_CONTRATO.TIPO_CAMBIO_FIJO = 0;
                    rq2.IM_CONTRATO.POSICION = CalcularPosicion(contrato.FechaDesde);
                    rq2.IM_TOPES_FIJ = topesFijacion.ToArray();
                    rq2.IM_DESC_BONIF = listaDescuentos.ToArray();
                    rq2.IM_CALIDAD = listaCalidades.ToArray();
                    rq2.IM_TIPO_NEGOCIO = contrato.Madre == true ? "MADRE" : contrato.Madre == false ? "HIJO" : contrato.EsFason == true ? "FASON" : contrato.PrestamoDevolucion == true ? "PRESTAMO_DEVOLUCION" : contrato.Venta == true ? "VENTA" : contrato.TipoNegocio.Descripcion;
                    rq2.IM_APERTURA = listaApertura.ToArray();
                    rq2.IM_SERVICIOS = servicioSap.ToArray();

                    logger.Debug(rq2.ToXml());

                    var log = new Log
                    {
                        Fecha = DateTime.Now,
                        Xml = rq2.ToXml()
                    };

                    var logId = repositorio.Agregar(log);
                    repositorio.GuardarCambios();

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_PRE_SLIP(rq2);
                    logger.Debug(devolucion.ToXml());

                    log = repositorio.Obtener<Log>(logId.Id);
                    log.Xml += devolucion.ToXml();
                    repositorio.GuardarCambios();

                    if (devolucion.EX_MENSAJE_ERROR != null && devolucion.EX_MENSAJE_ERROR != "")
                    {
                        throw new Exception(devolucion.EX_MENSAJE_ERROR);
                    }
                    //transaction.Complete();
                    return devolucion.EX_CONTRATO_SAP;

                //}


            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP: No se pudo finalizar el contrato.");
                logger.Error(e);
                throw;
            }
        }

        private static decimal? PrecioNetoSustentableSobrePrecio(Contrato contrato, decimal? precioNetoSustentable)
        {
            decimal porcentajeComision = contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 3).FirstOrDefault()?.Porcentaje ?? 0;
            decimal precioOriginal = contrato.Precio;
            decimal precioTarifaFlete = contrato.TarifaFlete ?? 0;
            precioOriginal += contrato.ImporteSustentable ?? 0;
            precioOriginal += contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 1).FirstOrDefault()?.Importe ?? 0;
            precioOriginal += contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 2).FirstOrDefault()?.Importe ?? 0;
            precioOriginal += contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 4).FirstOrDefault()?.Importe ?? 0;
            precioOriginal += (contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 4).FirstOrDefault()?.Porcentaje ?? 0) * contrato.Precio / 100; 
            porcentajeComision /= 100;
            precioOriginal += (precioOriginal * porcentajeComision) - precioTarifaFlete;
            precioOriginal += contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 3).FirstOrDefault()?.Importe ?? 0;
            precioNetoSustentable = Math.Round(precioOriginal, 2);
            return precioNetoSustentable;
        }

        private string RellenarEspaciosSAP(string value, int stringLength)
        {
            if (value != null)
            {
                int cantCeros = stringLength - value.Length;
                for (int i = 0; i < cantCeros; i++)
                {
                    value = " " + value;
                }
            }
            return value;
        }
        private string CalcularPosicion(DateTime fechaDesde)
        {
            var ultimoDiaHabilDelMes = diasHabilesAgent.ObtenerDiasHabilesDelMes(fechaDesde).LastOrDefault();
            var diferenciaEntreDias = fechaDesde - ultimoDiaHabilDelMes;
            var fechaDesdeMesSiguiente = fechaDesde.AddMonths(1);
            var dias = Math.Abs(diferenciaEntreDias.Days);
            return dias >= 10 ? (fechaDesde.Month.ToString().PadLeft(2, '0')) + "." + (fechaDesde.Year) :
                   ((fechaDesdeMesSiguiente.Month).ToString().PadLeft(2, '0')) + "." + (fechaDesdeMesSiguiente.Year);
        }
    }
}