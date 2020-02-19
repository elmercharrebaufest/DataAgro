using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AreaInfluencia
    {
        [Key]
        public int AreaInfluenciaId { get; set; }

        public string Descripcion { get; set; }
        
    }
}
   



