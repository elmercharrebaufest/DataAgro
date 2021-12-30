using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Negocio
    {
        [Key]
        public int Id { get; set; }
        public int MaterialId { get; set; } // MaterialId
        public int TipoNegocioId { get; set; } // TipoNegocioId
        public double Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio
        public int? CampanaId { get; set; } // CampañaId
        [JsonConverter(typeof(SinHora))]
        public DateTime FechaDesde { get; set; } // FechaDesde
        [JsonConverter(typeof(SinHora))]
        public DateTime FechaHasta { get; set; } // FechaHasta
        public int? ProveedorId { get; set; } // ProveedorId
        public string MonedaId { get; set; } // MonedaId (length: 5)
        [JsonConverter(typeof(SinHora))]
        public DateTime Fecha { get; set; } // Fecha
        public int? GrupoCompra { get; set; } // GrupoCompra
        public int? ComercialId { get; set; } // ComercialId
        public int? DiasPesificado { get; set; } // Dias_Pesificado
        public bool? TrigoEspecial { get; set; } // TrigoEspecial
        public int EstadoId { get; set; } // Estado (length: 50)
        public string UsuarioId { get; set; } // UsuarioId (length: 100)
        public string ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; } // Cantidad
        public string Observacion { get; set; }
        public int? DestinoId { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int? CorredorId { get; set; }
        public int? FinDelDiaId { get; set; }
        public bool? Pizarra { get; set; }
        public decimal? PrecioNeto { get; set; }
        public bool? PagoDiferido { get; set; }
        public int? StandardDeCalidadId { get; set; }
        public string Posicion { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        [JsonConverter(typeof(SinHora))]
        public DateTime? FechaDolarizado { get; set; } // Fecha_Dolarizado
        public bool? Dolarizado { get; set; }
        public bool OcultarEnTablero { get; set; }
        public double? CantidadAmpliado { get; set; }
        public int? CreditoDisponible { get; set; }
        public string MonedaCreditoDisponible { get; set; }

        [JsonConverter(typeof(SinHora))]
        public DateTime? DesdeFijacion { get; set; }
        [JsonConverter(typeof(SinHora))]
        public DateTime? HastaFijacion { get; set; }
        public int? CondicionFijacionId { get; set; }
        public string MotivoRechazo { get; set; }
        public int? TipoAgenteCompraId { get; set; }
        [JsonConverter(typeof(SinHora))]
        public DateTime? FechaConfirmacion { get; set; }
        public int? UsuarioConfirmadorId { get; set; }
        [JsonConverter(typeof(SinHora))]
        public DateTime FechaOperacion { get; set; } // FechaOperacion

        [JsonConverter(typeof(SinHora))]
        public DateTime? FechaCierta { get; set; }
        public bool? ChequeElectronico { get; set; }
        public bool? DolarizadoExpress { get; set; }
        public string PagoCBU { get; set; }
        public string MotivoOperacionAnterior { get; set; }

        public string ObservacionTercero { get; set; }
        public bool? CalidadTercero { get; set; }
        public bool? PagoDiferidoTercero { get; set; }
        public int? PagoDiferidoTerceroId { get; set; }
        public bool? DolarizadoTercero { get; set; }

        public bool? Canje { get; set; }

        public int? ProveedorCreadorId { get; set; }
        public decimal? Monto { get; set; }
        public string Insumo { get; set; }
        public string MonedaCanjeId { get; set; }

        public bool? PrestamoDevolucion { get; set; }
        public int? PlantaDestinoId { get; set; }
        public bool? DolarizadoCorredor { get; set; }
        public bool? SustentableTercero { get; set; }
        public bool? Venta { get; set; }

        public bool? ObligatoriedadCostoFinanciero { get; set; }

        public string UsuarioTercero { get; set; }

        public bool? Virtual { get; set; }

        [JsonConverter(typeof(SinHora))]
        public DateTime? FechaDesdeSustentable { get; set; } // FechaDesdeSustentable       
        [JsonConverter(typeof(SinHora))]
        public DateTime? FechaHastaSustentable { get; set; } // FechaHastaSustentable   
        public string PosicionCBOT { get; set; }
        public int? TipoPosicionCBOTId { get; set; }

        public int? CamaraId { get; set; }
        public int? ComisionAFavorId { get; set; }
        public decimal? PorcentajeComisionVenta { get; set; }

        public string FleteACargo { get; set; }
        public string KgBalanza { get; set; }
        public string Pago { get; set; }
        public int? ProcedenciaVentaId { get; set; }

        public int? CondicionDePagoDiaFijacion { get; set; }
        public int? CondicionDePagoDiaPesificado { get; set; }
        public string CondicionDePagoTipoFijacion { get; set; }
        public string CondicionDePagoTipoPesificado { get; set; }
        public int? CondicionDePagoFijacionVentaId { get; set; }
        public int? CondicionDePagoPesificadoVentaId { get; set; }
        public bool? Cesion { get; set; }



        public string CaratulaMAT { get; set; }
        public decimal? PrecioAjusteComision { get; set; }
        public string MonedaAjusteComisionId { get; set; }
        public bool? ObligatoriedadBonificacion { get; set; }

        public decimal? PrecioPonderado { get; set; } // Precio
        public decimal? PrecioNetoPonderado { get; set; } // PrecioNeto

        public int? BoletoVentaId { get; set; }
        public string MailVentaBoleto { get; set; }

        public int? KgMinimo { get; set; }
        public int? KgMaximo { get; set; }

        
        public string DescripcionOperacionAnterior { get; set; }

        public int? ProveedorComisionistaId { get; set; }

        //public DateTime FechaCarga { get; set; } // Fecha
        [ForeignKey("TipoAgenteCompraId")]
        public virtual TipoAgenteCompra TipoAgenteCompra { get; set; }
        [ForeignKey("CondicionFijacionId")]
        public virtual CondicionFijacion CondicionFijacion { get; set; }
        [ForeignKey("EstadoId")]
        public virtual EstadoContrato Estado { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; } // MaterialId
        [ForeignKey("TipoNegocioId")]
        public virtual TipoNegocio TipoNegocio { get; set; } // TipoNegocioId
        [ForeignKey("CampanaId")]
        public virtual Campaña Campana { get; set; } // CampañaId
        [ForeignKey("ProveedorId")]
        public virtual Proveedor Proveedor { get; set; } // ProveedorId
        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; } // MonedaId (length: 5)
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
        [ForeignKey("UsuarioConfirmadorId")]
        public virtual Comercial ComercialConfirmador { get; set; } // ComercialId
        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }
        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial ComercialCreador { get; set; }
        [ForeignKey("ProveedorCreadorId")]
        public virtual Proveedor ProveedorCreador { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }
        [ForeignKey("FinDelDiaId")]
        public virtual FinDelDia FinDelDia { get; set; }
        [ForeignKey("GrupoCompra")]
        public virtual GrupoDeCompras GrupoDeCompras { get; set; }
        [ForeignKey("StandardDeCalidadId")]
        public virtual StandardDeCalidad StandardDeCalidad { get; set; }

        [InverseProperty("Negocio")]
        public virtual List<AperturaPrecio> AperturaPrecio { get; set; }

        [InverseProperty("Negocio")]
        public virtual List<NegocioHistorico> NegocioHistorico { get; set; } = new List<NegocioHistorico>();

        [ForeignKey("MonedaCanjeId")]
        public virtual Moneda MonedaCanje { get; set; }

        [ForeignKey("PlantaDestinoId")]
        public virtual Centro PlantaDestino { get; set; }

        public virtual ICollection<DescuentoBonificacion> Descuentos { get; set; }

        [ForeignKey("TipoPosicionCBOTId")]
        public virtual TipoPosicionCBOT TipoPosicionCBOT { get; set; }
        [InverseProperty("FijacionCanje")]
        public virtual List<FijacionVirtualSap> FijacionCanje { get; set; }
        [InverseProperty("FijacionVirtual")]
        public virtual List<FijacionVirtualSap> FijacionVirtual { get; set; }

        [ForeignKey("CamaraId")]
        public virtual Camara Camara { get; set; }
        [ForeignKey("ComisionAFavorId")]
        public virtual ComisionAFavor ComisionAFavor { get; set; }
        [ForeignKey("ProcedenciaVentaId")]
        public virtual Localidad ProcedenciaVenta { get; set; }
        [ForeignKey("CondicionDePagoFijacionVentaId")]
        public virtual CondicionDePagoVenta CondicionDePagoFijacionVenta { get; set; }
        [ForeignKey("CondicionDePagoPesificadoVentaId")]
        public virtual CondicionDePagoVenta CondicionDePagoPesificadoVenta { get; set; }
        [ForeignKey("BoletoVentaId")]
        public virtual BoletoVenta BoletoVenta { get; set; }

        [ForeignKey("ProveedorComisionistaId")]
        public virtual Proveedor ProveedorComisionista { get; set; } // ProveedorId
        public Negocio()
        {
            Cantidad = 0;
            Precio = 0;
            TrigoEspecial = false;
            EstadoId = (int)EnumEstadoContrato.Pendiente;
            ContratoSAP = "";
            Ampliaciones = 0;
            Pizarra = false;
        }
    }
}



