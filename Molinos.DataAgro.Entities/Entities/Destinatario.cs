using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Destinatario
    {
        [Key]
        public int DestinatarioId { get; set; }

        public string Descripcion { get; set; }

        public bool? Inhabilitado { get; set; }

        public Destinatario()
        {
            DestinatarioId = 0;
            Descripcion = "";
            Inhabilitado = false;
        }
    }
}
   



