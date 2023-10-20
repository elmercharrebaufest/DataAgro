using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFAQManager
    {
        List<ManualesDto> TraerManuales();
        ManualResult EnviarSugerencia(ManualesDto manual, string sugerencia, string IdActiveDirectory);
        ManualResult RegistrarVisita(int idManual, string IdActiveDirectory);
        void ActualizarFechaUltimaActualizacionManualesFAQ();
    }
}
