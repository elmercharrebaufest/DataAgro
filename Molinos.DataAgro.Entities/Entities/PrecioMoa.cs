using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class PrecioMoa
    {
        [Key]
        public int Id { get; set; }
        public int TipoNegocioId { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }        
        public DateTime DesdeVigencia { get; set; }
        public DateTime HastaVigencia { get; set; }
        public DateTime? DesdeEntrega { get; set; }
        public DateTime? HastaEntrega { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public int? UsuarioCreadorId { get; set; }
        public bool? Habilitado { get; set; }
        public DateTime? FechaCreacion { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
        [ForeignKey("UsuarioCreadorId")]
        public virtual Comercial UsuarioCreador { get; set; }
    }
}

