using NLog;
using Molinos.DataAgro.Agent.ModificarContratoFinalizado;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
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
    public class ModificarContratoAgent : IModificarContratoAgent
    {
        private readonly IRepositorio repositorio;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly ILogger logger;
        private readonly IFinalizarContratoAgent finalizarContratoAgent;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ModificarContratoAgent(ILogger logger, IRepositorio repositorio, IDiasHabilesAgent diasHabilesAgent, IFinalizarContratoAgent finalizarContratoAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.diasHabilesAgent = diasHabilesAgent;
            this.finalizarContratoAgent = finalizarContratoAgent;
        }
        private string ModificarSinPI(Contrato contrato, Contrato contratoGuardado)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "OK";
            }
            try
            {
                Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;

                logger.Debug("Modificando Contrato Nro: " + contrato.Id);
                var listaDescuentos = new List<Zmpes5290>();
                var topesFijacion = new List<Zmpes5280>();
                var servicioSap = new List<Zmpes6620>();
                logger.Debug("Contrato Obtenido: " + contratoGuardado.Id);

                var descModificado = false;
                var listaMonedas = repositorio.Listar<Moneda>();
                var descuentosGenerales = new List<DescuentoBonificacion>();
                var descuentos = new List<DescuentoBonificacion>();
                var servicios = contrato.Servicios ?? new List<Servicio>();
                var servicioValor = repositorio.Listar<ServicioValor>();
                if (contrato.Descuentos != null)
                {
                    descuentosGenerales = contrato.Descuentos.Where(x => x.TipoPeriodoDBId == 1).ToList();
                    descuentos = contrato.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList();
                }
                if ((descuentos == null && contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count > 0) ||
                    descuentos != null && descuentos.Count != contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count ||
                    (contrato.PrecioPactado == null && contratoGuardado.PrecioPactado.ToList().Count > 0) ||
                    contrato.PrecioPactado != null && contrato.PrecioPactado.Count != contratoGuardado.PrecioPactado.Count ||
                         contrato.ImporteSustentable != contratoGuardado.ImporteSustentable ||
                         contrato.MonedaId != contratoGuardado.MonedaId)
                {
                    descModificado = true;
                }
                else
                {
                    foreach (var descBon in contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1))
                    {
                        if (!descuentos.Any(x => x.TipoPeriodoDBId == descBon.TipoPeriodoDBId
                         && x.Importe == descBon.Importe && x.MonedaId == descBon.MonedaId
                         && x.Porcentaje == descBon.Porcentaje && x.TipoDBId == descBon.TipoDBId)
                         )
                        {
                            descModificado = true;
                            break;
                        }
                    }
                    foreach (var precio in contratoGuardado.PrecioPactado)
                    {
                        if (!contrato.PrecioPactado.Any(x => x.FechaDesde == precio.FechaDesde
                        && x.FechaHasta == precio.FechaHasta
                        && x.Precio == precio.Precio
                        && x.MonedaPactadoId == precio.MonedaPactadoId
                        && x.ImportePactado == precio.ImportePactado
                        && x.MonedaImportePactadoId == precio.MonedaImportePactadoId
                        && x.Porcentaje == precio.Porcentaje))
                        {
                            descModificado = true;
                            break;
                        }
                    }
                }
                if (descuentos != null && descuentos.Count > 0)
                {
                    var listaPeriodo = repositorio.Listar<TipoPeriodoDB>();
                    var listaTipo = repositorio.Listar<TipoDB>();
                    foreach (var descBon in descuentos)
                    {
                        listaDescuentos.Add(new Zmpes5290
                        {
                            TipoPeriodo = listaPeriodo.Where(x => x.Id == descBon.TipoPeriodoDBId).Single().CodigoSap,
                            TipoDb = listaTipo.Where(x => x.Id == descBon.TipoDBId).Single().CodigoSap,
                            Fedesde = descBon.FechaDesde?.ToString("yyyy-MM-dd"),
                            Fehasta = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                            ImporteDb = descBon.Importe,
                            MonedaDb = descBon.MonedaId ?? "",
                            PorcDb = descBon.Porcentaje
                        });
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
                if (contrato.PrecioPactado != null)
                {
                    foreach (var p in contrato.PrecioPactado)
                    {
                        listaDescuentos.Add(new Zmpes5290
                        {
                            TipoPeriodo = "E",
                            TipoDb = "",
                            Fedesde = p.FechaDesde?.ToString("yyyy-MM-dd"),
                            Fehasta = p.FechaHasta?.ToString("yyyy-MM-dd"),
                            ImporteDb = p.ImportePactado ?? 0,
                            MonedaDb = p.MonedaImportePactadoId ?? "",
                            PorcDb = p.Porcentaje ?? 0,
                            Precio = p.Precio,
                            Moneda = p.MonedaPactadoId
                        });
                    }
                }
                logger.Debug("Descuentos modificados");

                var calModificado = false;
                if ((contrato.Calidad == null && contratoGuardado.Calidad.Count > 0) || contrato.Calidad != null && contratoGuardado.Calidad.Count != contrato.Calidad.Count)
                {
                    calModificado = true;
                }
                else
                {
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
                }
                var listaCalidades = new List<Zmpes5300>();
                if (contrato.Calidad != null)
                {
                    foreach (var cal in contrato.Calidad)
                    {
                        var listaCalidadEspecial = repositorio.Listar<CalidadEspecial>();
                        if (cal.StandardDeCalidadId == 2)
                        {
                            if ((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecialId == 4 || cal.CalidadEspecialId == 5))
                            {
                                listaCalidades.Add(new Zmpes5300
                                {
                                    Codigo = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                                        Codigo = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                                        Codigo = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                                Codigo = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
                                Valor = 0,
                                PorcDesde = 1,
                                PorcHasta = 1
                            });
                        }
                    }
                }

                logger.Debug("Calidades: " + contrato.Calidad);
                var apModificado = false;
                if ((contrato.AperturaPrecio == null && contratoGuardado.AperturaPrecio.Count > 0) || contrato.AperturaPrecio != null && contratoGuardado.AperturaPrecio.Count != contrato.AperturaPrecio.Count)
                {
                    apModificado = true;
                }
                else
                {
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
                }
                if (contrato.SustentableTipoDBId != contratoGuardado.SustentableTipoDBId && (contrato.SustentableTipoDBId == 1 || contratoGuardado.SustentableTipoDBId == 1))
                {
                    apModificado = true;
                }
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
                if (contrato.AperturaPrecio != null)
                {
                    var listaAperturaPrecios = repositorio.Listar<ConceptoAperturaPrecio>();
                    foreach (AperturaPrecio apertura in contrato.AperturaPrecio)
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            listaApertura.Add(new Zmpes5440
                            {
                                Concepto = listaAperturaPrecios.FirstOrDefault(x => x.Id == apertura.ConceptoAperturaPrecioId).CodigoSap,
                                Importe = apertura.Importe,
                                Moneda = contrato.TipoNegocioId == 1 ? apertura.MonedaId : contrato.MonedaId,
                                Porc = apertura.Porcentaje
                            });
                        }
                    }
                }

                logger.Debug("Apertura: " + contrato.AperturaPrecio);
                if (contrato.Descuentos == null)
                {
                    contrato.Descuentos = new List<DescuentoBonificacion>();
                }
                var apServicio = false;
                if ((contrato.Servicios == null && contratoGuardado.Servicios.Count > 0) || contrato.Servicios != null && contratoGuardado.Servicios.Count != contrato.Servicios.Count)
                {
                    apServicio = true;
                }
                else
                {
                    foreach (var ser in contratoGuardado.Servicios)
                    {
                        if (!contrato.Servicios.Any(x => x.Importe == ser.Importe))
                        {
                            apServicio = true;
                            break;
                        }
                    }
                }
                logger.Debug("Servicios: " + contrato.AperturaPrecio);
                foreach (var servicio in servicios)
                {
                    servicioSap.Add(new Zmpes6620
                    {
                        Codigo = servicioValor.Where(x => x.Id == servicio.ServicioValorId).FirstOrDefault().TipoServicio.CodigoSAP,
                        PorcDesde = servicio.Desde,
                        PorcHasta = servicio.Hasta,
                        Valor = servicio.Importe,
                        Moneda = servicio.MonedaId,
                        Fechaact = contrato.Fecha.ToString("yyyy-MM-dd"),
                        Horaact = contrato.Fecha.ToString("HH:mm:ss"),
                    }
                    );
                }
                var descuentoGeneralSobrePrecio = contrato.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();

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
                    descuentoGeneralSobrePrecio.MonedaId += contrato.MonedaSustentableId;
                }

                var descuentoGeneralFueraPrecio = contrato.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado != null ? contrato.FechaDolarizado.Value.ToString("yyyy-MM-dd") : "";
                string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = contrato.Calidad != null && contrato.Calidad.Count > 0 ? "4" : "1";

                var localidad = repositorio.Obtener<Localidad>(contrato.LocalidadId ?? 0);
                logger.Debug("Localidad obtenida");
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
                    contratoGuardado.DolarizadoCorredor != contrato.DolarizadoCorredor ||
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
                    contratoGuardado.EPA != contrato.EPA ||
                    contratoGuardado.EUDR != contrato.EUDR ||
                    contratoGuardado.TarifaAConvenir != contrato.TarifaAConvenir ||
                    contratoGuardado.TarifaFlete != contrato.TarifaFlete ||
                    contratoGuardado.TrigoEspecial != contrato.TrigoEspecial ||
                    contratoGuardado.UsuarioId != contrato.UsuarioId ||
                    contratoGuardado.Warrant != contrato.Warrant ||
                    contratoGuardado.ZonaId != contrato.ZonaId ||
                    contratoGuardado.FechaCierta != contrato.FechaCierta ||
                    contratoGuardado.PorcentajeDePago != contrato.PorcentajeDePago ||
                    contratoGuardado.TipoAgenteCompraId != contrato.TipoAgenteCompraId ||
                    contratoGuardado.CaratulaMAT != contrato.CaratulaMAT ||
                    contratoGuardado.CaratulaExtension != contrato.CaratulaExtension ||
                    contratoGuardado.PrecioAjusteComision != contrato.PrecioAjusteComision ||
                    contratoGuardado.MonedaAjusteComisionId != contrato.MonedaAjusteComisionId ||
                    contratoGuardado.ChequeElectronico != contrato.ChequeElectronico ||
                    contratoGuardado.DolarizadoExpress != contrato.DolarizadoExpress ||
                    contratoGuardado.PagoCBU != contrato.PagoCBU || contratoGuardado.Canje != contrato.Canje ||
                    contratoGuardado.Monto != contrato.Monto || contratoGuardado.Insumo != contrato.Insumo ||
                    contratoGuardado.MonedaCanjeId != contrato.MonedaCanjeId ||
                    contratoGuardado.PrestamoDevolucion != contrato.PrestamoDevolucion ||
                    contratoGuardado.PosicionCBOT != contrato.PosicionCBOT ||
                    contratoGuardado.TipoPosicionCBOTId != contrato.TipoPosicionCBOTId ||
                    contratoGuardado.PlantaDestinoId != contrato.PlantaDestinoId ||
                    contratoGuardado.Condicional != contrato.Condicional ||
                    contratoGuardado.CondicionalFecha != contrato.CondicionalFecha ||
                    contratoGuardado.CondicionalPosicion != contrato.CondicionalPosicion ||
                    contratoGuardado.CondicionalMonedaId != contrato.CondicionalMonedaId ||
                    contratoGuardado.CondicionalPrecio != contrato.CondicionalPrecio ||
                    contratoGuardado.CondicionalContratoId != contrato.CondicionalContratoId ||
                    contratoGuardado.CondicionalCantidad != contrato.CondicionalCantidad;

                if ((descuentosGenerales == null && contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count > 0) ||
                    descuentosGenerales != null && descuentosGenerales.Count != contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count)
                {
                    conModificado = true;
                }
                else
                {
                    foreach (var descBon in contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId == 1))
                    {
                        if (!descuentosGenerales.Any(x => x.TipoPeriodoDBId == descBon.TipoPeriodoDBId
                         && x.Importe == descBon.Importe && x.MonedaId == descBon.MonedaId
                         && x.Porcentaje == descBon.Porcentaje && x.TipoDBId == descBon.TipoDBId))
                        {
                            conModificado = true;
                            break;
                        }
                    }
                }

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

                var fechaContrato = repositorio.Obtener<Contrato, DateTime>(x => x.Id == contrato.Id, x => x.Fecha);

                var AnulaYReemplazaContratoSAP = contrato.AnulaYReemplazaContratoId == null ? "" :
                    repositorio.Obtener<Contrato, string>(x => x.Id == contrato.AnulaYReemplazaContratoId, x => x.ContratoSAP);
                var detalle = new Zmpes5270();

                detalle.Cantidad = Convert.ToDecimal(cantidadAbsoluta);
                detalle.ContrDataagro = contrato.Id.ToString();
                detalle.Cosecha = repositorio.Obtener<Campaña, string>(x => contrato.CampanaId == x.CampañaId, x => x.Descripcion);
                detalle.DiasDiferim = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0";
                detalle.FechaDesde = contrato.FechaDesde.ToString("yyyy-MM-dd");
                detalle.FechaEntrega = contrato.FechaEntrega.Value.ToString("yyyy-MM-dd");
                detalle.FechaHasta = contrato.FechaHasta.ToString("yyyy-MM-dd");
                detalle.FechaLimite = fechaDolarizadoString;
                detalle.GrupoCompras = contrato.TipoAgenteCompraId == 1 ? "902" : "";
                detalle.Moneda = contrato.MonedaId;
                detalle.NoInformarSio = noInformaSioString;
                detalle.PagoDiferido = (contrato.Dolarizado == true || contrato.DolarizadoCorredor == true) == true ? "X" : "";
                detalle.Material = repositorio.Obtener<Material, string>(x => contrato.MaterialId == x.MaterialId, x => x.Codigo);
                detalle.PagoDifArp = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "";
                detalle.PrecioPizarra = contrato.Precio;
                detalle.Precio = (contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : contrato.PrecioNeto ?? contrato.Precio;
                detalle.Proveedor = repositorio.Obtener<Proveedor, string>(x => contrato.ProveedorId == x.ProveedorId, x => x.CUIT);
                detalle.Provincia = contrato.ProvinciaId.ToString();
                detalle.Sustentable = contrato.Sustentable || contrato.EPA || contrato.EUDR ? "X" : "";
                detalle.Epa = contrato.EPA ? "X" : ""; //si es twin (EPA+EUDR) se completa el campo EPA.
                detalle.Eudr = contrato.EUDR && !contrato.EPA ? "X" : "";
                detalle.Especial = repositorio.Obtener<StandardDeCalidad, string>(x => contrato.StandardDeCalidadId == x.Id, x => x.CodigoSap);
                detalle.Fecha = contrato.FechaOperacion.ToString("yyyy-MM-dd");
                detalle.Usuario = repositorio.Obtener<Comercial, string>(x => contrato.ComercialId == x.ComercialId, x => x.IdActiveDirectory);
                detalle.Horaact = fechaContrato.ToString("HH:mm:ss");
                detalle.Procedencia = localidadString;
                detalle.Centro = repositorio.Obtener<Centro, string>(x => contrato.DestinoId == x.Id, x => x.CodigoSap);
                detalle.Clasificacion = repositorio.Obtener<ClasificacionCompraNet, string>(x => contrato.ClasificacionId == x.Id, x => x.Descripcion).ToUpper();
                detalle.IndOpCanje = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "";
                detalle.Consignatario = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "";
                detalle.CondFijacion = contrato.CondicionFijacionId.HasValue ? repositorio.Obtener<CondicionFijacion, string>(x => contrato.CondicionFijacionId == x.Id, x => x.CodigoSap) : "";
                detalle.Camiones = cantidadCamiones;
                detalle.Confirma = contrato.BoletoId == 1 ? "X" : "";
                detalle.Bolsa = contrato.BoletoId == 1 || contrato.BoletoId == 2 || contrato.BoletoId == 4 ? repositorio.Obtener<BolsaCompraNet, string>(x => contrato.BolsaId == x.Id, x => x.CodigoSap) : null;
                detalle.BolFisico = contrato.BoletoId == 2 ? "X" : "";
                detalle.CartaOferta = contrato.BoletoId == 4 ? "X" : "";
                detalle.Ninguno = contrato.BoletoId == 3 ? "X" : "";
                detalle.SinBoleto = contrato.BoletoId == 5 ? "X" : "";
                detalle.AutCg = contrato.Warrant == true ? "X" : "";
                detalle.AurCd = contrato.CD == true ? "X" : "";
                detalle.PagoDirVend = contrato.PagoDirectoVendedor == true ? "X" : "";
                detalle.EstabPropio = contrato.EstablecimientoPropio == true ? "X" : "";
                detalle.EstabArrendado = contrato.EstablecimientoPropio == false ? "X" : "";
                detalle.ImporteSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                detalle.MonedaSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                detalle.PorcSPrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                detalle.ImporteAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                detalle.MonedaAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                detalle.PorcAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                detalle.MercDescargada = contrato.MercsDeposito == true ? "X" : "";
                detalle.ObservacionCal1 = contrato.Observacion;
                detalle.CuitCorredor = contrato.CorredorId.HasValue ? repositorio.Obtener<Proveedor, string>(x => contrato.CorredorId == x.ProveedorId, x => x.CUIT) : "";
                detalle.PorcComision = contrato.CorredorId.HasValue ? contrato.PorcentajeComision.Value : 0;
                detalle.Contrcorr = contrato.ContratoCorredor ?? "";
                detalle.Contrvend = contrato.ContratoVendedor ?? "";
                detalle.SelCargoMoa = contrato.SelCargoMOA == true ? "X" : "";
                detalle.SelCargoVend = contrato.SelCargoVendedor == true ? "X" : "";
                detalle.ContratoMadre = contrato.ContratoMadre ?? "";
                detalle.Creador = contrato.ComercialCreadorId.HasValue ? repositorio.Obtener<Comercial, string>(x => contrato.ComercialCreadorId == x.ComercialId, x => x.IdActiveDirectory) : "";
                detalle.Zona = contrato.ZonaId.HasValue ? repositorio.Obtener<Zona, string>(x => contrato.ZonaId == x.Id, x => x.CodigoSap) : "";
                detalle.Compensacion = contrato.Compensacion == true ? "X" : "";
                detalle.FleteNivel = contrato.NivelTarifaId.HasValue ? repositorio.Obtener<NivelTarifa, string>(x => contrato.NivelTarifaId == x.Id, x => x.CodigoSap) : "";
                detalle.FleteTarifa = contrato.TarifaFlete ?? 0;
                detalle.FechaCierta = contrato.FechaCierta.HasValue ? contrato.FechaCierta.Value.ToString("yyyy-MM-dd") : null;
                detalle.Porcparcial = contrato.PorcentajeDePago ?? (contrato.MaterialId == (int)EnumMateriales.TRIGO ? (decimal)95.0 : (decimal)97.5);
                detalle.AgenteCompra = contrato.TipoAgenteCompraId == 1 ? "9952569841" : "";
                detalle.Caratula = contrato.CaratulaMAT;
                detalle.CaratulaExt = contrato.CaratulaExtension;
                detalle.PrecioComMat = contrato.PrecioAjusteComision ?? 0;
                detalle.MonedaComMat = contrato.MonedaAjusteComisionId;
                detalle.FechaCreacion = fechaContrato.ToString("yyyy-MM-dd");
                detalle.Zlsch = contrato.ChequeElectronico == true ? "=" : "";
                detalle.DolExpress = contrato.DolarizadoExpress == true ? "X" : "";
                detalle.CuentaMrp = contrato.PagoCBU != null ? contrato.PagoCBU.Split('-')[0] : "";
                detalle.PlantaDest = contrato.PlantaDestinoId != null ? repositorio.Obtener<Centro, string>(x => contrato.PlantaDestinoId == x.Id, x => x.CodigoSap) : repositorio.Obtener<Centro, string>(x => contrato.DestinoId == x.Id, x => x.CodigoSap);
                detalle.Canje = contrato.Canje == true ? "X" : "";
                detalle.DescInsumos = contrato.Insumo;
                detalle.MonedaDeuda = contrato.MonedaCanjeId == "USDM " ? "USD" : contrato.MonedaCanjeId;
                detalle.MontoDeuda = contrato.Monto ?? 0;
                detalle.PosicionCbot = contrato.PosicionCBOT;
                detalle.FijCbotMat = contrato.TipoPosicionCBOTId.HasValue ? contrato.TipoPosicionCBOTId.ToString() : "";
                detalle.Tercero = contrato.ProveedorCreadorId != null ? "X" : "";
                detalle.AnulaYReemp = AnulaYReemplazaContratoSAP;
                detalle.Condicional = contrato.Condicional == true ? "X" : "";
                detalle.FechaCond = contrato.CondicionalFecha != null ? contrato.CondicionalFecha.Value.ToString("yyyy-MM-dd") : "";
                detalle.MesCondMat = contrato.CondicionalPosicion ?? "";
                detalle.MonedaCond = contrato.CondicionalMonedaId ?? "";
                detalle.PrecioCond = contrato.CondicionalPrecio ?? 0;
                detalle.ContratoCond = contrato.CondicionalContrato != null ? contrato.CondicionalContrato.ContratoSAP : "";
                detalle.CantidadCond = contrato.CondicionalCantidad != null ? Convert.ToDecimal(contrato.CondicionalCantidad.Value) : 0;
                detalle.CondPago = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "04" : "";
                detalle.PorcMulta = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "10" : "";
                detalle.Pizarra = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "ROS" : "";
                detalle.TolInf = contrato.CantidadCamiones > 0 ? 0 : 3;
                detalle.TolSup = contrato.CantidadCamiones > 0 ? 0 : 3;

                #region BLEND
                detalle.CodigoTc = finalizarContratoAgent.DevolverTipoCambioSAP(contrato.TipoNegocioId, contrato.MonedaId, contrato.TipoAgenteCompraId, contrato.Fecha, true);
                #endregion
                logger.Info($"MODIFICA - NegocioId: {contrato.Id} - Contrato Nro: {contrato.ContratoSAP} - CODIGO_TC: {detalle.CodigoTc} - TipoDeCambioId: {contrato.TipoDeCambioId}");

                detalle.Bloqueo = "";
                detalle.TipoCambioFijo = 0;
                detalle.Posicion = CalcularPosicion(contrato.FechaDesde);
                var rq = new ZMprfcModificarContrato
                {
                    ImContrato = new Zmpes5560
                    {
                        Contrato = contrato.ContratoSAP.TrimStart('0'),
                        Detalle = detalle
                    }
                };
                var topFija =
                    contratoGuardado.DesdeFijacion != contrato.DesdeFijacion ||
                    contratoGuardado.HastaFijacion != contrato.HastaFijacion;
                rq.ImTopesFij = topesFijacion.ToArray();
                rq.ImModificacion = new Zmpes5570
                {
                    Contrato = conModificado ? "X" : "",
                    Apertura = apModificado ? "X" : "",
                    Calidad = calModificado ? "X" : "",
                    DescBonif = descModificado ? "X" : "",
                    TopesFij = topFija ? "X" : "",
                    Servicios = apServicio ? "X" : ""
                };
                rq.ImDescBonif = listaDescuentos.ToArray();
                rq.ImCalidad = listaCalidades.ToArray();
                rq.ImApertura = listaApertura.ToArray();
                rq.ImServicios = servicioSap.ToArray();

                logger.Debug(rq.ToXml());

                var log = new Log
                {
                    Fecha = DateTime.Now,
                    Xml = rq.ToXml()
                };

                var logId = repositorio.Agregar(log);
                repositorio.GuardarCambios();

                var devolucion = agent.ZMprfcModificarContrato(rq);
                logger.Debug(devolucion.ToXml());

                log = repositorio.Obtener<Log>(logId.Id);
                log.Xml += devolucion.ToXml();
                repositorio.GuardarCambios();

                //if (devolucion != null && !string.IsNullOrEmpty(devolucion.EX_MENSAJE) && devolucion.EX_MENSAJE.Contains("Error"))
                //{
                //    throw new Exception(devolucion.EX_MENSAJE);
                //}
                logger.Debug(devolucion != null && !string.IsNullOrEmpty(devolucion.ExMensaje) ? "Respuesta SAP: " + devolucion.ExMensaje : "OK SAP null");
                return devolucion != null && !string.IsNullOrEmpty(devolucion.ExMensaje) ? devolucion.ExMensaje : "OK";
            }
            catch (Exception e)
            {
                logger.Error(e, "Error comunicacion SAP");
                throw;
            }
        }
        private string ModificarConPI(Contrato contrato, Contrato contratoGuardado)
        {
            if (ConfigurationManager.AppSettings["SinConexionSap"] == "1")
            {
                return "OK";
            }
            try
            {
                SI_ZMPWS_DATAAGRO_MODIFICAR_CONTRATOClient agent = new SI_ZMPWS_DATAAGRO_MODIFICAR_CONTRATOClient();

                agent.ClientCredentials.UserName.UserName = UserSap;
                agent.ClientCredentials.UserName.Password = PassSap;
                logger.Debug("Modificando Contrato Nro: " + contrato.Id);
                var listaDescuentos = new List<ZMPES5290>();
                var topesFijacion = new List<ZMPES5280>();
                var servicioSap = new List<ZMPES6620>();
                logger.Debug("Contrato Obtenido: " + contratoGuardado.Id);

                var descModificado = false;
                var listaMonedas = repositorio.Listar<Moneda>();
                var descuentosGenerales = new List<DescuentoBonificacion>();
                var descuentos = new List<DescuentoBonificacion>();
                var servicios = contrato.Servicios ?? new List<Servicio>();
                var servicioValor = repositorio.Listar<ServicioValor>();
                if (contrato.Descuentos != null)
                {
                    descuentosGenerales = contrato.Descuentos.Where(x => x.TipoPeriodoDBId == 1).ToList();
                    descuentos = contrato.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList();
                }
                if ((descuentos == null && contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count > 0) ||
                    descuentos != null && descuentos.Count != contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count ||
                    (contrato.PrecioPactado == null && contratoGuardado.PrecioPactado.ToList().Count > 0) ||
                    contrato.PrecioPactado != null && contrato.PrecioPactado.Count != contratoGuardado.PrecioPactado.Count ||
                         contrato.ImporteSustentable != contratoGuardado.ImporteSustentable ||
                         contrato.MonedaId != contratoGuardado.MonedaId)
                {
                    descModificado = true;
                }
                else
                {
                    foreach (var descBon in contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1))
                    {
                        if (!descuentos.Any(x => x.TipoPeriodoDBId == descBon.TipoPeriodoDBId
                         && x.Importe == descBon.Importe && x.MonedaId == descBon.MonedaId
                         && x.Porcentaje == descBon.Porcentaje && x.TipoDBId == descBon.TipoDBId)
                         )
                        {
                            descModificado = true;
                            break;
                        }
                    }
                    foreach (var precio in contratoGuardado.PrecioPactado)
                    {
                        if (!contrato.PrecioPactado.Any(x => x.FechaDesde == precio.FechaDesde
                        && x.FechaHasta == precio.FechaHasta
                        && x.Precio == precio.Precio
                        && x.MonedaPactadoId == precio.MonedaPactadoId
                        && x.ImportePactado == precio.ImportePactado
                        && x.MonedaImportePactadoId == precio.MonedaImportePactadoId
                        && x.Porcentaje == precio.Porcentaje))
                        {
                            descModificado = true;
                            break;
                        }
                    }
                }
                if (descuentos != null && descuentos.Count > 0)
                {
                    var listaPeriodo = repositorio.Listar<TipoPeriodoDB>();
                    var listaTipo = repositorio.Listar<TipoDB>();
                    foreach (var descBon in descuentos)
                    {
                        listaDescuentos.Add(new ZMPES5290
                        {
                            TIPO_PERIODO = listaPeriodo.Where(x => x.Id == descBon.TipoPeriodoDBId).Single().CodigoSap,
                            TIPO_DB = listaTipo.Where(x => x.Id == descBon.TipoDBId).Single().CodigoSap,
                            FEDESDE = descBon.FechaDesde?.ToString("yyyy-MM-dd"),
                            FEHASTA = descBon.FechaHasta?.ToString("yyyy-MM-dd"),
                            IMPORTE_DB = descBon.Importe,
                            MONEDA_DB = descBon.MonedaId ?? "",
                            PORC_DB = descBon.Porcentaje
                        });
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
                if (contrato.PrecioPactado != null)
                {
                    foreach (var p in contrato.PrecioPactado)
                    {
                        listaDescuentos.Add(new ZMPES5290
                        {
                            TIPO_PERIODO = "E",
                            TIPO_DB = "",
                            FEDESDE = p.FechaDesde?.ToString("yyyy-MM-dd"),
                            FEHASTA = p.FechaHasta?.ToString("yyyy-MM-dd"),
                            IMPORTE_DB = p.ImportePactado ?? 0,
                            MONEDA_DB = p.MonedaImportePactadoId ?? "",
                            PORC_DB = p.Porcentaje ?? 0,
                            PRECIO = p.Precio,
                            MONEDA = p.MonedaPactadoId
                        });
                    }
                }
                logger.Debug("Descuentos modificados");

                var calModificado = false;
                if ((contrato.Calidad == null && contratoGuardado.Calidad.Count > 0) || contrato.Calidad != null && contratoGuardado.Calidad.Count != contrato.Calidad.Count)
                {
                    calModificado = true;
                }
                else
                {
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
                }
                var listaCalidades = new List<ZMPES5300>();
                if (contrato.Calidad != null)
                {
                    foreach (var cal in contrato.Calidad)
                    {
                        var listaCalidadEspecial = repositorio.Listar<CalidadEspecial>();
                        if (cal.StandardDeCalidadId == 2)
                        {
                            if ((cal.Valor >= 2 && cal.Valor <= 3) && (cal.CalidadEspecialId == 4 || cal.CalidadEspecialId == 5))
                            {
                                listaCalidades.Add(new ZMPES5300
                                {
                                    CODIGO = listaCalidadEspecial.FirstOrDefault(x => x.Id == cal.CalidadEspecialId).CodigoSap,
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
                }

                logger.Debug("Calidades: " + contrato.Calidad);
                var apModificado = false;
                if ((contrato.AperturaPrecio == null && contratoGuardado.AperturaPrecio.Count > 0) || contrato.AperturaPrecio != null && contratoGuardado.AperturaPrecio.Count != contrato.AperturaPrecio.Count)
                {
                    apModificado = true;
                }
                else
                {
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
                }
                if (contrato.SustentableTipoDBId != contratoGuardado.SustentableTipoDBId && (contrato.SustentableTipoDBId == 1 || contratoGuardado.SustentableTipoDBId == 1))
                {
                    apModificado = true;
                }
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
                if (contrato.AperturaPrecio != null)
                {
                    var listaAperturaPrecios = repositorio.Listar<ConceptoAperturaPrecio>();
                    foreach (AperturaPrecio apertura in contrato.AperturaPrecio)
                    {
                        if (apertura.Importe != 0 || apertura.Porcentaje != 0)
                        {
                            listaApertura.Add(new ZMPES5440
                            {
                                CONCEPTO = listaAperturaPrecios.FirstOrDefault(x => x.Id == apertura.ConceptoAperturaPrecioId).CodigoSap,
                                IMPORTE = apertura.Importe,
                                MONEDA = contrato.TipoNegocioId == 1 ? apertura.MonedaId : contrato.MonedaId,
                                PORC = apertura.Porcentaje
                            });
                        }
                    }
                }

                logger.Debug("Apertura: " + contrato.AperturaPrecio);
                if (contrato.Descuentos == null)
                {
                    contrato.Descuentos = new List<DescuentoBonificacion>();
                }
                var apServicio = false;
                if ((contrato.Servicios == null && contratoGuardado.Servicios.Count > 0) || contrato.Servicios != null && contratoGuardado.Servicios.Count != contrato.Servicios.Count)
                {
                    apServicio = true;
                }
                else
                {
                    foreach (var ser in contratoGuardado.Servicios)
                    {
                        if (!contrato.Servicios.Any(x => x.Importe == ser.Importe))
                        {
                            apServicio = true;
                            break;
                        }
                    }
                }
                logger.Debug("Servicios: " + contrato.AperturaPrecio);
                foreach (var servicio in servicios)
                {
                    servicioSap.Add(new ZMPES6620
                    {
                        CODIGO = servicioValor.Where(x => x.Id == servicio.ServicioValorId).FirstOrDefault().TipoServicio.CodigoSAP,
                        PORC_DESDE = servicio.Desde,
                        PORC_HASTA = servicio.Hasta,
                        VALOR = servicio.Importe,
                        MONEDA = servicio.MonedaId,
                        FECHAACT = contrato.Fecha.ToString("yyyy-MM-dd"),
                        HORAACT = contrato.Fecha.ToString("HH:mm:ss"),
                    }
                    );
                }
                var descuentoGeneralSobrePrecio = contrato.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();

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
                    descuentoGeneralSobrePrecio.MonedaId += contrato.MonedaSustentableId;
                }

                var descuentoGeneralFueraPrecio = contrato.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                string fechaDolarizadoString = contrato.FechaDolarizado != null ? contrato.FechaDolarizado.Value.ToString("yyyy-MM-dd") : "";
                string sustentableString = contrato.ImporteSustentable != null && contrato.ImporteSustentable.Value != 0 ? "X" : "";
                string noInformaSioString = contrato.NoInformaSio != null && contrato.NoInformaSio.Value ? "X" : "";
                string especialString = contrato.Calidad != null && contrato.Calidad.Count > 0 ? "4" : "1";

                var localidad = repositorio.Obtener<Localidad>(contrato.LocalidadId ?? 0);
                logger.Debug("Localidad obtenida");
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
                    contratoGuardado.DolarizadoCorredor != contrato.DolarizadoCorredor ||
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
                    contratoGuardado.EPA != contrato.EPA ||
                    contratoGuardado.EUDR != contrato.EUDR ||
                    contratoGuardado.TarifaAConvenir != contrato.TarifaAConvenir ||
                    contratoGuardado.TarifaFlete != contrato.TarifaFlete ||
                    contratoGuardado.TrigoEspecial != contrato.TrigoEspecial ||
                    contratoGuardado.UsuarioId != contrato.UsuarioId ||
                    contratoGuardado.Warrant != contrato.Warrant ||
                    contratoGuardado.ZonaId != contrato.ZonaId ||
                    contratoGuardado.FechaCierta != contrato.FechaCierta ||
                    contratoGuardado.PorcentajeDePago != contrato.PorcentajeDePago ||
                    contratoGuardado.TipoAgenteCompraId != contrato.TipoAgenteCompraId ||
                    contratoGuardado.CaratulaMAT != contrato.CaratulaMAT ||
                    contratoGuardado.CaratulaExtension != contrato.CaratulaExtension ||
                    contratoGuardado.PrecioAjusteComision != contrato.PrecioAjusteComision ||
                    contratoGuardado.MonedaAjusteComisionId != contrato.MonedaAjusteComisionId ||
                    contratoGuardado.ChequeElectronico != contrato.ChequeElectronico ||
                    contratoGuardado.DolarizadoExpress != contrato.DolarizadoExpress ||
                    contratoGuardado.PagoCBU != contrato.PagoCBU || contratoGuardado.Canje != contrato.Canje ||
                    contratoGuardado.Monto != contrato.Monto || contratoGuardado.Insumo != contrato.Insumo ||
                    contratoGuardado.MonedaCanjeId != contrato.MonedaCanjeId ||
                    contratoGuardado.PrestamoDevolucion != contrato.PrestamoDevolucion ||
                    contratoGuardado.PosicionCBOT != contrato.PosicionCBOT ||
                    contratoGuardado.TipoPosicionCBOTId != contrato.TipoPosicionCBOTId ||
                    contratoGuardado.PlantaDestinoId != contrato.PlantaDestinoId ||
                    contratoGuardado.Condicional != contrato.Condicional ||
                    contratoGuardado.CondicionalFecha != contrato.CondicionalFecha ||
                    contratoGuardado.CondicionalPosicion != contrato.CondicionalPosicion ||
                    contratoGuardado.CondicionalMonedaId != contrato.CondicionalMonedaId ||
                    contratoGuardado.CondicionalPrecio != contrato.CondicionalPrecio ||
                    contratoGuardado.CondicionalContratoId != contrato.CondicionalContratoId ||
                    contratoGuardado.CondicionalCantidad != contrato.CondicionalCantidad;

                if ((descuentosGenerales == null && contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count > 0) ||
                    descuentosGenerales != null && descuentosGenerales.Count != contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId != 1).ToList().Count)
                {
                    conModificado = true;
                }
                else
                {
                    foreach (var descBon in contratoGuardado.Descuentos.Where(x => x.TipoPeriodoDBId == 1))
                    {
                        if (!descuentosGenerales.Any(x => x.TipoPeriodoDBId == descBon.TipoPeriodoDBId
                         && x.Importe == descBon.Importe && x.MonedaId == descBon.MonedaId
                         && x.Porcentaje == descBon.Porcentaje && x.TipoDBId == descBon.TipoDBId))
                        {
                            conModificado = true;
                            break;
                        }
                    }
                }

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

                var fechaContrato = repositorio.Obtener<Contrato, DateTime>(x => x.Id == contrato.Id, x => x.Fecha);

                var AnulaYReemplazaContratoSAP = contrato.AnulaYReemplazaContratoId == null ? "" :
                    repositorio.Obtener<Contrato, string>(x => x.Id == contrato.AnulaYReemplazaContratoId, x => x.ContratoSAP);
                var detalle = new ZMPES5270();

                detalle.CANTIDAD = Convert.ToDecimal(cantidadAbsoluta);
                detalle.CONTR_DATAAGRO = contrato.Id.ToString();
                detalle.COSECHA = repositorio.Obtener<Campaña, string>(x => contrato.CampanaId == x.CampañaId, x => x.Descripcion);
                detalle.DIAS_DIFERIM = contrato.DiasPesificado != null ? contrato.DiasPesificado.ToString() : "0";
                detalle.FECHA_DESDE = contrato.FechaDesde.ToString("yyyy-MM-dd");
                detalle.FECHA_ENTREGA = contrato.FechaEntrega.Value.ToString("yyyy-MM-dd");
                detalle.FECHA_HASTA = contrato.FechaHasta.ToString("yyyy-MM-dd");
                detalle.FECHA_LIMITE = fechaDolarizadoString;
                detalle.GRUPO_COMPRAS = contrato.TipoAgenteCompraId == 1 ? "902" : "";
                detalle.MONEDA = contrato.MonedaId;
                detalle.NO_INFORMAR_SIO = noInformaSioString;
                detalle.PAGO_DIFERIDO = (contrato.Dolarizado == true || contrato.DolarizadoCorredor == true) == true ? "X" : "";
                detalle.MATERIAL = repositorio.Obtener<Material, string>(x => contrato.MaterialId == x.MaterialId, x => x.Codigo);
                detalle.PAGO_DIF_ARP = contrato.PagoDiferido.HasValue && contrato.PagoDiferido.Value ? "X" : "";
                detalle.PRECIO_PIZARRA = contrato.Precio;
                detalle.PRECIO = (contrato.EPA || contrato.EUDR || contrato.Sustentable) && precioNetoSustentable.HasValue ? precioNetoSustentable.Value : contrato.PrecioNeto ?? contrato.Precio;
                detalle.PROVEEDOR = repositorio.Obtener<Proveedor, string>(x => contrato.ProveedorId == x.ProveedorId, x => x.CUIT);
                detalle.PROVINCIA = contrato.ProvinciaId.ToString();
                detalle.SUSTENTABLE = contrato.Sustentable || contrato.EPA || contrato.EUDR ? "X" : "";
                detalle.EPA = contrato.EPA ? "X" : ""; //si es twin (EPA+EUDR) se completa el campo EPA.
                detalle.EUDR = contrato.EUDR && !contrato.EPA ? "X" : "";
                detalle.ESPECIAL = repositorio.Obtener<StandardDeCalidad, string>(x => contrato.StandardDeCalidadId == x.Id, x => x.CodigoSap);
                detalle.FECHA = contrato.FechaOperacion.ToString("yyyy-MM-dd");
                detalle.USUARIO = repositorio.Obtener<Comercial, string>(x => contrato.ComercialId == x.ComercialId, x => x.IdActiveDirectory);
                detalle.HORAACT = fechaContrato.ToString("HH:mm:ss");
                detalle.PROCEDENCIA = localidadString;
                detalle.CENTRO = repositorio.Obtener<Centro, string>(x => contrato.DestinoId == x.Id, x => x.CodigoSap);
                detalle.CLASIFICACION = repositorio.Obtener<ClasificacionCompraNet, string>(x => contrato.ClasificacionId == x.Id, x => x.Descripcion).ToUpper();
                detalle.IND_OP_CANJE = contrato.PlanCanje != null && contrato.PlanCanje.Value ? "X" : "";
                detalle.CONSIGNATARIO = contrato.Consignatario != null && contrato.Consignatario.Value ? "X" : "";
                detalle.COND_FIJACION = contrato.CondicionFijacionId.HasValue ? repositorio.Obtener<CondicionFijacion, string>(x => contrato.CondicionFijacionId == x.Id, x => x.CodigoSap) : "";
                detalle.CAMIONES = cantidadCamiones;
                detalle.CONFIRMA = contrato.BoletoId == 1 ? "X" : "";
                detalle.BOLSA = contrato.BoletoId == 1 || contrato.BoletoId == 2 || contrato.BoletoId == 4 ? repositorio.Obtener<BolsaCompraNet, string>(x => contrato.BolsaId == x.Id, x => x.CodigoSap) : null;
                detalle.BOL_FISICO = contrato.BoletoId == 2 ? "X" : "";
                detalle.CARTA_OFERTA = contrato.BoletoId == 4 ? "X" : "";
                detalle.NINGUNO = contrato.BoletoId == 3 ? "X" : "";
                detalle.SIN_BOLETO = contrato.BoletoId == 5 ? "X" : "";
                detalle.AUT_CG = contrato.Warrant == true ? "X" : "";
                detalle.AUR_CD = contrato.CD == true ? "X" : "";
                detalle.PAGO_DIR_VEND = contrato.PagoDirectoVendedor == true ? "X" : "";
                detalle.ESTAB_PROPIO = contrato.EstablecimientoPropio == true ? "X" : "";
                detalle.ESTAB_ARRENDADO = contrato.EstablecimientoPropio == false ? "X" : "";
                detalle.IMPORTE_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                detalle.MONEDA_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                detalle.PORC_S_PRECIO = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                detalle.IMPORTE_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                detalle.MONEDA_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                detalle.PORC_A_PRECIO = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                detalle.MERC_DESCARGADA = contrato.MercsDeposito == true ? "X" : "";
                detalle.OBSERVACION_CAL1 = contrato.Observacion;
                detalle.CUIT_CORREDOR = contrato.CorredorId.HasValue ? repositorio.Obtener<Proveedor, string>(x => contrato.CorredorId == x.ProveedorId, x => x.CUIT) : "";
                detalle.PORC_COMISION = contrato.CorredorId.HasValue ? contrato.PorcentajeComision.Value : 0;
                detalle.CONTRCORR = contrato.ContratoCorredor ?? "";
                detalle.CONTRVEND = contrato.ContratoVendedor ?? "";
                detalle.SEL_CARGO_MOA = contrato.SelCargoMOA == true ? "X" : "";
                detalle.SEL_CARGO_VEND = contrato.SelCargoVendedor == true ? "X" : "";
                detalle.CONTRATO_MADRE = contrato.ContratoMadre ?? "";
                detalle.CREADOR = contrato.ComercialCreadorId.HasValue ? repositorio.Obtener<Comercial, string>(x => contrato.ComercialCreadorId == x.ComercialId, x => x.IdActiveDirectory) : "";
                detalle.ZONA = contrato.ZonaId.HasValue ? repositorio.Obtener<Zona, string>(x => contrato.ZonaId == x.Id, x => x.CodigoSap) : "";
                detalle.COMPENSACION = contrato.Compensacion == true ? "X" : "";
                detalle.FLETE_NIVEL = contrato.NivelTarifaId.HasValue ? repositorio.Obtener<NivelTarifa, string>(x => contrato.NivelTarifaId == x.Id, x => x.CodigoSap) : "";
                detalle.FLETE_TARIFA = contrato.TarifaFlete ?? 0;
                detalle.FECHA_CIERTA = contrato.FechaCierta.HasValue ? contrato.FechaCierta.Value.ToString("yyyy-MM-dd") : null;
                detalle.PORCPARCIAL = contrato.PorcentajeDePago ?? (contrato.MaterialId == (int)EnumMateriales.TRIGO ? (decimal)95.0 : (decimal)97.5);
                detalle.AGENTE_COMPRA = contrato.TipoAgenteCompraId == 1 ? "9952569841" : "";
                detalle.CARATULA = contrato.CaratulaMAT;
                detalle.CARATULA_EXT = contrato.CaratulaExtension;
                detalle.PRECIO_COM_MAT = contrato.PrecioAjusteComision ?? 0;
                detalle.MONEDA_COM_MAT = contrato.MonedaAjusteComisionId;
                detalle.FECHA_CREACION = fechaContrato.ToString("yyyy-MM-dd");
                detalle.ZLSCH = contrato.ChequeElectronico == true ? "=" : "";
                detalle.DOL_EXPRESS = contrato.DolarizadoExpress == true ? "X" : "";
                detalle.CUENTA_MRP = contrato.PagoCBU != null ? contrato.PagoCBU.Split('-')[0] : "";
                detalle.PLANTA_DEST = contrato.PlantaDestinoId != null ? repositorio.Obtener<Centro, string>(x => contrato.PlantaDestinoId == x.Id, x => x.CodigoSap) : repositorio.Obtener<Centro, string>(x => contrato.DestinoId == x.Id, x => x.CodigoSap);
                detalle.CANJE = contrato.Canje == true ? "X" : "";
                detalle.DESC_INSUMOS = contrato.Insumo;
                detalle.MONEDA_DEUDA = contrato.MonedaCanjeId == "USDM " ? "USD" : contrato.MonedaCanjeId;
                detalle.MONTO_DEUDA = contrato.Monto ?? 0;
                detalle.POSICION_CBOT = contrato.PosicionCBOT;
                detalle.FIJ_CBOT_MAT = contrato.TipoPosicionCBOTId.HasValue ? contrato.TipoPosicionCBOTId.ToString() : "";
                detalle.TERCERO = contrato.ProveedorCreadorId != null ? "X" : "";
                detalle.ANULA_Y_REEMP = AnulaYReemplazaContratoSAP;
                detalle.CONDICIONAL = contrato.Condicional == true ? "X" : "";
                detalle.FECHA_COND = contrato.CondicionalFecha != null ? contrato.CondicionalFecha.Value.ToString("yyyy-MM-dd") : "";
                detalle.MES_COND_MAT = contrato.CondicionalPosicion ?? "";
                detalle.MONEDA_COND = contrato.CondicionalMonedaId ?? "";
                detalle.PRECIO_COND = contrato.CondicionalPrecio ?? 0;
                detalle.CONTRATO_COND = contrato.CondicionalContrato != null ? contrato.CondicionalContrato.ContratoSAP : "";
                detalle.CANTIDAD_COND = contrato.CondicionalCantidad != null ? Convert.ToDecimal(contrato.CondicionalCantidad.Value) : 0;
                detalle.COND_PAGO = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "04" : "";
                detalle.PORC_MULTA = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "10" : "";
                detalle.PIZARRA = contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "ROS" : "";
                detalle.TOL_INF = contrato.CantidadCamiones > 0 ? 0 : 3;
                detalle.TOL_SUP = contrato.CantidadCamiones > 0 ? 0 : 3;

                #region BLEND
                detalle.CODIGO_TC = finalizarContratoAgent.DevolverTipoCambioSAP(contrato.TipoNegocioId, contrato.MonedaId, contrato.TipoAgenteCompraId, contrato.Fecha, true);
                #endregion
                logger.Info($"MODIFICA - NegocioId: {contrato.Id} - Contrato Nro: {contrato.ContratoSAP} - CODIGO_TC: {detalle.CODIGO_TC} - TipoDeCambioId: {contrato.TipoDeCambioId}");

                detalle.BLOQUEO = "";
                detalle.TIPO_CAMBIO_FIJO = 0;
                detalle.POSICION = CalcularPosicion(contrato.FechaDesde);
                var rq = new Z_MPRFC_MODIFICAR_CONTRATO
                {
                    IM_CONTRATO = new ZMPES5560
                    {
                        CONTRATO = contrato.ContratoSAP.TrimStart('0'),
                        DETALLE = detalle
                    }
                };
                var topFija =
                    contratoGuardado.DesdeFijacion != contrato.DesdeFijacion ||
                    contratoGuardado.HastaFijacion != contrato.HastaFijacion;
                rq.IM_TOPES_FIJ = topesFijacion.ToArray();
                rq.IM_MODIFICACION = new ZMPES5570
                {
                    CONTRATO = conModificado ? "X" : "",
                    APERTURA = apModificado ? "X" : "",
                    CALIDAD = calModificado ? "X" : "",
                    DESC_BONIF = descModificado ? "X" : "",
                    TOPES_FIJ = topFija ? "X" : "",
                    SERVICIOS = apServicio ? "X" : ""
                };
                rq.IM_DESC_BONIF = listaDescuentos.ToArray();
                rq.IM_CALIDAD = listaCalidades.ToArray();
                rq.IM_APERTURA = listaApertura.ToArray();
                rq.IM_SERVICIOS = servicioSap.ToArray();

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

                //if (devolucion != null && !string.IsNullOrEmpty(devolucion.EX_MENSAJE) && devolucion.EX_MENSAJE.Contains("Error"))
                //{
                //    throw new Exception(devolucion.EX_MENSAJE);
                //}
                logger.Debug(devolucion != null && !string.IsNullOrEmpty(devolucion.EX_MENSAJE) ? "Respuesta SAP: " + devolucion.EX_MENSAJE : "OK SAP null");
                return devolucion != null && !string.IsNullOrEmpty(devolucion.EX_MENSAJE) ? devolucion.EX_MENSAJE : "OK";
            }
            catch (Exception e)
            {
                logger.Error(e, "Error comunicacion SAP");
                throw;
            }
        }


        public string Modificar(Contrato contrato, Contrato contratoGuardado)
        {
            string resultado = string.Empty;
            if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
            {
                resultado = ModificarSinPI(contrato, contratoGuardado);
            }
            else
            {
                resultado = ModificarConPI(contrato, contratoGuardado);
            }
            return resultado;
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
            precioOriginal += contrato.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 4).FirstOrDefault()?.Porcentaje ?? 0
                            * contrato.Precio / 100; porcentajeComision /= 100;
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
            var ultimoDiaHabil = diasHabilesAgent.UltimoDiaHabil(fechaDesde);
            var diferenteEntreDias = fechaDesde - ultimoDiaHabil;
            return diferenteEntreDias.Days >= 10 ? (fechaDesde.Month) + "-" + (fechaDesde.Year) :
                   (fechaDesde.Month + 1) + "-" + (fechaDesde.Year + 1);
        }
    }
}