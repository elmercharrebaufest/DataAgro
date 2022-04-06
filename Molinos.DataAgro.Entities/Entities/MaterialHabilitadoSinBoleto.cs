using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class MaterialHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }
       
        public int MaterialId { get; set; }
        
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }

}
