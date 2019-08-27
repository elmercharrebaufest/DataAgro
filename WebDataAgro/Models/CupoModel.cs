using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using Molinos.DataAgro.Entities.Resources;
using System.ComponentModel.DataAnnotations;

namespace WebDataAgro.Models
{
    public class CupoModel
    {
        public string ProveedorDescripcion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProveedorRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProveedorRequerido")]
        public int Proveedor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_PlantaRequerido")]
        public int Planta { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_MaterialRequerido")]
        public int Material { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaEntregaRequerido")]
        public DateTime FechaEntrega { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CantidadCupoRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_CantidadCupoRequerido")]
        public int CantidadCupos {get;set;}
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ZonaRequerido")]
        //public int Zona { get; set; }
        public int ZonaId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FleteRequerido")]
        public bool FleteAcarreo { get; set; }        
        public int? Calidad { get; set; }
        public string Observacion { get; set; }
        public bool Fason { get; set; }
        public string CUIT { get; set; }
        public Resultado Resultado { get; set; }
    }

}