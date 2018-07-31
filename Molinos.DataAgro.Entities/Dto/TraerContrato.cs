using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BasicoContrato
    {
        public string Cuit { get; set; }
        public string ContratoId { get; set; }
        public int MaterialId { get; set; }
        public int TipoNegocioId { get; set; }
        public double Cantidad { get; set; }
        public decimal Precio { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public int CampanaId { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public int ProveedorId { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public DateTime? Fecha { get; set; }
        public int GrupoCompra { get; set; }
        public int? ComercialId { get; set; }
        public int? ProvinciaId { get; set; }
        public int? LocalidadId { get; set; }

        public bool? Base { get; set; }
        public decimal? Importe_Sustentable { get; set; }
        public string MonedaId_Sustentable { get; set; }
        public string Moneda_Sustentable { get; set; }
        public DateTime? Fecha_Dolarizado { get; set; }
        public int? Dias_Pesificado { get; set; }
        public bool? NoInformaSIO { get; set; }

        public bool? TrigoEspecial { get; set; }
        public int? Estado { get; set; }
        public string UsuarioId { get; set; }

        public int? ContratoSAP { get; set; }

        public double? Ampliaciones { get; set; }
        public string TipoNegocio { get; set; }

        public string Proveedor { get; set; }

        public string Comercial { get; set; }
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
        public bool Sustentable { get; set; }
        public bool Dolarizado { get; set; }
        public bool Pesificado { get; set; }
        public int? Negocio { get; set; }

        public string Clasificacion { get; set; }
        public int? Destino { get; set; }
        public int? CantidadCamiones { get; set; }
        public bool Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? CondicionFijacion { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public int? StandardDeCalidad { get; set; }
        public int? CalidadEspecial { get; set; }
        public decimal? ValorCalidadEspecial { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }

        public List<DescuentosBonificaciones> Descuentos { get; set; }
    }


    public class StoredPorContratoResult
    {

        public List<BasicoContrato> BasicoContratoTraerPorFltro { get; set; }

        public List<BasicoContrato> ReporteContratoContratoTraerPorFltro { get; set; }
    }


}
