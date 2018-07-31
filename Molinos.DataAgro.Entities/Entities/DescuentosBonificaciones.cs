using Mastersoft.Framework.Interfaces;
using System;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class DescuentosBonificaciones : Entity
    {
        public int Id { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public float Porcentaje { get; set; }
        public int TipoDBId { get; set; }
        public int TipoPeriodoDBId { get; set; }
        public int ContratoId { get; set; }	
        public  Contrato Contrato { get; set; }
    }

}
   


