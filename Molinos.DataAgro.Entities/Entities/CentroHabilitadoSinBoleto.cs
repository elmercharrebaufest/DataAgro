using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class CentroHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }
       
        public int CentroId { get; set; }
        
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
    }

}
