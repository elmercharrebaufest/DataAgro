using System;
using System.Collections.Generic;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Interfaces
{
    public interface IDiasHabilesAgent
    {
        List<DateTime> ObtenerDiasHabiles();
        DateTime UltimoDiaHabil(DateTime? fecha);
        List<DateTime> ObtenerDiasHabilesDelMes(DateTime? fechaActual = null);
    }
}