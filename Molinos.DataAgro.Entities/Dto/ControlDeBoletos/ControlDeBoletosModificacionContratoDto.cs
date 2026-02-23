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
        public string Usuario { get; set; }
        public int CosechaId { get; set; }
        public int ProcedenciaId { get; set; }
        public int ProvinciaId { get; set; }
    }
}
