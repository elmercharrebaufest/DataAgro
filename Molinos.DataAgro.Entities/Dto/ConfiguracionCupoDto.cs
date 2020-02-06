using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class ConfiguracionCupoDto
    {
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string Centro { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
        public DateTime Fecha { get; set; }
        public int LimiteCupo { get; set; }
        public List<LimiteCupoDto> CantidadCupo { get; set; }

    }
}