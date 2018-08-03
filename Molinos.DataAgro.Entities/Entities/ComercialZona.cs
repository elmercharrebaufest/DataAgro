using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ComercialZona
    {
        [Key]
        public int ComercialZonaId { get; set; }
        public int ComercialId { get; set; }
        public int NroItem { get; set; }
        public int ZonaId { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ZonaId")]
        public virtual Zona Zona { get; set; }

    }
}
   


