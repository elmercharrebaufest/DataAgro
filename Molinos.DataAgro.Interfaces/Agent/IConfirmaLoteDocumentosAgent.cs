using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConfirmaLoteDocumentosAgent
    {
        ConfirmaAltaLoteResultDto ConfirmaLoteDocumentos(List<ResultadoClausula> clausulas, List<int> equipo, BasicoContrato contrato, EstadosConfirmaDto estadosConfirmaDto);
    }
}
