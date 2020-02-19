using Molinos.DataAgro.Entities.Entities;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ObtenerToneladasPorMaterial : IConsultaEscalar<float>
    {
        private readonly int informeId;
        private readonly int materialId;

        public ObtenerToneladasPorMaterial(int informeId, int materialId)
        {
            this.informeId = informeId;
            this.materialId = materialId;
        }

        public float Ejecutar(DbContext contexto)
        {
            return contexto.Set<InformeComercialProduccion>()
                            .Where(x => x.InformeComercial.InformeComercialId == informeId && x.Material.MaterialId == materialId)
                            .GroupBy(z => z.Material.MaterialId)
                            .Select(x => (float)x.Sum(a => a.Toneladas))
                            .DefaultIfEmpty(0)
                            .First();
        }
    }
}
