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

        public Resultado GrabarConfiguracionEspacioDinamico(ConfiguracionEspacioDinamico espacioDinamico, List<DiaCupo> dias)
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
                    foreach (var d in dias)
                    {
                        if (d.Cantidad > 0)
                        {
                            if (d.Fecha < DateTime.Today)
                            {
                                oEntityErrors.Error("CantidadCuposSAP", d.Fecha.ToShortDateString() + ": La Fecha de Ingreso no debe ser una fecha menor al día de hoy");
                                continue;
                            }
                            var configuracionCupo = repositorio.Obtener<ConfiguracionCupo>(x => x.Fecha == d.Fecha && !x.CierreCupera &&
                                                    x.MaterialId == espacioDinamico.MaterialId && x.CentroId == espacioDinamico.CentroId);
                            if (configuracionCupo != null)
                            {
                                if (configuracionCupo.LimiteCupo < (configuracionCupo.LimiteAlgoritmo + espacioDinamico.CantidadDeCupo))
                                {
                                    oEntityErrors.Error("espacioDinamico", "No se puede crear el espacio dinamico porque se excede del limite cupo configurado " + configuracionCupo.LimiteCupo);
                                    continue;
                                }
                            }
                            else
                            {
                                oEntityErrors.Error("espacioDinamico", "No hay cupera configurada para el día " + d.Fecha);
                                continue;
                            }

                            espacioDinamico.CantidadDeCupo = d.Cantidad.Value;
                            espacioDinamico.Fecha = d.Fecha;
                            repositorio.Agregar(espacioDinamico);
                            repositorio.GuardarCambios();
                            CrearSugerenciaDeEspacioDinamico(espacioDinamico);
                            ConfigurarLimiteAlgoritmo(espacioDinamico, oEntityErrors);
                        }
                    }
                }
                else
                {
                    var espacioDinamicoSave = repositorio.Obtener<ConfiguracionEspacioDinamico>(espacioDinamico.Id);
                    ConfigurarLimiteAlgoritmo(espacioDinamicoSave, oEntityErrors);
                    if (oEntityErrors.HayError)
                    {
                        return oEntityErrors;
                    }
                    espacioDinamicoSave.MaterialId = espacioDinamico.MaterialId;
                    espacioDinamicoSave.CentroId = espacioDinamico.CentroId;
                    espacioDinamicoSave.MaterialId = espacioDinamico.MaterialId;
                    espacioDinamicoSave.CantidadDeCupo = espacioDinamico.CantidadDeCupo;
                    espacioDinamicoSave.ProveedorId = espacioDinamico.ProveedorId;
                    espacioDinamicoSave.ComercialId = espacioDinamico.ComercialId;
                    espacioDinamicoSave.Calidad = espacioDinamico.Calidad;
                    CrearSugerenciaDeEspacioDinamico(espacioDinamicoSave);


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
            if (espacioDinamico.Id == 0)
            {
                if (repositorio.Existe<ConfiguracionEspacioDinamico>(x => x.ComercialId == espacioDinamico.ComercialId && x.ProveedorId == espacioDinamico.ProveedorId && x.CentroId == espacioDinamico.CentroId && x.MaterialId == espacioDinamico.MaterialId && x.Fecha == espacioDinamico.Fecha))
                {
                    errores.Error("espacioDinamico", "Ya existe configuración para ese Comercial, Proveedor, Material, Centro y Fecha");
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
        private void CrearDiaComercialParaElAlgortimo(SugerenciaCupo sugerencia)
        {
            var comercialDia = repositorio.Obtener<SugerenciaPorComercial>(x => x.Fecha == sugerencia.FechaSugerida
            && x.ComercialId == sugerencia.ComercialId && x.CentroId == x.CentroId && x.MaterialId == sugerencia.MaterialId);
            if (comercialDia != null)
            {
                comercialDia.Total += sugerencia.CantidadDeCupos;
            }
            else
            {
                var sugerenciaPorComercial = new SugerenciaPorComercial()
                {
                    ComercialId = sugerencia.ComercialId,
                    CentroId = sugerencia.CentroId,
                    MaterialId = sugerencia.MaterialId,
                    Fecha = sugerencia.FechaSugerida,
                    Total = sugerencia.CantidadDeCupos
                };
                repositorio.Agregar(sugerenciaPorComercial);
            }
        }
        private void CrearSugerenciaDeEspacioDinamico(ConfiguracionEspacioDinamico x)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(x.MaterialId));
            if (formula != null)
            {
                var fechaConfiguracion = x.Fecha.Date;
                if (formula.CuposDesde <= fechaConfiguracion && fechaConfiguracion <= formula.CuposHasta && formula.CentroId == x.CentroId)
                {
                    var tipoNegocioEspacioDinamico = repositorio.ObtenerPrimero<TipoNegocio>(a => a.Descripcion == "ESPACIO DINAMICO");
                    var zonaComercial = repositorio.Obtener<Comercial, string>(y => y.ComercialId == x.ComercialId, y => y.GrupoDeCompras.Descripcion);
                    var zona = repositorio.Obtener<ZonaCupo>(y => y.Descripcion == zonaComercial).Id;
                    var puntuacion = "{'Criterios':0.0}";
                    var sugerencia = new SugerenciaCupo
                    {
                        CentroId = x.CentroId,
                        StandardDeCalidad = x.Calidad,
                        ComercialId = x.ComercialId,
                        ProveedorId = x.ProveedorId,
                        CantidadDeCupos = x.CantidadDeCupo,
                        ConfiguracionEspacioDinamicoId = x.Id,
                        MaterialId = x.MaterialId,
                        MonedaId = null,
                        Precio = null,
                        ContratoSAP = null,
                        FechaSugerida = x.Fecha,
                        ZonaCupoId = zona,
                        TipoNegocioId = tipoNegocioEspacioDinamico.TipoNegocioId,
                        Puntuaciones = puntuacion,
                        Destinatario = "30715118773"
                    };
                    logger.Debug("Se creo una sugerencia para el dia: " + sugerencia.FechaSugerida);
                    repositorio.Agregar(sugerencia);
                    CrearDiaComercialParaElAlgortimo(sugerencia);
                }
            }
        }
        private Resultado ConfigurarLimiteAlgoritmo(ConfiguracionEspacioDinamico espacioDinamico, Resultado oEntityErrors)
        {
            var configuracionCupo = repositorio.Obtener<ConfiguracionCupo>(x => x.Fecha == espacioDinamico.Fecha && !x.CierreCupera &&
           x.MaterialId == espacioDinamico.MaterialId && x.CentroId == espacioDinamico.CentroId);
            if (configuracionCupo != null)
            {
                var limiteAlgoritmo = configuracionCupo.LimiteAlgoritmo;
                if (configuracionCupo.LimiteCupo > limiteAlgoritmo + espacioDinamico.CantidadDeCupo)
                {
                    configuracionCupo.LimiteAlgoritmo += espacioDinamico.CantidadDeCupo;
                }
                else
                {
                    oEntityErrors.Error("espacioDinamico", "No se puede editar el espacio dinamico porque se excede del limite cupo configurado " + configuracionCupo.LimiteCupo);
                    return oEntityErrors;
                }
            }
            else
            {
                oEntityErrors.Error("espacioDinamico", "No hay cupera configurada para el día " + espacioDinamico.Fecha);
                return oEntityErrors;
            }
            return oEntityErrors;
        }
    }
}
