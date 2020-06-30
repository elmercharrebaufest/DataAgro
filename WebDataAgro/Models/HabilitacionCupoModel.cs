using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;

namespace WebDataAgro.Models
{
    public class HabilitacionCupoModel
    {
        public int Id { get; set; }
        public int? ZonaCupoId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_MaterialRequerido")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime FechaDesde { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime FechaHasta { get; set; }
        public Resultado Resultado { get; set; }
    }
}