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
    public class BusquedaDatosAlmacenamiento : IConsultaEscalar<DataSourceResult>

    {
        private readonly DataSourceRequest request;
        private readonly List<int> comerciales;

        public BusquedaDatosAlmacenamiento(DataSourceRequest request,List<int> comerciales)
        {
            this.request = request;
            this.comerciales = comerciales;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> comerciales)
        {
            var resultado =
                 from pCom in contexto.Set<ProveedorComercial>()
                 join a in contexto.Set<Acopio>() on pCom.ProveedorId equals a.ProveedorId
                 join am in contexto.Set<AcopioMaterial>() on a.AcopioId equals am.AcopioId
                 join ac in contexto.Set<AcopioCampaña>() on a.AcopioId equals ac.AcopioId
                 where comerciales.Contains(pCom.ComercialId)

                 orderby pCom.ProveedorId
                select new DatosAlmacenamientoProveedor()
                {
                    Cuit = pCom.Proveedor.CUIT,                   
                    RazonSocial = pCom.Proveedor.RazonSocial,
                    Alquiladas = ac.HasArrendadas == true ? "X":"",
                    Campaña = am.Campaña.Descripcion,
                    CampañaPlanta = ac.Campaña.Descripcion,
                    CapacidadPlantaTn = ac.Toneladas,
                    HabilitadoSojaSustentable = pCom.Proveedor.AlmacHabilitadoSojaSust == true ? "SI" : "",
                    Localidad = a.Localidad.Nombre,
                    Material = am.Material.Descripcion,
                    Propias = ac.HasArrendadas == false? "X":"",
                    Provincia = a.Localidad.Provincia.Nombre,
                    Toneladas= am.Toneladas,
                    VolumenAnualTn = pCom.Proveedor.AlmacVolAnualTotal
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
