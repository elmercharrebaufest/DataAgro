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
            var datosContratos = new List<DatosFijacionDeContratoDto>();
            var hoy = DateTime.Now.Date;
            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var contratos = repositorio.Listar<Contrato, string>(x => x.ContratoSAP, x => x.TipoNegocioId == 1 && x.MaterialId == materialId && x.Proveedor.CUIT == CuitProveedor && (!string.IsNullOrEmpty(CuitCorredor) ? x.Corredor.CUIT == CuitCorredor : x.CorredorId == null) && x.ContratoSAP != null);
                if (contratos != null)
                {
                    var listaContratos = contratos.Where(x => x.StartsWith("000" + filtro));

                    foreach (var id in listaContratos)
                    {
                        var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad + (x.Ampliaciones ?? 0), x => x.ContratoSAP == id
                        && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum();
                        var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<FijacionDePrecioContrato, double>(x => x.Id == idFijacion && x.ContratoSAP == id, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;
                        var contrato = repositorio.Obtener<Contrato, DatosFijacionDeContratoDto>(x => x.ContratoSAP == id && x.TipoNegocioId == 1, x => new DatosFijacionDeContratoDto()
                        {
                            ContratoId = id.ToString(),
                            KilosAplicados = cantidad.ToString(),
                            KilosPendiente = (x.Cantidad - cantidad + cantidadFijacion).ToString(),
                            FechaDesde = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                            FechaHasta = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                            KilosContrato = x.Cantidad.ToString(),
                            DesdeEntrega = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                            HastaEntrega = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                            Posicion = x.FechaDesde.Month.ToString() + "." + x.FechaDesde.Year.ToString(),
                            Calidad = x.TrigoEspecial,
                            Campana = x.Campana.Descripcion,
                            PagoDiferido = x.PagoDiferido ?? false,
                            Centro = x.DestinoId,
                            Color = x.HastaFijacion.HasValue && x.HastaFijacion.Value < hoy ? "Red" : "#26337b",
                            CentroDescripcion = x.Destino != null ? x.Destino.Descripcion : null,

                        });
                        contrato.KilosAplicados = (double.Parse(contrato.KilosAplicados)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                        contrato.KilosPendiente = (double.Parse(contrato.KilosPendiente)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                        contrato.KilosContrato = (double.Parse(contrato.KilosContrato)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                        contrato.ContratoId = contrato.ContratoId.TrimStart('0');
                        contrato.Filtro = filtro + "|" + contrato.ContratoId;
                        contrato.ARecibirSinPrecio = 10000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                        contrato.RecibidoSinFijar = 19000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                        contrato.ImporteAPrecio = 100;
                        contrato.ImporteSobrePrecio = -200;
                        contrato.MonedaAPrecio = "ARP  ";
                        contrato.MonedaSobrePrecio = "ARP  ";
                        contrato.PorcentajeAPrecio = 5;
                        contrato.PorcentajeSobrePrecio = 10;
                        contrato.CondicionFijacionCod = "07";
                        contrato.CondicionPagoCod = "10";
                        contrato.CondicionFijacionDescripcion = "HASTA 14.30 HS POR PIZ / MERCADERIA";
                        contrato.CondicionPagoDescripcion = "10 DÍAS HÁBILES DE FECHA DE FIJACIÓN";
                        if (double.Parse(contrato.KilosPendiente) > 0)
                        {
                            datosContratos.Add(contrato);
                        }
                    }
                }
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

                    var devolucion = agent.SI_ZMPWS_DATAAGRO_CONTRATO_PEND_FIJACION(rq);
                    logger.Debug("Numero de contratos pendientes:" + devolucion.EX_SALIDA.Count());
                    var listaContratos = devolucion.EX_SALIDA.Where(x => x.CONTRATO.StartsWith("000" + filtro));
                    foreach (var contrato in listaContratos)
                    {
                        var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad +(x.Ampliaciones ?? 0), x => x.ContratoSAP == contrato.CONTRATO
                        && (x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado)).Sum();
                        var centro = repositorio.Obtener<Centro>(x => x.CodigoSap == contrato.CENTRO);
                        var cantidadFijacion = idFijacion != 0 ? repositorio.Obtener<FijacionDePrecioContrato, double>(x => x.Id == idFijacion && x.ContratoSAP == contrato.CONTRATO, x => x.Cantidad + (x.Ampliaciones ?? 0)) : 0;

                        var contratoParaFijacion = new DatosFijacionDeContratoDto
                        {
                            ContratoId = contrato.CONTRATO.TrimStart('0'),
                            KilosAplicados = ((double)contrato.KILOS_APLICADOS).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                            KilosPendiente = ((double)contrato.KILOS_PEND_FIJAR - cantidad + cantidadFijacion).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                            FechaDesde = DateTime.Parse(contrato.FECHA_DESDE).ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                            FechaHasta = DateTime.Parse(contrato.FECHA_HASTA).ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                            KilosContrato = contrato.KILOS_CONTRATO.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                            DesdeEntrega = DateTime.Parse(contrato.ENTREGA_DESDE).ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                            HastaEntrega = DateTime.Parse(contrato.ENTREGA_HASTA).ToString("dd/MM/yyyy", CultureInfo.CreateSpecificCulture("es-AR")),
                            Calidad = (((materialId == 3 || materialId == 4 || materialId == 5) && contrato.CALIDAD == "X") || ((materialId == 1 || materialId == 2) && contrato.CALIDAD != "X")) ? true : false,
                            Campana = contrato.COSECHA,
                            Posicion = contrato.POSICION,
                            PagoDiferido = contrato.PAGO_DIF_ARP == "X" ? true : false,
                            Centro = centro.Id,
                            ARecibirSinPrecio = contrato.A_RECIBIR_SIN_PRECIO.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                            RecibidoSinFijar = contrato.RECIBIDO_SIN_FIJAR.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
                            CentroDescripcion = centro.Descripcion,
                            ImporteAPrecio = contrato.IMPORTE_A_PRECIO,
                            ImporteSobrePrecio = contrato.IMPORTE_S_PRECIO,
                            MonedaAPrecio = contrato.MONEDA_A_PRECIO,
                            MonedaSobrePrecio = contrato.MONEDA_S_PRECIO,
                            PorcentajeAPrecio = contrato.PORC_A_PRECIO,
                            PorcentajeSobrePrecio = contrato.PORC_S_PRECIO,
                            CondicionFijacionCod = contrato.COND_FIJACION,
                            CondicionPagoCod = contrato.COND_PAGO,
                            CondicionFijacionDescripcion = repositorio.Obtener<CondicionFijacion, string>(x => x.CodigoSap == contrato.COND_FIJACION, x => x.Descripcion),
                            CondicionPagoDescripcion = repositorio.Obtener<CondicionPago, string>(x => x.CodigoSap == contrato.COND_PAGO, x => x.Descripcion),
                            Filtro = filtro + "|" + contrato.CONTRATO.TrimStart('0'),
                            Color = DateTime.Parse(contrato.FECHA_HASTA) < hoy ? "Red" : "#26337b",
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
