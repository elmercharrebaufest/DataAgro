using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto.Distribucion
{
    public class ProcesarEnDataAgroRequestDto
    {
        public List<FilaSapDto> Sap { get; set; }

        public ProcesarEnDataAgroRequestDto()
        {
            Sap = new List<FilaSapDto>();
        }
    }
}
