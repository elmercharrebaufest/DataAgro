using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioPactadosDto
    {
        public int Id { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal Precio { get; set; }
        public string MonedaPactadoId { get; set; }
        public string MonedaPactadoDesc { get; set; }
        public decimal? ImportePactado { get; set; }
        public string MonedaImportePactadoId { get; set; }
        public string MonedaImportePactadoDesc { get; set; }
        public decimal? Porcentaje { get; set; }
        public int ContratoId { get; set; }

    }
}
