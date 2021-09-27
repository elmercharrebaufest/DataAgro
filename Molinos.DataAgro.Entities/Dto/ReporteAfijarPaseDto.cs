using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteAfijarPaseDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public DateTime? FechaOperacion { get; set; }
        public DateTime? FechaHasta { get; set; }
        public DateTime FechaEntrega { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public string Negocio { get; set; }
        public int ProveedorId { get; set; }
        public int CorredorId { get; set; }
        public string Cuit { get; set; }
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string CUITCorredor { get; set; }
        public double Cantidad { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
        public decimal? PrecioNetoPonderado { get; set; }
        public decimal? PrecioPonderado { get; set; }
        public string Estado_Contrato { get; set; }
        public string MonedaBasis { get; set; }
        public string MonedaBonificacion { get; set; }
        public int Estado { get; set; }
        public string MonedaComision { get; set; }
        public string MonedaRedespacho { get; set; }
        public string MonedaFinanciero { get; set; }
        public decimal? ImporteBasis { get; set; }
        public decimal? PorcentajeBonificacion { get; set; }
        public decimal? ImporteBonificacion { get; set; }
        public decimal? ImporteComision { get; set; }
        public decimal? PorcentajeComision { get; set; }
        public decimal? ImporteRedespacho { get; set; }
        public decimal? ImporteFinanciero { get; set; }
        public double? KilosPendiente { get; set; }
        public string Moneda { get; set; }
        public string Posicion { get; set; }
        public decimal Plus { get; set; }
    }
}
