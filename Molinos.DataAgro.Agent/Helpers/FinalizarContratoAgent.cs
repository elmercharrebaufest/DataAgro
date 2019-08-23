using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.FinalizarContrato;
using Molinos.DataAgro.Entities.Dto;
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
    public class FinalizarContratoAgent : IFinalizarContratoAgent
    {
        private readonly IRepositorio repositorio;
        public FinalizarContratoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Finalizar(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {
            try
            {
                SI_ZMPWS_DATAAGRO_PRE_SLIPClient agent = new SI_ZMPWS_DATAAGRO_PRE_SLIPClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                logger.Debug("Finalizando Contrato Nro: " + contrato.ContratoId);
                var listaDescuentos = new List<ZMPES5290>();
                foreach (var descBon in descuentoBonificacion)
                {
                    if (descBon.TipoPeriodoDBId != 1)
                    {
                        listaDescuentos.Add(new ZMPES5290
                        {
                            TIPO_PERIODO = descBon.TipoPeriodoDB.CodigoSap,
                            TIPO_DB = descBon.TipoDB.CodigoSap,
                            FEDESDE = descBon.FechaDesde != null ? descBon.FechaDesde.Value.ToString("yyyy-MM-dd") : null,
                            FEHASTA = descBon.FechaHasta != null ? descBon.FechaHasta.Value.ToString("yyyy-MM-dd") : null,
                            IMPORTE_DB = descBon.Importe,
                            MONEDA_DB = descBon.Moneda.MonedaId ?? "",
                            PORC_DB = descBon.Porcentaje
                        }
                        );
                    };
                }
                if (contrato.ImporteSustentable != null && contrato.ImporteSustentable != 0)
                {
                    listaDescuentos.Add(new ZMPES5290
                    {
                        TIPO_PERIODO = "I",
                        TIPO_DB = "B",
                        FEDESDE = contrato.FechaDesde != null ? contrato.FechaDesde.ToString("yyyy-MM-dd") : null,
                        FEHASTA = contrato.FechaHasta != null ? contrato.FechaHasta.ToString("yyyy-MM-dd") : null,
                        IMPORTE_DB = contrato.ImporteSustentable.Value,
                        MONEDA_DB = contrato.MonedaSustentableId,
                        PORC_DB = 0
                    });
                }
                logger.Debug("Descuentos: " + descuentoBonificacion);
                var listaCalidades = new List<ZMPES5300>();
                foreach (var cal in calidad)
                {
                    if (cal.StandardDeCalidadId == 2)
                    {
                        if ((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecial.Id == 4 || cal.CalidadEspecial.Id == 5))
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
                }
                logger.Debug("Calidades: " + calidad);
                var listaApertura = new List<ZMPES5440>();

                foreach (AperturaPrecio apertura in contrato.AperturaPrecio)
                {
                    if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                    {
                        listaApertura.Add(new ZMPES5440
                        {
                            CONCEPTO = apertura.ConceptoAperturaPrecio.CodigoSap,
                            IMPORTE = apertura.Importe,
                            MONEDA = contrato.Moneda != null ? contrato.Moneda.MonedaId : null,
                            PORC = apertura.Porcentaje
                        });
                    }
                }
                logger.Debug("Apertura: " + contrato.AperturaPrecio);
                var descuentoGeneralSobrePrecio = descuentoBonificacion.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                var descuentoGeneralFueraPrecio = descuentoBonificacion.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado != null ? contrato.FechaDolarizado.Value.ToString("yyyy-MM-dd") : null;
                string pagoDiferidoString = contrato.FechaDolarizado != null ? "X" : "";
                string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = calidad.Count > 0 ? "4" : "1";

                string localidadString = RellenarEspaciosSAP(contrato.Localidad.CodLocalidad, 5);
                decimal cantidadCamiones = Convert.ToDecimal(contrato.CantidadCamiones ?? 0);
                logger.Debug("Cargando contrato");
                var rq = new Z_MPRFC_PRE_SLIP()
                {
                    IM_CONTRATO = new ZMPES5270
                    {
                        CANTIDAD = Convert.ToDecimal(contrato.Cantidad),
                        CONTR_DATAAGRO = contrato.ContratoId.ToString(),
                        COSECHA = contrato.Campana.Descripcion,
                        DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0",
                        FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd"),
                        FECHA_ENTREGA = contrato.FechaEntrega.ToString("yyyy-MM-dd"),
                        FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd"),
                        FECHA_LIMITE = fechaDolarizadoString,
                        GRUPO_COMPRAS = "",
                        MONEDA = contrato.Moneda?.MonedaId,
                        NO_INFORMAR_SIO = noInformaSioString,
                        PAGO_DIFERIDO = pagoDiferidoString,
                        MATERIAL = contrato.Material.Codigo,
                        PAGO_DIF_ARP = contrato.PagoDiferido.HasValue&& contrato.PagoDiferido.Value? "X":"",
                        PRECIO_PIZARRA = contrato.Precio,
                        PRECIO = contrato.PrecioNeto ?? contrato.Precio,
                        PROVEEDOR = contrato.Proveedor.CUIT,
                        PROVINCIA = contrato.ProvinciaId.ToString(),
                        SUSTENTABLE = sustentableString,
                        ESPECIAL = contrato.StandardDeCalidad != null? contrato.StandardDeCalidad.CodigoSap: especialString,
                        FECHA = contrato.Fecha.ToString("yyyy-MM-dd"),
                        USUARIO = contrato.Comercial.IdActiveDirectory,
                        HORAACT = contrato.Fecha.ToString("HH:mm:ss"),
                        PROCEDENCIA = localidadString,
                        CENTRO = contrato.Destino.CodigoSap,
                        CLASIFICACION = contrato.Clasificacion.Descripcion,
                        IND_OP_CANJE = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "",
                        CONSIGNATARIO = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "",
                        COND_FIJACION = contrato.CondicionFijacion?.CodigoSap,
                        CAMIONES = cantidadCamiones,
                        CONFIRMA = contrato.BoletoId == 1 ? "X" : "",
                        BOLSA = contrato.BoletoId == 1 || contrato.BoletoId == 2 || contrato.BoletoId == 4 ? contrato.Bolsa.CodigoSap : null,
                        BOL_FISICO = contrato.BoletoId == 2 ? "X" : "",
                        CARTA_OFERTA = contrato.BoletoId == 4 ? "X" : "",
                        NINGUNO = contrato.BoletoId == 3 ? "X" : "",
                        AUT_CG = contrato.Warrant == true ? "X" : "",
                        AUR_CD = contrato.CD == true ? "X" : "",
                        PAGO_DIR_VEND = contrato.PagoDirectoVendedor == true ? "X" : "",
                        ESTAB_PROPIO = contrato.EstablecimientoPropio == true ? "X" : "",
                        ESTAB_ARRENDADO = contrato.EstablecimientoPropio == false ? "X" : "",
                        IMPORTE_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0,
                        MONEDA_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null,
                        PORC_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0,
                        IMPORTE_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0,
                        MONEDA_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null,
                        PORC_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0,
                        MERC_DESCARGADA = contrato.MercsDeposito == true ? "X" : "",
                        OBSERVACION_CAL1 = contrato.Observacion,
                        CUIT_CORREDOR = contrato.Corredor != null ? contrato.Corredor.CUIT : "",
                        PORC_COMISION = contrato.Corredor != null ? contrato.PorcentajeComision.Value : 0,
                        CONTRCORR = contrato.ContratoCorredor ?? "",
                        CONTRVEND = contrato.ContratoVendedor ?? "",
                        SEL_CARGO_MOA = contrato.SelCargoMOA == true ? "X" : "",
                        SEL_CARGO_VEND = contrato.SelCargoVendedor == true ? "X" : "",
                        CONTRATO_MADRE = contrato.ContratoMadre ?? "",
                        CREADOR = contrato.ComercialCreador != null ? contrato.ComercialCreador.IdActiveDirectory : "",
                        ZONA = contrato.Zona != null ? contrato.Zona.CodigoSap:"",
                        COMPENSACION = contrato.Compensacion == true ? "X" : "",
                        FLETE_NIVEL = contrato.NivelTarifa!= null? contrato.NivelTarifa.CodigoSap : "" ,
                        FLETE_TARIFA = contrato.TarifaFlete ?? 0,
                    },
                    IM_TOPES_FIJ = new ZMPES5280
                    {
                        FE_DESDE = contrato.DesdeFijacion?.ToString("yyyy-MM-dd"),
                        FE_HASTA = contrato.HastaFijacion?.ToString("yyyy-MM-dd")
                    },
                    IM_DESC_BONIF = listaDescuentos.ToArray(),
                    IM_CALIDAD = listaCalidades.ToArray(),
                    IM_TIPO_NEGOCIO = contrato.Madre == true ? "MADRE" : contrato.Madre == false ? "HIJO" : contrato.TipoNegocio.Descripcion,
                    IM_APERTURA = listaApertura.ToArray()

                };
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.SI_ZMPWS_DATAAGRO_PRE_SLIP(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                if (devolucion.EX_MENSAJE_ERROR != null && devolucion.EX_MENSAJE_ERROR != "")
                {
                    throw new Exception(devolucion.EX_MENSAJE_ERROR);
                }

                return devolucion.EX_CONTRATO_SAP;
            }
            catch (Exception e)
            {
                logger.Error("Error comunicacion SAP", e);
                throw e;
            }

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
    }
}
