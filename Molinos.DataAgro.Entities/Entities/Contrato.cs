using Molinos.DataAgro.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Contrato : Negocio
    {
        /*los atributos comentados se pasaron a Negocio para compartirse con ContratoAcuerdo - DAT100-2826*/
        //[JsonConverter(typeof(SinHora))]
        //public DateTime FechaEntrega { get; set; }     
        //public int? ProvinciaId { get; set; }
        //public int? LocalidadId { get; set; }
        //public bool? Base { get; set; }
        //public bool? NoInformaSio { get; set; } 
        //public int ClasificacionId { get; set; }
        //public int? CantidadCamiones { get; set; }
        //public bool? Consignatario { get; set; }
        //public bool? PlanCanje { get; set; }
        //public bool? PagoDirectoVendedor { get; set; }
        //public bool? EstablecimientoPropio { get; set; }
        //public int? BoletoId { get; set; }
        //public int? BolsaId { get; set; }
        //public bool? MercsDeposito { get; set; }
        //public decimal? PorcentajeComision { get; set; }
        //public string ContratoVendedor { get; set; }
        //public string ContratoCorredor { get; set; }
        //public bool? SelCargoVendedor { get; set; }
        //public bool? SelCargoMOA { get; set; }
        //public bool? Madre { get; set; }
        //public string ContratoMadre { get; set; }
        public int? ContratoAcuerdoId { get; set; }
        //public int? ZonaId { get; set; }
        //public bool? Compensacion { get; set; }
        //public int? NivelTarifaId { get;  set; }
        //public decimal? TarifaFlete { get; set; }
        //public bool? EsFason { get; set; }
        //public decimal? PorcentajeDePago { get; set; }
        //public string CaratulaExtension { get; set; }        
        public int? AnulaYReemplazaContratoId { get; set; }
        public string MotivoReemplazo { get; set; }
        public bool? Condicional { get; set; }
        public decimal? CondicionalPrecio { get; set; }
        public string CondicionalMonedaId { get; set; }
        public double? CondicionalCantidad { get; set; }
        [JsonConverter(typeof(SinHora))]
        public DateTime? CondicionalFecha { get; set; }
        public string CondicionalPosicion { get; set; }
        public int? CondicionalContratoId { get; set; }
        //public bool? TarifaAConvenir { get; set; }
        //public bool? Sustentable { get; set; }
        //public decimal? ImporteSustentable { get; set; } // Importe_Sustentable
        //public string MonedaSustentableId { get; set; } // MonedaId_Sustentable 
        //public bool? EPA { get; set; }
        //public int? SustentableTipoDBId { get; set; }
        //public bool? ConDescarga { get; set; }

        //[ForeignKey("SustentableTipoDBId")]
        //public virtual TipoDB SustentableTipoDB { get; set; }

        [ForeignKey("CondicionalMonedaId")]
        public virtual Moneda CondicionalMoneda { get; set; }

        [ForeignKey("CondicionalContratoId")]
        public virtual Contrato CondicionalContrato { get; set; }

        [InverseProperty("CondicionalContrato")]
        public virtual ICollection<Contrato> CondicionalContratos { get; set; }

        [ForeignKey("AnulaYReemplazaContratoId")]
        public virtual Contrato AnulaYReemplazaContrato { get; set; }

        [InverseProperty("AnulaYReemplazaContrato")]
        public virtual ICollection<Contrato> AnulaYReemplazaContratos { get; set; }

        //[ForeignKey("ProvinciaId")]
        //public virtual Provincia Provincia { get; set; }

        //[ForeignKey("LocalidadId")]
        //public virtual Localidad Localidad { get; set; }

        //[ForeignKey("MonedaSustentableId")]
        //public virtual Moneda MonedaSustentable { get; set; } // MonedaId_Sustentable

        //[ForeignKey("ClasificacionId")]
        //public virtual ClasificacionCompraNet Clasificacion { get; set; }//ClasificacionId

        //[ForeignKey("BoletoId")]
        //public virtual BoletoCompraNet Boleto { get; set; }

        //[ForeignKey("BolsaId")]
        //public virtual BolsaCompraNet Bolsa { get; set; }

        [ForeignKey("ContratoAcuerdoId")]
        public virtual ContratoAcuerdo ContratoAcuerdo { get; set; }

        [InverseProperty("Contrato")]
        public virtual ICollection<Calidad> Calidad { get; set; }

        [InverseProperty("Contrato")]
        public virtual ICollection<PrecioPactado> PrecioPactado { get; set; }

        //[ForeignKey("ZonaId")]
        //public virtual Zona Zona { get; set; }

        //[ForeignKey("NivelTarifaId")]
        //public virtual NivelTarifa NivelTarifa { get; set; }

        public Contrato() : base()
        {
            Base = false;
            ImporteSustentable = 0;
            NoInformaSio = false;
            CantidadCamiones = 0;
        }
    }
}