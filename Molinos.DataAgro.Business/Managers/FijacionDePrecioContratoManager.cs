using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using NLog;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private readonly IMailManager mailManager;
        private readonly ILogger logger;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IValidarDocProcPagoAgent validarPagoAgente;
        private readonly IModificarFijacionAgent modificarFijacionAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly IConfiguracionManager configuracionManager;
        private readonly IValidarLiquidacionParaFijacionAgent validarLiquidacionParaFijacionAgent;
        private readonly ITipoDeCambioAgent tipoDeCambioAgent;
        private readonly IContratosParaFijacionVirtualAgent contratosParaFijacionVirtualAgent;
        private readonly IFinalizarFijacionVirtualAgent finalizarFijacionVirtual;
        private readonly IAnularFijacionVirtualAgent anularFijacionVirtual;
        private readonly INegocioManager negocioManager;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private readonly IAnularFijacionAgent anularFijacion;
        private readonly IValidarLiquidacionComisionesAgent validarLiquidacionComisionesAgent;
        private readonly IValidarLiquidacionFinalAgent validarLiquidacionFinalAgent;
        private readonly IValidarLiquidacionParcialAgent validarLiquidacionParcialAgent;
        private readonly IValidarPesificacionAgent validarPesificacionAgent;
        private readonly IContratoManager contratoManager;


        public FijacionDePrecioContratoManager(
            ILogger logger,
            IRepositorio repositorio,
            IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager,
            IPushNotificationManager oMSNotification,
            IFinalizarFijacionAgent oFinalizarFijacionAgent,
            IContratosParaFijacionAgent oContratosParaFijacionAgent,
            IMailManager mailManager, ILogDataAgroManager logDataAgroManager,
            IValidarDocProcPagoAgent validarPagoAgente, IModificarFijacionAgent modificarFijacionAgent,
            IDiasHabilesAgent diasHabilesAgent, IConfiguracionManager configuracionManager,
            IValidarLiquidacionParaFijacionAgent validarLiquidacionParaFijacionAgent,
            ITipoDeCambioAgent tipoDeCambioAgent, IContratosParaFijacionVirtualAgent contratosParaFijacionVirtualAgent,
            IFinalizarFijacionVirtualAgent finalizarFijacionVirtual, IAnularFijacionVirtualAgent anularFijacionVirtual,
            INegocioManager negocioManager, IConfiguracionInternaManager configuracionInternaManager,
            IAnularFijacionAgent anularFijacion, IValidarLiquidacionComisionesAgent validarLiquidacionComisionesAgent, IValidarLiquidacionFinalAgent validarLiquidacionFinalAgent,
            IValidarLiquidacionParcialAgent validarLiquidacionParcialAgent, IValidarPesificacionAgent validarPesificacionAgent, IContratoManager contratoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjProveedorManager = oMSProveedorManager;
            mobjComercialManager = oMSComercialManager;
            mobjNotification = oMSNotification;
            this.oFinalizarFijacionAgent = oFinalizarFijacionAgent;
            this.oContratosParaFijacionAgent = oContratosParaFijacionAgent;
            this.mailManager = mailManager;
            this.logDataAgroManager = logDataAgroManager;
            this.validarPagoAgente = validarPagoAgente;
            this.diasHabilesAgent = diasHabilesAgent;
            this.configuracionManager = configuracionManager;
            this.modificarFijacionAgent = modificarFijacionAgent;
            this.validarLiquidacionParaFijacionAgent = validarLiquidacionParaFijacionAgent;
            this.tipoDeCambioAgent = tipoDeCambioAgent;
            this.contratosParaFijacionVirtualAgent = contratosParaFijacionVirtualAgent;
            this.finalizarFijacionVirtual = finalizarFijacionVirtual;
            this.anularFijacionVirtual = anularFijacionVirtual;
            this.negocioManager = negocioManager;
            this.configuracionInternaManager = configuracionInternaManager;
            this.anularFijacion = anularFijacion;
            this.validarLiquidacionComisionesAgent = validarLiquidacionComisionesAgent;
            this.validarLiquidacionFinalAgent = validarLiquidacionFinalAgent;
            this.validarLiquidacionParcialAgent = validarLiquidacionParcialAgent;
            this.validarPesificacionAgent = validarPesificacionAgent;
            this.contratoManager = contratoManager;
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
            var aFijar = TraerDatosFijacion(fijacion.Proveedor.CUIT, fijacion.Corredor?.CUIT, fijacion.MaterialId, fijacion.ContratoSAP.Remove(0, 3), fijacion.Id);
            double kgAplicados = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosAplicados, out kgAplicados) ? kgAplicados : 0;
            double pendiente = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosPendiente, out pendiente) ? pendiente - kgAplicados : 0;
            if (pendiente <= ampliacion)
            {
                oEntityErrors.Error("", "La ampliación supera la cantidad disponible");
            }
            return oEntityErrors;
        }

        private GrabarContratoResult ValidarAmpliacionFijacionVirtual(FijacionDePrecioContrato fijacion, double ampliacion)
        {
            var oEntityErrors = new GrabarContratoResult();
            var aFijar = TraerDatosFijacionVirtual(fijacion.Proveedor.CUIT, fijacion.Corredor != null ? fijacion.Corredor.CUIT : null, fijacion.MaterialId, fijacion.ContratoSAP.Remove(0, 3), fijacion.Id);
            double kgAplicados = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosAplicados, out kgAplicados) ? kgAplicados : 0;
            double pendiente = aFijar.Count() > 0 && double.TryParse(aFijar.First().KilosPendiente, out pendiente) ? pendiente - kgAplicados : 0;
            if (pendiente <= ampliacion)
            {
                oEntityErrors.Error("", "La ampliación supera la cantidad disponible");
            }
            return oEntityErrors;
        }

        private Resultado Validar(FijacionDePrecioContrato oParam, Resultado oErrorMessages)
        {
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
            if (proveedor == null)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' es obligatorio.");
                return oErrorMessages;
            }
            if (proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor deshabilitado.");
                return oErrorMessages;
            }
            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacío.");
            }
            if (oParam.MaterialId == 0)
            {
                oErrorMessages.Error("Material", "El campo 'Material' no debe estar vacío.");
            }

            if (oParam.Cantidad == 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe estar vacío.");
            }

            if (oParam.Cantidad < 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe ser negativo.");
            }
            var cuitCorredor = oParam.CorredorId.HasValue ? mobjProveedorManager.TraerCuit(oParam.CorredorId.Value) : "";
            var cuitProveedor = mobjProveedorManager.TraerCuit(oParam.ProveedorId ?? 0);
            if (string.IsNullOrEmpty(oParam.ContratoSAP))
            {
                oErrorMessages.Error("ContratoId", "El campo 'Contrato' no debe estar vacío.");
                return oErrorMessages;
            }

            var fijacion = oParam.Virtual != true ? oContratosParaFijacionAgent.ObtenerContratos(cuitProveedor, cuitCorredor, oParam.MaterialId, oParam.ContratoSAP, oParam.Id).FirstOrDefault() :
               contratosParaFijacionVirtualAgent.ObtenerContratosCanje(cuitProveedor, cuitCorredor, oParam.MaterialId, oParam.ContratoSAP, oParam.Id).FirstOrDefault();

            if (fijacion == null)
            {
                oErrorMessages.Error("ContratoId", "El contrato no existe.");
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
                    oErrorMessages.Error("Cantidad", "La cantidad excede a los kilos del contrato.");
                }
                //var KilosMaximos = fijacion.KgMaximos;
                //if (KilosMaximos < oParam.Cantidad)
                //{
                //    oErrorMessages.Error("Cantidad", "La cantidad excede a los kilos Máximos ("+ fijacion.KgMaximos + ") del contrato.");
                //}
                //var KilosMinimos = fijacion.KgMinimos;
                //if (KilosMinimos > oParam.Cantidad)
                //{
                //    oErrorMessages.Error("Cantidad", "La cantidad es menor a los kilos Minimos (" + fijacion.KgMinimos + ") del contrato.");
                //}
                oParam.TrigoEspecial = oParam.Virtual != true && (fijacion.Calidad != null && fijacion.Calidad.Value);
            }
            if (oParam.Cantidad < 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe ser negativo.");
            }
            if (oParam.Precio == 0 && oParam.Pizarra.HasValue && !oParam.Pizarra.Value)
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacío.");
            }
            if (oParam.MonedaId == null && oParam.Pizarra.HasValue && !oParam.Pizarra.Value)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacío.");
            }
            if (oParam.ComercialId == 0 || oParam.ComercialId == null)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacío.");
            }
            if (oParam.Pizarra != true)
            {
                var rangosPrecio = repositorio.Listar<RangoPrecio>();
                if (rangosPrecio.Exists(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId && (x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio)))
                {
                    oErrorMessages.Error("Precio", "Precio fuera de Rango.");
                }
            }
            if (oParam.CampanaId == 0 || oParam.CampanaId == null)
            {
                oErrorMessages.Error("CampanaId", "Campaña del contrato seleccionado fuera del rango.");
            }
            if (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value && oParam.DiasPesificado == null)
            {
                oErrorMessages.Error("CampanaId", "Se deben completar los días en negocios de Pago Diferido.");
            }
            if (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0 &&
                oParam.MonedaId != "ARP  ")
            {
                oErrorMessages.Error("pagoDiferido", "La fijación de pago diferido siempre es en ARP.");
            }

            if (oParam.MonedaId != "USDM " && oParam.AperturaPrecio != null)
            {
                var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

                if (oParam.FechaCierta == null && oParam.PagoDiferido != true && concepto != null)
                {
                    oErrorMessages.Error("", "Concepto financiero es obligatorio con pago diferido.");
                }
                //if (oParam.FechaCierta != null && oParam.ObligatoriedadCostoFinanciero == true)
                //{
                //    if ((oParam.Pizarra.HasValue && !oParam.Pizarra.Value))
                //    {
                //        if (!((concepto != null && (oParam.FechaCierta != null) ||
                //            (concepto == null && (oParam.FechaCierta == null)))))
                //        {
                //            oErrorMessages.Error("", "Fecha cierta es obligatorio con el concepto financiero,");
                //        }
                //    }
                //}
                //else
                //{
                if (oParam.Pizarra.HasValue && !oParam.Pizarra.Value && oParam.FechaCierta == null)
                {
                    if (!((concepto != null && (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value) && (oParam.DiasPesificado.HasValue
                        && oParam.DiasPesificado.Value != 0)) || (concepto == null && (!oParam.PagoDiferido.HasValue ||
                        (oParam.PagoDiferido.HasValue && !oParam.PagoDiferido.Value)) &&
                        (!oParam.DiasPesificado.HasValue || (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value == 0)))))
                    {
                        oErrorMessages.Error("", "Días de diferimiento/costo financiero es obligatorio con el pago diferido en pesos.");
                    }
                }
                //}

            }
            //else
            //{
            //    var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

            //    if (concepto != null)
            //    {
            //        oErrorMessages.Error("", "El concepto financiero se debe completar solo cuando la moneda es ARP");
            //    }
            //}

            Negocio fijacionSave = null;
            if (oParam.Id > 0)
            {
                fijacionSave = repositorio.Obtener<Negocio>(oParam.Id);
            }
            if (oParam.Id > 0)
            {
                if ((oParam.FechaOperacion.Date != fijacionSave.Fecha.Date && oParam.FechaOperacion.Date < fijacionSave.Fecha.Date))
                {

                    var diaAnterior = diasHabilesAgent.UltimoDiaHabil(fijacionSave.Fecha.Date);

                    if (oParam.FechaOperacion < diaAnterior.Date && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior))
                    {
                        oErrorMessages.Error("FechaOperacion", "La Fecha de operación no puede ser anterior al último día habil (" + diaAnterior.ToString("dd/MM/yyyy") + ").");

                    }

                    if (string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior))
                    {
                        oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por el cual la fecha de operación es anterior al día de la fecha.");
                    }

                    if (!string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior) && oParam.DescripcionOperacionAnterior.Length <= 5)
                    {
                        oErrorMessages.Error("MotivoOperacionAnterior", "Es obligatorio ingresar un motivo con más de 5 caracteres.");
                    }
                }
                if (oParam.FechaOperacion.Date > fijacionSave.Fecha.Date)
                {
                    oErrorMessages.Error("NoInformaSio", "La fecha de operación no puede ser mayor a " + fijacionSave.Fecha.ToString("dd/MM/yyyy"));
                }
            }
            else
            {
                if (oParam.FechaOperacion.Date < DateTime.Now.Date)
                {
                    var diaAnterior = diasHabilesAgent.UltimoDiaHabil(null);

                    if (oParam.FechaOperacion.Date < diaAnterior.Date && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior))
                    {
                        oErrorMessages.Error("FechaOperacion", "La fecha de operación no puede ser anterior al último día habil (" + diaAnterior.ToString("dd/MM/yyyy") + ").");

                    }
                    if (string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior))
                    {
                        oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la fecha de operación es anterior al día de la fecha.");
                    }
                    if (!string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior) && oParam.DescripcionOperacionAnterior.Length <= 5)
                    {
                        oErrorMessages.Error("MotivoOperacionAnterior", "Es obligatorio ingresar un motivo con más de 5 caracteres.");
                    }
                }
            }
            if (oParam.Dolarizado.HasValue && oParam.Dolarizado.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la fecha de pesificación en negocios dolarizados.");
            }
            if (oParam.FechaDolarizado.HasValue && oParam.FechaDolarizado.Value < DateTime.Now.Date)
            {
                oErrorMessages.Error("dolarizado", "La fecha de dolarizado no es válida.");

            }

            if (oParam.FechaDolarizado != null)
            {
                var conf = configuracionManager.TraerConfiguraciones();
                if (conf != null)
                {
                    var cantidadDias = PermisosHelper.Is(PermisosDataAgro.ModificarLimiteDolarizado) ? conf.CantidadDiasDolarizadoLimiteMaximo : conf.CantidadDias;
                    var fechaLimite = oParam.FechaOperacion.AddDays(cantidadDias);
                    if (oParam.FechaDolarizado.Value.Date > fechaLimite.Date)
                    {
                        oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado debe ser menor o igual que los " + cantidadDias + " días.");
                    }
                }
            }
            if (oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero) /*&& oParam.Pizarra != true*/)
            {
                oErrorMessages.Error("Descuentos", "El costo financiero no puede ser negativo.");
            }

            if (oParam.PagoDiferido == true && oParam.DiasPesificado != null)
            {
                var conf = configuracionManager.TraerConfiguraciones();
                if (conf != null)
                {
                    var limitePesificado = PermisosHelper.Is(PermisosDataAgro.ModificarLimitePesificado) ? conf.CantidadDiasPesificadoLimite : conf.DiasDiferimiento;
                    if (oParam.DiasPesificado.Value > limitePesificado)
                    {
                        oErrorMessages.Error("Pago Diferido", "Los días de pesificado deben ser menos o igual a " + limitePesificado + " días.");
                    }
                }
            }
            if (oParam.EstadoId == 5 && oParam.DolarizadoExpress.HasValue && oParam.DolarizadoExpress.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la fecha de pesificación en negocios dolarizados.");
            }
            if ((oParam.DolarizadoExpress == true || oParam.Dolarizado == true || oParam.DolarizadoCorredor == true) && (oParam.Cesion == true || oParam.Anticipo == true))
            {
                oErrorMessages.Error("dolarizado", "No se puede completar Dolarizados porque el contrato tiene Cesión o Anticipo.");
            }

            if (fijacionSave != null && PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && fijacionSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
            {
                oErrorMessages.Error("Contrato", "No se puede modificar la fijación.");
            }

            if (fijacionSave != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                if (fijacionSave.DolarizadoTercero == true && oParam.Dolarizado != true && oParam.DolarizadoExpress != true && oParam.DolarizadoCorredor != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Dolarizado que marcó el tercero.");
                }

                if (fijacionSave.PagoDiferidoTercero == true && oParam.PagoDiferido != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Pago Diferido que marcó el tercero.");
                }

            }
            if (PermisosHelper.Is(PermisosDataAgro.ModificarFijacionVirtual) && oParam.Virtual != true)
            {
                oErrorMessages.Error("Virtual", "Es obligatorio completar el campo fijación virtual.");
            }
            if ((oParam.MaterialId == 4 || oParam.MaterialId == 5) && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(a => a.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones && (a.Importe > 0 || a.Porcentaje > 0)))
            {
                oErrorMessages.Error("Comisiones", "No se puede cargar el concepto comisiones en apertura de precio para negocios de girasol.");
            }
            return oErrorMessages;
        }

        public GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio)
        {
            var oEntityErrors = new GrabarFijacionResult();
            Validar(oFijacionDePrecio, oEntityErrors);

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
                if ((oFijacionDePrecio.ChequeElectronico != oFijacionDePrecioSave.ChequeElectronico && oFijacionDePrecio.ChequeElectronico == true) || oFijacionDePrecioSave.PagoCBU != oFijacionDePrecio.PagoCBU)
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
                if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oFijacionDePrecioSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
                {
                    oEntityErrors.Error("", "La Fijación no se puede modificar");
                    return oEntityErrors;
                }
                if ((oFijacionDePrecioSave.Precio != oFijacionDePrecio.Precio || oFijacionDePrecioSave.Cantidad != oFijacionDePrecio.Cantidad) && (oFijacionDePrecioSave.EstadoId != 1 && oFijacionDePrecioSave.EstadoId != 3 && oFijacionDePrecioSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion))
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

                //if (oFijacionDePrecioSave.Pizarra == true)
                //{
                //    var precio = 0;
                //    var pizarra = repositorio.Obtener<PrecioPizarra>(x => x.FechaDesde > oFijacionDePrecioSave.FechaOperacion
                //    && x.FechaHasta < oFijacionDePrecioSave.FechaOperacion);
                //    var cambio = tipoDeCambioAgent.TraerTipoDeCambio(null);
                //    if (pizarra != null)
                //    {
                //        if (pizarra.MonedaId == "ARP")
                //        {
                //            precio = (int)(pizarra.Precio / cambio);
                //        }
                //        oFijacionDePrecioSave.Precio = precio;
                //    }
                //}
                //else
                //{
                //    oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                //}

                oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                oFijacionDePrecioSave.Cantidad = oFijacionDePrecio.Cantidad;
                oFijacionDePrecioSave.Ampliaciones = oFijacionDePrecio.Ampliaciones;
                oFijacionDePrecioSave.Observacion = oFijacionDePrecio.Observacion;
                oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
                oFijacionDePrecioSave.ComercialId = oFijacionDePrecio.ComercialId;
                oFijacionDePrecioSave.ContratoId = oContratoId != null && oContratoId.Id > 0 ? oContratoId.Id : (int?)null;
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
                oFijacionDePrecioSave.PagoDiferido = oFijacionDePrecio.PagoDiferido;
                oFijacionDePrecioSave.DestinoId = oFijacionDePrecio.DestinoId;
                oFijacionDePrecioSave.FechaOperacion = oFijacionDePrecio.FechaOperacion;
                oFijacionDePrecioSave.ChequeElectronico = oFijacionDePrecio.ChequeElectronico;
                oFijacionDePrecioSave.PagoCBU = oFijacionDePrecio.PagoCBU;
                oFijacionDePrecioSave.MotivoOperacionAnterior = oFijacionDePrecio.MotivoOperacionAnterior;
                oFijacionDePrecioSave.DescripcionOperacionAnterior = oFijacionDePrecio.DescripcionOperacionAnterior;
                oFijacionDePrecioSave.Anticipo = oFijacionDePrecio.Anticipo;
                oFijacionDePrecioSave.Cesion = oFijacionDePrecio.Cesion;
                oFijacionDePrecioSave.ClasificacionContrato = oFijacionDePrecio.ClasificacionContrato;
                oFijacionDePrecioSave.ImporteAPrecioContrato = oFijacionDePrecio.ImporteAPrecioContrato;
                oFijacionDePrecioSave.PorcentajeAPrecioContrato = oFijacionDePrecio.PorcentajeAPrecioContrato;
                oFijacionDePrecioSave.MonedaAPrecioContrato = oFijacionDePrecio.MonedaAPrecioContrato;
                oFijacionDePrecioSave.ImporteSobrePrecioContrato = oFijacionDePrecio.ImporteSobrePrecioContrato;
                oFijacionDePrecioSave.PorcentajeSobrePrecioContrato = oFijacionDePrecio.PorcentajeSobrePrecioContrato;
                oFijacionDePrecioSave.MonedaSobrePrecioContrato = oFijacionDePrecio.MonedaSobrePrecioContrato;
                oFijacionDePrecioSave.Virtual = oFijacionDePrecio.Virtual;
                oFijacionDePrecioSave.FechaCierta = oFijacionDePrecio.FechaCierta;
                oFijacionDePrecioSave.ObligatoriedadCostoFinanciero = oFijacionDePrecio.FechaCierta.HasValue &&
                oFijacionDePrecio.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                oFijacionDePrecio.ObligatoriedadCostoFinanciero.HasValue && !oFijacionDePrecio.ObligatoriedadCostoFinanciero.Value
                ? null : oFijacionDePrecio.FechaCierta.HasValue ? oFijacionDePrecio.ObligatoriedadCostoFinanciero : null;
                oFijacionDePrecioSave.TipoPosicionCBOTId = oFijacionDePrecio.TipoPosicionCBOTId;
                oFijacionDePrecioSave.ProveedorComisionistaId = oFijacionDePrecio.ProveedorComisionistaId;

                if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
                {
                    oFijacionDePrecioSave.ObservacionTercero = oFijacionDePrecio.ObservacionTercero;
                    oFijacionDePrecioSave.DolarizadoTercero = oFijacionDePrecio.DolarizadoTercero;
                    oFijacionDePrecioSave.CalidadTercero = oFijacionDePrecio.CalidadTercero;
                    oFijacionDePrecioSave.PagoDiferidoTercero = oFijacionDePrecio.PagoDiferidoTercero;
                    oFijacionDePrecioSave.SustentableTercero = oFijacionDePrecio.SustentableTercero;
                    oFijacionDePrecioSave.EstadoId = (int)EnumEstadoContrato.PreAprobacion;
                    oFijacionDePrecioSave.UsuarioTercero = oFijacionDePrecio.UsuarioTercero;

                }

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
                oFijacionDePrecio.ObligatoriedadCostoFinanciero = oFijacionDePrecio.FechaCierta.HasValue &&
                oFijacionDePrecio.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                oFijacionDePrecio.ObligatoriedadCostoFinanciero.HasValue && !oFijacionDePrecio.ObligatoriedadCostoFinanciero.Value
                ? null : oFijacionDePrecio.FechaCierta.HasValue ? oFijacionDePrecio.ObligatoriedadCostoFinanciero : null;
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


            if ((oFijacionDePrecio.EstadoId < (int)EnumEstadoContrato.PreAprobacion
                || (!PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oFijacionDePrecio.EstadoId == (int)EnumEstadoContrato.PreAprobacion && oFijacionDePrecio.Id > 0))
                && ConfirmacionAutomatica(oFijacionDePrecioSave))
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
                if (oFijacionDePrecio.FechaDolarizado != null)
                {
                    oFijacionDePrecioSave.DolarizadoCorredor = oFijacionDePrecioSave.ClasificacionContrato.ToUpper() != "PRODUCTOR" || oFijacionDePrecio.CorredorId != null;
                }
                else
                {
                    oFijacionDePrecioSave.DolarizadoCorredor = false;
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
            List<int> tipoRangos = new List<int>() { (int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion };
            if (contrato.EstadoId == 1)
            {
                tipoRangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Confirmacion);
            }
            else
            {
                tipoRangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Reconfirmacion);
            }
            var rangos = repositorio.Listar<RangoConfirmacionAutomatica>(x =>
            (x.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.Fijacion || x.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecioYFijacion) &&
           x.FechaDesde <= hoy &&
           x.FechaHasta >= hoy &&
           x.MaterialId == contrato.MaterialId &&
           x.MonedaId == contrato.MonedaId &&
           precioContrato >= x.PrecioMinimo && precioContrato <= x.PrecioMaximo
           && tipoRangos.Contains(x.TipoRangoId)) ?? new List<RangoConfirmacionAutomatica>();

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
                && x.MaterialId == rango.MaterialId && x.Precio > 0);

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

            if (string.IsNullOrEmpty(oFijacionDePrecioSave.FijacionSAP) && (oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Confirmado || oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error))
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
                    string nroFijacionSAP = "";
                    if (oFijacionDePrecioSave.Virtual != true)
                    {
                        nroFijacionSAP = SapFinalizarFijacion(oFijacionDePrecioSave);

                    }
                    else
                    {
                        nroFijacionSAP = SapFinalizarFijacionVirtual(oFijacionDePrecioSave);
                        double nroFijacion = double.TryParse(nroFijacionSAP, out nroFijacion) ? nroFijacion : 0;
                        if (nroFijacion <= 0)
                        {
                            oEntityErrors.Error("", "Error al grabar la fijación virtual contrato bloqueado");
                            oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                            repositorio.GuardarCambios();
                            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), TipoAccionLogDataAgro.Crear, oFijacionDePrecioSave.GetType());
                            return oEntityErrors;
                        }
                        nroFijacionSAP = oFijacionDePrecioSave.ContratoSAP + nroFijacionSAP;
                    }
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
                        repositorio.GuardarCambios();
                        logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oFijacionDePrecioSave.Id), TipoAccionLogDataAgro.Crear, oFijacionDePrecioSave.GetType());

                        //Envio de mail

                        EnviarMailFijacion(oFijacionDePrecioSave.Id, idActiveDirectory);

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
                if (!string.IsNullOrEmpty(oFijacionDePrecioSave.FijacionSAP) && oFijacionDePrecioSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "La Fijación ya tiene ContratoSAP asignado, por favor comunicarse con sistemas.");
                    negocioManager.EnviarMailErrorFinalizarNegocio(fijacionDePrecioContratoId);
                }
            }
            return oEntityErrors;
        }

        public void EnviarMailFijacion(int fijacion, string idActiveDirectory)
        {
            FijacionDePrecioContrato fijacionDePrecio = repositorio.Obtener<FijacionDePrecioContrato>(fijacion);
            if (fijacionDePrecio.Virtual == true)
            {
                mobjProveedorManager.EnviarMailFijacionVirtual(fijacionDePrecio, idActiveDirectory, false);
            }
            else
            {
                mobjProveedorManager.EnviarEmailFijacion(fijacionDePrecio, idActiveDirectory);
            }
        }

        private string SapFinalizarFijacion(FijacionDePrecioContrato fijacion)
        {
            return oFinalizarFijacionAgent.Finalizar(fijacion);
        }

        private string SapFinalizarFijacionVirtual(FijacionDePrecioContrato fijacion)
        {
            return finalizarFijacionVirtual.FinalizarFijacionVirtual(fijacion);
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
                Negocio = fijac.FijacionSAP,
                Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial + " " + "(" + fijac.Proveedor.CUIT + ")",
                ContratoId = fijac.ContratoId ?? 0,
                ProveedorId = fijac.ProveedorId ?? 0,
                CorredorId = fijac.CorredorId ?? 0,
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
                EPA = false,
                EUDR = false,
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
                    PagoDiferido = fijac.PagoDiferidoContrato
                },
                FechaDesde = fijac.FechaDesde,
                FechaHasta = fijac.FechaHasta,
                Fecha = fijac.Fecha,
                FechaOperacion = fijac.FechaOperacion,
                ChequeElectronico = fijac.ChequeElectronico,
                PagoCBU = fijac.PagoCBU,
                MotivoOperacionAnterior = fijac.MotivoOperacionAnterior,
                DescripcionOperacionAnterior = fijac.DescripcionOperacionAnterior,
                FechaOperacionFormateado = SqlFunctions.DateName("day", fijac.FechaOperacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaOperacion.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaOperacion),
                ObservacionTercero = fijac.ObservacionTercero,
                DolarizadoTercero = fijac.DolarizadoTercero,
                CalidadTercero = fijac.CalidadTercero,
                PagoDiferidoTercero = fijac.PagoDiferidoTercero,
                SustentableTercero = fijac.SustentableTercero,
                DolarizadoCorredor = fijac.DolarizadoCorredor.Value,
                DolarizadoExpress = fijac.DolarizadoExpress.Value,
                Fecha_Dolarizado = fijac.FechaDolarizado,
                Anticipo = fijac.Anticipo,
                Cesion = fijac.Cesion,
                ClasificacionContrato = fijac.ClasificacionContrato,
                ImporteAPrecioContrato = fijac.ImporteAPrecioContrato,
                PorcentajeAPrecioContrato = fijac.PorcentajeAPrecioContrato,
                MonedaAPrecioContrato = fijac.MonedaAPrecioContrato,
                ImporteSobrePrecioContrato = fijac.ImporteSobrePrecioContrato,
                PorcentajeSobrePrecioContrato = fijac.PorcentajeSobrePrecioContrato,
                MonedaSobrePrecioContrato = fijac.MonedaSobrePrecioContrato,

                UsuarioId = fijac.UsuarioId,
                UsuarioTercero = fijac.UsuarioTercero,
                ProveedorCreador = fijac.ProveedorCreadorId,
                Virtual = fijac.Virtual,
                FechaCierta = fijac.FechaCierta,
                FechaCiertaFormateado = fijac.FechaCierta != null ? SqlFunctions.DateName("day", fijac.FechaCierta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)fijac.FechaCierta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", fijac.FechaCierta) : "",
                ObligatoriedadCostoFinanciero = fijac.ObligatoriedadCostoFinanciero,
                TipoPosicionCBOTId = fijac.TipoPosicionCBOTId,
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
            if (numero.Length == 1)
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
            long l = 0;
            if (string.IsNullOrEmpty(CuitProveedor) || !long.TryParse(CuitProveedor, out l))
            {
                return new List<DatosFijacionDeContratoDto>();
            }
            return oContratosParaFijacionAgent.ObtenerContratos(CuitProveedor, CuitCorredor, materialId, filtro, fijacionId);
        }
        public List<DatosFijacionDeContratoDto> TraerDatosFijacionVirtual(string CuitProveedor, string CuitCorredor, int materialId, string filtro, int fijacionId)
        {

            long l = 0;
            if (string.IsNullOrEmpty(CuitProveedor) || !long.TryParse(CuitProveedor, out l))
            {
                return new List<DatosFijacionDeContratoDto>();
            }
            return contratosParaFijacionVirtualAgent.ObtenerContratosCanje(CuitProveedor, CuitCorredor, materialId, filtro, fijacionId);
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
                Moneda = apertura.Moneda.Descripcion,
                Porcentaje = apertura.Porcentaje
            },
            x => x.NegocioId == fijacionId);
        }

        public void FinalizacionAutomatica(string idActiveDirectory)
        {
            List<FijacionDePrecioContrato> fijacioneConfirmados = repositorio.Listar<FijacionDePrecioContrato>(x => x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error);

            List<int> idsFijacionesConfirmadas = new List<int>();
            fijacioneConfirmados.ForEach(x => idsFijacionesConfirmadas.Add(x.Id));

            logger.Debug($"{idsFijacionesConfirmadas.Count} Fijaciones a finalizar con los ID {String.Join(", ", idsFijacionesConfirmadas)}");

            var oEntityErrors = new GrabarContratoResult();
            foreach (var fijacion in fijacioneConfirmados)
            {
                try
                {
                    logger.Debug("Finalizando Fijación ID: " + fijacion.Id);
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
                Validar(oFijacionDePrecioSave, oEntityErrors);

                if (oEntityErrors.HayErrores)
                {
                    return oEntityErrors;
                }
                if (ConfirmacionAutomatica(oFijacionDePrecioSave))
                {
                    oFijacionDePrecioSave.FechaConfirmacion = DateTime.Now;
                    oFijacionDePrecioSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato " + oFijacionDePrecioSave.Id + " se confirmo automaticamente por estar dentro de los rangos configurados");
                }
                else
                {
                    oFijacionDePrecioSave.EstadoId = (int)EnumEstadoContrato.Pendiente;
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
            var asunto = "Rechazo Fijación Molinos Agro S.A. –  " + fijacion.Proveedor.RazonSocial;
            var copia = new List<string>() { fijacion.Comercial.IdActiveDirectory, ConfigurationManager.AppSettings["CredentialUserName"] };
            var vista = CuerpoMailFijacion(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), fijacion);
            mailManager.EnviarMail(enviarA, asunto, "", copia, vista);
        }
        private AlternateView CuerpoMailFijacion(string filePath, FijacionDePrecioContrato fijacion)
        {
            LinkedResource res = new LinkedResource(filePath)
            {
                ContentId = Guid.NewGuid().ToString()
            };
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
                error = ValidarModificacionFijacionFinalizada(oContrato, oContratoSave);
                if (error.HayError)
                {
                    return error;
                }

                var fijacionSap = oContratoSave.ContratoSAP.PadLeft(10, '0');
                var oContratoId = repositorio.Obtener<Contrato>(x => x.ContratoSAP == fijacionSap);

                var proveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.ProveedorId, x => x.CUIT);
                var corredor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.CorredorId, x => x.CUIT);

                var DolarizadoExpress = oContratoSave.DolarizadoExpress ?? false;
                var DolarizadoCorredor = oContratoSave.DolarizadoCorredor ?? false;
                var Dolarizado = oContratoSave.Dolarizado ?? false;
                var fechaDolarizado = oContratoSave.FechaDolarizado;

                CargarDolarizado(oContrato, oContratoSave, oContratoId, oContrato.FechaOperacion.AddDays(30), proveedor, corredor);
                error = ValidarLiquidacionParaFijacion(oContratoSave, DolarizadoExpress, DolarizadoCorredor, Dolarizado, fechaDolarizado);
                if (error.HayError)
                {
                    return error;
                }
                oContratoSave.ChequeElectronico = oContrato.ChequeElectronico;
                oContratoSave.PagoCBU = oContrato.PagoCBU;
                oContratoSave.DiasPesificado = oContrato.DiasPesificado;


                var res = modificarFijacionAgent.Modificar(oContratoSave);
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

        private GrabarFijacionResult ValidarLiquidacionParaFijacion(FijacionDePrecioContrato oContratoSave, bool DolarizadoExpress, bool DolarizadoCorredor, bool Dolarizado, DateTime? fechaDolarizado)
        {
            GrabarFijacionResult error = new GrabarFijacionResult();
            if (DolarizadoExpress != oContratoSave.DolarizadoExpress || DolarizadoCorredor != oContratoSave.DolarizadoCorredor || Dolarizado != oContratoSave.Dolarizado || fechaDolarizado != oContratoSave.FechaDolarizado)
            {
                var result2 = validarLiquidacionParaFijacionAgent.Validar(oContratoSave.ContratoSAP, oContratoSave.FijacionSAP);
                if (result2 != "Ok")
                {
                    error.Error("", result2);
                }
            }
            return error;
        }

        private GrabarFijacionResult ValidarModificacionFijacionFinalizada(FijacionDePrecioContrato oContrato, FijacionDePrecioContrato oContratoSave)
        {
            GrabarFijacionResult result = new GrabarFijacionResult();
            if ((oContrato.DolarizadoExpress == true || oContrato.Dolarizado == true || oContrato.DolarizadoCorredor == true) && (oContratoSave.Cesion == true || oContratoSave.Anticipo == true))
            {
                result.Error("dolarizado", "No se puede completar Dolarizados porque el contrato tiene Cesion o Anticipo.");
            }
            if ((oContrato.DolarizadoExpress == true || oContrato.Dolarizado == true || oContrato.DolarizadoCorredor == true) && (oContratoSave.Cesion == true || oContratoSave.Anticipo == true))
            {
                result.Error("dolarizado", "No se puede completar Dolarizados porque el contrato tiene Cesion o Anticipo.");
            }
            //var KilosMaximos = oContrato.KgMaximo;
            //if (KilosMaximos < oContratoSave.Cantidad)
            //{
            //    result.Error("Cantidad", "La cantidad es menor a los kilos Máximos (" +  KilosMaximos + ") del contrato.");
            //}
            //var KilosMinimos = oContrato.KgMinimo;
            //if (KilosMinimos > oContratoSave.Cantidad)
            //{
            //    result.Error("Cantidad", "La cantidad es menor a los kilos Minimos (" + KilosMinimos + ") del contrato.");
            //}

            if (!oContrato.FechaDolarizado.HasValue && (oContrato.DolarizadoExpress == true || oContrato.Dolarizado == true || oContrato.DolarizadoCorredor == true))
            {
                result.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados.");
            }
            if (oContrato.FechaDolarizado.HasValue && oContrato.FechaDolarizado.Value < oContrato.FechaOperacion)
            {
                result.Error("dolarizado", "La fecha de dolarizado no es válida");
            }

            if (oContrato.DolarizadoExpress == true && oContrato.FechaDolarizado > oContrato.FechaOperacion.AddDays(30))
            {
                result.Error("DolarizadoExpress", "La fecha de dolarizado express no puede ser mayor a 30 días.");
            }

            if (oContratoSave.DolarizadoExpress == true && oContrato.Dolarizado == true && oContrato.FechaDolarizado <= oContrato.FechaOperacion.AddDays(30))
            {
                result.Error("Dolarizado", "La fecha de dolarizado no puede ser menor a 30 días.");
            }
            if (oContrato.FechaDolarizado != null)
            {
                var conf = configuracionManager.TraerConfiguraciones();
                if (conf != null)
                {
                    var cantidadDias = PermisosHelper.Is(PermisosDataAgro.ModificarLimiteDolarizado) ? conf.CantidadDiasDolarizadoLimiteMaximo : conf.CantidadDias;

                    var fechaLimite = oContrato.FechaOperacion.AddDays(cantidadDias);
                    if (oContrato.FechaDolarizado.Value.Date > fechaLimite.Date)
                    {
                        result.Error("Fecha Dolarizado", "La fecha dolarizado debe ser menor o igual que los " + cantidadDias + " días");
                    }
                }
            }
            if (oContrato.PagoDiferido == true && oContrato.DiasPesificado != null)
            {
                var conf = configuracionManager.TraerConfiguraciones();
                if (conf != null)
                {
                    var limitePesificado = PermisosHelper.Is(PermisosDataAgro.ModificarLimitePesificado) ? conf.CantidadDiasPesificadoLimite : conf.DiasDiferimiento;
                    if (oContrato.DiasPesificado.Value > limitePesificado)
                    {
                        result.Error("Pago Diferido", "Los dias de pesificado deben ser menor o igual que los " + limitePesificado + " días");
                    }
                }
            }

            return result;
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

        public Resultado AltaFijacionSap(FijacionDePrecioContrato fijacion, List<FijacionVirtualSAPDto> fijacionesVirtuales)
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
                if (error.Errores.Count > 0)
                {
                    return error;
                }
                var fijacionSave = new FijacionDePrecioContrato
                {
                    FijacionSAP = fijacion.FijacionSAP,
                    ConfirmadoSAP = true,
                    ChequeElectronico = fijacion.ChequeElectronico,
                    PagoCBU = fijacion.PagoCBU,
                    TrigoEspecial = fijacion.TrigoEspecial,
                    ContratoSAP = fijacion.ContratoSAP,
                    ContratoId = fijacion.ContratoId,
                    Precio = fijacion.Precio,
                    Cantidad = fijacion.Cantidad,
                    DestinoId = fijacion.DestinoId,
                    Pizarra = fijacion.Pizarra,
                    Posicion = fijacion.Posicion,
                    PagoDiferido = fijacion.PagoDiferido,
                    FechaOperacion = fijacion.FechaOperacion,
                    Fecha = fijacion.Fecha,
                    FechaHasta = fijacion.FechaHasta,
                    FechaDesde = fijacion.FechaDesde,
                    ProveedorId = fijacion.ProveedorId,
                    ComercialId = fijacion.ComercialId,
                    MaterialId = fijacion.MaterialId,
                    CorredorId = fijacion.CorredorId,
                    DiasPesificado = fijacion.DiasPesificado,
                    MonedaId = fijacion.MonedaId,
                    CampanaId = fijacion.CampanaId,
                    PrecioNeto = fijacion.PrecioNeto,
                    EstadoId = fijacion.EstadoId,
                    FechaConfirmacion = fijacion.FechaConfirmacion,
                    TipoNegocioId = fijacion.TipoNegocioId,
                    ComercialCreadorId = fijacion.ComercialCreadorId,
                    Canje = fijacion.Canje,
                    Virtual = fijacion.Virtual,
                    GrupoCompra = fijacion.GrupoCompra,
                    DescripcionOperacionAnterior = fijacion.MotivoOperacionAnterior,
                    MotivoOperacionAnterior = "Otro",
                    ClasificacionContrato = fijacion.ClasificacionContrato,
                    TipoPosicionCBOTId = fijacion.TipoPosicionCBOTId
                };

                if (fijacion.AperturaPrecio != null)
                {
                    var aperturas = repositorio.Listar<AperturaPrecio>(x => x.NegocioId != null && x.NegocioId == fijacionSave.Id);
                    repositorio.RemoverTodos(aperturas);
                    logger.Error("Iniciando Apertura");
                    fijacionSave.AperturaPrecio = fijacion.AperturaPrecio;
                }
                RelacionarVirtualConCanje(fijacionesVirtuales, fijacion, fijacionSave);
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

        private void RelacionarVirtualConCanje(List<FijacionVirtualSAPDto> fijacionesVirtuales, FijacionDePrecioContrato fijacion, FijacionDePrecioContrato fijacionSave)
        {
            fijacionSave.FijacionCanje = new List<FijacionVirtualSap>();
            logger.Error("Inicio Alta Cierre de canje");
            try
            {
                if (fijacionesVirtuales != null && fijacionesVirtuales.Count > 0)
                {
                    logger.Error("Fijacion de canje" + JsonConvert.SerializeObject(fijacionesVirtuales));

                    foreach (var item in fijacionesVirtuales)
                    {
                        var f = new FijacionVirtualSap()
                        {
                            FijacionVirtualId = repositorio.Obtener<FijacionDePrecioContrato, int?>(x => x.FijacionSAP == item.NumeroFijacionVirtual, x => x.Id),
                            Cantidad = item.Cantidad,
                            FijacionVirtualNro = item.NumeroFijacionVirtual
                        };
                        fijacionSave.FijacionCanje.Add(f);
                    }
                    logger.Error("Fin cierre de canje");
                }
            }
            catch (Exception e)
            {
                logger.Error("Error Cierre de canje");
                logger.Error("", e.Message);
                logger.Error(e);
            }

        }

        public Resultado AnularFijacionSAP(FijacionSAP fijacion, FijacionVirtualSAP fijacionVirtual)
        {
            logger.Debug("Inicializar AnularContratoSAP");
            var oFijacionSave = new FijacionDePrecioContrato();
            var oEntityErrors = new Resultado();
            if (fijacion != null)
            {
                var codigo = fijacion.Fijacion.PadLeft(10, '0');
                oFijacionSave = repositorio.ObtenerMayor<FijacionDePrecioContrato, int>(x => x.FijacionSAP == codigo, x => x.Id);
            }
            else if (fijacionVirtual != null)
            {
                oFijacionSave = repositorio.ObtenerMayor<FijacionDePrecioContrato, int>(x => x.FijacionSAP == fijacionVirtual.Contrato && x.Virtual == true, x => x.Id);
            }
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
                    double cantidadKilos = 0, kilosPendientes = 0;
                    if (fijacionVirtual != null)
                    {
                        var kilos = DevolverKilosPendientesAnularFijacionCanje(oFijacionSave.Id);

                        if (kilos.KilosPendientes >= oFijacionSave.Cantidad)
                        {
                            oFijacionSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                            cantidadKilos = oFijacionSave.Cantidad;
                            kilosPendientes = 0;
                        }
                        else
                        {
                            oFijacionSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                            oFijacionSave.Cantidad -= kilos.KilosPendientes;
                            cantidadKilos = kilos.KilosPendientes;
                            kilosPendientes = oFijacionSave.Cantidad - kilos.KilosPendientes;
                        }
                    }
                    else
                    {
                        oFijacionSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                        cantidadKilos = oFijacionSave.Cantidad;
                        kilosPendientes = 0;
                    }

                    LogAnulacionContrato logAnulacionContrato = new LogAnulacionContrato()
                    {
                        Fecha = DateTime.Now,
                        NegocioId = oFijacionSave.Id,
                        TipoNegocio = repositorio.Obtener<TipoNegocio, string>(x => x.TipoNegocioId == oFijacionSave.TipoNegocioId, x => x.Descripcion),
                        ComercialId = (int)oFijacionSave.ComercialId,
                        ContratoSAP = oFijacionSave.ContratoSAP,
                        FijacionSAP = oFijacionSave.FijacionSAP,
                        CantidadKilos = cantidadKilos,
                        KilosPendientes = kilosPendientes,
                    };
                    repositorio.Agregar<LogAnulacionContrato>(logAnulacionContrato);

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

        public Resultado AnularFijacionCarga(int fijacionId, string motivoRechazo)
        {
            var oEntityErrors = new Resultado();
            if (string.IsNullOrEmpty(motivoRechazo) || string.IsNullOrWhiteSpace(motivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var fijacion = repositorio.Obtener<FijacionDePrecioContrato>(fijacionId);

            fijacion.MotivoRechazo = motivoRechazo;
            if (fijacion != null && fijacion.EstadoId == (int)EnumEstadoContrato.PreAprobacion)
            {
                try
                {
                    fijacion.EstadoId = (int)EnumEstadoContrato.Eliminado;
                    fijacion.EstadoId = (int)EnumEstadoContrato.Eliminado;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacion.Id), TipoAccionLogDataAgro.Eliminar, fijacion.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "La Fijación no se puede anular");
            }
            return oEntityErrors;
        }
        public GrabarFijacionResult PreAnularFijacion(int contratoId, string motivo)
        {
            var oEntityErrors = new GrabarFijacionResult();

            var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(contratoId);

            if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado))
            {
                try
                {
                    if (oContratoSave.Virtual == null)
                    {
                        if (!ValidarFijacionDisponibleParaAnular(oContratoSave, oEntityErrors))
                        {
                            return oEntityErrors;
                        }
                    }
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.PreAnulado;
                    oContratoSave.MotivoRechazo = motivo;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }
            }
            return oEntityErrors;
        }
        public void BuscarComision(BasicoContrato negocio)
        {
            if (negocio.TipoNegocioId == 3 && (negocio.ImporteComision ?? 0) == 0 && (negocio.PorcentajeComision ?? 0) == 0)
            {
                var fijacion = TraerFijacion(negocio.Id);

                string typeOfRate = contratoManager.ObtenerTypeOfRate(negocio.TipoNegocioId, negocio.MonedaId, negocio.TipoAgenteCompraId, (DateTime)negocio.Fecha, negocio.Id != 0);
                if (fijacion.PorcentajeSobrePrecioContrato > 0)
                {
                    negocio.PorcentajeComision = fijacion.PorcentajeSobrePrecioContrato;
                }
                else if ((fijacion.ImporteSobrePrecioContrato ?? 0) != 0)
                {
                    negocio.ImporteComision = fijacion.ImporteSobrePrecioContrato;
                    if (fijacion.MonedaSobrePrecioContrato != negocio.MonedaId)
                    {
                        var cambio = tipoDeCambioAgent.TraerTipoDeCambio(negocio.FechaOperacion, typeOfRate);
                        if (negocio.MonedaId == "ARP  ")
                        {
                            negocio.ImporteComision = negocio.ImporteComision * cambio;
                        }
                        else
                        {
                            negocio.ImporteComision = negocio.ImporteComision / cambio;
                        }
                    }
                }
                else
                {
                    string contratoSAP = fijacion.DatosFijacion.ContratoId.PadLeft(10, '0');
                    var afijar = repositorio.Obtener<Contrato>(a => a.TipoNegocioId == 1 && a.ContratoSAP == contratoSAP && a.EstadoId == 5);
                    if (afijar != null && afijar.Descuentos != null && afijar.Descuentos.Count > 0)
                    {
                        var descuento = afijar.Descuentos.Where(a => a.TipoDBId == 1 && a.TipoPeriodoDBId == 1).FirstOrDefault();
                        if (descuento != null)
                        {
                            if (descuento.Porcentaje > 0)
                            {
                                negocio.PorcentajeComision = descuento.Porcentaje;
                            }
                            else if (descuento.Importe != 0)
                            {
                                negocio.ImporteComision = descuento.Importe;
                                if (descuento.MonedaId != negocio.MonedaId)
                                {
                                    var cambio = tipoDeCambioAgent.TraerTipoDeCambio(negocio.FechaOperacion, typeOfRate);
                                    if (negocio.MonedaId == "ARP  ")
                                    {
                                        negocio.ImporteComision = negocio.ImporteComision * cambio;
                                    }
                                    else
                                    {
                                        negocio.ImporteComision = negocio.ImporteComision / cambio;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public GrabarFijacionResult AnularFijacion(int fijacionId, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarFijacionResult();

            var oFijacionSave = repositorio.Obtener<FijacionDePrecioContrato>(fijacionId);

            if (oFijacionSave != null && (oFijacionSave.EstadoId == (int)EnumEstadoContrato.PreAnulado))
            {
                if (oFijacionSave.Virtual == true)
                {
                    var respuesta = anularFijacionVirtual.AnularFijacionVirtual(oFijacionSave, idActiveDirectory);
                    if (respuesta.Contains("OK"))
                    {
                        try
                        {
                            var kilos = DevolverKilosPendientesAnularFijacionCanje(fijacionId);
                            if (kilos.KilosPendientes >= oFijacionSave.Cantidad)
                            {
                                oFijacionSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                            }
                            else
                            {
                                oFijacionSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                                oFijacionSave.Cantidad -= kilos.KilosPendientes;
                            }

                            repositorio.GuardarCambios();
                            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacionId), TipoAccionLogDataAgro.Eliminar, oFijacionSave.GetType());
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                            oEntityErrors.Error("", e.Message);

                        }
                        //mobjProveedorManager.EnviarMailFijacionVirtual(oFijacionVirtualSave, idActiveDirectory, true);
                    }
                }
                else
                {
                    if (ValidarFijacionDisponibleParaAnular(oFijacionSave, oEntityErrors))
                    {
                        try
                        {
                            oFijacionSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                            repositorio.GuardarCambios();
                            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacionId), TipoAccionLogDataAgro.Eliminar, oFijacionSave.GetType());
                        }
                        catch (Exception e)
                        {
                            logger.Error(e);
                            oEntityErrors.Error("", e.Message);
                        }
                    }
                }
            }
            return oEntityErrors;
        }
        public GrabarFijacionResult RechazarPreAnularFijacionVirtual(int contratoId/*, string motivo*/)
        {
            var oEntityErrors = new GrabarFijacionResult();

            var fijacionSave = repositorio.Obtener<FijacionDePrecioContrato>(contratoId);
            //if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            //{
            //    oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
            //    return oEntityErrors;
            //}
            if (fijacionSave != null && (fijacionSave.EstadoId == (int)EnumEstadoContrato.PreAnulado))
            {
                try
                {
                    fijacionSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                    //oContratoSave.MotivoRechazo = motivo;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacionSave.Id), TipoAccionLogDataAgro.Crear, fijacionSave.GetType());

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }

            }

            return oEntityErrors;
        }

        public ResultadoDevolverKilosPendientesAnularFijacionCanjeDto DevolverKilosPendientesAnularFijacionCanje(int id)
        {
            ResultadoDevolverKilosPendientesAnularFijacionCanjeDto result = new ResultadoDevolverKilosPendientesAnularFijacionCanjeDto();
            try
            {
                var fijacionDePrecio = repositorio.Obtener<FijacionDePrecioContrato>(x => x.Id == id && x.Virtual == true);

                if (fijacionDePrecio.EstadoId != (int)EnumEstadoContrato.Finalizado && fijacionDePrecio.EstadoId != (int)EnumEstadoContrato.PreAnulado)
                {
                    return result;
                }
                var kilosConsumidos = repositorio.Listar<FijacionVirtualSap>(x => x.FijacionVirtualId == id && x.FijacionCanje.EstadoId == (int)EnumEstadoContrato.Finalizado).Sum(a => a.Cantidad);
                result.KilosPendientes = fijacionDePrecio.Cantidad - kilosConsumidos;

                return result;
            }
            catch (Exception e)
            {
                logger.Debug("Error al DevolverKilosPendientesAnularFijacionCanje id " + id);
                logger.Debug(e);
            }

            return result;
        }

        private decimal Redondear(decimal numero)
        {
            double final;
            double d10 = decimal.ToDouble(numero) / 10.00;
            final = Math.Round(d10 * 2, MidpointRounding.AwayFromZero) / 2;
            final = final * 10;
            return Convert.ToDecimal(final);
        }

        public GrabarFijacionResult GrabarFijacionDePrecioTercero(FijacionDePrecioContrato fijacion)
        {
            fijacion.FechaOperacion = new DateTime(fijacion.FechaOperacion.Year, fijacion.FechaOperacion.Month, fijacion.FechaOperacion.Day);

            if (fijacion.ComercialId == null || fijacion.ComercialId == 0)
            {
                fijacion.ComercialId = mobjComercialManager.ComercialAsociado(fijacion.CorredorId.HasValue && fijacion.CorredorId != 0 ? fijacion.CorredorId.Value : fijacion.ProveedorId ?? 0);
            }
            var comercial = mobjComercialManager.TraerComercial(fijacion.ComercialId.Value);
            var proveedorCreador = mobjProveedorManager.TraerProveedor(fijacion.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(fijacion.ProveedorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var cuitCorredor = "";
            if (fijacion.CorredorId > 0)
            {
                cuitCorredor = mobjProveedorManager.TraerProveedor(fijacion.CorredorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First().CUIT;
            }
            fijacion.UsuarioId = proveedorCreador.RazonSocial;

            if (fijacion.AperturaPrecio == null)
            {
                fijacion.AperturaPrecio = new List<AperturaPrecio>();

                foreach (EnumConceptoApertura concepto in (EnumConceptoApertura[])Enum.GetValues(typeof(EnumConceptoApertura)))
                {
                    fijacion.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = (int)concepto, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }
            }

            if (fijacion.PagoDiferidoTercero == true)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido().Where(x => x.CantidadDia >= fijacion.DiasPesificado).OrderBy(x => x.CantidadDia).FirstOrDefault();
                if (pago == null)
                {
                    return new GrabarFijacionResult { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "PagoDiferido", Message = "No hay una tasa de pago diferido para esa cantidad de dias." } } };
                }
                fijacion.PagoDiferido = fijacion.PagoDiferidoTercero;
                decimal ImporteFinanciero = Redondear(Math.Round(fijacion.Precio * (pago.Tasa / 100) * (fijacion.DiasPesificado.Value - 3) / 365));
                fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe = ImporteFinanciero;
                fijacion.PrecioNeto = fijacion.Precio + ImporteFinanciero;


            }

            var afijar = TraerDatosFijacion(proveedor.CUIT, cuitCorredor, fijacion.MaterialId, fijacion.ContratoSAP.TrimStart('0'), fijacion.Id);
            string typeOfRate = contratoManager.ObtenerTypeOfRate(fijacion.TipoNegocioId, fijacion.MonedaId, fijacion.TipoAgenteCompraId, fijacion.Fecha, fijacion.Id != 0);
            var cambio = tipoDeCambioAgent.TraerTipoDeCambio(fijacion.FechaOperacion, typeOfRate);
            if (afijar[0].Aperturas != null && afijar[0].Aperturas.Count > 0)
            {
                var aperturaAFijar = afijar[0].Aperturas;
                //redespacho
                if (afijar[0].Aperturas.Any(x => x.ConceptoAperturaPrecioId == 2))
                {
                    var redespacho = afijar[0].Aperturas.First(x => x.ConceptoAperturaPrecioId == 2).Importe;
                    redespacho = CalcularImporteSiEsEnDolares(redespacho, afijar, fijacion, cambio);
                    logger.Debug("redespacho: " + redespacho);
                    fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 2).Importe = redespacho;
                    fijacion.PrecioNeto = fijacion.Precio + redespacho;
                    logger.Debug("redespachoPrecioNeto: " + fijacion.PrecioNeto);

                }
                if (afijar[0].Aperturas.Any(x => x.ConceptoAperturaPrecioId == 4))
                {
                    var bonif = afijar[0].Aperturas.First(x => x.ConceptoAperturaPrecioId == 4).Importe;
                    bonif = CalcularImporteSiEsEnDolares(bonif, afijar, fijacion, cambio);
                    logger.Debug("bonif: " + bonif);
                    fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 4).Importe = bonif;
                    fijacion.PrecioNeto = fijacion.Precio + bonif;
                    logger.Debug("bonifPrecioNeto: " + fijacion.PrecioNeto);
                }
                if (afijar[0].Aperturas.Any(x => x.ConceptoAperturaPrecioId == 5))
                {
                    var basis = afijar[0].Aperturas.First(x => x.ConceptoAperturaPrecioId == 5).Importe;
                    basis = CalcularImporteSiEsEnDolares(basis, afijar, fijacion, cambio);
                    logger.Debug("basis: " + basis);
                    fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 5).Importe = basis;
                    fijacion.PrecioNeto = fijacion.Precio + basis;
                    logger.Debug("basisPrecioNeto: " + fijacion.PrecioNeto);
                }
            }
            if (afijar != null && afijar.Count > 0)
            {
                decimal comisionImporte = 0;
                decimal comisionPorcentaje = 0;
                fijacion.ClasificacionContrato = afijar[0].Clasificacion;
                fijacion.ImporteSobrePrecioContrato = afijar[0].ImporteSobrePrecio;
                fijacion.MonedaSobrePrecioContrato = afijar[0].MonedaSobrePrecio;
                fijacion.PorcentajeSobrePrecioContrato = afijar[0].PorcentajeSobrePrecio;
                fijacion.ImporteAPrecioContrato = afijar[0].ImporteAPrecio;
                fijacion.MonedaAPrecioContrato = afijar[0].MonedaAPrecio;
                fijacion.PorcentajeAPrecioContrato = afijar[0].PorcentajeAPrecio;
                if (afijar[0].ImporteSobrePrecio > 0)
                {
                    if (afijar[0].MonedaSobrePrecio?.Trim() == fijacion.MonedaId.Trim())
                    {
                        fijacion.PrecioNeto += afijar[0].ImporteSobrePrecio;
                        comisionImporte = afijar[0].ImporteSobrePrecio;
                    }
                    else
                    {
                        if (fijacion.MonedaId.Trim() == "ARP")
                        {
                            fijacion.PrecioNeto += afijar[0].ImporteSobrePrecio * cambio;
                            comisionImporte = afijar[0].ImporteSobrePrecio * cambio;
                        }
                        else
                        {
                            fijacion.PrecioNeto += afijar[0].ImporteSobrePrecio / cambio;
                            comisionImporte = afijar[0].ImporteSobrePrecio / cambio;
                        }
                    }

                }

                if (afijar[0].PorcentajeSobrePrecio > 0)
                {
                    fijacion.PrecioNeto += fijacion.PrecioNeto * afijar[0].PorcentajeSobrePrecio / 100;
                    comisionPorcentaje = afijar[0].PorcentajeSobrePrecio;
                }
                if (afijar[0].ImporteSobrePrecio > 0)
                {

                    fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 3).Importe = comisionImporte;
                    logger.Debug("comisionImporte: " + comisionImporte);
                }
                if (afijar[0].PorcentajeSobrePrecio > 0)
                {

                    fijacion.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 3).Porcentaje = comisionPorcentaje;
                    logger.Debug("comisionPorcentaje: " + comisionPorcentaje);
                }
            }
            fijacion.PagoDiferidoTerceroId = fijacion.PagoDiferidoTerceroId == -1 ? null : fijacion.PagoDiferidoTerceroId;
            fijacion.ContratoId = null;

            return GrabarFijacionDePrecio(fijacion);
        }
        public decimal CalcularImporteSiEsEnDolares(decimal importe, List<DatosFijacionDeContratoDto> afijar, FijacionDePrecioContrato fijacion, decimal cambio)
        {
            decimal nuevoImporte = 0;
            if (afijar[0].MonedaSobrePrecio?.Trim() == fijacion.MonedaId.Trim())
            {
                nuevoImporte = importe;
            }
            else
            {
                if (fijacion.MonedaId.Trim() == "ARP")
                {
                    nuevoImporte = importe * cambio;
                }
                else
                {
                    nuevoImporte = importe / cambio;
                }
            }

            return nuevoImporte;
        }

        private bool ValidarFijacionDisponibleParaAnular(FijacionDePrecioContrato fijacion, GrabarFijacionResult resultado)
        {

            var puedoAnular = true;
            var resultadoLiquidacionParcial = validarLiquidacionParcialAgent.ValidarLiquidacionParcial(fijacion);
            if (string.IsNullOrEmpty(resultadoLiquidacionParcial) || resultadoLiquidacionParcial != "OK")
            {
                resultado.Error("", resultadoLiquidacionParcial);
                puedoAnular = false;
                return puedoAnular;
            }
            var resultadoLiquidacionFinal = validarLiquidacionFinalAgent.ValidarLiquidacionFinal(fijacion);
            if (string.IsNullOrEmpty(resultadoLiquidacionFinal) || resultadoLiquidacionFinal != "OK")
            {
                resultado.Error("", resultadoLiquidacionFinal);
                puedoAnular = false;
                return puedoAnular;
            }
            var resultadoPesificacion = validarPesificacionAgent.ValidarPesificacion(fijacion);
            if (string.IsNullOrEmpty(resultadoPesificacion) || resultadoPesificacion != "OK")
            {
                resultado.Error("", resultadoPesificacion);
                puedoAnular = false;
                return puedoAnular;
            }

            var resultadoLiquidacion = validarLiquidacionComisionesAgent.ValidarLiquidacionComisiones(fijacion);
            if (string.IsNullOrEmpty(resultadoLiquidacion) || resultadoLiquidacion != "OK")
            {
                resultado.Error("", resultadoLiquidacion);
                puedoAnular = false;
                return puedoAnular;
            }


            return puedoAnular;
        }


        public void ConfirmacionAutomaticaPizarra13Hrs()
        {
            var oEntityErrors = new GrabarContratoResult();
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION &&
                                                                               x.EstadoId == (int)EnumEstadoContrato.Pendiente &&
                                                                               (x.Contrato.CondicionFijacion.CodigoSap == "02" ||
                                                                               x.Contrato.CondicionFijacion.CodigoSap == "05") &&
                                                                               x.Cantidad <= x.Contrato.KgMaximo);

            foreach (var fijacion in fijaciones)
            {
                try
                {
                    logger.Debug("Fijacion confirmada automaticamente:" + fijacion.Id);
                    fijacion.FechaConfirmacion = DateTime.Now;
                    fijacion.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacion.Id), TipoAccionLogDataAgro.Eliminar, fijacion.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    oEntityErrors.Error("", ex.Message);
                }
            }
        }

        public Resultado ConfirmarFijacionSAP(string fijacionSAP)
        {
            var resultado = new Resultado();
            if (string.IsNullOrEmpty(fijacionSAP))
            {
                resultado.Error("fijacionSAP", "fijacionSAP no puede ser null.");
                return resultado;
            }
            fijacionSAP = fijacionSAP.PadLeft(10, '0');
            var fijacion = repositorio.Obtener<FijacionDePrecioContrato>(x => x.FijacionSAP == fijacionSAP && x.EstadoId == (int)EnumEstadoContrato.Finalizado);
            if (fijacion == null)
            {
                resultado.Error("fijacionSAP", $"No se encontro la fijacion Nro {fijacionSAP}.");
                return resultado;
            }

            fijacion.ConfirmadoSAP = true;
            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerFijacion(fijacion.Id), TipoAccionLogDataAgro.Crear, fijacion.GetType());

            return resultado;
        }
    }
}