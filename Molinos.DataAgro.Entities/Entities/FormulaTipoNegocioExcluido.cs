using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Entities
{
    public partial class FormulaTipoNegocioExcluido
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Formula")]
        public int FormulaId { get; set; }
        public virtual Formula Formula { get; set; }
        [ForeignKey("TipoNegocio")]
        public int TipoNegocioId { get; set; }
        public virtual TipoNegocio TipoNegocio { get; set; }
    }
}
