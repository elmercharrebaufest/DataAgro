using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IPrecioPizarraManager
    {
        List<PrecioPizarraDto> TraerTodoPrecioPizarraPorMaterialYPizarra(int materialId, int pizarraId);
        List<PrecioPizarraDto> TraerTodoPrecioPizarra();
        Resultado GrabarPrecioPizarra(PrecioPizarra precioPizarra);
        List<MonedaDto> TraerTodoMoneda();
    }
}
