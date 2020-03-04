using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Fason : Negocio
    {
        public int TipoFasonId { get; set; }
        [ForeignKey("TipoFasonId")]
        public virtual TipoFason TipoFason { get; set; }
    }
}

