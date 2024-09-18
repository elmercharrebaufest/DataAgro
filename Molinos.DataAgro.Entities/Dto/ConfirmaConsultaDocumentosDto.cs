using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaConsultaDocumentosDto
    {
        public string idDocumento;
        public string idBolsa;
        public int consultaEstado;
        public int consultaEstadoDocumento;
        public EmpresaConfirmaDto enPoderDe;
        public int estadoDocumento;
    }

    public class EmpresaConfirmaDto
    {
        public TCodCaptionDto cUIT;
        public string razonSocial;
    }

    public class TCodCaptionDto
    {
        public string caption;
        public string codLista;
        public string value;
    }
}