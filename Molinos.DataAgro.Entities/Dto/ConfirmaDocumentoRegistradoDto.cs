using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaDocumentoRegistradoDto
    {
        public string NombreArchivo { get; set; }
        public byte[] PdfBinario { get; set; }
        public string EstadoConsulta { get; set; }
        public string Errores { get; set; }
    }
}
