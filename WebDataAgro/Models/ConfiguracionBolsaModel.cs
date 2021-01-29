using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ConfiguracionBolsaModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_DestinoRequerido")]
        public int DestinoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProvinciaRequerida")]
        public int ProvinciaId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_BolsaRequerida")]
        public int BolsaId { get; set; }
        public Resultado Resultado { get; internal set; }
    }
}