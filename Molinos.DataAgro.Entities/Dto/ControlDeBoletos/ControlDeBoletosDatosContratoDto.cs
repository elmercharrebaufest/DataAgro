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
        public int? BolsaId { get; set; }
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
        public string Corredor { get; set; }
        public string CuitCorredor { get; set; }
        public string Moneda { get; set; }
        public int? CorredorId { get; set; }
        public bool? PlanCanje { get; set; }
        public bool? OperaSinOblea { get; set; }
        public bool? EsCartaOferta { get; set; }
        public bool? EsSinBoleto { get; set; }
        public int? BoletoCompraNetId { get; set; }
        public string Proveedor { get; set; }
        public string TipoNegocio { get; set; }
        public string Comercial { get; set; }
        public string PorcentajePago { get; set; }
        public decimal? PrecioNeto { get; set; }
        public string Bolsa { get; set; }
        public string MercaderaDeposito { get; set; }
        public double? CantidadDeposito { get; set; }
        public double CantidadCamiones { get; set; }
        public string FechaHastaOriginal { get; set; }
        public double CantidadFijacionMinima { get; set; }
        public double CantidadFijacionMaxima { get; set; }
        public string FechaDolarizadoOriginal { get; set; }
        public string MonedaRedespacho { get; set; }
        public decimal? ImporteRedespacho { get; set; }
        public string Consignatario { get; set; }
        public int? VersionBoleto { get; set; }
        public string FechaGeneracion { get; set; }
    }
}
