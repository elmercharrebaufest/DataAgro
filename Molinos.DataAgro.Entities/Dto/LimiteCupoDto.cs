using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class LimiteCupoDto
    {
        public int Id { get; set; }
        public int ZonaCupoId { get; set; }
        public string ZonaCupo { get; set; }
        public int CantidadCupo { get; set; }
        public int CantidadCupoAnterior { get; set; } = 0;
        public int Consumidos { get; set; }
        public int Disponible { get; set; }
    }
}