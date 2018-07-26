using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business
{
    public class RiesgoComercialManager : IRiesgoComercialManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public RiesgoComercialManager(ILogger logger, IMSContextProvider oMSContextProvider, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<EntityErrors> ActualizacionDeRiesgoComercialAsync(RiesgoComercial oParam)
        {
            var oEntityErrors = new EntityErrors();

            EntityValid.ValidateAll(oParam, oEntityErrors.ListaErrores);

            if (oEntityErrors.ListaErrores.Count > 0)
            {
                return oEntityErrors;
            }

            var validacion = ValidarRiesgoComercial(oParam);

            if (validacion != null)
            {
                oEntityErrors.ListaErrores.Add(validacion);
                return oEntityErrors;
            }

            var oProveedorSave = mobjUnitOfWork.Repository<Proveedor>().Queryable()
                .Where(x => x.CUIT == oParam.CUIT).FirstOrDefault();

            if (oProveedorSave != null)
            {
                oProveedorSave.RiesgoComercialSap = oParam.RiesgoComercialDesc;

                oProveedorSave.ObjectState = Constants.Object_Modified;

                mobjUnitOfWork.Repository<Proveedor>().SaveEntity(oProveedorSave);

                await mobjUnitOfWork.SaveChangesAsync();

            }

            return oEntityErrors;
        }

        #region Validar
        public ErrorMessage ValidarRiesgoComercial(RiesgoComercial oRiesgoComercial)
        {
            var oProveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable();
            var oEstado = mobjUnitOfWork.Repository<Estado>().Queryable();

            var val = oProveedor.AsNoTracking().Where(x => x.CUIT == oRiesgoComercial.CUIT).FirstOrDefault();

            if (val == null)
            {
                return new ErrorMessage() { Message = "No existe el proveedor." };
            }

           

            return null;
        }
        #endregion
    }
}
