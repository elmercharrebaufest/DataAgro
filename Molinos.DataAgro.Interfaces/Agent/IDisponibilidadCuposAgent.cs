using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDisponibilidadCuposAgent
    {
        List<DisponibilidadCuposDto> TraerDisponibilidadCupos(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId);
    }
}