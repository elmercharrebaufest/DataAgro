using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato : Negocio
    {
        public int? ContratoId { get; set; }
        public string FijacionSAP { get; set; }
        public bool? PagoDiferidoContrato { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }        
        public bool? Anticipo { get; set; }
        public string ClasificacionContrato { get; set; }
        public decimal? ImporteAPrecioContrato { get; set; }
        public decimal? PorcentajeAPrecioContrato { get; set; }
        public string MonedaAPrecioContrato { get; set; }
        public decimal? ImporteSobrePrecioContrato { get; set; }
        public decimal? PorcentajeSobrePrecioContrato { get; set; }
        public string MonedaSobrePrecioContrato { get; set; }

   

    }
}

