using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class Boleto
    {
        [Key]
        public int Id { get; set; }

        public int TipoNegocioDetalleId { get; set; }
        public int NegocioId { get; set; }
        public int Version { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public int ComercialId { get; set; }

        [ForeignKey("TipoNegocioDetalleId")]
        public TipoNegocioDetalle TipoNegocioDetalle { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("ComercialId")]
        public Comercial Comercial { get; set; }

    }
}
