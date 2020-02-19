using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAgendaManager
    {
        List<AgendaStore> ExportarAgenda(RptActividadAgendaParam oParam, List<int> equipo);
        DatosIniAgendaActividad TraerDatosIniciales(List<int> equipo);
        List<AgendaStore> VistaPreviaAgenda(RptActividadAgendaParam oParam);
    }
}
