using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using Molinos.DataAgro.Entities.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Transactions;
using Molinos.DataAgro.Entities.Seguridad;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerServicios : IConsultaEscalar<IList<ServicioValorDto>>
    {
        private readonly int? materialId;
        private readonly int? centroId;

        public TraerServicios(int? materialId, int? centroId)
        {
            this.materialId = materialId;
            this.centroId = centroId;
        }
        private static IList<ServicioValorDto> Query(DbContext contexto, int? materialId, int? centroId)
        {

            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;
            var queryRango =
                from item in contexto.Set<TipoServicio>()
                join np in contexto.Set<ServicioValor>() on item.Id equals np.TipoServicio.Id into nps
                from np in nps.DefaultIfEmpty()
                where (materialId == null || (materialId != null && materialId.Value == np.MaterialId)) &&
                 (centroId == null || (centroId != null && centroId.Value == np.CentroId))
                select new ServicioValorDto
                {
                    ServicioValorId = np.Id,
                    Descripcion = item.Descripcion,
                    CodigoSAP = item.CodigoSAP,
                    Importe = np.Importe,
                    MonedaDescripcion = np.Moneda.Descripcion,
                    MaterialDescripcion = np.Material.Descripcion,
                    MaterialId = np.MaterialId,
                    MonedaId = np.MonedaId,
                    Desde = np.Desde,
                    Hasta = np.Hasta,
                    CentroId = np.CentroId,
                    Centro = np.Centro.Descripcion,
                };
           
            return queryRango.ToList();
        }
        public virtual IList<ServicioValorDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, materialId, centroId);
            }
        }
    }
}
