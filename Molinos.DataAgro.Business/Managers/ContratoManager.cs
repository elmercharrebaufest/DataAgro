using Autofac.Extras.NLog;
using KendoGridBinder;
using KendoGridBinder.ModelBinder.Mvc;
using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;

namespace Molinos.DataAgro.Business.Managers
{


    public class ContratoManager : IContratoManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;
        
        private IMaterialManager mobjMaterialManager;
        private ITipoNegocioManager mobjTipoNegocioManager;
        private ICampañaManager   mobjCampaniaManager;
        private IProvinciaManager mobjProvinciaManager;
        private ILocalidadManager mobjLocalidadManager;
        private IProveedorManager mobjProveedorManager;
        private IComercialManager mobjComercialManager;

        
        public ContratoManager(ILogger logger, IRepositorio repositorio, 
            IMaterialManager oMSMaterialManager, ITipoNegocioManager oMSTipoNegocioManager,
            ICampañaManager oMSCampaniaManager, IProvinciaManager oMSProvinciaManager, 
            ILocalidadManager oMSLocalidadManager, IProveedorManager oMSProveedorManager, 
            IComercialManager oMSComercialManager)
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
        }

        public DatosIniContrato TraerDatosCombo()
        {
            var datosCombo = new DatosIniContrato();

            datosCombo.prov = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry() { Provinciaid = x.ProvinciaId, Nombre = x.Nombre });

            datosCombo.loc = new List<LocalidadQry>();

            datosCombo.campaña = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry() { CampañaId = x.CampañaId, Descripcion = x.Descripcion });

            datosCombo.material = repositorio.Listar<Material, MaterialQry>(x => new MaterialQry() { MaterialId = x.MaterialId, Descripcion = x.Descripcion });

            datosCombo.moneda = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.comercial = repositorio.Listar<Comercial, ComercialQry>(x => new ComercialQry() { ComercialId = x.ComercialId, Comercial = x.Nombres + " " + x.Apellido },
                (x => x.Perfil.PerfilId == (int)EnumPerfil.Comercial || x.Perfil.PerfilId == (int)EnumPerfil.Jefe || x.Perfil.PerfilId == (int)EnumPerfil.Mesa), 0, "Comercial")                              ;

            datosCombo.monedaSustentable = repositorio.Listar<Moneda, MonedaQry>(x => new MonedaQry() { MonedaId = x.MonedaId, Descripcion = x.Descripcion });

            datosCombo.proveedor = repositorio.Listar<Proveedor, ProveedorQry>(x => new ProveedorQry() { ProveedorId = x.ProveedorId, Descripcion = x.RazonSocial });

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
            if (oParam.LocalidadId == 0 && oParam.TipoNegocioId == 1 || oParam.LocalidadId == 0 && oParam.TipoNegocioId == 2)
            {
                oErrorMessages.Error("LocalidadId", "El campo 'Localidad' no debe estar vacio");
            }
            if (oParam.ProvinciaId == 0 && oParam.TipoNegocioId == 1 || oParam.ProvinciaId == 0 && oParam.TipoNegocioId == 2)
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
            if (string.IsNullOrEmpty(oParam.MonedaId))
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
            if (oParam.ComercialId == 0)
            {
                oErrorMessages.Error("ComercialId", "El campo 'Comercial' no debe estar vacio");
            }
            if (oParam.ProvinciaId == 1 && oParam.EstablecimientoPropio == null)
            {
                oErrorMessages.Error("EstablecimientoPropio", "El campo 'Establecimiento' no debe estar vacio cuando Provincia es Buenos Aires");
            }
            if (oParam.TipoNegocioId == 1 && (oParam.CondicionFijacionId == null || oParam.DesdeFijacion == null || oParam.HastaFijacion == null))
            {
                oErrorMessages.Error("EstablecimientoPropio", "El 'Plazos y Topes de Fijación' no debe estar vacio cuando el contrato es 'A FIJAR'");
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

            if (oContrato.ContratoId != 0)
            {
                oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);
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
            oContratoSave.Estado = oContrato.Estado;
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
            oContratoSave.StandardDeCalidadId = oContrato.StandardDeCalidadId;
            oContratoSave.CalidadEspecialId = oContrato.CalidadEspecialId;
            oContratoSave.ValorCalidadEspecial = oContrato.ValorCalidadEspecial;
            oContratoSave.EstablecimientoPropio = oContrato.EstablecimientoPropio;
            oContratoSave.ClasificacionId = oContrato.ClasificacionId;
            oContratoSave.CantidadCamiones = oContrato.CantidadCamiones;
            oContratoSave.BoletoId = oContrato.BoletoId;
            oContratoSave.BolsaId = oContrato.BolsaId==0?null: oContrato.BolsaId;
            oContratoSave.DesdeFijacion = oContrato.DesdeFijacion;
            oContratoSave.HastaFijacion = oContrato.HastaFijacion;

            if (oContratoSave.Descuentos != null)
            {
                foreach (var descExistente in oContratoSave.Descuentos)
                {
                    if (oContrato.Descuentos == null || !oContrato.Descuentos.Any(x => x.Id == descExistente.Id))
                    {
                        repositorio.Remover(descExistente);
                    }
                }
            }

            foreach (var descuento in oContrato.Descuentos.Where(x => x.Id == 0))
            {
                descuento.Contrato = oContratoSave;
                repositorio.Agregar(descuento);
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
            repositorio.GuardarCambios();
            return oEntityErrors;
        }

        public KendoGrid<BasicoContrato> TraerTodosContratos(KendoGridMvcRequest request, List<int> listComercialesId)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosContratos(request, listComercialesId));
        }


        public GrabarContratoResult ConfirmarContrato(Contrato oContrato)
        {
            var oEntityErrors = new GrabarContratoResult();

            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);

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
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede confirmar");
            }

            return oEntityErrors;
        }

        public GrabarContratoResult FinalizarContrato(Contrato oContrato, string idActiveDirectory)
        {
            var oEntityErrors = new GrabarContratoResult();
            var oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);

            if (oContratoSave != null && (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado || oContratoSave.EstadoId == (int)EnumEstadoContrato.Con_Error))
            {
                oContratoSave.EstadoId = (int)EnumEstadoContrato.Con_Error;
                repositorio.GuardarCambios();
                try
                {
                    var objCampania = mobjCampaniaManager.TraerCampania(oContratoSave.CampanaId);
                    var objMaterial = mobjMaterialManager.TraerMaterial(oContratoSave.MaterialId);
                    var objProvincia = mobjProvinciaManager.TraerProvincia(oContratoSave.ProvinciaId != null ? oContratoSave.ProvinciaId.Value : 0);
                    var objTiponegocio = mobjTipoNegocioManager.TraerTipoNegociod(oContratoSave.TipoNegocioId);
                    var objLocalidad = mobjLocalidadManager.TraerLocalidad(oContratoSave.LocalidadId != null ? oContratoSave.LocalidadId.Value : 0);
                    var objProveedor = mobjProveedorManager.TraerProveedor(oContratoSave.ProveedorId);
                    var objComercial = mobjComercialManager.TraerComercial(oContratoSave.ComercialId != null ? oContratoSave.ComercialId.Value : 0);

                    string nroContratoSAP = SAPFinalizarContrato(oContratoSave, objCampania.Descripcion, objMaterial.Codigo, objProvincia.ProvinciaId.ToString(), objTiponegocio.Descripcion, objLocalidad.CodLocalidad, objProveedor.CUIT, objComercial != null ? objComercial.IdActiveDirectory : "");

                    oContratoSave = repositorio.Obtener<Contrato>(oContrato.ContratoId);

                    oContratoSave.EstadoId = (int)EnumEstadoContrato.Finalizado;
                    try
                    {
                        oContratoSave.ContratoSAP = Convert.ToInt32(nroContratoSAP);
                    }
                    catch (Exception e)
                    {
                        oContratoSave.ContratoSAP = 0;
                        logger.Error(e);
                    }

                    try
                    {
                        //Envio de mail
                        mobjProveedorManager.EnviarEmail(oContratoSave, idActiveDirectory);
                    }
                    catch (Exception e)
                    {
                        logger.Error(e);
                    }

                    repositorio.GuardarCambios();
                }
                catch (Exception e)
                {
                    logger.Error(e);
                    oEntityErrors.Error("", e.Message);
                }
            }
            else
            {
                if (oContratoSave.EstadoId == (int)EnumEstadoContrato.Confirmado)
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
            }
            else
            {
                oEntityErrors.Error("", "El contrato no se puede rechazar");
            }
            return oEntityErrors;
        }

        public int ObtenerComercialId(string idActiveDirectory)
        {
            return repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x=> x.ComercialId);
        }

        public string SAPFinalizarContrato(Contrato contrato, string campaniaDescripcion, string materialCodigo, string provinciaId, string tiponegocioDescripcion, string localidadCod, string proveedorCUIT, string comercial)
        {
            var SapFinalizarContrato = new FinalizarContratoAgent(logger);
            return SapFinalizarContrato.Finalizar(contrato, campaniaDescripcion, materialCodigo, provinciaId, tiponegocioDescripcion, localidadCod, proveedorCUIT, comercial);
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


    }

}
