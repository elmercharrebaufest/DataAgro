using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;

using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Mapping.Context;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Interfaces;
using Autofac.Extras.NLog;

namespace Molinos.DataAgro.Business
{
    public class PreslipManager : IPreslipManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public PreslipManager(ILogger logger, IMSContextProvider oMSContextProvider)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<DatosIniAbmPreslip> TraerDatosInicialesAsync()
        {
            var qry = new CombosQueries(mobjUnitOfWork);

            var oDatosIniciales = new DatosIniAbmPreslip()
            {
                TipoDeNegocio = await qry.GetTipoDeNegocioComboAsync(),
                Material = await qry.GetMaterialComboAsync(),
                Campaña = await qry.GetCampañaComboAsync()
            };
            
            return oDatosIniciales;
        }


        public async Task<ResultIniPreslip> TraerTodoPreslipAsync()
        {
            var oResult = new ResultIniPreslip();
           
            var oPreslip = mobjUnitOfWork.Repository<Preslip>().Queryable();
            var oTipoDeNegocio = mobjUnitOfWork.Repository<TipoDeNegocio>().Queryable();
            var oMaterial = mobjUnitOfWork.Repository<Material>().Queryable();
            var oCampaña = mobjUnitOfWork.Repository<Campaña>().Queryable();

            var query = oPreslip
                        .Join(oTipoDeNegocio, a => a.TipoDeNegocioId, b => b.TipoDeNegocioId, (a, b) => new { PRE = a, TDN = b })
                        .Join(oMaterial, a => a.PRE.MaterialId, b => b.MaterialId, (a, b) => new { a.PRE, a.TDN, MAT = b })
                        .Join(oCampaña, a => a.PRE.CampañaId, b => b.CampañaId, (a, b) => new { a.PRE, a.TDN, a.MAT, CAM = b })
                        .OrderBy(x => x.PRE.Cantidad)
                        .Select(x => new PreslipIni()
                        {
                             PreslipId = x.PRE.PreslipId,
                             TdnDescripcion = x.TDN.Descripcion,
                             MatDescripcion = x.MAT.Descripcion,
                             CamDescripcion = x.CAM.Descripcion,
                             Cantidad = x.PRE.Cantidad,
                             Precio = x.PRE.Precio,
                             FechaDesde = x.PRE.FechaDesde,
                             FechaHasta = x.PRE.FechaHasta,
                             FechaDeEntrega = x.PRE.FechaDeEntrega,
                             ProveedorId = x.PRE.ProveedorId,
                             EstadoPreslipId = x.PRE.EstadoPreslipId
                        });

            oResult.Preslip = await query.ToListAsync();

            return oResult;
        }


        public async Task<Preslip> TraerPreslipAsync(int intPreslipId)
        {
            var oPreslip = new Preslip();

            oPreslip = await mobjUnitOfWork.Repository<Preslip>()
                                 .Queryable()
                                 .Where(x => x.PreslipId == intPreslipId)
                                 .SingleOrDefaultAsync();

            if (oPreslip == null)
            {
                oPreslip = new Preslip()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oPreslip.ObjectState = Constants.Object_Modified;
            }
            
            return oPreslip;
        }


        public async Task<EntityErrors> GrabarPreslipAsync(Preslip oPreslip)
        {
            var oEntityErrors = new EntityErrors();
                      
            EntityValid.ValidateAll(oPreslip, oEntityErrors.ListaErrores);
 
            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            Preslip oPreslipSave;

            if (oPreslip.ObjectState == 0)
            {
                oPreslipSave = new Preslip()
                {
                    ObjectState = Constants.Object_Added
                };
            }
            else
            {
                oPreslipSave = await TraerPreslipAsync(oPreslip.PreslipId);
            }
         
            oPreslipSave.TipoDeNegocioId = oPreslip.TipoDeNegocioId;  
            oPreslipSave.MaterialId = oPreslip.MaterialId;  
            oPreslipSave.CampañaId = oPreslip.CampañaId;  
            oPreslipSave.Cantidad = oPreslip.Cantidad;  
            oPreslipSave.Precio = oPreslip.Precio;  
            oPreslipSave.FechaDesde = oPreslip.FechaDesde;  
            oPreslipSave.FechaHasta = oPreslip.FechaHasta;  
            oPreslipSave.FechaDeEntrega = oPreslip.FechaDeEntrega;  
            oPreslipSave.ProveedorId = oPreslip.ProveedorId;  
            oPreslipSave.EstadoPreslipId = oPreslip.EstadoPreslipId;  
            if (oPreslipSave.ObjectState == Constants.Object_Added)
            {
                oPreslipSave.PreslipId = ((mobjUnitOfWork.Repository<Preslip>().Queryable().Max(x => (int?)x.PreslipId)) ?? 0) + 1;
            }

            mobjUnitOfWork.Repository<Preslip>().SaveEntity(oPreslipSave);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }


        public async Task<EntityErrors> EliminarPreslipAsync(int intPreslipId)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Preslip>();

            var oPreslip = await oRepository
                                 .Queryable()
                                 .Where(x => x.PreslipId == intPreslipId)
                                 .SingleOrDefaultAsync();

            if (oPreslip != null)
            {
                oRepository.Delete(oPreslip);
            }

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

   
        public Preslip NuevoPreslip()
        {
            return new Preslip();
        }
             

    }
}




