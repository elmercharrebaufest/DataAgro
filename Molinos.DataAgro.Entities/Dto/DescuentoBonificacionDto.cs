using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class DescuentoBonificacionDto
    {
        public int Id { get; set; }
        public string FechaDesde { get; set; }
        public string FechaHasta { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public string Moneda { get; set; }
        public decimal Porcentaje { get; set; }
        public string TipoDBDesc { get; set; }
        public int TipoDBId { get; set; }
        public string TipoPeriodoDBDesc { get; set; }
        public int TipoPeriodoDBId { get; set; }
        public int ContratoId { get; set; }

    }
}
