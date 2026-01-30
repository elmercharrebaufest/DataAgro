using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IComprasDetalleAgent
    {
        List<CompraDetalleAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial);
    }
}