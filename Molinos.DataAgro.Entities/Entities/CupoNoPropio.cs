using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class CupoNoPropio
    {
        [Key]
        public int Id { get; set; }
        public int CentroId { get; set; }
        public string Codigo { get; set;}
        public DateTime FechaIngreso { get; set;}
        public int MaterialId { get; set;}
        public int? CupoId { get; set;}
        public DateTime FechaAlta { get; set; }
        public int Estado { get; set; }
        public bool Disponible { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; }
        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        [ForeignKey("CupoId")]
        public virtual Cupo Cupo { get; set; }
    }
}
