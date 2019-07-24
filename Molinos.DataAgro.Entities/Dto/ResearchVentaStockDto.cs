using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ResearchVentaStockDto
    {
        public int Id { get; set; }
        public decimal VendidoAPrecio { get; set; }
        public string Observaciones { get; set; }
        public decimal Almacenado { get; set; }
        public DateTime FechaHora { get; set; }
        public int MaterialId { get; set; }
        public int LocalidadId { get; set; }
        public string Partido { get; set; }
        public string Comercial { get; set; }
        public string Localidad { get; set; }
        public string Material { get; set; }
    }
}
