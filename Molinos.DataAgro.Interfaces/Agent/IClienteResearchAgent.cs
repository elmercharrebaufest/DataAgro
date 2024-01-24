using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface IClienteResearchAgent
    {
        List<Research> ConsultarItems();
    }
}
