using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class Confirma
    {
        [Key]
        public int Id { get; set; }
        public int NegocioId { get; set; }
        public int ComercialId { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaAnulacion { get; set; }
        public bool IsWebService { get; set; }
        public int? Version { get; set; }
        public string Archivo { get; set; }

        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }

        [ForeignKey("ComercialId")]
        public Comercial Comercial { get; set; }
    }
}
