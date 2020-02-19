using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;

namespace WebDataAgro.Models
{
    public class ConfiguracionCupoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CentroRequerido")]
        public int CentroId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_MaterialRequerido")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime Fecha { get; set; }
        //[Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CantidadCupoRequerido")]
        public int CantidadCupo { get; set; }
        public Resultado Resultado { get; set; }
    }
}