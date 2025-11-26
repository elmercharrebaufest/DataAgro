using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoDto
    {
        public bool Ok { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}