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
    public class CampañaManager : ICampañaManager
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

        
        public async Task<Campaña>TraerCampaniaAsync(int campaniaId)
        {
            var campania = mobjUnitOfWork.Repository<Campaña>().Queryable();

            var query = await campania.Where(z => z.CampañaId == campaniaId).SingleOrDefaultAsync();

            return query;
        }

        public async Task<List<Campaña>> TraerCampañasActivas()
        {
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();
            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable();
            try
            {
                var query = oMaterial
                            .Join(oCampaña, a => a.CampañaId, b => b.CampañaId, (a, b) => new { M = a, CAMP = b })
                            .Select(x => x.CAMP )
                            .Distinct()
                            .OrderByDescending(x => x.CampañaId)
                               .ToList();



                return query;
            }
            catch (Exception ex)
            {
                var a = ex;
            }

            return new List<Campaña>();
        }
 

        public async Task<List<Material>> TraerMaterialPorCampaña(int CampañaId)
        {
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();

            var query = await oMaterial.Where(z=> z.CampañaId == CampañaId).ToListAsync();

            return query;

        }

        public async Task<CampañaHome> TraerCampañaHomeAsync(int idComercial)
        {
            var list = new CampañaHome();

            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable();
            var oCampañaMaterial = mobjUnitOfWork.Repository<CampañaMaterial>().Queryable();
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();
            var oProveedorComercial= mobjUnitOfWork.Repository<ProveedorComercial>().Queryable();


            //var query = 
            /*
            var query = oCampañaMaterial
                        .Join(oMaterial, a => new { a.MaterialId,a.CampañaId }, b => new { b.MaterialId, CampañaId= (int)b.CampañaId },(a, b) => new {  CAM = a, M = b })
                        .Join(oCampaña, a =>  a.M.CampañaId, b => b.CampañaId, (a, b) => new { a.CAM,a.M , C = b })
                        .Join(oProveedorComercial, a=> a.CAM.ProveedorId,b => b.ProveedorId, (a, b) => new { a.CAM, a.M,a.C, CP = b })
                        .Where(x=> x.CP.ComercialId == idComercial)
                        .OrderByDescending(x => x.CAM.CampañaId)
                        .GroupBy(x=> new { x.M.Descripcion})                        
                        .Select(x => new MaterialCampaña()
                        {
                            Nombre = x.Key.Descripcion,
                            Toneladas = x.Sum(p=> p.CAM.ToneladasCompradas),
                            Campaña = x.Max(p=> p.C.Descripcion)
                        });
            */
            var lista = new List<MaterialCampaña>();
            try
            { 
                //lista = query.ToList();
                lista = mobjUnitOfWork.SelStore<MaterialCampaña>("DataAgro_ComprasHomeTraer", idComercial).ToList();
            }
            catch (Exception ex)
            {
                var a = ex;
            }



            if (lista.Count > 5)
            {
                var listReformulada = lista.OrderByDescending(x => x.Toneladas).Take(4);

                list.Materiales = listReformulada.ToList();

                double total = 0;

                foreach (var valor in lista)
                {
                    if (!listReformulada.Any(x => x.Nombre == valor.Nombre))
                    {
                        total += valor.Toneladas;
                    }
                }

                list.Materiales.Add(new MaterialCampaña() { Nombre = "Otros", Toneladas = total });
            }
            else
                list.Materiales = lista.OrderByDescending(x => x.Toneladas).ToList();


            return list;
        }

        public async Task<List<Campaña>> TraerCampañasPorGrano(int MaterialId)
        {
            var oCampañaMaterialHistorico = mobjUnitOfWork.Repository<CampañaMaterialHistorico>().Queryable().AsNoTracking();
            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking();
            var campaña = await oCampañaMaterialHistorico
                .Join(oCampaña, a => a.CampañaId, b => b.CampañaId, (a, b) => new { H = a, CAMP = b })
                .OrderByDescending(x=> x.CAMP.CampañaId)
                .Where(x => x.H.MaterialId == MaterialId)
                .Select(x => x.CAMP).ToListAsync();
            return campaña;
        }



        public async Task<List<Campaña>> TraerCampañaPorMaterial(int MaterialId)
        {
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking().FirstOrDefault(x => x.MaterialId == MaterialId);
            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking();

            int? CampañaId = null;
            if (oMaterial != null)
            {
                CampañaId = oMaterial.CampañaId;
                var campañas = oCampaña.Where(x => x.CampañaId >= CampañaId).OrderByDescending(x=> x.CampañaId).ToList();
                return campañas;
            }
            else
            {
                return new List<Campaña>();
            }
        }


        public CampañaMaterial TraerCampañaMaterial(int CampañaId, int proveedorId, int materialId)
        {
            var CampañaMaterial = new CampañaMaterial();

            CampañaMaterial = mobjUnitOfWork.Repository<CampañaMaterial>()
                                 .Queryable()
                                 .Where(x => x.CampañaId == CampañaId && x.ProveedorId == proveedorId && x.MaterialId == materialId)
                                 .SingleOrDefault();

            if (CampañaMaterial == null)
            {
                CampañaMaterial = new CampañaMaterial()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                CampañaMaterial.ObjectState = Constants.Object_Modified;
            }

            return CampañaMaterial;
        }

        //public Task<CampañaHome> TraerCampañaHomeAsync()
        //{
        //    throw new NotImplementedException();
        //}
    }
    
}
