using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerInformesSinFiltro : IConsultaEscalar<List<InformeList>>
    {
        public TraerInformesSinFiltro()
        {
            
        }

        private static List<InformeList> Query(DbContext contexto )
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var resultado =
                from x in contexto.Set<InformeComercial>()
                select new InformeList
                {
                    InformeComercialId = x.InformeComercialId,
                    Cuit = x.Proveedor.CUIT,
                    RazonSocial = x.Proveedor.RazonSocial,
                    Campaña = x.Campaña.Descripcion,
                    MaterialesList = x.InformeComercialProduccion.Select(y => y.Material.Descripcion),
                    MaterialesIdList = x.InformeComercialProduccion.Select(k => k.MaterialId),
                    Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    Seleccionado = false,
                    ProveedorId = x.ProveedorId,
                    CampanaId = x.CampañaId,
                    ComercialId = x.ComercialId,
                    FechaAlta = x.FechaAlta,
                    FechaDescarga = x.FechaDescarga,
                    OrigenDA = x.OrigenDA == true ? "Si" : "No",
                };

            return resultado.ToList();
        }

        public virtual List<InformeList> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto);
            }
        }
    }
}
