using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfirmaGeneradoDto
    {
        public string ContratoSAP { get; set; }
        public int ClaseNegocioId { get; set; }
        public int NegocioId { get; set; }
        public int ComercialId { get; set; }
        public bool Generado { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string FechaGeneracionFormateada { get { return FechaGeneracion.ToShortDateString(); } }
        public bool IsWebService { get; set; }
        public string Mensaje { get; set; }
        public int Version { get; set; }
        public string FijacionSAP { get; set; }
        public int TipoBoletoId { get; set; }
        public string NegocioSAP { get; set; }
        public List<string> Clausulas { get; set; } = new List<string>();
        public string Archivo { get { return "confirma" + FechaGeneracion.ToString("yyyy/MM/dd").Replace("/", String.Empty) + "_" + ContratoSAP + (!String.IsNullOrEmpty(FijacionSAP)?"_"+FijacionSAP:String.Empty) + "_V" + Version.ToString("D2") + ".xml"; } }
        public bool? TieneFechaGeneracionUltimoConfirma { get; set; }
    }
}