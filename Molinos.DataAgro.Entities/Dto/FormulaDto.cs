using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class FormulaDto
    {

        public int Id { get; set; }
        public int CriterioId { get; set; }
        public Criterio Criterio { get; set; }
        public DateTime CuposDesde { get; set; }
        public DateTime CuposHasta { get; set; }

        public DateTime NegociosDesde { get; set; }
        public DateTime NegociosHasta { get; set; }

        public int CentroId { get; set; }

        public DateTime Fecha { get; set; }
        public int MaterialId { get; set; }
        public string Material { get; set; }
    }


    public partial class FormulaDtoExcel
    {

        public string Material { get; set; }
        public string Criterio { get; set; }
        public int Puntuacion { get; set; }
        public DateTime CuposDesde { get; set; }
        public DateTime CuposHasta { get; set; }
        public DateTime NegociosDesde { get; set; }
        public DateTime NegociosHasta { get; set; }
    }
}
