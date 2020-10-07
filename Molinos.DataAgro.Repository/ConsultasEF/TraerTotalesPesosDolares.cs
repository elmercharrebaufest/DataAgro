using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    
    public class TraerTotalesPesosDolares : IConsultaEscalar<TotalPesosDolares>
    {
        private readonly DataSourceRequest request;
        private readonly List<int> equipo;
        private readonly List<int> corredoresComercial;

        public TraerTotalesPesosDolares(DataSourceRequest request, List<int> equipo, List<int> corredoresComercial)
        {
            this.request = request;
            this.equipo = equipo;
            this.corredoresComercial = corredoresComercial;
        }

        private static TotalPesosDolares Query(DbContext contexto, DataSourceRequest request, List<int> equipo, List<int> corredoresComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var corredor = PermisosHelper.Is(PermisosDataAgro.VerCorredorComercial);
            var precioPizarraPorMaterial = contexto.Set<PrecioPizarra>().GroupBy(x => x.MaterialId).Select(x => new { MaterialId = x.Key, x.OrderByDescending(y => y.FechaHasta).FirstOrDefault().MonedaId, x.OrderByDescending(y => y.FechaHasta).FirstOrDefault().Precio });
            var queryContratos =
                from contrato in contexto.Set<Negocio>()
                where (contrato.TipoNegocioId == 2 || contrato.TipoNegocioId == 3 || contrato.TipoNegocioId == 4 || contrato.TipoNegocioId == 5 || contrato.TipoNegocioId == 6   )
                && contrato.OcultarEnTablero == false
                && (contrato.EstadoId == 2 || contrato.EstadoId == 4 || contrato.EstadoId == 5)
                && ((contrato is Contrato && (contrato as Contrato).ContratoAcuerdo == null) || !(contrato is Contrato))  
                //&& ((contrato is Contrato && DbFunctions.TruncateTime((contrato as Contrato).FechaOperacion) == DbFunctions.TruncateTime((contrato as Contrato).Fecha)) || !(contrato is Contrato))
                && ((contrato is ContratoAcuerdo && (contrato as ContratoAcuerdo).TipoAgenteCompraId == null) || !(contrato is ContratoAcuerdo))
                && ((contrato is Contrato && (contrato as Contrato).TipoAgenteCompraId == null) || !(contrato is Contrato))
                && ((contrato is ContratoAcuerdo && (contrato as ContratoAcuerdo).PrecioNeto > 0) || !(contrato is ContratoAcuerdo))
                 && (
                !corredor ? (equipo.Contains(contrato.ComercialId != null ? contrato.ComercialId.Value : 0) ||
                equipo.Contains(contrato.ComercialCreadorId != null ? contrato.ComercialCreadorId.Value : 0)) :
                    (corredor &&
                    (corredoresComercial.Contains(contrato.ComercialId != null ? contrato.ComercialId.Value : 0) ||
                    corredoresComercial.Contains(contrato.ComercialCreadorId != null ? contrato.ComercialCreadorId.Value : 0)))
                    )
                select new TotalPesosDolares()
                {
                    Id = contrato.Id,
                    Cantidad = Math.Round(contrato.Cantidad / 1000),
                    FechaDesde = DbFunctions.TruncateTime(contrato.FechaDesde),
                    FechaHasta = DbFunctions.TruncateTime(contrato.FechaHasta),
                    Fecha = (contrato is Contrato || contrato is FijacionDePrecioContrato) ? DbFunctions.TruncateTime((contrato as Contrato).FechaOperacion) : DbFunctions.TruncateTime(contrato.Fecha),                    
                    GrupoCompraDescripcion = contrato.GrupoDeCompras.Descripcion,
                    Estado_Contrato = contrato.Estado.Descripcion,
                    Ampliaciones = contrato.Ampliaciones,
                    Proveedor = (contrato is AgenteCompra) ? (contrato as AgenteCompra).Operador.Descripcion : contrato.Proveedor == null ? "" : contrato.Proveedor.RazonSocial,
                    Corredor = contrato.Corredor == null ? "" : contrato.Corredor.RazonSocial,
                    ProveedorId = (contrato is AgenteCompra) ? (contrato as AgenteCompra).OperadorId : contrato.ProveedorId,
                    CorredorId = contrato.CorredorId != null ? contrato.CorredorId.Value : 0,
                    ComercialCreadorId = contrato.ComercialCreadorId,
                    Comercial = contrato.Comercial == null ? "" : contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido,
                    Material = contrato.Material == null ? "" : contrato.Material.Descripcion,
                    Campania = contrato.Campana == null ? "" : contrato.Campana.Descripcion,
                    TipoNegocio = (contrato.TipoNegocio == null ? "" : (contrato is Contrato && (contrato as Contrato).Madre == true) ? "CONVENIO" : (contrato is Contrato && (contrato as Contrato).Madre == false) ? "FIJ. CONVENIO" : (contrato is Contrato && (contrato as Contrato).EsFason == true) ? "FASON MP" : (contrato is Contrato && (contrato as Contrato).TipoAgenteCompraId > 0) ? "AGENTE DE COMPRAS MP" : (contrato is ContratoAcuerdo && (contrato as ContratoAcuerdo).TipoAgenteCompraId > 0) ? "ACUERDO AGENTE" : contrato.TipoNegocio.Descripcion),
                    Negocio = (contrato is FijacionDePrecioContrato && contrato.EstadoId == (int)EnumEstadoContrato.Finalizado) ? (contrato as FijacionDePrecioContrato).FijacionSAP : contrato.ContratoSAP != "0" ? contrato.ContratoSAP : "",
                    DestinoDescripcion = contrato.Destino.Descripcion,
                    ComercialId = contrato.ComercialId,
                    ComercialCreador = contrato.ComercialCreador == null ? contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido : contrato.ComercialCreador.Nombres + " " + contrato.ComercialCreador.Apellido,
                    TotalDolares = contrato.Pizarra == true ? (precioPizarraPorMaterial.Any(y => y.MaterialId == contrato.MaterialId) && precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == contrato.MaterialId).MonedaId == "USDM " ? precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == contrato.MaterialId).Precio : 0) : (contrato.MonedaId == "USDM " ? contrato.PrecioNeto != null ? (double)contrato.PrecioNeto.Value : (double)contrato.Precio : 0),
                    TotalPesos = contrato.Pizarra == true ? (precioPizarraPorMaterial.Any(y => y.MaterialId == contrato.MaterialId) && precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == contrato.MaterialId).MonedaId == "ARP  " ? precioPizarraPorMaterial.FirstOrDefault(y => y.MaterialId == contrato.MaterialId).Precio : 0) : (contrato.MonedaId == "ARP  " ? contrato.PrecioNeto != null ? (double)contrato.PrecioNeto.Value : (double)contrato.Precio : 0),
                    TotalGirasolAlto = contrato.MaterialId == 5 ? contrato.Cantidad : 0,
                    TotalGirasol = contrato.MaterialId == 4 ? contrato.Cantidad : 0,
                    TotalMaiz = contrato.MaterialId == 1 ? contrato.Cantidad : 0,
                    TotalSoja = contrato.MaterialId == 3 ? contrato.Cantidad : 0,
                    TotalTrigo = contrato.MaterialId == 2 ? contrato.Cantidad : 0,
                    ContratoSAP = contrato.ContratoSAP,
                    ContratoCorredor = (contrato is Contrato) ? (contrato as Contrato).ContratoCorredor : "",
                };

            GridHelper.ProcessFilters(request.Filter, ref queryContratos);
            
            var result2 = from a in queryContratos
                          group a by 0 into g
                          select new
                          {
                              TotalDolares = g.Sum(x => Math.Round(x.TotalDolares * x.Cantidad)),
                              TotalPesos = g.Sum(x => Math.Round(x.TotalPesos * x.Cantidad)),
                              TotalSoja = g.Sum(x => Math.Round(x.TotalSoja / 1000)),
                              TotalMaiz = g.Sum(x => Math.Round(x.TotalMaiz / 1000)),
                              TotalTrigo = g.Sum(x => Math.Round(x.TotalTrigo / 1000)),
                              TotalGirasol = g.Sum(x => Math.Round(x.TotalGirasol / 1000)),
                              TotalGirasolAlto = g.Sum(x => Math.Round(x.TotalGirasolAlto / 1000)),
                          };
            var result3 = result2.SingleOrDefault();
            if (result3 == null)
            {
                result3 = new
                {
                    TotalDolares = (double)0,
                    TotalPesos = (double)0,
                    TotalSoja = (double)0,
                    TotalMaiz = (double)0,
                    TotalTrigo = (double)0,
                    TotalGirasol = (double)0,
                    TotalGirasolAlto = (double)0
                };
            }
            var result4 = new TotalPesosDolares
            {
                TotalDolares = result3.TotalDolares,
                TotalPesos = result3.TotalPesos,
                TotalSoja = result3.TotalSoja,
                TotalMaiz = result3.TotalMaiz,
                TotalTrigo = result3.TotalTrigo,
                TotalGirasol = result3.TotalGirasol,
                TotalGirasolAlto = result3.TotalGirasolAlto,
            };
            return result4;
        }

        public virtual TotalPesosDolares Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, equipo, corredoresComercial);
            }
        }
    }
}
