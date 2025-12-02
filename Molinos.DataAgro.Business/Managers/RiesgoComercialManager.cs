using NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class RiesgoComercialManager : IRiesgoComercialManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public RiesgoComercialManager(ILogger logger, IRepositorio repositorio)
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

            var oProveedorSave = repositorio.Obtener<Proveedor>(x => x.CUIT == oParam.CUIT);

            if (oProveedorSave != null)
            {
                oProveedorSave.RiesgoComercialSap = oParam.RiesgoComercialDesc;

                repositorio.GuardarCambios();
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("No existe el proveedor"));
            }

            return oEntityErrors;
        }
    }

}
