using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ClasificacionHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }
       
        public int ClasificacionId { get; set; }
        
        [ForeignKey("ClasificacionId")]
        public virtual ClasificacionCompraNet Clasificacion { get; set; }
    }

}
