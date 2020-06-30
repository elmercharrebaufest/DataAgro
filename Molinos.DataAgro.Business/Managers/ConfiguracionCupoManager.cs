using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfiguracionCupoManager : IConfiguracionCupoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        public ConfiguracionCupoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado GrabarConfiguracionCupo(ConfiguracionCupo cupo)
        {
            var oEntityErrors = Validar(cupo);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                if (cupo.Id == 0)
                {
                    repositorio.Agregar(cupo);
                }
                else
                {
                    var cupoSave = repositorio.Obtener<ConfiguracionCupo>(cupo.Id);
                    cupoSave.MaterialId = cupo.MaterialId;
                    cupoSave.CentroId = cupo.CentroId;
                    cupoSave.MaterialId = cupo.MaterialId;
                    cupoSave.LimiteCupo = cupo.LimiteCupo;
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            return oEntityErrors;
        }
        private Resultado Validar(ConfiguracionCupo cupo) {
            var errores = new Resultado();

            //if (cupo.MaterialId == 0)
            //{
            //    errores.Error("cupo", "Seleccione un Material");
            //}
            //if (cupo.CentroId == 0)
            //{
            //    errores.Error("cupo", "Seleccione un Centro");
            //}
            if (cupo.Fecha == new DateTime())
            {
                errores.Error("cupo", "La fecha no puede estar vacia");
            }
            if (cupo.Id == 0)
            {
                if (repositorio.Existe<ConfiguracionCupo>(x => x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.Fecha == cupo.Fecha))
                {
                    errores.Error("cupo", "Ya existe configuración para ese Material, Centro y Fecha");
                }
            }
            else
            {
                var limites = repositorio.Listar<LimiteCupo, LimiteCupoDto>(x => new LimiteCupoDto { Id = x.Id, CantidadCupo = x.CantidadCupo }, x => x.ConfiguracionCupoId == cupo.Id);
                if (limites != null && cupo.LimiteCupo < limites.Sum(x=>x.CantidadCupo))
                {
                    errores.Error("cupo", "La cantidad no debe ser menor a la configuración ya cargada");
                }
            }
            return errores;
        }
        public KendoGrid<ConfiguracionCupoDto> TraerTodaConfiguracionCupo(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerConfiguracionesCupo(request));
        }
        public List<LimiteCupoDto> TraerLimites(int id)
        {
            return repositorio.Listar<LimiteCupo, LimiteCupoDto>(x => new LimiteCupoDto
            {
                Id = x.Id,
                ZonaCupo = x.ZonaCupo.CodigoSap,
                ZonaCupoId = x.ZonaCupoId,
                CantidadCupo = x.CantidadCupo
            }, x => x.ConfiguracionCupoId == id);
        }
        public Resultado GrabarLimites(List<LimiteCupo> limite)
        {
            var oEntityErrors = ValidarLimite(limite);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                foreach(var lim in limite)
                {
                    if(lim.Id == 0)
                    {
                        repositorio.Agregar(lim);
                    }
                    else
                    {
                        var limSave = repositorio.Obtener<LimiteCupo>(lim.Id);
                        limSave.CantidadCupo = lim.CantidadCupo;
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            return oEntityErrors;
        }
        private Resultado ValidarLimite(List<LimiteCupo> limite)
        {
            var resultado = new Resultado();
            var config = repositorio.Obtener<ConfiguracionCupo>(limite[0].ConfiguracionCupoId);
            if (limite.Sum(x=>x.CantidadCupo)> config.LimiteCupo)
            {
                resultado.Error("cantidad", "La cantidad de cupos excede el limite cargado");
            }
            if (limite.Sum(x => x.CantidadCupo) < config.LimiteCupo)
            {
                resultado.Error("cantidad", "La cantidad de cupos no alcanza el limite cargado");
            }
            return resultado;
        }
        public ConfiguracionCupoDto TraerConfiguracionCupo(int id)
        {
            return repositorio.Obtener<ConfiguracionCupo, ConfiguracionCupoDto>(x => x.Id == id, x => new ConfiguracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                MaterialId = x.MaterialId,
                CentroId = x.CentroId,
                LimiteCupo = x.LimiteCupo
            });
        }

        public List<ConfiguracionCupoDto> TraerTodaConfiguracionCupoPorDia(int zona)
        {
            var hoy = DateTime.Today;
            var cantidadCuposGenerados = (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaGeneracion) == hoy && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9)).Count;

            var limitePorZona = repositorio.Listar<LimiteCupo, ConfiguracionCupoDto>(x => new ConfiguracionCupoDto
            {
                Id = x.Id,
                Fecha = x.ConfiguracionCupo.Fecha,
                MaterialId = x.ConfiguracionCupo.MaterialId,
                CentroId = x.ConfiguracionCupo.CentroId,
                LimiteCupo = x.ConfiguracionCupo.LimiteCupo
            }, x => x.ConfiguracionCupo.Fecha == hoy && x.ZonaCupoId == zona && (x.ConfiguracionCupo.LimiteCupo - cantidadCuposGenerados) >= 0);

            if (limitePorZona.Count() == 0)
            {
                var limitePorCantidadCupo = repositorio.Listar<ConfiguracionCupo, ConfiguracionCupoDto>(x => new ConfiguracionCupoDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    MaterialId = x.MaterialId,
                    CentroId = x.CentroId,
                    LimiteCupo = x.LimiteCupo
                }, x => x.Fecha == hoy && (x.LimiteCupo - cantidadCuposGenerados) >= 0);

                return limitePorCantidadCupo;
            }
            return limitePorZona;
        }
    }
}
