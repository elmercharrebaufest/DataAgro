using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Agent.ContratosParaFijacion;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Configuration;
using System.Linq;
using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Helpers;
using System.Globalization;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Entities.Common.Enums;

namespace Molinos.DataAgro.Agent
{
    public class ContratosParaFijacionAgent : IContratosParaFijacionAgent
    {
        public ContratosParaFijacionAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public List<DatosFijacionDeContratoDto> ObtenerContratos(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            filtro = filtro == null ? "" : filtro;
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            var hoy = DateTime.Now.Date;
            Z_MPRFC_CONTRATO_PEND_FIJACIONResponse devolucion;
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var json = "{\"PropertyChanged\":null,\"eX_SALIDAField\":[" +
                    "{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[{\"PropertyChanged\":null,\"cONCEPTOField\":\"BO\",\"iMPORTEField\":1250.0,\"mONEDAField\":\"ARP\",\"pORCField\":0.0}],\"a_RECIBIR_SIN_PRECIOField\":38620.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-05-18\",\"fEHASTAField\":\"2021-05-24\",\"iMPORTE_DBField\":200.0,\"mONEDA_DBField\":\"ARP\",\"pORC_DBField\":2.0}],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":30000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002699205\",\"cOSECHAField\":\"19-20\",\"eNTREGA_DESDEField\":\"2021-05-18\",\"eNTREGA_HASTAField\":\"2021-06-18\",\"fECHA_DESDEField\":\"2021-05-18\",\"fECHA_HASTAField\":\"2021-06-18\",\"gRUPO_COMPRASField\":\"901\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":1250.0,\"kILOS_APLICADOSField\":59380.0,\"kILOS_CONTRATOField\":98000,\"kILOS_PEND_FIJARField\":96000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"ARP\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":0.4,\"pOSICIONField\":\"05.2021\",\"rECIBIDO_SIN_FIJARField\":57380.0,\"zONAField\":\"OIS\"}," +
                    "{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[{\"PropertyChanged\":null,\"cONCEPTOField\":\"RE\",\"iMPORTEField\":1250.0,\"mONEDAField\":\"ARP\",\"pORCField\":0.0}],\"a_RECIBIR_SIN_PRECIOField\":98000.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-05-18\",\"fEHASTAField\":\"2021-05-24\",\"iMPORTE_DBField\":12.0,\"mONEDA_DBField\":\"USDM\",\"pORC_DBField\":0.0}],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":30000.0,\"cENTROField\":\"1029\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002699204\",\"cOSECHAField\":\"19-20\",\"eNTREGA_DESDEField\":\"2021-05-18\",\"eNTREGA_HASTAField\":\"2021-06-18\",\"fECHA_DESDEField\":\"2021-05-18\",\"fECHA_HASTAField\":\"2021-06-18\",\"gRUPO_COMPRASField\":\"901\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0,\"kILOS_APLICADOSField\":0.0,\"kILOS_CONTRATOField\":98000,\"kILOS_PEND_FIJARField\":98000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"ARP\",\"pAGO_DIF_ARPField\":\"\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":1.0,\"pOSICIONField\":\"05.2021\",\"rECIBIDO_SIN_FIJARField\":0.0,\"zONAField\":\"OIS\", \"ProveedorComisionistaIdField\":\"93\" }," +
                    "{\"PropertyChanged\":null,\"aNTICIPOField\":\"\",\"aPERTURAField\":[],\"a_RECIBIR_SIN_PRECIOField\":24098.0,\"bLOQUEOField\":\"\",\"bONIF_FIJACIONField\":[{\"PropertyChanged\":null,\"fEDESDEField\":\"2021-05-18\",\"fEHASTAField\":\"2021-06-18\",\"iMPORTE_DBField\":20.0,\"mONEDA_DBField\":\"USDM\",\"pORC_DBField\":0.0}],\"cALIDADESField\":[],\"cALIDADField\":\"\",\"cANJEField\":\"\",\"cANT_MAXField\":30000.0,\"cENTROField\":\"1127\",\"cESIONField\":\"\",\"cLASIFICACIONField\":\"ACOPIADOR\",\"cOMPENSACIONField\":\"\",\"cOND_FIJACIONField\":\"07\",\"cOND_PAGOField\":\"04\",\"cONTRATOField\":\"0002699201\",\"cOSECHAField\":\"19-20\",\"eNTREGA_DESDEField\":\"2021-05-18\",\"eNTREGA_HASTAField\":\"2021-06-18\",\"fECHA_DESDEField\":\"2021-05-18\",\"fECHA_HASTAField\":\"2021-06-18\",\"gRUPO_COMPRASField\":\"901\",\"iMPORTE_A_PRECIOField\":0.0,\"iMPORTE_S_PRECIOField\":0,\"kILOS_APLICADOSField\":73902.0,\"kILOS_CONTRATOField\":98000,\"kILOS_PEND_FIJARField\":92000.0,\"mONEDA_A_PRECIOField\":\"\",\"mONEDA_S_PRECIOField\":\"USDM\",\"pAGO_DIF_ARPField\":\"X\",\"pORC_A_PRECIOField\":0.0,\"pORC_S_PRECIOField\":0.0,\"pOSICIONField\":\"05.2021\",\"rECIBIDO_SIN_FIJARField\":67902.0,\"zONAField\":\"OIS\"}" +
                    "]}";
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
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
            try
            {
                var listaContratos = devolucion.EX_SALIDA.Where(x => x.CONTRATO.StartsWith("000" + filtro.TrimStart('0')));
                var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
                var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
                var monedas = repositorio.Listar<Moneda>();
                var campañas = repositorio.Listar<Campaña>();
                var condicionPagos = repositorio.Listar<CondicionPago>();
                var condicionFijaciones = repositorio.Listar<CondicionFijacion>();
                var centros = repositorio.Listar<Centro>();

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
                            Moneda = monedas.Where(x => x.MonedaId.Trim() == apertura.MONEDA.Trim()).FirstOrDefault().Descripcion
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

                    var contratoParaFijacion = new DatosFijacionDeContratoDto();

                    contratoParaFijacion.ContratoId = contrato.CONTRATO.TrimStart('0');
                    contratoParaFijacion.KilosAplicados = ((double)contrato.KILOS_APLICADOS).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.KilosPendiente = ((double)contrato.KILOS_PEND_FIJAR - (cantidad /*+ cantidadFijacion*/)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.FechaDesde = DateTime.Parse(contrato.FECHA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.FechaHasta = DateTime.Parse(contrato.FECHA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.KilosContrato = contrato.KILOS_CONTRATO.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.DesdeEntrega = DateTime.Parse(contrato.ENTREGA_DESDE).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.HastaEntrega = DateTime.Parse(contrato.ENTREGA_HASTA).ToString("dd-MM-yyyy", CultureInfo.CreateSpecificCulture("es-AR"));
                    contratoParaFijacion.Calidad = (((materialId == 3 || materialId == 4 || materialId == 5) && contrato.CALIDAD == "X") || ((materialId == 1 || materialId == 2) && contrato.CALIDAD != "X")) ? true : false;
                    contratoParaFijacion.Campana = contrato.COSECHA;
                    contratoParaFijacion.CampanaId = campañas.Where(x => x.Descripcion == contrato.COSECHA).FirstOrDefault() != null ?
                        campañas.Where(x => x.Descripcion == contrato.COSECHA).FirstOrDefault().CampañaId : 0;
                    contratoParaFijacion.Posicion = contrato.POSICION;
                    contratoParaFijacion.PagoDiferido = contrato.PAGO_DIF_ARP == "X" ? true : false;
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
                    contratoParaFijacion.Cesion = contrato.CESION == "X" ? true : false;
                    contratoParaFijacion.Anticipo = contrato.ANTICIPO == "X" ? true : false;
                    contratoParaFijacion.Aperturas = aperturas;
                    contratoParaFijacion.Bonificaciones = bonificaciones;
                    contratoParaFijacion.Virtual = false;

                    var existeConAnulaYReemplaza = false;
                    var contDA = contratos.Where(x => x.ContratoSAP == contrato.CONTRATO).FirstOrDefault();
                    logger.Debug("contrato.CONTRATO " + contrato.CONTRATO);

                    if (contDA != null)
                    {
                        contratoParaFijacion.Pase = contDA.TipoPosicionCBOTId == 3;
                        var idContratoConAnulaYReemplaza = contDA.Id;
                        existeConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == contDA.Id);
                        contratoParaFijacion.ProveedorComisionistaId = contDA.ProveedorComisionistaId ?? null;
                    }


                    if (!existeConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                    {
                        datosContratos.Add(contratoParaFijacion);
                    }




                }

            }
            catch (Exception e)
            {

                throw;
            }
            return datosContratos.OrderBy(x => x.ContratoId).ToList();
        }
    }
}
