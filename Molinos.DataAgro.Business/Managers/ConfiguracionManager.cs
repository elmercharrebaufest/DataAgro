using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;

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
                oConfiguracionSave.ContratoAperturaPrecioPorcentajeDeComisionMaximo = oConfiguracion.ContratoAperturaPrecioPorcentajeDeComisionMaximo;
                oConfiguracionSave.ImporteSustentable = oConfiguracion.ImporteSustentable;
                oConfiguracionSave.DiasDiferimiento = oConfiguracion.DiasDiferimiento;
                oConfiguracionSave.ImporteSustentable = oConfiguracion.ImporteSustentable;
                oConfiguracionSave.CantidadAcuerdo = oConfiguracion.CantidadAcuerdo;
                oConfiguracionSave.CantidadDiasDolarizadoLimiteMaximo = oConfiguracion.CantidadDiasDolarizadoLimiteMaximo;
                oConfiguracionSave.CantidadMaxima = oConfiguracion.CantidadMaxima;
                oConfiguracionSave.CantidadDiasPesificadoLimite = oConfiguracion.CantidadDiasPesificadoLimite;
                oConfiguracionSave.RedespachoMaximoUSDM = oConfiguracion.RedespachoMaximoUSDM;
                oConfiguracionSave.RedespachoMaximoARP = oConfiguracion.RedespachoMaximoARP;
                oConfiguracionSave.ToleranciaPaseMax = oConfiguracion.ToleranciaPaseMax;
                oConfiguracionSave.ToleranciaPaseMin = oConfiguracion.ToleranciaPaseMin;
                oConfiguracionSave.ImporteSustentableEspecial = oConfiguracion.ImporteSustentableEspecial;
                oConfiguracionSave.AlgoritmoKilosMinimosParaSugerencia = oConfiguracion.AlgoritmoKilosMinimosParaSugerencia;
                oConfiguracionSave.AlgoritmoProcMaxSugerenciasProveedorDia = oConfiguracion.AlgoritmoProcMaxSugerenciasProveedorDia;
                oConfiguracionSave.Actualizacion = oConfiguracion.Actualizacion;
                oConfiguracionSave.ApiKeyBolsaRosario = oConfiguracion.ApiKeyBolsaRosario;
                oConfiguracionSave.SecretBolsaRosario = oConfiguracion.SecretBolsaRosario;
                oConfiguracionSave.MinutosCronometroConDescarga = oConfiguracion.MinutosCronometroConDescarga;
                oConfiguracionSave.CantidadMaximaDiasNegocioConDescarga = oConfiguracion.CantidadMaximaDiasNegocioConDescarga;
                oConfiguracionSave.PorcentajeVolumenNegocioConDescarga = oConfiguracion.PorcentajeVolumenNegocioConDescarga;
                oConfiguracionSave.ExigirNegocioEnSolExt = oConfiguracion.ExigirNegocioEnSolExt;
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

        public Resultado SetExigirNegocioEnSolExt(bool valor)
        {
            var resultado = new Resultado();
            var config = TraerConfiguraciones();

            if (config != null)
            {
                config.ExigirNegocioEnSolExt = valor;
            }
            else
            {
                resultado.Error("Error:E404", "Entidad Configuracion No Encontrada");
            }

            try
            {
                repositorio.GuardarCambios();
                logger.Debug("Se guardó correctamente");
            }
            catch (Exception ex)
            {
                resultado.Error("Error:E000", "Guardado incorrecto"+ex.Message);
                logger.Error(ex);
                throw;
            }

            return resultado;
        }
    }
}