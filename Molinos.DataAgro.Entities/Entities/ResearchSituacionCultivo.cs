using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchSituacionCultivo
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public int LocalidadId { get; set; }
        public int ComercialId { get; set; }
        public int EstadioId { get; set; }
        public string Situacion { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }

        [ForeignKey("EstadioId")]
        public virtual Estadio Estadio { get; set; }
        [ForeignKey("ComercialId")]
        public Comercial Comercial { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
    }
}
