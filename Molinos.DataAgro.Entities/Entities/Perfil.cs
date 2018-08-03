using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Perfil
    {
        [Key]
        public int PerfilId { get; set; }
        public string Descripcion { get; set; }
    }
}
   
