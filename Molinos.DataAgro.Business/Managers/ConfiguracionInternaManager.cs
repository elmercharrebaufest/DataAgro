using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{
    public class ConfiguracionInternaManager : IConfiguracionInternaManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ILogDataAgroManager logDataAgroManager;

        public ConfiguracionInternaManager(IRepositorio repositorio, ILogger logger, ILogDataAgroManager logDataAgroManager)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.logDataAgroManager = logDataAgroManager;
        }
        public Resultado GrabarPrecio(PrecioMoa oConfiguracion)
        {
            var oEntityErrors = ValidarPrecio(oConfiguracion);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            else
            {
                repositorio.Agregar(oConfiguracion);
            }
            try
            {
                var tipo = oConfiguracion.Id > 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerPrecio(oConfiguracion.Id), tipo);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
                logger.Debug("Se guardó correctamente");
            }
            return oEntityErrors;
        }
        public Resultado GrabarPizarra(HabilitacionPizarra oConfiguracion)
        {
            var oEntityErrors = ValidarPizarra(oConfiguracion);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            else
            {
                repositorio.Agregar(oConfiguracion);
            }

            try
            {
                var tipo = oConfiguracion.Id > 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerPizarra(oConfiguracion.Id), tipo);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
                logger.Debug("Se guardó correctamente");
            }

            return oEntityErrors;
        }
        public Resultado GrabarFijacion(HabilitacionFijacion oConfiguracion)
        {
            var oEntityErrors = ValidarFijacion(oConfiguracion);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            else
            {
                repositorio.Agregar(oConfiguracion);
            }

            try
            {
                var tipo = oConfiguracion.Id > 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oConfiguracion.Id), tipo);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se guardó correctamente"));
                logger.Debug("Se guardó correctamente");
            }

            return oEntityErrors;
        }

        public List<PrecioMoaDto> TraerPrecios()
        {
            var hoy = DateTime.Now;
            var lista = repositorio.Listar<PrecioMoa, PrecioMoaDto>(x => new PrecioMoaDto
            {
                Id = x.Id,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                MonedaId = x.MonedaId,
                Precio = x.Precio
            },
           x => (x.DesdeVigencia <= hoy && x.HastaVigencia >= hoy) || x.DesdeVigencia >= hoy)
                .OrderBy(x => x.DesdeVigencia).ThenBy(x => x.MaterialId).ToList();
            return lista;
        }
        public List<HabilitacionFijacionDto> TraerFijaciones()
        {
            var hoy = DateTime.Today;
            var lista = repositorio.Listar<HabilitacionFijacion, HabilitacionFijacionDto>(x => new HabilitacionFijacionDto
            {
                Id = x.Id,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Dia = x.Dia
            },
            x => x.Dia >= hoy)
                .OrderBy(x => x.Dia).ThenBy(x => x.MaterialId).ToList();

            return lista;
        }
        public List<HabilitacionPizarraDto> TraerPizarra()
        {
            var hoy = DateTime.Today;
            var lista = repositorio.Listar<HabilitacionPizarra, HabilitacionPizarraDto>(x => new HabilitacionPizarraDto
            {
                Id = x.Id,
                Dia = x.Dia,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId
            },
            x => x.Dia >= hoy)
                .OrderBy(x => x.Dia).ThenBy(x => x.DesdeVigencia).ToList();

            return lista;
        }
        public Resultado EliminarPrecio(int id)
        {
            var oEntityErrors = new Resultado();
            var pm = repositorio.Obtener<PrecioMoa>(id);
            logDataAgroManager.LogCambiosDataAgro(TraerPrecio(id), TipoAccionLogDataAgro.Eliminar);
            try
            {
                repositorio.Remover(pm);
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
        public Resultado EliminarPizarra(int id)
        {
            var oEntityErrors = new Resultado();
            var hp = repositorio.Obtener<HabilitacionPizarra>(id);
            logDataAgroManager.LogCambiosDataAgro(TraerPizarra(id), TipoAccionLogDataAgro.Eliminar);
            try
            {
                repositorio.Remover(hp);
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
        public Resultado EliminarFijacion(int id)
        {
            var oEntityErrors = new Resultado();
            var hf = repositorio.Obtener<HabilitacionFijacion>(id);
            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(id), TipoAccionLogDataAgro.Eliminar);
            try
            {
                repositorio.Remover(hf);
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
        private Resultado ValidarPrecio(PrecioMoa precio)
        {
            var error = new Resultado();
            if (precio.MaterialId == 0)
            {
                error.Error("Material", "Debe Seleccionar Material");
            }
            if (string.IsNullOrEmpty(precio.MonedaId))
            {
                error.Error("Moneda", "Debe Seleccionar Moneda");
            }
            if (precio.Precio <= 0)
            {
                error.Error("Precio", "El precio debe ser mayor a 0");
            }
            if (precio.DesdeVigencia > precio.HastaVigencia)
            {
                error.Error("Fecha", "La vigencia desde no debe ser mayor al Hasta");
            }
            if (precio.DesdeVigencia < DateTime.Today)
            {
                error.Error("Fecha", "Vigencia no debe ser anterior al dia de la fecha");
            }
            if (repositorio.Existe<PrecioMoa>(x => x.HastaVigencia > precio.DesdeVigencia && x.MaterialId == precio.MaterialId && x.MonedaId == precio.MonedaId))
            {
                error.Error("Precio", "Existe PrecioMoa para ese Material, Moneda y ese rango de vigencia");
            }
            return error;
        }
        private Resultado ValidarPizarra(HabilitacionPizarra pizarra)
        {
            var error = new Resultado();
            if (pizarra.Dia < DateTime.Today)
            {
                error.Error("Fecha", "Dia no debe ser anterior al dia de la fecha");
            }
            if (pizarra.DesdeVigencia > pizarra.HastaVigencia)
            {
                error.Error("Vigencia", "La vigencia desde no puede ser mayor al hasta");
            }
            if (repositorio.Existe<HabilitacionPizarra>(x => x.Dia == pizarra.Dia && x.HastaVigencia > pizarra.DesdeVigencia && x.MaterialId == pizarra.MaterialId))
            {
                error.Error("Vigencia", "Ya existe habilitación con ese rango para esa fecha y material");
            }
            return error;
        }
        private Resultado ValidarFijacion(HabilitacionFijacion fijacion)
        {
            var error = new Resultado();
            if (fijacion.MaterialId == 0)
            {
                error.Error("Material", "Debe Seleccionar Material");
            }
            if (fijacion.Dia < DateTime.Today)
            {
                error.Error("Fecha", "Dia no debe ser anterior al dia de la fecha");
            }
            if (repositorio.Existe<HabilitacionFijacion>(x => x.MaterialId == fijacion.MaterialId && x.Dia == fijacion.Dia))
            {
                error.Error("Vigencia", "Ya existe habilitación para ese material y fecha");
            }
            return error;
        }

        public IEnumerable<IGrouping<int, PrecioMoaCompraNetDto>> TraerPrecioCompraNet()
        {
            var listaPrecio = new List<PrecioMoaCompraNetDto>();
            var materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, Descripcion = x.Descripcion }, x => x.MaterialId != 5);
            var monedas = repositorio.Listar<Moneda, MonedaDto>(x => new MonedaDto { MonedaId = x.MonedaId, Descripcion = x.Descripcion });
            var ahora = DateTime.Now;
            var preciosMoa = repositorio.Listar<PrecioMoa, PrecioMoaCompraNetDto>(x => new PrecioMoaCompraNetDto
            {
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Precio = x.Precio,
                MonedaId = x.Moneda.Descripcion
            }, x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora);
            var hoy = DateTime.Today;
            var existePizarra = repositorio.Listar<HabilitacionPizarra>(x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora);
            foreach (var mat in materiales)
            {
                foreach (var mon in monedas)
                {
                    var precio = preciosMoa.Where(x => x.MonedaId == mon.Descripcion && x.MaterialId == mat.MaterialId).FirstOrDefault();
                    if (precio == null)
                    {
                        precio = new PrecioMoaCompraNetDto { MaterialId = mat.MaterialId, MonedaId = mon.Descripcion, Material = mat.Descripcion, Retirado = true };
                    }
                    precio.Pizarra = existePizarra.Any(x => x.MaterialId == mat.MaterialId);
                    listaPrecio.Add(precio);
                }
            }
            return listaPrecio.GroupBy(x => x.MaterialId);
        }
        public List<PrecioMoaCompraNetDto> TraerPrecioCompraNet(int materialId)
        {
            var ahora = DateTime.Now;
            var monedas = repositorio.Listar<Moneda, string>(x => x.MonedaId);
            var precios = new List<PrecioMoaCompraNetDto>();
            foreach (var monedaId in monedas)
            {
                var precio = repositorio.Obtener<PrecioMoa, PrecioMoaCompraNetDto>(x => x.MaterialId == materialId && x.MonedaId == monedaId && x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora,
                    x => new PrecioMoaCompraNetDto { MaterialId = x.MaterialId, MonedaId = x.MonedaId, Material = x.Material.Descripcion, Precio = x.Precio });
                if (precio == null) precio = new PrecioMoaCompraNetDto { MaterialId = materialId, MonedaId = monedaId, Precio = 0 };
                precios.Add(precio);
            }
            return precios;
        }
        public bool HabilitarPizarra(int material)
        {
            var ahora = DateTime.Now;
            return repositorio.Existe<HabilitacionPizarra>(x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora && x.MaterialId == material);
        }

        public PrecioMoaDto TraerPrecio(int id)
        {
            return repositorio.Obtener<PrecioMoa, PrecioMoaDto>(x => x.Id == id, x => new PrecioMoaDto
            {
                Id = x.Id,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                MonedaId = x.MonedaId,
                Precio = x.Precio
            });
        }
        public HabilitacionPizarraDto TraerPizarra(int id)
        {
            return repositorio.Obtener<HabilitacionPizarra, HabilitacionPizarraDto>(x => x.Id == id, x => new HabilitacionPizarraDto
            {
                Id = x.Id,
                Dia = x.Dia,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId
            });

        }
        public HabilitacionFijacionDto TraerFijacion(int id)
        {
            return repositorio.Obtener<HabilitacionFijacion, HabilitacionFijacionDto>(x => x.Id == id, x => new HabilitacionFijacionDto
            {
                Id = x.Id,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Dia = x.Dia
            });
        }
    }
}
