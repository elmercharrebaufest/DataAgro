using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BasicoContrato
    {
        public int Id { get; set; }
        public string Cuit { get; set; }
        public int ContratoId { get; set; }
        public int MaterialId { get; set; }
        public int? NivelTarifaId { get; set; }
        public int TipoNegocioId { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public string PrecioPlazo { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public int CampanaId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public string FechaDesdeFormateado { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string FechaHastaFormateado { get; set; }
        public int ProveedorId { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public DateTime? Fecha { get; set; }
        public string FechaFormateado { get; set; }
        public string Hora { get; set; }

        public string NivelTarifa { get; set; }
        public decimal? TarifaFlete { get; set; }
        public int GrupoCompra { get; set; }
        public string GrupoCompraDescripcion { get; set; }
        public int? ComercialId { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }
        public bool? Base { get; set; }
        public decimal? Importe_Sustentable { get; set; }
        public string MonedaId_Sustentable { get; set; }
        public string Moneda_Sustentable { get; set; }
        public DateTime? Fecha_Dolarizado { get; set; }
        public string Fecha_DolarizadoFormateado { get; set; }
        public int? Dias_Pesificado { get; set; }
        public bool? NoInformaSIO { get; set; }
        public bool? TrigoEspecial { get; set; }
        public int? Estado { get; set; }
        public string UsuarioId { get; set; }
        public string ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; }
        public string TipoNegocio { get; set; }
        public string Proveedor { get; set; }
        public string Corredor { get; set; }
        public string Comercial { get; set; }
        public string ComercialCreador { get; set; }
        public string Material { get; set; }
        
        public string Campania { get; set; }
        public string Provincia { get; set; }
        public string Localidad { get; set; }
        public string Estado_Contrato { get; set; }
        public int? Cantidad_F { get; set; }
        public decimal? Precio_F { get; set; }
        public string Proveedor_F { get; set; }
        public string Fecha_F { get; set; }
        public string Material_F { get; set; }
        public string MonedaId_F { get; set; }
        public string Moneda_F { get; set; }
        public int? Ampliaciones_F { get; set; }
        public DateTime Fecha_Order { get; set; }
        public int Estado_Order { get; set; }
        public string Observacion { get; set; }
        public string Observacion_F { get; set; }

        public int? FijacionDePrecioContratoId { get; set; }
        public bool? Sustentable { get; set; }
        public bool? Dolarizado { get; set; }
        public bool? Pesificado { get; set; }
        public string Negocio { get; set; }

        public int? ClasificacionId { get; set; }
        public string ClasificacionDescripcion { get; set; }
        public int? DestinoId { get; set; }
        public string DestinoDescripcion { get; set; }
        public int? CantidadCamiones { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? CondicionFijacion { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public string CalidadDescripcion { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }
        public string BoletoDescripcion { get; set; }
        public string BolsaDescripcion { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public string DesdeFijacionFormateado { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public string HastaFijacionFormateado { get; set; }
        public string CondicionFijacionDescripcion { get; set; }
        public List<DescuentoBonificacionDto> Descuentos { get; set; }
        public List<CalidadDto> Calidades { get; set; }
        public bool? MercsDeposito { get; set; }
        public int CorredorId { get; set; }
        public DatosFijacionDeContratoDto DatosFijacion { get; set; }
        public decimal? PorcentajeComision { get; set; }
        public string ContratoCorredor { get; set; }
        public string ContratoVendedor { get; set; }
        public bool? SelCargoMOA { get; set; }
        public bool? SelCargoVendedor { get; set; }
        public bool? Madre { get; set; }
        public string ContratoMadre { get; set; }
        public string Posicion { get; set; }
        public string TipoFason { get; set; }
        public int TipoFasonId { get; set; }
        public int FasonId { get; set; }
        public string Operador { get; set; }
        public int OperadorId { get; set; }
        public int AgenteId { get; set; }
        public List<AperturaPrecioDto> AperturaPrecios { get; set; }
        public List<PrecioPactadosDto> PreciosPactados { get; set; }
        public decimal? PrecioNeto { get; set; }
        public bool? Pizarra { get; set; }
        public int? StandardCalidadId { get; set; }
        public string StandardDeCalidadDescripcion { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? ZonaId { get; set; }
        public string ZonaDescripcion { get; set; }
        public int? AcuerdoId { get; set; }
        public decimal? ImporteFinanciero { get; set; }
        public decimal? ImporteRedespacho { get; set; }
        public decimal? ImporteComision { get; set; }
        public decimal? ImporteBonificacion { get; set; }
        public decimal? PorcentajeBonificacion { get; set; }     
        public bool? Compensacion { get; set; }
        public int? Acuerdo { get; set; }
        public string Rechazo { get; set; }
        public int? ComercialZonaId { get; set; }
        public string ComercialZonaDescripcion { get; set; }
    }

    public class StoredPorContratoResult
    {
        public List<BasicoContrato> BasicoContratoTraerPorFltro { get; set; }
        public List<BasicoContrato> ReporteContratoContratoTraerPorFltro { get; set; }
    }
}
