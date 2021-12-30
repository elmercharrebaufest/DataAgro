using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class RangoConfirmacionAutomatica
    {
        public int Id { get; set; }
        public decimal PrecioMinimo { get; set; }
        public decimal PrecioMaximo { get; set; }
        public int MaterialId { get; set; }
        public string MonedaId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int? ZonaId { get; set; }
        public int Cantidad { get; set; }
        public int DesdeMes { get; set; }
        public int DesdeAnio { get; set; }
        public int HastaMes { get; set; }
        public int HastaAnio { get; set; }
        public int TipoNegocioId { get; set; }
        public int TipoRangoId { get; set; }

        public int? UsuarioCreadorId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? DesdeEntrega { get; set; }
        public DateTime? HastaEntrega { get; set; }

        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocioRangoConfirmacionAutomatica TipoNegocio { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("ZonaId")]
        public virtual GrupoDeCompras Zona { get; set; }
        [ForeignKey("UsuarioCreadorId")]
        public virtual Comercial Comercial { get; set; }

        [ForeignKey("TipoRangoId")]
        public virtual TipoRangoConfirmacionAutomatica TipoRango { get; set; }
    }

}
   


