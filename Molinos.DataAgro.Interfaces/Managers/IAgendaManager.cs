using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAgendaManager
    {
        Task <List<AgendaStore>> ExportarAgenda(RptActividadAgendaParam oParam);
        Task<DatosIniAgendaActividad> TraerDatosInicialesAsync(string ActiveDirectoryId);

        Task<List<AgendaStore>> VistaPreviaAgenda(RptActividadAgendaParam oParam);
    }
}
