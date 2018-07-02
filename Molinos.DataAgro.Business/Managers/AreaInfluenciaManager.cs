using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class AreaInfluenciaManager : IAreaInfluenciaManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public AreaInfluenciaManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniAreaInfluencia> TraerTodoAreaInfluenciaAsync()
        {
            var oResult = new ResultIniAreaInfluencia();
           
            var oAreaInfluencia = mobjUnitOfWork.Repository<AreaInfluencia>().Queryable();

            var query = oAreaInfluencia
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new AreaInfluenciaIni()
                        {
                             AreaInfluenciaId = x.AreaInfluenciaId,
                             Descripcion = x.Descripcion
                        });

            oResult.AreaInfluencia = await query.ToListAsync();

            return oResult;
        }


        public async Task<AreaInfluencia> TraerAreaInfluenciaAsync(int intAreaInfluenciaId)
        {
            var oAreaInfluencia = new AreaInfluencia();

            oAreaInfluencia = await mobjUnitOfWork.Repository<AreaInfluencia>()
                                 .Queryable()
                                 .Where(x => x.AreaInfluenciaId == intAreaInfluenciaId)
                                 .SingleOrDefaultAsync();

            if (oAreaInfluencia == null)
            {
                oAreaInfluencia = new AreaInfluencia()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oAreaInfluencia.ObjectState = Constants.Object_Modified;
            }
            
            return oAreaInfluencia;
        }


        public async Task<EntityErrors> GrabarAreaInfluenciaAsync(AreaInfluencia oAreaInfluencia)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oAreaInfluencia, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            AreaInfluencia oAreaInfluenciaSave;

            if (oAreaInfluencia.ObjectState == 0)
            {
                oAreaInfluenciaSave = new AreaInfluencia()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oAreaInfluenciaSave = await TraerAreaInfluenciaAsync(oAreaInfluencia.AreaInfluenciaId);
            }
         
            oAreaInfluenciaSave.Descripcion = oAreaInfluencia.Descripcion;  
            if (oAreaInfluenciaSave.ObjectState == Constants.Object_Added)
            {
                oAreaInfluenciaSave.AreaInfluenciaId = ((mobjUnitOfWork.Repository<AreaInfluencia>().Queryable().Max(x => (int?)x.AreaInfluenciaId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<AreaInfluencia>().SaveEntity(oAreaInfluenciaSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarAreaInfluenciaAsync(int intAreaInfluenciaId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<AreaInfluencia>();

            var oAreaInfluencia = await oRepository
                                 .Queryable()
                                 .Where(x => x.AreaInfluenciaId == intAreaInfluenciaId)
                                 .SingleOrDefaultAsync();

            if (oAreaInfluencia != null)
            {
                oRepository.Delete(oAreaInfluencia);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

   
             

    }
}




