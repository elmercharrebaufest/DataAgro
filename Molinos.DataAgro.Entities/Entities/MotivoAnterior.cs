using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class MotivoAnterior
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
    }
}
   


