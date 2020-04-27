using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;
using System.Collections.Generic;

namespace WebDataAgro.Models
{
    public class ConfiguracionEspacioDinamicoModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CentroRequerido")]
        public int CentroId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_MaterialRequerido")]
        public int MaterialId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime FechaDesde { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime FechaHasta { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CantidadCupoRequerido")]
        public int CantidadCupo { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProveedorRequerido")]
        public int ProveedorId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ComercialRequerido")]
        public int ComercialId { get; set; }
        public Resultado Resultado { get; set; }
        public int? CalidadId { get; set; }
        public List<DiaCupo> Dias { get; set; }
    }
}