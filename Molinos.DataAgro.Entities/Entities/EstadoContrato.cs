using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class EstadoContrato
    {
        [Key]
        public int EstadoContratoId { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
