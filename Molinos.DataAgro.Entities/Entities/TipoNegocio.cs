using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoNegocio
    {
        [Key]
        public int TipoNegocioId { get; set; }
        public string Descripcion { get; set; }
    }


}
   


