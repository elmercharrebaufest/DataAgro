using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public class Archivo
    {
        public string Nombre { get; set; }
        public long PesoKB { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime FechaUltimaModificacion { get; set; }
    }
}
