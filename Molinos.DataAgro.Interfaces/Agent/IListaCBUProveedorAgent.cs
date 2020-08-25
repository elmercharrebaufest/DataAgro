using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IListaCBUProveedorAgent
    {
        List<PagoCBUDto> ListarCBU(string cuit, string filtro);
    }
}