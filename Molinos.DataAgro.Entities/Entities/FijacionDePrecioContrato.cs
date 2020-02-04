using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FijacionDePrecioContrato
    {
        [Key]
        public int FijacionDePrecioContratoId { get; set; }
        public int? ContratoId { get; set; }
        public string ContratoSAP { get; set; }
        public int ProveedorId { get; set; }
        public int? MaterialId { get; set; }
        public string MonedaId { get; set; }
        public int ComercialId { get; set; }
        public decimal Precio { get; set; }
        public double Cantidad { get; set; }
        public DateTime Fecha { get; set; }
        public int? Ampliaciones { get; set; }
        public int EstadoId { get; set; }
        public string Observacion { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int? CorredorId { get; set; }
        public string FijacionSAP { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public int CampanaId { get; set; }
        public string Posicion { get; set; }
        public bool? TrigoEspecial { get; set; }
        public int? FinDelDiaId { get; set; }
        public decimal? PrecioNeto { get; set; }
        public bool? Pizarra { get; set; }
        public int? DiasPesificado { get; set; }
        public bool? PagoDiferidoContrato { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? DestinoId { get; set; }
        public int? ProveedorCreadorId { get; set; }
        public string MotivoRechazo { get; set; }
        public string UsuarioCreador { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial ComercialCreador { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }
        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; }
        [ForeignKey("FinDelDiaId")]
        public virtual FinDelDia FinDelDia { get; set; }

        [InverseProperty("FijacionDePrecioContrato")]
        public List<AperturaPrecio> AperturaPrecio { get; set; }
        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }
        [ForeignKey("ProveedorCreadorId")]
        public virtual Proveedor ProveedorCreador { get; set; }
    }
}

