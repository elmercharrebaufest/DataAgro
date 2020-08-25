using System.Collections.Generic;
using Molinos.DataAgro.Entities.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IModificarFijacionAgent
    {
        string Modificar(FijacionDePrecioContrato contrato, FijacionDePrecioContrato oContratoSave);
    }
}