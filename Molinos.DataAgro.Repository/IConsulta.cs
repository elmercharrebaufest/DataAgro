using System.Collections.Generic;
using System.Data.Entity;

namespace Molinos.DataAgro.Repository
{
    public interface IConsulta<TEntidad>
    {
       List<TEntidad> Ejecutar(DbContext contexto);
    }
}
