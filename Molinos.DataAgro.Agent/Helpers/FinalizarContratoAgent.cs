using Molinos.DataAgro.Agent.FinalizarContrato;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Collections.Generic;
using System.Configuration;
using Autofac.Extras.NLog;
using System.Linq;
using System;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;

namespace Molinos.DataAgro.Agent.Helpers
{
    public class FinalizarContratoAgent : IFinalizarContratoAgent
    {
        private readonly IRepositorio repositorio;
        private readonly ITipoDeCambioAgent tipoCambioAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly ILogger logger;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public FinalizarContratoAgent(ILogger logger, IRepositorio repositorio, ITipoDeCambioAgent tipoCambioAgent, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.tipoCambioAgent = tipoCambioAgent;
            this.diasHabilesAgent = diasHabilesAgent;
        }

        public string FinalizarContratoSinPI(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {
            descuentoBonificacion = descuentoBonificacion ?? new List<DescuentoBonificacion>();
            var servicios = contrato.Servicios ?? new List<Servicio>();
            calidad = calidad ?? new List<Calidad>();
            logger.Debug("Finalizando Contrato Nro: " + contrato.Id);

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                var resp = "";
                resp = repositorio.ObtenerMayor<Negocio, int>(x => x.ContratoSAP != null && x.ContratoSAP != "", x => x.Id).ContratoSAP;
                long.TryParse(resp, out long numsap);
                numsap++;
                resp = numsap.ToString().PadLeft(10, '0');
                return resp;
            }
            try
            {
                //using (var transaction = new TransactionScope())
                //{
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                var listaDescuentos = new List<Zmpes5290>();
                var topesFijacion = new List<Zmpes5280>();
                var servicioSap = new List<Zmpes6620>();
                foreach (var descBon in descuentoBonificacion)
                {
                    if (descBon.TipoPeriodoDBId != 1)
                    {
                        listaDescuentos.Add(new Zmpes5290
                        {
                            TipoPeriodo = descBon.TipoPeriodoDB.CodigoSap,
                            TipoDb = descBon.TipoDB.CodigoSap,
                            Fedesde = descBon.FechaDesde?.ToString("yyyy-MM-dd"),
                            Fehasta = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                            ImporteDb = Math.Round(descBon.Importe * (1 + (descBon.Porcentaje / 100)), 2),
                            MonedaDb = descBon.MonedaId ?? "",
                            Moneda = descBon.MonedaId ?? "",
                            PorcDb = descBon.Porcentaje
                        }
                        );
                    }
                    ;
                }
                decimal? precioNetoSustentable = null;

                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId.HasValue)
                {
                    listaDescuentos.Add(new Zmpes5290
                    {
                        TipoPeriodo = "I",
                        TipoDb = "B",
                        Fedesde = contrato.FechaDesdeSustentable.HasValue ? contrato.FechaDesdeSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaDesde.ToString("yyyy-MM-dd"),
                        Fehasta = contrato.FechaHastaSustentable.HasValue ? contrato.FechaHastaSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaHasta.ToString("yyyy-MM-dd"),
                        ImporteDb = contrato.TarifaAConvenir == true ? -1 : contrato.SustentableTipoDBId == 1 ? 0 : contrato.ImporteSustentable.Value,
                        MonedaDb = contrato.MonedaSustentableId,
                        PorcDb = 0,
                        Precio = 0
                    });
                    if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                    {
                        precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                    }
                }

                string typeOfRate = contrato.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND ? "Z" : "M";

                if (contrato.PrecioPactado != null && contrato.PrecioPactado.Count > 0)
                {
                    var tipoCambio = decimal.Round(tipoCambioAgent.TraerTipoDeCambio(DateTime.Now.Date, typeOfRate), 2, MidpointRounding.AwayFromZero);
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
                        listaDescuentos.Add(new Zmpes5290
                        {
                            TipoPeriodo = "E",
                            TipoDb = "",
                            Fedesde = p.FechaDesde?.ToString("yyyy-MM-dd"),
                            Fehasta = p.FechaHasta?.ToString("yyyy-MM-dd"),
                            ImporteDb = 0, //p.ImportePactado ?? 0,
                            MonedaDb = "",//p.MonedaImportePactado != null ? p.MonedaImportePactadoId : "",
                            PorcDb = 0, //p.Porcentaje ?? 0,
                            Precio = Math.Round(p.Precio + importe + (p.Porcentaje.HasValue ? p.Precio * (p.Porcentaje.Value / 100) : 0), 2),
                            Moneda = p.MonedaPactadoId
                        });
                    }
                }
                logger.Debug("Descuentos: " + descuentoBonificacion.ToJson());
                var listaCalidades = new List<Zmpes5300>();
                foreach (var cal in calidad)
                {
                    if (cal.StandardDeCalidadId == 2)
                    {
                        if (((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecial.Id == 4 || cal.CalidadEspecial.Id == 5)) || cal.CalidadEspecial.Id == 10)
                        {
                            listaCalidades.Add(new Zmpes5300
                            {
                                Codigo = cal.CalidadEspecial.CodigoSap,
                                Valor = 0,
                                PorcDesde = 1,
                                PorcHasta = 1
                            }
                        );
                        }
                        else
                        {
                            if (cal.CalidadEspecialId == 1 && cal.PorcentajeHasta == 40)
                            {
                                listaCalidades.Add(new Zmpes5300
                                {
                                    Codigo = cal.CalidadEspecial.CodigoSap,
                                    Valor = cal.Valor,
                                    PorcDesde = cal.PorcentajeDesde ?? 0,
                                    PorcHasta = 51
                                }
                                );
                            }
                            else
                            {
                                listaCalidades.Add(new Zmpes5300
                                {
                                    Codigo = cal.CalidadEspecial.CodigoSap,
                                    Valor = cal.Valor,
                                    PorcDesde = cal.PorcentajeDesde ?? 0,
                                    PorcHasta = cal.PorcentajeHasta ?? 0
                                }
                                );
                            }
                        }
                    }
                    else if (cal.StandardDeCalidadId == 7)
                    {
                        listaCalidades.Add(new Zmpes5300
                        {
                            Codigo = cal.CalidadEspecial.CodigoSap,
                            Valor = 0,
                            PorcDesde = 1,
                            PorcHasta = 1
                        });
                    }
                }
                logger.Debug("Calidades: " + calidad.ToJson());

                var listaApertura = new List<Zmpes5440>();
                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.TarifaAConvenir != true && contrato.SustentableTipoDBId == 1) //bonificación sobre precio
                {
                    listaApertura.Add(new Zmpes5440
                    {
                        Concepto = "BO",
                        Importe = (decimal)contrato.ImporteSustentable,
                        Moneda = contrato.MonedaSustentableId,
                    });
                }
                foreach (AperturaPrecio apertura in contrato.AperturaPrecio)
                {
                    if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                    {
                        listaApertura.Add(new Zmpes5440
                        {
                            Concepto = apertura.ConceptoAperturaPrecio.CodigoSap,
                            Importe = apertura.Importe,
                            Moneda = contrato.TipoNegocioId == 2 ? contrato.Moneda?.MonedaId : apertura.MonedaId,
                            Porc = apertura.Porcentaje
                        });
                    }
                }

                logger.Debug("Apertura: " + contrato.AperturaPrecio.ToJson());
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
                    (contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho && x.Importe != 0) ||
                    contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && x.Porcentaje > 0)))
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
                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId == 1)
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
                logger.Debug("AperturaPrecio: " + contrato.AperturaPrecio.ToJson());
                logger.Debug("Servicios: " + contrato.Servicios.ToJson());
                foreach (var servicio in servicios)
                {
                    servicioSap.Add(new Zmpes6620
                    {
                        Codigo = servicio.ServicioValor.TipoServicio.CodigoSAP,
                        PorcDesde = servicio.Desde,
                        PorcHasta = servicio.Hasta,
                        Valor = servicio.Importe,
                        Moneda = servicio.MonedaId,
                        Fechaact = contrato.Fecha.ToString("yyyy-MM-dd"),
                        Horaact = contrato.Fecha.ToString("HH:mm:ss"),
                    });
                }
                logger.Debug($"Servicios a enviar a SAP para NegocioID {contrato.Id}: {servicioSap.ToXml()}");

                var descuentoGeneralFueraPrecio = descuentoBonificacion.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado?.ToString("yyyy-MM-dd");
                //string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = calidad.Count > 0 ? "4" : "1";

                string localidadString = RellenarEspaciosSAP(contrato.Localidad.CodLocalidad, 5);
                decimal cantidadCamiones = Convert.ToDecimal(contrato.CantidadCamiones ?? 0);
                var cantidadAbsoluta = Math.Abs(contrato.Cantidad);

                topesFijacion.Add(new Zmpes5280
                {
                    FeDesde = contrato.TipoNegocioId == 1 && contrato.DesdeFijacion.HasValue ? contrato.DesdeFijacion.Value.ToString("yyyy-MM-dd") : "",
                    FeHasta = contrato.TipoNegocioId == 1 && contrato.HastaFijacion.HasValue ? contrato.HastaFijacion.Value.ToString("yyyy-MM-dd") : "",
                    Horaact = "00:00:00",
                    CantMax = contrato.TipoNegocioId == 1 ?
                       cantidadAbsoluta < 30000 ?
                       Convert.ToDecimal(cantidadAbsoluta) :
                       (cantidadAbsoluta >= 30000 && cantidadAbsoluta <= 100000) ? 30000
                       : Convert.ToDecimal(contrato.KgMaximo) : 0,
                    CantMin = contrato.TipoNegocioId == 1 ? cantidadAbsoluta < 30000 ? Convert.ToDecimal(cantidadAbsoluta) : 30000 : 0,
                    Fechaact = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.Fecha.ToString("yyyy-MM-dd") :
                       repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd"),
                    Valor = "KG"

                });
                logger.Debug("Cargando contrato");
                var rq2 = new ZMprfcPreSlip
                {
                    ImContrato = new Zmpes5270()
                };
                rq2.ImContrato.Cantidad = Convert.ToDecimal(cantidadAbsoluta);
                rq2.ImContrato.ContrDataagro = contrato.Id.ToString();
                rq2.ImContrato.Cosecha = contrato.Campana.Descripcion;
                rq2.ImContrato.DiasDiferim = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0";
                rq2.ImContrato.FechaDesde = contrato.FechaDesde.ToString("yyyy-MM-dd");
                rq2.ImContrato.FechaEntrega = contrato.FechaEntrega.Value.ToString("yyyy-MM-dd");
                rq2.ImContrato.FechaHasta = contrato.FechaHasta.ToString("yyyy-MM-dd");
                rq2.ImContrato.FechaLimite = fechaDolarizadoString;
                rq2.ImContrato.GrupoCompras = contrato.TipoAgenteCompraId == 1 ? "902" : "";
                rq2.ImContrato.Moneda = contrato.Pizarra == true ? "ARP  " : contrato.MonedaId;
                rq2.ImContrato.NoInformarSio = noInformaSioString;
                rq2.ImContrato.PagoDiferido = (contrato.Dolarizado == true || contrato.DolarizadoCorredor == true) ? "X" : "";
                rq2.ImContrato.Material = contrato.Material.Codigo;
                rq2.ImContrato.PagoDifArp = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "";
                rq2.ImContrato.PrecioPizarra = contrato.Precio;
                rq2.ImContrato.Precio = (contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio;
                rq2.ImContrato.Proveedor = contrato.Proveedor.CUIT;
                rq2.ImContrato.Provincia = contrato.ProvinciaId.ToString();
                rq2.ImContrato.Sustentable = contrato.Sustentable || contrato.EPA || contrato.EUDR ? "X" : "";
                rq2.ImContrato.Epa = contrato.EPA ? "X" : "";
                rq2.ImContrato.Eudr = contrato.EUDR && !contrato.EPA ? "X" : "";
                rq2.ImContrato.Especial = contrato.StandardDeCalidad != null ? contrato.StandardDeCalidad.CodigoSap : especialString;
                rq2.ImContrato.Fecha = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.FechaOperacion.ToString("yyyy-MM-dd") : repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd");
                rq2.ImContrato.Usuario = contrato.Comercial.IdActiveDirectory;
                rq2.ImContrato.Horaact = contrato.Fecha.ToString("HH:mm:ss");
                rq2.ImContrato.Procedencia = localidadString;
                rq2.ImContrato.Centro = contrato.Destino.CodigoSap;
                rq2.ImContrato.Clasificacion = contrato.Clasificacion.Descripcion;
                rq2.ImContrato.IndOpCanje = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "";
                rq2.ImContrato.Consignatario = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "";
                rq2.ImContrato.CondFijacion = contrato.CondicionFijacion?.CodigoSap;
                rq2.ImContrato.Camiones = cantidadCamiones;
                rq2.ImContrato.Confirma = contrato.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA ? "X" : "";

                BolsaCompraNet bolsaNegocio = repositorio.Obtener<BolsaCompraNet>(contrato.BolsaId);

                rq2.ImContrato.Bolsa = contrato.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA ||
                                       contrato.BoletoId == (int)EnumBoletoCompraNet.FISICO ||
                                       contrato.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA ? bolsaNegocio.CodigoSap : null;

                rq2.ImContrato.BolFisico = contrato.BoletoId == (int)EnumBoletoCompraNet.FISICO ? "X" : "";
                rq2.ImContrato.CartaOferta = contrato.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA ? "X" : "";
                rq2.ImContrato.Ninguno = contrato.BoletoId == (int)EnumBoletoCompraNet.NINGUNO ? "X" : "";
                rq2.ImContrato.SinBoleto = contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO ? "X" : "";
                rq2.ImContrato.AutCg = contrato.Warrant == true ? "X" : "";
                rq2.ImContrato.AurCd = contrato.CD == true ? "X" : "";
                rq2.ImContrato.PagoDirVend = contrato.PagoDirectoVendedor == true ? "X" : "";
                rq2.ImContrato.EstabPropio = contrato.EstablecimientoPropio == true ? "X" : "";
                rq2.ImContrato.EstabArrendado = contrato.EstablecimientoPropio == false ? "X" : "";
                rq2.ImContrato.ImporteSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                rq2.ImContrato.MonedaSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                rq2.ImContrato.PorcSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                rq2.ImContrato.ImporteAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                rq2.ImContrato.MonedaAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                rq2.ImContrato.PorcAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                rq2.ImContrato.MercDescargada = contrato.MercsDeposito == true ? "X" : "";
                rq2.ImContrato.ObservacionCal1 = contrato.Observacion;
                rq2.ImContrato.CuitCorredor = contrato.Corredor != null ? contrato.Corredor.CUIT : "";
                rq2.ImContrato.PorcComision = contrato.Corredor != null ? contrato.PorcentajeComision.Value : 0;
                rq2.ImContrato.Contrcorr = contrato.ContratoCorredor ?? "";
                rq2.ImContrato.Contrvend = contrato.ContratoVendedor ?? "";
                rq2.ImContrato.SelCargoMoa = contrato.SelCargoMOA == true ? "X" : "";
                rq2.ImContrato.SelCargoVend = contrato.SelCargoVendedor == true ? "X" : "";
                rq2.ImContrato.ContratoMadre = contrato.ContratoMadre ?? "";
                rq2.ImContrato.Creador = contrato.ComercialCreador != null ? contrato.ComercialCreador.IdActiveDirectory : "";
                rq2.ImContrato.Zona = contrato.Zona != null ? contrato.Zona.CodigoSap : "";
                rq2.ImContrato.Compensacion = contrato.Compensacion == true ? "X" : "";
                rq2.ImContrato.FleteNivel = contrato.NivelTarifa != null ? contrato.NivelTarifa.CodigoSap : "";
                rq2.ImContrato.FleteTarifa = contrato.TarifaFlete ?? 0;
                rq2.ImContrato.FechaCierta = contrato.FechaCierta.HasValue ? contrato.FechaCierta.Value.ToString("yyyy-MM-dd") : null;
                rq2.ImContrato.Porcparcial = contrato.PorcentajeDePago ?? (contrato.MaterialId == (int)EnumMateriales.TRIGO ? (decimal)95.0 : (decimal)97.5);
                rq2.ImContrato.AgenteCompra = contrato.TipoAgenteCompraId == 1 ? "9952569841" : "";
                rq2.ImContrato.Caratula = contrato.CaratulaMAT;
                rq2.ImContrato.CaratulaExt = contrato.CaratulaExtension;
                rq2.ImContrato.PrecioComMat = contrato.PrecioAjusteComision ?? 0;
                rq2.ImContrato.MonedaComMat = contrato.MonedaAjusteComisionId;
                rq2.ImContrato.FechaCreacion = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.Fecha.ToString("yyyy-MM-dd") : repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd");
                rq2.ImContrato.Zlsch = contrato.ChequeElectronico == true ? "=" : "";
                rq2.ImContrato.DolExpress = contrato.DolarizadoExpress == true ? "X" : "";
                rq2.ImContrato.CuentaMrp = contrato.PagoCBU != null ? contrato.PagoCBU.Split('-')[0] : "";
                rq2.ImContrato.PlantaDest = contrato.PlantaDestino != null ? contrato.PlantaDestino.CodigoSap : contrato.Destino.CodigoSap;
                rq2.ImContrato.Canje = contrato.Canje == true ? "X" : "";
                rq2.ImContrato.DescInsumos = contrato.Insumo;
                rq2.ImContrato.MonedaDeuda = contrato.MonedaCanjeId == "USDM " ? "USD" : contrato.MonedaCanjeId;
                rq2.ImContrato.MontoDeuda = contrato.Monto ?? 0;
                rq2.ImContrato.PosicionCbot = contrato.PosicionCBOT ?? "";
                rq2.ImContrato.FijCbotMat = contrato.TipoPosicionCBOTId.HasValue ? contrato.TipoPosicionCBOTId.ToString() : "";
                rq2.ImContrato.Tercero = contrato.ProveedorCreadorId != null ? "X" : "";
                rq2.ImContrato.AnulaYReemp = contrato.AnulaYReemplazaContratoId == null ? "" : contrato.AnulaYReemplazaContrato.ContratoSAP;
                rq2.ImContrato.Condicional = contrato.Condicional == true ? "X" : "";
                rq2.ImContrato.FechaCond = contrato.CondicionalFecha != null ? contrato.CondicionalFecha.Value.ToString("yyyy-MM-dd") : "";
                rq2.ImContrato.MesCondMat = contrato.CondicionalPosicion ?? "";
                rq2.ImContrato.MonedaCond = contrato.CondicionalMonedaId ?? "";
                rq2.ImContrato.PrecioCond = contrato.CondicionalPrecio ?? 0;
                rq2.ImContrato.ContratoCond = contrato.CondicionalContrato != null ? contrato.CondicionalContrato.ContratoSAP : "";
                rq2.ImContrato.CantidadCond = contrato.CondicionalCantidad != null ? Convert.ToDecimal(contrato.CondicionalCantidad.Value) : 0;
                rq2.ImContrato.CondPago = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "04" : "";
                rq2.ImContrato.PorcMulta = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "10" : "";
                rq2.ImContrato.TolInf = contrato.CantidadCamiones > 0 ? 0 : 3;
                rq2.ImContrato.TolSup = contrato.CantidadCamiones > 0 ? 0 : 3;
                rq2.ImContrato.Pizarra = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "ROS" : "";

                string codigoTC = DevolverTipoCambioSAP(contrato.TipoNegocioId, contrato.MonedaId, contrato.TipoAgenteCompraId, contrato.Fecha);
                rq2.ImContrato.CodigoTc = codigoTC;
                logger.Info($"FINALIZA NegocioId: {contrato.Id} - CODIGO_TC: {rq2.ImContrato.CodigoTc} - TipoDeCambioId: {contrato.TipoDeCambioId}");

                rq2.ImContrato.Bloqueo = "";
                rq2.ImContrato.TipoCambioFijo = 0;
                rq2.ImContrato.Posicion = CalcularPosicion(contrato.FechaDesde);
                rq2.ImTopesFij = topesFijacion.ToArray();
                rq2.ImDescBonif = listaDescuentos.ToArray();
                rq2.ImCalidad = listaCalidades.ToArray();
                rq2.ImTipoNegocio = contrato.Madre == true ? "MADRE" : contrato.Madre == false ? "HIJO" : contrato.EsFason == true ? "FASON" : contrato.PrestamoDevolucion == true ? "PRESTAMO_DEVOLUCION" : contrato.Venta == true ? "VENTA" : contrato.TipoNegocio.Descripcion;
                rq2.ImApertura = listaApertura.ToArray();
                rq2.ImServicios = servicioSap.ToArray();

                logger.Debug(rq2.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq2.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.ZMprfcPreSlip(rq2);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                if (devolucion.ExMensajeError != null && devolucion.ExMensajeError != "")
                {
                    throw new Exception(devolucion.ExMensajeError);
                }
                //transaction.Complete();
                return devolucion.ExContratoSap;

                //}
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP: No se pudo finalizar el contrato.", e);
                throw;
            }
        }

        public string FinalizarContratoConPI(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {
            descuentoBonificacion = descuentoBonificacion ?? new List<DescuentoBonificacion>();
            var servicios = contrato.Servicios ?? new List<Servicio>();
            calidad = calidad ?? new List<Calidad>();
            logger.Debug("Finalizando Contrato Nro: " + contrato.Id);

            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                var resp = "";
                resp = repositorio.ObtenerMayor<Negocio, int>(x => x.ContratoSAP != null && x.ContratoSAP != "", x => x.Id).ContratoSAP;
                long.TryParse(resp, out long numsap);
                numsap++;
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
                            FEDESDE = descBon.FechaDesde?.ToString("yyyy-MM-dd"),
                            FEHASTA = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                            IMPORTE_DB = Math.Round(descBon.Importe * (1 + (descBon.Porcentaje / 100)), 2),
                            MONEDA_DB = descBon.MonedaId ?? "",
                            MONEDA = descBon.MonedaId ?? "",
                            PORC_DB = descBon.Porcentaje
                        }
                        );
                    }
                    ;
                }
                decimal? precioNetoSustentable = null;

                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId.HasValue)
                {
                    listaDescuentos.Add(new ZMPES5290
                    {
                        TIPO_PERIODO = "I",
                        TIPO_DB = "B",
                        FEDESDE = contrato.FechaDesdeSustentable.HasValue ? contrato.FechaDesdeSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaDesde.ToString("yyyy-MM-dd"),
                        FEHASTA = contrato.FechaHastaSustentable.HasValue ? contrato.FechaHastaSustentable.Value.ToString("yyyy-MM-dd") : contrato.FechaHasta.ToString("yyyy-MM-dd"),
                        IMPORTE_DB = contrato.TarifaAConvenir == true ? -1 : contrato.SustentableTipoDBId == 1 ? 0 : contrato.ImporteSustentable.Value,
                        MONEDA_DB = contrato.MonedaSustentableId,
                        PORC_DB = 0,
                        PRECIO = 0
                    });
                    if (contrato.TipoNegocioId == 2 && contrato.SustentableTipoDBId == 1) //a precio y sobre precio
                    {
                        precioNetoSustentable = PrecioNetoSustentableSobrePrecio(contrato, precioNetoSustentable);
                    }
                }

                string typeOfRate = contrato.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND ? "Z" : "M";

                if (contrato.PrecioPactado != null && contrato.PrecioPactado.Count > 0)
                {
                    var tipoCambio = decimal.Round(tipoCambioAgent.TraerTipoDeCambio(DateTime.Now.Date, typeOfRate), 2, MidpointRounding.AwayFromZero);
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
                logger.Debug("Descuentos: " + descuentoBonificacion.ToJson());
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
                logger.Debug("Calidades: " + calidad.ToJson());

                var listaApertura = new List<ZMPES5440>();
                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.TarifaAConvenir != true && contrato.SustentableTipoDBId == 1) //bonificación sobre precio
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
                            MONEDA = contrato.TipoNegocioId == 2 ? contrato.Moneda?.MonedaId : apertura.MonedaId,
                            PORC = apertura.Porcentaje
                        });
                    }
                }

                logger.Debug("Apertura: " + contrato.AperturaPrecio.ToJson());
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
                    (contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho && x.Importe != 0) ||
                    contrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && x.Porcentaje > 0)))
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
                if ((contrato.EPA || contrato.EUDR || contrato.Sustentable) && contrato.SustentableTipoDBId == 1)
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
                logger.Debug("AperturaPrecio: " + contrato.AperturaPrecio.ToJson());
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
                    });
                }
                logger.Debug($"Servicios a enviar a SAP para NegocioID {contrato.Id}: {servicioSap.ToXml()}");

                var descuentoGeneralFueraPrecio = descuentoBonificacion.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado?.ToString("yyyy-MM-dd");
                //string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = calidad.Count > 0 ? "4" : "1";

                string localidadString = RellenarEspaciosSAP(contrato.Localidad.CodLocalidad, 5);
                decimal cantidadCamiones = Convert.ToDecimal(contrato.CantidadCamiones ?? 0);
                var cantidadAbsoluta = Math.Abs(contrato.Cantidad);

                topesFijacion.Add(new ZMPES5280
                {
                    FE_DESDE = contrato.TipoNegocioId == 1 && contrato.DesdeFijacion.HasValue ? contrato.DesdeFijacion.Value.ToString("yyyy-MM-dd") : "",
                    FE_HASTA = contrato.TipoNegocioId == 1 && contrato.HastaFijacion.HasValue ? contrato.HastaFijacion.Value.ToString("yyyy-MM-dd") : "",
                    HORAACT = "00:00:00",
                    CANT_MAX = contrato.TipoNegocioId == 1 ?
                       cantidadAbsoluta < 30000 ?
                       Convert.ToDecimal(cantidadAbsoluta) :
                       (cantidadAbsoluta >= 30000 && cantidadAbsoluta <= 100000) ? 30000
                       : Convert.ToDecimal(contrato.KgMaximo) : 0,
                    CANT_MIN = contrato.TipoNegocioId == 1 ? cantidadAbsoluta < 30000 ? Convert.ToDecimal(cantidadAbsoluta) : 30000 : 0,
                    FECHAACT = contrato.ContratoAcuerdoId == null || contrato.ContratoAcuerdoId == 0 ? contrato.Fecha.ToString("yyyy-MM-dd") :
                       repositorio.Obtener<ContratoAcuerdo, DateTime>(x => x.Id == contrato.ContratoAcuerdoId, x => x.Fecha).ToString("yyyy-MM-dd"),
                    VALOR = "KG"

                });
                logger.Debug("Cargando contrato");
                var rq2 = new Z_MPRFC_PRE_SLIP
                {
                    IM_CONTRATO = new ZMPES5270()
                };
                rq2.IM_CONTRATO.CANTIDAD = Convert.ToDecimal(cantidadAbsoluta);
                rq2.IM_CONTRATO.CONTR_DATAAGRO = contrato.Id.ToString();
                rq2.IM_CONTRATO.COSECHA = contrato.Campana.Descripcion;
                rq2.IM_CONTRATO.DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0";
                rq2.IM_CONTRATO.FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd");
                rq2.IM_CONTRATO.FECHA_ENTREGA = contrato.FechaEntrega.Value.ToString("yyyy-MM-dd");
                rq2.IM_CONTRATO.FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd");
                rq2.IM_CONTRATO.FECHA_LIMITE = fechaDolarizadoString;
                rq2.IM_CONTRATO.GRUPO_COMPRAS = contrato.TipoAgenteCompraId == 1 ? "902" : "";
                rq2.IM_CONTRATO.MONEDA = contrato.Pizarra == true ? "ARP  " : contrato.MonedaId;
                rq2.IM_CONTRATO.NO_INFORMAR_SIO = noInformaSioString;
                rq2.IM_CONTRATO.PAGO_DIFERIDO = (contrato.Dolarizado == true || contrato.DolarizadoCorredor == true) ? "X" : "";
                rq2.IM_CONTRATO.MATERIAL = contrato.Material.Codigo;
                rq2.IM_CONTRATO.PAGO_DIF_ARP = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "";
                rq2.IM_CONTRATO.PRECIO_PIZARRA = contrato.Precio;
                rq2.IM_CONTRATO.PRECIO = (contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : (contrato.PrecioNeto.HasValue && contrato.PrecioNeto > 0) ? contrato.PrecioNeto.Value : contrato.Precio;
                rq2.IM_CONTRATO.PROVEEDOR = contrato.Proveedor.CUIT;
                rq2.IM_CONTRATO.PROVINCIA = contrato.ProvinciaId.ToString();
                rq2.IM_CONTRATO.SUSTENTABLE = contrato.Sustentable || contrato.EPA || contrato.EUDR ? "X" : "";
                rq2.IM_CONTRATO.EPA = contrato.EPA ? "X" : "";
                rq2.IM_CONTRATO.EUDR = contrato.EUDR && !contrato.EPA ? "X" : "";
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
                rq2.IM_CONTRATO.PORCPARCIAL = contrato.PorcentajeDePago ?? (contrato.MaterialId == (int)EnumMateriales.TRIGO ? (decimal)95.0 : (decimal)97.5);
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
                rq2.IM_CONTRATO.MONTO_DEUDA = contrato.Monto ?? 0;
                rq2.IM_CONTRATO.POSICION_CBOT = contrato.PosicionCBOT ?? "";
                rq2.IM_CONTRATO.FIJ_CBOT_MAT = contrato.TipoPosicionCBOTId.HasValue ? contrato.TipoPosicionCBOTId.ToString() : "";
                rq2.IM_CONTRATO.TERCERO = contrato.ProveedorCreadorId != null ? "X" : "";
                rq2.IM_CONTRATO.ANULA_Y_REEMP = contrato.AnulaYReemplazaContratoId == null ? "" : contrato.AnulaYReemplazaContrato.ContratoSAP;
                rq2.IM_CONTRATO.CONDICIONAL = contrato.Condicional == true ? "X" : "";
                rq2.IM_CONTRATO.FECHA_COND = contrato.CondicionalFecha != null ? contrato.CondicionalFecha.Value.ToString("yyyy-MM-dd") : "";
                rq2.IM_CONTRATO.MES_COND_MAT = contrato.CondicionalPosicion ?? "";
                rq2.IM_CONTRATO.MONEDA_COND = contrato.CondicionalMonedaId ?? "";
                rq2.IM_CONTRATO.PRECIO_COND = contrato.CondicionalPrecio ?? 0;
                rq2.IM_CONTRATO.CONTRATO_COND = contrato.CondicionalContrato != null ? contrato.CondicionalContrato.ContratoSAP : "";
                rq2.IM_CONTRATO.CANTIDAD_COND = contrato.CondicionalCantidad != null ? Convert.ToDecimal(contrato.CondicionalCantidad.Value) : 0;
                rq2.IM_CONTRATO.COND_PAGO = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "04" : "";
                rq2.IM_CONTRATO.PORC_MULTA = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "10" : "";
                rq2.IM_CONTRATO.TOL_INF = contrato.CantidadCamiones > 0 ? 0 : 3;
                rq2.IM_CONTRATO.TOL_SUP = contrato.CantidadCamiones > 0 ? 0 : 3;
                rq2.IM_CONTRATO.PIZARRA = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "ROS" : "";

                string codigoTC = DevolverTipoCambioSAP(contrato.TipoNegocioId, contrato.MonedaId, contrato.TipoAgenteCompraId, contrato.Fecha);
                rq2.IM_CONTRATO.CODIGO_TC = codigoTC;
                logger.Info($"FINALIZA NegocioId: {contrato.Id} - CODIGO_TC: {rq2.IM_CONTRATO.CODIGO_TC} - TipoDeCambioId: {contrato.TipoDeCambioId}");

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
                logger.Error("Error comunicacion SAP: No se pudo finalizar el contrato.", e);
                throw;
            }
        }

        public string Finalizar(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {
            string contratoSAP = string.Empty;
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                contratoSAP = FinalizarContratoSinPI(contrato, descuentoBonificacion, calidad);
            }
            else
            {
                contratoSAP = FinalizarContratoConPI(contrato, descuentoBonificacion, calidad);
            }
            return contratoSAP;
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

        public string DevolverTipoCambioSAP(int tipoNegocioId, string monedaId, int? tipoAgenteCompraId, DateTime fecha, bool? modifica = false)
        {
            string CargaDesdeBLEND = ConfigurationManager.AppSettings["CargaDesdeBLEND"];
            string CargaHastaBLEND = ConfigurationManager.AppSettings["CargaHastaBLEND"];
            string codigoTC;

            if ((modifica == false && fecha >= DateTime.Parse(CargaDesdeBLEND) && ConfigurationManager.AppSettings["ActivarBLEND"] == "Si") ||
                (modifica == true && (ConfigurationManager.AppSettings["ActivarBLEND"] == "Si" && fecha >= DateTime.Parse(CargaDesdeBLEND) ||
                    (ConfigurationManager.AppSettings["ActivarBLEND"] == "No" && fecha >= DateTime.Parse(CargaDesdeBLEND) && CargaHastaBLEND != "" && fecha < DateTime.Parse(CargaHastaBLEND)))))
            {
                codigoTC = tipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && monedaId == "USDM " && tipoAgenteCompraId == null ? "04" :
                    tipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && tipoAgenteCompraId == null ? "04" :
                    tipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && monedaId == "USDM " && tipoAgenteCompraId != null ? "03" : "";
            }
            else
            {
                codigoTC = tipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && monedaId == "USDM " && tipoAgenteCompraId == null ? "02" :
                    tipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && monedaId == "USDM " && tipoAgenteCompraId != null ? "03" : "";
            }

            return codigoTC;
        }
    }
}