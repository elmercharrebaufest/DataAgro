using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDisponibilidadCuposAgent
    {
        List<DisponibilidadCuposDto> TraerDisponibilidadCupos(DateTime? fecha, string zonaId, string centroId, string materialId);
    }
}