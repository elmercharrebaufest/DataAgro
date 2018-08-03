using System.Data.Entity;

namespace Molinos.DataAgro.Repository
{
    public interface IConsultaEscalar<TEntidad>
    {
       TEntidad Ejecutar(DbContext contexto);
    }
}
