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
        public int ClaseId { get; set; }
        public string AccionRealizada { get; set; }
        public string Clase { get; set; }
        public string Tipo { get; set; }
    }
}