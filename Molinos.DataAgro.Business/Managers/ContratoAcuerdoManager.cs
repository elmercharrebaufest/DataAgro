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
        private readonly IValidarDocProcPagoAgent validarPagoAgente;
        private readonly IConfiguracionManager configuracionManager;

        public ContratoAcuerdoManager(ILogger logger, IRepositorio repositorio,
            IDiasHabilesAgent diasHabilesAgent, ILogDataAgroManager logDataAgroManager, IValidarDocProcPagoAgent validarPagoAgente, IConfiguracionManager configuracionManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.diasHabilesAgent = diasHabilesAgent;
            this.logDataAgroManager = logDataAgroManager;
            this.validarPagoAgente = validarPagoAgente;
            this.configuracionManager = configuracionManager;
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
                                oContratoSave.ObligatoriedadCostoFinanciero = contratoOriginal.ObligatoriedadCostoFinanciero;


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

        public GrabarAcuerdoResult GrabarAcuerdo(ContratoAcuerdo oContratoAcuerdo)
        {
            var oEntityErrors = new GrabarAcuerdoResult();
            ContratoAcuerdo objContratoAcuerdo = null;
            List<DescuentoBonificacion> descuentosExistentes = null;
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
            oContratoAcuerdo.CorredorId = (oContratoAcuerdo.CorredorId == -1) ? null : oContratoAcuerdo.CorredorId;
            oContratoAcuerdo.ProveedorId = (oContratoAcuerdo.ProveedorId == -1) ? null : oContratoAcuerdo.ProveedorId;

            var estado = PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados) ? 2 : 1;
            if (PermisosHelper.Is(PermisosDataAgro.NegociosConfirmados))
            {
                oContratoAcuerdo.FechaConfirmacion = DateTime.Now;
            }
            if (oContratoAcuerdo.Id == 0)
            {
                oContratoAcuerdo.Fecha = DateTime.Now;
                oContratoAcuerdo.EstadoId = estado;
                oContratoAcuerdo.FechaOperacion = DateTime.Now;
                repositorio.Agregar(oContratoAcuerdo);
                if (ConfirmacionAutomatica(oContratoAcuerdo))
                {
                    oContratoAcuerdo.FechaConfirmacion = DateTime.Now;
                    oContratoAcuerdo.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato " + oContratoAcuerdo.Id + " se finalizo automaticamente por estar dentro de los rangos configurados");

                }
            }
            else
            {

                objContratoAcuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoAcuerdo.Id);

                descuentosExistentes = objContratoAcuerdo.Descuentos.ToList();
                if (estado == 1 && objContratoAcuerdo.EstadoId == (int)EnumEstadoContrato.Confirmado)
                {
                    string jsonContrato = JsonConvert.SerializeObject(objContratoAcuerdo, new JsonSerializerSettings()
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver(),
                        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                        PreserveReferencesHandling = PreserveReferencesHandling.Objects
                    });
                    objContratoAcuerdo.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oContratoAcuerdo.Id, TipoNegocioId = oContratoAcuerdo.TipoNegocioId, ComercialId = oContratoAcuerdo.ComercialId });

                }
                objContratoAcuerdo.MaterialId = oContratoAcuerdo.MaterialId;
                objContratoAcuerdo.DestinoId = oContratoAcuerdo.DestinoId;
                objContratoAcuerdo.FechaDesde = oContratoAcuerdo.FechaDesde;
                objContratoAcuerdo.FechaHasta = oContratoAcuerdo.FechaHasta;
                objContratoAcuerdo.EstadoId = estado == 1 ? 7 : 2;
                objContratoAcuerdo.ComercialCreadorId = oContratoAcuerdo.ComercialCreadorId;
                objContratoAcuerdo.Precio = oContratoAcuerdo.Precio;
                objContratoAcuerdo.Cantidad = oContratoAcuerdo.Cantidad;
                objContratoAcuerdo.ProveedorId = oContratoAcuerdo.ProveedorId;
                objContratoAcuerdo.CorredorId = oContratoAcuerdo.CorredorId;
                objContratoAcuerdo.MonedaId = oContratoAcuerdo.MonedaId;
                objContratoAcuerdo.StandardDeCalidadId = oContratoAcuerdo.StandardDeCalidadId;
                objContratoAcuerdo.CD = oContratoAcuerdo.CD;
                objContratoAcuerdo.Warrant = oContratoAcuerdo.Warrant;
                objContratoAcuerdo.DiasPesificado = oContratoAcuerdo.DiasPesificado;
                objContratoAcuerdo.PagoDiferido = oContratoAcuerdo.PagoDiferido;
                objContratoAcuerdo.Dolarizado = oContratoAcuerdo.Dolarizado;
                objContratoAcuerdo.FechaDolarizado = oContratoAcuerdo.FechaDolarizado;
                objContratoAcuerdo.ChequeElectronico = oContratoAcuerdo.ChequeElectronico;
                objContratoAcuerdo.PagoCBU = oContratoAcuerdo.PagoCBU;

                objContratoAcuerdo.TipoNegocioId = oContratoAcuerdo.TipoNegocioId;
                objContratoAcuerdo.CampanaId = oContratoAcuerdo.CampanaId;
                objContratoAcuerdo.GrupoCompra = oContratoAcuerdo.GrupoCompra;
                objContratoAcuerdo.ComercialId = oContratoAcuerdo.ComercialId;
                objContratoAcuerdo.UsuarioId = oContratoAcuerdo.UsuarioId;
                objContratoAcuerdo.TrigoEspecial = oContratoAcuerdo.TrigoEspecial;
                objContratoAcuerdo.Ampliaciones = oContratoAcuerdo.Ampliaciones;
                objContratoAcuerdo.Observacion = oContratoAcuerdo.Observacion;
                objContratoAcuerdo.PrecioNeto = oContratoAcuerdo.PrecioNeto;
                objContratoAcuerdo.Pizarra = oContratoAcuerdo.Pizarra;
                objContratoAcuerdo.DesdeFijacion = oContratoAcuerdo.DesdeFijacion;
                objContratoAcuerdo.HastaFijacion = oContratoAcuerdo.HastaFijacion;
                objContratoAcuerdo.CondicionFijacionId = oContratoAcuerdo.CondicionFijacionId;
                objContratoAcuerdo.CampanaId = oContratoAcuerdo.CampanaId;
                objContratoAcuerdo.FechaCierta = oContratoAcuerdo.FechaCierta;
                objContratoAcuerdo.ObligatoriedadCostoFinanciero = oContratoAcuerdo.FechaCierta.HasValue ? oContratoAcuerdo.ObligatoriedadCostoFinanciero : null;


                if (descuentosExistentes != null)
                {
                    foreach (var descExistente in descuentosExistentes)
                    {
                        if (objContratoAcuerdo.Descuentos == null || !objContratoAcuerdo.Descuentos.Any(x => x.Id == descExistente.Id))
                        {
                            repositorio.Remover(descExistente);
                        }
                    }
                }
                if (oContratoAcuerdo.Descuentos != null)
                {
                    foreach (var descuento in oContratoAcuerdo.Descuentos.Where(x => x.Id == 0))
                    {
                        descuento.Negocio = objContratoAcuerdo;
                        repositorio.Agregar(descuento);
                    }
                }

                if (objContratoAcuerdo.Calidad != null)
                {
                    foreach (var cal in objContratoAcuerdo.Calidad.ToList())
                    {
                        repositorio.Remover(cal);
                    }
                }
                else
                {
                    objContratoAcuerdo.Calidad = new List<Calidad>();
                }
                if (oContratoAcuerdo.Calidad != null)
                {
                    foreach (var cal in oContratoAcuerdo.Calidad)
                    {
                        objContratoAcuerdo.Calidad.Add(new Calidad
                        {
                            CalidadEspecialId = cal.CalidadEspecialId,
                            PorcentajeDesde = cal.PorcentajeDesde,
                            PorcentajeHasta = cal.PorcentajeHasta,
                            StandardDeCalidadId = cal.StandardDeCalidadId,
                            Valor = cal.Valor
                        });
                    }
                }
                if (objContratoAcuerdo.AperturaPrecio != null)
                {
                    foreach (var ap in objContratoAcuerdo.AperturaPrecio.ToList())
                    {
                        repositorio.Remover(ap);
                    }
                }
                else
                {
                    objContratoAcuerdo.AperturaPrecio = new List<AperturaPrecio>();
                }
                if (oContratoAcuerdo.AperturaPrecio != null)
                {
                    foreach (var y in oContratoAcuerdo.AperturaPrecio)
                    {
                        objContratoAcuerdo.AperturaPrecio.Add(new AperturaPrecio
                        {
                            NegocioId = y.NegocioId,
                            Id = y.Id,
                            ConceptoAperturaPrecioId = y.ConceptoAperturaPrecioId,
                            Importe = y.Importe,
                            MonedaId = y.MonedaId,
                            Porcentaje = y.Porcentaje
                        });
                    }
                }

                if (objContratoAcuerdo.PrecioPactado != null)
                {
                    foreach (var cal in objContratoAcuerdo.PrecioPactado.ToList())
                    {
                        repositorio.Remover(cal);
                    }
                }
                else
                {
                    objContratoAcuerdo.PrecioPactado = new List<PrecioPactado>();
                }
                if (oContratoAcuerdo.PrecioPactado != null)
                {
                    foreach (var cal in oContratoAcuerdo.PrecioPactado)
                    {
                        objContratoAcuerdo.PrecioPactado.Add(new PrecioPactado
                        {
                            Id = cal.Id,
                            FechaDesde = cal.FechaDesde,
                            FechaHasta = cal.FechaHasta,
                            Precio = cal.Precio,
                            MonedaPactadoId = cal.MonedaPactadoId,
                            ImportePactado = cal.ImportePactado,
                            MonedaImportePactadoId = cal.MonedaImportePactadoId,
                            Porcentaje = cal.Porcentaje,
                            ContratoId = cal.ContratoId
                        });
                    }
                }

                if (ConfirmacionAutomatica(oContratoAcuerdo))
                {
                    oContratoAcuerdo.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato " + oContratoAcuerdo.Id + " se finalizo automaticamente por estar dentro de los rangos configurados");

                }
            }
            try
            {
                repositorio.GuardarCambios();
                var contratoLog = objContratoAcuerdo ?? oContratoAcuerdo;
                var tipoDeAccion = (objContratoAcuerdo == null) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar;
                logDataAgroManager.LogCambiosDataAgro(TraerAcuerdo(contratoLog.Id), tipoDeAccion, contratoLog.GetType());

            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }

            logger.Debug("Guardando el Contrato Acuerdo");

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
            x.TipoNegocioId == 2 &&
            x.FechaDesde <= hoy &&
            x.FechaHasta >= hoy &&
            x.MaterialId == contrato.MaterialId &&
            x.MonedaId == contrato.MonedaId &&
            precioContrato >= x.PrecioMinimo && precioContrato <= x.PrecioMaximo
            && tipoRangos.Contains(x.TipoRangoId)) ?? new List<RangoConfirmacionAutomatica>();

            var rango = rangos.FirstOrDefault(
                x => contrato.FechaDesde >= new DateTime(x.DesdeAnio, x.DesdeMes, 1) &&
                   contrato.FechaHasta <= new DateTime(x.HastaAnio, x.HastaMes, DateTime.DaysInMonth(x.HastaAnio, x.HastaMes)));


            if (rango != null && contrato.Precio > 0)
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                var cantidad =
                    repositorio.Listar<Contrato, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 2
                 && x.MaterialId == rango.MaterialId && x.ContratoAcuerdoId == null);
                cantidad.AddRange(repositorio.Listar<ContratoAcuerdo, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 6
                 && x.MaterialId == rango.MaterialId && x.Precio > 0));
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

        private void Validar(ContratoAcuerdo oContratoAcuerdo, GrabarAcuerdoResult oEntityErrors)
        {
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oContratoAcuerdo.ProveedorId);
            if (proveedor != null && proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                oEntityErrors.Error("", "El Proveedor se encuentra deshabilitado");
            }
            if (oContratoAcuerdo.CampanaId == null || oContratoAcuerdo.CampanaId == 0)
            {
                oEntityErrors.Error("", "El campo 'Campaña' no debe estar vacio");
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
                oEntityErrors.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }
            if (oContratoAcuerdo.Dolarizado != true && oContratoAcuerdo.FechaDolarizado != null)
            {
                oEntityErrors.Error("Dolarizado", "Se debe completar Dolarizado si completó Fecha límite .");
            }
            if (oContratoAcuerdo.FechaCierta != null && oContratoAcuerdo.PagoDiferido == true)
            {
                oEntityErrors.Error("", "Fecha cierta o días de diferimiento son campos obligatorios con el concepto Financiero");
            }
            if (oContratoAcuerdo.PagoDiferido.HasValue && oContratoAcuerdo.PagoDiferido.Value)
            {
                if (!oContratoAcuerdo.DiasPesificado.HasValue || oContratoAcuerdo.DiasPesificado == 0)
                {
                    oEntityErrors.Error("", "Días de diferimiento es obligatorio con el Pago Diferido");
                }
                if (oContratoAcuerdo.AperturaPrecio != null)
                {
                    var concepto = oContratoAcuerdo.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
                    if (concepto == null)
                    {
                        oEntityErrors.Error("", "Concepto Financiero es obligatorio con el Pago Diferido");
                    }
                }
                else
                {
                    oEntityErrors.Error("", "Concepto Financiero es obligatorio con el Pago Diferido");
                }

            }

            if (oContratoAcuerdo.AperturaPrecio != null && oContratoAcuerdo.FechaCierta != null && oContratoAcuerdo.ObligatoriedadCostoFinanciero != false)
            {
                if ((oContratoAcuerdo.Pizarra.HasValue && !oContratoAcuerdo.Pizarra.Value))
                {
                    var concepto = oContratoAcuerdo.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
                    if (!((concepto != null && (oContratoAcuerdo.FechaCierta != null) ||
                        (concepto == null && (oContratoAcuerdo.FechaCierta == null)))))
                    {
                        oEntityErrors.Error("", "Fecha Cierta es obligatorio con el concepto Financiero");
                    }
                }
            }
                //else
                //{
                //    if ((oContratoAcuerdo.Pizarra.HasValue && !oContratoAcuerdo.Pizarra.Value))
                //    {
                //        var concepto = oContratoAcuerdo.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
                //        if (!((concepto != null && (oContratoAcuerdo.PagoDiferido.HasValue && oContratoAcuerdo.PagoDiferido.Value) && (oContratoAcuerdo.DiasPesificado.HasValue && oContratoAcuerdo.DiasPesificado.Value != 0)) ||
                //            (concepto == null && (!oContratoAcuerdo.PagoDiferido.HasValue || (oContratoAcuerdo.PagoDiferido.HasValue && !oContratoAcuerdo.PagoDiferido.Value)) && (!oContratoAcuerdo.DiasPesificado.HasValue || (oContratoAcuerdo.DiasPesificado.HasValue && oContratoAcuerdo.DiasPesificado.Value == 0)))))
                //        {
                //            oEntityErrors.Error("", "Días de diferimiento es obligatorio con el concepto Financiero");
                //        }
                //    }
                //}

            //if (oContratoAcuerdo.AperturaPrecio != null && oContratoAcuerdo.MonedaId == "  ARP")
            //{
            //    var concepto = oContratoAcuerdo.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
            //    if (concepto != null && oContratoAcuerdo.PagoDiferido != true)
            //    {
            //        oEntityErrors.Error("", "Pago Diferido es obligatorio con el concepto Financiero");
            //    }
            //}
            if (oContratoAcuerdo.Calidad != null)
            {
                var calidad = oContratoAcuerdo.Calidad.LastOrDefault(x => x.CalidadEspecialId == 1);
                if (calidad != null && calidad.PorcentajeHasta < 40)
                {
                    oEntityErrors.Error("", "Falta completar el rango de Dañados");
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
                oEntityErrors.Error("Precio", "Precio fuera de Rango, Precio Mínimo: " + rangosPrecio.PrecioMinimo + " Precio Máximo: " + rangosPrecio.PrecioMaximo + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
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
                    oEntityErrors.Error("Descuentos", "La fecha desde de descuento o bonificacion no puede ser mayor a la fecha hasta.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => (a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && (a.FechaDesde == null || a.FechaHasta == null)))
                {
                    oEntityErrors.Error("Descuentos", "La fecha desde y hasta de descuento o bonificacion es obligatoria.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && oContratoAcuerdo.Precio > 0)
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificacion por Fecha de Fijación o de Entrega en un acuerdo a precio.");
                }

                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 3 && (a.FechaDesde < oContratoAcuerdo.DesdeFijacion || a.FechaDesde > oContratoAcuerdo.HastaFijacion || a.FechaHasta < oContratoAcuerdo.DesdeFijacion || a.FechaHasta > oContratoAcuerdo.HastaFijacion)))
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificacion por Fecha de Fijación fuera del rango de Fijación.");
                }
                if (oContratoAcuerdo.Descuentos.Any(a => a.TipoPeriodoDBId == 2 && (a.FechaDesde < oContratoAcuerdo.FechaDesde || a.FechaDesde > oContratoAcuerdo.FechaHasta || a.FechaHasta < oContratoAcuerdo.FechaDesde || a.FechaHasta > oContratoAcuerdo.FechaHasta)))
                {
                    oEntityErrors.Error("Descuentos", "No se puede cargar descuento o bonificacion por Por Fecha de Entrega fuera del rango de Entrega.");
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
                Cantidad = x.Cantidad,
                CampanaId = x.CampanaId ?? 0,
                Campania = x.CampanaId == null ? "" : x.Campana.Descripcion,
                Precio = x.Precio,
                ComercialId = x.ComercialCreadorId,
                MaterialId = x.MaterialId,
                CD = x.CD,
                Warrant = x.Warrant,
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
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                Proveedor = (x.ProveedorId != null && x.ProveedorId > 0) ? x.Proveedor.RazonSocial + " (" + x.Proveedor.CUIT + ")" : "",
                ProveedorId = x.ProveedorId ?? 0,
                Corredor = (x.CorredorId != null && x.CorredorId > 0) ? x.Corredor.RazonSocial + " (" + x.Corredor.CUIT + ")" : "",
                CorredorId = x.CorredorId ?? 0,
                DestinoId = x.DestinoId,
                TipoNegocioId = 6,
                FechaHasta = x.FechaHasta,
                FechaDesde = x.FechaDesde,
                Estado = x.EstadoId,
                Estado_Contrato = x.Estado.Descripcion,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion,
                StandardCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                Dolarizado = x.Dolarizado,
                Fecha_DolarizadoFormateado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                Dias_Pesificado = x.DiasPesificado,
                PagoDiferido = x.PagoDiferido,
                Material = x.Material.Descripcion,
                Fecha = x.Fecha,
                DestinoDescripcion = x.Destino.Descripcion,
                TipoNegocio = x.TipoNegocio.Descripcion,
                CondicionFijacionDescripcion = x.CondicionFijacion.Descripcion,
                Comercial = x.Comercial.Apellido + " " + x.Comercial.Nombres,
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
                FechaCiertaFormateado = x.FechaCierta.HasValue ? SqlFunctions.DateName("day", x.FechaCierta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaCierta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaCierta) : "",
                ChequeElectronico = x.ChequeElectronico,
                PagoCBU = x.PagoCBU,
                ObligatoriedadCostoFinanciero = x.ObligatoriedadCostoFinanciero
            });
            return contrato;
        }

        public DatosIniAbmContratoAcuerdo TraerDatosIniciales()
        {
            throw new NotImplementedException();
        }
        public DatosIniComboContratoAcuerdo TraerDatosCombo()
        {
            var datosCombo = new DatosIniComboContratoAcuerdo
            {
                material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion }),
                destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion }),
                comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido }),
                moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion })
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
                oEntityErrors.Error("Confirmar", "El Contrato Acuerdo no se puede confirmar");
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
            Nullable<DateTime> fecha = null;
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
    }
}
