using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReporteCompraNetAgenteCompra
    {
        public int Id { get; set; }

        public int MaterialId { get; set; }

        public string Material { get; set; }

        public int TipoAgenteId { get; set; }

        public string TipoAgente { get; set; }

        public string Posicion { get; set; }

        public decimal PrecioPonderado { get; set; }

        public int OperadorId { get; set; }

        public string Operador { get; set; }

        public double Cantidad { get; set; }

    }
}
