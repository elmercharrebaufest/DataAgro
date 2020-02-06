using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Cupo : ICloneable
    {
        [Key]
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public int CentroId { get; set; }
        public int MaterialId { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string CupoSap { get; set; }
        public int ZonaCupoId { get; set; }
        public int? ComercialId { get; set; }
        public bool? FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public string Observaciones { get; set; }
        public bool? Fason { get; set; }
        public string Destinatario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public int EstadoCupoId { get; set; }
        public int? CupoStop { get; set; }
        public string CreacionStop { get; set; }
        public string ErrorStop { get; set; }
        public int? FasonId { get; set; }
        public int? ContratoId { get; set; }
        public int? AgenteCompraId { get; set; }
        public int? FijacionDePrecioContratoId { get; set; }

        public int? TipoNegocioId { get; set; }
        public int? ConfiguracionEspacioDinamicoId { get; set; }
        [ForeignKey("ConfiguracionEspacioDinamicoId")]
        public virtual ConfiguracionEspacioDinamico ConfiguracionEspacioDinamico { get; set; }
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("ZonaCupoId")]
        public virtual ZonaCupo ZonaCupo { get; set; }
        [ForeignKey("EstadoCupoId")]
        public virtual EstadoCupo EstadoCupo { get; set; }

        [ForeignKey("FasonId")]
        public virtual Fason FasonEntidad { get; set; }
        [ForeignKey("ContratoId")]
        public virtual Contrato Contrato { get; set; }
        [ForeignKey("AgenteCompraId")]
        public virtual AgenteCompra AgenteCompra { get; set; }
        [ForeignKey("FijacionDePrecioContratoId")]
        public virtual FijacionDePrecioContrato FijacionDePrecioContrato { get; set; }
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}