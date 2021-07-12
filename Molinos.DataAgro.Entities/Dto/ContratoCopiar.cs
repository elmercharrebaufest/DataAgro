using System;
using System.Globalization;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ContratoCopiar
    {
        public int Id { get; set; }
        public string RazonSocial { get; set; }
        public string Comercial { get; set; }
        
        public string Material { get; set; }
        public string Fecha { get; set; }
        public string ContratoSap { get; set; }
        public string Filtro { get; set; }
        public string tipoNegocio { get; set; }
        public double CantidadD { get; set; }
        public string Cantidad
        {
            get { return CantidadD.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")); }
        }
        public string Precio
        {
            get { return PrecioD.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")); }
        }
        public string NegocioDescripcion { get; set; }
        public decimal PrecioD { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Moneda { get; set; }
        public string MonedaId { get; set; }
    }
}
