using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Transactions;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class TraerInformeComercialProduccion : IConsulta<InformeComercialProduccionDto>
    {
        private readonly int proveedorId;
        private readonly int campaniaId;
        private readonly int materialId;
        private readonly InformeComercial informeComercial;

        public TraerInformeComercialProduccion(int proveedorId, int campaniaId, int materialId, InformeComercial informeComercial)
        {
            this.proveedorId = proveedorId;
            this.campaniaId = campaniaId;
            this.materialId = materialId;
            this.informeComercial = informeComercial;
        }

        private static List<InformeComercialProduccionDto> Query(DbContext contexto, int proveedorId, int campaniaId, int materialId, InformeComercial informeComercial)
        {
            ((System.Data.Entity.Infrastructure.IObjectContextAdapter)contexto).ObjectContext.CommandTimeout = 180;

            var listaInformeComercial = contexto.Set<CampoMaterial>()
                       .Where(x => x.Campo.Proveedor.ProveedorId == proveedorId && x.Campaña.CampañaId == campaniaId && x.Material.MaterialId == materialId)
                       .GroupBy(z => new { z.Campo.ArrendaPropia, z.Campo.Localidad, z.Material })
                       .Select(v => new InformeComercialProduccionDto()
                       {                           
                           Localidad = v.Key.Localidad,
                           Material = v.Key.Material,
                           Propio = (v.Key.ArrendaPropia == 1 ? true : false),
                           Alquilado = (v.Key.ArrendaPropia == 1 ? false : true),
                           Hectareas = v.Sum(q => q.Hectareas),
                           Toneladas = v.Sum(b => b.Toneladas)
                       }
                       ).ToList();
            foreach(var lista in listaInformeComercial)
            {
                lista.InformeComercial = informeComercial;
            }
            return listaInformeComercial;
        }

        public virtual List<InformeComercialProduccionDto> Ejecutar(DbContext contexto)
        {
            using (new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                return Query(contexto, proveedorId, campaniaId, materialId, informeComercial);
            }
        }
    }
}