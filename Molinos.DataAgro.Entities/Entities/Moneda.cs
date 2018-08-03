using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Moneda
    {
        [Key]
        public string MonedaId { get; set; }
        public string Descripcion { get; set; }       
    }    
}
   


