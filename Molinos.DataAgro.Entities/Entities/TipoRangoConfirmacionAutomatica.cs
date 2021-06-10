using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoRangoConfirmacionAutomatica
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }

}
   


