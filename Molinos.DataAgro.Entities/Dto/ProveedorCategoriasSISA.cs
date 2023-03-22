using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ProveedorCategoriasSISA
    {
        public string CUIT { get; set; }
        public string Mensaje { get; set; }
        public List<CategoriasSISA> CategoriasSISA { get; set; }
        public int Existe { get; set; }
    }

    public class CategoriasSISA
    {
        public int CodCategoria { get; set; }
        public string Categoria { get; set; }
    }

    public class CuitSegmentacion
    {
        public string CUIT { get; set; }
        public int Segmentacion { get; set; }
    }
}

