using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ConfirmaConsultaDocumentosDto
    {
        public string IdDocumento;
        public string IdBolsa;
        public int EstadoDocumento;
        public int ConsultaEstadoDocumento;
        public EmpresaConfirmaDto EnPoderDe;
        public List<Acciones> Acciones;
    }

    public class EmpresaConfirmaDto
    {
        public int CUIT;
        public string RazonSocial;
    }

    public class Acciones
    {
        public string FechaHora;
        public string Accion;
        public string Resultado;
        public string TipoDocumento;
        public string NroDocumento;
        public string Apellido;
        public string Nombre;
        public string Cargo;
    }
}