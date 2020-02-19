using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FACACOP
    {
        [Key]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public DateTime Fecha1 { get; set; }
        public DateTime Fecha2 { get; set; }
        public string ObservacionesEspeciales { get; set; }
    }
}
   


