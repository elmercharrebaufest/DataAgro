using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosDatosContratoDto
    {
        public int NegocioId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? CampanaId { get; set; }
        public int? LocalidadId { get; set; }
        public int? ClasificacionId { get; set; }
        public string ContratoSAP { get; set; }
        public string Material { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string Destino { get; set; }
        public string Clasificacion { get; set; }
        public string StandarCalidad { get; set; }
        public string Campana { get; set; }
        public string TipoBoleto { get; set; }
        public string FechaOperacion { get; set; }
        public string PeriodoEntrega { get; set; }
        public string CuitVendedor { get; set; }
        public string CuitCorredor { get; set; }
        public string Moneda { get; set; }
    }
}
