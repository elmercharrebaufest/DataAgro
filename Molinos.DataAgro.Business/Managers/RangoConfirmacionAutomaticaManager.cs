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
                Moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }),
                Zona= repositorio.Listar<GrupoDeCompras, ZonaQry>(x => new ZonaQry() { Id = x.Id, Descripcion = x.Descripcion })
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
                    Moneda = x.MonedaId,
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    Cantidad = x.Cantidad,
                    EntregaDesde = x.DesdeMes + "/" + x.DesdeAnio,
                    EntregaHasta = x.HastaMes + "/" + x.HastaAnio,
                    Zona = x.Zona != null ? x.Zona.Descripcion : ""
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
                MonedaId = x.MonedaId,
                FechaDesde = x.FechaDesde,
                HastaAnio=x.HastaAnio,
                ZonaId=x.ZonaId ?? 0,
                Zona=x.Zona.Descripcion,
                Cantidad=x.Cantidad,
                DesdeAnio=x.DesdeAnio,
                DesdeMes=x.DesdeMes,
                HastaMes=x.HastaMes,
                FechaHasta=x.FechaHasta
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
                oRangoSave.FechaDesde = oRango.FechaDesde;
                oRangoSave.FechaHasta = oRango.FechaHasta;
                oRangoSave.ZonaId = oRango.ZonaId;
                oRangoSave.Cantidad = oRango.Cantidad;
                oRangoSave.DesdeMes = oRango.DesdeMes;
                oRangoSave.HastaMes = oRango.HastaMes;
                oRangoSave.DesdeAnio = oRango.DesdeAnio;                
                oRangoSave.HastaAnio = oRango.HastaAnio;                
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

            logger.Debug("Nuevo Rango de Confirmacion automática desde" + oRango.PrecioMinimo + " Hasta " + oRango.PrecioMaximo + " Para " + oRango.MaterialId + " en " + oRango.MonedaId + " desde el" + oRango.FechaDesde);

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
            if (oRango.PrecioMaximo == 0)
            {
                oEntityErrors.Error("PrecioMaximo", "El valor máximo no puede ser cero");
            }
            if (oRango.PrecioMinimo > oRango.PrecioMaximo)
            {
                oEntityErrors.Error("Precio", "El valor minimo no puede ser mayor que el máximo");
            }
            if (oRango.MonedaId == null)
            {
                oEntityErrors.Error("Moneda", "El campo Moneda no puede estar vacío");
            }
            if (oRango.FechaDesde == null|| oRango.FechaHasta == null)
            {
                oEntityErrors.Error("FechaDesde", "Los campos Fecha Desde-Hasta no pueden estar vacíos");
            }
            if (oRango.MaterialId == 0)
            {
                oEntityErrors.Error("Material", "El campo Material no puede estar vacío");
            }
            if (oRango.Cantidad == 0)
            {
                oEntityErrors.Error("cantidad", "El campo Cantidad no puede estar vacío");
            }
            if (oRango.ZonaId == 0)
            {
                oEntityErrors.Error("Zona", "El campo Zona no puede estar vacío");
            }
            if (oRango.DesdeMes == 0 || oRango.HastaMes == 0)
            {
                oEntityErrors.Error("Mes", "Los campos Mes no pueden estar vacios");
            }
            if (oRango.DesdeAnio == 0 || oRango.HastaAnio == 0)
            {
                oEntityErrors.Error("Anio", "Los campos Año no pueden estar vacios");
            }
            var rangosExistentes = repositorio.Listar<RangoConfirmacionAutomatica>();
            if (rangosExistentes.Exists(x => 
            x.Id != oRango.Id &&
            x.MaterialId == oRango.MaterialId &&
            x.MonedaId == oRango.MonedaId && 
            x.FechaDesde == oRango.FechaDesde &&
            x.FechaHasta == oRango.FechaHasta &&
            x.ZonaId == oRango.ZonaId &&
            x.DesdeMes == oRango.DesdeMes &&
            x.HastaMes == oRango.HastaMes &&
            x.DesdeAnio == oRango.DesdeAnio &&
            x.HastaAnio == oRango.HastaAnio))
            {
                oEntityErrors.Error("Rango", "Ya existe un rango para los valores seleccionados");
            }
            return oEntityErrors;
        }
    }
}

