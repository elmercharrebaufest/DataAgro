using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class AgenteCompra : Negocio
    {
        public int OperadorId { get; set; }
        //public override int TipoAgenteCompraId { get; set; }     
        
        [ForeignKey("OperadorId")]
        public virtual Operador Operador { get; set; }

        //[ForeignKey("TipoAgenteCompraId")]
        //public virtual TipoAgenteCompra TipoAgenteCompra { get; set; }
    }
}

