using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface IConfirmaLoteBorradorAgent
    {
        ConfirmaAltaLoteBorradorResultDto ConfirmaLoteBorrador(List<ResultadoClausula> clausulas, List<int> equipo, BasicoContrato contrato, EstadosConfirmaDto estadosConfirmaDto);
    }
}
