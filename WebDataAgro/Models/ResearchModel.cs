using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebDataAgro.Models
{
    public class ResearchAvanceSiembraModel
    {
        public List<ResearchAvanceSiembraModel> AvanceSiembra { get; set; }
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadNombre { get; set; }
        public int ComercialId { get; set; }
        public decimal IntencionSiembra { get; set; }
        public decimal Avance { get; set; }
        public decimal CambioAA { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }
        public Resultado Resultado { get; set; }
        public List<ResearchAvanceSiembraDto> HistorialAvanceSiembra { get; set; }
        
    }

    public class ResearchAvanceCosechaModel
    {
        public List<ResearchAvanceCosechaModel> AvanceCosecha { get; set; }
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadNombre { get; set; }
        public int ComercialId { get; set; }
        public decimal RangoDesde { get; set; }
        public decimal RangoHasta { get; set; }
        public decimal Avance { get; set; }
        public decimal Rendimiento { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }
        public Resultado Resultado { get; set; }
        public List<ResearchAvanceCosechaDto> HistorialAvanceCosecha { get; set; }

    }

    public class ResearchSituacionCultivoModel
    {
        public List<ResearchSituacionCultivoModel> SituacionCultivo { get; set; }
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadNombre { get; set; }
        public int ComercialId { get; set; }
        public string Estadio { get; set; }
        public string Situacion { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }
        public Resultado Resultado { get; set; }
        public List<ResearchSituacionCultivoDto> HistorialSituacionCultivo { get; set; }

    }

    public class ResearchVentaStockModel
    {
        public List<ResearchVentaStockModel> VentaStock { get; set; }
        public int Id { get; set; }
        public int MaterialId { get; set; }
        public string MaterialDescripcion { get; set; }
        public int LocalidadId { get; set; }
        public string LocalidadNombre { get; set; }
        public int ComercialId { get; set; }
        public decimal VendidoAPrecio { get; set; }
        public decimal Almacenado { get; set; }
        public string Observaciones { get; set; }
        public DateTime FechaHora { get; set; }
        public Resultado Resultado { get; set; }
        public List<ResearchVentaStockDto> HistorialVentaStock { get; set; }

    }
}