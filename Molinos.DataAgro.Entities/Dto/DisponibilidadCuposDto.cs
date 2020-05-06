using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class DisponibilidadCuposDto
    {
        public DateTime Fecha { get; set; }
        public string MaterialNombre { get; set; }
        public string ZonaId { get; set; }
        public int Disponibles { get; set; }
        public int Consumidos { get; set; }
        public int Limite { get; set; }
        public string MaterialCodigo { get; set; }
        public int MaterialId { get; set; }
        public string ZonaNombre { get; set; }
        public string CentroNombre { get; set; }
        public string CentroCodigo { get; set; }
    }
}

