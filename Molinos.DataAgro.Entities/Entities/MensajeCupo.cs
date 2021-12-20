using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class MensajeCupo 
    {
        [Key]
        public int Id { get; set; }
       
        public int? ComercialId { get; set; }
        public int? MaterialId { get; set; }
        public int? CentroId { get; set; }
        public DateTime Fecha { get; set; }

        public string Datos { get; set; }
        
        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }


        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }

        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
    }
}