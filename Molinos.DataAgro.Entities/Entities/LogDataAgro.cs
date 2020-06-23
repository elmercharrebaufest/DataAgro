using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Entities
{
    public class LogDataAgro
    {
        [Key]
        public int Id { get; set; }
        public string Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public string DatoModificado { get; set; }
        public int? NegocioId { get; set; }
        public int? CupoId { get; set; }
        public int? ProveedorId { get; set; }
        public string AccionRealizada { get; set; }
        public string Clase { get; set; }
        public int? RangoConfirmacionAutomaticaId { get; set; }
    }
}