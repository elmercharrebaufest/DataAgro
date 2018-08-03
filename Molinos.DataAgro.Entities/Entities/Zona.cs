using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Zona
    {
        [Key]
        public int ZonaId { get; set; }
        public string Descripcion { get; set; }
    }
}
   


