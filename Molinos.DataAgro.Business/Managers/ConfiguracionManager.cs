using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public partial class ConfiguracionManager : IConfiguracionManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;
        public ConfiguracionManager(IRepositorio repositorio, ILogger logger)
        {
            this.repositorio = repositorio;
            this.logger = logger;
        }
        public Resultado GrabarFechaPesificacionDolarizado(Configuracion oConfiguracion)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oConfiguracion, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            var oConfiguracionSave = TraerConfiguraciones();
            if (oConfiguracionSave != null)
            {               
                oConfiguracionSave.CantidadDias = oConfiguracion.CantidadDias;
                oConfiguracionSave.ClaveStop = oConfiguracion.ClaveStop;
                oConfiguracionSave.ConexionABMStop = oConfiguracion.ConexionABMStop;
                oConfiguracionSave.ConexionConsultaStop = oConfiguracion.ConexionConsultaStop;
                oConfiguracionSave.TerminalStopId = oConfiguracion.TerminalStopId;
                oConfiguracionSave.CuitDestinoStop = oConfiguracion.CuitDestinoStop;
                oConfiguracionSave.CodigoLocalidadStop = oConfiguracion.CodigoLocalidadStop;
            }

            else
            {
                repositorio.Agregar(oConfiguracion);
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

            logger.Debug("Se guardó correctamente");

            return oEntityErrors;
        }

        public Configuracion TraerConfiguraciones()
        {
            return repositorio.Obtener<Configuracion>(1);

        }
    }
}
