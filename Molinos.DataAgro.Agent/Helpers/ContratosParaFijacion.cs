using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Configuration;
using System.Linq;

namespace Molinos.DataAgro.Agent
{
    public class ContratosParaFijacion
    {

        String UserSap = ConfigurationManager.AppSettings["SapUser"];
        String PassSap = ConfigurationManager.AppSettings["SapPass"];
        
        public static List<DatosFijacionDeContratoDto> ObtenerContratos(string CuitProveedor, string CuitCorredor, int materialId, string filtro, IRepositorio repositorio)
        {
            var datosContratos = new List<DatosFijacionDeContratoDto>();

            if (ConfigurationManager.AppSettings["ValorPruebaSap"] == "1")
            {
            var contratos = repositorio.Listar<Contrato, int>(x => x.ContratoSAP.Value, x => x.TipoNegocioId == 1 && x.MaterialId== materialId && x.Proveedor.CUIT == CuitProveedor && (CuitCorredor != "" ? x.Corredor.CUIT == CuitCorredor : x.Corredor.CUIT == null));
                
                foreach (var id in contratos)
                {
                    var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.ContratoSAP == id).Sum();
                    datosContratos.Add(repositorio.Obtener<Contrato, DatosFijacionDeContratoDto>(x => x.ContratoSAP == id && x.ContratoSAP.ToString().Contains(filtro), x => new DatosFijacionDeContratoDto()
                    {
                        ContratoId = id,
                        KilosAplicados = cantidad,
                        KilosPendiente = x.Cantidad - cantidad,
                        FechaDesde = x.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                        FechaHasta = x.HastaFijacion.HasValue ? SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) : "",
                        KilosContrato = x.Cantidad,
                        Filtro = filtro + "|" + id
                    }));
                }
            }

            return datosContratos;
        }

    }
}
