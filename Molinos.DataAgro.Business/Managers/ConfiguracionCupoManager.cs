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
                        var newConfiguracion = new ConfiguracionCupo
                        {
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
                    errorSap = EnviarConfiguracion(configuracion, null, configuracionSave.LimiteCupo, materiales, zonas, centros);
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
                //var ultima = i == zonas.Count() - 1;
                //logger.Debug("GenerarLimiteZona totalNeogcios" + totalNeogcios + " zonaCupo " + zonaCupo + " totalZona " + totalZona + " porcentajeZona " + porcentajeZona);
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
                var configuraciones = repositorio.Listar<ConfiguracionCupo>(x => ids.Contains(x.Id));

                foreach (var item in configuraciones.ToList())
                {
                    if (ids.Contains(item.Id))
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
                        //errorSap = ArmarLimiteZona(item.CantidadCupo.ToList(), item, true, materiales, zonas, centros, aceptar);
                        //if (!errorSap.HayError)
                        //{
                        //    item.CierreCupera = aceptar;
                        //}
                        //else
                        //{
                        //    errorSap.Error("Cierre Masivo - Cabecera + Zonas ", item.Fecha.ToShortDateString() + ": Error Sap:" + string.Join(", ", errorSap.Errores.Select(a => a.Source + " " + a.Message).ToList()));
                        //    break;
                        //}
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
    }
}
