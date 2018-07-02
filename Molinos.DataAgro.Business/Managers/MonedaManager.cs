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
    public class MonedaManager : IMonedaManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public MonedaManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------


        public async Task<ResultIniMoneda> TraerTodoAsync()
        {
            var oResult = new ResultIniMoneda();

            var oMoneda = mobjUnitOfWork.Repository<Moneda>().Queryable();

            var query = oMoneda
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new MonedaIni()
                        {
                            MonedaId = x.MonedaId,
                            Descripcion = x.Descripcion
                        });

            oResult.Moneda = await query.ToListAsync();

            return oResult;
        }

        public async Task<Moneda>  TraerMonedadAsync(string MonedaId)
        {
            var oMoneda = new Moneda();

            oMoneda = await mobjUnitOfWork.Repository<Moneda>()
                                 .Queryable()
                                 .Where(x => x.MonedaId == MonedaId)
                                 .SingleOrDefaultAsync();

            if (oMoneda == null)
            {
                oMoneda = new Moneda()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oMoneda.ObjectState = Constants.Object_Modified;
            }

            return oMoneda;
        }

        public async Task<EntityErrors> GrabarMonedaAsync(Moneda oMoneda)
        {
            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oMoneda, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Moneda oMonedaSave;

            if (oMoneda.ObjectState == 0)
            {
                oMonedaSave = new Moneda()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oMonedaSave = await TraerMonedadAsync(oMoneda.MonedaId);
            }

            oMonedaSave.Descripcion = oMoneda.Descripcion;
        

            mobjUnitOfWork.Repository<Moneda>().SaveEntity(oMonedaSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public async Task<EntityErrors> EliminarMonedaAsync(string MonedaId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Moneda>();

            var oMoneda = await oRepository
                                 .Queryable()
                                 .Where(x => x.MonedaId == MonedaId)
                                 .SingleOrDefaultAsync();

            if (oMoneda != null)
            {
                oRepository.Delete(oMoneda);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


      


     

    }
}




