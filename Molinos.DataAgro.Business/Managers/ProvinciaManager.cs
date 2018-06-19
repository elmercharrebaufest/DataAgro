using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.SqlServer;
using System.Data.Entity;
using System.Diagnostics;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business
{
    public class ProvinciaManager : IProvinciaManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicialización
        //--------------------------------------------------

        public void Inicializar(MSContext oContexto)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = new UnitOfWork(oContexto, new DataAgroContext(oContexto));
        }


        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            mobjContexto = oContexto;

            mobjUnitOfWork = oUnitOfWork;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<ResultIniProvincia> TraerTodoProvinciaAsync()
        {
            var oResult = new ResultIniProvincia();
           
            var oProvincia = mobjUnitOfWork.Repository<Provincia>().Queryable();

            var query = oProvincia
                        .OrderBy(x => x.Nombre)
                        .Select(x => new ProvinciaIni()
                        {
                             ProvinciaId = x.ProvinciaId,
                             Nombre = x.Nombre
                        });

            oResult.Provincia = await query.ToListAsync();

            return oResult;
        }


        public async Task<Provincia> TraerProvinciaAsync(int intProvinciaId)
        {
            var oProvincia = new Provincia();

            oProvincia = await mobjUnitOfWork.Repository<Provincia>()
                                 .Queryable()
                                 .Where(x => x.ProvinciaId == intProvinciaId)
                                 .SingleOrDefaultAsync();

            if (oProvincia == null)
            {
                oProvincia = new Provincia()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oProvincia.ObjectState = Constants.Object_Modified;
            }
            
            return oProvincia;
        }


        public async Task<EntityErrors> GrabarProvinciaAsync(Provincia oProvincia)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oProvincia, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Provincia oProvinciaSave;

            if (oProvincia.ObjectState == 0)
            {
                oProvinciaSave = new Provincia()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oProvinciaSave = await TraerProvinciaAsync(oProvincia.ProvinciaId);
            }
         
            oProvinciaSave.Nombre = oProvincia.Nombre;  
            if (oProvinciaSave.ObjectState == Constants.Object_Added)
            {
                oProvinciaSave.ProvinciaId = ((mobjUnitOfWork.Repository<Provincia>().Queryable().Max(x => (int?)x.ProvinciaId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Provincia>().SaveEntity(oProvinciaSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarProvinciaAsync(int intProvinciaId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Provincia>();

            var oProvincia = await oRepository
                                 .Queryable()
                                 .Where(x => x.ProvinciaId == intProvinciaId)
                                 .SingleOrDefaultAsync();

            if (oProvincia != null)
            {
                oRepository.Delete(oProvincia);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

   
             

    }
}




