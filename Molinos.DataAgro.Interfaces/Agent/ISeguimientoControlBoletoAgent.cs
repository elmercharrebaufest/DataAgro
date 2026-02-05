using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Agent
{
    public interface ISeguimientoControlBoletoAgent
    {
        string SeguimientoBoletos(SeguimientoControlDeBoletosDto seguimientoControlDeBoletos);
    }
}
