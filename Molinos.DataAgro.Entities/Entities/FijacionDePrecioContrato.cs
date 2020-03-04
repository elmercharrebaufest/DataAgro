using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato : Negocio
    {
        public int? ContratoId { get; set; }
        public string FijacionSAP { get; set; }
        public bool? PagoDiferidoContrato { get; set; }
        public int? ProveedorCreadorId { get; set; }
        public string MotivoRechazo { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
        
        [ForeignKey("ProveedorCreadorId")]
        public virtual Proveedor ProveedorCreador { get; set; }
    }
}

