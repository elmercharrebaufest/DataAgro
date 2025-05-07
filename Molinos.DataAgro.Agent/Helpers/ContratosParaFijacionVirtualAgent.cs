using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Agent.ContratosParaFijacionVirtual;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;

namespace Molinos.DataAgro.Agent
{
    public class ContratosParaFijacionVirtualAgent : IContratosParaFijacionVirtualAgent
    {
        public ContratosParaFijacionVirtualAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;


        public List<DatosFijacionDeContratoDto> ObtenerContratosCanjeSinPI(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            var hoy = DateTime.Now.Date;
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            logger.Info("SAP sin PI - RFC ZMprfcContratoCanjeGene");
            Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;

            var material = repositorio.Obtener<Material>(x => x.MaterialId == materialId);
            var rq = new ZMprfcContratoCanjeGene()
            {
                ImCorredor = CuitCorredor,
                ImCuit = CuitProveedor,
                ImMaterial = material.Codigo
            };
            logger.Debug(rq.ToXml());
            CultureInfo provider = CultureInfo.InvariantCulture;
            var devolucion = agent.ZMprfcContratoCanjeGene(rq);
            logger.Info("SAP sin PI - RFC ZMprfcContratoCanjeGene");
            logger.Debug("Numero de contratos pendientes: " + devolucion.ExSalida.Count());
            var listaContratos = devolucion.ExSalida.Where(x => x.Contrnum.StartsWith("000" + filtro.TrimStart('0')));
            var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
            var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
            var monedas = repositorio.Listar<Moneda>();
            foreach (var contrato in listaContratos)
            {
                var cantidad = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 3 && x.Virtual == true && x.ContratoSAP == contrato.Contrnum && x.Id != idFijacion
                && x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado).Sum(x => x.Cantidad + (x.Ampliaciones ?? 0));

                var centro = repositorio.Obtener<Centro>(x => x.CodigoSap == contrato.Centro);
                //var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<Negocio, double>(x => x.TipoNegocioId == 3 && x.Id == idFijacion && x.ContratoSAP == contrato.CONTRATO, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;
                //var calidades = new List<CalidadDto>();                       
                var contratoParaFijacion = new DatosFijacionDeContratoDto
                {
                    ContratoId = contrato.Contrnum.TrimStart('0'),
                    KilosAplicados = ((double)contrato.KilosFijados + cantidad).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    ARecibirSinPrecio = "0",
                    RecibidoSinFijar = "0",
                    KilosPendiente = ((double)contrato.KilosAFijar - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaDesde = DateTime.Parse(contrato.FechaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaHasta = DateTime.Parse(contrato.FechaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    DesdeEntrega = DateTime.Parse(contrato.FechaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    HastaEntrega = DateTime.Parse(contrato.FechaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    Campana = contrato.Cosecha,
                    CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == contrato.Cosecha, x => x.CampañaId),
                    Centro = centro.Id,
                    CentroDescripcion = centro.Descripcion,
                    Filtro = filtro + "|" + contrato.Contrnum.TrimStart('0'),
                    Color = DateTime.Parse(contrato.FechaHasta) < hoy ? "Red" : "#26337b",
                    KgContratoTotal = contrato.Cantidad,
                    KilosContrato = contrato.Unime,
                    Virtual = true,
                };
                var contratoDeBase = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contrato.Contrnum);
                if (contratoDeBase != null)
                {
                    var aperturas = new List<AperturaPrecioDto>();
                    foreach (var apertura in contratoDeBase.AperturaPrecio)
                    {
                        apertura.MonedaId = apertura.MonedaId ?? "";
                        var a = new AperturaPrecioDto()
                        {
                            ConceptoAperturaPrecioId = apertura != null ? apertura.ConceptoAperturaPrecioId : 0,
                            ConceptoAperturaPrecio = apertura != null ? apertura.ConceptoAperturaPrecio.Descripcion : "",
                            Importe = apertura.Importe != 0 ? apertura.Importe : 0,
                            Porcentaje = apertura.Porcentaje != 0 ? apertura.Porcentaje : 0,
                            MonedaId = apertura.MonedaId,
                            Moneda = apertura.Moneda != null ? monedas.Where(x => x.Descripcion.Trim() == apertura.Moneda.Descripcion.Trim()).FirstOrDefault().Descripcion : ""
                        };
                        aperturas.Add(a);
                    }
                    var bonificaciones = new List<DescuentoBonificacionDto>();

                    foreach (var bonif in contratoDeBase.Descuentos)
                    {
                        bonif.MonedaId = bonif.MonedaId ?? "";
                        var a = new DescuentoBonificacionDto()
                        {
                            FechaDesde = bonif.FechaDesde != null ? bonif.FechaDesde.Value.ToString("dd-MM-yyyy") : "",
                            FechaHasta = bonif.FechaHasta != null ? bonif.FechaHasta.Value.ToString("dd-MM-yyyy") : "",
                            Importe = bonif.Importe != 0 ? bonif.Importe : 0,
                            Porcentaje = bonif.Porcentaje != 0 ? bonif.Porcentaje : 0,
                            MonedaId = bonif.MonedaId,
                            Moneda = bonif.Moneda == null ? "" : bonif.Moneda.Descripcion == "" ? "" : monedas.Where(x => x.Descripcion.Trim() == bonif.Moneda.Descripcion.Trim()).FirstOrDefault().Descripcion
                        };
                        bonificaciones.Add(a);
                    }
                    contratoParaFijacion.Aperturas = aperturas;
                    contratoParaFijacion.Bonificaciones = bonificaciones;
                    contratoParaFijacion.CentroDescripcion = centro.Descripcion;
                    var descuentoGeneralFueraPrecio = contratoDeBase.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                    var descuentoGeneralSobrePrecio = contratoDeBase.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                    contratoParaFijacion.ImporteSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                    contratoParaFijacion.MonedaSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                    contratoParaFijacion.PorcentajeSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                    contratoParaFijacion.ImporteAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                    contratoParaFijacion.MonedaAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                    contratoParaFijacion.PorcentajeAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                    contratoParaFijacion.Clasificacion = contratoDeBase.Clasificacion.Descripcion;
                    var contratoConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == contratoDeBase.Id);

                    if (!contratoConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                    {
                        datosContratos.Add(contratoParaFijacion);
                    }
                }
                var contratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contrato.Contrnum, x => x.Id);
                var contratoAnulado = repositorio.Obtener<Contrato>(x => x.AnulaYReemplazaContratoId == contratoId && x.EstadoId == 5);
                if (contratoAnulado != null)
                {
                    listaContratos = listaContratos.Where(x => x.Contrnum != contratoAnulado.ContratoSAP);
                }
            }
            return datosContratos.OrderBy(x => x.ContratoId).ToList();
        }
        public List<DatosFijacionDeContratoDto> ObtenerContratosCanjeConPI(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            var hoy = DateTime.Now.Date;
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            var agent = new SI_ZMPWS_DATAAGRO_CONTRATO_CANJE_GENEClient();
            agent.ClientCredentials.UserName.UserName = UserSap;
            agent.ClientCredentials.UserName.Password = PassSap;
            var material = repositorio.Obtener<Material>(x => x.MaterialId == materialId);
            var rq = new Z_MPRFC_CONTRATO_CANJE_GENE()
            {
                IM_CORREDOR = CuitCorredor,
                IM_CUIT = CuitProveedor,
                IM_MATERIAL = material.Codigo
            };
            logger.Debug(rq.ToXml());
            CultureInfo provider = CultureInfo.InvariantCulture;
            var devolucion = agent.SI_ZMPWS_DATAAGRO_CONTRATO_CANJE_GENE(rq);
            logger.Debug("Numero de contratos pendientes: " + devolucion.EX_SALIDA.Count());
            var listaContratos = devolucion.EX_SALIDA.Where(x => x.CONTRNUM.StartsWith("000" + filtro.TrimStart('0')));
            var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
            var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
            var monedas = repositorio.Listar<Moneda>();
            foreach (var contrato in listaContratos)
            {
                var cantidad = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 3 && x.Virtual == true && x.ContratoSAP == contrato.CONTRNUM && x.Id != idFijacion
                && x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado).Sum(x => x.Cantidad + (x.Ampliaciones ?? 0));

                var centro = repositorio.Obtener<Centro>(x => x.CodigoSap == contrato.CENTRO);
                //var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<Negocio, double>(x => x.TipoNegocioId == 3 && x.Id == idFijacion && x.ContratoSAP == contrato.CONTRATO, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;
                //var calidades = new List<CalidadDto>();                       
                var contratoParaFijacion = new DatosFijacionDeContratoDto
                {
                    ContratoId = contrato.CONTRNUM.TrimStart('0'),
                    KilosAplicados = ((double)contrato.KILOS_FIJADOS + cantidad).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    ARecibirSinPrecio = "0",
                    RecibidoSinFijar = "0",
                    KilosPendiente = ((double)contrato.KILOS_A_FIJAR - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaDesde = DateTime.Parse(contrato.FECHA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaHasta = DateTime.Parse(contrato.FECHA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    DesdeEntrega = DateTime.Parse(contrato.FECHA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    HastaEntrega = DateTime.Parse(contrato.FECHA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    Campana = contrato.COSECHA,
                    CampanaId = repositorio.Obtener<Campaña, int>(x => x.Descripcion == contrato.COSECHA, x => x.CampañaId),
                    Centro = centro.Id,
                    CentroDescripcion = centro.Descripcion,
                    Filtro = filtro + "|" + contrato.CONTRNUM.TrimStart('0'),
                    Color = DateTime.Parse(contrato.FECHA_HASTA) < hoy ? "Red" : "#26337b",
                    KgContratoTotal = contrato.CANTIDAD,
                    KilosContrato = contrato.UNIME,
                    Virtual = true,
                };
                var contratoDeBase = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contrato.CONTRNUM);
                if (contratoDeBase != null)
                {
                    var aperturas = new List<AperturaPrecioDto>();
                    foreach (var apertura in contratoDeBase.AperturaPrecio)
                    {
                        apertura.MonedaId = apertura.MonedaId ?? "";
                        var a = new AperturaPrecioDto()
                        {
                            ConceptoAperturaPrecioId = apertura != null ? apertura.ConceptoAperturaPrecioId : 0,
                            ConceptoAperturaPrecio = apertura != null ? apertura.ConceptoAperturaPrecio.Descripcion : "",
                            Importe = apertura.Importe != 0 ? apertura.Importe : 0,
                            Porcentaje = apertura.Porcentaje != 0 ? apertura.Porcentaje : 0,
                            MonedaId = apertura.MonedaId,
                            Moneda = apertura.Moneda != null ? monedas.Where(x => x.Descripcion.Trim() == apertura.Moneda.Descripcion.Trim()).FirstOrDefault().Descripcion : ""
                        };
                        aperturas.Add(a);
                    }
                    var bonificaciones = new List<DescuentoBonificacionDto>();

                    foreach (var bonif in contratoDeBase.Descuentos)
                    {
                        bonif.MonedaId = bonif.MonedaId ?? "";
                        var a = new DescuentoBonificacionDto()
                        {
                            FechaDesde = bonif.FechaDesde != null ? bonif.FechaDesde.Value.ToString("dd-MM-yyyy") : "",
                            FechaHasta = bonif.FechaHasta != null ? bonif.FechaHasta.Value.ToString("dd-MM-yyyy") : "",
                            Importe = bonif.Importe != 0 ? bonif.Importe : 0,
                            Porcentaje = bonif.Porcentaje != 0 ? bonif.Porcentaje : 0,
                            MonedaId = bonif.MonedaId,
                            Moneda = bonif.Moneda == null ? "" : bonif.Moneda.Descripcion == "" ? "" : monedas.Where(x => x.Descripcion.Trim() == bonif.Moneda.Descripcion.Trim()).FirstOrDefault().Descripcion
                        };
                        bonificaciones.Add(a);
                    }
                    contratoParaFijacion.Aperturas = aperturas;
                    contratoParaFijacion.Bonificaciones = bonificaciones;
                    contratoParaFijacion.CentroDescripcion = centro.Descripcion;
                    var descuentoGeneralFueraPrecio = contratoDeBase.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 2).FirstOrDefault();
                    var descuentoGeneralSobrePrecio = contratoDeBase.Descuentos.AsQueryable().Where(x => x.TipoPeriodoDBId == 1 && x.TipoDBId == 1).FirstOrDefault();
                    contratoParaFijacion.ImporteSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.Importe : 0;
                    contratoParaFijacion.MonedaSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Importe != 0 ? descuentoGeneralSobrePrecio.MonedaId : null;
                    contratoParaFijacion.PorcentajeSobrePrecio = descuentoGeneralSobrePrecio != null && descuentoGeneralSobrePrecio.Porcentaje != 0 ? descuentoGeneralSobrePrecio.Porcentaje : 0;
                    contratoParaFijacion.ImporteAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.Importe : 0;
                    contratoParaFijacion.MonedaAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Importe != 0 ? descuentoGeneralFueraPrecio.MonedaId : null;
                    contratoParaFijacion.PorcentajeAPrecio = descuentoGeneralFueraPrecio != null && descuentoGeneralFueraPrecio.Porcentaje != 0 ? descuentoGeneralFueraPrecio.Porcentaje : 0;
                    contratoParaFijacion.Clasificacion = contratoDeBase.Clasificacion.Descripcion;
                    var contratoConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == contratoDeBase.Id);

                    if (!contratoConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                    {
                        datosContratos.Add(contratoParaFijacion);
                    }
                }
                var contratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contrato.CONTRNUM, x => x.Id);
                var contratoAnulado = repositorio.Obtener<Contrato>(x => x.AnulaYReemplazaContratoId == contratoId && x.EstadoId == 5);
                if (contratoAnulado != null)
                {
                    listaContratos = listaContratos.Where(x => x.CONTRNUM != contratoAnulado.ContratoSAP);
                }
            }
            return datosContratos.OrderBy(x => x.ContratoId).ToList();
        }

        public List<DatosFijacionDeContratoDto> ObtenerContratosCanje(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            filtro = filtro ?? "";
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var json = "{\"ARecibirSinPrecio\":\"98.000\",\"Anticipo\":false,\"Aperturas\":[{\"ConceptoAperturaPrecio\":\"Financiero\",\"ConceptoAperturaPrecioId\":1,\"FijacionId\":null,\"Id\":0,\"Importe\":120.0,\"Moneda\":\"USD\",\"MonedaId\":\"USDM\",\"Porcentaje\":0.0,\"contratoId\":null}],\"Calidad\":false,\"Calidades\":[],\"Campana\":\"19-20\",\"CampanaId\":8,\"Centro\":1,\"CentroDescripcion\":\"San Lorenzo\",\"Cesion\":false,\"ChequeElectronico\":null,\"Clasificacion\":\"OTROS\",\"Color\":\"#26337b\",\"CondicionFijacionCod\":\"07\",\"CondicionFijacionDescripcion\":\"MERCADO MOA\",\"CondicionPagoCod\":\"04\",\"CondicionPagoDescripcion\":\"4 DÍAS HÁBILES DE FECHA DE FIJACIÓN\",\"ContratoId\":\"2699076\",\"DesdeEntrega\":\"23-04-2021\",\"FechaDesde\":\"23-04-2021\",\"FechaHasta\":\"23-05-2021\",\"FijacionSap\":null,\"Filtro\":\"2699076|2699076\",\"HastaEntrega\":\"23-05-2021\",\"ImporteAPrecio\":0.0,\"ImporteSobrePrecio\":12.0,\"KilosAplicados\":\"0\",\"KilosContrato\":\"98.000\",\"Virtual\":\"true\",\"KilosPendiente\":\"98.000\",\"MonedaAPrecio\":\"\",\"MonedaSobrePrecio\":\"USDM\",\"PagoDiferido\":false,\"PorcentajeAPrecio\":0.0,\"PorcentajeSobrePrecio\":0.0,\"Posicion\":\"04.2021\",\"RecibidoSinFijar\":\"0\"}";
                DatosFijacionDeContratoDto contrato = json.FromJson<DatosFijacionDeContratoDto>();
                datosContratos.Add(contrato);
            }
            else
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        datosContratos = ObtenerContratosCanjeSinPI(CuitProveedor, CuitCorredor, materialId, filtro, idFijacion);
                    }
                    else
                    {
                        datosContratos = ObtenerContratosCanjeConPI(CuitProveedor, CuitCorredor, materialId, filtro, idFijacion);
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP en ContratosParaFijacionVirtualAgent ", e);
                    throw;
                }
            }
            return datosContratos;
        }
    }
}