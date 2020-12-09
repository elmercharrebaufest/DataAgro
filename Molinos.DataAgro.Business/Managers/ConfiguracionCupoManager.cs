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
                var error = new CupoResult { ListaCupos = new List<string>() };
                var materiales = repositorio.Listar<Material>();
                var zonas = repositorio.Listar<ZonaCupo>();
                var centros = repositorio.Listar<Centro>();
                var errorSap = new Resultado();
                if (configuracion.Id == 0)
                {
                    foreach (var d in dias)
                    {
                        var newConfiguracion = new ConfiguracionCupo {
                            CentroId = configuracion.CentroId,
                            CierreCupera = configuracion.CierreCupera,
                            MaterialId = configuracion.MaterialId
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
                            newConfiguracion.CantidadCupo = new List<LimiteCupo>();

                            errorSap = EnviarCabeceraConfiguracion(newConfiguracion, null, null, materiales, zonas, centros);
                            if (errorSap.HayError)
                            {
                                errorSap.Error("GrabarConfiguracionCupo - Cabecera ", newConfiguracion.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                                return errorSap;
                            }
                            //else
                            //{
                            //    repositorio.Agregar(newConfiguracion);
                            //}
                            GenerarLimiteZona(newConfiguracion, zonas);
                            errorSap = EnviarLimiteZona(newConfiguracion.CantidadCupo.ToList(), newConfiguracion, false, materiales, zonas, centros, newConfiguracion.CierreCupera);
                            if (!errorSap.HayError)
                            {
                                repositorio.Agregar(newConfiguracion);
                            }
                            else
                            {
                                errorSap.Error("GrabarConfiguracionCupo - Zonas ", newConfiguracion.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));

                                return errorSap;
                            }
                            repositorio.GuardarCambios();

                        }
                    }
                }
                else
                {
                    var configuracionSave = repositorio.Obtener<ConfiguracionCupo>(configuracion.Id);
                    errorSap = EnviarCabeceraConfiguracion(configuracion, null, configuracionSave.LimiteCupo, materiales, zonas, centros);
                    if (errorSap.HayError)
                    {
                        errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                        return errorSap;
                    }
                    else
                    {
                        configuracionSave.LimiteCupo = configuracion.LimiteCupo;
                        configuracionSave.CierreCupera = configuracion.CierreCupera;

                    }
                    errorSap = EnviarLimiteZona(configuracionSave.CantidadCupo.ToList(), configuracion, true, materiales, zonas, centros, configuracion.CierreCupera);
                    if (!errorSap.HayError)
                    {
                        configuracionSave.LimiteCupo = configuracion.LimiteCupo;
                        configuracionSave.CierreCupera = configuracion.CierreCupera;
                    }
                    else
                    {
                        errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                        return errorSap;
                    }
                    repositorio.GuardarCambios();
                }
                //cupoManager.CrearSugerenciaCupo(configuracion);
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
                foreach (var lim in limite)
                {
                    if (lim.Id == 0)
                    {
                        repositorio.Agregar(lim);
                    }
                    else
                    {
                        var limSave = repositorio.Obtener<LimiteCupo>(lim.Id);
                        var configuracion = repositorio.Obtener<ConfiguracionCupo>(lim.ConfiguracionCupoId);
                        var zonas = repositorio.Listar<ZonaCupo>();
                        var errorSap = new Resultado();
                        var result = "";
                        var configDto = new ConfiguracionCupoDto
                        {
                            LimiteCupo = lim.CantidadCupo,
                            CierreCupera = configuracion.CierreCupera,
                            Material = configuracion.Material.Codigo,
                            Fecha = configuracion.Fecha,
                            ZonaCupo = zonas.Where(x => x.Id == lim.ZonaCupoId).FirstOrDefault().CodigoSap,
                            LimiteCupoAnterior = lim.LimiteAnterior.Value,
                            Centro = configuracion.Centro.CodigoSap,
                        };
                        try
                        {
                            result = administracionCuperaAgent.AdministrarCupera(configDto);
                            if (!result.Equals("OK"))
                            {
                                errorSap.Error("GrabarLimites", configuracion.Fecha.ToShortDateString() + ": " + result);
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + e.Message);
                            break;
                        }

                        limSave.CantidadCupo = lim.CantidadCupo;
                        limSave.LimiteAnterior = lim.LimiteAnterior;

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
        private Resultado EnviarCabeceraConfiguracion(ConfiguracionCupo configuracion, bool? aceptar, int? limiteAnterior, List<Material> materiales, List<ZonaCupo> zonas, List<Centro> centros)
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
                //var ultima = i == zonas.Count() - 1;
                //logger.Debug("GenerarLimiteZona totalNeogcios" + totalNeogcios + " zonaCupo " + zonaCupo + " totalZona " + totalZona + " porcentajeZona " + porcentajeZona);
                if (double.IsNaN(porcentajeZona))
                {
                    porcentajeZona = 0;
                }
                var limite = new LimiteCupo
                {
                    CantidadCupo = (int)Math.Floor((configuracion.LimiteCupo * porcentajeZona) / 100),
                    //CantidadCupo = !ultima ? (int)Math.Floor((decimal)configuracion.LimiteCupo / zonas.Count()) :configuracion.LimiteCupo - configuracion.CantidadCupo.Sum(x => x.CantidadCupo),
                    ConfiguracionCupo = configuracion,
                    ZonaCupoId = zonas[i].Id,
                };
                configuracion.CantidadCupo.Add(limite);
            }
        }
        private Resultado EnviarLimiteZona(List<LimiteCupo> limite, ConfiguracionCupo configuracion, bool anterior, List<Material> materiales, List<ZonaCupo> zonas, List<Centro> centros, bool cerrarCupera)
        {
            logger.Debug("EnviarLimiteZona");
            var errorSap = new Resultado();
            var result = "";
            foreach (var item in limite)
            {
                logger.Debug("EnviarLimiteZona " + item.ZonaCupoId);

                var configDto = new ConfiguracionCupoDto
                {
                    LimiteCupo = item.CantidadCupo,
                    CierreCupera = cerrarCupera,
                    Material = materiales.Where(x => x.MaterialId == configuracion.MaterialId).FirstOrDefault().Codigo,
                    Fecha = configuracion.Fecha,
                    ZonaCupo = zonas.Where(x => x.Id == item.ZonaCupoId).FirstOrDefault().CodigoSap,
                    LimiteCupoAnterior = anterior ? (item.LimiteAnterior ?? 0) : 0,
                    Centro = centros.Where(x => x.Id == configuracion.CentroId).FirstOrDefault().CodigoSap,
                };
                try
                {
                    result = administracionCuperaAgent.AdministrarCupera(configDto);
                    if (!result.Equals("OK"))
                    {
                        logger.Debug("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + result);
                        errorSap.Error("EnviarLimiteZona", configuracion.Fecha.ToShortDateString() + ": " + result);
                        break;
                    }
                }
                catch (Exception e)
                {
                    errorSap.Error("CantidadCuposSAP", configuracion.Fecha.ToShortDateString() + ": " + e.Message);
                    break;
                }

            }
            return errorSap;
        }
        private Resultado Validar(ConfiguracionCupo cupo, List<DiaCupo> dias)
        {
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
            return repositorio.Obtener<ConfiguracionCupo, ConfiguracionCupoDto>(x => x.Id == id, x => new ConfiguracionCupoDto
            {
                Id = x.Id,
                Fecha = x.Fecha,
                MaterialId = x.MaterialId,
                CentroId = x.CentroId,
                LimiteCupo = x.LimiteCupo,
                CierreCupera = x.CierreCupera
            });
        }

        public Resultado CambioMasivo(List<int> ids, bool aceptar)
        {
            var errorSap = new Resultado();
            var materiales = repositorio.Listar<Material>();
            var zonas = repositorio.Listar<ZonaCupo>();
            var centros = repositorio.Listar<Centro>();
            try
            {
                var configuraciones = repositorio.Listar<ConfiguracionCupo>();

                foreach (var item in configuraciones.ToList())
                {
                    if (ids.Contains(item.Id))
                    {

                        errorSap = EnviarCabeceraConfiguracion(item, aceptar, item.LimiteCupo, materiales, zonas, centros);
                        if (errorSap.HayError)
                        {
                            errorSap.Error("CierreMasivo - Cabecera ", item.Fecha.ToShortDateString() + ": " + "Error sap" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                            return errorSap;
                        }
                        else
                        {
                            item.CierreCupera = aceptar;
                        }
                        errorSap = EnviarLimiteZona(item.CantidadCupo.ToList(), item, true, materiales, zonas, centros, aceptar);
                        if (!errorSap.HayError)
                        {
                            item.CierreCupera = aceptar;
                        }
                        else
                        {
                            errorSap.Error("Cierre Masivo - Cabecera + Zonas ", item.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                            break;
                        }
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


    }
}
