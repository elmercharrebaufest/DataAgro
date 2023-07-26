using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IConsultarAcuerdosGeneradosAgent
    {
        List<AcuerdoSap> ConsultarAcuerdos(string cuit, int materialId, string filtro);
    }
}
