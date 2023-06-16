using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Operador
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoPrimary { get; set; }
    }
}