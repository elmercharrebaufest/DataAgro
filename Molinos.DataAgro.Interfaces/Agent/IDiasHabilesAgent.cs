using System;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDiasHabilesAgent
    {
        List<DateTime> ObtenerDiasHabiles();
        DateTime UltimoDiaHabil(DateTime? fecha);
        List<DateTime> ObtenerDiasHabilesDelMes(DateTime? fechaActual = null);
        bool EsDiaHabil(DateTime fecha);
    }
}