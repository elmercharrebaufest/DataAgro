using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
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
using System.Data;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
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
        private readonly IContratosParaFijacionAgent contratosParaFijacionAgent;
        private readonly IConfiguracionInternaManager configuracionInternaManager;
        private readonly ICentroManager centroManager;
        private readonly ICupoManager cupoManager;

        public ContratoManager(ILogger logger, IRepositorio repositorio,
            IMaterialManager oMSMaterialManager, ITipoNegocioManager oMSTipoNegocioManager,
            ICampañaManager oMSCampaniaManager, IProvinciaManager oMSProvinciaManager,
            ILocalidadManager oMSLocalidadManager, IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager, IPushNotificationManager oMSNotification,
            IDiferencialManager diferencialManager, IFinalizarContratoAgent oFinalizarContratoAgent,
            IDiasHabilesAgent oDiasHabilesAgent, IRelacionCorredorProveedorAgent oRelacionCorredorProveedorAgent,
            IEliminarContratoAgent oEliminarContratoAgent, IConfiguracionManager configuracionManager,
            ICapacidadProductivaAgent capacidadProductiva, IAltaTempranaAgent altaTempranaAgent,
            IDiasHabilesAgent diasHabilesAgent, IModificarContratoAgent modificarContratoAgent,
            IMailManager mailManager, IStatusContratoAgent status, ILogDataAgroManager logDataAgroManager,
            IValidarDocProcPagoAgent validarPagoAgente, IListaCBUProveedorAgent cbuAgent, IModificarFijacionAgent modificarFijacionAgent,
            ICartasDePortePendienteAplicarAgent ccppAgent, IHttpContextManager httpContextManager,
            IValidacionCreditoAgent validarCreditoAgente, ITipoDeCambioAgent tipoCambioAgent,
            ICapacidadProductivaDisponibleAgent capacidadProductivaDisponibleAgent, INegocioManager negocioManager,
            IContratosParaFijacionAgent contratosParaFijacionAgent, IConfiguracionInternaManager configuracionInternaManager,
            ICentroManager centroManager, ICupoManager cupoManager)
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
            this.contratosParaFijacionAgent = contratosParaFijacionAgent;
            this.configuracionInternaManager = configuracionInternaManager;
            this.centroManager = centroManager;
            this.cupoManager = cupoManager;
        }

        public DatosIniContrato TraerDatosCombo(int? tipoNegocioId = null)
        {
            var datosCombo = new DatosIniContrato();
            var hoy = DateTime.Now;

            datosCombo.prov = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry() { Provinciaid = x.ProvinciaId, Nombre = x.Nombre, Orden = x.Orden, Inscripto = x.Inscripto }, null, 0, "Orden");

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
                datosCombo.tiponegocio = datosCombo.tiponegocio.Where(x => x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION).ToList();
            }
            if (PermisosHelper.Is(PermisosDataAgro.ModificarNegocios) || PermisosHelper.Is(PermisosDataAgro.ModificarNegFinalizados))
            {
                datosCombo.tiponegocio = tiposDeNegocio;
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosFason) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == (int)EnumTipoNegocio.FASON));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosAgente) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == (int)EnumTipoNegocio.AGENTE_DE_COMPRAS));
            }
            if (!PermisosHelper.Is(PermisosDataAgro.CrearNegociosAcuerdos) && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == (int)EnumTipoNegocio.CONTRATO_ACUERDO));
            }

            datosCombo.Clasificacion = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Bolsa = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            var destinos = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion, ProvinciaId = x.Localidad.ProvinciaId }, x => x.CargaNegocios == true);
            datosCombo.Destino = destinos.Where(x => x.Id == 1).ToList();
            datosCombo.Destino.AddRange(destinos.Where(x => x.Id != 1).OrderBy(x => x.Descripcion).ToList());
            datosCombo.Condicion = repositorio.Listar<CondicionFijacion, CondicionFijacionQry>(x => new CondicionFijacionQry() { Id = x.Id, Descripcion = x.Descripcion }, x => x.Habilitado);

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
            datosCombo.BoletoVenta = repositorio.Listar<BoletoVenta, BoletoVentaQry>(
                x => new BoletoVentaQry { Id = x.Id, Descripcion = x.Descripcion });

            //datosCombo.MinutosCronometroConDescarga = repositorio.Obtener<Configuracion>(1).MinutosCronometroConDescarga;
            Configuracion configuracion = repositorio.Obtener<Configuracion>(1);
            datosCombo.MinutosCronometroConDescarga = configuracion == null ? 5 : configuracion.MinutosCronometroConDescarga;

            return datosCombo;
        }

        private Resultado Validar(Contrato oParam, Resultado oErrorMessages)
        {
            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacío.");
                return oErrorMessages;
            }
            if (oParam.CampanaId == null || oParam.CampanaId == 0)
            {
                oErrorMessages.Error("campanaId", "El campo 'Campaña' no debe estar vacío");
            }
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
            if (oParam.ProveedorId == -1)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' debe tener un proveedor existente.");
                return oErrorMessages;
            }
            if (!string.IsNullOrEmpty(proveedor.RiesgoComercialSap))
            {
                if (proveedor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Riesgo Comercial Alto.");
                }
            }
            if (oParam.TipoAgenteCompraId == null)
            {
                if (proveedor.OperaConMATBA == true)
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor SOLO Opera con MATBA.");
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
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por Riesgo Comercial Alto.");
                    }
                }

                if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == oParam.CorredorId && x.EstadoId == (int)EnumEstadoContrato.Con_Error))
                {
                    oErrorMessages.Error("Estado", "Corredor no Operable por Estado BAJA.");
                }

                if (repositorio.Existe<FACACOP>(x => x.CUIT == corredor.CUIT))
                {
                    oErrorMessages.Error("CorredorId", "Corredor No Operable por ser Apócrifo.");
                }
                if (oParam.TipoAgenteCompraId == null)
                {
                    if (corredor.OperaConMATBA == true)
                    {
                        oErrorMessages.Error("CorredorId", "Corredor SOLO Opera con MATBA.");
                    }
                }
            }

            if (oParam.ClasificacionId == 0)
            {
                oErrorMessages.Error("ClasificacionId", "El campo 'Clasificación' no debe estar vacío.");
            }
            var config = repositorio.Obtener<Configuracion>(1);
            var importeSustentable = PermisosHelper.Is(PermisosDataAgro.ImporteSustentableEspecial) ? config.ImporteSustentableEspecial : config.ImporteSustentable;

            if (oParam.MonedaSustentableId == "USDM " && oParam.ImporteSustentable > importeSustentable)
            {
                oErrorMessages.Error("Importe", "Se excede la tarifa Sustentable, EPA o EUDR.");
            }

            int[] otros = { 2, 3, 4, 8, 9, 10, 11, 12, 13 };

            var sisa = new SISA();
            if (oParam.ClasificacionId == 1)
            {
                sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == (int)EnumEstadoSisa.PRODUCTOR && x.SituacionCategoria == "AL");
            }
            else if (oParam.ClasificacionId == 2)
            {
                sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == (int)EnumEstadoSisa.ACOPIADOR && x.SituacionCategoria == "AL");
            }
            else if (oParam.ClasificacionId == 3)
            {
                sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria != (int)EnumEstadoSisa.PRODUCTOR && x.CodCategoria != (int)EnumEstadoSisa.ACOPIADOR && x.CodCategoria != (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS && x.SituacionCategoria == "AL");
            }
            if (sisa != null)
            {
                if (sisa.EstadoCuit == 3 && proveedor.RiesgoComercialSap != "E")
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT 3.");
                }
                else if (sisa.EstadoCuit == 0)
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT Inactivo.");
                }
                if (sisa.SituacionCategoria != "AL")
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Situación Categoría BA.");
                }
                if (sisa.CodCategoria == (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS)
                {
                    oErrorMessages.Error("ProveedorId", "No operable por categoría Operador de Derivados Granarios.");
                }
            }
            else
            {
                oErrorMessages.Error("ProveedorId", "Proveedor No Operable por CUIT o Categoría Inactivo.");
            }

            if (corredor != null)
            {
                sisa = repositorio.Obtener<SISA>(x => x.CUIT == corredor.CUIT && x.CodCategoria == (int)EnumEstadoSisa.CORREDOR && x.SituacionCategoria == "AL");

                if (sisa != null)
                {
                    if (sisa.EstadoCuit == 3 && corredor.RiesgoComercialSap != "E")
                    {
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por Estado de CUIT 3.");
                    }
                    else if (sisa.EstadoCuit == 0)
                    {
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por Estado de CUIT Inactivo.");
                    }
                    if (sisa.SituacionCategoria != "AL")
                    {
                        oErrorMessages.Error("CorredorId", "Corredor No Operable por Situación Categoría BA.");
                    }
                }
                else
                {
                    oErrorMessages.Error("CorredorId", "Corredor No Operable por CUIT o Categoría Inactivo.");
                }
                if (!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                {
                    oErrorMessages.Error("Corredor", "El Proveedor no pertenece al Corredor seleccionado.");
                }
            }

            if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == oParam.ProveedorId && x.EstadoId == (int)EnumEstadoContrato.Con_Error))
            {
                oErrorMessages.Error("Estado", "Proveedor no Operable por Estado BAJA.");
            }
            var facacop = repositorio.Obtener<FACACOP>(x => x.CUIT == proveedor.CUIT);
            if (facacop != null)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor No Operable por ser Apócrifo.");
            }
            oParam.Destino = repositorio.Obtener<Centro>(x => x.Id == oParam.DestinoId);
            if (oParam.Venta != true)
            {
                if (oParam.Venta != true)
                {
                    oErrorMessages.Errores.AddRange(negocioManager.ValidarAltaTemprana(oParam, proveedor).Errores);
                }
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
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "El campo 'Campaña' no debe estar vacío.");
            }
            if (oParam.EsFason != true)
            {
                if (oParam.DestinoId == 0 || oParam.DestinoId == null)
                {
                    oErrorMessages.Error("DestinoId", "El campo 'Destino' no debe estar vacío.");
                }
                if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && (oParam.CondicionFijacionId == null || oParam.DesdeFijacion == null || oParam.HastaFijacion == null) && oParam.PrestamoDevolucion != true && oParam.Canje != true)
                {
                    oErrorMessages.Error("CondicionFijacionId", "Las Condiciones de Fijaciones no deben estar vacías cuando el contrato es 'A Fijar'.");
                }
                if (oParam.Venta != true)
                {
                    if (PermisosHelper.ObtenerUsuario() != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
                    {
                        if (oParam.Destino.ValidaRedespacho && (oParam.AperturaPrecio == null || !oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho)) /*&& oParam.Pizarra != true*/)
                        {
                            oErrorMessages.Error("Descuentos", "Se debe completar Redespacho en Acopios.");
                        }

                        //if (centro.ValidaRedespacho == true && (oParam.Descuentos == null || !oParam.Descuentos.Any(x => x.Importe < 0 && x.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO && x.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES)) && oParam.Pizarra == true)
                        //{
                        //    oErrorMessages.Error("Descuentos", " Se debe completar Redespacho en Acopios.");
                        //}

                        if (!oParam.Destino.ValidaRedespacho && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho) /*&& oParam.Pizarra != true*/)
                        {
                            oErrorMessages.Error("Descuentos", "Solo se debe completar Redespacho en Acopios.");
                        }

                        if (oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.Importe < 0 && x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero) /*&& oParam.Pizarra != true*/)
                        {
                            oErrorMessages.Error("Descuentos", "El costo financiero no puede ser negativo.");
                        }
                    }
                    if (oParam.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO)
                    {
                        oErrorMessages.Errores.AddRange(negocioManager.ValidarSinBoleto(oParam).Errores);
                    }
                }
            }

            if (oParam.Venta != true && oParam.ClasificacionId == 1)
            {
                var centroCodigoSap = repositorio.Obtener<Centro, string>(x => x.Id == oParam.DestinoId, x => x.CodigoSap);
                var material = repositorio.Obtener<Material, string>(x => x.MaterialId == oParam.MaterialId, x => x.Codigo);
                var cosecha = repositorio.Obtener<Campaña, string>(x => x.CampañaId == oParam.CampanaId, x => x.Descripcion);
                var result = capacidadProductiva.ObtenerCapacidadProductiva(proveedor.CUIT, (decimal)oParam.Cantidad, centroCodigoSap, cosecha, material);
                if (result.ToUpper() != "OK".ToUpper())
                {
                    oErrorMessages.Error("Capacidad Productiva al validar contrato: ", result);
                }
            }

            if (!string.IsNullOrEmpty(oParam.ContratoMadre))
            {
                var sap = oParam.ContratoMadre.PadLeft(10, '0');
                var cantidadMadre = repositorio.Obtener<Contrato, double>(x => x.ContratoSAP == sap, x => x.Cantidad);
                var sumaContratosHijos = repositorio.Listar<Contrato>(x => x.ContratoMadre == sap && x.Id != oParam.Id && (x.EstadoId <= 5 || x.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Select(x => x.Cantidad).Sum();
                if (cantidadMadre - sumaContratosHijos < oParam.Cantidad)
                {
                    oErrorMessages.Error("Cantidad", "La cantidad supera a la cantidad del convenio.");
                }
            }
            if (string.IsNullOrEmpty(oParam.ContratoMadre))
            {
                if (oParam.Precio == 0 && oParam.TipoNegocioId != (int)EnumTipoNegocio.A_FIJAR && !oParam.Pizarra.Value && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacío.");
                }

                if ((oParam.PrecioNeto == 0 || oParam.PrecioNeto == null) && !oParam.Pizarra.Value && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    oErrorMessages.Error("Precio", "El campo 'Precio Neto' no debe estar vacío.");
                }

                if (string.IsNullOrEmpty(oParam.MonedaId) && oParam.TipoNegocioId != (int)EnumTipoNegocio.A_FIJAR && !oParam.Pizarra.Value && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacío.");
                }
            }

            if ((oParam.LocalidadId == 0 || oParam.LocalidadId == null) && (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO))
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' no debe estar vacío.");
            }
            if ((oParam.LocalidadId == -1) && (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO))
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' debe tener un valor existente.");
            }
            if (oParam.ProvinciaId == null && (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO))
            {
                oErrorMessages.Error("ProvinciaId", "El campo 'Provincia' no debe estar vacío.");
            }
            if (!oParam.FechaEntrega.HasValue)
            {
                oErrorMessages.Error("FechaEntrega", "El campo 'Fecha de Entrega' no debe estar vacío.");
            }
            if (oParam.FechaDesde.Year == 1)
            {
                oErrorMessages.Error("FechaDesde", "El campo 'Fecha Desde' no debe estar vacío.");
            }
            if (oParam.FechaHasta.Year == 1)
            {
                oErrorMessages.Error("FechaHasta", "El campo 'Fecha Hasta' no debe estar vacío.");
            }

            if (oParam.TipoNegocioId == 0)
            {
                oErrorMessages.Error("TipoNegocioId", "El campo 'Tipo de Negocio' no debe estar vacío.");
            }

            if (oParam.ComercialId == 0 || oParam.ComercialId == null)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacío.");
            }
            //if (oParam.ProvinciaId == 1 && oParam.ClasificacionId == 1 && oParam.EstablecimientoPropio == null)
            //{
            //    oErrorMessages.Error("EstablecimientoPropio", "El campo 'Establecimiento' no debe estar vacio cuando Provincia es Buenos Aires y es Productor");
            //}

            if (oParam.BoletoId == 0 || oParam.BoletoId == null)
            {
                oErrorMessages.Error("BoletoId", "Boleto no debe estar vacío.");
            }
            if (oParam.BoletoId != (int)EnumBoletoCompraNet.SIN_BOLETO)
            {
                if ((oParam.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA || oParam.BoletoId == (int)EnumBoletoCompraNet.FISICO || oParam.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA) && (oParam.BolsaId == 0 || oParam.BolsaId == null))
                {
                    oErrorMessages.Error("BolsaId", "Bolsa no debe estar vacío cuando existe Boleto.");
                }
            }
            if (oParam.FechaDesde > oParam.FechaHasta)
            {
                oErrorMessages.Error("FechaDesdeHasta", "La 'Fecha Desde' no puede ser mayor a la 'Fecha Hasta'.\n");
            }
            if (oParam.DesdeFijacion.HasValue && oParam.HastaFijacion.HasValue)
            {
                if (oParam.DesdeFijacion > oParam.HastaFijacion)
                {
                    oErrorMessages.Error("FechaDesdeHastaFijacion", "Las fechas de fijación no son válidas.\n");
                }
                if (!diasHabilesAgent.EsDiaHabil(oParam.DesdeFijacion.Value) || !diasHabilesAgent.EsDiaHabil(oParam.HastaFijacion.Value))
                {
                    oErrorMessages.Error("FechaFijacion", "Las fechas de fijación no pueden ser días inhábiles (fin de semana o feriado).\n\n");
                }
            }

            var rangosPrecio = repositorio.Obtener<RangoPrecio>(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId);
            if (!oParam.Pizarra.Value && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                if (rangosPrecio != null && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && (oParam.Precio < rangosPrecio.PrecioMinimo || oParam.Precio > rangosPrecio.PrecioMaximo))
                {
                    oErrorMessages.Error("Precio", "Precio fuera de Rango - Precio Mínimo: " + rangosPrecio.PrecioMinimo.ToString("N2") + " y Precio Máximo: " + rangosPrecio.PrecioMaximo.ToString("N2") + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
                }
            }
            var contrato = repositorio.Obtener<Contrato>(oParam.Id);

            var cantidadMaxima = config.CantidadMaxima * 1000;
            if (oParam.ContratoAcuerdoId != null && oParam.ContratoAcuerdoId > 0)
            {
                var acuerdo = repositorio.Obtener<ContratoAcuerdo>(oParam.ContratoAcuerdoId);
                acuerdo.CantidadAmpliado = acuerdo.CantidadAmpliado ?? 0;
                var cantidadCargada = repositorio.Listar<Contrato>(d => oParam.Id != d.Id && d.ContratoAcuerdoId == oParam.ContratoAcuerdoId.Value && (d.EstadoId == (int)EnumEstadoContrato.Pendiente || d.EstadoId == (int)EnumEstadoContrato.Confirmado || d.EstadoId == (int)EnumEstadoContrato.Oferta || d.EstadoId == (int)EnumEstadoContrato.Con_Error || d.EstadoId == (int)EnumEstadoContrato.Finalizado || d.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Sum(d => d.Cantidad);
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == oParam.ContratoAcuerdoId.Value && (d.EstadoId == (int)EnumEstadoContrato.Pendiente || d.EstadoId == (int)EnumEstadoContrato.Confirmado || d.EstadoId == (int)EnumEstadoContrato.Oferta || d.EstadoId == (int)EnumEstadoContrato.Con_Error || d.EstadoId == (int)EnumEstadoContrato.Finalizado || d.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Sum(d => d.Cantidad);
                var tolerancia = config != null ? config.CantidadAcuerdo.Value * 1000 : 0;
                if (cantidadCargada + oParam.Cantidad > acuerdo.Cantidad + (tolerancia - acuerdo.CantidadAmpliado.Value))
                {
                    if (cantidadCargada + oParam.Cantidad > acuerdo.Cantidad + tolerancia - acuerdo.CantidadAmpliado)
                    {
                        oErrorMessages.Error("", "Cantidad del negocio mayor al saldo disponible del Acuerdo (" + (tolerancia - acuerdo.CantidadAmpliado.Value).ToString("N0") + " kg)");
                    }
                }
            }
            if (cantidadMaxima < oParam.Cantidad && !(oParam.EsFason == true || oParam.PrestamoDevolucion == true))
            {
                oErrorMessages.Error("", "Cantidad del negocio excedida (" + cantidadMaxima.ToString("N0") + " kg)");
            }
            if (oParam.StandardDeCalidadId == 0 || oParam.StandardDeCalidadId == null)
            {
                oErrorMessages.Error("", "Debe seleccionar alguna Calidad.");
            }
            if (oParam.MaterialId == (int)EnumMateriales.GIRASOL_AO && (oParam.ZonaId == 0 || oParam.ZonaId == null))
            {
                oErrorMessages.Error("", "Zona es obligatoria para Girasol Alto Oleico.");
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

                if (concepto != null && (concepto.Importe > 0 || concepto.Porcentaje > 0) && (oParam.MaterialId == (int)EnumMateriales.GIRASOL || oParam.MaterialId == (int)EnumMateriales.GIRASOL_AO))
                {
                    oErrorMessages.Error("", "En los negocios de Girasol la comisión debe ingresarse en descuentos y bonificaciones por fuera del precio.");
                }
            }
            if (oParam.MonedaId != "USDM " && oParam.AperturaPrecio != null && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
            {
                var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

                if (oParam.FechaCierta == null && oParam.PagoDiferido != true && concepto != null)
                {
                    oErrorMessages.Error("", "Días de diferimiento es obligatorio con el concepto financiero.");
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
                    if (!((concepto != null && oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value && (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0)) ||
                        (concepto == null && (!oParam.PagoDiferido.HasValue || (oParam.PagoDiferido.HasValue && !oParam.PagoDiferido.Value)) && (!oParam.DiasPesificado.HasValue || (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value == 0)))))
                    {
                        oErrorMessages.Error("", "Días de diferimiento/costo financiero es obligatorio con el pago diferido en pesos.");
                    }
                }
                //}
            }
            //else
            //{
            //    if (oParam.AperturaPrecio != null)
            //    {
            //        var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));

            //        if (concepto != null)
            //        {
            //            oErrorMessages.Error("", "El concepto financiero se debe completar solo cuando la moneda es ARP");
            //        }
            //    }

            //}

            if (PermisosHelper.ObtenerUsuario() != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                if (string.IsNullOrEmpty(oParam.PosicionCBOT) && oParam.TipoPosicionCBOTId != 3 && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Basis && (x.Importe != 0 || x.Porcentaje != 0)))
                {
                    oErrorMessages.Error("", "Se debe completar la Posición con el concepto Basis");
                }

                if (string.IsNullOrEmpty(oParam.PosicionCBOT) && oParam.TipoPosicionCBOTId != null && oParam.TipoPosicionCBOTId != 3)
                {
                    oErrorMessages.Error("", "Se debe completar el campo posición con CBOT o MAT");
                }

                if (string.IsNullOrEmpty(oParam.PosicionCBOT) && oParam.TipoPosicionCBOTId == 3)
                {
                    oErrorMessages.Error("", "Se debe completar el campo posición con a Fijar PASE");
                }
                if (!string.IsNullOrEmpty(oParam.PosicionCBOT) && (oParam.TipoPosicionCBOTId == null || oParam.TipoPosicionCBOTId == 0))
                {
                    oErrorMessages.Error("", "Se debe completar MAT, CBOT o PASE cuando esté completo el campo Posición.");
                }
                if (!string.IsNullOrEmpty(oParam.PosicionCBOT) && oParam.TipoPosicionCBOTId != 3 && (oParam.AperturaPrecio != null && !oParam.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Basis && (x.Importe != 0 || x.Porcentaje != 0))))
                {
                    oErrorMessages.Error("", "Se debe completar el concepto Basis con Posición CBOT o MAT.");
                }
            }

            if ((oParam.StandardDeCalidadId == (int)EnumStandarCalidad.ESPECIAL && oParam.Calidad == null))
            {
                oErrorMessages.Error("", "Se debe completar el campo Valor de Calidad.");
            }
            if (oParam.Calidad != null)
            {
                var calidad = oParam.Calidad.LastOrDefault(x => x.CalidadEspecialId == (int)EnumCalidadEspecial.DAÑADOS);
                if (calidad != null && calidad.PorcentajeHasta < 40)
                {
                    oErrorMessages.Error("", "Debe completar el rango de Dañados.");
                }
                calidad = oParam.Calidad.LastOrDefault(x => x.CalidadEspecialId == (int)EnumCalidadEspecial.GRANOS_VERDES);
                if (calidad != null && calidad.PorcentajeHasta < 100)
                {
                    oErrorMessages.Error("", "Debe completar el rango de Granos Verdes.");
                }
                if (calidad != null && calidad.Valor > 10)
                {
                    oErrorMessages.Error("", "El Valor no puede ser mayor a 10 en rango de Granos Verdes.");
                }
            }
            if (oParam.Dolarizado.HasValue && oParam.Dolarizado.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("Dolarizado", "Se debe completar la Fecha de pesificación en Negocios Dolarizados.");
            }
            if (oParam.DolarizadoExpress.HasValue && oParam.DolarizadoExpress.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("DolarizadoExpress", "Se debe completar la Fecha de pesificación en Negocios Dolarizados.");
            }

            if (oParam.Sustentable || oParam.EPA || oParam.EUDR)
            {
                if (oParam.MaterialId != (int)EnumMateriales.SOJA)
                {
                    oErrorMessages.Error("SustentableEPA", "Sustentable/EPA/EUDR solo está habilitado para el material Soja.");
                }
                if (oParam.MercsDeposito == true && oParam.SustentableTipoDBId.HasValue && oParam.SustentableTipoDBId.Value == 2)
                {
                    if (!oParam.FechaDesdeSustentable.HasValue || oParam.FechaDesdeSustentable.Value == null)
                    {
                        oErrorMessages.Error("SustentableEPA", "Debe indicar la fecha 'Desde' de sustentable/EPA/EUDR.");
                    }
                    if (!oParam.FechaHastaSustentable.HasValue || oParam.FechaHastaSustentable.Value == null)
                    {
                        oErrorMessages.Error("SustentableEPA", "Debe indicar la fecha 'Hasta' de sustentable/EPA/EUDR.");
                    }
                    if (oParam.FechaDesdeSustentable.HasValue && oParam.FechaHastaSustentable.HasValue
                        && oParam.FechaHastaSustentable.Value < oParam.FechaDesdeSustentable.Value)
                    {
                        oErrorMessages.Error("SustentableEPA", "Debe indicar un rango de fechas válido para sustentable/EPA/EUDR.");
                    }
                }
                if (oParam.EPA || oParam.EUDR)
                {
                    if (oParam.ImporteSustentable.HasValue && oParam.ImporteSustentable.Value > 0)
                    {
                        if (string.IsNullOrEmpty(oParam.MonedaSustentableId))
                        {
                            oErrorMessages.Error("EPA/EUDR", "Debe indicar la moneda para EPA/EUDR.");
                        }
                    }
                    else if (oParam.TarifaAConvenir != true)
                    {
                        oErrorMessages.Error("EPA/EUDR", "Debe indicar la tarifa para EPA/EUDR o tildar 'Tarifa a Convenir'.");
                    }
                    if (!oParam.SustentableTipoDBId.HasValue)
                    {
                        oErrorMessages.Error("EPA/EUDR", "Debe indicar si el importe para EPA/EUDR es sobre el precio o por fuera del precio.");
                    }
                }
                if (oParam.Sustentable)
                {
                    if (oParam.ImporteSustentable.HasValue && oParam.ImporteSustentable.Value > 0)
                    {
                        if (string.IsNullOrEmpty(oParam.MonedaSustentableId))
                        {
                            oErrorMessages.Error("Sustentable", "Debe indicar la moneda para Sustentable.");
                        }
                    }
                    else if (oParam.TarifaAConvenir != true)
                    {
                        oErrorMessages.Error("Sustentable", "Debe indicar la tarifa sustentable o tildar 'Tarifa a Convenir'.");
                    }
                    if (!oParam.SustentableTipoDBId.HasValue)
                    {
                        oErrorMessages.Error("Sustentable", "Debe indicar si el importe sustentable es sobre el precio o por fuera del precio.");
                    }
                }
            }
            var cantidadDias = PermisosHelper.Is(PermisosDataAgro.ModificarLimiteDolarizado) ? config.CantidadDiasDolarizadoLimiteMaximo : config.CantidadDias;
            if (oParam.FechaDolarizado != null)
            {
                if (config != null)
                {
                    var fechaLimite = oParam.FechaDesde.AddDays(30);
                    if (PermisosHelper.ObtenerUsuario() != null && !PermisosHelper.Is(PermisosDataAgro.ModificarLimiteDolarizado))
                    {
                        if (oParam.DolarizadoExpress == true && oParam.FechaDolarizado.Value > fechaLimite.Date)
                        {
                            oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado express debe ser menor o igual que los 30 días");
                        }
                    }
                    fechaLimite = oParam.FechaDesde.AddDays(cantidadDias);
                    if (oParam.FechaDolarizado.Value.Date > fechaLimite.Date)
                    {
                        oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado debe ser menor o igual que los " + cantidadDias + " días");
                    }
                    //var fechaLimite = oParam.FechaDesde.AddDays(cantidadDias);
                    //if (oParam.FechaDolarizado.Value.Date > fechaLimite.Date)
                    //{
                    //    oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado debe ser menor o igual que los " + cantidadDias + " días");
                    //}
                    //else
                    //{
                    //    fechaLimite = oParam.FechaDesde.AddDays(30);
                    //    if (oParam.DolarizadoExpress == true && oParam.FechaDolarizado.Value > fechaLimite.Date)
                    //    {
                    //        oErrorMessages.Error("Fecha Dolarizado", "La fecha dolarizado express debe ser menor o igual que los 30 días");
                    //    }
                    //}
                }
            }

            var fechaFijacion = oParam.HastaFijacion;
            var fechaAPrecio = oParam.FechaHasta.AddDays(cantidadDias);

            if (fechaFijacion.HasValue)
            {
                fechaFijacion = fechaFijacion.Value.AddDays(cantidadDias);
            }

            if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oParam.FechaDolarizado > fechaFijacion)
            {
                oErrorMessages.Error("dolarizado", "La fecha de pesificación no puede ser mayor a " + cantidadDias + " días de Fijación");
            }

            if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && oParam.FechaDolarizado > fechaAPrecio)
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

            if (oParam.FechaCierta != null && oParam.FechaCierta.Value < DateTime.Today)
            {
                oErrorMessages.Error("FechaCierta", "La Fecha Cierta debe ser mayor o igual al día de hoy.");
            }
            if (oParam.PorcentajeDePago == null || oParam.PorcentajeDePago.Value > 100 || oParam.PorcentajeDePago.Value < 0)
            {
                oErrorMessages.Error("PorcentajeDePago", "El Porcentaje de Pago debe estar entre 0 y 100.");
            }

            if (oParam.TipoAgenteCompraId != null)
            {
                if (string.IsNullOrEmpty(oParam.CaratulaMAT))
                {
                    oErrorMessages.Error("CaratulaMAT", "Debe completar Carátula MAT.");
                }
                if (oParam.PrecioAjusteComision == null)
                {
                    oErrorMessages.Error("PrecioAjusteComision", "Debe completar Precio Ajuste Comisión.");
                }
                if (string.IsNullOrEmpty(oParam.MonedaAjusteComisionId))
                {
                    oErrorMessages.Error("MonedaAjusteComisionId", "Debe completar Moneda Ajuste Comisión.");
                }
            }
            if (oParam.TipoAgenteCompraId == null && (oParam.CaratulaMAT != null || oParam.PrecioAjusteComision != null || oParam.MonedaAjusteComisionId != null || oParam.CaratulaExtension != null))
            {
                oErrorMessages.Error("TipoAgenteCompraId", "Debe seleccionar el agente de compra.");
            }

            if (oParam.FechaOperacion > DateTime.Today)
            {
                oErrorMessages.Error("FechaOperacion", "La fecha de operación tiene que ser menor o igual al día de hoy.");
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
                                oErrorMessages.Error("FechaOperacion", "La fecha de operación no puede ser anterior al último día hábil: " + diaAnterior.ToString("dd/MM/yyyy"));
                            }

                            if (string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior))
                            {
                                oErrorMessages.Error("MotivoOperacionAnterior", "Escriba el motivo por el cual la fecha de operación es anterior a hoy.");
                            }

                            if (!string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior) && oParam.DescripcionOperacionAnterior.Length <= 5)
                            {
                                oErrorMessages.Error("MotivoOperacionAnterior", "Es obligatorio escribir un motivo con más de 5 caracteres.");
                            }

                            if (oParam.Venta != true)
                            {
                                if ((oParam.NoInformaSio == null || oParam.NoInformaSio == false) && oParam.FechaOperacion < diaAnterior)
                                {
                                    oErrorMessages.Error("NoInformaSio", "La fecha de operación no puede ser anterior a " + diaAnterior.ToString("dd/MM/yyyy"));
                                }
                            }
                        }
                        if (oParam.FechaOperacion > contrato.Fecha.Date && oParam.Venta != true)
                        {
                            oErrorMessages.Error("NoInformaSio", "La fecha de operación no puede ser mayor a " + contrato.Fecha.ToString("dd/MM/yyyy"));
                        }
                    }
                    else
                    {
                        if (oParam.FechaOperacion < DateTime.Today)
                        {
                            var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(null);

                            if ((oParam.FechaOperacion < diaAnterior && !PermisosHelper.Is(PermisosDataAgro.NegociosFechaMayorDiaAnterior)
                                && oParam.PrestamoDevolucion != true && oParam.Canje != true && oParam.Venta != true))
                            {
                                oErrorMessages.Error("FechaOperacion", "La fecha de operación no puede ser anterior al ultimo día hábil." + diaAnterior.ToString("dd/MM/yyyy"));
                            }

                            if (string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior))
                            {
                                oErrorMessages.Error("MotivoOperacionAnterior", "Ingrese el motivo por el cual la fecha de operación es anterior al día de hoy.");
                            }

                            if (!string.IsNullOrEmpty(oParam.DescripcionOperacionAnterior) && oParam.DescripcionOperacionAnterior.Length <= 5)
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
                if (oParam.Canje == true)
                {
                    if (!oParam.Monto.HasValue)
                    {
                        oErrorMessages.Error("Monto", "Se debe completar el campo Monto cuando hay canje");
                    }
                    if (String.IsNullOrEmpty(oParam.MonedaCanjeId))
                    {
                        oErrorMessages.Error("Moneda", "Se debe completar la Moneda que corresponde al campo Canje");
                    }
                    if (String.IsNullOrEmpty(oParam.Insumo))
                    {
                        oErrorMessages.Error("Insumo", "Se debe completar el campo Insumo cuando hay Canje");
                    }
                }
                if (oParam.PrestamoDevolucion == true)
                {
                    if (!oParam.PlantaDestinoId.HasValue)
                    {
                        oErrorMessages.Error("PlantaDestino", "Se debe completar el campo Planta Destino cuando hay Préstamo Devolución");
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
                    oErrorMessages.Error("PagoCbu", "El CBU ingresado no es válido.");
                }
            }

            if (PermisosHelper.Is(PermisosDataAgro.ModificarCanje) && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                if (oParam.Canje != true)
                {
                    oErrorMessages.Error("Permiso Canje", "Es obligatorio completar el campo Canje");
                }
            }
            if (oParam.Descuentos != null)
            {
                foreach (var descuento in oParam.Descuentos)
                {
                    if (descuento.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO)
                    {
                        if ((oParam.MaterialId == (int)EnumMateriales.GIRASOL || oParam.MaterialId == (int)EnumMateriales.GIRASOL_AO) && (descuento.Porcentaje > 1 || descuento.Porcentaje < 0))
                        {
                            oErrorMessages.Error("Descuento", "El porcentaje debe estar entre 0% y 1%");
                        }
                    }
                }
            }
            if (contrato != null && !PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                if (contrato.DolarizadoTercero == true && oParam.Dolarizado != true && oParam.DolarizadoExpress != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Dolarizado que marco el tercero.");
                }

                if (contrato.SustentableTercero == true && !oParam.Sustentable && (!oParam.ImporteSustentable.HasValue || oParam.ImporteSustentable.Value == 0 || string.IsNullOrEmpty(oParam.MonedaSustentableId)))
                {
                    oErrorMessages.Error("Sustentable", "Debe indicar la tarifa de sustentable que marco el tercero.");
                }

                if (contrato.PagoDiferidoTercero == true && oParam.PagoDiferido != true)
                {
                    oErrorMessages.Error("Dolarizado", "Se debe completar Pago Diferido que marco el tercero.");
                }
                if (contrato.CalidadTercero == true)
                {
                    if (oParam.MaterialId == (int)EnumMateriales.MAIZ && oParam.StandardDeCalidadId == 2)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == (int)EnumMateriales.TRIGO && oParam.StandardDeCalidadId == 7)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == (int)EnumMateriales.SOJA && oParam.StandardDeCalidadId == 4)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == (int)EnumMateriales.GIRASOL && oParam.StandardDeCalidadId == 5)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                    if (oParam.MaterialId == (int)EnumMateriales.GIRASOL_AO && oParam.StandardDeCalidadId == 5)
                    {
                        oErrorMessages.Error("Calidad", "Se debe completar Calidad que marco el tercero.");
                    }
                }
            }

            if (oParam.DolarizadoCorredor != true && oParam.Dolarizado != true && oParam.DolarizadoExpress != true && oParam.FechaDolarizado != null)
            {
                oErrorMessages.Error("Dolarizado", "Se debe completar Dolarizado si completó Fecha límite.");
            }

            //    if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oParam.Descuentos != null && oParam.Descuentos.Any(a => a.Importe != 0 && a.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO))
            //    {
            //        oErrorMessages.Error("Descuentos y Bonificaciones", "No se puede completar importe en un descuento o bonificacion fuera de precio.");
            //    }

            if ((oParam.MaterialId == (int)EnumMateriales.GIRASOL || oParam.MaterialId == (int)EnumMateriales.GIRASOL_AO) && oParam.Descuentos != null && oParam.Descuentos.Any(a => a.Porcentaje > 0 && a.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO))
            {
                oErrorMessages.Error("Descuentos y Bonificaciones", "En los negocios de Girasol la comisión debe ingresarse en descuentos y bonificaciones por fuera del precio.");
            }

            if (oParam.PagoDiferido == true && oParam.DiasPesificado != null)
            {
                //var conf = configuracionManager.TraerConfiguraciones();
                if (config != null)
                {
                    var limitePesificado = /*PermisosHelper.Is(PermisosDataAgro.ModificarLimitePesificado) ? config.CantidadDiasPesificadoLimite :*/ config.DiasDiferimiento;
                    if (oParam.DiasPesificado.Value > limitePesificado)
                    {
                        oErrorMessages.Error("Pago Diferido", "Los días de pesificado deben ser menor o igual que los " + limitePesificado + " días.");
                    }
                }
            }

            if (contrato != null && PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && contrato.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
            {
                oErrorMessages.Error("Contrato", "No se puede modificar el contrato.");
            }

            if (!repositorio.Existe<Localidad>(a => a.LocalidadId == oParam.LocalidadId))
            {
                oErrorMessages.Error("Localidad", "La localidad seleccionada no es válida.");
            }
            if (!repositorio.Existe<Provincia>(a => a.ProvinciaId == oParam.ProvinciaId))
            {
                oErrorMessages.Error("Provincia", "La provincia seleccionada no es válida.");
            }

            if (oParam.Descuentos != null)
            {
                if (oParam.Descuentos.Any(a => a.FechaDesde != null && a.FechaHasta != null && a.FechaDesde > a.FechaHasta))
                {
                    oErrorMessages.Error("Descuentos", "La fecha 'Desde' de descuento o bonificación no puede ser mayor a la fecha 'Hasta'.");
                }
                if (oParam.Descuentos.Any(a => (a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION || a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_ENTREGA) && (a.FechaDesde == null || a.FechaHasta == null)))
                {
                    oErrorMessages.Error("Descuentos", "La fecha 'Desde' y 'Hasta' de descuento o bonificación es obligatoria.");
                }

                if (oParam.Descuentos.Any(a => a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION || a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_ENTREGA) && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Fijación o de Entrega en un negocio a precio.");
                }

                if (oParam.Descuentos.Any(a => a.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO && (a.Importe > 0 || a.Porcentaje > 0)) && oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && oParam.Precio > 0)
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificación Sobre Precio cuando tiene precio.");
                }

                if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oParam.Descuentos.Any(a => a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_FIJACION && (a.FechaDesde < oParam.DesdeFijacion || a.FechaDesde > oParam.HastaFijacion || a.FechaHasta < oParam.DesdeFijacion || a.FechaHasta > oParam.HastaFijacion)))
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Fijación fuera del rango de Fijación.");
                }
                if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oParam.Descuentos.Any(a => a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.POR_FECHA_DE_ENTREGA && (a.FechaDesde < oParam.FechaDesde || a.FechaDesde > oParam.FechaHasta || a.FechaHasta < oParam.FechaDesde || a.FechaHasta > oParam.FechaHasta)))
                {
                    oErrorMessages.Error("Descuentos", "No se puede cargar descuento o bonificación por Fecha de Entrega fuera del rango de Entrega.");
                }
            }
            if (oParam.ContratoCorredor != null && oParam.ContratoCorredor.Length > 10)
            {
                oErrorMessages.Error("ContratoCorredor", "El número de contrato corredor no puede superar los 10 caracteres.");
            }
            if (oParam.ContratoVendedor != null && oParam.ContratoVendedor.Length > 10)
            {
                oErrorMessages.Error("ContratoVendedor", "El número de contrato vendedor no puede superar los 10 caracteres.");
            }
            if (oParam.AnulaYReemplazaContratoId != null && string.IsNullOrEmpty(oParam.MotivoReemplazo))
            {
                oErrorMessages.Error("MotivoReemplazo", "Debe completar el motivo de reemplazo.");
            }
            if (oParam.Venta == true)
            {
                //if (oParam.CondicionDePagoDiaFijacion > 0 && (string.IsNullOrEmpty(oParam.CondicionDePagoTipoFijacion) || oParam.CondicionDePagoFijacionVentaId == null))
                //{
                //    oErrorMessages.Error("DiaVenta", "Debe completar la condición de pago.");
                //}

                //if (!string.IsNullOrEmpty(oParam.CondicionDePagoTipoFijacion) && (!oParam.CondicionDePagoDiaFijacion.HasValue || oParam.CondicionDePagoDiaFijacion.Value <= 0))
                //{
                //    oErrorMessages.Error("DiaVenta", "Debe completar los días en la condición de pago.");
                //}

                //if (oParam.CondicionDePagoFijacionVentaId.HasValue && oParam.CondicionDePagoFijacionVentaId > 0 && (!oParam.CondicionDePagoDiaFijacion.HasValue || oParam.CondicionDePagoDiaFijacion.Value <= 0))
                //{
                //    oErrorMessages.Error("DiaVenta", "Debe completar los días en la condición de pago.");
                //}

                //if (!oParam.CondicionDePagoPesificadoVentaId.HasValue && oParam.MonedaId == "USDM ")
                //{
                //    oErrorMessages.Error("DiaVenta", "La condicion de pesificación en 'Condiciones Adicionales de Venta' es obligatoria con la moneda USD.\n\n");
                //}

                //if (oParam.MonedaId == "USDM ")
                //{
                //    if (oParam.CondicionDePagoDiaPesificado > 0 && (string.IsNullOrEmpty(oParam.CondicionDePagoTipoPesificado) || oParam.CondicionDePagoPesificadoVentaId == null))
                //    {
                //        oErrorMessages.Error("DiaVenta", "Debe completar la condición de pesificación.");
                //    }

                //    if (!string.IsNullOrEmpty(oParam.CondicionDePagoTipoPesificado) && (!oParam.CondicionDePagoDiaPesificado.HasValue || oParam.CondicionDePagoDiaPesificado.Value <= 0))
                //    {
                //        oErrorMessages.Error("DiaVenta", "Debe completar los días en la condición de pesificación.");
                //    }

                //    if (oParam.CondicionDePagoPesificadoVentaId.HasValue && oParam.CondicionDePagoPesificadoVentaId > 0 && (!oParam.CondicionDePagoDiaPesificado.HasValue || oParam.CondicionDePagoDiaPesificado.Value <= 0))
                //    {
                //        oErrorMessages.Error("DiaVenta", "Debe completar los días en la condición de pesificación.");
                //    }
                //}

                if (oParam.BoletoVentaId == 5 && string.IsNullOrEmpty(oParam.MailVentaBoleto))
                {
                    oErrorMessages.Error("DiaVenta", "El campo mail es obligatorio cuando el boleto es a convenir.");
                }

                //if (oParam.ComisionAFavorId > 0 && (!oParam.PorcentajeComisionVenta.HasValue ||
                //    (oParam.PorcentajeComisionVenta.HasValue && oParam.PorcentajeComisionVenta.Value <= 0)))
                //{
                //    oErrorMessages.Error("ComisionAFavor", "Debe completar el porcentaje de comisión en las condiciones de venta cuando Comisión a Favor está completo.\n\n");
                //}

                //if (oParam.ComisionAFavorId == null && (oParam.PorcentajeComisionVenta.HasValue ||
                //  (oParam.PorcentajeComisionVenta.HasValue && oParam.PorcentajeComisionVenta.Value > 0)))
                //{
                //    oErrorMessages.Error("ComisionAFavor", "Debe completar la Comisión a Favor en las condiciones de venta cuando Porcentaje de Comisión está completo.\n\n");
                //}

                //if (oParam.CamaraId == null)
                //{
                //    oErrorMessages.Error("Camara", "El campo Camara es obligatorio");
                //}
                //if (oParam.ProcedenciaVentaId == null)
                //{
                //    oErrorMessages.Error("ProcedenciaVentaId", "El campo Destino de la Mercadería es obligatorio.");
                //}
                //if (string.IsNullOrEmpty(oParam.FleteACargo))
                //{
                //    oErrorMessages.Error("FleteACargo", "El campo Flete a Cargo es obligatorio.");
                //}
                //if (string.IsNullOrEmpty(oParam.KgBalanza))
                //{
                //    oErrorMessages.Error("KgBalanza", "El campo Kg Balanza es obligatorio.");
                //}
                //if (oParam.ComisionAFavorId == null)
                //{
                //    oErrorMessages.Error("ComisionAFavorId", "El campo Comisión a Favor es obligatorio");
                //}
                //if (oParam.PorcentajeComisionVenta == null)
                //{
                //    oErrorMessages.Error("PorcentajeComisionVenta", "El campo Porcentaje Comision Venta es obligatorio");
                //}
                //if (string.IsNullOrEmpty(oParam.Pago))
                //{
                //    oErrorMessages.Error("Pago", "El campo Pago es obligatorio.");
                //}
                //if (oParam.BoletoVentaId == null)
                //{
                //    oErrorMessages.Error("BoletoVentaId", "El campo Boleto es obligatorio.");
                //}
                //if (oParam.CondicionDePagoDiaPesificado == null)
                //{
                //    oErrorMessages.Error("CondicionDePagoDiaPesificado", "El campo 'Cantidad de días' de Condición de pesificación es obligatorio.");
                //}
                //if (string.IsNullOrEmpty(oParam.CondicionDePagoTipoPesificado))
                //{
                //    oErrorMessages.Error("CondicionDePagoTipoPesificado", "El campo 'Condición' de Condición de pesificación es obligatorio.");
                //}
                //if (oParam.CondicionDePagoPesificadoVentaId == null)
                //{
                //    oErrorMessages.Error("CondicionDePagoPesificadoVentaId", "El campo 'Plazo' de Condición de pesificación es obligatorio.");
                //}

                //if (oParam.FechaCierta.HasValue)
                //{
                //    if (oParam.CondicionDePagoDiaFijacion == null)
                //    {
                //        oErrorMessages.Error("CondicionDePagoDiaFijacion", "El campo 'Cantidad de días' de Condición de Pago es obligatorio.");
                //    }
                //    if (string.IsNullOrEmpty(oParam.CondicionDePagoTipoFijacion))
                //    {
                //        oErrorMessages.Error("CondicionDePagoTipoFijacion", "El campo 'Condición' de Condición de Pago es obligatorio.");
                //    }
                //    if (oParam.CondicionDePagoFijacionVentaId == null)
                //    {
                //        oErrorMessages.Error("CondicionDePagoFijacionVentaId", "El campo 'Plazo' de Condicion de Pago es obligatorio.");
                //    }
                //}

                var provinciaElegida = repositorio.Obtener<Localidad, int>(x => x.LocalidadId == oParam.ProcedenciaVentaId, x => x.ProvinciaId);
                var habilitadoVenta = repositorio.Obtener<Provincia, bool>(x => x.ProvinciaId == provinciaElegida, x => x.HabilitadoVenta);
                if (!habilitadoVenta)
                {
                    oErrorMessages.Error("HabilitadoVenta", "La provincia elegida como destino de la mercadería no está habilitada para ventas.\n\n");
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

                if (oParam.CorredorId.HasValue && corredor != null)
                {
                    var contactosCorredor = repositorio.Listar<ContactoComercial>(x => x.ProveedorId == oParam.CorredorId);
                    if (!string.IsNullOrEmpty(corredor.Email1))
                    {
                        listaEmail.Add(corredor.Email1);
                    }
                    if (!string.IsNullOrEmpty(corredor.Email2))
                    {
                        listaEmail.Add(corredor.Email2);
                    }
                    if (!string.IsNullOrEmpty(corredor.Email3))
                    {
                        listaEmail.Add(corredor.Email3);
                    }
                    if (!string.IsNullOrEmpty(corredor.Email4))
                    {
                        listaEmail.Add(corredor.Email4);
                    }
                    foreach (var contacto in contactosCorredor)
                    {
                        if (!string.IsNullOrEmpty(contacto.Email1))
                            listaEmail.Add(contacto.Email1);
                        if (!string.IsNullOrEmpty(contacto.Email2))
                            listaEmail.Add(contacto.Email2);
                        if (!string.IsNullOrEmpty(contacto.Email3))
                            listaEmail.Add(contacto.Email3);
                    }
                }

                if (!listaEmail.Any(x => x.Contains(oParam.UsuarioTercero)))
                {
                    oErrorMessages.Error("Usuario no registrado", "El usuario no esta habilitado para cargar negocios, por favor comunicarse con su comercial");
                }
            }

            if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
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
            if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && oParam.AperturaPrecio != null && oParam.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho))
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

            if (oParam.Insumo != null && oParam.Insumo.Length > 250)
            {
                oErrorMessages.Error("Insumo", "El campo Insumo no debe superar los 250 caracteres.");
            }

            if (oParam.Condicional == true)
            {
                var condicional = oParam.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId == 4 && a.Importe > 0).SingleOrDefault();
                if (condicional == null)
                {
                    oErrorMessages.Error("Condicional", "En negocios Condicionales debe completar el concepto de condicional en Apertura de Precio.");
                }
                if (oParam.CondicionalPrecio == null || oParam.CondicionalPrecio <= 0)
                {
                    oErrorMessages.Error("Condicional", "Debe completar el campo Precio Strike en Condicional.");
                }
                if (oParam.CondicionalCantidad == null || oParam.CondicionalCantidad <= 0)
                {
                    oErrorMessages.Error("Condicional", "Debe completar el campo Cantidad en Condicional.");
                }
                else
                {
                    if (oParam.CondicionalCantidad < oParam.Cantidad)
                    {
                        oErrorMessages.Error("Condicional", "Los Cantidad en Condicional debe ser mayor o igual a la Cantidad del contrato.");
                    }
                }

                if (oParam.CondicionalFecha == null)
                {
                    oErrorMessages.Error("Condicional", "Debe completar el campo Fecha en Condicional.");
                }
                if (string.IsNullOrEmpty(oParam.CondicionalPosicion))
                {
                    oErrorMessages.Error("Condicional", "Debe completar el campo Posicion en Condicional.");
                }
            }

            if (oParam.CondicionalContratoId.HasValue)
            {
                if (repositorio.Existe<Contrato>(x => x.CondicionalContratoId == oParam.CondicionalContratoId && x.Id != oParam.Id && (x.EstadoId != (int)EnumEstadoContrato.Rechazado && x.EstadoId != (int)EnumEstadoContrato.Eliminado)))
                {
                    oErrorMessages.Error("Condicional", "El contrato Condicional ya fue cargado.");
                }
            }

            if (oParam.MaterialId == (int)EnumMateriales.SOJA && oParam.Sustentable == true)
            {
                var campania = repositorio.Obtener<Campaña>(oParam.CampanaId);
                var material = repositorio.Obtener<Material>(oParam.MaterialId);

                if (oParam.CampanaId < material.CampañaId)
                {
                    oErrorMessages.Error("Campaña", $"En negocios sustentables la campaña elegida no puede ser menor a la campaña actual de soja ({material.Campaña.Descripcion}).");
                }

                if (campania.Hasta != null && oParam.FechaHasta > campania.Hasta.Value)
                {
                    oErrorMessages.Error("Condicional", "La fecha de entrega no puede abarcar días posteriores al " + campania.Hasta.Value.ToString("dd-MM-yyyy") + " para la campaña " + campania.Descripcion);
                }
            }
            //if (oParam.Id > 0 && oParam.Condicional == true && oParam.EstadoId == (int)EnumEstadoContrato.Finalizado)
            //{
            //    if (contrato != null && contrato.CondicionalContratos.Any(x => x.EstadoId == (int)EnumEstadoContrato.Finalizado))
            //    {
            //        if (contrato.CondicionalCantidad != oParam.CondicionalCantidad || contrato.CondicionalPrecio != oParam.CondicionalPrecio || contrato.ProveedorId != oParam.ProveedorId || contrato.CorredorId != oParam.CorredorId)
            //        {
            //            var segundoContrato = contrato.CondicionalContratos.Where(x => x.EstadoId == (int)EnumEstadoContrato.Finalizado).FirstOrDefault();
            //            var res = status.ValidarEstado(segundoContrato.ContratoSAP);
            //            var estado = (string.IsNullOrEmpty(res.Status) && res.NumeroSio == 0) ? "" :
            //                "El contrato asociado al condicional ya no se encuentra en slip o fue informado a SIO granos";
            //            if (estado != "")
            //            {
            //                oErrorMessages.Error("Condicional", estado);
            //            }
            //        }

            //    }

            //}
            if (!repositorio.Existe<Localidad>(a => a.LocalidadId == oParam.LocalidadId && a.ProvinciaId == oParam.ProvinciaId))
            {
                oErrorMessages.Error("Localidad", "La localidad ingresada no corresponde a la provincia.");
            }
            if (oParam.TipoPosicionCBOTId == 3 && oParam.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO)
            {
                oErrorMessages.Error("Posición", "No se puede crear un contrato SIN BOLETO con POSICION PASE.");
            }

            //if (oParam.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && oParam.TipoAgenteCompraId != null)
            //{
            //    if (ValidarFechaAgenteMP(oParam))
            //    {
            //        oErrorMessages.Error("FechaOperacion", "La fecha de operación para Agente de Compras MP no puede ser uno de los últimos 5 días hábiles del mes.");
            //    }
            //}

            return oErrorMessages;
        }

        public GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
            if (oContratoSave.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && oContratoSave.Venta == true)
            {
                oContrato.Ampliaciones = oContrato.Ampliaciones * -1;
            }
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
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == oContratoSave.ContratoAcuerdoId.Value && (d.EstadoId == (int)EnumEstadoContrato.Pendiente || d.EstadoId == (int)EnumEstadoContrato.Confirmado || d.EstadoId == (int)EnumEstadoContrato.Oferta || d.EstadoId == (int)EnumEstadoContrato.Con_Error || d.EstadoId == (int)EnumEstadoContrato.Finalizado || d.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Sum(d => d.Cantidad);
                var tolerancia = (config != null ? config.CantidadAcuerdo.Value * 1000 : 0);
                var totalAcuerdo = cantidadAcuerdo + tolerancia - (cantidadAmpliadoAcuerdo ?? 0);
                if (cantidadMaxima < cantidadTodoAcuerdo + oContrato.Ampliaciones && (oContratoSave.EsFason == true || oContratoSave.PrestamoDevolucion == true))
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

        public GrabarContratoResult GrabarContrato(Contrato oContrato, List<CupoConDescargaFechasDto> listCupoConDescargaFechas = null)
        {
            var oEntityErrors = new GrabarContratoResult();
            Validar(oContrato, oEntityErrors);

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            var listaServicioValor = repositorio.Listar<ServicioValor>();
            List<int> listaFiltradaServicioValor = new List<int>();
            if (listaServicioValor != null)
            {
                List<ServicioValor> listaFiltrada = listaServicioValor
                    .Where(x => x.MaterialId == oContrato.MaterialId && x.CentroId == oContrato.DestinoId).ToList();

                listaFiltrada.ForEach(x => listaFiltradaServicioValor.Add(x.Id));
            }

            var oContratoSave = new Contrato();
            if (oContrato.Id > 0) oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
            double kilosParametro = oContrato.Cantidad;

            Cupo cupoNuevo = null;
            if (oContrato.ConDescarga == true)
            {
                oEntityErrors.Errores = negocioManager.ControlesAccesoConDescarga(oContrato).Errores;
                if (oEntityErrors.Errores.Count > 0) return oEntityErrors;
            }

            if (listCupoConDescargaFechas != null)
            {
                if (oContrato.Id > 0 && kilosParametro >= oContratoSave.Cantidad)
                {
                    var cuposExistentes = repositorio.Contar<Cupo>(x => x.NegocioId == oContrato.Id);
                    oContrato.Cantidad = kilosParametro - (30000 * cuposExistentes); //conservo la cantidad que aún no tiene cupos
                }
                cupoNuevo = negocioManager.TransformarContratoACupo(oContrato);

                int sumaCuposCargaMasiva = 0;
                bool cargaMasiva = listCupoConDescargaFechas != null && listCupoConDescargaFechas.Count() > 0;
                if (cargaMasiva) sumaCuposCargaMasiva = listCupoConDescargaFechas.Sum(x => x.CantidadCupo) + listCupoConDescargaFechas.Sum(x => x.CantidadFlete);

                var error = cupoManager.Validar(cupoNuevo, sumaCuposCargaMasiva, oContrato.FechaHasta);
                if (error != null)
                {
                    oEntityErrors.Errores.AddRange(error.Errores);
                    oEntityErrors.ListaErrores.AddRange(error.ListaErrores);
                }

                // Validar disponibilidad según LIMITE CUPO CON DESCARGA
                listCupoConDescargaFechas.ForEach(x =>
                {
                    int sumaCuposPorFecha = x.CantidadCupo + x.CantidadFlete;

                    CupoResult cupoResult = cupoManager.ValidarDisponibilidadCuperaConDescarga(oContrato.MaterialId, (int)oContrato.DestinoId, x.Fecha, sumaCuposPorFecha);
                    if (cupoResult.HayError)
                        cupoResult.Errores.ForEach(y => oEntityErrors.Errores.Add(new ErrorMessage(400, y.Message)));
                });

                var cantidadCuposFletesPermitidos = Math.Ceiling(oContrato.Cantidad / 30000);
                if (cantidadCuposFletesPermitidos < sumaCuposCargaMasiva)
                {
                    string mensaje = oContrato.Id == 0 || kilosParametro == oContratoSave.Cantidad ? ".\n\n" : " que aún no tienen cupos.\n\n";
                    oEntityErrors.Errores.Add(new ErrorMessage(400, "La cantidad de cupos ingresada se excede con respecto a los kilos del negocio" + mensaje));
                }

                oContrato.Cantidad = kilosParametro;
            }

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            List<DescuentoBonificacion> descuentosExistentes = null;
            List<Calidad> calidadesExistentes = null;
            List<AperturaPrecio> aperturasExistentes = null;
            List<PrecioPactado> preciosExistentes = null;
            List<Servicio> serviciosExistentes = null;
            var hoy = DateTime.Now;

            if (oContrato.Id != 0)
            {
                if ((oContrato.ChequeElectronico != oContratoSave.ChequeElectronico && oContrato.ChequeElectronico.Value) || oContratoSave.PagoCBU != oContrato.PagoCBU)
                {
                    var result = validarPagoAgente.ValidarEstado(oContratoSave.ContratoSAP, "");
                    if (result != "Ok")
                    {
                        oEntityErrors.Error("", result);
                        return oEntityErrors;
                    }
                }
                var validacionServicioModificado = ValidarServicioModificado(oContrato, oContratoSave);
                descuentosExistentes = oContratoSave.Descuentos.ToList();
                calidadesExistentes = oContratoSave.Calidad.ToList();
                aperturasExistentes = oContratoSave.AperturaPrecio.ToList();
                preciosExistentes = oContratoSave.PrecioPactado.ToList();
                serviciosExistentes = oContratoSave.Servicios != null ? oContratoSave.Servicios.ToList() : new List<Servicio>();

                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }
                if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oContratoSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }

                if (oContrato.Servicios != null && oContrato.Servicios.Count > 0)
                {
                    logger.Debug($"GrabarServicioModificado() para: id {oContrato.Id}");
                    GrabarServicioModificado(oContrato.Servicios.ToList(), oContrato.MaterialId, oContrato.DestinoId ?? 0);

                    oContrato.Servicios = oContrato.Servicios.Where(x => listaFiltradaServicioValor.Contains(x.ServicioValorId)).ToList();
                }

                if ((oContratoSave.Precio != oContrato.Precio || oContratoSave.Cantidad != oContrato.Cantidad || oContratoSave.DesdeFijacion != oContrato.DesdeFijacion ||
                    oContratoSave.MonedaId != oContrato.MonedaId || oContratoSave.HastaFijacion != oContrato.HastaFijacion
                    || ValidarCalidadModificada(oContrato, oContratoSave)) || validacionServicioModificado && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente
                    && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta && oContratoSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion))
                {
                    if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
                    {
                        oContrato.EstadoId = 7;
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
                if (oContrato.Servicios != null && oContrato.Servicios.Count > 0)
                {
                    logger.Debug($"GrabarServicioModificado() para: id {oContrato.Id}, proveedor {oContrato.ProveedorId}, destino {oContrato.DestinoId}, fecha-desde {oContrato.FechaDesde}, cantidad {oContrato.Cantidad}, comercial {oContrato.ComercialId}");
                    GrabarServicioModificado(oContrato.Servicios.ToList(), oContrato.MaterialId, oContrato.DestinoId ?? 0);

                    oContrato.Servicios = oContrato.Servicios.Where(x => listaFiltradaServicioValor.Contains(x.ServicioValorId)).ToList();
                }
            }

            var confirmacionAutomatica = (oContrato.EstadoId < (int)EnumEstadoContrato.PreAprobacion || (!PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno) && oContrato.EstadoId == (int)EnumEstadoContrato.PreAprobacion && oContrato.Id > 0))
                && ConfirmacionAutomatica(oContrato) && DateTime.Today == oContrato.FechaOperacion.Date;

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
            oContratoSave.Venta = oContrato.Venta;
            if (oContratoSave.Venta == true)
                oContratoSave.Cantidad = -Math.Abs(oContratoSave.Cantidad);

            oContratoSave.Precio = oContrato.Precio;
            oContratoSave.FechaEntrega = oContrato.FechaEntrega;
            oContratoSave.CampanaId = oContrato.CampanaId;
            oContratoSave.FechaDesde = oContrato.FechaDesde;
            oContratoSave.FechaHasta = oContrato.FechaHasta;
            oContratoSave.ProveedorId = oContrato.ProveedorId;
            oContratoSave.MonedaId = oContrato.MonedaId ?? (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? "USDM " : null);
            oContratoSave.GrupoCompra = oContrato.GrupoCompra;
            oContratoSave.ComercialId = oContrato.ComercialId;
            oContratoSave.LocalidadId = oContrato.LocalidadId;
            oContratoSave.ProvinciaId = oContrato.ProvinciaId;
            oContratoSave.Base = oContrato.Base;
            oContratoSave.ImporteSustentable = oContrato.ImporteSustentable;
            oContratoSave.MonedaSustentableId = oContrato.MonedaSustentableId;
            oContratoSave.FechaDesdeSustentable = oContrato.FechaDesdeSustentable;
            oContratoSave.FechaHastaSustentable = oContrato.FechaHastaSustentable;
            oContratoSave.TarifaAConvenir = oContrato.TarifaAConvenir;
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
            oContratoSave.CantidadDeposito = oContrato.CantidadDeposito;
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
            oContratoSave.EPA = oContrato.EPA;
            oContratoSave.EUDR = oContrato.EUDR;
            oContratoSave.SustentableTipoDBId = oContrato.EPA || oContrato.Sustentable || oContrato.EUDR ? oContrato.SustentableTipoDBId : null;
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
            oContratoSave.DescripcionOperacionAnterior = oContrato.DescripcionOperacionAnterior;
            oContratoSave.ChequeElectronico = oContrato.ChequeElectronico;
            oContratoSave.DolarizadoExpress = oContrato.DolarizadoExpress;
            oContratoSave.PagoCBU = oContrato.PagoCBU;
            oContratoSave.Canje = oContrato.Canje;
            oContratoSave.MonedaCanjeId = oContrato.MonedaCanjeId;
            oContratoSave.Monto = oContrato.Monto;
            oContratoSave.Insumo = oContrato.Insumo;
            oContratoSave.PrestamoDevolucion = oContrato.PrestamoDevolucion;
            oContratoSave.PlantaDestinoId = oContrato.PlantaDestinoId;
            oContratoSave.ObligatoriedadCostoFinanciero = oContrato.FechaCierta.HasValue &&
             oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
             oContrato.ObligatoriedadCostoFinanciero.HasValue && !oContrato.ObligatoriedadCostoFinanciero.Value
             ? null : oContrato.FechaCierta.HasValue ? oContrato.ObligatoriedadCostoFinanciero : null;
            oContratoSave.PosicionCBOT = oContrato.PosicionCBOT;
            oContratoSave.TipoPosicionCBOTId = oContrato.TipoPosicionCBOTId;
            oContratoSave.AnulaYReemplazaContratoId = oContrato.AnulaYReemplazaContratoId;
            oContratoSave.MotivoReemplazo = oContrato.MotivoReemplazo;

            oContratoSave.ObligatoriedadBonificacion = oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 &&
             oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0)) &&
             oContrato.ObligatoriedadBonificacion.HasValue && !oContrato.ObligatoriedadBonificacion.Value
             ? null : oContrato.TipoPosicionCBOTId.HasValue && oContrato.TipoPosicionCBOTId == 3 ? oContrato.ObligatoriedadBonificacion : null;
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
            oContratoSave.BoletoVentaId = oContrato.BoletoVentaId;
            oContratoSave.MailVentaBoleto = oContrato.MailVentaBoleto;

            oContratoSave.Condicional = oContrato.Condicional;
            oContratoSave.CondicionalCantidad = oContrato.CondicionalCantidad;
            oContratoSave.CondicionalFecha = oContrato.CondicionalFecha;
            oContratoSave.CondicionalMonedaId = oContrato.CondicionalMonedaId;
            oContratoSave.CondicionalPosicion = oContrato.CondicionalPosicion;
            oContratoSave.CondicionalPrecio = oContrato.CondicionalPrecio;
            oContratoSave.CondicionalContratoId = oContrato.CondicionalContratoId;
            oContratoSave.KgMinimo = oContrato.KgMinimo;
            oContratoSave.KgMaximo = oContrato.KgMaximo;
            oContratoSave.ProveedorComisionistaId = oContrato.ProveedorComisionistaId;

            var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.ProveedorId, x => x.CUIT);
            oContratoSave.MonedaCreditoDisponible = validarCreditoAgente.ValidarCredito(cuit).Moneda;
            oContratoSave.ConDescarga = oContrato.ConDescarga;
            oContratoSave.DolarExportador = oContrato.DolarExportador;

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
                        //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta))
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
                    //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta))
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
                        //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta))
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
                    //if (oContratoSave.Id != 0 && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta))
                    //{
                    //    oContratoSave.EstadoId = 7;
                    //}
                }
            }

            if (serviciosExistentes != null)
            {
                foreach (var servExistente in serviciosExistentes)
                {
                    repositorio.Remover(servExistente);
                }
            }
            if (oContrato.Servicios != null)
            {
                foreach (var servicio in oContrato.Servicios)
                {
                    servicio.Negocio = oContratoSave;
                    repositorio.Agregar(servicio);
                }
            }
            if (oContrato.Servicios != null && oContrato.Servicios.Count > 0)
            {
                logger.Debug($"GrabarServicioModificado para ID {oContrato.Id}, proveedor {oContrato.ProveedorId}, destino {oContrato.DestinoId}, fecha-desde {oContrato.FechaDesde}, cantidad {oContrato.Cantidad}, comercial {oContrato.ComercialId}");
                GrabarServicioModificado(oContrato.Servicios.ToList(), oContrato.MaterialId, oContrato.DestinoId ?? 0);

                oContrato.Servicios = oContrato.Servicios.Where(x => listaFiltradaServicioValor.Contains(x.ServicioValorId)).ToList();
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
                logger.Debug("El contrato " + oContrato.Id + " se confirmó automaticamente por estar dentro de los rangos configurados");
            }
            if (PermisosHelper.Is(PermisosDataAgro.NuevoNegocioExterno))
            {
                oContratoSave.EstadoId = (int)EnumEstadoContrato.PreAprobacion;
            }
            var tipoDeLog = (oContratoSave.Id == 0 || string.IsNullOrEmpty(oContratoSave.ContratoSAP)) ? TipoAccionLogDataAgro.Crear : TipoAccionLogDataAgro.Modificar;
            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), tipoDeLog, oContratoSave.GetType());

            #region CREAR CUPOS CON DESCARGA

            if (listCupoConDescargaFechas != null)
            {
                cupoNuevo.NegocioId = oContratoSave.Id;
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

            #endregion CREAR CUPOS CON DESCARGA

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
                    logger.Error("No se pudo ValidarComprasDiferencial en GrabarContrato", ex);
                }
            }

            return oEntityErrors;
        }

        private bool ConfirmacionAutomatica(Contrato contrato)
        {
            var hoy = DateTime.Now;
            var precioContrato = contrato.Precio;
            List<int> tipoRangos = new List<int>() { (int)EnumTipoRangoConfirmacionAutomatica.ConfirmacionYReconfirmacion };
            if (contrato.EstadoId == (int)EnumEstadoContrato.Pendiente)
            {
                tipoRangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Confirmacion);
            }
            else
            {
                tipoRangos.Add((int)EnumTipoRangoConfirmacionAutomatica.Reconfirmacion);
            }
            var rangos = repositorio.Listar<RangoConfirmacionAutomatica>(x =>
            (x.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecio || x.TipoNegocioId == (int)EnumTipoNegocioRangoConfirmacionAutomatica.APrecioYFijacion) &&
            x.FechaDesde <= hoy &&
            x.FechaHasta >= hoy &&
            x.MaterialId == contrato.MaterialId &&
            x.MonedaId == contrato.MonedaId &&
            precioContrato >= x.PrecioMinimo && precioContrato <= x.PrecioMaximo
            && tipoRangos.Contains(x.TipoRangoId)) ?? new List<RangoConfirmacionAutomatica>();

            var rango = rangos.FirstOrDefault(
                x => contrato.FechaDesde >= x.DesdeEntrega &&
                   contrato.FechaHasta <= x.HastaEntrega);

            if (rango != null && contrato.Servicios != null && !contrato.Servicios.Any(x => x.Modificado == true))
            {
                var grupo = repositorio.Obtener<Comercial, int>(x => x.ComercialId == contrato.ComercialId, x => x.GrupoDeComprasId.Value);
                //double cantidad = 0;
                var cantidad = repositorio.Listar<Contrato, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                (x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error || x.EstadoId == (int)EnumEstadoContrato.Finalizado) && x.Id != contrato.Id && x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO
                && x.MaterialId == rango.MaterialId && x.ContratoAcuerdoId == null);
                cantidad.AddRange(repositorio.Listar<ContratoAcuerdo, double>(x => x.Cantidad, x => DbFunctions.TruncateTime(x.Fecha) == DbFunctions.TruncateTime(hoy) &&
                 (x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error || x.EstadoId == (int)EnumEstadoContrato.Finalizado) && x.Id != contrato.Id && x.TipoNegocioId == (int)EnumTipoNegocio.CONTRATO_ACUERDO
                 && x.MaterialId == rango.MaterialId && x.Precio > 0));
                //cantidad.AddRange(repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.Fecha == hoy &&
                //(x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error || x.EstadoId == (int)EnumEstadoContrato.Finalizado) && x.MaterialId == rango.MaterialId));
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
            if (oContratoSave.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR && oContratoSave.TipoPosicionCBOTId == 3)
            {
                var cantidad = ValidarSiCumpleLaTolerancia(contratoId);
                var config = configuracionManager.TraerConfiguraciones();

                var cantidadMinima = config.ToleranciaPaseMin * oContratoSave.Cantidad / 100;
                var cantidadMaxima = config.ToleranciaPaseMax * oContratoSave.Cantidad / 100;

                if (!TieneAsociados(contratoId))
                {
                    oEntityErrors.Error("", "El contrato no se puede confirmar porque no tiene negocios asociados.");
                    return oEntityErrors;
                }
                else
                {
                    oContratoSave.PrecioNetoPonderado = CalcularPrecioPonderadoEnAFijarPaseNeto(oContratoSave, oContratoSave.PrecioPonderado.Value);
                }
                if (cantidad < (oContratoSave.Cantidad - cantidadMinima))
                {
                    oEntityErrors.Error("", "El contrato no se puede confirmar porque no cumple la cantidad de tolerancia mínima." +
                        " Kg Contrato: " + oContratoSave.Cantidad.ToString("N0") + ". Kg Mínimo: " + (oContratoSave.Cantidad - cantidadMinima).ToString("N0"));
                    return oEntityErrors;
                }
                if (cantidad > (oContratoSave.Cantidad + cantidadMaxima))
                {
                    oEntityErrors.Error("", "El contrato no se puede confirmar porque excede la cantidad de tolerancia máxima. " +
                       "Kg Contrato: " + oContratoSave.Cantidad.ToString("N0") + ". Kg Máximo: " + (oContratoSave.Cantidad + cantidadMaxima).ToString("N0"));
                    return oEntityErrors;
                }
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
                var cantidadCargada = repositorio.Listar<Contrato>(d => contratoId != d.Id && d.ContratoAcuerdoId == contratoAcuerdoId.Value && (d.EstadoId == (int)EnumEstadoContrato.Pendiente || d.EstadoId == (int)EnumEstadoContrato.Confirmado || d.EstadoId == (int)EnumEstadoContrato.Oferta || d.EstadoId == (int)EnumEstadoContrato.Con_Error || d.EstadoId == (int)EnumEstadoContrato.Finalizado || d.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Sum(d => d.Cantidad);
                var cantidadTodoAcuerdo = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == contratoAcuerdoId.Value && (d.EstadoId == (int)EnumEstadoContrato.Pendiente || d.EstadoId == (int)EnumEstadoContrato.Confirmado || d.EstadoId == (int)EnumEstadoContrato.Oferta || d.EstadoId == (int)EnumEstadoContrato.Con_Error || d.EstadoId == (int)EnumEstadoContrato.Finalizado || d.EstadoId == (int)EnumEstadoContrato.Reconfirmar)).Sum(d => d.Cantidad);
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
                            oEntityErrors.Error("", "La cantidad del negocio es mayor al saldo disponible del acuerdo (" + (tolerancia - acuerdo.CantidadAmpliado.Value).ToString("N0") + " kg)");
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
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
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
            if (oContratoSave.Condicional == true && oContratoSave.CondicionalContratos.Any(x => x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado))
            {
                oEntityErrors.Error("", "El contrato no se puede anular ya que tiene un contrato condicional asociado.");
            }
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

            var listaServicioValor = repositorio.Listar<ServicioValor>();
            List<int> listaFiltradaServicioValor = new List<int>();
            if (listaServicioValor != null)
            {
                List<ServicioValor> listaFiltrada = listaServicioValor
                    .Where(x => x.MaterialId == oContratoSave.MaterialId && x.CentroId == oContratoSave.DestinoId)
                    .ToList();

                listaFiltrada.ForEach(x => listaFiltradaServicioValor.Add(x.Id));
            }

            if (oContratoSave != null && string.IsNullOrEmpty(oContratoSave.ContratoSAP) && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error))
            {
                Nullable<DateTime> fecha = null;

                var diaAnterior = oDiasHabilesAgent.UltimoDiaHabil(fecha);

                if (oContratoSave.Fecha < diaAnterior)
                {
                    oEntityErrors.Error("", "La fecha del contrato debe ser la de hoy o día hábil anterior.");
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

                    //if(oContratoSave.Servicios != null)
                    //    logger.Debug("Servicios oContratoSave: " + oContratoSave.Servicios.ToJson());

                    if (oContratoSave.Servicios != null)
                    {
                        var serviciosContrato = oContratoSave.Servicios.Select(s => new
                        {
                            Id = s.Id,
                            ServicioValor = s.ServicioValor,
                            Importe = s.Importe,
                            MonedaId = s.MonedaId,
                            NegocioId = s.NegocioId,
                            Desde = s.Desde,
                            Hasta = s.Hasta
                        }).ToList();

                        logger.Debug("Servicios oContratoSave: " + serviciosContrato.ToJson());

                        oContratoSave.Servicios = oContratoSave.Servicios.Where(x => listaFiltradaServicioValor.Contains(x.ServicioValorId)).ToList();
                    }

                    //if (oContratoSave.Servicios == null || (oContratoSave.Servicios != null && oContratoSave.Servicios.Count == 0))
                    //{
                    //    var objServicios = repositorio.Listar<Servicio>(x => x.NegocioId == oContratoSave.Id);
                    //    if (objApertura != null)
                    //        oContratoSave.Servicios = objServicios;
                    //}

                    #region BLEND_Finaliza

                    if (oContratoSave.MonedaId == "USDM ")
                    {
                        string codigoTC = oFinalizarContratoAgent.DevolverTipoCambioSAP(oContratoSave.TipoNegocioId, oContratoSave.MonedaId, oContratoSave.TipoAgenteCompraId, oContratoSave.Fecha);
                        oContratoSave.TipoDeCambioId = codigoTC == "04" ? (int)EnumTipoDeCambio.BLEND : (int)EnumTipoDeCambio.BNA;
                    }

                    #endregion BLEND_Finaliza

                    string nroContratoSAP = SAPFinalizarContrato(oContratoSave, objDescuento, objCalidad);

                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                    oContratoSave.FechaDolarizadoOriginal = oContratoSave.FechaDolarizado;
                    oContratoSave.FechaHastaOriginal = oContratoSave.FechaHasta;
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
                    logger.Info($"FinalizarContrato repositorio.GuardarCambios(); id: {oContratoSave.Id} ");

                    try
                    {
                        logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), TipoAccionLogDataAgro.Crear, oContratoSave.GetType());
                        logger.Info($"FinalizarContrato LogCambiosDataAgro; id: {oContratoSave.Id} ");
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        mailManager.EnviarMail(new List<string> { ConfigurationManager.AppSettings["EmailSoporte"] }, $"Error guardar log finalizar contrato {oContratoSave.Id}", "", null, null, null, null, null);
                    }

                    EnviarMailFinalizado(idActiveDirectory, oContratoSave, objDescuento, objCalidad);
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
                    oEntityErrors.Error("", "El contrato ya se encuentra Finalizado.");
                }
                else if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
                {
                    oEntityErrors.Error("", "El contrato ya ha sido Rechazado.");
                }
                else if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente || oContratoSave.EstadoId == (int)EnumEstadoContrato.Oferta)
                {
                    oEntityErrors.Error("", "El contrato debe ser Confirmado.");
                }
                if (!string.IsNullOrEmpty(oContratoSave.ContratoSAP) && oContratoSave.Estado.EstadoContratoId == (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El contrato ya tiene ContratoSAP asignado. Por favor, comunicarse con sistemas.");
                    negocioManager.EnviarMailErrorFinalizarNegocio(contratoId);
                }
                logger.Debug(" Error intentando finalizar el contrato ID " + contratoId + " y estadoId " + oContratoSave.EstadoId);
            }
            //repositorio.GuardarCambios();
            return oEntityErrors;
        }

        private void EnviarMailFinalizado(string idActiveDirectory, Contrato oContratoSave, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad)
        {
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

        public GrabarContratoResult BorrarContrato(Contrato oContrato)
        {
            var tipoAccion = TipoAccionLogDataAgro.Eliminar;
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(oContrato.MotivoRechazo) || string.IsNullOrWhiteSpace(oContrato.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
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
                            List<Servicio> serviciosExistentes = oContratoSave.Servicios != null ? oContratoSave.Servicios.ToList() : new List<Servicio>();
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
                            oContratoSave.CantidadDeposito = oContrato.CantidadDeposito;
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
                            oContratoSave.EPA = contratoOriginal.EPA;
                            oContratoSave.EUDR = contratoOriginal.EUDR;
                            oContratoSave.SustentableTipoDBId = contratoOriginal.SustentableTipoDBId;
                            oContratoSave.FechaCierta = contratoOriginal.FechaCierta;
                            oContratoSave.ContratoSAP = contratoOriginal.ContratoSAP;
                            oContratoSave.ContratoAcuerdoId = contratoOriginal.ContratoAcuerdoId;
                            oContratoSave.PagoCBU = contratoOriginal.PagoCBU;
                            oContratoSave.ChequeElectronico = contratoOriginal.ChequeElectronico;
                            oContratoSave.DolarizadoExpress = contratoOriginal.DolarizadoExpress;
                            oContratoSave.Canje = contratoOriginal.Canje;
                            oContratoSave.Monto = contratoOriginal.Monto;
                            oContratoSave.MonedaCanjeId = contratoOriginal.MonedaCanjeId;
                            oContratoSave.Insumo = contratoOriginal.Insumo;
                            oContratoSave.PrestamoDevolucion = contratoOriginal.PrestamoDevolucion;
                            oContratoSave.PlantaDestinoId = contratoOriginal.PlantaDestinoId;
                            oContratoSave.ObligatoriedadCostoFinanciero = contratoOriginal.FechaCierta.HasValue &&
                            oContrato.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Importe > 0 || x.Porcentaje > 0)) &&
                            oContrato.ObligatoriedadCostoFinanciero.HasValue && !contratoOriginal.ObligatoriedadCostoFinanciero.Value
                            ? null : contratoOriginal.FechaCierta.HasValue ? contratoOriginal.ObligatoriedadCostoFinanciero : null;

                            oContratoSave.ObligatoriedadBonificacion = contratoOriginal.TipoPosicionCBOTId.HasValue && contratoOriginal.TipoPosicionCBOTId == 3 &&
                            contratoOriginal.AperturaPrecio.Any(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Bonificaciones && (x.Importe > 0)) &&
                            contratoOriginal.ObligatoriedadBonificacion.HasValue && !contratoOriginal.ObligatoriedadBonificacion.Value
                            ? null : contratoOriginal.TipoPosicionCBOTId.HasValue && contratoOriginal.TipoPosicionCBOTId == 3 ? contratoOriginal.ObligatoriedadBonificacion : null;

                            oContratoSave.Venta = contratoOriginal.Venta;

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

                            if (serviciosExistentes != null)
                            {
                                foreach (var servExistente in serviciosExistentes)
                                {
                                    repositorio.Remover(servExistente);
                                }
                            }
                            if (contratoOriginal.Servicios != null)
                            {
                                foreach (var servicio in contratoOriginal.Servicios)
                                {
                                    repositorio.Agregar(new Servicio
                                    {
                                        Importe = servicio.Importe,
                                        Desde = servicio.Desde,
                                        MonedaId = servicio.MonedaId,
                                        NegocioId = servicio.NegocioId,
                                        ServicioValorId = servicio.ServicioValorId,
                                        Hasta = servicio.Hasta,
                                        Modificado = servicio.Modificado,
                                    });
                                }
                            }
                            if (oContrato.Servicios != null && oContrato.Servicios.Count > 0)
                            {
                                GrabarServicioModificado(contratoOriginal.Servicios.ToList(), contratoOriginal.MaterialId, contratoOriginal.DestinoId ?? 0);
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
                EliminarNegociosAsociados(oContrato.Id);
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
            if (contrato.EstadoId == (int)EnumEstadoContrato.Confirmado)
            {
                title = "Contrato Confirmado";
                message = "El contrato " + contrato.Id + " ha sido confirmado a las " + hora;
            }
            else if (contrato.EstadoId == (int)EnumEstadoContrato.Finalizado)
            {
                title = "Contrato Finalizado";
                message = "El contrato " + contrato.ContratoSAP + " ha sido finalizado a las " + hora + " por " + contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido;
            }
            else if (contrato.EstadoId == (int)EnumEstadoContrato.Rechazado)
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
                    }).ToList(),
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
            var servicios = new List<ServicioValorDto>();
            var contrato = repositorio.Obtener<Contrato, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                Id = x.Id,
                ContratoId = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                CorredorId = x.CorredorId ?? 0,
                Proveedor = x.Proveedor == null ? "" : x.Proveedor.RazonSocial + " " + "(" + x.Proveedor.CUIT + ")",
                Corredor = x.Corredor == null ? "" : x.Corredor.RazonSocial + " " + "(" + x.Corredor.CUIT + ")",
                ComercialId = x.ComercialId,
                FechaFormateado = DbFunctions.Right("0" + x.Fecha.Day, 2) + "-" + DbFunctions.Right("0" + x.Fecha.Month, 2) + "-" + x.Fecha.Year,
                FechaDesdeFormateado = DbFunctions.Right("0" + x.FechaDesde.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDesde.Month, 2) + "-" + x.FechaDesde.Year,
                FechaHastaFormateado = DbFunctions.Right("0" + x.FechaHasta.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaHasta.Month, 2) + "-" + x.FechaHasta.Year,
                FechaHastaOriginalFormateado = x.FechaHastaOriginal.HasValue ? DbFunctions.Right("0" + x.FechaHastaOriginal.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaHastaOriginal.Value.Month, 2) + "-" + x.FechaHastaOriginal.Value.Year : "",
                DesdeFijacionFormateado = x.DesdeFijacion.HasValue ? DbFunctions.Right("0" + x.DesdeFijacion.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.DesdeFijacion.Value.Month, 2) + "-" + x.DesdeFijacion.Value.Year : "",
                HastaFijacionFormateado = x.HastaFijacion.HasValue ? DbFunctions.Right("0" + x.HastaFijacion.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.HastaFijacion.Value.Month, 2) + "-" + x.HastaFijacion.Value.Year : "",
                FechaCiertaFormateado = x.FechaCierta.HasValue ? DbFunctions.Right("0" + x.FechaCierta.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaCierta.Value.Month, 2) + "-" + x.FechaCierta.Value.Year : "",
                TipoNegocioId = x.TipoNegocioId,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Ampliaciones = x.Ampliaciones,
                Precio = x.Precio,
                PrecioNeto = x.PrecioNeto,
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
                Sustentable = x.Sustentable,
                EPA = x.EPA,
                EUDR = x.EUDR,
                SustentableTipoDBId = x.SustentableTipoDBId,
                SustentableTipoDB = x.SustentableTipoDB != null ? x.SustentableTipoDB.Descripcion : "",
                Importe_Sustentable = x.ImporteSustentable,
                Moneda_Sustentable = x.MonedaSustentableId,
                TarifaAConvenir = x.TarifaAConvenir,
                Fecha_DolarizadoFormateado = x.FechaDolarizado.HasValue ? DbFunctions.Right("0" + x.FechaDolarizado.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDolarizado.Value.Month, 2) + "-" + x.FechaDolarizado.Value.Year : "",
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
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                CondicionFijacion = x.CondicionFijacionId,
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
                Pizarra = x.Pizarra.HasValue ? x.Pizarra.Value : false,
                StandardDeCalidadId = x.StandardDeCalidadId,
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
                Descuentos = x.Descuentos.Select(y => new DescuentoBonificacionDto
                {
                    Id = y.Id,
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? DbFunctions.Right("0" + y.FechaDesde.Value.Day, 2) + "-" + DbFunctions.Right("0" + y.FechaDesde.Value.Month, 2) + "-" + y.FechaDesde.Value.Year : "",
                    FechaHasta = y.FechaHasta != null ? DbFunctions.Right("0" + y.FechaHasta.Value.Day, 2) + "-" + DbFunctions.Right("0" + y.FechaHasta.Value.Month, 2) + "-" + y.FechaHasta.Value.Year : "",
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Moneda = y.MonedaId,
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
                    Id = y.Id,
                    contratoId = y.NegocioId,
                    ConceptoAperturaPrecio = y.ConceptoAperturaPrecio.Descripcion,
                    ConceptoAperturaPrecioId = y.ConceptoAperturaPrecioId,
                    Importe = y.Importe,
                    MonedaId = y.MonedaId,
                    Porcentaje = y.Porcentaje
                }).ToList(),
                PreciosPactados = x.PrecioPactado.Select(y => new PrecioPactadosDto
                {
                    Id = y.Id,
                    ContratoId = y.ContratoId,
                    FechaDesde = y.FechaDesde != null ? DbFunctions.Right("0" + y.FechaDesde.Value.Day, 2) + "-" + DbFunctions.Right("0" + y.FechaDesde.Value.Month, 2) + "-" + y.FechaDesde.Value.Year : "",
                    FechaHasta = y.FechaHasta != null ? DbFunctions.Right("0" + y.FechaHasta.Value.Day, 2) + "-" + DbFunctions.Right("0" + y.FechaHasta.Value.Month, 2) + "-" + y.FechaHasta.Value.Year : "",
                    ImportePactado = y.ImportePactado,
                    MonedaImportePactadoDesc = y.MonedaImportePactado.Descripcion,
                    MonedaImportePactadoId = y.MonedaImportePactadoId,
                    MonedaPactadoDesc = y.MonedaPactado.Descripcion,
                    MonedaPactadoId = y.MonedaPactadoId,
                    Porcentaje = y.Porcentaje,
                    Precio = y.Precio
                }).ToList(),
                PorcentajeDePago = x.PorcentajeDePago,
                TipoAgenteCompraId = x.TipoAgenteCompraId,
                CaratulaExtension = x.CaratulaExtension,
                CaratulaMAT = x.CaratulaMAT,
                PrecioAjusteComision = x.PrecioAjusteComision,
                MonedaAjusteComisionId = x.MonedaAjusteComisionId,
                ContratoAcuerdoId = x.ContratoAcuerdoId,
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
                FechaOperacionFormateado = DbFunctions.Right("0" + x.FechaOperacion.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaOperacion.Month, 2) + "-" + x.FechaOperacion.Year,
                MotivoOperacionAnterior = x.MotivoOperacionAnterior,
                DescripcionOperacionAnterior = x.DescripcionOperacionAnterior,
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
                FechaDesde_SustentableFormateado = x.FechaDesdeSustentable.HasValue ? DbFunctions.Right("0" + x.FechaDesdeSustentable.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDesdeSustentable.Value.Month, 2) + "-" + x.FechaDesdeSustentable.Value.Year : "",
                FechaHasta_Sustentable = x.FechaHastaSustentable,
                FechaHasta_SustentableFormateado = x.FechaHastaSustentable.HasValue ? DbFunctions.Right("0" + x.FechaHastaSustentable.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaHastaSustentable.Value.Month, 2) + "-" + x.FechaHastaSustentable.Value.Year : "",
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
                BoletoVentaId = x.BoletoVentaId,
                MailVentaBoleto = x.MailVentaBoleto,
                CreditoDisponible = x.CreditoDisponible,
                Cesion = x.Cesion,
                Condicional = x.Condicional,
                CondicionalCantidad = x.CondicionalCantidad,
                CondicionalFecha = x.CondicionalFecha,
                CondicionalFechaFormateado = x.CondicionalFecha != null ? SqlFunctions.DateName("day", x.CondicionalFecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.CondicionalFecha.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.CondicionalFecha) : "",
                CondicionalMonedaId = x.CondicionalMonedaId,
                CondicionalPosicion = x.CondicionalPosicion,
                CondicionalPrecio = x.CondicionalPrecio,
                CondicionalContratoId = x.CondicionalContratoId,
                CondicionalContratoSAP = x.CondicionalContrato.ContratoSAP,
                KgMinimo = x.KgMinimo ?? 0,
                KgMaximo = x.KgMaximo ?? 0,
                ProveedorComisionistaId = x.ProveedorComisionistaId,
                FechaDolarizadoOriginalFormateado = x.FechaDolarizadoOriginal.HasValue ? DbFunctions.Right("0" + x.FechaDolarizadoOriginal.Value.Day, 2) + "-" + DbFunctions.Right("0" + x.FechaDolarizadoOriginal.Value.Month, 2) + "-" + x.FechaDolarizadoOriginal.Value.Year : "",
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
                }).ToList(),
                ConDescarga = x.ConDescarga,
                DolarExportador = x.DolarExportador,
            });
            return contrato;
        }

        public void EnviarMailPendiente()
        {
            if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
            {
                ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
            }

            var hoy = DateTime.Today;
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
            }, x => (x.EstadoId == (int)EnumEstadoContrato.Pendiente || x.EstadoId == (int)EnumEstadoContrato.Oferta) && x.Fecha < hoy && (x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION) && x.Canje != true && x.PrestamoDevolucion != null);
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
            htmlBody += "En el presente mail se detallan los negocios a confirmar creados por el comercial " + nombreApellido + ": <br /><br />  ";
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
                "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public void FinalizacionAutomatica(string idActiveDirectory)
        {
            var contratosConfirmados = repositorio.Listar<Contrato, int>(a => a.Id, x => x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Con_Error);
            logger.Debug($"{contratosConfirmados.Count} contratos a finalizar con los ID {String.Join(", ", contratosConfirmados)}");
            var oEntityErrors = new GrabarContratoResult();
            foreach (var id in contratosConfirmados)
            {
                try
                {
                    logger.Debug("Finalizando contrato con ID " + id);
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
            var contratosPendientes = repositorio.Listar<Contrato>(x => x.EstadoId == (int)EnumEstadoContrato.Pendiente || x.EstadoId == (int)EnumEstadoContrato.Oferta);
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
            var fechaHoy = DateTime.Today;
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
            }, x => (x.EstadoId == (int)EnumEstadoContrato.Pendiente || x.EstadoId == (int)EnumEstadoContrato.Oferta) &&
            equipo.Contains(x.Comercial.ComercialId) &&
            x.Fecha < fechaHoy &&
            (x.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR || x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO || x.TipoNegocioId == (int)EnumTipoNegocio.FIJACION));
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
                PlanCanje = x.PlanCanje,
                RazonSocialComisionista = x.ComisionistaE != null ? x.ComisionistaE.RazonSocial : "",
                ComisionistaId = x.ComisionistaE != null ? x.ComisionistaId : null
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
                result.Error("", "No existe el Contrato Madre");
                return result;
            }
        }

        public GrabarContratoResult AnularContrato(Contrato oContrato, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            if (string.IsNullOrEmpty(oContrato.MotivoRechazo) || string.IsNullOrWhiteSpace(oContrato.MotivoRechazo))
            {
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
                return oEntityErrors;
            }
            EliminarNegociosAsociados(oContrato.Id);
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
                if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
                {
                    ServicePointManager.SecurityProtocol = (SecurityProtocolType)48 | (SecurityProtocolType)192 | (SecurityProtocolType)768 | (SecurityProtocolType)3072;
                }

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
                    logger.Debug($"No existen administrativos para informar SIO");
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
            htmlBody += "Por el presente mail se solicita anular el contrato " + contrato.ContratoSAP.TrimStart('0') + " de  SIO Granos. <br /><br />  ";
            htmlBody += "<br /><br /> Por favor, anularlo a la brevedad y comunicarse con " + comercial.Nombres + " " + comercial.Apellido + "." +
                "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<ContratoCopiar> TraerContratosPorSap(string nrocontratoSap)
        {
            return repositorio.ListarConsulta(new DevolverContratos(nrocontratoSap, null));
        }

        public List<ContratoCopiar> TraerContratosCondicionalPorSap(string nrocontratoSap)
        {
            return repositorio.ListarConsulta(new DevolverContratos(nrocontratoSap, true));
        }

        public List<ContratoCopiar> TraerContratosAcuerdo(string filtro)
        {
            return repositorio.ListarConsulta(new DevolverContratosAcuerdo(filtro));
        }

        public BasicoContrato TraerContratoAcuerdoACopiar(int contratoId)
        {
            var hoy = DateTime.Today;
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
                ObligatoriedadBonificacion = x.ObligatoriedadBonificacion,
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
                StandardDeCalidadId = x.StandardDeCalidadId,
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
            string tipoProv = proveedor.SegmentacionId == 5 || proveedor.SegmentacionId == 7 ? "CORR" : "PROV";
            return altaTempranaAgent.ObtenerAlta(proveedor.CUIT, tipoProv);
        }

        public Resultado ActualizarContratoSAP(Contrato contrato)
        {
            var error = new Resultado();
            logger.Debug("Actualizando contrato en BD DataAgro: " + contrato.Id);
            var contratoSave = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contrato.ContratoSAP && x.EstadoId != (int)EnumEstadoContrato.Eliminado);
            if (contratoSave == null || contratoSave.Id == 0)
            {
                error.Error("Contrato", "No existe el contrato en DataAgro");
                return error;
            }

            contrato.TipoNegocioId = contratoSave.TipoNegocioId;
            contrato.ComercialId = contratoSave.ComercialId;

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
            contratoSave.TarifaAConvenir = contrato.TarifaAConvenir;
            contratoSave.MonedaSustentableId = contrato.MonedaSustentableId;
            contratoSave.Sustentable = contrato.Sustentable;
            contratoSave.EPA = contrato.EPA;
            contratoSave.EUDR = contrato.EUDR;
            contratoSave.SustentableTipoDBId = contrato.SustentableTipoDBId;
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
            if (contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO && contratoSave.BoletoId != (int)EnumBoletoCompraNet.SIN_BOLETO)
            {
                contratoSave.BoletoId = contratoSave.BoletoId;
            }
            else
            {
                contratoSave.BoletoId = contrato.BoletoId;
            }
            contratoSave.BolsaId = contrato.BolsaId == 0 ? null : contrato.BolsaId;
            contratoSave.DesdeFijacion = contrato.DesdeFijacion;
            contratoSave.HastaFijacion = contrato.HastaFijacion;
            contratoSave.MercsDeposito = contrato.MercsDeposito;
            contratoSave.CantidadDeposito = contrato.MercsDeposito != true ? 0 : contratoSave.CantidadDeposito;
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

            contratoSave.Condicional = contrato.Condicional;
            contratoSave.CondicionalCantidad = contrato.CondicionalCantidad;
            contratoSave.CondicionalPrecio = contrato.CondicionalPrecio;
            contratoSave.CondicionalMonedaId = contrato.CondicionalMonedaId;
            contratoSave.CondicionalFecha = contrato.CondicionalFecha;
            contratoSave.CondicionalPosicion = contrato.CondicionalPosicion;
            contratoSave.CondicionalContratoId = contrato.CondicionalContratoId;
            if (contratoSave.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                contratoSave.KgMinimo = contrato.KgMinimo ?? contratoSave.KgMinimo;
                contratoSave.KgMaximo = contrato.KgMaximo ?? contratoSave.KgMaximo;
            }

            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoSave.Id), TipoAccionLogDataAgro.Modificar, contratoSave.GetType());

            return error;
        }

        private Resultado ActualizarContratoFinalizadoResultado(Contrato contrato)
        {
            var error = new Resultado();
            logger.Debug("Actualizando contrato en BD DataAgro: " + contrato.Id);
            //  GSIAN: Acá no debería obtener por el ID ? Puede existir mas de un ContratoSAP.
            var contratoSave = repositorio.Obtener<Contrato>(x => x.ContratoSAP == contrato.ContratoSAP && x.EstadoId != (int)EnumEstadoContrato.Eliminado);
            if (contratoSave == null || contratoSave.Id == 0)
            {
                error.Error("Contrato", "No existe el contrato en DataAgro");
                return error;
            }

            contrato.TipoNegocioId = contratoSave.TipoNegocioId;
            contrato.ComercialId = contratoSave.ComercialId;
            contratoSave.MaterialId = contrato.MaterialId;
            contratoSave.Cantidad = contrato.Cantidad;

            if (contratoSave.Venta == true)
                contratoSave.Cantidad = -Math.Abs(contratoSave.Cantidad);

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
            contratoSave.TarifaAConvenir = contrato.TarifaAConvenir;
            contratoSave.MonedaSustentableId = contrato.MonedaSustentableId;
            contratoSave.Sustentable = contrato.Sustentable;
            contratoSave.EPA = contrato.EPA;
            contratoSave.EUDR = contrato.EUDR;
            contratoSave.SustentableTipoDBId = contrato.SustentableTipoDBId;
            contratoSave.FechaDesdeSustentable = contrato.FechaDesdeSustentable;
            contratoSave.FechaHastaSustentable = contrato.FechaHastaSustentable;
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
            if (contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO && contratoSave.BoletoId != (int)EnumBoletoCompraNet.SIN_BOLETO)
            {
                contratoSave.BoletoId = contratoSave.BoletoId;
            }
            else
            {
                contratoSave.BoletoId = contrato.BoletoId;
            }
            contratoSave.BolsaId = contrato.BolsaId == 0 ? null : contrato.BolsaId;
            contratoSave.DesdeFijacion = contrato.DesdeFijacion;
            contratoSave.HastaFijacion = contrato.HastaFijacion;
            contratoSave.MercsDeposito = contrato.MercsDeposito;
            contratoSave.CantidadDeposito = contrato.CantidadDeposito;
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

            contratoSave.Condicional = contrato.Condicional;
            contratoSave.CondicionalCantidad = contrato.CondicionalCantidad;
            contratoSave.CondicionalPrecio = contrato.CondicionalPrecio;
            contratoSave.CondicionalMonedaId = contrato.CondicionalMonedaId;
            contratoSave.CondicionalFecha = contrato.CondicionalFecha;
            contratoSave.CondicionalPosicion = contrato.CondicionalPosicion;
            contratoSave.CondicionalContratoId = contrato.CondicionalContratoId;
            if (contratoSave.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                contratoSave.KgMinimo = contrato.KgMinimo ?? contratoSave.KgMinimo;
                contratoSave.KgMaximo = contrato.KgMaximo ?? contratoSave.KgMaximo;
            }
            var servicios = repositorio.Listar<Servicio>(x => x.NegocioId == contratoSave.Id);
            repositorio.RemoverTodos(servicios);

            contratoSave.Servicios = contrato.Servicios;
            contratoSave.ConDescarga = contrato.ConDescarga;
            contratoSave.DolarExportador = contrato.DolarExportador;

            if (contrato.Servicios != null && contrato.Servicios.Count > 0)
            {
                GrabarServicioModificado(contrato.Servicios.ToList(), contrato.MaterialId, contrato.DestinoId ?? 0);
            }
            repositorio.GuardarCambios();
            logDataAgroManager.LogCambiosDataAgro(TraerContrato(contratoSave.Id), TipoAccionLogDataAgro.Modificar, contratoSave.GetType());

            return error;
        }

        public GrabarContratoResult ActualizarContratoFinalizado(Contrato oContrato)
        {
            var error = new GrabarContratoResult();
            try
            {
                Validar(oContrato, error);

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
                List<Servicio> serviciosExistentes = null;

                oContratoSave = repositorio.Obtener<Contrato>(oContrato.Id);
                descuentosExistentes = oContratoSave.Descuentos.ToList();
                calidadesExistentes = oContratoSave.Calidad.ToList();
                aperturasExistentes = oContratoSave.AperturaPrecio.ToList();
                preciosExistentes = oContratoSave.PrecioPactado.ToList();
                serviciosExistentes = oContratoSave.Servicios != null ? oContratoSave.Servicios.ToList() : new List<Servicio>();
                oContrato.ContratoSAP = oContratoSave.ContratoSAP;
                var cuit = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == oContrato.ProveedorId, x => x.CUIT);
                oContratoSave.MonedaCreditoDisponible = validarCreditoAgente.ValidarCredito(cuit).Moneda;
                var validacionServicioModificado = ValidarServicioModificado(oContrato, oContratoSave);

                #region BLEND_Actualiza

                if (oContrato.MonedaId == "USDM ")
                {
                    string codigoTC = oFinalizarContratoAgent.DevolverTipoCambioSAP(oContrato.TipoNegocioId, oContrato.MonedaId, oContrato.TipoAgenteCompraId, oContrato.Fecha, true);
                    oContrato.TipoDeCambioId = codigoTC == "04" ? (int)EnumTipoDeCambio.BLEND : (int)EnumTipoDeCambio.BNA;
                }

                #endregion BLEND_Actualiza

                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Rechazado)
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
                if (oContrato.EstadoId != (int)EnumEstadoContrato.ReconfirmarFinalizado)
                {
                    if ((oContratoSave.Precio != oContrato.Precio || oContratoSave.Cantidad != oContrato.Cantidad || oContratoSave.DesdeFijacion != oContrato.DesdeFijacion ||
                    oContratoSave.MonedaId != oContrato.MonedaId || oContratoSave.HastaFijacion != oContrato.HastaFijacion
                    || ValidarCalidadModificada(oContrato, oContratoSave)) || validacionServicioModificado && (oContratoSave.EstadoId != (int)EnumEstadoContrato.Pendiente
                    && oContratoSave.EstadoId != (int)EnumEstadoContrato.Oferta && oContratoSave.EstadoId != (int)EnumEstadoContrato.PreAprobacion))
                    {
                        if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado)
                        {
                            oContrato.EstadoId = 11;
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
                var listaErrores = ActualizarContratoFinalizadoResultado(oContrato);
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

            var tienePermiso = repositorio.Obtener<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)) && x.ComercialId == contrato.ComercialCreadorId) != null;
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

            logger.Debug($"Se envió email del contrato modificado ID {contrato.Id} a {emailproveedor.ToJson()} con copia a {lista.ToJson()}. Contrato SAP: {contrato.ContratoSAP}");
        }

        public void EnviarMailImpuestos(int contratoId)
        {
            Contrato contratoDB = repositorio.Obtener<Contrato>(contratoId);

            if (contratoDB != null && contratoDB.EstadoId == (int)EnumEstadoContrato.Finalizado && (!contratoDB.Provincia.Inscripto || !contratoDB.Destino.Localidad.Provincia.Inscripto) && contratoDB.TipoNegocioId != (int)EnumTipoNegocio.FIJACION)
            {
                var comercial = new List<string>
                {
                    mailManager.GetEmailUserActiveDirectory(contratoDB.Comercial.IdActiveDirectory)
                };
                List<string> usuariosImpuestos = repositorio.Listar<Comercial, string>(x => x.Email, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.MailImpuestos)));

                var subject = $"Nuevo negocio con jurisdicción no inscripta: {contratoDB.ContratoSAP.TrimStart('0')}";

                mailManager.EnviarMail(contratoDB.Comercial, usuariosImpuestos, subject, "", comercial, CuerpoMailImpuesto(httpContextManager.ObtenerPathLogoMail(), contratoDB));

                logger.Debug($"Se envió email del contrato ID {contratoDB.Id} al sector de impuestos: {usuariosImpuestos.ToJson()} con copia a {comercial.ToJson()}. Contrato SAP: {contratoDB.ContratoSAP}");
            }
        }

        private AlternateView CuerpoMailImpuesto(String filePath, Contrato oContrato)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string htmlBody = "";
            htmlBody += $"En el presente mail se informa la creación del contrato número {oContrato.ContratoSAP.TrimStart('0')} de {oContrato.Cantidad:N0} kg de {oContrato.Material.Descripcion} con procedencia o destino en una jurisdicción donde MOA no está inscripto. <br /><br />  ";

            htmlBody += "Origen: " + oContrato.Provincia.Nombre + " <br /><br />  ";
            htmlBody += "Destino: " + oContrato.Destino.Localidad.Provincia.Nombre + " <br />";
            htmlBody += "<br /> <br />  Saludos Cordiales," +
            " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
            @"<img src='cid:" + res.ContentId + @"'/>" +
            "<br /> <br /> www.molinosagro.com.ar";

            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);

            return alternateView;
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
            htmlBody += "En el presente mail se detallan modificaciones en el negocio con Molinos Agro S.A: <br /><br />  ";
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

            if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
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
            else if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
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

            if (oContrato.Boleto != null && oContrato.BoletoId != (int)EnumBoletoCompraNet.NINGUNO && oContrato.Bolsa != null)
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

            if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
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
                    htmlBody += "CANTIDAD MÁXIMA A FIJAR " + Split((oContrato.KgMaximo ?? 0).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " kg<br />";
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
                htmlBody += (oContrato.Sustentable ? "SUSTENTABLE " : oContrato.EPA && oContrato.EUDR ? "EPA/EUDR " : !oContrato.EPA && oContrato.EUDR ? "EUDR " : "EPA ") + oContrato.ImporteSustentable + " " + oContrato.MonedaSustentable.Descripcion.ToUpper() + "<br />";
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
                    if (precio.ImportePactado != null && precio.MonedaImportePactado != null)
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
                htmlBody += "A pesificar en mes en curso mediante envío de mail a materias.primas@molinosagro.com.ar hasta las 13 hs. <br />";
            }
            if (oContrato.ChequeElectronico == true)
            {
                htmlBody += "Pago con Echeq <br />";
            }
            if (oContrato.PagoCBU != null)
            {
                htmlBody += "Pago con CBU: " + oContrato.PagoCBU + " <br />";
            }
            htmlBody += " </td></tr>";
            htmlBody += "</td></tr></table>";
            htmlBody += "<br /><br /> En caso de ser necesario, comuníquese con  " + oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? " (" + emailComercial + ")." : ".") +
                "<br /> <br />  Saludos Cordiales," +
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
                td1 = "<td style=\"border: 2px solid white; color:white; background-color: #F62459; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:white; background-color: #F62459; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                td1 = "<td style=\"border: 2px solid white; color:white; background-color: #C93756; padding: 5px 0; width: 250px;\">";
                td2 = "<td style=\"border: 2px solid white; color:white; background-color: #C93756; padding: 5px 0; width: 250px;\">";
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
                    TipoNegocioId = x.TipoNegocioId,
                    MonedaId = !string.IsNullOrEmpty(x.MonedaId) ? x.MonedaId : "",
                    Cantidad = x.Cantidad,
                    Precio = x.Precio,
                    StandardDeCalidadId = x.StandardDeCalidadId,
                    StandardDeCalidadDescripcion = x.StandardDeCalidad != null ? x.StandardDeCalidad.Descripcion : "",
                    Calidades = x.Calidad.Select(y => new CalidadDto
                    {
                        Id = y.Id,
                        CalidadEspecialDesc = y.CalidadEspecial != null ? y.CalidadEspecial.Descripcion : "",
                        CalidadEspecialId = y.CalidadEspecialId,
                        PorcentajeDesde = y.PorcentajeDesde,
                        PorcentajeHasta = y.PorcentajeHasta,
                        Valor = y.Valor,
                    }).ToList(),
                    DesdeFijacionFormateado = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.DesdeFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                    HastaFijacionFormateado = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.HastaFijacion.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.HastaFijacion) : "",
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
                        Modificado = y.Modificado,
                        TipoServicioId = y.ServicioValor.TipoServicio.Id
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
            var valor = repositorio.Listar<ServicioValor, ServicioValorDto>(y => new ServicioValorDto()
            {
                Id = y.Id,
                ServicioValorId = y.Id,
                Importe = y.Importe,
                MonedaId = y.MonedaId,
                Desde = y.Desde,
                Hasta = y.Hasta,
                MonedaDescripcion = y.Moneda.Descripcion,
                Descripcion = y.TipoServicio.Descripcion,
            });
            return new BasicoContrato
            {
                TipoNegocioId = x.TipoNegocioId,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                MonedaId = !string.IsNullOrEmpty(x.MonedaId) ? x.MonedaId : "",
                StandardDeCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                DesdeFijacion = x.DesdeFijacion,
                HastaFijacion = x.HastaFijacion,
                DesdeFijacionFormateado = x.DesdeFijacion != null ? x.DesdeFijacion.Value.ToString("d-M-yyyy") : "",
                HastaFijacionFormateado = x.HastaFijacion != null ? x.HastaFijacion.Value.ToString("d-M-yyyy") : "",
                Calidades = x.Calidad != null ? x.Calidad.Select(y => new CalidadDto
                {
                    Id = y.Id,
                    CalidadEspecialDesc = y.CalidadEspecial.Descripcion,
                    CalidadEspecialId = y.CalidadEspecialId,
                    PorcentajeDesde = y.PorcentajeDesde,
                    PorcentajeHasta = y.PorcentajeHasta,
                    Valor = y.Valor,
                }).ToList() : new List<CalidadDto>(),
                Servicios = x.Servicios.Select(y => new ServicioValorDto
                {
                    Id = y.Id,
                    ServicioValorId = y.ServicioValorId,
                    Descripcion = valor != null ? valor.Where(s => s.ServicioValorId == y.ServicioValorId).FirstOrDefault().Descripcion : "",
                    Importe = y.Importe,
                    MonedaDescripcion = valor != null ? valor.Where(s => s.ServicioValorId == y.ServicioValorId).FirstOrDefault().MonedaDescripcion : "",
                    MonedaId = y.Moneda.MonedaId,
                    Desde = y.Desde,
                    Hasta = y.Hasta,
                    Modificado = y.Modificado,
                }).ToList(),
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
                if (oContratoSave.Venta == true && oContratoSave.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                {
                    cantidadContrato = cantidadContrato * -1;
                }

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

                    LogAnulacionContrato logAnulacionContrato = new LogAnulacionContrato()
                    {
                        Fecha = DateTime.Now,
                        NegocioId = oContratoSave.Id,
                        TipoNegocio = repositorio.Obtener<TipoNegocio, string>(x => x.TipoNegocioId == oContratoSave.TipoNegocioId, x => x.Descripcion),
                        ComercialId = (int)oContratoSave.ComercialId,
                        ContratoSAP = oContratoSave.ContratoSAP,
                        CantidadKilos = cantidadContrato,
                        KilosPendientes = oContratoSave.Cantidad,
                    };
                    repositorio.Agregar<LogAnulacionContrato>(logAnulacionContrato);

                    repositorio.GuardarCambios();
                    logDataAgroManager.LogCambiosDataAgro(TraerContrato(oContratoSave.Id), tipo, oContratoSave.GetType());
                }
                catch (Exception e)
                {
                    logger.Error("Error AnularContratoSAP ", e);
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

        public Resultado AltaContratoSAP(Contrato contratoSap)
        {
            var error = new Resultado();
            logger.Debug("Alta contrato en BD DataAgro: " + contratoSap.Id);
            var contrato = new Contrato();
            try
            {
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
                contrato.TarifaAConvenir = contratoSap.TarifaAConvenir;
                contrato.Sustentable = contratoSap.Sustentable;
                contrato.EPA = contratoSap.EPA;
                contrato.EUDR = contratoSap.EUDR;
                contrato.SustentableTipoDBId = contratoSap.SustentableTipoDBId;
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
                contrato.DescripcionOperacionAnterior = contratoSap.MotivoOperacionAnterior;
                contrato.MotivoOperacionAnterior = "Otro";
                contrato.PosicionCBOT = contratoSap.PosicionCBOT;
                contrato.TipoPosicionCBOTId = contratoSap.TipoPosicionCBOTId;
                contrato.Cesion = contratoSap.Cesion;
                //if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                //{
                //    contrato.KgMinimo = contratoSap.KgMinimo ?? contrato.KgMinimo;
                //    contrato.KgMaximo = contratoSap.KgMaximo ?? contrato.KgMaximo;
                //}
                repositorio.Agregar(contrato);
                repositorio.GuardarCambios();
                logDataAgroManager.LogCambiosDataAgro(TraerContrato(contrato.Id), TipoAccionLogDataAgro.Crear, contrato.GetType());
            }
            catch (Exception e)
            {
                logger.Error("Error AltaContratoSAP ", e);
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
                Validar(contrato, oEntityErrors);
                if (oEntityErrors.Errores.Count > 0)
                {
                    return oEntityErrors;
                }
                if (contrato.Servicios != null && contrato.Servicios.Count > 0)
                {
                    GrabarServicioModificado(contrato.Servicios.ToList(), contrato.MaterialId, contrato.DestinoId ?? 0);
                }
                if (ConfirmacionAutomatica(contrato) && DateTime.Today == contrato.FechaOperacion.Date)
                {
                    contrato.FechaConfirmacion = DateTime.Now;
                    contrato.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato " + contrato.Id + " se confirmó automaticamente por estar dentro de los rangos configurados");
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
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
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
            var asunto = "Rechazo Contrato Molinos Agro S.A. –  " + contrato.Proveedor.RazonSocial;
            var copia = new List<string>() { contrato.Comercial.IdActiveDirectory, ConfigurationManager.AppSettings["CredentialUserName"] };
            var vista = CuerpoMailRechazo(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), contrato);
            mailManager.EnviarMail(enviarA, asunto, "", copia, vista);
        }

        private AlternateView CuerpoMailRechazo(string filePath, Contrato fijacion)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            var mail = "";
            try { mail = mailManager.GetEmailUserActiveDirectory(fijacion.Comercial.IdActiveDirectory); } catch (Exception e) { logger.Error("No existe mail para el usuario en AD " + e.Message); }

            var contacto = fijacion.Comercial != null ? fijacion.Comercial.Nombres + " " + fijacion.Comercial.Apellido + (!string.IsNullOrEmpty(mail) ? " (" + mail + ")." : ".") : "Mesa de Ayuda.";
            var htmlBody = $"En el presente mail se informa que el negocio generado con Molinos Agro S.A. ha sido rechazado. <br />" +
                $"Motivo: <br />  {fijacion.MotivoRechazo} <br />" +
                $"Ante cualquier consulta contactarse con {contacto}" +
                "<br /> <br />  Saludos Cordiales," +
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
            bc.PrecioPlazo = negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? negocio.HastaFijacion.Value.ToString("dd-MM-yyyy") : negocio.Precio.ToString();
            bc.FechaEntrega = negocio is Contrato ? (negocio as Contrato).FechaEntrega.Value.Date : (DateTime?)null;
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
            bc.Observacion = negocio.Observacion ?? "";

            bc.FijacionDePrecioContratoId = (negocio is FijacionDePrecioContrato) ? (int?)(negocio as FijacionDePrecioContrato).Id : null;
            bc.Sustentable = (negocio is Contrato) && (negocio as Contrato).Sustentable;
            bc.EPA = (negocio is Contrato) && (negocio as Contrato).EPA;
            bc.EUDR = (negocio is Contrato) && (negocio as Contrato).EUDR;
            bc.SustentableTipoDBId = negocio is Contrato && (negocio as Contrato).SustentableTipoDBId.HasValue ? (negocio as Contrato).SustentableTipoDBId : null;
            bc.TarifaAConvenir = negocio.TarifaAConvenir;
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
            bc.StandardDeCalidadId = negocio.StandardDeCalidadId;
            bc.Pizarra = negocio.Pizarra ?? null;
            bc.PagoDiferido = negocio.PagoDiferido ?? null;
            bc.ZonaId = (negocio is Contrato) ? (negocio as Contrato).ZonaId : null;
            bc.AcuerdoId = (negocio is ContratoAcuerdo) ? (int?)(negocio as ContratoAcuerdo).Id : null;
            bc.ImporteFinanciero = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 1).Importe : (decimal?)null;
            bc.ImporteRedespacho = negocio.AperturaPrecio.Count() > 0 ? negocio.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2).Importe : (decimal?)null; ;
            bc.PorcentajeComision = negocio.PorcentajeComision.HasValue ? negocio.PorcentajeComision : 0;
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
            bc.FechaOperacion = negocio.Id == 0 ? DateTime.Now : negocio.FechaOperacion.Date;
            bc.FechaOperacionFormateado = negocio.Id == 0 ? DateTime.Now.ToString("dd-MM-yyyy") : negocio.FechaOperacion.ToString("dd-MM-yyyy");
            bc.MotivoOperacionAnterior = negocio.MotivoOperacionAnterior;
            bc.DescripcionOperacionAnterior = negocio.DescripcionOperacionAnterior;
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
            bc.ObligatoriedadBonificacion = negocio.ObligatoriedadBonificacion;

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
                MonedaId = negocio.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR ? y.MonedaId : negocio.MonedaId,
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
            var valor = repositorio.Listar<ServicioValor, ServicioValorDto>(y => new ServicioValorDto()
            {
                Id = y.Id,
                ServicioValorId = y.Id,
                Importe = y.Importe,
                MonedaId = y.MonedaId,
                Desde = y.Desde,
                Hasta = y.Hasta,
                MonedaDescripcion = y.Moneda.Descripcion,
                Descripcion = y.TipoServicio.Descripcion,
            });
            bc.Servicios = negocio is Contrato && (negocio as Contrato).Servicios != null ? (negocio as Contrato).Servicios.Select(y => new ServicioValorDto
            {
                Id = y.Id,
                ServicioValorId = y.ServicioValorId,
                Importe = y.Importe,
                MonedaId = y.MonedaId,
                Desde = y.Desde,
                Hasta = y.Hasta,
                Modificado = y.Modificado,
                MonedaDescripcion = valor.Where(x => x.ServicioValorId == y.ServicioValorId).FirstOrDefault().MonedaDescripcion,
                Descripcion = valor.Where(x => x.ServicioValorId == y.ServicioValorId).FirstOrDefault().Descripcion
            }).ToList() : valor;
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

            bc.Condicional = (negocio is Contrato) ? (negocio as Contrato).Condicional : null;
            bc.CondicionalCantidad = (negocio is Contrato) ? (negocio as Contrato).CondicionalCantidad : null;
            bc.CondicionalFecha = (negocio is Contrato) ? (negocio as Contrato).CondicionalFecha : null;
            bc.CondicionalFechaFormateado = (negocio is Contrato) ? (negocio as Contrato).CondicionalFecha != null ? (negocio as Contrato).CondicionalFecha.Value.ToString("dd-MM-yyyy") : "" : "";
            bc.CondicionalMonedaId = (negocio is Contrato) ? (negocio as Contrato).CondicionalMonedaId : null;
            bc.CondicionalPosicion = (negocio is Contrato) ? (negocio as Contrato).CondicionalPosicion : null;
            bc.CondicionalPrecio = (negocio is Contrato) ? (negocio as Contrato).CondicionalPrecio : null;
            bc.CondicionalContratoId = (negocio is Contrato) ? (negocio as Contrato).CondicionalContratoId : null;
            if ((negocio is Contrato) && (negocio as Contrato).CondicionalContratoId != null)
            {
                bc.CondicionalContratoSAP = repositorio.Obtener<Contrato, string>(x => x.Id == bc.CondicionalContratoId, x => x.ContratoSAP);
            }
            bc.ProveedorComisionistaId = (negocio is Contrato) ? (negocio as Contrato).ProveedorComisionistaId ?? null : null;
            bc.FechaHastaOriginalFormateado = (negocio is Contrato) ? (negocio as Contrato).FechaHastaOriginal != null ? (negocio as Contrato).FechaHastaOriginal.Value.ToString("dd-MM-yyyy") : "" : "";
            bc.FechaDolarizadoOriginalFormateado = (negocio is Contrato) ? (negocio as Contrato).FechaDolarizadoOriginal != null ? (negocio as Contrato).FechaDolarizadoOriginal.Value.ToString("dd-MM-yyyy") : "" : "";
            bc.ConDescarga = (negocio is Contrato) && (negocio as Contrato).ConDescarga.HasValue && (negocio as Contrato).ConDescarga.Value;
            bc.DolarExportador = (negocio is Contrato) && (negocio as Contrato).DolarExportador.HasValue && (negocio as Contrato).DolarExportador.Value;
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
                oEntityErrors.Error("Rechazo", "Debe indicar el motivo de rechazo");
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

        public List<CcPpPendienteAplicarDto> ListarCartasDePortePendienteAplicar(CcPpPendienteAplicarDto req)
        {
            return ccppAgent.ListarCartasDePortePendienteAplicar(req);
        }

        public string ValidarCredito(string cuit, double cantidad, decimal precio, string moneda, string typeOfRate)
        {
            var tipoCambio = tipoCambioAgent.TraerTipoDeCambio(null, typeOfRate);
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

        public List<ContratoCopiar> TraerContratosAcuerdoPorCorredor(int corredorId)
        {
            DateTime? fecha = null;
            var dia = oDiasHabilesAgent.UltimoDiaHabil(fecha);
            return repositorio.ListarConsulta(new DevolverContratosAcuerdoPorCorredor(corredorId, dia));
        }

        public List<GrabarContratoResult> GrabarContratoMasivo(List<BasicoContrato> contratos)
        {
            try
            {
                logger.Debug("GrabarContratoMasivo");
                logger.Debug(contratos.ToXml());
            }
            catch (Exception)
            {
            }
            List<GrabarContratoResult> results = new List<GrabarContratoResult>();
            var acuerdo = TraerContratoAcuerdoACopiar(contratos.First().ContratoAcuerdoId.Value);
            var comercial = mobjComercialManager.TraerComercial(acuerdo.ComercialId.Value);
            var materiales = mobjMaterialManager.TraerTodoMaterial().Material;
            var monedas = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            var cuitsProveedor = contratos.Select(a => a.Cuit).ToList();
            var proveedores = repositorio.Listar<Proveedor, ProveedorDto>(
                a => new ProveedorDto { ProveedorId = a.ProveedorId, CUIT = a.CUIT, RazonSocial = a.RazonSocial },
                a => cuitsProveedor.Contains(a.CUIT) && a.Segmentacion.Grupo != "Corredores").ToList();

            var cantidadEnDA = repositorio.Listar<Contrato>(x => x.ContratoAcuerdoId == acuerdo.Id && x.EstadoId != (int)EnumEstadoContrato.Rechazado && x.EstadoId != (int)EnumEstadoContrato.Eliminado).Sum(x => x.Cantidad);
            var cantidadRecibida = contratos.Sum(x => x.Cantidad);
            if (acuerdo.Cantidad < cantidadEnDA + cantidadRecibida)
            {
                var resultadoError = new GrabarContratoResult();
                resultadoError.Error("Fatal", "La cantidad que se intentó cargar supera los kilos disponibles del acuerdo (" + (acuerdo.Cantidad - cantidadEnDA).ToString("N2", CultureInfo.CreateSpecificCulture("es-AR")) + "Kg).");
                results.Add(resultadoError);
                return results;
            }
            foreach (var item in contratos)
            {
                var proveedorid = proveedores.Where(a => a.CUIT == item.Cuit).FirstOrDefault()?.ProveedorId;
                if (proveedorid == 0 || proveedorid == null)
                {
                    results.Add(new GrabarContratoResult { ContratoId = int.Parse(item.Observacion), Errores = new List<ErrorMessage> { new ErrorMessage { Source = "Proveedor", Message = "El cuit no existe." } } });
                    continue;
                }
                var proveedor = proveedores.Where(a => a.CUIT == item.Cuit).FirstOrDefault();
                var boletobolsa = mobjProveedorManager.TraerBoletoBolsa(proveedorid.Value);
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
                if (item.FechaOperacion.Value.Date < DateTime.Today)
                {
                    contrato.MotivoOperacionAnterior = "Acuerdo " + item.ContratoAcuerdoId;
                    contrato.DescripcionOperacionAnterior = "Acuerdo " + item.ContratoAcuerdoId;
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
                contrato.StandardDeCalidadId = acuerdo.StandardDeCalidadId;
                contrato.ZonaId = acuerdo.ZonaId;
                contrato.Pizarra = acuerdo.Pizarra == true;
                contrato.CaratulaMAT = acuerdo.CaratulaMAT;
                contrato.PrecioAjusteComision = acuerdo.PrecioAjusteComision;
                contrato.MonedaAjusteComisionId = acuerdo.MonedaAjusteComisionId;
                contrato.PagoDirectoVendedor = acuerdo.PagoDirectoVendedor;

                if (item.ComercialCreadorId.HasValue)
                {
                    contrato.ProveedorCreadorId = null;
                }
                else
                {
                    contrato.ProveedorCreadorId = acuerdo.CorredorId;
                }
                contrato.CorredorId = acuerdo.CorredorId;
                contrato.ComercialCreadorId = item.ComercialCreadorId;
                contrato.UsuarioId = proveedor.RazonSocial;
                contrato.ComercialId = acuerdo.ComercialId;
                contrato.ProveedorId = proveedorid;
                contrato.BoletoId = boletobolsa.BoletoCompraNetId ?? 3;
                contrato.BolsaId = boletobolsa.BolsaCompraNetId;
                contrato.PorcentajeComision = 1;
                contrato.CondicionFijacionId = acuerdo.CondicionFijacion;
                contrato.DesdeFijacion = acuerdo.DesdeFijacion;
                contrato.HastaFijacion = acuerdo.HastaFijacion;

                if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                {
                    CalcularKgMaximoYMinimo(contrato);
                }

                var existe = repositorio.Existe<Contrato>(a => a.ContratoCorredor == contrato.ContratoCorredor && a.CorredorId == item.CorredorId && a.EstadoId != (int)EnumEstadoContrato.Eliminado && a.EstadoId != (int)EnumEstadoContrato.Rechazado);
                if (existe)
                {
                    results.Add(new GrabarContratoResult { ContratoId = int.Parse(item.Observacion), Errores = new List<ErrorMessage> { new ErrorMessage { Source = "Contrato Corredor", Message = "El contrato corredor ya existe." } } });
                }
                else
                {
                    logger.Debug($"GrabarContratoMasivo contratoCorredor {item.ContratoCorredor}, proveedorid {contrato.ProveedorId}, corredorid {contrato.CorredorId}");
                    logger.Debug(contrato.ToJson());
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

            return results;
        }

        private void EnviarMailAltaMasiva(List<GrabarContratoResult> results, List<BasicoContrato> contratos, List<string> enviarA)
        {
            var cuerpoMail = CuerpoMailAltaMasiva(httpContextManager.ObtenerPathLogoMail(), results, contratos);
            mailManager.EnviarMail(enviarA, "Resultado Importación Alta Masiva", "", null, cuerpoMail);
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
                         "<td " + style + (contrato != null && contrato.Moneda != null && contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? contrato.Moneda : "") + "</td>" +
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
                error.Error("Contrato", "No existe el contrato en DataAgro");
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
            var fijacionesDeAfijar = repositorio.Listar<FijacionDePrecioContrato>(x => x.ContratoSAP == contratoSap && x.EstadoId != (int)EnumEstadoContrato.Eliminado && x.EstadoId != (int)EnumEstadoContrato.Rechazado).ToList();
            var contratoId = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == contratoSap, x => x.Id);
            var contratoAnulado = repositorio.Obtener<Contrato>(x => x.AnulaYReemplazaContratoId == contratoId);
            if (contratoAnulado != null)
            {
                error.Error("Contrato", "Este contrato ya ha sido anulado y reemplazado");
                return error;
            }
            if (fijacionesDeAfijar.Count > 0)
            {
                error.Error("Contrato A fijar", "El contrato A fijar seleccionado tiene fijaciones hechas. En caso de querer continuar con esta anulación, por favor comunicarse con administración.");
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
        //                Color = x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "" : "",
        //                Posicion = x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta)) : x.Posicion,
        //            }, x => ((x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && x.TipoAgenteCompraId == null && x.ContratoSAP.Contains(numero)) || (x.TipoNegocioId == (int)EnumTipoNegocio.AGENTE_DE_COMPRAS && x.Id == id)) && x.MonedaId == "USDM "
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
        //                Color = x.Asociado.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "" : "",
        //                Posicion = x.Asociado.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (SqlFunctions.DateName("day", x.Asociado.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.Asociado.FechaHasta) + "/" + SqlFunctions.DateName("year", x.Asociado.FechaHasta)) : x.Asociado.Posicion,
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
            logger.Debug("Enviando mail a Comercial Venta " + email);
            logger.Debug("Enviando mail a Comercial Registrado " + comercialRegistrado);
            var emailComerciales = "";

            var tienePermiso = repositorio.Obtener<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.ModificarVenta)) && x.ComercialId == contrato.ComercialCreadorId) != null;
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
            var subject = "Nuevo negocio Molinos Agro S.A. – " + (contrato.Corredor != null ? contrato.Corredor.RazonSocial : contrato.Proveedor.RazonSocial);
            if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
            {
                string typeOfRate = contrato.TipoDeCambioId == (int)EnumTipoDeCambio.BLEND ? "Z" : "M";

                var importe = CalcularImporteDeOperacion(contrato.PrecioNeto ?? contrato.Precio, contrato.Cantidad, contrato.MaterialId, contrato.FechaOperacion, contrato.MonedaId, typeOfRate);
                mailManager.EnviarMail(contrato.Comercial, emailproveedor, subject, "", lista, CuerpoMailContratoVenta(httpContextManager.ObtenerPathLogoMail(), contrato, objDescuento, objCalidad, mailManager.GetEmailUserActiveDirectory(contrato.Comercial.IdActiveDirectory), false, importe));
            }
        }

        private AlternateView CuerpoMailContratoVenta(String filePath, Contrato oContrato, List<DescuentoBonificacion> objDescuento, List<Calidad> objCalidad, string emailComercial, bool? eliminar, decimal importe)
        {
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            //string style1;
            string style2;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #017940; padding: 5px 0; width: 175px;\">";
                //style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
                //style1 = "style =\"border: 2px solid white; color:#017940; background-color: #a7dabb; padding: 5px 0; width: 250px;\">";
                style2 = "style=\"border: 2px solid white; color:#017940; background-color: #cdeadc; padding: 5px 0; width: 250px;\">";
            }

            var linea = 0;

            string htmlBody = "";
            if (eliminar.HasValue && eliminar.Value)
            {
                htmlBody += "En el presente mail se detalla el negocio eliminado con Molinos Agro S.A.: <br /><br />  ";
            }
            else
            {
                htmlBody += "En el presente mail se detalla el nuevo negocio generado con Molinos Agro S.A.: <br /><br />  ";
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
            htmlBody += "<tr>" + th + "GRANO</th>" + Td(ref linea) + oContrato.Material.Descripcion.ToUpper() + (oContrato.Sustentable ? " (Sustentable)" 
                : oContrato.EPA && !oContrato.EUDR ? " (EPA)" : oContrato.EUDR && !oContrato.EPA ? " (EUDR)" : oContrato.EPA && oContrato.EUDR  ? " (EPA/EUDR)" : "") + "</td></tr>";
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

            htmlBody += "<tr>" + th + "CANTIDAD</th>" + Td(ref linea) + Split(Math.Abs(oContrato.Cantidad).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))
                + " Kg.";
            if (oContrato.CantidadCamiones != null)
            {
                htmlBody += " (" + oContrato.CantidadCamiones + " camiones)<br />";
            }
            htmlBody += "</td></tr>";
            htmlBody += "<tr>" + th + "PRECIO</th>" + Td(ref linea);
            if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
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
            else if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
            {
                htmlBody += "A FIJAR HASTA: <br />" + Split(oContrato.HastaFijacion.Value.ToShortDateString()) + "<br />" + Split(oContrato.CondicionFijacion.Descripcion.ToUpper()) + "</td></tr>";
            }
            htmlBody += "<tr>" + th + "PORCENTAJE DE PAGO</th>" + Td(ref linea) + oContrato.PorcentajeDePago.ToString() + "</td></tr>";
            htmlBody += "<tr>" + th + "PROCEDENCIA</th>" + Td(ref linea) + (oContrato.Destino.Localidad != null ?
                oContrato.Destino.Localidad.Nombre.ToUpper() + " (" + oContrato.Destino.Localidad.Provincia.Nombre.ToUpper() + ")" : oContrato.Destino.Descripcion) + "</td></tr>";
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
            if (oContrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
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
                htmlBody += (oContrato.Sustentable ? "SUSTENTABLE " : oContrato.EPA && oContrato.EUDR ? "EPA/EUDR " : !oContrato.EPA && oContrato.EUDR ? "EUDR " : "EPA ") + oContrato.ImporteSustentable + " " + oContrato.MonedaSustentable.Descripcion.ToUpper() + "<br />";
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
                htmlBody += "MERCADERÍA EN DEPÓSITO<br /> ";
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
                htmlBody += "CÁMARA<br />";
            }
            else if (oContrato.StandardDeCalidadId == 3)
            {
                htmlBody += "FÁBRICA<br />";
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
            htmlBody += "<tr>" + th + "BOLETO </th>" + Td(ref linea) + (oContrato.BoletoVenta != null ? oContrato.BoletoVentaId == 5 ? "A Convenir con " + oContrato.MailVentaBoleto : oContrato.BoletoVenta.Descripcion : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "CÁMARA </th>" + Td(ref linea) + (oContrato.Camara != null ? oContrato.Camara.Descripcion : "NO") + "</td></tr>";
            htmlBody += "<tr>" + th + "COMISIÓN A FAVOR </th>" + Td(ref linea) + (oContrato.ComisionAFavor != null ?
                oContrato.ComisionAFavor.Descripcion + " " + oContrato.PorcentajeComisionVenta + "%" : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "FLETE A CARGO </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.FleteACargo) ? oContrato.FleteACargo : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "KG BALANZA </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.KgBalanza) ? oContrato.KgBalanza : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "PAGO </th>" + Td(ref linea) + (!string.IsNullOrEmpty(oContrato.Pago) ? oContrato.Pago : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "DESTINO MERCADERÍA</th>" + Td(ref linea) + (oContrato.ProcedenciaVenta != null ?
               oContrato.ProcedenciaVenta.Nombre + " (" + oContrato.ProcedenciaVenta.Provincia.Nombre + ")" : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "IMPORTE DE LA OPERACIÓN</th>" + Td(ref linea) + Split(importe.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + "</td></tr>";

            //htmlBody += "<tr>" + th + "CREDITO DISPONIBLE</th>" + Td(ref linea) + (oContrato.CreditoDisponible.HasValue ? Split(oContrato.CreditoDisponible.Value.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR"))) + " " + oContrato.MonedaCreditoDisponible : "") + "</td></tr>";

            htmlBody += "<tr>" + th + "CONDICIÓN DE PAGO</th>" + Td(ref linea) + (oContrato.CondicionDePagoFijacionVenta != null ?
               oContrato.CondicionDePagoDiaFijacion + " dias " + oContrato.CondicionDePagoTipoFijacion + " " + oContrato.CondicionDePagoFijacionVenta.Descripcion : "") + "</td></tr>";
            htmlBody += "<tr>" + th + "CONDICIÓN DE PESIFICACIÓN</th>" + Td(ref linea) + (oContrato.CondicionDePagoPesificadoVenta != null ?
              oContrato.CondicionDePagoDiaPesificado + " dias " + oContrato.CondicionDePagoTipoPesificado + " " + oContrato.CondicionDePagoPesificadoVenta.Descripcion : "") + "</td></tr>";
            htmlBody += "</table>";

            htmlBody += "<br /><br /> Por favor, revisar que los datos sean correctos; de lo contrario contactarse con " + (oContrato.Comercial != null ? oContrato.Comercial.Nombres + " " + oContrato.Comercial.Apellido + (emailComercial != "" && emailComercial != null ? " (" + emailComercial + ")." : ".") : "Mesa de Ayuda.") +
                "<br /> <br />  Saludos Cordiales," +
                " <br /> <br />   Molinos Agro S.A.  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                "<br /> <br /> www.molinosagro.com.ar";
            AlternateView alternateView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            alternateView.LinkedResources.Add(res);
            return alternateView;
        }

        public List<NegocioAsociadoDto> DevolverContratosParaAsociar(int contratoId, string numero)
        {
            var aFijar = repositorio.Obtener<Contrato>(x => x.Id == contratoId);
            if (aFijar != null)
            {
                Int32.TryParse(numero, out int id);
                var contratoSap = numero == "" ? numero : "000" + id.ToString();
                var negociosAsociados = repositorio.Listar<NegocioAsociado, int>(x => x.AsociadoId);
                var negocios = repositorio.Listar<Negocio, NegocioAsociadoDto>(
                    x => new NegocioAsociadoDto()
                    {
                        Id = x.Id,
                        Contrato = !string.IsNullOrEmpty(x.ContratoSAP) ? x.ContratoSAP : x.Id.ToString(),
                        ContratoSap = x.ContratoSAP,
                        TipoNegocioDesc = x.TipoNegocio.Descripcion,
                        TipoNegocioId = x.TipoNegocioId,
                        Precio = x.Precio,
                        MonedaDesc = "USD",
                        MaterialDesc = x.Material.Descripcion,
                        Cantidad = x.Cantidad,
                        Campania = x.Campana.Descripcion,
                        Color = x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "" : "",
                        Posicion = x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (SqlFunctions.DatePart("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DatePart("year", x.FechaHasta)) : x.Posicion,
                    }, x => ((x.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO && x.TipoAgenteCompraId == null && (aFijar.PosicionCBOT == (x.FechaHasta.Month + "." + x.FechaHasta.Year).ToString()) && (x.ContratoSAP.StartsWith(contratoSap) || contratoSap == "") && (x.EstadoId == (int)EnumEstadoContrato.Finalizado || x.EstadoId == (int)EnumEstadoContrato.ReconfirmarFinalizado || x.EstadoId == (int)EnumEstadoContrato.PreAnulado)) ||
                     (x.TipoNegocioId == (int)EnumTipoNegocio.AGENTE_DE_COMPRAS && aFijar.PosicionCBOT == x.Posicion && x.EstadoId == (int)EnumEstadoContrato.Confirmado && (x.Id.ToString().StartsWith(id.ToString()) || id.ToString() == "0"))) && x.MaterialId == aFijar.MaterialId && x.MonedaId == "USDM " &&
                     x.CampanaId > x.Material.CampaniaTableroId && !negociosAsociados.Contains(x.Id));
                negocios = negocios/*.Where(x => EstaConfirmadoEnSAP(x.ContratoSap, x.TipoNegocioId))*/.Take(10).ToList();
                if (negocios.Count > 0)
                {
                    foreach (var item in negocios)
                    {
                        item.Identificador = (!string.IsNullOrEmpty(item.ContratoSap) ? item.ContratoSap : item.Id.ToString()) + " - " + item.TipoNegocioDesc + " - " + item.Precio.ToString("N2") + " USD - " + item.MaterialDesc + " - " + item.Cantidad.ToString("N0") + " Kg - " + item.Posicion;
                        item.Contrato = !string.IsNullOrEmpty(item.ContratoSap) ? item.ContratoSap.TrimStart('0') : item.Id.ToString();
                    }
                }

                return negocios;
            }
            return new List<NegocioAsociadoDto>();
        }

        public bool EstaConfirmadoEnSAP(string contratoSAP, int TipoNegocioId)
        {
            if (TipoNegocioId != 2) return true;
            var estado = status.ValidarEstado(contratoSAP);
            return string.IsNullOrEmpty(estado.Status) ? false : true;
        }

        public List<NegocioAsociadoDto> DevolverContratoAsociadosPase(int contratoId)
        {
            var aFijar = repositorio.Obtener<Contrato>(x => x.Id == contratoId);
            if (aFijar != null)
            {
                var negociosQueYaEstanAsociados = repositorio.Listar<NegocioAsociado, NegocioAsociadoDto>(
                    x => new NegocioAsociadoDto()
                    {
                        Id = x.Asociado.Id,
                        Contrato = !string.IsNullOrEmpty(x.Asociado.ContratoSAP) ? x.Asociado.ContratoSAP : x.Asociado.Id.ToString(),
                        TipoNegocioDesc = x.Asociado.TipoNegocio.Descripcion,
                        Precio = x.Asociado.Precio,
                        MonedaDesc = "USD",
                        MaterialDesc = x.Asociado.Material.Descripcion,
                        Cantidad = x.Asociado.Cantidad,
                        Campania = x.Asociado.Campana.Descripcion,
                        Color = x.Asociado.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? "" : "",
                        Posicion = x.Asociado.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO ? (SqlFunctions.DateName("day", x.Asociado.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.Asociado.FechaHasta) + "/" + SqlFunctions.DateName("year", x.Asociado.FechaHasta)) : x.Asociado.Posicion,
                    }, x => x.AFijarId == aFijar.Id);
                return negociosQueYaEstanAsociados;
            }
            return new List<NegocioAsociadoDto>();
        }

        public decimal CalcularImporteDeOperacion(decimal precio, double cantidad, int materialId, DateTime fechaoperacion, string moneda, string typeOfRate)
        {
            var diaAnterior = fechaoperacion.AddDays(-1);
            var cambio = tipoCambioAgent.TraerTipoDeCambio(diaAnterior, typeOfRate);
            if (moneda == "ARP  ")
            {
                precio = precio / cambio;
            }

            var iva = mobjMaterialManager.DevolverIVAPorMaterial(materialId);
            iva = iva / 100 + 1;
            var importe = (precio * (decimal)cantidad / 1000) * iva;
            return importe;
        }

        public Resultado EliminarNegociosAsociados(int contratoId)
        {
            var oEntityErrors = new Resultado();
            try
            {
                repositorio.RemoverTodos<NegocioAsociado>(x => x.AFijarId == contratoId);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error(e);
                oEntityErrors.Error("", e.Message);
            }

            return oEntityErrors;
        }

        public Resultado GrabarNegociosAsociados(List<NegocioAsociadoDto> negocios, int contratoId, decimal precioPonderado)
        {
            var oEntityErrors = EliminarNegociosAsociados(contratoId);
            var listaEntidad = new List<NegocioAsociado>();
            if (negocios != null && negocios.Count > 0)
            {
                foreach (var item in negocios)
                {
                    var asociado = new NegocioAsociado
                    {
                        AFijarId = contratoId,
                        AsociadoId = item.Id
                    };
                    listaEntidad.Add(asociado);
                }
                try
                {
                    var aFijar = repositorio.Obtener<Negocio>(x => x.Id == contratoId);
                    aFijar.PrecioPonderado = precioPonderado;
                    aFijar.PrecioNetoPonderado = CalcularPrecioPonderadoEnAFijarPaseNeto(aFijar, precioPonderado);
                    repositorio.AgregarTodos(listaEntidad);
                    repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);
                }
            }
            return oEntityErrors;
        }

        public decimal CalcularPrecioPonderadoEnAFijarPaseNeto(Negocio aFijar, decimal precioPonderado)
        {
            logger.Debug("CalcularPrecioPonderadoEnAFijarPaseNeto precioPonderado " + precioPonderado);
            decimal precioNeto = precioPonderado;
            decimal porcentajeComision = 0;
            var comision = aFijar.Descuentos.Where(y => y.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO && y.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && (y.Porcentaje != 0)).SingleOrDefault();
            if ((aFijar.CorredorId == null || aFijar.CorredorId == 0) && comision != null)
            {
                porcentajeComision = comision.Porcentaje / 100;
            }

            //Bonificacion
            var desc = aFijar.Descuentos.Where(y => y.TipoDBId == (int)EnumTipoDB.SOBRE_EL_PRECIO && y.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && (y.Importe != 0)).SingleOrDefault();
            if (desc != null)
            {
                precioNeto += desc.Importe;
                logger.Debug("CalcularPrecioPonderadoEnAFijarPaseNeto desc.Importe " + desc.Importe);
            }
            if (porcentajeComision > 0)
            {
                precioNeto += precioNeto * porcentajeComision;
            }
            logger.Debug("CalcularPrecioPonderadoEnAFijarPaseNeto precioNeto " + precioNeto);

            return precioNeto;
        }

        public bool TieneAsociados(int contratoId)
        {
            var lista = DevolverContratoAsociadosPase(contratoId);
            return lista.Count > 0;
        }

        public DataSourceResult TraerContratosReporteAFijarPase(DataSourceRequest filtro, List<int> equipo)
        {
            var result = repositorio.ObtenerConsultaEscalar(new TraerContratosReporteAFijarPase(filtro, equipo));

            List<string> contratosAFijarPaseSAPList = ((List<ReporteAfijarPaseDto>)result.Data).Select(x => x.Negocio).ToList();
            var fijacionesPase = repositorio.Listar<FijacionDePrecioContrato, BasicoContrato>(a => new BasicoContrato { ContratoSAP = a.ContratoSAP, Cantidad = a.Cantidad }, a => a.EstadoId == (int)EnumEstadoContrato.Finalizado && contratosAFijarPaseSAPList.Contains(a.ContratoSAP));
            var fijacionesKilos = fijacionesPase.GroupBy(a => a.ContratoSAP).Select(x => new BasicoContrato { ContratoSAP = x.Key, Cantidad = x.Sum(y => y.Cantidad) }).ToList();

            foreach (var afijar in (List<ReporteAfijarPaseDto>)result.Data)
            {
                var fijado = fijacionesKilos.Where(x => x.ContratoSAP == afijar.Negocio).SingleOrDefault();
                if (fijado == null || afijar.Cantidad > fijado.Cantidad)
                {
                    afijar.KilosPendiente = afijar.Cantidad - (fijado == null ? 0 : fijado.Cantidad);
                }
            }
            return result;
        }

        public bool EsUnContratoAsociado(int negocioId)
        {
            return repositorio.Obtener<NegocioAsociado>(x => x.AsociadoId == negocioId) == null ? false : true;
        }

        public string ValidarProveedor(int proveedorId, int clasificacion, bool planCanje = false, bool consignatario = false)
        {
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == proveedorId);
            var mensaje = "";
            var sisa = new SISA();
            if (proveedor != null && clasificacion != 0)
            {
                if (clasificacion == 1)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == (int)EnumEstadoSisa.PRODUCTOR && x.SituacionCategoria == "AL");
                }
                else if (clasificacion == 2)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria == (int)EnumEstadoSisa.ACOPIADOR && x.SituacionCategoria == "AL");
                }
                else if (clasificacion == 3)
                {
                    sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && x.CodCategoria != (int)EnumEstadoSisa.PRODUCTOR && x.CodCategoria != (int)EnumEstadoSisa.ACOPIADOR && x.CodCategoria != (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS && x.SituacionCategoria == "AL");
                }
                if (sisa != null)
                {
                    if (sisa.EstadoCuit == 3 && proveedor.RiesgoComercialSap != "E")
                    {
                        mensaje = "Proveedor No Operable por Estado de CUIT 3";
                        return mensaje;
                    }
                    else if (sisa.EstadoCuit == 0)
                    {
                        mensaje = "Proveedor No Operable por Estado de CUIT Inactivo";
                        return mensaje;
                    }
                    if (sisa.SituacionCategoria != "AL")
                    {
                        mensaje = "Proveedor No Operable por Situación Categoría BA";
                        return mensaje;
                    }
                    if (sisa.CodCategoria == (int)EnumEstadoSisa.OPERADOR_DE_DERIVADOS_GRANARIOS)
                    {
                        mensaje = "No operable por categoría Operador de Derivados Granarios";
                    }
                }
                else
                {
                    mensaje = "Proveedor No Operable por CUIT o Categoría Inactivo";
                    return mensaje;
                }
                if (planCanje || consignatario)
                {
                    string tipoProv = proveedor.SegmentacionId == 5 || proveedor.SegmentacionId == 7 ? "CORR" : "PROV";
                    var alta = altaTempranaAgent.ObtenerAlta(proveedor.CUIT, tipoProv);

                    if (string.IsNullOrEmpty(alta.Mensaje))
                    {
                        if (consignatario && alta.Consignatario == "NO")
                        {
                            mensaje = "El proveedor no está habilitado como Consignatario";
                        }
                        if (planCanje && alta.PlanCanje == "NO")
                        {
                            mensaje = "El proveedor no está habilitado como Proveedor Plan canje";
                        }
                    }
                }
            }
            return mensaje;
        }

        public bool ValidarCopiarContrato(int id)
        {
            var contrato = repositorio.Obtener<Contrato>(id);
            var permisos = false;
            if (PermisosHelper.Is(PermisosDataAgro.ModificarCanje) && contrato.Canje == true)
            {
                permisos = true;
                return permisos;
            }
            if (PermisosHelper.Is(PermisosDataAgro.ModificarVenta) && contrato.Venta == true)
            {
                permisos = true;
                return permisos;
            }
            if (PermisosHelper.Is(PermisosDataAgro.ModificarPrestamoDevolucion) && contrato.PrestamoDevolucion == true)
            {
                permisos = true;
                return permisos;
            }
            if (contrato.Canje != true && contrato.Venta != true && contrato.PrestamoDevolucion != true && !PermisosHelper.Is(PermisosDataAgro.ModificarCanje))
            {
                permisos = true;
                return permisos;
            }

            return permisos;
        }

        private double ValidarSiCumpleLaTolerancia(int contratoId)
        {
            return DevolverContratoAsociadosPase(contratoId).Sum(x => x.Cantidad);
        }

        public GrabarContratoResult GrabarContratoAPrecioTercero(Contrato contrato)
        {
            contrato.Base = false;
            contrato.NoInformaSio = false;
            contrato.TrigoEspecial = false;
            contrato.EsFason = false;
            if (contrato.ComercialId == null || contrato.ComercialId == 0)
            {
                contrato.ComercialId = mobjComercialManager.ComercialAsociado(contrato.CorredorId.HasValue && contrato.CorredorId != 0 ? contrato.CorredorId.Value : contrato.ProveedorId ?? 0);
            }

            var comercial = mobjComercialManager.TraerComercial(contrato.ComercialId.Value);
            var proveedorCreador = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            if (contrato.CorredorId > 0)
            {
                contrato.PorcentajeComision = 1;
            }
            contrato.UsuarioId = proveedorCreador.RazonSocial;
            contrato.PrecioNeto = contrato.Precio;
            if (contrato.AperturaPrecio == null)
            {
                contrato.AperturaPrecio = new List<AperturaPrecio>();
                foreach (EnumConceptoApertura concepto in (EnumConceptoApertura[])Enum.GetValues(typeof(EnumConceptoApertura)))
                {
                    contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = (int)concepto, Importe = 0, MonedaId = null, Porcentaje = 0 });
                }
            }

            if (contrato.PagoDiferidoTercero == true)
            {
                var pago = configuracionInternaManager.TraerPagosDiferido().Where(x => x.CantidadDia >= contrato.DiasPesificado).OrderBy(x => x.CantidadDia).FirstOrDefault();
                if (pago == null)
                {
                    return new GrabarContratoResult { Errores = new List<ErrorMessage> { new ErrorMessage { Source = "PagoDiferido", Message = "No hay una tasa de pago diferido para esa cantidad de dias." } } };
                }

                contrato.PagoDiferido = contrato.PagoDiferidoTercero;
                //var ImporteFinanciero = Math.Round(contrato.Precio * (pago.Tasa / 100) * (contrato.DiasPesificado.Value - 3) / 365 * 2, MidpointRounding.AwayFromZero) / 2;
                decimal ImporteFinanciero = Math.Round(contrato.Precio * (pago.Tasa / 100) * (contrato.DiasPesificado.Value - 3) / 365);
                ImporteFinanciero = Redondear(ImporteFinanciero);

                contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 1).Importe = ImporteFinanciero;
                contrato.PrecioNeto += ImporteFinanciero;
            }

            if (contrato.Pizarra != true)
            {
                if ((contrato.CorredorId == null || contrato.CorredorId == 0) && proveedor.Comision > 0)
                {
                    if (contrato.MaterialId != (int)EnumMateriales.GIRASOL && contrato.MaterialId != (int)EnumMateriales.GIRASOL_AO)
                    {
                        contrato.PrecioNeto += contrato.PrecioNeto * proveedor.Comision.Value / 100;
                        contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 3).Porcentaje = proveedor.Comision.Value;
                    }
                    else
                    {
                        if (contrato.Descuentos == null)
                            contrato.Descuentos = new List<DescuentoBonificacion>();
                        var desc = contrato.Descuentos.Where(a => a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && a.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO).FirstOrDefault();
                        if (desc == null)
                        {
                            contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 2, Porcentaje = proveedor.Comision.Value });
                        }
                        else
                        {
                            desc.Porcentaje = proveedor.Comision.Value;
                        }
                    }
                }
            }

            if (contrato.ComercialId.HasValue)
            {
                contrato.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (contrato.StandardDeCalidadId == 2)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 } };
            }
            if (contrato.StandardDeCalidadId == 7)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 } };
            }
            if (contrato.MaterialId == (int)EnumMateriales.GIRASOL_AO)
            {
                contrato.ZonaId = 1;
            }
            else
            {
                contrato.ZonaId = null;
            }

            contrato.PorcentajeDePago = 97.5m;
            contrato.PagoDiferidoTerceroId = contrato.PagoDiferidoTerceroId == -1 ? (int?)null : contrato.PagoDiferidoTerceroId;

            return GrabarContrato(contrato);
        }

        public GrabarContratoResult GrabarContratoAFijarTercero(Contrato contrato)
        {
            contrato.Base = false;
            contrato.NoInformaSio = false;
            contrato.TrigoEspecial = false;
            contrato.EsFason = false;

            if (contrato.ComercialId == null || contrato.ComercialId == 0)
            {
                contrato.ComercialId = mobjComercialManager.ComercialAsociado(contrato.CorredorId.HasValue && contrato.CorredorId != 0 ? contrato.CorredorId.Value : contrato.ProveedorId ?? 0);
            }
            var comercial = mobjComercialManager.TraerComercial(contrato.ComercialId.Value);
            var proveedorCreaador = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            var proveedor = mobjProveedorManager.TraerProveedor(contrato.ProveedorCreadorId.Value, comercial.IdActiveDirectory, new List<int>()).BasicoProveedorTraerPorProveedores.First();
            contrato.UsuarioId = proveedorCreaador.RazonSocial;
            if (contrato.CorredorId > 0)
            {
                contrato.PorcentajeComision = 1;
            }
            if (contrato.ComercialId.HasValue)
            {
                contrato.GrupoCompra = comercial.GrupoDeComprasId ?? 0;
            }
            if (contrato.StandardDeCalidadId == 2)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 } };
            }
            if (contrato.StandardDeCalidadId == 7)
            {
                contrato.Calidad = new List<Calidad> { new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 } };
            }
            if (contrato.MaterialId == (int)EnumMateriales.GIRASOL_AO)
            {
                contrato.ZonaId = 1;
            }
            else
            {
                contrato.ZonaId = null;
            }

            if ((contrato.CorredorId == null || contrato.CorredorId == 0) && proveedor.Comision > 0)
            {
                if (contrato.MaterialId != (int)EnumMateriales.GIRASOL && contrato.MaterialId != (int)EnumMateriales.GIRASOL_AO)
                {
                    if (contrato.AperturaPrecio == null)
                    {
                        contrato.AperturaPrecio = new List<AperturaPrecio>();
                        foreach (EnumConceptoApertura concepto in (EnumConceptoApertura[])Enum.GetValues(typeof(EnumConceptoApertura)))
                        {
                            contrato.AperturaPrecio.Add(new AperturaPrecio { ConceptoAperturaPrecioId = (int)concepto, Importe = 0, MonedaId = null, Porcentaje = 0 });
                        }
                    }
                    contrato.AperturaPrecio.First(x => x.ConceptoAperturaPrecioId == 3).Porcentaje = proveedor.Comision.Value;
                }
                else
                {
                    if (contrato.Descuentos == null)
                        contrato.Descuentos = new List<DescuentoBonificacion>();
                    var desc = contrato.Descuentos.Where(a => a.TipoPeriodoDBId == (int)EnumTipoPeriodoDB.GENERALES && a.TipoDBId == (int)EnumTipoDB.POR_FUERA_DEL_PRECIO).FirstOrDefault();
                    if (desc == null)
                    {
                        contrato.Descuentos.Add(new DescuentoBonificacion { TipoPeriodoDBId = 1, TipoDBId = 2, Porcentaje = proveedor.Comision.Value });
                    }
                    else
                    {
                        desc.Porcentaje = proveedor.Comision.Value;
                    }
                }
            }

            contrato.PorcentajeDePago = 97.5m;

            return GrabarContrato(contrato);
        }

        private decimal Redondear(decimal numero)
        {
            double final;
            double d10 = decimal.ToDouble(numero) / 10.00;
            final = Math.Round(d10 * 2, MidpointRounding.AwayFromZero) / 2;
            final = final * 10;
            return Convert.ToDecimal(final);
        }

        public bool Tiene2doCondicionalAsociado(int contratoId)
        {
            return repositorio.Existe<Contrato>(x =>
            x.Id == contratoId &&
            x.Condicional == true &&
            x.CondicionalContratos.Any(a =>
                a.EstadoId != (int)EnumEstadoContrato.Eliminado ||
                a.EstadoId != (int)EnumEstadoContrato.Rechazado)
            );
        }

        public int DevolverMilisegundos()
        {
            var ms = 30000;
            if (PermisosHelper.Is(PermisosDataAgro.ActualizarCompraNet))
            {
                var configuracion = configuracionManager.TraerConfiguraciones();
                ms = configuracion != null ? configuracion.Actualizacion : 30000;
            }
            return ms;
        }

        public CcPpPendienteAplicarDto ObtenerDatosMercaderiaEnDeposito(int? materialId, int? id, int? centro, int? corredorId, int? proveedorId, bool? tieneSustentable, bool? sinBoleto)
        {
            var disponible = new CcPpPendienteAplicarDto();
            id = id ?? 0;
            if (centro != null && materialId != null && proveedorId != null)
            {
                var centroCodigo = repositorio.Obtener<Centro, string>(x => x.Id == centro, x => x.CodigoSap);
                var cuitProveedor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == proveedorId, x => x.CUIT);
                var cuitCorredor = repositorio.Obtener<Proveedor, string>(x => x.ProveedorId == corredorId, x => x.CUIT);
                var materialCodigo = repositorio.Obtener<Material, string>(x => x.MaterialId == materialId, x => x.Codigo);
                DateTime hoyInclusive = DateTime.Today.AddDays(1);
                var contratosPendientes = repositorio.Listar<Contrato>(x => x.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO && x.Id != id && x.Proveedor.CUIT == cuitProveedor && x.DestinoId == centro && x.FechaDesde <= hoyInclusive &&
                x.MaterialId == materialId && x.EstadoId != (int)EnumEstadoContrato.Finalizado && x.EstadoId != (int)EnumEstadoContrato.Rechazado && x.EstadoId != (int)EnumEstadoContrato.Eliminado);

                var pendienteDto = new CcPpPendienteAplicarDto()
                {
                    Centro = centroCodigo,
                    Material = materialCodigo,
                    Proveedor = cuitProveedor,
                    Corredor = cuitCorredor,
                };

                var ccppPendientes = ListarCartasDePortePendienteAplicar(pendienteDto);
                if (ccppPendientes != null && ccppPendientes.Count > 0)
                {
                    var cantidadCartaDePorte = ccppPendientes.Where(x => !string.IsNullOrEmpty(x.CartasPorte)).Sum(x => x.Cantidad);
                    var cantidadDisponible = negocioManager.DevolverCantidadDisponible(tieneSustentable.GetValueOrDefault(), contratosPendientes, ccppPendientes, sinBoleto.GetValueOrDefault());
                    disponible.CantidadDisponible = (decimal)cantidadDisponible < 0 ? 0 : (decimal)cantidadDisponible;
                    disponible.CantidadTotal = cantidadCartaDePorte;
                }
            }
            return disponible;
        }

        public List<ExcelValidatorResumeItem> AltaMasivaContratos(DataSet dsExcel, string contratoAcuerdo, int ComercialId)
        {
            List<string> errores = new List<string>();
            try
            {
                int ncontratoAcuerdo;
                if (!int.TryParse(contratoAcuerdo, out ncontratoAcuerdo))
                {
                    throw new Exception("Debe seleccionar el contrato acuerdo.");
                }
                BasicoContrato acuerdo = negocioManager.TraerAcuerdo(ncontratoAcuerdo);
                if (acuerdo.Id == 0)
                {
                    throw new Exception("El Acuerdo seleccionado no es valido.");
                }
                if (dsExcel.Tables.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].TableName != "AltaMasiva")
                {
                    if (dsExcel.Tables[0].TableName == "Data")
                    {
                        throw new Exception("El documento no contiene información de contratos.");
                    }
                    else
                    {
                        throw new Exception("El documento no tiene el formato correcto. Utilice el Archivo Modelo");
                    }
                }

                var materiales = mobjMaterialManager.TraerTodoMaterial();
                var centros = centroManager.TraerTodoCentro();
                var campanias = mobjCampaniaManager.TraerTodoCampania();
                var validations = GetValidatorContratos(materiales.Material, centros.Centro);
                var validator = new ExcelValidator(validations);

                var resultValidation = validator.Validate(dsExcel.Tables[0], false);

                if (!resultValidation.IsValid)
                {
                    return resultValidation.Resume;
                    //return Json(new { Resume = resultValidation.Resume, Resultado = !resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    List<BasicoContrato> contratos = new List<BasicoContrato>();
                    int tiponegocioid = acuerdo.Precio > 0 ? 2 : 1;
                    List<int> rowsOk = resultValidation.RowsResult.Where(a => !a.HasError).Select(a => a.Row).ToList();
                    if (rowsOk.Count == 0)
                    {
                        return resultValidation.Resume;
                        //return Json(new { Resume = resultValidation.Resume, Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                    }
                    var rows = dsExcel.Tables[0].AsEnumerable().Select(x => x.ItemArray).Skip(0);
                    for (int ii = 0; ii < rows.Count(); ii++)
                    {
                        if (!rowsOk.Contains(ii))
                            continue;
                        var contrato = new BasicoContrato();
                        contrato.ContratoAcuerdoId = acuerdo.Id;
                        contrato.CorredorId = acuerdo.CorredorId;

                        contrato.ContratoCorredor = rows.ElementAt(ii)[0].ToString().Trim();
                        contrato.ContratoVendedor = rows.ElementAt(ii)[1].ToString().Trim();
                        contrato.MaterialId = materiales.Material.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[2].ToString().Trim().ToLower()).Single().MaterialId;
                        contrato.CampanaId = campanias.Where(a => a.Descripcion.Replace("-", "").ToLower() == rows.ElementAt(ii)[3].ToString().Trim().ToLower()).Single().CampañaId;
                        //contrato.Fecha = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaOperacion = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaDesde = DateTime.Parse(rows.ElementAt(ii)[5].ToString().Trim());
                        contrato.FechaHasta = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.FechaEntrega = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.Cantidad = int.Parse(rows.ElementAt(ii)[7].ToString().Trim());
                        contrato.Cuit = rows.ElementAt(ii)[8].ToString().Trim();
                        contrato.ClasificacionId = rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "productor" ? 1 : rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "acopiador" ? 2 : 3;
                        contrato.PlanCanje = rows.ElementAt(ii)[10].ToString().Trim().ToUpper() == "X";
                        contrato.Consignatario = rows.ElementAt(ii)[11].ToString().Trim().ToUpper() == "X";
                        contrato.DestinoId = centros.Centro.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[12].ToString().Trim().ToLower()).Single().Id;
                        contrato.LocalidadId = int.Parse(rows.ElementAt(ii)[13].ToString().Trim());
                        contrato.ProvinciaId = int.Parse(rows.ElementAt(ii)[14].ToString().Trim());
                        contrato.Observacion = ii.ToString().Trim();
                        contrato.ComercialCreadorId = ComercialId;

                        if (DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim()) < DateTime.Today)
                        {
                            contrato.MotivoOperacionAnterior = "Alta masiva";
                        }

                        //contrato.UsuarioTercero = GlobalVariables.ComercialId;
                        contratos.Add(contrato);
                    }
                    contratos.ForEach(x => x.MotivoOperacionAnterior = "Alta Masiva Acuerdo.");
                    ValidacionContratoFatal(contratos, acuerdo, resultValidation);
                    if (!resultValidation.IsValid)
                    {
                        List<ExcelValidatorResumeItem> erroresList = new List<ExcelValidatorResumeItem>();
                        foreach (var item in resultValidation.RowsResult)
                        {
                            List<string> errorsList = new List<string>();
                            ExcelValidatorResumeItem erroresItem = new ExcelValidatorResumeItem();
                            erroresItem.ContratoCorredor = item.ContratoCorredor;
                            erroresItem.Row = item.Row;

                            foreach (var item2 in item.ItemsResult)
                            {
                                errorsList.AddRange(item2.Errors);
                            }
                            erroresItem.Errors = errorsList;

                            erroresList.Add(erroresItem);
                        }
                        return erroresList;
                        //return Json(new { Resume = erroresList, Resultado = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        List<GrabarContratoResult> resultados = GrabarContratoMasivo(contratos);

                        foreach (var item in resultados)
                        {
                            if (item.HayError)
                            {
                                //item.ContratoId estoy usando ese campo para devolver el numero de row
                                resultValidation.RowsResult[item.ContratoId ?? 0].ItemsResult.Add(new ExcelValidatorItemResult { Errors = item.Errores.Select(a => a.Message).ToList(), Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "", Options = null, Position = 1, Required = true, Type = ExcelValidationColumnType.String } });
                                //errores.AddRange(item.Errores.Select(a => a.Message).ToList());
                            }
                        }
                        return resultValidation.Resume.OrderBy(x => x.HasError).ToList();
                        //return Json(new { Resume = resultValidation.Resume.OrderBy(x => x.HasError).ToList(), Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public List<ExcelValidatorResumeItem> AltaMasivaConvenios(DataSet dsExcel, int ComercialId)
        {
            List<string> errores = new List<string>();
            try
            {
                if (dsExcel.Tables.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].TableName != "AltaMasiva")
                {
                    if (dsExcel.Tables[0].TableName == "Data")
                    {
                        throw new Exception("El documento no contiene información de contratos.");
                    }
                    else
                    {
                        throw new Exception("El documento no tiene el formato correcto. Utilice el Archivo Modelo");
                    }
                }
                var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
                var materiales = mobjMaterialManager.TraerTodoMaterial();
                var centros = centroManager.TraerTodoCentro();
                var campanias = mobjCampaniaManager.TraerTodoCampania();
                var tipoBoletos = repositorio.Listar<BoletoCompraNet>();
                var bolsas = repositorio.Listar<BolsaCompraNet>();
                var tipoDB = repositorio.Listar<TipoDB>();
                var condicionFijacion = repositorio.Listar<CondicionFijacion>();
                var validations = GetValidatorConvenios(materiales.Material, centros.Centro);
                var validator = new ExcelValidator(validations);

                var resultValidation = validator.Validate(dsExcel.Tables[0], false);

                if (!resultValidation.IsValid)
                {
                    return resultValidation.Resume;
                }
                else
                {
                    List<Contrato> contratos = new List<Contrato>();
                    List<int> rowsOk = resultValidation.RowsResult.Where(a => !a.HasError).Select(a => a.Row).ToList();
                    if (rowsOk.Count == 0)
                    {
                        return resultValidation.Resume;
                    }
                    var rows = dsExcel.Tables[0].AsEnumerable().Select(x => x.ItemArray).Skip(0);
                    for (int ii = 0; ii < rows.Count(); ii++)
                    {
                        if (!rowsOk.Contains(ii))
                            continue;
                        var contrato = new Contrato();
                        contrato.TipoNegocioId = 1;
                        contrato.ContratoCorredor = rows.ElementAt(ii)[1].ToString().Trim();
                        contrato.ContratoVendedor = rows.ElementAt(ii)[1].ToString().Trim();
                        contrato.MaterialId = materiales.Material.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[2].ToString().Trim().ToLower()).Single().MaterialId;
                        contrato.CampanaId = campanias.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[3].ToString().Trim().ToLower()).Single().CampañaId;
                        //contrato.Fecha = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaOperacion = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaDesde = DateTime.Parse(rows.ElementAt(ii)[5].ToString().Trim());
                        contrato.FechaHasta = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.FechaEntrega = DateTime.Parse(rows.ElementAt(ii)[6].ToString().Trim());
                        contrato.Cantidad = int.Parse(rows.ElementAt(ii)[7].ToString().Trim());
                        var cuitProveedor = rows.ElementAt(ii)[8].ToString().Trim();
                        var cuitCorredor = rows.ElementAt(ii)[9].ToString().Trim();
                        var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuitProveedor && x.SegmentacionId < 5);
                        if (proveedor != null)
                        {
                            contrato.ProveedorId = proveedor.ProveedorId;
                        }
                        var corredor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuitCorredor && (x.SegmentacionId == 5 || x.SegmentacionId == 7));
                        if (!string.IsNullOrEmpty(cuitCorredor) && corredor != null)
                        {
                            //contrato.CorredorId = repositorio.Obtener<Proveedor>(x => x.CUIT == cuitCorredor && (x.SegmentacionId == 5 || x.SegmentacionId == 7)).ProveedorId;
                            contrato.CorredorId = corredor.ProveedorId;
                            contrato.PorcentajeComision = 1;
                        }
                        contrato.ClasificacionId = rows.ElementAt(ii)[10].ToString().Trim().ToLower() == "productor" ? 1 : rows.ElementAt(ii)[10].ToString().Trim().ToLower() == "acopiador" ? 2 : 3;
                        contrato.PlanCanje = rows.ElementAt(ii)[11].ToString().Trim().ToUpper() == "X";
                        contrato.Consignatario = rows.ElementAt(ii)[12].ToString().Trim().ToUpper() == "X";
                        contrato.DestinoId = centros.Centro.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[13].ToString().Trim().ToLower()).Single().Id;
                        contrato.LocalidadId = int.Parse(rows.ElementAt(ii)[14].ToString().Trim());
                        contrato.ProvinciaId = int.Parse(rows.ElementAt(ii)[15].ToString().Trim());
                        contrato.TarifaAConvenir = rows.ElementAt(ii)[16].ToString().Trim().ToUpper() == "X";
                        contrato.Sustentable = rows.ElementAt(ii)[16].ToString().Trim().ToUpper() == "X";
                        contrato.BoletoId = tipoBoletos.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[17].ToString().Trim().ToLower()).Single().Id;
                        if (string.IsNullOrEmpty(rows.ElementAt(ii)[18].ToString()))
                        {
                            //var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == contrato.Cuit && x.SegmentacionId < 5);
                            contrato.BolsaId = proveedor.BolsaCompraNetId;
                        }
                        else
                        {
                            contrato.BolsaId = bolsas.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[18].ToString().Trim().ToLower()).Single().Id;
                        }

                        contrato.Observacion = rows.ElementAt(ii)[19].ToString();
                        contrato.DesdeFijacion = DateTime.Parse(rows.ElementAt(ii)[20].ToString().Trim());
                        contrato.HastaFijacion = DateTime.Parse(rows.ElementAt(ii)[21].ToString().Trim());
                        contrato.CondicionFijacionId = condicionFijacion.Where(a => a.CodigoSap.ToLower() == rows.ElementAt(ii)[22].ToString().Trim().ToLower()).Single().Id;
                        int porcentajeDB = int.Parse(string.IsNullOrEmpty(rows.ElementAt(ii)[23].ToString().Trim()) ? "0" : rows.ElementAt(ii)[23].ToString().Trim());
                        if (porcentajeDB != 0)
                        {
                            if (rows.ElementAt(ii)[24] == null || string.IsNullOrEmpty(rows.ElementAt(ii)[24].ToString()))
                            {
                                ExcelValidatorRowResult excelValidatorRowResult = resultValidation.RowsResult.Where(a => a.Row == ii).Single();
                                var ret = new ExcelValidatorItemResult();
                                ret.Errors.Add("El campo Desc. Y Bonif es obligatorio si Desc. Y Bonif % es distinto de 0.");
                                ret.Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "Desc. Y Bonif" };
                                excelValidatorRowResult.ItemsResult.Add(ret);
                                continue;
                            }
                            contrato.Descuentos = new List<DescuentoBonificacion> { new DescuentoBonificacion {
                                TipoDBId = tipoDB.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[24].ToString().Trim().ToLower()).Single().Id,
                                Porcentaje = int.Parse(rows.ElementAt(ii)[23].ToString().Trim()),
                                TipoPeriodoDBId = 1,
                                MonedaId = "USDM "
                            }};
                        };
                        int descuentos = DBNull.Value.Equals(rows.ElementAt(ii)[24]) ? 0 : tipoDB.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[24].ToString().Trim().ToLower()).Single().Id;
                        if (descuentos != 0)
                        {
                            if (rows.ElementAt(ii)[23] == null || string.IsNullOrEmpty(rows.ElementAt(ii)[23].ToString()))
                            {
                                ExcelValidatorRowResult excelValidatorRowResult = resultValidation.RowsResult.Where(a => a.Row == ii).Single();
                                var ret = new ExcelValidatorItemResult();
                                ret.Errors.Add("El campo Desc. Y Bonif % es obligatorio si Desc. Y Bonif fue completado.");
                                ret.Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "Desc. Y Bonif %" };
                                excelValidatorRowResult.ItemsResult.Add(ret);
                                continue;
                            }
                        }

                        contrato.ComercialId = ComercialId;
                        contrato.Comercial = repositorio.Obtener<Comercial>(ComercialId);
                        contrato.GrupoCompra = contrato.Comercial.GrupoDeComprasId;
                        contrato.UsuarioId = contrato.Comercial.IdActiveDirectory;
                        contrato.ComercialCreadorId = ComercialId;
                        contrato.PorcentajeDePago = 97.5m;

                        CalcularKgMaximoYMinimo(contrato);
                        contrato.Calidad = new List<Calidad>();
                        switch (contrato.MaterialId)
                        {
                            case 1:
                                contrato.StandardDeCalidadId = 2;
                                contrato.Calidad.Add(new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 });
                                break;

                            case 2:
                                contrato.StandardDeCalidadId = 7;
                                contrato.Calidad.Add(new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 });
                                break;

                            case 3:
                                contrato.StandardDeCalidadId = 3;
                                break;

                            case 4:
                                contrato.StandardDeCalidadId = 5;
                                break;

                            case 5:
                                contrato.StandardDeCalidadId = 5;
                                break;

                            default:
                                break;
                        }
                        contrato.ObservacionTercero = ii.ToString().Trim();
                        contrato.AperturaPrecio = new List<AperturaPrecio>();
                        foreach (var item in conceptoList)
                        {
                            contrato.AperturaPrecio.Add(new AperturaPrecio { NegocioId = 0, ConceptoAperturaPrecioId = item.Id, Importe = 0, Porcentaje = 0, MonedaId = "USDM " });
                        }

                        contratos.Add(contrato);
                    }

                    if (!resultValidation.IsValid)
                    {
                        List<ExcelValidatorResumeItem> erroresList = new List<ExcelValidatorResumeItem>();
                        foreach (var item in resultValidation.RowsResult)
                        {
                            List<string> errorsList = new List<string>();
                            ExcelValidatorResumeItem erroresItem = new ExcelValidatorResumeItem();
                            erroresItem.ContratoCorredor = item.ContratoCorredor;
                            erroresItem.Row = item.Row;

                            foreach (var item2 in item.ItemsResult)
                            {
                                errorsList.AddRange(item2.Errors);
                            }
                            erroresItem.Errors = errorsList;

                            erroresList.Add(erroresItem);
                        }
                        return erroresList;
                        //return Json(new { Resume = erroresList, Resultado = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        foreach (var item in contratos)
                        {
                            var resultado = GrabarContrato(item);

                            logger.Debug($"Se creo el contrato: {item.Id} desde el alta masiva de convenios.");

                            if (resultado.HayError)
                            {
                                //item.ContratoId estoy usando ese campo para devolver el numero de row
                                resultValidation.RowsResult[Convert.ToInt32(item.ObservacionTercero)].ItemsResult.Add(new ExcelValidatorItemResult { Errors = resultado.Errores.Select(a => a.Message).ToList(), Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "", Options = null, Position = 1, Required = true, Type = ExcelValidationColumnType.String } });
                                //errores.AddRange(item.Errores.Select(a => a.Message).ToList());
                            }
                            else
                            {
                                logger.Debug($"Se esta creando el contrato {item.Id} con el alta masiva de convenios");
                            }
                        }
                        return resultValidation.Resume.OrderBy(x => x.HasError).ToList();
                        //return Json(new { Resume = resultValidation.Resume.OrderBy(x => x.HasError).ToList(), Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void CalcularKgMaximoYMinimo(Contrato contrato)
        {
            var cantidad = contrato.Cantidad;
            double cantidadMinima = 0;
            double cantidadMaxima = 0;
            if (cantidad > 0)
            {
                cantidadMaxima = 30 * cantidad / 100;
                cantidadMinima = 30000;
                if (cantidad < 30000)
                {
                    cantidadMaxima = cantidad;
                    cantidadMinima = cantidad;
                }
                else if (cantidadMaxima < 30000)
                {
                    cantidadMaxima = 30000;
                }
                //setear
                contrato.KgMinimo = (int)cantidadMinima;
                contrato.KgMaximo = (int)cantidadMaxima;
            }
            else
            {
                contrato.KgMinimo = (int)cantidadMinima;
                contrato.KgMaximo = (int)cantidadMaxima;
            }
        }

        private void ValidacionContratoFatal(List<BasicoContrato> contratos, BasicoContrato acuerdo, ExcelValidatorResult resultValidation)
        {
            int i = 0;
            foreach (var item in contratos)
            {
                List<ExcelValidatorItemResult> excelValidatorItemResults = new List<ExcelValidatorItemResult>();
                string indiceContrato = "Contrato corredor: " + item.ContratoCorredor + ". ";
                if (item.MaterialId != acuerdo.MaterialId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Grano", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El material no coincide con el del acuerdo seleccionado. " } });
                }
                if (item.CampanaId != acuerdo.CampanaId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Cosecha", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La cosecha no coincide con la del acuerdo seleccionado. " } });
                }
                if (item.FechaOperacion.Value.Date != acuerdo.FechaOperacion.Value.Date)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Operación", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha de Operación no coincide con la del acuerdo seleccionado. " } });
                }
                if (acuerdo.FechaDesde.HasValue)
                {
                    if (item.FechaDesde.Value.Date != acuerdo.FechaDesde.Value.Date)
                    {
                        excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha DesdeEntrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha Desde Entrega no coincide con la del acuerdo seleccionado. " } });
                    }
                }
                else
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El acuerdo no cuenta con Fecha Desde Entrega. " } });
                }
                if (acuerdo.FechaHasta.HasValue)
                {
                    if (item.FechaEntrega.Value.Date != acuerdo.FechaHasta.Value.Date)
                    {
                        excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "La Fecha Vto. Entrega no coincide con la del acuerdo seleccionado. " } });
                    }
                }
                else
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Fecha Vto. Entrega", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El acuerdo no cuenta con Fecha Vto. Entrega. " } });
                }
                if (item.DestinoId != acuerdo.DestinoId)
                {
                    excelValidatorItemResults.Add(new ExcelValidatorItemResult { Item = new ExcelValidatorItem { Name = "Destino", ErrorType = ExcelValidationErrorType.Fatal }, Errors = new List<string> { indiceContrato + "El Destino no coincide con el del acuerdo seleccionado. " } });
                }
                if (excelValidatorItemResults.Count > 0)
                {
                    resultValidation.RowsResult[i].ItemsResult.AddRange(excelValidatorItemResults);
                }
                //resultValidation.RowsResult[i].ContratoCorredor = item.ContratoCorredor;
                i++;
            }
        }

        private List<ExcelValidatorItem> GetValidatorContratos(List<MaterialIni> materiales, List<CentroIni> centros)
        {
            var ret = new List<ExcelValidatorItem>();
            var pos = 0;

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Grano",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = materiales.Where(a => a.MaterialId < 5).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Cosecha",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Operación",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha DesdeEntrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Vto.Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "TN",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Clasificacion",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "acopiador", "productor", "otros" },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Plan Canje",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Consignatario",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Destino",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = centros.Where(a => a.Id != 10).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROCEDENCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROVINCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            //configurar el resto de campos

            return ret;
        }

        private List<ExcelValidatorItem> GetValidatorConvenios(List<MaterialIni> materiales, List<CentroIni> centros)
        {
            var ret = new List<ExcelValidatorItem>();
            var pos = 0;

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Grano",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = materiales.Where(a => a.MaterialId < 5).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Cosecha",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.String
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Operación",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha DesdeEntrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Vto.Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "TN",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Clasificacion",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "acopiador", "productor", "otros" },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Plan Canje",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Consignatario",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Destino",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = centros.Where(a => a.Id != 10).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROCEDENCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROVINCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Sust a conv.",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Tipo Boleto",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = repositorio.Listar<BoletoCompraNet>().Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Bolsa",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = repositorio.Listar<BolsaCompraNet>().Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Observaciones",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.String
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fix desde",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fix hasta",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Condic. Fix",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = repositorio.Listar<CondicionFijacion>().Select(a => a.CodigoSap.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Desc. Y Bonif %",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Desc. Y Bonif",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Options = repositorio.Listar<TipoDB>().Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });
            //configurar el resto de campos

            return ret;
        }

        private List<ExcelValidatorItem> GetValidatorMATBA(List<MaterialIni> materiales, List<CentroIni> centros, List<Comercial> comerciales)
        {
            var ret = new List<ExcelValidatorItem>();
            var pos = 0;
            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "CUIT Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Agente de Compras",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "MATBA ROFEX SA".ToLower() },
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Tipo Negocio",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "A FIJAR".ToLower(), "A PRECIO".ToLower() },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Grano",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = materiales.Where(a => a.MaterialId < 5).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "KG",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Decimal
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Cosecha",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.String
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Precio",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Decimal
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Moneda",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Options = new List<string>() { "USD".ToLower(), "ARP".ToLower() },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Clasificacion",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "PRODUCTOR".ToLower(), "ACOPIADOR".ToLower(), "OTROS".ToLower() },
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Corredor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Contrato Vendedor",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Desde Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Fecha Vto.Entrega",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Plan Canje",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Consignatario",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Destino",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = centros.Where(a => a.Id != 10).Select(a => a.Descripcion.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROCEDENCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "PROVINCIA",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Int
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Caratula MAT",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.String
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Precio Ajuste Comision",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Moneda MAT",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = true,
                Options = new List<string>() { "USD".ToLower(), "ARP".ToLower() },
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Comercial",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Options = comerciales.Select(x => x.Nombres + " " + x.Apellido.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Sustentable",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Importe Sustentable",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Moneda Sustentable",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Options = new List<string>() { "USD".ToLower(), "ARP".ToLower() },
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Sust a conv.",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Bool
            });

            //ret.Add(new ExcelValidatorItem()
            //{
            //    Name = "Tipo Boleto",
            //    ErrorType = ExcelValidationErrorType.Fatal,
            //    Position = pos++,
            //    Required = true,
            //    Options = repositorio.Listar<BoletoCompraNet>().Select(a => a.Descripcion.ToLower()).ToList(),
            //    Type = ExcelValidationColumnType.List
            //});

            //ret.Add(new ExcelValidatorItem()
            //{
            //    Name = "Bolsa",
            //    ErrorType = ExcelValidationErrorType.Fatal,
            //    Position = pos++,
            //    Required = false,
            //    Options = repositorio.Listar<BolsaCompraNet>().Select(a => a.Descripcion.ToLower()).ToList(),
            //    Type = ExcelValidationColumnType.List
            //});
            ret.Add(new ExcelValidatorItem()
            {
                Name = "FIX desde",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "FIX hasta",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Date
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Condic. Fix",
                ErrorType = ExcelValidationErrorType.Fatal,
                Position = pos++,
                Required = false,
                Options = repositorio.Listar<CondicionFijacion>().Select(a => a.CodigoSap.ToLower()).ToList(),
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Calidad",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Options = new List<string>() { "camara", "fabrica", "grado", "grado 2" },
                Type = ExcelValidationColumnType.List
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Valor de calidad",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Long
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Redespacho",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.Decimal
            });

            ret.Add(new ExcelValidatorItem()
            {
                Name = "Observaciones",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = false,
                Type = ExcelValidationColumnType.String
            });
            ret.Add(new ExcelValidatorItem()
            {
                Name = "Porcentaje de Pago",
                ErrorType = ExcelValidationErrorType.Error,
                Position = pos++,
                Required = true,
                Type = ExcelValidationColumnType.Long
            });

            //configurar el resto de campos
            return ret;
        }

        public void ActualizarEstadoDeContratos()
        {
            var contratos = repositorio.Listar<Contrato>(x => x.ConfirmadoSAP != true && !string.IsNullOrEmpty(x.ContratoSAP)).ToList();
            if (contratos != null && contratos.Count > 0)
            {
                logger.Debug("Cambiar estado de contratos: Count " + contratos.Count() + " " + contratos.Select(x => x.ContratoSAP).ToJson());
                var estados = status.ValidarEstados(contratos.Select(x => x.ContratoSAP).ToList());
                foreach (var contrato in contratos)
                {
                    var res = estados.FirstOrDefault(a => a.ContratoSap == contrato.ContratoSAP);
                    if (res != null)
                        contrato.ConfirmadoSAP = !string.IsNullOrEmpty(res.Status);
                }
                repositorio.GuardarCambios();
            }
        }

        public List<ServicioValorDto> TraerTodoServicio(int? materialId, int? centroId)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerServicios(materialId, centroId)).ToList();
        }

        private void GrabarServicioModificado(List<Servicio> servicios, int materialid, int centroId)
        {
            logger.Debug($"Servicios - Material {materialid} - Centro {centroId}");
            var serviciosMaestro = TraerTodoServicio(materialid, centroId);
            if (servicios != null && servicios.Count > 0)
            {
                var serviciosContrato = servicios.Select(s => new
                {
                    Id = s.Id,
                    ServicioValor = s.ServicioValor,
                    Importe = s.Importe,
                    MonedaId = s.MonedaId,
                    NegocioId = s.NegocioId,
                    Desde = s.Desde,
                    Hasta = s.Hasta
                }).ToList();
                logger.Debug("Servicios en INI-GrabarServicioModificado(): " + (serviciosContrato.Count() > 15 ? serviciosContrato.ToJson() : (serviciosContrato.Count() + " servicios")));

                foreach (var s in servicios)
                {
                    if (serviciosMaestro.Any(x => x.ServicioValorId == s.ServicioValorId) && serviciosMaestro.Where(x => x.ServicioValorId == s.ServicioValorId).FirstOrDefault().Importe != s.Importe)
                    {
                        s.Modificado = true;
                    }
                    else
                    {
                        s.Modificado = false;
                    }
                }

                var serviciosContrato2 = servicios.Select(s => new
                {
                    Id = s.Id,
                    ServicioValor = s.ServicioValor,
                    Importe = s.Importe,
                    MonedaId = s.MonedaId,
                    NegocioId = s.NegocioId,
                    Desde = s.Desde,
                    Hasta = s.Hasta
                }).ToList();
                logger.Debug("Servicios en FIN-GrabarServicioModificado(): " + (serviciosContrato2.Count() > 15 ? serviciosContrato2.ToJson() : (serviciosContrato2.Count() + " servicios")));
            }
        }

        private bool ValidarServicioModificado(Contrato contratoSave, Contrato contrato)
        {
            var modificado = false;
            if (contratoSave.Servicios != null && contratoSave.Servicios.Count > 0)
            {
                foreach (var servicioSave in contratoSave.Servicios)
                {
                    var importe = contrato.Servicios != null && contrato.Servicios.Count > 0 ? contrato.Servicios.Where(x => x.ServicioValorId == servicioSave.ServicioValorId).FirstOrDefault()?.Importe : 0;
                    if (servicioSave.Importe != importe)
                    {
                        modificado = true;
                    }
                }
            }
            return modificado;
        }

        private bool ValidarFechaAgenteMP(Contrato contrato)
        {
            var fechaElegida = contrato.FechaOperacion;
            var diasHabiles = oDiasHabilesAgent.ObtenerDiasHabilesDelMes(fechaElegida);
            bool fechaInvalida = false;
            int cont = 0;
            for (int i = diasHabiles.Count - 1; cont < 5 && !fechaInvalida; i--)
            {
                if (fechaElegida == diasHabiles[i])
                    fechaInvalida = true;
                cont++;
            }

            return fechaInvalida;
        }

        public void ReenviarMailContrato(int contratoId, string idActiveDirectory)
        {
            Contrato contrato = repositorio.Obtener<Contrato>(contratoId);
            var descuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == contratoId);
            var calidad = repositorio.Listar<Calidad>(x => x.NegocioId == contratoId);
            EnviarMailFinalizado(idActiveDirectory, contrato, descuento, calidad);
        }

        public List<ExcelValidatorResumeItem> AltaMasivaMATBA(DataSet dsExcel, int ComercialId)
        {
            List<string> errores = new List<string>();
            try
            {
                if (dsExcel.Tables.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].Rows.Count == 0)
                {
                    throw new Exception("El archivo no contiene información.");
                }
                if (dsExcel.Tables[0].TableName != "AltaMasiva")
                {
                    if (dsExcel.Tables[0].TableName == "Data")
                    {
                        throw new Exception("El archivo no contiene información de contratos.");
                    }
                    else
                    {
                        throw new Exception("El archivo no tiene el formato correcto. Utilice el Archivo Modelo.");
                    }
                }
                var materiales = mobjMaterialManager.TraerTodoMaterial();
                var centros = centroManager.TraerTodoCentro();
                var comerciales = repositorio.Listar<Comercial>(x => x.AsignarNegocios).ToList();
                var validations = GetValidatorMATBA(materiales.Material, centros.Centro, comerciales);
                var validator = new ExcelValidator(validations);

                var resultValidation = validator.Validate(dsExcel.Tables[0], false);

                if (!resultValidation.IsValid)
                {
                    return resultValidation.Resume;
                    //return Json(new { Resume = resultValidation.Resume, Resultado = !resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var conceptoList = repositorio.Listar<ConceptoAperturaPrecio>();
                    var campanias = mobjCampaniaManager.TraerTodoCampania();
                    var tipoBoletos = repositorio.Listar<BoletoCompraNet>();
                    var bolsas = repositorio.Listar<BolsaCompraNet>();
                    var tipoDB = repositorio.Listar<TipoDB>();
                    var condicionFijacion = repositorio.Listar<CondicionFijacion>();
                    var tiponegocio = mobjTipoNegocioManager.TraerTodoTipoNegocio();
                    var moneda = repositorio.Listar<Moneda>();

                    List<Contrato> contratos = new List<Contrato>();
                    List<int> rowsOk = resultValidation.RowsResult.Where(a => !a.HasError).Select(a => a.Row).ToList();
                    if (rowsOk.Count == 0)
                    {
                        return resultValidation.Resume;
                    }
                    var rows = dsExcel.Tables[0].AsEnumerable().Select(x => x.ItemArray).Skip(0);
                    for (int ii = 0; ii < rows.Count(); ii++)
                    {
                        if (!rowsOk.Contains(ii))
                            continue;
                        var contrato = new Contrato();
                        var cuitProveedor = rows.ElementAt(ii)[0].ToString().Trim();
                        var cuitCorredor = rows.ElementAt(ii)[1].ToString().Trim();
                        var proveedor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuitProveedor && x.SegmentacionId < 5);
                        if (proveedor != null)
                        {
                            contrato.ProveedorId = proveedor.ProveedorId;
                        }
                        else
                        {
                            ExcelValidatorRowResult excelValidatorRowResult = resultValidation.RowsResult.Where(a => a.Row == ii).Single();
                            var ret = new ExcelValidatorItemResult();
                            ret.Errors.Add("No se encontró un proveedor con ese CUIT.");
                            ret.Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "CUIT Proveedor" };
                            excelValidatorRowResult.ItemsResult.Add(ret);
                            continue;
                        }
                        if (!string.IsNullOrEmpty(cuitCorredor))
                        {
                            var corredor = repositorio.Obtener<Proveedor>(x => x.CUIT == cuitCorredor && (x.SegmentacionId == 5 || x.SegmentacionId == 7));
                            if (corredor != null)
                            {
                                contrato.CorredorId = corredor.ProveedorId;
                                contrato.PorcentajeComision = 1;
                            }
                            else
                            {
                                ExcelValidatorRowResult excelValidatorRowResult = resultValidation.RowsResult.Where(a => a.Row == ii).Single();
                                var ret = new ExcelValidatorItemResult();
                                ret.Errors.Add("No se encontró un corredor con ese CUIT.");
                                ret.Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "CUIT Proveedor" };
                                excelValidatorRowResult.ItemsResult.Add(ret);
                                continue;
                            }
                        }
                        contrato.TipoAgenteCompraId = 1;
                        contrato.TipoNegocioId = tiponegocio.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[3].ToString().Trim().ToLower()).Single().TipoNegocioId;
                        contrato.MaterialId = materiales.Material.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[4].ToString().Trim().ToLower()).Single().MaterialId;
                        var calidades = mobjCampaniaManager.TraerCalidadPorMaterial(contrato.MaterialId);
                        contrato.Cantidad = int.Parse(rows.ElementAt(ii)[5].ToString().Trim());
                        if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_PRECIO)
                        {
                            contrato.Precio = decimal.Parse(rows.ElementAt(ii)[7].ToString().Trim());
                            if (contrato.Precio > 0)
                            {
                                contrato.PrecioNeto = contrato.Precio;
                            }
                            contrato.MonedaId = moneda.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[8].ToString().Trim().ToLower()).Single().MonedaId;
                        }
                        contrato.CampanaId = campanias.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[6].ToString().Trim().ToLower()).Single().CampañaId;
                        contrato.ClasificacionId = rows.ElementAt(ii)[9].ToString().Trim().ToLower() == "productor" ? 1 : rows.ElementAt(ii)[8].ToString().Trim().ToLower() == "acopiador" ? 2 : 3;
                        contrato.ContratoVendedor = rows.ElementAt(ii)[10].ToString().Trim();
                        if (!string.IsNullOrEmpty(cuitCorredor))
                        {
                            contrato.ContratoCorredor = string.IsNullOrWhiteSpace(rows.ElementAt(ii)[11].ToString().Trim()) ? rows.ElementAt(ii)[10].ToString().Trim() : rows.ElementAt(ii)[11].ToString().Trim();
                        }
                        //contrato.Fecha = DateTime.Parse(rows.ElementAt(ii)[4].ToString().Trim());
                        contrato.FechaOperacion = diasHabilesAgent.UltimoDiaHabil(null);
                        contrato.MotivoOperacionAnterior = "Otro";
                        contrato.DescripcionOperacionAnterior = "ES MATBA";
                        contrato.FechaDesde = DateTime.Parse(rows.ElementAt(ii)[12].ToString().Trim());
                        contrato.FechaHasta = DateTime.Parse(rows.ElementAt(ii)[13].ToString().Trim());
                        contrato.FechaEntrega = DateTime.Parse(rows.ElementAt(ii)[13].ToString().Trim());
                        //var cuitAgente = rows.ElementAt(ii)[8].ToString().Trim();
                        contrato.PlanCanje = rows.ElementAt(ii)[14].ToString().Trim().ToUpper() == "X";
                        contrato.Consignatario = rows.ElementAt(ii)[15].ToString().Trim().ToUpper() == "X";
                        contrato.DestinoId = centros.Centro.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[16].ToString().Trim().ToLower()).Single().Id;
                        contrato.LocalidadId = int.Parse(rows.ElementAt(ii)[17].ToString().Trim());
                        contrato.ProvinciaId = int.Parse(rows.ElementAt(ii)[18].ToString().Trim());
                        contrato.CaratulaMAT = rows.ElementAt(ii)[19].ToString().Trim();
                        var precioajuste = decimal.Parse(rows.ElementAt(ii)[20].ToString().Trim());
                        contrato.PrecioAjusteComision = precioajuste <= 0 ? contrato.Precio : precioajuste;
                        contrato.MonedaAjusteComisionId = moneda.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[21].ToString().Trim().ToLower()).Single().MonedaId;
                        contrato.ComercialId = comerciales.Where(a => (a.Nombres.ToLower() + " " + a.Apellido.ToLower()) == rows.ElementAt(ii)[22].ToString().Trim().ToLower()).Single().ComercialId;
                        contrato.Sustentable = rows.ElementAt(ii)[23].ToString().Trim().ToUpper() == "X";
                        if (contrato.Sustentable)
                        {
                            decimal importeSustentable = 0;
                            decimal.TryParse(rows.ElementAt(ii)[24].ToString().Trim(), out importeSustentable);
                            contrato.ImporteSustentable = importeSustentable == 0 ? (decimal?)null : importeSustentable;
                            contrato.MonedaSustentableId = moneda.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[25].ToString().Trim().ToLower()).SingleOrDefault()?.MonedaId;
                            contrato.TarifaAConvenir = rows.ElementAt(ii)[26].ToString().Trim().ToUpper() == "X";
                        }

                        contrato.BoletoId = (int)EnumBoletoCompraNet.NINGUNO;

                        if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                        {
                            contrato.DesdeFijacion = DateTime.Parse(rows.ElementAt(ii)[27].ToString().Trim());
                            contrato.HastaFijacion = DateTime.Parse(rows.ElementAt(ii)[28].ToString().Trim());
                            if (!string.IsNullOrEmpty(rows.ElementAt(ii)[30].ToString()))
                            {
                                contrato.CondicionFijacionId = condicionFijacion.Where(a => a.CodigoSap.ToLower() == rows.ElementAt(ii)[29].ToString().Trim().ToLower()).Single().Id;
                            }
                        }
                        contrato.GrupoCompra = comerciales.Where(a => (a.Nombres.ToLower() + " " + a.Apellido.ToLower()) == rows.ElementAt(ii)[22].ToString().Trim().ToLower()).Single().GrupoDeComprasId;
                        contrato.UsuarioId = comerciales.Where(x => x.ComercialId == contrato.ComercialId).FirstOrDefault().IdActiveDirectory;
                        contrato.ComercialCreadorId = ComercialId;

                        if (contrato.TipoNegocioId == (int)EnumTipoNegocio.A_FIJAR)
                        {
                            CalcularKgMaximoYMinimo(contrato);
                        }
                        contrato.Calidad = new List<Calidad>();

                        if (contrato.MaterialId != 0)
                        {
                            if (!string.IsNullOrEmpty(rows.ElementAt(ii)[30].ToString()))
                            {
                                var calidadListado = calidades.Where(a => a.Descripcion.ToLower() == rows.ElementAt(ii)[30].ToString().Trim().ToLower()).ToList();
                                if (calidadListado != null && calidadListado.Count > 0)
                                {
                                    var material = contrato.MaterialId;
                                    switch (rows.ElementAt(ii)[30].ToString())
                                    {
                                        case "Camara":
                                            contrato.StandardDeCalidadId = (material == 1 || material == 2) ? 1 : material == 3 ? 4 : 5;
                                            break;

                                        case "Fabrica":
                                            contrato.StandardDeCalidadId = 3;
                                            break;

                                        case "Grado":
                                            contrato.StandardDeCalidadId = material == 1 ? 2 : 7;
                                            contrato.Calidad.Add(
                                                new Calidad
                                                {
                                                    StandardDeCalidadId = material == 1 ? 2 : 7,
                                                    CalidadEspecialId = material == 1 ? 4 : 5,
                                                    Valor = material == 1 ? (string.IsNullOrEmpty(rows.ElementAt(ii)[31].ToString()) ? 2 : decimal.Parse(rows.ElementAt(ii)[31].ToString())) : 2
                                                });
                                            break;

                                        case "Grado 2":
                                            contrato.StandardDeCalidadId = material == 1 ? 2 : 7;
                                            contrato.Calidad.Add(
                                                new Calidad
                                                {
                                                    StandardDeCalidadId = material == 1 ? 2 : 7,
                                                    CalidadEspecialId = material == 1 ? 4 : 5,
                                                    Valor = material == 1 ? (string.IsNullOrEmpty(rows.ElementAt(ii)[31].ToString()) ? 2 : decimal.Parse(rows.ElementAt(ii)[31].ToString())) : 2
                                                });
                                            break;

                                        default:
                                            break;
                                    }
                                }
                            }
                            else
                            {
                                switch (contrato.MaterialId)
                                {
                                    case 1:
                                        contrato.StandardDeCalidadId = 2;
                                        contrato.Calidad.Add(new Calidad { StandardDeCalidadId = 2, CalidadEspecialId = 4, Valor = 2 });
                                        break;

                                    case 2:
                                        contrato.StandardDeCalidadId = 7;
                                        contrato.Calidad.Add(new Calidad { StandardDeCalidadId = 7, CalidadEspecialId = 5, Valor = 2 });
                                        break;

                                    case 3:
                                        contrato.StandardDeCalidadId = 3;
                                        break;

                                    case 4:
                                        contrato.StandardDeCalidadId = 5;
                                        break;

                                    case 5:
                                        contrato.StandardDeCalidadId = 5;
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }

                        contrato.ObservacionTercero = ii.ToString().Trim();
                        contrato.AperturaPrecio = new List<AperturaPrecio>();
                        var redespacho = string.IsNullOrEmpty(rows.ElementAt(ii)[32].ToString()) ? 0 : decimal.Parse(rows.ElementAt(ii)[32].ToString().Trim());
                        if (redespacho != 0)
                        {
                            contrato.AperturaPrecio.Add(new AperturaPrecio { NegocioId = 0, ConceptoAperturaPrecioId = 2, Importe = redespacho, Porcentaje = 0, MonedaId = "USDM " });
                            contrato.PrecioNeto += redespacho;
                        }
                        else
                        {
                            foreach (var item in conceptoList)
                            {
                                contrato.AperturaPrecio.Add(new AperturaPrecio { NegocioId = 0, ConceptoAperturaPrecioId = item.Id, Importe = 0, Porcentaje = 0, MonedaId = "USDM " });
                            }
                        }

                        contrato.Observacion = rows.ElementAt(ii)[33].ToString();
                        contrato.PorcentajeDePago = decimal.Parse(rows.ElementAt(ii)[34].ToString());

                        contratos.Add(contrato);
                    }

                    if (!resultValidation.IsValid)
                    {
                        List<ExcelValidatorResumeItem> erroresList = new List<ExcelValidatorResumeItem>();
                        foreach (var item in resultValidation.RowsResult)
                        {
                            List<string> errorsList = new List<string>();
                            ExcelValidatorResumeItem erroresItem = new ExcelValidatorResumeItem();
                            erroresItem.ContratoCorredor = item.ContratoCorredor;
                            erroresItem.Row = item.Row;

                            foreach (var item2 in item.ItemsResult)
                            {
                                errorsList.AddRange(item2.Errors);
                            }
                            erroresItem.Errors = errorsList;

                            erroresList.Add(erroresItem);
                        }
                        return erroresList;
                        //return Json(new { Resume = erroresList, Resultado = true }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        foreach (var item in contratos)
                        {
                            var resultado = GrabarContrato(item);
                            if (resultado.HayError)
                            {
                                //item.ContratoId estoy usando ese campo para devolver el numero de row
                                resultValidation.RowsResult[Convert.ToInt32(item.ObservacionTercero)].ItemsResult.Add(new ExcelValidatorItemResult { Errors = resultado.Errores.Select(a => a.Message).ToList(), Item = new ExcelValidatorItem { ErrorType = ExcelValidationErrorType.Error, Name = "", Options = null, Position = 1, Required = true, Type = ExcelValidationColumnType.String } });
                                //errores.AddRange(item.Errores.Select(a => a.Message).ToList());
                            }
                            else
                            {
                                logger.Debug($"Se esta creando el contrato con ID {item.Id} con el alta masiva de MATBA");
                            }
                        }
                        return resultValidation.Resume.OrderBy(x => x.HasError).ToList();
                        //return Json(new { Resume = resultValidation.Resume.OrderBy(x => x.HasError).ToList(), Resultado = resultValidation.IsValid }, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }



        public Resultado ValidarPantallaEnUso(PantallaEnUsoDto pantallaEnUso)
        {
            var oEntityErrors = new Resultado();
            int minutosCronometroConDescarga = repositorio.Obtener<Configuracion>(1).MinutosCronometroConDescarga;

            PantallaEnUso pantalla = repositorio.Listar<PantallaEnUso>(x => x.NombrePantalla == pantallaEnUso.NombrePantalla)
            .OrderByDescending(x => x.FechaHoraInicioUso)
            .FirstOrDefault();
            if (pantalla is null)
            {
                // PANTALLA SE ABRE POR 1RA VEZ.
                UsarPantalla(pantallaEnUso);
            }
            else
            {
                var fechaLimiteDeUso = pantalla.FechaHoraInicioUso.AddMinutes(minutosCronometroConDescarga);
                //   termino de usarla o paso el tiempo de uso
                if (pantalla.FechaHoraFinUso != null || fechaLimiteDeUso < DateTime.Now)
                {
                    UsarPantalla(pantallaEnUso);
                }
                else
                {
                    var ComercialId = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == pantallaEnUso.UsuarioId).ComercialId;
                    if (pantalla.ComercialId != ComercialId)
                    {
                        var comercial = repositorio.Obtener<Comercial>(x => x.ComercialId == pantalla.ComercialId);
                        oEntityErrors.Error("PantallaExiste", $"Pantalla en uso por: {comercial.Apellido} {comercial.Nombres} hasta las {fechaLimiteDeUso.ToString("HH:mm:ss")}.");
                        return oEntityErrors;
                    }
                }
                //if (pantalla.FechaHoraFinUso == null && pantalla.UsuarioId != pantallaEnUso.UsuarioId)
                //{
                //    oEntityErrors.Error("PantallaExiste", "Pantalla en uso por: " + pantalla.UsuarioId);
                //    return oEntityErrors;
                //}
                //else if (pantalla.FechaHoraFinUso != null)
                //{
                //    UsarPantalla(pantallaEnUso);
                //}
            }
            return oEntityErrors;
        }

        public void UsarPantalla(PantallaEnUsoDto pantallaEnUso)
        {
            logger.Debug("Alta Pantalla en uso en BD DataAgro: " + pantallaEnUso);
            var ComercialId = repositorio.Obtener<Comercial>(x => x.IdActiveDirectory == pantallaEnUso.UsuarioId).ComercialId;
            PantallaEnUso p = new PantallaEnUso();
            try
            {
                p.NombrePantalla = pantallaEnUso.NombrePantalla;
                p.UsuarioId = pantallaEnUso.UsuarioId;
                p.FechaHoraInicioUso = DateTime.Now;
                p.FechaHoraFinUso = null;
                p.ComercialId = ComercialId;

                repositorio.Agregar(p);
                repositorio.GuardarCambios();
            }
            catch (Exception e)
            {
                logger.Error("Error Alta Pantalla en uso.");
                logger.Error(e);
            }
        }

        public Resultado LiberarPantalla(PantallaEnUsoDto pantallaEnUso)
        {
            logger.Debug("Liberar pantalla en uso en BD DataAgro: " + pantallaEnUso);
            var oEntityErrors = new Resultado();

            try
            {
                PantallaEnUso pantalla = repositorio.Listar<PantallaEnUso>(x => x.NombrePantalla == pantallaEnUso.NombrePantalla && x.FechaHoraFinUso == null)
                    .OrderByDescending(x => x.FechaHoraInicioUso)
                    .FirstOrDefault();

                if (pantalla != null)
                {
                    pantalla.FechaHoraFinUso = DateTime.Now;
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception e)
            {
                logger.Error("Error Alta Pantalla en uso.");
                logger.Error(e);
            }

            return oEntityErrors;
        }

        public List<ConfiguracionCupoDto> CantidadDiasCuposConDescarga(string fechaDesdeNegocio, string fechaHastaNegocio, int materialId, int centroId, int comercialId)
        {
            DateTime fechaDesde = DateTime.ParseExact(fechaDesdeNegocio ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            DateTime fechaHasta = DateTime.ParseExact(fechaHastaNegocio ?? DateTime.Now.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);

            string grupoDeCompras = repositorio.Listar<Comercial>(x => x.ComercialId == comercialId).First().GrupoDeCompras.Descripcion;
            ZonaCupo zonaComercial = repositorio.Listar<ZonaCupo>(x => x.Descripcion == grupoDeCompras).FirstOrDefault();

            var diasParametro = repositorio.Obtener<Configuracion>(1).CantidadMaximaDiasNegocioConDescarga;
            var hoy = DateTime.Today;
            var fechaLimite = hoy.AddDays(diasParametro);

            List<ConfiguracionCupo> configuracionCupo = repositorio.Listar<ConfiguracionCupo>(x => x.CentroId == centroId &&
                                                                                                   x.MaterialId == materialId &&
                                                                                                   x.Fecha >= hoy &&
                                                                                                   x.Fecha <= fechaLimite &&
                                                                                                   x.Fecha <= fechaHasta &&
                                                                                                   x.Fecha >= fechaDesde);

            List<ConfiguracionCupoDto> configCupo = new List<ConfiguracionCupoDto>();
            configuracionCupo.ForEach(x =>
            {
                // tener en cuenta la zona del comercial asignado al negocio
                int limiteDescargaZona = x.CantidadCupo.Where(z => z.ZonaCupoId == zonaComercial.Id).Select<LimiteCupo, int>(z => z.CantidadCupoConDescarga).SingleOrDefault();

                // No tiene en cuenta anulados ni rechazados.
                int cantidadCuposConsumidos = repositorio.Contar<Cupo>(y => y.ConDescarga == true &&
                                                                            y.NegocioId != null &&
                                                                            y.FechaIngreso == x.Fecha &&
                                                                            y.MaterialId == materialId &&
                                                                            y.CentroId == centroId &&
                                                                            y.EstadoCupoId != 4 &&
                                                                            y.EstadoCupoId != 9 &&
                                                                            y.ZonaCupoId == zonaComercial.Id);

                var disponibilidadCuposConDescarga = limiteDescargaZona - cantidadCuposConsumidos;

                // Verificar si hay disponibilidad general.
                var cuposCreados = repositorio.Contar<Cupo>(y => y.FechaIngreso == x.Fecha &&
                                                                 y.MaterialId == materialId &&
                                                                 y.CentroId == centroId &&
                                                                 y.EstadoCupoId != 9 &&
                                                                 y.EstadoCupoId != 4);

                var limiteCupo = repositorio.Obtener<ConfiguracionCupo, int>(y => y.MaterialId == materialId &&
                                                                                  y.Fecha == x.Fecha &&
                                                                                  y.CentroId == centroId, y => y.LimiteCupo);

                var disponibilidadGeneralCupos = limiteCupo - cuposCreados;

                ConfiguracionCupoDto cc = new ConfiguracionCupoDto();

                cc.CentroId = x.CentroId;
                cc.Fecha = x.Fecha;
                cc.LimiteDescarga = x.LimiteDescarga;
                cc.MaterialId = x.MaterialId;
                cc.CuposConsumidos = cantidadCuposConsumidos;

                if (disponibilidadGeneralCupos <= 0)
                {
                    cc.CuposDisponibles = 0;
                }
                else if (disponibilidadCuposConDescarga <= disponibilidadGeneralCupos)
                {
                    cc.CuposDisponibles = disponibilidadCuposConDescarga;
                }
                else
                {
                    cc.CuposDisponibles = disponibilidadGeneralCupos;
                }

                cc.LimiteDescarga = limiteDescargaZona;

                configCupo.Add(cc);
            });

            return configCupo;
        }

        public List<CupoDto> TraerCuposConDescarga(int contratoId)
        {
            return repositorio.Listar<Cupo, CupoDto>(x => new CupoDto
            {
                NegocioId = x.NegocioId,
                FechaIngreso = x.FechaIngreso,
                CupoSap = x.CupoSap,
                FleteProcedencia = x.FleteProcedencia,
                Centro = x.Centro.Descripcion
            }, x => x.NegocioId == contratoId && x.ConDescarga.HasValue && x.ConDescarga.Value);
        }

        public string ObtenerTypeOfRate(int tipoNegocioId, string monedaId, int? tipoAgenteCompraId, DateTime fecha, bool modifica)
        {
            string codigoTC = oFinalizarContratoAgent.DevolverTipoCambioSAP(tipoNegocioId, monedaId, tipoAgenteCompraId, fecha, modifica);
            string typeOfRate = codigoTC == "04" ? "Z" : "M";

            return typeOfRate;
        }

        public string ConsultarRangoPrecio(int materialId, string moneda, decimal precio)
        {
            var rangosPrecio = repositorio.Obtener<RangoPrecio>(x => x.MaterialId == materialId && x.MonedaId == moneda);

            if (rangosPrecio != null && (precio < rangosPrecio.PrecioMinimo || precio > rangosPrecio.PrecioMaximo))
            {
                return $"Precio fuera de Rango - Precio Mínimo: {rangosPrecio.PrecioMinimo:N2} y Precio Máximo: {rangosPrecio.PrecioMaximo:N2} para {rangosPrecio.Material.Descripcion} en {rangosPrecio.Moneda.Descripcion}";
            }
            else
                return string.Empty;
        }
    }
}