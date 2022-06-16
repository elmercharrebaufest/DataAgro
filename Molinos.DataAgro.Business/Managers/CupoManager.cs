using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Criterios;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace Molinos.DataAgro.Business.Managers
{
    public class CupoManager : ICupoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly ICrearCupoAgent crearCupoAgent;
        private readonly IEliminarCupoAgent eliminarCupoAgent;
        private readonly IClienteStopAgent clienteStopAgent;
        private readonly IModificarCupoAgent modificarCupoAgent;
        private readonly IProveedorManager proveedorManager;
        private readonly IMailManager mailManager;
        private readonly IServicioCriterios servicioCriterios;
        private readonly IDisponibilidadCuposAgent disponibilidadCuposAgent;
        private readonly ICriterioCDWarrantAgent cdWarrant;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IComercialManager comercialManager;
        private readonly IServicioRepositorioScatoAgent servicioScato;
        private readonly IHttpContextManager httpContextManager;
        private readonly IAltaTempranaAgent altaTempranaAgent;
        private readonly ICumplimientoCuposAgent cumplimientoCuposAgent;
        private readonly IContratoKgPendienteAgent contratoKgPendienteAgent;

        public CupoManager(IRepositorio repositorio, ILogger logger, ICrearCupoAgent crearCupoAgent,
            IEliminarCupoAgent eliminarCupoAgent, IClienteStopAgent clienteStopAgent, IModificarCupoAgent modificarCupoAgent,
            IProveedorManager proveedorManager, IMailManager mailManager, IServicioCriterios servicioCriterios,
            IDisponibilidadCuposAgent disponibilidadCuposAgent, ICriterioCDWarrantAgent cdWarrant, ILogDataAgroManager logDataAgroManager,
            IComercialManager comercialManager, IServicioRepositorioScatoAgent servicioScato, IHttpContextManager httpContextManager,
            IAltaTempranaAgent altaTempranaAgent, ICumplimientoCuposAgent cumplimientoCuposAgent, IContratoKgPendienteAgent contratoKgPendienteAgent)
        {
            this.repositorio = repositorio;
            this.logger = logger;
            this.crearCupoAgent = crearCupoAgent;
            this.eliminarCupoAgent = eliminarCupoAgent;
            this.clienteStopAgent = clienteStopAgent;
            this.modificarCupoAgent = modificarCupoAgent;
            this.proveedorManager = proveedorManager;
            this.mailManager = mailManager;
            this.servicioCriterios = servicioCriterios;
            this.disponibilidadCuposAgent = disponibilidadCuposAgent;
            this.cdWarrant = cdWarrant;
            this.logDataAgroManager = logDataAgroManager;
            this.comercialManager = comercialManager;
            this.servicioScato = servicioScato;
            this.httpContextManager = httpContextManager;
            this.altaTempranaAgent = altaTempranaAgent;
            this.cumplimientoCuposAgent = cumplimientoCuposAgent;
            this.contratoKgPendienteAgent = contratoKgPendienteAgent;
        }
        public CupoResult GrabarCupo(Cupo cupo, List<DiaCupo> dias)
        {
            var error = new CupoResult { ListaCupos = new List<string>() };
            try
            {
                var comercial = repositorio.Obtener<Comercial>(cupo.ComercialId);
                cupo.Comercial = comercial;
                if (!error.HayError)
                {
                    cupo.Material = repositorio.Obtener<Material>(cupo.MaterialId);
                    cupo.Centro = repositorio.Obtener<Centro>(cupo.CentroId);
                    cupo.Proveedor = repositorio.Obtener<Proveedor>(cupo.ProveedorId);
                    cupo.ZonaCupo = repositorio.Obtener<ZonaCupo>(cupo.ZonaCupoId);

                    if (PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                    {
                        cupo.UsuarioCreador = PermisosHelper.ObtenerUsuario();
                        cupo.EstadoCupoId = 6;
                    }

                    if (cupo.Id == 0)
                    {
                        foreach (var d in dias)
                        {
                            if (d.Cantidad > 0)
                            {
                                if (d.Fecha < DateTime.Today)
                                {
                                    error.Error("CantidadCuposSAP", d.Fecha.ToShortDateString() + ": La Fecha de Ingreso no debe ser una fecha menor al día de hoy");
                                    continue;
                                }
                                ////CierreCupera
                                //var limitePorZona = this.TraerTodaConfiguracionCupoPorDia(cupo.ZonaCupoId, cupo.MaterialId, cupo.CentroId, d.Fecha);
                                //if (limitePorZona.Count() <= 0)
                                //{
                                //    error.Error("Cupera", "No hay cupera habilitada para el día " + d.Fecha.ToString("dd/MM/yyyy"));
                                //    continue;
                                //}
                                ////else if (limitePorZona.All(x => (x.LimiteCupo) <= 0))
                                ////{
                                ////    error.Error("Cupera", "No hay límite de cupo disponible para la zona");
                                ////}
                                cupo.FechaIngreso = d.Fecha;

                                //if (cupo.NegocioId == null && cupo.ConfiguracionEspacioDinamicoId == null)
                                //{
                                //    var creados = repositorio.Contar<Cupo>(
                                //        x => x.NegocioId == null && x.ConfiguracionEspacioDinamicoId == null && x.FechaIngreso == cupo.FechaIngreso
                                //        && x.MaterialId == cupo.MaterialId && x.CentroId == cupo.CentroId && x.EstadoCupoId != 9);
                                //    var fechaIngreso = cupo.FechaIngreso.Date;
                                //    var total = repositorio.Obtener<ConfiguracionCupo, int>(
                                //        x => x.MaterialId == cupo.MaterialId && x.Fecha == fechaIngreso && x.CentroId == cupo.CentroId && x.CierreCupera == false,
                                //        x => x.LimiteCupo - x.LimiteAlgoritmo);
                                //    if (d.Cantidad > total - creados)
                                //    {
                                //        error.Error("Cupera", "La cantidad de cupos solicitada excede la cantidad disponible para el día " + cupo.FechaIngreso.ToString("dd/MM/yyyy"));
                                //        continue;
                                //    }
                                //}

                                var listaCupos = new List<string>();
                                var errorSap = new Resultado();
                                var cuposConSap = new List<Cupo>();
                                if (!PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
                                {

                                    try
                                    {
                                        // cupos para Vicentin en CupoExterno
                                        if (cupo.Centro.NoPropio)
                                        {
                                            var consumidos = repositorio.Contar<Cupo>(x =>
                                            x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.FechaIngreso == cupo.FechaIngreso &&
                                            cupo.ZonaCupoId == x.ZonaCupoId && x.EstadoCupoId != 4 && x.EstadoCupoId != 9);

                                            ConfiguracionCupoDto limitePorZona = TraerLimitePorZona(cupo, d);

                                            if (limitePorZona == null)
                                            {
                                                error.Error("Cupera", "No hay cupera habilitada para el día " + d.Fecha.ToString("dd/MM/yyyy"));
                                                continue;
                                            }
                                            else if (limitePorZona.LimiteCupo <= 0)
                                            {
                                                error.Error("Cupera", "No hay límite de cupo disponible para la zona");
                                                continue;
                                            }

                                            var disponibles = limitePorZona.LimiteCupo - consumidos;
                                            if (disponibles <= 0)
                                            {
                                                error.Error("Cupera", "No hay límite de cupo disponible para la zona");
                                                continue;
                                            }

                                            disponibles = disponibles > d.Cantidad.Value ? d.Cantidad.Value : disponibles;

                                            var cuposNoPropios = repositorio.Listar<CupoNoPropio>(x => x.CupoId == null && x.Disponible == true && x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.FechaIngreso == cupo.FechaIngreso,
                                                disponibles);

                                            foreach (var cupoNp in cuposNoPropios)
                                            {
                                                listaCupos.Add(cupoNp.Codigo);
                                            }
                                        }
                                        else
                                        {
                                            listaCupos = crearCupoAgent.Crear(cupo, d.Cantidad.Value);
                                        }
                                    }
                                    catch (Exception e)
                                    {
                                        errorSap.Error("CantidadCuposSAP", cupo.FechaIngreso.ToShortDateString() + ": " + e.Message);
                                    }
                                    if (errorSap.HayError)
                                    {
                                        error.Errores.AddRange(errorSap.Errores);
                                        continue;
                                    }
                                    cupo.EstadoCupoId = cupo.Centro.CodigoSap == "1600" || cupo.Centro.CodigoSap == "1029" ? 6 : (cupo.Centro.NoPropio) ? 1 : 8;
                                    foreach (var cupoSap in listaCupos)
                                    {
                                        var nuevoCupo = (Cupo)cupo.Clone();
                                        nuevoCupo.CupoSap = cupoSap;
                                        cuposConSap.Add(nuevoCupo);
                                    }

                                    repositorio.AgregarTodos(cuposConSap);
                                    repositorio.GuardarCambios();

                                    List<CupoNoPropio> cuposnoPropio = new List<CupoNoPropio>();
                                    foreach (var cupoNuevo in cuposConSap)
                                    {
                                        if (cupo.Id == 0)
                                        {
                                            var cupoConId = repositorio.ObtenerMayor<Cupo, int>(x => x.CupoSap == cupoNuevo.CupoSap, x => x.Id);
                                            if (cupoNuevo.Centro.NoPropio)
                                            {
                                                var cupoVicentin = repositorio.Obtener<CupoNoPropio>(x => x.Codigo == cupoNuevo.CupoSap);
                                                cupoVicentin.Cupo = cupoConId;
                                                cupoVicentin.CupoId = cupoConId.Id;
                                                cuposnoPropio.Add(cupoVicentin);
                                            }
                                            logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoConId.Id, null), TipoAccionLogDataAgro.Crear);
                                        }
                                    }


                                    if (listaCupos.Count < d.Cantidad.Value)
                                    {
                                        error.Error("CantidadCuposSAP", "Se generaron " + listaCupos.Count + " de " + d.Cantidad.Value + " cupos solicitados para el dia " + cupo.FechaIngreso.ToShortDateString());
                                    }
                                    error.ListaCupos.AddRange(listaCupos);
                                }
                                else
                                {
                                    for (int i = 0; i < d.Cantidad.Value; i++)
                                    {
                                        var nuevoCupo = (Cupo)cupo.Clone();
                                        cuposConSap.Add(nuevoCupo);
                                    }
                                    repositorio.AgregarTodos(cuposConSap);
                                    repositorio.GuardarCambios();
                                }

                            }
                        }
                        if (error.ListaCupos.Count > 0)
                        {
                            if (!cupo.Centro.NoPropio)
                            {
                                EnviarEmail(cupo, error.ListaCupos);
                            }
                        }
                        return error;
                    }
                    else
                    {
                        var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
                        var cupoSave = repositorio.Obtener<Cupo>(cupo.Id);
                        cupoSave.ProveedorId = cupo.ProveedorId;
                        cupoSave.Proveedor = cupo.Proveedor;
                        cupoSave.Calidad = cupo.Calidad;
                        cupoSave.Observaciones = cupo.Observaciones;
                        cupoSave.Destinatario = cupo.Destinatario;
                        cupoSave.Fason = cupo.Fason;
                        cupoSave.FleteProcedencia = cupo.FleteProcedencia;
                        cupoSave.NegocioId = cupo.NegocioId;
                        cupoSave.ComercialId = cupo.ComercialId;
                        cupoSave.Comercial = cupo.Comercial;
                        //cupoSave.ConDescarga = cupo.ConDescarga;
                        if (cupoSave.Centro.NoPropio)
                        {
                            if (!string.IsNullOrEmpty(cupoSave.CTG))
                            {
                                error.Error("Centro", "Los cupos con CTG para centros no propios no pueden ser editados.");
                                return error;
                            }
                            var cupoNoPropio = repositorio.Obtener<CupoNoPropio>(x => x.CupoId == cupoSave.Id);
                            cupoNoPropio.Estado = cupoSave.EstadoCupoId;
                        }
                        if (!cupoSave.Centro.NoPropio)
                        {
                            var res = modificarCupoAgent.Modificar(cupoSave);
                            if (res != "Ok")
                            {
                                error.Error("SAP", $"Error al grabar en SAP: {res}");
                            }
                            if (!cupoSave.Centro.Acopio && !cupoSave.Centro.NoPropio)
                            {
                                if (datosConfiguracion.ConexionABMStop.HasValue && datosConfiguracion.ConexionABMStop.Value)
                                {
                                    if (cupoSave.CupoStop != null)
                                    {
                                        clienteStopAgent.ModificarCupo(cupoSave);
                                    }
                                }
                                else
                                {
                                    error.Error("Stop", "Sin Conexión a Stop. Modificado en SAP");
                                }
                            }
                        }
                        repositorio.GuardarCambios();
                        var asd = ObtenerCupo(cupoSave.Id, null);
                        logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSave.Id, null), TipoAccionLogDataAgro.Modificar);
                        return error;

                    }
                }
                return error;
            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Errores.Add(new ErrorMessage(400, e.Message));
                return error;
            }
        }

        private ConfiguracionCupoDto TraerLimitePorZona(Cupo cupo, DiaCupo d)
        {
            return repositorio.Obtener<LimiteCupo, ConfiguracionCupoDto>(
                x => DbFunctions.TruncateTime(x.ConfiguracionCupo.Fecha) == d.Fecha
                && x.ZonaCupoId == cupo.ZonaCupoId
                && x.ConfiguracionCupo.CentroId == cupo.CentroId
                && x.ConfiguracionCupo.MaterialId == cupo.MaterialId
                && !x.ConfiguracionCupo.CierreCupera,
                x => new ConfiguracionCupoDto
                {
                    Id = x.Id,
                    Fecha = x.ConfiguracionCupo.Fecha,
                    MaterialId = x.ConfiguracionCupo.MaterialId,
                    CentroId = x.ConfiguracionCupo.CentroId,
                    LimiteCupo = x.CantidadCupo,
                });
        }

        public Resultado Validar(Cupo cupo, int cantidadCupos, DateTime? fechaHasta)
        {
            var error = new Resultado();
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == cupo.ProveedorId);
            var negocio = repositorio.Obtener<Negocio>(x => x.Id == cupo.NegocioId);
            if (proveedor == null)
            {
                error.Error("ProveedorId", "El campo 'Proveedor' es obligatorio");
                return error;
            }
            if (proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                error.Error("ProveedorId", "Proveedor deshabilitado");
            }
            if (cupo.ProveedorId == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "El Proveedor no debe estar vacío"));
            }
            if (cupo.ProveedorId != 0 && ((cupo.NegocioId == null && proveedor.Comisionista != true) || (negocio != null && negocio.ProveedorComisionistaId == null)))
            {
                var cuit = repositorio.Obtener<Proveedor, string>(y => y.ProveedorId == cupo.ProveedorId, y => y.CUIT);
                var sisa = repositorio.Obtener<SISA>(x => x.CUIT == cuit && x.SituacionCategoria == "AL");
                if (sisa == null)
                {
                    error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor No Operable por CUIT Inactivo"));
                }
                else
                {
                    if (sisa.EstadoCuit == 3 && proveedor.RiesgoComercialSap != "E")
                    {
                        error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor No Operable por Estado de CUIT 3"));
                    }
                    else if (sisa.EstadoCuit == 0)
                    {
                        error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor No Operable por Estado de CUIT Inactivo"));
                    }
                }

                if (!string.IsNullOrEmpty(proveedor.RiesgoComercialSap) && proveedor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"] && proveedor.CuposConRiesgo != true)
                {
                    error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor No Operable por Riesgo Comercial Alto"));
                }

                if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == proveedor.ProveedorId && x.EstadoId == 4))
                {
                    error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor no Operable por Estado BAJA"));
                }

                if (repositorio.Existe<FACACOP>(x => x.CUIT == proveedor.CUIT))
                {
                    error.Errores.Add(new ErrorMessage(400, "Corredor/Proveedor No Operable por ser Apócrifo"));
                }

                if (proveedor.SegmentacionId != 5 && proveedor.SegmentacionId != 7)
                {
                    var alta = altaTempranaAgent.ObtenerAlta(proveedor.CUIT);
                    if (string.IsNullOrEmpty(alta.Mensaje))
                    {
                        if (alta.ProveedorGrano == "SI")
                        {
                            error.Errores.Add(new ErrorMessage(400, "El Corredor/Proveedor es un vendedor eventual"));
                        }
                    }
                    else
                    {
                        error.Errores.Add(new ErrorMessage(400, alta.Mensaje));
                    }
                }

            }
            if (cupo.Id == 0 && cantidadCupos == 0)
            {
                error.Errores.Add(new ErrorMessage(400, "La Cantidad no debe estar vacía"));
            }
            if (cupo.MaterialId == 3 && (cupo.Calidad == "" || cupo.Calidad == null))
            {
                error.Errores.Add(new ErrorMessage(400, "La calidad no debe estar vacia para Soja"));
            }
            if (cupo.Fason == true && (cupo.Destinatario == "" || cupo.Destinatario == null))
            {
                error.Errores.Add(new ErrorMessage(400, "CUIT Destinatario no debe estar vacío cuando elige Fasón/Préstamo Devolución"));
            }
            if (cupo.FechaIngreso.Date < DateTime.Today && fechaHasta.HasValue && fechaHasta < DateTime.Today)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha de Ingreso no debe ser una fecha menor al día de hoy"));
            }
            if (cupo.Id == 0 && fechaHasta.HasValue && fechaHasta < cupo.FechaIngreso)
            {
                error.Errores.Add(new ErrorMessage(400, "La Fecha Hasta de entrega no puede ser menor a la Fecha Desde"));
            }


            var centro = repositorio.Obtener<Centro>(x => x.Id == cupo.CentroId);
            if (centro != null && centro.NoPropio)
            {
                if (cupo.FleteProcedencia == true)
                {
                    error.Errores.Add(new ErrorMessage(400, "Los cupos con flete para centros no propios  estan dehabilitados."));
                }
                var cuposVicentin = repositorio.Listar<CupoNoPropio>(x => x.CupoId == null && x.Disponible == true && x.CentroId == cupo.CentroId && x.MaterialId == cupo.MaterialId && x.FechaIngreso == cupo.FechaIngreso).Take(cantidadCupos);
                if (cuposVicentin.Count() < cantidadCupos)
                {
                    error.Errores.Add(new ErrorMessage(400, "No existen cupos suficientes disponibles para el centro."));
                }
                if (!string.IsNullOrEmpty(cupo.CTG))
                {
                    error.Errores.Add(new ErrorMessage(400, "Los cupos con CTG para centros no propios no pueden ser editados."));
                }
            }

            //if ((cupo.NegocioId == 0 || cupo.NegocioId == null) && !PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            //{
            //    error.Errores.Add(new ErrorMessage(400, "Debe seleccionar un negocio"));
            //}
            var tieneQueValidar = proveedor.SegmentacionId == 2 || proveedor.SegmentacionId == 3 && proveedor.SegmentacionId == 4;
            if (proveedor != null && tieneQueValidar && centro != null && centro.CodigoSap == "1600")
            {
                var establecimientos = TraerEstablecimientos(proveedor.CUIT);
                var cantidadCupo = 0;
                if (establecimientos != null && establecimientos.Count > 0)
                {
                    foreach (var establecimiento in establecimientos)
                    {
                        if (establecimiento.Cantidad >= 28000)
                        {
                            cantidadCupo += (int)Math.Floor(establecimiento.Cantidad / 28000);
                        }
                    }
                    if (cantidadCupo < cantidadCupos)
                    {
                        if (cantidadCupo < 0)
                        {
                            error.Errores.Add(new ErrorMessage(400, "No se encontraron establecimientos con stock disponible"));
                            return error;
                        }
                        error.Errores.Add(new ErrorMessage(400, "No hay disponibilidad de stock para los cupos ingresados, la cantidad en cupos disponibles es: " + cantidadCupo));
                    }
                }
                else
                {
                    error.Errores.Add(new ErrorMessage(400, "No se encontraron establecimientos con stock disponible"));
                }
            }
            return error;
        }
        public DataSourceResult TraerCuposTabla(DataSourceRequest request, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosCupos(request, equipo));
        }

        public Resultado EliminarCupo(int id, string comercial, bool enviarMail)
        {
            try
            {
                var nuevoResultado = new Resultado();

                var cupoSap = repositorio.Obtener<Cupo>(id);
                var datosConfiguracion = repositorio.Obtener<Configuracion>(1);

                var resultStop = new Resultado();
                if (!cupoSap.Centro.NoPropio)
                {
                    resultStop = AnularCupoStop(cupoSap, datosConfiguracion, null);
                }
                if (!resultStop.HayError)
                {
                    if (cupoSap.Centro.NoPropio)
                    {
                        if (cupoSap.EstadoCupoId != 1)
                        {
                            var nuevoError = new Resultado();
                            nuevoError.Error("Error", "El cupo no puede ser anulado. Cupo: " + cupoSap.CupoSap);
                            return nuevoError;
                        }
                        var cuponoPropio = repositorio.Obtener<CupoNoPropio>(x => x.CupoId == cupoSap.Id);
                        cuponoPropio.CupoId = null;
                    }

                    cupoSap.EstadoCupoId = 4;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSap.Id, null), TipoAccionLogDataAgro.Eliminar);
                    if (!cupoSap.Centro.NoPropio)
                    {
                        var resultado = eliminarCupoAgent.Eliminar(cupoSap.CupoSap, comercial);
                        if (resultado != "OK")
                        {
                            resultStop.Error("SAP", $"Anulado correctamente en STOP, Error al anular en SAP: {resultado}");
                        }
                    }
                    var cupos = new List<CupoDto> {
                       new CupoDto {
                        ZonaCupo = cupoSap.ZonaCupo?.Descripcion,
                        CupoSap = cupoSap.CupoSap,
                        Proveedor = cupoSap.Proveedor?.RazonSocial,
                        Material = cupoSap.Material?.Descripcion,
                        ProveedorId = cupoSap.ProveedorId,
                        ComercialId = cupoSap.ComercialId
                       }
                    };
                    if (enviarMail)
                    {
                        EnviarMailProveedorAnulacionCupo(cupos);
                        EnviarMailComercialAnulacionCupo(comercial, cupos);
                        EnviarMailCreadorAnulacionCupo(cupos, comercial);
                    }
                }
                return resultStop;
            }
            catch (Exception e)
            {
                var nuevoResultado = new Resultado();
                nuevoResultado.Error("eliminar", e.Message);
                return nuevoResultado;
            }
        }

        private Resultado AnularCupoStop(Cupo cupoSap, Configuracion datosConfiguracion, ClienteStopAgent cliente)
        {
            logger.Debug("AnularCupoStop " + cupoSap.CupoSap + " " + cupoSap.ToJson());
            var nuevoResultado = new Resultado();
            if (!cupoSap.Centro.Acopio && cupoSap.EstadoCupoId == 1 && cupoSap.CupoStop != null)
            {
                if (datosConfiguracion.ConexionABMStop.HasValue && !datosConfiguracion.ConexionABMStop.Value)
                {
                    nuevoResultado.Error("Error", "Error al anular el cupo " + cupoSap.CupoSap + " en STOP: Sin conexión a STOP.");
                    return nuevoResultado;
                }
                var resultadoStop = cliente != null ? cliente.EliminarCupo(cupoSap) : clienteStopAgent.EliminarCupo(cupoSap);
                if (resultadoStop.HayError)
                {
                    logger.Debug("AnularCupoStop HayError " + cupoSap.CupoSap + " " + resultadoStop.Errores.Select(a => a.Message).ToJson());
                    foreach (var e in resultadoStop.Errores)
                    {
                        nuevoResultado.Error("Error", $"Error al anular el cupo { cupoSap.CupoSap } en STOP: {e.Message}."); ;
                    }
                    return nuevoResultado;
                }
            }
            return nuevoResultado;
        }

        public Resultado TransmitirCupos(List<string> cupos)
        {
            var result = new Resultado();
            var datosConfiguracion = repositorio.Obtener<Configuracion>(1);
            if (datosConfiguracion.ConexionABMStop.HasValue && !datosConfiguracion.ConexionABMStop.Value)
            {
                result.Error("Stop", "Sin conexión a STOP");
                return result;
            }
            try
            {
                clienteStopAgent.CrearCupo(cupos);
            }
            catch (Exception e)
            {
                result.Error("", e.Message);
            }
            return result;
        }
        public void TransmitirCupos()
        {
            clienteStopAgent.TransmitirJobCupos();
        }
        public List<RespuestaCupoStop> ConsultarCuposDiarios()
        {
            return clienteStopAgent.ConsultarCuposDiarios();
        }
        public CupoDto ObtenerCupo(int id, RepositorioEF repo)
        {
            var r = repo != null ? repo : repositorio;
            return r.Obtener<Cupo, CupoDto>(x => x.Id == id, x => new CupoDto
            {
                Id = x.Id,
                Calidad = x.Calidad,
                Centro = x.Centro.Descripcion,
                CentroId = x.CentroId,
                CentroCodigo = x.Centro.CodigoSap,
                ComercialId = x.ComercialId,
                Destinatario = x.Destinatario,
                Fason = x.Fason,
                FleteProcedencia = x.FleteProcedencia,
                FechaIngreso = x.FechaIngreso,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                Observaciones = x.Observaciones,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap,
                EstadoCupo = x.EstadoCupo.Descripcion,
                EstadoCupoId = x.EstadoCupoId,
                CartaPorte = x.CartaPorte,
                Chofer = x.Chofer,
                CodLocalidadOrigen = x.CodLocalidadOrigen,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                CorredorComprador = x.CorredorComprador,
                CorredorVendedor = x.CorredorVendedor,
                Cosecha = x.Cosecha,
                CTG = x.CTG,
                CTGFechaDesde = x.CTGFechaDesde,
                CTGFechaHasta = x.CTGFechaHasta,
                CuitOrigen = x.CuitOrigen,
                CuitOrigenAfip = x.CuitOrigenAfip,
                CupoStop = x.CupoStop == null ? "" : x.CupoSap.ToString(),
                EstadoPlanta = x.EstadoPlanta,
                FechaGeneracion = x.FechaGeneracion,
                FechaRegistro = x.FechaGeneracion,
                IntermediarioFlete = x.IntermediarioFlete,
                Km = x.Km,
                MercadoATermino = x.MercadoATermino,
                MotivoRechazo = x.MotivoRechazo,
                NroEstablecimientoOrigen = x.NroEstablecimientoOrigen,
                Peso = x.Peso,
                RemitenteComercial = x.RemitenteComercial,
                Transportista = x.Transportista,
                UsuarioCreador = x.UsuarioCreador,
                ZonaCupoSap = x.ZonaCupo.CodigoSap,
                Acopio = x.Centro.Acopio,
                NegocioId = x.NegocioId,
                ConDescarga = x.ConDescarga
            });
        }

        public List<CupoDto> ObtenerCupos(List<int> id, RepositorioEF repo)
        {
            var r = repo != null ? repo : repositorio;
            return r.Listar<Cupo, CupoDto>(x => new CupoDto
            {
                Id = x.Id,
                Calidad = x.Calidad,
                Centro = x.Centro.Descripcion,
                CentroId = x.CentroId,
                CentroCodigo = x.Centro.CodigoSap,
                ComercialId = x.ComercialId,
                Destinatario = x.Destinatario,
                Fason = x.Fason,
                FleteProcedencia = x.FleteProcedencia,
                FechaIngreso = x.FechaIngreso,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                Observaciones = x.Observaciones,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap,
                EstadoCupo = x.EstadoCupo.Descripcion,
                EstadoCupoId = x.EstadoCupoId,
                CartaPorte = x.CartaPorte,
                Chofer = x.Chofer,
                CodLocalidadOrigen = x.CodLocalidadOrigen,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                CorredorComprador = x.CorredorComprador,
                CorredorVendedor = x.CorredorVendedor,
                Cosecha = x.Cosecha,
                CTG = x.CTG,
                CTGFechaDesde = x.CTGFechaDesde,
                CTGFechaHasta = x.CTGFechaHasta,
                CuitOrigen = x.CuitOrigen,
                CuitOrigenAfip = x.CuitOrigenAfip,
                CupoStop = x.CupoStop == null ? "" : x.CupoSap.ToString(),
                EstadoPlanta = x.EstadoPlanta,
                FechaGeneracion = x.FechaGeneracion,
                FechaRegistro = x.FechaGeneracion,
                IntermediarioFlete = x.IntermediarioFlete,
                Km = x.Km,
                MercadoATermino = x.MercadoATermino,
                MotivoRechazo = x.MotivoRechazo,
                NroEstablecimientoOrigen = x.NroEstablecimientoOrigen,
                Peso = x.Peso,
                RemitenteComercial = x.RemitenteComercial,
                Transportista = x.Transportista,
                UsuarioCreador = x.UsuarioCreador,
                ZonaCupoSap = x.ZonaCupo.CodigoSap,
                Acopio = x.Centro.Acopio,
                NegocioId = x.NegocioId
            }, x => id.Contains(x.Id));
        }

        public Resultado EliminarVarios(List<int> c, string comercial)
        {
            var resultado = new Resultado();
            foreach (var cupo in c)
            {
                var resCupos = EliminarCupo(cupo, comercial, false);
                if (resCupos.HayError)
                {
                    resultado.Errores.AddRange(resCupos.Errores);
                }
            }
            var cupos = repositorio.Listar<Cupo, CupoDto>(x => new CupoDto
            {
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap,
                Proveedor = x.Proveedor.RazonSocial,
                Material = x.Material.Descripcion,
                ProveedorId = x.ProveedorId,
                ComercialId = x.ComercialId
            }, x => x.EstadoCupoId == 4 && c.Contains(x.Id)).ToList();

            if (!resultado.HayError)
            {
                EnviarMailProveedorAnulacionCupo(cupos);
                EnviarMailComercialAnulacionCupo(comercial, cupos);
                EnviarMailCreadorAnulacionCupo(cupos, comercial);
            }
            return resultado;
        }

        public string ObtenerCodigoSap(int id)
        {
            return repositorio.Obtener<Cupo, string>(x => x.Id == id, x => x.CupoSap);
        }

        private void EnviarEmail(Cupo cupo, List<string> listaCupos)
        {
            try
            {
                var id = cupo.ProveedorId;
                var proveedorContacto = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == id && x.Cupo == true);
                if (proveedorContacto.Count == 0)
                {
                    return;
                }
                string emailComercial = "";

                if (cupo.Comercial != null)
                {
                    try
                    {
                        emailComercial = mailManager.GetEmailUserActiveDirectory(cupo.Comercial.IdActiveDirectory);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }
                }

                var oMensaje = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                };

                if (proveedorContacto.Count > 0)
                {
                    foreach (var contacto in proveedorContacto)
                    {
                        if (!string.IsNullOrEmpty(contacto.Email1))
                        {
                            oMensaje.To.Add(contacto.Email1);
                        }
                    }
                    if (!string.IsNullOrEmpty(emailComercial))
                        oMensaje.CC.Add(emailComercial);
                }
                else if (!string.IsNullOrEmpty(emailComercial))
                {
                    oMensaje.To.Add(emailComercial);
                }
                else
                {
                    logger.Debug($"El contrato {cupo.Id} no tiene ContactoComercial para el proveedor {cupo.ProveedorId} ni email comercial");
                    return;
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);

                if (cupo.Comercial.RolesAsociados.Any(a => a.Descripcion == "Reenvio Mails Cupos Corredores Rosario"))
                {
                    var comerciales = comercialManager.TraerTodoComercial().Comercial.Where(a => a.Rol.ToUpper().Contains("Reenvio Mails Cupos Corredores Rosario".ToUpper()) && a.Deshabilitado != true && a.ComercialId != cupo.Comercial.ComercialId).ToList();
                    foreach (var item in comerciales)
                    {
                        var comercialAdicional = repositorio.Obtener<Comercial>(item.ComercialId);
                        try
                        {
                            var emailAdicional = mailManager.GetEmailUserActiveDirectory(comercialAdicional.IdActiveDirectory);
                            oMensaje.CC.Add(emailAdicional);
                        }
                        catch (Exception)
                        {
                        }

                    }
                }

                oMensaje.AlternateViews.Add(CuerpoMail(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/storeCircular.PNG"), listaCupos, cupo, emailComercial,
                    System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/Circular.PNG"), System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/molinosCircular.PNG")));
                var subject = "";

                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                {
                    subject += "Mail Pruebas - ";
                }
                subject += "Cupos Molinos Agro S.A. - ";
                subject += cupo.Proveedor.RazonSocial;
                oMensaje.Subject = subject;
                oMensaje.BodyEncoding = Encoding.UTF8;

                oMensaje.Headers.Add("Content-class", "urn:content-classes:calendarmessage");

                SmtpClient oCliente = default(SmtpClient);

                int Condicion = 0;
                if (int.TryParse(ConfigurationManager.AppSettings["SmtpServerPort"], out Condicion))
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"], int.Parse(ConfigurationManager.AppSettings["SmtpServerPort"]));
                }
                else
                {
                    oCliente = new SmtpClient(ConfigurationManager.AppSettings["SmtpServer"]);
                }

                if (ConfigurationManager.AppSettings["SmtpAnonimo"] != "S")
                {
                    oCliente.UseDefaultCredentials = ConfigurationManager.AppSettings["UseDefaultCredentials"] == "S";
                    oCliente.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["CredentialUserName"],
                        ConfigurationManager.AppSettings["CredentialPassword"]);
                }

                oCliente.EnableSsl = ConfigurationManager.AppSettings["EnableSSL"] == "S";

                oCliente.Send(oMensaje);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private AlternateView CuerpoMail(String filePath, List<string> listaCupos, Cupo cupo, string emailComercial, String circular, String molinos)
        {
            LinkedResource store = new LinkedResource(filePath);
            store.ContentId = Guid.NewGuid().ToString();
            LinkedResource img = new LinkedResource(circular);
            img.ContentId = Guid.NewGuid().ToString();
            LinkedResource res = new LinkedResource(molinos);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;

            string htmlBody = "";

            htmlBody += "En el presente mail, se detalla los cupos generados con Molinos Agro S.A.: <br /><br />  ";

            if (cupo.MaterialId == 2)
            {
                htmlBody += "<b style=\"font-size: 18px;text-decoration: underline;background-color: yellow;\">Trigo libre de HB4</b>" + "<br />";

            }
            foreach (var c in listaCupos)
            {
                htmlBody += c + "<br />";
            }
            htmlBody += "<br />";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            var destino = cupo.Centro.CodigoSap == "1600" || cupo.Centro.CodigoSap == "1029" ? " SAN LORENZO - SANTA FE - BENIELLI 398" : cupo.Centro.Descripcion;
            htmlBody += "<tr>" + Td(ref linea, 2) + "Con destino a " + destino.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "FECHA DESCARGA: </th>" + Td(ref linea) + Split(cupo.FechaIngreso.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + Td(ref linea) + cupo.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINATARIO: </th>" + Td(ref linea) + (cupo.Destinatario.ToUpper() == "30715118773" ? "MOLINOS AGRO S.A.-30715118773" : cupo.Destinatario.ToUpper()) + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINO: </th>" + Td(ref linea) + "MOLINOS AGRO S.A.-30715118773" + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + Td(ref linea) + cupo.Material.Descripcion.ToUpper() + "</td></tr>";

            if (cupo.Centro.CodigoSap == "1600" && (cupo.MaterialId == 1 || cupo.MaterialId == 2 || cupo.MaterialId == 3) || cupo.Observaciones != null)
            {
                htmlBody += "<tr>" + th + "OBSERVACIÓN</th>" + Td(ref linea);
            }
            if (cupo.Observaciones != null)
            {
                htmlBody += cupo.Observaciones + "<br />";
            }
            if (cupo.Centro.CodigoSap == "1600" && (cupo.MaterialId == 1 || cupo.MaterialId == 2 || cupo.MaterialId == 3))
            {

                if (cupo.MaterialId == 1)
                {
                    htmlBody += "ESPECIAL<br />";
                }
                if (cupo.MaterialId == 2)
                {
                    htmlBody += "ESPECIAL<br />";
                }
                if (cupo.MaterialId == 3)
                {
                    htmlBody += "SUSTENTABLE<br />";
                }

            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /> Recordamos que el cupo tiene validez desde las 0 hrs hasta las 23:59 hrs del mismo día para el cual fue otorgado el cupo. Evitar el arribo previo o posterior a dicha fecha, ya que perjudican la operatoria, haciendo más lento el circuito de descarga y por ende mayores demoras para los transportes. A su vez, aquellos que no cumplan con la franja que corresponde al cupo podrán sufrir sanciones.";
            htmlBody += "<br /><br /> Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + cupo.Comercial.Nombres + " " + cupo.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br />" +
                "<br /> www.molinosagro.com.ar <br />" +
                "<table>" +
                "<tr >" +
                "<td rowspan='2'>" + @"<img src='cid:" + img.ContentId + @"'/> " + "</td> " +
                "<td>" + @"<a href='https://play.google.com/store/apps/details?id=com.appcircular'><img src='cid:" + store.ContentId + @"'/></a>" + "</td>" +
                "</tr>" +
                "<tr>" +
                "<td>" + @"<a href='www.molinosagro.com.ar'><img src='cid:" + res.ContentId + @"'/></a>" + " </td>" +
                "</tr>" +
                "</table>" +
                " ";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            alternateView.LinkedResources.Add(img);
            alternateView.LinkedResources.Add(store);
            return alternateView;
        }
        public string Split(string str)
        {
            var enumNumero = Enumerable.Range(0, str.Length / 2)
                .Select(i => str.Substring(i * 2, 2)).ToList();
            if (str.Length % 2 == 1)
            {
                enumNumero.Add(str[str.Length - 1].ToString());
            }
            var nuevoString = "";

            for (int i = 0; i < enumNumero.Count(); i++)
            {
                nuevoString += "<span>" + enumNumero[i] + "</span>";
            }
            return nuevoString;
        }
        public string Td(ref int linea, int largo = 1)
        {
            string td1 = "";
            string td2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                td1 = "<td colspan=\"" + largo + "\" style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                td2 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                td2 = "<td colspan=\"" + largo + "\" style=\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
            }
            linea += 1;
            if (linea % 2 == 0)
            {
                return td1;
            }
            else
            {
                return td2;
            }
        }
        private string TrEncabezado(Cupo c, ref int linea)
        {
            string style1 = "";
            string style2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            logger.Debug($"cupo numero: {c.Id}");
            linea += 1;
            if (linea % 2 == 0)
            {
                return "<tr>" +
                    "<td " + style1 + c.Material.Descripcion + "</td>" +
                     "<td " + style1 + Split(c.FechaIngreso.ToShortDateString()) + "</td>" +
                     "<td " + style1 + c.Proveedor.RazonSocial + "</td>" +
                     "<td " + style1 + c.CupoSap + "</td>" +
                     "<td " + style1 + "Sin CTG" + "</td></ tr>";
            }
            else
            {
                return "<tr>" +
                        "<td " + style2 + c.Material.Descripcion + "</td>" +
                        "<td " + style2 + Split(c.FechaIngreso.ToShortDateString()) + "</td>" +
                        "<td " + style2 + c.Proveedor.RazonSocial + "</td>" +
                        "<td " + style2 + c.CupoSap + "</td>" +
                        "<td " + style2 + "Sin CTG" + "</td></ tr>";
            }

        }
        public void CrearSugerenciaCupo()
        {
            var materiales = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
            var sugerencias = new List<SugerenciaCupoDto>();
            var formulas = new List<FormulaDto>();
            foreach (var material in materiales)
            {
                var dto = ObtenerFormulaDto(material.MaterialId);
                sugerencias.AddRange(CrearSugerenciaCupo(material.MaterialId, dto, null));
                formulas.Add(dto);
            }
            logger.Debug("Enviando Mail EnviarMailNegociosDeAlgoritmo");
            EnviarMailNegociosDeAlgoritmo(GenerarExcelNegociosAlgoritmo(ConvertirADtoExcel(sugerencias), formulas));

            //EnviarMail
        }

        public FormulaDto ObtenerFormulaDto(int material)
        {
            return FormulaToDto(repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(material)));
        }



        public List<SugerenciaCupoExcel> ConvertirADtoExcel(List<SugerenciaCupoDto> dto)
        {
            var listaExcel = new List<SugerenciaCupoExcel>();
            foreach (var item in dto)
            {
                var excel = new SugerenciaCupoExcel
                {
                    TipoNegocio = item.TipoNegocioDesc,
                    ContratoSAP = item.ContratoSAP,
                    RazonSocial = item.ProveedorDesc,
                    CUIT = item.ProveedorCUIT,
                    Destinatario = item.Destinatario,
                    Precio = item.Precio,
                    KgNegocio = item.KgNegocio,
                    Material = item.MaterialDesc,
                    Moneda = String.IsNullOrEmpty(item.MonedaId) ? "" : item.MonedaId,
                    FechaDesde = item.FechaDesde,
                    FechaHasta = item.FechaHasta,
                    Comercial = item.ComercialDesc,
                    Zona = item.ZonaDescrip,
                    Centro = item.CentroDesc,
                    CDWarrant = item.CDWarrant == true ? "SI" : "NO",
                    Fason = item.Fason == true ? "SI" : "NO",
                    Priorizado = string.IsNullOrEmpty(item.Inhabilitado) ? item.Priorizado == true ? "SI" : "NO" : item.Inhabilitado,
                    Puntaje = item.PuntuacionTotal <= 0 ? 0 : item.PuntuacionTotal,
                    CantidadSugerida = item.CantidadDeCupos,
                    FechaSugerida = item.FechaSugerida
                };
                listaExcel.Add(excel);
            }
            return listaExcel.OrderByDescending(x => x.Puntaje).ToList();
        }      
        public List<SugerenciaCupoDto> CrearSugerenciaCupo(int MaterialId, FormulaDto formula, ConfiguracionCupo configuracion = null)
        {
            try
            {
                DateTime hoy = DateTime.Now.Date;
                var formulaDto = formula;
                logger.Debug("CrearSugerenciaCupo - se obtuvo la formula para material: " + MaterialId);
                var formulaSave = repositorio.Obtener<Formula>(formulaDto.Id);



                if (configuracion != null)
                {
                    logger.Debug("CrearSugerenciaCupo - configuracion: " + (configuracion == null ? "null" : configuracion.ToJson()));
                    logger.Debug("desde" + formulaDto.CuposDesde + " hasta " + formulaDto.CuposHasta + "- Configuracion: " + configuracion.Fecha);
                    if (!(formulaDto.CuposDesde <= configuracion.Fecha && formulaDto.CuposHasta >= configuracion.Fecha))
                    {
                        return new List<SugerenciaCupoDto>();
                    }
                }
                logger.Debug("Inicio Algoritmo");

                formulaSave.CuposDesde = formulaDto.CuposDesde;
                formulaSave.CuposHasta = formulaDto.CuposHasta;
                formulaSave.NegociosDesde = formulaDto.NegociosDesde;
                formulaSave.NegociosHasta = formulaDto.NegociosHasta;
                formulaSave.CriterioId = formulaDto.CriterioId;
                formulaSave.CentroId = formulaDto.CentroId;
                formulaSave.MaterialId = formulaDto.MaterialId;
                formulaSave.Fecha = formulaDto.Fecha;
                formulaSave.Usada = true;

                formulaDto.CuposDesde = formulaDto.CuposDesde < hoy ? hoy : formulaDto.CuposDesde;

                logger.Debug("CrearSugerenciaCupo - inicio de disponibilidad en planta.");
                List<ConfiguracionCupoDto> disponibilidadEnPlantas = ObtenerDisponibilidadEnPlantas(formulaDto);
                logger.Debug("CrearSugerenciaCupo - fin de disponibilidad en planta.");

                List<SugerenciaCupoDto> negocios = new List<SugerenciaCupoDto>();

                logger.Debug("CrearSugerenciaCupo - inicio de obtener negocios.");
                ObtenerNegocios(hoy, formulaDto, negocios);
                logger.Debug("CrearSugerenciaCupo - fin de obtener negocios.");

                logger.Debug("CrearSugerenciaCupo - inicio de ValidarHabilitaciones.");
                var inhabilitados = ValidarHabilitaciones(negocios);
                logger.Debug("CrearSugerenciaCupo - fin de ValidarHabilitaciones.");

                logger.Debug("CrearSugerenciaCupo - inicio de ObtenerPuntajes.");
                ObtenerPuntajes(formulaDto, negocios);
                logger.Debug("CrearSugerenciaCupo - fin de ObtenerPuntajes.");

                logger.Debug("CrearSugerenciaCupo - inicio de PriorizarSegunDisponibilidad.");
                PriorizarSegunDisponibilidad(disponibilidadEnPlantas, negocios);
                logger.Debug("CrearSugerenciaCupo - negocios priorizados: " + negocios.Where(a => a.Priorizado).Count());
                logger.Debug("CrearSugerenciaCupo - fin de PriorizarSegunDisponibilidad.");

                List<SugerenciaCupo> sugerencias = negocios.Where(a => a.Priorizado).Select(a => new SugerenciaCupo
                {
                    //AgenteCompraId = a.AgenteCompraId,
                    ZonaCupoId = a.ZonaCupoId ?? 0,
                    CantidadDeCupos = a.CantidadDeCupos,
                    CantidadCupoOriginal = a.CantidadDeCupos,
                    CentroId = a.DestinoId,
                    NegocioId = a.NegocioId,
                    //ContratoId = a.ContratoId,
                    //FasonId = a.FasonId,
                    FechaSugerida = a.FechaSugerida,
                    //FijacionDePrecioContratoId = a.FijacionDePrecioContratoId,
                    ConfiguracionEspacioDinamicoId = a.ConfiguracionEspacioDinamicoId,
                    MaterialId = a.MaterialId,
                    MonedaId = a.MonedaId,
                    Precio = a.Precio,
                    TipoNegocioId = a.TipoNegocioId,
                    Puntuacion = a.PuntuacionTotal,
                    ProveedorId = a.ProveedorId,
                    Destinatario = a.Destinatario,
                    ComercialId = a.ComercialId,
                    StandardDeCalidad = a.StandardDeCalidad,
                    Aceptado = null,
                    Puntuaciones = JsonConvert.SerializeObject(a.Puntuaciones),
                    ContratoSAP = a.ContratoSAP,
                    CDWarrant = a.CDWarrant == true ? true : false,
                    KgNegocio = a.KgNegocio,
                    KgPendienteAplicar = a.KgPendienteAplicar,

                }).ToList();



                var solicitudesSugerenciasId = repositorio.Listar<AdministracionCupo, int>(a => a.SugerenciaCupoId.Value, x => x.SugerenciaCupoId != null);

                var solicitudesRechazadas = repositorio.Listar<AdministracionCupo, int>(a => a.SugerenciaCupoId.Value, x => x.SugerenciaCupoId != null && x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoRechazadoAdministracionCupo);

                var sugerenciasPendientes = repositorio.Listar<SugerenciaCupo>(a => a.Aceptado != false && a.MaterialId == formulaDto.MaterialId && solicitudesRechazadas.Contains(a.Id));
                sugerenciasPendientes.ForEach(a => a.Aceptado = false);

                //repositorio.RemoverTodos<SugerenciaCupo>(a => a.Aceptado != false && a.MaterialId == formulaDto.MaterialId && !solicitudesSugerenciasId.Contains(a.Id));
                repositorio.RemoverTodos<SugerenciaCupo>(a => a.Aceptado == null && a.MaterialId == formulaDto.MaterialId && !solicitudesSugerenciasId.Contains(a.Id));
                repositorio.AgregarTodos(sugerencias);
                CargarDatosSugerenciasPorComercial(sugerencias, formulaDto.MaterialId);
                repositorio.GuardarCambios();

                logger.Debug("CrearSugerenciaCupo - GuardarCambios.");
                negocios.AddRange(inhabilitados);
                return negocios;
            }
            catch (Exception e)
            {
                logger.Error(e);
                throw;
            }
        }
        private List<SugerenciaCupoDto> ValidarHabilitaciones(List<SugerenciaCupoDto> negocios)
        {
            var negocioInhabilitados = new List<SugerenciaCupoDto>();
            for (int i = negocios.Count - 1; i >= 0; i--)
            {
                var negocio = negocios[i];
                var result = Validar(new Cupo { CentroId = negocio.CentroId, NegocioId = negocio.NegocioId, ProveedorId = negocio.ProveedorId.Value, FechaIngreso = negocio.FechaHasta }, 1, DateTime.Now.Date.AddDays(1));
                if (result.HayError)
                {
                    // proxima etapa guardar detalle del error para mostrar al comecial por que se 
                    negocios[i].Inhabilitado = "Proveedor Inhabilitado";
                    negocioInhabilitados.Add(negocios[i]);
                    negocios.RemoveAt(i);
                }
            }

            return negocioInhabilitados;
        }

        private List<ConfiguracionCupoDto> ObtenerDisponibilidadEnPlantas(FormulaDto formula)
        {
            //cupos no rechazados en rango de fecha creados por el algoritmo
            List<Cupo> cupos = repositorio.Listar<Cupo>(x =>
                x.FechaIngreso >= formula.CuposDesde && x.FechaIngreso <= formula.CuposHasta &&
                x.CentroId == formula.CentroId && x.MaterialId == formula.MaterialId &&
                x.EstadoCupoId != 4 && x.EstadoCupoId != 9 &&
                (x.NegocioId != null || x.ConfiguracionEspacioDinamicoId != null)
            );
            logger.Debug("CrearSugerenciaCupo - se obtuvieron " + cupos.Count + " cupos.");

            //solicitudes pendientes
            List<AdministracionCupo> solicitudesPendientes = repositorio.Listar<AdministracionCupo>(x =>
                x.Fecha >= formula.CuposDesde && x.Fecha <= formula.CuposHasta &&
                x.CentroId == formula.CentroId && x.MaterialId == formula.MaterialId &&
                x.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Algoritmo &&
                x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo
                && x.SugerenciaCupoId != null
            );
            logger.Debug("CrearSugerenciaCupo - se obtuvieron " + solicitudesPendientes.Count + " solicitudes pendientes.");

            List<ConfiguracionCupo> configuracionCupo = repositorio.Listar<ConfiguracionCupo>(x => (x.Fecha >= formula.CuposDesde && x.Fecha <= formula.CuposHasta) && x.CentroId == formula.CentroId && x.MaterialId == formula.MaterialId);

            List<ConfiguracionCupoDto> disponibilidadEnPlantas = configuracionCupo.Select(x => new ConfiguracionCupoDto { CentroId = x.CentroId, LimiteCupo = x.LimiteCupo, LimiteAlgoritmo = x.LimiteAlgoritmo, MaterialId = x.MaterialId, Fecha = x.Fecha }).ToList();
            foreach (var cupo in cupos)
            {
                var configuracion = disponibilidadEnPlantas.Where(a => a.CentroId == cupo.CentroId && a.MaterialId == cupo.MaterialId && a.Fecha == cupo.FechaIngreso).SingleOrDefault();
                if (configuracion != null)
                {
                    configuracion.LimiteAlgoritmo -= 1;
                }
            }
            foreach (var sugerencia in solicitudesPendientes)
            {
                var configuracion = disponibilidadEnPlantas.Where(a => a.CentroId == sugerencia.CentroId && a.MaterialId == sugerencia.MaterialId && a.Fecha == sugerencia.Fecha).SingleOrDefault();
                if (configuracion != null)
                {
                    configuracion.LimiteAlgoritmo -= sugerencia.CantidadFleteProcedencia + sugerencia.CantidadCupo;
                }
            }
            foreach (var item in disponibilidadEnPlantas)
            {
                logger.Debug("CrearSugerenciaCupo - DisponibilidadEnPlanta:" + item.Fecha.ToString("dd/MM/yyyy") + ",cantidad:" + item.LimiteAlgoritmo + "materialid:" + item.MaterialId);
            }
            return disponibilidadEnPlantas;
        }

        private void ObtenerPuntajes(FormulaDto formula, List<SugerenciaCupoDto> negocios)
        {
            foreach (var item in negocios.ToList())
            {
                formula.Criterio.Dto = item;
                formula.Criterio.Dto.formula = formula;
                item.PuntuacionTotal = Math.Round(servicioCriterios.Calcular(formula.Criterio), 2);

                ArmarPuntuaciones(formula.Criterio, item.Puntuaciones, 0);

                item.formula = null;
            }
            negocios = negocios.OrderByDescending(a => a.PuntuacionTotal).ToList();
        }

        private void PriorizarSegunDisponibilidad(List<ConfiguracionCupoDto> disponibilidadEnPlantas, List<SugerenciaCupoDto> negocios)
        {
            var configuracion = repositorio.Obtener<Configuracion>(1);
            var idsProveedores = negocios.Select(a => a.ProveedorId ?? 0).Distinct();
            decimal puntajeMinimo = negocios == null || negocios.Count() == 0 ? 0 : negocios.Min(a => a.PuntuacionTotal);
            int porcentajeMaximo = configuracion.AlgoritmoProcMaxSugerenciasProveedorDia;
            List<TopeSugerenciasPorDiaPorProveedor> limitePorProveedor = new List<TopeSugerenciasPorDiaPorProveedor>();
            foreach (var disponibilidad in disponibilidadEnPlantas)
            {
                var topeCupos = disponibilidad.LimiteAlgoritmo * porcentajeMaximo / 100;
                foreach (var idProveedor in idsProveedores)
                {
                    limitePorProveedor.Add(new TopeSugerenciasPorDiaPorProveedor
                    {
                        Fecha = disponibilidad.Fecha,
                        Disponible = topeCupos,
                        ProveedorId = idProveedor
                    });
                }
            }
            List<SugerenciaCupoDto> newNegocios = new List<SugerenciaCupoDto>();

            //primera ronda de sugerencias TENIENDO en cuenta el limite de % por dia por prov
            foreach (var negocio in negocios.OrderByDescending(a => a.PuntuacionTotal))
            {
                var disponibles = disponibilidadEnPlantas.Where(a => a.MaterialId == negocio.MaterialId && a.LimiteAlgoritmo > 0 /*&& a.Fecha >= negocio.FechaDesde && a.Fecha <= negocio.FechaHasta*/).OrderBy(a => a.Fecha).ToList();

                foreach (var disponible in disponibles)
                {
                    var limiteProveedor = limitePorProveedor.Where(a => a.ProveedorId == negocio.ProveedorId && a.Fecha == disponible.Fecha).Single();
                    //pasa al proximo dia si ya le asigno todo lo que podia a ese dia por proveedor
                    if (limiteProveedor.Disponible <= 0) continue;

                    //si hay disponible para ese dia y el negocio no esta priorizado
                    if (disponible.LimiteAlgoritmo > 0 && !negocio.Priorizado)
                    {
                        //asignacion total del negocio ( limite del algoritmo mayor a cantCupos y cantCupos menor al limite por dia
                        if (disponible.LimiteAlgoritmo >= negocio.CantidadDeCupos && negocio.CantidadDeCupos <= limiteProveedor.Disponible)
                        {
                            disponible.LimiteAlgoritmo -= negocio.CantidadDeCupos;
                            limiteProveedor.Disponible -= negocio.CantidadDeCupos;
                            negocio.Priorizado = true;
                            negocio.FechaSugerida = disponible.Fecha.Date;
                        }
                        else //asignacion parcial del negocio
                        {
                            if (disponible.LimiteAlgoritmo > 0)
                            {
                                //creo por el menor de los dos
                                int cantidadAcrear = disponible.LimiteAlgoritmo <= limiteProveedor.Disponible ? disponible.LimiteAlgoritmo : limiteProveedor.Disponible;

                                var newNegocio = (SugerenciaCupoDto)negocio.Clone();
                                newNegocio.CantidadDeCupos = cantidadAcrear;
                                limiteProveedor.Disponible -= cantidadAcrear;
                                newNegocio.Priorizado = true;
                                newNegocio.FechaSugerida = disponible.Fecha.Date;
                                newNegocios.Add(newNegocio);

                                negocio.CantidadDeCupos -= cantidadAcrear;
                                disponible.LimiteAlgoritmo -= cantidadAcrear;
                                negocio.Priorizado = false;
                            }

                        }

                    }
                }
            }

            //segunda ronda de sugerencias de lo pendiente SIN tener en cuenta el limite de % por dia por prov
            foreach (var negocio in negocios.OrderByDescending(a => a.PuntuacionTotal))
            {
                var disponibles = disponibilidadEnPlantas.Where(a => a.MaterialId == negocio.MaterialId && a.LimiteAlgoritmo > 0 /*&& a.Fecha >= negocio.FechaDesde && a.Fecha <= negocio.FechaHasta*/).OrderBy(a => a.Fecha).ToList();

                foreach (var disponible in disponibles)
                {
                    if (disponible.LimiteAlgoritmo > 0 && !negocio.Priorizado)
                    {
                        if (disponible.LimiteAlgoritmo >= negocio.CantidadDeCupos)
                        {
                            disponible.LimiteAlgoritmo -= negocio.CantidadDeCupos;
                            negocio.Priorizado = true;
                            negocio.FechaSugerida = disponible.Fecha.Date;
                        }
                        else
                        {
                            if (disponible.LimiteAlgoritmo > 0)
                            {
                                var newNegocio = (SugerenciaCupoDto)negocio.Clone();
                                newNegocio.CantidadDeCupos = disponible.LimiteAlgoritmo;
                                newNegocio.Priorizado = true;
                                newNegocio.FechaSugerida = disponible.Fecha.Date;
                                newNegocios.Add(newNegocio);

                                negocio.CantidadDeCupos -= disponible.LimiteAlgoritmo;
                                disponible.LimiteAlgoritmo = 0;
                                negocio.Priorizado = false;
                            }
                        }
                    }
                }
            }
            if (newNegocios.Count > 0)
            {
                negocios.AddRange(newNegocios);
            }

            var sugerenciasAgrupadas = new List<SugerenciaCupoDto>();
            foreach (var item in negocios.GroupBy(x => new { x.ProveedorId, x.Priorizado, x.NegocioId, x.FechaSugerida, x.ConfiguracionEspacioDinamicoId }))
            {
                var sugerencia = item.First();
                sugerencia.CantidadDeCupos = item.Sum(a => a.CantidadDeCupos);
                sugerencia.CantidadCupoOriginal = item.Sum(a => a.CantidadCupoOriginal);
                sugerenciasAgrupadas.Add(sugerencia);
            }
            negocios.RemoveAll(a => true);
            negocios.AddRange(sugerenciasAgrupadas);
        }

        private void ObtenerNegocios(DateTime hoy, FormulaDto formula, List<SugerenciaCupoDto> negocios)
        {

            //negocios disponibles

            var pizarraLista = repositorio.Listar<PrecioPizarra>(x => formula.NegociosDesde <= x.FechaHasta && formula.NegociosHasta >= x.FechaHasta);


            //if (formula.CentroId == 1)//centro 1 = san lorenso
            //{
            //    Dictionary<int, int> agenteCompraUsados = cupos.Where(x => x.AgenteCompraId != null).GroupBy(x => x.AgenteCompraId.Value).ToDictionary(a => a.Key, a => a.Count());
            //    var agentes = repositorio.Listar<AgenteCompra, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId=x.ComercialId, Destinatario = "", ProveedorId = null, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = 1, AgenteCompraId = x.Id, MaterialId = x.MaterialId, TipoNegocioId = 5, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.Fecha, FechaHasta = x.Fecha, FechaDesdeString = x.Fecha.ToString(), FechaHastaString = x.Fecha.ToString() }, x => x.Fecha >= formula.NegociosDesde && x.Fecha <= formula.NegociosHasta && x.EstadoId == 5);
            //    foreach (var item in agentes)
            //    {
            //        if (agenteCompraUsados.Any(a => a.Key == item.AgenteCompraId))
            //        {
            //            item.CantidadCupos -= agenteCompraUsados.Where(a => a.Key == item.AgenteCompraId).Single().Value;
            //        }
            //    }
            //    agentes = agentes.Where(a => a.CantidadCupos > 0).ToList();
            //    negocios.AddRange(agentes);

            //    Dictionary<int, int> fasonUsados = cupos.Where(x => x.FasonId != null).GroupBy(x => x.FasonId.Value).ToDictionary(a => a.Key, a => a.Count());
            //    var fasones = repositorio.Listar<Fason, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId = x.ComercialId, Destinatario = x.Fasonero.CUIT, ProveedorId = x.FasoneroId, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = 1, FasonId = x.Id, MaterialId = x.MaterialId, TipoNegocioId = 4, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, FechaDesdeString = x.FechaDesde.ToString(), FechaHastaString = x.FechaHasta.ToString() }, x => ((x.FechaDesde >= formula.NegociosDesde && x.FechaDesde <= formula.NegociosHasta) || (x.FechaHasta >= formula.NegociosDesde && x.FechaHasta <= formula.NegociosHasta)) && x.EstadoId == 5);
            //    foreach (var item in fasones)
            //    {
            //        if (fasonUsados.Any(a => a.Key == item.FasonId))
            //        {
            //            item.CantidadCupos -= fasonUsados.Where(a => a.Key == item.FasonId).Single().Value;
            //        }
            //    }
            //    fasones = fasones.Where(a => a.CantidadCupos > 0).ToList();
            //    negocios.AddRange(fasones);
            //    List<ZonaCupo> zonaCupos = repositorio.Listar<ZonaCupo>();
            //    foreach (var item in negocios)
            //    {
            //        var zona = zonaCupos.Where(a => a.Descripcion == item.ZonaDescrip).SingleOrDefault();
            //        if (zona != null)
            //        {
            //            item.ZonaCupoId = zona.Id;
            //        }
            //    }
            //}

            //Dictionary<int, int> fijacionDePrecioContratoUsados = cupos.Where(x => x.FijacionDePrecioContratoId != null).GroupBy(x => x.FijacionDePrecioContratoId.Value).ToDictionary(a => a.Key, a => a.Count());
            //var fijaciones = repositorio.Listar<FijacionDePrecioContrato, SugerenciaCupoDto>(x => new SugerenciaCupoDto { ComercialId = x.ComercialId, Destinatario = x.Proveedor.CUIT, ProveedorId = x.ProveedorId, ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion, CantidadCupos = (int)Math.Ceiling(x.Cantidad / 30000), DestinoId = x.DestinoId.Value, FijacionDePrecioContratoId = x.FijacionDePrecioContratoId, MaterialId = x.MaterialId.Value, TipoNegocioId = 3, MonedaId = x.MonedaId, Precio = x.Precio, FechaDesde = x.FechaDesde, FechaHasta = x.FechaHasta, FechaDesdeString = x.FechaDesde.ToString(), FechaHastaString = x.FechaHasta.ToString() }, x => x.MaterialId != null && ((x.FechaDesde >= formula.NegociosDesde && x.FechaDesde <= formula.NegociosHasta) || (x.FechaHasta >= formula.NegociosDesde && x.FechaHasta <= formula.NegociosHasta)) && x.EstadoId == 5 && x.DestinoId == formula.CentroId);
            //foreach (var item in fijaciones)
            //{
            //    if (fijacionDePrecioContratoUsados.Any(a => a.Key == item.FijacionDePrecioContratoId))
            //    {
            //        item.CantidadCupos -= fijacionDePrecioContratoUsados.Where(a => a.Key == item.FijacionDePrecioContratoId).Single().Value;
            //    }
            //}
            //fijaciones = fijaciones.Where(a => a.CantidadCupos > 0).ToList();
            //negocios.AddRange(fijaciones);
            //logger.Debug("CrearSugerenciaCupo - fijaciones obtenidos: " + fijaciones.Count());


            //foreach (var fijacion in negocios.Where(a => a.TipoNegocioId == 3 && a.Precio == 0).ToList())
            //{
            //    var pizarra = pizarraLista.Where(a => a.MaterialId == fijacion.MaterialId).SingleOrDefault();
            //    if (pizarra == null)
            //    {
            //        pizarra = repositorio.Listar<PrecioPizarra>(a => a.MaterialId == fijacion.MaterialId ).OrderByDescending(a => a.FechaDesde).Take(1).Single();
            //        pizarraLista.Add(pizarra);
            //    }
            //    fijacion.MonedaId = pizarra.MonedaId;
            //    fijacion.Precio = pizarra.Precio;

            //}
            //logger.Debug("CrearSugerenciaCupo - pongo precio pizarra a FijacionDePrecioContrato que no tienen precio");

            var contratos = repositorio.Listar<Contrato, SugerenciaCupoDto>(x =>
                new SugerenciaCupoDto
                {
                    StandardDeCalidad = x.StandardDeCalidadId.HasValue ? x.StandardDeCalidad.Descripcion : "",
                    ComercialId = x.ComercialId.Value,
                    Destinatario = "30715118773",
                    ProveedorCUIT = x.CorredorId > 0 && x.Corredor != null ? x.Corredor.CUIT : x.Proveedor.CUIT,
                    ProveedorId = x.CorredorId > 0 && x.Corredor != null ? x.CorredorId : x.ProveedorId,
                    ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion,
                    CantidadDeCupos = (int)Math.Ceiling(x.Cantidad / 30000),
                    DestinoId = x.DestinoId.Value,
                    NegocioId = x.Id,
                    MaterialId = x.MaterialId,
                    MonedaId = x.MonedaId,
                    Precio = x.Precio,
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    TipoNegocioId = x.TipoNegocioId,
                    ContratoSAP = x.ContratoSAP,
                    KgNegocio = x.Cantidad,
                    KgPendienteAplicar = 0,
                    MaterialDesc = x.Material.Descripcion,
                    ProveedorDesc = x.CorredorId > 0 && x.Corredor != null ? x.Corredor.RazonSocial : x.Proveedor.RazonSocial,
                    ComercialDesc = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    TipoNegocioDesc = x.TipoNegocio.Descripcion,
                    CDWarrant = x.Warrant == true ? true : false,
                    Fason = x.EsFason,
                    CentroDesc = x.Destino.Descripcion,
                },
                    x =>
                    //(formula.NegociosDesde >= x.FechaDesde && formula.NegociosHasta < x.FechaHasta) || (formula.NegociosHasta <= x.FechaHasta &&
                    //formula.NegociosHasta > x.FechaDesde) || (formula.NegociosDesde <= x.FechaDesde && formula.NegociosHasta >= x.FechaHasta))
                    x.Sustentable != true &&
                    formula.NegociosDesde <= x.FechaHasta && formula.NegociosHasta >= x.FechaHasta
                    && x.EstadoId == 5 && x.DestinoId == formula.CentroId && x.MercsDeposito != true && x.MaterialId == formula.MaterialId);
            logger.Debug("CrearSugerenciaCupo - Contratos todos: " + contratos.Count());
            logger.Debug("CrearSugerenciaCupo - Contratos todos: " + contratos.Select(a => a.ContratoSAP).ToList().ToJson());


            var zonas = repositorio.Listar<ZonaCupo>();

            var kilosMinimosParaSugerencia = repositorio.Obtener<Configuracion>(1).AlgoritmoKilosMinimosParaSugerencia * 1000;
            var solicitudesSugerenciasId = repositorio.Listar<AdministracionCupo, int>(a => a.SugerenciaCupoId.Value, a => a.SugerenciaCupoId != null);
            contratos = contratos.Where(x => !solicitudesSugerenciasId.Contains(x.NegocioId.Value)).ToList();
            var negociosId = contratos.Where(x => x.ContratoSAP != null && x.ContratoSAP != "").Select(a => a.NegocioId).ToList();
            var antesDeAyer = DateTime.Now.Date.AddDays(-2);
            var ayer = DateTime.Now.Date.AddDays(-1);

            //var cuposNoCumplidos = repositorio.Listar<Cupo, CupoDto>(
            //    x => new CupoDto { Id = x.Id, Cumplimiento = x.Cumplimiento, FechaIngreso = x.FechaIngreso, NegocioId = x.NegocioId },
            //    x => x.Cumplimiento == false && x.NegocioId != null && negociosId.Contains(x.NegocioId ?? 0) && x.FechaIngreso <= antesDeAyer);

            var cuposPendientes = repositorio.Listar<Cupo, CupoDto>(
                x => new CupoDto { Id = x.Id, Cumplimiento = x.Cumplimiento, FechaIngreso = x.FechaIngreso, NegocioId = x.NegocioId },
                x => x.Cumplimiento != true && x.NegocioId != null && negociosId.Contains(x.NegocioId ?? 0) && x.EstadoCupoId != 4 && x.EstadoCupoId != 9 && x.FechaIngreso >= ayer)
                .GroupBy(x => x.NegocioId.Value).ToDictionary(a => a.Key, a => a.Count());


            //Dictionary<int, int> cuposCumplidos = repositorio.Listar<Cupo>(x =>
            //      x.NegocioId != null && negociosId.Contains(x.NegocioId ?? 0) &&
            //      x.EstadoCupoId != 4 && x.EstadoCupoId != 9
            //      && x.Cumplimiento != false
            //      && x.NegocioId != null && x.ComercialId != null).GroupBy(x => x.NegocioId.Value).ToDictionary(a => a.Key, a => a.Count());

            //solicitudes pendientes
            List<AdministracionCupo> solicitudesPendientes = repositorio.Listar<AdministracionCupo>(x =>
                x.Fecha >= formula.CuposDesde && x.Fecha <= formula.CuposHasta &&
                x.CentroId == formula.CentroId && x.MaterialId == formula.MaterialId &&
                x.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Algoritmo &&
                x.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo
                && x.SugerenciaCupoId != null
            );

            List<ContratoKgPendiente> contratosKgPendiente = contratos.Select(a => new ContratoKgPendiente { ContratoId = a.Id, ContratoSAP = a.ContratoSAP }).ToList();
            contratosKgPendiente = contratoKgPendienteAgent.Consultar(contratosKgPendiente);
            logger.Debug("CrearSugerenciaCupo - Contratos KgPendiente: " + contratosKgPendiente.ToJson());
            var minimo = Convert.ToSingle(kilosMinimosParaSugerencia * 100) / 30000;
            foreach (var item in contratos)
            {
                item.ZonaCupoId = zonas.Where(a => a.Descripcion == item.ZonaDescrip).Select(a => a.Id).SingleOrDefault();

                var KgPendiente = contratosKgPendiente.Where(a => a.ContratoSAP == item.ContratoSAP).FirstOrDefault().KgPendiente;

                if (KgPendiente >= kilosMinimosParaSugerencia)
                {
                    item.KgPendienteAplicar = KgPendiente;
                    var cantidadcupos = Convert.ToSingle(KgPendiente) / 30000;
                    item.CantidadDeCupos = KgPendiente / 30000;
                    var excedente = (cantidadcupos - Math.Truncate(cantidadcupos)) * 100;
                    if (Convert.ToInt32(excedente) >= Convert.ToInt32(minimo))
                    {
                        item.CantidadDeCupos += 1;
                    }
                }
                else
                {
                    item.CantidadDeCupos = 0;
                }
                if (cuposPendientes.Any(a => a.Key == item.NegocioId))
                {
                    item.CantidadDeCupos -= cuposPendientes.Where(a => a.Key == item.NegocioId).Single().Value;
                }

                //analizar si tiene sentido por que ahora la cantiad depende de los kg pendientes y no de la cantidad de cupos por negocios
                //if (cuposNoCumplidos.Any(x => x.NegocioId == item.NegocioId))
                //{
                //    item.CantidadDeCupos += cuposNoCumplidos.Count(x => x.NegocioId == item.NegocioId);
                //}
                if (solicitudesPendientes.Any(a => a.SugerenciaCupo != null && a.SugerenciaCupo.NegocioId == item.NegocioId))
                {
                    item.CantidadDeCupos -= solicitudesPendientes
                        .Where(a => a.SugerenciaCupo != null && a.SugerenciaCupo.NegocioId == item.NegocioId)
                        .Sum(a => a.CantidadCupo + a.CantidadFleteProcedencia);
                }
            }
            contratos = contratos.Where(a => a.CantidadDeCupos > 0).ToList();
            negocios.AddRange(contratos);
            logger.Debug("CrearSugerenciaCupo - Contratos obtenidos: " + contratos.Count());
            logger.Debug("CrearSugerenciaCupo - Contratos obtenidos: " + contratos.Select(a => a.ContratoSAP).ToList().ToJson());

            var warrant = cdWarrant.ConsultarContratoWarrant(formula.NegociosDesde, formula.NegociosHasta);
            foreach (var c in contratos)
            {
                var item = warrant.Where(x => x.ContratoSAP == c.ContratoSAP).FirstOrDefault();
                if (item != null)
                {
                    c.CDWarrant = true;
                }
            }


            TipoNegocio tipoNegocioEspacioDinamico = repositorio.ObtenerPrimero<TipoNegocio>(a => a.Descripcion == "ESPACIO DINAMICO");
            var espacioDinamicoLista = repositorio.Listar<ConfiguracionEspacioDinamico, SugerenciaCupoDto>(x =>
            new SugerenciaCupoDto
            {
                StandardDeCalidad = x.Calidad,
                ComercialId = x.ComercialId,
                Destinatario = "30715118773",
                ProveedorCUIT = x.Proveedor.CUIT,
                ProveedorId = x.ProveedorId,
                ZonaDescrip = x.Comercial.GrupoDeCompras.Descripcion,
                ZonaCupoId = null,
                CantidadDeCupos = x.CantidadDeCupo,
                DestinoId = x.CentroId,
                ConfiguracionEspacioDinamicoId = x.Id,
                MaterialId = x.MaterialId,
                MonedaId = "",
                Precio = null,
                FechaDesde = x.Fecha,
                FechaHasta = x.Fecha,
                TipoNegocioId = tipoNegocioEspacioDinamico.TipoNegocioId,
                ContratoSAP = null,
                KgPendienteAplicar = x.CantidadDeCupo * 30000,
                KgNegocio = x.CantidadDeCupo * 30000,
                MaterialDesc = x.Material.Descripcion,
                ProveedorDesc = x.Proveedor.RazonSocial,
                ComercialDesc = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                TipoNegocioDesc = "Espacio Dinamico"
            }, x => formula.CuposDesde <= x.Fecha && formula.CuposHasta >= x.Fecha && x.CentroId == formula.CentroId && x.MaterialId == formula.MaterialId);

            var espacioDinamicoIds = espacioDinamicoLista.Select(a => a.Id).ToList();

            Dictionary<int, int> espacioDinamicoUsados = repositorio.Listar<Cupo>(x =>
                 x.ConfiguracionEspacioDinamicoId != null && espacioDinamicoIds.Contains(x.NegocioId ?? 0) &&
                 x.EstadoCupoId != 1 && x.EstadoCupoId != 4 && x.EstadoCupoId != 9 && x.Cumplimiento != false
                 && x.ComercialId != null).GroupBy(x => x.ConfiguracionEspacioDinamicoId.Value).ToDictionary(a => a.Key, a => a.Count());

            //cuposNoCumplidos = repositorio.Listar<Cupo, CupoDto>(
            //    x => new CupoDto { Id = x.Id, Cumplimiento = x.Cumplimiento, FechaIngreso = x.FechaIngreso, NegocioId = x.ConfiguracionEspacioDinamicoId, },
            //    x => x.Cumplimiento == false && x.ConfiguracionEspacioDinamicoId != null && espacioDinamicoIds.Contains(x.ConfiguracionEspacioDinamicoId ?? 0) && x.FechaIngreso <= antesDeAyer);

            foreach (var item in espacioDinamicoLista)
            {
                item.ZonaCupoId = zonas.Where(a => a.Descripcion == item.ZonaDescrip).Select(a => a.Id).SingleOrDefault();
                if (espacioDinamicoUsados.Any(a => a.Key == item.ConfiguracionEspacioDinamicoId))
                {
                    item.CantidadDeCupos -= espacioDinamicoUsados.Where(a => a.Key == item.ConfiguracionEspacioDinamicoId).Single().Value;
                }

                //if (cuposNoCumplidos.Any(x => x.NegocioId == item.ConfiguracionEspacioDinamicoId))
                //{
                //    item.CantidadDeCupos += cuposNoCumplidos.Count(x => x.NegocioId == item.ConfiguracionEspacioDinamicoId);
                //}

                if (solicitudesPendientes.Any(a => a.SugerenciaCupo != null && a.SugerenciaCupo.ConfiguracionEspacioDinamicoId == item.ConfiguracionEspacioDinamicoId))
                {
                    item.CantidadDeCupos -= solicitudesPendientes
                        .Where(a => a.SugerenciaCupo != null && a.SugerenciaCupo.ConfiguracionEspacioDinamicoId == item.ConfiguracionEspacioDinamicoId)
                        .Sum(a => a.CantidadCupo + a.CantidadFleteProcedencia);
                }
            }
            espacioDinamicoLista = espacioDinamicoLista.Where(a => a.CantidadDeCupos > 0).ToList();
            negocios.AddRange(espacioDinamicoLista);

            logger.Debug("CrearSugerenciaCupo - Espacio Dinamico obtenidos: " + espacioDinamicoLista.Count());
        }


        private void ArmarPuntuaciones(Criterio criterio, Dictionary<string, decimal> puntuaciones, int guiones)
        {
            string guion = new String('-', guiones);
            puntuaciones.Add(guion + criterio.DisplayName, Math.Round(criterio.Puntuacion, 2));
            if (criterio.Hijos != null && criterio.Hijos.Count > 0)
            {
                foreach (var hijo in criterio.Hijos)
                {
                    ArmarPuntuaciones(hijo, puntuaciones, guiones + 1);
                }
            }
        }

        public List<SugerenciaCupoDto> ObtenerSugerenciaCupo(int ComercialId, int? materialId)
        {
            var materiales = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni { MaterialId = x.MaterialId, Descripcion = x.Descripcion }, x => materialId == null || x.MaterialId == materialId);
            var sugerencias = new List<SugerenciaCupoDto>();
            foreach (var material in materiales)
            {
                var formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(material.MaterialId));
                var resultado = repositorio.ObtenerConsultaEscalar(new ObtenerSugerencias(formula.CuposDesde, formula.CuposHasta, ComercialId, material.MaterialId, "1029"));
                foreach (var item in resultado)
                {
                    item.Puntuaciones = JsonConvert.DeserializeObject<Dictionary<string, decimal>>(item.PuntuacionesString);
                }
                sugerencias.AddRange(resultado);
            }
            var hoy = DateTime.Now.Date;
            return sugerencias.Where(x => x.FechaSugerida >= hoy).ToList();
        }
        private List<SugerenciaCupo> ObtenerSugerenciaCupoPorProveedor(List<int> proveedor, int materialId)
        {
            var fecha = DateTime.Now.Date;
            List<SugerenciaCupo> resultado = repositorio.Listar<SugerenciaCupo>(a => a.Aceptado == null && a.MaterialId == materialId && proveedor.Contains(a.ProveedorId.Value)
             && a.FechaSugerida >= fecha);

            return resultado;
        }
        public IList<SugerenciaCupoDto> ObtenerSugerenciaCupoAgrupadasPorProveedor(int comercialId, int materialId, string centroId)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(materialId));
            return repositorio.ObtenerConsultaEscalar(new ObtenerSugerenciaAgrupadasPorProveedor(formula.CuposDesde, formula.CuposHasta, comercialId, materialId, centroId)).Where(x => x.CantidadDeCupos > 0).ToList();
        }
        private IList<SugerenciaCupoDto> ObtenerSugerenciasAgrupadasPorProveedor(int comercialId, int materialId, string centroId)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(materialId));
            return repositorio.ObtenerConsultaEscalar(new ObtenerSugerenciaAgrupadasPorProveedor(formula.CuposDesde, formula.CuposHasta, comercialId, materialId, centroId));
        }

        public List<SugerenciaPorComercialDto> ObtenerSugerenciaPorComercialFecha(int comercialId, int materialId, string centroId)
        {
            Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(materialId));
            return repositorio.Listar<SugerenciaPorComercial, SugerenciaPorComercialDto>(x => new SugerenciaPorComercialDto
            {
                Id = x.Id,
                CentroId = x.CentroId,
                ComercialId = x.ComercialId,
                MaterialId = x.MaterialId,
                Total = x.Total,
                Fecha = x.Fecha
            }, x => x.Fecha >= formula.CuposDesde && x.Fecha <= formula.CuposHasta
            && x.ComercialId == comercialId && x.MaterialId == materialId && x.Centro.CodigoSap == centroId).ToList();
        }

        public List<CupoResult> AceptarSugerenciaCupo(List<SugerenciaCupoDto> sugerenciasAceptadas)
        {
            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            List<CupoResult> resultado = new List<CupoResult>();
            var result = new CupoResult();
            var result2 = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            var proveedores = repositorio.Listar<Proveedor>();
            var ids = sugerenciasAceptadas.Select(b => b.Id).ToArray();
            var solicitudes = new List<string>();
            List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a => ids.Contains(a.Id) && a.Aceptado == null);

            foreach (var s in sugerencias)
            {
                if (sugerencias.Count > 0)
                {
                    var sugerenciaPorComercial = ObtenerSugerenciaPorComercial(s.FechaSugerida, s.ComercialId, s.MaterialId, s.Centro.CodigoSap);
                    var cantidadIngresada = sugerenciasAceptadas.Where(a => a.Id == s.Id).Single().CantidadDeCupos + (sugerenciasAceptadas.Where(a => a.Id == s.Id).Single().CantidadFleteProcedencia ?? 0);
                    //Acepto o resto las sugerencias, en base a las sugerencias creo los cupos

                    var devolver = s.CantidadDeCupos - cantidadIngresada > 0;
                    var grupoDeCompras = comerciales.Where(x => x.ComercialId == s.ComercialId).First().GrupoDeCompras.Descripcion;
                    if (devolver)
                    {
                        var devolucion = new AdministracionCupo()
                        {
                            ComercialCreadorId = comercialCreador,
                            CantidadCupo = s.CantidadDeCupos - cantidadIngresada,
                            ComercialId = s.ComercialId,
                            Fecha = s.FechaSugerida,
                            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo,
                            MaterialId = s.MaterialId,
                            ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                            CentroId = s.CentroId,
                            Excedente = false,
                            TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                            FechaCreacion = DateTime.Now,
                        };
                        repositorio.Agregar(devolucion);
                        solicitudes.Add(s.Proveedor.RazonSocial + "<br/><hr /> Para la fecha: " + s.FechaSugerida.ToString("dd/MM/yyyy") + " se devolvieron: " + devolucion.CantidadCupo + (devolucion.CantidadCupo > 1 ? " cupos" : " cupo"));

                        SugerenciaCupo sugerenciaParteRechazada = new SugerenciaCupo
                        {
                            Aceptado = false,
                            CantidadCupoOriginal = s.CantidadCupoOriginal,
                            CantidadDeCupos = s.CantidadDeCupos - cantidadIngresada,
                            CDWarrant = s.CDWarrant,
                            FechaSugerida = s.FechaSugerida,
                            ProveedorId = s.ProveedorId,
                            CentroId = s.CentroId,
                            ComercialId = s.ComercialId,
                            ConfiguracionEspacioDinamicoId = s.ConfiguracionEspacioDinamicoId,
                            ContratoSAP = s.ContratoSAP,
                            MaterialId = s.MaterialId,
                            MonedaId = s.MonedaId == "     " ? null : s.MonedaId,
                            MotivoRechazo = "Generado Automaticamente por devolucion de cupos.",
                            Destinatario = s.Destinatario,
                            NegocioId = s.NegocioId,
                            Precio = s.Precio,
                            Puntuacion = s.Puntuacion,
                            Puntuaciones = s.Puntuaciones,
                            StandardDeCalidad = s.StandardDeCalidad,
                            TipoNegocioId = s.TipoNegocioId,
                            ZonaCupoId = s.ZonaCupoId
                        };
                        repositorio.Agregar(sugerenciaParteRechazada);

                    }

                    s.CantidadDeCupos = cantidadIngresada;
                    s.Aceptado = true;
                    var fleteSolicitud = sugerenciasAceptadas.Where(a => a.Id == s.Id).Single().CantidadFleteProcedencia ?? 0;
                    var cupoNormalSolicitud = sugerenciasAceptadas.Where(a => a.Id == s.Id).Single().CantidadDeCupos;
                    if (sugerenciaPorComercial.Total < cantidadIngresada)
                    {
                        s.CantidadDeCupos = sugerenciaPorComercial.Total;
                        var razonSocial = proveedores.Where(x => x.ProveedorId == s.ProveedorId).First().RazonSocial;

                        var solicitud = new AdministracionCupo()
                        {
                            CantidadCupo = 0,
                            ComercialCreadorId = comercialCreador,
                            ComercialId = s.ComercialId,
                            Fecha = s.FechaSugerida,
                            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                            MaterialId = s.MaterialId,
                            ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                            CentroId = s.CentroId,
                            Excedente = true,
                            CantidadFleteProcedencia = 0,
                            ProveedorId = s.ProveedorId,
                            TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                            FechaCreacion = DateTime.Now,
                        };

                        if (sugerenciaPorComercial.Total < cupoNormalSolicitud)
                        {
                            //Generar solicitudes por la resta
                            solicitud.CantidadCupo = cupoNormalSolicitud - sugerenciaPorComercial.Total;

                            //Generear cupos
                            result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, sugerenciaPorComercial.Total, false, null);
                        }
                        else
                        {
                            //Generar cupos
                            result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, cupoNormalSolicitud, false, null);
                        }

                        if (sugerenciaPorComercial.Total < fleteSolicitud)
                        {
                            //Generar solicitudes por la resta
                            solicitud.CantidadFleteProcedencia = fleteSolicitud - sugerenciaPorComercial.Total;

                            //Generear cupos
                            result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, sugerenciaPorComercial.Total, true, null);
                        }
                        else
                        {
                            //Generar cupos
                            result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, fleteSolicitud, true, null);
                        }

                        for (int i = 0; i < result2.ListaCupos.Count; i++)
                        {
                            result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
                        }


                        result.Errores.AddRange(result2.Errores);
                        result.ListaCupos.AddRange(result2.ListaCupos);

                        if (solicitud.CantidadFleteProcedencia > 0 || solicitud.CantidadCupo > 0)
                        {
                            repositorio.Agregar(solicitud);
                            logger.Debug(s.Proveedor.RazonSocial + "<br/> Se creo una solicitud flete: " + solicitud.CantidadFleteProcedencia + " cupo normal: " + solicitud.CantidadCupo);
                        }

                        solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
                        solicitudes.Add(s.Proveedor.RazonSocial + "<br/>Para la fecha: " + s.FechaSugerida.ToString("dd/MM/yyyy") + " se generó una solicitud: <br />Cupo normales:" + solicitud.CantidadCupo + "<br /> Cupo con flete: " + solicitud.CantidadFleteProcedencia);


                    }
                    else
                    {
                        result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, cupoNormalSolicitud, false, null);
                        result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, fleteSolicitud, true, null);
                    }
                    if (!result.HayError)
                    {
                        s.Aceptado = true;
                    }
                    else
                    {
                        if (result.ListaCupos.Count > 0)
                        {
                            s.CantidadDeCupos -= result.ListaCupos.Count;
                        }
                    }
                    result.ListaCupos.AddRange(solicitudes);
                    resultado.Add(result);
                    result.ListaCupos.Add("<br/>");
                    result.ListaCupos.Insert(0, s.Proveedor.RazonSocial + "<br/>");
                }
            }
            repositorio.GuardarCambios();
            return resultado;
        }

        private CupoResult CrearCupoSugerenciaDetalle(SugerenciaCupo s, SugerenciaPorComercial sugerenciaPorComercial, int cantidadACrear, bool esFlete, DateTime? fechaCupo)
        {
            var result = new CupoResult();
            if (cantidadACrear == 0)
            {
                return result;
            }
            Cupo cupo = new Cupo
            {
                ProveedorId = s.Negocio != null && s.Negocio.ProveedorComisionistaId != null ? s.Negocio.ProveedorComisionistaId.Value : s.ProveedorId.Value,
                CentroId = s.CentroId,
                MaterialId = s.MaterialId,
                FechaIngreso = fechaCupo ?? s.FechaSugerida,
                ZonaCupoId = s.ZonaCupoId.Value,
                ComercialId = s.ComercialId,
                Calidad = s.StandardDeCalidad,
                Fason = s.TipoNegocioId == 4,
                Destinatario = s.Destinatario,
                FechaGeneracion = DateTime.Now,
                Observaciones = null,//---
                CupoSap = "",//---
                FleteProcedencia = esFlete,//---
                EstadoCupoId = 1,//---
                CupoStop = null,//---
                CreacionStop = "",//---
                ErrorStop = "",//---
                NegocioId = s.NegocioId,
                ConfiguracionEspacioDinamicoId = s.ConfiguracionEspacioDinamicoId,
                TipoNegocioId = s.TipoNegocioId,
            };

            result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadACrear, Fecha = fechaCupo ?? s.FechaSugerida } });

            sugerenciaPorComercial.Total -= result.ListaCupos.Count();
            if (sugerenciaPorComercial.Total < 0)
            {
                sugerenciaPorComercial.Total = 0;
            }
            return result;
        }

        public CupoResult AceptarCupoExcedente(int administracionId)
        {
            try
            {
                var sugerencia = repositorio.Obtener<AdministracionCupo>(administracionId);
                CupoResult resultado = new CupoResult();
                Cupo cupo = new Cupo
                {
                    ProveedorId = sugerencia.ProveedorId.Value,//---Agentecompra no tiene proveedor
                    CentroId = sugerencia.CentroId,
                    MaterialId = sugerencia.MaterialId,
                    FechaIngreso = sugerencia.Fecha,
                    ZonaCupoId = sugerencia.ZonaId,
                    ComercialId = sugerencia.ComercialId,
                    Calidad = "",
                    Fason = false,
                    Destinatario = "",
                    FechaGeneracion = DateTime.Now,
                    Observaciones = null,//---
                    CupoSap = "",//---
                    FleteProcedencia = false,//---
                    EstadoCupoId = 1,//---
                    CupoStop = null,//---
                    CreacionStop = "",//---
                    ErrorStop = "",//---
                    NegocioId = 7,
                    ConfiguracionEspacioDinamicoId = null,
                    TipoNegocioId = null,
                    AdministracionCupoId = administracionId
                };

                CupoResult result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerencia.CantidadCupo, Fecha = sugerencia.Fecha } });

                if (sugerencia.CantidadFleteProcedencia > 0)
                {
                    cupo.FleteProcedencia = true;
                    CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = sugerencia.CantidadFleteProcedencia, Fecha = sugerencia.Fecha } });

                    result.Errores.AddRange(result2.Errores);
                    result.ListaCupos.AddRange(result2.ListaCupos);
                }
                if (!result.HayError)
                {

                    sugerencia.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo;
                }
                else
                {
                    if (result.ListaCupos.Count > 0)
                    {
                        sugerencia.CantidadCupo -= result.ListaCupos.Count;
                    }
                }
                resultado.Errores.AddRange(result.Errores);
                resultado.ListaCupos.AddRange(result.ListaCupos);
                repositorio.GuardarCambios();

                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                return null;
            }
        }


        public List<DateTime> FechasComprendidas(int? materialId = null)
        {
            var fechas = new List<DateTime>();

            var materiales = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni { MaterialId = x.MaterialId, Descripcion = x.Descripcion }, x => x.MaterialId == materialId || materialId == null);
            foreach (var material in materiales)
            {
                Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(material.MaterialId));
                var fechaInicio = formula.CuposDesde < DateTime.Now.Date ? DateTime.Now.Date : formula.CuposDesde;
                var i = 1;
                fechas.Add(fechaInicio);
                while (fechaInicio.AddDays(i) <= formula.CuposHasta)
                {
                    fechas.Add(fechaInicio.AddDays(i++));
                }
            }
            return fechas.Distinct().ToList();
        }
        //private CupoResult ValidarDisponibilidad(List<SugerenciaCupo> sugerenciasAceptadas)
        //{
        //    Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula());
        //    var formulaDto = FormulaToDto(formula);
        //    List<Cupo> cupos = repositorio.Listar<Cupo>(x => x.FechaIngreso >= formulaDto.CuposDesde && x.FechaIngreso <= formulaDto.CuposHasta && x.CentroId == formulaDto.CentroId);
        //    List<ConfiguracionCupoDto> disponibilidadEnPlantas = ObtenerDisponibilidadEnPlantas(formulaDto, cupos);
        //    CupoResult result = new CupoResult();
        //    foreach (var sugerencia in sugerenciasAceptadas)
        //    {
        //        var disponibles = disponibilidadEnPlantas.Where(a => a.MaterialId == sugerencia.MaterialId && a.LimiteCupo > 0 && a.Fecha.Date == sugerencia.FechaSugerida.Date).ToList();
        //        if (disponibles.Count == 0)
        //        {
        //            result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en planta.");
        //            return result;
        //        }
        //        foreach (var disponible in disponibles)
        //        {

        //            if (disponible.CantidadCupo.Count() > 0)
        //            {
        //                var disponiblezona = disponible.CantidadCupo.Where(a => a.ZonaCupo == sugerencia.ZonaCupo.Descripcion).SingleOrDefault();
        //                if (disponiblezona != null)
        //                {
        //                    if (disponiblezona.CantidadCupo >= sugerencia.CantidadDeCupos)
        //                    {
        //                        disponiblezona.CantidadCupo -= sugerencia.CantidadDeCupos;
        //                        disponible.LimiteCupo -= sugerencia.CantidadDeCupos;
        //                    }
        //                    else
        //                    {
        //                        result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en la zona.");
        //                        return result;
        //                    }
        //                }
        //                else
        //                {
        //                    result.Error("Disponible en planta", "La cantidad de cupos que intenta generar para la fecha " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " supera el disponible en la zona.");
        //                    return result;
        //                }
        //            }
        //            else
        //            {
        //                disponible.LimiteCupo -= sugerencia.CantidadDeCupos;
        //            }
        //        }
        //    }

        //    return result;
        //}

        private FormulaDto FormulaToDto(Formula formula)
        {
            return new FormulaDto
            {
                CuposDesde = formula.CuposDesde,
                CuposHasta = formula.CuposHasta,
                NegociosDesde = formula.NegociosDesde,
                NegociosHasta = formula.NegociosHasta,
                Criterio = formula.Criterio,
                Id = formula.Id,
                CentroId = formula.CentroId,
                CriterioId = formula.CriterioId,
                Fecha = formula.Fecha,
                MaterialId = formula.MaterialId,
                Material = formula.Material.Descripcion
            };
        }

        public CupoResult RechazarSugerenciaCupo(List<int> ids, string motivo)
        {
            var resultado = new CupoResult();
            try
            {
                var activeCreador = PermisosHelper.ObtenerUsuario();
                var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
                var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
                var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
                if (validarSiHayCierre)
                {
                    resultado.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                    return resultado;
                }

                List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a => ids.Contains(a.Id) && a.Aceptado == null);
                var comercialId = sugerencias.First().ComercialId;
                var sugerenciasPorComercial = repositorio.Listar<SugerenciaPorComercial>(
                           x => x.ComercialId == comercialId);


                List<AdministracionCupo> admCupos = new List<AdministracionCupo>();
                foreach (var sugerencia in sugerencias)
                {
                    var spc = sugerenciasPorComercial.Where(a => a.MaterialId == sugerencia.MaterialId && a.Fecha == sugerencia.FechaSugerida && a.CentroId == a.CentroId).FirstOrDefault();

                    sugerencia.Aceptado = false;
                    sugerencia.MotivoRechazo = motivo;
                    admCupos.Add(new AdministracionCupo
                    {
                        ComercialCreadorId = comercialCreador,
                        CantidadCupo = spc.Total < sugerencia.CantidadDeCupos ? spc.Total : sugerencia.CantidadDeCupos,
                        CentroId = sugerencia.CentroId,
                        ComercialId = sugerencia.ComercialId,
                        Excedente = false,
                        EstadoId = 0,
                        MaterialId = sugerencia.MaterialId,
                        ProveedorId = sugerencia.ProveedorId,
                        ZonaId = sugerencia.ZonaCupoId ?? 1,
                        Fecha = sugerencia.FechaSugerida,
                        TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                    });
                    if (spc != null)
                    {
                        spc.Total -= sugerencia.CantidadDeCupos;
                        if (spc.Total < 0)
                        {
                            spc.Total = 0;
                        }
                    }
                }
                repositorio.AgregarTodos(admCupos);

                repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                var nuevoResultado = new CupoResult();
                nuevoResultado.Error("eliminar", e.Message);
                return nuevoResultado;
            }

        }

        private void darFormatoAlFiltro(IEnumerable<Filter> filtros)
        {
            filtros.ToList().ForEach(p =>
            {
                if (p.Value == null)
                {
                    darFormatoAlFiltro(p.Filters);
                }
                else
                {
                    if (p.Value.GetType().IsArray)
                    {
                        var unicoValueDelArray = p.Value as object[];
                        p.Value = unicoValueDelArray.First();
                    }
                }
            });
            convertirFechaDe<CupoDto>(filtros);
            convertirBool<CupoDto>(filtros);
        }

        private void convertirFechaDe<TAlgunDto>(IEnumerable<Filter> filtros)
        {
            var propiedadesConFechas = typeof(TAlgunDto).GetProperties().Where(x => x.PropertyType == typeof(DateTime)).ToList();

            var filtrosConFechas = filtros.Where(y => propiedadesConFechas.Any(x => x.Name == y.Field));
            filtrosConFechas.ToList().ForEach(x =>
            {
                x.Value = DateTime.Parse(x.Value.ToString());
            });
        }

        private void convertirBool<TAlgunDto>(IEnumerable<Filter> filtros)
        {

            var propiedadesConBool = typeof(TAlgunDto).GetProperties().Where(x => x.PropertyType == typeof(bool)).ToList();

            var filtrosConFechas = filtros.Where(y => propiedadesConBool.Any(x => x.Name == y.Field));
            filtrosConFechas.ToList().ForEach(x =>
            {

                x.Value = bool.Parse(x.Value.ToString());
            });
        }

        //==============view

        public List<EstadoCupoDto> TraerTodoLosEstados()
        {
            return repositorio.Listar<EstadoCupo, EstadoCupoDto>(x => new EstadoCupoDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion,
                Orden = x.Orden,
            });
        }

        public SugerenciaCupoDto ObtenerSugerencia(int sugerenciaId)
        {
            return repositorio.Obtener<SugerenciaCupo, SugerenciaCupoDto>(a => a.Id == sugerenciaId, a => new SugerenciaCupoDto
            {
                Id = a.Id,
                Precio = a.Precio,
                TipoNegocioId = a.TipoNegocioId,
                TipoNegocioDesc = a.TipoNegocio.Descripcion,
                CentroId = a.CentroId,
                NegocioId = a.NegocioId,
                CantidadDeCupos = a.CantidadDeCupos,
                DestinoId = 1,
                FechaSugerida = a.FechaSugerida,
                MaterialId = a.MaterialId,
                MaterialDesc = a.Material.Descripcion,
                MonedaId = a.MonedaId == null ? "" : a.MonedaId,
                MonedaDesc = a.MonedaId == null ? "" : a.Moneda.Descripcion,
                PuntuacionTotal = a.Puntuacion,
                ProveedorId = a.ProveedorId,
                ProveedorDesc = a.ProveedorId.HasValue ? a.Proveedor.RazonSocial : "",
                ProveedorCUIT = a.ProveedorId.HasValue ? a.Proveedor.CUIT : "",
                PuntuacionesString = a.Puntuaciones,
                ContratoSAP = a.ContratoSAP,
                ZonaCupoId = a.ZonaCupoId,
                ZonaDescrip = a.ZonaCupo.Descripcion,
                ConfiguracionEspacioDinamicoId = a.ConfiguracionEspacioDinamicoId
            });
        }

        //private CupoResult CrearCupos(DiaCupo detalle, SugerenciaCupo sugerencia, Dictionary<DateTime, int> cuposDevueltos, bool esLaUltimaSugerencia)
        //{
        //    var resultado = new CupoResult();
        //    int cuposDevueltosParaLaFecha = 0;
        //    if (esLaUltimaSugerencia)
        //    {
        //        cuposDevueltos.TryGetValue(sugerencia.FechaSugerida, out cuposDevueltosParaLaFecha);
        //    }

        //    var cantidadFleteProcedencia = detalle.CantidadFleteProcedencia ?? 0;
        //    cantidadFleteProcedencia = sugerencia.CantidadDeCupos < cantidadFleteProcedencia ? sugerencia.CantidadDeCupos : cantidadFleteProcedencia;
        //    var cantidadSugerencia = sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha > detalle.CantidadSugerencia
        //                                ? detalle.CantidadSugerencia : sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha;

        //    if (cantidadSugerencia == sugerencia.CantidadDeCupos + cuposDevueltosParaLaFecha && cuposDevueltos.ContainsKey(sugerencia.FechaSugerida.Date))
        //    {
        //        cuposDevueltos[sugerencia.FechaSugerida] -= cantidadSugerencia - sugerencia.CantidadDeCupos;
        //    }

        //    var cupo = new Cupo
        //    {
        //        ProveedorId = sugerencia.ProveedorId.Value,//---Agentecompra no tiene proveedor
        //        CentroId = sugerencia.CentroId,
        //        MaterialId = sugerencia.MaterialId,
        //        FechaIngreso = sugerencia.FechaSugerida,
        //        ZonaCupoId = sugerencia.ZonaCupoId.Value,
        //        ComercialId = sugerencia.ComercialId,
        //        Calidad = sugerencia.StandardDeCalidad,
        //        Fason = sugerencia.TipoNegocioId == 4,
        //        Destinatario = sugerencia.Destinatario,
        //        FechaGeneracion = DateTime.Now,
        //        Observaciones = null,//---
        //        CupoSap = "",//---
        //        FleteProcedencia = false,//---
        //        EstadoCupoId = 1,//---
        //        CupoStop = null,//---
        //        CreacionStop = "",//---
        //        ErrorStop = "",
        //        NegocioId = sugerencia.NegocioId,
        //        ConfiguracionEspacioDinamicoId = sugerencia.ConfiguracionEspacioDinamicoId,
        //        TipoNegocioId = sugerencia.TipoNegocioId,
        //    };

        //    logger.Debug($"Cupos {cuposDevueltos} ");
        //    //si tengo al menos un cupo normal para crear...
        //    if (cantidadSugerencia - cantidadFleteProcedencia > 0)
        //    {
        //        var result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadSugerencia - cantidadFleteProcedencia, Fecha = sugerencia.FechaSugerida } });
        //        detalle.CantidadSugerencia -= cantidadSugerencia;
        //        //sugerencia.CantidadDeCupos = cantidadSugerencia;
        //        resultado.Errores.AddRange(result.Errores);
        //        resultado.ListaCupos.AddRange(result.ListaCupos);
        //    }

        //    //si tengo al menos un cupo flete proc para crear...
        //    if (cantidadFleteProcedencia > 0)
        //    {
        //        cupo.FleteProcedencia = true;
        //        CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cantidadFleteProcedencia, Fecha = sugerencia.FechaSugerida } });
        //        detalle.CantidadFleteProcedencia -= cantidadFleteProcedencia;
        //        resultado.Errores.AddRange(result2.Errores);
        //        for (int i = 0; i < result2.ListaCupos.Count; i++)
        //        {
        //            result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
        //        }
        //        resultado.ListaCupos.AddRange(result2.ListaCupos);
        //    }
        //    return resultado;
        //}


        private Dictionary<DateTime, int> CalcularCuposDevueltos(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, List<SugerenciaCupo> sugerenciaTodosLosProveedores)
        {
            var cuposDevueltos = new Dictionary<DateTime, int>();
            foreach (var detalle in datosTablaPorProveedor)
            {
                foreach (var fechaProveedor in detalle.Detalles)
                {
                    var sugerenciasProveedorFecha = sugerenciaTodosLosProveedores.Where(a => a.FechaSugerida == fechaProveedor.Fecha && a.ProveedorId == detalle.ProveedorId);
                    var totalDeCuposIngresadosEnPantalla = fechaProveedor.CantidadSugerencia;
                    var totalDeCuposEnSugerenciasExistentes = sugerenciasProveedorFecha.Sum(x => x.CantidadDeCupos);
                    if (totalDeCuposIngresadosEnPantalla < totalDeCuposEnSugerenciasExistentes)
                    {
                        var cuposDevueltosDelDia = totalDeCuposEnSugerenciasExistentes - totalDeCuposIngresadosEnPantalla;
                        if (cuposDevueltos.ContainsKey(fechaProveedor.Fecha))
                        {
                            cuposDevueltos[fechaProveedor.Fecha] += cuposDevueltosDelDia;
                        }
                        else
                        {
                            cuposDevueltos.Add(fechaProveedor.Fecha, cuposDevueltosDelDia);
                        }
                    }
                }
            }
            return cuposDevueltos;
        }
        public List<DiaCupo> Panel()
        {
            var lista = new List<DiaCupo>();
            try
            {
                var materiales = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
                foreach (var material in materiales)
                {
                    Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(material.MaterialId));
                    List<ConfiguracionCupo> configuracionCupo = repositorio.Listar<ConfiguracionCupo>(x => x.Fecha >= formula.CuposDesde && x.Fecha <= formula.CuposHasta && x.CentroId == formula.CentroId);
                    List<ConfiguracionCupoDto> disponibilidadEnPlanta = configuracionCupo.Select(x => new ConfiguracionCupoDto { CentroId = x.CentroId, LimiteAlgoritmo = x.LimiteAlgoritmo, LimiteCupo = x.LimiteCupo, MaterialId = x.MaterialId, Fecha = x.Fecha }).ToList();
                    var sugerencias = repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida >= formula.CuposDesde && x.FechaSugerida <= formula.CuposHasta && x.CentroId == formula.CentroId && x.Aceptado == true && x.MaterialId == material.MaterialId);
                    var fechasComprendidas = FechasComprendidas(null);
                    var sugerenciasPorComercial = repositorio.Listar<SugerenciaPorComercial, SugerenciaPorComercialDto>(a => new SugerenciaPorComercialDto { CentroId = a.CentroId, ComercialId = a.ComercialId, Fecha = a.Fecha, Id = a.Id, MaterialId = a.MaterialId, Total = a.Total });

                    foreach (var fecha in fechasComprendidas)
                    {
                        if (fecha >= formula.CuposDesde && fecha <= formula.CuposHasta)
                        {
                            var CantidadCuposGenerados = (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaIngreso) == fecha && x.CentroId == formula.CentroId && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9 && x.MaterialId == material.MaterialId)).Count;
                            var hoy = DateTime.Now.Date;
                            var cupo = new DiaCupo()
                            {
                                Fecha = fecha,
                                MaterialId = material.MaterialId,
                                Material = material.Descripcion,
                                //total de dispo para el dia
                                CantidadDisponibilidadPlanta = disponibilidadEnPlanta.Where(x => x.Fecha == fecha && x.MaterialId == material.MaterialId).Sum(y => y.LimiteCupo),
                                //Disponibilidad en planta
                                CantidadDisponibilidadDia = disponibilidadEnPlanta.Where(x => x.Fecha == fecha && x.MaterialId == material.MaterialId).Sum(y => y.LimiteCupo) - disponibilidadEnPlanta.Where(x => x.Fecha == fecha && x.MaterialId == material.MaterialId).Sum(y => y.LimiteAlgoritmo),
                                //Disponibilidad algoritmo
                                CantidadAlgoritmo = disponibilidadEnPlanta.Where(x => x.Fecha == fecha && x.MaterialId == material.MaterialId).Sum(y => y.LimiteAlgoritmo),

                                //Sugerencias Aceptadas
                                CantidadSugerenciaAceptadaDia = (int)repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == true && x.MaterialId == material.MaterialId),
                                //Sugerencias Devueltas
                                CantidadCuposDevueltos = (int)repositorio.Sumar<AdministracionCupo>(x => x.CantidadCupo + x.CantidadFleteProcedencia, x => x.Fecha == fecha && !x.Excedente && x.MaterialId == material.MaterialId),
                                //Solicitudes Aceptadas
                                CantidadSolicitudesAceptadas = (int)repositorio.Sumar<AdministracionCupo>(x => x.CantidadCupo + x.CantidadFleteProcedencia, x => x.Fecha == fecha && x.Excedente && x.EstadoId == 1 && x.MaterialId == material.MaterialId),
                                //Solicitudes Pendientes Sugerencias
                                CantidadSolicitudesPendientes = (int)repositorio.Sumar<AdministracionCupo>(x => x.CantidadCupo + x.CantidadFleteProcedencia, x => x.Fecha == fecha && x.Excedente && x.EstadoId == 3 && x.MaterialId == material.MaterialId && x.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Algoritmo),
                                //Solicitudes Pendientes Extra
                                CantidadSolicitudesPendientesExtra = (int)repositorio.Sumar<AdministracionCupo>(x => x.CantidadCupo + x.CantidadFleteProcedencia, x => x.Fecha == fecha && x.Excedente && x.EstadoId == 3 && x.MaterialId == material.MaterialId && x.TipoAdministracionCupoId == (int)EnumTipoAdministracionCupo.Extraordinaria),


                                CantidadSugerenciaPendiente = (int)repositorio.Listar<SugerenciaCupo>(x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == null
                                && x.MaterialId == material.MaterialId).Sum(x => x.CantidadDeCupos),
                                // no se usa
                                //CantidadSugerencia = (int)repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.MaterialId == material.MaterialId),
                            };
                            //var CantidadSugerenciaRechazadaDia = (int)repositorio.Sumar<SugerenciaCupo>(x => x.CantidadDeCupos, x => x.FechaSugerida == fecha && x.CentroId == formula.CentroId && x.Aceptado == false && x.MaterialId == material.MaterialId);
                            //cupo.CantidadCuposLibres = (cupo.CantidadDisponibilidadDia - cupo.CantidadSugerenciaAceptadaDia - cupo.CantidadSugerenciaPendiente + cupo.CantidadCuposDevueltos - cupo.CantidadSolicitudesAceptadas);
                            //cupo.CantidadSugerenciaAceptadaDia = cupo.CantidadSugerenciaAceptadaDia >= cupo.CantidadCuposDevueltos ?
                            //    cupo.CantidadSugerenciaAceptadaDia - cupo.CantidadCuposDevueltos : 0;

                            //cupo.CantidadCuposLibres = cupo.CantidadDisponibilidadDia - CantidadCuposGenerados - cupo.CantidadSugerenciaPendiente + cupo.CantidadCuposDevueltos;

                            var sugerenciasPendientes = sugerenciasPorComercial.Where(a => a.Fecha == fecha && a.MaterialId == material.MaterialId).Sum(a => a.Total);
                            //Cupos Libres
                            logger.Debug($"Cantidad Planta : {cupo.CantidadDisponibilidadPlanta}, sug Pendientes: {cupo.CantidadSugerenciaPendiente}, Cupos creados: {CantidadCuposGenerados}");
                            cupo.CantidadCuposLibres = cupo.CantidadDisponibilidadPlanta - CantidadCuposGenerados;
                            lista.Add(cupo);
                        }
                    }
                }

            }
            catch (Exception e)
            {
                return lista;
            }
            return lista.OrderBy(x => x.Fecha).ToList();
        }
        public List<SugerenciaNoAceptada> SugerenciasNoAceptadas()
        {
            var lista = new List<SugerenciaNoAceptada>();
            var materiales = repositorio.Listar<Material, MaterialIni>(x => new MaterialIni { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
            foreach (var material in materiales)
            {
                Formula formula = repositorio.ObtenerConsultaEscalar(new ObtenerUltimaFormula(material.MaterialId));
                var dias = new List<DateTime>();
                var fechaInicio = formula.CuposDesde < DateTime.Now.Date ? DateTime.Now.Date : formula.CuposDesde;
                var i = 1;
                dias.Add(fechaInicio);
                while (fechaInicio.AddDays(i) <= formula.CuposHasta)
                {
                    dias.Add(fechaInicio.AddDays(i++));
                }
                var cupoDesde = (formula.CuposDesde < DateTime.Now.Date ? DateTime.Now.Date : formula.CuposDesde);
                var sugerenciaComercialDia = repositorio.Listar<SugerenciaCupo>(x => x.FechaSugerida >= cupoDesde && x.FechaSugerida <= formula.CuposHasta
                && x.CentroId == formula.CentroId && x.Aceptado == null && x.MaterialId == material.MaterialId && (x.Solicitudes.Count == 0 || x.Solicitudes.All(y => y.EstadoId == (int)EnumEstadoAdministracionCupo.EstadoRechazadoAdministracionCupo))).GroupBy(y => y.Comercial);
                foreach (var sug in sugerenciaComercialDia)
                {
                    var dia = new SugerenciaNoAceptada()
                    {
                        Material = material.Descripcion,
                        MaterialId = material.MaterialId,
                        NombreComercial = sug.Key.Nombres + " " + sug.Key.Apellido,
                        Diacupo = new List<DiaCupo>()
                    };
                    foreach (var d in sug.GroupBy(x => x.FechaSugerida))
                    {
                        foreach (var f in dias)
                        {
                            if(f == d.Key)
                            {
                                dia.Diacupo.Add(new DiaCupo
                                {
                                    Fecha = d.Key,
                                    Cantidad = d.Sum(x => x.CantidadDeCupos)
                                });
                            }
                            else
                            {
                                dia.Diacupo.Add(new DiaCupo
                                {
                                    Fecha = f,
                                    Cantidad = 0
                                });
                            }
                        }
                       
                    }
                    lista.Add(dia);
                }
            }

            return lista;
        }


        public void EnviarMailSinCtg()
        {
            var cupos = repositorio.Listar<Cupo>(x => x.EstadoCupoId == 1 && x.FechaIngreso == DateTime.Today, 0, "CupoSap").GroupBy(x => new { ProveedorId = x.ProveedorId, ComercialId = x.ComercialId });

            foreach (var p in cupos)
            {
                logger.Debug("EnvioMailSinCtg ComercialId " + p.Key.ComercialId + " ProveedorId " + p.Key.ProveedorId);
                var lista = new List<string>();

                var comercial = repositorio.Obtener<Comercial>(p.Key.ComercialId);
                var email = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
                var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == p.Key.ProveedorId && x.Cupo == true);
                if (emailproveedor.Count <= 0)
                {
                    continue;
                }
                lista.Add(email);
                if (comercial.RolesAsociados.Any(a => a.Descripcion == "Reenvio Mails Cupos Corredores Rosario"))
                {
                    var comerciales = comercialManager.TraerTodoComercial().Comercial.Where(a => a.Rol.ToUpper().Contains("Reenvio Mails Cupos Corredores Rosario".ToUpper()) && a.Deshabilitado != true && a.ComercialId != p.Key.ComercialId).ToList();
                    foreach (var item in comerciales)
                    {
                        var comercialAdicional = repositorio.Obtener<Comercial>(item.ComercialId);
                        try
                        {
                            var emailAdicional = mailManager.GetEmailUserActiveDirectory(comercialAdicional.IdActiveDirectory);
                            lista.Add(emailAdicional);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
                mailManager.EnviarMail(emailproveedor,
                   "Estado de cupos", "", lista, CuerpoMailSinCtg(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"),
                    p.ToList(), comercial));

            }
        }

        private AlternateView CuerpoMailSinCtg(String filePath, List<Cupo> cupos, Comercial comercial)
        {
            var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla los cupos sin activar con Molinos Agro S.A: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "Material" + "</td>" +
                    th + "Fecha de Cupo" + "</td>" +
                    th + "Productor/Corredor" + "</td>" +
                    th + "Cupos Generados" + "</td>" +
                    th + "Estado de cupo:" + "</td>" +
                    "</tr>";

            foreach (var c in cupos)
            {
                htmlBody += TrEncabezado(c, ref linea);
            }

            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /><br /> En el caso que sea necesario, comuníquese con  " + comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<DisponibilidadCuposDto> TraerCupoDisponibilidad(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId)
        {
            List<DisponibilidadCuposDto> resultado = disponibilidadCuposAgent.TraerDisponibilidadCupos(fechaDesde, fechaHasta, zonaId, centroId, materialId);
            List<DisponibilidadCuposDto> resultadoNoPropios = TraerCupoDisponibilidadNoPropios(fechaDesde, fechaHasta, zonaId, centroId, materialId);



            resultado.AddRange(resultadoNoPropios);
            return resultado;
        }

        public List<DisponibilidadCuposDto> TraerCupoDisponibilidadNoPropios(DateTime? fechaDesde, DateTime? fechaHasta, string zonaId, List<string> centroId, string materialId)
        {
            var centros = repositorio.Listar<Centro>();
            List<int> centrosIds = new List<int>();
            if (centroId == null || centroId.Count == 0)
            {
                centros = centros.Where(a => a.NoPropio == true).ToList();
            }
            else
            {
                centros = centros.Where(a => centroId.Contains(a.CodigoSap) && a.NoPropio == true).ToList();
            }
            centrosIds = centros.Select(a => a.Id).ToList();
            if (fechaHasta.HasValue)
            {
                fechaHasta = fechaHasta.Value.AddDays(1);
            }
            //List<ConfiguracionCupo> conf = new List<ConfiguracionCupo>();
            //foreach(var item in centros)
            //{
            //    conf.AddRange(repositorio.Listar<ConfiguracionCupo>(a => a.Centro.NoPropio == true && fechaDesde <= a.Fecha && fechaHasta >= a.Fecha && a.CentroId == item.Id));
            //}
            var conf = repositorio.Listar<ConfiguracionCupo>(a => a.Centro.NoPropio == true && fechaDesde <= a.Fecha && fechaHasta >= a.Fecha && centrosIds.Contains(a.CentroId));

            List<DisponibilidadCuposDto> result = new List<DisponibilidadCuposDto>();
            foreach (var itemConf in conf)
            {
                if (!String.IsNullOrEmpty(zonaId))
                {
                    var zona = repositorio.Obtener<ZonaCupo>(x => x.CodigoSap == zonaId);
                    var limiteCupoZona = itemConf.CantidadCupo.Where(x => x.ZonaCupo.CodigoSap == zonaId).FirstOrDefault();
                    var CuposConsumidos = repositorio.Contar<CupoNoPropio>(x => x.FechaIngreso == itemConf.Fecha && x.CupoId != null && x.CentroId == itemConf.CentroId && x.MaterialId == itemConf.MaterialId && x.Cupo.ZonaCupoId == zona.Id);
                    //var CuposDisponibles = LimiteCupo - CuposConsumidos;
                    result.Add(new DisponibilidadCuposDto
                    {
                        CentroCodigo = itemConf.Centro.CodigoSap,
                        CentroNombre = itemConf.Centro.Descripcion,
                        Consumidos = CuposConsumidos,
                        Disponibles = limiteCupoZona.CantidadCupo - CuposConsumidos,
                        Fecha = itemConf.Fecha,
                        Limite = limiteCupoZona.CantidadCupo,
                        MaterialCodigo = itemConf.Material.Codigo,
                        MaterialId = itemConf.MaterialId,
                        MaterialNombre = itemConf.Material.Descripcion,
                        ZonaId = zonaId,
                        ZonaNombre = zona.Descripcion
                    });
                }
                else
                {
                    //var zonas = repositorio.Obtener<ZonaCupo>(x=> itemConf.Centro.);
                    foreach(var itemCupo in itemConf.CantidadCupo)
                    {
                        if (itemCupo.CantidadCupo > 0)
                        {
                            var CuposConsumidosZona = repositorio.Contar<CupoNoPropio>(x => x.FechaIngreso == itemConf.Fecha && x.CupoId != null && x.CentroId == itemConf.CentroId && x.MaterialId == itemConf.MaterialId && x.Cupo.ZonaCupoId == itemCupo.ZonaCupoId);
                            result.Add(new DisponibilidadCuposDto
                            {
                                CentroCodigo = itemConf.Centro.CodigoSap,
                                CentroNombre = itemConf.Centro.Descripcion,
                                Consumidos = CuposConsumidosZona,
                                Disponibles = itemCupo.CantidadCupo - CuposConsumidosZona,
                                Fecha = itemConf.Fecha,
                                Limite = itemCupo.CantidadCupo,
                                MaterialCodigo = itemConf.Material.Codigo,
                                MaterialId = itemConf.MaterialId,
                                MaterialNombre = itemConf.Material.Descripcion,
                                ZonaId = itemCupo.ZonaCupoId.ToString(),
                                ZonaNombre = itemCupo.ZonaCupo.Descripcion
                            });
                        }
                    }
                    //var CuposConsumidos = repositorio.Contar<CupoNoPropio>(x => x.FechaIngreso == itemConf.Fecha && x.CupoId != null && x.CentroId == itemConf.CentroId && x.MaterialId == itemConf.MaterialId);
                    //var LimiteCupo = repositorio.Contar<CupoNoPropio>(x => x.FechaIngreso == itemConf.Fecha && x.Disponible == true && x.CentroId == itemConf.CentroId && x.MaterialId == itemConf.MaterialId);
                    ////var CuposDisponibles = LimiteCupo - CuposConsumidos;
                    //result.Add(new DisponibilidadCuposDto
                    //{
                    //    CentroCodigo = itemConf.Centro.CodigoSap,
                    //    CentroNombre = itemConf.Centro.Descripcion,
                    //    Consumidos = CuposConsumidos,
                    //    Disponibles = itemConf.LimiteCupo - CuposConsumidos,
                    //    Fecha = itemConf.Fecha,
                    //    Limite = itemConf.LimiteCupo,
                    //    MaterialCodigo = itemConf.Material.Codigo,
                    //    MaterialId = itemConf.MaterialId,
                    //    MaterialNombre = itemConf.Material.Descripcion,
                    //    //ZonaId = itemConf.,

                    //});
                }
            }
            return result;
        }

        public CupoResult RechazarCupo(Cupo cupo, string idActiveDirectory)
        {
            var oEntityErrors = new CupoResult();

            var oCupoSave = repositorio.Obtener<Cupo>(cupo.Id);
            oCupoSave.MotivoRechazo = cupo.MotivoRechazo;

            if (oCupoSave != null && (oCupoSave.EstadoCupoId == 6))
            {
                try
                {
                    oCupoSave.EstadoCupoId = 9;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(oCupoSave.Id, null), TipoAccionLogDataAgro.Eliminar);
                    EnviarMailCupoRechazado(oCupoSave, idActiveDirectory);
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }
            }

            return oEntityErrors;
        }
        public void EnviarMailCupoRechazado(Cupo cupo, string idActiveDirectory)
        {

            var lista = new List<string>();
            var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == cupo.Proveedor.ProveedorId && x.Cupo == true);

            lista.Add(idActiveDirectory);
            if (emailproveedor.Count <= 0)
            {
                return;
            }
            var comercial = repositorio.Obtener<Comercial>(comercialManager.ComercialAsociado(cupo.ProveedorId));
            mailManager.EnviarMail(emailproveedor, "Cupo Rechazado", "", lista,
                CuerpoMailRechazarCupo(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), cupo, comercial));
        }

        private AlternateView CuerpoMailRechazarCupo(String filePath, Cupo cupo, Comercial comercial)
        {
            var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla el cupo rechazado por Molinos Agro S.A: <br /><br />  ";

            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "FECHA DESCARGA: </th>" + Td(ref linea) + Split(cupo.FechaIngreso.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "VENDEDOR/CORREDOR: </th>" + Td(ref linea) + cupo.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINATARIO: </th>" + Td(ref linea) + (cupo.Destinatario.ToUpper() == "30715118773" ? "MOLINOS AGRO S.A.-30715118773" : cupo.Destinatario.ToUpper()) + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINO: </th>" + Td(ref linea) + "MOLINOS AGRO S.A.-30715118773" + "</td></tr>";
            htmlBody += "<tr>" + th + "GRANO: </th>" + Td(ref linea) + cupo.Material.Descripcion.ToUpper() + "</td></tr>";


            htmlBody += "</table>";
            htmlBody += "Motivo de rechazo: " + cupo.MotivoRechazo;
            htmlBody += "<br /><br /> En el caso que sea necesario, comuníquese con  " + comercial.Nombres + " " + comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public CupoResult AceptarCupo(Cupo cupo)
        {
            var error = new CupoResult { ListaCupos = new List<string>() };
            var listaCupos = new List<string>();
            var errorSap = new Resultado();
            var cuposConSap = new List<Cupo>();
            var cupoSave = repositorio.Obtener<Cupo>(cupo.Id);
            try
            {
                listaCupos = crearCupoAgent.Crear(cupoSave, 1);
            }
            catch (Exception e)
            {
                errorSap.Error("CantidadCuposSAP", cupoSave.FechaIngreso.ToShortDateString() + ": " + e.Message);
            }
            if (errorSap.HayError)
            {
                error.Errores.AddRange(errorSap.Errores);
                return error;
            }
            cupoSave.CupoSap = listaCupos[0];
            cupoSave.EstadoCupoId = 1;
            repositorio.GuardarCambios();
            return error;
        }

        public Resultado ActualizarCupoSAP(Cupo cupoSAP)
        {
            Resultado resultado = new Resultado();
            var cupoSave = repositorio.Obtener<Cupo>(cupoSAP.Id);
            cupoSave.FechaIngreso = cupoSAP.FechaIngreso;
            cupoSave.MaterialId = cupoSAP.MaterialId;
            cupoSave.ProveedorId = cupoSAP.ProveedorId;
            cupoSave.CentroId = cupoSAP.CentroId;
            cupoSave.ZonaCupoId = cupoSAP.ZonaCupoId;
            cupoSave.Observaciones = cupoSAP.Observaciones;
            cupoSave.Destinatario = cupoSAP.Destinatario;
            cupoSave.FleteProcedencia = cupoSAP.FleteProcedencia;
            cupoSave.Calidad = cupoSAP.Calidad;
            //cupoSave.ComercialId = cupoSAP.ComercialId;
            cupoSave.EstadoCupoId = cupoSAP.EstadoCupoId;

            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSave.Id, null), cupoSAP.EstadoCupoId == 4 ? TipoAccionLogDataAgro.Eliminar : TipoAccionLogDataAgro.Modificar);

            return resultado;
        }


        public List<CupoDto> ListarCupo(string cupoSap)
        {
            return repositorio.Listar<Cupo, CupoDto>(x => new CupoDto
            {
                Id = x.Id,
                Calidad = x.Calidad,
                Centro = x.Centro.Descripcion,
                CentroId = x.CentroId,
                ComercialId = x.ComercialId,
                Destinatario = x.Destinatario,
                Fason = x.Fason,
                FleteProcedencia = x.FleteProcedencia,
                FechaIngreso = x.FechaIngreso,
                MaterialId = x.MaterialId,
                Material = x.Material.Descripcion,
                Observaciones = x.Observaciones,
                ProveedorId = x.ProveedorId,
                Proveedor = x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")",
                ZonaCupoId = x.ZonaCupoId,
                ZonaCupo = x.ZonaCupo.Descripcion,
                CupoSap = x.CupoSap,
                EstadoCupo = x.EstadoCupo.Descripcion,
                EstadoCupoId = x.EstadoCupoId,
                CartaPorte = x.CartaPorte,
                Chofer = x.Chofer,
                CodLocalidadOrigen = x.CodLocalidadOrigen,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                CorredorComprador = x.CorredorComprador,
                CorredorVendedor = x.CorredorVendedor,
                Cosecha = x.Cosecha,
                CTG = x.CTG,
                CTGFechaDesde = x.CTGFechaDesde,
                CTGFechaHasta = x.CTGFechaHasta,
                CuitOrigen = x.CuitOrigen,
                CuitOrigenAfip = x.CuitOrigenAfip,
                CupoStop = x.CupoStop == null ? "" : x.CupoSap.ToString(),
                EstadoPlanta = x.EstadoPlanta,
                FechaGeneracion = x.FechaGeneracion,
                FechaRegistro = x.FechaGeneracion,
                IntermediarioFlete = x.IntermediarioFlete,
                Km = x.Km,
                MercadoATermino = x.MercadoATermino,
                MotivoRechazo = x.MotivoRechazo,
                NroEstablecimientoOrigen = x.NroEstablecimientoOrigen,
                Peso = x.Peso,
                RemitenteComercial = x.RemitenteComercial,
                Transportista = x.Transportista,
                UsuarioCreador = x.UsuarioCreador,
                ZonaCupoSap = x.ZonaCupo.CodigoSap,
                Acopio = x.Centro.Acopio,
            },
            x => x.CupoSap.Contains(cupoSap), 15);
        }

        public List<BasicoContrato> TraerNegocioConCupoDisponible(string proveedorCuit, int material, int centro, string filtro, DateTime desde, DateTime hasta)
        {
            var negocio = repositorio.Listar<Negocio, BasicoContrato>(x => new BasicoContrato
            {
                TipoNegocioId = x.TipoNegocioId,
                TipoNegocio = x.TipoNegocio.Descripcion,
                Negocio = x.ContratoSAP,
                Cantidad = x.Cantidad,
                Id = x.Id,
                Fecha = x.Fecha
            }, x => x.Proveedor.CUIT == proveedorCuit && x.MaterialId == material && x.EstadoId == 5 && x.DestinoId == centro &&
            (x.FechaDesde <= desde && x.FechaHasta >= hasta) &&
            (x is Contrato) && x.ContratoSAP.Contains(filtro), 15, "Fecha", Entities.Helpers.DirOrden.Desc);

            var negociosSugeridos = new List<BasicoContrato>();
            for (int i = 0; i < negocio.Count(); i++)
            {
                var n = negocio[i];
                var cupoNegocio = repositorio.Contar<Cupo>(x => x.Negocio.ContratoSAP == n.Negocio);

                var cantidad = n.Cantidad - (cupoNegocio * 30000);
                if (cantidad > 0)
                {
                    var cantidadMaxCupo = Math.Ceiling(cantidad / 30000);
                    n.CantidadMaximaCupo = (int)cantidadMaxCupo;
                    negociosSugeridos.Add(n);
                }


            }

            return negociosSugeridos;
        }

        public Resultado AltaCupoSAP(Cupo cupoSAP)
        {
            Resultado resultado = new Resultado();
            try
            {
                var cupoSave = new Cupo
                {
                    FechaIngreso = cupoSAP.FechaIngreso,
                    MaterialId = cupoSAP.MaterialId,
                    ProveedorId = cupoSAP.ProveedorId,
                    CentroId = cupoSAP.CentroId,
                    ZonaCupoId = cupoSAP.ZonaCupoId,
                    Observaciones = cupoSAP.Observaciones,
                    Destinatario = cupoSAP.Destinatario,
                    FleteProcedencia = cupoSAP.FleteProcedencia,
                    Calidad = cupoSAP.Calidad,
                    CupoSap = cupoSAP.CupoSap,
                    EstadoCupoId = cupoSAP.EstadoCupoId,
                    FechaGeneracion = DateTime.Now,
                    ComercialId = cupoSAP.ComercialId
                };
                repositorio.Agregar(cupoSave);
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(ObtenerCupo(cupoSave.Id, null), TipoAccionLogDataAgro.Crear);

            }
            catch (Exception e)
            {
                logger.Error(e);
            }
            return resultado;
        }

        private void CargarDatosSugerenciasPorComercial(List<SugerenciaCupo> sugerencias, int MaterialId)
        {
            var s = sugerencias.GroupBy(x => new { x.ComercialId, x.CentroId, x.MaterialId, x.FechaSugerida }).ToList();
            var listaSugerenciaPorComercial = new List<SugerenciaPorComercial>();
            repositorio.RemoverTodos<SugerenciaPorComercial>(x => x.Id == x.Id && x.MaterialId == MaterialId);
            foreach (var item in s)
            {
                var total = new SugerenciaPorComercial
                {
                    ComercialId = item.Key.ComercialId,
                    CentroId = item.Key.CentroId,
                    MaterialId = item.Key.MaterialId,
                    Fecha = item.Key.FechaSugerida,
                    Total = item.Sum(x => x.CantidadDeCupos)
                };
                listaSugerenciaPorComercial.Add(total);

            }

            repositorio.AgregarTodos(listaSugerenciaPorComercial);
        }

        public void EliminarSugerenciaDeCupos(ConfiguracionCupo configuracion)
        {
            try
            {
                repositorio.RemoverTodos<SugerenciaCupo>(x => x.CentroId == configuracion.CentroId && x.MaterialId == configuracion.MaterialId
            && x.FechaSugerida == configuracion.Fecha);
            }
            catch (Exception e)
            {
                logger.Error("Error al eliminar las sugerencias");
                logger.Error(e.Message);
            }
            try
            {
                repositorio.RemoverTodos<SugerenciaPorComercial>(x => x.MaterialId == configuracion.MaterialId && x.CentroId == configuracion.CentroId
                && x.Fecha == configuracion.Fecha);
            }
            catch (Exception e)
            {
                logger.Error("Error al eliminar las sugerencias por comercial");
                logger.Error(e.Message);
            }

        }

        //public CupoResult ConfirmarSugerencia(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, int materialId, string centroId)
        //{
        //    //datosTabla un elemento por cada proveedor a confirmar
        //    //datosTabla.Detalle un elemento por cada fecha de cada proveedor

        //    var sugerenciaTodosLosProveedores = ObtenerSugerenciaCupoPorProveedor(datosTablaPorProveedor.Select(x => x.ProveedorId).ToList(), materialId);

        //    CupoResult resultado = new CupoResult();

        //    //Calculo los cupos que va a devolver
        //    var cuposDevueltos = CalcularCuposDevueltos(datosTablaPorProveedor, sugerenciaTodosLosProveedores);
        //    logger.Debug($"ConfirmarSugerencia {datosTablaPorProveedor.Count} ");
        //    var centro = repositorio.Obtener<Centro, int>(x => x.CodigoSap == centroId, x => x.Id);
        //    foreach (var detalle in datosTablaPorProveedor)
        //    {
        //        //Sugerencia para un día del proveedor agrupadas en un solo objeto
        //        foreach (var fechaProveedor in detalle.Detalles)
        //        {
        //            var comercial = repositorio.Obtener<Comercial>(detalle.ComercialId);
        //            var sugerenciasProveedorFecha = sugerenciaTodosLosProveedores.Where(a => a.FechaSugerida == fechaProveedor.Fecha && a.ProveedorId == detalle.ProveedorId).ToList();
        //            var totalDeCuposIngresadosEnPantalla = fechaProveedor.CantidadSugerencia;
        //            var totalDeCuposEnSugerenciasExistentes = sugerenciasProveedorFecha.Sum(x => x.CantidadDeCupos);

        //            foreach (var s in sugerenciasProveedorFecha)
        //            {

        //                s.Aceptado = true;
        //                var res = CrearCupos(fechaProveedor, s, cuposDevueltos, s == sugerenciasProveedorFecha.Last());
        //                resultado.Errores.AddRange(res.Errores);
        //                resultado.ListaCupos.AddRange(res.ListaCupos);
        //            }

        //            if (totalDeCuposIngresadosEnPantalla - totalDeCuposEnSugerenciasExistentes > 0)
        //            {
        //                logger.Debug($"CrearSugerencia  {detalle.ProveedorId} {fechaProveedor.Fecha} {fechaProveedor.CantidadSugerencia} ");

        //                var autorizacion = new AdministracionCupo()
        //                {
        //                    CantidadCupo = totalDeCuposIngresadosEnPantalla - totalDeCuposEnSugerenciasExistentes,
        //                    ComercialId = detalle.ComercialId,
        //                    Fecha = fechaProveedor.Fecha,
        //                    ProveedorId = detalle.ProveedorId,
        //                    CantidadFleteProcedencia = fechaProveedor.CantidadFleteProcedencia.Value,
        //                    EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
        //                    MaterialId = materialId,
        //                    ZonaId = repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial.GrupoDeCompras.Descripcion, x => x.Id),
        //                    CentroId = centro,
        //                    Excedente = true,
        //                    TipoAdministracionCupoId  = (int)EnumTipoAdministracionCupo.Algoritmo,
        //                };
        //                repositorio.Agregar(autorizacion);
        //            }
        //            if (cuposDevueltos.ContainsKey(fechaProveedor.Fecha.Date) && cuposDevueltos[fechaProveedor.Fecha.Date] != 0)
        //            {
        //                var autorizacion = new AdministracionCupo()
        //                {
        //                    CantidadCupo = cuposDevueltos[fechaProveedor.Fecha],
        //                    ComercialId = detalle.ComercialId,
        //                    Fecha = fechaProveedor.Fecha,
        //                    ProveedorId = detalle.ProveedorId,
        //                    CantidadFleteProcedencia = fechaProveedor.CantidadFleteProcedencia.Value,
        //                    EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
        //                    MaterialId = materialId,
        //                    ZonaId = repositorio.Obtener<ZonaCupo, int>(x => x.Descripcion == comercial.GrupoDeCompras.Descripcion, x => x.Id),
        //                    CentroId = centro,
        //                    Excedente = ,
        //                    TipoAdministracionCupoId  = (int) EnumTipoAdministracionCupo.Algoritmo,
        //                };
        //                repositorio.Agregar(autorizacion);
        //            }
        //        }
        //    }
        //    repositorio.GuardarCambios();
        //    return resultado;
        //}
        private CupoResult CrearCupos(DiaCupo detalle, SugerenciaCupo sugerencia, SugerenciaPorComercial sugerenciaPorComercial)
        {
            var resultado = new CupoResult();

            var cupo = new Cupo
            {
                ProveedorId = sugerencia.Negocio != null && sugerencia.Negocio.ProveedorComisionistaId != null ? sugerencia.Negocio.ProveedorComisionistaId.Value : sugerencia.ProveedorId.Value,
                MaterialId = sugerencia.MaterialId,
                FechaIngreso = detalle.Fecha,
                ZonaCupoId = sugerencia.ZonaCupoId.Value,
                ComercialId = sugerencia.ComercialId,
                Calidad = sugerencia.StandardDeCalidad,
                Fason = sugerencia.TipoNegocioId == 4,
                Destinatario = sugerencia.Destinatario,
                FechaGeneracion = DateTime.Now,
                Observaciones = null,
                CupoSap = "",
                FleteProcedencia = false,
                EstadoCupoId = 1,
                CupoStop = null,
                CreacionStop = "",
                ErrorStop = "",
                NegocioId = sugerencia.NegocioId,
                ConfiguracionEspacioDinamicoId = sugerencia.ConfiguracionEspacioDinamicoId,
                TipoNegocioId = sugerencia.TipoNegocioId,
                CentroId = sugerencia.CentroId
            };

            var cuposGenerados = detalle.Cantidad > sugerencia.CantidadDeCupos ? sugerencia.CantidadDeCupos : detalle.Cantidad;
            cuposGenerados = cuposGenerados > sugerenciaPorComercial.Total ? sugerenciaPorComercial.Total : cuposGenerados;
            //si tengo al menos un cupo normal para crear...
            if (detalle.Cantidad > 0)
            {
                if (cuposGenerados > 0)
                {
                    var result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cuposGenerados, Fecha = detalle.Fecha } });
                    resultado.Errores.AddRange(result.Errores);
                    resultado.ListaCupos.AddRange(result.ListaCupos);
                    detalle.Cantidad -= cuposGenerados;
                }
            }

            //si tengo al menos un cupo flete proc para crear...
            if (detalle.CantidadFleteProcedencia > 0 && sugerencia.CantidadDeCupos > 0)
            {
                cuposGenerados = detalle.CantidadFleteProcedencia > sugerencia.CantidadDeCupos ? sugerencia.CantidadDeCupos : detalle.CantidadFleteProcedencia;
                cuposGenerados = cuposGenerados > sugerenciaPorComercial.Total ? sugerenciaPorComercial.Total : cuposGenerados;

                if (cuposGenerados > 0)
                {
                    cupo.FleteProcedencia = true;
                    CupoResult result2 = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = cuposGenerados, Fecha = detalle.Fecha } });
                    resultado.Errores.AddRange(result2.Errores);
                    for (int i = 0; i < result2.ListaCupos.Count; i++)
                    {
                        result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
                    }
                    resultado.ListaCupos.AddRange(result2.ListaCupos);
                    detalle.CantidadFleteProcedencia -= cuposGenerados;
                }
            }
            return resultado;
        }

        public List<SugerenciaCupo> SugerenciasParaAceptar(int proveedorId, int comercialId, string centro, int materialId, DateTime? fecha)
        {
            return repositorio.Listar<SugerenciaCupo>(
                                         x => (x.FechaSugerida == fecha || fecha == null) && x.ProveedorId == proveedorId
                                            && x.ComercialId == comercialId
                                            && x.MaterialId == materialId && x.Centro.CodigoSap == centro && x.Aceptado == null, 0, "Puntuacion", DirOrden.Desc);
        }

        public SugerenciaPorComercial ObtenerSugerenciaPorComercial(DateTime fecha, int comercialId, int materialId, string centro)
        {
            var sugerenciaPorcomercial = repositorio.Obtener<SugerenciaPorComercial>(
                           x => x.Fecha == fecha && x.ComercialId == comercialId
                              && x.MaterialId == materialId && x.Centro.CodigoSap == centro);
            if (sugerenciaPorcomercial == null)
            {
                var centroObj = repositorio.Obtener<Centro>(x => x.CodigoSap == centro);
                sugerenciaPorcomercial = new SugerenciaPorComercial
                {
                    CentroId = centroObj.Id,
                    ComercialId = comercialId,
                    MaterialId = materialId,
                    Fecha = fecha,
                    Total = 0
                };
                repositorio.Agregar(sugerenciaPorcomercial);
            }

            return sugerenciaPorcomercial;

        }

        public CupoResult ConfirmarSugerencia(List<ConfirmacionSugerenciaCupoDto> datosTablaPorProveedor, List<DiaCupo> devoluciones, int materialId, string centroId, int comercialId)
        {
            CupoResult resultado = new CupoResult();
            var mensajes = new List<MensajeCupoDto>();
            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            if (validarSiHayCierre)
            {
                resultado.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            var centro = repositorio.Obtener<Centro, int>(x => x.CodigoSap == centroId, x => x.Id);
            var proveedores = repositorio.Listar<Proveedor>();
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            var hoy = DateTime.Now.Date;
            try
            {
                if (datosTablaPorProveedor != null && datosTablaPorProveedor.Count > 0)
                {
                    foreach (var p in datosTablaPorProveedor)
                    {
                        var detalles = p.Detalles != null ? p.Detalles.Where(x => x.Cantidad > 0 || x.CantidadFleteProcedencia > 0).ToList() : null;
                        if (detalles != null && detalles.Count > 0)
                        {
                            foreach (var f in detalles)
                            {
                                var cantidadIngresada = f.Cantidad + f.CantidadFleteProcedencia;
                                var sugerenciasParaAceptar = SugerenciasParaAceptar(p.ProveedorId, p.ComercialId, centroId, materialId, f.Fecha);
                                sugerenciasParaAceptar = sugerenciasParaAceptar.Count <= 0 || sugerenciasParaAceptar.Sum(x => x.CantidadDeCupos) < cantidadIngresada ? SugerenciasParaAceptar(p.ProveedorId, p.ComercialId, centroId, materialId, null) : sugerenciasParaAceptar;

                                var sugerenciaPorComercial = ObtenerSugerenciaPorComercial(f.Fecha, p.ComercialId, materialId, centroId);

                                //Actualizo la tabla nueva


                                logger.Debug("Flete Procedencia - : " + f.CantidadFleteProcedencia);
                                logger.Debug("Cupos normales: " + f.Cantidad);
                                logger.Debug("Cantidad Ingresada: " + cantidadIngresada);

                                var cantidadSolicitudes = sugerenciaPorComercial == null ? cantidadIngresada.Value : cantidadIngresada.Value - sugerenciaPorComercial.Total;
                                if (sugerenciaPorComercial != null && cantidadIngresada > sugerenciaPorComercial.Total)
                                {
                                    cantidadIngresada = sugerenciaPorComercial.Total;
                                }
                                if (sugerenciaPorComercial == null)
                                {
                                    cantidadIngresada = 0;
                                }

                                if (sugerenciasParaAceptar.Count > 0)
                                {
                                    foreach (var s in sugerenciasParaAceptar)
                                    {

                                        var razonSocial = proveedores.Where(x => x.ProveedorId == p.ProveedorId).First().RazonSocial;
                                        if (cantidadIngresada > 0)
                                        {
                                            //Acepto o resto las sugerencias, en base a las sugerencias creo los cupos
                                            var aceptarSugerencia = cantidadIngresada >= s.CantidadDeCupos;
                                            //var datos = new List<string>();
                                            if (aceptarSugerencia)
                                            {
                                                cantidadIngresada -= sugerenciaPorComercial.Total > s.CantidadDeCupos ? s.CantidadDeCupos : sugerenciaPorComercial.Total;

                                                var original = sugerenciaPorComercial.Total > s.CantidadDeCupos ? s.CantidadDeCupos : sugerenciaPorComercial.Total;
                                                //resultado.ListaCupos.AddRange(datos);
                                                var res = CrearCupos(f, s, sugerenciaPorComercial);
                                                if (original == res.ListaCupos.Count())
                                                {
                                                    s.Aceptado = true;
                                                    s.CantidadDeCupos = original;
                                                    s.FechaSugerida = f.Fecha;
                                                    logger.Debug("Se aceptó la totalidad de la sugerencia: " + original);

                                                    //ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Cupos", null, res.ListaCupos, null, mensajes);
                                                }
                                                else
                                                {
                                                    s.CantidadDeCupos -= res.ListaCupos.Count();
                                                    //sugerenciaPorComercial.Total -= res.ListaCupos.Count();
                                                    logger.Debug("Se aceptó la totalidad de la sugerencia: " + original + " error al generar cupos, pudo sacar: " + res.ListaCupos.Count());
                                                    var nuevaSugerencia = CopiarSugrencia(f.Fecha, s, res.ListaCupos.Count());
                                                    repositorio.Agregar(nuevaSugerencia);
                                                    s.Aceptado = null;
                                                    //ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Cupos", null, res.ListaCupos, null, mensajes);
                                                }
                                                resultado.Errores.AddRange(res.Errores);
                                                if (res.HayErrores)
                                                {
                                                    ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Error", res.Errores, null, null, mensajes);
                                                }

                                                resultado.ListaCupos.AddRange(res.ListaCupos);
                                                sugerenciaPorComercial.Total -= res.ListaCupos.Count();
                                                ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Cupos", null, res.ListaCupos, null, mensajes);

                                            }
                                            else
                                            {

                                                cantidadIngresada = 0;
                                                //datos.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
                                                //resultado.ListaCupos.AddRange(datos);
                                                var res = CrearCupos(f, s, sugerenciaPorComercial);
                                                if (res.HayErrores)
                                                {
                                                    ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Error", res.Errores, null, null, mensajes);
                                                }
                                                resultado.Errores.AddRange(res.Errores);
                                                resultado.ListaCupos.AddRange(res.ListaCupos);
                                                var nuevaSugerencia = CopiarSugrencia(f.Fecha, s, res.ListaCupos.Count());
                                                s.CantidadDeCupos -= res.ListaCupos.Count();
                                                repositorio.Agregar(nuevaSugerencia);
                                                sugerenciaPorComercial.Total -= res.ListaCupos.Count();
                                                ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Cupos", null, res.ListaCupos, null, mensajes);


                                            }
                                        }
                                    }
                                }
                                //Creo solicitudes si se pasa del dia
                                if (cantidadSolicitudes > 0)
                                {
                                    var configuracion = repositorio.Obtener<ConfiguracionCupo>(x => x.Centro.CodigoSap == centroId && x.Fecha == f.Fecha && x.MaterialId == materialId);
                                    var razonSocial = proveedores.Where(x => x.ProveedorId == p.ProveedorId).First().RazonSocial;
                                    var errores = new List<ErrorMessage>();
                                    if (configuracion != null)
                                    {
                                        var cantidadCuposGenerados = (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaIngreso) == f.Fecha && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9)).Count;

                                        if (configuracion.LimiteCupo < (cantidadIngresada + cantidadCuposGenerados))
                                        {
                                            var error = new ErrorMessage() { Message = "No hay límite disponible" };
                                            errores.Add(error);
                                            ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Error", errores, null, null, mensajes);
                                            resultado.Error("Error", p.ProveedorDesc + "<br/> <hr /> Para la fecha: " + f.Fecha.ToString("dd/MM/yyyy") + " no hay límite disponible");
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        var error = new ErrorMessage() { Message = "No hay límite disponible" };
                                        errores.Add(error);
                                        ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Error", errores, null, null, mensajes);
                                        resultado.Error("Error", p.ProveedorDesc + "<br/> <hr /> Para la fecha: " + f.Fecha.ToString("dd/MM/yyyy") + " no hay límite disponible");
                                        continue;

                                    }

                                    var solicitudes = new List<string>();
                                    var grupoDeCompras = comerciales.Where(x => x.ComercialId == p.ComercialId).First().GrupoDeCompras.Descripcion;
                                    var autorizacion = new AdministracionCupo()
                                    {
                                        ComercialCreadorId = comercialCreador,
                                        CantidadCupo = f.Cantidad.Value,
                                        ComercialId = p.ComercialId,
                                        Fecha = f.Fecha,
                                        ProveedorId = p.ProveedorId,
                                        CantidadFleteProcedencia = f.CantidadFleteProcedencia.Value,
                                        EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                                        MaterialId = materialId,
                                        ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                                        CentroId = centro,
                                        Excedente = true,
                                        TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                                    };
                                    logger.Debug("Se creo una solicitud: " + cantidadSolicitudes);

                                    repositorio.Agregar(autorizacion);
                                    var listaSolicitudes = new List<string>();
                                    solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr /> " + razonSocial : "");
                                    solicitudes.Add(p.ProveedorDesc + "<br/>Para la fecha: " + f.Fecha.ToString("dd/MM/yyyy") + " se generó una solicitud: <br />Cupo normales:" + autorizacion.CantidadCupo + "<br /> Cupo con flete: " + autorizacion.CantidadFleteProcedencia);
                                    listaSolicitudes.Add((f.CantidadFleteProcedencia ?? 0) + " - flete");
                                    listaSolicitudes.Add((f.Cantidad ?? 0) + " - normal");
                                    ArmarMensajeCupo(p.ProveedorId, f.Fecha, razonSocial, "Solicitudes", null, null, listaSolicitudes, mensajes);
                                    //resultado.ListaCupos.AddRange(solicitudes);


                                }

                            }
                        }
                        repositorio.GuardarCambios();
                    }

                }
                if (devoluciones != null)
                {
                    foreach (var d in devoluciones)
                    {
                        var devueltos = ObtenerSugerenciaPorComercial(d.Fecha, comercialId, materialId, centroId);
                        if (devueltos == null)
                        {
                            resultado.Error("Devolucion", "<hr /> Para la fecha: " + d.Fecha.ToString("dd/MM/yyyy") + " la cantidad ingresada es incorrecta");
                            continue;
                        }
                        if (d.CantidadCuposDevueltos != 0 && devueltos != null && devueltos.Total > 0)
                        {
                            var solicitudes = new List<string>();
                            if (d.CantidadCuposDevueltos > devueltos.Total)
                            {
                                resultado.Error("Devolucion", "<hr /> Para la fecha: " + d.Fecha.ToString("dd/MM/yyyy") + " la cantidad ingresada es incorrecta");
                                continue;
                            }
                            var grupoDeCompras = comerciales.Where(x => x.ComercialId == comercialId).First().GrupoDeCompras.Descripcion;
                            var autorizacion = new AdministracionCupo()
                            {
                                ComercialCreadorId = comercialCreador,
                                CantidadCupo = d.CantidadCuposDevueltos,
                                ComercialId = comercialId,
                                Fecha = d.Fecha,
                                EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo,
                                MaterialId = materialId,
                                ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                                CentroId = centro,
                                Excedente = false,
                                TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                            };
                            logger.Debug("Se devolvieron: " + d.CantidadCuposDevueltos);

                            repositorio.Agregar(autorizacion);
                            solicitudes.Add("<hr /> Para la fecha: " + d.Fecha.ToString("dd/MM/yyyy") + " se devolvieron: " + autorizacion.CantidadCupo + (autorizacion.CantidadCupo > 1 ? " cupos" : " cupo"));
                            resultado.ListaCupos.AddRange(solicitudes);

                            if (d.CantidadCuposDevueltos > devueltos.Total)
                            {
                                devueltos.Total = 0;
                            }
                            else
                            {
                                devueltos.Total -= d.CantidadCuposDevueltos;
                            }

                        }
                    }
                }
                DevolverSugerenciasCuandoNoHayaNegocioAsociado(materialId, centroId, comercialId, zonaCupo, comerciales, centro);
                CrearJsonMensajes(comercialId, materialId, centro, mensajes);
                repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error(e);
                resultado.Errores.Add(new ErrorMessage(400, e.Message));
                return resultado;
            }
        }

        private void CrearJsonMensajes(int comercialId, int materialId, int centroId, List<MensajeCupoDto> mensajes)
        {
            string jsonContrato = JsonConvert.SerializeObject(mensajes, new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.Objects
            });
            var msj = new MensajeCupo() { ComercialId = comercialId, MaterialId = materialId, CentroId = centroId, Datos = jsonContrato, Fecha = DateTime.Now };
            repositorio.Agregar(msj);

        }

        private static void ArmarMensajeCupo(int proveedorId, DateTime fecha, string razonSocial, string descripcion, List<ErrorMessage> errores, List<string> cupos, List<string> solicitudes, List<MensajeCupoDto> mensajes)
        {
            var mensaje = new MensajeCupoDto()
            {
                Descripcion = descripcion,
                ProveedorId = proveedorId,
                Proveedor = razonSocial,
                Solicitudes = solicitudes ?? new List<string> { "" },
                Cupos = cupos ?? new List<string> { "" },
                Fecha = fecha,
                Error = errores ?? new List<ErrorMessage> { new ErrorMessage { Message = "" } }
            };
            mensajes.Add(mensaje);
        }

        private void DevolverSugerenciasCuandoNoHayaNegocioAsociado(int materialId, string centroId, int comercialId, List<ZonaCupo> zonaCupo, List<Comercial> comerciales, int centro)
        {
            var lista = ObtenerSugerenciasAgrupadasPorProveedor(comercialId, materialId, centroId);
            var sugerenciaPorComercial = ObtenerSugerenciaPorComercialFecha(comercialId, materialId, centroId);
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            if (sugerenciaPorComercial != null && sugerenciaPorComercial.Count() > 0 && lista != null && lista.Count() > 0)
            {
                foreach (var s in sugerenciaPorComercial)
                {
                    foreach (var item in lista)
                    {
                        if (s.Fecha == item.FechaSugerida)
                        {
                            if (lista.Where(x => x.FechaSugerida == s.Fecha).Sum(x => x.CantidadDeCupos) <= 0)
                            {
                                var zona = comerciales.Where(x => x.ComercialId == comercialId).First().GrupoDeCompras.Descripcion;
                                var a = new AdministracionCupo()
                                {
                                    ComercialCreadorId = comercialCreador,
                                    CantidadCupo = s.Total,
                                    ComercialId = comercialId,
                                    Fecha = item.FechaSugerida,
                                    EstadoId = (int)EnumEstadoAdministracionCupo.EstadoAceptadoAdministracionCupo,
                                    MaterialId = materialId,
                                    ZonaId = zonaCupo.Where(x => x.Descripcion == zona).First().Id,
                                    CentroId = centro,
                                    Excedente = false,
                                    TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                                };
                                repositorio.Agregar(a);
                                var sugerenciaDia = repositorio.Obtener<SugerenciaPorComercial>(s.Id);
                                sugerenciaDia.Total = 0;
                            }
                        }
                    }
                }
            }
        }

        private SugerenciaCupo CopiarSugrencia(DateTime fecha, SugerenciaCupo s, int cantidad)
        {
            var nuevaSugerencia = CopiarEntidad.ShallowCopyEntity(s);
            nuevaSugerencia.CantidadDeCupos = cantidad;
            nuevaSugerencia.MonedaId = !String.IsNullOrWhiteSpace(s.MonedaId) ? s.MonedaId : null;
            nuevaSugerencia.Aceptado = true;
            nuevaSugerencia.FechaSugerida = fecha;
            repositorio.Agregar(nuevaSugerencia);
            return nuevaSugerencia;
        }

        public List<ConfiguracionCupoDto> TraerTodaConfiguracionCupoPorDia(int zona, int material, int centro, DateTime hoy)
        {
            var actual = hoy.Date;
            var cantidadCuposGenerados = (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaIngreso) == actual && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9)).Count;

            var limitePorZona = repositorio.Listar<LimiteCupo, ConfiguracionCupoDto>(x => new ConfiguracionCupoDto
            {
                Id = x.Id,
                Fecha = x.ConfiguracionCupo.Fecha,
                MaterialId = x.ConfiguracionCupo.MaterialId,
                CentroId = x.ConfiguracionCupo.CentroId,
                LimiteCupo = x.CantidadCupo,
            }, x => DbFunctions.TruncateTime(x.ConfiguracionCupo.Fecha) == hoy && x.ZonaCupoId == zona && x.ConfiguracionCupo.CentroId == centro && x.ConfiguracionCupo.MaterialId == material && !x.ConfiguracionCupo.CierreCupera);

            if (limitePorZona.Count() == 0)
            {
                var limitePorCantidadCupo = repositorio.Listar<ConfiguracionCupo, ConfiguracionCupoDto>(x => new ConfiguracionCupoDto
                {
                    Id = x.Id,
                    Fecha = x.Fecha,
                    MaterialId = x.MaterialId,
                    CentroId = x.CentroId,
                    LimiteCupo = x.LimiteCupo
                }, x => DbFunctions.TruncateTime(x.Fecha) == actual && x.MaterialId == material && x.CentroId == centro /* && (x.LimiteCupo - cantidadCuposGenerados) >= 0*/ && !x.CierreCupera);

                return limitePorCantidadCupo;
            }
            return limitePorZona;
        }

        public List<EstablecimientoStockDto> TraerEstablecimientos(string proveedor)
        {
            var cosecha = repositorio.Obtener<Material, string>(x => x.MaterialId == 3, x => x.Campaña.Descripcion);
            try
            {
                var establecimiento = servicioScato.ListarEstablecimientos(proveedor, cosecha);
                return establecimiento;
            }
            catch (Exception e)
            {
                logger.Error("Error al obtener establecimientos desde scato. Proveedor: " + proveedor + ", cosecha: " + cosecha, e);
                return new List<EstablecimientoStockDto>();
            }

        }

        public void AnulacionMasiva(List<int> equipo, string comercialId, List<int> ids, string path)
        {
            try
            {
                var momento = DateTime.Now;
                //var ids = cuposResult.Data.Cast<CupoDto>().Select(x => x.Id);
                if (ids != null)
                {
                    ids = ids.ToList();
                    var result = new Resultado();
                    var errorStop = new Resultado();
                    var context = new DataAgroDbContext();
                    var repo = new RepositorioEF(context);
                    var logmanager = new LogDataAgroManager(repo, logger);
                    var eliminarcupoSap = new EliminarCupoAgent(logger, repo);
                    var cupos = repo.Listar<Cupo>(x => ids.Contains(x.Id));
                    var datosConfiguracion = repo.Obtener<Configuracion>(1);
                    var mail = new MailManager(logger, repo);
                    var listaCuposOk = new List<CupoDto>();
                    var listaCuposError = new List<CupoDto>();

                    foreach (var c in cupos)
                    {
                        try
                        {
                            if (c.Centro.NoPropio)
                            {
                                if (c.EstadoCupoId == 1)
                                {
                                    var noPropio = repo.Obtener<CupoNoPropio>(a => a.Codigo == c.CupoSap);
                                    noPropio.CupoId = null;
                                    c.EstadoCupoId = 4;
                                    repo.GuardarCambios();
                                    logmanager.LogCambiosDataAgro(ObtenerCupo(c.Id, repo), TipoAccionLogDataAgro.Eliminar);
                                    var cuposOk = new CupoDto
                                    {
                                        CupoSap = c.CupoSap,
                                        Proveedor = c.Proveedor.RazonSocial,
                                        Material = c.Material.Descripcion,
                                        ZonaCupo = c.ZonaCupo.Descripcion,
                                        ComercialId = c.ComercialId,
                                        ProveedorId = c.ProveedorId
                                    };
                                    listaCuposOk.Add(cuposOk);
                                }
                            }
                            else
                            {
                                var cliente = new ClienteStopAgent(logger, repo, () => { return this; }, logmanager);
                                logger.Debug($"El cupo: {c.CupoSap} se esta anulando en STOP: " + DateTime.Now);
                                errorStop = AnularCupoStop(c, datosConfiguracion, cliente);
                                logger.Debug($"Fin STOP: {c.CupoSap} : " + DateTime.Now);
                                if (errorStop.HayError)
                                {
                                    var cupoError = new CupoDto
                                    {
                                        CupoSap = c.CupoSap,
                                        Proveedor = c.Proveedor.RazonSocial,
                                        Material = c.Material.Descripcion,
                                        MensajeError = "Error al anular en STOP: " + errorStop.ListaErrores.First().Message,
                                        ZonaCupo = c.ZonaCupo.Descripcion
                                    };
                                    listaCuposError.Add(cupoError);
                                }
                                else
                                {
                                    c.EstadoCupoId = 4;
                                    logger.Debug($"El cupo: {c.CupoSap} se esta anulando en SAP " + DateTime.Now);
                                    var resultado = eliminarcupoSap.Eliminar(c.CupoSap, comercialId);
                                    logger.Debug($"Fin SAP: {c.CupoSap} : " + DateTime.Now);
                                    if (resultado != "OK")
                                    {
                                        if (c.Centro.Acopio)
                                        {
                                            c.EstadoCupoId = 1;
                                        }
                                        var cupoError = new CupoDto
                                        {
                                            CupoSap = c.CupoSap,
                                            Proveedor = c.Proveedor.RazonSocial,
                                            Material = c.Material.Descripcion,
                                            MensajeError = "Anulado Ok en STOP. Error al anular en SAP: " + resultado,
                                            ZonaCupo = c.ZonaCupo.Descripcion
                                        };
                                        listaCuposError.Add(cupoError);
                                    }

                                }

                                if (!errorStop.HayError)
                                {
                                    logmanager.LogCambiosDataAgro(ObtenerCupo(c.Id, repo), TipoAccionLogDataAgro.Eliminar);

                                    var cuposOk = new CupoDto
                                    {
                                        CupoSap = c.CupoSap,
                                        Proveedor = c.Proveedor.RazonSocial,
                                        Material = c.Material.Descripcion,
                                        ZonaCupo = c.ZonaCupo.Descripcion,
                                        ComercialId = c.ComercialId,
                                        ProveedorId = c.ProveedorId
                                    };
                                    listaCuposOk.Add(cuposOk);
                                }
                            }


                        }
                        catch (Exception e)
                        {
                            logger.Error("Error al anular el cupo " + c.CupoSap, e);
                            var cuposOk = new CupoDto
                            {
                                CupoSap = c.CupoSap,
                                Proveedor = "",
                                Material = "",
                                ZonaCupo = "",
                                ComercialId = 0,
                                ProveedorId = 0,
                                MensajeError = "Error al anular",
                            };
                            listaCuposOk.Add(cuposOk);
                        }
                    }
                    repo.GuardarCambios();
                    EnviarMailAnulacionCupo(result, comercialId, momento, repo, mail, path, listaCuposError, listaCuposOk);
                    EnviarMailCreadorAnulacionCupo(listaCuposOk, comercialId);
                    EnviarMailProveedorAnulacionCupo(listaCuposOk);
                }
            }
            catch (Exception e)
            {
                logger.Error("Error al anular los cupos", e);
            }


        }

        private void EnviarMailAnulacionCupo(Resultado resultado, string active, DateTime momento, RepositorioEF repo, MailManager mail, string path, List<CupoDto> conError, List<CupoDto> ok)
        {
            try
            {
                var lista = new List<string>();
                var comercial = new List<string>();
                var c = repo.Obtener<Comercial>(x => x.IdActiveDirectory == active);
                var copia = repo.Listar<Comercial>(x => x.IdActiveDirectory == active &&
                x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VisualizarAnulacionEnReporteCupo)));
                lista.AddRange(copia.Select(x => x.IdActiveDirectory));
                comercial.Add(c.IdActiveDirectory);
                if (comercial.Count <= 0)
                {
                    return;
                }
                var alterView = CuerpoMailAnulacionCupo(path, resultado, momento, conError, ok);
                mail.EnviarMail(comercial, "Anulación de cupos", "", lista, alterView);

            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailAnulacionCupo", e.Message);
            }
        }

        private AlternateView CuerpoMailAnulacionCupo(String filePath, Resultado resultado, DateTime momento, List<CupoDto> conError, List<CupoDto> ok)
        {
            //var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla el resultado de la Anulación Masiva de cupos solicitada en el día " + momento.ToString("dd-MM-yyyy hh:mm") + "hs.: <br />";

            if (ok.Count > 0)
            {
                htmlBody += "<h4> Cupos Anulados correctamente: </h4><br />";
                htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
                htmlBody += "<tr>" +
                        th + "Cupo" + "</td>" +
                        th + "Grano" + "</td>" +
                        th + "Vendedor/Corredor" + "</td>" +
                        th + "Zona" + "</td>" +
                        "</tr>";

                foreach (var c in ok)
                {
                    string style1 = "";
                    string style2 = "";
                    if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    else
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    logger.Debug($"cupo numero: {c.Id}");
                    linea += 1;
                    if (linea % 2 == 0)
                    {
                        htmlBody += "<tr>" +
                            "<td " + style1 + c.CupoSap + "</td>" +
                            "<td " + style1 + c.Material + "</td>" +
                            "<td " + style1 + c.Proveedor + "</td>" +
                            "<td " + style1 + c.ZonaCupo + "</td> </tr> ";
                    }
                    else
                    {
                        htmlBody += "<tr>" +
                             "<td " + style2 + c.CupoSap + "</td>" +
                             "<td " + style2 + c.Material + "</td>" +
                             "<td " + style2 + c.Proveedor + "</td>" +
                             "<td " + style2 + c.ZonaCupo + "</td> </tr> ";
                    }
                }
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table><br />";
            if (conError.Count > 0)
            {
                htmlBody += "<h4> Cupos con error al anular: </h4><br />";
                htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
                htmlBody += "<tr>" +
                        th + "Cupo" + "</td>" +
                        th + "Grano" + "</td>" +
                        th + "Vendedor/Corredor" + "</td>" +
                        th + "Zona" + "</td>" +
                        th + "Mensaje de error" + "</td>" +
                        "</tr>";
                foreach (var c in conError)
                {
                    string style1 = "";
                    string style2 = "";
                    if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    else
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    logger.Debug($"cupo numero: {c.Id}");
                    linea += 1;
                    if (linea % 2 == 0)
                    {
                        htmlBody += "<tr>" +
                            "<td " + style1 + c.CupoSap + "</td>" +
                            "<td " + style1 + c.Material + "</td>" +
                            "<td " + style1 + c.Proveedor + "</td>" +
                            "<td " + style1 + c.ZonaCupo + "</td>" +
                            "<td " + style1 + c.MensajeError + "</td> </tr> ";
                    }
                    else
                    {
                        htmlBody += "<tr>" +
                             "<td " + style2 + c.CupoSap + "</td>" +
                             "<td " + style2 + c.Material + "</td>" +
                             "<td " + style2 + c.Proveedor + "</td>" +
                             "<td " + style2 + c.ZonaCupo + "</td>" +
                             "<td " + style2 + c.MensajeError + "</td> </tr> ";
                    }
                }
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private void EnviarMailProveedorAnulacionCupo(List<CupoDto> cupos)
        {
            try
            {
                if (cupos.Count > 0)
                {
                    foreach (var cuposPorProveedor in cupos.GroupBy(x => x.ProveedorId))
                    {
                        var id = cuposPorProveedor.Key;
                        var proveedorContacto = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == id && x.Cupo == true && x.Email1 != null);
                        if (proveedorContacto.Count == 0)
                        {
                            return;
                        }
                        var lista = proveedorContacto.ToList();
                        var path = httpContextManager.ObtenerPathLogoMail();
                        var cupo = cuposPorProveedor.ToList();
                        var alterView = CuerpoMailProveedorAnulacionCupo(path, cupo);
                        mailManager.EnviarMail(null, lista, "Anulación de cupos", "", null, alterView);
                    }
                }
                else return;
            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailAnulacionCupo", e.Message);
            }
        }

        private void EnviarMailComercialAnulacionCupo(string active, List<CupoDto> cupos)
        {
            try
            {
                var comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == active);
                if (comercial != null)
                {
                    var lista = new List<string> { comercial.IdActiveDirectory };
                    var path = httpContextManager.ObtenerPathLogoMail();
                    var alterView = CuerpoMailProveedorAnulacionCupo(path, cupos);
                    mailManager.EnviarMail(lista, "Anulacion de cupo", "", null, alterView, null, null);
                }
                else
                {
                    return;
                }

            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailAnulacionCupo", e.Message);
            }
        }

        private void EnviarMailCreadorAnulacionCupo(List<CupoDto> cupos, string active)
        {
            try
            {
                foreach (var cuposPorComercialCreador in cupos.GroupBy(x => x.ComercialId))
                {
                    var id = cuposPorComercialCreador.Key;
                    var comercialContacto = repositorio.Listar<Comercial>(x => x.ComercialId == id && x.IdActiveDirectory != active);
                    if (comercialContacto.Count == 0)
                    {
                        continue;
                    }
                    var lista = comercialContacto.Select(x => x.IdActiveDirectory).ToList();
                    var path = httpContextManager.ObtenerPathLogoMail();
                    var cupo = cuposPorComercialCreador.ToList();
                    var alterView = CuerpoMailProveedorAnulacionCupo(path, cupo);
                    mailManager.EnviarMail(lista, "Anulacion de cupo", "", null, alterView, null, null);
                }
            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailAnulacionCupo", e.Message);
            }
        }

        private AlternateView CuerpoMailProveedorAnulacionCupo(String filePath, List<CupoDto> cupos)
        {
            //var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla el resultado de la Anulación de cupos <br />";

            if (cupos.Count > 0)
            {

                htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
                htmlBody += "<tr>" +
                        th + "Cupo" + "</td>" +
                        th + "Grano" + "</td>" +
                        th + "Vendedor/Corredor" + "</td>" +
                        th + "Zona" + "</td>" +
                        "</tr>";

                foreach (var c in cupos)
                {
                    string style1 = "";
                    string style2 = "";
                    if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    else
                    {
                        style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                        style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                    }
                    logger.Debug($"cupo numero: {c.Id}");
                    linea += 1;
                    if (linea % 2 == 0)
                    {
                        htmlBody += "<tr>" +
                            "<td " + style1 + c.CupoSap + "</td>" +
                            "<td " + style1 + c.Material + "</td>" +
                            "<td " + style1 + c.Proveedor + "</td>" +
                            "<td " + style1 + c.ZonaCupo + "</td> </tr> ";
                    }
                    else
                    {
                        htmlBody += "<tr>" +
                             "<td " + style2 + c.CupoSap + "</td>" +
                             "<td " + style2 + c.Material + "</td>" +
                             "<td " + style2 + c.Proveedor + "</td>" +
                             "<td " + style2 + c.ZonaCupo + "</td> </tr> ";
                    }
                }
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<CierreCupera> DevolverTodoCierreCupera()
        {
            return repositorio.Listar<CierreCupera>().ToList();
        }
        public void EnviarMailSugerenciasPendientesPorComercial()
        {
            var comerciales = repositorio.Listar<Comercial>();
            foreach (var comercial in comerciales)
            {
                if (comercial != null && repositorio.ObtenerConsultaEscalar(new TraerSugerenciasPendientes(comercial.ComercialId)))
                {
                    var lista = new List<string> { comercial.IdActiveDirectory };
                    var path = httpContextManager.ObtenerPathLogoMail();
                    var alterView = CuerpoMailSugerenciasPendientesPorComercial(path, comercial.ComercialId);
                    mailManager.EnviarMail(lista, "Sugerencias Pendientes", "", null, alterView, null, null);
                }
                else
                {
                    continue;
                }
            }
        }

        private AlternateView CuerpoMailSugerenciasPendientesPorComercial(String filePath, int comercialId)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla las Sugerencias Pendientes a confirmar o devolver, para realizar una de estas acciones presione <a href='" + ConfigurationManager.AppSettings["UrlBaseDataAgro"] + "'>Aquí</a><br />";

            var materiales = repositorio.Listar<Material>().Distinct();
            foreach (var material in materiales)
            {
                var lista = ObtenerSugerenciaCupoAgrupadasPorProveedor(comercialId, material.MaterialId, "1029");
                var sugerenciaPorComercial = ObtenerSugerenciaPorComercialFecha(comercialId, material.MaterialId, "1029");
                var fechas = FechasComprendidas(material.MaterialId);

                if (lista.Count > 0 && sugerenciaPorComercial.Count > 0)
                {
                    htmlBody += "<h4>" + material.Descripcion + "</h4><br />";
                    htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
                    htmlBody += "<tr>" +
                            th + "Proveedor" + "</td>";
                    foreach (var f in fechas)
                    {
                        var cant = sugerenciaPorComercial.Where(x => x.Fecha == f).Sum(x => x.Total);
                        htmlBody += th + "Fecha: " + f.ToString("dd/MM/yyyy") + "</td>";

                    }
                    htmlBody += "</tr>";
                    foreach (var p in lista.GroupBy(x => x.ProveedorDesc))
                    {
                        string style1 = "";
                        string style2 = "";
                        if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                        {
                            style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                            style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                        }
                        else
                        {
                            style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                            style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                        }
                        linea += 1;
                        if (linea % 2 == 0)
                        {
                            htmlBody += "<tr>" +
                                "<td " + style1 + p.Key + "</td>";
                            //"<td " + style1 +  + "</td> </tr> ";
                        }
                        else
                        {
                            htmlBody += "<tr>" +
                                 "<td " + style2 + p.Key + "</td>";
                            //"<td " + style2 + cant + "</td> </tr> ";
                        }
                        foreach (var f in fechas)
                        {
                            var cant = p.Where(x => x.FechaSugerida == f && x.ProveedorDesc == p.Key).Sum(x => x.CantidadDeCupos);
                            if (linea % 2 == 0)
                            {
                                htmlBody += /*"<tr>" +*/
                                    "<td " + style1 + cant + "</td>";
                            }
                            else
                            {
                                htmlBody +=/* "<tr>" +*/
                                     "<td " + style2 + cant + "</td>";
                            }

                        }
                        htmlBody += "</tr>";
                    }
                }
                htmlBody += " </td></tr>";
                htmlBody += "</td></tr></table>";
            }

            htmlBody += "<br /> <br />  Saludos Cordiales" +
              " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
              @"<img src='cid:" + res.ContentId + @"'/>" +
              "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;

        }

        public CupoResult GenerarSolicitudExtraordinaria(AdministracionCupoDto solicitud)
        {
            var result = new CupoResult();
            try
            {
                var centro = repositorio.Obtener<Centro, int>(x => x.CodigoSap == solicitud.CentroId.ToString(), x => x.Id);
                var resultado = Validar(new Cupo { ProveedorId = solicitud.ProveedorId.Value, FechaIngreso = solicitud.Fecha, CentroId = centro }, 1, solicitud.Fecha);
                if (resultado.HayError)
                {
                    result.Errores = resultado.Errores;
                    return result;
                }
                var grupoDeCompras = repositorio.Listar<Comercial>(x => x.ComercialId == solicitud.ComercialId).First().GrupoDeCompras.Descripcion;
                var zona = repositorio.Listar<ZonaCupo>(x => x.Descripcion == grupoDeCompras).FirstOrDefault();
                //var configuracion = repositorio.Obtener<ConfiguracionCupo>(x => x.MaterialId == solicitud.MaterialId && x.Centro.CodigoSap == solicitud.CentroId.ToString() && x.Fecha == solicitud.Fecha.Date);

                string active = PermisosHelper.ObtenerUsuario();
                var creadorId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == active, x => x.ComercialId);

                List<ConfiguracionCupo> listaConfiguraciones = new List<ConfiguracionCupo>();
                if (solicitud.Dias == null)
                {
                    var configuracion = repositorio.Obtener<ConfiguracionCupo>(x => x.MaterialId == solicitud.MaterialId && x.Centro.CodigoSap == solicitud.CentroId.ToString() && x.Fecha == solicitud.Fecha.Date);
                    if (configuracion == null)
                    {
                        result.Error("Configuracion", "No hay cupera creada para el dia seleccionado.");
                        return result;
                    }
                    listaConfiguraciones.Add(configuracion);
                }
                else
                {
                    foreach (var itemDias in solicitud.Dias)
                    {
                        var configuracion = repositorio.Obtener<ConfiguracionCupo>(x => x.MaterialId == solicitud.MaterialId && x.Centro.CodigoSap == solicitud.CentroId.ToString() && x.Fecha == itemDias.Fecha);
                        if (configuracion == null)
                        {
                            result.Error("Configuracion", "No hay cupera creada para el dia " + itemDias.Fecha.ToString("dd/MM/yyyy"));
                            return result;
                        }
                        listaConfiguraciones.Add(configuracion);
                    }
                }

                if (string.IsNullOrEmpty(solicitud.Destinatario))
                    solicitud.Destinatario = "30715118773";


                if (zona == null)
                {
                    result.Error("Zona", "El comercial seleccionado no tiene zona cupo asignada.");
                    return result;
                }
                //if (configuracion == null)
                //{
                //    result.Error("Configuracion", "No hay cupera creada para el dia seleccionado.");
                //    return result;
                //}
                List<string> listaSolicitudesGeneradas = new List<string>();
                foreach (var itemConfiguracion in listaConfiguraciones)
                {
                    if (itemConfiguracion.LiberarCupera == true)
                    {
                        Cupo cupo = new Cupo
                        {
                            ProveedorId = solicitud.ProveedorId.Value,//---Agentecompra no tiene proveedor
                            CentroId = centro,
                            MaterialId = solicitud.MaterialId,
                            FechaIngreso = solicitud.Fecha,
                            ZonaCupoId = zona.Id,
                            ComercialId = solicitud.ComercialId,
                            Calidad = solicitud.Calidad,
                            Fason = solicitud.Fason.HasValue ? solicitud.Fason.Value : false,
                            Destinatario = solicitud.Destinatario,
                            FechaGeneracion = DateTime.Now,
                            Observaciones = null,//---
                            CupoSap = "",//---
                            FleteProcedencia = solicitud.CantidadFleteProcedencia > 0,//---
                            EstadoCupoId = 1,//---
                            CupoStop = null,//---
                            CreacionStop = "",//---
                            ErrorStop = "",//---
                            NegocioId = null,
                            ConfiguracionEspacioDinamicoId = null,
                            TipoNegocioId = 7,
                            ConDescarga = solicitud.ConDescarga
                        };
                        if (solicitud.CantidadCupo > 0)
                        {
                            if (solicitud.Dias != null)
                            {
                                result = GrabarCupo(cupo, solicitud.Dias.Where(x => x.Fecha == itemConfiguracion.Fecha).ToList());
                            }
                            else
                            {
                                result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = solicitud.CantidadCupo, Fecha = solicitud.Fecha } });
                            }
                        }
                        if (solicitud.CantidadFleteProcedencia > 0)
                        {
                            if (solicitud.Dias != null)
                            {
                                var itemD = solicitud.Dias.Where(x => x.Fecha == itemConfiguracion.Fecha).FirstOrDefault();
                                result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = itemD.Cantidad, Fecha = itemD.Fecha } });
                            }
                            else
                            {
                                result = GrabarCupo(cupo, new List<DiaCupo> { new DiaCupo { Cantidad = solicitud.CantidadFleteProcedencia, Fecha = solicitud.Fecha } });
                            }
                        }
                        //foreach(string itemCupo in result.ListaCupos)
                        //{
                        //    listaSolicitudesGeneradas.Add("Se generó el cupo " + itemCupo + " para el " + itemConfiguracion.Fecha.ToString("dd/MM/yyyy"));
                        //}

                        listaSolicitudesGeneradas.Add("Cupo(s) generado(s) para el " + itemConfiguracion.Fecha.ToString("dd/MM/yyyy"));
                        if (result.HayError)
                        {
                            foreach (var err in result.Errores)
                            {
                                listaSolicitudesGeneradas.Add(err.Message);
                            }
                        }
                        else
                        {
                            listaSolicitudesGeneradas.AddRange(result.ListaCupos);
                        }
                    }
                    else
                    {
                        solicitud.Excedente = true;
                        solicitud.EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo;
                        solicitud.CentroId = centro;
                        solicitud.ZonaId = zona.Id;
                        solicitud.FechaCreacion = DateTime.Now;
                        solicitud.Fecha = solicitud.Fecha.Date;
                        solicitud.ComercialCreadorId = creadorId;
                        solicitud.SugerenciaCupoId = null;
                        if (solicitud.Dias != null)
                        {
                            var itemD = solicitud.Dias.Where(x => x.Fecha == itemConfiguracion.Fecha).FirstOrDefault();
                            AdministracionCupo solicitudItem = new AdministracionCupo
                            {
                                Calidad = solicitud.Calidad,
                                CantidadCupo = itemD.Cantidad.HasValue ? itemD.Cantidad.Value : 0,
                                CantidadFleteProcedencia = solicitud.CantidadFleteProcedencia,
                                //Centro = solicitud.Centro,
                                CentroId = solicitud.CentroId,
                                //Comercial = solicitud.Comercial,
                                ComercialCreadorId = solicitud.ComercialCreadorId,
                                ComercialId = solicitud.ComercialId,
                                ConDescarga = solicitud.ConDescarga,
                                Destinatario = solicitud.Destinatario,
                                EstadoId = solicitud.EstadoId,
                                Excedente = solicitud.Excedente,
                                Fason = solicitud.Fason,
                                Fecha = itemD.Fecha,
                                FechaCreacion = solicitud.FechaCreacion,
                                FechaDecision = solicitud.FechaDecision,
                                Id = solicitud.Id,
                                //Material = 
                                MaterialId = solicitud.MaterialId,
                                Observacion = solicitud.Observacion,
                                ProveedorId = solicitud.ProveedorId,
                                SugerenciaCupoId = solicitud.SugerenciaCupoId,
                                TipoAdministracionCupoId = solicitud.TipoAdministracionCupoId,
                                ZonaId = solicitud.ZonaId
                            };
                            repositorio.Agregar(solicitudItem);

                        }
                        else
                        {
                            AdministracionCupo solicitudCupo = new AdministracionCupo
                            {
                                Calidad = solicitud.Calidad,
                                CantidadCupo = solicitud.CantidadCupo,
                                CantidadFleteProcedencia = solicitud.CantidadFleteProcedencia,
                                //Centro = solicitud.Centro,
                                CentroId = solicitud.CentroId,
                                //Comercial = solicitud.Comercial,
                                ComercialCreadorId = solicitud.ComercialCreadorId,
                                ComercialId = solicitud.ComercialId,
                                ConDescarga = solicitud.ConDescarga,
                                Destinatario = solicitud.Destinatario,
                                EstadoId = solicitud.EstadoId,
                                Excedente = solicitud.Excedente,
                                Fason = solicitud.Fason,
                                Fecha = solicitud.Fecha,
                                FechaCreacion = solicitud.FechaCreacion,
                                FechaDecision = solicitud.FechaDecision,
                                Id = solicitud.Id,
                                //Material = 
                                MaterialId = solicitud.MaterialId,
                                Observacion = solicitud.Observacion,
                                ProveedorId = solicitud.ProveedorId,
                                SugerenciaCupoId = solicitud.SugerenciaCupoId,
                                TipoAdministracionCupoId = solicitud.TipoAdministracionCupoId,
                                ZonaId = solicitud.ZonaId
                            };
                            repositorio.Agregar(solicitudCupo);
                        }
                        listaSolicitudesGeneradas.Add("La solicitud para el " + itemConfiguracion.Fecha.ToString("dd/MM/yyyy") + " se genero correctamente.");
                    }
                }
                result.ListaCupos.Clear();
                result.ListaCupos.AddRange(listaSolicitudesGeneradas);
                repositorio.GuardarCambios();
                return result;
            }
            catch (Exception e)
            {
                logger.Error("Error al GenerarSolicitudExtraordinaria");
                logger.Error(e);
                result.Error("", "Ha ocurrido un error al generar la solicitud.");
                return result;
            }

        }

        public List<MensajeCupoDto> MostrarDetalle(int comercialSeleccionado, string centro, int materialId)
        {
            var fecha = DateTime.Now.Date;
            var centroId = repositorio.Obtener<Centro, int>(x => x.CodigoSap == centro, x => x.Id);
            var json = repositorio.Listar<MensajeCupo>(x => x.ComercialId == comercialSeleccionado && DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(fecha) && x.CentroId == centroId && x.MaterialId == materialId).OrderByDescending(x => x.Id).FirstOrDefault();
            var mensaje = new List<MensajeCupoDto>();
            if (json != null)
            {
                mensaje = JsonConvert.DeserializeObject<List<MensajeCupoDto>>(json.Datos);
            }

            return mensaje;
        }

        public List<CupoResult> AceptarSugerenciaCupo(int idSugerencia, int cupoNormalSolicitud, int fleteSolicitud, bool porProveedor = false)
        {
            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var solicitudes = new List<string>();
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            List<CupoResult> resultado = new List<CupoResult>();
            var result = new CupoResult();
            var result2 = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            //var proveedores = repositorio.Listar<Proveedor>();

            SugerenciaCupo s = repositorio.Obtener<SugerenciaCupo>(a => idSugerencia == a.Id && a.Aceptado == null);
            if (s == null)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La sugerencia ya fue aceptada");
                resultado.Add(errorCupera);
                return resultado;
            }


            var sugerenciaPorComercial = ObtenerSugerenciaPorComercial(s.FechaSugerida, s.ComercialId, s.MaterialId, s.Centro.CodigoSap);
            var cantidadIngresada = cupoNormalSolicitud + fleteSolicitud;
            //Acepto o resto las sugerencias, en base a las sugerencias creo los cupos

            var grupoDeCompras = comerciales.Where(x => x.ComercialId == s.ComercialId).First().GrupoDeCompras.Descripcion;
            if (s.CantidadDeCupos > cantidadIngresada)//parte no confirmada
            {
                SugerenciaCupo nuevaSugerencia = ClonarSugerencia(s);
                nuevaSugerencia.CantidadDeCupos = s.CantidadDeCupos - cantidadIngresada;
                nuevaSugerencia.CantidadCupoOriginal = s.CantidadDeCupos - cantidadIngresada;
                s.CantidadCupoOriginal = cantidadIngresada;
                s.CantidadDeCupos = cantidadIngresada;
                nuevaSugerencia.Aceptado = null;
                repositorio.Agregar(nuevaSugerencia);
            }

            if (!porProveedor)
            {
                s.CantidadDeCupos = cantidadIngresada;
            }

            if (sugerenciaPorComercial.Total < cantidadIngresada)
            {
                int cantidadCupoSugerencia = s.CantidadDeCupos;
                s.CantidadDeCupos = sugerenciaPorComercial.Total;

                var solicitud = new AdministracionCupo()
                {
                    ComercialCreadorId = comercialCreador,
                    CantidadCupo = 0,
                    CantidadFleteProcedencia = 0,
                    ComercialId = s.ComercialId,
                    Fecha = s.FechaSugerida,
                    FechaCreacion = DateTime.Now,
                    EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                    MaterialId = s.MaterialId,
                    ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                    CentroId = s.CentroId,
                    Excedente = true,
                    ProveedorId = s.ProveedorId,
                    TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                };
                SugerenciaCupo nuevaSugerenciaSolicitud = ClonarSugerencia(s);
                nuevaSugerenciaSolicitud.Aceptado = null;
                nuevaSugerenciaSolicitud.CantidadDeCupos = 0;
                nuevaSugerenciaSolicitud.CantidadCupoOriginal = 0;

                //primero se generan los de flete
                if (sugerenciaPorComercial.Total < fleteSolicitud)
                {
                    var cantidadAPedir = fleteSolicitud < cantidadCupoSugerencia ? fleteSolicitud : cantidadCupoSugerencia;
                    cantidadCupoSugerencia -= cantidadAPedir;
                    //Generar solicitudes por la resta
                    solicitud.CantidadFleteProcedencia = cantidadAPedir - sugerenciaPorComercial.Total;
                    resultado.Add(new CupoResult { CuposFlete = solicitud.CantidadFleteProcedencia });
                    nuevaSugerenciaSolicitud.CantidadDeCupos = solicitud.CantidadFleteProcedencia;
                    nuevaSugerenciaSolicitud.CantidadCupoOriginal = solicitud.CantidadFleteProcedencia;

                    //Generear cupos
                    result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, sugerenciaPorComercial.Total, true, null);
                }
                else
                {
                    resultado.Add(new CupoResult { CuposFlete = fleteSolicitud });

                    //Generar cupos
                    result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, fleteSolicitud, true, null);
                }

                //despues los comunes
                if (sugerenciaPorComercial.Total < cupoNormalSolicitud)
                {
                    var cantidadAPedir = cupoNormalSolicitud < cantidadCupoSugerencia ? cupoNormalSolicitud : cantidadCupoSugerencia;
                    //Generar solicitudes por la resta
                    solicitud.CantidadCupo = cantidadAPedir - sugerenciaPorComercial.Total;
                    resultado.Add(new CupoResult { CuposNormales = solicitud.CantidadCupo });

                    nuevaSugerenciaSolicitud.CantidadDeCupos += solicitud.CantidadCupo;
                    nuevaSugerenciaSolicitud.CantidadCupoOriginal += solicitud.CantidadCupo;
                    //Generear cupos
                    result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, sugerenciaPorComercial.Total, false, null);
                }
                else
                {
                    resultado.Add(new CupoResult { CuposNormales = cupoNormalSolicitud });
                    //Generar cupos
                    result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, cupoNormalSolicitud, false, null);
                }

                if (solicitud.CantidadFleteProcedencia > 0 || solicitud.CantidadCupo > 0)
                {
                    solicitud.SugerenciaCupo = nuevaSugerenciaSolicitud;
                    repositorio.Agregar(solicitud);
                    logger.Debug(s.Proveedor.RazonSocial + "<br/>Se creo una solicitud flete: " + solicitud.CantidadFleteProcedencia + " cupo normal: " + solicitud.CantidadCupo);
                }

                //solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
                solicitudes.Add("<br/>Para la fecha: " + s.FechaSugerida.ToString("dd/MM/yyyy") + " se generó una solicitud: <br />Cupo normales:" + solicitud.CantidadCupo + "<br /> Cupo con flete: " + solicitud.CantidadFleteProcedencia);
            }
            else
            {
                int cantidadFlete = fleteSolicitud > s.CantidadDeCupos ? s.CantidadDeCupos : fleteSolicitud;
                result2 = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, cantidadFlete, true, null);//primero se generan los de flete
                resultado.Add(new CupoResult { CuposFlete = result2.ListaCupos.Count() });

                int cantidadNormal = cupoNormalSolicitud > (s.CantidadDeCupos - result2.ListaCupos.Count()) ? (s.CantidadDeCupos - result2.ListaCupos.Count()) : cupoNormalSolicitud;
                result = CrearCupoSugerenciaDetalle(s, sugerenciaPorComercial, cantidadNormal, false, null);//despues los comunes
                resultado.Add(new CupoResult { CuposNormales = result.ListaCupos.Count() });
            }
            if (!result.HayError)
            {
                s.Aceptado = true;
            }
            else
            {
                if (result.ListaCupos.Count > 0)
                {
                    s.CantidadDeCupos -= result.ListaCupos.Count;
                }
            }
            result.ListaCupos.AddRange(solicitudes);

            for (int i = 0; i < result2.ListaCupos.Count; i++)
            {
                result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
            }
            result.Errores.AddRange(result2.Errores);
            result.ListaCupos.AddRange(result2.ListaCupos);
            if (result.ListaCupos.Count > 0)
            {
                result.ListaCupos.Insert(0, s.Proveedor.RazonSocial + "<br/>");
            }
            resultado.Add(result);

            repositorio.GuardarCambios();
            return resultado;
        }

        public SugerenciaCupo ClonarSugerencia(SugerenciaCupo s)
        {
            SugerenciaCupo clon = new SugerenciaCupo
            {
                Aceptado = null,
                CantidadCupoOriginal = s.CantidadCupoOriginal,
                CantidadDeCupos = s.CantidadDeCupos,
                CDWarrant = s.CDWarrant,
                FechaSugerida = s.FechaSugerida,
                ProveedorId = s.ProveedorId,
                CentroId = s.CentroId,
                ComercialId = s.ComercialId,
                ConfiguracionEspacioDinamicoId = s.ConfiguracionEspacioDinamicoId,
                ContratoSAP = s.ContratoSAP,
                MaterialId = s.MaterialId,
                MonedaId = s.MonedaId == "     " ? null : s.MonedaId,
                MotivoRechazo = "",
                Destinatario = s.Destinatario,
                NegocioId = s.NegocioId,
                Precio = s.Precio,
                Puntuacion = s.Puntuacion,
                Puntuaciones = s.Puntuaciones,
                StandardDeCalidad = s.StandardDeCalidad,
                TipoNegocioId = s.TipoNegocioId,
                ZonaCupoId = s.ZonaCupoId,
                KgPendienteAplicar = s.KgPendienteAplicar,
                KgNegocio = s.KgNegocio

            };
            return clon;
        }

        public CupoResult RechazarSugerenciaCupo(int id, int cantidad)
        {
            var resultado = new CupoResult();
            try
            {
                var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
                var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
                var activeCreador = PermisosHelper.ObtenerUsuario();
                var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
                if (validarSiHayCierre)
                {
                    resultado.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                    return resultado;
                }

                SugerenciaCupo sugerencia = repositorio.Obtener<SugerenciaCupo>(a => id == a.Id && a.Aceptado == null);
                var comercialId = sugerencia.ComercialId;
                var sugerenciasPorComercial = repositorio.Listar<SugerenciaPorComercial>(
                           x => x.ComercialId == comercialId);


                List<AdministracionCupo> admCupos = new List<AdministracionCupo>();

                var spc = sugerenciasPorComercial.Where(a => a.MaterialId == sugerencia.MaterialId && a.Fecha == sugerencia.FechaSugerida && a.CentroId == a.CentroId).FirstOrDefault();
                if (cantidad == sugerencia.CantidadDeCupos)
                    sugerencia.Aceptado = false;
                sugerencia.CantidadDeCupos -= cantidad;
                sugerencia.MotivoRechazo = "";
                admCupos.Add(new AdministracionCupo
                {
                    ComercialCreadorId = comercialCreador,
                    CantidadCupo = cantidad,
                    CentroId = sugerencia.CentroId,
                    ComercialId = sugerencia.ComercialId,
                    Excedente = false,
                    EstadoId = 0,
                    MaterialId = sugerencia.MaterialId,
                    ProveedorId = sugerencia.ProveedorId,
                    ZonaId = sugerencia.ZonaCupoId ?? 1,
                    Fecha = sugerencia.FechaSugerida,
                    TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                });
                if (spc != null)
                {
                    spc.Total -= cantidad;
                    if (spc.Total < 0)
                        spc.Total = 0;
                }

                repositorio.AgregarTodos(admCupos);

                repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                var nuevoResultado = new CupoResult();
                nuevoResultado.Error("eliminar", e.Message);
                return nuevoResultado;
            }

        }

        public List<CupoResult> AceptarSugerenciaCupoPorProveedor(int proveedorId, int materialId, DateTime fecha, int cupoNormalSolicitud, int fleteSolicitud, int comercialId, string centroCodigo)
        {


            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var solicitudes = new List<string>();
            List<CupoResult> resultado = new List<CupoResult>();

            var result = new CupoResult();
            var result2 = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            //var proveedores = repositorio.Listar<Proveedor>();

            List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a =>
                a.ProveedorId == proveedorId
                && a.MaterialId == materialId
                && a.Aceptado == null
                && a.ComercialId == comercialId
                && a.FechaSugerida == fecha
                && a.Centro.CodigoSap == centroCodigo
                && (a.Solicitudes.All(y => y.EstadoId == 2) || a.Solicitudes.Count() == 0)
            , 0, "Puntuacion", DirOrden.Desc);
            if (sugerencias.Count == 0)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La sugerencia ya fue aceptada");
                resultado.Add(errorCupera);
                return resultado;
            }

            foreach (var item in sugerencias)
            {
                if (cupoNormalSolicitud > 0 || fleteSolicitud > 0)
                {
                    var cupoResults = AceptarSugerenciaCupo(item.Id, cupoNormalSolicitud, fleteSolicitud, true);
                    var cFletes = cupoResults.Sum(a => a.CuposFlete);
                    var cNormal = cupoResults.Sum(a => a.CuposNormales);
                    cupoNormalSolicitud -= cNormal;
                    fleteSolicitud -= cFletes;
                    foreach (var cres in cupoResults)
                    {
                        result.Errores.AddRange(cres.Errores);
                        result.ListaCupos.AddRange(cres.ListaCupos);
                    }
                }
                else
                {
                    break;
                }
            }
            resultado.Add(result);


            //var sugerenciaPorComercial = ObtenerSugerenciaPorComercial(sugerencias.First().FechaSugerida, sugerencias.First().ComercialId, sugerencias.First().MaterialId, sugerencias.First().Centro.CodigoSap);
            //var cantidadPendiente = cupoNormalSolicitud + fleteSolicitud;
            //var grupoDeCompras = comerciales.Where(x => x.ComercialId == sugerencias.First().ComercialId).First().GrupoDeCompras.Descripcion;
            //List<SugerenciaCupo> nuevaSugerencias = new List<SugerenciaCupo>();
            ////creo los flete
            //foreach (var sugerencia in sugerencias)
            //{
            //    if (fleteSolicitud == 0)
            //        break;

            //    if (sugerencia.CantidadDeCupos > fleteSolicitud)//parte no confirmada
            //    {
            //        SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
            //        nuevaSugerencia.CantidadDeCupos = sugerencia.CantidadDeCupos - fleteSolicitud;
            //        nuevaSugerencia.CantidadCupoOriginal = sugerencia.CantidadDeCupos - fleteSolicitud;
            //        nuevaSugerencia.Aceptado = null;
            //        nuevaSugerencias.Add(nuevaSugerencia);
            //        repositorio.Agregar(nuevaSugerencia);
            //        sugerencia.CantidadDeCupos = fleteSolicitud;
            //        sugerencia.CantidadCupoOriginal = fleteSolicitud;
            //    }

            //    AdministracionCupo solicitud = null;
            //    if (sugerenciaPorComercial.Total < fleteSolicitud)
            //    {
            //        sugerencia.CantidadDeCupos = sugerenciaPorComercial.Total;
            //        //var razonSocial = proveedores.Where(x => x.ProveedorId == sugerencia.ProveedorId).First().RazonSocial;

            //        solicitud = new AdministracionCupo()
            //        {
            //            CantidadCupo = 0,
            //            ComercialId = sugerencia.ComercialId,
            //            Fecha = sugerencia.FechaSugerida,
            //            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
            //            MaterialId = sugerencia.MaterialId,
            //            ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
            //            CentroId = sugerencia.CentroId,
            //            Excedente = true,
            //            CantidadFleteProcedencia = 0,
            //            ProveedorId = sugerencia.ProveedorId,
            //            TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
            //            SugerenciaCupoId = sugerencia.Id,
            //        };

            //        //primero se generan los de flete
            //        //if (sugerenciaPorComercial.Total < fleteSolicitud)
            //        //{
            //        //Generar solicitudes por la resta
            //        solicitud.CantidadFleteProcedencia = fleteSolicitud - sugerenciaPorComercial.Total;
            //        //Generear cupos
            //        result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, sugerenciaPorComercial.Total, true);
            //        //}
            //        //else
            //        //{
            //        //    //Generar cupos
            //        //    result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, fleteSolicitud, true);
            //        //}
            //        if (solicitud.CantidadFleteProcedencia > 0)
            //        {
            //            repositorio.Agregar(solicitud);
            //            logger.Debug("Se creo una solicitud flete: " + solicitud.CantidadFleteProcedencia);
            //        }

            //        //solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
            //        solicitudes.Add("Para la fecha: " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " se generó una solicitud de Cupo con flete: " + solicitud.CantidadFleteProcedencia);
            //    }
            //    else
            //    {
            //        result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, sugerencia.CantidadDeCupos, true);//primero se generan los de flete
            //    }
            //    if (!result.HayError)
            //    {
            //        if (solicitud == null)
            //        {
            //            sugerencia.Aceptado = true;
            //        }
            //    }
            //    else
            //    {
            //        if (result.ListaCupos.Count > 0)
            //        {
            //            sugerencia.CantidadDeCupos -= result.ListaCupos.Count;
            //        }
            //    }
            //    result.ListaCupos.AddRange(solicitudes);

            //    for (int i = 0; i < result2.ListaCupos.Count; i++)
            //    {
            //        result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
            //    }
            //    result.Errores.AddRange(result2.Errores);
            //    result.ListaCupos.AddRange(result2.ListaCupos);
            //    fleteSolicitud -= sugerencia.CantidadDeCupos;
            //}

            //sugerencias.AddRange(nuevaSugerencias);
            ////sugerencias = repositorio.Listar<SugerenciaCupo>(a => a.ProveedorId == proveedorId && a.MaterialId == materialId && a.Aceptado == null && a.ComercialId == comercialId && a.FechaSugerida == fecha && a.Centro.CodigoSap == centroCodigo, 0, "Puntuacion", DirOrden.Desc);
            ////creo los comunes
            //foreach (var sugerencia in sugerencias.Where(a => a.Aceptado == null).OrderByDescending(a => a.Puntuacion).ThenByDescending(a => a.Id))
            //{
            //    if (cupoNormalSolicitud == 0)
            //        break;

            //    if (sugerencia.CantidadDeCupos > cupoNormalSolicitud)//parte no confirmada
            //    {
            //        SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
            //        nuevaSugerencia.CantidadDeCupos = sugerencia.CantidadDeCupos - cupoNormalSolicitud;
            //        nuevaSugerencia.CantidadCupoOriginal = sugerencia.CantidadDeCupos - cupoNormalSolicitud;
            //        nuevaSugerencia.Aceptado = null;
            //        repositorio.Agregar(nuevaSugerencia);
            //        sugerencia.CantidadDeCupos = cupoNormalSolicitud;
            //        sugerencia.CantidadCupoOriginal = cupoNormalSolicitud;
            //    }

            //    AdministracionCupo solicitud = null;
            //    if (sugerenciaPorComercial.Total < cupoNormalSolicitud)
            //    {
            //        sugerencia.CantidadDeCupos = sugerenciaPorComercial.Total;
            //        //var razonSocial = proveedores.Where(x => x.ProveedorId == sugerencia.ProveedorId).First().RazonSocial;

            //        solicitud = new AdministracionCupo()
            //        {
            //            CantidadCupo = 0,
            //            ComercialId = sugerencia.ComercialId,
            //            Fecha = sugerencia.FechaSugerida,
            //            EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
            //            MaterialId = sugerencia.MaterialId,
            //            ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
            //            CentroId = sugerencia.CentroId,
            //            Excedente = true,
            //            CantidadFleteProcedencia = 0,
            //            ProveedorId = sugerencia.ProveedorId,
            //            TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
            //            SugerenciaCupoId = sugerencia.Id
            //        };


            //        //despues los comunes
            //        //if (sugerenciaPorComercial.Total < cupoNormalSolicitud)
            //        //{
            //        //Generar solicitudes por la resta
            //        solicitud.CantidadCupo = cupoNormalSolicitud - sugerenciaPorComercial.Total;
            //        sugerencia.CantidadDeCupos = cupoNormalSolicitud - sugerenciaPorComercial.Total;
            //        //Generear cupos
            //        result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, sugerenciaPorComercial.Total, false);
            //        //}
            //        //else
            //        //{
            //        //    //Generar cupos
            //        //    result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, cupoNormalSolicitud, false);
            //        //}


            //        if (solicitud.CantidadCupo > 0)
            //        {
            //            repositorio.Agregar(solicitud);
            //            logger.Debug("Se creo una solicitud cupo normal: " + solicitud.CantidadCupo);
            //        }

            //        //solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
            //        solicitudes.Add("Para la fecha: " + sugerencia.FechaSugerida.ToString("dd/MM/yyyy") + " se generó una solicitud: <br />Cupo normales:" + solicitud.CantidadCupo);
            //    }
            //    else
            //    {
            //        result2 = CrearCupoSugerenciaDetalle(sugerencia, sugerenciaPorComercial, sugerencia.CantidadDeCupos, false);//despues los comunes
            //    }
            //    if (!result2.HayError)
            //    {
            //        if (solicitud == null)
            //        {
            //            sugerencia.Aceptado = true;
            //        }
            //    }
            //    else
            //    {
            //        if (result2.ListaCupos.Count > 0)
            //        {
            //            sugerencia.CantidadDeCupos -= result2.ListaCupos.Count;
            //        }
            //    }
            //    result2.ListaCupos.AddRange(solicitudes);
            //    result.Errores.AddRange(result2.Errores);
            //    result.ListaCupos.AddRange(result2.ListaCupos);
            //    cupoNormalSolicitud -= sugerencia.CantidadDeCupos;
            //}
            //resultado.Add(result);
            //repositorio.GuardarCambios();
            return resultado;
        }

        public List<CupoResult> ModificarSugerenciaCupo(List<AceptarSugerenciaCupoDto> items)
        {

            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var solicitudes = new List<string>();
            List<CupoResult> resultado = new List<CupoResult>();
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            var result = new CupoResult();
            var result2 = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            var proveedorId = items.First().proveedorId;
            var idSugerencia = items.First().idSugerencia;
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            SugerenciaCupo sugerencia = repositorio.Obtener<SugerenciaCupo>(a => idSugerencia == a.Id && a.Aceptado == null);
            if (sugerencia == null)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La sugerencia ya fue aceptada");
                resultado.Add(errorCupera);
                return resultado;
            }

            //lo que no completo en la modificacion
            if (sugerencia.CantidadDeCupos > items.Sum(a => a.cantidad + a.cantidadFleteProcedencia))
            {
                SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                nuevaSugerencia.CantidadDeCupos = sugerencia.CantidadDeCupos - items.Sum(a => a.cantidad + a.cantidadFleteProcedencia);
                nuevaSugerencia.CantidadCupoOriginal = sugerencia.CantidadDeCupos - items.Sum(a => a.cantidad + a.cantidadFleteProcedencia);
                sugerencia.CantidadCupoOriginal = items.Sum(a => a.cantidad + a.cantidadFleteProcedencia);
                nuevaSugerencia.Aceptado = null;
                repositorio.Agregar(nuevaSugerencia);
                sugerencia.CantidadDeCupos = items.Sum(a => a.cantidad + a.cantidadFleteProcedencia);
            }

            if (!items.Any(a => a.fecha == sugerencia.FechaSugerida))
            {
                sugerencia.CantidadCupoOriginal = 0;
                sugerencia.CantidadDeCupos = 0;
                sugerencia.Aceptado = true;
            }

            foreach (var item in items)
            {
                solicitudes = new List<string>();
                var totalPorDia = ObtenerSugerenciaPorComercial(item.fecha.Value, sugerencia.ComercialId, sugerencia.MaterialId, sugerencia.Centro.CodigoSap);
                var cantidadIngresada = item.cantidad + item.cantidadFleteProcedencia;

                var grupoDeCompras = comerciales.Where(x => x.ComercialId == sugerencia.ComercialId).First().GrupoDeCompras.Descripcion;
                SugerenciaCupo sugerenciaFecha = null;
                // si es la fecha de la sugerencia lo que modifico es la sugerencia sino creo una nueva y uso esa
                if (sugerencia.FechaSugerida == item.fecha)
                {
                    sugerencia.CantidadDeCupos = cantidadIngresada;
                    sugerenciaFecha = sugerencia;
                }
                else
                {
                    SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                    nuevaSugerencia.CantidadDeCupos = cantidadIngresada;
                    nuevaSugerencia.CantidadCupoOriginal = cantidadIngresada;
                    nuevaSugerencia.FechaSugerida = item.fecha.Value;
                    nuevaSugerencia.Aceptado = null;
                    repositorio.Agregar(nuevaSugerencia);
                    sugerenciaFecha = nuevaSugerencia;
                }

                //creo solicitud si no tiene disponibles
                if (totalPorDia.Total < cantidadIngresada)
                {
                    //si la cupera esta liberada para el algoritmo

                    sugerenciaFecha.CantidadDeCupos = totalPorDia.Total;
                    var solicitud = new AdministracionCupo()
                    {
                        ComercialCreadorId = comercialCreador,
                        CantidadCupo = 0,
                        CantidadFleteProcedencia = 0,
                        ComercialId = sugerenciaFecha.ComercialId,
                        Fecha = sugerenciaFecha.FechaSugerida,
                        EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                        MaterialId = sugerenciaFecha.MaterialId,
                        ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                        CentroId = sugerenciaFecha.CentroId,
                        Excedente = true,
                        ProveedorId = sugerenciaFecha.ProveedorId,
                        TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                        FechaCreacion = DateTime.Now,
                    };

                    SugerenciaCupo nuevaSugerenciaSolicitud = ClonarSugerencia(sugerenciaFecha);
                    nuevaSugerenciaSolicitud.Aceptado = null;
                    nuevaSugerenciaSolicitud.CantidadDeCupos = 0;
                    nuevaSugerenciaSolicitud.CantidadCupoOriginal = 0;
                    var configuracion = DevolverCuperaLiberada(sugerenciaFecha);
                    //primero se generan los de flete
                    var total = totalPorDia.Total;
                    if (configuracion != null)
                    {
                        total += configuracion.LimiteCupo - ObtenerCuposConsumidosPorFecha(sugerenciaFecha.FechaSugerida, sugerenciaFecha);
                    }
                    if (total < item.cantidadFleteProcedencia)
                    {

                        //Generar solicitudes por la resta y la sugerencia de esa solicitud
                        solicitud.CantidadFleteProcedencia = item.cantidadFleteProcedencia - total;
                        nuevaSugerenciaSolicitud.CantidadDeCupos = solicitud.CantidadFleteProcedencia;
                        nuevaSugerenciaSolicitud.CantidadCupoOriginal = solicitud.CantidadFleteProcedencia;
                        //Generear cupos
                        result2 = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, total, true, null);
                        total -= result2.ListaCupos.Count();
                    }
                    else
                    {
                        //Generar cupos
                        result2 = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, item.cantidadFleteProcedencia, true, null);
                        total -= result2.ListaCupos.Count();
                    }
                    //despues los comunes
                    if (total < item.cantidad)
                    {
                        //Generar solicitudes por la resta y la sugerencia de esa solicitud
                        solicitud.CantidadCupo = item.cantidad - total;
                        nuevaSugerenciaSolicitud.CantidadDeCupos += solicitud.CantidadCupo;
                        nuevaSugerenciaSolicitud.CantidadCupoOriginal += solicitud.CantidadCupo;
                        //Generear cupos
                        result = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, total, false, null);
                        total -= result.ListaCupos.Count();
                    }
                    else
                    {
                        //Generar cupos
                        result = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, item.cantidad, false, null);
                        total -= result.ListaCupos.Count();
                    }

                    if (solicitud.CantidadFleteProcedencia > 0 || solicitud.CantidadCupo > 0)
                    {
                        solicitud.SugerenciaCupo = nuevaSugerenciaSolicitud;
                        repositorio.Agregar(solicitud);
                        logger.Debug("Se creo una solicitud flete: " + solicitud.CantidadFleteProcedencia + " cupo normal: " + solicitud.CantidadCupo);
                        solicitudes.Add("Para la fecha: " + sugerenciaFecha.FechaSugerida.ToString("dd/MM/yyyy") + " se generó una solicitud: <br />Cupo normales:" + solicitud.CantidadCupo + "<br /> Cupo con flete: " + solicitud.CantidadFleteProcedencia);
                    }

                    //solicitudes.Add(!String.IsNullOrEmpty(razonSocial) ? "<hr />" + razonSocial : "");
                }
                else
                {
                    result2 = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, item.cantidadFleteProcedencia, true, null);//primero se generan los de flete
                    result = CrearCupoSugerenciaDetalle(sugerenciaFecha, totalPorDia, item.cantidad, false, null);//despues los comunes
                }
                if (!result.HayError)
                {
                    sugerenciaFecha.Aceptado = true;
                }
                else
                {
                    if (result.ListaCupos.Count > 0)
                    {
                        sugerenciaFecha.CantidadDeCupos -= result.ListaCupos.Count;
                    }
                }


                result.ListaCupos.AddRange(solicitudes);

                for (int i = 0; i < result2.ListaCupos.Count; i++)
                {
                    result2.ListaCupos[i] = "<strong>*" + result2.ListaCupos[i] + "*</strong>";
                }
                result.Errores.AddRange(result2.Errores);
                result.ListaCupos.AddRange(result2.ListaCupos);
                resultado.Add(result);
            }

            if (resultado.First().ListaCupos.Count() > 0)
            {
                resultado.First().ListaCupos.Insert(0, proveedor != null ? proveedor.RazonSocial : "");
            }

            repositorio.GuardarCambios();
            return resultado;
        }


        public List<CupoResult> ModificarSugerenciaCupoPorProveedor(List<AceptarSugerenciaCupoDto> sugerenciasAceptadasDesdeElFront)
        {

            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var solicitudes = new List<string>();
            var solicitudesDto = new List<AdministracionCupoDto>();
            List<CupoResult> resultado = new List<CupoResult>();
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            var resultNormales = new CupoResult();
            var resultFletes = new CupoResult();
            var resultFinal = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            var proveedorId = sugerenciasAceptadasDesdeElFront.First().proveedorId;
            var materialId = sugerenciasAceptadasDesdeElFront.First().materialId.Value;
            var centroSAP = sugerenciasAceptadasDesdeElFront.First().centroId;
            var fechaOriginal = sugerenciasAceptadasDesdeElFront.First().fechaOriginal;
            var comercialId = sugerenciasAceptadasDesdeElFront.First().comercialId;
            var grupoDeCompras = comerciales.Where(x => x.ComercialId == comercialId).First().GrupoDeCompras.Descripcion;
            var proveedorDesc = repositorio.Obtener<Proveedor>(proveedorId);

            List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a =>
                a.Aceptado == null
                && a.ProveedorId == proveedorId
                && a.MaterialId == materialId
                && a.Centro.CodigoSap == centroSAP
                && a.FechaSugerida == fechaOriginal
                && a.ComercialId == comercialId
                && (a.Solicitudes.All(y => y.EstadoId == 2) || a.Solicitudes.Count() == 0)
            , 0, "Puntuacion", DirOrden.Desc);

            if (sugerencias.Count() == 0)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "No hay sugerencias pendientes");
                resultado.Add(errorCupera);
                return resultado;
            }

            //sugerenciasAceptadasDesdeElFront
            foreach (var sugerenciaAceptadaDesdeElFront in sugerenciasAceptadasDesdeElFront)
            {
                var centroId = repositorio.Obtener<Centro, int>(a => a.CodigoSap == sugerenciaAceptadaDesdeElFront.centroId, a => a.Id);
                var totalPorDia = ObtenerSugerenciaPorComercial(sugerenciaAceptadaDesdeElFront.fecha.Value, comercialId, materialId, centroSAP);
                var total = totalPorDia.Total;

                var cuperaLiberada = DevolverCuperaLiberada(sugerenciaAceptadaDesdeElFront.materialId.Value, centroId, sugerenciaAceptadaDesdeElFront.fecha.Value);
                if (cuperaLiberada != null)
                {
                    total = cuperaLiberada.LimiteCupo - ObtenerCuposConsumidosPorFecha(sugerenciaAceptadaDesdeElFront.materialId.Value, centroId, sugerenciaAceptadaDesdeElFront.fecha.Value);
                }
                var nuevaSugerencias = new List<SugerenciaCupo>();

                //acepta sugerencias / crea cupos
                foreach (var sugerencia in sugerencias.Where(a => a.Aceptado == null).OrderByDescending(a => a.Puntuacion).ThenBy(a => a.Id))
                {
                    if (total <= 0)
                    {
                        break;
                    }
                    resultNormales = new CupoResult();
                    resultFletes = new CupoResult();
                    //si quedan pendientes para creaer
                    if ((sugerenciaAceptadaDesdeElFront.cantidad + sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia) > 0)
                    {
                        //si quedan pendientes de flete
                        int puedoCrearFlete = new List<int> { total, sugerencia.CantidadDeCupos, sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia }.Min();
                        if (sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia > 0 && puedoCrearFlete > 0)
                        {
                            resultFletes = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, puedoCrearFlete, true, sugerenciaAceptadaDesdeElFront.fecha.Value);
                            resultFinal.ListaCupos.AddRange(resultFletes.ListaCupos.Select(a => "*" + a + "*"));
                            total -= resultFletes.ListaCupos.Count();
                            //creo una nueva sug por lo que se confirmar
                            if (sugerencia.CantidadDeCupos > puedoCrearFlete)
                            {
                                SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                                nuevaSugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                if (!resultFletes.HayError)
                                {
                                    nuevaSugerencia.CantidadDeCupos = puedoCrearFlete;
                                    nuevaSugerencia.CantidadCupoOriginal = puedoCrearFlete;
                                    nuevaSugerencia.Aceptado = true;
                                    sugerencia.CantidadCupoOriginal -= puedoCrearFlete;
                                    sugerencia.CantidadDeCupos -= puedoCrearFlete;

                                    sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= puedoCrearFlete;
                                }
                                else
                                {
                                    if (resultFletes.ListaCupos.Count() > 0)
                                    {
                                        puedoCrearFlete = resultFletes.ListaCupos.Count();
                                        sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= puedoCrearFlete;
                                        nuevaSugerencia.Aceptado = true;
                                        nuevaSugerencia.CantidadDeCupos = puedoCrearFlete;
                                        nuevaSugerencia.CantidadCupoOriginal = puedoCrearFlete;
                                        sugerencia.CantidadCupoOriginal -= puedoCrearFlete;
                                        sugerencia.CantidadDeCupos -= puedoCrearFlete;
                                    }
                                }
                                if (resultFletes.ListaCupos.Count() > 0)
                                {
                                    repositorio.Agregar(nuevaSugerencia);
                                    nuevaSugerencias.Add(nuevaSugerencia);
                                }
                            }
                            else
                            {
                                if (!resultFletes.HayError)
                                {
                                    sugerencia.Aceptado = true;
                                    sugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                    sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= puedoCrearFlete;
                                }
                                else
                                {
                                    if (resultFletes.ListaCupos.Count() > 0)
                                    {
                                        puedoCrearFlete = resultFletes.ListaCupos.Count();
                                        SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                                        nuevaSugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                        nuevaSugerencia.CantidadDeCupos = puedoCrearFlete;
                                        nuevaSugerencia.CantidadCupoOriginal = puedoCrearFlete;
                                        sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= puedoCrearFlete;
                                        nuevaSugerencia.Aceptado = true;
                                        sugerencia.CantidadCupoOriginal -= puedoCrearFlete;
                                        sugerencia.CantidadDeCupos -= puedoCrearFlete;
                                        repositorio.Agregar(nuevaSugerencia);
                                    }
                                }
                            }

                        }

                        //si quedan pendientes normales
                        int puedoCrear = new List<int> { total, sugerencia.CantidadDeCupos, sugerenciaAceptadaDesdeElFront.cantidad }.Min();
                        if (sugerenciaAceptadaDesdeElFront.cantidad > 0 && puedoCrear > 0)
                        {
                            resultNormales = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, puedoCrear, false, sugerenciaAceptadaDesdeElFront.fecha);
                            resultFinal.ListaCupos.AddRange(resultNormales.ListaCupos.Select(a => a));
                            total -= resultNormales.ListaCupos.Count();
                            //creo una nueva sug por lo que se confirmar
                            if (sugerencia.CantidadDeCupos > puedoCrear)
                            {
                                SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                                nuevaSugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                if (!resultNormales.HayError)
                                {
                                    nuevaSugerencia.CantidadDeCupos = puedoCrear;
                                    nuevaSugerencia.CantidadCupoOriginal = puedoCrear;
                                    nuevaSugerencia.Aceptado = true;
                                    sugerencia.CantidadCupoOriginal -= puedoCrear;
                                    sugerencia.CantidadDeCupos -= puedoCrear;
                                    sugerenciaAceptadaDesdeElFront.cantidad -= puedoCrear;
                                }
                                else
                                {
                                    if (resultNormales.ListaCupos.Count() > 0)
                                    {
                                        puedoCrear = resultNormales.ListaCupos.Count();
                                        sugerenciaAceptadaDesdeElFront.cantidad -= puedoCrear;
                                        nuevaSugerencia.Aceptado = true;
                                        nuevaSugerencia.CantidadDeCupos = puedoCrear;
                                        nuevaSugerencia.CantidadCupoOriginal = puedoCrear;
                                        sugerencia.CantidadCupoOriginal -= puedoCrear;
                                        sugerencia.CantidadDeCupos -= puedoCrear;
                                    }
                                }
                                if (resultNormales.ListaCupos.Count() > 0)
                                {
                                    repositorio.Agregar(nuevaSugerencia);
                                    nuevaSugerencias.Add(nuevaSugerencia);
                                }
                            }
                            else
                            {
                                if (!resultNormales.HayError)
                                {
                                    sugerencia.Aceptado = true;
                                    sugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                    sugerenciaAceptadaDesdeElFront.cantidad -= puedoCrear;
                                }
                                else
                                {
                                    if (resultNormales.ListaCupos.Count() > 0)
                                    {
                                        puedoCrear = resultNormales.ListaCupos.Count();
                                        SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                                        nuevaSugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                        nuevaSugerencia.CantidadDeCupos = puedoCrear;
                                        nuevaSugerencia.CantidadCupoOriginal = puedoCrear;
                                        sugerenciaAceptadaDesdeElFront.cantidad -= puedoCrear;
                                        nuevaSugerencia.Aceptado = true;
                                        sugerencia.CantidadCupoOriginal -= puedoCrear;
                                        sugerencia.CantidadDeCupos -= puedoCrear;
                                        repositorio.Agregar(nuevaSugerencia);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // si no esta liberada la cupera creo solicitudes
                if (cuperaLiberada == null)
                {
                    //crear solicitudes flete
                    foreach (var sugerencia in sugerencias.Where(a => a.Aceptado == null && (a.Solicitudes == null || a.Solicitudes.Count() == 0)).OrderByDescending(a => a.Puntuacion).ThenBy(a => a.Id))
                    {
                        if (sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia > 0)
                        {
                            int puedoSolicitarFlete = new List<int> { sugerencia.CantidadDeCupos, sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia }.Min();
                            var solicitud = new AdministracionCupo()
                            {
                                ComercialCreadorId = comercialCreador,
                                CantidadCupo = 0,
                                CantidadFleteProcedencia = 0,
                                ComercialId = sugerencia.ComercialId,
                                Fecha = sugerenciaAceptadaDesdeElFront.fecha.Value,
                                EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                                MaterialId = sugerencia.MaterialId,
                                ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                                CentroId = sugerencia.CentroId,
                                Excedente = true,
                                ProveedorId = sugerencia.ProveedorId,
                                TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                                FechaCreacion = DateTime.Now,
                            };
                            if (puedoSolicitarFlete >= sugerencia.CantidadDeCupos)
                            {
                                solicitud.CantidadFleteProcedencia = sugerencia.CantidadDeCupos;
                                solicitud.SugerenciaCupo = sugerencia;
                                sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= sugerencia.CantidadDeCupos;
                                sugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                            }
                            else
                            {
                                SugerenciaCupo nuevaSugerenciaSolicitud = ClonarSugerencia(sugerencia);
                                nuevaSugerenciaSolicitud.CantidadDeCupos = puedoSolicitarFlete;
                                nuevaSugerenciaSolicitud.CantidadCupoOriginal = puedoSolicitarFlete;
                                nuevaSugerenciaSolicitud.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                solicitud.CantidadFleteProcedencia = puedoSolicitarFlete;
                                solicitud.SugerenciaCupo = nuevaSugerenciaSolicitud;
                                sugerenciaAceptadaDesdeElFront.cantidadFleteProcedencia -= puedoSolicitarFlete;
                                sugerencia.CantidadDeCupos -= puedoSolicitarFlete;
                                sugerencia.CantidadCupoOriginal -= puedoSolicitarFlete;
                                repositorio.Agregar(solicitud);
                            }
                            solicitudesDto.Add(new AdministracionCupoDto { Fecha = solicitud.Fecha, CantidadDeCupo = solicitud.CantidadCupo, CantidadFleteProcedencia = solicitud.CantidadFleteProcedencia });

                        }
                        else
                        {
                            break;
                        }

                    }

                    //crear solicitudes normal
                    foreach (var sugerencia in sugerencias.Where(a => a.Aceptado == null && (a.Solicitudes == null || a.Solicitudes.Count() == 0)).OrderByDescending(a => a.Puntuacion).ThenBy(a => a.Id))
                    {
                        if (sugerenciaAceptadaDesdeElFront.cantidad > 0)
                        {
                            int puedoSolicitar = new List<int> { sugerencia.CantidadDeCupos, sugerenciaAceptadaDesdeElFront.cantidad }.Min();
                            var solicitud = new AdministracionCupo()
                            {
                                ComercialCreadorId = comercialCreador,
                                CantidadCupo = 0,
                                CantidadFleteProcedencia = 0,
                                ComercialId = sugerencia.ComercialId,
                                Fecha = sugerenciaAceptadaDesdeElFront.fecha.Value,
                                EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                                MaterialId = sugerencia.MaterialId,
                                ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                                CentroId = sugerencia.CentroId,
                                Excedente = true,
                                ProveedorId = sugerencia.ProveedorId,
                                TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                                FechaCreacion = DateTime.Now,
                            };
                            if (puedoSolicitar >= sugerencia.CantidadDeCupos)
                            {
                                solicitud.CantidadCupo = sugerencia.CantidadDeCupos;
                                solicitud.SugerenciaCupo = sugerencia;
                                sugerenciaAceptadaDesdeElFront.cantidad -= sugerencia.CantidadDeCupos;
                                sugerencia.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                            }
                            else
                            {
                                SugerenciaCupo nuevaSugerenciaSolicitud = ClonarSugerencia(sugerencia);
                                nuevaSugerenciaSolicitud.CantidadDeCupos = puedoSolicitar;
                                nuevaSugerenciaSolicitud.CantidadCupoOriginal = puedoSolicitar;
                                nuevaSugerenciaSolicitud.FechaSugerida = sugerenciaAceptadaDesdeElFront.fecha.Value;
                                solicitud.CantidadCupo = puedoSolicitar;
                                solicitud.SugerenciaCupo = nuevaSugerenciaSolicitud;
                                sugerenciaAceptadaDesdeElFront.cantidad -= puedoSolicitar;
                                sugerencia.CantidadDeCupos -= puedoSolicitar;
                                sugerencia.CantidadCupoOriginal -= puedoSolicitar;
                                repositorio.Agregar(solicitud);
                            }
                            solicitudesDto.Add(new AdministracionCupoDto { Fecha = solicitud.Fecha, CantidadDeCupo = solicitud.CantidadCupo, CantidadFleteProcedencia = solicitud.CantidadFleteProcedencia });

                        }
                        else
                        {
                            break;
                        }

                    }

                }
            }


            foreach (var solicitudDto in solicitudesDto.GroupBy(x => x.Fecha))
            {
                solicitudes.Add("Para la fecha: " + solicitudDto.Key.ToString("dd/MM/yyyy") + " se generó una solicitud: " +
                    (solicitudDto.Sum(a => a.CantidadDeCupo) > 0 ? "<br />Cupo normales:" + solicitudDto.Sum(a => a.CantidadDeCupo) : "") +
                    (solicitudDto.Sum(a => a.CantidadFleteProcedencia) > 0 ? "<br />Cupo con flete:" + solicitudDto.Sum(a => a.CantidadFleteProcedencia) : "")
                );
            }
            //solicitudes.Insert(0, (proveedorDesc != null ? proveedorDesc.RazonSocial + "<br/>" : ""));
            //solicitudes.Add("<br/>");
            //
            if (resultFinal.ListaCupos.Count > 0)
            {
                resultFinal.ListaCupos.Insert(0, (proveedorDesc != null ? proveedorDesc.RazonSocial : ""));
            }
            resultFinal.ListaCupos.AddRange(solicitudes);
            resultado.Add(resultFinal);

            repositorio.GuardarCambios();
            return resultado;
        }

        public List<CupoResult> ModificarSugerenciaCupoPorProveedor2(List<AceptarSugerenciaCupoDto> items)
        {

            var primerCierre = this.DevolverTodoCierreCupera().FirstOrDefault();
            var validarSiHayCierre = primerCierre != null ? primerCierre.Cierre : false;
            var solicitudes = new List<string>();
            var solicitudesDto = new List<AdministracionCupoDto>();
            List<CupoResult> resultado = new List<CupoResult>();
            var activeCreador = PermisosHelper.ObtenerUsuario();
            var comercialCreador = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == activeCreador, x => x.ComercialId);
            var resultNormales = new CupoResult();
            var resultFletes = new CupoResult();
            var resultFinal = new CupoResult();
            if (validarSiHayCierre)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "La Cupera se encuentra momentáneamente bloqueada, por cualquier duda o inconveniente comunicarse con el Administrador de la Cupera.");
                resultado.Add(errorCupera);
                return resultado;
            }
            var zonaCupo = repositorio.Listar<ZonaCupo>();
            var comerciales = repositorio.Listar<Comercial>();
            var proveedorId = items.First().proveedorId;
            var materialId = items.First().materialId.Value;
            var centroSAP = items.First().centroId;
            var fechaOriginal = items.First().fechaOriginal;
            var comercialId = items.First().comercialId;
            var grupoDeCompras = comerciales.Where(x => x.ComercialId == comercialId).First().GrupoDeCompras.Descripcion;
            var proveedorDesc = repositorio.Obtener<Proveedor>(proveedorId);

            List<SugerenciaCupo> sugerencias = repositorio.Listar<SugerenciaCupo>(a =>
                a.Aceptado == null
                && a.ProveedorId == proveedorId
                && a.MaterialId == materialId
                && a.Centro.CodigoSap == centroSAP
                && a.FechaSugerida == fechaOriginal
                && a.ComercialId == comercialId
                && (a.Solicitudes.All(y => y.EstadoId == 2) || a.Solicitudes.Count() == 0)
            , 0, "Puntuacion", DirOrden.Desc);

            if (sugerencias.Count() == 0)
            {
                var errorCupera = new CupoResult();
                errorCupera.Error("CantidadCuposSAP", "No hay sugerencias pendientes");
                resultado.Add(errorCupera);
                return resultado;
            }
            //sugerenciasAceptadasDesdeElFront
            foreach (var item in items)
            {
                var nuevaSugerencias = new List<SugerenciaCupo>();
                var totalPorDia = ObtenerSugerenciaPorComercial(item.fecha.Value, comercialId, materialId, centroSAP);
                var totalPendiente = item.cantidad + item.cantidadFleteProcedencia;

                //agrego el where y el ordern por que se agrean y aceptan sugerencias
                foreach (var sugerencia in sugerencias.Where(a => a.Aceptado == null).OrderByDescending(a => a.Puntuacion).ThenBy(a => a.Id))
                {
                    resultNormales = new CupoResult();
                    resultFletes = new CupoResult();
                    if (totalPendiente > 0)
                    {
                        //creo una nueva sug por lo que no se va a confirmar
                        if (sugerencia.CantidadDeCupos > totalPendiente)
                        {
                            SugerenciaCupo nuevaSugerencia = ClonarSugerencia(sugerencia);
                            nuevaSugerencia.CantidadDeCupos = sugerencia.CantidadDeCupos - totalPendiente;
                            nuevaSugerencia.CantidadCupoOriginal = sugerencia.CantidadDeCupos - totalPendiente;
                            sugerencia.CantidadCupoOriginal = totalPendiente;
                            nuevaSugerencia.Aceptado = null;
                            nuevaSugerencias.Add(nuevaSugerencia);
                            repositorio.Agregar(nuevaSugerencia);
                            sugerencia.CantidadDeCupos = totalPendiente;
                        }
                        totalPendiente -= sugerencia.CantidadDeCupos;

                        sugerencia.FechaSugerida = item.fecha.Value;

                        //creo solicitud si no tiene disponibles
                        if (totalPorDia.Total < sugerencia.CantidadDeCupos)
                        {
                            var solicitud = new AdministracionCupo()
                            {
                                ComercialCreadorId = comercialCreador,
                                CantidadCupo = 0,
                                CantidadFleteProcedencia = 0,
                                ComercialId = sugerencia.ComercialId,
                                Fecha = sugerencia.FechaSugerida,
                                EstadoId = (int)EnumEstadoAdministracionCupo.EstadoPendienteAdministracionCupo,
                                MaterialId = sugerencia.MaterialId,
                                ZonaId = zonaCupo.Where(x => x.Descripcion == grupoDeCompras).First().Id,
                                CentroId = sugerencia.CentroId,
                                Excedente = true,
                                ProveedorId = sugerencia.ProveedorId,
                                TipoAdministracionCupoId = (int)EnumTipoAdministracionCupo.Algoritmo,
                                FechaCreacion = DateTime.Now,
                            };

                            SugerenciaCupo nuevaSugerenciaSolicitud = ClonarSugerencia(sugerencia);
                            nuevaSugerenciaSolicitud.Aceptado = null;
                            nuevaSugerenciaSolicitud.CantidadDeCupos = 0;
                            nuevaSugerenciaSolicitud.CantidadCupoOriginal = 0;

                            int generados = 0;
                            var configuracion = DevolverCuperaLiberada(sugerencia);
                            var total = totalPorDia.Total;
                            if (configuracion != null)
                            {
                                total += configuracion.LimiteCupo - ObtenerCuposConsumidosPorFecha(sugerencia.FechaSugerida, sugerencia);
                            }
                            //primero se generan los de flete
                            if (item.cantidadFleteProcedencia > 0)//10
                            {
                                generados = item.cantidadFleteProcedencia < sugerencia.CantidadDeCupos ? item.cantidadFleteProcedencia : sugerencia.CantidadDeCupos;
                                //1                       //3
                                if (total < generados)
                                {
                                    //Generar solicitudes por la resta y la sugerencia de esa solicitud
                                    solicitud.CantidadFleteProcedencia = total - generados;
                                    item.cantidadFleteProcedencia -= generados;
                                    nuevaSugerenciaSolicitud.CantidadDeCupos += solicitud.CantidadFleteProcedencia;
                                    nuevaSugerenciaSolicitud.CantidadCupoOriginal += solicitud.CantidadFleteProcedencia;
                                    //Generear cupos
                                    resultFletes = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, generados, true, null);
                                    total -= resultFletes.ListaCupos.Count();

                                }
                                else
                                {
                                    //Generar cupos
                                    resultFletes = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, generados, true, null);
                                    total -= resultFletes.ListaCupos.Count();
                                }
                            }


                            //despues los comunes
                            if (item.cantidad > 0)
                            {
                                if (total < (sugerencia.CantidadDeCupos - generados))
                                {
                                    //Generar solicitudes por la resta y la sugerencia de esa solicitud
                                    solicitud.CantidadCupo = (sugerencia.CantidadDeCupos - generados) - total;
                                    item.cantidad -= (sugerencia.CantidadDeCupos - generados);
                                    nuevaSugerenciaSolicitud.CantidadDeCupos += solicitud.CantidadCupo;
                                    nuevaSugerenciaSolicitud.CantidadCupoOriginal += solicitud.CantidadCupo;
                                    //Generear cupos
                                    resultNormales = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, total, true, null);
                                    total -= resultNormales.ListaCupos.Count();
                                }
                                else
                                {
                                    //Generar cupos
                                    resultNormales = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, (sugerencia.CantidadDeCupos - generados), true, null);
                                    total -= resultNormales.ListaCupos.Count();
                                }
                            }

                            sugerencia.CantidadDeCupos -= solicitud.CantidadCupo + solicitud.CantidadFleteProcedencia;

                            if (solicitud.CantidadFleteProcedencia > 0 || solicitud.CantidadCupo > 0)
                            {
                                solicitud.SugerenciaCupo = nuevaSugerenciaSolicitud;
                                repositorio.Agregar(solicitud);
                                logger.Debug("Se creo una solicitud flete: " + solicitud.CantidadFleteProcedencia + " cupo normal: " + solicitud.CantidadCupo);
                                solicitudesDto.Add(new AdministracionCupoDto { Fecha = solicitud.Fecha, CantidadDeCupo = solicitud.CantidadCupo, CantidadFleteProcedencia = solicitud.CantidadFleteProcedencia });
                            }

                        }
                        else
                        {
                            var cantidadFlete = item.cantidadFleteProcedencia < sugerencia.CantidadDeCupos ? item.cantidadFleteProcedencia : sugerencia.CantidadDeCupos;
                            if (item.cantidadFleteProcedencia > 0)
                            {
                                item.cantidadFleteProcedencia -= cantidadFlete;
                                resultFletes = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, cantidadFlete, true, null);//primero se generan los de flete
                            }
                            if (item.cantidad > 0)
                            {
                                var cantidad = item.cantidad < (sugerencia.CantidadDeCupos - cantidadFlete) ? item.cantidad : (sugerencia.CantidadDeCupos - cantidadFlete);
                                item.cantidad -= cantidad;
                                resultNormales = CrearCupoSugerenciaDetalle(sugerencia, totalPorDia, cantidad, false, null);//despues los comunes
                            }
                        }
                        if (!resultNormales.HayError)
                        {
                            sugerencia.Aceptado = true;
                        }
                        else
                        {
                            if (resultNormales.ListaCupos.Count > 0)
                            {
                                sugerencia.CantidadDeCupos -= resultNormales.ListaCupos.Count;
                            }
                        }



                        for (int i = 0; i < resultFletes.ListaCupos.Count; i++)
                        {
                            resultFletes.ListaCupos[i] = "<strong>*" + resultFletes.ListaCupos[i] + "*</strong>";
                        }
                        resultNormales.Errores.AddRange(resultFletes.Errores);
                        resultNormales.ListaCupos.AddRange(resultFletes.ListaCupos);

                        resultFinal.Errores.AddRange(resultNormales.Errores);
                        resultFinal.ListaCupos.AddRange(resultNormales.ListaCupos);
                        //--
                    }
                    else
                    {
                        break;
                    }
                }
                sugerencias.AddRange(nuevaSugerencias);
            }


            foreach (var solicitudDto in solicitudesDto.GroupBy(x => x.Fecha))
            {
                solicitudes.Add("Para la fecha: " + solicitudDto.Key.ToString("dd/MM/yyyy") + " se generó una solicitud: " +
                    (solicitudDto.Sum(a => a.CantidadDeCupo) > 0 ? "<br />Cupo normales:" + solicitudDto.Sum(a => a.CantidadDeCupo) : "") +
                    (solicitudDto.Sum(a => a.CantidadFleteProcedencia) > 0 ? "<br />Cupo con flete:" + solicitudDto.Sum(a => a.CantidadFleteProcedencia) : "")
                );
            }
            //solicitudes.Insert(0, (proveedorDesc != null ? proveedorDesc.RazonSocial + "<br/>" : ""));
            //solicitudes.Add("<br/>");
            //
            if (resultFinal.ListaCupos.Count > 0)
            {
                resultFinal.ListaCupos.Insert(0, (proveedorDesc != null ? proveedorDesc.RazonSocial : ""));
            }
            resultFinal.ListaCupos.AddRange(solicitudes);
            resultado.Add(resultFinal);

            repositorio.GuardarCambios();
            return resultado;
        }


        public void ActualizarCumplimientoCupos(DateTime fecha)
        {
            var cuposSave = repositorio.Listar<Cupo>(x => x.FechaIngreso == fecha);
            //var cupos = cuposSave.Select(x => x.CupoSap).ToList();
            var resultado = cumplimientoCuposAgent.Ejecutar(new List<string>(),fecha);

            foreach (var item in resultado)
            {
                var cupo = cuposSave.Where(x => x.CupoSap == item.Codigo).SingleOrDefault();
                if (cupo != null)
                {
                    cupo.Cumplimiento = item.Cumplimiento;
                }
            }
            repositorio.GuardarCambios();
        }

        private void EnviarMailNegociosDeAlgoritmo(byte[] excel)
        {
            try
            {
                var path = httpContextManager.ObtenerPathLogoMail();
                var alterView = CuerpoMailNegociosAlgoritmo(path);
                var comerciales = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.AlgoritimoDeCupos)));
                var mail = new List<string> { };
                if (comerciales != null && comerciales.Count > 0)
                {
                    mail.AddRange(comerciales.Select(x => x.IdActiveDirectory).ToList());
                }
                mail.Add("dataagro@baufest.com");
                var asunto = "Prueba - Resultado Algoritmo de cupos";
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    asunto = "Resultado Algoritmo de cupos";
                }
                mailManager.EnviarMail(mail, asunto, "", null, alterView, excel, "Reporte Algoritmo.xlsx");

            }
            catch (Exception e)
            {

                logger.Error("Error al enviar el mail EnviarMailNegociosDeAlgoritmo", e.Message);
            }
        }


        private AlternateView CuerpoMailNegociosAlgoritmo(String filePath)
        {
            //var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla adjunto el resultado de los negocios que el algoritmo tomo para priorizar <br />";

            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private List<SugerenciaCupoAgrupadasExcel> DevolverSugerenciasAgrupadas(List<SugerenciaCupoExcel> oDatos, List<FormulaDto> formulas)
        {
            var sugerencias = new List<SugerenciaCupoAgrupadasExcel>();
            oDatos = oDatos.Where(x => x.Priorizado == "SI").ToList();
            foreach (var item in oDatos.GroupBy(x => x.Material))
            {
                var dias = new List<DateTime>();
                var fechaInicio = formulas.Where(x => x.Material == item.Key).FirstOrDefault().CuposDesde < DateTime.Now.Date ? DateTime.Now.Date : formulas.Where(x => x.Material == item.Key).FirstOrDefault().CuposDesde;
                var i = 1;
                dias.Add(fechaInicio);
                while (fechaInicio.AddDays(i) <= formulas.Where(x => x.Material == item.Key).FirstOrDefault().CuposHasta)
                {
                    dias.Add(fechaInicio.AddDays(i++));
                }
                foreach (var dato in oDatos.GroupBy(x => x.RazonSocial))
                {
                    var sugerencia = new SugerenciaCupoAgrupadasExcel()
                    {
                        Material = item.Key,
                        RazonSocial = dato.Key,
                        DiaCupo = new List<DiaCupo>()
                    };
                    foreach (var dia in dias)
                    {
                        var formula = dato.Where(x => x.FechaSugerida.Date == dia.Date).DefaultIfEmpty(new SugerenciaCupoExcel() { CantidadSugerida = 0, FechaSugerida = dia }).Sum(x => x.CantidadSugerida);
                        sugerencia.DiaCupo.Add(new DiaCupo { Fecha = dia, Cantidad = formula });
                    }
                    sugerencias.Add(sugerencia);
                }
            }
            return sugerencias;
        }

        public byte[] GenerarExcelNegociosAlgoritmo(List<SugerenciaCupoExcel> oDatos, List<FormulaDto> formulas)
        {

            var excel = new ExcelPackage();

            var oColumnas = oDatos;


            var oPropRow = oColumnas.GetType().GetProperties();

            var cantColumns = oPropRow.Count();

            //Soja
            var workSheet1 = excel.Workbook.Worksheets.Add("Soja");
            var soja = DevolverSugerenciasAgrupadas(oDatos, formulas).Where(x => x.Material == "Soja").ToList();
            workSheet1.Cells[2, 1].LoadFromCollection(soja, false);
            workSheet1.Cells[1, 1].Value = "Material";
            workSheet1.Column(1).AutoFit();
            workSheet1.Cells[1, 2].Value = "Razon Social";
            workSheet1.Column(2).AutoFit();
            ArmarAgrupacionExcel(workSheet1, soja);
            //Maiz
            var workSheet2 = excel.Workbook.Worksheets.Add("Maiz");
            var maiz = DevolverSugerenciasAgrupadas(oDatos, formulas).Where(x => x.Material == "Maiz").ToList();
            workSheet2.Cells[2, 1].LoadFromCollection(maiz, false);
            workSheet2.Cells[1, 1].Value = "Material";
            workSheet2.Column(1).AutoFit();
            workSheet2.Cells[1, 2].Value = "Razon Social";
            workSheet2.Column(2).AutoFit();
            ArmarAgrupacionExcel(workSheet2, maiz);
            //Trigo
            var workSheet3 = excel.Workbook.Worksheets.Add("Trigo");
            var trigo = DevolverSugerenciasAgrupadas(oDatos, formulas).Where(x => x.Material == "Trigo").ToList();
            workSheet3.Cells[2, 1].LoadFromCollection(trigo, false);
            workSheet3.Cells[1, 1].Value = "Material";
            workSheet3.Column(1).AutoFit();
            workSheet3.Cells[1, 2].Value = "Razon Social";
            workSheet3.Column(2).AutoFit();
            ArmarAgrupacionExcel(workSheet3, trigo);
            //Girasol
            var workSheet4 = excel.Workbook.Worksheets.Add("Girasol");
            var girasol = DevolverSugerenciasAgrupadas(oDatos, formulas).Where(x => x.Material == "Girasol").ToList();
            workSheet4.Cells[2, 1].LoadFromCollection(girasol, false);
            workSheet4.Cells[1, 1].Value = "Material";
            workSheet4.Column(1).AutoFit();
            workSheet4.Cells[1, 2].Value = "Razon Social";
            workSheet4.Column(2).AutoFit();
            ArmarAgrupacionExcel(workSheet4, girasol);

            var workSheet9 = excel.Workbook.Worksheets.Add("Detalle Negocios");
            workSheet9.Cells[1, 1].LoadFromCollection(oDatos, true);


            //Soja
            var j = 1;
            if (oDatos.Count > 0)
            {
                oPropRow = oDatos[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet9.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet9.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet9.Cells[1, j].Value != null)
            {
                workSheet9.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet9.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet9.Cells[1, j].Style.Font.Bold = true;

                j++;
            }


            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);
                return ms.ToArray();
            }

        }

        private static void ArmarAgrupacionExcel(ExcelWorksheet workSheet13, List<SugerenciaCupoAgrupadasExcel> agrupacion)
        {
            if (agrupacion != null && agrupacion.Count() > 0)
            {

                for (int i = 0; i < agrupacion.FirstOrDefault().DiaCupo.Count(); i++)
                {
                    var contador = 3;
                    workSheet13.Cells[1, (contador + i)].Value = agrupacion.FirstOrDefault().DiaCupo[i].Fecha.ToString("dd/MM/yyyy");
                    workSheet13.Column((contador + i)).AutoFit();
                }
                for (int i = 1; i <= agrupacion.Count(); i++)
                {
                    for (int k = 0; k < agrupacion.FirstOrDefault().DiaCupo.Count(); k++)
                    {
                        var f = 1;
                        var c = 3;
                        workSheet13.Cells[(f + i), (c + k)].Value = agrupacion[i - 1].DiaCupo[k].Cantidad;
                        workSheet13.Column((c + k)).AutoFit();

                    }
                };
            }
           

        }

        private ConfiguracionCupo DevolverCuperaLiberada(SugerenciaCupo sugerencia)
        {

            return repositorio.Obtener<ConfiguracionCupo>(x => x.MaterialId == sugerencia.MaterialId && x.CentroId == sugerencia.CentroId &&
             x.Fecha == sugerencia.FechaSugerida && x.CierreCupera != true && x.LiberarCupera == true);

        }
        private ConfiguracionCupo DevolverCuperaLiberada(int MaterialId, int CentroId, DateTime Fecha)
        {

            return repositorio.Obtener<ConfiguracionCupo>(x => x.MaterialId == MaterialId && x.CentroId == CentroId &&
             x.Fecha == Fecha && x.CierreCupera != true && x.LiberarCupera == true);

        }

        private int ObtenerCuposConsumidosPorFecha(DateTime fecha, SugerenciaCupo s)
        {
            return (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaIngreso) == fecha &&
           x.CentroId == s.CentroId && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9 && x.MaterialId == s.MaterialId)).Count;
        }

        private int ObtenerCuposConsumidosPorFecha(int MaterialId, int CentroId, DateTime fecha)
        {
            return (int)repositorio.Listar<Cupo>(x => DbFunctions.TruncateTime(x.FechaIngreso) == fecha &&
           x.CentroId == CentroId && (x.EstadoCupoId != 4 && x.EstadoCupoId != 9 && x.MaterialId == MaterialId)).Count;
        }

        public List<RespuestaCupoNoPropioStop> ConsultarMisTurnosActivos()
        {
            return clienteStopAgent.ConsultarMisTurnosActivos();
        }
    }
}