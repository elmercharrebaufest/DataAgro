
using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ProveedorDto
    {
        public int ProveedorId { get; set; }
        public string CUIT { get; set; }
        public string RazonSocial { get; set; }
        public string Localidad { get; set; }
        public string Provincia { get; set; }
        public int? LocalidadId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? EstadoId { get; set; }
        //public int SegmentacionId { get; set; }
        //public string NombreReferente { get; set; }
        //public int? Calificacion { get; set; }
        //public string Intermediario { get; set; }
        //public string Observaciones { get; set; }
        public string Direccion { get; set; }
        public string CodigoPostal { get; set; }
        //public int? AreaInfluenciaId { get; set; }
        //public double? AlmacVolAnualTotal { get; set; }
        //public bool? AlmacHabilitadoSojaSust { get; set; }
        //public double? AlmacTonsMaxSojaSust { get; set; }
        //public double? AlmacHectSojaSust { get; set; }
        //public bool? ClienteMOA { get; set; }
        //public string GrupoCompras { get; set; }
        //public string Email1 { get; set; }
        //public string Email2 { get; set; }
        //public string Email3 { get; set; }
        //public string Email4 { get; set; }
        //public string Telefono1 { get; set; }
        //public int? TipoTelefono1Id { get; set; }
        //public string Telefono2 { get; set; }
        //public int? TipoTelefono2Id { get; set; }
        //public string Telefono3 { get; set; }
        //public int? TipoTelefono3Id { get; set; }
        //public string Telefono4 { get; set; }
        //public int? TipoTelefono4Id { get; set; }
        //public DateTime? FechaUltimoContacto { get; set; }
        //public string RiesgoComercialSap { get; set; }
        //public DateTime? FechaAlta { get; set; }
        public int? LocalidadCompraNetId { get; set; }
        public int? ProvinciaCompraNetId { get; set; }
        public string LocalidadCompraNet { get; set; }
        public string ProvinciaCompraNet { get; set; }
        public int? ClasificacionCompraNetId { get; set; }
        public string ClasificacionDescripcion { get; set; }
        //public int? BoletoCompraNetId { get; set; }
        //public int? BolsaCompraNetId { get; set; }
        public decimal? ComisionPorcentaje { get; set; }
        public bool? Consignatario { get; set; }
        public int SegmentacionId { get; set; }
        public bool? Deshabilitado { get; set; }
        public string Alias { get; set; }
        public int? ComisionistaId { get; set; }

        public bool? CuposConRiesgo { get; set; }

        public bool Comisionista { get; set; }
        public int? EstadoHomeId { get; set; }
        public string EstadoHomeMensaje { get; set; }
    }

    public class MensajeProveedorDto
    {
        public string DescripcionEstado { get; set; }
        public string Mensaje { get; set; }
    }
}



