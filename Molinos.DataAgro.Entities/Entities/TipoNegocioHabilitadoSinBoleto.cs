using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class TipoNegocioHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }

        public int TipoNegocioId { get; set; }

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
    }

}
