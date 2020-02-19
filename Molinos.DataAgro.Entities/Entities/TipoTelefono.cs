using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoTelefono
    {
        [Key]
        public int TipoTelefonoId { get; set; }
        public string Descripcion { get; set; }
    }
}
   


