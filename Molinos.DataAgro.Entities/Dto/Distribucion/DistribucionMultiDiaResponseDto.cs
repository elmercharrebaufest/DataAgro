using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class DistribucionMultiDiaResponseDto
    {
        public List<ResultadoDiaDto> ResultadosPorDia { get; set; }
        public List<FilaSapDto> SapConsolidado { get; set; }
        public ResumenGlobalDto ResumenGlobal { get; set; }

        public DistribucionMultiDiaResponseDto()
        {
            ResultadosPorDia = new List<ResultadoDiaDto>();
            SapConsolidado = new List<FilaSapDto>();
            ResumenGlobal = new ResumenGlobalDto();
        }
    }
}
