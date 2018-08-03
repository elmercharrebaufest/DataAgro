using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Segmentacion
    {
        [Key]
        public int SegmentacionId { get; set; }
        public string Descripcion { get; set; }
        public string Grupo { get; set; }
    }


}
   


