using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class PrecioPizarraDto
    {
        public int Id { get; set; }
        public int Precio { get; set; }
        public int MaterialId { get; set; }
        public int PizarraId { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public string UnidadMedida { get; set; }
        public string Material { get; set; }
        public string Pizarra { get; set; }
        public DateTime Fecha { get; set; }
    }
}
