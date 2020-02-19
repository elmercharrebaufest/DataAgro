using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class NotificacionResearch
    {
        [Key]
        public int Id { get; set; }
        public int CampanaId { get; set; }
        public int MaterialId { get; set; }
        public string Mensaje { get; set; }
        public int TipoResearchId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("TipoResearchId")]
        public virtual TipoResearch TipoResearch { get; set; }
    }
}
   


