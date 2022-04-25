using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class CupoNoPropioDto
    {
        public int Id { get; set; }
        public string Codigo { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime? FechaIngreso { get; set; }
        public int? CupoId { get; set; }
        public string Cupo { get; set; }
        public DateTime FechaAlta { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; }
        public bool Disponible { get; set; }
        public string CentroCodigo { get; set; }
        public bool Utilizado { get; set; }
    }
}
