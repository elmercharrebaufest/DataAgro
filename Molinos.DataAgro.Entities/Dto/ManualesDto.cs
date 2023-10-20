using System;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ManualesDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Path { get; set; }
        public DateTime FechaUltimaActualizacion { get; set; }
        public int Version { get; set; }
        public int CantidadVisitas { get; set; }
    }

    public class ManualResult : Resultado
    { 

    }
}
