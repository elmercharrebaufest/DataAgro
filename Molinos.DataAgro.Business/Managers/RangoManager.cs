using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class RangoManager : IRangoManager
    { 
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public RangoManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmRango TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAbmRango()
            {
                Material = qry.GetMaterialCombo(),
                Moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion })
        };
        }

        public ResultIniRango TraerTodoRango()
        {
            return new ResultIniRango
            {
                Rango = repositorio.Listar<RangoPrecio, RangoIni>(x => new RangoIni()
                {
                    Id = x.Id,
                    PrecioMinimo = x.PrecioMinimo,
                    PrecioMaximo = x.PrecioMaximo,
                    Material = x.Material.Descripcion,
                    Moneda = x.MonedaId
                }, null, 0, "Material")
            };
        }

        public RangoPrecioDto TraerRango(int id)
        {
            return repositorio.Obtener<RangoPrecio, RangoPrecioDto>(x => x.Id == id, x => new RangoPrecioDto
            {
                Id = x.Id,
                PrecioMinimo = x.PrecioMinimo,
                PrecioMaximo = x.PrecioMaximo,
                Material = x.Material.Descripcion,
                MaterialId=x.MaterialId,
                Moneda = x.Moneda.Descripcion,
                MonedaId =x.MonedaId
            }) ?? new RangoPrecioDto();
        }

        public Resultado GrabarRango(RangoPrecio oRango)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oRango, oEntityErrors);
            ValidarRango(oEntityErrors, oRango);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            if (oRango.Id != 0)
            {
                var oRangoSave = repositorio.Obtener<RangoPrecio>(oRango.Id);
                oRangoSave.PrecioMinimo = oRango.PrecioMinimo;
                oRangoSave.PrecioMaximo = oRango.PrecioMaximo;
                oRangoSave.MaterialId = oRango.MaterialId;
                oRangoSave.MonedaId = oRango.MonedaId;
            }
            else
            {
                repositorio.Agregar(oRango);
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

            logger.Debug("Nuevo Rango de Precio desde" + oRango.PrecioMinimo + " Hasta " + oRango.PrecioMaximo + " Para " + oRango.MaterialId + " en " + oRango.MonedaId);

            return oEntityErrors;
        }

        public Resultado EliminarRango(int id)
        {
            var oEntityErrors = new Resultado(); 

            repositorio.Remover<RangoPrecio>(id);

            logger.Debug("Eliminando el Rango:" + id);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }

        private Resultado ValidarRango(Resultado oEntityErrors,RangoPrecio oRango)
        {
            var rangosExistentes = repositorio.Listar<RangoPrecio>();
            if (oRango.PrecioMaximo == 0)
            {
                oEntityErrors.Error("PrecioMaximo", "El valor máximo no puede ser cero");
            }
            if (oRango.MonedaId == null)
            {
                oEntityErrors.Error("Moneda", "El campo Moneda no puede estar vacío");
            }
            if (oRango.PrecioMinimo > oRango.PrecioMaximo)
            {
                oEntityErrors.Error("Precio", "El valor mínimo no puede ser mayor que el máximo");
            }
            if (oRango.MaterialId == 0)
            {
                oEntityErrors.Error("Material", "El campo Material no puede estar vacío");
            }
            if (rangosExistentes.Exists(x=> x.Id != oRango.Id && x.MaterialId == oRango.MaterialId && x.MonedaId == oRango.MonedaId))
            {
                oEntityErrors.Error("Rango", "Ya existe un rango para el grano y moneda elegidos");
            }
            return oEntityErrors;
        }
    }
}

