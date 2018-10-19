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

    public class RangoConfirmacionAutomaticaManager : IRangoConfirmacionAutomaticaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;

        public RangoConfirmacionAutomaticaManager(ILogger logger, IRepositorio repositorio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
        }
        public DatosIniAbmRangoConfirmacionAutomatica TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAbmRangoConfirmacionAutomatica()
            {
                Material = qry.GetMaterialCombo(),
                Moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion })
            };
        }

        public ResultIniRangoConfirmacionAutomatica TraerTodoRango()
        {
            return new ResultIniRangoConfirmacionAutomatica
            {
                Rango = repositorio.Listar<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaIni>(x => new RangoConfirmacionAutomaticaIni()
                {
                    Id = x.Id,
                    PrecioMinimo = x.PrecioMinimo,
                    PrecioMaximo = x.PrecioMaximo,
                    Material = x.Material.Descripcion,
                    Moneda = x.MonedaId
                }, null, 0, "Material")
            };
        }

        public RangoConfirmacionAutomaticaDto TraerRango(int id)
        {
            return repositorio.Obtener<RangoConfirmacionAutomatica, RangoConfirmacionAutomaticaDto>(x => x.Id == id, x => new RangoConfirmacionAutomaticaDto
            {
                Id = x.Id,
                PrecioMinimo = x.PrecioMinimo,
                PrecioMaximo = x.PrecioMaximo,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Moneda = x.Moneda.Descripcion,
                MonedaId = x.MonedaId
            }) ?? new RangoConfirmacionAutomaticaDto();
        }

        public Resultado GrabarRangoConfirmacionAutomatica(RangoConfirmacionAutomatica oRango)
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
                var oRangoSave = repositorio.Obtener<RangoConfirmacionAutomatica>(oRango.Id);
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

            logger.Debug("Nuevo Rango de Confirmacion automática desde" + oRango.PrecioMinimo + " Hasta " + oRango.PrecioMaximo + " Para " + oRango.MaterialId + " en " + oRango.MonedaId);

            return oEntityErrors;
        }

        public Resultado EliminarRangoConfirmacionAutomatica(int id)
        {
            var oEntityErrors = new Resultado();

            repositorio.Remover<RangoConfirmacionAutomatica>(id);

            logger.Debug("Eliminando el Rango de Confirmacion Automatica:" + id);
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

        private Resultado ValidarRango(Resultado oEntityErrors, RangoConfirmacionAutomatica oRango)
        {
            var rangosExistentes = repositorio.Listar<RangoConfirmacionAutomatica>();
            if (oRango.PrecioMinimo > oRango.PrecioMaximo)
            {
                oEntityErrors.Error("Precio", "El valor minimo no puede ser mayor que el máximo");
            }
            if (rangosExistentes.Exists(x=> x.Id != oRango.Id && x.MaterialId == oRango.MaterialId && x.MonedaId == oRango.MonedaId))
            {
                oEntityErrors.Error("Rango", "Ya existe un rango para el grano y moneda elegidos");
            }
            return oEntityErrors;
        }
    }
}

