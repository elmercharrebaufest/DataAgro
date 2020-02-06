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
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfiguracionEspacioDinamicoManager : IConfiguracionEspacioDinamicoManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        public ConfiguracionEspacioDinamicoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }

        public Resultado GrabarConfiguracionEspacioDinamico(ConfiguracionEspacioDinamico espacioDinamico)
        {
            var oEntityErrors = Validar(espacioDinamico);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                if (espacioDinamico.Id == 0)
                {
                    repositorio.Agregar(espacioDinamico);
                }
                else
                {
                    var espacioDinamicoSave = repositorio.Obtener<ConfiguracionEspacioDinamico>(espacioDinamico.Id);
                    espacioDinamicoSave.MaterialId = espacioDinamico.MaterialId;
                    espacioDinamicoSave.CentroId = espacioDinamico.CentroId;
                    espacioDinamicoSave.MaterialId = espacioDinamico.MaterialId;
                    espacioDinamicoSave.CantidadDeCupo = espacioDinamico.CantidadDeCupo;
                    espacioDinamicoSave.ProveedorId = espacioDinamico.ProveedorId;
                    espacioDinamicoSave.ComercialId = espacioDinamico.ComercialId;
                    espacioDinamicoSave.Calidad = espacioDinamico.Calidad;

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
        private Resultado Validar(ConfiguracionEspacioDinamico espacioDinamico)
        {
            var errores = new Resultado();

            if (espacioDinamico.MaterialId == 0)
            {
                errores.Error("espaciodinamico", "seleccione un material");
            }
            if (espacioDinamico.MaterialId == 3 && espacioDinamico.Calidad == "")
            {
                errores.Error("espaciodinamico", "seleccione una calidad");
            }
            if (espacioDinamico.ComercialId == 0)
            {
                errores.Error("espaciodinamico", "seleccione un Comercial");
            }
            if (espacioDinamico.CantidadDeCupo <= 0)
            {
                errores.Error("espaciodinamico", "la cantidad de cupos tiene que ser mayor a 0");
            }
            if (espacioDinamico.ProveedorId == 0)
            {
                errores.Error("espaciodinamico", "seleccione un proveedor");
            }
            if (espacioDinamico.CentroId == 0)
            {
                errores.Error("espaciodinamico", "seleccione un centro");
            }
            if (espacioDinamico.Fecha == new DateTime())
            {
                errores.Error("espacioDinamico", "La fecha no puede estar vacia");
            }
            if (espacioDinamico.Id == 0)
            {
                if (repositorio.Existe<ConfiguracionEspacioDinamico>(x => x.ComercialId == espacioDinamico.ComercialId && x.ProveedorId == espacioDinamico.ProveedorId && x.CentroId == espacioDinamico.CentroId && x.MaterialId == espacioDinamico.MaterialId && x.Fecha == espacioDinamico.Fecha))
                {
                    errores.Error("espacioDinamico", "Ya existe configuración para ese Comercial,Proveedor, Material, Centro y Fecha");
                }
            }
            return errores;
        }
        public KendoGrid<ConfiguracionEspacioDinamicoDto> TraerTodaConfiguracionEspacioDinamico(KendoGridMvcRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerConfiguracionesEspacioDinamico(request));
        }
        public ConfiguracionEspacioDinamicoDto TraerConfiguracionEspacioDinamico(int id)
        {
            return repositorio.Obtener<ConfiguracionEspacioDinamico, ConfiguracionEspacioDinamicoDto>(x => x.Id == id, x => new ConfiguracionEspacioDinamicoDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                MaterialId = x.MaterialId,
                CentroId = x.CentroId,
                ProveedorId = x.Proveedor.ProveedorId,
                ProveedorCUIT = x.Proveedor.CUIT,
                ProveedorRazonSocial = x.Proveedor.RazonSocial,
                CantidadDeCupo = x.CantidadDeCupo,
                ComercialId = x.ComercialId,
                ComercialNombre = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                Calidad = x.Calidad
            });
        }

        public Resultado EliminarConfiguracionEspacioDinamico(int id)
        {
            ConfiguracionEspacioDinamico conf = repositorio.Obtener<ConfiguracionEspacioDinamico>(id);
            Resultado oEntityErrors = ValidarEliminar(id);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                repositorio.Remover<ConfiguracionEspacioDinamico>(conf);
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

        private Resultado ValidarEliminar(int id)
        {
            var errores = new Resultado();
            bool existe = repositorio.Existe<Cupo>(a => a.ConfiguracionEspacioDinamicoId == id);
            if (existe)
            {
                errores.Error("espacioDinamico", "No se puede eliminar este Espacio ya fue usado.");
            }
            return errores;
        }


    }
}
