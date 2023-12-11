using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ResearchCondicion
    {
        [Key]
        public int CondicionId { get; set; }
        public string Descripcion { get; set; }
    }
}
