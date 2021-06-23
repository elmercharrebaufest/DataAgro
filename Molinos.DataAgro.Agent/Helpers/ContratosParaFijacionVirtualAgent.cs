using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Agent.ContratosParaFijacionVirtual;
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
    public class ContratosParaFijacionVirtualAgent : IContratosParaFijacionVirtualAgent
    {
        public ContratosParaFijacionVirtualAgent(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public List<DatosFijacionDeContratoDto> ObtenerContratosCanje(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int idFijacion)
        {
            filtro = filtro == null ? "" : filtro;
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            var hoy = DateTime.Now.Date;
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var json = "{\"ARecibirSinPrecio\":\"98.000\",\"Anticipo\":false,\"Aperturas\":[{\"ConceptoAperturaPrecio\":\"Basis\",\"ConceptoAperturaPrecioId\":5,\"FijacionId\":null,\"Id\":0,\"Importe\":120.0,\"Moneda\":\"USD\",\"MonedaId\":\"USDM\",\"Porcentaje\":0.0,\"contratoId\":null}],\"Calidad\":false,\"Calidades\":[],\"Campana\":\"19-20\",\"CampanaId\":8,\"Centro\":1,\"CentroDescripcion\":\"S. Lorenzo\",\"Cesion\":false,\"ChequeElectronico\":null,\"Clasificacion\":\"OTROS\",\"Color\":\"#26337b\",\"CondicionFijacionCod\":\"07\",\"CondicionFijacionDescripcion\":\"MERCADO MOA\",\"CondicionPagoCod\":\"04\",\"CondicionPagoDescripcion\":\"4 DÍAS HÁBILES DE FECHA DE FIJACIÓN\",\"ContratoId\":\"2699076\",\"DesdeEntrega\":\"23-04-2021\",\"FechaDesde\":\"23-04-2021\",\"FechaHasta\":\"23-05-2021\",\"FijacionSap\":null,\"Filtro\":\"2699076|2699076\",\"HastaEntrega\":\"23-05-2021\",\"ImporteAPrecio\":0.0,\"ImporteSobrePrecio\":12.0,\"KilosAplicados\":\"0\",\"KilosContrato\":\"98.000\",\"Virtual\":\"true\",\"KilosPendiente\":\"98.000\",\"MonedaAPrecio\":\"\",\"MonedaSobrePrecio\":\"USDM\",\"PagoDiferido\":false,\"PorcentajeAPrecio\":0.0,\"PorcentajeSobrePrecio\":0.0,\"Posicion\":\"04.2021\",\"RecibidoSinFijar\":\"0\"}";
                DatosFijacionDeContratoDto contrato = json.FromJson<DatosFijacionDeContratoDto>();
                datosContratos.Add(contrato);

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
                //        contrato.Clasificacion = "PRODUCTOR";
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

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_CONTRATO_CANJE_GENE(rq);
                    logger.Debug("Numero de contratos pendientes:" + devolucion.EX_SALIDA.Count());
                    var listaContratos = devolucion.EX_SALIDA.Where(x => x.CONTRNUM.StartsWith("000" + filtro.TrimStart('0')));
                    var calidadesEspeciales = repositorio.Listar<CalidadEspecial>();
                    var conceptoAperturas = repositorio.Listar<ConceptoAperturaPrecio>();
                    var monedas = repositorio.Listar<Moneda>();
                    foreach (var contrato in listaContratos)
                    {                     
                         var cantidad = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 3 && x.Virtual == true && x.ContratoSAP == contrato.CONTRNUM && x.Id != idFijacion
                         && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum(x => x.Cantidad + (x.Ampliaciones ?? 0));

                        var centro = repositorio.Obtener<Centro>(x => x.CodigoSap == contrato.CENTRO);
                        //var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<Negocio, double>(x => x.TipoNegocioId == 3 && x.Id == idFijacion && x.ContratoSAP == contrato.CONTRATO, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;
                        //var calidades = new List<CalidadDto>();                       
                        var contratoParaFijacion = new DatosFijacionDeContratoDto
                        {
                            ContratoId = contrato.CONTRNUM.TrimStart('0'),                            
                            KilosAplicados = ((double)contrato.KILOS_FIJADOS).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
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

                        if (double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                        {
                            datosContratos.Add(contratoParaFijacion);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
            return datosContratos.OrderBy(x => x.ContratoId).ToList();
        }
    }
}
