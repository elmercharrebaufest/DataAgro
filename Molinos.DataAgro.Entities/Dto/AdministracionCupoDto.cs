using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class AdministracionCupoDto
    {
        public int Id { get; set; }
        public int? ProveedorId { get; set; }
        public int? ComercialId { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadCupo { get; set; }
        public int CantidadDeCupo { get; set; } //es igual al anterior?
        public int CantidadFleteProcedencia { get; set; }
        public int CantidadDeCupoMax { get; set; }
        public int CantidadFleteProcedenciaMax { get; set; }
        public int EstadoId { get; set; }
        public int CentroId { get; set; }
        public int ZonaId { get; set; }
        public int MaterialId { get; set; }
        public string Comercial { get; set; }
        public string Proveedor { get; set; }
        public string Centro { get; set; }
        public string Zona { get; set; }
        public string Material { get; set; }
        public string StandardDeCalidad { get; set; }
        public int TipoNegocioId { get; set; }
        public string Destinatario { get; set; }
        public int? NegocioId { get; set; }
        public int? ConfiguracionEspacioDinamicoId { get; set; }
        public bool Excedente { get; set; }
        public bool? Fason { get; set; }
        public string Estado { get; set; }
        public string TipoAdministracionCupo { get; set; }
        public int TipoAdministracionCupoId { get; set; }
        public string Observacion { get; set; }
        public int? ComercialCreadorId { get; set; }
        public DateTime? FechaCreacion { get; set; }
        public DateTime? FechaCreacionSinHora => FechaCreacion?.Date;
        public DateTime? FechaDecision { get; set; }
        public int? SugerenciaCupoId { get; set; }
        public string Hora => FechaCreacion?.ToString("HH:mm") ?? string.Empty;
        public bool? ConDescarga { get; set; }
        public DateTime? FechaCreacionConHora { get; set; }
        public string Calidad { get; set; }
        public List<DiaCupo> Dias { get; set; }
        public int CantidadFleteProcedenciaOriginal { get; set; }
        public int CantidadDeCupoOriginal { get; set; }
        public bool Sustentable { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; }
        public string ContratoSAP { get; set; }
        public double KgPendientes { get; set; }
        public int CuposRestantes { get; set; }
    }
}
