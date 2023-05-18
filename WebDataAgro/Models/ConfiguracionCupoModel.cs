using Molinos.DataAgro.Entities.Dto;
using System;
using System.ComponentModel.DataAnnotations;
using Molinos.DataAgro.Entities.Resources;
using System.Collections.Generic;

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
        public bool CierreCupera { get; set; }
        public bool LiberarCupera { get; set; }
        public int CantidadAlgoritmo { get; set; }
        public List<DiaCupo> Dias { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_FechaRequerido")]
        public DateTime FechaHasta { get; set; }
        public int CantidadDescarga { get; set; }
    }
}