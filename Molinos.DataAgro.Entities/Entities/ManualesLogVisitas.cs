using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ManualesLogVisitas
    {
        [Key]
        public int Id { get; set; }
        public int ManualId { get; set; }
        public int ComercialId { get; set; }
        public DateTime FechaVisita { get; set; }
    }
}
