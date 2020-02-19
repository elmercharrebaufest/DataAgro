using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

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
            var oResult = new ResultIniMaterial
            {
                Material = repositorio.Listar<Material, MaterialIni>
                       (x => new MaterialIni()
                       {
                           MaterialId = x.MaterialId,
                           Codigo = x.Codigo,
                           Descripcion = x.Descripcion
                       },
                       x => (oParam.Codigo.Trim() == "" || x.Codigo.Contains(oParam.Codigo.Trim())) &&
                                   (oParam.Descripcion.Trim() == "" || x.Descripcion.Contains(oParam.Descripcion.Trim())), 500, "Descripcion")
            };
            return oResult;
        }

        public MaterialDto TraerMaterial(int intMaterialId)
        {
            return repositorio.Obtener<Material, MaterialDto>(x => x.MaterialId == intMaterialId, x => new MaterialDto { MaterialId = x.MaterialId, CampaniaTableroId = x.CampaniaTableroId, CampañaId = x.CampañaId, Codigo = x.Codigo, Descripcion = x.Descripcion}) ?? new MaterialDto();
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
                var oMaterialSave = repositorio.Obtener<Material>(oMaterial.MaterialId);
                oMaterialSave.Codigo = oMaterial.Codigo;
                oMaterialSave.Descripcion = oMaterial.Descripcion;
                oMaterialSave.CampañaId = oMaterial.CampañaId;
                oMaterialSave.CampaniaTableroId = oMaterial.CampaniaTableroId;
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

        public DatosIniAbmMaterial TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);
            return new DatosIniAbmMaterial()
            {
                Material = qry.GetAbmMaterialCombo(),
                Campania = qry.GetAbmCampaniaCombo(),
                CampaniaTablero = qry.GetAbmCampaniaTableroCombo()
            };
        }

        public ResultIniMaterial TraerTodoMaterial()
        {
            return new ResultIniMaterial
            {
                Material = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni()
                {
                    MaterialId = x.MaterialId,
                    Descripcion = x.Descripcion,
                    Codigo = x.Codigo,
                    CampaniaActual = x.Campaña.Descripcion,
                    CampaniaIdActual = x.CampañaId ?? 0,
                    CampaniaTablero = x.CampaniaTablero.Descripcion,
                    CampaniaTableroId = x.CampaniaTableroId ?? 0
                }, null, 0, "Descripcion")
            };
        }
    }
}




