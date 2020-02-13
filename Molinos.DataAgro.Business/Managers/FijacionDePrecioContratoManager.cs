using Autofac.Extras.NLog;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
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

        public FijacionDePrecioContratoManager(
            ILogger logger,
            IRepositorio repositorio,
            IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager,
            IPushNotificationManager oMSNotification,
            IFinalizarFijacionAgent oFinalizarFijacionAgent,
            IContratosParaFijacionAgent oContratosParaFijacionAgent,
            IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent,
            IMailManager mailManager)
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
            var oFijacionDePrecioContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oFijacion.FijacionDePrecioContratoId);
            var oEntityErrors = new GrabarContratoResult();

            if (oFijacionDePrecioContratoSave.Estado.EstadoContratoId != (int)EnumEstadoContrato.Finalizado && oFijacionDePrecioContratoSave.Estado.EstadoContratoId != (int)EnumEstadoContrato.Rechazado)
            {
                oFijacionDePrecioContratoSave.Ampliaciones = oFijacion.Ampliaciones.Value;
                oFijacionDePrecioContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Pendiente);
                try
                {
                    repositorio.GuardarCambios();
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

        private Resultado Validar(FijacionDePrecioContrato oParam, Resultado oErrorMessages)
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
            if (string.IsNullOrEmpty(oParam.ContratoSAP))
            {
                oErrorMessages.Error("Contrato", "El campo 'Contrato' no debe estar vacio");
            }
            if (oParam.Cantidad < 0)
            {
                oErrorMessages.Error("Cantidad", "El campo 'Cantidad' no debe ser negativo");
            }
            var cuitCorredor = oParam.CorredorId.HasValue ? mobjProveedorManager.TraerCuit(oParam.CorredorId.Value):"";
            var cuitProveedor =  mobjProveedorManager.TraerCuit(oParam.ProveedorId);
            var listaContrato = oContratosParaFijacionAgent.ObtenerContratos(cuitProveedor, cuitCorredor, oParam.MaterialId.Value, "", oParam.FijacionDePrecioContratoId);
            if (string.IsNullOrEmpty(oParam.ContratoSAP))
            {
                oErrorMessages.Error("ContratoId", "El campo 'Contrato' no debe estar vacio");
                return oErrorMessages;
            }
            var fijacion = listaContrato.FirstOrDefault(x => x.ContratoId == oParam.ContratoSAP.TrimStart('0'));
            if (fijacion == null)
            {
                oErrorMessages.Error("ContratoId", "El Contrato no existe");
                return oErrorMessages;
            }
            else
            {
                var cantidadContrato = fijacion.KilosPendiente;
                if (double.Parse(cantidadContrato.Replace(".", "")) - oParam.Cantidad < 0)
                {
                    oErrorMessages.Error("Cantidad", "La cantidad excede a los kilos del contrato");
                }
                oParam.TrigoEspecial = fijacion.Calidad.Value;
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
            var rangosPrecio = repositorio.Listar<RangoPrecio>();
            if (rangosPrecio.Exists(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId && (x.PrecioMaximo < oParam.Precio || x.PrecioMinimo > oParam.Precio)))
            {
                oErrorMessages.Error("Precio", "Precio fuera de Rango");
            }
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "Campaña del Contrato seleccionado fuera del rango");
            }
            if (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value && oParam.DiasPesificado == null)
            {
                oErrorMessages.Error("CampanaId", "Se debe completar la los Días en negocios de Pago Diferido");
            }
            if ( oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0 && 
                oParam.MonedaId!= "ARP  " )
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
            return oErrorMessages;
        }

        public GrabarFijacionResult GrabarFijacionDePrecio(FijacionDePrecioContrato oFijacionDePrecio)
        {
            var oEntityErrors = new GrabarFijacionResult();

            this.Validar(oFijacionDePrecio, oEntityErrors);

            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }
            var hoy = DateTime.Now;
        
            var oFijacionDePrecioSave = oFijacionDePrecio;
            var oContratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == oFijacionDePrecio.ContratoSAP, x => x.ContratoId);
            if (oFijacionDePrecio.FijacionDePrecioContratoId != 0)
            {
                oFijacionDePrecioSave = repositorio.Obtener<FijacionDePrecioContrato>(oFijacionDePrecio.FijacionDePrecioContratoId);
                if (oFijacionDePrecioSave.EstadoId == 5 || oFijacionDePrecioSave.EstadoId == 6)
                {
                    oEntityErrors.Error("", "La Fijación no se puede modificar");
                    return oEntityErrors;
                }
                if ((oFijacionDePrecioSave.Precio != oFijacionDePrecio.Precio || oFijacionDePrecioSave.Cantidad != oFijacionDePrecio.Cantidad) && (oFijacionDePrecioSave.EstadoId != 1 && oFijacionDePrecioSave.EstadoId != 3))
                {
                    oFijacionDePrecioSave.EstadoId = 7;
                }
                oFijacionDePrecioSave.Precio = oFijacionDePrecio.Precio;
                oFijacionDePrecioSave.Cantidad = oFijacionDePrecio.Cantidad;
                oFijacionDePrecioSave.Ampliaciones = oFijacionDePrecio.Ampliaciones;
                oFijacionDePrecioSave.Observacion = oFijacionDePrecio.Observacion;
                oFijacionDePrecioSave.ProveedorId = oFijacionDePrecio.ProveedorId;
                oFijacionDePrecioSave.ComercialId = oFijacionDePrecio.ComercialId;
                oFijacionDePrecioSave.ContratoId = oContratoId != 0 ? oContratoId : (int?)null;
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
                if (oFijacionDePrecio.AperturaPrecio != null)
                {
                    var aperturas = repositorio.Listar<AperturaPrecio>(x => x.FijacionId != null && x.FijacionId == oFijacionDePrecioSave.FijacionDePrecioContratoId);
                    repositorio.RemoverTodos(aperturas);
                    oFijacionDePrecioSave.AperturaPrecio = oFijacionDePrecio.AperturaPrecio;
                }
            }
            else
            {
                oFijacionDePrecio.ContratoSAP = oFijacionDePrecio.ContratoSAP.PadLeft(10, '0');
                oFijacionDePrecio.Fecha = DateTime.Now;
                if (oContratoId == 0)
                {
                    oFijacionDePrecioSave.ContratoId = null;
                }
                else
                {
                    oFijacionDePrecioSave.ContratoId = oContratoId;
                }
                repositorio.Agregar(oFijacionDePrecioSave);
            }
            if (oFijacionDePrecio.EstadoId < (int)EnumEstadoContrato.PreAprobacion && ConfirmacionAutomatica(oFijacionDePrecioSave))
            {
                oFijacionDePrecioSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                logger.Debug("El contrato " + oFijacionDePrecioSave.FijacionDePrecioContratoId + " se finalizo automaticamente por estar dentro de los rangos configurados");
            }
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
        
        private bool ConfirmacionAutomatica(FijacionDePrecioContrato contrato)
        {
            var hoy = DateTime.Now;
            var rango = repositorio.Obtener<RangoConfirmacionAutomatica>(x =>
            x.FechaDesde <= hoy &&
            x.FechaHasta >= hoy &&
            x.MaterialId == contrato.MaterialId &&
            x.MonedaId == contrato.MonedaId);

            if (rango != null)
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                var cantidad =
                    repositorio.Listar<Contrato, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.TipoNegocioId == 2
                 && x.MaterialId == rango.MaterialId);
                cantidad.AddRange(repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.Fecha == hoy &&
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.FijacionDePrecioContratoId != contrato.FijacionDePrecioContratoId 
                && x.MaterialId == rango.MaterialId));

                var total = cantidad.Sum();
                var precioContrato = contrato.Precio;

                var valor = contrato.FechaDesde >= new DateTime(rango.DesdeAnio, rango.DesdeMes, 1) &&
                    contrato.FechaHasta <= new DateTime(rango.HastaAnio, rango.HastaMes, DateTime.DaysInMonth(rango.HastaAnio, rango.HastaMes)) &&
                    (total + contrato.Cantidad) <= rango.Cantidad &&
                    (rango.ZonaId == 47 || rango.ZonaId == null || grupo == rango.ZonaId) &&
                     precioContrato >= rango.PrecioMinimo && precioContrato <= rango.PrecioMaximo;
                return valor;
            }
            else
            {
                return false;
            }
        }
        public GrabarFijacionResult ConfirmarFijacion(int fijacionDePrecioContratoId)
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
                    repositorio.GuardarCambios();
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
                    var objApertura = repositorio.Listar<AperturaPrecio>(x => x.FijacionId == oFijacionDePrecioSave.FijacionDePrecioContratoId);

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
            var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oContrato.FijacionDePrecioContratoId);

            if (oContratoSave.Estado.EstadoContratoId < (int)EnumEstadoContrato.Finalizado)
            {
                oContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Rechazado);

                try
                {
                    repositorio.GuardarCambios();
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
            var cantidad = repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.ContratoSAP == sap.ContratoSAP && x.FijacionDePrecioContratoId != id).Sum();
            var contrato = repositorio.Obtener<FijacionDePrecioContrato, BasicoContrato>(x => x.FijacionDePrecioContratoId == id, fijac => new BasicoContrato
            {
                Id = fijac.FijacionDePrecioContratoId,
                Proveedor = fijac.Proveedor == null ? "" : fijac.Proveedor.RazonSocial + " " + "(" + fijac.Proveedor.CUIT + ")",
                ContratoId = fijac.ContratoId.HasValue ? fijac.ContratoId.Value : 0,
                ProveedorId = fijac.ProveedorId,
                ComercialId = fijac.ComercialId,
                MaterialId = fijac.MaterialId != null ? fijac.MaterialId.Value : 0,
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
                CampanaId = fijac.CampanaId,
                Campania = fijac.Campana.Descripcion,
                Provincia = "",
                TipoNegocio = "FIJACION",
                Localidad = "",
                Observacion = fijac.Observacion ?? "",
                FijacionDePrecioContratoId = fijac.FijacionDePrecioContratoId,
                Sustentable = false,
                Dolarizado = false,
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
                }
            });
            contrato.DatosFijacion.ContratoId = contrato.DatosFijacion.ContratoId.TrimStart('0');
            if (contrato.ContratoId!=0)
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
                contratoId = apertura.ContratoId,
                Id = apertura.Id,
                ConceptoAperturaPrecio = apertura.ConceptoAperturaPrecio.Descripcion,
                ConceptoAperturaPrecioId = apertura.ConceptoAperturaPrecioId,
                Importe = apertura.Importe,
                MonedaId = apertura.MonedaId,
                Porcentaje = apertura.Porcentaje
            },
            x => x.FijacionId == fijacionId);
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
                    var error = FinalizarFijacion(fijacion.FijacionDePrecioContratoId, idActiveDirectory);
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
                oFijacionDePrecioSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Pendiente);

                try
                {
                    repositorio.GuardarCambios();
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
        private void EnviarMailRechazo(FijacionDePrecioContrato fijacion) {
            var id = fijacion.CorredorId.HasValue ? fijacion.CorredorId : fijacion.ProveedorId;
            var enviarA = repositorio.Listar<ContactoComercial,string>(x=>x.Email1,x => x.ProveedorId == id && x.CompraNet == true);
            var asunto = ConfigurationManager.AppSettings["AmbientePruebas"] != "1"? "":"Prueba - ";
            asunto += "Rechazo Fijación Molinos Agro S.A. –  " + fijacion.Proveedor.RazonSocial;
            var copia = new List<string>() { fijacion.Comercial.IdActiveDirectory, ConfigurationManager.AppSettings["CredentialUserName"] };
            var vista = CuerpoMailFijacion(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), fijacion);
            mailManager.EnviarMail(enviarA, asunto,"",copia,vista);
        }
        private AlternateView CuerpoMailFijacion(string filePath, FijacionDePrecioContrato fijacion)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();            
            var mail = "";
                try { mail = mailManager.GetEmailUserActiveDirectory(fijacion.Comercial.IdActiveDirectory); }catch(Exception e) { logger.Error("No existe mail para el usuario en AD" + e.Message); }
            
            var contacto = fijacion.Comercial != null ? fijacion.Comercial.Nombres + " " + fijacion.Comercial.Apellido + (!string.IsNullOrEmpty(mail)? " (" + mail + ")." : ".") : "Mesa de Ayuda.";
            var htmlBody = $"En el presente mail, se informa que el negocio generado con Molinos Agro S.A. ha sido rechazado <br />" +
                $"Motivo: <br />  {fijacion.MotivoRechazo} <br />" +
                $"Ante cualquier consulta contactarse con {contacto}"+
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.   <br /><br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            htmlBody += "<style> table, th, td{ }</style>";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, "text/html");
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
    }
}




