using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Estado
    {
        [Key]
        public int EstadoId { get; set; }
        public string Descripcion { get; set; }
    }
}
   
