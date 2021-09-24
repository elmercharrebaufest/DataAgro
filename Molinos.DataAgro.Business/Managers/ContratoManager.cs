using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
namespace Molinos.DataAgro.Business.Managers
{
    public class ContratoManager : IContratoManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;



        private readonly IMaterialManager mobjMaterialManager;
        private readonly ITipoNegocioManager mobjTipoNegocioManager;
        private readonly ICampañaManager mobjCampaniaManager;
        private readonly IProvinciaManager mobjProvinciaManager;
        private readonly ILocalidadManager mobjLocalidadManager;
        private readonly IProveedorManager mobjProveedorManager;
        private readonly IComercialManager mobjComercialManager;
        private readonly IPushNotificationManager mobjNotification;
        private readonly IDiferencialManager diferencialManager;
        private readonly IContratoAcuerdoManager contratoAcuerdoManager;
        private readonly IFinalizarContratoAgent oFinalizarContratoAgent;
        private readonly IDiasHabilesAgent oDiasHabilesAgent;
        private readonly IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent;
        private readonly IEliminarContratoAgent oEliminarContratoAgent;
        private readonly IConfiguracionManager configuracionManager;
        private readonly ICapacidadProductivaAgent capacidadProductiva;
        private readonly IAltaTempranaAgent altaTempranaAgent;
        private readonly IDiasHabilesAgent diasHabilesAgent;
        private readonly IModificarContratoAgent modificarContratoAgent;
        private readonly IMailManager mailManager;
        private readonly IStatusContratoAgent status;
        private readonly ILogDataAgroManager logDataAgroManager;
        private readonly IValidarDocProcPagoAgent validarPagoAgente;
        private readonly IListaCBUProveedorAgent cbuAgent;
        private readonly IModificarFijacionAgent modificarFijacionAgent;
        private readonly ICartasDePortePendienteAplicarAgent ccppAgent;
        private readonly IHttpContextManager httpContextManager;
        private readonly IValidacionCreditoAgent validarCreditoAgente;
        private readonly ITipoDeCambioAgent tipoCambioAgent;
        private readonly ICapacidadProductivaDisponibleAgent capacidadProductivaDisponibleAgent;
        private readonly INegocioManager negocioManager;

        public ContratoManager(ILogger logger, IRepositorio repositorio,
            IMaterialManager oMSMaterialManager, ITipoNegocioManager oMSTipoNegocioManager,
            ICampañaManager oMSCampaniaManager, IProvinciaManager oMSProvinciaManager,
            ILocalidadManager oMSLocalidadManager, IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager,
            IPushNotificationManager oMSNotification,
            IDiferencialManager diferencialManager,
            IContratoAcuerdoManager contratoAcuerdoManager,
            IFinalizarContratoAgent oFinalizarContratoAgent,
            IDiasHabilesAgent oDiasHabilesAgent,
            IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent,
            IEliminarContratoAgent oEliminarContratoAgent, IConfiguracionManager configuracionManager,
            ICapacidadProductivaAgent capacidadProductiva, IAltaTempranaAgent altaTempranaAgent,
            IDiasHabilesAgent diasHabilesAgent, IModificarContratoAgent modificarContratoAgent,
            IMailManager mailManager, IStatusContratoAgent status,
            ILogDataAgroManager logDataAgroManager,
            IValidarDocProcPagoAgent validarPagoAgente,
            IListaCBUProveedorAgent cbuAgent, IModificarFijacionAgent modificarFijacionAgent,
            ICartasDePortePendienteAplicarAgent ccppAgent,
            IHttpContextManager httpContextManager, IValidacionCreditoAgent validarCreditoAgente, ITipoDeCambioAgent tipoCambioAgent,
            ICapacidadProductivaDisponibleAgent capacidadProductivaDisponibleAgent,
            INegocioManager negocioManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            mobjMaterialManager = oMSMaterialManager;
            mobjCampaniaManager = oMSCampaniaManager;
            mobjProvinciaManager = oMSProvinciaManager;
            mobjLocalidadManager = oMSLocalidadManager;
            mobjProveedorManager = oMSProveedorManager;
            mobjComercialManager = oMSComercialManager;
            mobjTipoNegocioManager = oMSTipoNegocioManager;
            mobjNotification = oMSNotification;
            this.diferencialManager = diferencialManager;
            this.contratoAcuerdoManager = contratoAcuerdoManager;
            this.oFinalizarContratoAgent = oFinalizarContratoAgent;
            this.oDiasHabilesAgent = oDiasHabilesAgent;
            this.oRelacionCorredorProveedorAgent = oRelacionCorredorProveedorAgent;
            this.oEliminarContratoAgent = oEliminarContratoAgent;
            this.configuracionManager = configuracionManager;
            this.altaTempranaAgent = altaTempranaAgent;
            this.diasHabilesAgent = diasHabilesAgent;
            this.modificarContratoAgent = modificarContratoAgent;
            this.mailManager = mailManager;
            this.capacidadProductiva = capacidadProductiva;
            this.status = status;
            this.logDataAgroManager = logDataAgroManager;
            this.validarPagoAgente = validarPagoAgente;
            this.cbuAgent = cbuAgent;
            this.modificarFijacionAgent = modificarFijacionAgent;
            this.ccppAgent = ccppAgent;
            this.httpContextManager = httpContextManager;
            this.validarCreditoAgente = validarCreditoAgente;
            this.tipoCambioAgent = tipoCambioAgent;
            this.capacidadProductivaDisponibleAgent = capacidadProductivaDisponibleAgent;
            this.negocioManager = negocioManager;
        }

        public DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null)
        {
            var datosCombo = new DatosIniContrato();
            var hoy = DateTime.Now;

            datosCombo.prov = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry() { Provinciaid = x.ProvinciaId, Nombre = x.Nombre, Orden = x.Orden }, null, 0, "Orden");

            datosCombo.loc = new List<LocalidadQry>();

            if (!PermisosHelper.Is(PermisosDataAgro.IngresoExterno))
            {
                datosCombo.campaña = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion });
                datosCombo.material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
                datosCombo.comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido },
                   x => x.Deshabilitado != true && x.AsignarNegocios == true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ListaComercialCompraNet)), 0, "Comercial");
            }
            else
            {
                datosCombo.campaña = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion });

                var listaMaterial = repositorio.Listar<HabilitacionPizarra, MaterialQry>(
                    x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Material.Descripcion, CampaniaTableroId = x.Material.CampaniaTableroId.Value, CampañaIdActual = x.Material.CampañaId.Value },
                    x => x.DesdeVigencia <= hoy
                    && x.HastaVigencia >= hoy
                    && (tipoNegocioId == null || x.TipoNegocioId == tipoNegocioId.Value)
                    );

                listaMaterial.AddRange(repositorio.Listar<PrecioMoa, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Material.Descripcion, CampaniaTableroId = x.Material.CampaniaTableroId.Value, CampañaIdActual = x.Material.CampañaId.Value },
                    x => x.DesdeVigencia <= hoy && x.HastaVigencia >= hoy && (tipoNegocioId == null || x.TipoNegocioId == tipoNegocioId.Value)));

                datosCombo.material = listaMaterial.GroupBy(y => y.MaterialId).Select(y => y.FirstOrDefault()).ToList();

                datosCombo.comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido },
                    x => x.Deshabilitado != true && x.AsignarNegocios == true && x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ListaComercialCompraNet)), 0, "Comercial");
            }
            datosCombo.moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });
            datosCombo.monedaSustentable = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            var tiposDeNegocio = repositorio.Listar<TipoNegocio, TipoNegocioQry>(x => new TipoNegocioQry() { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion }).Where(a => a.Descripcion != "ESPACIO DINAMICO").ToList();
            datosCombo.tiponegocio = tiposDeNegocio;
            if (PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio = datosCombo.tiponegocio.Where(x => x.TipoNegocioId == 1 || x.TipoNegocioId == 3).ToList();
            }
            if (PermisosHelper.Is(PermisosDataAgro.ModificarNegocios) || PermisosHelper.Is(PermisosDataAgro.ModificarNegFinalizados))
            {
                datosCombo.tiponegocio = tiposDeNegocio;
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosFason) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 4));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 5));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosAcuerdos) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 6));
            }

            datosCombo.Clasificacion = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Bolsa = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            var destinos = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.Destino = destinos.Where(x => x.Id == 1).ToList();
            datosCombo.Destino.AddRange(destinos.Where(x => x.Id != 1).OrderBy(x => x.Descripcion).ToList());
            datosCombo.Condicion = repositorio.Listar<CondicionFijacion, CondicionFijacionQry>(x => new CondicionFijacionQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Standard = repositorio.Listar<StandardDeCalidad, StandardDeCalidadQry>(x => new StandardDeCalidadQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoDB = repositorio.Listar<TipoDB, TipoDBQry>(x => new TipoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoPeriodoDB = repositorio.Listar<TipoPeriodoDB, TipoPeriodoDBQry>(x => new TipoPeriodoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.NivelTarifa = repositorio.Listar<NivelTarifa, NivelTarifaQry>(x => new NivelTarifaQry() { Id = x.Id, Descripcion = x.Descripcion, CodigoSap = x.CodigoSap });

            datosCombo.MonedaDescuento = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });
            datosCombo.TipoFason = repositorio.Listar<TipoFason, TipoFasonQry>(x => new TipoFasonQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.TipoAgenteCompra = repositorio.Listar<TipoAgenteCompra, TipoAgenteCompraQry>(x => new TipoAgenteCompraQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.Operador = repositorio.Listar<Operador, OperadorQry>(x => new OperadorQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.Zona = repositorio.Listar<Zona, ZonaQry>(x => new ZonaQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.MotivoAnterior = repositorio.Listar<MotivoAnterior, MotivoAnteriorQry>(x => new MotivoAnteriorQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoPosicionCBOT = repositorio.Listar<TipoPosicionCBOT, TipoPosicionCBOTQry>(x => new TipoPosicionCBOTQry() { Id = x.Id, Descripcion = x.Descripcion });

            Array estadosValues = Enum.GetValues(typeof(EnumEstadoContrato));

            foreach (int estadoValue in estadosValues)
            {
                string estadoName = Enum.GetName(typeof(EnumEstadoContrato), estadoValue);

                EstadosContratos item = new EstadosContratos(estadoValue, estadoName);

                datosCombo.estadoContrato.Add(item);
            }
            datosCombo.Camara = repositorio.Listar<Camara, CamaraQry>(x => new CamaraQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.CondicionDePagoPesificadoVenta = repositorio.Listar<CondicionDePagoVenta, CondicionDePagoVentaQry>(
                x => new CondicionDePagoVentaQry { Id = x.Id, Descripcion = x.Descripcion, CondicionFijacion = x.CondicionFijacion, CondicionPesificado = x.CondicionPesificado }, x => x.CondicionPesificado == true);

            datosCombo.CondicionDePagoFijacionVenta = repositorio.Listar<CondicionDePagoVenta, CondicionDePagoVentaQry>(
                x => new CondicionDePagoVentaQry { Id = x.Id, Descripcion = x.Descripcion, CondicionFijacion = x.CondicionFijacion, CondicionPesificado = x.CondicionPesificado }, x => x.CondicionFijacion == true);

            datosCombo.ComisionAFavor = repositorio.Listar<ComisionAFavor, ComisionAFavorQry>(x => new ComisionAFavorQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.FleteACargo = new List<FleteACargoQry> { new FleteACargoQry { Descripcion = "Vendedor" }, new FleteACargoQry { Descripcion = "Comprador" } };
            datosCombo.KgBalanza = new List<KgBalanzaQry> { new KgBalanzaQry { Descripcion = "Origen" }, new KgBalanzaQry { Descripcion = "Destino" } };
            datosCombo.Pago = new List<PagoQry> { new PagoQry { Descripcion = "Contra entrega" }, new PagoQry { Descripcion = "Anticipado" } };
            datosCombo.CondicionPago = new List<CondicionPagoQry> { new CondicionPagoQry { Descripcion = "Corridos" }, new CondicionPagoQry { Descripcion = "Hábiles" } };
            return datosCombo;
        }

        private Resultado Validar(Contrato oParam, Resultado oErrorMessages, bool validacionesMinimas)
        {
            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacio");
                return oErrorMessages;
            }
            if (oParam.CampanaId == null || oParam.CampanaId == 0)
            {
                oErrorMessages.Error("campanaId", "El campo 'Campaña' no debe estar vacio");
            }
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
            if (proveedor == null)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' es obligatorio");
                return oErrorMessages;
            }
            if (proveedor.Deshabilitado.HasValue && proveedor.Deshabilitado.Value != false)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor deshabilitado");
                return oErrorMessages;
            }
            if (oParam.ProveedorId == -1)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' debe tener un proveedor existente");
                return oErrorMessages;
            }
            if (!string.IsNullOrEmpty(proveedor.RiesgoComercialSap))
            {
                if (proveedor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Riesgo Comercial Alto");
                }
            }

            Proveedor corredor = null;
            if (oParam.CorredorId != null && oParam.CorredorId > 0)
            {
                corredor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.CorredorId);
                if (!string.IsNullOrEmpty(corredor.RiesgoComercialSap))
                {
                    if (corredor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                    {
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por Riesgo Comercial Alto");
                    }
                }

                if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == oParam.CorredorId && x.EstadoId == 4))
                {
                    oErrorMessages.Error("Estado", "Corredor no Operable por Estado BAJA");
                }

                if (repositorio.Existe<FACACOP>(x => x.CUIT == corredor.CUIT))
                {
                    oErrorMessages.Error("CorredorId", "Corredor No Operable por ser Apócrifo");
                }
            }


            if (oParam.ClasificacionId == 0)
            {
                oErrorMessages.Error("ClasificacionId", "El campo 'Clasificación' no debe estar vacio");
            }
            var config = repositorio.Obtener<Configuracion>(1);
            if (oParam.MonedaSustentableId == "USDM " && oParam.ImporteSustentable > config.ImporteSustentable)
            {
                oErrorMessages.Error("Importe", "Se excede Tarifa Sustentable");
            }
            int[] otros = { 2, 3, 4, 8, 9, 10, 11, 12, 13 };

            if (!validacionesMinimas)
            {
                var sisa = new SISA();
                if (oParam.ClasificacionId == 1)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == 1 && x.SituacionCategoria == "AL");
                }
                else if (oParam.ClasificacionId == 2)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == 6 && x.SituacionCategoria == "AL");
                }
                else if (oParam.ClasificacionId == 3)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria != 1 && x.CodCategoria != 6 && x.SituacionCategoria == "AL");
                }
                if (sisa != null)
                {
                    if (sisa.EstadoCuit == 3 && proveedor.RiesgoComercialSap != "E")
                    {
                        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT 3");
                    }
                    else if (sisa.EstadoCuit == 0)
                    {
                        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT Inactivo");
                    }
                    if (sisa.SituacionCategoria != "AL")
                    {
                        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Situación Categoría BA");
                    }
                }
                else
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por CUIT o Categoria Inactivo");
                }

                if (corredor != null)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == corredor.CUIT && x.CodCategoria == 2 && x.SituacionCategoria == "AL");

                    if (sisa != null)
                    {
                        if (sisa.EstadoCuit == 3 && corredor.RiesgoComercialSap != "E")
                        {
                            oErrorMessages.Error("CorredorId", "Corredor No Operable por Estado de CUIT 3");
                        }
                        else if (sisa.EstadoCuit == 0)
                        {
                            oErrorMessages.Error("CorredorId", "Corredor No Operable por Estado de CUIT Inactivo");
                        }
                        if (sisa.SituacionCategoria != "AL")
                        {
                            oErrorMessages.Error("CorredorId", "Corredor No Operable por Situación Categoría BA");
                        }
                    }
                    else
                    {
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por CUIT o Categoria Inactivo");
                    }
                }
            }

            if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == oParam.ProveedorId && x.EstadoId == 4))
            {
                oErrorMessages.Error("Estado", "Proveedor no Operable por Estado BAJA");
            }
            var facacop = repositorio.Obtener<FACACOP>(x => x.CUIT == proveedor.CUIT);
            if (facacop != null)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor No Operable por ser Apócrifo");
            }

            if (!validacionesMinimas)
            {
                if (oParam.CorredorId != null)
                {
                    if (!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                    {
                        oErrorMessages.Error("Corredor", "El Proveedor no pertenece al Corredor seleccionado");
                    }
                }
            }
            var alta = altaTempranaAgent.ObtenerAlta(proveedor.CUIT);
            if (oParam.Venta != true)
            {
                if (string.IsNullOrEmpty(alta.Mensaje))
                {
                    if (oParam.Consignatario.HasValue && oParam.Consignatario.Value && alta.Consignatario == "NO")
                    {
                        oErrorMessages.Error("Consignatario", "El proveedor no está habilitado como Consignatario");
                    }
                    if (oParam.PlanCanje.HasValue && oParam.PlanCanje.Value && alta.PlanCanje == "NO")
                    {
                        oErrorMessages.Error("PlanCanje", "El proveedor no está habilitado como Proveedor Plan canje");
                    }
                    var boletoCompraNetProvincias = repositorio.Listar<BoletoCompraNetProvincia>(x => x.BoletoCompraNetId == 4);
                    if (oParam.BoletoId == 4 && !boletoCompraNetProvincias.Any(x => x.ProvinciaId == oParam.ProvinciaId))
                    {
                        oErrorMessages.Error("Carta Oferta", "No está habilitado Carta Oferta");
                    }
                    else if (oParam.BoletoId == 4 && boletoCompraNetProvincias.Any(x => x.ProvinciaId == oParam.ProvinciaId) && alta.Carta == "NO")
                    {
                        oErrorMessages.Error("Carta Oferta", "No está habilitado Carta Oferta");
                    }

                    if (alta.AltaTemprana == "SI")
                    {
                        if (alta.Bolsa == "NO" && alta.Nosis == "NO")
                        {
                            oErrorMessages.Error("", "El vendedor de alta temprana no tiene informe Nosis aprobado ni legajo de la bolsa");
                        }
                        if (alta.Bolsa == "NO" && alta.Nosis == "SI")
                        {
                            oErrorMessages.Error("", "El vendedor de alta temprana no tiene legajo de la bolsa");
                        }
                        if (alta.Bolsa == "SI" && alta.Nosis == "NO")
                        {
                            oErrorMessages.Error("", "El vendedor de alta temprana no tiene informe Nosis aprobado");
                        }
                    }
                    if (alta.ProveedorGrano == "SI")
                    {
                        oErrorMessages.Error("MateriasPrimas", "El proveedor es un vendedor eventual");
                    }
                    if (alta.BoletoFisico == "NO" && oParam.BoletoId == 2)
                    {
                        oErrorMessages.Error("BoletoFisico", "No está habilitado Boleto Físico");
                    }
                }
                else
                {
                    oErrorMessages.Error("", alta.Mensaje);
                }
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
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "El campo 'Campaña' no debe estar vacio");
            }
            var centro = repositorio.Obtener<Centro>(x => x.Id == oParam.DestinoId);
            if (!validacionesMinimas || (oParam.EsFason != true && validacionesMinimas))
            {
                if (oParam.DestinoId == 0 || oParam.DestinoId == null)
                {
                    oErrorMessages.Error("DestinoId", "El campo 'Destino' no debe estar vacio");
                }
                if (oParam.TipoNegocioId == 1 && (oParam.CondicionFijacionId == null || oParam.DesdeFijacion == null || oParam.HastaFijacion == null) && oParam.PrestamoDevolucion != true && oParam.Canje != true)
                {
                    oErrorMessages.Error("CondicionFijacionId", "Las Condiciones de Fijaciones no debe estar vacio cuando el contrato es 'A FIJAR'");
                }
                if (oParam.Venta != true)
                {
                    if (PermisosHelper.ObtenerUsuario() != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
                    {
                        if (centro.ValidaRedespacho == true && (oParam.AperturaPrecio == null || !oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho)) && oParam.Pizarra != true)
                        {
                            oErrorMessages.Error("Descuentos", " Se debe completar Redespacho en Acopios.");
                        }

                        if (centro.ValidaRedespacho == true && (oParam.Descuentos == null || !oParam.Descuentos.Any(x => x.Importe < 0 && x.TipoDBId == 1 && x.TipoPeriodoDBId == 1)) && oParam.Pizarra == true)
                        {
                            oErrorMessages.Error("Descuentos", " Se debe completar Redespacho en Acopios.");
                        }

                        if (centro.ValidaRedespacho == false && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho) && oParam.Pizarra != true)
                        {
                            oErrorMessages.Error("Descuentos", " Solo se debe completar Redespacho en Acopios.");
                        }

                    }
                }

            }

            if (oParam.Venta != true)
            {
                if (oParam.ClasificacionId == 1)
                {
                    var centroCodigoSap = repositorio.Obtener<Centro, string>(x => x.Id == oParam.DestinoId, x => x.CodigoSap);
                    var material = repositorio.Obtener<Material, string>(x => x.MaterialId == oParam.MaterialId, x => x.Codigo);
                    var cosecha = repositorio.Obtener<Campaña, string>(x => x.CampañaId == oParam.CampanaId, x => x.Descripcion);
                    var result = capacidadProductiva.ObtenerCapacidadProductiva(proveedor.CUIT, (decimal)oParam.Cantidad, centroCodigoSap, cosecha, material);
                    if (result.ToUpper() != "OK".ToUpper())
                    {
                        oErrorMessages.Error("Capacidad Productiva", result);
                    }
                }
            }

            if (!string.IsNullOrEmpty(oParam.ContratoMadre))
            {
                var sap = oParam.ContratoMadre.PadLeft(10, '0');
                var cantidadMadre = repositorio.Obtener<Contrato, double>(x => x.ContratoSAP == sap, x => x.Cantidad);
                var sumaContratosHijos = repositorio.Listar<Contrato>(x => x.ContratoMadre == sap && x.Id != oParam.Id && (x.EstadoId <= 5 || x.EstadoId == 7)).Select(x => x.Cantidad).Sum();
                if (cantidadMadre - sumaContratosHijos < oParam.Cantidad)
                {
                    oErrorMessages.Error("Cantidad", "La cantidad supera a la cantidad del Convenio");
                }
            }
            if (!validacionesMinimas || (string.IsNullOrEmpty(oParam.ContratoMadre) && validacionesMinimas))
            {
                if (oParam.Precio == 0 && oParam.TipoNegocioId != 1 && (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2))
                {
                    oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacio");
                }

                if ((oParam.PrecioNeto == 0 || oParam.PrecioNeto == null) && (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2))
                {
                    oErrorMessages.Error("Precio", "El campo 'Precio Neto' no debe estar vacio");
                }

                if ((string.IsNullOrEmpty(oParam.MonedaId) && oParam.TipoNegocioId != 1) && (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2))
                {
                    oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
                }
            }


            if ((oParam.LocalidadId == 0 || oParam.LocalidadId == null) && (oParam.TipoNegocioId == 1 || oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' no debe estar vacio");
            }
            if ((oParam.LocalidadId == -1) && (oParam.TipoNegocioId == 1 || oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' debe tener un valor existente");
            }
            if (oParam.ProvinciaId == null && (oParam.TipoNegocioId == 1 || oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("ProvinciaId", "El campo 'Provincia' no debe estar vacio");
            }
            if (oParam.FechaEntrega.Year == 1)
            {
                oErrorMessages.Error("FechaEntrega", "El campo 'Fecha de Entrega' no debe estar vacio");
            }
            if (oParam.FechaDesde.Year == 1)
            {
                oErrorMessages.Error("FechaDesde", "El campo 'Fecha Desde' no debe estar vacio");
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Error("FechaHasta", "El campo 'Fecha Hasta' no debe estar vacio");
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Error("FechaHasta", "El campo 'Fecha Hasta' no debe estar vacio");
            }

            if (oParam.TipoNegocioId == 0)
            {
                oErrorMessages.Error("TipoNegocioId", "El campo 'Tipo de Negocio' no debe estar vacio");
            }

            if (oParam.ComercialId == 0 || oParam.ComercialId == null)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacio");
            }
            if (oParam.ProvinciaId == 1 && oParam.ClasificacionId == 1 && oParam.EstablecimientoPropio == null)
            {
                oErrorMessages.Error("EstablecimientoPropio", "El campo 'Establecimiento' no debe estar vacio cuando Provincia es Buenos Aires y es Productor");
            }

            if (oParam.BoletoId == 0 || oParam.BoletoId == null)
            {
                oErrorMessages.Error("BoletoId", "Boleto no debe estar vacio");
            }
            if ((oParam.BoletoId == 1 || oParam.BoletoId == 2 || oParam.BoletoId == 4) && (oParam.BolsaId == 0 || oParam.BolsaId == null))
            {
                oErrorMessages.Error("BolsaId", "Bolsa no debe estar vacio cuando existe Boleto");
            }
            if (oParam.BoletoId == 4 && oParam.BolsaId != 1)
            {
                oErrorMessages.Error("BolsaCartaOfertaId", "La Bolsa debe ser Buenos Aires cuando el Boleto es 'Carta Oferta'");
            }
            if (oParam.FechaDesde > oParam.FechaHasta)
            {
                oErrorMessages.Error("FechaDesdeHasta", "Fecha Inválida");
            }
            if (oParam.DesdeFijacion > oParam.HastaFijacion)
            {
                oErrorMessages.Error("FechaDesdeHastaFijacion", "Fecha de Fijación inválida");
            }
            if (!validacionesMinimas)
            {
                var rangosPrecio = repositorio.Obtener<RangoPrecio>(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId);
                if (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2)
                {
                    if (rangosPrecio != null && oParam.TipoNegocioId == 2 && (oParam.Precio < rangosPrecio.PrecioMinimo || oParam.Precio > rangosPrecio.PrecioMaximo))
                    {
                        oErrorMessages.Error("Precio", "Precio fuera de Rango, Precio Mínimo: " + rangosPrecio.PrecioMinimo + " Precio Máximo: " + rangosPrecio.PrecioMaximo + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
                    }
                }
            }
            var contrato = repositorio.Obtener<Negocio>(oParam.Id);

            var cantidadMaxima = config.CantidadMaxima * 1000;
            if (oParam.ContratoAcuerdoId != null && oParam.ContratoAcuerdoId > 0)
            {
                var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oParam.ContratoAcuerdoId);
                acuerdo.CantidadAmpliado = acuerdo.CantidadAmpliado ?? 0;
                var cantidadCargada = repositorio.Listar<Contrato>(d => oParam.Id != d.Id && d.ContratoAcuerdoId == oParam.ContratoAcuerdoId.Value && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum(d => d.Cantidad);
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == oParam.ContratoAcuerdoId.Value && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum(d => d.Cantidad);
                var tolerancia = config != null ? config.CantidadAcuerdo.Value * 1000 : 0;
                if (cantidadCargada + oParam.Cantidad > acuerdo.Cantidad + (tolerancia - acuerdo.CantidadAmpliado.Value))
                {
                    if (cantidadCargada + oParam.Cantidad > acuerdo.Cantidad + tolerancia - acuerdo.CantidadAmpliado)
                    {
                        oErrorMessages.Error("", "Cantidad del negocio mayor al saldo disponible del Acuerdo (" + (tolerancia - acuerdo.CantidadAmpliado.Value).ToString("N0") + " kg)");
                    }

                }
            }
            if (cantidadMaxima < oParam.Cantidad)
            {
                oErrorMessages.Error("", "Cantidad del negocio excedida (" + cantidadMaxima.ToString("N0") + " kg)");
            }
            if (!validacionesMinimas)
            {
                if (oParam.StandardDeCalidadId == 0 || oParam.StandardDeCalidadId == null)
                {
                    oErrorMessages.Error("", "Debe seleccionar alguna Calidad");
                }
            }
            if (oParam.MaterialId == 5 && (oParam.ZonaId == 0 || oParam.ZonaId == null))
            {
                oErrorMessages.Error("", "Zona es obligatoria para Girasol Alto Oleico");
            }
            if (oParam.AperturaPrecio != null)
            {
                var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Comisiones);
                if (concepto != null && (concepto.Importe > 0 || concepto.Porcentaje > 0))
                {
                    if (concepto.Porcentaje > 1)
                    {
                        oErrorMessages.Error("", "El porcentaje del concepto Comisiones no puede ser mayor a 1%");
                    }
                }

                if (concepto != null && (concepto.Importe > 0 || concepto.Porcentaje > 0) && (oParam.MaterialId == 4 || oParam.MaterialId == 5))
                {
                    oErrorMessages.Error("", "En los negocios de Girasol la comisión debe ingresarse en descuentos y bonificaciones por fuera del precio.");
                }
            }
            if (oParam.MonedaId != "USDM " && oParam.AperturaPrecio != null && oParam.TipoNegocioId == 2)
            {

                var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

                if (oParam.FechaCierta == null && oParam.PagoDiferido != true && concepto != null)
                {
                    oErrorMessages.Error("", ".Días de diferimiento es obligatorio con el concepto financiero");
                }
                //if (oParam.FechaCierta != null && oParam.ObligatoriedadCostoFinanciero != false)
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
                    if (!((concepto != null && (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value) && (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0)) ||
                        (concepto == null && (!oParam.PagoDiferido.HasValue || (oParam.PagoDiferido.HasValue && !oParam.PagoDiferido.Value)) && (!oParam.DiasPesificado.HasValue || (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value == 0)))))
                    {
                        oErrorMessages.Error("", "Dias de diferimiento/costo financiero es obligatorio con el pago diferido en pesos");
                    }
                }
                //}
            }
            else
            {
                if (oParam.AperturaPrecio != null)
                {
                    var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

                    if (concepto != null)
                    {
                        oErrorMessages.Error("", "El concepto financiero se debe completar solo cuando la moneda es ARP");
                    }
                }

            }

            if (PermisosHelper.ObtenerUsuario() != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                if (string.IsNullOrEmpty(oParam.PosicionCBOT) && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Basis && (x.Importe != 0 || x.Porcentaje != 0)))
                {
                    oErrorMessages.Error("", "Se debe completar Posicion CBOT con el concepto Basis");
                }

                if (!string.IsNullOrEmpty(oParam.PosicionCBOT) && (oParam.AperturaPrecio != null && !oParam.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Basis && (x.Importe != 0 || x.Porcentaje != 0))))
                {
                    oErrorMessages.Error("", "Se debe completar el concepto Basis con Posicion CBOT");
                }

                if (!string.IsNullOrEmpty(oParam.PosicionCBOT) && (oParam.TipoPosicionCBOTId == null || oParam.TipoPosicionCBOTId == 0))
                {
                    oErrorMessages.Error("", "Se debe completar el Tipo Posicion CBOT con Posicion CBOT");
                }
            }

            if ((oParam.StandardDeCalidadId == 2 && oParam.Calidad == null))
            {
                oErrorMessages.Error("", "Se debe completar el campo Valor de Calidad");
            }
            if (oParam.Calidad != null)
            {
                var calidad = oParam.Calidad.LastOrDefault(x => x.CalidadEspecialId == 1);
                if (calidad != null && calidad.PorcentajeHasta < 40)
                {
                    oErrorMessages.Error("", "Falta completar el rango de Dañados");
                }
                calidad = oParam.Calidad.LastOrDefault(x => x.CalidadEspecialId == 2);
                if (calidad != null && calidad.PorcentajeHasta < 100)
                {
                    oErrorMessages.Error("", "Falta completar el rango de Granos Verdes");
                }
                if (calidad != null && calidad.Valor > 10)
                {
                    oErrorMessages.Error("", "El Valor no puede ser mayor a 10 en rango de Granos Verdes");
                }
            }
            if (oParam.Dolarizado.HasValue && oParam.Dolarizado.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }
            if (oParam.DolarizadoExpress.HasValue && oParam.DolarizadoExpress.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("DolarizadoExpress", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }
            if (oParam.Sustentable.HasValue && oParam.Sustentable.Value)
            {
                if (!oParam.ImporteSustentable.HasValue || oParam.ImporteSustentable.Value == 0 || string.IsNullOrEmpty(oParam.MonedaSustentableId))
                {
                    oErrorMessages.Error("Sustentable", "Debe indicar tarifa de sustentable");
                }
                if (oParam.MercsDeposito == true)
                {
                    if (!oParam.FechaDesdeSustentable.HasValue || oParam.FechaDesdeSustentable.Value == null)
                    {
                        oErrorMessages.Error("Sustentable", "Debe indicar fecha desde de sustentable");
                    }
                    if (!oParam.FechaHastaSustentable.HasValue || oParam.FechaHastaSustentable.Value == null)
                    {
                        oErrorMessages.Error("Sustentable", "Debe indicar fecha hasta de sustentable");
                    }
                    if (oParam.FechaDesdeSustentable.HasValue && oParam.FechaHastaSustentable.HasValue
                        && oParam.FechaHastaSustentable.Value < oParam.FechaDesdeSustentable.Value)
                    {
                        oErrorMessages.Error("Sustentable", "Debe indicar rango de fechas válido de sustentable");
                    }
                }
            }
            var cantidadDias = PermisosHelper.Is(PermisosDataAgro.ModificarLimiteDolarizado) ? config.CantidadDiasDolarizadoLimiteMaximo : config.CantidadDias;
            if (oParam.FechaDolarizado != null)
            {
                if (config != null)
                {
                    var fechaLimite = oParam.FechaDesde.AddDays(cantidadDias);
                    if (oParam.FechaDolarizado.Value.Date > fechaLimite.Date)
                    {
                        oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado debe ser menor o igual que los " + cantidadDias + " días");
                    }
                }
            }

            var fechaFijacion = oParam.HastaFijacion;
            var fechaAPrecio = oParam.FechaHasta.AddDays(cantidadDias);

            if (fechaFijacion.HasValue)
            {
                fechaFijacion = fechaFijacion.Value.AddDays(cantidadDias);
            }

            if (oParam.TipoNegocioId == 1 && oParam.FechaDolarizado > fechaFijacion)
            {
                oErrorMessages.Error("dolarizado", "La fecha de pesificación no puede ser mayor a " + cantidadDias + " días de Fijación");
            }

            if (oParam.TipoNegocioId == 2 && oParam.FechaDolarizado > fechaAPrecio)
            {
                oErrorMessages.Error("dolarizado", "La fecha de pesificación no puede ser mayor a " + cantidadDias + " días de la Entrega");
            }

            if (oParam.TarifaFlete != null && (oParam.NivelTarifaId == null || oParam.NivelTarifaId == 0))
            {
                oErrorMessages.Error("", "Se debe cargar Nivel de Tarifa cuando hay Tarifa");
            }
            if (oParam.NivelTarifaId != null && oParam.NivelTarifaId != 0 && oParam.TarifaFlete == null)
            {
                oErrorMessages.Error("", "Se debe cargar Tarifa cuando hay Nivel de Tarifa");
            }
            if (oParam.FechaCierta != null && oParam.FechaCierta.Value < DateTime.Now.Date)
            {
                oErrorMessages.Error("FechaCierta", "La Fecha Cierta debe ser mayor o igual al día de la fecha.");
            }
            if (oParam.PorcentajeDePago == null || oParam.PorcentajeDePago.Value > 100 || oParam.PorcentajeDePago.Value < 0)
            {
                oErrorMessages.Error("PorcentajeDePago", "El Porcentaje de Pago debe ser entre 0 y 100.");
            }
            if (!validacionesMinimas)
            {
                if (oParam.TipoAgenteCompraId != null)
                {
                    if (oParam.CaratulaMAT == null)
                    {
                        oErrorMessages.Error("CaratulaMAT", "Debe completar Caratula MAT.");
                    }
                    if (oParam.PrecioAjusteComision == null)
                    {
                        oErrorMessages.Error("PrecioAjusteComision", "Debe completar Precio Ajuste Comisión.");
                    }
                    if (oParam.MonedaAjusteComisionId == null)
                    {
                        oErrorMessages.Error("MonedaAjusteComisionId", "Debe completar Moneda Ajuste Comisión.");
                    }
                }
                if (oParam.TipoAgenteCompraId == null && (oParam.CaratulaMAT != null || oParam.PrecioAjusteComision != null || oParam.MonedaAjusteComisionId != null || oParam.CaratulaExtension != null))
                {
                    oErrorMessages.Error("TipoAgenteCompraId", "Debe seleccionar el agente de compra.");
                }
            }

            if (!validacionesMinimas)
            {
                if (oParam.FechaOperacion > DateTime.Now.Date)
                {
                    oErrorMessages.Error("FechaOperacion", "La Fecha tiene que ser menor o igual al día de la fecha.");
                }
                else
                {
                    if (oParam.AnulaYReemplazaContratoId == null)
                    {
                        if (oParam.Id > 0)
                        {
                            if ((oParam.FechaOperacion != contrato.Fecha.Date && oParam.FechaOperacion < contrato.Fecha.Date))
                            {

                                var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(contrato.Fecha.Date);

                                if ((oParam.FechaOperacion < diaAnterior && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior)
                                    && oParam.PrestamoDevolucion != true && oParam.Canje != true && oParam.Venta != true))
                                {
                                    oErrorMessages.Error("FechaOperacion", "La Fecha Operación no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));
                                }

                                if (string.IsNullOrEmpty(oParam.MotivoOperacionAnterior))
                                {
                                    oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la Fecha Operacion es anterior al día de la fecha.");
                                }

                                if (!string.IsNullOrEmpty(oParam.MotivoOperacionAnterior) && oParam.MotivoOperacionAnterior.Length <= 5)
                                {
                                    oErrorMessages.Error("MotivoOperacionAnterior", "Es obligatorio ingresar un motivo con más de 5 caracteres");
                                }

                                if (oParam.Venta != true)
                                {
                                    if ((oParam.NoInformaSio == null || oParam.NoInformaSio == false) && oParam.FechaOperacion < diaAnterior)
                                    {
                                        oErrorMessages.Error("NoInformaSio", "Fecha de operación no puede ser anterior a " + diaAnterior.ToString("dd/MM/yyyy"));
                                    }
                                }
                            }
                            if (oParam.FechaOperacion > contrato.Fecha.Date && oParam.Venta != true)
                            {
                                oErrorMessages.Error("NoInformaSio", "Fecha de operación no puede ser mayor a " + contrato.Fecha.ToString("dd/MM/yyyy"));
                            }
                        }
                        else
                        {
                            if (oParam.FechaOperacion < DateTime.Now.Date)
                            {
                                var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(null);

                                if ((oParam.FechaOperacion < diaAnterior && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior)
                                    && oParam.PrestamoDevolucion != true && oParam.Canje != true && oParam.Venta != true))
                                {
                                    oErrorMessages.Error("FechaOperacion", "La Fecha Operación no puede ser anterior al ultimo día habil." + diaAnterior.ToString("dd/MM/yyyy"));
                                }

                                if (string.IsNullOrEmpty(oParam.MotivoOperacionAnterior))
                                {
                                    oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la Fecha Operacion es anterior al día de la fecha.");
                                }

                                if (!string.IsNullOrEmpty(oParam.MotivoOperacionAnterior) && oParam.MotivoOperacionAnterior.Length <= 5)
                                {
                                    oErrorMessages.Error("MotivoOperacionAnterior", "Es obligatorio ingresar un motivo con más de 5 caracteres");
                                }

                                if ((oParam.NoInformaSio == null || oParam.NoInformaSio == false) && oParam.FechaOperacion < diaAnterior && oParam.Venta != true/* && oParam.PrestamoDevolucion != true && oParam.Canje != true*/)
                                {
                                    oErrorMessages.Error("NoInformaSio", "Fecha de operación no puede ser anterior a " + diaAnterior.ToString("dd/MM/yyyy"));
                                }
                            }
                        }
                    }


                    //if (!validacionesMinimas)
                    //{
                    if (oParam.Canje == true)
                    {
                        if (!oParam.Monto.HasValue)
                        {
                            oErrorMessages.Error("Monto", "Se Debe completar el campo Monto cuando hay Canje");
                        }
                        if (String.IsNullOrEmpty(oParam.MonedaCanjeId))
                        {
                            oErrorMessages.Error("Moneda", "Se Debe completar la Moneda que corresponde al campo Canje");
                        }
                        if (String.IsNullOrEmpty(oParam.Insumo))
                        {
                            oErrorMessages.Error("Insumo", "Se Debe completar el campo Insumo cuando hay Canje");
                        }
                    }
                    //}
                    if (!validacionesMinimas)
                    {
                        if (oParam.PrestamoDevolucion == true)
                        {
                            if (!oParam.PlantaDestinoId.HasValue)
                            {
                                oErrorMessages.Error("PlantaDestino", "Se Debe completar el campo Planta Destino cuando hay Préstamo Devolución");
                            }
                        }
                    }
                    //var cosecha = repositorio.Obtener<Campaña, string>(x => x.CampañaId == oParam.CampanaId, x => x.Descripcion);
                    //if (!string.IsNullOrEmpty(cosecha) && oParam.FechaDesde != null && oParam.FechaHasta != null)
                    //{
                    //    var anios = cosecha.Split('-');
                    //    var anioInicial = "20" + anios[0];
                    //    var anioFinal = "20" + anios[1];

                    //    var campaniaDesde = new DateTime(int.Parse(anioInicial), 01, 01);
                    //    var campaniaHasta = new DateTime(int.Parse(anioFinal), 12, 31);
                    //    var fechaDesde = oParam.FechaDesde;
                    //    var fechaHasta = oParam.FechaHasta;

                    //    if (fechaDesde < campaniaDesde || fechaHasta > campaniaHasta)
                    //    {
                    //        oErrorMessages.Error("MotivoOperacionAnterior", " La campaña esta fuera de rango");

                    //    }
                    //}
                    //DateTime fecha = repositorio.Listar<Contrato>(d => oParam.Id == d.Id ).Select(d => d.Fecha).Single();
                    //if (oParam.FechaOperacion.Date < fecha.Date && oParam.Id != 0)
                    //{
                    //    if (string.IsNullOrEmpty(oParam.MotivoOperacionAnterior))
                    //    {
                    //        oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por la cual la Fecha Operacion es anterior al día de la fecha.");
                    //    }
                    //}
                }
            }


            if (!validacionesMinimas)
            {
                if (oParam.PagoCBU != null)
                {
                    var existePago = ListarCBU(proveedor.CUIT, oParam.PagoCBU);
                    var validarPagoCbu = false;
                    foreach (var item in existePago)
                    {
                        if (item.Pago == oParam.PagoCBU)
                        {
                            validarPagoCbu = true;
                        }
                    }
                    if (!validarPagoCbu)
                    {
                        oErrorMessages.Error("PagoCbu", "El cbu ingresado no es válido");
                    }

                }

                if (PermisosHelper.Is(PermisosDataAgro.ModificarCanje) && oParam.TipoNegocioId == 1)
                {
                    if (oParam.Canje != true)
                    {
                        oErrorMessages.Error("Permiso Canje", "Es obligatorio completar el campo Canje");
                    }
                }
            }
            if (contrato != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                if (contrato.DolarizadoTercero == true && oParam.Dolarizado != true && oParam.DolarizadoExpress != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Dolarizado que marco el tercero.");
                }

                if (contrato.SustentableTercero == true && oParam.Sustentable != true && (!oParam.ImporteSustentable.HasValue || oParam.ImporteSustentable.Value == 0 || string.IsNullOrEmpty(oParam.MonedaSustentableId)))
                {
                    oErrorMessages.Error("Sustentable", "Debe indicar tarifa de sustentable que marco el tercero.");
                }

                if (contrato.PagoDiferidoTercero == true && oParam.PagoDiferido != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Pago Diferido que marco el tercero.");
                }
                if (contrato.CalidadTercero == true)
                {
                    if (oParam.MaterialId == 1 && oParam.StandardDeCalidadId == 2)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == 2 && oParam.StandardDeCalidadId == 7)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == 3 && oParam.StandardDeCalidadId == 4)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == 4 && oParam.StandardDeCalidadId == 5)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == 5 && oParam.StandardDeCalidadId == 5)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                }
            }

            if (oParam.DolarizadoCorredor != true && oParam.Dolarizado != true && oParam.DolarizadoExpress != true && oParam.FechaDolarizado != null)
            {
                oErrorMessages.Error("Dolarizado", "Se debe completar Dolarizado si completó Fecha límite .");
            }

            if (oParam.TipoNegocioId == 1 && oParam.Descuentos != null && oParam.Descuentos.Any(a => a.Importe != 0 && a.TipoDBId == 2))
            {
                oErrorMessages.Error("Descuentos y Bonificaciones", "No se puede completar importe en un descuento o bonificacion fuera de precio.");
            }

            if (oParam.PagoDiferido == true && oParam.DiasPesificado != null)
            {
                //var conf = configuracionManager.TraerConfiguraciones();
                if (config != null)
                {
                    var limitePesificado = /*PermisosHelper.Is(PermisosDataAgro.ModificarLimitePesificado) ? config.CantidadDiasPesificadoLimite :*/ config.DiasDiferimiento;
                    if (oParam.DiasPesificado.Value > limitePesificado)
                    {
                        oErrorMessages.Error("Pago Diferido", "Los dias de pesificado deben ser menor o igual que los " + limitePesificado + " días");
                    }
                }
            }

            if (contrato != null && PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && contrato.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
            {
                oErrorMessages.Error("Contrato", "No se puede modificar el contrato.");
            }

            if (!repositorio.Existe<Localidad>(a => a.LocalidadId == oParam.LocalidadId))
            {
                oErrorMessages.Error("Localidad", "La localidad seleccionada no es valida.");
            }
            if (!repositorio.Existe<Provincia>(a => a.ProvinciaId == oParam.ProvinciaId))
            {
                oErrorMessages.Error("Provincia", "La provincia seleccionada no es valida.");
            }

            if (oParam.Descuentos != null)
            {
                if (oParam.Descuentos.Any(a => a.FechaDesde != null && a.FechaHasta != null && a.FechaDesde > a.FechaHasta))
                {
                    oErrorMessages.Error("Descuentos", "La fecha desde de descuento o bonificacion no puede ser mayor a la fecha hasta.");
                }
                if (oParam.Descuentos.Any(a => (a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && (a.FechaDesde == null || a.FechaHasta == null)))
                {
                    oErrorMessages.Error("Descuentos", "La fecha desde y hasta de descuento o bonificacion es obligatoria.");
                }
                if (!validacionesMinimas)
                {
                    if (oParam.Descuentos.Any(a => a.TipoPeriodoDBId == 3 || a.TipoPeriodoDBId == 2) && oParam.TipoNegocioId == 2)
                    {
                        oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificacion por Fecha de Fijación o de Entrega en un a precio.");
                    }
                }

                if (oParam.Descuentos.Any(a => a.TipoDBId == 1 && (a.Importe > 0 || a.Porcentaje > 0)) && oParam.TipoNegocioId == 2 && oParam.Precio > 0)
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificacion Sobre Precio cuando tiene precio.");
                }

                if (oParam.TipoNegocioId == 1 && oParam.Descuentos.Any(a => a.TipoPeriodoDBId == 3 && (a.FechaDesde < oParam.DesdeFijacion || a.FechaDesde > oParam.HastaFijacion || a.FechaHasta < oParam.DesdeFijacion || a.FechaHasta > oParam.HastaFijacion)))
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificacion por Fecha de Fijación fuera del rango de Fijación.");
                }
                if (oParam.TipoNegocioId == 1 && oParam.Descuentos.Any(a => a.TipoPeriodoDBId == 2 && (a.FechaDesde < oParam.FechaDesde || a.FechaDesde > oParam.FechaHasta || a.FechaHasta < oParam.FechaDesde || a.FechaHasta > oParam.FechaHasta)))
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificacion por Por Fecha de Entrega fuera del rango de Entrega.");
                }
            }
            if (oParam.ContratoCorredor != null && oParam.ContratoCorredor.Length > 10)
            {
                oErrorMessages.Error("ContratoCorredor", "El numero de contrato corredor no puede ser más largo que 10 caracteres.");
            }
            if (oParam.ContratoVendedor != null && oParam.ContratoVendedor.Length > 10)
            {
                oErrorMessages.Error("ContratoVendedor", "El numero de contrato vendedor no puede ser más largo que 10 caracteres.");
            }
            if (oParam.AnulaYReemplazaContratoId != null && string.IsNullOrEmpty(oParam.MotivoReemplazo))
            {
                oErrorMessages.Error("MotivoReemplazo", "Complete el motivo de reemplazo.");
            }
            if (oParam.Venta == true)
            {
                if (oParam.CondicionDePagoDiaFijacion > 0 && (string.IsNullOrEmpty(oParam.CondicionDePagoTipoFijacion) || oParam.CondicionDePagoFijacionVentaId == null))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar la condición de pago");
                }

                if (!string.IsNullOrEmpty(oParam.CondicionDePagoTipoFijacion) && (!oParam.CondicionDePagoDiaFijacion.HasValue || oParam.CondicionDePagoDiaFijacion.Value <= 0))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar los dias en la condición de pago");
                }

                if (oParam.CondicionDePagoFijacionVentaId.HasValue && oParam.CondicionDePagoFijacionVentaId > 0 && (!oParam.CondicionDePagoDiaFijacion.HasValue || oParam.CondicionDePagoDiaFijacion.Value <= 0))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar los dias en la condición de pago");
                }

                if (oParam.CondicionDePagoDiaPesificado > 0 && (string.IsNullOrEmpty(oParam.CondicionDePagoTipoPesificado) || oParam.CondicionDePagoPesificadoVentaId == null))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar la condición de pesificación");
                }

                if (!string.IsNullOrEmpty(oParam.CondicionDePagoTipoPesificado) && (!oParam.CondicionDePagoDiaPesificado.HasValue || oParam.CondicionDePagoDiaPesificado.Value <= 0))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar los dias en la condición de pesificación");
                }

                if (oParam.CondicionDePagoPesificadoVentaId.HasValue && oParam.CondicionDePagoPesificadoVentaId > 0 && (!oParam.CondicionDePagoDiaPesificado.HasValue || oParam.CondicionDePagoDiaPesificado.Value <= 0))
                {
                    oErrorMessages.Error("DiaVenta", "Debe completar los dias en la condición de pesificación");
                }

                if (!oParam.CondicionDePagoPesificadoVentaId.HasValue && oParam.MonedaId == "USDM ")
                {
                    oErrorMessages.Error("DiaVenta", "La condicion de pesificado en condiciones adicionales de venta es obligatoria con la moneda USD");
                }

                if (oParam.ComisionAFavorId > 0 && (!oParam.PorcentajeComisionVenta.HasValue ||
                    (oParam.PorcentajeComisionVenta.HasValue && oParam.PorcentajeComisionVenta.Value <= 0)))
                {
                    oErrorMessages.Error("ComisionAFavor", "Debe completar el porcentaje de comisión en las condiciones de venta cuando Comision a Favor está completo");
                }

                if (oParam.ComisionAFavorId == null && (oParam.PorcentajeComisionVenta.HasValue ||
                  (oParam.PorcentajeComisionVenta.HasValue && oParam.PorcentajeComisionVenta.Value > 0)))
                {
                    oErrorMessages.Error("ComisionAFavor", "Debe completar la Comisión a Favor en las condiciones de venta cuando Porcentaje de comision está completo");
                }
            }

            if (proveedor != null && !string.IsNullOrEmpty(oParam.UsuarioTercero))
            {
                var contactos = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == proveedor.ProveedorId);
                var listaEmail = new List<string>();
                if (!string.IsNullOrEmpty(proveedor.Email1))
                {
                    listaEmail.Add(proveedor.Email1);
                }
                if (!string.IsNullOrEmpty(proveedor.Email2))
                {
                    listaEmail.Add(proveedor.Email2);
                }
                if (!string.IsNullOrEmpty(proveedor.Email3))
                {
                    listaEmail.Add(proveedor.Email3);
                }
                if (!string.IsNullOrEmpty(proveedor.Email4))
                {
                    listaEmail.Add(proveedor.Email4);
                }
                foreach (var contacto in contactos)
                {
                    if (!string.IsNullOrEmpty(contacto.Email1))
                        listaEmail.Add(contacto.Email1);
                    if (!string.IsNullOrEmpty(contacto.Email2))
                        listaEmail.Add(contacto.Email2);
                    if (!string.IsNullOrEmpty(contacto.Email3))
                        listaEmail.Add(contacto.Email3);
                }

                if (!listaEmail.Any(x => x.Contains(oParam.UsuarioTercero)))
                {
                    oErrorMessages.Error("Usuario no registrado", "El usuario no esta habilitado para cargar negocios, por favor comunicarse con su comercial");
                }
            }


            if (!validacionesMinimas && oParam.TipoNegocioId == 1 && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
            {
                var redespacho = oParam.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho).SingleOrDefault();
                if (redespacho != null && redespacho.Importe != 0)
                {
                    if (redespacho.MonedaId != "USDM ")
                    {
                        oErrorMessages.Error("Redespacho", "El redespacho debe cargarse en USDM.");
                    }
                    if (redespacho.Importe < (Math.Abs(config.RedespachoMaximoUSDM) * -1))
                    {
                        oErrorMessages.Error("Redespacho", "El redespacho no puede ser superior a " + (Math.Abs(config.RedespachoMaximoUSDM) * -1).ToString() + ".");
                    }
                }
            }
            if (!validacionesMinimas && oParam.TipoNegocioId == 2 && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
            {
                var redespacho = oParam.AperturaPrecio.Where(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho).SingleOrDefault();
                if (redespacho != null && redespacho.Importe != 0)
                {
                    if (oParam.MonedaId == "USDM " && redespacho.Importe < (Math.Abs(config.RedespachoMaximoUSDM) * -1))
                    {
                        oErrorMessages.Error("Redespacho", "El redespacho en USDM no puede ser superior a " + (Math.Abs(config.RedespachoMaximoUSDM) * -1).ToString() + ".");
                    }

                    if (oParam.MonedaId == "ARP  " && redespacho.Importe > (Math.Abs(config.RedespachoMaximoARP) * -1))
                    {
                        oErrorMessages.Error("Redespacho", "El redespacho en ARP no puede ser menor a " + (Math.Abs(config.RedespachoMaximoARP) * -1).ToString() + ".");
                    }
                }
            }

            return oErrorMessages;
        }

        public GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
            if (oContratoSave.ContratoMadre != null)
            {
                var sap = oContratoSave.ContratoMadre.PadLeft(10, '0');
                var cantidadMadre = repositorio.Obtener<Contrato, double>(x => x.ContratoSAP == sap, x => x.Cantidad);
                var sumaContratosHijos = repositorio.Listar<Contrato>(x => x.ContratoMadre == sap && x.Id != oContrato.Id).Select(x => x.Cantidad).Sum();
                if (cantidadMadre - sumaContratosHijos < oContratoSave.Cantidad + oContrato.Ampliaciones)
                {
                    oEntityErrors.Error("Cantidad", "La cantidad supera a la cantidad del Convenio");
                }
            }

            ValidarCantidadAcuerdoTolerancia(oContratoSave, oContrato, oEntityErrors);
            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado))
            {
                string jsonContrato = JsonConvert.SerializeObject(oContratoSave, new JsonSerializerSettings()
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects
                });
                oContratoSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oContratoSave.Id, TipoNegocioId = oContratoSave.TipoNegocioId, ComercialId = oContratoSave.ComercialId });
                oContratoSave.Ampliaciones = oContrato.Ampliaciones.Value;
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Reconfirmar;
                if (oContratoSave.CantidadCamiones != null && oContratoSave.CantidadCamiones > 0)
                {
                    oContratoSave.CantidadCamiones = Convert.ToInt32(Math.Ceiling(((decimal)oContratoSave.Cantidad + (decimal)oContrato.Ampliaciones) / 30000));
                }
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Modificar, oContratoSave.GetType());
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede ampliar");
            }



            return oEntityErrors;
        }
        private GrabarContratoResult ValidarCantidadAcuerdoTolerancia(Contrato oContratoSave, Contrato oContrato, GrabarContratoResult oEntityErrors)
        {
            if (oContratoSave.ContratoAcuerdoId != null && oContratoSave.ContratoAcuerdoId > 0)
            {
                var config = repositorio.Obtener<Configuracion>(1);
                var cantidadMaxima = config.CantidadMaxima * 1000;
                var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoSave.ContratoAcuerdoId);
                var cantidadAcuerdo = acuerdo.Cantidad;
                var cantidadAmpliadoAcuerdo = acuerdo.CantidadAmpliado;
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == oContratoSave.ContratoAcuerdoId.Value && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum(d => d.Cantidad);
                var tolerancia = (config != null ? config.CantidadAcuerdo.Value * 1000 : 0);
                var totalAcuerdo = cantidadAcuerdo + tolerancia - (cantidadAmpliadoAcuerdo ?? 0);
                if (cantidadMaxima < cantidadTodoAcuerdo + oContrato.Ampliaciones)
                {
                    oEntityErrors.Error("", "Cantidad del negocio excedida (" + cantidadMaxima.ToString("N0") + " kg)");
                    return oEntityErrors;
                }
                else if (totalAcuerdo < oContrato.Ampliaciones + cantidadTodoAcuerdo)
                {
                    oEntityErrors.Error("", "Cantidad del negocio mayor al saldo disponible del Acuerdo (" + (totalAcuerdo - cantidadTodoAcuerdo).ToString("N0") + " kg)");
                    return oEntityErrors;
                }
            }
            return oEntityErrors;
        }
        public GrabarContratoResult GrabarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();

            Validar(oContrato, oEntityErrors, false);

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            var oContratoSave = new Contrato();
            List<DescuentoBonificacion> descuentosExistentes = null;
            List<Calidad> calidadesExistentes = null;
            List<AperturaPrecio> aperturasExistentes = null;
            List<PrecioPactado> preciosExistentes = null;
            var hoy = DateTime.Now;

            if (oContrato.Id != 0)
            {
                oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
                if ((oContrato.ChequeElectronico != oContratoSave.ChequeElectronico && oContrato.ChequeElectronico.Value) || oContratoSave.PagoCBU != oContrato.PagoCBU)
                {
                    var result = validarPagoAgente.ValidarEstado(oContratoSave.ContratoSAP, "");
                    if (result != "Ok")
                    {
                        oEntityErrors.Error("", result);
                        return oEntityErrors;
                    }
                }
                descuentosExistentes = oContratoSave.Descuentos.ToList();
                calidadesExistentes = oContratoSave.Calidad.ToList();
                aperturasExistentes = oContratoSave.AperturaPrecio.ToList();
                preciosExistentes = oContratoSave.PrecioPactado.ToList();

                if (oContratoSave.EstadoId == 5 || oContratoSave.EstadoId == 6)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }
                if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oContratoSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }
                if ((oContratoSave.Precio != oContrato.Precio || oContratoSave.Cantidad != oContrato.Cantidad || oContratoSave.MonedaId != oContrato.MonedaId || ValidarCalidadModificada(oContrato, oContratoSave))
                    && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta && oContratoSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion))
                {
                    oContrato.EstadoId = 7;
                    if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                    {
                        string jsonContrato = JsonConvert.SerializeObject(oContratoSave, new JsonSerializerSettings()
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver(),
                            ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                            PreserveReferencesHandling = PreserveReferencesHandling.Objects
                        });
                        oContratoSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oContrato.Id, TipoNegocioId = oContrato.TipoNegocioId, ComercialId = oContrato.ComercialId });
                    }

                }
                else
                {
                    logger.Debug("Contrato Confirmado por estado: " + oContratoSave.EstadoId);

                    oContrato.EstadoId = oContratoSave.EstadoId;
                }
            }
            else
            {
                oContratoSave.Fecha = hoy;
                oContratoSave.ComercialCreadorId = oContrato.ComercialCreadorId;
                oContratoSave.ProveedorCreadorId = oContrato.ProveedorCreadorId;
                oContratoSave.UsuarioId = oContrato.UsuarioId;
            }

            var confirmacionAutomatica = (oContrato.EstadoId < (int)EnumEstadoContrato.PreAprobacion || (!PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oContrato.EstadoId == (int)EnumEstadoContrato.PreAprobacion && oContrato.Id > 0))
                && ConfirmacionAutomatica(oContrato) && DateTime.Now.Date == oContrato.FechaOperacion.Date;

            if (oContrato.ContratoAcuerdoId != null && oContrato.ContratoAcuerdoId != 0 && oContrato.ContratoAcuerdoId.HasValue && oContrato.ProveedorCreadorId == null)
            {
                logger.Debug("Contrato Confirmado por contratoAcuerdo: " + oContrato.ContratoAcuerdoId);
                oContrato.EstadoId = 2;
            }
            if (confirmacionAutomatica || (oContrato.ContratoAcuerdoId != null && oContrato.ContratoAcuerdoId != 0 && oContrato.ContratoAcuerdoId.HasValue && oContrato.ProveedorCreadorId == null))
            {
                AgregarAmpliacionAlAcuerdo(oEntityErrors, oContrato.Id, oContrato.Cantidad, 0, oContrato.ContratoAcuerdoId, oContratoSave.Cantidad);
                if (oEntityErrors.HayError) return oEntityErrors;
            }
            oContrato.DolarizadoCorredor = false;
            if (oContrato.CorredorId.HasValue && oContrato.Dolarizado == true)
            {
                oContrato.Dolarizado = false;
                oContrato.DolarizadoCorredor = true;
            }
            oContratoSave.MaterialId = oContrato.MaterialId;
            oContratoSave.TipoNegocioId = oContrato.TipoNegocioId;
            oContratoSave.Cantidad = oContrato.Cantidad;
            oContratoSave.Precio = oContrato.Precio;
            oContratoSave.FechaEntrega = oContrato.FechaEntrega;
            oContratoSave.CampanaId = oContrato.CampanaId;
            oContratoSave.FechaDesde = oContrato.FechaDesde;
            oContratoSave.FechaHasta = oContrato.FechaHasta;
            oContratoSave.ProveedorId = oContrato.ProveedorId;
            oContratoSave.MonedaId = oContrato.MonedaId;
            oContratoSave.GrupoCompra = oContrato.GrupoCompra;
            oContratoSave.ComercialId = oContrato.ComercialId;
            oContratoSave.LocalidadId = oContrato.LocalidadId;
            oContratoSave.ProvinciaId = oContrato.ProvinciaId;
            oContratoSave.Base = oContrato.Base;
            oContratoSave.ImporteSustentable = oContrato.ImporteSustentable;
            oContratoSave.MonedaSustentableId = oContrato.MonedaSustentableId;
            oContratoSave.FechaDesdeSustentable = oContrato.FechaDesdeSustentable;
            oContratoSave.FechaHastaSustentable = oContrato.FechaHastaSustentable;
            oContratoSave.FechaDolarizado = oContrato.FechaDolarizado;
            oContratoSave.DiasPesificado = oContrato.DiasPesificado;
            oContratoSave.NoInformaSio = oContrato.NoInformaSio;
            oContratoSave.TrigoEspecial = oContrato.TrigoEspecial;
            oContratoSave.EstadoId = oContrato.EstadoId;
            oContratoSave.Ampliaciones = oContrato.Ampliaciones;
            oContratoSave.Observacion = oContrato.Observacion;
            oContratoSave.DestinoId = oContrato.DestinoId;
            oContratoSave.Consignatario = oContrato.Consignatario;
            oContratoSave.PlanCanje = oContrato.PlanCanje;
            oContratoSave.CondicionFijacionId = oContrato.CondicionFijacionId;
            oContratoSave.CD = oContrato.CD;
            oContratoSave.Warrant = oContrato.Warrant;
            oContratoSave.PagoDirectoVendedor = oContrato.PagoDirectoVendedor;
            oContratoSave.EstablecimientoPropio = oContrato.EstablecimientoPropio != null ? oContrato.EstablecimientoPropio : null;
            oContratoSave.ClasificacionId = oContrato.ClasificacionId;
            oContratoSave.CantidadCamiones = oContrato.CantidadCamiones;
            oContratoSave.BoletoId = oContrato.BoletoId;
            oContratoSave.BolsaId = oContrato.BolsaId == 0 ? null : oContrato.BolsaId;
            oContratoSave.DesdeFijacion = oContrato.DesdeFijacion;
            oContratoSave.HastaFijacion = oContrato.HastaFijacion;
            oContratoSave.MercsDeposito = oContrato.MercsDeposito;
            oContratoSave.CorredorId = oContrato.CorredorId;
            oContratoSave.PorcentajeComision = oContrato.PorcentajeComision;
            oContratoSave.ContratoVendedor = oContrato.ContratoVendedor;
            oContratoSave.ContratoCorredor = oContrato.ContratoCorredor;
            oContratoSave.SelCargoVendedor = oContrato.SelCargoVendedor;
            oContratoSave.SelCargoMOA = oContrato.SelCargoMOA;
            oContratoSave.Madre = oContrato.Madre;
            oContratoSave.EsFason = oContrato.EsFason;
            oContratoSave.ContratoMadre = oContrato.ContratoMadre?.PadLeft(10, '0');
            oContratoSave.PrecioNeto = oContrato.PrecioNeto;
            oContratoSave.StandardDeCalidadId = oContrato.StandardDeCalidadId;
            oContratoSave.Pizarra = oContrato.Pizarra;
            oContratoSave.PagoDiferido = oContrato.PagoDiferido;
            oContratoSave.PagoDiferidoTerceroId = oContrato.PagoDiferidoTerceroId;
            oContratoSave.ZonaId = oContrato.ZonaId;
            oContratoSave.Compensacion = oContrato.Compensacion;
            oContratoSave.TarifaFlete = oContrato.TarifaFlete;
            oContratoSave.NivelTarifaId = oContrato.NivelTarifaId == 0 ? null : oContrato.NivelTarifaId;
            oContratoSave.Dolarizado = oContrato.Dolarizado;
            oContratoSave.DolarizadoCorredor = oContrato.DolarizadoCorredor;
            oContratoSave.Sustentable = oContrato.Sustentable;
            oContratoSave.FechaCierta = oContrato.FechaCierta;
            oContratoSave.PorcentajeDePago = oContrato.PorcentajeDePago;
            oContratoSave.TipoAgenteCompraId = oContrato.TipoAgenteCompraId;
            oContratoSave.CaratulaExtension = oContrato.CaratulaExtension;
            oContratoSave.CaratulaMAT = oContrato.CaratulaMAT;
            oContratoSave.PrecioAjusteComision = oContrato.PrecioAjusteComision;
            oContratoSave.MonedaAjusteComisionId = oContrato.MonedaAjusteComisionId;
            oContratoSave.ContratoAcuerdoId = oContrato.ContratoAcuerdoId;
            oContratoSave.FechaOperacion = oContrato.FechaOperacion;
            oContratoSave.MotivoOperacionAnterior = oContrato.MotivoOperacionAnterior;
            oContratoSave.ChequeElectronico = oContrato.ChequeElectronico;
            oContratoSave.DolarizadoExpress = oContrato.DolarizadoExpress;
            oContratoSave.PagoCBU = oContrato.PagoCBU;
            oContratoSave.Canje = oContrato.Canje;
            oContratoSave.MonedaCanjeId = oContrato.MonedaCanjeId;
            oContratoSave.Monto = oContrato.Monto;
            oContratoSave.Insumo = oContrato.Insumo;
            oContratoSave.PrestamoDevolucion = oContrato.PrestamoDevolucion;
            oContratoSave.PlantaDestinoId = oContrato.PlantaDestinoId;
            oContratoSave.Venta = oContrato.Venta;
            oContratoSave.ObligatoriedadCostoFinanciero = oContrato.FechaCierta.HasValue &&
             oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
             oContrato.ObligatoriedadCostoFinanciero.HasValue && !oContrato.ObligatoriedadCostoFinanciero.Value
             ? null : oContrato.FechaCierta.HasValue ? oContrato.ObligatoriedadCostoFinanciero : null;
            oContratoSave.PosicionCBOT = oContrato.PosicionCBOT;
            oContratoSave.TipoPosicionCBOTId = oContrato.TipoPosicionCBOTId;
            oContratoSave.AnulaYReemplazaContratoId = oContrato.AnulaYReemplazaContratoId;
            oContratoSave.MotivoReemplazo = oContrato.MotivoReemplazo;

            //oContratoSave.ObligatoriedadBonificacion = oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 &&
            // oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0)) &&
            // oContrato.ObligatoriedadBonificacion.HasValue && !oContrato.ObligatoriedadBonificacion.Value
            // ? null : oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 ? oContrato.ObligatoriedadBonificacion : null;
            if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                oContratoSave.CalidadTercero = oContrato.CalidadTercero;
                oContratoSave.DolarizadoTercero = oContrato.DolarizadoTercero;
                oContratoSave.ObservacionTercero = oContrato.ObservacionTercero;
                oContratoSave.PagoDiferidoTercero = oContrato.PagoDiferidoTercero;
                oContratoSave.SustentableTercero = oContrato.SustentableTercero;
                oContratoSave.UsuarioTercero = oContrato.UsuarioTercero;
            }

            oContratoSave.ProcedenciaVentaId = oContrato.ProcedenciaVentaId;
            oContratoSave.CamaraId = oContrato.CamaraId;
            oContratoSave.ComisionAFavorId = oContrato.ComisionAFavorId;
            oContratoSave.PorcentajeComisionVenta = oContrato.PorcentajeComisionVenta;
            oContratoSave.FleteACargo = oContrato.FleteACargo;
            oContratoSave.KgBalanza = oContrato.KgBalanza;
            oContratoSave.CondicionDePagoDiaPesificado = oContrato.CondicionDePagoDiaPesificado;
            oContratoSave.CondicionDePagoTipoPesificado = oContrato.CondicionDePagoTipoPesificado;
            oContratoSave.CondicionDePagoPesificadoVentaId = oContrato.CondicionDePagoPesificadoVentaId;
            oContratoSave.Pago = oContrato.Pago;
            oContratoSave.CondicionDePagoDiaFijacion = oContrato.CondicionDePagoDiaFijacion;
            oContratoSave.CondicionDePagoTipoFijacion = oContrato.CondicionDePagoTipoFijacion;
            oContratoSave.CondicionDePagoFijacionVentaId = oContrato.CondicionDePagoFijacionVentaId;
            oContratoSave.CreditoDisponible = oContrato.CreditoDisponible;
            var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.ProveedorId, x => x.CUIT);
            oContratoSave.MonedaCreditoDisponible = validarCreditoAgente.ValidarCredito(cuit).Moneda;

            if (oContratoSave.PrecioPactado != null)
            {
                foreach (var precio in preciosExistentes)
                {
                    repositorio.Remover(precio);
                }
            }
            else
            {
                oContratoSave.PrecioPactado = new List<PrecioPactado>();
            }
            if (oContrato.PrecioPactado != null)
            {
                foreach (var precio in oContrato.PrecioPactado)
                {
                    precio.ContratoId = oContrato.Id;
                    oContratoSave.PrecioPactado.Add(precio);
                }
            }
            if (descuentosExistentes != null)
            {
                foreach (var descExistente in descuentosExistentes)
                {
                    if (oContrato.Descuentos == null || !oContrato.Descuentos.Any(x => x.Id == descExistente.Id))
                    {
                        repositorio.Remover(descExistente);
                        //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                        //{
                        //    oContratoSave.EstadoId = 7;
                        //}
                    }
                }
            }
            if (oContrato.Descuentos != null)
            {
                foreach (var descuento in oContrato.Descuentos.Where(x => x.Id == 0))
                {
                    descuento.Negocio = oContratoSave;
                    repositorio.Agregar(descuento);
                    //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                    //{
                    //    oContratoSave.EstadoId = 7;
                    //}
                }
            }

            if (calidadesExistentes != null)
            {
                foreach (var calExistente in calidadesExistentes)
                {
                    if (oContrato.Calidad == null || !oContrato.Calidad.Any(x => x.Id == calExistente.Id))
                    {
                        repositorio.Remover(calExistente);
                        //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                        //{
                        //    oContratoSave.EstadoId = 7;
                        //}
                    }
                }
            }
            if (oContrato.Calidad != null)
            {
                foreach (var calidad in oContrato.Calidad.Where(x => x.Id == 0))
                {
                    calidad.Contrato = oContratoSave;
                    repositorio.Agregar(calidad);
                    //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                    //{
                    //    oContratoSave.EstadoId = 7;
                    //}
                }
            }

            if (oContratoSave.ContratoSAP != null)
            {
                oContratoSave.ContratoSAP = oContrato.ContratoSAP;
            }

            if (oContratoSave.Id == 0)
            {
                oContratoSave.ContratoAcuerdoId = oContrato.ContratoAcuerdoId;
                repositorio.Agregar(oContratoSave);
            }



            if (aperturasExistentes != null)
            {
                foreach (var aperturaExistente in aperturasExistentes)
                {
                    repositorio.Remover(aperturaExistente);
                }
            }

            if (oContrato.AperturaPrecio != null && oContrato.AperturaPrecio.Count > 0)
            {
                if (oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
                {
                    var redespacho = oContrato.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho);
                    redespacho.Importe = -1 * Math.Abs(redespacho.Importe);
                }
                oContratoSave.AperturaPrecio = oContrato.AperturaPrecio;
            }

            if (confirmacionAutomatica)
            {
                oContratoSave.FechaConfirmacion = DateTime.Now;
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                logger.Debug("El contrato " + oContrato.Id + " se confirmo automaticamente por estar dentro de los rangos configurados");

            }
            if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                oContratoSave.EstadoId = (int)EnumEstadoContrato.PreAprobacion;
            }
            var tipoDeLog = (oContratoSave.Id == 0 || string.IsNullOrEmpty(oContratoSave.ContratoSAP)) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar;
            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), tipoDeLog, oContratoSave.GetType());
            if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
            {
                try
                {
                    if (oContratoSave.ComercialId.HasValue)
                    {
                        diferencialManager.ValidarComprasDiferencial(oContratoSave.ComercialId.Value);
                    }
                    else
                    {
                        diferencialManager.ValidarComprasDiferencial(oContratoSave.Comercial.ComercialId);
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("No se pudo ValidarComprasDiferencial", ex);
                }
            }

            return oEntityErrors;
        }
        private bool ConfirmacionAutomatica(Contrato contrato)
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
                x => contrato.FechaDesde >= x.DesdeEntrega &&
                   contrato.FechaHasta <= x.HastaEntrega);


            if (rango != null)
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                var cantidad =
                    repositorio.Listar<Contrato, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 2
                 && x.MaterialId == rango.MaterialId && x.ContratoAcuerdoId == null);
                cantidad.AddRange(repositorio.Listar<ContratoAcuerdo, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.Id != contrato.Id && x.TipoNegocioId == 6
                 && x.MaterialId == rango.MaterialId && x.Precio > 0));
                //cantidad.AddRange(repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.Fecha == hoy &&
                //(x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == rango.MaterialId));
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

        public DataSourceResult TraerTodosContratos(DataSourceRequest request, bool corredor, List<int> listComercialesId, List<int> corredoresComercial)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosContratos(request, corredor, listComercialesId, corredoresComercial));
        }
        //public DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro, bool corredor, List<int> listComercialesId, List<int> corredoresComercial)
        public DataSourceResult TraerContratosFiltrados(DataSourceRequest filtro, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerContratosPorFiltro(filtro, equipo));
        }
        public GrabarContratoResult ConfirmarContrato(int contratoId, int usuarioConfirmador)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);
            if (oContratoSave == null)
            {
                oEntityErrors.Error("", "El contrato no se puede confirmar");
                return oEntityErrors;
            }

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente ||
                                          oContratoSave.EstadoId == (int)EnumEstadoContrato.Oferta ||
                                          oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar))
            {
                AgregarAmpliacionAlAcuerdo(oEntityErrors, oContratoSave.Id, oContratoSave.Cantidad, oContratoSave.Ampliaciones, oContratoSave.ContratoAcuerdoId, null);
                if (oEntityErrors.HayError) return oEntityErrors;
                oContratoSave.Cantidad += oContratoSave.Ampliaciones ?? 0;
                oContratoSave.CantidadAmpliado = (oContratoSave.CantidadAmpliado ?? 0) + (oContratoSave.Ampliaciones ?? 0);
                oContratoSave.Ampliaciones = 0;

                oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                oContratoSave.UsuarioConfirmadorId = usuarioConfirmador;
                oContratoSave.FechaConfirmacion = DateTime.Now;
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());

                try
                {
                    diferencialManager.ValidarComprasDiferencial(oContratoSave.ComercialId.Value);

                }
                catch (Exception ex)
                {
                    logger.Error("No se pudo ValidarComprasDiferencial", ex);
                }

                var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.ComercialId.Value);
                try
                {
                    foreach (var comercialId in comerciales)
                    {
                        EnviarNotificacion(comercialId, oContratoSave);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }
            }



            return oEntityErrors;
        }

        private void AgregarAmpliacionAlAcuerdo(GrabarContratoResult oEntityErrors, int contratoId, double cantidad, double? ampliaciones, int? contratoAcuerdoId, double? cantidadOriginal)
        {
            ampliaciones = ampliaciones ?? 0;
            cantidadOriginal = cantidadOriginal ?? 0;
            var config = repositorio.Obtener<Configuracion>(1);

            if (contratoAcuerdoId != null && contratoAcuerdoId > 0)
            {
                var acuerdo = repositorio.Obtener<ContratoAcuerdo>(contratoAcuerdoId);
                acuerdo.CantidadAmpliado = acuerdo.CantidadAmpliado ?? 0;
                var cantidadCargada = repositorio.Listar<Contrato>(d => contratoId != d.Id && d.ContratoAcuerdoId == contratoAcuerdoId.Value && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum(d => d.Cantidad);
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == contratoAcuerdoId.Value && (d.EstadoId == 1 || d.EstadoId == 2 || d.EstadoId == 3 || d.EstadoId == 4 || d.EstadoId == 5 || d.EstadoId == 7)).Sum(d => d.Cantidad);
                var tolerancia = config != null ? config.CantidadAcuerdo.Value * 1000 : 0;
                if (cantidad < cantidadOriginal)
                {
                    if (acuerdo.CantidadAmpliado > 0)
                    {
                        acuerdo.Cantidad -= acuerdo.CantidadAmpliado.Value < (cantidadOriginal.Value - cantidad) ? acuerdo.CantidadAmpliado.Value : cantidadOriginal.Value - cantidad;
                        acuerdo.CantidadAmpliado -= acuerdo.CantidadAmpliado.Value < (cantidadOriginal.Value - cantidad) ? acuerdo.CantidadAmpliado.Value : (cantidadOriginal.Value - cantidad);
                    }
                }
                else
                {
                    if (cantidadCargada + cantidad + ampliaciones > acuerdo.Cantidad)// es una ampliacion
                    {
                        if (cantidadCargada + cantidad + ampliaciones > acuerdo.Cantidad + (tolerancia - acuerdo.CantidadAmpliado))
                        {
                            oEntityErrors.Error("", "Cantidad del negocio mayor al saldo disponible del Acuerdo (" + (tolerancia - acuerdo.CantidadAmpliado.Value).ToString("N0") + " kg)");
                        }
                        else
                        {
                            if (contratoId == 0)//nueuvo
                            {
                                var disponible = acuerdo.Cantidad - cantidadCargada;
                                if (cantidad > disponible)
                                {
                                    acuerdo.Cantidad += cantidad - disponible;
                                    acuerdo.CantidadAmpliado += cantidad - disponible;
                                }
                            }
                            if (contratoId > 0 && ampliaciones == 0)//edicion
                            {
                                var disponible = acuerdo.Cantidad - cantidadTodoAcuerdo;
                                if (cantidad - cantidadOriginal > disponible)
                                {
                                    acuerdo.Cantidad += cantidad - cantidadOriginal.Value - disponible;
                                    acuerdo.CantidadAmpliado += cantidad - cantidadOriginal - disponible;
                                }
                            }
                            if (ampliaciones > 0)//ampliaciones
                            {
                                var disponible = acuerdo.Cantidad - cantidadTodoAcuerdo;
                                if (ampliaciones > disponible)
                                {
                                    acuerdo.Cantidad += ampliaciones.Value - disponible;
                                    acuerdo.CantidadAmpliado += ampliaciones - disponible;
                                }
                            }

                        }
                    }

                }

            }
        }

        public GrabarContratoResult PreAnularContrato(int contratoId, string motivo)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);

            if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }

            var res = status.ValidarEstado(oContratoSave.ContratoSAP);
            var estado = (string.IsNullOrEmpty(res.Status) && res.NumeroSio == 0) ? "" : "El contrato ya no se encuentra en slip o fue informado a SIO granos";
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado) && String.IsNullOrEmpty(estado))
            {
                try
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.PreAnulado;
                    oContratoSave.MotivoRechazo = motivo;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }
            }
            else
            {
                oEntityErrors.Error("", estado);
            }

            return oEntityErrors;

        }
        public GrabarContratoResult RechazarPreAnularContrato(int contratoId/*, string motivo*/)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);
            //if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            //{
            //    oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
            //    return oEntityErrors;
            //}
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.PreAnulado))
            {
                try
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                    //oContratoSave.MotivoRechazo = motivo;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());

                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }

            }

            return oEntityErrors;
        }
        public GrabarContratoResult AnularContratoPreAnulado(int contratoId, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);
            var res = status.ValidarEstado(oContratoSave.ContratoSAP);
            var estado = (string.IsNullOrEmpty(res.Status) && res.NumeroSio == 0) ? "" : "El contrato ya no se encuentra en slip o fue informado a SIO granos";
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.PreAnulado) && String.IsNullOrEmpty(estado))
            {
                var respuesta = oEliminarContratoAgent.Eliminar(oContratoSave);
                if (respuesta.Contains("Error"))
                {
                    if (respuesta.Contains("SIO"))
                    {
                        var administrativo = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailSio)));
                        EnviarMailSio(oContratoSave, administrativo, idActiveDirectory);
                    }
                    oEntityErrors.Error("", respuesta);
                }
                else
                {
                    try
                    {
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                        if (oContratoSave.ContratoAcuerdoId.HasValue)
                        {
                            var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoSave.ContratoAcuerdoId);
                            if (acuerdo != null && acuerdo.CantidadAmpliado > 0)
                            {
                                acuerdo.Cantidad -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                                acuerdo.CantidadAmpliado -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                            }
                        }
                        repositorio.GuardarCambios();
                        logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoId), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        oEntityErrors.Error("", e.Message);

                    }
                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.Id);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.NegocioId == oContratoSave.Id);
                    mobjProveedorManager.EnviarEmail(oContratoSave, objDescuento, objCalidad, idActiveDirectory, true);
                }
            }
            else
            {
                oEntityErrors.Error("", estado);
            }

            return oEntityErrors;
        }

        public GrabarContratoResult FinalizarContrato(int contratoId, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(a => a.Id == contratoId);

            if (oContratoSave != null && string.IsNullOrEmpty(oContratoSave.ContratoSAP) && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error))
            {
                Nullable<DateTime> fecha = null;

                var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(fecha);

                if (oContratoSave.Fecha < diaAnterior)
                {
                    oEntityErrors.Error("", "Fecha del contrato debe ser la de hoy o día hábil anterior");
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());

                    return oEntityErrors;
                }
                try
                {
                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.Id);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.NegocioId == oContratoSave.Id);
                    var objApertura = repositorio.Listar<AperturaPrecio>(x => x.NegocioId == oContratoSave.Id);

                    if (objApertura == null)
                    {
                        var conceptos = repositorio.Listar<ConceptoAperturaPrecio>();
                        oContratoSave.AperturaPrecio = new List<AperturaPrecio>();
                        foreach (ConceptoAperturaPrecio concepto in conceptos)
                        {
                            oContratoSave.AperturaPrecio.Add(new AperturaPrecio
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
                        oContratoSave.AperturaPrecio = objApertura;
                    }

                    string nroContratoSAP = SAPFinalizarContrato(oContratoSave, objDescuento, objCalidad);

                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Finalizado;


                    try
                    {
                        oContratoSave.ContratoSAP = nroContratoSAP;
                    }
                    catch (Exception e)
                    {
                        oContratoSave.ContratoSAP = "";
                        logger.Error(e);
                    }
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());
                    try
                    {
                        if (oContratoSave.EsFason != true && oContratoSave.TipoAgenteCompraId == null
                            && oContratoSave.Canje != true && oContratoSave.PrestamoDevolucion != true
                            && oContratoSave.Venta != true)
                        {
                            mobjProveedorManager.EnviarEmail(oContratoSave, objDescuento, objCalidad, idActiveDirectory, null);
                            var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.Comercial.ComercialId);
                            foreach (var comercialId in comerciales)
                            {
                                EnviarNotificacion(comercialId, oContratoSave);
                            }
                        }

                        if (oContratoSave.Venta.HasValue && oContratoSave.Venta != false)
                        {
                            EnviarMailVenta(oContratoSave, objDescuento, objCalidad, idActiveDirectory);
                        }
                        if (oContratoSave.Canje.HasValue && oContratoSave.Canje != false)
                        {
                            mobjProveedorManager.EnviarMailCanje(oContratoSave, objDescuento, objCalidad, idActiveDirectory);
                        }
                        if (oContratoSave.PrestamoDevolucion.HasValue && oContratoSave.PrestamoDevolucion != false)
                        {
                            mobjProveedorManager.EnviarMailPrestamoDevolucion(oContratoSave, objDescuento, objCalidad, idActiveDirectory);
                        }
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                }
                catch (Exception e)
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);
                }
            }
            else
            {
                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                {
                    oEntityErrors.Error("", "El contrato ya se encuentra Finalizado");
                }
                else if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El contrato ya ha sido Rechazado");
                }
                else if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente || oContratoSave.EstadoId == (int)EnumEstadoContrato.Oferta)
                {
                    oEntityErrors.Error("", "El contrato debe ser Confirmado");
                }
                if (!string.IsNullOrEmpty(oContratoSave.ContratoSAP) && oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El contrato ya tiene ContratoSAP asignado, por favor comunicarse con sistemas.");
                    negocioManager.EnviarMailErrorFinalizarNegocio(contratoId);
                }
                logger.Debug(" Error Intentando finalizar el contrato " + contratoId + " estado: " + oContratoSave.EstadoId);
            }
            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public GrabarContratoResult BorrarContrato(Contrato oContrato)
        {
            var tipoAccion = TipoAccionLogDataAgro.Eliminar;
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(oContrato.MotivoRechazo) || string.IsNullOrWhiteSpace(oContrato.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
            oContratoSave.MotivoRechazo = oContrato.MotivoRechazo;

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.ReconfirmarFinalizado) || (int)oContratoSave.EstadoId < (int)EnumEstadoContrato.Finalizado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
            {
                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar || oContratoSave.EstadoId == (int)EnumEstadoContrato.ReconfirmarFinalizado)
                {
                    if (oContratoSave.Ampliaciones > 0)
                    {
                        oContratoSave.Ampliaciones = 0;
                        if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar)
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                            tipoAccion = TipoAccionLogDataAgro.Crear;
                        }
                        else
                        {
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Pendiente;
                            tipoAccion = TipoAccionLogDataAgro.Modificar;
                        }
                    }
                    else
                    {
                        var historico = oContratoSave.NegocioHistorico.LastOrDefault();
                        if (historico != null)
                        {
                            tipoAccion = TipoAccionLogDataAgro.Modificar;

                            Contrato contratoOriginal = JsonConvert.DeserializeObject<Contrato>(historico.Datos);
                            oContratoSave.MaterialId = contratoOriginal.MaterialId;
                            oContratoSave.TipoNegocioId = contratoOriginal.TipoNegocioId;
                            oContratoSave.Cantidad = contratoOriginal.Cantidad;
                            oContratoSave.Precio = contratoOriginal.Precio;
                            oContratoSave.FechaEntrega = contratoOriginal.FechaEntrega;
                            oContratoSave.CampanaId = contratoOriginal.CampanaId;
                            oContratoSave.FechaDesde = contratoOriginal.FechaDesde;
                            oContratoSave.FechaHasta = contratoOriginal.FechaHasta;
                            oContratoSave.ProveedorId = contratoOriginal.ProveedorId;
                            oContratoSave.MonedaId = contratoOriginal.MonedaId;
                            oContratoSave.GrupoCompra = contratoOriginal.GrupoCompra;
                            oContratoSave.ComercialId = contratoOriginal.ComercialId;
                            oContratoSave.LocalidadId = contratoOriginal.LocalidadId;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.ProvinciaId = contratoOriginal.ProvinciaId;
                            oContratoSave.Base = contratoOriginal.Base;
                            oContratoSave.ImporteSustentable = contratoOriginal.ImporteSustentable;
                            oContratoSave.MonedaSustentableId = contratoOriginal.MonedaSustentableId;
                            oContratoSave.FechaDolarizado = contratoOriginal.FechaDolarizado;
                            oContratoSave.DiasPesificado = contratoOriginal.DiasPesificado;
                            oContratoSave.NoInformaSio = contratoOriginal.NoInformaSio;
                            oContratoSave.TrigoEspecial = contratoOriginal.TrigoEspecial;
                            oContratoSave.EstadoId = contratoOriginal.EstadoId;
                            oContratoSave.UsuarioId = contratoOriginal.UsuarioId;
                            oContratoSave.Ampliaciones = contratoOriginal.Ampliaciones;
                            oContratoSave.Observacion = contratoOriginal.Observacion;
                            oContratoSave.DestinoId = contratoOriginal.DestinoId;
                            oContratoSave.CantidadCamiones = contratoOriginal.CantidadCamiones;
                            oContratoSave.Consignatario = contratoOriginal.Consignatario;
                            oContratoSave.PlanCanje = contratoOriginal.PlanCanje;
                            oContratoSave.CondicionFijacionId = contratoOriginal.CondicionFijacionId;
                            oContratoSave.CD = contratoOriginal.CD;
                            oContratoSave.Warrant = contratoOriginal.Warrant;
                            oContratoSave.PagoDirectoVendedor = contratoOriginal.PagoDirectoVendedor;
                            oContratoSave.EstablecimientoPropio = contratoOriginal.EstablecimientoPropio != null ? contratoOriginal.EstablecimientoPropio : null;
                            oContratoSave.ClasificacionId = contratoOriginal.ClasificacionId;
                            oContratoSave.CantidadCamiones = contratoOriginal.CantidadCamiones;
                            oContratoSave.BoletoId = contratoOriginal.BoletoId;
                            oContratoSave.BolsaId = contratoOriginal.BolsaId == 0 ? null : contratoOriginal.BolsaId;
                            oContratoSave.DesdeFijacion = contratoOriginal.DesdeFijacion;
                            oContratoSave.HastaFijacion = contratoOriginal.HastaFijacion;
                            oContratoSave.MercsDeposito = contratoOriginal.MercsDeposito;
                            oContratoSave.ComercialCreadorId = contratoOriginal.ComercialCreadorId;
                            oContratoSave.CorredorId = contratoOriginal.CorredorId;
                            oContratoSave.PorcentajeComision = contratoOriginal.PorcentajeComision;
                            oContratoSave.ContratoVendedor = contratoOriginal.ContratoVendedor;
                            oContratoSave.ContratoCorredor = contratoOriginal.ContratoCorredor;
                            oContratoSave.SelCargoVendedor = contratoOriginal.SelCargoVendedor;
                            oContratoSave.SelCargoMOA = contratoOriginal.SelCargoMOA;
                            oContratoSave.Madre = contratoOriginal.Madre;
                            oContratoSave.ContratoMadre = contratoOriginal.ContratoMadre?.PadLeft(10, '0');
                            oContratoSave.PrecioNeto = contratoOriginal.PrecioNeto;
                            oContratoSave.StandardDeCalidadId = contratoOriginal.StandardDeCalidadId;
                            oContratoSave.Pizarra = contratoOriginal.Pizarra;
                            oContratoSave.PagoDiferido = contratoOriginal.PagoDiferido;
                            oContratoSave.ZonaId = contratoOriginal.ZonaId;
                            oContratoSave.Compensacion = contratoOriginal.Compensacion;
                            oContratoSave.TarifaFlete = contratoOriginal.TarifaFlete;
                            oContratoSave.NivelTarifaId = contratoOriginal.NivelTarifaId == 0 ? null : contratoOriginal.NivelTarifaId;
                            oContratoSave.Dolarizado = contratoOriginal.Dolarizado;
                            oContratoSave.Sustentable = contratoOriginal.Sustentable;
                            oContratoSave.FechaCierta = contratoOriginal.FechaCierta;
                            oContratoSave.ContratoSAP = contratoOriginal.ContratoSAP;
                            oContratoSave.ContratoAcuerdoId = oContrato.ContratoAcuerdoId;
                            oContratoSave.PagoCBU = oContrato.PagoCBU;
                            oContratoSave.ChequeElectronico = oContratoSave.ChequeElectronico;
                            oContratoSave.DolarizadoExpress = oContrato.DolarizadoExpress;
                            oContratoSave.Canje = oContrato.Canje;
                            oContratoSave.Monto = oContrato.Monto;
                            oContratoSave.MonedaCanjeId = oContrato.MonedaCanjeId;
                            oContratoSave.Insumo = oContrato.Insumo;
                            oContratoSave.PrestamoDevolucion = oContrato.PrestamoDevolucion;
                            oContratoSave.PlantaDestinoId = oContrato.PlantaDestinoId;
                            oContratoSave.ObligatoriedadCostoFinanciero = oContrato.FechaCierta.HasValue &&
                            oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                            oContrato.ObligatoriedadCostoFinanciero.HasValue && !oContrato.ObligatoriedadCostoFinanciero.Value
                            ? null : oContrato.FechaCierta.HasValue ? oContrato.ObligatoriedadCostoFinanciero : null;

                            //oContratoSave.ObligatoriedadBonificacion = oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 &&
                            //oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0)) &&
                            //oContrato.ObligatoriedadBonificacion.HasValue && !oContrato.ObligatoriedadBonificacion.Value
                            //? null : oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 ? oContrato.ObligatoriedadBonificacion : null;

                            oContrato.Venta = oContrato.Venta;


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

                            if (oContratoSave.Descuentos != null)
                            {
                                for (int i = oContratoSave.Descuentos.Count - 1; i > -1; i--)
                                {
                                    repositorio.Remover(oContratoSave.Descuentos.First());
                                }
                            }
                            else
                            {
                                oContratoSave.Descuentos = new List<DescuentoBonificacion>();
                            }

                            if (contratoOriginal.Descuentos != null)
                            {
                                foreach (var descuento in contratoOriginal.Descuentos)
                                {
                                    repositorio.Agregar(new DescuentoBonificacion { ContratoId = descuento.ContratoId, FechaDesde = descuento.FechaDesde, FechaHasta = descuento.FechaHasta, Importe = descuento.Importe, MonedaId = descuento.MonedaId, Porcentaje = descuento.Porcentaje, TipoDBId = descuento.TipoDBId, TipoPeriodoDBId = descuento.TipoPeriodoDBId });
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
                            if (oContratoSave.ContratoAcuerdoId.HasValue)
                            {
                                var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoSave.ContratoAcuerdoId);
                                if (acuerdo != null && acuerdo.CantidadAmpliado > 0)
                                {
                                    acuerdo.Cantidad -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                                    acuerdo.CantidadAmpliado -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                                }
                            }
                            oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                            tipoAccion = TipoAccionLogDataAgro.Eliminar;
                        }
                    }


                }
                else
                {
                    if (oContratoSave.ContratoAcuerdoId.HasValue)
                    {
                        var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoSave.ContratoAcuerdoId);
                        if (acuerdo != null && acuerdo.CantidadAmpliado > 0)
                        {
                            acuerdo.Cantidad -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                            acuerdo.CantidadAmpliado -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                        }
                    }
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                    tipoAccion = TipoAccionLogDataAgro.Eliminar;
                }

                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), tipoAccion, oContratoSave.GetType());

                var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.Comercial.ComercialId);
                try
                {
                    foreach (var comercialId in comerciales)
                    {
                        EnviarNotificacion(comercialId, oContratoSave);
                    }
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }


            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede rechazar");
            }
            return oEntityErrors;
        }

        private void EnviarNotificacion(int comercialId, Contrato contrato)
        {
            var tokens = repositorio.Listar<SuscripcionComercial>(x => x.ComercialId == comercialId);
            var title = "";
            var message = "";
            var hora = DateTime.Now.ToString("hh:mm");
            if (contrato.EstadoId == 2)
            {
                title = "Contrato Confirmado";
                message = "El contrato " + contrato.Id + " ha sido confirmado a las " + hora;
            }
            else if (contrato.EstadoId == 5)
            {
                title = "Contrato Finalizado";
                message = "El contrato " + contrato.ContratoSAP + " ha sido finalizado a las " + hora + " por " + contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido;
            }
            else if (contrato.EstadoId == 6)
            {
                title = "Contrato Rechazado";
                message = "El contrato " + contrato.Id + " ha sido rechazado a las " + hora;
            }
            var url = "/CompraNet";
            foreach (var to in tokens)
            {
                mobjNotification.QueueMessage(to.Key, title, message, url);
            }
        }

        private string SAPFinalizarContrato(Contrato contrato, List<DescuentoBonificacion> descuentoBonificacion, List<Calidad> calidad)
        {

            return oFinalizarContratoAgent.Finalizar(contrato, descuentoBonificacion, calidad);
        }

        public List<DescuentoBonificacionDto> TraerDescuentosPorContrato(int contratoId)
        {
            return repositorio.Listar<DescuentoBonificacion, DescuentoBonificacionDto>(desc => new DescuentoBonificacionDto()
            {
                ContratoId = desc.ContratoId,
                Id = desc.Id,
                FechaDesde = desc.FechaDesde != null ? SqlFunctions.DateName("day", desc.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)desc.FechaDesde.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", desc.FechaDesde) : "",

                FechaHasta = desc.FechaHasta != null ? SqlFunctions.DateName("day", desc.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)desc.FechaHasta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", desc.FechaHasta) : "",
                Importe = desc.Importe,
                Porcentaje = desc.Porcentaje,
                MonedaId = desc.MonedaId,
                Moneda = desc.MonedaId,
                TipoDBId = desc.TipoDBId,
                TipoDBDesc = desc.TipoDB.Descripcion,
                TipoPeriodoDBDesc = desc.TipoPeriodoDB.Descripcion,
                TipoPeriodoDBId = desc.TipoPeriodoDBId
            },
            x => x.ContratoId == contratoId);
        }
        public List<CalidadDto> TraerCalidadesPorContrato(int contratoId, int acuerdoId)
        {
            var c = contratoId == 0 ? (int?)null : contratoId;
            var a = acuerdoId == 0 ? (int?)null : acuerdoId;
            return repositorio.Listar<Calidad, CalidadDto>(cal => new CalidadDto()
            {
                ContratoId = cal.NegocioId,
                Id = cal.Id,
                CalidadEspecialDesc = cal.CalidadEspecial.Descripcion,
                CalidadEspecialId = cal.CalidadEspecialId,
                Valor = cal.Valor,
                PorcentajeDesde = cal.PorcentajeDesde,
                PorcentajeHasta = cal.PorcentajeHasta,
            },
            x => x.NegocioId == c || x.NegocioId == a);
        }
        public DatosContratoDto TraerDatosDeContrato(int contratoId)
        {
            return repositorio.Obtener<Contrato, DatosContratoDto>(x => x.Id == contratoId,
                x => new DatosContratoDto()
                {
                    Calidades = x.Calidad.Select(cal => new CalidadDto()
                    {
                        ContratoId = cal.NegocioId,
                        Id = cal.Id,
                        CalidadEspecialDesc = cal.CalidadEspecial.Descripcion,
                        CalidadEspecialId = cal.CalidadEspecialId,
                        Valor = cal.Valor,
                        PorcentajeDesde = cal.PorcentajeDesde,
                        PorcentajeHasta = cal.PorcentajeHasta,
                    }).ToList(),
                    DescuentosBonificaciones = x.Descuentos.Select(desc => new DescuentoBonificacionDto()
                    {
                        ContratoId = desc.ContratoId,
                        Id = desc.Id,
                        FechaDesde = desc.FechaDesde != null ? SqlFunctions.DateName("day", desc.FechaDesde).Trim() + "-" +
                                                SqlFunctions.StringConvert((double)desc.FechaDesde.Value.Month).TrimStart() + "-" +
                                                SqlFunctions.DateName("year", desc.FechaDesde) : "",

                        FechaHasta = desc.FechaHasta != null ? SqlFunctions.DateName("day", desc.FechaHasta).Trim() + "-" +
                                                SqlFunctions.StringConvert((double)desc.FechaHasta.Value.Month).TrimStart() + "-" +
                                                SqlFunctions.DateName("year", desc.FechaHasta) : "",
                        Importe = desc.Importe,
                        Porcentaje = desc.Porcentaje,
                        MonedaId = desc.MonedaId,
                        Moneda = desc.MonedaId,
                        TipoDBId = desc.TipoDBId,
                        TipoDBDesc = desc.TipoDB.Descripcion,
                        TipoPeriodoDBDesc = desc.TipoPeriodoDB.Descripcion,
                        TipoPeriodoDBId = desc.TipoPeriodoDBId
                    }).ToList(),
                    Precios = x.PrecioPactado.Select(pre => new PrecioPactadosDto
                    {
                        ContratoId = pre.ContratoId,
                        FechaDesde = pre.FechaDesde != null ? SqlFunctions.DateName("day", pre.FechaDesde).Trim() + "-" +
                                               SqlFunctions.StringConvert((double)pre.FechaDesde.Value.Month).TrimStart() + "-" +
                                               SqlFunctions.DateName("year", pre.FechaDesde) : "",
                        FechaHasta = pre.FechaHasta != null ? SqlFunctions.DateName("day", pre.FechaHasta).Trim() + "-" +
                                               SqlFunctions.StringConvert((double)pre.FechaHasta.Value.Month).TrimStart() + "-" +
                                               SqlFunctions.DateName("year", pre.FechaHasta) : "",
                        Id = pre.Id,
                        ImportePactado = pre.ImportePactado,
                        MonedaImportePactadoDesc = pre.MonedaImportePactado.Descripcion,
                        MonedaImportePactadoId = pre.MonedaImportePactadoId,
                        MonedaPactadoDesc = pre.MonedaPactado.Descripcion,
                        MonedaPactadoId = pre.MonedaPactadoId,
                        Porcentaje = pre.Porcentaje,
                        Precio = pre.Precio
                    }).ToList()
                });
        }
        public DatosContratoDto TraerDatosDeContratoAcuerdo(int contratoId)
        {
            return repositorio.Obtener<ContratoAcuerdo, DatosContratoDto>(x => x.Id == contratoId,
                x => new DatosContratoDto()
                {
                    Calidades = x.Calidad.Select(cal => new CalidadDto()
                    {
                        ContratoId = cal.NegocioId,
                        Id = cal.Id,
                        CalidadEspecialDesc = cal.CalidadEspecial.Descripcion,
                        CalidadEspecialId = cal.CalidadEspecialId,
                        Valor = cal.Valor,
                        PorcentajeDesde = cal.PorcentajeDesde,
                        PorcentajeHasta = cal.PorcentajeHasta,
                    }).ToList(),
                    DescuentosBonificaciones = x.Descuentos.Select(desc => new DescuentoBonificacionDto()
                    {
                        ContratoId = desc.ContratoId,
                        Id = desc.Id,
                        FechaDesde = desc.FechaDesde != null ? SqlFunctions.DateName("day", desc.FechaDesde).Trim() + "-" +
                                                SqlFunctions.StringConvert((double)desc.FechaDesde.Value.Month).TrimStart() + "-" +
                                                SqlFunctions.DateName("year", desc.FechaDesde) : "",

                        FechaHasta = desc.FechaHasta != null ? SqlFunctions.DateName("day", desc.FechaHasta).Trim() + "-" +
                                                SqlFunctions.StringConvert((double)desc.FechaHasta.Value.Month).TrimStart() + "-" +
                                                SqlFunctions.DateName("year", desc.FechaHasta) : "",
                        Importe = desc.Importe,
                        Porcentaje = desc.Porcentaje,
                        MonedaId = desc.MonedaId,
                        Moneda = desc.MonedaId,
                        TipoDBId = desc.TipoDBId,
                        TipoDBDesc = desc.TipoDB.Descripcion,
                        TipoPeriodoDBDesc = desc.TipoPeriodoDB.Descripcion,
                        TipoPeriodoDBId = desc.TipoPeriodoDBId
                    }).ToList(),
                    Precios = x.PrecioPactado.Select(y => new PrecioPactadosDto
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
                });
        }
        public BasicoContrato TraerContrato(int contratoId)
        {
            var contrato = repositorio.Obtener<Contrato, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {

                ContratoId = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                CorredorId = x.CorredorId ?? 0,
                Proveedor = x.Proveedor == null ? "" : x.Proveedor.RazonSocial + " " + "(" + x.Proveedor.CUIT + ")",
                Corredor = x.Corredor == null ? "" : x.Corredor.RazonSocial + " " + "(" + x.Corredor.CUIT + ")",
                ComercialId = x.ComercialId,
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                TipoNegocioId = x.TipoNegocioId,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Ampliaciones = x.Ampliaciones,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                CampanaId = x.CampanaId ?? 0,
                ProvinciaId = x.ProvinciaId,
                Provincia = x.Provincia.Nombre,
                LocalidadId = x.LocalidadId,
                Localidad = x.Localidad.Nombre,
                ContratoSAP = x.ContratoSAP,
                Base = x.Base,
                Observacion = x.Observacion,
                Estado = x.EstadoId,
                Estado_Contrato = x.Estado.Descripcion,
                Importe_Sustentable = x.ImporteSustentable,
                Moneda_Sustentable = x.MonedaSustentableId,
                Fecha_DolarizadoFormateado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                Dolarizado = x.Dolarizado,
                DolarizadoCorredor = x.DolarizadoCorredor,
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
                DesdeFijacionFormateado = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.DesdeFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                HastaFijacionFormateado = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.HastaFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.HastaFijacion) : "",
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                CondicionFijacion = x.CondicionFijacionId,
                CD = x.CD,
                Warrant = x.Warrant,
                PagoDirectoVendedor = x.PagoDirectoVendedor,
                EstablecimientoPropio = x.EstablecimientoPropio,
                MercsDeposito = x.MercsDeposito,
                PorcentajeComision = x.PorcentajeComision,
                ContratoCorredor = x.ContratoCorredor,
                ContratoVendedor = x.ContratoVendedor,
                SelCargoMOA = x.SelCargoMOA,
                SelCargoVendedor = x.SelCargoVendedor,
                Madre = x.Madre,
                EsFason = x.EsFason,
                ContratoMadre = x.ContratoMadre,
                Pizarra = x.Pizarra.HasValue ? x.Pizarra.Value : false,
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
                //ObligatoriedadBonificacion = x.ObligatoriedadBonificacion,
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
                    CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                    CalidadEspecialId = y.CalidadEspecialId,
                    PorcentajeDesde = y.PorcentajeDesde,
                    PorcentajeHasta = y.PorcentajeHasta,
                    Valor = y.Valor,
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
                FechaCiertaFormateado = x.FechaCierta != null ? SqlFunctions.DateName("day", x.FechaCierta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaCierta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaCierta) : "",
                PorcentajeDePago = x.PorcentajeDePago,
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                CaratulaExtension = x.CaratulaExtension,
                CaratulaMAT = x.CaratulaMAT,
                PrecioAjusteComision = x.PrecioAjusteComision,
                MonedaAjusteComisionId = x.MonedaAjusteComisionId,
                ContratoAcuerdoId = x.ContratoAcuerdoId,

                Id = x.Id,
                Material = x.Material == null ? "" : x.Material.Descripcion,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                Fecha = x.Fecha,
                Campania = x.CampanaId == null ? "" : x.Campana.Descripcion,
                DestinoDescripcion = x.DestinoId == null ? "" : x.Destino.Descripcion,
                TipoNegocio = x.TipoNegocio == null ? "" : x.TipoNegocio.Descripcion,
                CondicionFijacionDescripcion = x.CondicionFijacion.Descripcion,
                ClasificacionDescripcion = x.Clasificacion == null ? "" : x.Clasificacion.Descripcion,
                Comercial = x.Comercial == null ? "" : x.Comercial.Apellido + " " + x.Comercial.Nombres,
                FechaOperacion = x.FechaOperacion,
                FechaOperacionFormateado = SqlFunctions.DateName("day", x.FechaOperacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaOperacion.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaOperacion),
                MotivoOperacionAnterior = x.MotivoOperacionAnterior,
                ChequeElectronico = x.ChequeElectronico,
                DolarizadoExpress = x.DolarizadoExpress,
                PagoCBU = x.PagoCBU,
                CalidadTercero = x.CalidadTercero,
                DolarizadoTercero = x.DolarizadoTercero,
                PagoDiferidoTercero = x.PagoDiferidoTercero,
                ObservacionTercero = x.ObservacionTercero,
                Canje = x.Canje,
                Monto = x.Monto,
                MonedaCanjeId = x.MonedaCanjeId,
                Insumo = x.Insumo,
                PrestamoDevolucion = x.PrestamoDevolucion.HasValue ? x.PrestamoDevolucion.Value : false,
                PlantaDestinoId = x.PlantaDestinoId.HasValue ? x.PlantaDestinoId.Value : 0,
                PlantaDestinoDescripcion = !x.PlantaDestinoId.HasValue ? "" : x.Destino.Descripcion,
                SustentableTercero = x.SustentableTercero,
                Venta = x.Venta,
                FechaDesde_Sustentable = x.FechaDesdeSustentable,
                FechaDesde_SustentableFormateado = x.FechaDesdeSustentable != null ? SqlFunctions.DateName("day", x.FechaDesdeSustentable).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesdeSustentable.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesdeSustentable) : "",
                FechaHasta_Sustentable = x.FechaHastaSustentable,
                FechaHasta_SustentableFormateado = x.FechaHastaSustentable != null ? SqlFunctions.DateName("day", x.FechaHastaSustentable).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHastaSustentable.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHastaSustentable) : "",
                PosicionCBOT = x.PosicionCBOT,
                TipoPosicionCBOTId = x.TipoPosicionCBOTId,
                TipoPosicionCBOT = x.TipoPosicionCBOT.Descripcion,
                ProveedorCreador = x.ProveedorCreadorId,
                UsuarioId = x.UsuarioId,
                UsuarioTercero = x.UsuarioTercero,
                FechaCierta = x.FechaCierta,
                Fecha_Dolarizado = x.FechaDolarizado,
                AnulaYReemplazaContratoId = x.AnulaYReemplazaContratoId,
                AnulaYReemplazaContratoSAP = x.AnulaYReemplazaContrato.ContratoSAP,
                MotivoReemplazo = x.MotivoReemplazo,
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
                CreditoDisponible = x.CreditoDisponible,
                Cesion = x.Cesion
            });
            return contrato;
        }

        public void EnviarMailPendiente()
        {
            var hoy = DateTime.Now.Date;
            var contratosPendientes = repositorio.Listar<Negocio, AvisoContratoDto>(x => new AvisoContratoDto
            {
                ContratoId = x.Id,
                RazonSocial = x.Proveedor.RazonSocial,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                Moneda = x.MonedaId,
                FechaDb = x.Fecha,
                ComercialCreadorAD = x.ComercialCreadorId.HasValue ? x.ComercialCreador.IdActiveDirectory : x.Comercial.IdActiveDirectory,
                NombreApellido = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => (x.EstadoId == 1 || x.EstadoId == 3) && x.Fecha < hoy && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2 || x.TipoNegocioId == 3) && x.Canje != true && x.PrestamoDevolucion != null);
            var comercialesMesa = repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, IdActiveDirectory = x.IdActiveDirectory }, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.NotificacionesMailTodos)));
            var mailComercialesMesa = new List<string>();
            foreach (var mesa in comercialesMesa)
            {
                try
                {
                    mailComercialesMesa.Add(mailManager.GetEmailUserActiveDirectory(mesa.IdActiveDirectory));
                }
                catch (Exception e)
                {
                    logger.Error(e);
                }

            }
            if (contratosPendientes != null)
            {
                string emailComercial = "";
                foreach (var contratosPorCreador in contratosPendientes.GroupBy(x => x.ComercialCreadorAD))
                {
                    try
                    {

                        logger.Debug("Numero de Contratos Pendientes " + contratosPorCreador.ToList().Count + " del comercial " + contratosPorCreador.Key);
                        var oMensaje = new MailMessage
                        {
                            From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                        };
                        if (contratosPendientes != null)
                        {
                            try
                            {
                                emailComercial = mailManager.GetEmailUserActiveDirectory(contratosPorCreador.Key);
                                if (!string.IsNullOrEmpty(emailComercial))
                                {
                                    oMensaje.To.Add(emailComercial);
                                }
                            }
                            catch (Exception e)
                            {
                                logger.Error(e);
                            }
                        }
                        if (mailComercialesMesa != null)
                        {
                            foreach (var mesa in mailComercialesMesa)
                            {
                                if (!string.IsNullOrEmpty(mesa) && mesa != emailComercial)
                                {
                                    oMensaje.To.Add(mesa);
                                }
                            }
                        }

                        oMensaje.CC.Add(new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"]));
                        var rutaMolinos = httpContextManager.ObtenerPathLogoMail();
                        oMensaje.AlternateViews.Add(CuerpoMailContrato(rutaMolinos, contratosPorCreador.ToList(), contratosPorCreador.Key));
                        oMensaje.Subject = "Negocios Pendientes CompraNet";

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
            }
        }
        private AlternateView CuerpoMailContrato(string filePath, List<AvisoContratoDto> contratosPendientes, string idActiveDirectory)
        {
            var nombreApellido = repositorio.Obtener<Comercial, string>(x => x.IdActiveDirectory == idActiveDirectory, x => x.Nombres + " " + x.Apellido);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th = "<th style=\"border: 1px solid white; color: white; text-align:center; background-color: #017940; padding: 5px 0; width: 175px;\">";
            string td = "<td style=\"border: 1px solid white; color: black; text-align:center; background-color: #a4e751; padding: 5px 0; width: 175px;\">";
            string htmlBody = "";
            htmlBody += "En el presente mail, se detallan los negocios a Confirmar creados por el comercial " + nombreApellido + ": <br /><br />  ";
            htmlBody += "<table><tr>" + th + "PROVEEDOR</th>" + th + "CANTIDAD</th>" + th + "PRECIO</th>" + th + "FECHA DE CARGA</th>" + th + "COMERCIAL</th></tr>";
            foreach (var contrato in contratosPendientes)
            {
                htmlBody += "<tr>";
                htmlBody += td + contrato.RazonSocial + "</td>";
                htmlBody += td + contrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")) + "</td>";
                htmlBody += td + contrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + contrato.Moneda + "</td>";
                htmlBody += td + contrato.Fecha + "</td>";
                htmlBody += td + contrato.NombreApellido + "</td>";
                htmlBody += "</tr>";
            }
            htmlBody += "</table>";
            htmlBody += "<br /><br /> Por favor revisarlos a la brevedad." +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void FinalizacionAutomatica(string idActiveDirectory)
        {
            var contratosConfirmados = repositorio.Listar<Contrato, int>(a => a.Id, x => x.EstadoId == 2 || x.EstadoId == 4);
            logger.Debug("Contratos a Finalizar: " + contratosConfirmados.Count);
            var oEntityErrors = new GrabarContratoResult();
            foreach (var id in contratosConfirmados)
            {
                try
                {
                    var error = FinalizarContrato(id, idActiveDirectory);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                    oEntityErrors.Error("", ex.Message);
                }
            }
        }

        public void BorradoAutomatico()
        {
            var contratosPendientes = repositorio.Listar<Contrato>(x => x.EstadoId == 1 || x.EstadoId == 3);
            logger.Debug("Contratos Pendientes: " + contratosPendientes.Count);
            var diasOperables = oDiasHabilesAgent.ObtenerDiasHabiles();
            var ultimosDiasOperable = DateTime.Today;
            var i = 0;
            while (i < 1 || !diasOperables.Contains(ultimosDiasOperable))
            {
                ultimosDiasOperable = ultimosDiasOperable.AddDays(-1);
                if (diasOperables.Contains(ultimosDiasOperable))
                {
                    i++;
                }

            }
            var borrados = 0;
            foreach (var contrato in contratosPendientes)
            {
                if (contrato.Fecha.Date < ultimosDiasOperable)
                {
                    try
                    {
                        BorrarContrato(contrato);
                        borrados++;
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                    }
                }
            }
            logger.Debug("Contratos eliminados: " + borrados);
        }

        public List<AvisoContratoDto> TraerContratosPendientes(List<int> equipo)
        {
            var fechaHoy = DateTime.Now.Date;
            return repositorio.Listar<Negocio, AvisoContratoDto>(x => new AvisoContratoDto
            {
                ContratoId = x.Id,
                RazonSocial = x.Proveedor.RazonSocial,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                Moneda = x.MonedaId,
                FechaDb = x.Fecha,
                ComercialCreadorAD = x.ComercialCreadorId.HasValue ? x.ComercialCreador.IdActiveDirectory : x.Comercial.IdActiveDirectory,
                NombreApellido = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => (x.EstadoId == 1 || x.EstadoId == 3) &&
            equipo.Contains(x.Comercial.ComercialId) &&
            x.Fecha < fechaHoy &&
            (x.TipoNegocioId == 1 || x.TipoNegocioId == 2 || x.TipoNegocioId == 3));
        }

        public DatosCompraNetDto TraerDatosCompraNet(int id)
        {
            var compranet = repositorio.Obtener<Proveedor, DatosCompraNetDto>(x => x.ProveedorId == id, x => new DatosCompraNetDto()
            {
                ProveedorId = id,
                BoletoCompraNetId = x.BoletoCompraNetId,
                BolsaCompraNetId = x.BolsaCompraNetId,
                ClasificacionCompraNetId = x.ClasificacionCompraNetId,
                Consignatario = x.Consignatario,
                LocalidadId = x.LocalidadCompraNetId,
                ProvinciaId = x.ProvinciaCompraNetId,
                Localidad = x.LocalidadCompraNet.Nombre,
                Provincia = x.ProvinciaCompraNet.Nombre,
                ComisionPorcentaje = x.ComisionPorcentaje,
                PlanCanje = x.PlanCanje
            });
            return compranet;
        }

        public ContratoResult TraerContratoMadre(string sap)
        {
            sap = sap.PadLeft(10, '0');
            var result = new ContratoResult();
            var idContrato = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == sap && x.Madre == true, x => x.Id);
            if (idContrato != 0)
            {
                result.Contrato = TraerContrato(idContrato);
                return result;
            }
            else
            {
                result.Error("", "No existe Contrato Madre");
                return result;
            }
        }
        public GrabarContratoResult AnularContrato(Contrato oContrato, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(oContrato.MotivoRechazo) || string.IsNullOrWhiteSpace(oContrato.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
            oContratoSave.MotivoRechazo = oContrato.MotivoRechazo;
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado))
            {
                var respuesta = oEliminarContratoAgent.Eliminar(oContratoSave);
                if (respuesta.Contains("Error"))
                {
                    if (respuesta.Contains("SIO"))
                    {
                        var administrativo = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailSio)));
                        EnviarMailSio(oContratoSave, administrativo, idActiveDirectory);
                    }
                    oEntityErrors.Error("", respuesta);
                }
                else
                {
                    try
                    {
                        if (oContratoSave.ContratoAcuerdoId.HasValue)
                        {
                            var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oContratoSave.ContratoAcuerdoId);
                            if (acuerdo != null && acuerdo.CantidadAmpliado > 0)
                            {
                                acuerdo.Cantidad -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                                acuerdo.CantidadAmpliado -= oContratoSave.Cantidad > acuerdo.CantidadAmpliado ? acuerdo.CantidadAmpliado.Value : oContratoSave.Cantidad;
                            }
                        }
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                        repositorio.GuardarCambios();
                        logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        oEntityErrors.Error("", e.Message);

                    }
                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.Id);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.NegocioId == oContratoSave.Id);
                    mobjProveedorManager.EnviarEmail(oContratoSave, objDescuento, objCalidad, idActiveDirectory, true);
                }
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede eliminar");
            }
            return oEntityErrors;
        }

        private void EnviarMailSio(Contrato oContrato, List<Comercial> administrativo, string idActiveDirectory)
        {
            try
            {
                var emailComercial = new List<string>();

                if (administrativo != null)
                {
                    foreach (var com in administrativo)
                    {
                        try { emailComercial.Add(mailManager.GetEmailUserActiveDirectory(com.IdActiveDirectory)); }
                        catch (Exception e) { logger.Error(e); }
                    }
                }

                var oMensaje = new MailMessage
                {
                    From = new MailAddress(ConfigurationManager.AppSettings["CredentialUserName"])
                };

                if (emailComercial.Count > 0)
                {
                    foreach (var adm in emailComercial)
                    {
                        oMensaje.To.Add(adm);
                    }
                }
                else
                {
                    logger.Debug($"No existen Administrativos para Informar SIO");
                    return;
                }
                oMensaje.CC.Add(ConfigurationManager.AppSettings["CredentialUserName"]);

                oMensaje.AlternateViews.Add(CuerpoMailSIO(httpContextManager.ObtenerPathLogoMail(), oContrato, idActiveDirectory));
                if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
                {
                    oMensaje.Subject = "Anulacion de Contrato Molinos Agro S.A. - " + oContrato.Proveedor.RazonSocial;
                }
                else
                {
                    oMensaje.Subject = "Mail Pruebas - Anulacion de Contrato Molinos Agro S.A. - " + oContrato.Proveedor.RazonSocial;
                }
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
        private AlternateView CuerpoMailSIO(string filePath, Contrato contrato, string idActiveDirectory)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            Comercial comercial = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == idActiveDirectory);
            string htmlBody = "";
            htmlBody += "Por el presente mail, se solicita anular el contrato " + contrato.ContratoSAP.TrimStart('0') + " de  SIO Granos <br /><br />  ";
            htmlBody += "<br /><br /> Por favor anularlos a la brevedad y comunicarse con " + comercial.Nombres + " " + comercial.Apellido +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<ContratoCopiar> TraerContratosPorSap(string nrocontratoSap)
        {
            return repositorio.ListarConsulta(new DevolverContratos(nrocontratoSap));
        }

        public List<ContratoCopiar> TraerContratosAcuerdo(string filtro)
        {
            return repositorio.ListarConsulta(new DevolverContratosAcuerdo(filtro));
        }


        public BasicoContrato TraerContratoAcuerdoACopiar(int contratoId)
        {
            var hoy = DateTime.Now.Date;
            var contrato = repositorio.Obtener<ContratoAcuerdo, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                ContratoId = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                Proveedor = x.Proveedor == null ? "" : x.Proveedor.RazonSocial + " " + "(" + x.Proveedor.CUIT + ")",
                CorredorId = x.CorredorId ?? 0,
                Corredor = x.Corredor == null ? "" : x.Corredor.RazonSocial + " " + "(" + x.Corredor.CUIT + ")",
                ComercialId = x.ComercialCreadorId,
                Fecha = x.Fecha,
                FechaOperacion = x.Fecha,
                FechaOperacionFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                MotivoOperacionAnterior = DbFunctions.TruncateTime(x.Fecha) != hoy ? "Acuerdo " + x.Id : "",
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                FechaCiertaFormateado = x.FechaCierta.HasValue ? SqlFunctions.DateName("day", x.FechaCierta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaCierta.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaCierta) : "",
                TipoNegocioId = x.Precio > 0 ? 2 : 1,
                HastaFijacion = x.HastaFijacion,
                DesdeFijacion = x.DesdeFijacion,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                DesdeFijacionFormateado = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.DesdeFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                HastaFijacionFormateado = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.HastaFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.HastaFijacion) : "",
                CondicionFijacionDescripcion = x.CondicionFijacion.Descripcion,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Ampliaciones = null,
                Precio = x.Precio,
                PrecioNeto = x.PrecioNeto,
                MonedaId = x.MonedaId,
                CampanaId = x.CampanaId ?? x.Material.CampaniaTableroId ?? x.Material.CampañaId ?? 0,
                ProvinciaId = x.Proveedor.ProvinciaCompraNetId,
                Provincia = x.Proveedor.ProvinciaCompraNet.Nombre,
                LocalidadId = x.Proveedor.LocalidadCompraNetId,
                Localidad = x.Proveedor.LocalidadCompraNet.Nombre,
                ContratoSAP = "",
                Base = null,
                Observacion = "",
                Estado = x.EstadoId,
                Importe_Sustentable = 0,
                Moneda_Sustentable = "",
                NoInformaSIO = null,
                TrigoEspecial = null,
                ClasificacionId = x.Proveedor.ClasificacionCompraNetId,
                DestinoId = x.DestinoId,
                PlanCanje = null,
                Consignatario = x.Proveedor.Consignatario,
                CantidadCamiones = 0,
                BoletoId = x.Proveedor.BoletoCompraNetId,
                BolsaId = x.Proveedor.BolsaCompraNetId,
                CondicionFijacion = x.CondicionFijacionId,
                PagoDirectoVendedor = null,
                EstablecimientoPropio = null,
                MercsDeposito = null,
                PorcentajeComision = null,
                ContratoCorredor = "",
                ContratoVendedor = "",
                SelCargoMOA = null,
                SelCargoVendedor = null,
                Madre = null,
                ContratoMadre = null,
                FechaCierta = x.FechaCierta,
                ObligatoriedadCostoFinanciero = x.ObligatoriedadCostoFinanciero,
                //ObligatoriedadBonificacion = x.ObligatoriedadBonificacion,
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
                StandardCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                Dolarizado = x.Dolarizado == true || x.DolarizadoCorredor == true,
                PagoDiferido = x.PagoDiferido,
                Fecha_DolarizadoFormateado = x.FechaDolarizado.HasValue ? SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                Fecha_Dolarizado = x.FechaDolarizado,
                Dias_Pesificado = x.DiasPesificado,
                CD = x.CD,
                Warrant = x.Warrant,
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
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                ChequeElectronico = x.ChequeElectronico,
                PagoCBU = x.PagoCBU
            });
            Nullable<DateTime> fecha = null;

            var dia = oDiasHabilesAgent.UltimoDiaHabil(fecha);
            if (contrato.Fecha < dia)
            {
                return new BasicoContrato();
            }
            else
            {
                return contrato;
            }
        }

        public List<AperturaPrecioDto> TraerAperturaDePrecioPorContrato(int contratoId)
        {
            var lista = repositorio.Listar<AperturaPrecio, AperturaPrecioDto>(x => new AperturaPrecioDto
            {
                contratoId = x.NegocioId,
                Id = x.Id,
                ConceptoAperturaPrecio = x.ConceptoAperturaPrecio.Descripcion,
                ConceptoAperturaPrecioId = x.ConceptoAperturaPrecioId,
                Importe = x.Importe,
                MonedaId = x.MonedaId,
                Moneda = x.Moneda.Descripcion,
                Porcentaje = x.Porcentaje
            },
            x => x.NegocioId == contratoId);
            return lista;
        }
        public TotalPesosDolares TraerTotalesPesosDolares(DataSourceRequest request, List<int> listComercialesId, List<int> corredoresComercial)
        {
            var resultados = repositorio.ObtenerConsultaEscalar(new TraerTotalesPesosDolares(request, listComercialesId, corredoresComercial));

            return resultados;
        }

        public List<EstadoContratoDto> TraerTodoLosEstados()
        {
            return repositorio.Listar<EstadoContrato, EstadoContratoDto>(x => new EstadoContratoDto
            {
                EstadoContratoId = x.EstadoContratoId,
                Descripcion = x.Descripcion
            });
        }

        public List<BoletoCompraNetDto> TraerTodosLosBoletos()
        {
            return repositorio.Listar<BoletoCompraNet, BoletoCompraNetDto>(x => new BoletoCompraNetDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion
            });
        }

        public List<GrupoDeComprasDto> TraerTodoGrupoDeCompras()
        {
            return repositorio.Listar<GrupoDeCompras, GrupoDeComprasDto>(x => new GrupoDeComprasDto
            {
                Id = x.Id,
                Descripcion = x.Descripcion
            });
        }
        public AltaTempranaNRCODto ValidarProveedor(int proveedorId)
        {
            var proveedor = repositorio.Obtener<Proveedor>(proveedorId);
            return altaTempranaAgent.ObtenerAlta(proveedor.CUIT);
        }

        public Resultado ActualizarContratoSAP(Contrato contrato, bool validacionesMinimas)
        {
            var error = new Resultado();
            logger.Debug("Actualizando contrato en BD DataAgro: " + contrato.Id);
            var contratoSave = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contrato.ContratoSAP);
            if (contratoSave == null || contratoSave.Id == 0)
            {
                error.Error("Contrato", "No existe contrato en DataAgro");
                return error;
            }
            contrato.TipoNegocioId = contratoSave.TipoNegocioId;
            contrato.ComercialId = contratoSave.ComercialId;
            Validar(contrato, error, validacionesMinimas);

            if (error.Errores.Count > 0)
            {
                return error;
            }
            contratoSave.MaterialId = contrato.MaterialId;
            contratoSave.Cantidad = contrato.Cantidad;
            contratoSave.Precio = contrato.Precio;
            contratoSave.FechaEntrega = contrato.FechaEntrega;
            contratoSave.CampanaId = contrato.CampanaId;
            contratoSave.FechaDesde = contrato.FechaDesde;
            contratoSave.FechaHasta = contrato.FechaHasta;
            contratoSave.ProveedorId = contrato.ProveedorId;
            contratoSave.MonedaId = contrato.MonedaId;
            contratoSave.LocalidadId = contrato.LocalidadId;
            contratoSave.ProvinciaId = contrato.ProvinciaId;
            contratoSave.Base = contrato.Base;
            contratoSave.ImporteSustentable = contrato.ImporteSustentable;
            contratoSave.MonedaSustentableId = contrato.MonedaSustentableId;
            contratoSave.FechaDolarizado = contrato.FechaDolarizado;
            contratoSave.Dolarizado = contrato.Dolarizado;
            contratoSave.DolarizadoCorredor = contrato.DolarizadoCorredor;
            contratoSave.DiasPesificado = contrato.DiasPesificado;
            contratoSave.PagoDiferido = contrato.PagoDiferido;
            contratoSave.NoInformaSio = contrato.NoInformaSio;
            contratoSave.TrigoEspecial = contrato.TrigoEspecial;
            contratoSave.Ampliaciones = contrato.Ampliaciones;
            contratoSave.Observacion = contrato.Observacion;
            contratoSave.DestinoId = contrato.DestinoId;
            contratoSave.CantidadCamiones = contrato.CantidadCamiones;
            contratoSave.Consignatario = contrato.Consignatario;
            contratoSave.PlanCanje = contrato.PlanCanje;
            contratoSave.CondicionFijacionId = contrato.CondicionFijacionId;
            contratoSave.CD = contrato.CD;
            contratoSave.Warrant = contrato.Warrant;
            contratoSave.PagoDirectoVendedor = contrato.PagoDirectoVendedor;
            contratoSave.EstablecimientoPropio = contrato.EstablecimientoPropio != null ? contrato.EstablecimientoPropio : null;
            contratoSave.ClasificacionId = contrato.ClasificacionId;
            contratoSave.CantidadCamiones = contrato.CantidadCamiones;
            contratoSave.BoletoId = contrato.BoletoId;
            contratoSave.BolsaId = contrato.BolsaId == 0 ? null : contrato.BolsaId;
            contratoSave.DesdeFijacion = contrato.DesdeFijacion;
            contratoSave.HastaFijacion = contrato.HastaFijacion;
            contratoSave.MercsDeposito = contrato.MercsDeposito;
            contratoSave.CorredorId = contrato.CorredorId;
            contratoSave.PorcentajeComision = contrato.PorcentajeComision;
            contratoSave.ContratoVendedor = contrato.ContratoVendedor;
            contratoSave.ContratoCorredor = contrato.ContratoCorredor;
            contratoSave.SelCargoVendedor = contrato.SelCargoVendedor;
            contratoSave.SelCargoMOA = contrato.SelCargoMOA;
            contratoSave.ContratoMadre = contrato.ContratoMadre?.PadLeft(10, '0');
            contratoSave.PrecioNeto = contrato.PrecioNeto;
            contratoSave.StandardDeCalidadId = contrato.StandardDeCalidadId;
            contratoSave.Pizarra = contrato.Pizarra;
            contratoSave.PagoDiferido = contrato.PagoDiferido;
            contratoSave.ZonaId = contrato.ZonaId;
            contratoSave.Compensacion = contrato.Compensacion;
            contratoSave.TarifaFlete = contrato.TarifaFlete;
            contratoSave.NivelTarifaId = contrato.NivelTarifaId == 0 ? null : contrato.NivelTarifaId;
            contratoSave.FechaCierta = contrato.FechaCierta;
            contratoSave.EstadoId = contrato.EstadoId;
            contratoSave.TipoAgenteCompraId = contrato.TipoAgenteCompraId;
            contratoSave.CaratulaMAT = contrato.CaratulaMAT;
            contratoSave.CaratulaExtension = contrato.CaratulaExtension;
            contratoSave.PrecioAjusteComision = contrato.PrecioAjusteComision;
            contratoSave.MonedaAjusteComisionId = contrato.MonedaAjusteComisionId;
            contratoSave.FechaCierta = contrato.FechaCierta;
            contratoSave.ChequeElectronico = contrato.ChequeElectronico;
            contratoSave.DolarizadoExpress = contrato.DolarizadoExpress;
            contratoSave.PagoCBU = contrato.PagoCBU;
            contratoSave.PlantaDestinoId = contrato.PlantaDestinoId;

            var calidades = repositorio.Listar<Calidad>(x => x.NegocioId == contratoSave.Id);
            repositorio.RemoverTodos(calidades);
            var descuentos = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == contratoSave.Id);
            repositorio.RemoverTodos(descuentos);
            var aperturas = repositorio.Listar<AperturaPrecio>(x => x.NegocioId == contratoSave.Id);
            repositorio.RemoverTodos(aperturas);
            var precios = repositorio.Listar<PrecioPactado>(x => x.ContratoId == contratoSave.Id);
            repositorio.RemoverTodos(precios);

            contratoSave.Calidad = contrato.Calidad;
            contratoSave.Descuentos = contrato.Descuentos;
            contratoSave.AperturaPrecio = contrato.AperturaPrecio;
            contratoSave.PrecioPactado = contrato.PrecioPactado;
            contratoSave.PorcentajeDePago = contrato.PorcentajeDePago;
            contratoSave.PosicionCBOT = contrato.PosicionCBOT;
            contratoSave.TipoPosicionCBOTId = contrato.TipoPosicionCBOTId;
            contratoSave.Cesion = contrato.Cesion;

            // no se deberian poder modificar
            //contratoSave.GrupoCompra = contrato.GrupoCompra;
            //contratoSave.Canje = contrato.Canje;
            //contratoSave.PrestamoDevolucion = contrato.PrestamoDevolucion;
            //contratoSave.Monto = contrato.Monto;
            //contratoSave.MonedaCanjeId = contrato.MonedaCanjeId;
            //contratoSave.Insumo = contrato.Insumo;

            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoSave.Id), TipoAccionLogDataAgro.Modificar, contratoSave.GetType());

            return error;
        }

        public GrabarContratoResult ActualizarContratoFinalizado(Contrato oContrato)
        {
            var error = new GrabarContratoResult();
            try
            {
                Validar(oContrato, error, false);

                if (error.Errores.Count > 0)
                {
                    return error;
                }

                oContrato.DolarizadoCorredor = false;
                if (oContrato.CorredorId.HasValue && oContrato.Dolarizado == true)
                {
                    oContrato.Dolarizado = false;
                    oContrato.DolarizadoCorredor = true;
                }

                var oContratoSave = new Contrato();
                List<DescuentoBonificacion> descuentosExistentes = null;
                List<Calidad> calidadesExistentes = null;
                List<AperturaPrecio> aperturasExistentes = null;
                List<PrecioPactado> preciosExistentes = null;

                oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
                descuentosExistentes = oContratoSave.Descuentos.ToList();
                calidadesExistentes = oContratoSave.Calidad.ToList();
                aperturasExistentes = oContratoSave.AperturaPrecio.ToList();
                preciosExistentes = oContratoSave.PrecioPactado.ToList();
                oContrato.ContratoSAP = oContratoSave.ContratoSAP;
                var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.ProveedorId, x => x.CUIT);
                oContratoSave.MonedaCreditoDisponible = validarCreditoAgente.ValidarCredito(cuit).Moneda;
                if (oContratoSave.EstadoId == 6)
                {
                    error.Error("", "El contrato no se puede modificar");
                    return error;
                }
                if ((oContrato.ChequeElectronico != oContratoSave.ChequeElectronico && oContrato.ChequeElectronico == true) || oContratoSave.PagoCBU != oContrato.PagoCBU)
                {
                    var result = validarPagoAgente.ValidarEstado(oContrato.ContratoSAP, "");
                    if (result != "Ok")
                    {
                        error.Error("", result);
                        return error;
                    }
                }
                if (oContrato.EstadoId != 11)
                {
                    if ((oContratoSave.Precio != oContrato.Precio || oContratoSave.Cantidad != oContrato.Cantidad || oContratoSave.MonedaId != oContrato.MonedaId) && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3) || ValidarCalidadModificada(oContrato, oContratoSave))
                    {
                        oContrato.EstadoId = 11;
                        if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                        {
                            string jsonContrato = JsonConvert.SerializeObject(oContratoSave, new JsonSerializerSettings()
                            {
                                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                                PreserveReferencesHandling = PreserveReferencesHandling.Objects
                            });
                            oContratoSave.NegocioHistorico.Add(new NegocioHistorico { Datos = jsonContrato, Fecha = DateTime.Now, NegocioId = oContrato.Id, TipoNegocioId = oContrato.TipoNegocioId, ComercialId = oContrato.ComercialId });
                        }

                    }
                    else
                    {

                        oContrato.ContratoSAP = repositorio.Obtener<Contrato, string>(x => x.Id == oContrato.Id, x => x.ContratoSAP);

                        var res = modificarContratoAgent.Modificar(oContrato, oContratoSave);
                        if (res.Contains("Error"))
                        {
                            error.Error("SAP", res);
                            return error;
                        }
                        logger.Debug("Actualizacion SAP ok");


                    }
                }
                var listaErrores = ActualizarContratoSAP(oContrato, false);
                error.Errores.AddRange(listaErrores.Errores);
            }
            catch (Exception e)
            {
                logger.Error(e);
                error.Error("", e.Message + ".");
            }
            return error;
        }
        public GrabarContratoResult ReconfirmarFinalizado(int contratoId, string comercialRegistrado)
        {
            var error = new GrabarContratoResult();
            try
            {
                var json = repositorio.Listar<NegocioHistorico>(x => x.NegocioId == contratoId).LastOrDefault().Datos;
                var contratoSave = JsonConvert.DeserializeObject<Contrato>(json);
                var contrato = repositorio.Obtener<Contrato>(contratoId);
                var res = modificarContratoAgent.Modificar(contrato, contratoSave);

                if (res.Contains("Error"))
                {
                    error.Error("SAP", res);
                    return error;
                }
                else
                {
                    contrato.EstadoId = 5;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(contrato.Id), TipoAccionLogDataAgro.Modificar, contrato.GetType());
                    if (contrato.Canje != true && contrato.PrestamoDevolucion != true && contrato.Venta != true)
                    {
                        EnviarMail(contrato, contratoSave, comercialRegistrado);
                    }
                }
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                error.Error("", e.Message);
                throw;
            }
            return error;
        }
        public bool ValidarCalidadModificada(Contrato contrato, Contrato contratoGuardado)
        {
            var calModificado = false;

            if ((contrato.Calidad == null && contratoGuardado.Calidad.Count > 0) || contrato.Calidad != null && contratoGuardado.Calidad.Count != contrato.Calidad.Count)
            {
                calModificado = true;
            }
            else
            {
                foreach (var cal in contratoGuardado.Calidad)
                {
                    if (!contrato.Calidad.Any(x => x.Valor == cal.Valor
                    && x.StandardDeCalidadId == cal.StandardDeCalidadId
                    && x.CalidadEspecialId == cal.CalidadEspecialId
                    && x.PorcentajeDesde == cal.PorcentajeDesde && x.PorcentajeHasta == cal.PorcentajeHasta))
                    {
                        calModificado = true;
                        break;
                    }
                }
            }
            if (contrato.StandardDeCalidadId != contratoGuardado.StandardDeCalidadId)
            {
                calModificado = true;
            }
            return calModificado;
        }
        public List<ContratoIdDto> TraerContratosSAP(string desde, string hasta)
        {
            var desdeId = !string.IsNullOrEmpty(desde) ? repositorio.Obtener<Contrato, int>(x => x.ContratoSAP.Contains(desde), x => x.Id) : 0;
            var hastaId = !string.IsNullOrEmpty(hasta) ? repositorio.Obtener<Contrato, int>(x => x.ContratoSAP.Contains(hasta), x => x.Id) :
                repositorio.ObtenerMayor<Contrato, int, int>(x => true, x => x.Id, x => x.Id);
            return repositorio.Listar<Contrato, ContratoIdDto>(x => new ContratoIdDto { ContratoId = x.Id, ContratoSAP = x.ContratoSAP }
            , x => x.Id >= desdeId && x.Id <= hastaId && !string.IsNullOrEmpty(x.ContratoSAP));
        }
        public RangoPrecioDto ObtenerRangoDePrecios(int materialId, string monedaId)
        {
            return repositorio.Obtener<RangoPrecio, RangoPrecioDto>(x => x.MaterialId == materialId && x.MonedaId == monedaId,
                x => new RangoPrecioDto
                {
                    PrecioMaximo = x.PrecioMaximo,
                    PrecioMinimo = x.PrecioMinimo
                });
        }

        public EstadoSAPDto ValidarStatus(int contratoId)
        {
            var resultado = new EstadoSAPDto();
            var contrato = repositorio.Obtener<Contrato, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato()
            {
                ContratoSAP = x.ContratoSAP,
                Estado = x.EstadoId
            });
            if (contrato != null)
            {
                if (contrato.Estado == 5)
                {
                    resultado = status.ValidarEstado(contrato.ContratoSAP);
                    return resultado;

                }
            }
            return resultado;
        }

        private void EnviarMail(Contrato contrato, Contrato contratoSave, string comercial)
        {
            var lista = new List<string>();
            var email = "";
            if (contrato.Comercial.IdActiveDirectory != comercial && !PermisosHelper.Is(PermisosDataAgro.NoRecibirMail))
            {
                email = mailManager.GetEmailUserActiveDirectory(contrato.Comercial.IdActiveDirectory);
                lista.Add(email);
            }
            var comercialRegistrado = mailManager.GetEmailUserActiveDirectory(comercial);
            lista.Add(comercialRegistrado);
            var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == (contrato.CorredorId != null ? contrato.CorredorId : contrato.ProveedorId));
            logger.Debug("Enviando mail a Comercial " + email);
            logger.Debug("Enviando mail a Comercial Registrado " + comercialRegistrado);
            var emailComerciales = "";

            var tienePermiso = repositorio.Obtener<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)) && x.ComercialId == contrato.ComercialCreadorId) != null ? true : false;
            logger.Debug("Usuario tiene permiso " + tienePermiso);
            if (tienePermiso)
            {
                var corredoresComerciales = mobjComercialManager.ListarComercialesCorredor();
                corredoresComerciales.Remove(contrato.Comercial);
                corredoresComerciales.Remove(contrato.ComercialCreador);
                var sinMail = mobjComercialManager.ListarComercialesSinRecibirMail();
                foreach (var item in sinMail)
                {
                    corredoresComerciales.Remove(item);
                }
                logger.Debug("Enviando mail a " + string.Join(", ", corredoresComerciales.Select(x => x.IdActiveDirectory)));
                foreach (Comercial corredorComercialCopia in corredoresComerciales)
                {
                    try
                    {
                        emailComerciales = mailManager.GetEmailUserActiveDirectory(corredorComercialCopia.IdActiveDirectory);
                        logger.Debug("Mail encontrado para " + emailComerciales + "  " + corredorComercialCopia.IdActiveDirectory);
                        if (!String.IsNullOrEmpty(emailComerciales))
                        {

                            lista.Add(emailComerciales);
                        }
                    }
                    catch (Exception e) { logger.Error(e); }
                }
            }
            var subject = "Modificación negocio Molinos Agro S.A. – " + (contrato.Corredor != null ? contrato.Corredor.RazonSocial : contrato.Proveedor.RazonSocial);

            mailManager.EnviarMail(contrato.Comercial, emailproveedor, subject, "", lista, CuerpoMailContrato(httpContextManager.ObtenerPathLogoMail(), contrato, contratoSave));
        }

        private AlternateView CuerpoMailContrato(String filePath, Contrato oContrato, Contrato contratoSave)
        {
            var emailComercial = mailManager.GetEmailUserActiveDirectory(oContrato.Comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            string th1;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
                th1 = "<th style =\"border: 2px solid white; color: white; background-color: #F62459; padding: 5px 0; width: 175px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
                th1 = "<th style =\"border: 2px solid white; color: white; background-color: #C93756; padding: 5px 0; width: 175px;\">";
            }
            var linea = 0;
            string htmlBody = "";
            htmlBody += "En el presente mail, se detalla modificaciones en el negocio con Molinos Agro S.A: <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";

            htmlBody += "<tr  style=\"background-color: #FFB3A7;\">" + th + "FECHA</th>" + Td(ref linea);
            if (oContrato.ContratoAcuerdoId != null)
            {
                htmlBody += Split(oContrato.ContratoAcuerdo.Fecha.ToShortDateString()) + "</td></tr>";
            }
            else
            {
                htmlBody += Split(oContrato.Fecha.ToShortDateString()) + "</td></tr>";
            }
            htmlBody += "<tr> " + th + "GRANO</th>" + Td(ref linea) + oContrato.Material.Descripcion.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "CONTRATO</th>" + Td(ref linea) + Split(oContrato.ContratoSAP.TrimStart('0')) + "</td></tr>";
            if (oContrato.DestinoId != null)
            {
                if (oContrato.DestinoId != contratoSave.DestinoId)
                {
                    htmlBody += "<tr>" + th1 + "DESTINO</th>" + TdCambio(ref linea) + oContrato.Destino.Descripcion.ToUpper() + "</td></tr>";
                }
                else
                {
                    htmlBody += "<tr>" + th + "DESTINO</th>" + Td(ref linea) + oContrato.Destino.Descripcion.ToUpper() + "</td></tr>";
                }
            }
            if (oContrato.Proveedor.RazonSocial != contratoSave.Proveedor.RazonSocial)
            {
                htmlBody += "<tr>" + th1 + "PROVEEDOR</th>" + TdCambio(ref linea) + oContrato.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "PROVEEDOR</th>" + Td(ref linea) + oContrato.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            }
            if (oContrato.Proveedor.CUIT != contratoSave.Proveedor.CUIT)
            {
                htmlBody += "<tr>" + th1 + "CUIT</th>" + TdCambio(ref linea) + Split(oContrato.Proveedor.CUIT.ToString()) + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "CUIT</th>" + Td(ref linea) + Split(oContrato.Proveedor.CUIT.ToString()) + "</td></tr>";
            }
            if (oContrato.Corredor != null)
            {
                if (oContrato.Corredor.RazonSocial != contratoSave.Corredor.RazonSocial)
                {
                    htmlBody += "<tr>" + th1 + "CORREDOR</th>" + TdCambio(ref linea) + oContrato.Corredor.RazonSocial.ToUpper() + "</td></tr>";
                }
                {
                    htmlBody += "<tr>" + th + "CORREDOR</th>" + Td(ref linea) + oContrato.Corredor.RazonSocial.ToUpper() + "</td></tr>";
                }
                if (oContrato.Corredor.CUIT != contratoSave.Corredor.CUIT)
                {
                    htmlBody += "<tr>" + th1 + "CUIT CORREDOR</th>" + TdCambio(ref linea) + Split(oContrato.Corredor.CUIT.ToString()) + "</td></tr>";

                }
                else
                {
                    htmlBody += "<tr>" + th + "CUIT CORREDOR</th>" + Td(ref linea) + Split(oContrato.Corredor.CUIT.ToString()) + "</td></tr>";
                }
            }
            if (oContrato.Clasificacion.Descripcion != contratoSave.Clasificacion.Descripcion)
            {
                htmlBody += "<tr>" + th1 + "FIGURA</th>" + TdCambio(ref linea) + oContrato.Clasificacion.Descripcion.ToUpper();
            }
            else
            {
                htmlBody += "<tr>" + th + "FIGURA</th>" + Td(ref linea) + oContrato.Clasificacion.Descripcion.ToUpper();
            }
            if (oContrato.Consignatario == true)
            {
                htmlBody += " CONSIG";
            }

            htmlBody += "</td></tr>";

            if (oContrato.Cantidad != contratoSave.Cantidad)
            {
                htmlBody += "<tr>" + th1 + "CANTIDAD</th>" + TdCambio(ref linea) + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))
               + " Kg.";
            }
            else
            {
                htmlBody += "<tr>" + th + "CANTIDAD</th>" + Td(ref linea) + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))
                               + " Kg.";
            }
            if (oContrato.CantidadCamiones != null)
            {
                htmlBody += " (" + oContrato.CantidadCamiones + " camiones)<br />";
            }
            htmlBody += "</td></tr>";


            if (oContrato.TipoNegocioId == 2)
            {
                if (oContrato.Pizarra.HasValue && oContrato.Pizarra.Value)
                {
                    if (!contratoSave.Pizarra.HasValue)
                    {
                        htmlBody += "<tr>" + th1 + "PRECIO</th>" + TdCambio(ref linea) + "Pizarra</td></tr>";
                    }
                    else
                    {
                        htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea) + "Pizarra</td></tr>";
                    }
                }
                else
                {
                    if (oContrato.PrecioNeto.HasValue)
                    {
                        if (!contratoSave.PrecioNeto.HasValue || (oContrato.PrecioNeto != contratoSave.PrecioNeto))
                        {
                            htmlBody += "<tr>" + th1 + "PRECIO</th>" + TdCambio(ref linea) + Split(oContrato.PrecioNeto.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                        }
                        else
                        {
                            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea) + Split(oContrato.PrecioNeto.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                        }
                    }
                    else
                    {
                        if (oContrato.Precio != contratoSave.Precio)
                        {
                            htmlBody += "<tr>" + th1 + "PRECIO</th>" + TdCambio(ref linea) + Split(oContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                        }
                        else
                        {
                            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea) + Split(oContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                        }
                    }
                }
            }
            else if (oContrato.TipoNegocioId == 1)
            {
                if (oContrato.HastaFijacion != contratoSave.HastaFijacion)
                {
                    htmlBody += "<tr>" + th1 + "PRECIO</th>" + TdCambio(ref linea) + "A FIJAR HASTA: <br />" + Split(oContrato.HastaFijacion.Value.ToShortDateString()) + "<br />" + Split(oContrato.CondicionFijacion.Descripcion.ToUpper()) + "</td></tr>";
                }
                else
                {
                    htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea) + "A FIJAR HASTA: <br />" + Split(oContrato.HastaFijacion.Value.ToShortDateString()) + "<br />" + Split(oContrato.CondicionFijacion.Descripcion.ToUpper()) + "</td></tr>";
                }
            }
            if (oContrato.PorcentajeDePago != contratoSave.PorcentajeDePago)
            {
                htmlBody += "<tr>" + th1 + "PORCENTAJE DE PAGO</th>" + TdCambio(ref linea) + oContrato.PorcentajeDePago.ToString() + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "PORCENTAJE DE PAGO</th>" + Td(ref linea) + oContrato.PorcentajeDePago.ToString() + "</td></tr>";
            }
            if (oContrato.Localidad.Nombre != contratoSave.Localidad.Nombre)
            {
                htmlBody += "<tr>" + th1 + "PROCEDENCIA</th>" + TdCambio(ref linea) + oContrato.Localidad.Nombre.ToUpper() + " - " + oContrato.Provincia.Nombre.ToUpper() + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "PROCEDENCIA</th>" + Td(ref linea) + oContrato.Localidad.Nombre.ToUpper() + " - " + oContrato.Provincia.Nombre.ToUpper() + "</td></tr>";
            }
            if (oContrato.FechaDesde != contratoSave.FechaDesde)
            {
                htmlBody += "<tr>" + th1 + "ENT. DESDE</th>" + TdCambio(ref linea) + Split(oContrato.FechaDesde.ToShortDateString()) + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "ENT. DESDE</th>" + Td(ref linea) + Split(oContrato.FechaDesde.ToShortDateString()) + "</td></tr>";
            }
            if (oContrato.FechaHasta != contratoSave.FechaHasta)
            {
                htmlBody += "<tr>" + th1 + "ENT. HASTA</th>" + TdCambio(ref linea) + Split(oContrato.FechaHasta.ToShortDateString()) + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "ENT. HASTA</th>" + Td(ref linea) + Split(oContrato.FechaHasta.ToShortDateString()) + "</td></tr>";
            }
            if (oContrato.Campana.Descripcion != contratoSave.Campana.Descripcion)
            {
                htmlBody += "<tr>" + th1 + "COSECHA</th>" + TdCambio(ref linea) + oContrato.Campana.Descripcion.ToUpper() + "</td></tr>";
            }
            else
            {
                htmlBody += "<tr>" + th + "COSECHA</th>" + Td(ref linea) + oContrato.Campana.Descripcion.ToUpper() + "</td></tr>";
            }
            if (!String.IsNullOrEmpty(oContrato.ContratoMadre))
            {
                if (!String.IsNullOrEmpty(contratoSave.ContratoMadre) && oContrato.ContratoMadre != contratoSave.ContratoMadre)
                {
                    htmlBody += "<tr>" + th + "CONTRATO MADRE</th>" + Td(ref linea) + oContrato.ContratoMadre.TrimStart('0').ToUpper() + "</td></tr>";
                }
                else
                {
                    htmlBody += "<tr>" + th + "CONTRATO MADRE</th>" + Td(ref linea) + oContrato.ContratoMadre.TrimStart('0').ToUpper() + "</td></tr>";
                }
            }

            if (oContrato.BoletoId != null && oContrato.BoletoId != 3)
            {
                if (oContrato.Boleto.Descripcion != contratoSave.Boleto.Descripcion || oContrato.Bolsa.Descripcion != contratoSave.Bolsa.Descripcion)
                {
                    htmlBody += "<tr>" + th1 + "BOLETO</th>" + TdCambio(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + " " + oContrato.Bolsa.Descripcion.ToUpper() + "</td></tr>";
                }
                else
                {
                    htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + " " + oContrato.Bolsa.Descripcion.ToUpper() + "</td></tr>";
                }
            }
            else if (oContrato.BoletoId == 3)
            {
                if (oContrato.Boleto.Descripcion != contratoSave.Boleto.Descripcion)
                {
                    htmlBody += "<tr>" + th1 + "BOLETO</th>" + TdCambio(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + "</td></tr>";
                }
                else
                {
                    htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + "</td></tr>";
                }
            }
            var modificado = ValidarCalidadModificada(oContrato, contratoSave);

            if (modificado)
            {
                htmlBody += "<tr>" + th1 + "OBSERVACIÓN</th>" + TdCambio(ref linea);
            }
            else
            {
                htmlBody += "<tr>" + th + "OBSERVACIÓN</th>" + Td(ref linea);
            }

            if (oContrato.TipoNegocioId == 1)
            {
                if (oContrato.Cantidad < 30000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
                else if (oContrato.Cantidad >= 30000 && oContrato.Cantidad <= 100000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
                else if (oContrato.Cantidad >= 100000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split((oContrato.Cantidad + (30 * oContrato.Cantidad) / 100).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
            }
            if (oContrato.EstablecimientoPropio == true)
            {
                htmlBody += "ESTABLECIMIENTO PROPIO<br />";
            }
            else if (oContrato.EstablecimientoPropio == false)
            {
                htmlBody += "ESTABLECIMIENTO ARRENDADO<br />";
            }
            if (oContrato.ImporteSustentable != null)
            {
                htmlBody += "SUSTENTABLE " + oContrato.ImporteSustentable + " " + oContrato.MonedaSustentable.Descripcion.ToUpper() + "<br />";
            }
            if (oContrato.ClasificacionId == 1 && oContrato.CorredorId == null && oContrato.Dolarizado.Value)
            {
                htmlBody += "DOLARIZADO MÍNIMO 30 DÍAS<br />";
            }
            if (oContrato.FechaDolarizado != null)
            {
                htmlBody += "FECHA DOLARIZADO " + Split(oContrato.FechaDolarizado.Value.ToShortDateString()) + "<br />";
            }
            if (oContrato.DiasPesificado != null)
            {
                htmlBody += "PAGO DIFERIDO <br />";
                htmlBody += "DÍAS DE DIFERIMIENTO " + oContrato.DiasPesificado + "<br />";
            }
            if (oContrato.CD == true)
            {
                htmlBody += "PAGO CD<br />";
            }
            else if (oContrato.Warrant == true)
            {
                htmlBody += "PAGO WARRANT<br />";
            }
            if (oContrato.PagoDirectoVendedor == true)
            {
                htmlBody += "PAGO DIRECTO<br /> ";
            }
            if (oContrato.MercsDeposito == true)
            {
                htmlBody += "MERCADERIA EN DEPOSITO<br /> ";
            }
            foreach (var desc in oContrato.Descuentos)
            {
                if (desc.Importe > 0 || desc.Porcentaje > 0)
                {
                    htmlBody += "BONIFICACIONES " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                }
                else if (desc.Importe < 0 || desc.Porcentaje < 0)
                {
                    htmlBody += "DESCUENTOS " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                }
                if (desc.Importe != 0)
                {
                    htmlBody += desc.Importe + " " + desc.Moneda.Descripcion + "<br />";
                }

                if (desc.Porcentaje != 0)
                {
                    htmlBody += desc.Porcentaje + "%<br />";
                }
            }

            if (oContrato.StandardDeCalidadId == 7)
            {
                htmlBody += "CALIDAD GRADO 2<br />";
            }
            else if (oContrato.TrigoEspecial == true)
            {
                htmlBody += "CALIDAD ESPECIAL ";
            }
            else if (oContrato.StandardDeCalidadId == 1 || oContrato.StandardDeCalidadId == 4 || oContrato.StandardDeCalidadId == 5)
            {
                htmlBody += "CALIDAD CÁMARA ";
            }
            else if (oContrato.StandardDeCalidadId == 3)
            {
                htmlBody += "CALIDAD FÁBRICA ";
            }
            if (oContrato.Calidad != null && oContrato.StandardDeCalidadId != 7)
            {
                foreach (var cal in oContrato.Calidad)
                {
                    htmlBody += cal.CalidadEspecial.Descripcion.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "<br />";
                    if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                    {
                        htmlBody += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "%<br />";
                    }
                }
            }
            if (oContrato.PrecioPactado != null && oContrato.PrecioPactado.Count > 0)
            {
                htmlBody += "PRECIO PACTADO <br />";
                foreach (var precio in oContrato.PrecioPactado)
                {
                    htmlBody += "Precio " + precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + " " + precio.MonedaPactado.Descripcion.ToUpper() + "<br />";
                    if (precio.ImportePactado != null)
                    {
                        htmlBody += "Importe Pactado " + precio.ImportePactado.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + " " + precio.MonedaImportePactado.Descripcion.ToUpper() + "<br />";
                    }
                    if (precio.Porcentaje != null)
                    {
                        htmlBody += "Porcentaje Pactado " + precio.Porcentaje.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "%<br />";
                    }
                }
            }
            if (oContrato.PlanCanje == true)
            {
                htmlBody += "PLAN CANJE" + "<br />";
            }
            if (oContrato.ContratoVendedor != null)
            {
                htmlBody += "Contrato Vendedor: " + oContrato.ContratoVendedor + "<br />";
            }
            if (oContrato.ContratoCorredor != null)
            {
                htmlBody += "Contrato Corredor: " + oContrato.ContratoCorredor + "<br />";
            }
            if (oContrato.SelCargoMOA == true)
            {
                htmlBody += " Sellado 100% a Cargo MOA " + "<br />";
            }
            if (oContrato.SelCargoVendedor == true)
            {
                htmlBody += " Sellado 100% a Cargo vendedor " + "<br />";
            }
            if (oContrato.Compensacion == true)
            {
                htmlBody += " NEGOCIO COMPENSACIÓN " + "<br />";
            }
            if ((oContrato.NivelTarifa != null) && oContrato.TarifaFlete > 0)
            {
                htmlBody += " NIVEL DE TARIFA: " + oContrato.NivelTarifa.Descripcion.ToUpper() + "<br />" +
                    "TARIFA DE FLETE: " + oContrato.TarifaFlete + "<br />";
            }
            if (oContrato.Observacion != null)
            {
                htmlBody += oContrato.Observacion + "<br />";
            }
            if (oContrato.FechaCierta.HasValue)
            {
                htmlBody += "Fecha Cierta: " + oContrato.FechaCierta.Value.ToString("dd/MM/yyyy") + "<br />";
            }
            if (oContrato.DolarizadoExpress == true)
            {
                htmlBody += "A pesificar en mes en curso mediante envió de mail a materias.primas@molinosagro.com.ar hasta las 13 hs. <br />";
            }
            if (oContrato.ChequeElectronico == true)
            {
                htmlBody += "Pago con Echeq <br />";
            }
            if (oContrato.PagoCBU != null)
            {
                htmlBody += "Pago con Cbu: " + oContrato.PagoCBU + " <br />";
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /><br /> En el caso que sea necesario, comuníquese con  " + oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        private string Split(string str)
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
        private string Td(ref int linea)
        {
            string td1 = "";
            string td2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #bba7da; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #dccdea; padding: 5px 0; width: 250px;\">";
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

        private string TdCambio(ref int linea)
        {
            string td1 = "";
            string td2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #F62459; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #F62459; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td style=\"border: 2px solid white; color:#017940; background-color: #C93756; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:#017940; background-color: #C93756; padding: 5px 0; width: 250px;\">";
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

        public List<BasicoContrato> CompararNegocioReconfirmado(int contratoId)
        {
            try
            {
                var json = repositorio.Listar<NegocioHistorico>(x => x.NegocioId == contratoId).LastOrDefault().Datos;
                var contratoSave = JsonConvert.DeserializeObject<Contrato>(json);
                Expression<Func<Contrato, BasicoContrato>> proyeccion = x => new BasicoContrato
                {
                    MonedaId = x.MonedaId,
                    Cantidad = x.Cantidad,
                    Precio = x.Precio,
                    StandardCalidadId = x.StandardDeCalidadId,
                    StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                    Calidades = x.Calidad.Select(y => new CalidadDto
                    {
                        Id = y.Id,
                        CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                        CalidadEspecialId = y.CalidadEspecialId,
                        PorcentajeDesde = y.PorcentajeDesde,
                        PorcentajeHasta = y.PorcentajeHasta,
                        Valor = y.Valor,
                    }).ToList(),

                };
                var contrato = repositorio.Obtener<Contrato, BasicoContrato>(x => x.Id == contratoId, proyeccion);
                var basicoSave = TransformarABasicoContrato(contratoSave);
                var contratos = new List<BasicoContrato>();
                contratos.Add(contrato);
                contratos.Add(basicoSave);
                return contratos;

            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        private BasicoContrato TransformarABasicoContrato(Contrato x)
        {
            return new BasicoContrato
            {
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                StandardCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,

                Calidades = x.Calidad != null ? x.Calidad.Select(y => new CalidadDto
                {
                    Id = y.Id,
                    CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                    CalidadEspecialId = y.CalidadEspecialId,
                    PorcentajeDesde = y.PorcentajeDesde,
                    PorcentajeHasta = y.PorcentajeHasta,
                    Valor = y.Valor,
                }).ToList() : new List<CalidadDto>()
            };
        }

        public bool DiferenciaEnCalidades(int contratoId)
        {
            var json = repositorio.Listar<NegocioHistorico>(x => x.NegocioId == contratoId).LastOrDefault().Datos;
            var contratoSave = JsonConvert.DeserializeObject<Contrato>(json);
            var contrato = repositorio.Obtener<Contrato>(contratoId);
            return ValidarCalidadModificada(contratoSave, contrato);
        }
        public Resultado AnularContratoSAP(ContratoSAP contrato)
        {
            logger.Debug("Inicializar AnularContratoSAP");

            var oEntityErrors = new Resultado();
            var codigo = contrato.CodigoSap.PadLeft(10, '0');
            var oContratoSave = repositorio.ObtenerMayor<Contrato, int>(x => x.ContratoSAP == codigo, x => x.Id);

            string jsonObjeto = JsonConvert.SerializeObject(contrato, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
                PreserveReferencesHandling = PreserveReferencesHandling.None,
                Formatting = Formatting.Indented,
            });
            logger.Debug("Campos a editar: " + jsonObjeto);

            if (oContratoSave != null)
            {
                logger.Debug("Contrato: " + oContratoSave.ContratoSAP);
                double cantidadContrato = double.TryParse(contrato.Cantidad, out cantidadContrato) ? cantidadContrato : 0;
                try
                {
                    var cantidadKg = (double)oContratoSave.Cantidad + cantidadContrato;
                    var tipo = TipoAccionLogDataAgro.Eliminar;
                    if (cantidadKg <= 0)
                    {
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                        oContratoSave.Cantidad = cantidadKg;
                    }
                    else
                    {
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                        oContratoSave.Cantidad = cantidadKg;
                        tipo = TipoAccionLogDataAgro.Modificar;
                    }

                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), tipo, oContratoSave.GetType());
                }
                catch (Exception e)
                {
                    logger.Error("Error Anular ContratoSap");
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);

                }
            }
            else
            {
                oEntityErrors.Errores.Add(new ErrorMessage("No existe el contrato"));

            }
            return oEntityErrors;
        }

        public List<PagoCBUDto> ListarCBU(string cuitProveedor, string filtro)
        {
            return cbuAgent.ListarCBU(cuitProveedor, filtro);
        }

        public Resultado AltaContratoSAP(Contrato contratoSap, bool validacionesMinimas)
        {
            var error = new Resultado();
            logger.Debug("Alta contrato en BD DataAgro: " + contratoSap.Id);
            var contrato = new Contrato();
            try
            {

                Validar(contratoSap, error, validacionesMinimas);

                if (error.Errores.Count > 0)
                {
                    return error;
                }
                contrato.MaterialId = contratoSap.MaterialId;
                contrato.Cantidad = contratoSap.Cantidad;
                contrato.Precio = contratoSap.Precio;
                contrato.FechaEntrega = contratoSap.FechaEntrega;
                contrato.CampanaId = contratoSap.CampanaId;
                contrato.FechaDesde = contratoSap.FechaDesde;
                contrato.FechaHasta = contratoSap.FechaHasta;
                contrato.ProveedorId = contratoSap.ProveedorId;
                contrato.MonedaId = contratoSap.MonedaId;
                contrato.GrupoCompra = contratoSap.GrupoCompra;
                contrato.LocalidadId = contratoSap.LocalidadId;
                contrato.ProvinciaId = contratoSap.ProvinciaId;
                contrato.Base = contratoSap.Base;
                contrato.ImporteSustentable = contratoSap.ImporteSustentable;
                contrato.MonedaSustentableId = contratoSap.MonedaSustentableId;
                contrato.FechaDolarizado = contratoSap.FechaDolarizado;
                contrato.Dolarizado = contratoSap.Dolarizado;
                contrato.DolarizadoCorredor = contratoSap.DolarizadoCorredor;
                contrato.DiasPesificado = contratoSap.DiasPesificado;
                contrato.PagoDiferido = contratoSap.PagoDiferido;
                contrato.NoInformaSio = contratoSap.NoInformaSio;
                contrato.TrigoEspecial = contratoSap.TrigoEspecial;
                contrato.Ampliaciones = contratoSap.Ampliaciones;
                contrato.Observacion = contratoSap.Observacion;
                contrato.DestinoId = contratoSap.DestinoId;
                contrato.CantidadCamiones = contratoSap.CantidadCamiones;
                contrato.Consignatario = contratoSap.Consignatario;
                contrato.PlanCanje = contratoSap.PlanCanje;
                contrato.CondicionFijacionId = contratoSap.CondicionFijacionId;
                contrato.CD = contratoSap.CD;
                contrato.Warrant = contratoSap.Warrant;
                contrato.PagoDirectoVendedor = contratoSap.PagoDirectoVendedor;
                contrato.EstablecimientoPropio = contratoSap.EstablecimientoPropio != null ? contratoSap.EstablecimientoPropio : null;
                contrato.ClasificacionId = contratoSap.ClasificacionId;
                contrato.CantidadCamiones = contratoSap.CantidadCamiones;
                contrato.BoletoId = contratoSap.BoletoId;
                contrato.BolsaId = contratoSap.BolsaId == 0 ? null : contratoSap.BolsaId;
                contrato.DesdeFijacion = contratoSap.DesdeFijacion;
                contrato.HastaFijacion = contratoSap.HastaFijacion;
                contrato.MercsDeposito = contratoSap.MercsDeposito;
                contrato.CorredorId = contratoSap.CorredorId;
                contrato.PorcentajeComision = contratoSap.PorcentajeComision;
                contrato.ContratoVendedor = contratoSap.ContratoVendedor;
                contrato.ContratoCorredor = contratoSap.ContratoCorredor;
                contrato.SelCargoVendedor = contratoSap.SelCargoVendedor;
                contrato.SelCargoMOA = contratoSap.SelCargoMOA;
                contrato.ContratoMadre = contratoSap.ContratoMadre?.PadLeft(10, '0');
                contrato.PrecioNeto = contratoSap.PrecioNeto;
                contrato.StandardDeCalidadId = contratoSap.StandardDeCalidadId;
                contrato.Pizarra = contratoSap.Pizarra;
                contrato.PagoDiferido = contratoSap.PagoDiferido;
                contrato.ZonaId = contratoSap.ZonaId;
                contrato.Compensacion = contratoSap.Compensacion;
                contrato.TarifaFlete = contratoSap.TarifaFlete;
                contrato.NivelTarifaId = contratoSap.NivelTarifaId == 0 ? null : contratoSap.NivelTarifaId;
                contrato.FechaCierta = contratoSap.FechaCierta;
                contrato.EstadoId = contratoSap.EstadoId;
                contrato.TipoAgenteCompraId = contratoSap.TipoAgenteCompraId;
                contrato.CaratulaMAT = contratoSap.CaratulaMAT;
                contrato.CaratulaExtension = contratoSap.CaratulaExtension;
                contrato.PrecioAjusteComision = contratoSap.PrecioAjusteComision;
                contrato.MonedaAjusteComisionId = contratoSap.MonedaAjusteComisionId;
                contrato.ChequeElectronico = contratoSap.ChequeElectronico;
                contrato.PagoCBU = contratoSap.PagoCBU;
                contrato.DolarizadoExpress = contratoSap.DolarizadoExpress;
                contrato.Calidad = contratoSap.Calidad;
                contrato.Descuentos = contratoSap.Descuentos;
                contrato.AperturaPrecio = contratoSap.AperturaPrecio;
                contrato.PrecioPactado = contratoSap.PrecioPactado;
                contrato.PorcentajeDePago = contratoSap.PorcentajeDePago;
                contrato.FechaOperacion = contratoSap.FechaOperacion;
                contrato.Fecha = contratoSap.Fecha;
                contrato.TipoNegocioId = contratoSap.TipoNegocioId;
                contrato.ContratoSAP = contratoSap.ContratoSAP;
                contrato.UsuarioId = contratoSap.UsuarioId;
                contrato.ComercialId = contratoSap.ComercialId;
                contrato.ComercialCreadorId = contratoSap.ComercialCreadorId;
                contrato.EsFason = contratoSap.EsFason;
                contrato.Madre = contratoSap.Madre;
                contrato.FechaCierta = contratoSap.FechaCierta;
                contrato.Canje = contratoSap.Canje;
                contrato.Monto = contratoSap.Monto;
                contrato.MonedaCanjeId = contratoSap.MonedaCanjeId;
                contrato.Insumo = contratoSap.Insumo;
                contrato.PrestamoDevolucion = contratoSap.PrestamoDevolucion;
                contrato.PlantaDestinoId = contratoSap.PlantaDestinoId;
                contrato.MotivoOperacionAnterior = contratoSap.MotivoOperacionAnterior;
                contrato.PosicionCBOT = contratoSap.PosicionCBOT;
                contrato.TipoPosicionCBOTId = contratoSap.TipoPosicionCBOTId;
                contrato.Cesion = contratoSap.Cesion;

                repositorio.Agregar(contrato);
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
            }
            catch (Exception e)
            {
                logger.Error("Error Alta Contrato Sap ");
                logger.Error(e);

            }
            return error;
        }

        //public GrabarFijacionResult ActualizarFijacion(FijacionDePrecioContrato oContrato)
        //{
        //    var error = new GrabarFijacionResult();
        //    try
        //    {
        //        var oContratoSave = repositorio.Obtener<FijacionDePrecioContrato>(oContrato.Id);
        //        oContrato.ContratoSAP = oContratoSave.ContratoSAP;



        //        if ((oContrato.ChequeElectronico != oContratoSave.ChequeElectronico && oContrato.ChequeElectronico.Value) || oContratoSave.PagoCBU != oContrato.PagoCBU)
        //        {
        //            var result = validarPagoAgente.ValidarEstado(oContrato.ContratoSAP, "");
        //            if (result != "Ok")
        //            {
        //                error.Error("", result);
        //                return error;
        //            }
        //        }
        //        oContrato.ContratoSAP = repositorio.Obtener<FijacionDePrecioContrato, string>(x => x.Id == oContrato.Id, x => x.ContratoSAP);
        //        var res = modificarFijacionAgent.Modificar(oContrato, oContratoSave);
        //        if (res.Contains("Error"))
        //        {
        //            error.Error("SAP", res);
        //            return error;
        //        }



        //        logger.Debug("Actualizando contrato en BD DataAgro: " + oContrato.Id);



        //        if (oContratoSave == null || oContrato.Id == 0)
        //        {
        //            error.Error("Contrato", "No existe contrato en DataAgro");
        //        }
        //        oContratoSave.PagoCBU = oContrato.PagoCBU;
        //        repositorio.GuardarCambios();
        //        logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Modificar, oContrato.GetType());



        //    }
        //    catch (Exception e)
        //    {
        //        logger.Error(e);
        //        error.Error("", e.Message + ".");
        //    }
        //    return error;
        //}

        public string ObtenerSapContrato(int id)
        {
            var contrato = repositorio.Obtener<Contrato, string>(x => x.Id == id, x => x.ContratoSAP);
            return !String.IsNullOrEmpty(contrato) ? contrato.Substring(3, 7) : "";
        }

        public string ObtenerSapFijacion(int id)
        {
            var fijacion = repositorio.Obtener<FijacionDePrecioContrato, string>(x => x.Id == id, x => x.FijacionSAP);
            return !String.IsNullOrEmpty(fijacion) ? fijacion : "";
        }

        public Resultado AprobarContrato(int id)
        {
            var oEntityErrors = new Resultado();
            var contrato = repositorio.Obtener<Contrato>(id);

            if (contrato.Estado.EstadoContratoId == (int)EnumEstadoContrato.PreAprobacion)
            {
                Validar(contrato, oEntityErrors, false);
                if (oEntityErrors.Errores.Count > 0)
                {
                    return oEntityErrors;
                }

                if (ConfirmacionAutomatica(contrato) && DateTime.Now.Date == contrato.FechaOperacion.Date)
                {
                    contrato.FechaConfirmacion = DateTime.Now;
                    contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato " + contrato.Id + " se confirmo automaticamente por estar dentro de los rangos configurados");
                }
                else
                {
                    contrato.EstadoId = (int)EnumEstadoContrato.Pendiente;
                }
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "El Contrato no se puede aprobar");
            }

            return oEntityErrors;
        }

        public GrabarContratoResult BorrarContratoPreAprobacion(int id, string motivo)
        {
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(motivo) || string.IsNullOrWhiteSpace(motivo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<Contrato>(id);

            if (oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.PreAprobacion)
            {
                oContratoSave.Estado = repositorio.Obtener<EstadoContrato>((int)EnumEstadoContrato.Rechazado);
                oContratoSave.MotivoRechazo = motivo;
                try
                {
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());


                    EnviarMailRechazo(oContratoSave);
                }
                catch (Exception ex)
                {
                    logger.Error(ex);
                }
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede rechazar");
            }
            return oEntityErrors;
        }

        private void EnviarMailRechazo(Contrato contrato)
        {
            var id = contrato.CorredorId.HasValue ? contrato.CorredorId : contrato.ProveedorId;
            var enviarA = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == id && x.CompraNet == true);
            var asunto = ConfigurationManager.AppSettings["AmbientePruebas"] != "1" ? "" : "Prueba - ";
            asunto += "Rechazo Contrato Molinos Agro S.A. –  " + contrato.Proveedor.RazonSocial;
            var copia = new List<string>() { contrato.Comercial.IdActiveDirectory, ConfigurationManager.AppSettings["CredentialUserName"] };
            var vista = CuerpoMailRechazo(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), contrato);
            mailManager.EnviarMail(enviarA, asunto, "", copia, vista);
        }
        private AlternateView CuerpoMailRechazo(string filePath, Contrato fijacion)
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
        public BasicoContrato NegocioABasicoContrato(Negocio negocio)
        {
            var bc = new BasicoContrato();
            var proveedor = negocio.ProveedorId != null ? repositorio.Obtener<Proveedor>(negocio.ProveedorId) : null;
            var corredor = negocio.CorredorId != null ? repositorio.Obtener<Proveedor>(negocio.CorredorId) : null;
            var operador = (negocio is AgenteCompra) ? repositorio.Obtener<Operador>((negocio as AgenteCompra).OperadorId) : null;
            var comercial = repositorio.Obtener<Comercial>(negocio.ComercialId);

            if ((negocio is Contrato) && (negocio as Contrato).AnulaYReemplazaContratoId != null)
            {
                bc.AnulaYReemplazaContratoId = (negocio as Contrato).AnulaYReemplazaContratoId;
                bc.AnulaYReemplazaContratoSAP = repositorio.Obtener<Contrato, string>(x => x.Id == bc.AnulaYReemplazaContratoId, x => x.ContratoSAP);
                bc.MotivoReemplazo = (negocio as Contrato).MotivoReemplazo;
            }

            bc.Id = negocio.Id;
            bc.ContratoId = negocio is Contrato ? negocio.Id : negocio is FijacionDePrecioContrato ? (negocio as FijacionDePrecioContrato).ContratoId.HasValue ? (negocio as FijacionDePrecioContrato).ContratoId.Value : 0 : 0;
            bc.ProveedorId = negocio.ProveedorId ?? 0;
            bc.CorredorId = negocio.CorredorId != null ? negocio.CorredorId.Value : 0;
            bc.ComercialId = negocio.ComercialId;
            //bc.//ComercialZonaId = negocio.Comercial.GrupoDeComprasId;
            bc.MaterialId = negocio.MaterialId;
            bc.TipoNegocioId = negocio.TipoNegocioId;
            bc.Cantidad = negocio.Cantidad;
            bc.Precio = negocio.Precio;
            bc.PrecioPlazo = negocio.TipoNegocioId == 1 ? negocio.HastaFijacion.Value.ToString("dd-MM-yyyy") : negocio.Precio.ToString();
            bc.FechaEntrega = negocio is Contrato ? (negocio as Contrato).FechaEntrega.Date : (DateTime?)null;
            bc.CampanaId = negocio.CampanaId ?? 0;
            bc.FechaDesde = negocio.FechaDesde.Date;
            bc.FechaHasta = negocio.FechaHasta.Date;
            bc.FechaHastaFormateado = negocio is Contrato ? (negocio as Contrato).FechaHasta.ToString("dd-MM-yyyy") : negocio is ContratoAcuerdo ? (negocio as ContratoAcuerdo).FechaHasta.ToString("dd-MM-yyyy") : "";
            bc.FechaDesdeFormateado = negocio is Contrato ? (negocio as Contrato).FechaDesde.ToString("dd-MM-yyyy") : negocio is ContratoAcuerdo ? (negocio as ContratoAcuerdo).FechaDesde.ToString("dd-MM-yyyy") : "";
            bc.MonedaId = negocio.MonedaId;
            bc.Fecha = negocio.Fecha.Date;
            bc.Hora = negocio.Fecha.ToString("hh:mm");
            bc.Fecha_Order = negocio.Fecha.Date;

            // bc.GrupoCompra = (negocio is FijacionDePrecioContrato && (negocio as FijacionDePrecioContrato).ComercialId.HasValue) ? (negocio as FijacionDePrecioContrato).Comercial.GrupoDeComprasId.Value :
            //                         negocio.GrupoCompra.HasValue ? negocio.GrupoCompra.Value : 0;

            bc.ProvinciaId = negocio is Contrato ? (negocio as Contrato).ProvinciaId : null;
            bc.LocalidadId = negocio is Contrato ? (negocio as Contrato).LocalidadId : null;
            bc.Base = negocio is Contrato ? (negocio as Contrato).Base : null;
            bc.Importe_Sustentable = negocio is Contrato ? ((decimal)(negocio as Contrato).ImporteSustentable) : (decimal?)null;
            bc.MonedaId_Sustentable = negocio is Contrato ? (negocio as Contrato).MonedaSustentableId : "";
            bc.Moneda_Sustentable = negocio is Contrato ? (negocio as Contrato).MonedaSustentableId : "";
            bc.Fecha_Dolarizado = negocio is Contrato && ((negocio as Contrato).FechaDolarizado).HasValue ? ((negocio as Contrato).FechaDolarizado).Value.Date : (DateTime?)null;
            bc.Fecha_DolarizadoFormateado = negocio is Contrato && ((negocio as Contrato).FechaDolarizado).HasValue ? ((negocio as Contrato).FechaDolarizado).Value.ToString("dd-MM-yyyy") : "";
            bc.Dias_Pesificado = negocio.DiasPesificado;
            bc.NoInformaSIO = negocio is Contrato ? (negocio as Contrato).NoInformaSio : (bool?)null;
            bc.Estado = negocio.EstadoId;
            bc.UsuarioId = negocio.UsuarioId;
            bc.ContratoSAP = negocio.ContratoSAP;
            bc.Ampliaciones = negocio.Ampliaciones;

            bc.Cuit = proveedor == null ? "" : proveedor.CUIT;
            bc.Proveedor = (negocio is AgenteCompra) && operador != null ? operador.Descripcion : proveedor == null ? "" : (!string.IsNullOrEmpty(proveedor.Alias) ? (proveedor.Alias + " - " + proveedor.RazonSocial) : proveedor.RazonSocial) + $"({proveedor.CUIT})";
            bc.Corredor = corredor == null ? "" : (!string.IsNullOrEmpty(corredor.Alias) ? (corredor.Alias + " - " + corredor.RazonSocial) : corredor.RazonSocial) + $"({corredor.CUIT})";
            bc.CUITCorredor = corredor == null ? "" : corredor.CUIT;
            bc.Observacion = negocio.Observacion != null ? negocio.Observacion : "";

            bc.FijacionDePrecioContratoId = (negocio is FijacionDePrecioContrato) ? (int?)(negocio as FijacionDePrecioContrato).Id : null;
            bc.Sustentable = (negocio is Contrato) && (negocio as Contrato).ImporteSustentable != null && (negocio as Contrato).ImporteSustentable > 0;
            bc.Dolarizado = negocio.Dolarizado.Value;
            bc.Pesificado = negocio.DiasPesificado != null;
            bc.Negocio = negocio is ContratoAcuerdo ? negocio.Id.ToString() : (negocio is FijacionDePrecioContrato && (negocio.EstadoId == (int)EnumEstadoContrato.Finalizado || negocio.EstadoId == (int)EnumEstadoContrato.Eliminado)) ? (negocio as FijacionDePrecioContrato).FijacionSAP : negocio.ContratoSAP != "0" ? negocio.ContratoSAP : "";
            bc.DestinoId = negocio.DestinoId;
            bc.CantidadCamiones = (negocio is Contrato) ? (negocio as Contrato).CantidadCamiones : (int?)null;
            bc.Consignatario = (negocio is Contrato) ? (negocio as Contrato).Consignatario : false;
            bc.PlanCanje = (negocio is Contrato) ? (negocio as Contrato).PlanCanje : false;
            bc.CD = (negocio is Contrato) ? (negocio as Contrato).CD : null;
            bc.Warrant = (negocio is Contrato) ? (negocio as Contrato).Warrant : null;
            bc.PagoDirectoVendedor = (negocio is Contrato) ? (negocio as Contrato).PagoDirectoVendedor : null;
            bc.EstablecimientoPropio = (negocio is Contrato) ? (negocio as Contrato).EstablecimientoPropio : null;
            bc.BoletoId = (negocio is Contrato) ? (negocio as Contrato).BoletoId : null;
            bc.BolsaId = (negocio is Contrato) ? (negocio as Contrato).BolsaId : null;
            bc.DesdeFijacion = (negocio is Contrato) && ((negocio as Contrato).DesdeFijacion).HasValue ? ((negocio as Contrato).DesdeFijacion).Value.Date : (DateTime?)null;
            bc.DesdeFijacionFormateado = (negocio is Contrato) && ((negocio as Contrato).DesdeFijacion).HasValue ? ((negocio as Contrato).DesdeFijacion).Value.ToString("dd-MM-yyyy") : "";
            bc.HastaFijacion = (negocio is Contrato) && ((negocio as Contrato).HastaFijacion).HasValue ? ((negocio as Contrato).HastaFijacion).Value.Date : (DateTime?)null;
            bc.HastaFijacionFormateado = (negocio is Contrato) && ((negocio as Contrato).HastaFijacion).HasValue ? ((negocio as Contrato).HastaFijacion).Value.ToString("dd-MM-yyyy") : "";
            bc.CondicionFijacion = (negocio is Contrato) ? (negocio as Contrato).CondicionFijacionId : null;
            bc.ClasificacionId = (negocio is Contrato) ? (negocio as Contrato).ClasificacionId : (int?)null;
            bc.CalidadDescripcion = negocio.TrigoEspecial == true ? "Especial" : "Cámara";
            bc.MercsDeposito = (negocio is Contrato) ? ((negocio as Contrato).MercsDeposito == true ? (negocio as Contrato).MercsDeposito : false) : null;
            bc.ComercialCreadorId = negocio.ComercialCreadorId;
            bc.ContratoCorredor = (negocio is Contrato) ? (negocio as Contrato).ContratoCorredor : "";
            bc.ContratoVendedor = (negocio is Contrato) ? (negocio as Contrato).ContratoVendedor : "";
            bc.SelCargoMOA = (negocio is Contrato) ? (negocio as Contrato).SelCargoMOA : null;
            bc.SelCargoVendedor = (negocio is Contrato) ? (negocio as Contrato).SelCargoVendedor : null;
            bc.Posicion = (negocio is Fason) ? (negocio as Fason).Posicion : (negocio is AgenteCompra) ? (negocio as AgenteCompra).Posicion : "";
            bc.FasonId = (negocio is Fason) ? (negocio as Fason).Id : 0;
            bc.OperadorId = (negocio is AgenteCompra) ? (negocio as AgenteCompra).OperadorId : 0;
            bc.AgenteId = (negocio is AgenteCompra) ? (negocio as AgenteCompra).Id : 0;
            bc.PrecioNeto = negocio.PrecioNeto;
            bc.StandardCalidadId = negocio.StandardDeCalidadId;
            bc.Pizarra = negocio.Pizarra ?? null;
            bc.PagoDiferido = negocio.PagoDiferido ?? null;
            bc.ZonaId = (negocio is Contrato) ? (negocio as Contrato).ZonaId : null;
            bc.AcuerdoId = (negocio is ContratoAcuerdo) ? (int?)(negocio as ContratoAcuerdo).Id : null;
            bc.ImporteFinanciero = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 1).Importe : (decimal?)null;
            bc.ImporteRedespacho = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe : (decimal?)null; ;
            bc.PorcentajeComision = (negocio is Contrato) ? (negocio as Contrato).PorcentajeComision : 0;
            bc.ImporteComision = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 3).Importe : (decimal?)null;
            bc.ImporteBonificacion = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Importe : (decimal?)null;
            bc.PorcentajeBonificacion = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 4).Porcentaje : (decimal?)null;
            bc.TarifaFlete = (negocio is Contrato) ? (negocio as Contrato).TarifaFlete : null;
            bc.Compensacion = (negocio is Contrato) ? (negocio as Contrato).Compensacion : null;
            bc.Acuerdo = (negocio is Contrato) ? (negocio as Contrato).ContratoAcuerdoId : null;
            bc.Rechazo = negocio.MotivoRechazo;
            bc.OcultarEnTablero = negocio.OcultarEnTablero;
            bc.FechaCierta = negocio.FechaCierta.HasValue ? negocio.FechaCierta.Value.Date : (DateTime?)null;
            bc.FechaCiertaFormateado = negocio.FechaCierta.HasValue ? negocio.FechaCierta.Value.ToString("dd-MM-yyyy") : "";
            bc.EsFason = negocio is Contrato ? (negocio as Contrato).EsFason : false;
            bc.PorcentajeDePago = negocio is Contrato ? (negocio as Contrato).PorcentajeDePago : null;
            bc.FechaOperacion = negocio.FechaOperacion.Date;
            bc.FechaOperacionFormateado = negocio.FechaOperacion.ToString("dd-MM-yyyy");
            bc.MotivoOperacionAnterior = negocio.MotivoOperacionAnterior;
            bc.FechaConfirmacion = negocio.FechaConfirmacion.HasValue ? negocio.FechaConfirmacion.Value.Date : (DateTime?)null;
            bc.ChequeElectronicoValor = negocio.ChequeElectronico.HasValue ? (negocio.ChequeElectronico.Value ? "Si" : "No") : "";
            bc.DolarizadoExpress = negocio.DolarizadoExpress.Value;
            bc.PagoCBU = negocio.PagoCBU;
            bc.CalidadTercero = negocio.CalidadTercero;
            bc.DolarizadoTercero = negocio.DolarizadoTercero;
            bc.PagoDiferidoTercero = negocio.PagoDiferidoTercero;
            bc.ObservacionTercero = negocio.ObservacionTercero;
            bc.Canje = negocio.Canje;
            bc.MonedaCanjeId = negocio.MonedaCanjeId;
            bc.Monto = negocio.Monto;
            bc.Insumo = negocio.Insumo;
            bc.SustentableTercero = negocio.SustentableTercero;
            bc.ObligatoriedadCostoFinanciero = negocio.ObligatoriedadCostoFinanciero;
            //bc.ObligatoriedadBonificacion = negocio.ObligatoriedadBonificacion;

            bc.Descuentos = negocio.Descuentos.Select(y => new DescuentoBonificacionDto
            {
                ContratoId = y.ContratoId,
                FechaDesde = y.FechaDesde != null ? y.FechaDesde.Value.ToString("dd-MM-yyyy") : "",
                FechaHasta = y.FechaHasta != null ? y.FechaHasta.Value.ToString("dd-MM-yyyy") : "",
                Importe = y.Importe,
                MonedaId = y.MonedaId,
                Moneda = y.MonedaId,
                Id = y.Id,
                Porcentaje = y.Porcentaje,
                TipoDBDesc = repositorio.Obtener<TipoDB, string>(x => x.Id == y.TipoDBId, x => x.Descripcion),
                TipoDBId = y.TipoDBId,
                TipoPeriodoDBDesc = repositorio.Obtener<TipoPeriodoDB, string>(x => x.Id == y.TipoPeriodoDBId, x => x.Descripcion),
                TipoPeriodoDBId = y.TipoPeriodoDBId
            }).ToList();

            bc.Calidades = negocio is Contrato && (negocio as Contrato).Calidad != null ? (negocio as Contrato).Calidad.Select(y => new CalidadDto
            {
                Valor = y.Valor,
                CalidadEspecialId = y.CalidadEspecialId,
                CalidadEspecialDesc = repositorio.Obtener<CalidadEspecial, string>(x => x.Id == y.CalidadEspecialId, x => x.Descripcion),
                PorcentajeDesde = y.PorcentajeDesde,
                PorcentajeHasta = y.PorcentajeHasta
            }).ToList() : new List<CalidadDto>();

            bc.AperturaPrecios = negocio is Contrato && (negocio as Contrato).AperturaPrecio != null ? (negocio as Contrato).AperturaPrecio.Select(y => new AperturaPrecioDto
            {
                ConceptoAperturaPrecioId = y.ConceptoAperturaPrecioId,
                Importe = y.Importe,
                MonedaId = negocio.TipoNegocioId == 1 ? y.MonedaId : negocio.MonedaId,
                Porcentaje = y.Porcentaje
            }).ToList() : new List<AperturaPrecioDto>();

            bc.PreciosPactados = negocio is Contrato ? (negocio as Contrato).PrecioPactado.Select(y => new PrecioPactadosDto
            {
                ContratoId = y.ContratoId,
                FechaDesde = y.FechaDesde != null ? y.FechaDesde.Value.ToString("dd-MM-yyyy") : "",
                FechaHasta = y.FechaHasta != null ? y.FechaHasta.Value.ToString("dd-MM-yyyy") : "",
                Id = y.Id,
                ImportePactado = y.ImportePactado,
                MonedaImportePactadoDesc = y.MonedaImportePactadoId,
                MonedaImportePactadoId = y.MonedaImportePactadoId,
                MonedaPactadoDesc = y.MonedaPactadoId,
                MonedaPactadoId = y.MonedaPactadoId,
                Porcentaje = y.Porcentaje,
                Precio = y.Precio
            }).ToList() : new List<PrecioPactadosDto>();
            var clasificacion = (negocio is Contrato) ? (negocio as Contrato).ClasificacionId : (int?)null;
            bc.ClasificacionDescripcion = (negocio is Contrato) ? repositorio.Obtener<ClasificacionCompraNet, string>(x => x.Id == clasificacion, x => x.Descripcion) : "";
            bc.CalidadDescripcion = negocio.TrigoEspecial == true ? "Especial" : "Cámara";
            bc.ComercialZonaDescripcion = comercial != null && comercial.GrupoDeCompras != null ? comercial.GrupoDeCompras.Descripcion : "";
            var bolsaId = (negocio is Contrato) ? (negocio as Contrato).BolsaId : null;
            bc.BolsaDescripcion = (negocio is Contrato) ? repositorio.Obtener<BolsaCompraNet, string>(x => x.Id == bolsaId, x => x.Descripcion) : "";
            bc.CondicionFijacionDescripcion = (negocio is Contrato) ? repositorio.Obtener<CondicionFijacion, string>(x => x.Id == negocio.CondicionFijacionId, x => x.Descripcion) : "";
            var localidad = (negocio is Contrato) ? (negocio as Contrato).LocalidadId : null;
            var provincia = (negocio is Contrato) ? (negocio as Contrato).ProvinciaId : null;
            bc.Localidad = (negocio is Contrato) && (!(negocio is Contrato) || localidad == null) ? "" : repositorio.Obtener<Localidad, string>(x => x.LocalidadId == localidad, x => x.Nombre);
            bc.Provincia = (negocio is Contrato) && (!(negocio is Contrato) || provincia == null) ? "" : repositorio.Obtener<Provincia, string>(x => x.ProvinciaId == provincia, x => x.Nombre);
            bc.PrecioAjusteComision = (negocio is Contrato) ? (negocio as Contrato).PrecioAjusteComision : (decimal?)null;
            bc.MonedaAjusteComisionId = (negocio is Contrato) ? (negocio as Contrato).MonedaAjusteComisionId : "";
            bc.CaratulaMAT = (negocio is Contrato) ? (negocio as Contrato).CaratulaMAT : "";
            bc.TipoAgenteCompraId = (negocio is Contrato) ? (negocio as Contrato).TipoAgenteCompraId : (negocio is ContratoAcuerdo) ? (negocio as ContratoAcuerdo).TipoAgenteCompraId : (int?)null;
            bc.PrestamoDevolucion = negocio.PrestamoDevolucion.HasValue ? negocio.PrestamoDevolucion.Value : false;
            bc.PlantaDestinoId = negocio.PlantaDestinoId.HasValue ? negocio.PlantaDestinoId.Value : 0;
            bc.FechaHasta_SustentableFormateado = (negocio is Contrato) && (negocio as Contrato).FechaHastaSustentable != null ?
                (negocio as Contrato).FechaHastaSustentable.Value.ToString("dd-MM-yyyy") : "";
            bc.FechaDesde_SustentableFormateado = (negocio is Contrato) && (negocio as Contrato).FechaDesdeSustentable != null ?
                (negocio as Contrato).FechaDesdeSustentable.Value.ToString("dd-MM-yyyy") : "";
            bc.PosicionCBOT = negocio.PosicionCBOT;
            bc.TipoPosicionCBOTId = negocio.TipoPosicionCBOTId;
            bc.TipoPosicionCBOT = (negocio is Contrato) && (negocio as Contrato).TipoPosicionCBOT != null ? negocio.TipoPosicionCBOT.Descripcion : "";
            bc.ProveedorCreador = negocio.ProveedorCreadorId;
            return bc;
        }

        public TipoNegocio DevolverNamespaceNegocio(int tipo)
        {
            return repositorio.Obtener<TipoNegocio>(x => x.TipoNegocioId == tipo);
        }


        public Resultado AnularContratoCarga(int contratoId, string motivoRechazo)
        {
            var oEntityErrors = new Resultado();
            if (string.IsNullOrEmpty(motivoRechazo) || string.IsNullOrWhiteSpace(motivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar motivo de rechazo");
                return oEntityErrors;
            }
            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.PreAprobacion))
            {

                try
                {
                    oContratoSave.MotivoRechazo = motivoRechazo;
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoId), TipoAccionLogDataAgro.Eliminar, oContratoSave.GetType());
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);
                }
            }
            else
            {
                oEntityErrors.Error("", "No se pudo anular el contratro");
            }

            return oEntityErrors;
        }

        //public List<CcPpPerndienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req)
        //{
        //    return ccppAgent.ListarCartasDePortePendienteAplicar(req);
        //}

        public string ValidarCredito(string cuit, double cantidad, decimal precio, string moneda)
        {
            var tipoCambio = tipoCambioAgent.TraerTipoDeCambio(null);
            var resultado = "";
            if (!string.IsNullOrEmpty(cuit) && cantidad > 0 && precio > 0 && !string.IsNullOrEmpty(moneda))
            {
                var validacionCredito = validarCreditoAgente.ValidarCredito(cuit);
                var importeNegocio = ((precio * (decimal)cantidad) / 1000);
                if (importeNegocio > 0 && tipoCambio > 0 && validacionCredito != null)
                {
                    if (validacionCredito.Moneda == "ARP" && moneda == "ARP  " || validacionCredito.Moneda == "USDM" && moneda == "USDM ")
                    {
                        resultado = importeNegocio > validacionCredito.Monto ? "Sin Crédito" : validacionCredito.Monto + " " + validacionCredito.Moneda;
                    }
                    if (validacionCredito.Moneda == "ARP" && moneda == "USDM ")
                    {
                        resultado = (importeNegocio * tipoCambio) > validacionCredito.Monto ? "Sin Crédito" : validacionCredito.Monto + " " + validacionCredito.Moneda;
                    }
                    if (validacionCredito.Moneda == "USDM" && moneda == "ARP  ")
                    {
                        resultado = (importeNegocio / tipoCambio) > validacionCredito.Monto ? "Sin Crédito" : validacionCredito.Monto + " " + validacionCredito.Moneda;
                    }
                    if (validacionCredito.Moneda == "")
                    {
                        resultado = "Sin Crédito";
                    }
                }
            }


            return resultado;
        }

        public List<CcPpPerndienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPerndienteAplicarDto req)
        {
            return ccppAgent.ListarCartasDePortePendienteAplicar(req);
        }

        public List<ContratoCopiar> TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            DateTime? fecha = null;
            var dia = oDiasHabilesAgent.UltimoDiaHabil(fecha);
            return repositorio.ListarConsulta(new DevolverContratosAcuerdoPorCorredor(corredorId, dia));
        }

        public List<GrabarContratoResult> GrabarContratoMasivo(List<BasicoContrato> contratos)
        {
            List<GrabarContratoResult> results = new List<GrabarContratoResult>();
            var acuerdo = TraerContratoAcuerdoACopiar(contratos.First().ContratoAcuerdoId.Value);
            var comercial = mobjComercialManager.TraerComercial(acuerdo.ComercialId.Value);
            var materiales = mobjMaterialManager.TraerTodoMaterial().Material;
            var monedas = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            foreach (var item in contratos)
            {


                var proveedorid = mobjProveedorManager.ObtenerIdProveedorPorCuit(item.Cuit);
                if (proveedorid == 0)
                {
                    results.Add(new GrabarContratoResult { ContratoId = int.Parse(item.Observacion), Errores = new List<ErrorMessage> { new ErrorMessage { Source = "Proveedor", Message = "El cuit no existe." } } });
                    continue;
                }
                var proveedor = mobjProveedorManager.TraerProveedor(proveedorid, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
                var boletobolsa = mobjProveedorManager.TraerBoletoBolsa(proveedorid);
                item.Proveedor = proveedor.RazonSocial;
                item.Material = materiales.Where(a => a.MaterialId == item.MaterialId).Single().Descripcion;
                item.FechaOperacion = acuerdo.FechaOperacion.Value.Date;
                item.FechaDesde = acuerdo.FechaDesde;
                item.FechaHasta = acuerdo.FechaHasta;
                item.FechaEntrega = acuerdo.FechaHasta;
                item.DesdeFijacion = acuerdo.DesdeFijacion;
                item.HastaFijacion = acuerdo.HastaFijacion;
                Contrato contrato = new Contrato();
                contrato.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
                contrato.ContratoCorredor = item.ContratoCorredor;
                contrato.ContratoVendedor = item.ContratoVendedor;
                contrato.MaterialId = item.MaterialId;
                contrato.CampanaId = item.CampanaId;
                contrato.FechaOperacion = item.FechaOperacion.Value;
                contrato.Fecha = DateTime.Now;
                if (item.FechaOperacion.Value.Date < DateTime.Now.Date)
                {
                    contrato.MotivoOperacionAnterior = "Acuerdo " + item.ContratoAcuerdoId;
                }
                contrato.FechaDesde = item.FechaDesde.Value;
                contrato.FechaHasta = item.FechaHasta.Value;
                contrato.FechaEntrega = item.FechaEntrega.Value;
                contrato.Cantidad = item.Cantidad;
                contrato.ClasificacionId = item.ClasificacionId.Value;
                contrato.PlanCanje = item.PlanCanje;
                contrato.Consignatario = item.Consignatario;
                contrato.DestinoId = item.DestinoId;
                contrato.LocalidadId = item.LocalidadId;
                contrato.ProvinciaId = item.ProvinciaId;
                contrato.ContratoAcuerdoId = item.ContratoAcuerdoId;
                contrato.UsuarioTercero = item.UsuarioTercero;

                contrato.EstadoId = 9;
                contrato.AperturaPrecio = new List<AperturaPrecio>();
                foreach (var ap in acuerdo.AperturaPrecios)
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio
                    {
                        ConceptoAperturaPrecioId = ap.ConceptoAperturaPrecioId,
                        Importe = ap.Importe,
                        MonedaId = ap.MonedaId,
                        Porcentaje = ap.Porcentaje
                    });
                }
                contrato.Descuentos = new List<DescuentoBonificacion>();
                foreach (var d in acuerdo.Descuentos)
                {
                    contrato.Descuentos.Add(new DescuentoBonificacion
                    {
                        TipoDBId = d.TipoDBId,
                        TipoPeriodoDBId = d.TipoPeriodoDBId,
                        Importe = d.Importe,
                        MonedaId = d.MonedaId,
                        Porcentaje = d.Porcentaje
                    });
                }
                contrato.Calidad = new List<Calidad>();
                foreach (var c in acuerdo.Calidades)
                {
                    contrato.Calidad.Add(new Calidad
                    {
                        CalidadEspecialId = c.CalidadEspecialId,
                        PorcentajeDesde = c.PorcentajeDesde,
                        PorcentajeHasta = c.PorcentajeHasta,
                        StandardDeCalidadId = 2,
                        Valor = c.Valor,
                    });
                }
                DateTime? nullDate = null;
                contrato.PrecioPactado = new List<PrecioPactado>();
                foreach (var p in acuerdo.PreciosPactados)
                {
                    contrato.PrecioPactado.Add(new PrecioPactado
                    {
                        FechaDesde = string.IsNullOrEmpty(p.FechaDesde) ? nullDate : new DateTime(int.Parse(p.FechaDesde.Split('-')[2]), int.Parse(p.FechaDesde.Split('-')[1]), int.Parse(p.FechaDesde.Split('-')[0])),
                        FechaHasta = string.IsNullOrEmpty(p.FechaHasta) ? nullDate : new DateTime(int.Parse(p.FechaHasta.Split('-')[2]), int.Parse(p.FechaHasta.Split('-')[1]), int.Parse(p.FechaHasta.Split('-')[0])),
                        ImportePactado = p.ImportePactado,
                        MonedaImportePactadoId = p.MonedaImportePactadoId,
                        MonedaPactadoId = p.MonedaPactadoId,
                        Porcentaje = p.Porcentaje,
                        Precio = p.Precio
                    });
                }

                contrato.TipoAgenteCompraId = acuerdo.TipoAgenteCompraId;
                contrato.FechaCierta = acuerdo.FechaCierta;
                contrato.PorcentajeDePago = 97.5m;
                contrato.NivelTarifaId = acuerdo.NivelTarifaId;
                contrato.TarifaFlete = acuerdo.TarifaFlete;
                contrato.Observacion = acuerdo.Observacion;
                contrato.TipoNegocioId = acuerdo.Precio > 0 ? 2 : 1;
                item.TipoNegocioId = acuerdo.Precio > 0 ? 2 : 1;
                item.TipoNegocio = acuerdo.Precio > 0 ? "A Precio" : "A Fijar";
                contrato.Precio = acuerdo.Precio;
                contrato.PrecioNeto = acuerdo.PrecioNeto;
                item.Precio = acuerdo.Precio;
                item.PrecioNeto = acuerdo.PrecioNeto;
                contrato.MonedaId = acuerdo.MonedaId;
                item.Moneda = monedas.Where(a => a.MonedaId == acuerdo.MonedaId).SingleOrDefault() != null ? monedas.Where(a => a.MonedaId == acuerdo.MonedaId).SingleOrDefault().Descripcion : "";
                contrato.CantidadCamiones = acuerdo.CantidadCamiones;
                contrato.Base = acuerdo.Base;
                contrato.ImporteSustentable = acuerdo.Importe_Sustentable;
                contrato.MonedaSustentableId = acuerdo.Moneda_Sustentable;
                contrato.Dolarizado = acuerdo.Dolarizado;
                contrato.FechaDolarizado = acuerdo.Fecha_Dolarizado;
                contrato.DiasPesificado = acuerdo.Dias_Pesificado;
                contrato.PagoDiferido = acuerdo.PagoDiferido;
                contrato.NoInformaSio = acuerdo.NoInformaSIO;
                contrato.TrigoEspecial = acuerdo.TrigoEspecial;
                contrato.CD = acuerdo.CD;
                contrato.Warrant = acuerdo.Warrant;
                contrato.ChequeElectronico = acuerdo.ChequeElectronico;
                contrato.DolarizadoExpress = acuerdo.DolarizadoExpress;
                contrato.PagoCBU = acuerdo.PagoCBU;
                contrato.PagoDirectoVendedor = acuerdo.PagoDirectoVendedor;
                contrato.EstablecimientoPropio = acuerdo.EstablecimientoPropio;
                contrato.Madre = acuerdo.Madre;
                contrato.Venta = acuerdo.Venta;
                contrato.ContratoMadre = acuerdo.ContratoMadre;
                contrato.SelCargoMOA = acuerdo.SelCargoMOA;
                contrato.SelCargoVendedor = acuerdo.SelCargoVendedor;
                contrato.MercsDeposito = acuerdo.MercsDeposito;
                contrato.StandardDeCalidadId = acuerdo.StandardCalidadId;
                contrato.ZonaId = acuerdo.ZonaId;
                contrato.Pizarra = acuerdo.Pizarra == true;
                contrato.CaratulaMAT = acuerdo.CaratulaMAT;
                contrato.PrecioAjusteComision = acuerdo.PrecioAjusteComision;
                contrato.MonedaAjusteComisionId = acuerdo.MonedaAjusteComisionId;
                contrato.PagoDirectoVendedor = acuerdo.PagoDirectoVendedor;

                contrato.ProveedorCreadorId = acuerdo.CorredorId;
                contrato.CorredorId = acuerdo.CorredorId;
                contrato.ComercialCreadorId = null;
                contrato.UsuarioId = proveedor.RazonSocial;
                contrato.ComercialId = acuerdo.ComercialId;
                contrato.ProveedorId = proveedorid;
                contrato.BoletoId = boletobolsa.BoletoCompraNetId ?? 3;
                contrato.BolsaId = boletobolsa.BolsaCompraNetId;
                contrato.PorcentajeComision = 1;
                contrato.CondicionFijacionId = acuerdo.CondicionFijacion;
                contrato.DesdeFijacion = acuerdo.DesdeFijacion;
                contrato.HastaFijacion = acuerdo.HastaFijacion;

                var existe = repositorio.Existe<Contrato>(a => a.ContratoCorredor == contrato.ContratoCorredor && a.CorredorId == item.CorredorId && a.EstadoId != 8 && a.EstadoId != 6);
                if (existe)
                {
                    results.Add(new GrabarContratoResult { ContratoId = int.Parse(item.Observacion), Errores = new List<ErrorMessage> { new ErrorMessage { Source = "Contrato Corredor", Message = "El contrato corredor ya existe." } } });
                }
                else
                {
                    var result = GrabarContrato(contrato);
                    result.ContratoId = int.Parse(item.Observacion);
                    results.Add(result);
                }
            }
            if (contratos.Count > 0)
            {
                var enviarA = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == acuerdo.CorredorId && x.CompraNet == true);
                EnviarMailAltaMasiva(results, contratos, enviarA);
            }

            foreach (var item in results.Where(x => x.ContratoId != null && x.ContratoId > 0))
            {
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(item.ContratoId.Value), TipoAccionLogDataAgro.Crear, new Contrato().GetType());
            }
            return results;
        }

        private void EnviarMailAltaMasiva(List<GrabarContratoResult> results, List<BasicoContrato> contratos, List<string> enviarA)
        {
            var cuerpoMail = CuerpoMailAltaMasiva(httpContextManager.ObtenerPathLogoMail(), results, contratos);
            mailManager.EnviarMail(enviarA, "Resultado importacion alta masiva", "", null, cuerpoMail);
        }
        private AlternateView CuerpoMailAltaMasiva(string filePath, List<GrabarContratoResult> results, List<BasicoContrato> contratos)
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
            htmlBody += "En el presente mail se detalla los resultados de la importacion: <br /><br />  ";

            if (results.Where(a => a.HayError == false).Count() > 0)
            {
                htmlBody += " <br /><br />Los siguientes contratos quedan pendientes a verificar por el Comercial: <br /><br />  ";
                CrearTabla(results.Where(a => a.HayError == false).ToList(), th, ref linea, ref htmlBody, contratos);
            }
            else
            {
                htmlBody += "<b>No se pudo generar ningun contrato.</b>";
            }


            if (results.Where(a => a.HayError == true).Count() > 0)
            {
                htmlBody += " <br /><br />A continuacion se listan los contratos que no se pudieron generar: <br /><br />  ";
                CrearTabla(results.Where(a => a.HayError == true).ToList(), th, ref linea, ref htmlBody, contratos);
            }
            else
            {
                htmlBody += "<b>Ningun registro con error.</b>";
            }

            htmlBody += "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }
        private static void CrearTabla(List<GrabarContratoResult> results, string th, ref int linea, ref string htmlBody, List<BasicoContrato> contratos)
        {
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" +
                                th + "Linea" + "</td>" +
                                th + "Contrato Corredor" + "</td>" +
                                th + "Material" + "</td>" +
                                th + "Precio Base" + "</td>" +
                                th + "Moneda" + "</td>" +
                                th + "Cantidad (Kg)" + "</td>" +
                                //th + "Fecha de Carga" + "</td>" +
                                //th + "Fecha de Operacion" + "</td>" +
                                th + "Proveedor" + "</td>" +
                                //th + "Tipo" + "</td>" +
                                (!results.First().HayError ? "" : th + "Resultado" + "</td>") +
                                "</tr>";

            foreach (var c in results)
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
                    style1 = "style =\"border: 0px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                    style2 = "style=\"border: 0px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
                }

                linea += 1;
                var style = style1;
                if (linea % 2 == 0)
                {
                    style = style1;
                }
                else
                {
                    style = style2;
                }
                var contrato = contratos.Where(a => a.Observacion == (c.ContratoId ?? -1).ToString()).SingleOrDefault();
                htmlBody += "<tr>" +
                         "<td " + style + ((c.ContratoId ?? 0) + 2) + "</td>" +
                         "<td " + style + (contrato == null ? "" : contrato.ContratoCorredor) + "</td>" +
                         "<td " + style + (contrato == null ? "" : contrato.Material) + "</td>" +
                         "<td " + style + (contrato == null ? "" : contrato.Precio.ToString()) + "</td>" +
                         "<td " + style + (contrato != null && contrato.Moneda != null && contrato.TipoNegocioId == 2 ? contrato.Moneda : "") + "</td>" +
                         "<td " + style + (contrato == null ? "" : contrato.Cantidad.ToString()) + "</td>" +
                         //"<td " + style + (contrato != null && contrato.Fecha.HasValue ?  contrato.Fecha.Value.ToString("dd/MM/yyyy"):"") + "</td>" +
                         //"<td " + style + (contrato != null && contrato.FechaOperacion.HasValue ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : "") + "</td>" +
                         "<td " + style + (contrato == null ? "" : contrato.Proveedor?.ToString()) + "</td>" +
                         //"<td " + style + (contrato == null ? "" : contrato.TipoNegocio.ToString()) + "</td>" +
                         (!results.First().HayError ? "" : "<td " + style + "<b style='color:red;'>" + string.Join("<br>", c.Errores.Select(x => x.Message).ToList()) + "</b>" + "</td>");
            }

            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
        }

        public List<CapacidadProductivaPendienteDto> ObtenerCapacidadProductivaPendiente(int proveedorId)
        {
            var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == proveedorId, x => x.CUIT);
            var result = capacidadProductivaDisponibleAgent.ObtenerCapacidadProductivaPendiente(cuit);
            return result;
        }

        public Resultado ActualizarCesionContratoSAP(string contratoSAP, bool cesion)
        {
            var error = new Resultado();
            logger.Debug("Actualizando cesion contrato en BD DataAgro: " + contratoSAP);
            var contratoSave = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contratoSAP);
            if (contratoSave == null || contratoSave.Id == 0)
            {
                error.Error("Contrato", "No existe contrato en DataAgro");
                return error;
            }
            logger.Debug("Actualizando cesion contrato en BD DataAgro id: " + contratoSave.Id);

            contratoSave.Cesion = cesion;

            repositorio.GuardarCambios();

            logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoSave.Id), TipoAccionLogDataAgro.Modificar, contratoSave.GetType());

            return error;
        }

        public Resultado ValidacionesAnulaYReemplaza(string contratoSap)
        {
            var error = new Resultado();
            var fijacionesDeAfijar = repositorio.Listar<FijacionDePrecioContrato>(x => x.ContratoSAP == contratoSap && x.EstadoId != 8 && x.EstadoId != 6).ToList();
            var contratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contratoSap, x => x.Id);
            var contratoAnulado = repositorio.Obtener<Contrato>(x => x.AnulaYReemplazaContratoId == contratoId);
            if (contratoAnulado != null)
            {
                error.Error("Contrato", "Este contrato ya ha sido anulado y reemplazado");
                return error;
            }
            if (fijacionesDeAfijar.Count > 0)
            {
                error.Error("Contrato A fijar", "El contrato A fijar seleccionado tiene hechas fijaciones, en caso de querer continuar con esta anulación, por favor comunicarse con administración");
                return error;
            }
            return error;
        }

        //public List<NegocioAsociadoDto> DevolverContratosParaAsociar(int contratoId, string numero)
        //{
        //    var aFijar = repositorio.Obtener<Contrato>(x => x.Id == contratoId);
        //    if (aFijar != null)
        //    {
        //        var negociosQueYaEstanAsociados = DevolverContratoAsociadosPase(contratoId).Select(x => x.Id);
        //        Int32.TryParse(numero, out int id);
        //        var negocios = repositorio.Listar<Negocio, NegocioAsociadoDto>(
        //            x => new NegocioAsociadoDto()
        //            {
        //                Id = x.Id,
        //                TipoNegocioDesc = x.TipoNegocio.Descripcion,
        //                Precio = x.Precio,
        //                MonedaDesc = "USD",
        //                MaterialDesc = x.Material.Descripcion,
        //                Cantidad = x.Cantidad,
        //                Campania = x.Campana.Descripcion,
        //                Color = x.TipoNegocioId == 2 ? "" : "",
        //                Posicion = x.TipoNegocioId == 2 ? (SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta)) : x.Posicion,
        //            }, x => ((x.TipoNegocioId == 2 && x.TipoAgenteCompraId == null && x.ContratoSAP.Contains(numero)) || (x.TipoNegocioId == 5 && x.Id == id)) && x.MonedaId == "USDM "
        //               && !negociosQueYaEstanAsociados.Contains(x.Id) && x.CampanaId == x.Material.CampaniaTableroId && x.MaterialId == aFijar.MaterialId);
        //        return negocios;
        //    }
        //    return new List<NegocioAsociadoDto>();
        //}

        //public List<NegocioAsociadoDto> DevolverContratoAsociadosPase(int contratoSap)
        //{
        //    var aFijar = repositorio.Obtener<Contrato>(x => x.Id == contratoSap);
        //    if (aFijar != null)
        //    {
        //        var negociosQueYaEstanAsociados = repositorio.Listar<NegocioAsociado, NegocioAsociadoDto>(
        //            x => new NegocioAsociadoDto()
        //            {
        //                Id = x.Asociado.Id,
        //                TipoNegocioDesc = x.Asociado.TipoNegocio.Descripcion,
        //                Precio = x.Asociado.Precio,
        //                MonedaDesc = "USD",
        //                MaterialDesc = x.Asociado.Material.Descripcion,
        //                Cantidad = x.Asociado.Cantidad,
        //                Campania = x.Asociado.Campana.Descripcion,
        //                Color = x.Asociado.TipoNegocioId == 2 ? "" : "",
        //                Posicion = x.Asociado.TipoNegocioId == 2 ? (SqlFunctions.DateName("day", x.Asociado.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.Asociado.FechaHasta) + "/" + SqlFunctions.DateName("year", x.Asociado.FechaHasta)) : x.Asociado.Posicion,
        //            }, x => x.AFijarId == aFijar.Id);
        //        return negociosQueYaEstanAsociados;
        //    }
        //    return new List<NegocioAsociadoDto>();
        //}
        public void EnviarMailVenta(Contrato contrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string comercial)
        {
            var lista = new List<string>();
            var email = "";
            if (contrato.Comercial.IdActiveDirectory != comercial)
            {
                email = mailManager.GetEmailUserActiveDirectory(contrato.Comercial.IdActiveDirectory);
                lista.Add(email);
            }
            var comercialRegistrado = mailManager.GetEmailUserActiveDirectory(comercial);
            lista.Add(comercialRegistrado);
            var emailproveedor = repositorio.Listar<ContactoComercial, string>(x => x.Email1, x => x.ProveedorId == (contrato.CorredorId != null ? contrato.CorredorId : contrato.ProveedorId));
            logger.Debug("Enviando mail a Comercial Venta" + email);
            logger.Debug("Enviando mail a Comercial Registrado " + comercialRegistrado);
            var emailComerciales = "";

            var tienePermiso = repositorio.Obtener<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ModificarVenta)) && x.ComercialId == contrato.ComercialCreadorId) != null ? true : false;
            logger.Debug("Usuario tiene permiso " + tienePermiso);
            if (tienePermiso)
            {
                var comercialVenta = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ModificarVenta))); ;
                comercialVenta.Remove(contrato.Comercial);
                comercialVenta.Remove(contrato.ComercialCreador);
                logger.Debug("Enviando mail a " + string.Join(", ", comercialVenta.Select(x => x.IdActiveDirectory)));
                foreach (Comercial corredorComercialCopia in comercialVenta)
                {
                    try
                    {
                        emailComerciales = mailManager.GetEmailUserActiveDirectory(corredorComercialCopia.IdActiveDirectory);
                        logger.Debug("Mail encontrado para " + emailComerciales + "  " + corredorComercialCopia.IdActiveDirectory);
                        if (!String.IsNullOrEmpty(emailComerciales))
                        {

                            lista.Add(emailComerciales);
                        }
                    }
                    catch (Exception e) { logger.Error(e); }
                }
            }
            var subject = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                subject = "Nuevo negocio Molinos Agro S.A. – " + (contrato.Corredor != null ? contrato.Corredor.RazonSocial : contrato.Proveedor.RazonSocial);

            }
            else
            {
                subject = "Mail Prueba - Nuevo negocio Molinos Agro S.A. – " + (contrato.Corredor != null ? contrato.Corredor.RazonSocial : contrato.Proveedor.RazonSocial);


                var importe = CalcularImporteDeOperacion(contrato.PrecioNeto ?? contrato.Precio, contrato.Cantidad, contrato.MaterialId, contrato.FechaOperacion, contrato.MonedaId);
                mailManager.EnviarMail(contrato.Comercial, emailproveedor, subject, "", lista, CuerpoMailContratoVenta(httpContextManager.ObtenerPathLogoMail(), contrato, objDescuento, objCalidad, mailManager.GetEmailUserActiveDirectory(contrato.Comercial.IdActiveDirectory), false, importe));
            }
        }


        private AlternateView CuerpoMailContratoVenta(String filePath, Contrato oContrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string emailComercial, bool? eliminar, decimal importe)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            string style1 = "";
            string style2 = "";
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
                style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
                style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }

            var linea = 0;

            string htmlBody = "";
            if (eliminar.HasValue && eliminar.Value)
            {
                htmlBody += "En el presente mail, se detalla el negocio eliminado con Molinos Agro S.A.: <br /><br />  ";
            }
            else
            {
                htmlBody += "En el presente mail, se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />  ";
            }

            htmlBody += "<strong>Participantes </strong><br /><br />  ";

            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" +
                    th + "" + "</td>" +
                    th + "VENDEDOR" + "</td>" +
                    th + "CORREDOR" + "</td>" +
                    th + "COMPRADOR" + "</td>" +
                    "</tr>";

            htmlBody += "<tr>" +
                 "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px; border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\"> CUIT" + "</td>" +
                 "<td " + style2 + ConfigurationManager.AppSettings["Cuit"] + "</td>" +
                 "<td " + style2 + (oContrato.Corredor != null ? oContrato.Corredor.CUIT.ToUpper() : "") + "</td>" +
                 "<td " + style2 + oContrato.Proveedor.CUIT.ToUpper() + "</td> </tr>";

            htmlBody += "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px; border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\"> RAZÓN SOCIAL" + "</td>" +
                "<td " + style2 + ConfigurationManager.AppSettings["RazonSocial"] + "</td>" +
                "<td " + style2 + (oContrato.Corredor != null ? oContrato.Corredor.RazonSocial.ToUpper() : "") + "</td>" +
                "<td " + style2 + oContrato.Proveedor.RazonSocial.ToUpper() + "</td> </tr>";

            htmlBody += "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px; border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\"> DOMICILIO" + "</td>" +
                "<td " + style2 + ConfigurationManager.AppSettings["Domicilio"] + "</td>" +
                "<td " + style2 + (oContrato.Corredor != null ? oContrato.Corredor.Direccion.ToUpper() : "") + "</td>" +
                "<td " + style2 + oContrato.Proveedor.Direccion.ToUpper() + "</td> </tr>";

            htmlBody += "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px; border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\"> LOCALIDAD" + "</td>" +
                "<td " + style2 + ConfigurationManager.AppSettings["Localidad"] + "</td>" +
                "<td " + style2 + (oContrato.Corredor != null ? oContrato.Corredor.Localidad.Nombre.ToUpper() : "") + "</td>" +
                "<td " + style2 + oContrato.Proveedor.Localidad.Nombre.ToUpper() + "</td> </tr>";


            htmlBody += "<td style=\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px; border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\"> PROVINCIA" + "</td>" +
                "<td " + style2 + ConfigurationManager.AppSettings["Provincia"] + "</td>" +
                "<td " + style2 + (oContrato.Corredor != null ? oContrato.Corredor.Provincia.Nombre.ToUpper() : "") + "</td>" +
                "<td " + style2 + oContrato.Proveedor.Provincia.Nombre.ToUpper() + "</td> </tr>";

            htmlBody += " </td></tr>";

            htmlBody += "</td></tr></table><br /><br /> ";

            htmlBody += "<strong>Datos generales</strong> <br /><br />  ";

            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "NEGOCIO</th>" + Td(ref linea) + "VENTA" + "</td></tr>";
            htmlBody += "<tr>" + th + "FECHA</th>" + Td(ref linea);
            if (oContrato.ContratoAcuerdoId != null)
            {
                htmlBody += Split(oContrato.ContratoAcuerdo.Fecha.ToShortDateString()) + "</td></tr>";
            }
            else
            {
                htmlBody += Split(oContrato.FechaOperacion.ToShortDateString()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "GRANO</th>" + Td(ref linea) + oContrato.Material.Descripcion.ToUpper() + "</td></tr>";
            htmlBody += "<tr>" + th + "CONTRATO</th>" + Td(ref linea) + Split(oContrato.ContratoSAP.TrimStart('0')) + "</td></tr>";
            if (oContrato.DestinoId != null)
            {
                htmlBody += "<tr>" + th + "DESTINO</th>" + Td(ref linea) + (oContrato.ProcedenciaVenta != null ?
                oContrato.ProcedenciaVenta.Nombre + " (" + oContrato.ProcedenciaVenta.Provincia.Nombre + ")" : "") + "</td></tr>";
            }
            //htmlBody += "<tr>" + th + "PROVEEDOR</th>" + Td(ref linea) + oContrato.Proveedor.RazonSocial.ToUpper() + "</td></tr>";
            //htmlBody += "<tr>" + th + "CUIT</th>" + Td(ref linea) + Split(oContrato.Proveedor.CUIT.ToString()) + "</td></tr>";
            //if (oContrato.Corredor != null)
            //{
            //    htmlBody += "<tr>" + th + "CORREDOR</th>" + Td(ref linea) + oContrato.Corredor.RazonSocial.ToUpper() + "</td></tr>";
            //    htmlBody += "<tr>" + th + "CUIT CORREDOR</th>" + Td(ref linea) + Split(oContrato.Corredor.CUIT.ToString()) + "</td></tr>";
            //}
            //htmlBody += "<tr>" + th + "FIGURA</th>" + Td(ref linea) + oContrato.Clasificacion.Descripcion.ToUpper();
            if (oContrato.Consignatario == true)
            {
                htmlBody += " CONSIG";
            }
            htmlBody += "</td></tr>";

            htmlBody += "<tr>" + th + "CANTIDAD</th>" + Td(ref linea) + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))
                + " Kg.";
            if (oContrato.CantidadCamiones != null)
            {
                htmlBody += " (" + oContrato.CantidadCamiones + " camiones)<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea);
            if (oContrato.TipoNegocioId == 2)
            {
                if (oContrato.Pizarra.HasValue && oContrato.Pizarra.Value)
                {
                    htmlBody += "Pizarra</td></tr>";
                }
                else
                {
                    if (oContrato.PrecioNeto.HasValue)
                    {
                        htmlBody += Split(oContrato.PrecioNeto.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                    }
                    else
                    {
                        htmlBody += Split(oContrato.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.Moneda.Descripcion.ToUpper() + "</td></tr>";
                    }
                }
            }
            else if (oContrato.TipoNegocioId == 1)
            {
                htmlBody += "A FIJAR HASTA: <br />" + Split(oContrato.HastaFijacion.Value.ToShortDateString()) + "<br />" + Split(oContrato.CondicionFijacion.Descripcion.ToUpper()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "PORCENTAJE DE PAGO</th>" + Td(ref linea) + oContrato.PorcentajeDePago.ToString() + "</td></tr>";
            htmlBody += "<tr>" + th + "PROCEDENCIA</th>" + Td(ref linea) + (oContrato.Destino.Localidad != null ? 
                oContrato.Destino.Localidad.Nombre.ToUpper() +" (" + oContrato.Destino.Localidad.Provincia.Nombre.ToUpper() + ")" : oContrato.Destino.Descripcion) + "</td></tr>";
            htmlBody += "<tr>" + th + "ENT. DESDE</th>" + Td(ref linea) + Split(oContrato.FechaDesde.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "ENT. HASTA</th>" + Td(ref linea) + Split(oContrato.FechaHasta.ToShortDateString()) + "</td></tr>";
            htmlBody += "<tr>" + th + "COSECHA</th>" + Td(ref linea) + oContrato.Campana.Descripcion.ToUpper() + "</td></tr>";
            if (!String.IsNullOrEmpty(oContrato.ContratoMadre))
            {
                htmlBody += "<tr>" + th + "CONTRATO MADRE</th>" + Td(ref linea) + oContrato.ContratoMadre.TrimStart('0').ToUpper() + "</td></tr>";
            }

            //if (oContrato.BoletoId != null && oContrato.BoletoId != 3)
            //{
            //    htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + " " + oContrato.Bolsa.Descripcion.ToUpper() + "</td></tr>";
            //}
            //else if (oContrato.BoletoId == 3)
            //{
            //    htmlBody += "<tr>" + th + "BOLETO</th>" + Td(ref linea) + oContrato.Boleto.Descripcion.ToUpper() + "</td></tr>";
            //}
            htmlBody += "<tr>" + th + "OBSERVACIÓN</th>" + Td(ref linea);
            if (oContrato.TipoNegocioId == 1)
            {
                if (oContrato.Cantidad < 30000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split(oContrato.Cantidad.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
                else if (oContrato.Cantidad >= 30000 && oContrato.Cantidad <= 100000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
                else if (oContrato.Cantidad >= 100000)
                {
                    htmlBody += "CANTIDAD MÍNIMA A FIJAR " + Split(30000.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split((oContrato.Cantidad).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
                }
            }
            if (oContrato.EstablecimientoPropio == true)
            {
                htmlBody += "ESTABLECIMIENTO PROPIO<br />";
            }
            else if (oContrato.EstablecimientoPropio == false)
            {
                htmlBody += "ESTABLECIMIENTO ARRENDADO<br />";
            }
            if (oContrato.ImporteSustentable != null && oContrato.ImporteSustentable > 0)
            {
                htmlBody += "SUSTENTABLE " + oContrato.ImporteSustentable + " " + oContrato.MonedaSustentable.Descripcion.ToUpper() + "<br />";
            }
            if (oContrato.ClasificacionId == 1 && oContrato.CorredorId == null && oContrato.Dolarizado == true)
            {
                htmlBody += "DOLARIZADO MÍNIMO 30 DÍAS<br />";
            }
            if (oContrato.FechaDolarizado != null)
            {
                htmlBody += "FECHA DOLARIZADO " + Split(oContrato.FechaDolarizado.Value.ToShortDateString()) + "<br />";
            }
            if (oContrato.DiasPesificado != null)
            {
                htmlBody += "PAGO DIFERIDO <br />";
                htmlBody += "DÍAS DE DIFERIMIENTO " + oContrato.DiasPesificado + "<br />";
            }
            if (oContrato.CD == true)
            {
                htmlBody += "PAGO CD<br />";
            }
            else if (oContrato.Warrant == true)
            {
                htmlBody += "PAGO WARRANT<br />";
            }
            if (oContrato.PagoDirectoVendedor == true)
            {
                htmlBody += "PAGO DIRECTO<br /> ";
            }
            if (oContrato.MercsDeposito == true)
            {
                htmlBody += "MERCADERIA EN DEPOSITO<br /> ";
            }
            if (objDescuento != null)
            {
                foreach (var desc in objDescuento)
                {
                    if (desc.Importe > 0 || desc.Porcentaje > 0)
                    {
                        htmlBody += "BONIFICACIONES " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                    }
                    else if (desc.Importe < 0 || desc.Porcentaje < 0)
                    {
                        htmlBody += "DESCUENTOS " + "<br />" + desc.TipoDB.Descripcion.ToUpper() + "<br />";
                    }
                    if (desc.Importe != 0)
                    {
                        htmlBody += desc.Importe + " " + desc.Moneda.Descripcion + "<br />";
                    }

                    if (desc.Porcentaje != 0)
                    {
                        htmlBody += desc.Porcentaje + "%<br />";
                    }
                }
            }
            if (oContrato.StandardDeCalidadId == 1)
            {
                htmlBody += "CAMARA<br />";
            }
            else if (oContrato.StandardDeCalidadId == 3)
            {
                htmlBody += "FABRICA<br />";
            }
            else if (oContrato.StandardDeCalidadId == 7)
            {
                htmlBody += "CALIDAD GRADO 2<br />";
            }
            else if (oContrato.TrigoEspecial == true)
            {
                htmlBody += "CALIDAD ESPECIAL ";
            }
            if (objCalidad != null && oContrato.StandardDeCalidadId != 7)
            {
                foreach (var cal in objCalidad)
                {
                    htmlBody += cal.CalidadEspecial.Descripcion.ToUpper() + " " + cal.Valor.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "<br />";
                    if (cal.PorcentajeDesde != null && cal.PorcentajeHasta != null)
                    {
                        htmlBody += "Porc. Desde " + cal.PorcentajeDesde + "% Hasta " + cal.PorcentajeHasta + "%<br />";
                    }
                }
            }
            if (oContrato.PrecioPactado != null && oContrato.PrecioPactado.Count > 0)
            {
                htmlBody += "PRECIO PACTADO <br />";
                foreach (var precio in oContrato.PrecioPactado)
                {
                    htmlBody += " Si la entrega se realiza entre el " + precio.FechaDesde.Value.ToString("dd/MM/yyyy") + " y el " + precio.FechaHasta.Value.ToString("dd/MM/yyyy") +
                        " el precio será " + precio.Precio.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + " " + precio.MonedaPactado.Descripcion.ToUpper() +
                        "<br />";

                    if (precio.ImportePactado != null && precio.ImportePactado > 0 && precio.MonedaImportePactado != null)
                    {
                        htmlBody += "Importe Pactado " + precio.ImportePactado.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + " " + precio.MonedaImportePactado.Descripcion.ToUpper() + "<br />";
                    }
                    if (precio.Porcentaje != null)
                    {
                        htmlBody += "Porcentaje Pactado " + precio.Porcentaje.Value.ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "%<br />";
                    }
                }
            }
            if (oContrato.PlanCanje == true)
            {
                htmlBody += "PLAN CANJE" + "<br />";
            }
            if (oContrato.ContratoVendedor != null)
            {
                htmlBody += "Contrato Vendedor: " + oContrato.ContratoVendedor + "<br />";
            }
            if (oContrato.ContratoCorredor != null)
            {
                htmlBody += "Contrato Corredor: " + oContrato.ContratoCorredor + "<br />";
            }
            if (oContrato.SelCargoMOA == true)
            {
                htmlBody += " Sellado 100% a Cargo MOA " + "<br />";
            }
            if (oContrato.SelCargoVendedor == true)
            {
                htmlBody += " Sellado 100% a Cargo vendedor " + "<br />";
            }
            if (oContrato.Compensacion == true)
            {
                htmlBody += " NEGOCIO COMPENSACIÓN " + "<br />";
            }
            if ((oContrato.NivelTarifa != null) && oContrato.TarifaFlete > 0)
            {
                htmlBody += " NIVEL DE TARIFA: " + oContrato.NivelTarifa.Descripcion.ToUpper() + "<br />" +
                    "TARIFA DE FLETE: " + oContrato.TarifaFlete + "<br />";
            }
            if (oContrato.Observacion != null)
            {
                htmlBody += oContrato.Observacion + "<br />";
            }
            if (oContrato.FechaCierta.HasValue)
            {
                htmlBody += "Fecha Cierta de pago: " + oContrato.FechaCierta.Value.ToString("dd/MM/yyyy") + "<br />";
            }
            if (oContrato.DolarizadoExpress == true)
            {
                htmlBody += "A pesificar en mes en curso mediante envió de mail a materias.primas@molinosagro.com.ar hasta las 13 hs. <br />";
            }
            if (oContrato.ChequeElectronico == true)
            {
                htmlBody += "Pago con Echeq <br />";
            }
            if (oContrato.PagoCBU != null)
            {
                htmlBody += "Pago con Cbu: " + oContrato.PagoCBU + " <br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "</table>";
            htmlBody += "<br /><strong>Condiciones de Venta</strong> <br /><br />  ";
            htmlBody += "<table style=\"border-collapse: collapse;border: 2px solid white; text-align:center; font-size: 13px;\">";
            htmlBody += "<tr>" + th + "CAMARA </th>" + Td(ref linea) + (oContrato.Camara != null ? oContrato.Camara.Descripcion : "NO") + "</td></tr>";
            htmlBody += "<tr>" + th + "COMISION A FAVOR </th>" + Td(ref linea) + (oContrato.ComisionAFavor != null ?
                oContrato.ComisionAFavor.Descripcion + " " + oContrato.PorcentajeComisionVenta + "%" : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "FLETE A CARGO </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.FleteACargo) ? oContrato.FleteACargo : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "KG BALANZA </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.KgBalanza) ? oContrato.KgBalanza : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "PAGO </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.Pago) ? oContrato.Pago : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINO MERCADERIA</th>" + Td(ref linea) + (oContrato.ProcedenciaVenta != null ?
               oContrato.ProcedenciaVenta.Nombre + " (" + oContrato.ProcedenciaVenta.Provincia.Nombre + ")" : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "IMPORTE DE LA OPERACION</th>" + Td(ref linea) + Split(importe.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td></tr>";

            //htmlBody += "<tr>" + th + "CREDITO DISPONIBLE</th>" + Td(ref linea) + (oContrato.CreditoDisponible.HasValue ? Split(oContrato.CreditoDisponible.Value.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.MonedaCreditoDisponible : "") + "</td></tr>";

            htmlBody += "<tr>" + th + "CONDICION DE PAGO</th>" + Td(ref linea) + (oContrato.CondicionDePagoFijacionVenta != null ?
               oContrato.CondicionDePagoDiaFijacion + " dias " + oContrato.CondicionDePagoTipoFijacion + " " + oContrato.CondicionDePagoFijacionVenta.Descripcion : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "CONDICION DE PESIFICACION</th>" + Td(ref linea) + (oContrato.CondicionDePagoPesificadoVenta != null ?
              oContrato.CondicionDePagoDiaPesificado + " dias " + oContrato.CondicionDePagoTipoPesificado + " " + oContrato.CondicionDePagoPesificadoVenta.Descripcion : "") + "</td></tr>";
            htmlBody += "</table>";

            htmlBody += "<br /><br /> Por favor revisar que los datos sean correctos, de lo contrario contactarse con " + (oContrato.Comercial != null ? oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? "(" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                "<br /> <br />  Saludos Cordiales" +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public decimal CalcularImporteDeOperacion(decimal precio, double cantidad, int materialId, DateTime fechaoperacion, string moneda)
        {
            var diaAnterior = fechaoperacion.AddDays(-1);
            var cambio = tipoCambioAgent.TraerTipoDeCambio(diaAnterior);
            if (moneda == "ARP  ")
            {
                precio = precio / cambio;
            }

            var iva = mobjMaterialManager.DevolverIVAPorMaterial(materialId);
            iva = iva / 100 + 1;
            var importe = (precio * (decimal)cantidad / 1000) * iva;
            return importe;
        }
    }
}