using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerExportarAllEstablecimientos : IConsulta<CampoDetalleAll>
    {
        private readonly List<string> proveedores;

        public TraerExportarAllEstablecimientos(List<string> proveedores)
        {
            this.proveedores = proveedores;
        }

        private static List<CampoDetalleAll> Query(DbContext contexto, List<string> proveedores)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado = contexto.Set<CampoDetalle>().Where(x => proveedores.Contains(x.Proveedor.CUIT)).Select(x =>
                       new CampoDetalleAll
                       {
                           Cuit = x.Proveedor.CUIT,
                           RazonSocial = x.Proveedor.RazonSocial,
                           Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
                           HectareasCultivables = x.HectareasCultivables,
                           HectareasTotales = x.HectareasTotales,
                           ImportId = x.ImportId,
                           Latitud = x.Latitud,
                           Longitud = x.Longitud,
                           Material = x.Material.Descripcion,
                           Nombre = x.Nombre,
                           Localidad = x.Localidad.Nombre,
                           Provincia = x.Localidad.Provincia.Nombre,
                           Partido = x.Localidad.Partido.Descripcion,
                           Rinde = x.Rinde,
                           Campaña = x.Campaña.Descripcion
                       });
            return resultado.Distinct().ToList();
        }

        public virtual List<CampoDetalleAll> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedores);
            }
        }
    }
}
