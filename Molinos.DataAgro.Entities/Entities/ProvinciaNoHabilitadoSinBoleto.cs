using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ProvinciaNoHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }

        public int ProvinciaId { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }
    }

}
