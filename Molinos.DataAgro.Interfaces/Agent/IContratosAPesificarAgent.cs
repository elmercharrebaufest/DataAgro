using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratosAPesificarAgent
    {
        List<PesificarAgentDto> ConsultarPorUnProveedor(string cuit);
        List<PesificarAgentDto> ConsultarTodo(List<string> cuits);
    }
}