using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerVentaStock : IConsultaEscalar<KendoGrid<ResearchVentaStockDto>>
    {
        private readonly KendoGridMvcRequest request;

        public TraerVentaStock(KendoGridMvcRequest request)
        {
            this.request = request;
        }

        private static KendoGrid<ResearchVentaStockDto> Query(DbContext contexto, KendoGridMvcRequest request)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var queryContratos =
                from ventaStock in contexto.Set<ResearchVentaStock>()
                select new ResearchVentaStockDto()
                {
                    Id = ventaStock.Id,
                    Comercial = ventaStock.Comercial.Apellido + " " + ventaStock.Comercial.Nombres,
                    FechaHora = ventaStock.FechaHora,
                    Localidad = ventaStock.Localidad.Nombre+" ("+ventaStock.Localidad.Provincia.Nombre+")",
                    LocalidadId = ventaStock.LocalidadId,
                    Partido=ventaStock.Localidad.Partido.Descripcion,
                    Material = ventaStock.Material.Descripcion,
                    MaterialId = ventaStock.MaterialId,
                    Observaciones = ventaStock.Observaciones,
                    Almacenado = ventaStock.Almacenado,
                    VendidoAPrecio = ventaStock.VendidoAPrecio
                };


            return new KendoGrid<ResearchVentaStockDto>(request, queryContratos);
        }

        public virtual KendoGrid<ResearchVentaStockDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request);
            }
        }
    }
}
