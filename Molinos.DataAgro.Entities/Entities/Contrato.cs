using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Contrato
    {
        [Key]
        public int ContratoId { get; set; } // ContratoId (Primary key)
        public int MaterialId { get; set; } // MaterialId
        public int TipoNegocioId { get; set; } // TipoNegocioId
        public double Cantidad { get; set; } // Cantidad
        public decimal Precio { get; set; } // Precio
        public DateTime FechaEntrega { get; set; } // FechaEntrega
        public int CampanaId { get; set; } // CampañaId
        public DateTime FechaDesde { get; set; } // FechaDesde
        public DateTime FechaHasta { get; set; } // FechaHasta
        public int ProveedorId { get; set; } // ProveedorId
        public string MonedaId { get; set; } // MonedaId (length: 5) 
        public DateTime Fecha { get; set; } // Fecha
        public int GrupoCompra { get; set; } // GrupoCompra
        public int? ComercialId { get; set; } // ComercialId
        public int? ProvinciaId { get; set; } // ProvinciaId
        public int? LocalidadId { get; set; } // LocalidadId 
        public bool? Base { get; set; } // Base
        public decimal? ImporteSustentable { get; set; } // Importe_Sustentable
        public string MonedaSustentableId { get; set; } // MonedaId_Sustentable 
        public DateTime? FechaDolarizado { get; set; } // Fecha_Dolarizado
        public int? DiasPesificado { get; set; } // Dias_Pesificado
        public bool? NoInformaSio { get; set; } // NoInformaSIO
        public bool? TrigoEspecial { get; set; } // TrigoEspecial
        public int EstadoId { get; set; } // Estado (length: 50)
        public string UsuarioId { get; set; } // UsuarioId (length: 100)
        public string ContratoSAP { get; set; }
        public double? Ampliaciones { get; set; } // Cantidad
        public string Observacion { get; set; }
        public int ClasificacionId { get; set; }//ClasificacionId
        public int? DestinoId { get; set; }
        public int? CantidadCamiones { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public int? CondicionFijacionId { get; set; }
        public bool? CD { get; set; }
        public bool? Warrant { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public bool? MercsDeposito { get; set; }
        public int? ComercialCreadorId { get; set; }
        public int? CorredorId { get; set; }
        public decimal? PorcentajeComision { get; set; }
        public string ContratoVendedor { get; set; }
        public string ContratoCorredor { get; set; }
        public bool? SelCargoVendedor { get; set; }
        public bool? SelCargoMOA { get; set; }
        public bool? Madre { get; set; }
        public string ContratoMadre { get; set; }
        public int? FinDelDiaId { get; set; }
        public int? ContratoAcuerdoId { get; set; }
        public bool? Pizarra { get; set; }
        public decimal? PrecioNeto { get; set; }

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
        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; } // ProvinciaId
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; } // LocalidadId
        [ForeignKey("MonedaSustentableId")]
        public virtual Moneda MonedaSustentable { get; set; } // MonedaId_Sustentable
        [ForeignKey("ClasificacionId")]
        public virtual ClasificacionCompraNet Clasificacion { get; set; }//ClasificacionId
        [ForeignKey("DestinoId")]
        public virtual Centro Destino { get; set; }
        [ForeignKey("CondicionFijacionId")]
        public virtual CondicionFijacion CondicionFijacion { get; set; }
        [ForeignKey("BoletoId")]
        public virtual BoletoCompraNet Boleto { get; set; }
        [ForeignKey("BolsaId")]
        public virtual BolsaCompraNet Bolsa { get; set; }
        [ForeignKey("ComercialCreadorId")]
        public virtual Comercial ComercialCreador { get; set; }
        [ForeignKey("CorredorId")]
        public virtual Proveedor Corredor { get; set; }
        [ForeignKey("FinDelDiaId")]
        public virtual FinDelDia FinDelDia { get; set; }

        [InverseProperty("Contrato")]
        public ICollection<DescuentoBonificacion> Descuentos { get; set; }
        [InverseProperty("Contrato")]
        public ICollection<Calidad> Calidad { get; set; }

        [ForeignKey("GrupoCompra")]
        public virtual GrupoDeCompras GrupoDeCompras { get; set; }

        [ForeignKey("ContratoAcuerdoId")]
        public virtual ContratoAcuerdo ContratoAcuerdo { get; set; }

        [InverseProperty("Contrato")]
        public List<AperturaPrecio> AperturaPrecio { get; set; }

        public Contrato()
        {
            Cantidad = 0;
            Precio = 0;
            Base = false;
            ImporteSustentable = 0;
            NoInformaSio = false;
            TrigoEspecial = false;
            EstadoId = (int)EnumEstadoContrato.Pendiente;
            ContratoSAP = "";
            Ampliaciones = 0;
            CantidadCamiones = 0;
            Pizarra = false;
        }
    }
}



