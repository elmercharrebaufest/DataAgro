using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class BusquedaContactosComercialReporte : IConsultaEscalar<DataSourceResult>

    {
        private readonly DataSourceRequest request;
        private readonly int? proveedorId;

        public BusquedaContactosComercialReporte(DataSourceRequest request, int? proveedorId)
        {
            this.request = request;
            this.proveedorId = proveedorId;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, int? proveedorId)
        {
            var resultado =
             from prov in contexto.Set<Proveedor>()
             join proCom in contexto.Set<ProveedorComercial>() on prov.ProveedorId equals proCom.ProveedorId
             join com in contexto.Set<Comercial>() on proCom.ComercialId equals com.ComercialId
             join estHom in contexto.Set<EstadoHome>() on prov.EstadoHome.Id equals estHom.Id
             join est in contexto.Set<Estado>() on prov.Estado.EstadoId equals est.EstadoId
             where !proveedorId.HasValue || prov.ProveedorId == proveedorId.Value

             orderby proCom.ProveedorId
             select new DatosContactoProveedor()
             {
                 Apellido = proCom.Comercial.Apellido,
                 Cuit = prov.CUIT,
                 Email = proCom.Comercial.Email,
                 Nombre = proCom.Comercial.Nombres,
                 RazonSocial = prov.RazonSocial,
                 Estado = prov.Estado.Descripcion,
                 EstadoHome = prov.EstadoHome.Descripcion,
                 ComercialAsignado = string.Concat(proCom.Comercial.Apellido, " ", proCom.Comercial.Nombres)
             };
            GridHelper.TruncateTime(request.Filter, ref resultado);

            return resultado.ToDataSourceResult(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, proveedorId);
            }
        }
    }
}
