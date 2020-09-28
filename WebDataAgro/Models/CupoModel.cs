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
        public int Id { get; set; }
        public string Siguientes { get; set; }
        public string ProveedorDescripcion { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProveedorRequerido")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ProveedorRequerido")]
        public int Proveedor { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_PlantaRequerido")]
        public string PlantaId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_MaterialRequerido")]
        public int MaterialId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaEntregaRequerido")]
        public DateTime FechaEntrega { get; set; }
        public DateTime FechaHastaEntrega { get; set; }
        public int? CantidadCupos { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ZonaRequerido")]
        [Range(1, 999.99, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ZonaRequerido")]

        //public int Zona { get; set; }
        public int ZonaId { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FleteRequerido")]
        public bool FleteAcarreo { get; set; }
        public int? CalidadId { get; set; }
        public string Observacion { get; set; }
        public bool FasonId { get; set; }
        public string CuitId { get; set; }
        public List<DiaCupo> Dias { get; set; }
        public CupoResult Resultado { get; set; }

        public int? Negocio { get; set; }       
        public int? NegocioId { get; set; }

    }
    
}