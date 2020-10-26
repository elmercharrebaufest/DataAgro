using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class HabilitacionCampaña
    {
        [Key]
        public int Id { get; set; }
        
        public int MaterialId { get; set; }
        public int CampañaId { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
    }
}
