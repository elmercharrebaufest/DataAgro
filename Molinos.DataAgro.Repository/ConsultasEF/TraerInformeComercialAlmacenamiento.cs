using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerInformeComercialAlmacenamiento : IConsulta<InformeComercialAlmacenamientoDto>
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

        private static List<InformeComercialAlmacenamientoDto> Query(DbContext contexto, int proveedorId, int campaniaId, InformeComercial informeComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var listaInformeAlmacenamiento = contexto.Set<AcopioCampaña>()
                       .Where(x => x.Acopio.Proveedor.ProveedorId == proveedorId && x.Campaña.CampañaId == campaniaId)
                       .GroupBy(z => new { z.Acopio.Localidad, z.HasArrendadas })
                       .Select(v => new InformeComercialAlmacenamientoDto()
                       {
                           Localidad = v.Key.Localidad,
                           Toneladas = v.Sum(b => b.Toneladas),
                           Propia = v.Key.HasArrendadas,
                           Alquilada = !v.Key.HasArrendadas
                       }
                       ).ToList();
            foreach(var almacenamiento in listaInformeAlmacenamiento)
            {
                almacenamiento.InformeComercial = informeComercial;
            }
            return listaInformeAlmacenamiento;
        }

        public virtual List<InformeComercialAlmacenamientoDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedorId, campaniaId, informeComercial);
            }
        }
    }
}
