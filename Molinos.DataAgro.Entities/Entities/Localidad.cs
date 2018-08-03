using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Localidad
    {
        [Key]
        public int LocalidadId { get; set; }
        public string CodLocalidad { get; set; }
        public string Nombre { get; set; }
        public int ProvinciaId { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }
        
        public Localidad()
        {
            CodLocalidad = "";
            Nombre = "";  
        }
    }
}
   



