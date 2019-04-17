using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDatosProveedorAgent
    {
        List<DatosProveedorAgentDto> ObtenerDatosDeProveedor(List<Datos> datos);
        List<DatosProveedorAgentDto> ObtenerDatosDeProveedorEstado(List<string> CUIT, List<string> usuarios);
    }
}