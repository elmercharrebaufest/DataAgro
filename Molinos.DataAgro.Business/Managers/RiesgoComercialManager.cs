using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

namespace Molinos.DataAgro.Business
{
    public class RiesgoComercialManager : IRiesgoComercialManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public RiesgoComercialManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public Resultado ActualizacionDeRiesgoComercial(RiesgoComercial oParam)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oParam, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            var oProveedorSave = repositorio.Obtener<Proveedor>(x => x.CUIT == oParam.CUIT);

            if (oProveedorSave == null)
            {
                oEntityErrors.Error("Proveedor", "No existe el proveedor.");
                return oEntityErrors;
            }
            else
            {
                oProveedorSave.RiesgoComercialSap = oParam.RiesgoComercialDesc;

                try
                {
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
    }
}
