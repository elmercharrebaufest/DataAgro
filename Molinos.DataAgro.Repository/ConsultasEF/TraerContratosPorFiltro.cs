using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerContratosPorFiltro : IConsultaEscalar<KendoGridContratoDto>
    {
        private readonly FiltroReporteNegocioDto request;
        private readonly List<int> equipo;
        private readonly bool corredor;
        private readonly List<int> corredoresComercial;

        public TraerContratosPorFiltro(FiltroReporteNegocioDto request, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            this.request = request;
            this.equipo = equipo;
            this.corredor = corredor;
            this.corredoresComercial = corredoresComercial;
        }

        private static KendoGridContratoDto Query(DbContext contexto, FiltroReporteNegocioDto request, bool corredor, List<int> equipo, List<int> corredoresComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var fechaEntregaDesde = (!string.IsNullOrEmpty(request.FechaEntregaDesde) ? (DateTime?)DateTime.ParseExact(request.FechaEntregaDesde, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null);
            var FechaEntregaHasta = (!string.IsNullOrEmpty(request.FechaEntregaHasta) ? (DateTime?)DateTime.ParseExact(request.FechaEntregaHasta, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null);
            var fechaCarga = (!string.IsNullOrEmpty(request.FechaCarga) ? (DateTime?)DateTime.ParseExact(request.FechaCarga, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null);
            var fechaCargaHasta = (!string.IsNullOrEmpty(request.FechaCargaHasta) ? (DateTime?)DateTime.ParseExact(request.FechaCargaHasta, "dd-MM-yyyy", CultureInfo.InvariantCulture) : fechaCarga);
            var FechaHastaFijacion = (!string.IsNullOrEmpty(request.FechaHastaFijacion) ? (DateTime?)DateTime.ParseExact(request.FechaHastaFijacion, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null);
            var FechaLimiteDolarizado = (!string.IsNullOrEmpty(request.FechaLimiteDolarizado) ? (DateTime?)DateTime.ParseExact(request.FechaLimiteDolarizado, "dd-MM-yyyy", CultureInfo.InvariantCulture) : null);
            request.ProveedorId = request.ProveedorId ?? (new int[0]);
            request.CorredorId = request.CorredorId ?? (new int[0]);
            var listaContrato = new List<string>();
            if (!string.IsNullOrEmpty(request.ContratoSAP))
            {
                request.ContratoSAPHasta = string.IsNullOrEmpty(request.ContratoSAPHasta) ? request.ContratoSAP : request.ContratoSAPHasta;
                for (var i = int.Parse(request.ContratoSAP); i <= int.Parse(request.ContratoSAPHasta); i++)
                {
                    listaContrato.Add(i.ToString().PadLeft(10, '0'));
                }
            }
            var queryContratos = TraerTodosContratosSinFiltro.QueryBase(contexto, corredor, equipo, corredoresComercial);

            queryContratos = queryContratos.Where(contrato =>
                    (!string.IsNullOrEmpty(request.ContratoSAP) ? listaContrato.Contains(contrato.ContratoSAP) : true) &&
                    (request.ProveedorId.Any() ? request.ProveedorId.Contains(contrato.ProveedorId) : true) &&
                    (request.CorredorId.Any() ? request.CorredorId.Contains(contrato.CorredorId) : true) &&
                    (request.TipoNegocioId != 0 ? request.TipoNegocioId == contrato.TipoNegocioId : true) &&
                    (request.CampaniaId != 0 ? request.CampaniaId == contrato.CampanaId : true) &&
                    (request.MaterialId != 0 ? request.MaterialId == contrato.MaterialId : true) &&
                    (request.GrupoDeCompraId != 0 ? request.GrupoDeCompraId == contrato.GrupoCompra : true) &&
                    (request.EstadoId != 0 ? request.EstadoId == contrato.Estado : true) &&
                    (request.CentroId != 0 ? request.CentroId == contrato.DestinoId : true) &&
                    (request.BoletoCompraNetId != 0 ? request.BoletoCompraNetId == contrato.BoletoId : true) &&
                    (request.ImporteSustentable != null ? request.ImporteSustentable == contrato.Importe_Sustentable : true) &&
                    (request.DiasDiferimiento != 0 ? request.DiasDiferimiento == contrato.Dias_Pesificado : true) &&
                    (fechaCarga != null ? fechaCarga <= contrato.Fecha && fechaCargaHasta >= contrato.Fecha : true) &&
                    (fechaEntregaDesde != null && FechaEntregaHasta == null ? fechaEntregaDesde == contrato.FechaDesde : true) &&
                    (FechaEntregaHasta != null && fechaEntregaDesde == null ? FechaEntregaHasta == contrato.FechaHasta : true) &&
                    (fechaEntregaDesde != null && FechaEntregaHasta != null ? (fechaEntregaDesde <= contrato.FechaDesde && FechaEntregaHasta >= contrato.FechaHasta) : true) &&
                    (FechaHastaFijacion != null ? FechaHastaFijacion == contrato.HastaFijacion : true) &&
                    (FechaLimiteDolarizado != null ? FechaLimiteDolarizado == contrato.Fecha_Dolarizado : true) &&
                    (request.Importe ? (contrato.Sustentable.HasValue && contrato.Sustentable.Value ) : true) &&
                    (request.Diferimiento ? (contrato.Pesificado.HasValue && contrato.Pesificado.Value ) : true) &&
                    (request.Dolarizado ? (contrato.Dolarizado.HasValue && contrato.Dolarizado.Value ) : true)
            );

            var itemsTotales = queryContratos.Count();
            if (request.Sort != null)
            {
                var primeraColumna = true;
                foreach(var orden in request.Sort)
                {
                    var selectorOrden = Expresiones.Propiedad<BasicoContrato>(orden.field);
                    if (primeraColumna)
                    {
                        queryContratos = orden.dir == "asc"
                                 ? queryContratos.OrderBy(selectorOrden)
                                 : queryContratos.OrderByDescending(selectorOrden);
                        primeraColumna = false;
                    }
                    else
                    {
                        queryContratos = orden.dir == "asc"
                                 ? ((IOrderedQueryable<BasicoContrato>)queryContratos).ThenBy(selectorOrden)
                                 : ((IOrderedQueryable<BasicoContrato>)queryContratos).ThenBy(selectorOrden);
                    }
                    
                }
            }
            else
            {
                queryContratos= queryContratos.OrderBy(x => x.Fecha);
            }
            var resultadoPagina = queryContratos.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize);

            return new KendoGridContratoDto
            {
                Data = resultadoPagina.ToList(),
                Total = itemsTotales
            };
        }
        
        public virtual KendoGridContratoDto Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, corredor, equipo, corredoresComercial);
            }
        }
    }
}
