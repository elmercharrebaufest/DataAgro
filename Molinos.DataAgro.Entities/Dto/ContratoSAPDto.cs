using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ContratoSAPDto
    {
        public string ContratoSAP { get; set; }
        public decimal Cantidad { get; set; }
        public string Cosecha { get; set; }
        public int DiasDiferimiento { get; set; }
        public string FechaDesde { get; set; }
        public string FechaEntrega { get; set; }
        public string FechaHasta { get; set; }
        public string FechaLimite { get; set; }
        public string FechaOperacion { get; set; }
        public string GrupoCompras { get; set; }
        public string Moneda { get; set; }
        public string NoInformaSio { get; set; }
        public string PagoDiferido { get; set; }
        public string Material { get; set; }
        public string PagoDifArp { get; set; }
        public decimal Precio { get; set; }
        public string Proveedor { get; set; }
        public int Provincia { get; set; }
        public string Sustentable { get; set; }
        public string Especial { get; set; }
        public string Procedencia { get; set; }
        public string Centro { get; set; }
        public string Clasificacion { get; set; }
        public string IndOpCanje { get; set; }
        public string Consignatario { get; set; }
        public string CondFijacion { get; set; }
        public int Camiones { get; set; }
        public string Confirma { get; set; }
        public string Bolsa { get; set; }
        public string BolFisico { get; set; }
        public string CartaOferta { get; set; }
        public string Ninguno { get; set; }
        public string AutCg { get; set; }
        public string AurCd { get; set; }
        public string PagoDirVend { get; set; }
        public string EstabPropio { get; set; }
        public string EstabArrendado { get; set; }
        public string ImporteSPrecio { get; set; }
        public string MonedaSPrecio { get; set; }
        public string PorcSPrecio { get; set; }
        public string ImporteAPrecio { get; set; }
        public string MonedaAPrecio { get; set; }
        public string PorcAPrecio { get; set; }
        public string MercDescargada { get; set; }
        public string ObservacionCal1 { get; set; }
        public string CuitCorredor { get; set; }
        public decimal PorcComision { get; set; }
        public string ContrCorr { get; set; }
        public string ContrVend { get; set; }
        public string SelCargoMOA { get; set; }
        public string SelCargoVend { get; set; }
        public string ContratoMadre { get; set; }
        public string Zona { get; set; }
        public string Compensacion { get; set; }
        public string FleteNivel { get; set; }
        public decimal FleteTarifa { get; set; }
        public string FeDesdeFij { get; set; }
        public string FeHastaFij { get; set; }
        public string ContratoCorredor { get; set; }
        public string TipoNegocio { get; set; }

        public string Comercial { get; set; }

        public decimal Monto { get; set; }

        public List<CalidadSAP> Calidad { get; set; }
        public List<DescuentoBonificacionSap> DescuentoBonificaciones { get; set; }
        public List<AperturaPrecioSap> Apertura { get; set; }
        public decimal? PrecioNeto { get; set; }
        public decimal? PorcentajeDePago { get; set; }
        public string TipoAgenteCompraId { get; set; }
        public string CaratulaMAT { get; set; }
        public string CaratulaExtension { get; set; }
        public decimal? PrecioAjusteComision { get; set; }
        public string MonedaAjusteComisionId { get; set; }
        public string DolarizadoExpress { get; set; }
        public string ZLSCH { get; set; }
        public string CUENTA_MRP { get; set; }
        public string ComercialCreador { get; set; }
        public string FechaCreacion { get; set; }
        public string HORAACT { get; set; }
        public string FechaCierta { get; set; }
        public string Canje { get; set; }
        public string Insumo { get; set; }
        public string MonedaCanjeId { get; set; }
    }

    public class CalidadSAP
    {
        public string Codigo { get; set; }
        public decimal Valor { get; set; }
        public decimal PorcentajeDesde { get; set; }
        public decimal PorcentajeHasta { get; set; }
    }
    public class DescuentoBonificacionSap
    {
        public string TipoPeriodo { get; set; }
        public string TipoDescBon { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal Importe { get; set; }
        public string MonedaDB { get; set; }
        public decimal PorcentajeDB { get; set; }
        public decimal Precio { get; set; }
        public string Moneda { get; set; }
    }

    public class AperturaPrecioSap
    {
        public string Concepto { get; set; }
        public decimal Importe { get; set; }
        public string Moneda { get; set; }
        public decimal Porcentaje { get; set; }
    }
}



