using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CanalOperacion
    {
        [Key]
        public int CanalOperacionId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }

        public CanalOperacion()
        {
            Inhabilitado = false;
        }
    }
}
   



