using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PesificarAgentDto
    {
        public string Material { get; set; }
        public string Contrato { get; set; }
        public int? CantidadPendiente { get; set; }
        public string Comercial { get; set; }
        public string Fijacion { get; set; }
        public DateTime? FechaFijacion { get; set; }
        public DateTime? FechaHastaDolarizado { get; set; }
        public DateTime? FechaUltimaAplicacion { get; set; }
        public bool DolarizadoNoProductor { get; set; }
        public string CuitCorredor { get; set; }
        public string CuitVendedor { get; set; }
        public bool DolarizadoExpress { get; set; }
        public string Moneda { get; set; }
        public decimal KgNoPesificable { get; set; }
        public decimal KgVencimientoPesificable { get; set; }
        public decimal Precio { get; set; }
        public string NombreCorredor { get; set; }
        public string NombreVendedor { get; set; }
        public string Unidad { get; set; }
        public bool Dolarizado { get; set; }
        public string Clasificacion { get; set; }
        public string Anticipo { get; set; }
    }

}


