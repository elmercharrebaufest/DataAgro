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
    public class LocalidadManager : ILocalidadManager
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
        
        public async Task<DatosIniAbmLocalidad> TraerDatosInicialesAsync()
        {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAbmLocalidad()
            {
                Provincia = await qry.GetProvinciaComboAsync()
            };

            return oDatosIniciales;
        }


        public async Task<ResultIniLocalidad> TraerFiltroLocalidadAsync(ParamAbmLocalidad oParam)
        {
            var oResult = new ResultIniLocalidad();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable();
            var oProvincia = mobjUnitOfWork.Repository<Provincia>().Queryable();

            var query = oLocalidad
                        .Join(oProvincia, a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new { LOC = a, PRO = b })
                        .Where(x => (oParam.Nombre.Trim() == "" || x.LOC.Nombre.Contains(oParam.Nombre.Trim())) &&
                                    (oParam.ProvinciaId == null || x.LOC.ProvinciaId == oParam.ProvinciaId))
                        .Take(500)
                        .OrderBy(x => x.PRO.Nombre)
                        .ThenBy(x => x.LOC.Nombre)
                        .Select(x => new LocalidadIni()
                        {
                            LocalidadId = x.LOC.LocalidadId,
                            CodLocalidad = x.LOC.CodLocalidad,
                            Nombre = x.LOC.Nombre,
                            ProNombre = x.PRO.Nombre,
                        });

          

            oResult.Localidad = await query.ToListAsync();

            return oResult;
        }

        public async Task<ResultIniLocalidad> TraerLocalidadPorProvincia(int ProvinciaId) {
            var oResult = new ResultIniLocalidad();

            var oLocalidad = mobjUnitOfWork.Repository<Localidad>().Queryable();
            var oProvincia = mobjUnitOfWork.Repository<Provincia>().Queryable();

            var query = oLocalidad
                        .Join(oProvincia, a => a.ProvinciaId, b => b.ProvinciaId, (a, b) => new { LOC = a, PRO = b })
                        .Where(x => x.LOC.ProvinciaId == ProvinciaId)
                        .Select(x => new LocalidadIni() {
                            LocalidadId = x.LOC.LocalidadId,
                            CodLocalidad = x.LOC.CodLocalidad,
                            Nombre = x.LOC.Nombre,
                            ProNombre = x.PRO.Nombre,
                        });
            
            oResult.Localidad = await query.ToListAsync();

            return oResult;
        }

        public async Task<Localidad> TraerLocalidadAsync(int intLocalidadId)
        {
            var oLocalidad = new Localidad();

            oLocalidad = await mobjUnitOfWork.Repository<Localidad>()
                                 .Queryable()
                                 .Where(x => x.LocalidadId == intLocalidadId)
                                 .SingleOrDefaultAsync();

            if (oLocalidad == null)
            {
                oLocalidad = new Localidad()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oLocalidad.ObjectState = Constants.Object_Modified;
            }
            
            return oLocalidad;
        }


        public async Task<EntityErrors> GrabarLocalidadAsync(Localidad oLocalidad)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oLocalidad, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Localidad oLocalidadSave;

            if (oLocalidad.ObjectState == 0)
            {
                oLocalidadSave = new Localidad()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oLocalidadSave = await TraerLocalidadAsync(oLocalidad.LocalidadId);
            }
         
            oLocalidadSave.CodLocalidad = oLocalidad.CodLocalidad;  
            oLocalidadSave.Nombre = oLocalidad.Nombre;  
            oLocalidadSave.ProvinciaId = oLocalidad.ProvinciaId; 
             
            if (oLocalidadSave.ObjectState == Constants.Object_Added)
            {
                oLocalidadSave.LocalidadId = ((mobjUnitOfWork.Repository<Localidad>().Queryable().Max(x => (int?)x.LocalidadId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Localidad>().SaveEntity(oLocalidadSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarLocalidadAsync(int intLocalidadId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Localidad>();

            var oLocalidad = await oRepository
                                 .Queryable()
                                 .Where(x => x.LocalidadId == intLocalidadId)
                                 .SingleOrDefaultAsync();

            if (oLocalidad != null)
            {
                oRepository.Delete(oLocalidad);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        public List<Localidad> ListarLocalidad(string localidad)
        {
            return mobjUnitOfWork.Repository<Localidad>().Queryable().Where(x => localidad != "" && x.Nombre.Contains(localidad)).Take(15).ToList();
        }        
    }
}




