using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class DistribucionResponseDto
    {
        public string Fecha { get; set; }
        public List<ResultadoContratoDistribucionDto> Resultados { get; set; }
        public List<ResumenDistribucionDto> ResumenPorMaterial { get; set; }
        public List<ResumenDistribucionDto> ResumenPorOperador { get; set; }
        public List<ResumenDistribucionDto> ResumenPorClase { get; set; }
        public List<FilaSapDto> Sap { get; set; }

        public DistribucionResponseDto()
        {
            Resultados = new List<ResultadoContratoDistribucionDto>();
            ResumenPorMaterial = new List<ResumenDistribucionDto>();
            ResumenPorOperador = new List<ResumenDistribucionDto>();
            ResumenPorClase = new List<ResumenDistribucionDto>();
            Sap = new List<FilaSapDto>();
        }
    }
}
