using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public string CupoStop { get; set; }
        public int ZonaCupoId { get; set; }
        public string ZonaCupo { get; set; }
        public int? ComercialId { get; set; }
        public string Comercial { get; set; }
        public bool? FleteProcedencia { get; set; }
        public string Calidad { get; set; }
        public string Observaciones { get; set; }
        public bool? Fason { get; set; }
        public string Destinatario { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string FechaRegistro { get; set; }
        public int EstadoCupoId { get; set; }
        public string EstadoCupo { get; set; }
        public string MensajeError { get; set; }
        public bool Acopio { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int EstadoOrden { get; set; }
    }
}