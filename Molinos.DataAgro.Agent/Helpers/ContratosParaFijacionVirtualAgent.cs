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
                        IM_CORREDOR = !string.IsNullOrEmpty(CuitCorredor) ? "C" + CuitCorredor.Remove(CuitCorredor.Length - 1).Remove(0, 2) : "",
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
                            KilosAplicados = ((double)contrato.KILOS_FIJADOS + cantidad).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")),
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
                        var idContratoConAnulaYReemplaza = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contrato.CONTRNUM, x => x.Id);
                        var contratoConAnulaYReemplaza = repositorio.Existe<Contrato>(x => x.AnulaYReemplazaContratoId == idContratoConAnulaYReemplaza);

                        if (!contratoConAnulaYReemplaza && double.Parse(contratoParaFijacion.KilosPendiente) > 0)
                        {
                            datosContratos.Add(contratoParaFijacion);
                        }
                        var contratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contrato.CONTRNUM, x => x.Id);
                        var contratoAnulado = repositorio.Obtener<Contrato>(x => x.AnulaYReemplazaContratoId == contratoId && x.EstadoId == 5);
                        if (contratoAnulado != null)
                        {
                            listaContratos = listaContratos.Where(x => x.CONTRNUM != contratoAnulado.ContratoSAP);
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
