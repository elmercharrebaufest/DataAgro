using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ResultadoCriterioDto
    {
        public decimal Puntuacion { get; set; }
        public Dictionary<string, decimal> Puntuaciones { get; set; } = new Dictionary<string, decimal>();
    }
}