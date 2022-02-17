using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;
using System.Collections.Generic;

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
}