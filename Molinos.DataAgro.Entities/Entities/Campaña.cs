using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Campaña
    {
        [Key]
        public int CampañaId { get; set; }
        public string Descripcion { get; set; }
        public DateTime? Hasta { get; set; }
    }
}
   


