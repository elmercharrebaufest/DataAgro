using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class BoletoArchivoDto
    {
        public string Nombre { get; set; }
        public string Url { get; set; }
        public string Tamano { get; set; }
        public string FechaUltimaEscritura { get; set; }
    }
}
