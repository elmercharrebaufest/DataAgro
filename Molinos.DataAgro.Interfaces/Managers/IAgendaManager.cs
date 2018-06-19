using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;

using Molinos.DataAgro.Entities;


namespace Molinos.DataAgro.Interfaces
{
    public interface IAgendaManager
    {

        void Inicializar(MSContext oContexto);

        void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork);

        Task <List<AgendaStore>> ExportarAgenda(RptActividadAgendaParam oParam);
        Task<DatosIniAgendaActividad> TraerDatosInicialesAsync(string ActiveDirectoryId);

        Task<List<AgendaStore>> VistaPreviaAgenda(RptActividadAgendaParam oParam);
    }
}
