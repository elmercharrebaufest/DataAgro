using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class RG2300
    {
        [Key]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Categoria { get; set; }
        public string Situacion { get; set; }
        public string CBU { get; set; }
        public DateTime? FechaActCBU { get; set; }
        public DateTime? FechaPubInclusion { get; set; }
        public DateTime? FechaPubSuspension { get; set; }
        public DateTime? FechaLevSuspension { get; set; }
        public DateTime? FechaNotExclusion { get; set; }
        public DateTime? FechaActRegistro { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaGeneracion { get; set; }
    }


}
   


