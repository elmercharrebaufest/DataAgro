using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA
{
    public class ControlDeBoletosValidacionIADatosContratoDto
    {
        public string CuitVendedor { get; set; }

        public string CuitCorredor { get; set; }

        public string Material { get; set; }

        public double Kilos { get; set; }

        public decimal Precio { get; set; }

        public string Moneda { get; set; }

        public string PeriodoEntrega { get; set; }

        public DateTime FechaOperacion { get; set; }

        public string ProvinciaOrigen { get; set; }

        public string LocalidadOrigen { get; set; }

        public string Destino { get; set; }

        public string Cosecha { get; set; }

        public DateTime? FechaFijacionDesde { get; set; }

        public DateTime? FechaFijacionHasta { get; set; }

        public decimal? CantidadMinFijacion { get; set; }

        public decimal? CantidadMaxFijacion { get; set; }

        public string Bolsa { get; set; }

        public string TipoBoleto { get; set; }
    }
}
