using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DiferencialDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public int DiferencialDefault { get; set; }
        public string Comercial { get; set; }
        public List<DiferencialDto> historialDiferencial { get; set; }
        public Resultado Resultado { get; set; }
    }
}



