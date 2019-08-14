using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
   public partial class ResearchVentaStock
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int LocalidadId { get; set; }
        public int CampaniaId { get; set; }
        public int ComercialId { get; set; }
        public decimal Almacenado { get; set; }
        public decimal VendidoAPrecio{ get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }
        [ForeignKey("ComercialId")]
        public Comercial Comercial { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }       
        [ForeignKey("CampaniaId")]
        public virtual Campaña Campania { get; set; }
    }
}
