using Molinos.DataAgro.Entities.Entities;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class InformeComercialProduccionDto
    {
        [Key]
        public int InformeComerciaProduccionId { get; set; }
        public int InformeComercialId { get; set; }
        public int? MaterialId { get; set; }
        public int? Hectareas { get; set; }
        public decimal? Toneladas { get; set; }
        public int? LocalidadId { get; set; }
        public bool? Propio { get; set; }
        public bool? Alquilado { get; set; }
        public bool? RtaOkSap { get; set; }
        public string MensajeSap { get; set; }
        public DateTime? FechaDescarga { get; set; }

        public virtual InformeComercial InformeComercial { get; set; }       
        public virtual Material Material { get; set; }      
        public virtual Localidad Localidad { get; set; }
    }
}
