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
        public int CantidadDia { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public string DesdeVigencia { get; set; }
        public string HastaVigencia { get; set; }
        public string DiaPizarra { get; set; }
        public string PizarraDesde { get; set; }
        public string PizarraHasta { get; set; }
        public int MaterialPizarra { get; set; }
        public string FijacionDia { get; set; }
        public int TipoNegocioId { get; set; }
        public DateTime? DesdeEntrega { get; set; }
        public DateTime? HastaEntrega { get; set; }
        public DateTime? DesdeFijacion { get; set; }
        public DateTime? HastaFijacion { get; set; }
        public int MaterialFijacionId { get; set; }
        public int TipoNegocioIdPizarra { get; set; }
        public DateTime? DesdeEntregaPizarra { get; set; }
        public DateTime? HastaEntregaPizarra { get; set; }
        public int MaterialCampañaId { get; set; }
        public int CampañaId { get; set; }

        public Resultado ResultadoPrecio { get; set; }
        public Resultado ResultadoPizarra { get; set; }
        public Resultado ResultadoFijacion { get; set; }
        public Resultado ResultadoCampaña { get; set; }
        public Resultado ResultadoPago { get; set; }
        public List<PrecioMoaDto> PrecioMoa { get; set; }
        public List<HabilitacionPizarraDto> HabilitacionPizarra { get; set; }
        public List<HabilitacionFijacionDto> HabilitacionFijacion { get; set; }
        public List<HabilitacionCampañaDto> HabilitacionCampaña { get; set; }
        public List<HabilitacionPagoDiferidoDto> HabilitacionPagoDiferido { get; set; }
        public ConfiguracionInternaModel()
        {
            PrecioMoa = new List<PrecioMoaDto>();
            HabilitacionFijacion = new List<HabilitacionFijacionDto>();
            HabilitacionPizarra = new List<HabilitacionPizarraDto>();
            HabilitacionCampaña = new List<HabilitacionCampañaDto>();
            HabilitacionPagoDiferido = new List<HabilitacionPagoDiferidoDto>();
        }
    }
}