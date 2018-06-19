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
    public class MaterialManager : IMaterialManager
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
        
        public async Task<ResultIniMaterial> TraerFiltroMaterialAsync(ParamAbmMaterial oParam)
        {
            var oResult = new ResultIniMaterial();
           
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();
            
            var query = oMaterial
                       .Where(x => (oParam.Codigo.Trim() == "" || x.Codigo.Contains(oParam.Codigo.Trim())) &&
                                   (oParam.Descripcion.Trim() == "" || x.Descripcion.Contains(oParam.Descripcion.Trim())))
                       .Take(500)
                       .OrderBy(x => x.Descripcion)
                       .Select(x => new MaterialIni()
                       {
                           MaterialId = x.MaterialId,
                           Codigo = x.Codigo,
                           Descripcion = x.Descripcion
                       });



            oResult.Material = await query.ToListAsync();

            return oResult;
        }

        public async Task<Material> TraerMaterialAsync(int intMaterialId)
        {
            var oMaterial = new Material();

            oMaterial = await mobjUnitOfWork.Repository<Material>()
                                 .Queryable()
                                 .Where(x => x.MaterialId == intMaterialId)
                                 .SingleOrDefaultAsync();

            if (oMaterial == null)
            {
                oMaterial = new Material()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oMaterial.ObjectState = Constants.Object_Modified;
            }
            
            return oMaterial;
        }


        public async Task<EntityErrors> GrabarMaterialAsync(Material oMaterial)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oMaterial, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Material oMaterialSave;

            if (oMaterial.ObjectState == 0)
            {
                oMaterialSave = new Material()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oMaterialSave = await TraerMaterialAsync(oMaterial.MaterialId);
            }
         
            oMaterialSave.Codigo = oMaterial.Codigo;  
            oMaterialSave.Descripcion = oMaterial.Descripcion;  

            if (oMaterialSave.ObjectState == Constants.Object_Added)
            {
                oMaterialSave.MaterialId = ((mobjUnitOfWork.Repository<Material>().Queryable().Max(x => (int?)x.MaterialId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Material>().SaveEntity(oMaterialSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarMaterialAsync(int intMaterialId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Material>();

            var oMaterial = await oRepository
                                 .Queryable()
                                 .Where(x => x.MaterialId == intMaterialId)
                                 .SingleOrDefaultAsync();

            if (oMaterial != null)
            {
                oRepository.Delete(oMaterial);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

   
             

    }
}




