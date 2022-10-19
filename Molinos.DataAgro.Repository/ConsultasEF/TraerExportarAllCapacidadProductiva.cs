using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerExportarAllCapacidadProductiva : IConsulta<CapacidadProductivaAll>
    {
        private readonly List<string> proveedores;

        public TraerExportarAllCapacidadProductiva(List<string> proveedores)
        {
            this.proveedores = proveedores;
        }

        private static List<CapacidadProductivaAll> Query(DbContext contexto, List<string> proveedores)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado = contexto.Set<CapacidadProductiva>().Where(x => proveedores.Contains(x.Proveedor.CUIT)).Select(x =>
                       new CapacidadProductivaAll
                       {
                           Cuit = x.Proveedor.CUIT,
                           RazonSocial = x.Proveedor.RazonSocial,
                           Material = x.Material.Descripcion,
                           Campania = x.Campania.Descripcion,
                           Cantidad = x.Cantidad,
                           UnidadMedida = x.UnidadMedida,
                           Porcentaje = x.Porcentaje
                       });
            return resultado.Distinct().ToList();
        }

        public virtual List<CapacidadProductivaAll> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedores);
            }
        }
    }
}
