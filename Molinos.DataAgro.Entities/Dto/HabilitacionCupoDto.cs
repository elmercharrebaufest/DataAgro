using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class HabilitacionCupoDto
    {
        public int Id { get; set; }
        public int ZonaCupoId { get; set; }
        public int MaterialId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string Zona { get; set; }
        public string Material { get; set; }
    }
}