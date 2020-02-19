using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IComprasAgent
    {
        List<CompraAgentDto> Comprar(string CUIT, string UsuarioComercial);
        List<CompraAgentDto> ComprarIniciales(List<string> CUIT, string UsuarioComercial);
    }
}