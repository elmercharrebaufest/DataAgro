using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Condicion
    {
        [Key]
        public int CondicionId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }

        public Condicion()
        {
            Descripcion = "";
        }
    }
}
   



