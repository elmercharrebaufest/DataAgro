using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IVisualizarCapacidadProductivaAgent
    {
        List<CapacidadProductivaDto> VisualizarCapacidadProductiva(int proveedorID, ProveedorDto proveedorDto = null, List<Material> materialesList = null, List<Campaña> campaniasList = null);
    }
}
