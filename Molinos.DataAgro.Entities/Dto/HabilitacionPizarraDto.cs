using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class HabilitacionPizarraDto
    {
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public DateTime DesdeVigencia { get; set; }
        public DateTime HastaVigencia { get; set; }
    }
}
