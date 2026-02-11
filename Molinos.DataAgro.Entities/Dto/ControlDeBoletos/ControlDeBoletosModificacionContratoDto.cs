using Molinos.DataAgro.Entities.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto.ControlDeBoletos
{
    public class ControlDeBoletosModificacionContratoDto
    {
        public int NegocioId { get; set; }
        public int ClasificacionId { get; set; }
        public string Contrato { get; set; }
        public int CosechaId { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public int ProcedenciaId { get; set; }
        public int ProvinciaId { get; set; }
        public string Usuario { get; set; } 
    }

    public class ControlDeBoletosRegistrarAccionesDto
    {
        public List<int> ControlDeBoletoIds { get; set; }
        public int AccionControlDeBoletos { get; set; }
    }

}
