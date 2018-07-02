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
    public class CanalOperacionManager : ICanalOperacionManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CanalOperacionManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniCanalOperacion> TraerTodoCanalOperacionAsync()
        {
            var oResult = new ResultIniCanalOperacion();
           
            var oCanalOperacion = mobjUnitOfWork.Repository<CanalOperacion>().Queryable();

            var query = oCanalOperacion
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new CanalOperacionIni()
                        {
                             CanalOperacionId = x.CanalOperacionId,
                             Descripcion = x.Descripcion
                        });

            oResult.CanalOperacion = await query.ToListAsync();

            return oResult;
        }


        public async Task<CanalOperacion> TraerCanalOperacionAsync(int intCanalOperacionId)
        {
            var oCanalOperacion = new CanalOperacion();

            oCanalOperacion = await mobjUnitOfWork.Repository<CanalOperacion>()
                                 .Queryable()
                                 .Where(x => x.CanalOperacionId == intCanalOperacionId)
                                 .SingleOrDefaultAsync();

            if (oCanalOperacion == null)
            {
                oCanalOperacion = new CanalOperacion()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCanalOperacion.ObjectState = Constants.Object_Modified;
            }
            
            return oCanalOperacion;
        }


        public async Task<EntityErrors> GrabarCanalOperacionAsync(CanalOperacion oCanalOperacion)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oCanalOperacion, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            CanalOperacion oCanalOperacionSave;

            if (oCanalOperacion.ObjectState == 0)
            {
                oCanalOperacionSave = new CanalOperacion()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCanalOperacionSave = await TraerCanalOperacionAsync(oCanalOperacion.CanalOperacionId);
            }

            var validacion = ValidarCanalOperacion(oCanalOperacion, oCanalOperacionSave.ObjectState,
                (oCanalOperacionSave.ObjectState == Constants.Object_Modified ? (int?)oCanalOperacion.CanalOperacionId : null));

            if (validacion != null)
            {
                oEntityErrors.ListaErrores.Add(validacion);
                return oEntityErrors;
            }

            oCanalOperacionSave.Descripcion = oCanalOperacion.Descripcion;
            oCanalOperacionSave.Inhabilitado = oCanalOperacion.Inhabilitado;

            if (oCanalOperacionSave.ObjectState == Constants.Object_Added)
            {
                oCanalOperacionSave.CanalOperacionId = ((mobjUnitOfWork.Repository<CanalOperacion>().Queryable().Max(x => (int?)x.CanalOperacionId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<CanalOperacion>().SaveEntity(oCanalOperacionSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarCanalOperacionAsync(int intCanalOperacionId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<CanalOperacion>();

            var oCanalOperacion = await oRepository
                                 .Queryable()
                                 .Where(x => x.CanalOperacionId == intCanalOperacionId)
                                 .SingleOrDefaultAsync();

            if (oCanalOperacion != null)
            {
                oRepository.Delete(oCanalOperacion);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        #region Validar
        public ErrorMessage ValidarCanalOperacion(CanalOperacion oCanalOperaciones, int ObjectState, int? Id = null)
        {
            var oCanalOperacion = mobjUnitOfWork.Repository<CanalOperacion>().Queryable();

            CanalOperacion val = null;

            if (ObjectState == Constants.Object_Added)
            {
                val = oCanalOperacion
                    .Where(x => x.Descripcion == oCanalOperaciones.Descripcion).FirstOrDefault();
            }
            else if (ObjectState == Constants.Object_Modified)
            {
                val = oCanalOperacion
                    .Where(x => x.Descripcion == oCanalOperaciones.Descripcion
                    && x.CanalOperacionId != Id).FirstOrDefault();
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




