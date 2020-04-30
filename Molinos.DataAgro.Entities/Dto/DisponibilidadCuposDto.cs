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
        public string Disponibles { get; set; }
        public string Consumidos { get; set; }
        public string Limite { get; set; }
        public string MaterialCodigo { get; set; }
        public int MaterialId { get; set; }
    }
}

