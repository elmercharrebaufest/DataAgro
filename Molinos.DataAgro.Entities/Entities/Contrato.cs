using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Contrato : Negocio
    {
        [JsonConverter(typeof(SinHora))]
        public DateTime FechaEntrega { get; set; } // FechaEntrega       
        public int? ProvinciaId { get; set; } // ProvinciaId
        public int? LocalidadId { get; set; } // LocalidadId 
        public bool? Base { get; set; } // Base
        public decimal? ImporteSustentable { get; set; } // Importe_Sustentable
        public string MonedaSustentableId { get; set; } // MonedaId_Sustentable 
        public bool? NoInformaSio { get; set; } // NoInformaSIO
        public int ClasificacionId { get; set; }//ClasificacionId
        public int? CantidadCamiones { get; set; }
        public bool? Consignatario { get; set; }
        public bool? PlanCanje { get; set; }
        public bool? PagoDirectoVendedor { get; set; }
        public bool? EstablecimientoPropio { get; set; }
        public int? BoletoId { get; set; }
        public int? BolsaId { get; set; }

        public bool? MercsDeposito { get; set; }
        public decimal? PorcentajeComision { get; set; }
        public string ContratoVendedor { get; set; }
        public string ContratoCorredor { get; set; }
        public bool? SelCargoVendedor { get; set; }
        public bool? SelCargoMOA { get; set; }
        public bool? Madre { get; set; }
        public string ContratoMadre { get; set; }
        public int? ContratoAcuerdoId { get; set; }
        public int? ZonaId { get; set; }
        public bool? Compensacion { get; set; }
        public int? NivelTarifaId { get;  set; }
        public decimal? TarifaFlete { get; set; }
        public bool? Sustentable { get; set; }
        public bool? EsFason { get; set; }
        public decimal? PorcentajeDePago { get; set; }
        public string CaratulaExtension { get; set; }        
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

        [ForeignKey("ProvinciaId")]
        public virtual Provincia Provincia { get; set; } // ProvinciaId
        [ForeignKey("LocalidadId")]
        public virtual Localidad Localidad { get; set; } // LocalidadId
        [ForeignKey("MonedaSustentableId")]
        public virtual Moneda MonedaSustentable { get; set; } // MonedaId_Sustentable
        [ForeignKey("ClasificacionId")]
        public virtual ClasificacionCompraNet Clasificacion { get; set; }//ClasificacionId

        [ForeignKey("BoletoId")]
        public virtual BoletoCompraNet Boleto { get; set; }
        [ForeignKey("BolsaId")]
        public virtual BolsaCompraNet Bolsa { get; set; }
        [ForeignKey("ContratoAcuerdoId")]
        public virtual ContratoAcuerdo ContratoAcuerdo { get; set; }
        [InverseProperty("Contrato")]
        public virtual ICollection<Calidad> Calidad { get; set; } 
        [InverseProperty("Contrato")]
        public virtual ICollection<PrecioPactado> PrecioPactado { get; set; }     

        [ForeignKey("ZonaId")]
        public virtual Zona Zona { get; set; }

        [ForeignKey("NivelTarifaId")]
        public virtual NivelTarifa NivelTarifa { get; set; }

        public Contrato() : base()
        {
            Base = false;
            ImporteSustentable = 0;
            NoInformaSio = false;
            CantidadCamiones = 0;
        }
    }
}



