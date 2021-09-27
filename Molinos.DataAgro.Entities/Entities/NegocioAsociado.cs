using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class NegocioAsociado
    {
        [Key]
        public int Id { get; set; }
        public int AFijarId { get; set; }        
        public int AsociadoId { get; set; }

        [ForeignKey("AFijarId")]
        public virtual Negocio Contrato { get; set; }
        [ForeignKey("AsociadoId")]
        public virtual Negocio Asociado { get; set; }
    }
}
   


