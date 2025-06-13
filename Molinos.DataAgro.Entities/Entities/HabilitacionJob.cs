using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class HabilitacionJob
    {
        [Key]
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Habilitado { get; set; }
    }
}
