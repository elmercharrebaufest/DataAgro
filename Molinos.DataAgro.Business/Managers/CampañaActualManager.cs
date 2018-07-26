using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class CampañaActualManager : ICampañaActualManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CampañaActualManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
        public async Task<EntityErrors> ActualizacionCampañaActualAsync(CampañaActual oParam)
        {

            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oParam, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            int CampañaId = 0;

            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();

            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable();

            var material = oMaterial.AsNoTracking().Where(x => x.Codigo == oParam.Material).FirstOrDefault();

            if (material == null)
            { 
                oEntityErrors.HayError = true;
                oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "No existe el material." });
            }
            else
            {
                string[] arrAux = oParam.Campaña.Split('-');
                for (int i = 0; i < arrAux.Length; i++)
                {
                    arrAux[i] = (Convert.ToInt32(arrAux[i]) + 1).ToString();
                }
                string strAux = string.Join("-",arrAux);
                var valAux = oCampaña.AsNoTracking().Where(x => x.Descripcion == strAux).FirstOrDefault();
                if (valAux == null)
                    CampañaId = InsertarCampaña(strAux);


                var val = oCampaña.AsNoTracking().Where(x => x.Descripcion == oParam.Campaña).FirstOrDefault();
                

                if (val == null)
                    CampañaId = InsertarCampaña(oParam.Campaña);
                else
                    CampañaId = val.CampañaId;


                var oCampañaMaterialHistorico = mobjUnitOfWork.Repository<CampañaMaterialHistorico>().Queryable();
                int CampañaMaterialHistoricoId = oCampañaMaterialHistorico.AsNoTracking().Select(x => x.CampañaMaterialHistoricoId)
                                                    .DefaultIfEmpty(0)
                                                    .Max();
                CampañaMaterialHistoricoId += 1;
                var oCampañaHistorico = new CampañaMaterialHistorico()
                {

                    ObjectState = Constants.Object_Added,
                    CampañaMaterialHistoricoId = CampañaMaterialHistoricoId ,
                    CampañaId = CampañaId,
                    Fecha = DateTime.Now,
                    MaterialId = material.MaterialId
                };

                try
                {
                    mobjUnitOfWork.Repository<CampañaMaterialHistorico>().SaveEntity(oCampañaHistorico);
                    mobjUnitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw; 
                }


                ActualizarCampañaActual(CampañaId, material.MaterialId);
            }

            return oEntityErrors;
        }

        private void ActualizarCampañaActual(int campañaId, int materialId)
        {
            var oMaterialSave = mobjUnitOfWork.Repository<Material>().Queryable()
               .Where(x => x.MaterialId== materialId).FirstOrDefault();

            if (oMaterialSave != null)
            {
                oMaterialSave.CampañaId = campañaId;

                oMaterialSave.ObjectState = Constants.Object_Modified;

                mobjUnitOfWork.Repository<Material>().SaveEntity(oMaterialSave);

                mobjUnitOfWork.SaveChanges();

            }
        }

        private int InsertarCampaña(string Campaña)
        {
            var oCamp = mobjUnitOfWork.Repository<Campaña>().Queryable();

            int CampañaId = oCamp.Max(x=> x.CampañaId) + 1 ;

            var oCampaña = new Campaña()
            {
                ObjectState = Constants.Object_Added,
                Descripcion = Campaña,
                CampañaId = CampañaId
            };

            try
            {
                mobjUnitOfWork.Repository<Campaña>().SaveEntity(oCampaña);

                mobjUnitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                var a = 1;
            }

            return CampañaId;
            
        }

    }
}
