using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Resources;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace WebDataAgro.Models
{
    public class HabilitacionBoletoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "TipoNegocioRequerido")]
        public int TipoNegocioId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "DescripcionRequerido")]
        public string Descripcion { get; set; }
        public bool CartaOferta { get; set; }
        public bool Confirma { get; set; }
        public bool BoletoFisico { get; set; }

        public List<TipoNegocioDetalleDto> TipoNegocioDetalles { get; set; }
    }
    
    public class BoletoCompraNetProvinciaModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "BoletoRequerido")]
        public int BoletoCompraNetId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "ProvinciaRequerido")]
        public int ProvinciaId { get; set; }

        public List<BoletoCompraNetProvinciaDto> BoletoCompraNetProvincias { get; set; }
    }

    public class HabilitacionBoletoViewModel
    {
        public List<TipoNegocioDetalleDto> TipoNegocioDetalle { get; set; }
        public List<BoletoCompraNetProvinciaDto> BoletoCompraNetProvincia { get; set; }
        public IEnumerable<SelectListItem> TipoNegocio { get; set; }
        public IEnumerable<SelectListItem> ProvinciaList { get; set; }
        public IEnumerable<SelectListItem> TipoBoleto { get; set; }
    }

}