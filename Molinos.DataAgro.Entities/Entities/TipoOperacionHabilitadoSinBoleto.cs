using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class TipoOperacionHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }
        public bool? Corredor { get; set; }
        public bool? Directo { get; set; }
    }

}
