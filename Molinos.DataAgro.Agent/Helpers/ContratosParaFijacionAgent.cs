using Molinos.DataAgro.Agent.WS_GAQ_sin_PI;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using NLog;
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
                var contratoParaFijacion = new DatosFijacionDeContratoDto()
                {
                    ContratoId = contrato.Contrato.TrimStart('0'),
                    KilosAplicados = ((double)contrato.KilosAplicados).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    KilosPendiente = ((double)contrato.KilosPendFijar - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaDesde = DateTime.Parse(contrato.FechaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    FechaHasta = DateTime.Parse(contrato.FechaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    KilosContrato = contrato.KilosContrato.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    DesdeEntrega = DateTime.Parse(contrato.EntregaDesde).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    HastaEntrega = DateTime.Parse(contrato.EntregaHasta).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                    Calidad = ((materialId == 3 || materialId == 4 || materialId == 5) && contrato.Calidad == "X") || ((materialId == 1 || materialId == 2) && contrato.Calidad != "X"),
                    Campana = contrato.Cosecha,
                    CampanaId = campañas.Where(x => x.Descripcion == contrato.Cosecha).FirstOrDefault() != null ?
                        campañas.Where(x => x.Descripcion == contrato.Cosecha).FirstOrDefault().CampañaId : 0,
                    Posicion = contrato.Posicion,
                    PagoDiferido = contrato.PagoDifArp == "X",
                    Centro = centro.Id,
                    ARecibirSinPrecio = contrato.ARecibirSinPrecio.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    RecibidoSinFijar = contrato.RecibidoSinFijar.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                    CentroDescripcion = centro.Descripcion,
                    ImporteAPrecio = contrato.ImporteAPrecio,
                    ImporteSobrePrecio = contrato.ImporteSPrecio,
                    MonedaAPrecio = contrato.MonedaAPrecio,
                    MonedaSobrePrecio = contrato.MonedaSPrecio,
                    PorcentajeAPrecio = contrato.PorcAPrecio,
                    PorcentajeSobrePrecio = contrato.PorcSPrecio,
                    CondicionFijacionCod = contrato.CondFijacion,
                    CondicionPagoCod = contrato.CondPago,
                    CondicionFijacionDescripcion = condicionFijaciones.Where(x => x.CodigoSap == contrato.CondFijacion).FirstOrDefault() != null ?
                        condicionFijaciones.Where(x => x.CodigoSap == contrato.CondFijacion).FirstOrDefault().Descripcion : "",
                    CondicionPagoDescripcion = condicionPagos.Where(x => x.CodigoSap == contrato.CondPago).SingleOrDefault() != null ?
                        condicionPagos.Where(x => x.CodigoSap == contrato.CondPago).SingleOrDefault().Descripcion : "",
                    Filtro = filtro + "|" + contrato.Contrato.TrimStart('0'),
                    Color = DateTime.Parse(contrato.FechaHasta) < hoy ? "Red" : "",
                    Calidades = calidades,
                    Clasificacion = contrato.Clasificacion,
                    Cesion = contrato.Cesion == "X",
                    Anticipo = contrato.Anticipo == "X",
                    Aperturas = aperturas,
                    Bonificaciones = bonificaciones,
                    Virtual = false,
                    Sustentables = sustentables
                };

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
                    contratoParaFijacion.KgMaximos = contDA.KgMaximo;
                    contratoParaFijacion.KgMinimos = contDA.KgMinimo;
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
            ZMprfcContratoPendFijacionResponse devolucionSinPI = new ZMprfcContratoPendFijacionResponse();

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var json = "{\"PropertyChanged\":null,\"exSalidaField\":[{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":4500000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695552\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-09-01\",\"entregaHastaField\":\"2025-10-25\",\"fechaDesdeField\":\"2025-06-30\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":15000000.0,\"kilosContratoField\":15000000,\"kilosPendFijarField\":450000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"09.2025\",\"recibidoSinFijarField\":500000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":4500000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695553\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-09-01\",\"entregaHastaField\":\"2025-10-25\",\"fechaDesdeField\":\"2025-06-30\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":15000000.0,\"kilosContratoField\":15000000,\"kilosPendFijarField\":1350000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"09.2025\",\"recibidoSinFijarField\":1350000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":4500000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695593\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-10-01\",\"entregaHastaField\":\"2025-10-31\",\"fechaDesdeField\":\"2025-07-02\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":15000000.0,\"kilosContratoField\":15000000,\"kilosPendFijarField\":11265707.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"10.2025\",\"recibidoSinFijarField\":11265707.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":4500000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695594\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-10-01\",\"entregaHastaField\":\"2025-11-03\",\"fechaDesdeField\":\"2025-07-02\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":15000000.0,\"kilosContratoField\":15000000,\"kilosPendFijarField\":15000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"10.2025\",\"recibidoSinFijarField\":15000000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695596\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-09-18\",\"entregaHastaField\":\"2025-10-31\",\"fechaDesdeField\":\"2025-07-02\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":10000000.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":90000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"10.2025\",\"recibidoSinFijarField\":90000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":900000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695978\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-10-01\",\"entregaHastaField\":\"2025-11-15\",\"fechaDesdeField\":\"2025-08-06\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":3000000.0,\"kilosContratoField\":3000000,\"kilosPendFijarField\":3000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"10.2025\",\"recibidoSinFijarField\":3000000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":2400000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002695979\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-10-01\",\"entregaHastaField\":\"2025-11-03\",\"fechaDesdeField\":\"2025-08-06\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":8000000.0,\"kilosContratoField\":8000000,\"kilosPendFijarField\":8000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"10.2025\",\"recibidoSinFijarField\":8000000.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":36511.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":30000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697528\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-11-12\",\"entregaHastaField\":\"2025-11-30\",\"fechaDesdeField\":\"2025-11-12\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":36511,\"kilosPendFijarField\":36511.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"11.2025\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[{\"PropertyChanged\":null,\"fedesdeField\":\"2025-11-12\",\"fehastaField\":\"2025-11-30\",\"importeDbField\":0.01,\"monedaDbField\":\"USDM\"}],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":0.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":30000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697531\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-11-12\",\"entregaHastaField\":\"2025-11-30\",\"fechaDesdeField\":\"2025-11-12\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":88072.0,\"kilosContratoField\":88072,\"kilosPendFijarField\":88072.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"11.2025\",\"recibidoSinFijarField\":88072.0,\"sustentableField\":[{\"PropertyChanged\":null,\"fedesdeField\":\"2025-03-12\",\"fehastaField\":\"2025-11-30\",\"importeDbField\":0.01,\"monedaDbField\":\"USDM\"}],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":191101.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":57330.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697544\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-11-12\",\"entregaHastaField\":\"2025-11-30\",\"fechaDesdeField\":\"2025-11-12\",\"fechaHastaField\":\"2025-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":191101,\"kilosPendFijarField\":191101.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"11.2025\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":8000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":2400000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002696151\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-11-01\",\"entregaHastaField\":\"2025-11-30\",\"fechaDesdeField\":\"2025-08-22\",\"fechaHastaField\":\"2026-02-27\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":8000000,\"kilosPendFijarField\":8000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"11.2025\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":1318015.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002696152\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2025-11-01\",\"entregaHastaField\":\"2025-11-30\",\"fechaDesdeField\":\"2025-08-22\",\"fechaHastaField\":\"2026-02-27\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":8681985.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"11.2025\",\"recibidoSinFijarField\":8681985.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697486\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-02-01\",\"entregaHastaField\":\"2026-02-28\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"02.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697487\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-02-01\",\"entregaHastaField\":\"2026-02-28\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"02.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697488\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-02-01\",\"entregaHastaField\":\"2026-02-28\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"02.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697489\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-02-01\",\"entregaHastaField\":\"2026-02-28\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"02.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697490\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-01-15\",\"entregaHastaField\":\"2026-02-15\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"01.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697491\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-01-15\",\"entregaHastaField\":\"2026-02-15\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"01.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697492\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-01-15\",\"entregaHastaField\":\"2026-02-15\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"01.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"},{\"PropertyChanged\":null,\"aRecibirSinPrecioField\":10000000.0,\"anticipoField\":\"\",\"aperturaField\":[],\"bloqueoField\":\"\",\"bonifFijacionField\":[],\"calidadField\":\"\",\"calidadesField\":[],\"canjeField\":\"\",\"cantMaxField\":3000000.0,\"centroField\":\"1029\",\"cesionField\":\"\",\"clasificacionField\":\"ACOPIADOR\",\"compensacionField\":\"\",\"condFijacionField\":\"07\",\"condPagoField\":\"04\",\"contratoField\":\"0002697493\",\"cosechaField\":\"24-25\",\"entregaDesdeField\":\"2026-01-15\",\"entregaHastaField\":\"2026-02-15\",\"fechaDesdeField\":\"2025-11-11\",\"fechaHastaField\":\"2026-12-30\",\"fijCbotMatField\":\"\",\"grupoComprasField\":\"900\",\"importeAPrecioField\":0.0,\"importeSPrecioField\":0.0,\"kilosAplicadosField\":0.0,\"kilosContratoField\":10000000,\"kilosPendFijarField\":10000000.0,\"monedaAPrecioField\":\"\",\"monedaSPrecioField\":\"\",\"pagoDifArpField\":\"\",\"porcAPrecioField\":0.0,\"porcSPrecioField\":0.0,\"posicionCbotField\":\"\",\"posicionField\":\"01.2026\",\"recibidoSinFijarField\":0.0,\"sustentableField\":[],\"zonaField\":\"CRO\"}]}";
                devolucionSinPI = json.FromJson<ZMprfcContratoPendFijacionResponse>();
            }
            else
            {
                try
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
                catch (Exception e)
                {
                    logger.Error(e, "Error comunicacion SAP al obtener contratos para fijación.");
                    throw;
                }
            }
            try
            {
                datosContratos = CargarContratosSinPi(devolucionSinPI, materialId, filtro, idFijacion);

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