using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchHumedadSuelo
    {
        [Key]
        public int HumedadSueloId { get; set; }
        public string Descripcion { get; set; }
    }
}
