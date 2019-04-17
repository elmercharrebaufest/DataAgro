using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratosParaFijacionAgent
    {
        List<DatosFijacionDeContratoDto> ObtenerContratos(string CuitProveedor, string CuitCorredor, int materialId, string filtro);
    }
}