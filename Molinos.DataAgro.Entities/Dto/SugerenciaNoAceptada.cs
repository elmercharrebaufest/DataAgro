using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class SugerenciaNoAceptada
    {
        public string NombreComercial { get; set; }
        public List<DiaCupo> Diacupo { get; set; }
        public string Material { get; set; }
        public int MaterialId { get; set; }
    }
}
