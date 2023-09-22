using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FormulaTipoNegocioExcluidoDto
    {
        public int Id { get; set; }
        public int FormulaId { get; set; }
        public int TipoNegocioId { get; set; }
        public string TipoNegocio { get; set; }
    }
}
