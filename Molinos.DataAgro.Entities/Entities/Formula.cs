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
        public int Inicio { get; set; }
        public int CantDias { get; set; }
        
        public DateTime FechaDesde { get { return DateTime.Now.Date.AddDays(Inicio); } }
        public DateTime FechaHasta { get { return DateTime.Now.Date.AddDays(Inicio + CantDias); } }

        public int CentroId { get; set; }

        public DateTime Fecha { get; set; }
        public bool? Usada { get; set; }

        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //}
    }
}