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
using System.Data.Entity.SqlServer;
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

        public ContratoAcuerdoManager(ILogger logger, IRepositorio repositorio, IDiasHabilesAgent diasHabilesAgent, ILogDataAgroManager logDataAgroManager,
            IConfiguracionManager configuracionManager, ICupoManager cupoManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.diasHabilesAgent = diasHabilesAgent;
            this.logDataAgroManager = logDataAgroManager;
            this.configuracionManager = configuracionManager;
            this.cupoManager = cupoManager;
        }

        public GrabarAcuerdoResult BorrarAcuerdo(ContratoAcuerdo oAcuerdo)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            if (string.IsNullOrEmpty(oAcuerdo.MotivoRechazo) || string.IsNullOrWhiteSpace(oAcuerdo.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
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
                        logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        oEntityErrors.Error("", "El Contrato Acuerdo no pudo rechazar");
                    }
                }
                else
                {
                    oEntityErrors.Error("", "El Contrato Acuerdo no se puede rechazar");
                }
            }
            else
            {
                oEntityErrors.Error("", "El Contrato Acuerdo ya se ha utilizado y no se puede rechazar");
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

            var listaServicioValor = repositorio.Listar<ServicioValor>();
            List<int> listaFiltradaServicioValor = new List<int>();
            if (listaServicioValor != null)
            {
                List<ServicioValor> listaFiltrada = listaServicioValor.Where(x => x.MaterialId == oContratoAcuerdo.MaterialId && x.CentroId == oContratoAcuerdo.DestinoId).ToList();

                listaFiltrada.ForEach(x => listaFiltradaServicioValor.Add(x.Id));
            }

            Cupo cupoNuevo = null;
            if (listCupoConDescargaFechas != null)
            {
                cupoNuevo = TransformarAcuerdoACupo(oContratoAcuerdo); // TransformarAEntidad

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
                    oEntityErrors.Errores.Add(new ErrorMessage(400, "La cantidad de cupos/fletes ingresados se exceden respecto a los KG del Negocio."));
                }
            }

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            ContratoAcuerdo oAcuerdoSave = new ContratoAcuerdo();
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

            if (oContratoAcuerdo.Id == 0) //es nuevo
            {
                oAcuerdoSave.Fecha = DateTime.Now;
                oAcuerdoSave.EstadoId = estado;
                oAcuerdoSave.FechaOperacion = DateTime.Now;
                oAcuerdoSave.ComercialCreadorId = oContratoAcuerdo.ComercialCreadorId;
            }
            else //se está modificando
            {
                oAcuerdoSave = repositorio.Obtener<ContratoAcuerdo>(oContratoAcuerdo.Id);

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
                logger.Debug("El contrato " + oContratoAcuerdo.Id + " se finalizó automaticamente por estar dentro de los rangos configurados");
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
                logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(oAcuerdoSave.Id), tipoDeAccion, oAcuerdoSave.GetType());

                logger.Debug("Contrato Acuerdo Guardado");
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
            var centro = repositorio.Obtener<Centro>(x => x.Id == oContratoAcuerdo.DestinoId);
            if (/*oContratoAcuerdo.DestinoId != 13 && oContratoAcuerdo.DestinoId != 1 && oContratoAcuerdo.DestinoId != 6 && oContratoAcuerdo.DestinoId != 7 &&*/
                oContratoAcuerdo.AperturaPrecio != null && !oContratoAcuerdo.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho
                && (x.Importe != 0 || x.Porcentaje != 0)) && centro.ValidaRedespacho != false)
            {
                oEntityErrors.Error("", "Se debe completar Redespacho en Acopios");
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
            var conf = configuracionManager.TraerConfiguraciones();
            if (conf != null)
            {
                var cantidadMaxima = conf.CantidadMaxima * 1000;
                if (cantidadMaxima < oContratoAcuerdo.Cantidad)
                {
                    oEntityErrors.Error("", "Cantidad del negocio excedida (" + cantidadMaxima.ToString("N0") + " kg)");
                }
            }
        }

        public BasicoContrato TraerAcuerdo(int id)
        {
            var contrato = repositorio.Obtener<ContratoAcuerdo, BasicoContrato>(x => x.Id == id, x => new BasicoContrato
            {
                Id = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                CorredorId = x.CorredorId ?? 0,
                Proveedor = (x.ProveedorId != null && x.ProveedorId > 0) ? x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")" : "",
                Corredor = (x.CorredorId != null && x.CorredorId > 0) ? x.Corredor.RazonSocial + " (" + x.Corredor.CUIT + ")" : "",
                ComercialId = x.ComercialId,
                Cantidad = x.Cantidad,
                TipoNegocioId = 6,
                MaterialId = x.MaterialId,
                CampanaId = x.CampanaId ?? 0,
                Campania = x.CampanaId == null ? "" : x.Campana.Descripcion,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion,
                ProvinciaId = x.ProvinciaId,
                Provincia = x.Provincia.Nombre,
                LocalidadId = x.LocalidadId,
                Localidad = x.Localidad.Nombre,
                ContratoSAP = x.ContratoSAP,
                Base = x.Base,
                Observacion = x.Observacion,
                Estado = x.EstadoId,
                Estado_Contrato = x.Estado.Descripcion,
                Sustentable = x.Sustentable,
                EPA = x.EPA,
                SustentableTipoDBId = x.SustentableTipoDBId,
                SustentableTipoDB = x.SustentableTipoDB != null ? x.SustentableTipoDB.Descripcion : "",
                Importe_Sustentable = x.ImporteSustentable,
                Moneda_Sustentable = x.MonedaSustentableId,
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                DesdeFijacionFormateado = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.DesdeFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                HastaFijacionFormateado = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.HastaFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.HastaFijacion) : "",
                Fecha_DolarizadoFormateado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                FechaCierta = x.FechaCierta,
                FechaCiertaFormateado = x.FechaCierta.HasValue ? SqlFunctions.DateName("day", x.FechaCierta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaCierta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaCierta) : "",
                FechaDesde_Sustentable = x.FechaDesdeSustentable,
                FechaDesde_SustentableFormateado = x.FechaDesdeSustentable != null ? SqlFunctions.DateName("day", x.FechaDesdeSustentable).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesdeSustentable.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesdeSustentable) : "",
                FechaHasta_Sustentable = x.FechaHastaSustentable,
                FechaHasta_SustentableFormateado = x.FechaHastaSustentable != null ? SqlFunctions.DateName("day", x.FechaHastaSustentable).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHastaSustentable.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHastaSustentable) : "",
                FechaOperacion = x.FechaOperacion,
                FechaOperacionFormateado = SqlFunctions.DateName("day", x.FechaOperacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaOperacion.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaOperacion),
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                CondicionFijacion = x.CondicionFijacionId,
                CondicionFijacionDescripcion = x.CondicionFijacion.Descripcion,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                Dolarizado = x.Dolarizado,
                DolarizadoCorredor = x.DolarizadoCorredor,
                Fecha_Dolarizado = x.FechaDolarizado,
                Dias_Pesificado = x.DiasPesificado,
                NoInformaSIO = x.NoInformaSio,
                TrigoEspecial = x.TrigoEspecial,
                ClasificacionId = x.ClasificacionId,
                DestinoId = x.DestinoId,
                PlanCanje = x.PlanCanje,
                Consignatario = x.Consignatario,
                CantidadCamiones = x.CantidadCamiones,
                BoletoId = x.BoletoId,
                BolsaId = x.BolsaId,
                CD = x.CD,
                Warrant = x.Warrant,
                PagoDirectoVendedor = x.PagoDirectoVendedor,
                EstablecimientoPropio = x.EstablecimientoPropio,
                MercsDeposito = x.MercsDeposito,
                CantidadDeposito = x.CantidadDeposito,
                PorcentajeComision = x.PorcentajeComision,
                ContratoCorredor = x.ContratoCorredor,
                ContratoVendedor = x.ContratoVendedor,
                SelCargoMOA = x.SelCargoMOA,
                SelCargoVendedor = x.SelCargoVendedor,
                Madre = x.Madre,
                EsFason = x.EsFason,
                ContratoMadre = x.ContratoMadre,
                Pizarra = x.Pizarra ?? false,
                StandardCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                PagoDiferido = x.PagoDiferido,
                PagoDiferidoTerceroId = x.PagoDiferidoTerceroId,
                ZonaId = x.ZonaId,
                ZonaDescripcion = x.Zona.Descripcion,
                Compensacion = x.Compensacion,
                NivelTarifaId = x.NivelTarifaId,
                TarifaFlete = x.TarifaFlete,
                ObligatoriedadCostoFinanciero = x.ObligatoriedadCostoFinanciero,
                ObligatoriedadBonificacion = x.ObligatoriedadBonificacion,
                Material = x.Material.Descripcion,
                Fecha = x.Fecha,
                DestinoDescripcion = x.Destino.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
                ChequeElectronico = x.ChequeElectronico,
                PagoCBU = x.PagoCBU,
                PosicionCBOT = x.PosicionCBOT,
                TipoPosicionCBOTId = x.TipoPosicionCBOTId,
                TipoPosicionCBOT = x.TipoPosicionCBOT.Descripcion,
                ProveedorCreador = x.ProveedorCreadorId,
                UsuarioId = x.UsuarioId,
                UsuarioTercero = x.UsuarioTercero,
                DolarizadoExpress = x.DolarizadoExpress,
                CalidadTercero = x.CalidadTercero,
                DolarizadoTercero = x.DolarizadoTercero,
                PagoDiferidoTercero = x.PagoDiferidoTercero,
                ObservacionTercero = x.ObservacionTercero,
                Canje = x.Canje,
                Monto = x.Monto,
                MonedaCanjeId = x.MonedaCanjeId,
                Insumo = x.Insumo,
                PrestamoDevolucion = x.PrestamoDevolucion ?? false,
                PlantaDestinoId = x.PlantaDestinoId ?? 0,
                PlantaDestinoDescripcion = !x.PlantaDestinoId.HasValue ? "" : x.Destino.Descripcion,
                SustentableTercero = x.SustentableTercero,
                Venta = x.Venta,
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                TipoAgenteCompra = x.TipoAgenteCompraId == null ? "" : x.TipoAgenteCompra.Descripcion,
                CaratulaMAT = x.CaratulaMAT,
                PrecioAjusteComision = x.PrecioAjusteComision,
                MonedaAjusteComisionId = x.MonedaAjusteComisionId,
                ProveedorComisionistaId = x.ProveedorComisionistaId,
                KgMinimo = x.KgMinimo ?? 0,
                KgMaximo = x.KgMaximo ?? 0,
                ProcedenciaVentaId = x.ProcedenciaVentaId,
                ProvinciaVentaId = x.ProcedenciaVenta.ProvinciaId,
                ProvinciaVenta = x.ProcedenciaVenta.Provincia.Nombre,
                LocalidadVenta = x.ProcedenciaVenta.Nombre,
                CamaraId = x.CamaraId,
                ComisionAFavorId = x.ComisionAFavorId,
                PorcentajeComisionVenta = x.PorcentajeComisionVenta,
                FleteACargo = x.FleteACargo,
                KgBalanza = x.KgBalanza,
                CondicionDePagoDiaPesificado = x.CondicionDePagoDiaPesificado,
                CondicionDePagoTipoPesificado = x.CondicionDePagoTipoPesificado,
                CondicionDePagoPesificadoVentaId = x.CondicionDePagoPesificadoVentaId,
                Pago = x.Pago,
                CondicionDePagoDiaFijacion = x.CondicionDePagoDiaFijacion,
                CondicionDePagoTipoFijacion = x.CondicionDePagoTipoFijacion,
                CondicionDePagoFijacionVentaId = x.CondicionDePagoFijacionVentaId,
                BoletoVentaId = x.BoletoVentaId,
                MailVentaBoleto = x.MailVentaBoleto,
                CreditoDisponible = x.CreditoDisponible,
                Cesion = x.Cesion,
                TarifaAConvenir = x.TarifaAConvenir,
                ConDescarga = x.ConDescarga,
                DolarExportador = x.DolarExportador,
                Descuentos = x.Descuentos.Select(y => new DescuentoBonificacionDto
                {
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? DbFunctions.Right("00" + SqlFunctions.DateName("day", y.FechaDesde).Trim(), 2) + "-" +
                                            DbFunctions.Right("00" + SqlFunctions.StringConvert((double)y.FechaDesde.Value.Month).TrimStart(), 2) + "-" +
                                           SqlFunctions.DateName("year", y.FechaDesde) : "",
                    FechaHasta = y.FechaHasta != null ? DbFunctions.Right("00" + SqlFunctions.DateName("day", y.FechaHasta).Trim(), 2) + "-" +
                                            DbFunctions.Right("00" + SqlFunctions.StringConvert((double)y.FechaHasta.Value.Month).TrimStart(), 2) + "-" +
                                           SqlFunctions.DateName("year", y.FechaHasta) : "",
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Moneda = y.MonedaId,
                    Id = y.Id,
                    Porcentaje = y.Porcentaje,
                    TipoDBDesc = y.TipoDB.Descripcion,
                    TipoDBId = y.TipoDBId,
                    TipoPeriodoDBDesc = y.TipoPeriodoDB.Descripcion,
                    TipoPeriodoDBId = y.TipoPeriodoDBId
                }).ToList(),
                Calidades = x.Calidad.Select(y => new CalidadDto
                {
                    Id = y.Id,
                    Valor = y.Valor,
                    CalidadEspecialId = y.CalidadEspecialId,
                    CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                    PorcentajeDesde = y.PorcentajeDesde,
                    PorcentajeHasta = y.PorcentajeHasta
                }).ToList(),
                AperturaPrecios = x.AperturaPrecio.Select(y => new AperturaPrecioDto
                {
                    contratoId = y.NegocioId,
                    Id = y.Id,
                    ConceptoAperturaPrecio = y.ConceptoAperturaPrecio.Descripcion,
                    ConceptoAperturaPrecioId = y.ConceptoAperturaPrecioId,
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Porcentaje = y.Porcentaje
                }).ToList(),
                PreciosPactados = x.PrecioPactado.Select(y => new PrecioPactadosDto
                {
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? SqlFunctions.DateName("day", y.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)y.FechaDesde.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", y.FechaDesde) : "",
                    FechaHasta = y.FechaHasta != null ? SqlFunctions.DateName("day", y.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)y.FechaHasta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", y.FechaHasta) : "",
                    Id = y.Id,
                    ImportePactado = y.ImportePactado,
                    MonedaImportePactadoDesc = y.MonedaImportePactado.Descripcion,
                    MonedaImportePactadoId = y.MonedaImportePactadoId,
                    MonedaPactadoDesc = y.MonedaPactado.Descripcion,
                    MonedaPactadoId = y.MonedaPactadoId,
                    Porcentaje = y.Porcentaje,
                    Precio = y.Precio
                }).ToList(),
                Servicios = x.Servicios.Select(y => new ServicioValorDto
                {
                    Id = y.Id,
                    ServicioValorId = y.ServicioValor.Id,
                    Descripcion = y.ServicioValor.TipoServicio.Descripcion,
                    CodigoSAP = y.ServicioValor.TipoServicio.CodigoSAP,
                    Importe = y.Importe,
                    MonedaDescripcion = y.Moneda.Descripcion,
                    MonedaId = y.Moneda.MonedaId,
                    Desde = y.Desde,
                    Hasta = y.Hasta,
                    TipoServicioId = y.ServicioValor.TipoServicio.Id,
                    Modificado = y.Modificado
                }).ToList()
            });
            return contrato;
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
                    logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    throw;
                }
            }
            else
            {
                oEntityErrors.Error("Confirmar", "El Contrato Acuerdo no se pudo confirmar");
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
                    logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(oAcuerdoSave.Id), TipoAccionLogDataAgro.Crear, oAcuerdoSave.GetType());

                }
                catch (Exception ex)
                {
                    oAcuerdoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Con_Error);
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(oAcuerdoSave.Id), TipoAccionLogDataAgro.Crear, oAcuerdoSave.GetType());

                    oEntityErrors.Error("", ex.Message);
                    logger.Error(ex);
                }
            }
            else
            {
                if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "El Acuerdo ya se encuentra Finalizado");
                }
                else if (oAcuerdoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El Acuerdo ya ha sido Rechazado");
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
                logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(acuerdo.Id), TipoAccionLogDataAgro.Eliminar, acuerdo.GetType());
            }

            repositorio.GuardarCambios();
        }

        private Cupo TransformarAcuerdoACupo(ContratoAcuerdo contrato)
        {
            var cuitProveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == contrato.ProveedorId, x => x.CUIT);
            var comercial = repositorio.Obtener<Comercial>(x => x.ComercialId == contrato.ComercialId);
            var comercialId = contrato.ComercialId;
            var grupoDeCompras = comercial.GrupoDeCompras.Descripcion;
            var zonaComercial = repositorio.Listar<ZonaCupo>(x => x.Descripcion == grupoDeCompras).First();

            var cupoNuevo = new Cupo
            {
                Id = 0,
                ProveedorId = contrato.CorredorId == null ? contrato.ProveedorId.Value : contrato.CorredorId.Value,
                MaterialId = contrato.MaterialId,
                FechaIngreso = contrato.FechaEntrega.Value,
                CentroId = contrato.DestinoId.Value,
                FleteProcedencia = contrato.FleteACargo == "true",
                Calidad = contrato.MaterialId == 3 ? contrato.StandardDeCalidadId == 4 ? "Camara" : "Fabrica" : "",
                Observaciones = contrato.Observacion,
                Fason = contrato.EsFason,
                Destinatario = "30715118773",
                ComercialId = comercialId,
                FechaGeneracion = DateTime.Now,
                NegocioId = contrato.Id,
                ZonaCupoId = zonaComercial.Id,
                ConDescarga = contrato.ConDescarga,
                Sustentable = contrato.Sustentable,
                EPA = contrato.EPA,
                ComercialCreadorId = contrato.ComercialCreadorId,
            };

            return cupoNuevo;
        }
    }
}
