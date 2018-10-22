using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class SISA
    {
        [Key]
        public int Id { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public int EstadoCuit { get; set; }
        public DateTime? FechaVigenciaEstado { get; set; }
        public DateTime? FechaNotifDFEEstado { get; set; }
        public string CBU { get; set; }
        public DateTime? FechaActCBU { get; set; }
        public string Categoria { get; set; }
        public string SituacionCategoria { get; set; }
        public DateTime? FechaVigenciaCategoria { get; set; }
        public DateTime? FechaNotifDFECategoria { get; set; }
        public string Observaciones { get; set; }
        public DateTime? FechaGeneracion { get; set; }
    }    
}