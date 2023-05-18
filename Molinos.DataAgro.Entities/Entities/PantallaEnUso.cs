using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class PantallaEnUso
    {
        [Key]
        public int Id { get; set; }
        public string NombrePantalla { get; set; }
        public int? ComercialId { get; set; }
        public DateTime FechaHoraInicioUso { get; set; }
        public DateTime? FechaHoraFinUso { get; set; }
        public string UsuarioId { get; set; }
    }
}
