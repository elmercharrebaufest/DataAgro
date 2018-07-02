using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class AgendaManager : IAgendaManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public AgendaManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        public async Task<DatosIniAgendaActividad> TraerDatosInicialesAsync(string ActiveDirectory)
        {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAgendaActividad()
            {
                TiposActividades = await qry.GetTipoActividadComboAsync(),
                Proveedores = await qry.GetProveedorPorComercialComboAsync(ActiveDirectory),                
            };

            return oDatosIniciales;
        }

        public async Task<List<AgendaStore>> ExportarAgenda(RptActividadAgendaParam oParam)
        {

            var ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => x.IdActiveDirectory == oParam.ActiveDirectoryId).SingleOrDefault().ComercialId;
                                

            var Agenda = mobjUnitOfWork.SelStoreAsync<AgendaStore>("DataAgro_TraerAgenda", ComercialId, oParam.ActividadDetalle, oParam.TipoActividad, oParam.ProveedorId, oParam.FechaDesde, oParam.FechaHasta);

            var oResult = await Agenda.ToListAsync();           

            return oResult;
        }

        public async Task<List<AgendaStore>> VistaPreviaAgenda(RptActividadAgendaParam oParam)
        {
            var ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().Where(x => x.IdActiveDirectory == oParam.ActiveDirectoryId).SingleOrDefault().ComercialId;
            
            return mobjUnitOfWork.SelStoreAsync<AgendaStore>("DataAgro_TraerAgenda", ComercialId, oParam.ActividadDetalle, oParam.TipoActividad, oParam.ProveedorId, oParam.FechaDesde, oParam.FechaHasta).ToList();
        }


    }
}
