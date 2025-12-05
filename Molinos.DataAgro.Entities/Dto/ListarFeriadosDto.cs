using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ListarFeriadosDto
    {
        public List<FechaFeriadoDto> Datos { get; set; }

        public ListarFeriadosDto()
        {
            this.Datos = new List<FechaFeriadoDto>();
        }
    }
}