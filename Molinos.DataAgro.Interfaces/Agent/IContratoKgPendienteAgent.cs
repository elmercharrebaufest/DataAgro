using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratoKgPendienteAgent
    {
        List<ContratoKgPendiente> Consultar(List<ContratoKgPendiente> contratos);
    }
}