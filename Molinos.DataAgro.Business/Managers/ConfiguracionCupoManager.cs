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
        private readonly IAdministracionCuperaAgent administracionCuperaAgent;
        private readonly ICupoManager cupoManager;




        public ConfiguracionCupoManager(ILogger logger, IRepositorio repositorio, IAdministracionCuperaAgent administracionCuperaAgent, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.administracionCuperaAgent = administracionCuperaAgent;
            this.cupoManager = cupoManager;


        }

        public Resultado GrabarConfiguracionCupo(ConfiguracionCupo configuracion, List<DiaCupo> dias)
        {
            var oEntityErrors = Validar(configuracion, dias);
            if (oEntityErrors.HayError)
            {
                return oEntityErrors;
            }
            try
            {
                if (configuracion.LiberarCupera == true)
                {
                    configuracion.LimiteAlgoritmo = 0;
                }
                var error = new CupoResult { ListaCupos = new List<string>() };
                var materiales = repositorio.Listar<Material>();
                var zonas = repositorio.Listar<ZonaCupo>();
                var centros = repositorio.Listar<Centro>();
                var errorSap = new Resultado();
                if (configuracion.Id == 0)
                {
                    foreach (var d in dias)
                    {
                        var newConfiguracion = new ConfiguracionCupo
                        {
                            CentroId = configuracion.CentroId,
                            CierreCupera = configuracion.CierreCupera,
                            MaterialId = configuracion.MaterialId,
                            LiberarCupera = configuracion.LiberarCupera,
                            LimiteAnterior = configuracion.LimiteAnterior
                        };
                        if (d.Cantidad > 0)
                        {
                            if (d.Fecha < DateTime.Today)
                            {
                                error.Error("CantidadCuposSAP", d.Fecha.ToShortDateString() + ": La Fecha de Ingreso no debe ser una fecha menor al día de hoy");
                                continue;
                            }
                            newConfiguracion.Fecha = d.Fecha.Date;
                            newConfiguracion.LimiteCupo = d.Cantidad.Value;
                            newConfiguracion.LimiteAlgoritmo = d.CantidadAlgoritmo.Value;
                            newConfiguracion.CantidadCupo = new List<LimiteCupo>();
                            GenerarLimiteZona(newConfiguracion, zonas);
                            errorSap = EnviarConfiguracion(newConfiguracion, null, null, materiales, zonas, centros);
                            if (errorSap.HayError)
                            {
                                errorSap.Error("GrabarConfiguracionCupo - Cabecera ", newConfiguracion.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                                return errorSap;
                            }
                            else
                            {
                                repositorio.Agregar(newConfiguracion);
                                repositorio.GuardarCambios();

                            }
                            //errorSap = EnviarLimiteZona(newConfiguracion.CantidadCupo.ToList(), newConfiguracion, false, materiales, zonas, centros, newConfiguracion.CierreCupera);
                            //if (!errorSap.HayError)
                            //{
                            //    repositorio.Agregar(newConfiguracion);
                            //}
                            //else
                            //{
                            //    errorSap.Error("GrabarConfiguracionCupo - Zonas ", newConfiguracion.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));

                            //    return errorSap;
                            //}

                        }
                    }
                }
                else
                {
                    if (configuracion.LiberarCupera == true)
                    {
                        configuracion.LimiteAlgoritmo = 0;
                    }
                    var configuracionSave = repositorio.Obtener<ConfiguracionCupo>(configuracion.Id);
                    configuracion.CantidadCupo = new List<LimiteCupo>();
                    foreach (var x in configuracionSave.CantidadCupo.ToList())
                    {

                        configuracion.CantidadCupo.Add(new LimiteCupo
                        {
                            CantidadCupo = x.CantidadCupo,
                            LimiteAnterior = x.LimiteAnterior ?? 0,
                            ZonaCupoId = x.ZonaCupoId,
                            ZonaCupo = x.ZonaCupo
                        });
                    }
                    if (configuracionSave.LimiteCupo != configuracion.LimiteCupo || configuracionSave.CierreCupera != configuracion.CierreCupera)
                    {
                        errorSap = EnviarConfiguracion(configuracion, null, configuracionSave.LimiteCupo, materiales, zonas, centros);
                    }
                    if (errorSap.HayError)
                    {
                        errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                        return errorSap;
                    }
                    else
                    {
                        configuracionSave.LimiteAnterior = configuracionSave.LimiteCupo;
                        configuracionSave.LimiteCupo = configuracion.LimiteCupo;
                        configuracionSave.LimiteAlgoritmo = configuracion.LimiteAlgoritmo;
                        configuracionSave.CierreCupera = configuracion.CierreCupera;
                        configuracionSave.LiberarCupera = configuracion.LiberarCupera;
                    }

                    repositorio.GuardarCambios();
                }
                if (configuracion.LiberarCupera == true)
                {
                    cupoManager.EliminarSugerenciaDeCupos(configuracion);
                }
                else
                {
                    cupoManager.CrearSugerenciaCupo(configuracion.MaterialId, configuracion);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                oEntityErrors.Error(ex.Source, ex.Message);
                throw;
            }
            return oEntityErrors;
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
                var configuracion = repositorio.Obtener<ConfiguracionCupo>(limite.First().ConfiguracionCupoId);

                foreach (var item in configuracion.CantidadCupo)
                {
                    item.LimiteAnterior = item.CantidadCupo;
                    item.CantidadCupo = limite.Where(a => a.ZonaCupoId == item.ZonaCupoId).First().CantidadCupo;
                }
                var materiales = repositorio.Listar<Material>();
                var zonas = repositorio.Listar<ZonaCupo>();
                var centros = repositorio.Listar<Centro>();
                var errorSap = EnviarConfiguracion(configuracion, null, configuracion.LimiteCupo, materiales, zonas, centros);
                if (errorSap.HayError)
                {
                    errorSap.Error("GrabarLimites", configuracion.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                    return errorSap;
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
        private Resultado EnviarConfiguracion(ConfiguracionCupo configuracion, bool? aceptar, int? limiteAnterior, List<Material> materiales, List<ZonaCupo> zonas, List<Centro> centros)
        {
            logger.Debug("EnviarCabeceraConfiguracion");
            var errorSap = new Resultado();
            var result = "";
            var config = new ConfiguracionCupoDto
            {
                LimiteCupo = configuracion.LimiteCupo,
                CierreCupera = aceptar != null ? aceptar : configuracion.CierreCupera,
                Material = materiales.Where(x => x.MaterialId == configuracion.MaterialId).FirstOrDefault().Codigo,
                Fecha = configuracion.Fecha,
                ZonaCupo = "",
                LimiteCupoAnterior = limiteAnterior != null ? limiteAnterior.Value : 0,
                Centro = centros.Where(x => x.Id == configuracion.CentroId).FirstOrDefault().CodigoSap,
                CantidadCupo = configuracion.CantidadCupo.Select(x => new LimiteCupoDto
                {
                    CantidadCupo = x.CantidadCupo,
                    CantidadCupoAnterior = x.LimiteAnterior ?? 0,
                    ZonaCupoId = x.ZonaCupoId,
                    ZonaCupo = zonas.Where(y => y.Id == x.ZonaCupoId).Single().CodigoSap
                }).ToList()
            };
            try
            {
                result = administracionCuperaAgent.AdministrarCupera(config);
                if (!result.Equals("OK"))
                {
                    errorSap.Error("EnviarCabeceraConfiguracion", configuracion.Fecha.ToShortDateString() + ": " + result);
                    return errorSap;
                }
            }
            catch (Exception e)
            {
                errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + e.Message);
                return errorSap;

            }
            return errorSap;
        }
        private void GenerarLimiteZona(ConfiguracionCupo configuracion, List<ZonaCupo> zonas)
        {
            var totalNeogcios = repositorio.Listar<Contrato>(x => x.FechaHasta == configuracion.Fecha && x.EstadoId == 5 && x.MaterialId == configuracion.MaterialId && x.DestinoId == configuracion.CentroId).Sum(x => x.Cantidad);
            for (int i = 0; i < zonas.Count(); i++)
            {
                var zonaCupo = zonas[i].Descripcion;
                var totalZona = repositorio.Listar<Contrato>(x => x.FechaHasta == configuracion.Fecha && x.EstadoId == 5 && x.Comercial.GrupoDeCompras.Descripcion == zonaCupo && x.MaterialId == configuracion.MaterialId && x.DestinoId == configuracion.CentroId).Sum(x => x.Cantidad);
                var porcentajeZona = (totalZona * 100) / totalNeogcios;
                if (double.IsNaN(porcentajeZona))
                {
                    porcentajeZona = 0;
                }
                var limite = new LimiteCupo
                {
                    CantidadCupo = (int)Math.Floor((configuracion.LimiteCupo * porcentajeZona) / 100),
                    LimiteAnterior = 0,
                    //CantidadCupo = !ultima ? (int)Math.Floor((decimal)configuracion.LimiteCupo / zonas.Count()) :configuracion.LimiteCupo - configuracion.CantidadCupo.Sum(x => x.CantidadCupo),
                    ConfiguracionCupo = configuracion,
                    ZonaCupoId = zonas[i].Id,
                };
                configuracion.CantidadCupo.Add(limite);
            }
            if (!configuracion.CantidadCupo.Any(x => x.CantidadCupo > 0))
            {
                configuracion.CantidadCupo.First().CantidadCupo = configuracion.LimiteCupo;
            }
        }
        private Resultado Validar(ConfiguracionCupo cupo, List<DiaCupo> dias)
        {
            var errores = new Resultado();

            //if (cupo.MaterialId == 0)
            //{
            //    errores.Error("cupo", "Seleccione un Material");
            //}
            if (cupo.LimiteAlgoritmo > cupo.LimiteCupo)
            {
                errores.Error("Limite", "El límite del Algoritmo no debe superar la cantidad de " + cupo.LimiteCupo);
            }
            if (cupo.Fecha == new DateTime())
            {
                errores.Error("cupo", "La fecha no puede estar vacia");
            }
            if (cupo.Id == 0)
            {
                DateTime desde = dias.Min(x => x.Fecha);
                DateTime hasta = dias.Max(x => x.Fecha);
                if (repositorio.Existe<ConfiguracionCupo>(x => x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.Fecha >= desde && x.Fecha <= hasta))
                {
                    errores.Error("cupo", "Ya existe configuración para ese Material, Centro y Fecha");
                }
            }
            //else
            //{
            //    var limites = repositorio.Listar<LimiteCupo, LimiteCupoDto>(x => new LimiteCupoDto { Id = x.Id, CantidadCupo = x.CantidadCupo }, x => x.ConfiguracionCupoId == cupo.Id);
            //    if (limites != null && cupo.LimiteCupo < limites.Sum(x => x.CantidadCupo))
            //    {
            //        errores.Error("cupo", "La cantidad no debe ser menor a la configuración ya cargada");
            //    }
            //}
            return errores;
        }
        public KendoGrid<ConfiguracionCupoDto> TraerTodaConfiguracionCupo(KendoGridMvcRequest request)
        {
            var configuraciones = repositorio.ObtenerConsultaEscalar(new TraerConfiguracionesCupo(request));
            //var listaConfiguraciones = configuraciones.Data.ToList();
            var desde = configuraciones.Data.Min(a => a.Fecha);
            var hasta = configuraciones.Data.Max(a => a.Fecha);
            var centros = configuraciones.Data.Select(a => a.CentroCodigoSap).Distinct().ToList();
            var disponibilidades = cupoManager.TraerCupoDisponibilidad(desde, hasta, "", centros, "");

            foreach (var configuracion in configuraciones.Data.ToList())
            {
                var listaZonas = repositorio.Listar<ZonaCupo, string>(x => x.CodigoSap);

                var disponibilidad = disponibilidades.Where(a => a.Fecha == configuracion.Fecha && a.CentroCodigo == configuracion.CentroCodigoSap && a.MaterialCodigo == configuracion.MaterialCodigoSap).ToList();

                configuracion.CuposConsumidos += disponibilidad.Sum(X => X.Consumidos);
                configuracion.CuposDisponibles += disponibilidad.Sum(X => X.Disponibles);
            }
            return configuraciones;
        }
        public List<LimiteCupoDto> TraerLimites(int id)
        {
            var zonas = repositorio.Listar<LimiteCupo, LimiteCupoDto>(x => new LimiteCupoDto
            {
                Id = x.Id,
                ZonaCupo = x.ZonaCupo.CodigoSap,
                ZonaCupoId = x.ZonaCupoId,
                CantidadCupo = x.CantidadCupo,
            }, x => x.ConfiguracionCupoId == id);

            var configuracion = repositorio.Obtener<ConfiguracionCupo>(id);

            var listaZonas = repositorio.Listar<ZonaCupo, string>(x => x.CodigoSap);
            var disponibilidad = cupoManager.TraerCupoDisponibilidad(configuracion.Fecha, configuracion.Fecha, "", new List<string>() { configuracion.Centro.CodigoSap }, configuracion.Material.Codigo);
            foreach (var d in disponibilidad)
            {
                if (zonas.Any(x => x.ZonaCupo == d.ZonaId))
                {
                    zonas.Where(x => x.ZonaCupo == d.ZonaId).FirstOrDefault().Consumidos = d.Consumidos;
                    zonas.Where(x => x.ZonaCupo == d.ZonaId).FirstOrDefault().Disponible = d.Disponibles;
                }
            }
            return zonas;
        }
        private Resultado ValidarLimite(List<LimiteCupo> limite)
        {
            var resultado = new Resultado();
            var config = repositorio.Obtener<ConfiguracionCupo>(limite[0].ConfiguracionCupoId);
            if (limite.Sum(x => x.CantidadCupo) > config.LimiteCupo)
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
            var configuracion = repositorio.Obtener<ConfiguracionCupo, ConfiguracionCupoDto>(x => x.Id == id, x => new ConfiguracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                MaterialId = x.MaterialId,
                CentroId = x.CentroId,
                LimiteCupo = x.LimiteCupo,
                CierreCupera = x.CierreCupera,
                LimiteAlgoritmo = x.LimiteAlgoritmo,
                LiberarCupera = x.LiberarCupera,
                Centro = x.Centro.Descripcion,
                Material = x.Material.Descripcion,

            });

            var listaZonas = repositorio.Listar<ZonaCupo, string>(x => x.CodigoSap);
            foreach (var zona in listaZonas)
            {
                var disponibilidad = cupoManager.TraerCupoDisponibilidad(configuracion.Fecha, configuracion.Fecha, zona, new List<string>() { configuracion.CentroCodigoSap }, configuracion.MaterialCodigoSap);
                configuracion.CuposConsumidos += disponibilidad.Sum(X => X.Consumidos);
                configuracion.CuposConsumidos += disponibilidad.Sum(X => X.Consumidos);
                configuracion.CuposDisponibles += disponibilidad.Sum(X => X.Disponibles);
            }
            return configuracion;
        }
        public Resultado CambioMasivo(bool aceptar)
        {
            var errorSap = new Resultado();
            var materiales = repositorio.Listar<Material>();
            var zonas = repositorio.Listar<ZonaCupo>();
            var centros = repositorio.Listar<Centro>();
            try
            {
                var hoy = DateTime.Now.Date;
                var configuraciones = repositorio.Listar<ConfiguracionCupo>(a => a.Fecha >= hoy);

                foreach (var item in configuraciones.ToList())
                {
                    errorSap = EnviarConfiguracion(item, aceptar, item.LimiteCupo, materiales, zonas, centros);
                    if (errorSap.HayError)
                    {
                        errorSap.Error("CierreMasivo - Cabecera ", item.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                        return errorSap;
                    }
                    else
                    {
                        item.CierreCupera = aceptar;
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                errorSap.Error("CantidadCuposSAP", e.Message);
            }
            return errorSap;
        }
        public Resultado GrabarLimitesMasivo(List<LimiteCupo> limite, List<int> configuracionesIds)
        {
            var erroresSap = new Resultado();
            //var configuracionesCupo = repositorio.Listar<ConfiguracionCupo>(x => configuracionesIds.Contains(x.Id));
            var zonas = repositorio.Listar<ZonaCupo>();
            var materiales = repositorio.Listar<Material>();
            var centros = repositorio.Listar<Centro>();

            foreach (var id in configuracionesIds)
            {
                ConfiguracionCupo configuracionCupo = repositorio.Obtener<ConfiguracionCupo>(id);

                foreach (var item in configuracionCupo.CantidadCupo)
                {
                    item.LimiteAnterior = item.CantidadCupo;
                    item.CantidadCupo = limite.Where(a => a.ZonaCupoId == item.ZonaCupoId).First().CantidadCupo;
                }

                var errorSap = EnviarConfiguracion(configuracionCupo, null, configuracionCupo.LimiteCupo, materiales, zonas, centros);
                if (errorSap.HayError)
                {
                    erroresSap.Error("GrabarLimitesMasivo", configuracionCupo.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                    break;
                }
                repositorio.GuardarCambios();

                //foreach (var limiteCupo in configuracionCupo.CantidadCupo)
                //{
                //    limiteCupo.LimiteAnterior = limiteCupo.CantidadCupo;
                //    limiteCupo.CantidadCupo = limite.Where(a => a.ZonaCupoId == limiteCupo.ZonaCupoId).Single().CantidadCupo;
                //    if (limiteCupo.LimiteAnterior != limiteCupo.CantidadCupo)
                //    {
                //        var configDto = new ConfiguracionCupoDto
                //        {
                //            LimiteCupo = limiteCupo.CantidadCupo,
                //            CierreCupera = configuracionCupo.CierreCupera,
                //            Material = configuracionCupo.Material.Codigo,
                //            Fecha = configuracionCupo.Fecha,
                //            ZonaCupo = zonas.Where(x => x.Id == limiteCupo.ZonaCupoId).FirstOrDefault().CodigoSap,
                //            LimiteCupoAnterior = limiteCupo.LimiteAnterior.Value,
                //            Centro = configuracionCupo.Centro.CodigoSap,
                //        };
                //        try
                //        {
                //            var result = administracionCuperaAgent.AdministrarCupera(configDto);
                //            if (!result.Equals("OK"))
                //            {
                //                errorSap.Error("GrabarLimitesMasivo", configuracionCupo.Fecha.ToShortDateString() + ": " + result);
                //                break;
                //            }
                //        }
                //        catch (Exception e)
                //        {
                //            errorSap.Error("GrabarLimitesMasivoSAP", configuracionCupo.Fecha.ToShortDateString() + ": " + e.Message);
                //            break;
                //        }
                //    }
                //}
            }

            return erroresSap;
        }

        public Resultado ModificarConfiguracion(int? id, int? limite, int? algoritmo, bool? bloquear, bool? liberar)
        {
            var configuracionDto = repositorio.Obtener<ConfiguracionCupo, ConfiguracionCupoDto>(x => x.Id == id.Value,
                x => new ConfiguracionCupoDto
                {
                    LimiteCupo = limite != null ? limite.Value : x.LimiteCupo,
                    LimiteAlgoritmo = algoritmo != null ? algoritmo.Value : x.LimiteAlgoritmo,
                    CierreCupera = bloquear != null ? bloquear.Value : x.CierreCupera,
                    LiberarCupera = liberar != null ? liberar.Value : x.LiberarCupera,
                    Id = x.Id,
                    Fecha = x.Fecha,
                    MaterialId = x.MaterialId,
                    CentroId = x.CentroId,
                });
            var configuracion = new ConfiguracionCupo()
            {
                LimiteCupo = configuracionDto.LimiteCupo,
                LimiteAlgoritmo = configuracionDto.LimiteAlgoritmo,
                CierreCupera = configuracionDto.CierreCupera != null ? configuracionDto.CierreCupera.Value : false,
                LiberarCupera = configuracionDto.LiberarCupera,
                Id = configuracionDto.Id,
                Fecha = configuracionDto.Fecha,
                CentroId = configuracionDto.CentroId,
                MaterialId = configuracionDto.MaterialId
            };
            var dias = new List<DiaCupo>();
            return GrabarConfiguracionCupo(configuracion, dias);
        }
    }
}
