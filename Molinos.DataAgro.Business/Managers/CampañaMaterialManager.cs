using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class CampañaMaterialManager : ICampañaMaterial
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public CampañaMaterialManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        public async Task<EntityErrors> TraerCampañasPorGrano(List<CampañaMaterialSAPDTO> oCampañaMaterialSAP)
        {

            var oEntityErrors = new EntityErrors();
            var oProveedores = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking();
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking();
            var oCampañaMaterial = mobjUnitOfWork.Repository<CampañaMaterial>().Queryable().AsNoTracking();
            var oCampañaMaterialPorMes = mobjUnitOfWork.Repository<CampañaMaterialPorMes>().Queryable().AsNoTracking();
            var oComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
            int? ProveedorId;
            int? MaterialId;
            int? ComercialId;


            foreach (var camp in oCampañaMaterialSAP)
            {
                
                ProveedorId = null;
                if (!string.IsNullOrEmpty(camp.CUIT))
                {
                    var proveedor = oProveedores.FirstOrDefault(x => x.CUIT == camp.CUIT);
                    if (proveedor != null)
                    {
                        ProveedorId = proveedor.ProveedorId;
                    }
                    else
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "No existe el CUIT" });
                        return oEntityErrors;
                    }
                }

                int? CampañaId = null;
                if (!string.IsNullOrEmpty(camp.Campaña))
                {
                    var campaña = oCampaña.FirstOrDefault(x => x.Descripcion == camp.Campaña);
                    if (campaña != null)
                    {
                        CampañaId = campaña.CampañaId;
                    }
                    else
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "No existe la Campaña." });
                        return oEntityErrors;
                    }
                }

                MaterialId = null;
                if (!string.IsNullOrEmpty(camp.Material))
                {
                    var material = oMaterial.FirstOrDefault(x => x.Codigo== camp.Material);
                    if (material != null)
                    {
                        MaterialId = material.MaterialId;
                    }
                    else
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "No existe el Material." });
                        return oEntityErrors;
                    }
                }
                
                ComercialId = null;

                string Comercialaux = ConfigurationManager.AppSettings["Comercial"].ToString();
                string Comercialauxiliar = ConfigurationManager.AppSettings["ComercialAuxiliar"].ToString();

                if (!string.IsNullOrEmpty(camp.Comercial))
                {
                    var auxcomercial = string.Empty;
                    if (camp.Comercial == Comercialaux)
                    {
                        auxcomercial = Comercialauxiliar;
                    }
                    else
                    {
                        auxcomercial = camp.Comercial.ToLower().Trim();
                    }

                    var comercial = oComercial.FirstOrDefault(x => x.IdActiveDirectory.ToLower().Trim() == auxcomercial);

                    if (comercial != null)
                    {
                        ComercialId = comercial.ComercialId;
                    }
                    else
                    {
                        oEntityErrors.HayError = true;
                        oEntityErrors.ListaErrores.Add(new ErrorMessage() { Message = "No existe el comercial." });
                        return oEntityErrors;
                    }
                }

                
                var oCampañaMaterialSave = await  ObtenerCampañaIdMaterial(ProveedorId, MaterialId,CampañaId,(double)camp.Toneladas, camp.Mes, (int)camp.Año);

                var CampañaMaterialPorMes = await GuardarMes(oCampañaMaterialSave.CampañaMaterialId, camp.Mes, (double)camp.Toneladas, (int)camp.Año,(int)ComercialId);

                mobjUnitOfWork.Repository<CampañaMaterial>().SaveEntity(oCampañaMaterialSave);

                mobjUnitOfWork.Repository<CampañaMaterialPorMes>().SaveEntity(CampañaMaterialPorMes);
                try
                {
                    mobjUnitOfWork.SaveChanges();
                }
                catch (Exception ex)
                {
                    throw;
                }
                
            }

            
            return oEntityErrors;
        }


        public async Task<CampañaMaterialPorMes> GuardarMes(int CampañaMaterialId,string Mes, double Toneladas, int Año, int ComercialId)
        {
            int MesId = DevolverIdMes(Mes);
            var oCampañaMaterialMes = mobjUnitOfWork.Repository<CampañaMaterialPorMes>().Queryable();
            var oCampañaMaterialPorMesSave = oCampañaMaterialMes.FirstOrDefault(x => x.CampañaMaterialId == CampañaMaterialId
               && x.Mes == MesId && x.Año == Año && x.ComercialId == ComercialId);

            if (oCampañaMaterialPorMesSave != null)
            {
                oCampañaMaterialPorMesSave.ObjectState = Constants.Object_Modified;
                oCampañaMaterialPorMesSave.Toneladas = Toneladas;
            }
            else
            {
                var campañaMaterialPorMesId = oCampañaMaterialMes.Max(x => (int?)x.CampañaMaterialPorMesId ?? 0) + 1;

                oCampañaMaterialPorMesSave = new CampañaMaterialPorMes()
                {
                    CampañaMaterialPorMesId= campañaMaterialPorMesId,
                    CampañaMaterialId =CampañaMaterialId,
                    NroItem= 1,
                    Mes= MesId,
                    Año=Año,
                    ObjectState = Constants.Object_Added,
                    ComercialId =ComercialId,
                    Toneladas= Toneladas
                };

               

            }

            return oCampañaMaterialPorMesSave;
        }

        public async Task<CampañaMaterial> ObtenerCampañaIdMaterial(int? proveedorId,int? MaterialId,int? CampañaId,double? total,string Mes,int Año)
        {
            int MesId = DevolverIdMes(Mes);

            var oCampañaMaterial = mobjUnitOfWork.Repository<CampañaMaterial>().Queryable().AsNoTracking();
            var campañamaterial = oCampañaMaterial.FirstOrDefault(x => x.CampañaId == CampañaId
               && x.ProveedorId == proveedorId && x.MaterialId == MaterialId);

            if (campañamaterial != null)
            {
                double TotalTonelada = mobjUnitOfWork.Repository<CampañaMaterialPorMes>().Queryable().AsNoTracking().
                            Where(x => x.CampañaMaterialId == campañamaterial.CampañaMaterialId && (! (x.Mes == MesId && x.Año == Año)))
                            .Sum(x => x.Toneladas) ?? 0;
                TotalTonelada += (double)total;
                campañamaterial.ToneladasCompradas = (double)TotalTonelada;
                campañamaterial.ObjectState = Constants.Object_Modified;
                return campañamaterial;
            }
            else
            {
                var campañaMaterialId = oCampañaMaterial.Max(x => (int?)x.CampañaMaterialId ?? 0) +1;
                var oCampañaMaterialSave = new CampañaMaterial()
                {
                    CampañaId=(int)CampañaId,
                    MaterialId= (int)MaterialId,
                    ProveedorId= (int)proveedorId,
                    NroItem= 1,
                    CampañaMaterialId= campañaMaterialId,
                    ToneladasCompradas = (double)total,
                    ObjectState = Constants.Object_Added
                };

                return oCampañaMaterialSave;
            } 
        }

        private int DevolverIdMes(string mES)
        {
            switch (mES.ToLower())
            {

                case "enero":
                    return 1;
                case "febrero":
                    return 2;
                case "marzo":
                    return 3;
                case "abril":
                    return 4;
                case "mayo":
                    return 5;
                case "junio":
                    return 6;
                case "julio":
                    return 7;
                case "agosto":
                    return 8;
                case "septiembre":
                    return 9;
                case "octubre":
                    return 10;
                case "noviembre":
                    return 11;
                case "diciembre":
                    return 12;
            }
            return 1;
        }

    }

}
