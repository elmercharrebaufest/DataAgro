using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ResearchSituacionCultivoDto
    {
        public int Id { get; set; }
        public int EstadioId { get; set; }
        public string Observaciones { get; set; }
        public string Situacion { get; set; }
        public DateTime FechaHora { get; set; }
        public int MaterialId { get; set; }
        public int LocalidadId { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Partido { get; set; }
        public string Material { get; set; }
        public string Estadio { get; set; }
        public int CampaniaId { get; set; }
        public string Campania { get; set; }
    }
}
