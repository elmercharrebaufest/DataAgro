using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FechaFeriado
    {
        [Key]
        public int Id { get; set; }
        public DateTime Feriado { get; set; }
    }

}