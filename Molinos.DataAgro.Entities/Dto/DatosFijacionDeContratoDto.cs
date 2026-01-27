using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DatosFijacionDeContratoDto
    {
        public string ContratoId { get; set; }
        public string KilosAplicados { get; set; }
        public string KilosPendiente { get; set; }
        public string KilosContrato { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Filtro { get; set; }
        public string DesdeEntrega { get; set; }
        public string HastaEntrega { get; set; }
        public string Posicion { get; set; }
        public bool? Calidad { get; set; }
        public string Campana { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? Centro { get; set; }
        public string CentroDescripcion { get; set; }
        public string ARecibirSinPrecio { get; set; }
        public string RecibidoSinFijar { get; set; }
        public string Color { get; set; }
        public decimal ImporteAPrecio { get; set; }
        public decimal ImporteSobrePrecio { get; set; }
        public string MonedaAPrecio { get; set; }
        public string MonedaSobrePrecio { get; set; }
        public decimal PorcentajeAPrecio { get; set; }
        public decimal PorcentajeSobrePrecio { get; set; }
        public string CondicionFijacionCod { get; set; }
        public string CondicionFijacionDescripcion { get; set; }
        public string CondicionPagoCod { get; set; }
        public string CondicionPagoDescripcion { get; set; }
        public bool? ChequeElectronico { get; set; }
        public List<CalidadDto> Calidades { get; set; }
        public int CampanaId { get; set; }
        public string Clasificacion { get; set; }
        public bool Cesion { get; set; }
        public bool Anticipo { get; set; }
        public string FijacionSap { get; set; }
        public List<AperturaPrecioDto> Aperturas { get; set; }
        public List<DescuentoBonificacionDto> Bonificaciones { get; set; }
        public bool Virtual { get; set; }
        public decimal KgContratoTotal { get; set; }
        public bool Pase { get; set; }
        public int? ProveedorComisionistaId { get; set; }
        public List<SustentableDto> Sustentables { get; set; }
        public bool MercsDeposito { get; set; }
        public bool TarifaAConvenir { get; set; }
        public bool MercAplicada { get; set; }
        public bool Sustentable { get; set; }
        public bool EPA { get; set; }
        public bool EUDR { get; set; }
        public decimal? ImporteSustentable { get; set; }
        public Moneda MonedaSustentable { get; set; }
        public int? SustentableTipoDBId { get; set; }
        public int? KgMaximos { get; set; }
        public int? KgMinimos { get; set; }
    }
}