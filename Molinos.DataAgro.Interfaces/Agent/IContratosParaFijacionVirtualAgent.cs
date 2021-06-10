using System.Collections.Generic;
using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IContratosParaFijacionVirtualAgent
    {
        List<DatosFijacionDeContratoDto> ObtenerContratosCanje(string CuitProveedor, string CuitCorredor, int materialId, string filtro,int fijacionId);
    }
}