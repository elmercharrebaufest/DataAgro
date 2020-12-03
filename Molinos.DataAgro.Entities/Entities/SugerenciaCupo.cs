using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class SugerenciaCupo : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public DateTime FechaSugerida { get; set; }
        public int CantidadDeCupos { get; set; }
        public int CantidadCupoOriginal { get; set; }
        
        //public int? FasonId { get; set; }
        //public int? ContratoId { get; set; }
        //public int? AgenteCompraId { get; set; }
        //public int? FijacionDePrecioContratoId { get; set; }
        public int? NegocioId { get; set; }
        public int TipoNegocioId { get; set; }
        public decimal? Precio { get; set; }
        public string MonedaId { get; set; }
        public decimal Puntuacion { get; set; }
        public int? ProveedorId { get; set; }
        public bool? Aceptado { get; set; }
        public string StandardDeCalidad { get; set; }
        public int? ZonaCupoId { get; set; }
        public string Destinatario { get; set; }
        public int ComercialId { get; set; }
        public string ContratoSAP { get; set; }
        public int? ConfiguracionEspacioDinamicoId { get; set; }

        [ForeignKey("ConfiguracionEspacioDinamicoId")]
        public virtual ConfiguracionEspacioDinamico ConfiguracionEspacioDinamico { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("NegocioId")]
        public virtual Negocio Negocio { get; set; }
        //[ForeignKey("FasonId")]
        //public virtual Fason Fason { get; set; }
        //[ForeignKey("ContratoId")]
        //public virtual Contrato Contrato { get; set; }
        //[ForeignKey("AgenteCompraId")]
        //public virtual AgenteCompra AgenteCompra { get; set; }
        //[ForeignKey("FijacionDePrecioContratoId")]
        //public virtual FijacionDePrecioContrato FijacionDePrecioContrato { get; set; }
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }

        [ForeignKey("ZonaCupoId")]
        public virtual ZonaCupo ZonaCupo { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        public string Puntuaciones { get; set; }
        public string MotivoRechazo { get; set; }
        public bool CDWarrant { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}