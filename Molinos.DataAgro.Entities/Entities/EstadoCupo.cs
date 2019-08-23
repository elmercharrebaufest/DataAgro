using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class EstadoCupo
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int Orden { get; set; }
    }
}
