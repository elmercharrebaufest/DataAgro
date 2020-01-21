using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Molinos.DataAgro.Entities.Resources;

namespace WebDataAgro.Models
{
    public class ConfiguracionInternaModel
    {
        public int Id { get; set; }
        public decimal Precio { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public string DesdeVigencia { get; set; }
        public string HastaVigencia { get; set; }
        public string DiaPizarra { get; set; }
        public string PizarraDesde { get; set; }
        public string PizarraHasta { get; set; }
        public int MaterialPizarra { get; set; }
        public string FijacionDia { get; set; }
        public int MaterialFijacionId { get; set; }
        public Resultado ResultadoPrecio { get; set; }
        public Resultado ResultadoPizarra { get; set; }
        public Resultado ResultadoFijacion { get; set; }
        public List<PrecioMoaDto> PrecioMoa { get; set; }
        public List<HabilitacionPizarraDto> HabilitacionPizarra { get; set; }
        public List<HabilitacionFijacionDto> HabilitacionFijacion { get; set; }
        public ConfiguracionInternaModel()
        {
            PrecioMoa = new List<PrecioMoaDto>();
            HabilitacionFijacion = new List<HabilitacionFijacionDto>();
            HabilitacionPizarra = new List<HabilitacionPizarraDto>();
        }
    }
}