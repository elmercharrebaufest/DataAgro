using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICrearCupoAgent
    {
        List<string> Crear(Cupo cupo,int cantidadCupos);
    }
}