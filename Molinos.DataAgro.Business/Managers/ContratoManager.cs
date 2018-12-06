using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Agent;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private ILogger logger;

        private IMaterialManager mobjMaterialManager;
        private ITipoNegocioManager mobjTipoNegocioManager;
        private ICampañaManager mobjCampaniaManager;
        private IProvinciaManager mobjProvinciaManager;
        private ILocalidadManager mobjLocalidadManager;
        private IProveedorManager mobjProveedorManager;
        private IComercialManager mobjComercialManager;
        private readonly IPushNotificationManager mobjNotification;

        public ContratoManager(ILogger logger, IRepositorio repositorio,
            IMaterialManager oMSMaterialManager, ITipoNegocioManager oMSTipoNegocioManager,
            ICampañaManager oMSCampaniaManager, IProvinciaManager oMSProvinciaManager,
            ILocalidadManager oMSLocalidadManager, IProveedorManager oMSProveedorManager,
            IComercialManager oMSComercialManager,
            IPushNotificationManager oMSNotification)
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
                (x => x.Perfil.PerfilId == (int)EnumPerfil.Comercial || x.Perfil.PerfilId == (int)EnumPerfil.Jefe || x.Perfil.PerfilId == (int)EnumPerfil.Mesa), 0, "Comercial");

            datosCombo.monedaSustentable = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.tiponegocio = repositorio.Listar<TipoNegocio, TipoNegocioQry>(x => new TipoNegocioQry() { TipoNegocioId = x.TipoNegocioId, Descripcion = x.Descripcion });

            datosCombo.Clasificacion = repositorio.Listar<ClasificacionCompraNet, ClasificacionCompraNetQry>(x => new ClasificacionCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Bolsa = repositorio.Listar<BolsaCompraNet, BolsaCompraNetQry>(x => new BolsaCompraNetQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Destino = repositorio.Listar<Centro, CentroQry>(x => new CentroQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Condicion = repositorio.Listar<CondicionFijacion, CondicionFijacionQry>(x => new CondicionFijacionQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.Standard = repositorio.Listar<StandardDeCalidad, StandardDeCalidadQry>(x => new StandardDeCalidadQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoDB = repositorio.Listar<TipoDB, TipoDBQry>(x => new TipoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.TipoPeriodoDB = repositorio.Listar<TipoPeriodoDB, TipoPeriodoDBQry>(x => new TipoPeriodoDBQry() { Id = x.Id, Descripcion = x.Descripcion });

            datosCombo.MonedaDescuento = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

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
            var proveedor = repositorio.Obtener<Proveedor>(x => x.ProveedorId == oParam.ProveedorId);
            if (!string.IsNullOrEmpty(proveedor.RiesgoComercialSap))
            {
                if (proveedor.RiesgoComercialSap.ToLower() == ConfigurationManager.AppSettings["RiesgoComercialAltoSap"])
                {
                    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Riesgo Comercial Alto");
                }
            }
            int[] otros = { 2, 3, 4, 8, 9, 10, 11, 12, 13 };
            //var sisa = repositorio.Obtener<SISA>(x => x.CUIT == proveedor.CUIT && (x.CodCategoria == 1 && oParam.ClasificacionId == 1) || (x.CodCategoria == 6 && oParam.ClasificacionId == 2) || (otros.Contains(x.CodCategoria) && oParam.ClasificacionId == 3));
            //if (sisa != null)
            //{
            //    if (sisa.EstadoCuit == 3) 
            //    {
            //        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT 3");
            //    }else if (sisa.EstadoCuit == 0 )
            //    {
            //        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Estado de CUIT Inactivo");
            //    }                
            //    if (sisa.SituacionCategoria != "AL")
            //    {
            //        oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Situación Categoría BA");
            //    }
            //}
            //else
            //{
            //    oErrorMessages.Error("ProveedorId", "Proveedor No Operable por Inactivo");
            //}
            
            var facacop = repositorio.Obtener<FACACOP>(x => x.CUIT == proveedor.CUIT);
            if (facacop != null)
            {
                oErrorMessages.Error("ProveedorId", "Proveedor No Operable por ser Apócrifo");
            }

            if(oParam.CorredorId != null)
            {
                if(!repositorio.Existe<CorredorProveedor>(x => x.CorredorId == oParam.CorredorId && x.ProveedorId == oParam.ProveedorId))
                {
                    oErrorMessages.Error("Corredor", "El Proveedor no pertenece al Corredor seleccionado");
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
            if (oParam.Precio == 0 && oParam.TipoNegocioId != 1)
            {
                oErrorMessages.Error("Precio", "El campo 'Precio' no debe estar vacio");
            }
            if (oParam.ClasificacionId == 0)
            {
                oErrorMessages.Error("ClasificacionId", "El campo 'Clasificación' no debe estar vacio");
            }
            if (oParam.DestinoId == 0 || oParam.DestinoId == null)
            {
                oErrorMessages.Error("DestinoId", "El campo 'Destino' no debe estar vacio");
            }
            if ((oParam.LocalidadId == 0 || oParam.LocalidadId == null) && (oParam.TipoNegocioId == 1 || oParam.TipoNegocioId == 2))
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' no debe estar vacio");
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
            if (string.IsNullOrEmpty(oParam.MonedaId) && oParam.TipoNegocioId != 1)
            {
                oErrorMessages.Error("MonedaId", "El campo 'Moneda' no debe estar vacio");
            }
            if (oParam.TipoNegocioId == 0)
            {
                oErrorMessages.Error("TipoNegocioId", "El campo 'Tipo de Negocio' no debe estar vacio");
            }
            if (oParam.CampanaId == 0)
            {
                oErrorMessages.Error("CampanaId", "El campo 'Campaña' no debe estar vacio");
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
            if (rangosPrecio != null && oParam.TipoNegocioId == 2 && (oParam.Precio < rangosPrecio.PrecioMinimo || oParam.Precio > rangosPrecio.PrecioMaximo))
            {
                oErrorMessages.Error("Precio", "Precio fuera de Rango, Precio Mínimo: " + rangosPrecio.PrecioMinimo + " Precio Máximo: " + rangosPrecio.PrecioMaximo + " para " + rangosPrecio.Material.Descripcion + " en " + rangosPrecio.Moneda.Descripcion);
            }
            return oErrorMessages;
        }

        public GrabarContratoResult GrabarAmpliacionContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado))
            {
                oContratoSave.Ampliaciones = oContrato.Ampliaciones.Value;
                oContratoSave.EstadoId = oContratoSave.Base == true ? (int)EnumEstadoContrato.Oferta : (int)EnumEstadoContrato.Pendiente;

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
            List<RangoConfirmacionAutomatica> rangosConfirmacionAutomaticaExistentes = repositorio.Listar<RangoConfirmacionAutomatica>(x => x.MaterialId == oContrato.MaterialId &&
                                                                                                            x.MonedaId == oContrato.MonedaId);

            if (oContrato.ContratoId != 0)
            {
                oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);
                descuentosExistentes = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContrato.ContratoId);
                calidadesExistentes = repositorio.Listar<Calidad>(x => x.ContratoId == oContrato.ContratoId);
                if (oContratoSave.EstadoId > (int)EnumEstadoContrato.Con_Error)
                {
                    oEntityErrors.Error("", "El contrato no se puede modificar");
                    return oEntityErrors;
                }
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

            if (descuentosExistentes != null)
            {
                foreach (var descExistente in descuentosExistentes)
                {
                    if (oContrato.Descuentos == null || !oContrato.Descuentos.Any(x => x.Id == descExistente.Id))
                    {
                        repositorio.Remover(descExistente);
                    }
                }
            }
            if (oContrato.Descuentos != null)
            {
                foreach (var descuento in oContrato.Descuentos.Where(x => x.Id == 0))
                {
                    descuento.Contrato = oContratoSave;
                    repositorio.Agregar(descuento);
                }
            }
            if (calidadesExistentes != null)
            {
                foreach (var calidadExistente in calidadesExistentes)
                {
                    if (oContrato.Calidad == null || !oContrato.Calidad.Any(x => x.Id == calidadExistente.Id))
                    {
                        repositorio.Remover(calidadExistente);
                    }
                }
            }
            if (oContrato.Calidad != null)
            {
                foreach (var calidad in oContrato.Calidad.Where(x => x.Id == 0))
                {
                    calidad.Contrato = oContratoSave;
                    repositorio.Agregar(calidad);
                }
            }
            if (oContratoSave.Fecha.Date != oContrato.Fecha.Date)
            {
                oContratoSave.Fecha = oContrato.Fecha;
            }

            if (oContratoSave.ContratoSAP != null)
            {
                oContratoSave.ContratoSAP = oContrato.ContratoSAP;
            }

            if (oContratoSave.ContratoId == 0)
            {
                repositorio.Agregar(oContratoSave);
            }

            if (rangosConfirmacionAutomaticaExistentes.Count >= 1)
            {
                if (rangosConfirmacionAutomaticaExistentes.Any(x => x.PrecioMinimo <= oContratoSave.Precio && x.PrecioMaximo >= oContratoSave.Precio))
                {
                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;
                    logger.Debug("El contrato" + oContratoSave.ContratoId + "se finalizo automaticamente por estar dentro de los rangos configurados");
                }
            }

            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<int> listComercialesId)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosContratos(request, listComercialesId));
        }

        public GrabarContratoResult ConfirmarContrato(int contratoId)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(contratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Pendiente || oContratoSave.EstadoId == (int)EnumEstadoContrato.Oferta))
            {
                try
                {
                    oContratoSave.Cantidad += oContratoSave.Ampliaciones.Value;
                    oContratoSave.Ampliaciones = 0;
                }
                catch { }

                oContratoSave.EstadoId = (int)EnumEstadoContrato.Confirmado;

                repositorio.GuardarCambios();
                var comerciales = mobjComercialManager.CadenaComerciales(oContratoSave.Comercial.ComercialId);
                try
                {
                    foreach (var comercialId in comerciales)
                    {
                        EnviarNotificacion(comercialId, oContratoSave);
                    }
                }
                catch(Exception e)
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
                if (oContratoSave.CorredorId != null)
                {
                    try
                    {
                        var relacionCorredor = new RelacionCorredorProveedorAgent(repositorio);
                        if (!relacionCorredor.ObtenerRelacionCorredorProveedor(oContratoSave.Corredor.CUIT, oContratoSave.Proveedor.CUIT))
                        {
                            throw new Exception(string.Format("No existe Relación entre Corredor {0} y Proveedor {1}", oContratoSave.Corredor.CUIT, oContratoSave.Proveedor.CUIT));
                        }
                    }
                    catch (Exception e)
                    {
                        oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                        repositorio.GuardarCambios();
                        logger.Error(e);
                        oEntityErrors.Error("", e.Message);
                        return oEntityErrors;
                    }
                }
                try
                {

                    var objDescuento = repositorio.Listar<DescuentoBonificacion>(x => x.ContratoId == oContratoSave.ContratoId);
                    var objCalidad = repositorio.Listar<Calidad>(x => x.ContratoId == oContratoSave.ContratoId);
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
                        //Envio de mail
                        mobjProveedorManager.EnviarEmail(oContratoSave, objDescuento, objCalidad, idActiveDirectory);
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
            var SapFinalizarContrato = new FinalizarContratoAgent(logger);
            return SapFinalizarContrato.Finalizar(contrato, descuentoBonificacion, calidad);
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
                PorcentajeHasta = cal.PorcentajeHasta
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
                PorcentajeComision =x.PorcentajeComision,
                ContratoCorredor = x.ContratoCorredor,
                ContratoVendedor = x.ContratoVendedor,
                SelCargoMOA = x.SelCargoMOA,
                SelCargoVendedor = x.SelCargoVendedor
            });
            contrato.Descuentos = TraerDescuentosPorContrato(contratoId);
            contrato.Calidades = TraerCalidadesPorContrato(contratoId);
            return contrato;
        }

        public void EnviarMailPendiente()
        {
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
            }, x => x.EstadoId == 1 || x.EstadoId == 3);
            var comercialesMesa = repositorio.Listar<Comercial, ComercialDto>(x => new ComercialDto { ComercialId = x.ComercialId, IdActiveDirectory = x.IdActiveDirectory }, x => x.PerfilId == 7);
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
                        oMensaje.AlternateViews.Add(CuerpoMailContrato(System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/MolinosAgro.png"), contratosPorCreador.ToList(), contratosPorCreador.Key));
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
            htmlBody += "<table><tr>" + th + "PROVEEDOR</th>"+ th + "CANTIDAD</th>"+ th + "PRECIO</th>" + th + "FECHA DE CARGA</th>"+ th + "COMERCIAL</th></tr>";
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
                catch(Exception ex)
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
            var diasOperables = new DiasHabiles().ObtenerDiasHabiles(repositorio);
            var ultimosDiasOperable = DateTime.Today;
            var i = 0;            
            while (i<1 || !diasOperables.Contains(ultimosDiasOperable))
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
            }, x => (x.EstadoId == 1 || x.EstadoId == 3)&& equipo.Contains(x.Comercial.ComercialId) && x.Fecha < fechaHoy);
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
                Provincia = x.ProvinciaCompraNet.Nombre
            });
            return compranet;
        }
    }
}
