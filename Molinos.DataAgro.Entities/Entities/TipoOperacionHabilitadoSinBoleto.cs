using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class TipoOperacionHabilitadoSinBoleto
    {
        [Key]
        public int Id { get; set; }
        public bool? Corredor { get; set; }
        public bool? Directo { get; set; }
    }

}
