using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ObjetivoComercial
    {
        [Key]
        public int Id { get; set; }
        public int CampanaId { get; set; }
        public int MaterialId { get; set; }        
        public int ComercialId { get; set; }
        public double ToneladasObjetivos { get; set; }

        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}



