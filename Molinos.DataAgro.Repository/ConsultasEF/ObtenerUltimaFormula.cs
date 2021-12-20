using Molinos.DataAgro.Entities.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ObtenerUltimaFormula : IConsultaEscalar<Formula>
    {

        private readonly int materialId;

        public ObtenerUltimaFormula(int materialId)
        {
            this.materialId = materialId;
        }

        //Se hizo esta query para materializar toda la estructura y poder hacer una copia de la misma
        public Formula Ejecutar(DbContext contexto)
        {
            return contexto.Set<Formula>()
                             .AsNoTracking()
                             .Where(a => a.MaterialId == materialId)
                             .Include("Criterio.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos")
                             .OrderByDescending(a => a.Id)
                             .Take(1)
                             .SingleOrDefault();
        }


    }
}
