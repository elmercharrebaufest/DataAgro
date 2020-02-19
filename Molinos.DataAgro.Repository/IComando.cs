using System.Data.Entity;

namespace Molinos.DataAgro.Repository
{
    public interface IComando<TResultado>
    {
        TResultado Ejecutar(DbContext contexto);
    }
}
