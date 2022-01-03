using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Linq;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace Molinos.DataAgro.Business
{

    public class RangoConfirmacionAutomaticaManager : IRangoConfirmacionAutomaticaManager
    {
        private ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly ILogDataAgroManager logDataAgroManager;


        public RangoConfirmacionAutomaticaManager(ILogger logger, IRepositorio repositorio, ILogDataAgroManager logDataAgroManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.logDataAgroManager = logDataAgroManager;
        }
        public DatosIniAbmRangoConfirmacionAutomatica TraerDatosIniciales()
        {
            var qry = new CombosQueries(logger, repositorio);

            return new DatosIniAbmRangoConfirmacionAutomatica()
            {
                Material = qry.GetMaterialCombo(),
                Moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion }),
                Zona = repositorio.Listar<GrupoDeCompras, ZonaQry>(x => new ZonaQry() { Id = x.Id, Descripcion = x.Descripcion }),
                TipoNegocio = repositorio.Listar<TipoNegocioRangoConfirmacionAutomatica, TipoNegocioDto>(x => new TipoNegocioDto() { TipoNegocioId = x.Id, Descripcion = x.Descripcion }),
                TipoRango = repositorio.Listar<TipoRangoConfirmacionAutomatica, TipoRangoConfirmacionAutomaticaDto>(x => new TipoRangoConfirmacionAutomaticaDto() { TipoRangoId = x.Id, Descripcion = x.Descripcion }),
            };
        }

        public ResultIniRangoConfirmacionAutomatica TraerTodoRangoDisponible()
        {
            var hoy = DateTime.Today;
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
                    Cantidad = x.Cantidad / 1000,
                    EntregaDesde = x.DesdeEntrega.HasValue ? x.DesdeEntrega.Value.Day +"/"+ x.DesdeEntrega.Value.Month +"/"+ x.DesdeEntrega.Value.Year : "",
                    EntregaHasta = x.HastaEntrega.HasValue ? x.HastaEntrega.Value.Day + "/" + x.HastaEntrega.Value.Month + "/" + x.HastaEntrega.Value.Year : "",
                    Zona = x.Zona != null ? x.Zona.Descripcion : "",
                    TipoNegocio = x.TipoNegocio.Descripcion,
                    TipoRangoId = x.TipoRangoId,
                    TipoRango = x.TipoRango.Descripcion,
                }, x => DbFunctions.TruncateTime(x.FechaDesde) <= hoy && DbFunctions.TruncateTime(x.FechaHasta) >= hoy, 0, "Material")
            };
        }

        public DataSourceResult TraerTodoRango(DataSourceRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodoRago(request));
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
                HastaAnio = x.HastaAnio,
                ZonaId = x.ZonaId ?? 0,
                Zona = x.Zona.Descripcion,
                Cantidad = x.Cantidad / 1000,
                DesdeAnio = x.DesdeAnio,
                DesdeMes = x.DesdeMes,
                HastaMes = x.HastaMes,
                FechaHasta = x.FechaHasta,
                TipoNegocioId = x.TipoNegocioId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                UsuarioCreador = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                FechaCreacion = x.FechaCreacion,
                TipoRangoId = x.TipoRangoId,
                TipoRango = x.TipoRango.Descripcion,
                DesdeEntrega = x.DesdeEntrega,
                HastaEntrega = x.HastaEntrega
            }) ?? new RangoConfirmacionAutomaticaDto();
        }

        public Resultado GrabarRangoConfirmacionAutomatica(RangoConfirmacionAutomatica oRango, int comercialId)
        {
            var oEntityErrors = new Resultado();

            EntityValid.ValidateAll(oRango, oEntityErrors);
            ValidarRango(oEntityErrors, oRango);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            var tipo = oRango.Id != 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
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
                oRangoSave.Cantidad = oRango.Cantidad * 1000;
                oRangoSave.DesdeMes = oRango.DesdeEntrega.HasValue ? oRango.DesdeEntrega.Value.Month : 0;
                oRangoSave.HastaMes = oRango.HastaEntrega.HasValue ? oRango.HastaEntrega.Value.Month : 0;
                oRangoSave.DesdeAnio = oRango.DesdeEntrega.HasValue ? oRango.DesdeEntrega.Value.Year : 0;
                oRangoSave.HastaAnio = oRango.HastaEntrega.HasValue ? oRango.HastaEntrega.Value.Year : 0;
                oRangoSave.TipoNegocioId = oRango.TipoNegocioId;
                oRangoSave.TipoRangoId = oRango.TipoRangoId;
                oRangoSave.DesdeEntrega = oRango.DesdeEntrega;
                oRangoSave.HastaEntrega = oRango.HastaEntrega;


            }
            else
            {
                oRango.UsuarioCreadorId = comercialId;
                oRango.FechaCreacion = DateTime.Now;
                oRango.Cantidad *= 1000;
                repositorio.Agregar(oRango);
            }

            try
            {
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerRango(oRango.Id), tipo);
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
            logDataAgroManager.LogCambiosDataAgro(TraerRango(id), TipoAccionLogDataAgro.Eliminar);
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
            if (oRango.FechaDesde == null || oRango.FechaHasta == null)
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

            if ((oRango.DesdeEntrega == null || oRango.HastaEntrega == null) && oRango.TipoNegocioId == 2)
            {
                oEntityErrors.Error("Mes", "Los campos Fecha Entrega no pueden estar vacios");
            }
            //if ((oRango.DesdeAnio == 0 || oRango.HastaAnio == 0) && oRango.TipoNegocioId == 2)
            //{
            //    oEntityErrors.Error("Anio", "Los campos Año no pueden estar vacios");
            //}
            if (oRango.FechaHasta < oRango.FechaDesde)
            {
                oEntityErrors.Error("Fechas", "La fecha hasta no puede ser menor que: " + oRango.FechaDesde);
            }
            if (oRango.HastaEntrega < oRango.DesdeEntrega)
            {
                oEntityErrors.Error("Fechas", "La fecha hasta de entrega no puede ser menor que: " + oRango.DesdeEntrega);
            }
            var rangos = new List<int>();
            if (oRango.TipoRangoId == (int)EnumTipoRangoConfirmacionAutomatica.Confirmacion)
            {
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Confirmacion);
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion);
            }
            if (oRango.TipoRangoId == (int)EnumTipoRangoConfirmacionAutomatica.Reconfirmacion)
            {
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Reconfirmacion);
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion);
            }
            if (oRango.TipoRangoId == (int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion)
            {
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Confirmacion);
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Reconfirmacion);
                rangos.Add((int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion);
            }

            var tipos = new List<int>();
            tipos.Add((int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecioYFijacion);
            if (oRango.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecio)
            {
                tipos.Add((int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecio);
            }
            if (oRango.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.Fijacion)
            {
                tipos.Add((int)EnumTipoNegocioRangoConfirmacionAutomatica.Fijacion);
            }
            if (oRango.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecioYFijacion)
            {
                tipos.Add((int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecio);
                tipos.Add((int)EnumTipoNegocioRangoConfirmacionAutomatica.Fijacion);
            }

            var rangosExistentes = repositorio.Listar<RangoConfirmacionAutomatica>();
            if (rangosExistentes.Exists(x =>
            x.Id != oRango.Id &&
            x.MaterialId == oRango.MaterialId &&
            x.MonedaId == oRango.MonedaId &&
            //x.FechaDesde == oRango.FechaDesde &&
            //x.FechaHasta == oRango.FechaHasta &&
            ((x.FechaDesde <= oRango.FechaDesde && x.FechaHasta >= oRango.FechaDesde) ||
            (x.FechaDesde <= oRango.FechaHasta && x.FechaHasta >= oRango.FechaHasta)) &&
            x.ZonaId == oRango.ZonaId &&
            (x.DesdeEntrega == oRango.DesdeEntrega || oRango.DesdeEntrega == null || x.DesdeEntrega == null) &&
            (x.HastaEntrega == oRango.HastaEntrega || oRango.HastaEntrega == null || x.HastaEntrega == null) &&
            rangos.Contains(x.TipoRangoId) &&
            tipos.Contains(x.TipoNegocioId) 
            ))
            {
                oEntityErrors.Error("Rango", "Ya existe un rango para los valores seleccionados");
            }
            if (oRango.TipoNegocioId == 0)
            {
                oEntityErrors.Error("TipoNegocio", "El campo Tipo Negocio no puede estar vacío");
            }
            return oEntityErrors;
        }
    }
}

