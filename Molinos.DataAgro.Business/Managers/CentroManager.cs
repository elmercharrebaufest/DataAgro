using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{

    public class CentroManager : ICentroManager
    { 
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CentroManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }
        public async Task<DatosIniAbmCentro> TraerDatosInicialesAsync()
        {
            //si es alta le mandoo cero, si modifDatosIniAbmCentroico le mando el id desde la grilla
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAbmCentro()
            {
                Centro = await ObtenerCentros(0)
            };

            return oDatosIniciales;
        }

        public async Task<ResultIniCentro> TraerTodoCentroAsync()
        {
            var oResult = new ResultIniCentro();

            var oCentro = mobjUnitOfWork.Repository<Centro>().Queryable();

            var query = oCentro
                        .OrderBy(x => x.Descripcion)
                        .Select(x => new CentroIni()
                        {
                            Id = x.Id,
                            Descripcion = x.Descripcion,
                            CodigoSap = x.CodigoSap
                        });

            oResult.Centro = await query.ToListAsync();

            return oResult;
        }

        public async Task<List<CentroCombo>> ObtenerCentros(int centroId)
        {
            var list = new List<CentroCombo>();
            var qry = new CombosQueries(mobjUnitOfWork);

            list = await qry.GetAbmCentroComboAsync();

            if (centroId != 0)
            {
                var resultStored = mobjUnitOfWork.SelStore<Centro>("DataAgro_ComercialesJerarquicos_Traer", centroId).ToList();
                list.RemoveAll(x => resultStored.Any(z => z.Id == x.Id));
            }

            return list;
        }

        public async Task<Centro> TraerCentroAsync(int id)
        {
            var oCentro = new Centro();

            oCentro = await mobjUnitOfWork.Repository<Centro>()
                                 .Queryable()
                                 .Where(x => x.Id == id)
                                 .SingleOrDefaultAsync();

            if (oCentro == null)
            {
                oCentro = new Centro()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCentro.ObjectState = Constants.Object_Modified;
            }

            return oCentro;
        }

        public async Task<EntityErrors> GrabarCentroAsync(Centro oCentro)
        {
            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oCentro, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }
           
            Centro oCentroSave;
            if (oCentro.ObjectState == 0)
            {
                oCentroSave = new Centro()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oCentroSave = await TraerCentroAsync(oCentro.Id);
            }

            oCentroSave.Descripcion = oCentro.Descripcion;
            oCentroSave.CodigoSap = oCentro.CodigoSap;

            mobjUnitOfWork.Repository<Centro>().SaveEntity(oCentroSave);

            await mobjUnitOfWork.SaveChangesAsync();

            logger.Debug("Guardando el centro:" + oCentro.Descripcion);

            return oEntityErrors;
        }

        public async Task<EntityErrors> EliminarCentroAsync(int id)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Centro>();

            var oCentro = await oRepository
                                 .Queryable()
                                 .Where(x => x.Id == id)
                                 .SingleOrDefaultAsync();

            if (oCentro != null)
            {
                oRepository.Delete(oCentro);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            logger.Debug("Eliminando el centro:" + oCentro.Descripcion);

            return oEntityErrors;
        }
    }
}

