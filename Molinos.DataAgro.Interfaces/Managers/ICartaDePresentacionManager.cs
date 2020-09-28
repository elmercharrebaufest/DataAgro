using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICartaDePresentacionManager
    {
    
        RptCartaDePresentacionInfo GenerarCartaDePresentacion(RptCartaDePresentacionInfo informe, List<NuevoProduccion> nuevosCampos, List<NuevoAcopio> nuevosAcopios );

       
    }
}
