using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IControlDeBoletosValidacionIAManagerIA
    {
        ValidacionDeBoletosIADatosContratoDto ObtenerDatosDeContrato(string contratoSAP);
        List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulas(string contratoSAP);
    }
}
