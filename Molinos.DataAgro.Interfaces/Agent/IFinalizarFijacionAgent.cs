using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFinalizarFijacionAgent
    {
        string Finalizar(FijacionDePrecioContrato fijacion);
    }
}