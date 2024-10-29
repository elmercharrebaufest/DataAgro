using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Entities.Validations;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Molinos.DataAgro.Business
{

    public class ContratoAcuerdoManager : IContratoAcuerdoManager
    {
        private readonly ILogger logger;
        private readonly IRepositorio repositorio;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IConfiguracionManager configuracionManager;
        private readonly ICupoManager cupoManager;
        private readonly INegocioManager negocioManager;

        public ContratoAcuerdoManager(ILogger logger, IRepositorio repositorio, IDiasHabilesAgent diasHabilesAgent, ILogDataAgroManager logDataAgroManager,
            IConfiguracionManager configuracionManager, ICupoManager cupoManager, INegocioManager negocioManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.diasHabilesAgent = diasHabilesAgent;
            this.logDataAgroManager = logDataAgroManager;
            this.configuracionManager = configuracionManager;
            this.cupoManager = cupoManager;
            this.negocioManager = negocioManager;
        }

        public GrabarAcuerdoResult BorrarAcuerdo(ContratoAcuerdo oAcuerdo)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            if (string.IsNullOrEmpty(oAcuerdo.MotivoRechazo) || string.IsNullOrWhiteSpace(oAcuerdo.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
                return oEntityErrors;
            }
            if (!repositorio.Existe<Contrato>(x => x.ContratoAcuerdoId == oAcuerdo.Id))
            {
                var oContratoSave = repositorio.Obtener<ContratoAcuerdo>(oAcuerdo.Id);
                oContratoSave.MotivoRechazo = oAcuerdo.MotivoRechazo;

                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado ||
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error ||
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente ||
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar ||
                    oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                    {
                        if (oContratoSave.Ampliaciones > 0)
                        {
                            oContratoSave.Ampliaciones = 0;
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                        }
                        else
                        {
                            var historico = oContratoSave.NegocioHistorico.LastOrDefault();
                            if (historico != null)
                            {
                                ContratoAcuerdo contratoOriginal = JsonConvert.DeserializeObject<ContratoAcuerdo>(historico.Datos);
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
                                oContratoSave.CampanaId = contratoOriginal.CampanaId;
                                oContratoSave.DolarizadoExpress = contratoOriginal.DolarizadoExpress;
                                oContratoSave.FechaCierta = contratoOriginal.FechaCierta;
                                oContratoSave.ObligatoriedadCostoFinanciero = contratoOriginal.FechaCierta.HasValue &&
                                contratoOriginal.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                                contratoOriginal.ObligatoriedadCostoFinanciero.HasValue && !contratoOriginal.ObligatoriedadCostoFinanciero.Value
                                 ? null : contratoOriginal.FechaCierta.HasValue ? contratoOriginal.ObligatoriedadCostoFinanciero : null;

                                if (oContratoSave.PrecioPactado != null)
                                {
                                    for (int i = oContratoSave.PrecioPactado.Count - 1; i > -1; i--)
                                    {
                                        repositorio.Remover(oContratoSave.PrecioPactado.First());
                                    }
                                }
                                else
                                {
                                    oContratoSave.PrecioPactado = new List<PrecioPactado>();
                                }
                                if (contratoOriginal.PrecioPactado != null)
                                {
                                    foreach (var precio in contratoOriginal.PrecioPactado)
                                    {
                                        precio.Id = 0;
                                        oContratoSave.PrecioPactado.Add(precio);
                                    }
                                }

                                if (oContratoSave.Calidad != null)
                                {
                                    for (int i = oContratoSave.Calidad.Count - 1; i > -1; i--)
                                    {
                                        repositorio.Remover(oContratoSave.Calidad.First());
                                    }
                                }
                                else
                                {
                                    oContratoSave.Calidad = new List<Calidad>();
                                }

                                if (contratoOriginal.Calidad != null)
                                {
                                    foreach (var calidad in contratoOriginal.Calidad)
                                    {
                                        repositorio.Agregar(new Calidad { CalidadEspecialId = calidad.CalidadEspecialId, NegocioId = calidad.NegocioId, PorcentajeDesde = calidad.PorcentajeDesde, PorcentajeHasta = calidad.PorcentajeHasta, StandardDeCalidadId = calidad.StandardDeCalidadId, Valor = calidad.Valor });
                                    }
                                }

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
                        logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        oEntityErrors.Error("", "El Contrato Acuerdo no se pudo rechazar.\n\n");
                    }
                }
                else
                {
                    oEntityErrors.Error("", "El Contrato Acuerdo no se puede rechazar.\n\n");
                }
            }
            else
            {
                oEntityErrors.Error("", "El Contrato Acuerdo ya se ha utilizado y no se puede rechazar.\n\n");
            }
            return oEntityErrors;
        }

        public GrabarAcuerdoResult GrabarAcuerdo(ContratoAcuerdo oContratoAcuerdo, List<CupoConDescargaFechasDto> listCupoConDescargaFechas = null)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            EntityValid.ValidateAll(oContratoAcuerdo, oEntityErrors);
            if (oEntityErrors.HayErrores)
            {
                return oEntityErrors;
            }

            Validar(oContratoAcuerdo, oEntityErrors);
            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            ContratoAcuerdo oAcuerdoSave = new ContratoAcuerdo();
            if (oContratoAcuerdo.Id > 0) oAcuerdoSave = repositorio.Obtener<ContratoAcuerdo>(oContratoAcuerdo.Id);
            double kilosParametro = oContratoAcuerdo.Cantidad;

            Cupo cupoNuevo = null;
            if (oContratoAcuerdo.ConDescarga == true)
            {
                oEntityErrors.Errores = negocioManager.ControlesAccesoConDescarga(oContratoAcuerdo).Errores;
                if (oEntityErrors.Errores.Count > 0) return oEntityErrors;
            }

            if (listCupoConDescargaFechas != null)
            {
                if (oContratoAcuerdo.Id > 0 && kilosParametro > oAcuerdoSave.Cantidad)
                {
                    var cuposExistentes = repositorio.Contar<Cupo>(x => x.NegocioId == oContratoAcuerdo.Id);
                    oContratoAcuerdo.Cantidad = kilosParametro - (30000 * cuposExistentes); //conservo la cantidad que aún no tiene cupos
                }
                cupoNuevo = negocioManager.TransformarContratoACupo(oContratoAcuerdo);

                int sumaCuposCargaMasiva = 0;
                bool cargaMasiva = listCupoConDescargaFechas != null && listCupoConDescargaFechas.Count() > 0;
                if (cargaMasiva) sumaCuposCargaMasiva = listCupoConDescargaFechas.Sum(x => x.CantidadCupo) + listCupoConDescargaFechas.Sum(x => x.CantidadFlete);

                var error = cupoManager.Validar(cupoNuevo, sumaCuposCargaMasiva, oContratoAcuerdo.FechaHasta);
                if (error != null)
                {
                    oEntityErrors.Errores.AddRange(error.Errores);
                    oEntityErrors.ListaErrores.AddRange(error.ListaErrores);
                }

                // Validar disponibilidad según LIMITE CUPO CON DESCARGA
                listCupoConDescargaFechas.ForEach(x =>
                {
                    int sumaCuposPorFecha = x.CantidadCupo + x.CantidadFlete;

                    CupoResult cupoResult = cupoManager.ValidarDisponibilidadCuperaConDescarga(oContratoAcuerdo.MaterialId, (int)oContratoAcuerdo.DestinoId, x.Fecha, sumaCuposPorFecha);
                    if (cupoResult.HayError)
                        cupoResult.Errores.ForEach(y => oEntityErrors.Errores.Add(new ErrorMessage(400, y.Message)));
                });

                var cantidadCuposFletesPermitidos = Math.Ceiling(oContratoAcuerdo.Cantidad / 30000);
                if (cantidadCuposFletesPermitidos < sumaCuposCargaMasiva)
                {
                    string mensaje = oContratoAcuerdo.Id == 0 || kilosParametro == oAcuerdoSave.Cantidad ? ".\n\n" : " que aún no tienen cupos.\n\n";
                    oEntityErrors.Errores.Add(new ErrorMessage(400, "La cantidad de cupos ingresada se excede con respecto a los kilos del negocio" + mensaje));
                }

                oContratoAcuerdo.Cantidad = kilosParametro;
            }

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            oContratoAcuerdo.CorredorId = (oContratoAcuerdo.CorredorId == -1) ? null : oContratoAcuerdo.CorredorId;
            oContratoAcuerdo.ProveedorId = (oContratoAcuerdo.ProveedorId == -1) ? null : oContratoAcuerdo.ProveedorId;

            var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 1;
            if (PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados))
            {
                oContratoAcuerdo.FechaConfirmacion = DateTime.Now;
            }

            oContratoAcuerdo.DolarizadoCorredor = false;
            if (oContratoAcuerdo.CorredorId.HasValue && oContratoAcuerdo.Dolarizado == true)
            {
                oContratoAcuerdo.Dolarizado = false;
                oContratoAcuerdo.DolarizadoCorredor = true;
            }

            if (oContratoAcuerdo.AperturaPrecio == null)
                oContratoAcuerdo.AperturaPrecio = new List<AperturaPrecio>();

            if (oContratoAcuerdo.Id == 0)
            {
                oAcuerdoSave.Fecha = DateTime.Now;
                oAcuerdoSave.EstadoId = estado;
                oAcuerdoSave.FechaOperacion = DateTime.Now;
                oAcuerdoSave.ComercialCreadorId = oContratoAcuerdo.ComercialCreadorId;
            }
            else //se está modificando
            {

                if (estado == 1 && oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                {
                    string jsonContrato = JsonConvert.SerializeObject(oAcuerdoSave, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    oAcuerdoSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oContratoAcuerdo.Id, TipoNegocioId = oContratoAcuerdo.TipoNegocioId, ComercialId = oContratoAcuerdo.ComercialId });
                }
            }

            if (oAcuerdoSave.Descuentos != null)
            {
                foreach (var descExistente in oAcuerdoSave.Descuentos.ToList())
                {
                    if (oAcuerdoSave.Descuentos == null || !oAcuerdoSave.Descuentos.Any(x => x.Id == descExistente.Id))
                    {
                        repositorio.Remover(descExistente);
                    }
                }
            }
            if (oContratoAcuerdo.Descuentos != null)
            {
                foreach (var descuento in oContratoAcuerdo.Descuentos.Where(x => x.Id == 0))
                {
                    descuento.Negocio = oAcuerdoSave;
                    repositorio.Agregar(descuento);
                }
            }

            if (oAcuerdoSave.Calidad != null)
            {
                foreach (var cal in oAcuerdoSave.Calidad.ToList())
                {
                    repositorio.Remover(cal);
                }
            }
            if (oContratoAcuerdo.Calidad != null)
            {
                oAcuerdoSave.Calidad = oContratoAcuerdo.Calidad;
            }

            if (oAcuerdoSave.AperturaPrecio != null)
            {
                foreach (var ap in oAcuerdoSave.AperturaPrecio.ToList())
                {
                    repositorio.Remover(ap);
                }
            }
            if (oContratoAcuerdo.AperturaPrecio != null && oContratoAcuerdo.AperturaPrecio.Count > 0)
            {
                if (oContratoAcuerdo.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
                {
                    var redespacho = oContratoAcuerdo.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho);
                    redespacho.Importe = -1 * Math.Abs(redespacho.Importe);
                }
                oAcuerdoSave.AperturaPrecio = oContratoAcuerdo.AperturaPrecio;
            }

            if (oAcuerdoSave.Servicios != null)
            {
                foreach (var serv in oAcuerdoSave.Servicios.ToList())
                {
                    repositorio.Remover(serv);
                }
            }
            if (oContratoAcuerdo.Servicios != null)
            {
                oAcuerdoSave.Servicios = oContratoAcuerdo.Servicios;
            }

            if (oAcuerdoSave.PrecioPactado != null)
            {
                foreach (var precio in oAcuerdoSave.PrecioPactado.ToList())
                {
                    repositorio.Remover(precio);
                }
            }
            if (oContratoAcuerdo.PrecioPactado != null)
            {
                oAcuerdoSave.PrecioPactado = oContratoAcuerdo.PrecioPactado;
            }

            oAcuerdoSave.ObligatoriedadCostoFinanciero = oContratoAcuerdo.FechaCierta.HasValue &&
                oContratoAcuerdo.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                oContratoAcuerdo.ObligatoriedadCostoFinanciero.HasValue && !oContratoAcuerdo.ObligatoriedadCostoFinanciero.Value
                ? null : oContratoAcuerdo.FechaCierta.HasValue ? oContratoAcuerdo.ObligatoriedadCostoFinanciero : null;
            oContratoAcuerdo.DolarizadoCorredor = false;
            if (oContratoAcuerdo.CorredorId.HasValue && oContratoAcuerdo.Dolarizado == true)
            {
                oContratoAcuerdo.Dolarizado = false;
                oContratoAcuerdo.DolarizadoCorredor = true;
            }
            oAcuerdoSave.MaterialId = oContratoAcuerdo.MaterialId;
            oAcuerdoSave.TipoNegocioId = oContratoAcuerdo.TipoNegocioId;
            oAcuerdoSave.Cantidad = oContratoAcuerdo.Cantidad;
            oAcuerdoSave.Venta = oContratoAcuerdo.Venta;
            if (oAcuerdoSave.Venta == true) oAcuerdoSave.Cantidad = -Math.Abs(oAcuerdoSave.Cantidad);
            oAcuerdoSave.Precio = oContratoAcuerdo.Precio;
            oAcuerdoSave.FechaEntrega = oContratoAcuerdo.FechaEntrega;
            oAcuerdoSave.CampanaId = oContratoAcuerdo.CampanaId;
            oAcuerdoSave.FechaDesde = oContratoAcuerdo.FechaDesde;
            oAcuerdoSave.FechaHasta = oContratoAcuerdo.FechaHasta;
            oAcuerdoSave.ProveedorId = oContratoAcuerdo.ProveedorId;
            oAcuerdoSave.MonedaId = oContratoAcuerdo.MonedaId;
            oAcuerdoSave.GrupoCompra = oContratoAcuerdo.GrupoCompra;
            oAcuerdoSave.ComercialId = oContratoAcuerdo.ComercialId;
            oAcuerdoSave.LocalidadId = oContratoAcuerdo.LocalidadId;
            oAcuerdoSave.ProvinciaId = oContratoAcuerdo.ProvinciaId;
            oAcuerdoSave.Base = oContratoAcuerdo.Base;
            oAcuerdoSave.ImporteSustentable = oContratoAcuerdo.ImporteSustentable;
            oAcuerdoSave.MonedaSustentableId = oContratoAcuerdo.MonedaSustentableId;
            oAcuerdoSave.FechaDesdeSustentable = oContratoAcuerdo.FechaDesdeSustentable;
            oAcuerdoSave.FechaHastaSustentable = oContratoAcuerdo.FechaHastaSustentable;
            oAcuerdoSave.TarifaAConvenir = oContratoAcuerdo.TarifaAConvenir;
            oAcuerdoSave.FechaDolarizado = oContratoAcuerdo.FechaDolarizado;
            oAcuerdoSave.DiasPesificado = oContratoAcuerdo.DiasPesificado;
            oAcuerdoSave.NoInformaSio = oContratoAcuerdo.NoInformaSio;
            oAcuerdoSave.TrigoEspecial = oContratoAcuerdo.TrigoEspecial;
            oAcuerdoSave.EstadoId = estado == 1 ? 7 : 2;
            oAcuerdoSave.Ampliaciones = oContratoAcuerdo.Ampliaciones;
            oAcuerdoSave.Observacion = oContratoAcuerdo.Observacion;
            oAcuerdoSave.DestinoId = oContratoAcuerdo.DestinoId;
            oAcuerdoSave.Consignatario = oContratoAcuerdo.Consignatario;
            oAcuerdoSave.CondicionFijacionId = oContratoAcuerdo.CondicionFijacionId;
            oAcuerdoSave.CD = oContratoAcuerdo.CD;
            oAcuerdoSave.Warrant = oContratoAcuerdo.Warrant;
            oAcuerdoSave.PagoDirectoVendedor = oContratoAcuerdo.PagoDirectoVendedor;
            oAcuerdoSave.EstablecimientoPropio = oContratoAcuerdo.EstablecimientoPropio;
            oAcuerdoSave.ClasificacionId = oContratoAcuerdo.ClasificacionId;
            oAcuerdoSave.CantidadCamiones = oContratoAcuerdo.CantidadCamiones;
            oAcuerdoSave.BoletoId = oContratoAcuerdo.BoletoId;
            oAcuerdoSave.BolsaId = oContratoAcuerdo.BolsaId == 0 ? null : oContratoAcuerdo.BolsaId;
            oAcuerdoSave.DesdeFijacion = oContratoAcuerdo.DesdeFijacion;
            oAcuerdoSave.HastaFijacion = oContratoAcuerdo.HastaFijacion;
            oAcuerdoSave.MercsDeposito = oContratoAcuerdo.MercsDeposito;
            oAcuerdoSave.CantidadDeposito = oContratoAcuerdo.CantidadDeposito;
            oAcuerdoSave.CorredorId = oContratoAcuerdo.CorredorId;
            oAcuerdoSave.PorcentajeComision = oContratoAcuerdo.PorcentajeComision;
            oAcuerdoSave.ContratoVendedor = oContratoAcuerdo.ContratoVendedor;
            oAcuerdoSave.ContratoCorredor = oContratoAcuerdo.ContratoCorredor;
            oAcuerdoSave.SelCargoMOA = oContratoAcuerdo.SelCargoMOA;
            oAcuerdoSave.SelCargoVendedor = oContratoAcuerdo.SelCargoVendedor;
            oAcuerdoSave.Madre = oContratoAcuerdo.Madre;
            oAcuerdoSave.ContratoMadre = oContratoAcuerdo.ContratoMadre;
            oAcuerdoSave.EsFason = oContratoAcuerdo.EsFason;
            oAcuerdoSave.PrecioNeto = oContratoAcuerdo.PrecioNeto;
            oAcuerdoSave.StandardDeCalidadId = oContratoAcuerdo.StandardDeCalidadId;
            oAcuerdoSave.Pizarra = oContratoAcuerdo.Pizarra;
            oAcuerdoSave.Pago = oContratoAcuerdo.Pago;
            oAcuerdoSave.PagoDiferido = oContratoAcuerdo.PagoDiferido;
            oAcuerdoSave.PagoCBU = oContratoAcuerdo.PagoCBU;
            oAcuerdoSave.Dolarizado = oContratoAcuerdo.Dolarizado;
            oAcuerdoSave.DolarizadoCorredor = oContratoAcuerdo.DolarizadoCorredor;
            oAcuerdoSave.DolarizadoExpress = oContratoAcuerdo.DolarizadoExpress;
            oAcuerdoSave.ChequeElectronico = oContratoAcuerdo.ChequeElectronico;
            oAcuerdoSave.UsuarioId = oContratoAcuerdo.UsuarioId;
            oAcuerdoSave.CampanaId = oContratoAcuerdo.CampanaId;
            oAcuerdoSave.FechaCierta = oContratoAcuerdo.FechaCierta;
            oAcuerdoSave.Sustentable = oContratoAcuerdo.Sustentable;
            oAcuerdoSave.EPA = oContratoAcuerdo.EPA;
            oAcuerdoSave.SustentableTipoDBId = oContratoAcuerdo.EPA == true || oContratoAcuerdo.Sustentable == true ? oContratoAcuerdo.SustentableTipoDBId : null;
            oAcuerdoSave.TipoAgenteCompraId = oContratoAcuerdo.TipoAgenteCompraId;
            oAcuerdoSave.CaratulaExtension = oContratoAcuerdo.CaratulaExtension;
            oAcuerdoSave.CaratulaMAT = oContratoAcuerdo.CaratulaMAT;
            oAcuerdoSave.PrecioAjusteComision = oContratoAcuerdo.PrecioAjusteComision;
            oAcuerdoSave.MonedaAjusteComisionId = oContratoAcuerdo.MonedaAjusteComisionId;
            oAcuerdoSave.MotivoOperacionAnterior = oContratoAcuerdo.MotivoOperacionAnterior;
            oAcuerdoSave.DescripcionOperacionAnterior = oContratoAcuerdo.DescripcionOperacionAnterior;
            oAcuerdoSave.Canje = oContratoAcuerdo.Canje;
            oAcuerdoSave.PlanCanje = oContratoAcuerdo.PlanCanje;
            oAcuerdoSave.MonedaCanjeId = oContratoAcuerdo.MonedaCanjeId;
            oAcuerdoSave.Monto = oContratoAcuerdo.Monto;
            oAcuerdoSave.Insumo = oContratoAcuerdo.Insumo;
            oAcuerdoSave.PrestamoDevolucion = oContratoAcuerdo.PrestamoDevolucion;
            oAcuerdoSave.PlantaDestinoId = oContratoAcuerdo.PlantaDestinoId;
            oAcuerdoSave.PosicionCBOT = oContratoAcuerdo.PosicionCBOT;
            oAcuerdoSave.TipoPosicionCBOTId = oContratoAcuerdo.TipoPosicionCBOTId;
            oAcuerdoSave.PorcentajeDePago = oContratoAcuerdo.PorcentajeDePago;
            oAcuerdoSave.PorcentajeComision = oContratoAcuerdo.PorcentajeComision;
            oAcuerdoSave.PorcentajeComisionVenta = oContratoAcuerdo.PorcentajeComisionVenta;
            oAcuerdoSave.ProcedenciaVentaId = oContratoAcuerdo.ProcedenciaVentaId;
            oAcuerdoSave.CamaraId = oContratoAcuerdo.CamaraId;
            oAcuerdoSave.ComisionAFavorId = oContratoAcuerdo.ComisionAFavorId;
            oAcuerdoSave.FleteACargo = oContratoAcuerdo.FleteACargo;
            oAcuerdoSave.KgBalanza = oContratoAcuerdo.KgBalanza;
            oAcuerdoSave.CondicionDePagoDiaPesificado = oContratoAcuerdo.CondicionDePagoDiaPesificado;
            oAcuerdoSave.CondicionDePagoTipoPesificado = oContratoAcuerdo.CondicionDePagoTipoPesificado;
            oAcuerdoSave.CondicionDePagoPesificadoVentaId = oContratoAcuerdo.CondicionDePagoPesificadoVentaId;
            oAcuerdoSave.CondicionDePagoDiaFijacion = oContratoAcuerdo.CondicionDePagoDiaFijacion;
            oAcuerdoSave.CondicionDePagoTipoFijacion = oContratoAcuerdo.CondicionDePagoTipoFijacion;
            oAcuerdoSave.CondicionDePagoFijacionVentaId = oContratoAcuerdo.CondicionDePagoFijacionVentaId;
            oAcuerdoSave.CreditoDisponible = oContratoAcuerdo.CreditoDisponible;
            oAcuerdoSave.BoletoVentaId = oContratoAcuerdo.BoletoVentaId;
            oAcuerdoSave.MailVentaBoleto = oContratoAcuerdo.MailVentaBoleto;
            oAcuerdoSave.KgMinimo = oContratoAcuerdo.KgMinimo;
            oAcuerdoSave.KgMaximo = oContratoAcuerdo.KgMaximo;
            oAcuerdoSave.ProveedorComisionistaId = oContratoAcuerdo.ProveedorComisionistaId;
            oAcuerdoSave.ConDescarga = oContratoAcuerdo.ConDescarga;

            if (oAcuerdoSave.ContratoSAP != null)
            {
                oAcuerdoSave.ContratoSAP = oContratoAcuerdo.ContratoSAP;
            }
            if (ConfirmacionAutomatica(oContratoAcuerdo))
            {
                oContratoAcuerdo.EstadoId = (int)EnumEstadoContrato.Confirmado;
                logger.Debug("El contrato " + oContratoAcuerdo.Id + " se finalizó automaticamente por estar dentro de los rangos configurados.\n\n");
            }
            if (oAcuerdoSave.Id == 0)
            {
                repositorio.Agregar(oAcuerdoSave);
            }
            #region CREAR CUPOS CON DESCARGA
            if (listCupoConDescargaFechas != null)
            {
                cupoNuevo.NegocioId = oAcuerdoSave.Id;
                CupoResult cupoGrabado;

                listCupoConDescargaFechas.ForEach(x =>
                {
                    if (x.CantidadCupo > 0)
                    {
                        cupoNuevo.FleteProcedencia = false;
                        List<DiaCupo> listDiaCupo = new List<DiaCupo>() { new DiaCupo { Fecha = x.Fecha, Cantidad = x.CantidadCupo } };

                        cupoGrabado = cupoManager.GrabarCupo(cupoNuevo, listDiaCupo, false);

                        oEntityErrors.Errores.AddRange(cupoGrabado.Errores);
                        oEntityErrors.ListaCupos.AddRange(cupoGrabado.ListaCupos);
                        oEntityErrors.ListaErrores.AddRange(cupoGrabado.ListaErrores);
                    }
                    if (x.CantidadFlete > 0)
                    {
                        cupoNuevo.FleteProcedencia = true;
                        List<DiaCupo> listDiaFlete = new List<DiaCupo> { new DiaCupo { Fecha = x.Fecha, Cantidad = x.CantidadFlete } };

                        cupoGrabado = cupoManager.GrabarCupo(cupoNuevo, listDiaFlete, false);

                        CupoResult crTemp = new CupoResult();
                        crTemp.ListaCupos.AddRange(cupoGrabado.ListaCupos.Select(a => "*" + a + "*"));
                        cupoGrabado.ListaCupos = new List<string>();
                        cupoGrabado.ListaCupos.AddRange(crTemp.ListaCupos);

                        oEntityErrors.Errores.AddRange(cupoGrabado.Errores);
                        oEntityErrors.ListaCupos.AddRange(cupoGrabado.ListaCupos);
                        oEntityErrors.ListaErrores.AddRange(cupoGrabado.ListaErrores);
                    }
                });
            }
            #endregion
            try
            {
                var tipoDeAccion = (oAcuerdoSave.Id == 0 || string.IsNullOrEmpty(oAcuerdoSave.ContratoSAP)) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(oAcuerdoSave.Id), tipoDeAccion, oAcuerdoSave.GetType());

                logger.Debug("Contrato Acuerdo Guardado con ID " + oAcuerdoSave.Id);
            }
            catch (Exception ex)
            {
                logger.Error("Error en GrabarAcuerdo: ", ex);
                throw;
            }

            return oEntityErrors;
        }

        private bool ConfirmacionAutomatica(ContratoAcuerdo contrato)
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
            x.TipoNegocioId == 2 && x.FechaDesde <= hoy && x.FechaHasta >= hoy &&
            x.MaterialId == contrato.MaterialId && x.MonedaId == contrato.MonedaId &&
            precioContrato >= x.PrecioMinimo && precioContrato <= x.PrecioMaximo
            && tipoRangos.Contains(x.TipoRangoId)) ?? new List<RangoConfirmacionAutomatica>();

            var rango = rangos.FirstOrDefault(x => contrato.FechaDesde >= x.DesdeEntrega && contrato.FechaHasta <= x.HastaEntrega);

            if (rango != null && contrato.Precio > 0)
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                var cantidad = repositorio.Listar<Contrato, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 2 && x.MaterialId == rango.MaterialId && x.ContratoAcuerdoId == null);

                cantidad.AddRange(repositorio.Listar<ContratoAcuerdo, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 6 && x.MaterialId == rango.MaterialId && x.Precio > 0));

                var total = cantidad.Sum();
                var valor = (total + contrato.Cantidad) <= rango.Cantidad && (rango.ZonaId == 47 || rango.ZonaId == null || grupo == rango.ZonaId);
                return valor;
            }
            else
            {
                return false;
            }
        }

        private void Validar(ContratoAcuerdo oContratoAcuerdo, GrabarAcuerdoResult oEntityErrors)
        {
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oContratoAcuerdo.ProveedorId);
            if (proveedor != null && proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                oEntityErrors.Error("", "El Proveedor se encuentra deshabilitado");
            }
            if (oContratoAcuerdo.CampanaId == null || oContratoAcuerdo.CampanaId == 0)
            {
                oEntityErrors.Error("", "El campo 'Campaña' no debe estar vacío");
            }
            if (oContratoAcuerdo.Precio < 0)
            {
                oEntityErrors.Error("", "El precio debe ser mayor o igual a 0");
            }
            if (oContratoAcuerdo.Cantidad <= 0)
            {
                oEntityErrors.Error("", "La Cantidad debe ser mayor o igual a 0");
            }
            if (oContratoAcuerdo.ComercialCreadorId == 0)
            {
                oEntityErrors.Error("", "El campo Comercial es obligatorio");
            }
            if (oContratoAcuerdo.DestinoId == 0)
            {
                oEntityErrors.Error("", "El campo Destino es obligatorio");
            }
            if (oContratoAcuerdo.MaterialId == 0)
            {
                oEntityErrors.Error("", "El campo Material es obligatorio");
            }
            if (oContratoAcuerdo.MonedaId == null)
            {
                oEntityErrors.Error("", "El campo Moneda es obligatorio");
            }
            if (oContratoAcuerdo.FechaHasta == new DateTime())
            {
                oEntityErrors.Error("", "La fecha es obligatoria");
            }
            if (oContratoAcuerdo.Dolarizado.HasValue && oContratoAcuerdo.Dolarizado.Value && !oContratoAcuerdo.FechaDolarizado.HasValue)
            {
                oEntityErrors.Error("Dolarizado", "Se debe completar la Fecha de pesificación en negocios dolarizados");
            }
            if (oContratoAcuerdo.Dolarizado != true && oContratoAcuerdo.FechaDolarizado != null)
            {
                oEntityErrors.Error("Dolarizado", "Se debe completar Dolarizado si completó Fecha límite.");
            }
            if (oContratoAcuerdo.FechaCierta != null && oContratoAcuerdo.PagoDiferido == true)
            {
                oEntityErrors.Error("", "Los campos fecha cierta y pago diferido son excluyentes");
            }

            var conceptoFinanciero = oContratoAcuerdo.AperturaPrecio?.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
            if (oContratoAcuerdo.FechaCierta == null && oContratoAcuerdo.PagoDiferido != true && conceptoFinanciero != null)
            {
                oEntityErrors.Error("", "El campo Concepto Financiero es obligatorio con pago diferido");
            }
            if (oContratoAcuerdo.PagoDiferido.HasValue && oContratoAcuerdo.PagoDiferido.Value)
            {
                if (!oContratoAcuerdo.DiasPesificado.HasValue || oContratoAcuerdo.DiasPesificado == 0)
                {
                    oEntityErrors.Error("", "Días de diferimiento es obligatorio con pago diferido");
                }
                if (conceptoFinanciero == null)
                {
                    oEntityErrors.Error("", "El campo Concepto Financiero es obligatorio con pago diferido");
                }
            }

            if (oContratoAcuerdo.Pizarra.HasValue && !oContratoAcuerdo.Pizarra.Value && oContratoAcuerdo.FechaCierta == null)
            {
                if (!((conceptoFinanciero != null && (oContratoAcuerdo.PagoDiferido.HasValue && oContratoAcuerdo.PagoDiferido.Value) && (oContratoAcuerdo.DiasPesificado.HasValue && oContratoAcuerdo.DiasPesificado.Value != 0)) ||
                    (conceptoFinanciero == null && (!oContratoAcuerdo.PagoDiferido.HasValue || (oContratoAcuerdo.PagoDiferido.HasValue && !oContratoAcuerdo.PagoDiferido.Value))
                    && (!oContratoAcuerdo.DiasPesificado.HasValue || (oContratoAcuerdo.DiasPesificado.HasValue && oContratoAcuerdo.DiasPesificado.Value == 0)))))
                {
                    oEntityErrors.Error("", "Días de diferimiento/costo financiero es obligatorio con el pago diferido en pesos");
                }
            }

            if (oContratoAcuerdo.Calidad != null)
            {
                var calidad = oContratoAcuerdo.Calidad.LastOrDefault(x => x.CalidadEspecialId == 1);
                if (calidad != null && calidad.PorcentajeHasta < 40)
                {
                    oEntityErrors.Error("", "Falta completar el rango de dañados");
                }
                calidad = oContratoAcuerdo.Calidad.LastOrDefault(x => x.CalidadEspecialId == 2);
                if (calidad != null && calidad.PorcentajeHasta < 100)
                {
                    oEntityErrors.Error("", "Falta completar el rango de Granos Verdes");
                }
            }
            var rangosPrecio = repositorio.Obtener<RangoPrecio>(x => x.MaterialId == oContratoAcuerdo.MaterialId && x.MonedaId == oContratoAcuerdo.MonedaId);

            if (oContratoAcuerdo.Precio > 0 && rangosPrecio != null && (oContratoAcuerdo.Precio < rangosPrecio.PrecioMinimo || oContratoAcuerdo.Precio > rangosPrecio.PrecioMaximo))
            {
                oEntityErrors.Error("Precio", "Precio fuera de Rango - Precio Mínimo: " + rangosPrecio.PrecioMinimo + " y Precio Máximo: " + rangosPrecio.PrecioMaximo + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
            }
            oContratoAcuerdo.Destino = repositorio.Obtener<Centro>(x => x.Id == oContratoAcuerdo.DestinoId);
            if (oContratoAcuerdo.Venta != true)
            {
                if (oContratoAcuerdo.Destino.ValidaRedespacho != false && oContratoAcuerdo.AperturaPrecio != null && !oContratoAcuerdo.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho
                && (x.Importe != 0 || x.Porcentaje != 0)))
                {
                    oEntityErrors.Error("Descuentos", "Se debe completar Redespacho en Acopios");
                }
                if (oContratoAcuerdo.Destino.ValidaRedespacho == false && oContratoAcuerdo.AperturaPrecio != null && oContratoAcuerdo.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
                {
                    oEntityErrors.Error("Descuentos", "Solo se debe completar Redespacho en Acopios.");
                }

                if (oContratoAcuerdo.AperturaPrecio != null && oContratoAcuerdo.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero))
                {
                    oEntityErrors.Error("Descuentos", "El costo financiero no puede ser negativo.");
                }
            }
            if (oContratoAcuerdo.Precio == 0 && !string.IsNullOrWhiteSpace(oContratoAcuerdo.PagoCBU))
            {
                oEntityErrors.Error("", "No se puede completar Pago CBU en un Acuerdo a Fijar");
            }
            if (oContratoAcuerdo.Precio == 0 && oContratoAcuerdo.ChequeElectronico == true)
            {
                oEntityErrors.Error("", "No se puede completar Echeq en un Acuerdo a Fijar");
            }
            if (oContratoAcuerdo.Descuentos != null)
            {
                if (oContratoAcuerdo.Descuentos.Any(a => a.FechaDesde != null && a.FechaHasta != null && a.FechaDesde > a.FechaHasta))
                {
                    oEntityErrors.Error("Descuentos", "La fecha desde de descuento o bonificación no puede ser mayor a la fecha hasta.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => (a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && (a.FechaDesde == null || a.FechaHasta == null)))
                {
                    oEntityErrors.Error("Descuentos", "La fecha desde y hasta de descuento o bonificación es obligatoria.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && oContratoAcuerdo.Precio > 0)
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Fijación o de Entrega en un acuerdo a precio.");
                }

                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 3 && (a.FechaDesde < oContratoAcuerdo.DesdeFijacion || a.FechaDesde > oContratoAcuerdo.HastaFijacion || a.FechaHasta < oContratoAcuerdo.DesdeFijacion || a.FechaHasta > oContratoAcuerdo.HastaFijacion)))
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Fijación fuera del rango de Fijación.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 2 && (a.FechaDesde < oContratoAcuerdo.FechaDesde || a.FechaDesde > oContratoAcuerdo.FechaHasta || a.FechaHasta < oContratoAcuerdo.FechaDesde || a.FechaHasta > oContratoAcuerdo.FechaHasta)))
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Entrega fuera del rango de Entrega.");
                }
            }

            if ((oContratoAcuerdo.Sustentable.HasValue && oContratoAcuerdo.Sustentable.Value) || (oContratoAcuerdo.EPA.HasValue && oContratoAcuerdo.EPA.Value))
            {
                if (oContratoAcuerdo.MaterialId != 3)
                {
                    oEntityErrors.Error("SustentableEPA", "Sustentable/EPA solo está habilitado para el material Soja.");
                }
                if (oContratoAcuerdo.MercsDeposito == true && oContratoAcuerdo.SustentableTipoDBId.HasValue && oContratoAcuerdo.SustentableTipoDBId.Value == 2)
                {
                    if (!oContratoAcuerdo.FechaDesdeSustentable.HasValue || oContratoAcuerdo.FechaDesdeSustentable.Value == null)
                    {
                        oEntityErrors.Error("SustentableEPA", "Debe indicar la fecha 'Desde' de sustentable/EPA.");
                    }
                    if (!oContratoAcuerdo.FechaHastaSustentable.HasValue || oContratoAcuerdo.FechaHastaSustentable.Value == null)
                    {
                        oEntityErrors.Error("SustentableEPA", "Debe indicar la fecha 'Hasta' de sustentable/EPA.");
                    }
                    if (oContratoAcuerdo.FechaDesdeSustentable.HasValue && oContratoAcuerdo.FechaHastaSustentable.HasValue
                        && oContratoAcuerdo.FechaHastaSustentable.Value < oContratoAcuerdo.FechaDesdeSustentable.Value)
                    {
                        oEntityErrors.Error("SustentableEPA", "Debe indicar un rango de fechas válido para sustentable/EPA.");
                    }
                }
                if (oContratoAcuerdo.EPA.GetValueOrDefault())
                {
                    if (oContratoAcuerdo.ImporteSustentable.HasValue && oContratoAcuerdo.ImporteSustentable.Value > 0)
                    {
                        if (string.IsNullOrEmpty(oContratoAcuerdo.MonedaSustentableId))
                        {
                            oEntityErrors.Error("EPA", "Debe indicar la moneda para EPA.");
                        }
                    }
                    else if (oContratoAcuerdo.TarifaAConvenir != true)
                    {
                        oEntityErrors.Error("EPA", "Debe indicar la tarifa para EPA o tildar 'Tarifa a Convenir'.");
                    }
                    if (!oContratoAcuerdo.SustentableTipoDBId.HasValue)
                    {
                        oEntityErrors.Error("EPA", "Debe indicar si el importe para EPA es sobre el precio o por fuera del precio.");
                    }
                }
                if (oContratoAcuerdo.Sustentable.GetValueOrDefault())
                {
                    if (oContratoAcuerdo.ImporteSustentable.HasValue && oContratoAcuerdo.ImporteSustentable.Value > 0)
                    {
                        if (string.IsNullOrEmpty(oContratoAcuerdo.MonedaSustentableId))
                        {
                            oEntityErrors.Error("Sustentable", "Debe indicar la moneda para Sustentable.");
                        }
                    }
                    else if (oContratoAcuerdo.TarifaAConvenir != true)
                    {
                        oEntityErrors.Error("Sustentable", "Debe indicar la tarifa sustentable o tildar 'Tarifa a Convenir'.");
                    }
                    if (!oContratoAcuerdo.SustentableTipoDBId.HasValue)
                    {
                        oEntityErrors.Error("Sustentable", "Debe indicar si el importe sustentable es sobre el precio o por fuera del precio.");
                    }
                }
            }
            var conf = configuracionManager.TraerConfiguraciones();
            if (conf != null)
            {
                var cantidadMaxima = conf.CantidadMaxima * 1000;
                if (cantidadMaxima < oContratoAcuerdo.Cantidad)
                {
                    oEntityErrors.Error("", "Cantidad del negocio excedida (" + cantidadMaxima.ToString("N0") + " kg)");
                }
            }
            if (oContratoAcuerdo.Venta != true)
            {
                oEntityErrors.Errores.AddRange(negocioManager.ValidarAltaTemprana(oContratoAcuerdo, proveedor).Errores);
                if (oContratoAcuerdo.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO)
                {
                    oEntityErrors.Errores.AddRange(negocioManager.ValidarSinBoleto(oContratoAcuerdo).Errores);
                }
            }
        }

        public DatosIniComboContratoAcuerdo TraerDatosCombo()
        {
            var datosCombo = new DatosIniComboContratoAcuerdo
            {
                Material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }),
                Destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion }),
                Comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }),
                Moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion })
            };

            return datosCombo;
        }

        public ResultIniContratoAcuerdo TraerTodoContratoAcuerdo()
        {
            var contratos = new ResultIniContratoAcuerdo
            {
                ContratoAcuerdo = repositorio.Listar<ContratoAcuerdo, ContratoAcuerdoIni>(x => new ContratoAcuerdoIni()
                {
                    Cantidad = x.Cantidad,
                    Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                    Destino = x.Destino.Descripcion,
                    Fecha = DbFunctions.TruncateTime(x.Fecha),
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    Id = x.Id,
                    Precio = x.Precio,
                    Material = x.Material.Descripcion,
                    Proveedor = x.Proveedor.RazonSocial,
                    Corredor = x.Corredor.RazonSocial,
                    EstadoId = x.EstadoId,
                    Estado = x.Estado.Descripcion,
                    MonedaId = x.MonedaId,
                    Moneda = x.Moneda.Descripcion
                })
            };

            contratos.ContratoAcuerdo.ForEach(x => x.PorcentajeCargado = (decimal)Math.Round((repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == x.Id).Sum(d => d.Cantidad) / x.Cantidad), 2) * 100);
            return contratos;
        }

        public ContratoAcuerdoDto ObtenerContratoAcuerdoParaAsociar(DateTime fecha, int destinoId, int materialId, int proveedorId)
        {
            var fechaSinTiempo = fecha.Date;
            return repositorio.Obtener<ContratoAcuerdo, ContratoAcuerdoDto>(x => DbFunctions.TruncateTime(x.Fecha) == fechaSinTiempo
                                && x.DestinoId == destinoId
                                && x.MaterialId == materialId
                                && (x.ProveedorId == proveedorId)
                                && x.EstadoId == (int)EnumEstadoContrato.Confirmado,
                                x => new ContratoAcuerdoDto()
                                {
                                    Cantidad = x.Cantidad,
                                    Comercial = x.Comercial.Nombres,
                                    Destino = x.Destino.Descripcion,
                                    FechaHasta = x.FechaHasta,
                                    Id = x.Id,
                                    Precio = x.Precio,
                                    Material = x.Material.Descripcion,
                                    Proveedor = x.Proveedor.RazonSocial,
                                    Corredor = x.Corredor.RazonSocial,
                                    EstadoId = x.EstadoId,
                                    Estado = x.Estado.Descripcion
                                });
        }

        public Resultado ConfirmarContratoAcuerdo(int id, int usuarioConfirmador)
        {
            var oEntityErrors = new Resultado();

            var contrato = repositorio.Obtener<ContratoAcuerdo>(id);
            if (contrato.EstadoId == (int)EnumEstadoContrato.Pendiente || contrato.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;

                logger.Debug("Confirmando el Acuerdo: " + id);
                try
                {
                    contrato.UsuarioConfirmadorId = usuarioConfirmador;
                    contrato.FechaConfirmacion = DateTime.Now;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("Confirmar", "El Contrato Acuerdo no se pudo confirmar.\n\n");
            }
            return oEntityErrors;
        }

        public GrabarAcuerdoResult FinalizarAcuerdo(int acuerdoId)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            var oAcuerdoSave = repositorio.Obtener<ContratoAcuerdo>(acuerdoId);

            if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Con_Error)
            {
                try
                {
                    oAcuerdoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Finalizado);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(oAcuerdoSave.Id), TipoAccionLogDataAgro.Crear, oAcuerdoSave.GetType());

                }
                catch (Exception ex)
                {
                    oAcuerdoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(oAcuerdoSave.Id), TipoAccionLogDataAgro.Crear, oAcuerdoSave.GetType());

                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "El acuerdo ya se encuentra finalizado.\n\n");
                }
                else if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El acuerdo ya se encuentra rechazado.\n\n");
                }
            }
            return oEntityErrors;
        }

        public void AnularAcuerdos()
        {
            DateTime? fecha = null;
            var dia = diasHabilesAgent.UltimoDiaHabil(fecha);
            var listaAcuerdo = repositorio.Listar<ContratoAcuerdo>(x => x.Fecha < dia && x.EstadoId == 2);

            foreach (var acuerdo in listaAcuerdo)
            {
                var cantidad = repositorio.Listar<Contrato, double>(d => d.Cantidad, d => d.ContratoAcuerdoId == acuerdo.Id && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum();

                acuerdo.Cantidad = (int)cantidad;
                acuerdo.EstadoId = 5;
                logDataAgroManager.LogCambiosDataAgro(negocioManager.TraerAcuerdo(acuerdo.Id), TipoAccionLogDataAgro.Eliminar, acuerdo.GetType());
            }

            repositorio.GuardarCambios();
        }

    }
}
