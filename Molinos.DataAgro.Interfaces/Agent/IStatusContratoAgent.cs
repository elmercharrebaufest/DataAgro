using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IStatusContratoAgent
    {
        EstadoSAPDto ValidarEstado(string contratoSap);
        List<EstadoSAPDto> ValidarEstados(List<string> contratosSap);
    }
}