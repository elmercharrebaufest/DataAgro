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

namespace Molinos.DataAgro.Agent
{
    public class ContratosParaFijacionAgent
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

        public List<DatosFijacionDeContratoDto> ObtenerContratos(string CuitProveedor, string CuitCorredor, int materialId, string filtro)
        {
            var datosContratos = new List<DatosFijacionDeContratoDto>();

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
                var contratos = repositorio.Listar<Contrato, int>(x => x.ContratoSAP.Value, x => x.TipoNegocioId == 1 && x.MaterialId == materialId && x.Proveedor.CUIT == CuitProveedor && (CuitCorredor != "" ? x.Corredor.CUIT == CuitCorredor : x.Corredor.CUIT == null));

                foreach (var id in contratos)
                {
                    var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.ContratoSAP == id).Sum();
                    datosContratos.Add(repositorio.Obtener<Contrato, DatosFijacionDeContratoDto>(x => x.ContratoSAP == id && x.ContratoSAP.ToString().Contains(filtro), x => new DatosFijacionDeContratoDto()
                    {
                        ContratoId = id.ToString(),
                        KilosAplicados = cantidad,
                        KilosPendiente = x.Cantidad - cantidad,
                        FechaDesde = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                        FechaHasta = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) : "",
                        KilosContrato = x.Cantidad,
                        Filtro = filtro + "|" + id
                    }));
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
                    foreach(var contrato in devolucion.EX_SALIDA)
                    {
                        datosContratos.Add(new DatosFijacionDeContratoDto
                        {
                            ContratoId = contrato.CONTRATO,
                            KilosAplicados = (double)contrato.KILOS_APLICADOS,
                            KilosPendiente = (double)contrato.KILOS_PEND_FIJAR,
                            FechaDesde = contrato.FECHA_DESDE,
                            FechaHasta = contrato.FECHA_HASTA,
                            KilosContrato = (double)contrato.KILOS_CONTRATO,
                            Filtro = filtro + "|" + contrato.CONTRATO
                        });
                    }                    
                }
                catch(Exception e)
                {
                    logger.Error("Error comunicacion SAP", e);
                    throw e;
                }
            }
            return datosContratos;
        }
    }
}
