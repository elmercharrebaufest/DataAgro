using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReportePesificadoDto
    {
       public string MaterialDesc { get; set; }
        public string Contrato { get; set; }
        public int? CantidadPendiente { get; set; }
        public string ComercialDesc { get; set; }
        public string Fijacion { get; set; }
        public DateTime? FechaFijacion { get; set; }
        public DateTime? FechaHastaDolarizado { get; set; }
        public DateTime? FechaUltimaAplicacion { get; set; }
        public bool? DolarizadoNoProductor { get; set; }
        public string CuitCorredor { get; set; }
        public string CuitVendedor { get; set; }
        public bool? DolarizadoExpress { get; set; }
        public string MonedaId { get; set; }
        public decimal? KgNoPesificable { get; set; }
        public decimal? KgVencimientoPesificable { get; set; }
        public decimal? Precio { get; set; }
        public string NombreCorredor { get; set; }
        public string NombreVendedor { get; set; }
        public string Unidad { get; set; }
        public bool? Dolarizado { get; set; }
        public string Clasificacion { get; set; }
        public decimal? KgTotales { get; set; }
        public int? Id { get; set; }
        public int? MaterialId { get; set; }
        public int? ComercialId { get; set; }
        public bool NingunDolarizado { get; set; }
        public decimal? USDPesificable { get; set; }
        public decimal? USDNoPesificable { get; set; }
        public decimal? USDTotal { get; set; }
        public decimal? USDTotalizador { get; set; }
        public bool Pase { get; set; }
        public double? KgTotalesPase { get; set; }
        public decimal? Plus { get; set; }
        public string Posicion { get; set; }
    }

}


