using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business.Managers
{
    public class AgendaManager : IAgendaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public AgendaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public DatosIniAgendaActividad TraerDatosIniciales(List<int> equipo)
        {
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAgendaActividad()
            {
                TiposActividades = qry.GetTipoActividadCombo(),
                Proveedores = qry.GetProveedorPorComercialCombo(equipo),
            };
        }

        public List<AgendaStore> ExportarAgenda(RptActividadAgendaParam oParam, List<int> equipo)
        {
            return repositorio.ListarConsulta(new TraerAgenda(equipo, oParam));
        }

        public List<AgendaStore> VistaPreviaAgenda(RptActividadAgendaParam oParam)
        {
            var comercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == oParam.ActiveDirectoryId, x => x.ComercialId);
            var data =  repositorio.SelStore<AgendaStore>("DataAgro_TraerAgenda", 0, comercialId, oParam.ActividadDetalle, oParam.TipoActividad, oParam.ProveedorId, oParam.FechaDesde, oParam.FechaHasta);
            return data;
        }
    }
}
