using Molinos.DataAgro.Entities.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Molinos.DataAgro.Entities.Dto
{
    public class PrecioMoaDto
    {
        public int Id { get; set; }
        public decimal? Precio { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime DesdeVigencia { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime HastaVigencia { get; set; }
        public string TipoNegocio { get; set; }
        public int TipoNegocioId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime? HastaEntrega { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime? DesdeEntrega { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime? DesdeFijacion { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy hh\\:mm tt}")]
        public DateTime? HastaFijacion { get; set; }
        public string TipoConfiguracion { get; set; }
        public string UsuarioCreador { get; set; }
        public DateTime? FechaCreacion { get; set; }
    }
}
