using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Calidad
    {
        public int Id { get; set; }
        public int StandardDeCalidadId { get; set; }
        public int CalidadEspecialId { get; set; }
        public decimal Valor { get; set; }
        public int NegocioId { get; set; }
        public decimal? PorcentajeDesde { get; set; }
        public decimal? PorcentajeHasta { get; set; }

        [ForeignKey("StandardDeCalidadId")]
        public virtual StandardDeCalidad StandardDeCalidad { get; set; }
        [ForeignKey("CalidadEspecialId")]
        public virtual CalidadEspecial CalidadEspecial { get; set; }
        [ForeignKey("NegocioId")]
        public virtual Contrato Contrato { get; set; }
        [ForeignKey("NegocioId")]
        public virtual ContratoAcuerdo ContratoAcuerdo { get; set; }
    }

}
   


