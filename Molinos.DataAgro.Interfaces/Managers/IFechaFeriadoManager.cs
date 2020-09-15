using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFechaFeriadoManager
    {
        FechaFeriadoDto Traer(int Id);
        Resultado Grabar(FechaFeriado feriado);
        Resultado Eliminar(int Id);
        List<FechaFeriadoDto> TraerTodo();
    }
}


