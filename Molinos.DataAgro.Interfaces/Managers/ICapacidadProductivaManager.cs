using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICapacidadProductivaManager
    {
        List<CapacidadProductivaDto> ObtenerCapacidadProductiva(int proveedorId);
        void ActualizarCapacidadProductiva();
    }
}
