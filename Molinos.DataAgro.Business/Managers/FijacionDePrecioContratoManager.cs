using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net.Mail;

namespace Molinos.DataAgro.Business.Managers
{
    public class FijacionDePrecioContratoManager : IFijacionDePrecioContratoManager
    {
        private readonly IRepositorio repositorio;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly IPushNotificationManager mobjNotification;
        private readonly IFinalizarFijacionAgent oFinalizarFijacionAgent;
        private readonly IContratosParaFijacionAgent oContratosParaFijacionAgent;
        private readonly IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent;
        private readonly IMailManager mailManager;
        private readonly ILogger logger;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IValidarDocProcPagoAgent validarPagoAgente;
        private readonly IModificarFijacionAgent modificarFijacionAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;


        public FijacionDePrecioContratoManager(
            ILogger logger,
            IRepositorio repositorio,
            IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager,
            IPushNotificationManager oMSNotification,
            IFinalizarFijacionAgent oFinalizarFijacionAgent,
            IContratosParaFijacionAgent oContratosParaFijacionAgent,
            IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent,
            IMailManager mailManager, ILogDataAgroManager logDataAgroManager,
            IValidarDocProcPagoAgent validarPagoAgente, IModificarFijacionAgent modificarFijacionAgent, IDiasHabilesAgent diasHabilesAgent)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjProveedorManager = oMSProveedorManager;
            mobjComercialManager = oMSComercialManager;
            mobjNotification = oMSNotification;
            this.oFinalizarFijacionAgent = oFinalizarFijacionAgent;
            this.oContratosParaFijacionAgent = oContratosParaFijacionAgent;
            this.oRelacionCorredorProveedorAgent = oRelacionCorredorProveedorAgent;
            this.mailManager = mailManager;
            this.logDataAgroManager = logDataAgroManager;
            this.validarPagoAgente = validarPagoAgente;
            this.diasHabilesAgent = diasHabilesAgent;
            this.modificarFijacionAgent = modificarFijacionAgent;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public DatosIniAbmFijacionDePrecioContrato TraerDatosIniciales()
        {
            return new DatosIniAbmFijacionDePrecioContrato
            {
                material = repositorio.Listar<Material, MaterialQry>(x =>
                                                                new MaterialQry()
                                                                {
                                                                    MaterialId = x.MaterialId,
                                                                    Descripcion = x.Descripcion
                                                                }),

                moneda = repositorio.Listar<Moneda, MonedaQry>(x =>
                                                                new MonedaQry()
                                                                {
                                                                    MonedaId = x.MonedaId,
                                                                    Descripcion = x.Descripcion
                                                                }),

                comercial = repositorio.Listar<Comercial, ComercialQry>(x =>
                                                                new ComercialQry()
                                                                {
                                                                    ComercialId = x.ComercialId,
                                                                    Comercial = x.Nombres + " " + x.Apellido
                                                                }),

                proveedor = repositorio.Listar<Proveedor, ProveedorQry>(x =>
                                                                new ProveedorQry()
                                                                {
                                                                    ProveedorId = x.ProveedorId,
                                                                    Descripcion = x.RazonSocial
                                                                })
            };
        }

        public GrabarContratoResult GrabarAmpliacionFijacion(FijacionDePrecioContrato oFijacion)
        {
            var oFijacionDePrecioContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oFijacion.Id);
            var oEntityErrors = ValidarAmpliacionFijacion(oFijacionDePrecioContratoSave, oFijacion.Ampliaciones.Value);

            if (!oEntityErrors.HayError && oFijacionDePrecioContratoSave.Estado.EstadoContratoId != (int)EnumEstadoContrato.Finalizado && oFijacionDePrecioContratoSave.Estado.EstadoContratoId != (int)EnumEstadoContrato.Rechazado)
            {
                oFijacionDePrecioContratoSave.Ampliaciones = oFijacion.Ampliaciones.Value;
                oFijacionDePrecioContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Pendiente);
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioContratoSave.Id), TipoAccionLogDataAgro.Modificar, oFijacionDePrecioContratoSave.GetType());

                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede modificar");
            }
            return oEntityErrors;
        }
        private GrabarContratoResult ValidarAmpliacionFijacion(FijacionDePrecioContrato fijacion, double ampliacion)
        {
            var oEntityErrors = new GrabarContratoResult();
            var aFijar = TraerDatosFijacion(fijacion.Proveedor.CUIT, fijacion.Corredor != null ? fijacion.Corredor.CUIT : null, fijacion.MaterialId, fijacion.ContratoSAP.Remove(0, 3), fijacion.Id);
            double kgAplicados = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosAplicados, out kgAplicados) ? kgAplicados : 0;
            double pendiente = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosPendiente, out pendiente) ? pendiente - kgAplicados : 0;
            if (pendiente <= ampliacion)
            {
                oEntityErrors.Error("", "La ampliación supera la cantidad disponible");
            }
            return oEntityErrors;
        }
        private Resultado Validar(FijacionDePrecioContrato oParam, Resultado oErrorMessages, bool validacionesMinimas)
        {

            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacio");
            }
            if (oParam.MaterialId == 0)
            {
                oErrorMessages.Error("Material", "El campo 'Material' no debe estar vacio");
            }

            if (oParam.Cantidad == 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe estar vacio");
            }

            if (oParam.Cantidad < 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe ser negativo");
            }
            var cuitCorredor = oParam.CorredorId.HasValue ? mobjProveedorManager.TraerCuit(oParam.CorredorId.Value) : "";
            var cuitProveedor = mobjProveedorManager.TraerCuit(oParam.ProveedorId ?? 0);
            if (string.IsNullOrEmpty(oParam.ContratoSAP))
            {
                oErrorMessages.Error("ContratoId", "El campo 'Contrato' no debe estar vacio");
                return oErrorMessages;
            }
            if (validacionesMinimas)
            {
                var fijacion = oContratosParaFijacionAgent.ObtenerContratos(cuitProveedor, cuitCorredor, oParam.MaterialId, oParam.ContratoSAP, oParam.Id).FirstOrDefault();
                if (fijacion == null)
                {
                    oErrorMessages.Error("ContratoId", "El Contrato no existe");
                    return oErrorMessages;
                }
                else
                {
                    //double kilosContrato = 0;
                    //if (oParam.Id > 0)
                    //{
                    //    kilosContrato = repositorio.Obtener<Negocio, double>(a => a.Id == oParam.Id, a => a.Cantidad);
                    //}
                    var KilosPendiente = double.Parse(fijacion.KilosPendiente.Replace(".", "")) /*+ kilosContrato*/;
                    if (KilosPendiente < oParam.Cantidad)
                    {
                        oErrorMessages.Error("Cantidad", "La cantidad excede a los kilos del contrato");
                    }
                    oParam.TrigoEspecial = fijacion.Calidad.Value;
                }
            }
            if (oParam.Cantidad < 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe ser negativo");
            }
            if (oParam.Precio == 0 && oParam.Pizarra.HasValue && !oParam.Pizarra.Value)
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacio");
            }
            if (oParam.MonedaId == null && oParam.Pizarra.HasValue && !oParam.Pizarra.Value)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
            }
            if (oParam.ComercialId == 0)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacio");
            }
            if (validacionesMinimas)
            {
                var rangosPrecio = repositorio.Listar<RangoPrecio>();
                if (rangosPrecio.Exists(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId && (x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio)))
                {
                    oErrorMessages.Error("Precio", "Precio fuera de Rango");
                }
            }
            if (oParam.CampanaId == 0 || oParam.CampanaId == null)
            {
                oErrorMessages.Error("CampanaId", "Campaña del Contrato seleccionado fuera del rango");
            }
            if (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value && oParam.DiasPesificado == null)
            {
                oErrorMessages.Error("CampanaId", "Se debe completar la los Días en negocios de Pago Diferido");
            }
            if (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0 &&
                oParam.MonedaId != "ARP  ")
            {
                oErrorMessages.Error("pagoDiferido", "La Fijación de pago diferido siempre es en ARP");
            }

            if (oParam.AperturaPrecio != null)
            {
                if (oParam.Pizarra.HasValue && !oParam.Pizarra.Value)
                {
                    var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
                    if (!((concepto != null && (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value) && (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0)) ||
                        (concepto == null && (!oParam.PagoDiferido.HasValue || (oParam.PagoDiferido.HasValue && !oParam.PagoDiferido.Value)) && (!oParam.DiasPesificado.HasValue || (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value == 0)))))
                    {
                        oErrorMessages.Error("", "Días de diferimiento es obligatorio con el concepto Financiero");
                    }
                }
            }
            if (validacionesMinimas)
            {
                if (oParam.Id > 0)
                {
                    var fijacionSave = repositorio.Obtener<Negocio>(oParam.Id);
                    if ((oParam.FechaOperacion.Date != fijacionSave.Fecha.Date && oParam.FechaOperacion.Date < fijacionSave.Fecha.Date))
                    {

                        var diaAnterior = diasHabilesAgent.UltimoDiaHabil(fijacionSave.Fecha.Date);

                        if (oParam.FechaOperacion < diaAnterior.Date && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior))
                        {
                            oErrorMessages.Error("FechaOperacion", "La Fecha Operacion no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));

                        }

                        if (string.IsNullOrEmpty(oParam.MotivoOperacionAnterior))
                        {
                            oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la Fecha Operacion es anterior al día de la fecha.");
                        }
                    }
                    if (oParam.FechaOperacion.Date > fijacionSave.Fecha.Date)
                    {
                        oErrorMessages.Error("NoInformaSio", "Fecha de operación no puede ser mayor a " + fijacionSave.Fecha.ToString("dd/MM/yyyy"));
                    }
                }
                else
                {
                    if (oParam.FechaOperacion.Date < DateTime.Now.Date)
                    {
                        var diaAnterior = diasHabilesAgent.UltimoDiaHabil(null);

                        if (oParam.FechaOperacion.Date < diaAnterior.Date && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior))
                        {
                            oErrorMessages.Error("FechaOperacion", "La Fecha Operación no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));

                        }

                        if (string.IsNullOrEmpty(oParam.MotivoOperacionAnterior))
                        {
                            oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la Fecha Operacion es anterior al día de la fecha.");
                        }
                    }
                }
            }
            if (oParam.Dolarizado.HasValue && oParam.Dolarizado.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }
            if (oParam.FechaDolarizado.HasValue && oParam.FechaDolarizado.Value < DateTime.Now.Date)
            {
                oErrorMessages.Error("dolarizado", "La fecha de dolarizado no es válida");

            }
            if(oParam.EstadoId == 5 && oParam.DolarizadoExpress.HasValue && oParam.DolarizadoExpress.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }
            return oErrorMessages;
        }

        public GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio)
        {
            var oEntityErrors = new GrabarFijacionResult();
            this.Validar(oFijacionDePrecio, oEntityErrors, true);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            var hoy = DateTime.Now;

            var oFijacionDePrecioSave = oFijacionDePrecio;
            var fijacionSap = oFijacionDePrecio.ContratoSAP.PadLeft(10, '0');
            var oContratoId = repositorio.Obtener<Contrato>(x => x.ContratoSAP == fijacionSap);

            var tipoCambio = oFijacionDePrecio.Id != 0 ? TipoAccionLogDataAgro.Modificar : TipoAccionLogDataAgro.Crear;
            var fechaDolarizado = oFijacionDePrecio.FechaOperacion.AddDays(30);
            var proveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oFijacionDePrecio.ProveedorId, x => x.CUIT);
            var corredor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oFijacionDePrecio.CorredorId, x => x.CUIT);
            CargarDolarizado(oFijacionDePrecio, oFijacionDePrecioSave, oContratoId, fechaDolarizado, proveedor, corredor);

            if (oFijacionDePrecio.Id != 0)
            {
                oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecioContrato>(oFijacionDePrecio.Id);
                if ((oFijacionDePrecio.ChequeElectronico != oFijacionDePrecioSave.ChequeElectronico && oFijacionDePrecio.ChequeElectronico.Value) || oFijacionDePrecioSave.PagoCBU != oFijacionDePrecio.PagoCBU)
                {
                    var result = validarPagoAgente.ValidarEstado(oFijacionDePrecioSave.ContratoSAP, oFijacionDePrecioSave.FijacionSAP);
                    if (result != "Ok")
                    {
                        oEntityErrors.Error("", result);
                        return oEntityErrors;
                    }
                }
                if (oFijacionDePrecioSave.EstadoId == 6)
                {
                    oEntityErrors.Error("", "La Fijación no se puede modificar");
                    return oEntityErrors;
                }
                if ((oFijacionDePrecioSave.Precio != oFijacionDePrecio.Precio || oFijacionDePrecioSave.Cantidad != oFijacionDePrecio.Cantidad) && (oFijacionDePrecioSave.EstadoId != 1 && oFijacionDePrecioSave.EstadoId != 3))
                {
                    if (oFijacionDePrecioSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                    {
                        string jsonContrato = JsonConvert.SerializeObject(oFijacionDePrecioSave, new JsonSerializerSettings()
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects
                        });
                        oFijacionDePrecioSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oFijacionDePrecioSave.Id, TipoNegocioId = oFijacionDePrecioSave.TipoNegocioId, ComercialId = oFijacionDePrecioSave.ComercialId });

                    }

                    oFijacionDePrecioSave.EstadoId = 7;

                }



                oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                oFijacionDePrecioSave.Cantidad = oFijacionDePrecio.Cantidad;
                oFijacionDePrecioSave.Ampliaciones = oFijacionDePrecio.Ampliaciones;
                oFijacionDePrecioSave.Observacion = oFijacionDePrecio.Observacion;
                oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
                oFijacionDePrecioSave.ComercialId = oFijacionDePrecio.ComercialId;
                oFijacionDePrecioSave.ContratoId = oContratoId.Id != 0 ? oContratoId.Id : (int?)null;
                oFijacionDePrecioSave.MonedaId = oFijacionDePrecio.MonedaId;
                oFijacionDePrecioSave.MaterialId = oFijacionDePrecio.MaterialId;
                oFijacionDePrecioSave.CorredorId = oFijacionDePrecio.CorredorId;
                oFijacionDePrecioSave.ContratoSAP = oFijacionDePrecio.ContratoSAP.PadLeft(10, '0');
                oFijacionDePrecioSave.CampanaId = oFijacionDePrecio.CampanaId;
                oFijacionDePrecioSave.Posicion = oFijacionDePrecio.Posicion;
                oFijacionDePrecioSave.TrigoEspecial = oFijacionDePrecio.TrigoEspecial;
                oFijacionDePrecioSave.FechaDesde = oFijacionDePrecio.FechaDesde;
                oFijacionDePrecioSave.FechaHasta = oFijacionDePrecio.FechaHasta;
                oFijacionDePrecioSave.PrecioNeto = oFijacionDePrecio.PrecioNeto;
                oFijacionDePrecioSave.Pizarra = oFijacionDePrecio.Pizarra;
                oFijacionDePrecioSave.DiasPesificado = oFijacionDePrecio.DiasPesificado;
                oFijacionDePrecioSave.PagoDiferidoContrato = oFijacionDePrecio.PagoDiferidoContrato;
                oFijacionDePrecioSave.DestinoId = oFijacionDePrecio.DestinoId;
                oFijacionDePrecioSave.FechaOperacion = oFijacionDePrecio.FechaOperacion;
                oFijacionDePrecioSave.ChequeElectronico = oFijacionDePrecio.ChequeElectronico;
                oFijacionDePrecioSave.PagoCBU = oFijacionDePrecio.PagoCBU;
                oFijacionDePrecioSave.MotivoOperacionAnterior = oFijacionDePrecio.MotivoOperacionAnterior;
                CargarDolarizado(oFijacionDePrecio, oFijacionDePrecioSave, oContratoId, fechaDolarizado, proveedor, corredor);
                if (oFijacionDePrecio.AperturaPrecio != null)
                {
                    var aperturas = repositorio.Listar<AperturaPrecio>(x => x.NegocioId != null && x.NegocioId == oFijacionDePrecioSave.Id);
                    repositorio.RemoverTodos(aperturas);
                    oFijacionDePrecioSave.AperturaPrecio = oFijacionDePrecio.AperturaPrecio;
                }
            }
            else
            {

                oFijacionDePrecio.ContratoSAP = oFijacionDePrecio.ContratoSAP.PadLeft(10, '0');
                oFijacionDePrecio.Fecha = DateTime.Now;
                if (oContratoId != null)
                {
                    if (oContratoId.Id == 0)
                    {
                        oFijacionDePrecioSave.ContratoId = null;
                    }
                    else
                    {
                        oFijacionDePrecioSave.ContratoId = oContratoId.Id;
                    }
                }
                repositorio.Agregar(oFijacionDePrecioSave);
            }


            if (oFijacionDePrecio.EstadoId < (int)EnumEstadoContrato.PreAprobacion && ConfirmacionAutomatica(oFijacionDePrecioSave))
            {
                oFijacionDePrecioSave.FechaConfirmacion = DateTime.Now;
                oFijacionDePrecioSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                logger.Debug("El contrato " + oFijacionDePrecioSave.Id + " se finalizo automaticamente por estar dentro de los rangos configurados");
            }
            try
            {
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), tipoCambio, oFijacionDePrecioSave.GetType());

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }

        private void CargarDolarizado(FijacionDePrecioContrato oFijacionDePrecio, FijacionDePrecioContrato oFijacionDePrecioSave, Contrato oContratoId, DateTime fechaDolarizado, string proveedor, string corredor)
        {
            if (oContratoId != null)
            {
                var datoContrato = TraerDatosFijacion(proveedor, corredor, oFijacionDePrecio.MaterialId, oContratoId.ContratoSAP.Remove(0, 3), 0);
                if (datoContrato != null && oFijacionDePrecio.FechaDolarizado != null)
                {
                    oFijacionDePrecioSave.DolarizadoCorredor = datoContrato.First().Clasificacion.ToUpper() != "PRODUCTOR" || oFijacionDePrecio.CorredorId != null ? true : false;
                }
            }

            oFijacionDePrecioSave.Dolarizado = fechaDolarizado != null ? (oFijacionDePrecio?.FechaDolarizado > fechaDolarizado && (oFijacionDePrecioSave.DolarizadoCorredor == false || oFijacionDePrecioSave.DolarizadoCorredor == null) ? true : false) : false;
            oFijacionDePrecioSave.DolarizadoExpress = fechaDolarizado != null ? (oFijacionDePrecio?.FechaDolarizado <= fechaDolarizado && (oFijacionDePrecioSave.DolarizadoCorredor == false || oFijacionDePrecioSave.DolarizadoCorredor == null) ? true : false) : false;
            oFijacionDePrecioSave.FechaDolarizado = oFijacionDePrecio.FechaDolarizado;
        }

        private bool ConfirmacionAutomatica(FijacionDePrecioContrato contrato)
        {
            var hoy = DateTime.Now;
            var precioContrato = contrato.Precio;

            var rangos = repositorio.Listar<RangoConfirmacionAutomatica>(x =>
           x.TipoNegocioId == 3 &&
           x.FechaDesde <= hoy &&
           x.FechaHasta >= hoy &&
           x.MaterialId == contrato.MaterialId &&
           x.MonedaId == contrato.MonedaId &&
           precioContrato >= x.PrecioMinimo && precioContrato <= x.PrecioMaximo) ?? new List<RangoConfirmacionAutomatica>();

            var rango = rangos.FirstOrDefault(
                   //x => contrato.FechaDesde >= new DateTime(x.DesdeAnio, x.DesdeMes, 1) &&
                   //   contrato.FechaHasta <= new DateTime(x.HastaAnio, x.HastaMes, DateTime.DaysInMonth(x.HastaAnio, x.HastaMes))
                   );

            if (rango != null)
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                var cantidad =
                    repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.Fecha == hoy &&
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id
                && x.MaterialId == rango.MaterialId);

                var total = cantidad.Sum();
                var valor =
                    (total + contrato.Cantidad) <= rango.Cantidad &&
                    (rango.ZonaId == 47 || rango.ZonaId == null || grupo == rango.ZonaId);
                return valor;
            }
            else
            {
                return false;
            }
        }
        public GrabarFijacionResult ConfirmarFijacion(int fijacionDePrecioContratoId, int usuarioConfirmador)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecioContrato>(fijacionDePrecioContratoId);

            if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Pendiente || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Oferta || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                if (oFijacionDePrecioSave.Ampliaciones != null)
                {
                    oFijacionDePrecioSave.Cantidad += oFijacionDePrecioSave.Ampliaciones.Value;
                    oFijacionDePrecioSave.Ampliaciones = 0;
                }

                oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Confirmado);

                try
                {
                    oFijacionDePrecioSave.UsuarioConfirmadorId = usuarioConfirmador;
                    oFijacionDePrecioSave.FechaConfirmacion = DateTime.Now;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), TipoAccionLogDataAgro.Crear, oFijacionDePrecioSave.GetType());


                    var comerciales = mobjComercialManager.CadenaComerciales(oFijacionDePrecioSave.Comercial.ComercialId);
                    foreach (var comercialId in comerciales)
                    {
                        EnviarNotificacion(comercialId, oFijacionDePrecioSave);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede confirmar");
            }

            return oEntityErrors;
        }

        public GrabarFijacionResult FinalizarFijacion(int fijacionDePrecioContratoId, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecioContrato>(fijacionDePrecioContratoId);

            if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
            {
                try
                {
                    oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);
                    var objApertura = repositorio.Listar<AperturaPrecio>(x => x.NegocioId == oFijacionDePrecioSave.Id);

                    if (objApertura == null)
                    {
                        var conceptos = repositorio.Listar<ConceptoAperturaPrecio>();
                        oFijacionDePrecioSave.AperturaPrecio = new List<AperturaPrecio>();
                        foreach (ConceptoAperturaPrecio concepto in conceptos)
                        {
                            oFijacionDePrecioSave.AperturaPrecio.Add(new AperturaPrecio
                            {
                                ConceptoAperturaPrecio = concepto,
                                Importe = 0,
                                Moneda = null,
                                Porcentaje = 0
                            });
                        }
                    }
                    else
                    {
                        oFijacionDePrecioSave.AperturaPrecio = objApertura;
                    }

                    string nroFijacionSAP = SapFinalizarFijacion(oFijacionDePrecioSave);
                    try
                    {
                        oFijacionDePrecioSave.FijacionSAP = nroFijacionSAP;
                    }
                    catch (Exception e)
                    {
                        oFijacionDePrecioSave.FijacionSAP = "";
                        logger.Error(e);
                    }
                    try
                    {
                        //Envio de mail
                        mobjProveedorManager.EnviarEmailFijacion(oFijacionDePrecioSave, idActiveDirectory);

                        repositorio.GuardarCambios();
                        logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), TipoAccionLogDataAgro.Crear, oFijacionDePrecioSave.GetType());


                        var comerciales = mobjComercialManager.CadenaComerciales(oFijacionDePrecioSave.Comercial.ComercialId);
                        foreach (var comercialId in comerciales)
                        {
                            EnviarNotificacion(comercialId, oFijacionDePrecioSave);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }//Envio de mail

                }
                catch (Exception ex)
                {
                    oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "La Fijación ya se encuentra Finalizada");
                }
                else if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "La Fijación ya ha sido Rechazada");
                }
                else if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Pendiente || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Oferta)
                {
                    oEntityErrors.Error("", "La Fijación debe ser Confirmada");
                }
            }
            return oEntityErrors;
        }
        private string SapFinalizarFijacion(FijacionDePrecioContrato fijacion)
        {
            return oFinalizarFijacionAgent.Finalizar(fijacion);
        }
        public GrabarContratoResult BorrarFijacion(FijacionDePrecioContrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(oContrato.MotivoRechazo) || string.IsNullOrWhiteSpace(oContrato.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oContrato.Id);

            oContratoSave.MotivoRechazo = oContrato.MotivoRechazo;
            if (oContratoSave.Estado.EstadoContratoId < (int)EnumEstadoContrato.Finalizado || oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                {
                    if (oContratoSave.Ampliaciones > 0)
                    {
                        oContratoSave.Ampliaciones = 0;
                        if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                        }
                        else
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Pendiente;
                        }
                    }
                    else
                    {
                        var historico = oContratoSave.NegocioHistorico.LastOrDefault();
                        if (historico != null)
                        {
                            FijacionDePrecioContrato contratoOriginal = JsonConvert.DeserializeObject<FijacionDePrecioContrato>(historico.Datos);
                            oContratoSave.MaterialId = contratoOriginal.MaterialId;
                            oContratoSave.TipoNegocioId = contratoOriginal.TipoNegocioId;
                            oContratoSave.Cantidad = contratoOriginal.Cantidad;
                            oContratoSave.Precio = contratoOriginal.Precio;
                            oContratoSave.CampanaId = contratoOriginal.CampanaId;
                            oContratoSave.FechaDesde = contratoOriginal.FechaDesde;
                            oContratoSave.FechaHasta = contratoOriginal.FechaHasta;
                            oContratoSave.ProveedorId = contratoOriginal.ProveedorId;
                            oContratoSave.MonedaId = contratoOriginal.MonedaId;
                            oContratoSave.GrupoCompra = contratoOriginal.GrupoCompra;
                            oContratoSave.ComercialId = contratoOriginal.ComercialId;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.FechaDolarizado = contratoOriginal.FechaDolarizado;
                            oContratoSave.DiasPesificado = contratoOriginal.DiasPesificado;
                            oContratoSave.TrigoEspecial = contratoOriginal.TrigoEspecial;
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.Ampliaciones = contratoOriginal.Ampliaciones;
                            oContratoSave.Observacion = contratoOriginal.Observacion;
                            oContratoSave.DestinoId = contratoOriginal.DestinoId;
                            oContratoSave.CondicionFijacionId = contratoOriginal.CondicionFijacionId;
                            oContratoSave.CD = contratoOriginal.CD;
                            oContratoSave.Warrant = contratoOriginal.Warrant;
                            oContratoSave.DesdeFijacion = contratoOriginal.DesdeFijacion;
                            oContratoSave.HastaFijacion = contratoOriginal.HastaFijacion;
                            oContratoSave.ComercialCreadorId = contratoOriginal.ComercialCreadorId;
                            oContratoSave.CorredorId = contratoOriginal.CorredorId;
                            oContratoSave.PrecioNeto = contratoOriginal.PrecioNeto;
                            oContratoSave.StandardDeCalidadId = contratoOriginal.StandardDeCalidadId;
                            oContratoSave.Pizarra = contratoOriginal.Pizarra;
                            oContratoSave.PagoDiferido = contratoOriginal.PagoDiferido;
                            oContratoSave.Dolarizado = contratoOriginal.Dolarizado;
                            oContratoSave.ContratoSAP = contratoOriginal.ContratoSAP;
                            oContratoSave.ContratoId = contratoOriginal.ContratoId;
                            oContratoSave.Posicion = contratoOriginal.Posicion;
                            oContratoSave.PagoDiferidoContrato = contratoOriginal.PagoDiferidoContrato;
                            oContratoSave.ChequeElectronico = contratoOriginal.ChequeElectronico;
                            oContratoSave.PagoCBU = contratoOriginal.PagoCBU;
                            oContrato.FechaOperacion = contratoOriginal.FechaOperacion;

                            if (oContratoSave.AperturaPrecio != null)
                            {
                                for (int i = oContratoSave.AperturaPrecio.Count - 1; i > -1; i--)
                                {
                                    repositorio.Remover(oContratoSave.AperturaPrecio.First());
                                }
                            }
                            else
                            {
                                oContratoSave.AperturaPrecio = new List<AperturaPrecio>();
                            }

                            if (contratoOriginal.AperturaPrecio != null)
                            {
                                foreach (var apertura in contratoOriginal.AperturaPrecio)
                                {
                                    repositorio.Agregar(new AperturaPrecio { ConceptoAperturaPrecioId = apertura.ConceptoAperturaPrecioId, Importe = apertura.Importe, MonedaId = apertura.MonedaId, NegocioId = apertura.NegocioId, Porcentaje = apertura.Porcentaje });
                                }
                            }
                        }
                        else
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                        }
                    }


                }
                else
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                }
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());


                    var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.Comercial.ComercialId);
                    foreach (var comercialId in comerciales)
                    {
                        EnviarNotificacion(comercialId, oContratoSave);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede rechazar");
            }
            return oEntityErrors;
        }
        public GrabarContratoResult BorrarFijacionPreAprobacion(int id, string motivo)
        {
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(id);

            if (oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.PreAprobacion)
            {
                oContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Rechazado);
                oContratoSave.MotivoRechazo = motivo;
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());


                    EnviarMailRechazo(oContratoSave);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede rechazar");
            }
            return oEntityErrors;
        }
        private void EnviarNotificacion(int comercialId, FijacionDePrecioContrato fijacion)
        {
            var tokens = repositorio.Listar<SuscripcionComercial>(x => x.ComercialId == comercialId);
            var title = "";
            var message = "";
            var hora = DateTime.Now.ToString("hh:mm");
            if (fijacion.EstadoId == 2)
            {
                title = "Contrato Confirmado";
                message = "La fijación del contrato " + fijacion.ContratoId + " ha sido confirmado a las " + hora;
            }
            else if (fijacion.EstadoId == 5)
            {
                title = "Contrato Finalizado";
                message = "La fijación del contrato  " + fijacion.ContratoId + " ha sido finalizado a las " + hora + " por " + fijacion.Comercial.Nombres + " " + fijacion.Comercial.Apellido;
            }
            else if (fijacion.EstadoId == 6)
            {
                title = "Contrato Rechazado";
                message = "La fijación del contrato  " + fijacion.ContratoId + " ha sido rechazado a las " + hora;
            }
            var url = "/CompraNet";
            foreach (var to in tokens)
            {
                mobjNotification.QueueMessage(to.Key, title, message, url);
            }
        }

        public BasicoContrato TraerFijacion(int id)
        {
            var sap = repositorio.Obtener<FijacionDePrecioContrato>(id);
            var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.ContratoSAP == sap.ContratoSAP && x.Id != id).Sum();
            var contrato = repositorio.Obtener<FijacionDePrecioContrato, BasicoContrato>(x => x.Id == id, fijac => new BasicoContrato
            {
                Id = fijac.Id,
                Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial + " " + "(" + fijac.Proveedor.CUIT + ")",
                ContratoId = fijac.ContratoId.HasValue ? fijac.ContratoId.Value : 0,
                ProveedorId = fijac.ProveedorId ?? 0,
                ComercialId = fijac.ComercialId,
                MaterialId = fijac.MaterialId,
                TipoNegocioId = 3,
                Cantidad = fijac.Cantidad,
                Precio = fijac.Precio,
                MonedaId = fijac.MonedaId,
                Moneda = fijac.Moneda == null ? "" : fijac.Moneda.Descripcion,
                FechaFormateado = SqlFunctions.DateName("day", fijac.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.Fecha),
                Fecha_Order = fijac.Fecha,
                FechaDesdeFormateado = SqlFunctions.DateName("day", fijac.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", fijac.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaHasta),
                GrupoCompra = 0,
                MonedaId_Sustentable = "",
                Moneda_Sustentable = "",
                Estado = fijac.EstadoId,
                Estado_Contrato = fijac.Estado.Descripcion,
                Estado_Order = fijac.Estado.Orden,
                UsuarioId = "",
                Ampliaciones = fijac.Ampliaciones,
                Cuit = fijac.Proveedor == null ? "" : fijac.Proveedor.CUIT,
                Comercial = fijac.Comercial == null ? "" : fijac.Comercial.Nombres + " " + fijac.Comercial.Apellido,
                Material = fijac.Material == null ? "" : fijac.Material.Descripcion,
                CampanaId = fijac.CampanaId ?? 0,
                Campania = fijac.Campana.Descripcion,
                Provincia = "",
                TipoNegocio = "FIJACION",
                Localidad = "",
                Observacion = fijac.Observacion ?? "",
                FijacionDePrecioContratoId = fijac.Id,
                Sustentable = false,
                Dolarizado = fijac.Dolarizado.Value,
                Pesificado = false,
                TrigoEspecial = fijac.TrigoEspecial,
                Posicion = fijac.Posicion,
                DestinoId = fijac.DestinoId,
                DestinoDescripcion = fijac.Destino.Descripcion,
                Consignatario = false,
                PlanCanje = false,
                EstablecimientoPropio = false,
                BoletoDescripcion = "",
                BolsaDescripcion = "",
                CondicionFijacionDescripcion = "",
                ClasificacionDescripcion = "",
                Corredor = fijac.Corredor == null ? "" : fijac.Corredor.RazonSocial + " " + "(" + fijac.Corredor.CUIT + ")",
                Pizarra = fijac.Pizarra ?? false,
                Dias_Pesificado = fijac.DiasPesificado,
                PagoDiferido = fijac.PagoDiferido,
                Fecha_DolarizadoFormateado = fijac.FechaDolarizado != null ? SqlFunctions.DateName("day", fijac.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaDolarizado) : "",
                DatosFijacion = new DatosFijacionDeContratoDto()
                {
                    ContratoId = fijac.ContratoSAP.ToString(),
                    KilosAplicados = cantidad.ToString(),
                    KilosPendiente = fijac.Contrato != null ? ((double?)fijac.Contrato.Cantidad - cantidad).ToString() : "0",
                    FechaDesde = fijac.Contrato.DesdeFijacion.HasValue ? SqlFunctions.DateName("day", fijac.Contrato.DesdeFijacion).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)fijac.Contrato.DesdeFijacion.Value.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", fijac.Contrato.DesdeFijacion) : "",
                    FechaHasta = fijac.Contrato.HastaFijacion.HasValue ? SqlFunctions.DateName("day", fijac.Contrato.HastaFijacion).Trim() + "/" +
                                           SqlFunctions.StringConvert((double)fijac.Contrato.HastaFijacion.Value.Month).TrimStart() + "/" +
                                           SqlFunctions.DateName("year", fijac.Contrato.HastaFijacion) : "",
                    PagoDiferido = fijac.PagoDiferidoContrato,
                },
                FechaDesde = fijac.FechaDesde,
                FechaHasta = fijac.FechaHasta,
                Fecha = fijac.Fecha,
                FechaOperacion = fijac.FechaOperacion,
                ChequeElectronico = fijac.ChequeElectronico,
                PagoCBU = fijac.PagoCBU,
                MotivoOperacionAnterior = fijac.MotivoOperacionAnterior,
                FechaOperacionFormateado = SqlFunctions.DateName("day", fijac.FechaOperacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaOperacion.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaOperacion),
                ObservacionTercero = fijac.ObservacionTercero,
                DolarizadoCorredor = fijac.DolarizadoCorredor.Value,
                DolarizadoExpress = fijac.DolarizadoExpress.Value,
                Fecha_Dolarizado = fijac.FechaDolarizado
            });
            contrato.DatosFijacion.ContratoId = contrato.DatosFijacion.ContratoId.TrimStart('0');
            if (contrato.ContratoId != 0)
            {
                contrato.DatosFijacion = FechaString(contrato.DatosFijacion);
                contrato.DatosFijacion.KilosAplicados = (double.Parse(contrato.DatosFijacion.KilosAplicados)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
                contrato.DatosFijacion.KilosPendiente = (double.Parse(contrato.DatosFijacion.KilosPendiente)).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"));
            }
            contrato.AperturaPrecios = TraerAperturaDePrecioPorFijacion(contrato.Id);

            return contrato;
        }
        private DatosFijacionDeContratoDto FechaString(DatosFijacionDeContratoDto datos)
        {
            datos.FechaDesde = FechaConCeros(datos.FechaDesde.Split('/'));
            datos.FechaHasta = FechaConCeros(datos.FechaHasta.Split('/'));
            return datos;
        }
        private string FechaConCeros(string[] numero)
        {
            if(numero.Length == 1)
            {
                return "";
            }
            for (var i = 0; i < 2; i++)
            {
                if (int.Parse(numero[i]) < 10)
                {
                    numero[i] = '0' + numero[i];
                }
            }
            var fecha = numero[0] + '/' + numero[1] + '/' + numero[2];
            return fecha;
        }
        public List<DatosFijacionDeContratoDto> TraerDatosFijacion(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int fijacionId)
        {
            return oContratosParaFijacionAgent.ObtenerContratos(CuitProveedor, CuitCorredor, materialId, filtro, fijacionId);
        }

        public List<AperturaPrecioDto> TraerAperturaDePrecioPorFijacion(int fijacionId)
        {
            return repositorio.Listar<AperturaPrecio, AperturaPrecioDto>(apertura => new AperturaPrecioDto()
            {
                contratoId = apertura.NegocioId,
                Id = apertura.Id,
                ConceptoAperturaPrecio = apertura.ConceptoAperturaPrecio.Descripcion,
                ConceptoAperturaPrecioId = apertura.ConceptoAperturaPrecioId,
                Importe = apertura.Importe,
                MonedaId = apertura.MonedaId,
                Porcentaje = apertura.Porcentaje
            },
            x => x.NegocioId == fijacionId);
        }
        public void FinalizacionAutomatica(string idActiveDirectory)
        {
            var fijacioneConfirmados = repositorio.Listar<FijacionDePrecioContrato>(x => x.EstadoId == 2 || x.EstadoId == 4);
            logger.Debug("Fijaciones a Finalizar: " + fijacioneConfirmados.Count);
            var oEntityErrors = new GrabarContratoResult();
            foreach (var fijacion in fijacioneConfirmados)
            {
                try
                {
                    var error = FinalizarFijacion(fijacion.Id, idActiveDirectory);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    oEntityErrors.Error("", ex.Message);
                }
            }
        }
        public GrabarFijacionResult AprobarFijacion(int id)
        {
            var oEntityErrors = new GrabarFijacionResult();
            var oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecioContrato>(id);

            if (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.PreAprobacion)
            {
                if (ConfirmacionAutomatica(oFijacionDePrecioSave))
                {
                    oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Confirmado);

                }
                else
                {
                    oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Pendiente);
                }
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), TipoAccionLogDataAgro.Crear, oFijacionDePrecioSave.GetType());


                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede aprobar");
            }

            return oEntityErrors;
        }
        private void EnviarMailRechazo(FijacionDePrecioContrato fijacion)
        {
            var id = fijacion.CorredorId.HasValue ? fijacion.CorredorId : fijacion.ProveedorId;
            var enviarA = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == id && x.CompraNet == true);
            var asunto = ConfigurationManager.AppSettings["AmbientePruebas"] != "1" ? "" : "Prueba - ";
            asunto += "Rechazo Fijación Molinos Agro S.A. –  " + fijacion.Proveedor.RazonSocial;
            var copia = new List<string>() { fijacion.Comercial.IdActiveDirectory, ConfigurationManager.AppSettings["CredentialUserName"] };
            var vista = CuerpoMailFijacion(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), fijacion);
            mailManager.EnviarMail(enviarA, asunto, "", copia, vista);
        }
        private AlternateView CuerpoMailFijacion(string filePath, FijacionDePrecioContrato fijacion)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            var mail = "";
            try { mail = mailManager.GetEmailUserActiveDirectory(fijacion.Comercial.IdActiveDirectory); } catch (Exception e) { logger.Error("No existe mail para el usuario en AD" + e.Message); }

            var contacto = fijacion.Comercial != null ? fijacion.Comercial.Nombres + " " + fijacion.Comercial.Apellido + (!string.IsNullOrEmpty(mail) ? " (" + mail + ")." : ".") : "Mesa de Ayuda.";
            var htmlBody = $"En el presente mail, se informa que el negocio generado con Molinos Agro S.A. ha sido rechazado <br />" +
                $"Motivo: <br />  {fijacion.MotivoRechazo} <br />" +
                $"Ante cualquier consulta contactarse con {contacto}" +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.   <br /><br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            htmlBody += "<style> table, th, td{ }</style>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        public GrabarFijacionResult ActualizarFijacion(FijacionDePrecioContrato oContrato)
        {
            var error = new GrabarFijacionResult();
            try
            {
                var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oContrato.Id);
                oContrato.ContratoSAP = oContratoSave.ContratoSAP;

                if ((oContrato.ChequeElectronico != oContratoSave.ChequeElectronico && oContrato.ChequeElectronico.Value) || oContratoSave.PagoCBU != oContrato.PagoCBU)
                {
                    var result = validarPagoAgente.ValidarEstado(oContratoSave.ContratoSAP, oContratoSave.FijacionSAP);
                    if (result != "Ok")
                    {
                        error.Error("", result);
                        return error;
                    }
                }
                oContrato.ContratoSAP = repositorio.Obtener<FijacionDePrecioContrato, string>(x => x.Id == oContrato.Id, x => x.ContratoSAP);

                oContrato.Dolarizado = oContratoSave.DolarizadoCorredor == true ? false : oContrato.Dolarizado;
                oContrato.DolarizadoExpress = oContrato.DolarizadoExpress;
                oContrato.DolarizadoCorredor = oContrato.DolarizadoExpress == true ? false : oContratoSave.DolarizadoCorredor;

                var res = modificarFijacionAgent.Modificar(oContrato, oContratoSave);
                if (res != "Se actualizaron los datos correctamente")
                {
                    error.Error("SAP", res);
                     return error;
                }

                logger.Debug("Actualizando fijacion en BD DataAgro: " + oContrato.Id);

                if (oContratoSave == null || oContrato.Id == 0)
                {
                    error.Error("Fijacion", "No existe contrato en DataAgro");
                }
                oContratoSave.ChequeElectronico = oContrato.ChequeElectronico;
                oContratoSave.PagoCBU = oContrato.PagoCBU;
                oContratoSave.Dolarizado = oContratoSave.Dolarizado;
                oContratoSave.DolarizadoExpress = oContrato.DolarizadoExpress;
                oContratoSave.DolarizadoCorredor = oContrato.DolarizadoCorredor;
                oContratoSave.FechaDolarizado = oContrato.FechaDolarizado;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oContratoSave.Id), TipoAccionLogDataAgro.Modificar, oContrato.GetType());

            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Error("", e.Message + ".");
            }
            return error;
        }

        public List<DateTime> FechaFeriados()
        {
            return repositorio.Listar<FechaFeriado>().Select(x => x.Feriado).ToList();
        }

        public DateTime UltimoDiaHabil()
        {
            Nullable<DateTime> fecha = null;

            return diasHabilesAgent.UltimoDiaHabil(fecha);
        }
      
        public Resultado ActualizarFijacionSap(FijacionDePrecioContrato fijacion)
        {
            var error = new Resultado();
            try
            {                     
            logger.Debug("Actualizando contrato en BD DataAgro: " + fijacion.Id);
            var contratoSave = repositorio.Obtener<FijacionDePrecioContrato>(x => x.FijacionSAP.Contains(fijacion.FijacionSAP));
            if (contratoSave == null || contratoSave.Id == 0)
            {
                error.Error("Fijacion", "No existe la fijacion en DataAgro");
            }
            //this.Validar(fijacion, error);
            if (error.Errores.Count > 0)
            {
                return error;
            }
            contratoSave.ChequeElectronico = fijacion.ChequeElectronico;
            contratoSave.PagoCBU = fijacion.PagoCBU;
                //contratoSave.Precio = fijacion.Precio;
                //contratoSave.Cantidad = fijacion.Cantidad;
                //contratoSave.Ampliaciones = fijacion.Ampliaciones;
                //contratoSave.Observacion = fijacion.Observacion;
                //contratoSave.ProveedorId = fijacion.ProveedorId;
                //contratoSave.ComercialId = fijacion.ComercialId;
                //contratoSave.ContratoId = fijacion.ContratoId;
                //contratoSave.MonedaId = fijacion.MonedaId;
                //contratoSave.MaterialId = fijacion.MaterialId;
                //contratoSave.CorredorId = fijacion.CorredorId;
                //contratoSave.ContratoSAP = fijacion.ContratoSAP.PadLeft(10, '0');
                //contratoSave.CampanaId = fijacion.CampanaId;
                //contratoSave.Posicion = fijacion.Posicion;
                //contratoSave.TrigoEspecial = fijacion.TrigoEspecial;
                //contratoSave.FechaDesde = fijacion.FechaDesde;
                //contratoSave.FechaHasta = fijacion.FechaHasta;
                //contratoSave.PrecioNeto = fijacion.PrecioNeto;
                //contratoSave.Pizarra = fijacion.Pizarra;
                //contratoSave.DiasPesificado = fijacion.DiasPesificado;
                //contratoSave.PagoDiferidoContrato = fijacion.PagoDiferidoContrato;
                //contratoSave.DestinoId = fijacion.DestinoId;
                //contratoSave.FechaOperacion = fijacion.FechaOperacion;            
                //contratoSave.MotivoOperacionAnterior = fijacion.MotivoOperacionAnterior;

                //if (fijacion.AperturaPrecio != null)
                //{
                //    var aperturas = repositorio.Listar<AperturaPrecio>(x => x.NegocioId != null && x.NegocioId == contratoSave.Id);
                //    repositorio.RemoverTodos(aperturas);
                //    contratoSave.AperturaPrecio = fijacion.AperturaPrecio;
                //}         

            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(contratoSave.Id), TipoAccionLogDataAgro.Modificar, fijacion.GetType());
            }
            catch (Exception e)
            {

                logger.Error("", e.Message);
            }
            return error;
        }

        public Resultado AltaFijacionSap(FijacionDePrecioContrato fijacion)
        {
            var error = new Resultado();
            try
            {
                logger.Debug("Alta fijacion en BD DataAgro: " + fijacion.Id);
                var fijarSave = repositorio.Obtener<Contrato>(x => x.ContratoSAP.Contains(fijacion.ContratoSAP));
                if (fijarSave == null || fijarSave.Id == 0)
                {
                    error.Error("Fijacion", "No existe el contrato en DataAgro");
                }               
                this.Validar(fijacion, error, false);
                if (error.Errores.Count > 0)
                {
                    return error;
                }
                var fijacionSave = new FijacionDePrecioContrato();
                fijacionSave.FijacionSAP = fijacion.FijacionSAP;
                fijacionSave.ChequeElectronico = fijacion.ChequeElectronico;
                fijacionSave.PagoCBU = fijacion.PagoCBU;             
                fijacionSave.TrigoEspecial = fijacion.TrigoEspecial;
                fijacionSave.ContratoSAP = fijacion.ContratoSAP;
                fijacionSave.ContratoId = fijacion.ContratoId;
                fijacionSave.Precio = fijacion.Precio;
                fijacionSave.Cantidad = fijacion.Cantidad;
                fijacionSave.DestinoId = fijacion.DestinoId;
                fijacionSave.Pizarra = fijacion.Pizarra;
                fijacionSave.Posicion = fijacion.Posicion;
                fijacionSave.PagoDiferido = fijacion.PagoDiferido;
                fijacionSave.FechaOperacion = fijacion.FechaOperacion;
                fijacionSave.Fecha = fijacion.Fecha;
                fijacionSave.FechaHasta = fijacion.FechaHasta;
                fijacionSave.FechaDesde = fijacion.FechaDesde;
                fijacionSave.ProveedorId = fijacion.ProveedorId;
                fijacionSave.ComercialId = fijacion.ComercialId;
                fijacionSave.MaterialId = fijacion.MaterialId;
                fijacionSave.CorredorId = fijacion.CorredorId;               
                fijacionSave.DiasPesificado = fijacion.DiasPesificado;
                fijacionSave.MonedaId = fijacion.MonedaId;
                fijacionSave.CampanaId = fijacion.CampanaId;
                fijacionSave.PrecioNeto = fijacion.PrecioNeto;
                fijacionSave.EstadoId = fijacion.EstadoId;
                fijacionSave.FechaConfirmacion = fijacion.FechaConfirmacion;
                fijacionSave.TipoNegocioId = fijacion.TipoNegocioId;
                fijacionSave.ComercialCreadorId = fijacion.ComercialCreadorId;
                fijacionSave.ComercialId = fijacion.ComercialId;
                fijacionSave.Canje = fijacion.Canje;
                fijacionSave.Fecha = fijacion.Fecha;
                fijacionSave.GrupoCompra = fijacion.GrupoCompra;

                if (fijacion.AperturaPrecio != null)
                {
                    var aperturas = repositorio.Listar<AperturaPrecio>(x => x.NegocioId != null && x.NegocioId == fijacionSave.Id);
                    repositorio.RemoverTodos(aperturas);
                    logger.Error("Iniciando Apertura");
                    fijacionSave.AperturaPrecio = fijacion.AperturaPrecio;
                }
                repositorio.Agregar(fijacionSave);
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacionSave.Id), TipoAccionLogDataAgro.Crear, fijacionSave.GetType());
            }
            catch (Exception e)
            {
                logger.Error("Error Alta FijacionSap");
                logger.Error("", e.Message);
                logger.Error(e);
            }
            return error;
        }

        public Resultado AnularFijacionSAP(FijacionSAP fijacion)
        {
            logger.Debug("Inicializar AnularContratoSAP");

            var oEntityErrors = new Resultado();
            var codigo = fijacion.Fijacion.PadLeft(10, '0');
            var oFijacionSave = repositorio.ObtenerMayor<FijacionDePrecioContrato, int>(x => x.FijacionSAP == codigo, x => x.Id);

            string jsonObjeto = JsonConvert.SerializeObject(fijacion, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
            });
            logger.Debug("Campos a editar: " + jsonObjeto);

            if (oFijacionSave != null)
            {
                logger.Debug("Fijacion: " + oFijacionSave.FijacionSAP);

                try
                {

                    oFijacionSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionSave.Id), TipoAccionLogDataAgro.Eliminar, oFijacionSave.GetType());
                }
                catch (Exception e)
                {
                    logger.Error("Error Anular FijacionSap");
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("No existe la fijacion"));

            }
            return oEntityErrors;
        }
    }
}




