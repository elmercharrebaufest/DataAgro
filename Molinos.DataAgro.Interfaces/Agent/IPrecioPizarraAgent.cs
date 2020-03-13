using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IPrecioPizarraAgent
    {
        string Crear(PrecioPizarra precioPizarra);
        string Anular(PrecioPizarra precioPizarra);

    }
}