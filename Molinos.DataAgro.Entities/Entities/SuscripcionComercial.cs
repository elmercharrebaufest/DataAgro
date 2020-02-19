using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class SuscripcionComercial
    {
        [Key]
        public int Id { get; set; }
        public int ComercialId { get; set; }
        public string Key { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        
    }
}


