using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Objetivo
    {
        [Key]
        public int ObjetivoId { get; set; }
        public int CampañaId { get; set; }
        public int NroItem { get; set; }
        public int ProveedorId { get; set; }
        public int MaterialId { get; set; }
        public int? ComercialId { get; set; }
        public int? GrupoDeComprasId { get; set; }

        public double ToneladasObjetivos { get; set; }

        [ForeignKey("CampañaId")]
        public virtual Campaña Campaña { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
    }
}



