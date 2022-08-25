using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class ServicioValor
    {
        public int Id { get; set; }
        public int TipoServicioId { get; set; }
        public decimal Importe { get; set; }
        public string MonedaId { get; set; }
        public int MaterialId { get; set; }
        public decimal Desde { get; set; }
        public decimal Hasta { get; set; }
        public int CentroId { get; set; }

        [ForeignKey("TipoServicioId")]
        public virtual TipoServicio TipoServicio { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }

    }

}



