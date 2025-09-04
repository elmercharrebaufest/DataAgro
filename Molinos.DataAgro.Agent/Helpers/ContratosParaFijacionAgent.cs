using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent.ContratosParaFijacion;
using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class ContratosParaFijacionAgent : IContratosParaFijacionAgent
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly string UserSap = ConfigurationManager.AppSettings["SapUser"];
        private readonly string PassSap = ConfigurationManager.AppSettings["SapPass"];

        public ContratosParaFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        private List<DatosFijacionDeContratoDto> CargarContratosSinPi(ZMprfcContratoPendFijacionResponse devolucion, int materialId, string filtro, int idFijacion)
        {
            var hoy = DateTime.Now.Date;
            var listaContratos = devolucion.ExSalida.Where(x => x.Contrato.StartsWith("000" + filtro.TrimStart('0')));
            var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
            var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
            var monedas = repositorio.Listar<Moneda>();
            var campañas = repositorio.Listar<Campaña>();
            var condicionPagos = repositorio.Listar<CondicionPago>();
            var condicionFijaciones = repositorio.Listar<CondicionFijacion>();
            var centros = repositorio.Listar<Centro>();
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            CultureInfo provider = CultureInfo.InvariantCulture;

            var contratosSap = listaContratos.Select(a => a.Contrato).ToList();
            var contratos = repositorio.Listar<Contrato>(x => contratosSap.Contains(x.ContratoSAP) && x.EstadoId == 5 && x.TipoNegocioId == 1);
            foreach (var contrato in listaContratos)
            {
                var cantidad = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 3 && x.Virtual != true && x.ContratoSAP == contrato.Contrato && x.Id != idFijacion
                 && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum(x => x.Cantidad + (x.Ampliaciones ?? 0));
                var centro = centros.Where(x => x.CodigoSap == contrato.Centro).FirstOrDefault();
                var calidades = new List<CalidadDto>();
                foreach (var calidad in contrato.Calidades)
                {
                    var c = new CalidadDto()
                    {
                        PorcentajeDesde = calidad.PorcDesde,
                        PorcentajeHasta = calidad.PorcHasta,
                        Valor = calidad.Valor,
                        CalidadEspecialDesc = calidadesEspeciales.Where(x => x.CodigoSap == calidad.Codigo).FirstOrDefault().Descripcion
                    };
                    calidades.Add(c);
                }

                var aperturas = new List<AperturaPrecioDto>();
                foreach (var apertura in contrato.Apertura)
                {
                    apertura.Moneda = apertura.Moneda ?? "";
                    var a = new AperturaPrecioDto()
                    {
                        ConceptoAperturaPrecioId = conceptoAperturas.Where(x => x.CodigoSap == apertura.Concepto).FirstOrDefault().Id,
                        ConceptoAperturaPrecio = conceptoAperturas.Where(x => x.CodigoSap == apertura.Concepto).FirstOrDefault().Descripcion,
                        Importe = apertura.Importe,
                        Porcentaje = apertura.Porc,
                        MonedaId = apertura.Moneda,
                        Moneda = monedas.Where(x => x.MonedaId.Trim() == apertura.Moneda.Trim()).FirstOrDefault()?.Descripcion
                    };
                    aperturas.Add(a);
                }
                var bonificaciones = new List<DescuentoBonificacionDto>();

                foreach (var bonif in contrato.BonifFijacion)
                {
                    bonif.MonedaDb = bonif.MonedaDb ?? "";
                    var a = new DescuentoBonificacionDto()
                    {
                        FechaDesde = DateTime.ParseExact(bonif.Fedesde, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        FechaHasta = DateTime.ParseExact(bonif.Fehasta, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        Importe = bonif.ImporteDb,
                        Porcentaje = bonif.PorcDb,
                        MonedaId = bonif.MonedaDb,
                        Moneda = bonif.MonedaDb == "" ? "" : monedas.Where(x => x.MonedaId.Trim() == bonif.MonedaDb.Trim()).FirstOrDefault().Descripcion
                    };
                    bonificaciones.Add(a);
                }

                var sustentables = new List<SustentableDto>();
                foreach (var sustentable in contrato.Sustentable)
                {
                    sustentables.Add(new SustentableDto
                    {
                        FechaDesde = DateTime.ParseExact(sustentable.Fedesde, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        FechaHasta = DateTime.ParseExact(sustentable.Fehasta, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        Importe = sustentable.ImporteDb,
                        MonedaId = sustentable.MonedaDb,
                    });
                }
                var contratoParaFijacion = new DatosFijacionDeContratoDto();
                contratoParaFijacion.ContratoId = contrato.Contrato.TrimStart('0');
                contratoParaFijacion.KilosAplicados = ((double)contrato.KilosAplicados).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.KilosPendiente = ((double)contrato.KilosPendFijar - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.FechaDesde = DateTime.Parse(contrato.FechaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.FechaHasta = DateTime.Parse(contrato.FechaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.KilosContrato = contrato.KilosContrato.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.DesdeEntrega = DateTime.Parse(contrato.EntregaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.HastaEntrega = DateTime.Parse(contrato.EntregaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.Calidad = ((materialId == 3 || materialId == 4 || materialId == 5) && contrato.Calidad == "X") || ((materialId == 1 || materialId == 2) && contrato.Calidad != "X");
                contratoParaFijacion.Campana = contrato.Cosecha;
                contratoParaFijacion.CampanaId = campañas.Where(x => x.Descripcion == contrato.Cosecha).FirstOrDefault() != null ?
                    campañas.Where(x => x.Descripcion == contrato.Cosecha).FirstOrDefault().CampañaId : 0;
                contratoParaFijacion.Posicion = contrato.Posicion;
                contratoParaFijacion.PagoDiferido = contrato.PagoDifArp == "X";
                contratoParaFijacion.Centro = centro.Id;
                contratoParaFijacion.ARecibirSinPrecio = contrato.ARecibirSinPrecio.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.RecibidoSinFijar = contrato.RecibidoSinFijar.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.CentroDescripcion = centro.Descripcion;
                contratoParaFijacion.ImporteAPrecio = contrato.ImporteAPrecio;
                contratoParaFijacion.ImporteSobrePrecio = contrato.ImporteSPrecio;
                contratoParaFijacion.MonedaAPrecio = contrato.MonedaAPrecio;
                contratoParaFijacion.MonedaSobrePrecio = contrato.MonedaSPrecio;
                contratoParaFijacion.PorcentajeAPrecio = contrato.PorcAPrecio;
                contratoParaFijacion.PorcentajeSobrePrecio = contrato.PorcSPrecio;
                contratoParaFijacion.CondicionFijacionCod = contrato.CondFijacion;
                contratoParaFijacion.CondicionPagoCod = contrato.CondPago;
                contratoParaFijacion.CondicionFijacionDescripcion = condicionFijaciones.Where(x => x.CodigoSap == contrato.CondFijacion).FirstOrDefault() != null ?
                    condicionFijaciones.Where(x => x.CodigoSap == contrato.CondFijacion).FirstOrDefault().Descripcion : "";
                contratoParaFijacion.CondicionPagoDescripcion = condicionPagos.Where(x => x.CodigoSap == contrato.CondPago).SingleOrDefault() != null ?
                    condicionPagos.Where(x => x.CodigoSap == contrato.CondPago).SingleOrDefault().Descripcion : "";
                contratoParaFijacion.Filtro = filtro + "|" + contrato.Contrato.TrimStart('0');
                contratoParaFijacion.Color = DateTime.Parse(contrato.FechaHasta) < hoy ? "Red" : "";
                contratoParaFijacion.Calidades = calidades;
                contratoParaFijacion.Clasificacion = contrato.Clasificacion;
                contratoParaFijacion.Cesion = contrato.Cesion == "X";
                contratoParaFijacion.Anticipo = contrato.Anticipo == "X";
                contratoParaFijacion.Aperturas = aperturas;
                contratoParaFijacion.Bonificaciones = bonificaciones;
                contratoParaFijacion.Virtual = false;
                contratoParaFijacion.Sustentables = sustentables;

                var existeConAnulaYReemplaza = false;
                var contDA = contratos.Where(x => x.ContratoSAP == contrato.Contrato && x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && x.EstadoId == (int)EnumEstadoContrato.Finalizado).FirstOrDefault();
                logger.Debug("contrato.CONTRATO " + contrato.Contrato);

                contratoParaFijacion.MercAplicada = contrato.KilosAplicados > 0;
                contratoParaFijacion.TarifaAConvenir = sustentables.Any(a => a.Importe == -1);
                if (contDA != null)
                {
                    contratoParaFijacion.MercsDeposito = contDA.MercsDeposito ?? false;
                    contratoParaFijacion.Pase = contDA.TipoPosicionCBOTId == 3;
                    var idContratoConAnulaYReemplaza = contDA.Id;
                    existeConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == contDA.Id);
                    contratoParaFijacion.ProveedorComisionistaId = contDA.ProveedorComisionistaId ?? null;
                    contratoParaFijacion.Sustentable = contDA.Sustentable;
                    contratoParaFijacion.EPA = contDA.EPA;
                    contratoParaFijacion.EUDR = contDA.EUDR;
                    contratoParaFijacion.MonedaSustentable = contDA.MonedaSustentable;
                    contratoParaFijacion.ImporteSustentable = contDA.ImporteSustentable;
                    contratoParaFijacion.SustentableTipoDBId = contDA.SustentableTipoDBId;
                }

                if (!existeConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                {
                    datosContratos.Add(contratoParaFijacion);
                }
            }
            return datosContratos;
        }

        private List<DatosFijacionDeContratoDto> CargarContratosConPi(Z_MPRFC_CONTRATO_PEND_FIJACIONResponse devolucion, int materialId, string filtro, int idFijacion)
        {
            var hoy = DateTime.Now.Date;
            var listaContratos = devolucion.EX_SALIDA.Where(x => x.CONTRATO.StartsWith("000" + filtro.TrimStart('0')));
            var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
            var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
            var monedas = repositorio.Listar<Moneda>();
            var campañas = repositorio.Listar<Campaña>();
            var condicionPagos = repositorio.Listar<CondicionPago>();
            var condicionFijaciones = repositorio.Listar<CondicionFijacion>();
            var centros = repositorio.Listar<Centro>();
            var datosContratos = new List<DatosFijacionDeContratoDto>();

            CultureInfo provider = CultureInfo.InvariantCulture;

            var contratosSap = listaContratos.Select(a => a.CONTRATO).ToList();
            var contratos = repositorio.Listar<Contrato>(x => contratosSap.Contains(x.ContratoSAP) && x.EstadoId == 5 && x.TipoNegocioId == 1);
            foreach (var contrato in listaContratos)
            {
                var cantidad = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 3 && x.Virtual != true && x.ContratoSAP == contrato.CONTRATO && x.Id != idFijacion
                 && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum(x => x.Cantidad + (x.Ampliaciones ?? 0));
                var centro = centros.Where(x => x.CodigoSap == contrato.CENTRO).FirstOrDefault();
                var calidades = new List<CalidadDto>();
                foreach (var calidad in contrato.CALIDADES)
                {
                    var c = new CalidadDto()
                    {
                        PorcentajeDesde = calidad.PORC_DESDE,
                        PorcentajeHasta = calidad.PORC_HASTA,
                        Valor = calidad.VALOR,
                        CalidadEspecialDesc = calidadesEspeciales.Where(x => x.CodigoSap == calidad.CODIGO).FirstOrDefault().Descripcion
                    };
                    calidades.Add(c);
                }

                var aperturas = new List<AperturaPrecioDto>();
                foreach (var apertura in contrato.APERTURA)
                {
                    apertura.MONEDA = apertura.MONEDA ?? "";
                    var a = new AperturaPrecioDto()
                    {
                        ConceptoAperturaPrecioId = conceptoAperturas.Where(x => x.CodigoSap == apertura.CONCEPTO).FirstOrDefault().Id,
                        ConceptoAperturaPrecio = conceptoAperturas.Where(x => x.CodigoSap == apertura.CONCEPTO).FirstOrDefault().Descripcion,
                        Importe = apertura.IMPORTE,
                        Porcentaje = apertura.PORC,
                        MonedaId = apertura.MONEDA,
                        Moneda = monedas.Where(x => x.MonedaId.Trim() == apertura.MONEDA.Trim()).FirstOrDefault()?.Descripcion
                    };
                    aperturas.Add(a);
                }
                var bonificaciones = new List<DescuentoBonificacionDto>();

                foreach (var bonif in contrato.BONIF_FIJACION)
                {
                    bonif.MONEDA_DB = bonif.MONEDA_DB ?? "";
                    var a = new DescuentoBonificacionDto()
                    {
                        FechaDesde = DateTime.ParseExact(bonif.FEDESDE, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        FechaHasta = DateTime.ParseExact(bonif.FEHASTA, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        Importe = bonif.IMPORTE_DB,
                        Porcentaje = bonif.PORC_DB,
                        MonedaId = bonif.MONEDA_DB,
                        Moneda = bonif.MONEDA_DB == "" ? "" : monedas.Where(x => x.MonedaId.Trim() == bonif.MONEDA_DB.Trim()).FirstOrDefault().Descripcion
                    };
                    bonificaciones.Add(a);
                }

                var sustentables = new List<SustentableDto>();
                foreach (var sustentable in contrato.SUSTENTABLE)
                {
                    sustentables.Add(new SustentableDto
                    {
                        FechaDesde = DateTime.ParseExact(sustentable.FEDESDE, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        FechaHasta = DateTime.ParseExact(sustentable.FEHASTA, "yyyy-MM-dd", provider).ToString("dd-MM-yyyy"),
                        Importe = sustentable.IMPORTE_DB,
                        MonedaId = sustentable.MONEDA_DB,
                    });
                }


                var contratoParaFijacion = new DatosFijacionDeContratoDto();
                contratoParaFijacion.ContratoId = contrato.CONTRATO.TrimStart('0');
                contratoParaFijacion.KilosAplicados = ((double)contrato.KILOS_APLICADOS).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.KilosPendiente = ((double)contrato.KILOS_PEND_FIJAR - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.FechaDesde = DateTime.Parse(contrato.FECHA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.FechaHasta = DateTime.Parse(contrato.FECHA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.KilosContrato = contrato.KILOS_CONTRATO.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.DesdeEntrega = DateTime.Parse(contrato.ENTREGA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.HastaEntrega = DateTime.Parse(contrato.ENTREGA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.Calidad = ((materialId == 3 || materialId == 4 || materialId == 5) && contrato.CALIDAD == "X") || ((materialId == 1 || materialId == 2) && contrato.CALIDAD != "X");
                contratoParaFijacion.Campana = contrato.COSECHA;
                contratoParaFijacion.CampanaId = campañas.Where(x => x.Descripcion == contrato.COSECHA).FirstOrDefault() != null ?
                    campañas.Where(x => x.Descripcion == contrato.COSECHA).FirstOrDefault().CampañaId : 0;
                contratoParaFijacion.Posicion = contrato.POSICION;
                contratoParaFijacion.PagoDiferido = contrato.PAGO_DIF_ARP == "X";
                contratoParaFijacion.Centro = centro.Id;
                contratoParaFijacion.ARecibirSinPrecio = contrato.A_RECIBIR_SIN_PRECIO.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.RecibidoSinFijar = contrato.RECIBIDO_SIN_FIJAR.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contratoParaFijacion.CentroDescripcion = centro.Descripcion;
                contratoParaFijacion.ImporteAPrecio = contrato.IMPORTE_A_PRECIO;
                contratoParaFijacion.ImporteSobrePrecio = contrato.IMPORTE_S_PRECIO;
                contratoParaFijacion.MonedaAPrecio = contrato.MONEDA_A_PRECIO;
                contratoParaFijacion.MonedaSobrePrecio = contrato.MONEDA_S_PRECIO;
                contratoParaFijacion.PorcentajeAPrecio = contrato.PORC_A_PRECIO;
                contratoParaFijacion.PorcentajeSobrePrecio = contrato.PORC_S_PRECIO;
                contratoParaFijacion.CondicionFijacionCod = contrato.COND_FIJACION;
                contratoParaFijacion.CondicionPagoCod = contrato.COND_PAGO;
                contratoParaFijacion.CondicionFijacionDescripcion = condicionFijaciones.Where(x => x.CodigoSap == contrato.COND_FIJACION).FirstOrDefault() != null ?
                    condicionFijaciones.Where(x => x.CodigoSap == contrato.COND_FIJACION).FirstOrDefault().Descripcion : "";
                contratoParaFijacion.CondicionPagoDescripcion = condicionPagos.Where(x => x.CodigoSap == contrato.COND_PAGO).SingleOrDefault() != null ?
                    condicionPagos.Where(x => x.CodigoSap == contrato.COND_PAGO).SingleOrDefault().Descripcion : "";
                contratoParaFijacion.Filtro = filtro + "|" + contrato.CONTRATO.TrimStart('0');
                contratoParaFijacion.Color = DateTime.Parse(contrato.FECHA_HASTA) < hoy ? "Red" : "";
                contratoParaFijacion.Calidades = calidades;
                contratoParaFijacion.Clasificacion = contrato.CLASIFICACION;
                contratoParaFijacion.Cesion = contrato.CESION == "X";
                contratoParaFijacion.Anticipo = contrato.ANTICIPO == "X";
                contratoParaFijacion.Aperturas = aperturas;
                contratoParaFijacion.Bonificaciones = bonificaciones;
                contratoParaFijacion.Virtual = false;
                contratoParaFijacion.Sustentables = sustentables;

                var existeConAnulaYReemplaza = false;
                var contDA = contratos.Where(x => x.ContratoSAP == contrato.CONTRATO && x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && x.EstadoId == (int)EnumEstadoContrato.Finalizado).FirstOrDefault();
                logger.Debug("contrato.CONTRATO " + contrato.CONTRATO);

                contratoParaFijacion.MercAplicada = contrato.KILOS_APLICADOS > 0;
                contratoParaFijacion.TarifaAConvenir = sustentables.Any(a => a.Importe == -1);
                if (contDA != null)
                {
                    contratoParaFijacion.MercsDeposito = contDA.MercsDeposito ?? false;
                    contratoParaFijacion.Pase = contDA.TipoPosicionCBOTId == 3;
                    var idContratoConAnulaYReemplaza = contDA.Id;
                    existeConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == contDA.Id);
                    contratoParaFijacion.ProveedorComisionistaId = contDA.ProveedorComisionistaId ?? null;
                    contratoParaFijacion.Sustentable = contDA.Sustentable;
                    contratoParaFijacion.EPA = contDA.EPA;
                    contratoParaFijacion.EUDR = contDA.EUDR;
                    contratoParaFijacion.MonedaSustentable = contDA.MonedaSustentable;
                    contratoParaFijacion.ImporteSustentable = contDA.ImporteSustentable;
                    contratoParaFijacion.SustentableTipoDBId = contDA.SustentableTipoDBId;
                }

                if (!existeConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                {
                    datosContratos.Add(contratoParaFijacion);
                }
            }
            return datosContratos;
        }

        public List<DatosFijacionDeContratoDto> ObtenerContratos(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            filtro = filtro ?? "";
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            var hoy = DateTime.Now.Date;
            Z_MPRFC_CONTRATO_PEND_FIJACIONResponse devolucion = new Z_MPRFC_CONTRATO_PEND_FIJACIONResponse();
            ZMprfcContratoPendFijacionResponse devolucionSinPI = new ZMprfcContratoPendFijacionResponse();

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var json = "{\"PropertyChanged\":null,\"eX_SALIDAField\":[{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1168\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002657206\",\"cOSECHAField\":\"19-20\",\"eNTREGA_DESDEField\":\"2020-08-17\",\"eNTREGA_HASTAField\":\"2021-01-07\",\"fECHA_DESDEField\":\"2020-08-21\",\"fECHA_HASTAField\":\"2021-03-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":995467.0,\"kILOS_CONTRATOField\":995467,\"kILOS_PEND_FIJARField\":467.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"X\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"08.2020\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":467.0,\"sUSTENTABLEField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2020-08-17\",\"fEHASTAField\":\"2020-08-20\",\"iMPORTE_DBField\":2.0,\"mONEDA_DBField\":\"USDM\"},{\"PropertyChanged\":null,\"fEDESDEField\":\"2020-08-21\",\"fEHASTAField\":\"2020-09-21\",\"iMPORTE_DBField\":2.0,\"mONEDA_DBField\":\"USDM\"},{\"PropertyChanged\":null,\"fEDESDEField\":\"2020-09-22\",\"fEHASTAField\":\"2021-01-07\",\"iMPORTE_DBField\":2.0,\"mONEDA_DBField\":\"USDM\"}],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[{\"PropertyChanged\":null,\"cONCEPTOField\":\"BO\",\"iMPORTEField\":15.0,\"mONEDAField\":\"USDM\",\"pORCField\":0.0}],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":600000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002663423\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2021-06-01\",\"eNTREGA_HASTAField\":\"2021-06-30\",\"fECHA_DESDEField\":\"2021-05-07\",\"fECHA_HASTAField\":\"2022-04-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":15.0,\"kILOS_APLICADOSField\":2000000.0,\"kILOS_CONTRATOField\":2000000,\"kILOS_PEND_FIJARField\":1775000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"USDM\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":0.0,\"pOSICIONField\":\"06.2021\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":1775000.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-07-28\",\"fEHASTAField\":\"2022-04-30\",\"iMPORTE_DBField\":12.5,\"mONEDA_DBField\":\"USDM\",\"pORC_DBField\":0.0}],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002665528\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2021-05-02\",\"eNTREGA_HASTAField\":\"2021-08-28\",\"fECHA_DESDEField\":\"2021-07-28\",\"fECHA_HASTAField\":\"2022-04-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":1000000.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":0.0,\"pOSICIONField\":\"07.2021\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":1000000.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-07-02\",\"fEHASTAField\":\"2022-04-30\",\"iMPORTE_DBField\":13.0,\"mONEDA_DBField\":\"USDM\",\"pORC_DBField\":1.0}],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002665018\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2021-05-01\",\"eNTREGA_HASTAField\":\"2021-05-31\",\"fECHA_DESDEField\":\"2021-07-02\",\"fECHA_HASTAField\":\"2022-04-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":1000000.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"05.2021\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":1000000.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664771\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-03-15\",\"eNTREGA_HASTAField\":\"2022-04-15\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-05-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"03.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664772\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-03-15\",\"eNTREGA_HASTAField\":\"2022-04-15\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-05-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"03.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":600000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002660918\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2021-07-13\",\"eNTREGA_HASTAField\":\"2022-02-15\",\"fECHA_DESDEField\":\"2021-03-09\",\"fECHA_HASTAField\":\"2022-06-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":1.5,\"kILOS_APLICADOSField\":2000000.0,\"kILOS_CONTRATOField\":2000000,\"kILOS_PEND_FIJARField\":1080000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"USDM\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"01.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":1080000.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1994973.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":600000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002660933\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2022-02-01\",\"eNTREGA_HASTAField\":\"2022-02-28\",\"fECHA_DESDEField\":\"2021-03-09\",\"fECHA_HASTAField\":\"2022-06-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":1.5,\"kILOS_APLICADOSField\":5027.0,\"kILOS_CONTRATOField\":2000000,\"kILOS_PEND_FIJARField\":2000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"USDM\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"02.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":5027.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664773\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-04-01\",\"eNTREGA_HASTAField\":\"2022-04-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-06-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"04.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664774\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-04-01\",\"eNTREGA_HASTAField\":\"2022-04-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-06-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"04.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664775\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-04-01\",\"eNTREGA_HASTAField\":\"2022-04-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-06-30\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"04.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664776\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-05-01\",\"eNTREGA_HASTAField\":\"2022-05-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-08-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"05.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664777\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-05-01\",\"eNTREGA_HASTAField\":\"2022-05-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-08-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"05.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664778\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-05-01\",\"eNTREGA_HASTAField\":\"2022-05-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-08-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"05.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":900000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002669680\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2022-01-17\",\"eNTREGA_HASTAField\":\"2022-02-17\",\"fECHA_DESDEField\":\"2022-01-17\",\"fECHA_HASTAField\":\"2022-10-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":900000.0,\"kILOS_CONTRATOField\":900000,\"kILOS_PEND_FIJARField\":900000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"01.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":900000.0,\"sUSTENTABLEField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2022-01-17\",\"fEHASTAField\":\"2022-02-17\",\"iMPORTE_DBField\":5.0,\"mONEDA_DBField\":\"USDM\"}],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":0.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":1000000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002669389\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2021-11-01\",\"eNTREGA_HASTAField\":\"2022-01-25\",\"fECHA_DESDEField\":\"2022-01-03\",\"fECHA_HASTAField\":\"2022-10-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":1000000.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"01.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":1000000.0,\"sUSTENTABLEField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-11-01\",\"fEHASTAField\":\"2022-01-02\",\"iMPORTE_DBField\":5.0,\"mONEDA_DBField\":\"USDM\"},{\"PropertyChanged\":null,\"fEDESDEField\":\"2022-01-03\",\"fEHASTAField\":\"2022-01-25\",\"iMPORTE_DBField\":5.0,\"mONEDA_DBField\":\"USDM\"}],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":78489.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002670350\",\"cOSECHAField\":\"20-21\",\"eNTREGA_DESDEField\":\"2022-02-08\",\"eNTREGA_HASTAField\":\"2022-02-28\",\"fECHA_DESDEField\":\"2022-02-08\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":821511.0,\"kILOS_CONTRATOField\":900000,\"kILOS_PEND_FIJARField\":900000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"02.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":821511.0,\"sUSTENTABLEField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2022-02-08\",\"fEHASTAField\":\"2022-02-28\",\"iMPORTE_DBField\":5.0,\"mONEDA_DBField\":\"USDM\"}],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664784\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-10-01\",\"eNTREGA_HASTAField\":\"2022-10-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"10.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664783\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-10-01\",\"eNTREGA_HASTAField\":\"2022-10-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"10.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664782\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-09-01\",\"eNTREGA_HASTAField\":\"2022-09-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"09.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664781\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-09-01\",\"eNTREGA_HASTAField\":\"2022-09-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"09.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664780\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-08-01\",\"eNTREGA_HASTAField\":\"2022-08-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"08.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664779\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-08-01\",\"eNTREGA_HASTAField\":\"2022-08-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2022-12-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"08.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664785\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-11-01\",\"eNTREGA_HASTAField\":\"2022-11-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2023-01-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"11.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664786\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-11-01\",\"eNTREGA_HASTAField\":\"2022-11-30\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2023-01-31\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"11.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1000000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":300000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664787\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-12-01\",\"eNTREGA_HASTAField\":\"2022-12-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2023-02-28\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1000000,\"kILOS_PEND_FIJARField\":1000000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"12.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"},{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":1560000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":468000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002664788\",\"cOSECHAField\":\"21-22\",\"eNTREGA_DESDEField\":\"2022-12-01\",\"eNTREGA_HASTAField\":\"2022-12-31\",\"fECHA_DESDEField\":\"2021-06-11\",\"fECHA_HASTAField\":\"2023-02-28\",\"fIJ_CBOT_MATField\":\"\",\"gRUPO_COMPRASField\":\"900\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0.0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":1560000,\"kILOS_PEND_FIJARField\":1560000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"12.2022\",\"pOSICION_CBOTField\":\"\",\"rECIBIDO_SIN_FIJARField\":0.0,\"sUSTENTABLEField\":[],\"zONAField\":\"OIC\"}]}";
                devolucion = json.FromJson<Z_MPRFC_CONTRATO_PEND_FIJACIONResponse>();

                //var contratos = repositorio.Listar<Contrato, string>(x => x.ContratoSAP, x => x.TipoNegocioId == 1 && x.MaterialId == materialId && x.Proveedor.CUIT == CuitProveedor && (!string.IsNullOrEmpty(CuitCorredor) ? x.Corredor.CUIT == CuitCorredor : x.CorredorId == null) && x.ContratoSAP != null);
                //if (contratos != null)
                //{
                //    filtro = filtro.TrimStart('0');
                //    var listaContratos = contratos.Where(x => x.StartsWith("000" + filtro));
                //    foreach (var id in listaContratos)
                //    {
                //        var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad + (x.Ampliaciones ?? 0), x => x.ContratoSAP == id
                //        && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum();
                //        var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<FijacionDePrecioContrato, double>(x => x.Id == idFijacion && x.ContratoSAP == id, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;

                //        var contrato = repositorio.Obtener<Contrato, DatosFijacionDeContratoDto>(x => x.ContratoSAP == id && x.TipoNegocioId == 1, x => new DatosFijacionDeContratoDto()
                //        {
                //            ContratoId = id.ToString(),
                //            KilosAplicados = cantidad.ToString(),
                //            KilosPendiente = (x.Cantidad - cantidad + cantidadFijacion).ToString(),
                //            FechaDesde = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "-" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "-" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                //            FechaHasta = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.HastaFijacion) + "-" + SqlFunctions.DatePart("month", x.HastaFijacion) + "-" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                //            KilosContrato = x.Cantidad.ToString(),
                //            DesdeEntrega = SqlFunctions.DateName("day", x.FechaDesde) + "-" + SqlFunctions.DatePart("month", x.FechaDesde) + "-" + SqlFunctions.DateName("year", x.FechaDesde),
                //            HastaEntrega = SqlFunctions.DateName("day", x.FechaHasta) + "-" + SqlFunctions.DatePart("month", x.FechaHasta) + "-" + SqlFunctions.DateName("year", x.FechaHasta),
                //            Posicion = x.FechaDesde.Month.ToString() + "." + x.FechaDesde.Year.ToString(),
                //            Calidad = true,
                //            Campana = x.Campana.Descripcion,
                //            CampanaId = x.Campana.CampañaId,
                //            PagoDiferido = x.PagoDiferido ?? false,
                //            Centro = x.DestinoId,
                //            Color = x.HastaFijacion.HasValue && x.HastaFijacion.Value < hoy ? "Red" : "#26337b",
                //            CentroDescripcion = x.Destino != null ? x.Destino.Descripcion : null,
                //            Calidades = x.Calidad.Select(y => new CalidadDto
                //            {
                //                PorcentajeDesde = y.PorcentajeDesde,
                //                PorcentajeHasta = y.PorcentajeHasta,
                //                Valor = y.Valor,
                //                CalidadEspecialDesc = y.CalidadEspecial.Descripcion
                //            }).ToList()
                //        });
                //        contrato.KilosAplicados = (double.Parse(contrato.KilosAplicados)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                //        contrato.KilosPendiente = (double.Parse(contrato.KilosPendiente)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                //        contrato.KilosContrato = (double.Parse(contrato.KilosContrato)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                //        contrato.ContratoId = contrato.ContratoId.TrimStart('0');
                //        contrato.Filtro = filtro + "|" + contrato.ContratoId;
                //        contrato.ARecibirSinPrecio = 10000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                //        contrato.RecibidoSinFijar = 19000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                //        contrato.ImporteAPrecio = 10;
                //        contrato.ImporteSobrePrecio = -20;
                //        contrato.MonedaAPrecio = "USDM ";
                //        contrato.MonedaSobrePrecio = "USDM ";
                //        contrato.PorcentajeAPrecio = 5;
                //        contrato.PorcentajeSobrePrecio = 10;
                //        contrato.CondicionFijacionCod = "07";
                //        contrato.CondicionPagoCod = "10";
                //        contrato.CondicionFijacionDescripcion = "HASTA 14.30 HS POR PIZ / MERCADERIA";
                //        contrato.CondicionPagoDescripcion = "10 DÍAS HÁBILES DE FECHA DE FIJACIÓN";
                //        contrato.Cesion = false;
                //        contrato.Anticipo = false;
                //        contrato.Clasificacion = "PRODUCTOR";
                //        contrato.Aperturas = new List<AperturaPrecioDto> {

                //            new AperturaPrecioDto{
                //                ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Redespacho,
                //                Importe = 2,
                //                Porcentaje =0,
                //                MonedaId = "USDM ",
                //                Moneda ="USD"
                //            },
                //             new AperturaPrecioDto{
                //                ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Comisiones,
                //                Importe = 0,
                //                Porcentaje =1,
                //                MonedaId = "USDM ",
                //                Moneda ="USD"
                //            },                             
                //            // new AperturaPrecioDto{
                //            //    ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Bonificaciones,
                //            //    Importe = 4,
                //            //    Porcentaje =0,
                //            //    MonedaId = "USDM ",
                //            //    Moneda ="USD"
                //            //},
                //            new AperturaPrecioDto{
                //                ConceptoAperturaPrecioId = (int)EnumConceptoApertura.Basis,
                //                Importe = 5,
                //                Porcentaje =0,
                //                MonedaId = "USDM ",
                //                Moneda ="USD"
                //            }
                //        };
                //        if (double.Parse(contrato.KilosPendiente) > 0)
                //        {
                //            datosContratos.Add(contrato);
                //        }
                //    }
                //}
            }
            else
            {
                try
                {
                    if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                    {
                        Z_MP_WS_DATAAGRO_DIRECTOClient agent = new Z_MP_WS_DATAAGRO_DIRECTOClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;
                        var material = repositorio.Obtener<Material>(x => x.MaterialId == materialId);
                        var rq = new ZMprfcContratoPendFijacion()
                        {
                            ImCorredor = CuitCorredor,
                            ImProveedor = CuitProveedor,
                            ImMaterial = material.Codigo
                        };
                        logger.Debug(rq.ToXml());

                        devolucionSinPI = agent.ZMprfcContratoPendFijacion(rq);

                        logger.Debug("Numero de contratos pendientes:" + devolucionSinPI.ExSalida.Count());
                    }
                    else
                    {
                        SI_ZMPWS_DATAAGRO_CONTRATO_PEND_FIJACIONClient agent = new SI_ZMPWS_DATAAGRO_CONTRATO_PEND_FIJACIONClient();
                        agent.ClientCredentials.UserName.UserName = UserSap;
                        agent.ClientCredentials.UserName.Password = PassSap;
                        var material = repositorio.Obtener<Material>(x => x.MaterialId == materialId);
                        var rq = new Z_MPRFC_CONTRATO_PEND_FIJACION()
                        {
                            IM_CORREDOR = CuitCorredor,
                            IM_PROVEEDOR = CuitProveedor,
                            IM_MATERIAL = material.Codigo
                        };
                        logger.Debug(rq.ToXml());

                        devolucion = agent.SI_ZMPWS_DATAAGRO_CONTRATO_PEND_FIJACION(rq);
                        logger.Debug("Numero de contratos pendientes:" + devolucion.EX_SALIDA.Count());
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP al obtener contratos para fijación.", e);
                    throw;
                }
            }
            try
            {
                if (ConfigurationManager.AppSettings["SAPsinPI"] == "1")
                {
                    datosContratos = CargarContratosSinPi(devolucionSinPI, materialId, filtro, idFijacion);
                }
                else
                {
                    datosContratos = CargarContratosConPi(devolucion, materialId, filtro, idFijacion);
                }


            }
            catch (Exception e)
            {
                logger.Error("ContratosParaFijacionAgent: " + e);
                throw;
            }
            return datosContratos.OrderBy(x => x.ContratoId).ToList();
        }
    }
}