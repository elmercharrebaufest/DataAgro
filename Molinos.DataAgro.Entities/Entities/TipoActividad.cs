using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoActividad
    {
        [Key]
        public int TipoActividadId { get; set; }
        public string Descripcion { get; set; }
        public int? CamposExtra { get; set; }
    }


}
   


