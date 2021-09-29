using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public class ReportePesificado
    {
        public virtual int Id { get; set; }
        public virtual int? MaterialId { get; set; }
        public virtual string Contrato { get; set; }
        public virtual int? CantidadPendiente { get; set; }
        public virtual int? ComercialId { get; set; }
        public virtual string Fijacion { get; set; }
        public virtual DateTime? FechaFijacion { get; set; }
        public virtual DateTime? FechaHastaDolarizado { get; set; }
        public virtual DateTime? FechaUltimaAplicacion { get; set; }
        public virtual bool? DolarizadoNoProductor { get; set; }
        public virtual string CuitCorredor { get; set; }
        public virtual string CuitVendedor { get; set; }
        public virtual bool DolarizadoExpress { get; set; }
        public virtual string MonedaId { get; set; }
        public virtual decimal? KgNoPesificable { get; set; }
        public virtual decimal? KgVencimientoPesificable { get; set; }
        public virtual decimal? Precio { get; set; }
        public virtual string NombreCorredor { get; set; }
        public virtual string NombreVendedor { get; set; }
        public virtual string Unidad { get; set; }
        public virtual bool? Dolarizado { get; set; }
        public virtual string Clasificacion { get; set; }
        public virtual decimal KgTotales { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; } // MonedaId (length: 5)
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; } // ComercialId
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; } // MaterialId
        public bool Pase { get; set; }
        public decimal? Plus { get; set; }
        public string Posicion { get; set; }
        public double? KgTotalesPase { get; set; }
    }

}


