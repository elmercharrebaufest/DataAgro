using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class Interes
    {
        [Key]
        public int InteresId { get; set; }

        public string Descripcion { get; set; }
        
        public Interes()
        {
            Descripcion = "";
        }
    }
}
