using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class TipoNegocioManager : ITipoNegocioManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public TipoNegocioManager(ILogger logger, IMSContextProvider oMSContextProvider, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------


        public async Task<ResultIniTipoNegocio> TraerTodoAsync()
        {
            var oResult = new ResultIniTipoNegocio();

            var oTipoNegocio = mobjUnitOfWork.Repository<TipoNegocio>().Queryable();

            var query = oTipoNegocio
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new TipoNegocioIni()
                        {
                            TipoNegocioId = x.TipoNegocioId,
                            Descripcion = x.Descripcion
                        });

            oResult.TipoNegocio = await query.ToListAsync();

            return oResult;
        }

        public async Task<TipoNegocio>  TraerTipoNegociodAsync(int TipoNegocioId)
        {
            var oTipoNegocio = new TipoNegocio();

            oTipoNegocio = await mobjUnitOfWork.Repository<TipoNegocio>()
                                 .Queryable()
                                 .Where(x => x.TipoNegocioId == TipoNegocioId)
                                 .SingleOrDefaultAsync();

            if (oTipoNegocio == null)
            {
                oTipoNegocio = new TipoNegocio()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oTipoNegocio.ObjectState = Constants.Object_Modified;
            }

            return oTipoNegocio;
        }

        public async Task<EntityErrors> GrabarTipoNegocioAsync(TipoNegocio oTipoNegocio)
        {
            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oTipoNegocio, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            TipoNegocio oTipoNegocioSave;

            if (oTipoNegocio.ObjectState == 0)
            {
                oTipoNegocioSave = new TipoNegocio()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oTipoNegocioSave = await TraerTipoNegociodAsync(oTipoNegocio.TipoNegocioId);
            }

            oTipoNegocioSave.Descripcion = oTipoNegocio.Descripcion;
        

            mobjUnitOfWork.Repository<TipoNegocio>().SaveEntity(oTipoNegocioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<EntityErrors> EliminarTipoNegocioAsync(int TipoNegocioId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<TipoNegocio>();

            var oTipoNegocio = await oRepository
                                 .Queryable()
                                 .Where(x => x.TipoNegocioId == TipoNegocioId)
                                 .SingleOrDefaultAsync();

            if (oTipoNegocio != null)
            {
                oRepository.Delete(oTipoNegocio);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

     

       
    }
}




