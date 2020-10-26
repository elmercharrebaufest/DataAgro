using Molinos.DataAgro.Entities.Entities;
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class HabilitacionCampañaDto
    {
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public int CampañaId { get; set; }
        public string Campaña { get; set; }
       
    }
}
