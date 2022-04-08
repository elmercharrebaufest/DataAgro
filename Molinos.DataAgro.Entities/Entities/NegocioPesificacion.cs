using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class NegocioPesificacion
    {
        [Key]
        public int Id { get; set; }
        public bool? Excepcion { get; set; }
        public DateTime? FechaExcepcion { get; set; }
        public DateTime? FechaInstruccion { get; set; }
        public DateTime? FechaEnvio { get; set; }
        public int? ComercialId { get; set; }

        public int NegocioId { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

    }
}
   


