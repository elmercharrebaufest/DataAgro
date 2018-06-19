using Mastersoft.Framework.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mastersoft.Framework.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using System.Data.Entity;
using Molinos.DataAgro.Interfaces;

namespace Molinos.DataAgro.Business
{
    public class RiesgoComercialManager : IRiesgoComercialManager
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
