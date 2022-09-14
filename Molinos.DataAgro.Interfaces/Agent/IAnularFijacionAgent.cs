using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAnularFijacionAgent
    {
        string AnularFijacion(FijacionDePrecioContrato fijacion);
    }
}