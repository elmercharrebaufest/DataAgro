using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfiguracionBolsaManager : IConfiguracionBolsaManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;

        public ConfiguracionBolsaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado GrabarConfiguracionBolsa(ConfiguracionBolsa bolsa)
        {
            var oEntityErrors = Validar(bolsa);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            var error = new Resultado();

            try
            {
                if (bolsa.Id == 0)
                {
                    repositorio.Agregar(bolsa);
                }
                else
                {
                    var bolsaSave = repositorio.Obtener<ConfiguracionBolsa>(bolsa.Id);
                    bolsaSave.ProvinciaId = bolsa.ProvinciaId;
                    bolsaSave.DestinoId = bolsa.DestinoId;
                    bolsaSave.BolsaId = bolsa.BolsaId;

                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                error.Error(ex.Source, ex.Message);
                throw;
            }
            return error;
        }

        private Resultado Validar(ConfiguracionBolsa bolsa)
        {
            var errores = new Resultado();
            if (repositorio.Obtener<ConfiguracionBolsa>(x => x.ProvinciaId == bolsa.ProvinciaId && x.DestinoId == bolsa.DestinoId && x.Id != bolsa.Id) != null)
            {
                errores.Error("cupo", "Esta configuración ya existe");
            }
            return errores;
        }

        public KendoGrid<ConfiguracionBolsaDto> TraerTodaConfiguracionBolsa(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerConfiguracionesBolsa(request));
        }

        public ConfiguracionBolsaDto TraerConfiguracionBolsa(int id)
        {
            return repositorio.Obtener<ConfiguracionBolsa, ConfiguracionBolsaDto>(x => x.Id == id, x => new ConfiguracionBolsaDto
            {
                Id = x.Id,
                BolsaId = x.BolsaId,
                DestinoId = x.DestinoId,
                ProvinciaId = x.ProvinciaId
            });
        }

        public Resultado EliminarConfiguracionBolsa(int id)
        {
            var oEntityErrors = new Resultado();
            var tc = repositorio.Obtener<ConfiguracionBolsa>(x => x.Id == id);
            try
            {
                repositorio.Remover(tc);
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }

        public ConfiguracionBolsa TraerConfiguracionBolsaConDestinoYProcedencia(int destinoId, int ProvinciaId)
        {
            return repositorio.Obtener<ConfiguracionBolsa>(x => x.DestinoId == destinoId && x.ProvinciaId == ProvinciaId);
        }

    }
}
