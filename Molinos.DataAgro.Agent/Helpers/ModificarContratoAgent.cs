using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ModificarContratoFinalizado;
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
    public class ModificarContratoAgent : IModificarContratoAgent
    {
        private readonly IRepositorio repositorio;
        public ModificarContratoAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;

        public string Modificar(Contrato contrato)
        {
            try
            {
                SI_ZMPWS_DATAAGRO_MODIFICAR_CONTRATOClient agent = new SI_ZMPWS_DATAAGRO_MODIFICAR_CONTRATOClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                logger.Debug("Finalizando Contrato Nro: " + contrato.ContratoId);
                var listaDescuentos = new List<ZMPES5290>();
                var contratoGuardado = repositorio.Obtener<Contrato>(contrato.ContratoId);
                var descModificado = false;
                var listaMonedas = repositorio.Listar<Moneda>();
                foreach (var descBon in contratoGuardado.Descuentos)
                {
                    if (!contrato.Descuentos.Any(x => x.TipoPeriodoDBId == descBon.TipoPeriodoDBId
                     && x.Importe == descBon.Importe && x.MonedaId == descBon.MonedaId
                     && x.Porcentaje == descBon.Porcentaje && x.TipoDBId == descBon.TipoDBId))
                    {
                        descModificado = true;
                        break;
                    }
                }

                foreach (var descBon in contratoGuardado.Descuentos)
                {
                    if (descBon.TipoPeriodoDBId != 1)
                    {
                        var listaPeriodo = repositorio.Listar<TipoPeriodoDB>();
                        var listaTipo = repositorio.Listar<TipoDB>();
                        listaDescuentos.Add(new ZMPES5290
                        {
                            TIPO_PERIODO = listaPeriodo.FirstOrDefault(x => x.Id == descBon.TipoPeriodoDBId).CodigoSap,
                            TIPO_DB = listaTipo.FirstOrDefault(x => x.Id == descBon.TipoDBId).CodigoSap,
                            FEDESDE = descBon.FechaDesde?.ToString("yyyy-MM-dd"),
                            FEHASTA = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                            IMPORTE_DB = descBon.Importe,
                            MONEDA_DB = listaMonedas.Any(x => x.MonedaId == descBon.MonedaId) ? 
                            listaMonedas.FirstOrDefault(x => x.MonedaId == descBon.MonedaId).MonedaId : "",
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
                
                var calModificado = false;
                foreach (var cal in contratoGuardado.Calidad)
                {
                    if (!contrato.Calidad.Any(x => x.Valor == cal.Valor
                    && x.StandardDeCalidadId == cal.StandardDeCalidadId
                    && x.CalidadEspecialId == cal.CalidadEspecialId
                    && x.PorcentajeDesde == cal.PorcentajeDesde && x.PorcentajeHasta == cal.PorcentajeHasta))
                    {
                        calModificado = true;
                        break;
                    }
                }

                var listaCalidades = new List<ZMPES5300>();
                foreach (var cal in contratoGuardado.Calidad)
                {
                    var listaCalidadEspecial = repositorio.Listar<CalidadEspecial>();
                    if (cal.StandardDeCalidadId == 2)
                    {
                        if ((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecial.Id == 4 || cal.CalidadEspecial.Id == 5))
                        {
                            listaCalidades.Add(new ZMPES5300
                            {
                                CODIGO = listaCalidadEspecial.FirstOrDefault(x=> x.Id == cal.CalidadEspecialId).CodigoSap,
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
                                    CODIGO = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                                    CODIGO = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                            CODIGO = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
                            VALOR = 0,
                            PORC_DESDE = 1,
                            PORC_HASTA = 1
                        });
                    }
                }         
                logger.Debug("Calidades: " + contrato.Calidad);
                var apModificado = false;
                foreach (var ap in contratoGuardado.AperturaPrecio)
                {
                    if (!contrato.AperturaPrecio.Any(x => x.Importe == ap.Importe
                    && x.MonedaId == ap.MonedaId
                    && x.Porcentaje == ap.Porcentaje
                    && x.ConceptoAperturaPrecioId == ap.ConceptoAperturaPrecioId))
                    {
                        apModificado = true;
                        break;
                    }
                }
                var listaApertura = new List<ZMPES5440>();
                if (contrato.TipoNegocioId == 2)
                {
                    var listaAperturaPrecios = repositorio.Listar<ConceptoAperturaPrecio>();
                    foreach (AperturaPrecio apertura in contratoGuardado.AperturaPrecio)
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            listaApertura.Add(new ZMPES5440
                            {
                                CONCEPTO = listaAperturaPrecios.FirstOrDefault(x => x.Id == apertura.ConceptoAperturaPrecioId).CodigoSap,
                                IMPORTE = apertura.Importe,
                                MONEDA = contrato.Moneda?.MonedaId,
                                PORC = apertura.Porcentaje
                            });
                        }
                    }
                }
                
                logger.Debug("Apertura: " + contratoGuardado.AperturaPrecio);
                var descuentoGeneralSobrePrecio = contratoGuardado.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                var descuentoGeneralFueraPrecio = contratoGuardado.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado?.ToString("yyyy-MM-dd");
                string pagoDiferidoString = contrato.FechaDolarizado != null ? "X" : "";
                string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = contratoGuardado.Calidad.Count > 0 ? "4" : "1";

                var localidad = repositorio.Obtener<Localidad>(contrato.LocalidadId);
                string localidadString = RellenarEspaciosSAP(localidad.CodLocalidad, 5);
                decimal cantidadCamiones = Convert.ToDecimal(contrato.CantidadCamiones ?? 0);
                logger.Debug("Cargando contrato");
                var conModificado =
                    contratoGuardado.BoletoId != contrato.BoletoId ||
                    contratoGuardado.BolsaId != contrato.BolsaId ||
                    contratoGuardado.CampanaId != contrato.CampanaId ||
                    contratoGuardado.Cantidad != contrato.Cantidad ||
                    contratoGuardado.CantidadCamiones != contrato.CantidadCamiones ||
                    contratoGuardado.CD != contrato.CD ||
                    contratoGuardado.ClasificacionId != contrato.ClasificacionId ||
                    contratoGuardado.ComercialCreadorId != contrato.ComercialCreadorId ||
                    contratoGuardado.ComercialId != contrato.ComercialId ||
                    contratoGuardado.Compensacion != contrato.Compensacion ||
                    contratoGuardado.CondicionFijacionId != contrato.CondicionFijacionId ||
                    contratoGuardado.Consignatario != contrato.Consignatario ||
                    contratoGuardado.ContratoAcuerdoId != contrato.ContratoAcuerdoId ||
                    contratoGuardado.ContratoCorredor != contrato.ContratoCorredor ||
                    contratoGuardado.ContratoMadre != contrato.ContratoMadre ||
                    contratoGuardado.ContratoVendedor != contrato.ContratoVendedor ||
                    contratoGuardado.CorredorId != contrato.CorredorId ||
                    contratoGuardado.DestinoId != contrato.DestinoId ||
                    contratoGuardado.DiasPesificado != contrato.DiasPesificado ||
                    contratoGuardado.Dolarizado != contrato.Dolarizado ||
                    contratoGuardado.EstablecimientoPropio != contrato.EstablecimientoPropio ||
                    contratoGuardado.FechaDesde != contrato.FechaDesde ||
                    contratoGuardado.FechaDolarizado != contrato.FechaDolarizado ||
                    contratoGuardado.FechaEntrega != contrato.FechaEntrega ||
                    contratoGuardado.FechaHasta != contrato.FechaHasta ||
                    contratoGuardado.GrupoCompra != contrato.GrupoCompra ||
                    contratoGuardado.ImporteSustentable != contrato.ImporteSustentable ||
                    contratoGuardado.LocalidadId != contrato.LocalidadId ||
                    contratoGuardado.Madre != contrato.Madre ||
                    contratoGuardado.MaterialId != contrato.MaterialId ||
                    contratoGuardado.MercsDeposito != contrato.MercsDeposito ||
                    contratoGuardado.MonedaId != contrato.MonedaId ||
                    contratoGuardado.MonedaSustentableId != contrato.MonedaSustentableId ||
                    contratoGuardado.NivelTarifaId != contrato.NivelTarifaId ||
                    contratoGuardado.NoInformaSio != contrato.NoInformaSio ||
                    contratoGuardado.Observacion != contrato.Observacion ||
                    contratoGuardado.PagoDiferido != contrato.PagoDiferido ||
                    contratoGuardado.PagoDirectoVendedor != contrato.PagoDirectoVendedor ||
                    contratoGuardado.Pizarra != contrato.Pizarra ||
                    contratoGuardado.PlanCanje != contrato.PlanCanje ||
                    contratoGuardado.PorcentajeComision != contrato.PorcentajeComision ||
                    contratoGuardado.Precio != contrato.Precio ||
                    contratoGuardado.PrecioNeto != contrato.PrecioNeto ||
                    contratoGuardado.ProveedorId != contrato.ProveedorId ||
                    contratoGuardado.ProvinciaId != contrato.ProvinciaId ||
                    contratoGuardado.SelCargoMOA != contrato.SelCargoMOA ||
                    contratoGuardado.SelCargoVendedor != contrato.SelCargoVendedor ||
                    contratoGuardado.StandardDeCalidadId != contrato.StandardDeCalidadId ||
                    contratoGuardado.Sustentable != contrato.Sustentable ||
                    contratoGuardado.TarifaFlete != contrato.TarifaFlete ||
                    contratoGuardado.TrigoEspecial != contrato.TrigoEspecial ||
                    contratoGuardado.UsuarioId != contrato.UsuarioId ||
                    contratoGuardado.Warrant != contrato.Warrant ||
                    contratoGuardado.ZonaId != contrato.ZonaId;
                var rq = new Z_MPRFC_MODIFICAR_CONTRATO
                {
                    IM_CONTRATO = new ZMPES5560
                    {
                        CONTRATO = contrato.ContratoSAP,
                        DETALLE = new ZMPES5270
                        {
                            CANTIDAD = Convert.ToDecimal(contrato.Cantidad),
                            CONTR_DATAAGRO = contrato.ContratoId.ToString(),
                            COSECHA = repositorio.Obtener<Campaña, string>(x => contrato.CampanaId == x.CampañaId, x => x.Descripcion),
                            DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0",
                            FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd"),
                            FECHA_ENTREGA = contrato.FechaEntrega.ToString("yyyy-MM-dd"),
                            FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd"),
                            FECHA_LIMITE = fechaDolarizadoString,
                            GRUPO_COMPRAS = "",
                            MONEDA = repositorio.Obtener<Moneda, string>(x => contrato.MonedaId == x.MonedaId, x => x.MonedaId),                            
                            NO_INFORMAR_SIO = noInformaSioString,
                            PAGO_DIFERIDO = pagoDiferidoString,
                            MATERIAL = repositorio.Obtener<Material, string>(x => contrato.MaterialId == x.MaterialId, x => x.Codigo),
                            PAGO_DIF_ARP = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "",
                            PRECIO_PIZARRA = contrato.Precio,
                            PRECIO = contrato.PrecioNeto ?? contrato.Precio,
                            PROVEEDOR = repositorio.Obtener<Proveedor, string>(x => contrato.ProveedorId == x.ProveedorId, x => x.CUIT),
                            PROVINCIA = contrato.ProvinciaId.ToString(),
                            SUSTENTABLE = sustentableString,
                            ESPECIAL = repositorio.Obtener<StandardDeCalidad, string>(x => contrato.StandardDeCalidadId == x.Id, x => x.CodigoSap),
                            FECHA = contrato.Fecha.ToString("yyyy-MM-dd"),
                            USUARIO = repositorio.Obtener<Comercial, string>(x => contrato.ComercialId == x.ComercialId, x => x.IdActiveDirectory),
                            HORAACT = contrato.Fecha.ToString("HH:mm:ss"),
                            PROCEDENCIA = localidadString,
                            CENTRO = repositorio.Obtener<Centro, string>(x => contrato.DestinoId == x.Id, x => x.CodigoSap),
                            CLASIFICACION = repositorio.Obtener<ClasificacionCompraNet, string>(x => contrato.ClasificacionId == x.Id, x => x.Descripcion),
                            IND_OP_CANJE = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "",
                            CONSIGNATARIO = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "",
                            COND_FIJACION = contrato.CondicionFijacionId.HasValue?
                            repositorio.Obtener<CondicionFijacion, string>(x => contrato.CondicionFijacionId == x.Id, x => x.CodigoSap):"",
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
                            CUIT_CORREDOR = contrato.CorredorId.HasValue? 
                            repositorio.Obtener<Proveedor, string>(x => contrato.CorredorId == x.ProveedorId, x => x.CUIT) : "",                            
                            PORC_COMISION = contrato.CorredorId.HasValue? contrato.PorcentajeComision.Value : 0,
                            CONTRCORR = contrato.ContratoCorredor ?? "",
                            CONTRVEND = contrato.ContratoVendedor ?? "",
                            SEL_CARGO_MOA = contrato.SelCargoMOA == true ? "X" : "",
                            SEL_CARGO_VEND = contrato.SelCargoVendedor == true ? "X" : "",
                            CONTRATO_MADRE = contrato.ContratoMadre ?? "",
                            CREADOR = contrato.ComercialCreadorId.HasValue?
                            repositorio.Obtener<Comercial, string>(x => contrato.ComercialCreadorId == x.ComercialId, x => x.IdActiveDirectory): "",
                            ZONA = contrato.ZonaId.HasValue ? 
                            repositorio.Obtener<Zona, string>(x => contrato.ZonaId == x.Id, x => x.CodigoSap) : "",
                            COMPENSACION = contrato.Compensacion == true ? "X" : "",
                            FLETE_NIVEL = contrato.NivelTarifaId.HasValue?
                            repositorio.Obtener<NivelTarifa, string>(x => contrato.NivelTarifaId == x.Id, x => x.CodigoSap) : "",
                            FLETE_TARIFA = contrato.TarifaFlete ?? 0,
                        }
                    }
                };
                var topFija =
                    contratoGuardado.DesdeFijacion != contrato.DesdeFijacion ||
                    contratoGuardado.HastaFijacion != contrato.HastaFijacion;
                rq.IM_TOPES_FIJ = new ZMPES5280
                {
                    FE_DESDE = contrato.DesdeFijacion?.ToString("yyyy-MM-dd"),
                    FE_HASTA = contrato.HastaFijacion?.ToString("yyyy-MM-dd")
                };
                rq.IM_MODIFICACION = new ZMPES5570
                {
                    CONTRATO = conModificado ? "X" : "",
                    APERTURA = apModificado ? "X" : "",
                    CALIDAD = calModificado ? "X" : "",
                    DESC_BONIF = descModificado ? "X" : "",
                    TOPES_FIJ = topFija ? "X" : ""
                };
                rq.IM_DESC_BONIF = listaDescuentos.ToArray();
                rq.IM_CALIDAD = listaCalidades.ToArray();
                rq.IM_APERTURA = listaApertura.ToArray();
            
                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.SI_ZMPWS_DATAAGRO_MODIFICAR_CONTRATO(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                if (devolucion.EX_MENSAJE.Contains("Error"))
                {
                    throw new Exception(devolucion.EX_MENSAJE);
                }

                return devolucion.EX_MENSAJE;
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
