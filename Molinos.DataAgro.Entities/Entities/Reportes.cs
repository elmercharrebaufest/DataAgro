using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Reportes
    {
        [Key]
        public string Identificador { get; set; }
        public byte[] Contenido { get; set; }
        public string FileName { get; set; }

    }
}
