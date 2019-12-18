using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
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
            IDiasHabilesAgent diasHabilesAgent)
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
            this.capacidadProductiva = capacidadProductiva;
        }

        public DatosIniContrato TraerDatosCombo()
        {
            var datosCombo = new DatosIniContrato();

            datosCombo.prov = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry() { Provinciaid = x.ProvinciaId, Nombre = x.Nombre, Orden = x.Orden }, null, 0, "Orden");

            datosCombo.loc = new List<LocalidadQry>();

            datosCombo.campaña = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion });

            datosCombo.material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion });

            datosCombo.moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido },
                (x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso == PermisosDataAgro.ListaComercialCompraNet))), 0, "Comercial");

            datosCombo.monedaSustentable = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.tiponegocio = repositorio.Listar<TipoNegocio, TipoNegocioQry>(x => new TipoNegocioQry() { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion });
            if (!PermisosHelper.Is(PermisosDataAgro.VerTodosNegocios))
            {
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 4));
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 5));
                datosCombo.tiponegocio.RemoveAt(datosCombo.tiponegocio.FindIndex(x => x.TipoNegocioId == 6));
            }
            datosCombo.Clasificacion = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Bolsa = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Condicion = repositorio.Listar<CondicionFijacion, CondicionFijacionQry>(x => new CondicionFijacionQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Standard = repositorio.Listar<StandardDeCalidad, StandardDeCalidadQry>(x => new StandardDeCalidadQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoDB = repositorio.Listar<TipoDB, TipoDBQry>(x => new TipoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoPeriodoDB = repositorio.Listar<TipoPeriodoDB, TipoPeriodoDBQry>(x => new TipoPeriodoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.MonedaDescuento = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.NivelTarifa = repositorio.Listar<NivelTarifa, NivelTarifaQry>(x => new NivelTarifaQry() { Id = x.Id, Descripcion = x.Descripcion, CodigoSap = x.CodigoSap });

            datosCombo.MonedaDescuento = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });
            datosCombo.TipoFason = repositorio.Listar<TipoFason, TipoFasonQry>(x => new TipoFasonQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.TipoAgenteCompra = repositorio.Listar<TipoAgenteCompra, TipoAgenteCompraQry>(x => new TipoAgenteCompraQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.Operador = repositorio.Listar<Operador, OperadorQry>(x => new OperadorQry() { Id = x.Id, Descripcion = x.Descripcion });
            datosCombo.Zona= repositorio.Listar<Zona, ZonaQry>(x => new ZonaQry() { Id = x.Id, Descripcion = x.Descripcion });
            Array estadosValues = Enum.GetValues(typeof(EnumEstadoContrato));

            foreach (int estadoValue in estadosValues)
            {
                string estadoName = Enum.GetName(typeof(EnumEstadoContrato), estadoValue);

                EstadosContratos item = new EstadosContratos(estadoValue, estadoName);

                datosCombo.estadoContrato.Add(item);
            }

            return datosCombo;
        }

        private Resultado Validar(Contrato oParam, Resultado oErrorMessages)
        {
            if (oParam.ProveedorId == 0)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' no debe estar vacio");
                return oErrorMessages;
            }
            if (oParam.ProveedorId == -1)
            {
                oErrorMessages.Error("ProveedorId", "El campo 'Proveedor' debe tener un proveedor existente");
                return oErrorMessages;
            }
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
            if (!string.IsNullOrEmpty(proveedor.RiesgoComercialSap))
            {
                if (proveedor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Riesgo Comercial Alto");
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
                if (sisa.EstadoCuit == 3)
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
            if (repositorio.Existe<ProveedorEstado>(x => x.ProveedorId == oParam.ProveedorId && x.EstadoId == 4))
            {
                oErrorMessages.Error("Estado", "Proveedor no Operable por Estado BAJA");
            }
            var facacop = repositorio.Obtener<FACACOP>(x => x.CUIT == proveedor.CUIT);
            if (facacop != null)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor No Operable por ser Apócrifo");
            }

            if (oParam.CorredorId != null)
            {
                if (!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                {
                    oErrorMessages.Error("Corredor", "El Proveedor no pertenece al Corredor seleccionado");
                }
            }
            var alta = altaTempranaAgent.ObtenerAlta(proveedor.CUIT);
            if (string.IsNullOrEmpty(alta.Mensaje))
            {
                if (oParam.Consignatario.HasValue&& oParam.Consignatario.Value&& alta.Consignatario == "NO") {
                    oErrorMessages.Error("Consignatario","El proveedor no está habilitado como Consignatario");                    
                }
                if (oParam.PlanCanje.HasValue && oParam.PlanCanje.Value && alta.PlanCanje == "NO") {
                    oErrorMessages.Error("PlanCanje","El proveedor no está habilitado como Proveedor Plan canje");
                }
                if (oParam.BoletoId == 4 && oParam.ProvinciaId != 1 && oParam.ProvinciaId != 12 && oParam.ProvinciaId != 21)
                {
                    oErrorMessages.Error("Carta Oferta", "No está habilitado Carta Oferta");
                }
                else if (oParam.BoletoId == 4 && (oParam.ProvinciaId == 1 || oParam.ProvinciaId == 12 || oParam.ProvinciaId == 21) && alta.Carta == "NO")
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
            }
            else
            {
                oErrorMessages.Error("", alta.Mensaje);
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
            if (oParam.DestinoId == 0 || oParam.DestinoId == null)
            {
                oErrorMessages.Error("DestinoId", "El campo 'Destino' no debe estar vacio");
            }
            if (oParam.ClasificacionId == 1)
            {
                var centro = repositorio.Obtener<Centro, string>(x => x.Id == oParam.DestinoId, x => x.CodigoSap);
                var material = repositorio.Obtener<Material, string>(x => x.MaterialId == oParam.MaterialId, x => x.Codigo);
                var cosecha = repositorio.Obtener<Campaña, string>(x => x.CampañaId == oParam.CampanaId, x => x.Descripcion);
                var result = capacidadProductiva.ObtenerCapacidadProductiva(proveedor.CUIT, (decimal)oParam.Cantidad, centro, cosecha, material);
                if (result.ToUpper() != "OK".ToUpper())
                {
                    oErrorMessages.Error("Capacidad Productiva", result);
                }
            }
            
            if (oParam.ContratoMadre != null)
            {
                var sap = oParam.ContratoMadre.PadLeft(10, '0');
                var cantidadMadre = repositorio.Obtener<Contrato, double>(x => x.ContratoSAP == sap, x => x.Cantidad);
                var sumaContratosHijos = repositorio.Listar<Contrato>(x => x.ContratoMadre == sap && x.ContratoId != oParam.ContratoId && (x.EstadoId <= 5 || x.EstadoId == 7)).Select(x => x.Cantidad).Sum();
                if (cantidadMadre - sumaContratosHijos < oParam.Cantidad)
                {
                    oErrorMessages.Error("Cantidad", "La cantidad supera a la cantidad del Convenio");
                }
            }
            if (oParam.Precio == 0 && oParam.TipoNegocioId != 1 && (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacio");
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
            if ((string.IsNullOrEmpty(oParam.MonedaId) && oParam.TipoNegocioId != 1) && (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
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
            if (oParam.TipoNegocioId == 1 && (oParam.CondicionFijacionId == null || oParam.DesdeFijacion == null || oParam.HastaFijacion == null))
            {
                oErrorMessages.Error("CondicionFijacionId", "Las Condiciones de Fijaciones no debe estar vacio cuando el contrato es 'A FIJAR'");
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

            var rangosPrecio = repositorio.Obtener<RangoPrecio>(x => x.MaterialId == oParam.MaterialId && x.MonedaId == oParam.MonedaId);
            if (!oParam.Pizarra.Value && oParam.TipoNegocioId == 2)
            {
                if (rangosPrecio != null && oParam.TipoNegocioId == 2 && (oParam.Precio < rangosPrecio.PrecioMinimo || oParam.Precio > rangosPrecio.PrecioMaximo))
                {
                    oErrorMessages.Error("Precio", "Precio fuera de Rango, Precio Mínimo: " + rangosPrecio.PrecioMinimo + " Precio Máximo: " + rangosPrecio.PrecioMaximo + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
                }
            }
            if (oParam.ContratoAcuerdoId != null && oParam.ContratoAcuerdoId > 0)
            {
                var cantidadAcuerdo = contratoAcuerdoManager.TraerAcuerdo(oParam.ContratoAcuerdoId.Value).Cantidad;
                var cantidadCargada = repositorio.Listar<Contrato>(d => d.ContratoAcuerdoId == oParam.ContratoAcuerdoId.Value).Sum(d => d.Cantidad);
                if (cantidadAcuerdo < cantidadCargada + oParam.Cantidad)
                {
                    oErrorMessages.Error("", "Cantidad del negocio mayor al saldo disponible del Acuerdo (" + (cantidadAcuerdo - cantidadCargada).ToString("N0") + " tn)");
                }
            }
            if (oParam.StandardDeCalidadId == 0 || oParam.StandardDeCalidadId == null)
            {
                oErrorMessages.Error("", "Debe seleccionar alguna Calidad");
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
            }

            if (oParam.AperturaPrecio != null && oParam.TipoNegocioId == 2)
            {
                if ((oParam.Pizarra.HasValue && !oParam.Pizarra.Value))
                {
                    var concepto = oParam.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Financiero && (x.Porcentaje != 0 || x.Importe != 0));
                    if (!((concepto != null && (oParam.PagoDiferido.HasValue && oParam.PagoDiferido.Value) && (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value != 0)) ||
                        (concepto == null && (!oParam.PagoDiferido.HasValue || (oParam.PagoDiferido.HasValue && !oParam.PagoDiferido.Value)) && (!oParam.DiasPesificado.HasValue || (oParam.DiasPesificado.HasValue && oParam.DiasPesificado.Value == 0)))))
                    {
                        oErrorMessages.Error("", "Días de diferimiento es obligatorio con el concepto Financiero");
                    }
                }
            }
            if (oParam.DestinoId != 1 && oParam.TipoNegocioId == 2 && oParam.AperturaPrecio != null && !oParam.AperturaPrecio.Exists(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho && (x.Importe != 0 || x.Porcentaje != 0)))
            {
                oErrorMessages.Error("", "Se debe completar Redespacho en Acopios");
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
            }
            if (oParam.Dolarizado.HasValue && oParam.Dolarizado.Value && !oParam.FechaDolarizado.HasValue)
            {
                oErrorMessages.Error("dolarizado", "Se debe completar la Fecha de pesificación en negocios Dolarizados");
            }

            if (oParam.Sustentable.HasValue && oParam.Sustentable.Value && (!oParam.ImporteSustentable.HasValue || oParam.ImporteSustentable.Value == 0 || string.IsNullOrEmpty(oParam.MonedaSustentableId)))
            {
                oErrorMessages.Error("Sustentable", "Debe indicar tarifa de sustentable");
            }
            var cantidadDias = config.CantidadDias;
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
            
            return oErrorMessages;
        }

        public GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);
            if (oContratoSave.ContratoMadre != null)
            {
                var sap = oContratoSave.ContratoMadre.PadLeft(10, '0');
                var cantidadMadre = repositorio.Obtener<Contrato, double>(x => x.ContratoSAP == sap, x => x.Cantidad);
                var sumaContratosHijos = repositorio.Listar<Contrato>(x => x.ContratoMadre == sap && x.ContratoId != oContrato.ContratoId).Select(x => x.Cantidad).Sum();
                if (cantidadMadre - sumaContratosHijos < oContratoSave.Cantidad + oContrato.Ampliaciones)
                {
                    oEntityErrors.Error("Cantidad", "La cantidad supera a la cantidad del Convenio");
                }
            }
            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado))
            {
                oContratoSave.Ampliaciones = oContrato.Ampliaciones.Value;
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Reconfirmar;
                if (oContratoSave.CantidadCamiones != null)
                {
                    oContratoSave.CantidadCamiones = Convert.ToInt32(Math.Ceiling(((decimal)oContratoSave.Cantidad + (decimal)oContrato.Ampliaciones) / 30000));
                }
                repositorio.GuardarCambios();
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede ampliar");
            }

           

            return oEntityErrors;
        }

        public GrabarContratoResult GrabarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();

            Validar(oContrato, oEntityErrors);

            if (oEntityErrors.Errores.Count > 0)
            {
                return oEntityErrors;
            }

            var oContratoSave = oContrato;
            List<DescuentoBonificacion> descuentosExistentes = null;
            List<Calidad> calidadesExistentes = null;
            List<AperturaPrecio> aperturasExistentes = null;
            var hoy = DateTime.Now;

            if (oContrato.ContratoId != 0)
            {
                oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);
                descuentosExistentes = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContrato.ContratoId);
                calidadesExistentes = repositorio.Listar<Calidad>(x => x.ContratoId == oContrato.ContratoId);
                aperturasExistentes = repositorio.Listar<AperturaPrecio>(x => x.ContratoId == oContrato.ContratoId);
                if (oContratoSave.EstadoId == 5 || oContratoSave.EstadoId == 6)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }
                if ((oContratoSave.Precio != oContrato.Precio || oContratoSave.Cantidad != oContrato.Cantidad || oContratoSave.MonedaId != oContrato.MonedaId) && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                {
                    oContrato.EstadoId = 7;
                }
                else
                {
                    oContrato.EstadoId = oContratoSave.EstadoId;
                }
            } else
            {
                oContrato.Fecha = hoy;
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
            oContratoSave.UsuarioId = oContrato.UsuarioId;
            oContratoSave.ProvinciaId = oContrato.ProvinciaId;
            oContratoSave.Base = oContrato.Base;
            oContratoSave.ImporteSustentable = oContrato.ImporteSustentable;
            oContratoSave.MonedaSustentableId = oContrato.MonedaSustentableId;
            oContratoSave.FechaDolarizado = oContrato.FechaDolarizado;
            oContratoSave.DiasPesificado = oContrato.DiasPesificado;
            oContratoSave.NoInformaSio = oContrato.NoInformaSio;
            oContratoSave.TrigoEspecial = oContrato.TrigoEspecial;
            oContratoSave.EstadoId = oContrato.EstadoId;
            oContratoSave.UsuarioId = oContrato.UsuarioId;
            oContratoSave.Ampliaciones = oContrato.Ampliaciones;
            oContratoSave.Observacion = oContrato.Observacion;
            oContratoSave.DestinoId = oContrato.DestinoId;
            oContratoSave.CantidadCamiones = oContrato.CantidadCamiones;
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
            oContratoSave.ComercialCreadorId = oContrato.ComercialCreadorId;
            oContratoSave.CorredorId = oContrato.CorredorId;
            oContratoSave.PorcentajeComision = oContrato.PorcentajeComision;
            oContratoSave.ContratoVendedor = oContrato.ContratoVendedor;
            oContratoSave.ContratoCorredor = oContrato.ContratoCorredor;
            oContratoSave.SelCargoVendedor = oContrato.SelCargoVendedor;
            oContratoSave.SelCargoMOA = oContrato.SelCargoMOA;
            oContratoSave.Madre = oContrato.Madre;
            oContratoSave.ContratoMadre = oContrato.ContratoMadre?.PadLeft(10, '0');
            oContratoSave.PrecioNeto = oContrato.PrecioNeto;
            oContratoSave.StandardDeCalidadId = oContrato.StandardDeCalidadId;
            oContratoSave.Pizarra = oContrato.Pizarra;
            oContratoSave.PagoDiferido = oContrato.PagoDiferido;
            oContratoSave.ZonaId = oContrato.ZonaId;
            oContratoSave.Compensacion = oContrato.Compensacion;
            oContratoSave.TarifaFlete = oContrato.TarifaFlete;
            oContratoSave.NivelTarifaId = oContrato.NivelTarifaId == 0 ? null : oContrato.NivelTarifaId;

            if (descuentosExistentes != null)
            {
                foreach (var descExistente in descuentosExistentes)
                {
                    if (oContrato.Descuentos == null || !oContrato.Descuentos.Any(x => x.Id == descExistente.Id))
                    {
                        repositorio.Remover(descExistente);
                        if (oContratoSave.ContratoId != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                        {
                            oContratoSave.EstadoId = 7;
                        }
                    }
                }
            }
            if (oContrato.Descuentos != null)
            {
                foreach (var descuento in oContrato.Descuentos.Where(x => x.Id == 0))
                {
                    descuento.Contrato = oContratoSave;
                    repositorio.Agregar(descuento);
                    if (oContratoSave.ContratoId != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                    {
                        oContratoSave.EstadoId = 7;
                    }
                }
            }
            if (calidadesExistentes != null)
            {
                foreach (var calExistente in calidadesExistentes)
                {
                    if (oContrato.Calidad == null || !oContrato.Calidad.Any(x => x.Id == calExistente.Id))
                    {
                        repositorio.Remover(calExistente);
                        if (oContratoSave.ContratoId != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                        {
                            oContratoSave.EstadoId = 7;
                        }
                    }
                }
            }
            if (oContrato.Calidad != null)
            {
                foreach (var calidad in oContrato.Calidad.Where(x => x.Id == 0))
                {
                    calidad.Contrato = oContratoSave;
                    repositorio.Agregar(calidad);
                    if (oContratoSave.ContratoId != 0 && (oContratoSave.EstadoId != 1 && oContratoSave.EstadoId != 3))
                    {
                        oContratoSave.EstadoId = 7;
                    }
                }
            }

            if (oContratoSave.ContratoSAP != null)
            {
                oContratoSave.ContratoSAP = oContrato.ContratoSAP;
            }

            if (oContratoSave.ContratoId == 0)
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

            if (oContrato.AperturaPrecio != null)
            {
                var redespacho = oContrato.AperturaPrecio.Find(x => x.ConceptoAperturaPrecioId == (int)EnumConceptoApertura.Redespacho);
                redespacho.Importe = -1 * Math.Abs(redespacho.Importe);
                oContratoSave.AperturaPrecio = oContrato.AperturaPrecio;
            }

            if (ConfirmacionAutomatica(oContrato))
            {
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                logger.Debug("El contrato " + oContrato.ContratoId + " se finalizo automaticamente por estar dentro de los rangos configurados");

            }
            repositorio.GuardarCambios();
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
             (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.ContratoId != contrato.ContratoId && x.TipoNegocioId == 2
             && x.MaterialId== rango.MaterialId);
            cantidad.AddRange(repositorio.Listar<FijacionDePrecioContrato, double>(x => x.Cantidad, x => x.Fecha == hoy &&
            (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && x.MaterialId == rango.MaterialId));
            var total = cantidad.Sum();
            var precioContrato = contrato.PrecioNeto ?? contrato.Precio;

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

        public KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, bool corredor, List<int> listComercialesId, List<int> corredoresComercial, bool? compranet = null)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosContratos(request, corredor, listComercialesId, corredoresComercial, compranet));
        }
        public KendoGridContratoDto TraerContratosFiltrados(FiltroReporteNegocioDto filtro, bool corredor, List<int> listComercialesId, List<int> corredoresComercial)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerContratosPorFiltro(filtro, corredor, listComercialesId, corredoresComercial));
        }
        public GrabarContratoResult ConfirmarContrato(int contratoId)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente ||
                                          oContratoSave.EstadoId == (int)EnumEstadoContrato.Oferta ||
                                          oContratoSave.EstadoId == (int)EnumEstadoContrato.Reconfirmar))
            {
                oContratoSave.Cantidad += oContratoSave.Ampliaciones ?? 0;
                oContratoSave.Ampliaciones = 0;

                oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;

                repositorio.GuardarCambios();
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
            else
            {
                oEntityErrors.Error("", "El contrato no se puede confirmar");
            }

            return oEntityErrors;
        }

        public GrabarContratoResult FinalizarContrato(int contratoId, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);
            
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error))
            {
                var diaAnterior = diasHabilesAgent.UltimoDiaHabil();

                if (oContratoSave.Fecha < diaAnterior)
                {
                    oEntityErrors.Error("", "Fecha del contrato debe ser la de hoy o día hábil anterior");
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                    repositorio.GuardarCambios();
                    return oEntityErrors;
                }
                try
                {
                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.ContratoId);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.ContratoId == oContratoSave.ContratoId);
                    var objApertura = repositorio.Listar<AperturaPrecio>(x => x.ContratoId == oContratoSave.ContratoId);

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
                    repositorio.GuardarCambios();

                    try
                    {
                        oContratoSave.ContratoSAP = nroContratoSAP;
                    }
                    catch (Exception e)
                    {
                        oContratoSave.ContratoSAP = "";
                        logger.Error(e);
                    }

                    try
                    {
                        mobjProveedorManager.EnviarEmail(oContratoSave, objDescuento, objCalidad, idActiveDirectory, null);
                        var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.Comercial.ComercialId);
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
                catch (Exception e)
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                    repositorio.GuardarCambios();
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
            }
            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public GrabarContratoResult BorrarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId < (int)EnumEstadoContrato.Finalizado))
            {
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Rechazado;
                repositorio.GuardarCambios();
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
                message = "El contrato " + contrato.ContratoId + " ha sido confirmado a las " + hora;
            }
            else if (contrato.EstadoId == 5)
            {
                title = "Contrato Finalizado";
                message = "El contrato " + contrato.ContratoSAP + " ha sido finalizado a las " + hora + " por " + contrato.Comercial.Nombres + " " + contrato.Comercial.Apellido;
            }
            else if (contrato.EstadoId == 6)
            {
                title = "Contrato Rechazado";
                message = "El contrato " + contrato.ContratoId + " ha sido rechazado a las " + hora;
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
                TipoDBId = desc.TipoDBId,
                TipoDBDesc = desc.TipoDB.Descripcion,
                TipoPeriodoDBDesc = desc.TipoPeriodoDB.Descripcion,
                TipoPeriodoDBId = desc.TipoPeriodoDBId
            },
            x => x.ContratoId == contratoId);
        }
        public List<CalidadDto> TraerCalidadesPorContrato(int contratoId)
        {
            return repositorio.Listar<Calidad, CalidadDto>(cal => new CalidadDto()
            {
                ContratoId = cal.ContratoId,
                Id = cal.Id,
                CalidadEspecialDesc = cal.CalidadEspecial.Descripcion,
                CalidadEspecialId = cal.CalidadEspecialId,
                Valor = cal.Valor,
                PorcentajeDesde = cal.PorcentajeDesde,
                PorcentajeHasta = cal.PorcentajeHasta,                
            },
            x => x.ContratoId == contratoId);
        }
        public BasicoContrato TraerContrato(int contratoId)
        {
            var contrato = repositorio.Obtener<Contrato, BasicoContrato>(x => x.ContratoId == contratoId, x => new BasicoContrato
            {
                ContratoId = x.ContratoId,
                ProveedorId = x.ProveedorId,
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
                CampanaId = x.CampanaId,
                ProvinciaId = x.ProvinciaId,
                Provincia = x.Provincia.Nombre,
                LocalidadId = x.LocalidadId,
                Localidad = x.Localidad.Nombre,
                ContratoSAP = x.ContratoSAP,
                Base = x.Base,
                Observacion = x.Observacion,
                Estado = x.EstadoId,
                Importe_Sustentable = x.ImporteSustentable,
                Moneda_Sustentable = x.MonedaSustentableId,
                Fecha_DolarizadoFormateado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDolarizado.Value.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDolarizado) : "",
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
                ContratoMadre = x.ContratoMadre,
                Pizarra = x.Pizarra.HasValue ? x.Pizarra.Value : false,
                StandardCalidadId = x.StandardDeCalidadId,
                StandardDeCalidadDescripcion = x.StandardDeCalidad.Descripcion,
                PagoDiferido= x.PagoDiferido,
                ZonaId = x.ZonaId,
                ZonaDescripcion = x.Zona.Descripcion,
                Compensacion=x.Compensacion,
                NivelTarifaId = x.NivelTarifaId,
                TarifaFlete = x.TarifaFlete
                
            });
            contrato.Descuentos = TraerDescuentosPorContrato(contratoId);
            contrato.Calidades = TraerCalidadesPorContrato(contratoId);
            contrato.AperturaPrecios = TraerAperturaDePrecioPorContrato(contratoId);
            return contrato;
        }

        public void EnviarMailPendiente()
        {
            var hoy = DateTime.Now.Date;
            var contratosPendientes = repositorio.Listar<Contrato, AvisoContratoDto>(x => new AvisoContratoDto
            {
                ContratoId = x.ContratoId,
                RazonSocial = x.Proveedor.RazonSocial,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                Moneda = x.MonedaId,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                ComercialCreadorAD = x.ComercialCreadorId.HasValue ? x.ComercialCreador.IdActiveDirectory : x.Comercial.IdActiveDirectory,
                NombreApellido = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => (x.EstadoId == 1 || x.EstadoId == 3) && x.Fecha < hoy);
            var comercialesMesa = repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, IdActiveDirectory = x.IdActiveDirectory }, x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso== PermisosDataAgro.NotificacionesMailTodos)));
            var mailComercialesMesa = new List<string>();
            foreach (var mesa in comercialesMesa)
            {
                try
                {
                    mailComercialesMesa.Add(mobjProveedorManager.GetEmailUserActiveDirectory(mesa.IdActiveDirectory));
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
                                emailComercial = mobjProveedorManager.GetEmailUserActiveDirectory(contratosPorCreador.Key);
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
                        var rutaMolinos = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png");
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
            var contratosConfirmados = repositorio.Listar<Contrato>(x => x.EstadoId == 2 || x.EstadoId == 4);
            logger.Debug("Contratos a Finalizar: " + contratosConfirmados.Count);
            var oEntityErrors = new GrabarContratoResult();
            foreach (var contrato in contratosConfirmados)
            {
                try
                {
                    var error = FinalizarContrato(contrato.ContratoId, idActiveDirectory);
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
            return repositorio.Listar<Contrato, AvisoContratoDto>(x => new AvisoContratoDto
            {
                ContratoId = x.ContratoId,
                RazonSocial = x.Proveedor.RazonSocial,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                Moneda = x.MonedaId,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                ComercialCreadorAD = x.ComercialCreadorId.HasValue ? x.ComercialCreador.IdActiveDirectory : x.Comercial.IdActiveDirectory,
                NombreApellido = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => (x.EstadoId == 1 || x.EstadoId == 3) && equipo.Contains(x.Comercial.ComercialId) && x.Fecha < fechaHoy);
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
                ComisionPorcentaje = x.ComisionPorcentaje
            });
            return compranet;
        }

        public ContratoResult TraerContratoMadre(string sap)
        {
            sap = sap.PadLeft(10, '0');
            var result = new ContratoResult();
            var idContrato = repositorio.Obtener<Contrato, int>(x => x.ContratoSAP == sap && x.Madre == true, x => x.ContratoId);
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
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);
            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Finalizado))
            {
                var respuesta = oEliminarContratoAgent.Eliminar(oContratoSave);
                if (respuesta.Contains("Error"))
                {
                    if (respuesta.Contains("SIO"))
                    {
                        var administrativo = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y=>y.PermisosAsociados.Any(z=>z.Permiso==PermisosDataAgro.MailSio)));
                        EnviarMailSio(oContratoSave, administrativo, idActiveDirectory);
                    }
                    oEntityErrors.Error("", respuesta);
                }
                else
                {
                    try
                    {
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Eliminado;
                        repositorio.GuardarCambios();
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                        oEntityErrors.Error("", e.Message);

                    }
                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.ContratoId);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.ContratoId == oContratoSave.ContratoId);
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
                        try { emailComercial.Add(mobjProveedorManager.GetEmailUserActiveDirectory(com.IdActiveDirectory)); }
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

                oMensaje.AlternateViews.Add(CuerpoMailSIO(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), oContrato, idActiveDirectory));
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
            var contrato = repositorio.Obtener<ContratoAcuerdo, BasicoContrato>(x => x.Id == contratoId, x => new BasicoContrato
            {
                ContratoId = x.Id,
                ProveedorId = x.ProveedorId ?? 0,
                Proveedor = x.Proveedor == null ? "" : x.Proveedor.RazonSocial + " " + "(" + x.Proveedor.CUIT + ")",
                CorredorId = x.CorredorId ?? 0,
                Corredor = x.Corredor == null ? "" : x.Corredor.RazonSocial + " " + "(" + x.Corredor.CUIT + ")",
                ComercialId = x.ComercialCreadorId,
                Fecha = x.Fecha,
                FechaDesdeFormateado = SqlFunctions.DateName("day", x.FechaDesde).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.DateName("day", x.FechaHasta).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.FechaHasta),
                FechaFormateado = SqlFunctions.DateName("day", x.Fecha).Trim() + "-" +
                                           SqlFunctions.StringConvert((double)x.Fecha.Month).TrimStart() + "-" +
                                           SqlFunctions.DateName("year", x.Fecha),
                TipoNegocioId = 2,
                MaterialId = x.MaterialId,
                Cantidad = x.Cantidad,
                Ampliaciones = null,
                Precio = x.Precio,
                MonedaId = x.MonedaId,
                CampanaId = 0,
                ProvinciaId = x.Proveedor.ProveedorId,
                Provincia = x.Proveedor.Provincia.Nombre,
                LocalidadId = x.Proveedor.LocalidadId,
                Localidad = x.Proveedor.Localidad.Nombre,
                ContratoSAP = "",
                Base = null,
                Observacion = "",
                Estado = x.EstadoId,
                Importe_Sustentable = 0,
                Moneda_Sustentable = "",
                Fecha_DolarizadoFormateado = "",
                Dias_Pesificado = null,
                NoInformaSIO = null,
                TrigoEspecial = null,
                ClasificacionId = x.Proveedor.ClasificacionCompraNetId,
                DestinoId = x.DestinoId,
                PlanCanje = null,
                Consignatario = x.Proveedor.Consignatario,
                CantidadCamiones = 0,
                BoletoId = x.Proveedor.BoletoCompraNetId,
                BolsaId = x.Proveedor.BolsaCompraNetId,
                DesdeFijacionFormateado = "",
                HastaFijacionFormateado = "",
                CondicionFijacion = null,
                CD = null,
                Warrant = null,
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

            });
            var dia = diasHabilesAgent.UltimoDiaHabil();
            if(contrato.Fecha < dia)
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
                contratoId = x.ContratoId,
                Id = x.Id,
                ConceptoAperturaPrecio = x.ConceptoAperturaPrecio.Descripcion,
                ConceptoAperturaPrecioId = x.ConceptoAperturaPrecioId,
                Importe = x.Importe,
                MonedaId = x.MonedaId,
                Porcentaje = x.Porcentaje
            },
            x => x.ContratoId == contratoId);
            return lista;
        }
        public TotalPesosDolares TraerTotalesPesosDolares(KendoGridMvcRequest request, List<int> listComercialesId, List<int> corredoresComercial)
        {
            var resultados = repositorio.ObtenerConsultaEscalar(new TraerTotalesPesosDolares(request, listComercialesId, corredoresComercial));
            var materialesId = resultados.Data.Select(x => x.MaterialId).Distinct();
           
            return new TotalPesosDolares {
                TotalDolares = resultados.Data.Sum(x =>Math.Round(x.TotalDolares*((decimal)x.Cantidad),0)),
                TotalPesos = resultados.Data.Sum(x => Math.Round(x.TotalPesos * ((decimal)x.Cantidad),0)),
                TotalSoja = resultados.Data.Sum(x => Math.Round( x.TotalSoja / 1000)),
                TotalMaiz = resultados.Data.Sum(x => Math.Round(x.TotalMaiz / 1000)),
                TotalTrigo = resultados.Data.Sum(x => Math.Round(x.TotalTrigo / 1000)),
                TotalGirasol = resultados.Data.Sum(x => Math.Round(x.TotalGirasol / 1000)),
                TotalGirasolAlto = resultados.Data.Sum(x => Math.Round(x.TotalGirasolAlto / 1000))
            };
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
    }
}
