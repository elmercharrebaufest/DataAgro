using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{

    public class CampaniaActualManager : ICampaniaActualManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public CampaniaActualManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------
        public Resultado ActualizacionCampaniaActual(CampaniaActual oParam)
        {

            var oEntityErrors = new Resultado();

            int CampañaId = 0;
            
            var material = repositorio.Obtener<Material>(x => x.Codigo == oParam.Material);

            if (material == null)
            {
                oEntityErrors.Errores.Add(new ErrorMessage() { Message = "No existe el material." });
            }
            else
            {
                string[] arrAux = oParam.Campania.Split('-');
                for (int i = 0; i < arrAux.Length; i++)
                {
                    arrAux[i] = (Convert.ToInt32(arrAux[i]) + 1).ToString();
                }
                string strAux = string.Join("-", arrAux);
                var valAux = repositorio.Obtener<Campaña>(x => x.Descripcion == strAux);
                if (valAux == null)
                {
                    CampañaId = InsertarCampaña(strAux);
                }
                
                var val = repositorio.Obtener<Campaña>(x => x.Descripcion == oParam.Campania);                
                if (val == null)
                {
                    CampañaId = InsertarCampaña(oParam.Campania);
                }
                else
                {
                    CampañaId = val.CampañaId;
                }
                
                try
                {
                    repositorio.Agregar(new CampañaMaterialHistorico()
                    {
                        CampañaId = CampañaId,
                        Fecha = DateTime.Now,
                        MaterialId = material.MaterialId
                    });
                    material.CampañaId = CampañaId;
                    repositorio.GuardarCambios();
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }                
            }

            return oEntityErrors;
        }
        
        private int InsertarCampaña(string Campaña)
        {
            return repositorio.Agregar(new Campaña
            {
                Descripcion = Campaña
            }).CampañaId;
        }

    }
}
