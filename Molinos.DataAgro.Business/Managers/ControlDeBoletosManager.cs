using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;

namespace Molinos.DataAgro.Business.Managers
{
    public class ControlDeBoletosManager : IControlDeBoletosManager
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        private readonly IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent;
        private readonly ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent;
        private readonly IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent;
        private readonly IDatosCertificacionControlBoletoAgent datosCertificacionControlBoletoAgent;
        public ControlDeBoletosManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, 
                                       IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent, 
                                       ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent,
                                       IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent,
                                       IDatosCertificacionControlBoletoAgent datosCertificacionControlBoletoAgent
                                       )
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.modificacionContratoControlBoletoAgent = modificacionContratoControlBoletoAgent;
            this.seguimientoControlBoletoAgent = seguimientoControlBoletoAgent;
            this.confirmaConsultaDocumentosAgent = confirmaConsultaDocumentosAgent;
            this.datosCertificacionControlBoletoAgent = datosCertificacionControlBoletoAgent;
        }

        public Resultado AsociarConfirma(int negocioId)
        {
            throw new NotImplementedException();
        }

        #region Reporte Seguimiento Boletos

        public List<ControlDeBoletosReporteSeguimientoConsultaDto> GetReporteDeSeguimientoBoletos(ControlDeBoletoFiltroSeguimientoDto filtros)
        {
            var query = from cb in repositorio.Listar<ControlDeBoletos>()
                        join pre in repositorio.Listar<ControlDeBoletosPreCertificacion>()
                            on cb.Id equals pre.ControlDeBoletosId into preJoin
                        from pre in preJoin.DefaultIfEmpty()
                        join seg in repositorio.Listar<ControlDeBoletosSeguimiento>()
                            on cb.Id equals seg.ControlDeBoletosId into segJoin
                        from seg in segJoin.DefaultIfEmpty()
                        select new { cb, pre, seg };

            // FILTROS GENERALES
            if (!string.IsNullOrEmpty(filtros.ContratoSAPDesde))
                query = query.Where(x => string.Compare(x.cb.Negocio.ContratoSAP, filtros.ContratoSAPDesde) >= 0);

            if (!string.IsNullOrEmpty(filtros.ContratoSAPHasta))
                query = query.Where(x => string.Compare(x.cb.Negocio.ContratoSAP, filtros.ContratoSAPHasta) <= 0);

            if (filtros.MaterialId.HasValue)
                query = query.Where(x => x.cb.Negocio.MaterialId == filtros.MaterialId.Value);

            if (filtros.Proveedor.HasValue)
                query = query.Where(x => x.cb.Negocio.ProveedorId == filtros.Proveedor.Value);

            if (filtros.BolsaId.HasValue)
                query = query.Where(x => x.cb.Negocio.BolsaId == filtros.BolsaId.Value);

            // FILTROS PRECERTIFICACIÓN
            if (filtros.FechaCertificacionDesde.HasValue)
                query = query.Where(x => x.pre != null && x.pre.FechaCertificacion >= filtros.FechaCertificacionDesde.Value);

            if (filtros.FechaCertificacionHasta.HasValue)
                query = query.Where(x => x.pre != null && x.pre.FechaCertificacion <= filtros.FechaCertificacionHasta.Value);

            if (filtros.FechaVencimientoCertificacionDesde.HasValue)
                query = query.Where(x => x.pre != null && x.pre.FechaVencimiento >= filtros.FechaVencimientoCertificacionDesde.Value);

            if (filtros.FechaVencimientoCertificacionHasta.HasValue)
                query = query.Where(x => x.pre != null && x.pre.FechaVencimiento <= filtros.FechaVencimientoCertificacionHasta.Value);

            // FILTROS SEGUIMIENTO
            if (filtros.FechaRecepBoletoDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaRecepBoleto.HasValue && x.seg.FechaRecepBoleto.Value >= filtros.FechaRecepBoletoDesde.Value);

            if (filtros.FechaRecepBoletoHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaRecepBoleto.HasValue && x.seg.FechaRecepBoleto.Value <= filtros.FechaRecepBoletoHasta.Value);

            if (filtros.FechaEnviadoFirmaDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnviadoFirma.HasValue && x.seg.FechaEnviadoFirma.Value >= filtros.FechaEnviadoFirmaDesde.Value);

            if (filtros.FechaEnviadoFirmaHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnviadoFirma.HasValue && x.seg.FechaEnviadoFirma.Value <= filtros.FechaEnviadoFirmaHasta.Value);

            if (filtros.FechaRecibFirmaDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaRecibFirma.HasValue && x.seg.FechaRecibFirma.Value >= filtros.FechaRecibFirmaDesde.Value);

            if (filtros.FechaRecibFirmaHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaRecibFirma.HasValue && x.seg.FechaRecibFirma.Value <= filtros.FechaRecibFirmaHasta.Value);

            if (filtros.FechaEnvioBolsaDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnvioBolsa.HasValue && x.seg.FechaEnvioBolsa.Value >= filtros.FechaEnvioBolsaDesde.Value);

            if (filtros.FechaEnvioBolsaHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnvioBolsa.HasValue && x.seg.FechaEnvioBolsa.Value <= filtros.FechaEnvioBolsaHasta.Value);

            if (filtros.FechaVueltaBolsaDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaVueltaBolsa.HasValue && x.seg.FechaVueltaBolsa.Value >= filtros.FechaVueltaBolsaDesde.Value);

            if (filtros.FechaVueltaBolsaHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaVueltaBolsa.HasValue && x.seg.FechaVueltaBolsa.Value <= filtros.FechaVueltaBolsaHasta.Value);

            if (filtros.FechaEnvioAfipDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnvioAfip.HasValue && x.seg.FechaEnvioAfip.Value >= filtros.FechaEnvioAfipDesde.Value);

            if (filtros.FechaEnvioAfipHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaEnvioAfip.HasValue && x.seg.FechaEnvioAfip.Value <= filtros.FechaEnvioAfipHasta.Value);

            if (filtros.FechaVueltaAfipDesde.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaVueltaAfip.HasValue && x.seg.FechaVueltaAfip.Value >= filtros.FechaVueltaAfipDesde.Value);

            if (filtros.FechaVueltaAfipHasta.HasValue)
                query = query.Where(x => x.seg != null && x.seg.FechaVueltaAfip.HasValue && x.seg.FechaVueltaAfip.Value <= filtros.FechaVueltaAfipHasta.Value);

            // PAGINACIÓN Y PROYECCIÓN
            var data = query
                .OrderBy(x => x.cb.Id)
                .Skip(filtros.Skip)
                .Take(filtros.Take)
                .Select(x => new ControlDeBoletosReporteSeguimientoConsultaDto
                {
                    Id = x.cb.Id,
                    NegocioId = x.cb.NegocioId,
                    ControlDeBoletosEstadoId = x.cb.ControlDeBoletosEstadoId,
                    ControlDeBoletosEstado = x.cb.ControlDeBoletosEstado.Descripcion,
                    EsConfirma = x.cb.EsConfirma,
                    AltaIdLoteConfirma = x.cb.AltaIdLoteConfirma.HasValue ? x.cb.AltaIdLoteConfirma.Value.ToString() : null,
                    IdentificadorConfirma = x.cb.IdentificadorConfirma.HasValue ? x.cb.IdentificadorConfirma.Value.ToString() : null,
                    FechaCreacion = x.cb.FechaCreacion,
                    FechaModificacion = x.cb.FechaModificacion,
                    EstadoConfirmaId = x.cb.EstadoConfirmaId,
                    FechaControlIniciado = x.cb.FechaControlIniciado,
                    FechaControlFinalizado = x.cb.FechaControlFinalizado,
                    FechaCertificacionCompletada = x.cb.FechaCertificacionCompletada,
                    FechaRegistroDatosOblea = x.cb.FechaRegistroDatosOblea,
                    MaterialId = x.cb.Negocio.MaterialId,
                    Material = x.cb.Negocio.Material.Descripcion,
                    BolsaCompraNetId = x.cb.Negocio.BolsaId ?? 0,
                    BolsaCompraNet = x.cb.Negocio.Bolsa.Descripcion,
                    ComercialId = x.cb.Negocio.ComercialId ?? 0,
                    Comercial = x.cb.Negocio.Comercial.Nombres + " " + x.cb.Negocio.Comercial.Apellido,
                    TipoBoleto = repositorio.Obtener<BoletoCompraNet>(x.cb.Negocio.BoletoId).Descripcion,
                    ContratoSAP = x.cb.Negocio.ContratoSAP,
                    ProveedorId = x.cb.Negocio.ProveedorId ?? 0,
                    Proveedor = x.cb.Negocio.Proveedor.RazonSocial,
                    PreCertificacionId = x.pre != null ? (int?)x.pre.Id : null,
                    SeguimientoBoletoId = x.seg != null ? (int?)x.seg.Id : null,
                    FechaCertificacion = x.pre != null ? (DateTime?)x.pre.FechaCertificacion : null,
                    FechaVencimientoCertificacion = x.pre != null ? (DateTime?)x.pre.FechaVencimiento : null,
                    FechaEnviadoFirma = x.seg != null ? x.seg.FechaEnviadoFirma : null,
                    FechaEnvio = x.seg != null ? x.seg.FechaEnvio : null,
                    FechaEnvioAfip = x.seg != null ? x.seg.FechaEnvioAfip : null,
                    FechaEnvioBolsa = x.seg != null ? x.seg.FechaEnvioBolsa : null,
                    FechaRecepBoleto = x.seg != null ? x.seg.FechaRecepBoleto : null,
                    FechaRecibFirma = x.seg != null ? x.seg.FechaRecibFirma : null,
                    FechaVueltaAfip = x.seg != null ? x.seg.FechaVueltaAfip : null,
                    FechaVueltaBolsa = x.seg != null ? x.seg.FechaVueltaBolsa : null,
                    FechaAcopio = x.seg != null ? x.seg.FechaAcopio : null
                })
                .ToList();

            return data;
        }

        #endregion


        #region Metodo para cargar combos
        public List<BolsaCompraNet> GetBolsaCompraNet()
        {
            return this.repositorio.Listar<BolsaCompraNet>();
        }
        public List<ComercialCombo> GetComercial()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmComercialCombo();
        }
        public List<MaterialCombo> GetMaterial()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmMaterialCombo();
        }
        public List<ProveedorCombo> GetProveedorPorComercial(List<int> equipo)
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetProveedorPorComercialCombo(equipo);
        }
        public List<Provincia> GetProvincias()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetProvinciaCombo();
        }
        public List<CampaniaCombo> GetCosechas()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetAbmCampaniaCombo();
        }
        public List<ClasificacionCompraNet> GetClasificaciones()
        {
            var qry = new CombosQueries(logger, repositorio);
            return qry.GetClasificacionCombo();
        }
        public List<Localidad> GetProcedencias(int provinciaId)
        {
            return this.repositorio.Listar<Localidad>(x => x.ProvinciaId == provinciaId);
        }
        public List<TipoOblea> GetTipoOblea()
        {
            return this.repositorio.Listar<TipoOblea>().OrderBy(x=> x.Descripcion).ToList();
        }
        public List<BoletoCompraNet> GetBoletoCompraNet()
        {
            return this.repositorio.Listar<BoletoCompraNet>().OrderBy(x => x.Descripcion).ToList();
        }
        #endregion

        #region Pendientes de Control
        public List<ControlDeBoletosConsultaDto> GetControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros)
        {
            var query = repositorio.Listar<ControlDeBoletos>()
                .Where(x => (filtros.EstadoControlId == null ||  (x.ControlDeBoletosEstadoId == (int)filtros.EstadoControlId)));

            if (!string.IsNullOrEmpty(filtros.ContratoSAPDesde))
                query = query.Where(x => string.Compare(x.Negocio.ContratoSAP, filtros.ContratoSAPDesde) >= 0);

            if (!string.IsNullOrEmpty(filtros.ContratoSAPHasta))
                query = query.Where(x => string.Compare(x.Negocio.ContratoSAP, filtros.ContratoSAPHasta) <= 0);

            if (filtros.MaterialId.HasValue)
                query = query.Where(x => x.Negocio.MaterialId == filtros.MaterialId.Value);

            if (filtros.EstadoControlId.HasValue)
                query = query.Where(x => x.ControlDeBoletosEstadoId == filtros.EstadoControlId.Value);

            if (filtros.EsConfirma)
                query = query.Where(x => x.EsConfirma == true);

            if (filtros.FechaCargaDesde.HasValue)
                query = query.Where(x => x.FechaCreacion >= filtros.FechaCargaDesde.Value);

            if (filtros.FechaCargaHasta.HasValue)
                query = query.Where(x => x.FechaCreacion <= filtros.FechaCargaHasta.Value);

            if (filtros.Proveedor.HasValue)
                query = query.Where(x => x.Negocio.ProveedorId == filtros.Proveedor.Value);

            if (filtros.BolsaId.HasValue)
                query = query.Where(x => x.Negocio.BolsaId == filtros.BolsaId.Value);

            if (filtros.ComercialId.HasValue)
                query = query.Where(x => x.Negocio.ComercialId == filtros.ComercialId.Value);

            // 🚀 PROYECCIÓN FINAL
            var list = query.ToList();
            var result = new List<ControlDeBoletosConsultaDto>();

            foreach (var controlBoleto in list)
            {
                // Buscar PreCertificacion y Seguimiento por ControlDeBoletosId
                var preCertificacion = repositorio.Obtener<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == controlBoleto.Id);
                var seguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlBoleto.Id);

                result.Add(new ControlDeBoletosConsultaDto
                {
                    Id = controlBoleto.Id,
                    NegocioId = controlBoleto.NegocioId,
                    ControlDeBoletosEstadoId = controlBoleto.ControlDeBoletosEstadoId,
                    ControlDeBoletosEstado = controlBoleto.ControlDeBoletosEstado.Descripcion,
                    EsConfirma = controlBoleto.EsConfirma,
                    AltaIdLoteConfirma = controlBoleto.AltaIdLoteConfirma,
                    IdentificadorConfirma = controlBoleto.IdentificadorConfirma,
                    FechaCreacion = controlBoleto.FechaCreacion,
                    FechaModificacion = controlBoleto.FechaModificacion,
                    EstadoConfirmaId = controlBoleto.EstadoConfirmaId,
                    EstadoConfirma = controlBoleto.EstadoConfirmaId.HasValue ? repositorio.Obtener<EstadoConfirma>(controlBoleto.EstadoConfirmaId.Value)?.Descripcion : null,
                    TipoBoleto = repositorio.Obtener<BoletoCompraNet>(controlBoleto.Negocio.BoletoId).Descripcion,
                    ControlIniciado = controlBoleto.ControlIniciado,
                    ControlFinalizado = controlBoleto.ControlFinalizado,
                    CertificacionCompletada = controlBoleto.CertificacionCompletada,
                    RegistroDatosOblea = controlBoleto.RegistroDatosOblea,
                    FechaControlIniciado = controlBoleto.FechaControlIniciado,
                    FechaControlFinalizado = controlBoleto.FechaControlFinalizado,
                    FechaCertificacionCompletada = controlBoleto.FechaCertificacionCompletada,
                    FechaRegistroDatosOblea = controlBoleto.FechaRegistroDatosOblea,
                    MaterialId = controlBoleto.Negocio.MaterialId,
                    Material = controlBoleto.Negocio.Material.Descripcion,
                    BolsaCompraNetId = controlBoleto.Negocio.BolsaId,
                    BolsaCompraNet = controlBoleto.Negocio.Bolsa.Descripcion,
                    ComercialId = controlBoleto.Negocio.ComercialId,
                    Comercial = controlBoleto.Negocio.Comercial.Nombres + " " + controlBoleto.Negocio.Comercial.Apellido,
                    ContratoSAP = controlBoleto.Negocio.ContratoSAP,
                    ProveedorId = controlBoleto.Negocio.ProveedorId,
                    Proveedor = controlBoleto.Negocio.Proveedor.RazonSocial,
                    PreCertificacionId = preCertificacion != null ? (int?)preCertificacion.Id : null,
                    SeguimientoBoletoId = seguimiento != null ? (int?)seguimiento.Id : null
                });
            }

            return result;
        }
        public Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null)
        {
            var oResultado = new Resultado();
            var negocio = repositorio.Obtener<Negocio>(negocioId);
            if (negocio != null)
            {
                var existe = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocioId);
                if (existe == null)
                {
                    var controlDeBoletos = new ControlDeBoletos()
                    {
                        NegocioId = negocioId,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA,
                        AltaIdLoteConfirma = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdLoteConfirma : (int?)null,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? (int)EnumEstadoConfirma.PENDIENTE : (int?)null,
                        ControlIniciado = false,
                        ControlFinalizado = false,
                        CertificacionCompletada = false,
                        RegistroDatosOblea = false
                    };
                    repositorio.Agregar(controlDeBoletos);
                    repositorio.GuardarCambios();
                }
            }
            return oResultado;
        }
        public Resultado RegistrarAcciones(List<int> ControlDeBoletoIds, EnumControlDeBoletosAcciones accion)
        {
            var oResultado = new Resultado();
            try
            {
                var boletos = repositorio.Listar<ControlDeBoletos>(x => ControlDeBoletoIds.Contains(x.Id));

                if (boletos.Count == 0)
                {
                    oResultado.Errores.Add(new ErrorMessage()
                    {
                        Message = "No se encontró el control de boletos.",
                    });
                    return oResultado;
                }

                var ahora = DateTime.Now;

                foreach (var boleto in boletos)
                {
                    switch (accion)
                    {
                        case EnumControlDeBoletosAcciones.ControlIniciado:
                            boleto.ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.EN_PROCESO;
                            boleto.ControlIniciado = true;
                            boleto.FechaControlIniciado = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.RegistroDatosOblea:
                            boleto.ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.EN_OBLEA;
                            boleto.RegistroDatosOblea = true;
                            boleto.FechaRegistroDatosOblea = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.CertificacionCompletada:
                            boleto.ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.EN_CERTIFICACION;
                            boleto.CertificacionCompletada = true;
                            boleto.FechaCertificacionCompletada = ahora;
                            break;

                        case EnumControlDeBoletosAcciones.ControlFinalizado:
                            boleto.ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.FINALIZADO;
                            boleto.ControlFinalizado = true;
                            boleto.FechaControlFinalizado = ahora;
                            break;

                        default:
                            oResultado.Errores.Add(new ErrorMessage()
                            {
                                Message = "Acción no válida.",
                            });
                            return oResultado;
                    }
                    repositorio.GuardarCambios();
                }
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                return oResultado;
            }
            return oResultado;
        }
        #endregion

        #region Modificacion de Contrato
        public Resultado ModificacionContrato(ControlDeBoletosModificacionContratoDto dto)
        {
            var resultado = new Resultado();

            try
            {
                var negocio = repositorio.Obtener<Negocio>(x => x.Id == dto.NegocioId);

                if (negocio == null)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Message = "No se encontró el negocio."
                    });
                    return resultado;
                }

                var provincia = repositorio.Obtener<Provincia>(dto.ProvinciaId);
                var campana = repositorio.Obtener<Campaña>(dto.CosechaId);
                var procedencia = repositorio.Obtener<Localidad>(dto.ProcedenciaId);
                var clasificacion = repositorio.Obtener<ClasificacionCompraNet>(dto.ClasificacionId);

                negocio.Provincia = provincia;
                negocio.Campana = campana;
                negocio.ProcedenciaVenta = procedencia;
                negocio.Clasificacion = clasificacion;

                repositorio.GuardarCambios();

                #region Modificar Contrato en SAP
                var controlDeBoletosModificarContrato = new ControlDeBoletosModificarContratoDto();
                controlDeBoletosModificarContrato.Cosecha = campana.Descripcion;
                controlDeBoletosModificarContrato.Contrato = negocio.ContratoSAP;
                controlDeBoletosModificarContrato.Clasificacion = clasificacion.Descripcion;
                controlDeBoletosModificarContrato.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                controlDeBoletosModificarContrato.Hora = DateTime.Now.ToString("HH:mm:ss");
                controlDeBoletosModificarContrato.Procedencia = procedencia.CodLocalidad;
                controlDeBoletosModificarContrato.Provincia = provincia.ProvinciaId.ToString();
                controlDeBoletosModificarContrato.Usuario = dto.Usuario;
                modificacionContratoControlBoletoAgent.ModificarContrato(controlDeBoletosModificarContrato);
                #endregion

            }
            catch (Exception ex)
            {
                resultado.Errores.Add(new ErrorMessage
                {
                    Message = "Ocurrió un error al modificar el contrato."
                });

                logger.Error(ex);
            }

            return resultado;
        }
        #endregion

        #region Datos de Seguimiento
        public ControlDeBoletosDatosSeguimientoDto ObtenerDatosDeSeguimiento(int datosSeguimientoId)
        {
            var seguimientoControlDeBoletos = new ControlDeBoletosDatosSeguimientoDto();
            var controlDeBoletosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.Id == datosSeguimientoId);
            if (controlDeBoletosSeguimiento != null)
            {
                seguimientoControlDeBoletos.Id = controlDeBoletosSeguimiento.Id;
                seguimientoControlDeBoletos.ControlDeBoletosId = controlDeBoletosSeguimiento.ControlDeBoletosId;
                seguimientoControlDeBoletos.BoletoCompraNetId = controlDeBoletosSeguimiento.BoletoCompraNetId;
                seguimientoControlDeBoletos.BolsaCompraNetId = controlDeBoletosSeguimiento.BolsaCompraNetId;
                seguimientoControlDeBoletos.BolsaSellado = controlDeBoletosSeguimiento.BolsaSellado;
                seguimientoControlDeBoletos.FechaAcopio = controlDeBoletosSeguimiento.FechaAcopio;
                seguimientoControlDeBoletos.FechaEnviadoFirma = controlDeBoletosSeguimiento.FechaEnviadoFirma;
                seguimientoControlDeBoletos.FechaEnvio = controlDeBoletosSeguimiento.FechaEnvio;
                seguimientoControlDeBoletos.FechaEnvioAfip = controlDeBoletosSeguimiento.FechaEnvioAfip;
                seguimientoControlDeBoletos.FechaEnvioBolsa = controlDeBoletosSeguimiento.FechaEnvioBolsa;
                seguimientoControlDeBoletos.FechaRecepBoleto = controlDeBoletosSeguimiento.FechaRecepBoleto;
                seguimientoControlDeBoletos.FechaRecibFirma = controlDeBoletosSeguimiento.FechaRecibFirma;
                seguimientoControlDeBoletos.FechaVueltaAfip = controlDeBoletosSeguimiento.FechaVueltaAfip;
                seguimientoControlDeBoletos.FechaVueltaBolsa = controlDeBoletosSeguimiento.FechaVueltaBolsa;
                seguimientoControlDeBoletos.ObsCtrlBoleto = controlDeBoletosSeguimiento.ObsCtrlBoleto;
                seguimientoControlDeBoletos.ObsCtrlBoleto2 = controlDeBoletosSeguimiento.ObsCtrlBoleto2;
            }
            return seguimientoControlDeBoletos;
        }
        public Resultado RegistroDatosDeSeguimiento(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var oResultado = new Resultado();
            try
            {
                if (controlDeBoletosDatosSeguimiento.Id > 0)
                {
                    var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(controlDeBoletosDatosSeguimiento.Id);
                    datosSeguimiento.BoletoCompraNet = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId);
                    datosSeguimiento.BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId);
                    datosSeguimiento.BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado;

                    datosSeguimiento.FechaAcopio = controlDeBoletosDatosSeguimiento.FechaAcopio;
                    datosSeguimiento.FechaEnviadoFirma = controlDeBoletosDatosSeguimiento.FechaEnviadoFirma;
                    datosSeguimiento.FechaEnvio = controlDeBoletosDatosSeguimiento.FechaEnvio;
                    datosSeguimiento.FechaEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip;
                    datosSeguimiento.FechaEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa;
                    datosSeguimiento.FechaRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepBoleto;
                    datosSeguimiento.FechaRecibFirma = controlDeBoletosDatosSeguimiento.FechaRecibFirma;
                    datosSeguimiento.FechaVueltaAfip = controlDeBoletosDatosSeguimiento.FechaVueltaAfip;
                    datosSeguimiento.FechaVueltaBolsa = controlDeBoletosDatosSeguimiento.FechaVueltaBolsa;
                    datosSeguimiento.ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto;
                    datosSeguimiento.ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2;
                    datosSeguimiento.FechaModificacion = DateTime.Now;
                    repositorio.GuardarCambios();
                }
                else
                {
                    var datosSeguimiento = new ControlDeBoletosSeguimiento()
                    {
                        ControlDeBoletosId = controlDeBoletosDatosSeguimiento.ControlDeBoletosId,
                        BoletoCompraNet = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId),
                        BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId),
                        BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado,
                        FechaAcopio = controlDeBoletosDatosSeguimiento.FechaAcopio,
                        FechaEnviadoFirma = controlDeBoletosDatosSeguimiento.FechaEnviadoFirma,
                        FechaEnvio = controlDeBoletosDatosSeguimiento.FechaEnvio,
                        FechaEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip,
                        FechaEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa,
                        FechaRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepBoleto,
                        FechaRecibFirma = controlDeBoletosDatosSeguimiento.FechaRecibFirma,
                        FechaVueltaAfip = controlDeBoletosDatosSeguimiento.FechaVueltaAfip,
                        FechaVueltaBolsa = controlDeBoletosDatosSeguimiento.FechaVueltaBolsa,
                        FechaCreacion = DateTime.Now,
                        ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto,
                        ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2,
                    };
                    repositorio.Agregar(datosSeguimiento);
                    repositorio.GuardarCambios();
                    RegistrarAcciones(new List<int> { datosSeguimiento.Id }, EnumControlDeBoletosAcciones.CertificacionCompletada);

                }

                #region Registro de Datos para Seguimiento
                var seguimientoControlDeBoletos = new SeguimientoControlDeBoletosDto();
                seguimientoControlDeBoletos.Bolsa = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId).CodigoSap;
                seguimientoControlDeBoletos.BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado;
                seguimientoControlDeBoletos.Contrato = repositorio.Obtener<Negocio>(repositorio.Obtener<ControlDeBoletos>(controlDeBoletosDatosSeguimiento.ControlDeBoletosId).NegocioId).ContratoSAP;
                seguimientoControlDeBoletos.FeEnviadoFirma = controlDeBoletosDatosSeguimiento.FechaEnviadoFirma?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeEnvio = controlDeBoletosDatosSeguimiento.FechaEnvio?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepBoleto?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeRecibFirma = controlDeBoletosDatosSeguimiento.FechaRecibFirma?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeVueltaAfip = controlDeBoletosDatosSeguimiento.FechaVueltaAfip?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FeVueltaBolsa = controlDeBoletosDatosSeguimiento.FechaVueltaBolsa?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.FecAcopio = controlDeBoletosDatosSeguimiento.FechaAcopio?.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                seguimientoControlDeBoletos.Hora = DateTime.Now.ToString("HH:mm:ss");
                seguimientoControlDeBoletos.ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto;
                seguimientoControlDeBoletos.ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2;
                seguimientoControlDeBoletos.TipoBoleto = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId).Id.ToString("D2");
                seguimientoControlDeBoletos.Usuario = string.Empty;
                seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);
                #endregion

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        #endregion

        #region Datos de PreCertificacion
        public ControlDeBoletosPreCertificacionDto ObtenerDatosPreCertificacion(int datosPreCertificacionId)
        {
            var datosPreCertificacionDto = new ControlDeBoletosPreCertificacionDto();

            var preCertificacion = repositorio.Obtener<ControlDeBoletosPreCertificacion>(x => x.Id == datosPreCertificacionId);

            datosPreCertificacionDto.Id = preCertificacion.Id;
            datosPreCertificacionDto.ControlDeBoletosId = preCertificacion.ControlDeBoletosId;
            datosPreCertificacionDto.FechaCertificacion = preCertificacion.FechaCertificacion;
            datosPreCertificacionDto.FechaVencimiento = preCertificacion.FechaVencimiento;
            datosPreCertificacionDto.BolsaCompraNetId = preCertificacion.BolsaCompraNetId;
            datosPreCertificacionDto.Oblea = preCertificacion.Oblea;
            datosPreCertificacionDto.TipoObleaId = preCertificacion.TipoObleaId;
            return datosPreCertificacionDto;
        }
        public Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion)
        {
            var oResultado = new Resultado();
            try
            {
                if (controlDeBoletosPreCertificacion.Id > 0)
                {
                    var preCertificacion = repositorio.Obtener<ControlDeBoletosPreCertificacion>(controlDeBoletosPreCertificacion.Id);
                    if (preCertificacion != null)
                    {
                        preCertificacion.FechaCertificacion = controlDeBoletosPreCertificacion.FechaCertificacion;
                        preCertificacion.FechaVencimiento = controlDeBoletosPreCertificacion.FechaVencimiento;
                        preCertificacion.BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosPreCertificacion.BolsaCompraNetId);
                        preCertificacion.Oblea = controlDeBoletosPreCertificacion.Oblea;
                        preCertificacion.TipoOblea = repositorio.Obtener<TipoOblea>(controlDeBoletosPreCertificacion.TipoObleaId);
                        preCertificacion.FechaModificacion = DateTime.Now;
                        repositorio.GuardarCambios();

                    }
                }
                else
                {
                    var preCertificacion = new ControlDeBoletosPreCertificacion()
                    {
                        ControlDeBoletosId = controlDeBoletosPreCertificacion.ControlDeBoletosId,
                        FechaCertificacion = controlDeBoletosPreCertificacion.FechaCertificacion,
                        FechaVencimiento = controlDeBoletosPreCertificacion.FechaVencimiento,
                        BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosPreCertificacion.BolsaCompraNetId),
                        Oblea = controlDeBoletosPreCertificacion.Oblea,
                        TipoOblea = repositorio.Obtener<TipoOblea>(controlDeBoletosPreCertificacion.TipoObleaId),
                        FechaCreacion = DateTime.Now
                    };
                    repositorio.Agregar(preCertificacion);
                    repositorio.GuardarCambios();
                    RegistrarAcciones(new List<int> { preCertificacion.Id}, EnumControlDeBoletosAcciones.RegistroDatosOblea);
                }

                #region Registro de Datos para Certificación
                var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto();
                var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto();
                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(x => x.Id == controlDeBoletosPreCertificacion.ControlDeBoletosId);
                
                datosCertificacionCabeceraDto.Contrato = repositorio.Obtener<Negocio>(x => x.Id == controlDeBoletos.NegocioId).ContratoSAP;
                datosCertificacionCabeceraDto.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                datosCertificacionCabeceraDto.Hora = DateTime.Now.ToString("HH:mm:ss");
                datosCertificacionCabeceraDto.Fijacion = string.Empty;
                datosCertificacionCabeceraDto.Usuario = string.Empty;

                datosCertificacionDetalleDto.Bolsa = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosPreCertificacion.BolsaCompraNetId).CodigoSap;
                datosCertificacionDetalleDto.Oblea = controlDeBoletosPreCertificacion.Oblea;
                datosCertificacionDetalleDto.FeCertificacion = controlDeBoletosPreCertificacion.FechaCertificacion.ToString("yyyy-MM-dd");
                datosCertificacionDetalleDto.FeVencCerti = controlDeBoletosPreCertificacion.FechaVencimiento.ToString("yyyy-MM-dd");
                datosCertificacionDetalleDto.Rechazado = string.Empty;
                datosCertificacionDetalleDto.Tipo = repositorio.Obtener<TipoOblea>(controlDeBoletosPreCertificacion.TipoObleaId).Codigo;
                datosCertificacionCabeceraDto.Detalle = new List<RegistroDatosCertificacionControlDeBoletosDetalleDto>() { datosCertificacionDetalleDto };
                this.datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
                #endregion

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        #endregion

        #region Tracking de Boletos
        public List<ControlDeBoletoTrackingDto> ObtenerTrackingBoletos(int controlDeBoletosId)
        {
            var tracking = repositorio.Listar<ControlDeBoletoTracking>(x => x.ControlDeBoletosId == controlDeBoletosId);
            return tracking.Select(x => new ControlDeBoletoTrackingDto
            {
                Id = x.Id,
                ControlDeBoletosId = x.ControlDeBoletosId,
                Accion = x.Accion,
                Resultado = x.Resultado,
                ValorAnterior = x.ValorAnterior,
                ValorNuevo = x.ValorNuevo,
                Cargo = x.Cargo,
                Nombre = x.Nombre,
                Apellido = x.Apellido,
                FechaHora = x.FechaHora,
                UsuarioModificacion = x.UsuarioModificacion
            }).ToList();
        }
        #endregion

        #region Metodo Tareas Programadas
        public Resultado ProcesarBoletosPendientesControl()
        {
            var oResultado = new Resultado();
            try
            {
                var boletosPendientes = repositorio.Listar<ControlDeBoletos>(x => x.EstadoConfirmaId == (int)EnumEstadoConfirma.PENDIENTE);
                if (boletosPendientes.Count > 0)
                {
                    foreach(var boleto in boletosPendientes)
                    {
                        var bolsaConfirma = Convert.ToInt32(boleto.Negocio.Bolsa.CodigoConfirma);
                        var respuestaConsultaDocumentos = confirmaConsultaDocumentosAgent.ConsultaDocumentos(bolsaConfirma, boleto.Negocio.ContratoSAP);
                        if (respuestaConsultaDocumentos!=null && respuestaConsultaDocumentos.Count > 0)
                        {
                            foreach (var documento in respuestaConsultaDocumentos)
                            {
                                foreach(var accion in documento.Acciones)
                                {

                                    var existe = this.repositorio.Obtener<ControlDeBoletoTracking>(x=> x.ControlDeBoletosId == boleto.Id && x.Accion == accion.Accion && x.Resultado == x.Resultado && x.FechaHora == x.FechaHora);
                                    if (existe == null)
                                    {
                                        var controlDeBoletoTracking = new ControlDeBoletoTracking()
                                        {
                                            ControlDeBoletosId = boleto.Id,
                                            Accion = accion.Accion,
                                            Resultado = accion.Resultado,
                                            ValorAnterior = string.Empty,
                                            ValorNuevo = string.Empty,
                                            Cargo = accion.Cargo,
                                            Nombre = accion.Nombre,
                                            Apellido = accion.Apellido,
                                            FechaHora = DateTime.Now,
                                            UsuarioModificacion = string.Format("{0} {1}", accion.Nombre, accion.Apellido)
                                        };
                                        this.repositorio.Agregar(controlDeBoletoTracking);
                                    }

                                }
                            }
                            this.repositorio.GuardarCambios();
                        }
                    }
                }
                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage()
                {
                    Message = ex.Message,
                });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        #endregion

    }
}
