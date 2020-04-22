using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class BusquedaDatosProduccion : IConsultaEscalar<DataSourceResult>

    {
        private readonly DataSourceRequest request;
        private readonly List<int> comerciales;

        public BusquedaDatosProduccion(DataSourceRequest request,List<int> comerciales)
        {
            this.request = request;
            this.comerciales = comerciales;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> comerciales)
        {
            var resultado =
                 from pCom in contexto.Set<ProveedorComercial>()
                 join c in contexto.Set<Campo>() on pCom.ProveedorId equals c.ProveedorId
                 join cm in contexto.Set<CampoMaterial>() on c.CampoId equals cm.CampoId
                 where comerciales.Contains(pCom.ComercialId)

                 orderby pCom.ProveedorId
                select new DatosProduccionProveedor()
                {
                    Cuit = pCom.Proveedor.CUIT,                   
                    RazonSocial = pCom.Proveedor.RazonSocial,
                    Localidad = c.Localidad.Nombre,
                    Provincia = c.Localidad.Provincia.Nombre,
                    Alquiladas = c.ArrendaPropia == 0 ? "SI":"",
                    Propias = c.ArrendaPropia ==1 ? "SI":"No",
                    Campaña = cm.Campaña.Descripcion,
                    Hectareas= cm.Hectareas,
                    Toneladas=cm.Toneladas,
                    Material = cm.Material.Descripcion
                };

            GridHelper.TruncateTime(request.Filter, ref resultado);
            return resultado.ToDataSourceResult(request);
        }

        public virtual DataSourceResult Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, request, comerciales);
            }
        }
    }
}
