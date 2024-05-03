using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class PrecioPizarraModel
    {
        public List<PrecioPizarraModel> Precios { get; set; }
        public List<PrecioPizarraDto> HistorialPrecioPizarra { get; set; }
        public int Id { get; set; }
        public int Precio { get; set; }
        public int MaterialId { get; set; }
        public string MonedaId { get; set; }
        public int PizarraId { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public string Moneda { get; set; }
        public string UnidadMedida { get; set; }
        public string Material { get; set; }
        public string Pizarra { get; set; }
        public Resultado Resultado { get; set; }
        public string FechaActualizaPrecio { get; set; }
    }
}