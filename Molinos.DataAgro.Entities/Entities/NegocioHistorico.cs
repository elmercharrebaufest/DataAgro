using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class NegocioHistorico
    {
        [Key]
        public int Id { get; set; }

        public int NegocioId { get; set; }

        public int TipoNegocioId { get; set; }

        public string Datos { get; set; }

        public DateTime Fecha { get; set; }

        public int? ComercialId { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }

    }
}
