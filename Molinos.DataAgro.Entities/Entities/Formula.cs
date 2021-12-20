using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class Formula//: ICloneable 
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Criterio")]
        public int CriterioId { get; set; }
        public virtual Criterio Criterio { get; set; }        
        
        public DateTime CuposDesde { get; set; }
        public DateTime CuposHasta { get; set; }

        public DateTime NegociosDesde { get; set; }
        public DateTime NegociosHasta { get; set; }

        public int CentroId { get; set; }

        public DateTime Fecha { get; set; }
        public bool? Usada { get; set; }

        [ForeignKey("Material")]
        public int MaterialId { get; set; }
        public virtual Material Material { get; set; }
        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //}
    }
}