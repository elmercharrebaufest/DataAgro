using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class CierreDelDiaMailExternos
    {
        [Key]
        public int Id { get; set; }
        public string Mail { get; set; }
    }
}
