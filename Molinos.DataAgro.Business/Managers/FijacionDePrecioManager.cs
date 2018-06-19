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
    public class FijacionDePrecioManager : IFijacionDePrecioManager
    {
        //--------------------------------------------------
        //  Variables Privadas
        //--------------------------------------------------

        private MSContext mobjContexto;
        private IUnitOfWorkAsync mobjUnitOfWork;

        //--------------------------------------------------
        //  Inicializacion
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

        public async Task<DatosIniAbmFijacionDePrecio> TraerDatosInicialesAsync()
        {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAbmFijacionDePrecio()
            {
                Material = await qry.GetMaterialComboAsync()
            };
            
            return oDatosIniciales;
        }


        public async Task<ResultIniFijacionDePrecio> TraerTodoFijacionDePrecioAsync()
        {
            var oResult = new ResultIniFijacionDePrecio();
           
            var oFijacionDePrecio = mobjUnitOfWork.Repository<FijacionDePrecio>().Queryable();
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();

            var query = oFijacionDePrecio
                        .Join(oMaterial, a => a.MaterialId, b => b.MaterialId, (a, b) => new { FDP = a, MAT = b })
                        .OrderBy(x => x.FDP.Precio)
                        .Select(x => new FijacionDePrecioIni()
                        {
                             FijacionId = x.FDP.FijacionId,
                             MatDescripcion = x.MAT.Descripcion,
                             Precio = x.FDP.Precio,
                             Fecha = x.FDP.Fecha,
                             ProveedorId = x.FDP.ProveedorId
                        });

            oResult.FijacionDePrecio = await query.ToListAsync();

            return oResult;
        }


        public async Task<FijacionDePrecio> TraerFijacionDePrecioAsync(int intFijacionId)
        {
            var oFijacionDePrecio = new FijacionDePrecio();

            oFijacionDePrecio = await mobjUnitOfWork.Repository<FijacionDePrecio>()
                                 .Queryable()
                                 .Where(x => x.FijacionId == intFijacionId)
                                 .SingleOrDefaultAsync();

            if (oFijacionDePrecio == null)
            {
                oFijacionDePrecio = new FijacionDePrecio()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oFijacionDePrecio.ObjectState = Constants.Object_Modified;
            }
            
            return oFijacionDePrecio;
        }


        public async Task<EntityErrors> GrabarFijacionDePrecioAsync(FijacionDePrecio oFijacionDePrecio, string idActiveDirectory)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oFijacionDePrecio, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            FijacionDePrecio oFijacionDePrecioSave;

            if (oFijacionDePrecio.ObjectState == 0)
            {
                oFijacionDePrecioSave = new FijacionDePrecio()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oFijacionDePrecioSave = await TraerFijacionDePrecioAsync(oFijacionDePrecio.FijacionId);
            }
         
            oFijacionDePrecioSave.MaterialId = oFijacionDePrecio.MaterialId;  
            oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;  
            oFijacionDePrecioSave.Fecha = oFijacionDePrecio.Fecha;  
            oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;  
            if (oFijacionDePrecioSave.ObjectState == Constants.Object_Added)
            {
                oFijacionDePrecioSave.FijacionId = ((mobjUnitOfWork.Repository<FijacionDePrecio>().Queryable().Max(x => (int?)x.FijacionId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<FijacionDePrecio>().SaveEntity(oFijacionDePrecioSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<FijacionDePrecio>();

            var oFijacionDePrecio = await oRepository
                                 .Queryable()
                                 .Where(x => x.FijacionId == intFijacionId)
                                 .SingleOrDefaultAsync();

            if (oFijacionDePrecio != null)
            {
                oRepository.Delete(oFijacionDePrecio);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

   
        public FijacionDePrecio NuevoFijacionDePrecio()
        {
            return new FijacionDePrecio();
        }
             

    }
}




