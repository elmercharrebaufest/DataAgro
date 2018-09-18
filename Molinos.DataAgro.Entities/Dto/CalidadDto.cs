using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CalidadDto
    {
        public int Id { get; set; }
        public int CalidadEspecialId { get; set; }
        public string CalidadEspecialDesc { get; set; }
        public decimal Valor { get; set; }
        public int ContratoId { get; set; }
        public decimal? PorcentajeDesde { get; set; }
        public decimal? PorcentajeHasta { get; set; }

    }
}
