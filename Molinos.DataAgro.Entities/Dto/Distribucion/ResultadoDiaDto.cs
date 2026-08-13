using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ResultadoDiaDto
    {
        public string Fecha { get; set; }
        public List<ResultadoContratoDistribucionDto> Resultados { get; set; }
        public List<FilaSapDto> Sap { get; set; }

        public ResultadoDiaDto()
        {
            Resultados = new List<ResultadoContratoDistribucionDto>();
            Sap = new List<FilaSapDto>();
        }
    }
}
