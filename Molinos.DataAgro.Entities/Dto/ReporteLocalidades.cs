using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{

    public class ReporteLocalidadesModel
    {
        public List<ReporteLocalidades> tablero { get; set; } = new List<ReporteLocalidades>();

    }

    public class ReporteLocalidades
    {
        public string CodLocalidad { get; set; }
        public int PartidoId { get; set; }
        public int ProvinciaId { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string NombreProvincia { get; set; }
    }
   
}