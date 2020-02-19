using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CampañaMaterialPorMes
    {
        [Key]
        public int CampañaMaterialPorMesId { get; set; }
        public int? NroItem { get; set; }
        public int? Mes { get; set; }
        public int? Año { get; set; }
        public double? Toneladas { get; set; }
        public int? CampañaMaterialId { get; set; }
        public int? ComercialId { get; set; }

        [ForeignKey("CampañaMaterialId")]
        public virtual CampañaMaterial CampañaMaterial { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }

    }    
}
   


