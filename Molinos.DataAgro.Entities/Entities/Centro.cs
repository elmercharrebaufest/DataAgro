using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Centro
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public string CodigoSap { get; set; }
    }

}
   


