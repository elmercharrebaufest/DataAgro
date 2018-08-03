using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class InformeComercialEstado
    {
        [Key]
        public int EstadoInformeId { get; set; }
        public string Descripcion { get; set; }
    }
}
