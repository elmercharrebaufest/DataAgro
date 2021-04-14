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
        private readonly IDiasHabilesAgent diasHabilesAgent;

        public ConfiguracionInternaManager(IRepositorio repositorio, ILogger logger, ILogDataAgroManager logDataAgroManager, IDiasHabilesAgent diasHabilesAgent)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.logDataAgroManager = logDataAgroManager;
            this.diasHabilesAgent = diasHabilesAgent;
        }
        public Resultado GrabarPrecio(PrecioMoa oConfiguracion, string active)
        {
            var oEntityErrors = ValidarPrecio(oConfiguracion);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            else
            {
                oConfiguracion.UsuarioCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == active, x => x.ComercialId);
                oConfiguracion.FechaCreacion = DateTime.Now;
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
        public Resultado GrabarPizarra(HabilitacionPizarra oConfiguracion, string active, DateTime fechaHasta)
        {
            var oEntityErrors = ValidarPizarra(oConfiguracion, fechaHasta);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            List<DateTime> EachDay = new List<DateTime>();

            for (var day = oConfiguracion.Dia.Date; day.Date <= fechaHasta.Date; day = day.AddDays(1))
            {
                oConfiguracion.UsuarioCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == active, x => x.ComercialId);
                oConfiguracion.FechaCreacion = DateTime.Now;
                var nuevoHabilitacionPizarra = (HabilitacionPizarra)oConfiguracion.Clone();
                nuevoHabilitacionPizarra.Dia = day;
                repositorio.Agregar(nuevoHabilitacionPizarra);

                try
                {
                    var tipo = nuevoHabilitacionPizarra.Id > 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerPizarra(nuevoHabilitacionPizarra.Id), tipo);
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
            var ultimoDiaHabil = diasHabilesAgent.UltimoDiaHabil(null);
            var lista = repositorio.Listar<PrecioMoa, PrecioMoaDto>(x => new PrecioMoaDto
            {
                Id = x.Id,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                DesdeEntrega = x.DesdeEntrega,
                HastaEntrega = x.HastaEntrega,
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                MaterialId = x.MaterialId,
                TipoNegocioId = x.TipoNegocioId,
                Material = x.Material.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                MonedaId = x.MonedaId,
                Precio = x.Precio
            },
           x => (x.DesdeVigencia <= hoy && x.HastaVigencia >= hoy) || x.DesdeVigencia >= hoy || (x.DesdeVigencia <= ultimoDiaHabil && x.HastaVigencia >= ultimoDiaHabil))
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
                MaterialId = x.MaterialId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            },
            x => x.Dia >= hoy)
                .OrderBy(x => x.Dia).ThenBy(x => x.DesdeVigencia).ToList();

            return lista;
        }
        public List<HabilitacionPagoDiferidoDto> TraerPagoDiferido()
        {
            var hoy = DateTime.Today;
            var lista = repositorio.Listar<HabilitacionPagoDiferido, HabilitacionPagoDiferidoDto>(x => new HabilitacionPagoDiferidoDto
            {
                Id = x.Id,
                CantidadDia = x.CantidadDia,
                Importe = x.Importe,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            }, x => x.Habilitado && x.HastaVigencia >= hoy, 0, "DesdeVigencia", Entities.Helpers.DirOrden.Asc).ToList();

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
            if (precio.TipoNegocioId == 0)
            {
                error.Error("TipoNegocioId", "Debe Seleccionar Tipo de Negocio");
            }
            else
            {
                if (precio.TipoNegocioId == 1)
                {
                    //fijacion
                    if (precio.DesdeFijacion == null)
                    {
                        error.Error("DesdeFijacion", "Debe Seleccionar Desde Fijacion");
                    }
                    if (precio.HastaFijacion == null)
                    {
                        error.Error("HastaFijacion", "Debe Seleccionar Hasta Fijacion");
                    }

                    if (precio.DesdeFijacion < DateTime.Now.Date)
                    {
                        error.Error("DesdeFijacion", "Desde Fijacion no debe ser anterior al dia de la fecha");
                    }
                    if (precio.HastaFijacion < DateTime.Now.Date)
                    {
                        error.Error("HastaFijacion", "Hasta Fijacion no debe ser anterior al dia de la fecha");
                    }

                    if (precio.HastaFijacion.HasValue && precio.DesdeFijacion.HasValue && (precio.HastaFijacion < precio.DesdeFijacion))
                    {
                        error.Error("HastaFijacion", "Hasta Fijacion debe ser mayor a Desde Fijacion");
                    }

                }

                if (precio.TipoNegocioId == 2 || precio.TipoNegocioId == 1)
                {
                    //entrega
                    if (precio.DesdeEntrega == null)
                    {
                        error.Error("DesdeEntrega", "Debe Seleccionar Desde Entrega");
                    }
                    if (precio.HastaEntrega == null)
                    {
                        error.Error("HastaEntrega", "Debe Seleccionar Hasta Entrega");
                    }

                    if (precio.DesdeEntrega < DateTime.Now.Date)
                    {
                        error.Error("DesdeEntrega", "Desde Entrega no debe ser anterior al dia de la fecha");
                    }
                    if (precio.HastaEntrega < DateTime.Now.Date)
                    {
                        error.Error("HastaEntrega", "Hasta Entrega no debe ser anterior al dia de la fecha");
                    }

                    if (precio.HastaEntrega.HasValue && precio.DesdeEntrega.HasValue && (precio.HastaEntrega < precio.DesdeEntrega))
                    {
                        error.Error("HastaEntrega", "Hasta Entrega debe ser mayor a Desde Entrega");
                    }
                }

                if (precio.TipoNegocioId == 3 || precio.TipoNegocioId == 2)
                {
                    if (string.IsNullOrEmpty(precio.MonedaId))
                    {
                        error.Error("Moneda", "Debe Seleccionar Moneda");
                    }
                    if (precio.Precio <= 0)
                    {
                        error.Error("Precio", "El precio debe ser mayor a 0");
                    }
                }

                if (precio.TipoNegocioId == 1)
                {
                    if (repositorio.Existe<PrecioMoa>(x => x.HastaVigencia > precio.DesdeVigencia && x.HastaEntrega >= precio.DesdeEntrega && x.MaterialId == precio.MaterialId && x.TipoNegocioId == precio.TipoNegocioId))
                    {
                        error.Error("Precio", "Existe PrecioMoa para ese Tipo de Negocio, Material y ese rango de entrega y vigencia");
                    }
                }
                if (precio.TipoNegocioId == 2)
                {
                    if (repositorio.Existe<PrecioMoa>(x => x.HastaVigencia > precio.DesdeVigencia && x.HastaEntrega >= precio.DesdeEntrega && x.MaterialId == precio.MaterialId && x.MonedaId == precio.MonedaId && x.TipoNegocioId == precio.TipoNegocioId))
                    {
                        error.Error("Precio", "Existe PrecioMoa para ese Tipo de Negocio, Material, Moneda y ese rango de entrega y vigencia");
                    }
                }
                if (precio.TipoNegocioId == 3)
                {
                    if (repositorio.Existe<PrecioMoa>(x => x.HastaVigencia > precio.DesdeVigencia && x.MaterialId == precio.MaterialId && x.MonedaId == precio.MonedaId && x.TipoNegocioId == precio.TipoNegocioId))
                    {
                        error.Error("Precio", "Existe PrecioMoa para ese Tipo de Negocio, Material, Moneda y ese rango de vigencia");
                    }
                }
            }
            if (precio.MaterialId == 0)
            {
                error.Error("Material", "Debe Seleccionar Material");
            }

            if (precio.DesdeVigencia > precio.HastaVigencia)
            {
                error.Error("Fecha", "La vigencia desde no debe ser mayor al Hasta");
            }
            if (precio.DesdeVigencia < DateTime.Today)
            {
                error.Error("Fecha", "Vigencia no debe ser anterior al dia de la fecha");
            }



            return error;
        }
        private Resultado ValidarPizarra(HabilitacionPizarra pizarra, DateTime fechaHasta)
        {
            var error = new Resultado();
            if (pizarra.TipoNegocioId == 0)
            {
                error.Error("TipoNegocioId", "No Selecciono el Tipo de Negocio");
            }
            else
            {
                if (pizarra.TipoNegocioId == 2)
                {
                    //entrega
                    if (pizarra.DesdeEntrega == null)
                    {
                        error.Error("DesdeEntrega", "Debe Seleccionar Desde Entrega");
                    }
                    if (pizarra.HastaEntrega == null)
                    {
                        error.Error("HastaEntrega", "Debe Seleccionar Hasta Entrega");
                    }

                    if (pizarra.DesdeEntrega < DateTime.Now.Date)
                    {
                        error.Error("DesdeEntrega", "Desde Entrega no debe ser anterior al dia de la fecha");
                    }
                    if (pizarra.HastaEntrega < DateTime.Now.Date)
                    {
                        error.Error("HastaEntrega", "Hasta Entrega no debe ser anterior al dia de la fecha");
                    }

                    if (pizarra.HastaEntrega.HasValue && pizarra.DesdeEntrega.HasValue && (pizarra.HastaEntrega < pizarra.DesdeEntrega))
                    {
                        error.Error("HastaEntrega", "Hasta Entrega debe ser mayor a Desde Entrega");
                    }

                }
            }
            if (pizarra.Dia < DateTime.Today)
            {
                error.Error("Fecha", "Dia no debe ser anterior al dia de la fecha");
            }
            if (pizarra.DesdeVigencia > pizarra.HastaVigencia)
            {
                error.Error("Vigencia", "La vigencia desde no puede ser mayor al hasta");
            }
            if (pizarra.Dia > fechaHasta)
            {
                error.Error("Dia", "La fecha desde no puede ser mayor al hasta");
            }
            if (repositorio.Existe<HabilitacionPizarra>(x => pizarra.Dia >= x.Dia && fechaHasta <= x.Dia && x.HastaVigencia > pizarra.DesdeVigencia && x.MaterialId == pizarra.MaterialId && pizarra.TipoNegocioId == x.TipoNegocioId))
            {
                error.Error("Vigencia", "Ya existe habilitación con ese rango para esa fecha y material");
            }
            return error;
        }

        private Resultado ValidarPagoDiferido(HabilitacionPagoDiferido pago)
        {
            var error = new Resultado();
            if (pago.TipoNegocioId == 0)
            {
                error.Error("TipoNegocioId", "No Selecciono el Tipo de Negocio");
            }
            if (pago.MaterialId == 0)
            {
                error.Error("Material", "No Selecciono el Material");
            }
            if (pago.CantidadDia <= 0)
            {
                error.Error("CantidadDia", "El campo Cantidad de Días es obligatorio");
            }
            if (pago.Importe <= 0)
            {
                error.Error("Importe", "El campo Importe es obligatorio");
            }
            if (pago.DesdeVigencia > pago.HastaVigencia)
            {
                error.Error("Vigencia", "La vigencia desde no puede ser mayor al hasta");
            }
            if (repositorio.Existe<HabilitacionPagoDiferido>(x => x.CantidadDia == pago.CantidadDia && x.HastaVigencia > pago.DesdeVigencia && x.MaterialId == pago.MaterialId && pago.TipoNegocioId == x.TipoNegocioId && x.Habilitado))
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

        public IEnumerable<IGrouping<int, PrecioMoaCompraNetDto>> TraerPrecioCompraNet(int? tiponegocio = null)
        {
            tiponegocio = tiponegocio == null ? 3 : 0;
            var listaPrecio = new List<PrecioMoaCompraNetDto>();
            var materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, Descripcion = x.Descripcion }, x => x.MaterialId != 5);
            var tipoNegocios = repositorio.Listar<TipoNegocio, TipoNegocioDto>(x => new TipoNegocioDto { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion },
                x => tiponegocio == 0 ? x.TipoNegocioId <= 3 : x.TipoNegocioId == 3);

            var monedas = repositorio.Listar<Moneda, MonedaDto>(x => new MonedaDto { MonedaId = x.MonedaId, Descripcion = x.Descripcion });
            var ahora = DateTime.Now;
            var preciosMoa = repositorio.Listar<PrecioMoa, PrecioMoaCompraNetDto>(x => new PrecioMoaCompraNetDto
            {
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Precio = x.Precio,
                MonedaId = x.Moneda.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId,
                DesdeEntrega = x.DesdeEntrega,
                DesdeFijacion = x.DesdeFijacion,
                HastaEntrega = x.HastaEntrega,
                HastaFijacion = x.HastaFijacion
            }, x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora && (x.TipoNegocioId == tiponegocio || tiponegocio == 0));
            var hoy = DateTime.Today;
            var existePizarra = repositorio.Listar<HabilitacionPizarra>(x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora && (x.TipoNegocioId == tiponegocio || tiponegocio == 0));
            foreach (var neg in tipoNegocios)
            {
                foreach (var mat in materiales)
                {
                    foreach (var mon in monedas)
                    {
                        if (tiponegocio == 3)
                        {
                            var precio = preciosMoa.Where(x => (x.MonedaId == mon.Descripcion || neg.TipoNegocioId == 1) && x.MaterialId == mat.MaterialId && x.TipoNegocioId == neg.TipoNegocioId).FirstOrDefault();
                            if (precio == null)
                            {
                                precio = new PrecioMoaCompraNetDto { MaterialId = mat.MaterialId, MonedaId = mon.Descripcion, Material = mat.Descripcion, Retirado = true, TipoNegocio = neg.Descripcion, TipoNegocioId = neg.TipoNegocioId };
                            }
                            precio.Pizarra = existePizarra.Any(x => x.MaterialId == mat.MaterialId && x.TipoNegocioId == neg.TipoNegocioId);
                            listaPrecio.Add(precio);
                        }
                        else
                        {
                            if (neg.TipoNegocioId == 1 && mon.Descripcion == "ARP")
                            {
                                continue;
                            }
                            var precio = preciosMoa.Where(x => (x.MonedaId == mon.Descripcion || neg.TipoNegocioId == 1) && x.MaterialId == mat.MaterialId && x.TipoNegocioId == neg.TipoNegocioId).ToList();

                            foreach (var item in precio)
                            {
                                item.Pizarra = existePizarra.Any(x => x.MaterialId == mat.MaterialId && x.TipoNegocioId == neg.TipoNegocioId);
                            }
                            if (existePizarra.Any(x => x.MaterialId == mat.MaterialId && x.TipoNegocioId == neg.TipoNegocioId) && mon.Descripcion == "ARP")
                            {
                                var pizarra = existePizarra.FirstOrDefault();
                                precio.Add(new PrecioMoaCompraNetDto
                                {
                                    DesdeEntrega = pizarra.DesdeEntrega,
                                    HastaEntrega = pizarra.HastaEntrega,
                                    Pizarra = true,
                                    MaterialId = mat.MaterialId,
                                    MonedaId = "Pizarra",
                                    Material = mat.Descripcion,
                                    Retirado = false,
                                    TipoNegocio = neg.Descripcion,
                                    TipoNegocioId = neg.TipoNegocioId
                                });
                            }
                            if (precio.Count == 0)
                            {
                                precio.Add(new PrecioMoaCompraNetDto { MaterialId = mat.MaterialId, MonedaId = mon.Descripcion, Material = mat.Descripcion, Retirado = true, TipoNegocio = neg.Descripcion, TipoNegocioId = neg.TipoNegocioId });
                            }
                            listaPrecio.AddRange(precio);
                        }

                    }
                }
            }

            return listaPrecio.GroupBy(x => x.MaterialId);
        }
        public List<PrecioMoaCompraNetDto> TraerPrecioCompraNet(int materialId, int? tiponegocio = null)
        {
            tiponegocio = tiponegocio ?? 3;
            var ahora = DateTime.Now;
            var monedas = repositorio.Listar<Moneda, string>(x => x.MonedaId);
            var precios = new List<PrecioMoaCompraNetDto>();
            foreach (var monedaId in monedas)
            {
                var precio = repositorio.Obtener<PrecioMoa, PrecioMoaCompraNetDto>(x =>
                x.MaterialId == materialId
                && (x.MonedaId == monedaId || tiponegocio == 1)
                && x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora && x.TipoNegocioId == tiponegocio,
                    x => new PrecioMoaCompraNetDto
                    {
                        MaterialId = x.MaterialId,
                        MonedaId = x.MonedaId,
                        Material = x.Material.Descripcion,
                        Precio = x.Precio,
                        DesdeEntrega = x.DesdeEntrega,
                        DesdeFijacion = x.DesdeFijacion,
                        HastaEntrega = x.HastaEntrega,
                        HastaFijacion = x.HastaFijacion
                    });
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

        public List<HabilitacionPagoDiferidoDto> TraerPagosDiferido(int tipoNegocio, int material)
        {
            var ahora = DateTime.Now;
            var pagosDiferidosVigentes = repositorio.Listar<HabilitacionPagoDiferido, HabilitacionPagoDiferidoDto>(x => new HabilitacionPagoDiferidoDto
            {
                Importe = x.Importe,
                CantidadDia = x.CantidadDia,
                Id = x.Id
            }, x => x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora
                    && x.TipoNegocioId == tipoNegocio && x.MaterialId == material && x.Habilitado);

            return pagosDiferidosVigentes;
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
                TipoNegocioId = x.TipoNegocioId,
                TipoNegocio = x.TipoNegocio.Descripcion,
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
                MaterialId = x.MaterialId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            });

        }

        public HabilitacionPagoDiferidoDto TraerPagoDiferido(int id)
        {
            return repositorio.Obtener<HabilitacionPagoDiferido, HabilitacionPagoDiferidoDto>(x => x.Id == id, x => new HabilitacionPagoDiferidoDto
            {
                Id = x.Id,
                CantidadDia = x.CantidadDia,
                Importe = x.Importe,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId,
                Habilitado = x.Habilitado
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

        public List<HabilitacionCampañaDto> TraerCampaña()
        {
            var hoy = DateTime.Today;
            var lista = repositorio.Listar<HabilitacionCampaña, HabilitacionCampañaDto>(x => new HabilitacionCampañaDto
            {
                Id = x.Id,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Campaña = x.Campaña.Descripcion,
                CampañaId = x.CampañaId
            }).OrderBy(x => x.MaterialId).ThenBy(a => a.CampañaId).ToList();

            return lista;
        }

        public Resultado EliminarCampaña(int id)
        {
            var oEntityErrors = new Resultado();
            var hp = repositorio.Obtener<HabilitacionCampaña>(id);
            logDataAgroManager.LogCambiosDataAgro(TraerCampaña(id), TipoAccionLogDataAgro.Eliminar);
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

        public HabilitacionCampañaDto TraerCampaña(int id)
        {
            return repositorio.Obtener<HabilitacionCampaña, HabilitacionCampañaDto>(x => x.Id == id, x => new HabilitacionCampañaDto
            {
                Id = x.Id,

                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Campaña = x.Campaña.Descripcion,
                CampañaId = x.CampañaId
            });
        }
        public Resultado GrabarCampaña(HabilitacionCampaña oConfiguracion)
        {
            var oEntityErrors = ValidarCampaña(oConfiguracion);
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
                logDataAgroManager.LogCambiosDataAgro(TraerCampaña(oConfiguracion.Id), tipo);
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

        private Resultado ValidarCampaña(HabilitacionCampaña oConfiguracion)
        {
            Resultado resultado = new Resultado();
            if (oConfiguracion.CampañaId == 0)
            {
                resultado.Error("Campaña", "Debe seleccionar la Campaña.");
            }
            if (oConfiguracion.MaterialId == 0)
            {
                resultado.Error("Material", "Debe seleccionar el Material.");
            }
            if (repositorio.Existe<HabilitacionCampaña>(x => x.MaterialId == oConfiguracion.MaterialId && oConfiguracion.CampañaId == x.CampañaId))
            {
                resultado.Error("Vigencia", "Ya existe habilitación con ese material y cosecha.");
            }
            return resultado;
        }

        public HabilitacionPizarraDto HabilitarPizarraExterno(int material, int tiponegocio)
        {
            var ahora = DateTime.Now;
            return repositorio.Obtener<HabilitacionPizarra, HabilitacionPizarraDto>(x =>
            x.DesdeVigencia <= ahora && x.HastaVigencia >= ahora &&
            x.MaterialId == material && x.TipoNegocioId == tiponegocio, x => new HabilitacionPizarraDto
            {
                Id = x.Id,
                Dia = x.Dia,
                DesdeVigencia = x.DesdeVigencia,
                HastaVigencia = x.HastaVigencia,
                DesdeEntrega = x.DesdeEntrega,
                HastaEntrega = x.HastaEntrega,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId
            });
        }

        public List<HabilitacionCampañaDto> HabilitarCampañaExterno(int material)
        {
            return repositorio.Listar<HabilitacionCampaña, HabilitacionCampañaDto>(x => new HabilitacionCampañaDto
            {
                Id = x.Id,

                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Campaña = x.Campaña.Descripcion,
                CampañaId = x.CampañaId
            }, x => x.MaterialId == material);
        }

        public Resultado GrabarPagoDiferido(HabilitacionPagoDiferido oConfiguracion, string active)
        {
            var oEntityErrors = ValidarPagoDiferido(oConfiguracion);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            else
            {
                oConfiguracion.UsuarioCreadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == active, x => x.ComercialId);
                oConfiguracion.FechaCreacion = DateTime.Now;
                oConfiguracion.Habilitado = true;
                repositorio.Agregar(oConfiguracion);
            }
            try
            {
                var tipo = oConfiguracion.Id > 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerPagoDiferido(oConfiguracion.Id), tipo);
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
        public Resultado EliminarHabilitacionPagoDiferido(int id)
        {
            var oEntityErrors = new Resultado();
            try
            {
                var hp = repositorio.Obtener<HabilitacionPagoDiferido>(id);
                hp.Habilitado = false;
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            logDataAgroManager.LogCambiosDataAgro(TraerPagoDiferido(id), TipoAccionLogDataAgro.Eliminar);

            if (!oEntityErrors.HayError)
            {
                oEntityErrors.Errores.Add(new ErrorMessage(200, "Se eliminó correctamente"));
            }
            return oEntityErrors;
        }
    }
}
