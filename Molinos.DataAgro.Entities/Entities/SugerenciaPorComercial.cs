using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public class SugerenciaPorComercial
    {
        public int Id { get; set; }

        public int ComercialId { get; set; }

        public int CentroId { get;set;}
        public int MaterialId { get; set; }
        public int Total { get; set; }

        [ForeignKey("ComercialId")]
        public virtual Comercial Comercial { get; set; }
        [ForeignKey("CentroId")]
        public virtual Centro Centro { get; set; } 

        [ForeignKey("MaterialId")]
        public virtual Material Material { get; set; }
        public DateTime Fecha { get; set; }
    }
}
