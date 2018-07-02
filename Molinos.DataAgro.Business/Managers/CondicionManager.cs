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
    public class CondicionManager : ICondicionManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CondicionManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniCondicion> TraerTodoCondicionAsync()
        {
            var oResult = new ResultIniCondicion();
           
            var oCondicion = mobjUnitOfWork.Repository<Condicion>().Queryable();

            var query = oCondicion
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new CondicionIni()
                        {
                             CondicionId = x.CondicionId,
                             Descripcion = x.Descripcion
                        });

            oResult.Condicion = await query.ToListAsync();

            return oResult;
        }


        public async Task<Condicion> TraerCondicionAsync(int intCondicionId)
        {
            var oCondicion = new Condicion();

            oCondicion = await mobjUnitOfWork.Repository<Condicion>()
                                 .Queryable()
                                 .Where(x => x.CondicionId == intCondicionId)
                                 .SingleOrDefaultAsync();

            if (oCondicion == null)
            {
                oCondicion = new Condicion()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCondicion.ObjectState = Constants.Object_Modified;
            }
            
            return oCondicion;
        }


        public async Task<EntityErrors> GrabarCondicionAsync(Condicion oCondicion)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oCondicion, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Condicion oCondicionSave;

            if (oCondicion.ObjectState == 0)
            {
                oCondicionSave = new Condicion()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCondicionSave = await TraerCondicionAsync(oCondicion.CondicionId);
            }

            var validacion = ValidarCondicion(oCondicion, oCondicionSave.ObjectState,
                (oCondicionSave.ObjectState == Constants.Object_Modified ? (int?)oCondicion.CondicionId : null));

            if (validacion != null)
            {
                oEntityErrors.ListaErrores.Add(validacion);
                return oEntityErrors;
            }

            oCondicionSave.Descripcion = oCondicion.Descripcion;
            oCondicionSave.Inhabilitado = oCondicion.Inhabilitado;
            if (oCondicionSave.ObjectState == Constants.Object_Added)
            {
                oCondicionSave.CondicionId = ((mobjUnitOfWork.Repository<Condicion>().Queryable().Max(x => (int?)x.CondicionId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Condicion>().SaveEntity(oCondicionSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarCondicionAsync(int intCondicionId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Condicion>();

            var oCondicion = await oRepository
                                 .Queryable()
                                 .Where(x => x.CondicionId == intCondicionId)
                                 .SingleOrDefaultAsync();

            if (oCondicion != null)
            {
                oRepository.Delete(oCondicion);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        #region Validar
        public ErrorMessage ValidarCondicion(Condicion oCondiciones, int ObjectState, int? Id = null)
        {
            var oCondicion = mobjUnitOfWork.Repository<Condicion>().Queryable();

            Condicion val = null;

            if (ObjectState == Constants.Object_Added)
            {
                val = oCondicion
                    .Where(x => x.Descripcion == oCondiciones.Descripcion).FirstOrDefault();
            }
            else if (ObjectState == Constants.Object_Modified)
            {
                val = oCondicion
                    .Where(x => x.Descripcion == oCondiciones.Descripcion
                    && x.CondicionId != Id).FirstOrDefault();
            }

            if (val != null)
            {
                return new ErrorMessage() { Message = "Existe un registro de iguales carecteristicas." };
            }
            return null;
        } 
        #endregion

    }
}




