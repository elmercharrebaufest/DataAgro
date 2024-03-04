using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class PuestoApoderado
    {
        [Key]
        public int PuestoApoderadoId { get; set; }
        public string Descripcion { get; set; }
        public PuestoApoderado()
        {
            Descripcion = "";
        }
    }
}
   



