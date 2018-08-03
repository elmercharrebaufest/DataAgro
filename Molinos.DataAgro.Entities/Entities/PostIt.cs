using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class PostIt
    {
        [Key]
        public int ComercialId { get; set; }
        public string Texto { get; set; }
    }
}
