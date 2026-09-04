using Molinos.DataAgro.Entities.Dto.Distribucion;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDistribucionCuposManager
    {
        DistribucionResponseDto Calcular(DistribucionCuposRequestDto request);
        DistribucionMultiDiaResponseDto CalcularMultiDia(DistribucionMultiDiaRequestDto request);
    }
}
