using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;

namespace Molinos.DataAgro.Business
{
    public class TipoNegocioManager : ITipoNegocioManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public TipoNegocioManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public TipoNegocioDto TraerTipoNegociod(int tipoNegocioId)
        {
            return repositorio.Obtener<TipoNegocio, TipoNegocioDto>(x => x.TipoNegocioId == tipoNegocioId, x=> new TipoNegocioDto(){TipoNegocioId = x.TipoNegocioId,Descripcion =x.Descripcion}) ?? new TipoNegocioDto();
        }
    }
}




