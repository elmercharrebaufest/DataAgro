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
        public bool? Cesion { get; set; }
        public bool? Anticipo { get; set; }


    }
}

