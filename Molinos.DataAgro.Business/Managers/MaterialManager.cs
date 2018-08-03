using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class MaterialManager : IMaterialManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public MaterialManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public ResultIniMaterial TraerFiltroMaterial(ParamAbmMaterial oParam)
        {
            var oResult = new ResultIniMaterial();

            oResult.Material = repositorio.Listar<Material, MaterialIni>
                       (x => new MaterialIni()
                       {
                           MaterialId = x.MaterialId,
                           Codigo = x.Codigo,
                           Descripcion = x.Descripcion
                       },
                       x => (oParam.Codigo.Trim() == "" || x.Codigo.Contains(oParam.Codigo.Trim())) &&
                                   (oParam.Descripcion.Trim() == "" || x.Descripcion.Contains(oParam.Descripcion.Trim())), 500, "Descripcion");

            return oResult;
        }

        public Material TraerMaterial(int intMaterialId)
        {
            return repositorio.Obtener<Material>(intMaterialId) ?? new Material();
        }


        public Resultado GrabarMaterial(Material oMaterial)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oMaterial, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oMaterial.MaterialId != 0)
            {
                var oMaterialSave = TraerMaterial(oMaterial.MaterialId);
                oMaterialSave.Codigo = oMaterial.Codigo;
                oMaterialSave.Descripcion = oMaterial.Descripcion;
            }
            else
            {
                repositorio.Agregar(oMaterial);
            }
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }


        public Resultado EliminarMaterial(int intMaterialId)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<Material>(intMaterialId);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            return oEntityErrors;
        }




    }
}




