using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Molinos.DataAgro.Entities.Resources;

namespace WebDataAgro.Models
{
    public class ConfiguracionModel
    {
        public int Id { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_Dias")]
        public int CantidadDias { get; set; }
        [Required(ErrorMessageResourceType = typeof(Text), ErrorMessageResourceName = "Error_ClaveStop")]
        public string ClaveStop { get; set; }
        public bool ConexionABMStop { get; set; }
        public bool ConexionConsultaStop { get; set; }
        public int TerminalStopId { get; set; }
        public string CuitDestinoStop { get; set; }
        public int CodigoLocalidadStop { get; set; }
        public decimal ImporteSustentable { get; set; }
        public Resultado Resultado { get; set; }
        public decimal? ContratoAperturaPrecioPorcentajeDeComisionMaximo { get; set; }
    }
}