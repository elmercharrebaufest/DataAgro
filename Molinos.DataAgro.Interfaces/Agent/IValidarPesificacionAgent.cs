using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarPesificacionAgent
    {
        string ValidarPesificacion(FijacionDePrecioContrato fijacion);
    }
}