using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerBusquedaContactosProveedorId : IConsulta<Contactos>
    {
        private readonly oParamBusqueda oParam;
        private readonly List<int> comercialesId;

        public TraerBusquedaContactosProveedorId(oParamBusqueda oParam, List<int> comercialesId)
        {
            this.comercialesId = comercialesId;
            this.oParam = oParam;
        }

        private static List<Contactos> Query(DbContext contexto, oParamBusqueda oParam, List<int> comercialesId)
        {

            var proveedores = from proveedor in contexto.Set<Proveedor>()
                              join ProveedorComercial in contexto.Set<ProveedorComercial>() on proveedor.ProveedorId equals ProveedorComercial.ProveedorId
                              join comercial in contexto.Set<Comercial>() on ProveedorComercial.ComercialId equals comercial.ComercialId
                              where comercialesId.Contains(ProveedorComercial.ComercialId)
                              select new Contactos()
                              {
                                 ProveedorId = proveedor.ProveedorId
                              };

            return proveedores.Distinct().ToList();
        }

        public virtual List<Contactos> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, oParam, comercialesId);
            }
        }
    }
}
