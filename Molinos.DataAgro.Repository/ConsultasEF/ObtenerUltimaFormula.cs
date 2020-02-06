using Molinos.DataAgro.Entities.Entities;
using System;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Repository.ConsultasEF
{
    public class ObtenerUltimaFormula : IConsultaEscalar<Formula>
    {
       

        public ObtenerUltimaFormula()
        {
            
        }

        //Se hizo esta query para materializar toda la estructura y poder hacer una copia de la misma
        public Formula Ejecutar(DbContext contexto)
        {
           return contexto.Set<Formula>()
                            .AsNoTracking()
                            .Include("Criterio.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos.Hijos")
                            .OrderByDescending(a=>a.Id)
                            .Take(1)
                            .SingleOrDefault();
        }

        
    }
}
