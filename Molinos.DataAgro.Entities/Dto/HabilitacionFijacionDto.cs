using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class HabilitacionFijacionDto
    {
        public int Id { get; set; }
        public DateTime Dia { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
    }
}
