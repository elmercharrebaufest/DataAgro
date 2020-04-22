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
    public class BusquedaContactosComercial : IConsultaEscalar<DataSourceResult>

    {
        private readonly DataSourceRequest request;
        private readonly List<int> comerciales;

        public BusquedaContactosComercial(DataSourceRequest request,List<int> comerciales)
        {
            this.request = request;
            this.comerciales = comerciales;
        }

        private static DataSourceResult Query(DbContext contexto, DataSourceRequest request, List<int> comerciales)
        {
            var resultado = from cc in contexto.Set<ContactoComercial>()
                            join pCom in contexto.Set<ProveedorComercial>() on cc.ProveedorId equals pCom.ProveedorId                             
                            join proveedor in contexto.Set<Proveedor>() on cc.ProveedorId equals proveedor.ProveedorId into provi
                            from provis in provi.DefaultIfEmpty()
                            join ContactoComercialInteres in contexto.Set<ContactoComercialInteres>() on cc.ContactoComercialId equals ContactoComercialInteres.ContactoComercialId into ComInteres
                            from intereses in ComInteres.DefaultIfEmpty()
                            where comerciales.Contains(pCom.ComercialId)
                            orderby pCom.ProveedorId
                            select new DatosContactoProveedor()
                            {
                                Apellido = cc.Apellido,
                                Cuit = provis.CUIT,
                                Email = cc.Email1,
                                FechaNacimiento = cc.FechaNacimiento.HasValue ? cc.FechaNacimiento : null,
                                Interes = intereses.Interes.Descripcion,
                                Nombre = cc.Nombres,
                                OtrosIntereses = cc.OtrosIntereses,
                                Principal = cc.EsPrincipal.HasValue && cc.EsPrincipal.Value ? "SI" : "NO",
                                Profesion = cc.Cargo,
                                Puesto = cc.Puesto,
                                RazonSocial = provis.RazonSocial,
                                Telefono = cc.Telefono1,
                                PrincipalCupo = cc.Cupo.HasValue && cc.Cupo.Value ? "SI" : "NO",
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
