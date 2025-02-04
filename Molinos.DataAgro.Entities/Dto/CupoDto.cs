using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CupoDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string Proveedor { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime FechaIngreso { get; set; }
        public DateTime HoraIngreso { get; set; }
        public string CupoSap { get; set; }
        public int CupoStop { get; set; }
        public int ZonaCupoId { get; set; }
        public string ZonaCupo { get; set; }
        public string ZonaCupoSap { get; set; }
        public int? ComercialId { get; set; }
        public string Comercial { get; set; }
        public bool? FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public string Observaciones { get; set; }
        public bool? Fason { get; set; }
        public string Destinatario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public DateTime? FechaRegistro => FechaGeneracion.Date;
        public int EstadoCupoId { get; set; }
        public string EstadoCupo { get; set; }
        public string MensajeError { get; set; }
        public bool Acopio { get; set; }
        public string Fecha { get; set; }
        public string Hora => FechaGeneracion.ToString("HH:mm") ?? string.Empty;
        public int EstadoOrden { get; set; }
        public string UsuarioCreador { get; set; }
        public string MotivoRechazo { get; set; }

        public string EstadoPlanta { get; set; }
        public string CartaPorte { get; set; }
        public string CTG { get; set; }
        public DateTime? CTGFechaDesde { get; set; }
        public DateTime? CTGFechaHasta { get; set; }
        public string CuitOrigen { get; set; }
        public string RemitenteComercial { get; set; }
        public string CorredorComprador { get; set; }
        public string CorredorVendedor { get; set; }
        public string MercadoATermino { get; set; }
        public string Cosecha { get; set; }
        public string Peso { get; set; }
        public string Km { get; set; }
        public string IntermediarioFlete { get; set; }
        public string Transportista { get; set; }
        public string Chofer { get; set; }
        public string CuitOrigenAfip { get; set; }
        public string CodLocalidadOrigen { get; set; }
        public string NroEstablecimientoOrigen { get; set; }
        public int? NegocioId { get; set; }
        public string CentroCodigo { get; set; }
        public bool? Cumplimiento { get; set; }
        public bool? ConDescarga { get; set; }
        public string Codigo { get; set; }
        public bool Sustentable { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; }
    }
}