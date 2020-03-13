using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class PrecioPizarra
    {
        public int Id { get; set; }
        public int Precio { get; set; }
        public int MaterialId { get; set; }
        public int PizarraId { get; set; }
        public DateTime FechaDesde { get; set; }
        public DateTime FechaHasta { get; set; }
        public string MonedaId { get; set; }
        public string UnidadMedida { get; set; }
        public int? ComercialId { get; set; }

        [ForeignKey("MonedaId")]
        public virtual Moneda Moneda { get; set; }

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }

        [ForeignKey("PizarraId")]
        public virtual Pizarra Pizarra { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }

    }
}
