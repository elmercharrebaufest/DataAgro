using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class TipoNegocioDetalle
    {
        [Key]
        public int Id { get; set; }
        public string Descripcion { get; set; }
        public int TipoNegocioId { get; set; }
        public bool BoletoFisico { get; set; }
        public bool CartaOferta { get; set; }
        public bool Confirma { get; set; }

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }

    }
}
