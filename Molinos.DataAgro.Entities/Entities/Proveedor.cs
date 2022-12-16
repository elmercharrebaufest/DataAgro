
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Proveedor
    {
        [Key]
        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public int? EstadoId { get; set; }
        public string RazonSocial { get; set; }
        public int SegmentacionId { get; set; }
        public string NombreReferente { get; set; }
        public int? Calificacion { get; set; }
        public string Intermediario { get; set; }
        public string Observaciones { get; set; }
        public string Direccion { get; set; }
        public int? LocalidadId { get; set; }
        public int? ProvinciaId { get; set; }
        public string CodigoPostal { get; set; }
        public int? AreaInfluenciaId { get; set; }
        public double? AlmacVolAnualTotal { get; set; }
        public bool? AlmacHabilitadoSojaSust { get; set; }
        public double? AlmacTonsMaxSojaSust { get; set; }
        public double? AlmacHectSojaSust { get; set; }
        public bool? ClienteMOA { get; set; }
        public string GrupoCompras { get; set; }
        public string Email1 { get; set; }
        public string Email2 { get; set; }
        public string Email3 { get; set; }
        public string Email4 { get; set; }
        public string Telefono1 { get; set; }
        public int? TipoTelefono1Id { get; set; }
        public string Telefono2 { get; set; }
        public int? TipoTelefono2Id { get; set; }
        public string Telefono3 { get; set; }
        public int? TipoTelefono3Id { get; set; }
        public string Telefono4 { get; set; }
        public int? TipoTelefono4Id { get; set; }
        public DateTime? FechaUltimoContacto { get; set; }
        public string RiesgoComercialSap { get; set; }
        public DateTime? FechaAlta { get; set; }
        public int? LocalidadCompraNetId { get; set; }
        public int? ProvinciaCompraNetId { get; set; }
        public int? ClasificacionCompraNetId { get; set; }
        public int? BoletoCompraNetId { get; set; }
        public int? BolsaCompraNetId { get; set; }
        public bool? Consignatario { get; set; }
        public decimal? ComisionPorcentaje { get; set; }
        public bool? PlanCanje { get; set; }
        public bool? CuposConRiesgo { get; set; }
        public string Alias { get; set; }

        public int? ComisionistaId { get; set; }
        public bool? Comisionista { get; set; }
        public bool? OperaConMATBA { get; set; }
        public int? EstadoHomeId { get; set; }
        public string EstadoHomeMensaje { get; set; }
        public bool? Deshabilitado { get; set; }

        [ForeignKey("EstadoId")]
        public virtual Estado Estado { get; set; }
        [ForeignKey("SegmentacionId")]
        public virtual Segmentacion Segmentacion { get; set; }
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; }
        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; }
        [ForeignKey("AreaInfluenciaId")]
        public virtual AreaInfluencia AreaInfluencia { get; set; }

        [ForeignKey("LocalidadCompraNetId")]
        public virtual Localidad LocalidadCompraNet { get; set; }
        [ForeignKey("ProvinciaCompraNetId")]
        public virtual Provincia ProvinciaCompraNet { get; set; }
        [ForeignKey("ClasificacionCompraNetId")]
        public virtual ClasificacionCompraNet ClasificacionCompraNet { get; set; }
        [ForeignKey("BoletoCompraNetId")]
        public virtual BoletoCompraNet BoletoCompraNet { get; set; }
        [ForeignKey("BolsaCompraNetId")]
        public virtual BolsaCompraNet BolsaCompraNet { get; set; }

        [InverseProperty("ProveedoresAsociados")]
        public virtual ICollection<Rol> RolesAsociados { get; set; }

        [InverseProperty("Proveedor")]
        public virtual ICollection<ProveedorComercial> ProveedorComercialAsociados { get; set; }

        [ForeignKey("ComisionistaId")]
        public virtual Proveedor ComisionistaE { get; set; }
        [ForeignKey("EstadoHomeId")]
        public virtual EstadoHome EstadoHome { get; set; }

    }
}



