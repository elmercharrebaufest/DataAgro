using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerInformeComercialAlmacenamiento : IConsulta<InformeComercialProduccion>
    {
        private readonly int proveedorId;
        private readonly int campaniaId;
        private readonly InformeComercial informeComercial;

        public TraerInformeComercialAlmacenamiento(int proveedorId, int campaniaId, InformeComercial informeComercial)
        {
            this.proveedorId = proveedorId;
            this.campaniaId = campaniaId;
            this.informeComercial = informeComercial;
        }

        private static List<InformeComercialProduccion> Query(DbContext contexto, int proveedorId, int campaniaId, InformeComercial informeComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            return contexto.Set<AcopioCampaña>()
                       .Where(x => x.Acopio.Proveedor.ProveedorId == proveedorId && x.Campaña.CampañaId == campaniaId)
                       .GroupBy(z => new { z.Acopio.Localidad, z.HasArrendadas })
                       .Select(v => new InformeComercialProduccion()
                       {
                           InformeComercial = informeComercial,
                           Localidad = v.Key.Localidad,
                           Toneladas = (decimal)v.Sum(b => b.Toneladas),
                           Propio = v.Key.HasArrendadas,
                           Alquilado = !v.Key.HasArrendadas
                       }
                       ).ToList();
        }

        public virtual List<InformeComercialProduccion> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedorId, campaniaId, informeComercial);
            }
        }
    }
}
