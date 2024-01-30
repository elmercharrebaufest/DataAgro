using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IPrecioPizarraManager
    {
        List<PrecioPizarraDto> TraerTodoPrecioPizarraPorMaterialYPizarra(int materialId, int pizarraId);
        List<PrecioPizarraDto> TraerTodoPrecioPizarra();
        Resultado GrabarPrecioPizarra(PrecioPizarra precioPizarra, bool manual = false);
        List<MonedaDto> TraerTodoMoneda();
        PrecioPizarraDto TraerPrecioPizarraPorId(int id);
        Resultado EliminarPizarra(int id);
        void ActualizarPrecioPizarra(DateTime fecha, bool manual);
    }
}
