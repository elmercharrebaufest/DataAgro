using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AcopioCampaña
    {
        [Key]
        public int AcopioCampañaId { get; set; }
        public int AcopioId { get; set; }        
        public int NroItem { get; set; }
        public double? Toneladas { get; set; }
        public int CampañaId { get; set; }        
        public bool? HasArrendadas { get; set; }

        [ForeignKey("AcopioId")]
        public virtual Acopio Acopio { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
    }


}



