using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IPrecioPizarraAgent
    {
        string Crear(PrecioPizarra precioPizarra);
        string Anular(PrecioPizarra precioPizarra);

    }
}