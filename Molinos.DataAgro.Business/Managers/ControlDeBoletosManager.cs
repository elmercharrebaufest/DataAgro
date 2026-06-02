using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
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
        private readonly IAltaTempranaAgent altaTempranaAgent;
        private readonly ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent;
        private readonly IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent;
        private readonly IDatosCertificacionControlBoletoAgent datosCertificacionControlBoletoAgent;
        private readonly ILogDataAgroManager logDataAgroManager;
        public ControlDeBoletosManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager, 
                                       IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent, 
                                       ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent,
                                       IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent,
                                       IDatosCertificacionControlBoletoAgent datosCertificacionControlBoletoAgent,
                                       IAltaTempranaAgent altaTempranaAgent,
                                       ILogDataAgroManager logDataAgroManager
                                       )
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.modificacionContratoControlBoletoAgent = modificacionContratoControlBoletoAgent;
            this.seguimientoControlBoletoAgent = seguimientoControlBoletoAgent;
            this.confirmaConsultaDocumentosAgent = confirmaConsultaDocumentosAgent;
            this.datosCertificacionControlBoletoAgent = datosCertificacionControlBoletoAgent;
            this.altaTempranaAgent = altaTempranaAgent;
            this.logDataAgroManager = logDataAgroManager;
        }

        #region Reporte Seguimiento Boletos

        public List<ControlDeBoletosReporteSeguimientoConsultaDto> GetReporteDeSeguimientoBoletos(ControlDeBoletoFiltroSeguimientoDto filtros)
        {
            var controlBoletosQuery = repositorio.Listar<ControlDeBoletos>().AsQueryable();

            // FILTROS GENERALES
            if (!string.IsNullOrWhiteSpace(filtros.ContratoSAP))
            {
                var contratosList = filtros.ContratoSAP
                    .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();
                if (contratosList.Count > 0)
                    controlBoletosQuery = controlBoletosQuery.Where(cb => contratosList.Contains(cb.Negocio.ContratoSAP));
            }
            else
            {
                if (filtros.MaterialId.HasValue)
                    controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.MaterialId == filtros.MaterialId.Value);

                if (filtros.Proveedor.HasValue)
                    controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.ProveedorId == filtros.Proveedor.Value);

                if (filtros.BolsaId.HasValue)
                    controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.BolsaId == filtros.BolsaId.Value);
            }

            // Query principal con LEFT JOINs
            var query = from cb in controlBoletosQuery
                        join pre in repositorio.Listar<ControlDeBoletosPreCertificacion>()
                            on cb.Id equals pre.ControlDeBoletosId into preJoin
                        from pre in preJoin.DefaultIfEmpty()
                        join seg in repositorio.Listar<ControlDeBoletosSeguimiento>()
                            on cb.Id equals seg.ControlDeBoletosId into segJoin
                        from seg in segJoin.DefaultIfEmpty()
                        select new { cb, pre, seg };

            if (string.IsNullOrWhiteSpace(filtros.ContratoSAP))
            {
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
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionBoleto.HasValue && x.seg.FechaRecepcionBoleto.Value >= filtros.FechaRecepBoletoDesde.Value);

                if (filtros.FechaRecepBoletoHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionBoleto.HasValue && x.seg.FechaRecepcionBoleto.Value <= filtros.FechaRecepBoletoHasta.Value);

                if (filtros.FechaEnviadoFirmaDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioFirmas.HasValue && x.seg.FechaEnvioFirmas.Value >= filtros.FechaEnviadoFirmaDesde.Value);

                if (filtros.FechaEnviadoFirmaHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioFirmas.HasValue && x.seg.FechaEnvioFirmas.Value <= filtros.FechaEnviadoFirmaHasta.Value);

                if (filtros.FechaRecibFirmaDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionFirma.HasValue && x.seg.FechaRecepcionFirma.Value >= filtros.FechaRecibFirmaDesde.Value);

                if (filtros.FechaRecibFirmaHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionFirma.HasValue && x.seg.FechaRecepcionFirma.Value <= filtros.FechaRecibFirmaHasta.Value);

                if (filtros.FechaEnvioBolsaDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioBolsa.HasValue && x.seg.FechaEnvioBolsa.Value >= filtros.FechaEnvioBolsaDesde.Value);

                if (filtros.FechaEnvioBolsaHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioBolsa.HasValue && x.seg.FechaEnvioBolsa.Value <= filtros.FechaEnvioBolsaHasta.Value);

                if (filtros.FechaVueltaBolsaDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionBolsa.HasValue && x.seg.FechaRecepcionBolsa.Value >= filtros.FechaVueltaBolsaDesde.Value);

                if (filtros.FechaVueltaBolsaHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionBolsa.HasValue && x.seg.FechaRecepcionBolsa.Value <= filtros.FechaVueltaBolsaHasta.Value);

                if (filtros.FechaEnvioAfipDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioAfip.HasValue && x.seg.FechaEnvioAfip.Value >= filtros.FechaEnvioAfipDesde.Value);

                if (filtros.FechaEnvioAfipHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioAfip.HasValue && x.seg.FechaEnvioAfip.Value <= filtros.FechaEnvioAfipHasta.Value);

                if (filtros.FechaVueltaAfipDesde.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionAfip.HasValue && x.seg.FechaRecepcionAfip.Value >= filtros.FechaVueltaAfipDesde.Value);

                if (filtros.FechaVueltaAfipHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaRecepcionAfip.HasValue && x.seg.FechaRecepcionAfip.Value <= filtros.FechaVueltaAfipHasta.Value);
            }

            // PROYECCIÓN — sin Skip/Take: la paginación la aplica el controller (igual que GetControlBoletosPendientes)
            var data = query
                .OrderBy(x => x.cb.Id)
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
                    BolsaCompraNet = x.cb.Negocio.Bolsa != null ? x.cb.Negocio.Bolsa.Descripcion : null,
                    ComercialId = x.cb.Negocio.ComercialId ?? 0,
                    Comercial = x.cb.Negocio.Comercial != null ? x.cb.Negocio.Comercial.Nombres + " " + x.cb.Negocio.Comercial.Apellido : null,
                    TipoBoleto = x.cb.Negocio.Boleto != null ? x.cb.Negocio.Boleto.Descripcion : null,
                    ContratoSAP = x.cb.Negocio.ContratoSAP,
                    ProveedorId = x.cb.Negocio.ProveedorId ?? 0,
                    Proveedor = x.cb.Negocio.Proveedor != null ? x.cb.Negocio.Proveedor.RazonSocial : null,
                    PreCertificacionId = x.pre != null ? (int?)x.pre.Id : null,
                    SeguimientoBoletoId = x.seg != null ? (int?)x.seg.Id : null,
                    FechaCertificacion = x.pre != null ? (DateTime?)x.pre.FechaCertificacion : null,
                    FechaVencimientoCertificacion = x.pre != null ? (DateTime?)x.pre.FechaVencimiento : null,
                    FechaEnviadoFirma = x.seg != null ? x.seg.FechaEnvioFirmas : null,
                    FechaEnvio = null,
                    FechaEnvioAfip = x.seg != null ? x.seg.FechaEnvioAfip : null,
                    FechaEnvioBolsa = x.seg != null ? x.seg.FechaEnvioBolsa : null,
                    FechaRecepBoleto = x.seg != null ? x.seg.FechaRecepcionBoleto : null,
                    FechaRecibFirma = x.seg != null ? x.seg.FechaRecepcionFirma : null,
                    FechaVueltaAfip = x.seg != null ? x.seg.FechaRecepcionAfip : null,
                    FechaVueltaBolsa = x.seg != null ? x.seg.FechaRecepcionBolsa : null,
                    FechaAcopio = null
                })
                .ToList();

            return data;
        }

        #endregion

        #region Metodo para cargar combos
        private static string FormatDate(DateTime? dt)
        {
            if (!dt.HasValue) return string.Empty;
            return dt.Value.ToString("yyyy-MM-dd");
        }
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

            // Limitar a negocios confirmados en SAP en los últimos 2 meses (consulta optimizada con ReadUncommitted)
            var negociosIds = repositorio.ObtenerConsultaEscalar(new TraerNegociosPendientesControlBoleto());

            if (negociosIds != null && negociosIds.Count > 0)
            {
                var negociosIdsEnQuery = query
                    .Select(cb => cb.NegocioId)
                    .Distinct()
                    .ToList();
                var negociosNoEnQuery = negociosIds
                    .Where(id => !negociosIdsEnQuery.Contains(id))
                    .ToList();

                if (negociosNoEnQuery != null && negociosNoEnQuery.Count > 0)
                {
                    var controlBoletosNuevos = negociosNoEnQuery.Select(id => new ControlDeBoletos
                    {
                        NegocioId = id,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = false,
                        AltaIdLoteConfirma = null,
                        AltaIdDocumentoConfirma = null,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (int?)null,
                        ControlIniciado = false,
                        ControlFinalizado = false,
                        CertificacionCompletada = false,
                        RegistroDatosOblea = false,
                        EsConfirmaAltaBorrador = false
                    }).ToList();
                    repositorio.AgregarTodos(controlBoletosNuevos);
                    repositorio.GuardarCambios();
                }
            }

            if (!string.IsNullOrWhiteSpace(filtros.NegocioSAP))
            {
                var negociosSAPList = filtros.NegocioSAP.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .ToList();

                if (negociosSAPList.Count > 0)
                    query = query.Where(n => negociosSAPList.Contains(n.Negocio.ContratoSAP));
            }
            else
            {
                if (filtros.MaterialId.HasValue)
                    query = query.Where(x => x.Negocio.MaterialId == filtros.MaterialId.Value);

                if (filtros.EstadoControlId.HasValue)
                    query = query.Where(x => x.ControlDeBoletosEstadoId == filtros.EstadoControlId.Value);

                if (filtros.EsConfirma)
                    query = query.Where(x => x.EsConfirma == true);

                if (filtros.FechaCargaDesde.HasValue)
                    query = query.Where(x => x.FechaCreacion >= filtros.FechaCargaDesde.Value);

                if (filtros.FechaCargaHasta.HasValue)
                    query = query.Where(x => x.FechaCreacion < filtros.FechaCargaHasta.Value.Date.AddDays(1));

                if (filtros.Proveedor.HasValue)
                    query = query.Where(x => x.Negocio.ProveedorId == filtros.Proveedor.Value);

                if (filtros.BolsaId.HasValue)
                    query = query.Where(x => x.Negocio.BolsaId == filtros.BolsaId.Value);

                if (filtros.ComercialId.HasValue)
                    query = query.Where(x => x.Negocio.ComercialId == filtros.ComercialId.Value);
            }



            // Optimización: Usar query LINQ con LEFT JOIN en lugar de N+1 queries
            var result = (from cb in query
                          join preOblea in repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.TipoOblea.Codigo == "O")
                              on cb.Id equals preOblea.ControlDeBoletosId into preJoinOblea
                          from preOblea in preJoinOblea.DefaultIfEmpty()

                          join prePlanCanje in repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.TipoOblea.Codigo == "F")
                              on cb.Id equals prePlanCanje.ControlDeBoletosId into preJoinPlanCanje
                          from prePlanCanje in preJoinPlanCanje.DefaultIfEmpty()

                          join preAfip in repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.TipoOblea.Codigo == "A")
                              on cb.Id equals preAfip.ControlDeBoletosId into preJoinAfip
                          from preAfip in preJoinAfip.DefaultIfEmpty()

                          join preProvisoria in repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.TipoOblea.Codigo == "P")
                              on cb.Id equals preProvisoria.ControlDeBoletosId into preJoinProvisoria
                          from preProvisoria in preJoinProvisoria.DefaultIfEmpty()

                          join seg in repositorio.Listar<ControlDeBoletosSeguimiento>()
                              on cb.Id equals seg.ControlDeBoletosId into segJoin
                          from seg in segJoin.DefaultIfEmpty()
                          select new ControlDeBoletosConsultaDto
                          {
                              Id = cb.Id,
                              NegocioId = cb.NegocioId,
                              ControlDeBoletosEstadoId = cb.ControlDeBoletosEstadoId,
                              ControlDeBoletosEstado = cb.ControlDeBoletosEstado.Descripcion,
                              EsConfirma = cb.EsConfirma,
                              AltaIdLoteConfirma = cb.AltaIdLoteConfirma,
                              IdentificadorConfirma = cb.IdentificadorConfirma,
                              FechaCreacion = cb.FechaCreacion,
                              FechaModificacion = cb.FechaModificacion,
                              EstadoConfirmaId = cb.EstadoConfirmaId,
                              EstadoConfirma = cb.EstadoConfirmaId.HasValue ? repositorio.Obtener<EstadoConfirma>(cb.EstadoConfirmaId.Value).Descripcion : null,
                              TipoBoleto = cb.Negocio.Boleto.Descripcion,
                              ControlIniciado = cb.ControlIniciado,
                              ControlFinalizado = cb.ControlFinalizado,
                              CertificacionCompletada = cb.CertificacionCompletada,
                              RegistroDatosOblea = cb.RegistroDatosOblea,
                              FechaControlIniciado = cb.FechaControlIniciado,
                              FechaControlFinalizado = cb.FechaControlFinalizado,
                              FechaCertificacionCompletada = cb.FechaCertificacionCompletada,
                              FechaRegistroDatosOblea = cb.FechaRegistroDatosOblea,
                              MaterialId = cb.Negocio.MaterialId,
                              Material = cb.Negocio.Material.Descripcion,
                              BolsaCompraNetId = cb.Negocio.BolsaId != null ? (int?)cb.Negocio.BolsaId : null,
                              BolsaCompraNet = cb.Negocio.Bolsa != null ? cb.Negocio.Bolsa.Descripcion : null,
                              ComercialId = cb.Negocio.ComercialId,
                              Comercial = cb.Negocio.Comercial.Nombres + " " + cb.Negocio.Comercial.Apellido,
                              ContratoSAP = cb.Negocio.ContratoSAP,
                              ProveedorId = cb.Negocio.ProveedorId,
                              Proveedor = cb.Negocio.Proveedor.RazonSocial,
                              SeguimientoBoletoId = seg != null ? (int?)seg.Id : null,
                              TipoAltaConfirma = cb.EsConfirma? (cb.EsConfirmaAltaBorrador? "Alta Borrador" : "Alta Definitiva") : string.Empty,
                          }).ToList();

            return result;
        }
        public Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null, int? altaIdDocumentoConfirma = null, bool? esConfirmaAltaBorrador = false)
        {
            var oResultado = new Resultado();
            var negocio = repositorio.Obtener<Negocio>(negocioId);
            if (negocio != null)
            {
                var controlDeBoletosExiste = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocioId);
                if (controlDeBoletosExiste == null)
                {

                    var controlDeBoletos = new ControlDeBoletos()
                    {
                        NegocioId = negocioId,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA,
                        AltaIdLoteConfirma = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdLoteConfirma : (int?)null,
                        AltaIdDocumentoConfirma = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdDocumentoConfirma : (int?)null,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? (int)EnumEstadoConfirma.PENDIENTE : (int?)null,
                        ControlIniciado = false,
                        ControlFinalizado = false,
                        CertificacionCompletada = false,
                        RegistroDatosOblea = false,
                        EsConfirmaAltaBorrador = (bool)((negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? esConfirmaAltaBorrador : false)
                    };
                    repositorio.Agregar(controlDeBoletos);
                    repositorio.GuardarCambios();
                }
                else
                {
                    controlDeBoletosExiste.FechaAnulacionConfirma = DateTime.Now;
                    controlDeBoletosExiste.EstadoConfirmaId = (int)EnumEstadoConfirma.ANULADO;
                    controlDeBoletosExiste.ControlDeBoletosEstadoId = (int) EnumControlDeBoletosEstado.ANULADO;
                    repositorio.GuardarCambios();

                    var controlDeBoletosSustitutorio = new ControlDeBoletos()
                    {
                        NegocioId = negocioId,
                        FechaCreacion = DateTime.Now,
                        EsConfirma = negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA,
                        AltaIdLoteConfirma = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdLoteConfirma : (int?)null,
                        AltaIdDocumentoConfirma = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? altaIdDocumentoConfirma : (int?)null,
                        ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                        EstadoConfirmaId = (negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? (int)EnumEstadoConfirma.PENDIENTE : (int?)null,
                        ControlIniciado = false,
                        ControlFinalizado = false,
                        CertificacionCompletada = false,
                        RegistroDatosOblea = false,
                        AltaIdLoteConfirmaAnterior = controlDeBoletosExiste.AltaIdLoteConfirma,
                        AltaIdDocumentoConfirmaAnterior = controlDeBoletosExiste.AltaIdDocumentoConfirma,
                        EsConfirmaAltaBorrador = (bool)((negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ? esConfirmaAltaBorrador : false)
                    };
                    repositorio.Agregar(controlDeBoletosSustitutorio);
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
        private void ModificarContratoEnDataAgro(Negocio negocio, Provincia provincia, Campaña campana, Localidad procedencia, ClasificacionCompraNet clasificacion)
        {
            negocio.Provincia = provincia;
            negocio.Campana = campana;
            negocio.ProcedenciaVenta = procedencia;
            negocio.Clasificacion = clasificacion;
            repositorio.GuardarCambios();
            var objetoLog = new
            {
                NegocioId = negocio.Id,
                ContratoSAP = negocio.ContratoSAP,
                Provincia = provincia.Nombre,
                Campana = campana.Descripcion,
                Procedencia = procedencia.Nombre,
                Clasificacion = clasificacion.Descripcion
            };
            this.logDataAgroManager.LogCambiosControlBoletos(objetoLog, TipoAccionLogDataAgro.Modificar, negocio.Id, "Modificacion de Contrato - Control de Boletos");
        }

        public Resultado ModificacionContrato(ControlDeBoletosModificacionContratoDto dto)
        {
            var resultado = new Resultado();

            try
            {
                var negocio = repositorio.Obtener<Negocio>(x => x.Id == dto.NegocioId);

                if (negocio == null)
                {
                    resultado.Errores.Add(new ErrorMessage { Message = "No se encontró el negocio." });
                    return resultado;
                }

                var provincia      = repositorio.Obtener<Provincia>(dto.ProvinciaId);
                var campana        = repositorio.Obtener<Campaña>(dto.CosechaId);
                var procedencia    = repositorio.Obtener<Localidad>(dto.ProcedenciaId);
                var clasificacion  = repositorio.Obtener<ClasificacionCompraNet>(dto.ClasificacionId);
                var ahora          = DateTime.Now;

                var controlDeBoletosModificarContrato = new ControlDeBoletosModificarContratoDto
                {
                    Cosecha       = campana.Descripcion,
                    Contrato      = negocio.ContratoSAP,
                    Clasificacion = clasificacion.Descripcion,
                    Fecha         = ahora.ToString("yyyy-MM-dd"),
                    Hora          = ahora.ToString("HH:mm:ss"),
                    Procedencia   = procedencia.CodLocalidad,
                    Provincia     = provincia.ProvinciaId.ToString(),
                    Usuario       = dto.Usuario
                };

                modificacionContratoControlBoletoAgent.ModificarContrato(controlDeBoletosModificarContrato);
                ModificarContratoEnDataAgro(negocio, provincia, campana, procedencia, clasificacion);
            }
            catch (Exception ex)
            {
                resultado.Errores.Add(new ErrorMessage { Message = "Ocurrió un error al modificar el contrato." });
                logger.Error(ex);
            }

            return resultado;
        }
        public ControlDeBoletosDatosContratoDto ObtenerDatosDeContrato(int negocioId)
        {
            var datosContrato = new ControlDeBoletosDatosContratoDto();
            var contrato = repositorio.Obtener<Negocio>(x => x.Id == negocioId);
            if (contrato != null)
            {
                datosContrato.NegocioId = contrato.Id;
                datosContrato.LocalidadId = contrato.LocalidadId;
                datosContrato.CampanaId = contrato.CampanaId;
                datosContrato.ProvinciaId = contrato.ProvinciaId;
                datosContrato.ClasificacionId = contrato.ClasificacionId;
                datosContrato.ContratoSAP = contrato.ContratoSAP;
                datosContrato.Material = contrato.Material.Descripcion;
                datosContrato.Provincia = contrato.Provincia.Nombre;
                datosContrato.Localidad = contrato.Localidad.Nombre;
                datosContrato.Cantidad = contrato.Cantidad;
                datosContrato.Precio = contrato.Precio;
                datosContrato.Destino = contrato.Destino.Descripcion;
                datosContrato.Clasificacion = contrato.Clasificacion.Descripcion;
                datosContrato.StandarCalidad = contrato.StandardDeCalidad.Descripcion;
                datosContrato.Campana = contrato.Campana.Descripcion;
                datosContrato.TipoBoleto = contrato.Boleto.Descripcion;
                datosContrato.FechaOperacion = contrato.FechaOperacion.ToString("dd/MM/yyyy");
                datosContrato.PeriodoEntrega = contrato.FechaDesde.ToString("dd/MM/yyyy") + " - " + contrato.FechaHasta.ToString("dd/MM/yyyy");
                datosContrato.CuitVendedor = contrato.Proveedor.CUIT;
                datosContrato.CuitCorredor = contrato.CorredorId > 0 ? contrato.Corredor.CUIT : string.Empty;
                datosContrato.Moneda = contrato.Moneda?.MonedaId;
                datosContrato.CorredorId = contrato.CorredorId;
                datosContrato.PlanCanje = contrato.PlanCanje;
            }

            var cuitProveedor = contrato.CorredorId > 0 ? contrato.Corredor.CUIT : contrato.Proveedor.CUIT;
            var tipoProveedor = contrato.CorredorId > 0 ? "CORR" : "PROV";
            var operaSinOblea = VerificarOperaSinOblea(cuitProveedor, tipoProveedor);
            datosContrato.OperaSinOblea = operaSinOblea == "SI";
            return datosContrato;
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
                seguimientoControlDeBoletos.FechaRecepcionBoleto = controlDeBoletosSeguimiento.FechaRecepcionBoleto;

                seguimientoControlDeBoletos.FechaEnvioFirmas = controlDeBoletosSeguimiento.FechaEnvioFirmas;
                seguimientoControlDeBoletos.FechaEnvioBolsa = controlDeBoletosSeguimiento.FechaEnvioBolsa;
                seguimientoControlDeBoletos.FechaEnvioAfip = controlDeBoletosSeguimiento.FechaEnvioAfip;

                seguimientoControlDeBoletos.FechaRecepcionFirma = controlDeBoletosSeguimiento.FechaRecepcionFirma;
                seguimientoControlDeBoletos.FechaRecepcionBolsa = controlDeBoletosSeguimiento.FechaRecepcionBolsa;
                seguimientoControlDeBoletos.FechaRecepcionAfip = controlDeBoletosSeguimiento.FechaRecepcionAfip;

                seguimientoControlDeBoletos.FechaEnvioSellado = controlDeBoletosSeguimiento.FechaEnvioSellado;

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
                // 1. Primero registrar en SAP (sistema externo)
                var resultadoSeguimiento = RegistrarSeguimientoEnSAP(controlDeBoletosDatosSeguimiento);
                if (resultadoSeguimiento.HayError)
                    return resultadoSeguimiento;

                // 2. Luego persistir en la base de datos local
                PersistirDatosDeSeguimientoLocal(controlDeBoletosDatosSeguimiento);

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage() { Message = ex.Message });
                logger.Error(ex.Message);
                return oResultado;
            }
        }

        private Resultado RegistrarSeguimientoEnSAP(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var oResultado = new Resultado();
            try
            {
                var seguimientoControlDeBoletos = ConstruirSeguimientoDto(controlDeBoletosDatosSeguimiento);
                seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage() { Message = "Error al registrar datos de seguimiento en SAP" });
                logger.Error(ex.Message);
            }
            return oResultado;
        }

        private SeguimientoControlDeBoletosDto ConstruirSeguimientoDto(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(controlDeBoletosDatosSeguimiento.ControlDeBoletosId);
            var contrato = repositorio.Obtener<Negocio>(controlDeBoletos.NegocioId).ContratoSAP;
            var bolsa = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId).CodigoSap;
            var tipoBoleto = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId).Id.ToString("D2");
            var ahora = DateTime.Now;

            return new SeguimientoControlDeBoletosDto
            {
                Bolsa = bolsa,
                BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado,
                Contrato = contrato,
                FeEnvio = string.Empty,
                FecAcopio = string.Empty,
                Fecha = ahora.ToString("yyyy-MM-dd"),
                Hora = ahora.ToString("HH:mm:ss"),
                TipoBoleto = tipoBoleto,
                Usuario = string.Empty,
                FeRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto?.ToString("yyyy-MM-dd"),
                FeEnviadoFirma = controlDeBoletosDatosSeguimiento.FechaEnvioFirmas?.ToString("yyyy-MM-dd"),
                FeEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip?.ToString("yyyy-MM-dd"),
                FeEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa?.ToString("yyyy-MM-dd"),
                FeRecibFirma = controlDeBoletosDatosSeguimiento.FechaRecepcionFirma?.ToString("yyyy-MM-dd"),
                FeVueltaAfip = controlDeBoletosDatosSeguimiento.FechaRecepcionAfip?.ToString("yyyy-MM-dd"),
                FeVueltaBolsa = controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa?.ToString("yyyy-MM-dd"),
                ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto,
                ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2,
            };
        }

        private void PersistirDatosDeSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            if (controlDeBoletosDatosSeguimiento.Id > 0)
            {
                ActualizarSeguimientoLocal(controlDeBoletosDatosSeguimiento);
            }
            else
            {
                InsertarSeguimientoLocal(controlDeBoletosDatosSeguimiento);
            }
        }

        private void ActualizarSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(controlDeBoletosDatosSeguimiento.Id);
            datosSeguimiento.BoletoCompraNet = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId);
            datosSeguimiento.BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId);
            datosSeguimiento.BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado;
            datosSeguimiento.FechaRecepcionBoleto = controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto;
            datosSeguimiento.FechaEnvioFirmas = controlDeBoletosDatosSeguimiento.FechaEnvioFirmas;
            datosSeguimiento.FechaEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa;
            datosSeguimiento.FechaEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip;
            datosSeguimiento.FechaRecepcionFirma = controlDeBoletosDatosSeguimiento.FechaRecepcionFirma;
            datosSeguimiento.FechaRecepcionBolsa = controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa;
            datosSeguimiento.FechaRecepcionAfip = controlDeBoletosDatosSeguimiento.FechaRecepcionAfip;
            datosSeguimiento.FechaEnvioSellado = controlDeBoletosDatosSeguimiento.FechaEnvioSellado;
            datosSeguimiento.ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto;
            datosSeguimiento.ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2;
            datosSeguimiento.FechaModificacion = DateTime.Now;
            repositorio.GuardarCambios();
            this.logDataAgroManager.LogCambiosControlBoletos(controlDeBoletosDatosSeguimiento, TipoAccionLogDataAgro.Modificar, datosSeguimiento.Id, "Modificacion de Seguimiento - Control de Boletos");

        }

        private void InsertarSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var datosSeguimiento = new ControlDeBoletosSeguimiento
            {
                ControlDeBoletosId = controlDeBoletosDatosSeguimiento.ControlDeBoletosId,
                BoletoCompraNet = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId),
                BolsaCompraNet = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId),
                BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado,
                FechaRecepcionBoleto = controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto,
                FechaEnvioFirmas = controlDeBoletosDatosSeguimiento.FechaEnvioFirmas,
                FechaEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa,
                FechaEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip,
                FechaRecepcionFirma = controlDeBoletosDatosSeguimiento.FechaRecepcionFirma,
                FechaRecepcionBolsa = controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa,
                FechaRecepcionAfip = controlDeBoletosDatosSeguimiento.FechaRecepcionAfip,
                FechaEnvioSellado = controlDeBoletosDatosSeguimiento.FechaEnvioSellado,
                FechaCreacion = DateTime.Now,
                ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto,
                ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2,
            };
            repositorio.Agregar(datosSeguimiento);
            repositorio.GuardarCambios();
            RegistrarAcciones(new List<int> { datosSeguimiento.Id }, EnumControlDeBoletosAcciones.CertificacionCompletada);
            this.logDataAgroManager.LogCambiosControlBoletos(controlDeBoletosDatosSeguimiento, TipoAccionLogDataAgro.Crear, datosSeguimiento.Id, "Registro de Seguimiento - Control de Boletos");
        }
        #endregion

        #region Datos de PreCertificacion
        public string VerificarTipoBoletoyFechaRecepcion(int controlDeBoletosId)
        {
            var resultado = "NO";
            var seguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(s => s.ControlDeBoletosId == controlDeBoletosId);
            if (seguimiento != null)
            {
                if (seguimiento.BoletoCompraNetId > 0 && seguimiento.FechaRecepcionBoleto.HasValue)
                    resultado = "SI";
            }
            return resultado;
        }
        public string VerificarOperaSinOblea(string cuit, string tipoProveedor)
        {
            var resultado = string.Empty;
            var obtenerAlta = this.altaTempranaAgent.ObtenerAlta(cuit, tipoProveedor);
            if (obtenerAlta == null)
                return resultado;
            resultado = obtenerAlta.SinOblea;
            return resultado;
        }
        public ControlDeBoletosPreCertificacionDto ObtenerDatosPreCertificacion(int controlDeBoletosId)
        {
            var listaDatosPreCertificacionDto = new ControlDeBoletosPreCertificacionDto();
            listaDatosPreCertificacionDto.Detalle = new List<ControlDeBoletosDatosPreCertificacionDto>();
            var preCertificacion = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == controlDeBoletosId);
            foreach (var item in preCertificacion)
            {
                var datosPreCertificacionDto = new ControlDeBoletosDatosPreCertificacionDto();
                datosPreCertificacionDto.Id = item.Id;
                datosPreCertificacionDto.ControlDeBoletosId = item.ControlDeBoletosId;
                datosPreCertificacionDto.FechaCertificacion = item.FechaCertificacion;
                datosPreCertificacionDto.FechaVencimiento = item.FechaVencimiento;
                datosPreCertificacionDto.BolsaCompraNetId = item.BolsaCompraNetId;
                datosPreCertificacionDto.Oblea = item.Oblea;
                datosPreCertificacionDto.TipoObleaId = item.TipoObleaId;
                datosPreCertificacionDto.CodigoTipoOblea = item.TipoOblea.Codigo;
                datosPreCertificacionDto.Rechazado = item.Rechazado;
                listaDatosPreCertificacionDto.Detalle.Add(datosPreCertificacionDto);
            }
            return listaDatosPreCertificacionDto;
        }
        public Resultado RegistrarDatosPreCertificacion(ControlDeBoletosPreCertificacionDto controlDeBoletosPreCertificacion)
        {
            var oResultado = new Resultado();
            try
            {
                var controlDeBoletosId = controlDeBoletosPreCertificacion.ControlDeBoletosId;

                // Códigos de tipo oblea presentes en el request
                var codigosEnRequest = controlDeBoletosPreCertificacion.Detalle!=null ? controlDeBoletosPreCertificacion.Detalle
                    .Select(d => d.CodigoTipoOblea)
                    .ToHashSet() : new HashSet<string>();

                // Registros actuales en BD para este control (con TipoOblea cargado)
                var registrosEnBd = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    new List<Expression<Func<ControlDeBoletosPreCertificacion, object>>> { r => r.TipoOblea },
                    x => x.ControlDeBoletosId == controlDeBoletosId);

                var ahora = DateTime.Now;

                // ── 1. Upsert: crear o actualizar los registros que vienen en el request ──
                if (controlDeBoletosPreCertificacion.Detalle != null)
                {
                    foreach (var item in controlDeBoletosPreCertificacion.Detalle)
                    {
                        var tipoOblea = repositorio.Listar<TipoOblea>(t => t.Codigo == item.CodigoTipoOblea).FirstOrDefault();
                        if (tipoOblea == null) continue;

                        var existente = registrosEnBd.FirstOrDefault(r => r.TipoOblea != null && r.TipoOblea.Codigo == item.CodigoTipoOblea);

                        if (existente != null)
                        {
                            // Modificar
                            existente.Oblea = item.Oblea;
                            existente.FechaCertificacion = item.FechaCertificacion ?? existente.FechaCertificacion;
                            existente.FechaVencimiento = item.FechaVencimiento ?? existente.FechaVencimiento;
                            existente.BolsaCompraNetId = item.BolsaCompraNetId ?? existente.BolsaCompraNetId;
                            existente.TipoObleaId = tipoOblea.Id;
                            existente.Rechazado = item.Rechazado ?? existente.Rechazado;
                            existente.FechaModificacion = ahora;
                            repositorio.GuardarCambios();
                            RegistrarAcciones(new List<int> { existente.Id }, EnumControlDeBoletosAcciones.RegistroDatosOblea);
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Modificar, existente.Id, "Modificacion de Certificacion - Control de Boletos");

                        }
                        else
                        {
                            // Crear
                            var nuevo = new ControlDeBoletosPreCertificacion
                            {
                                ControlDeBoletosId = item.ControlDeBoletosId,
                                Oblea = item.Oblea,
                                TipoObleaId = tipoOblea.Id,
                                BolsaCompraNetId = item.BolsaCompraNetId,
                                FechaCertificacion = item.FechaCertificacion,
                                FechaVencimiento = item.FechaVencimiento,
                                Rechazado = item.Rechazado,
                                FechaCreacion = ahora
                            };
                            repositorio.Agregar(nuevo);
                            repositorio.GuardarCambios();
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Crear, nuevo.Id, "Registro de Certificacion - Control de Boletos");
                            RegistrarAcciones(new List<int> { nuevo.Id }, EnumControlDeBoletosAcciones.RegistroDatosOblea);
                        }
                    }
                }
                // ── 2. Eliminar registros en BD que ya no están en el request ──
                var registrosAEliminar = registrosEnBd
                    .Where(r => r.TipoOblea != null && !codigosEnRequest.Contains(r.TipoOblea.Codigo))
                    .ToList();

                foreach (var eliminado in registrosAEliminar)
                {
                    // Notificar a SAP que el registro ya no tiene valores (campos vacíos)
                    RegistrarDatosCertificacionParaPreCertificacion(eliminado, oResultado, sinValores: true);
                    repositorio.Remover(eliminado);
                }

                if (registrosAEliminar.Any())
                    repositorio.GuardarCambios();

                // ── 3. Sincronizar con SAP los registros vigentes ──
                var registrosVigentes = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    new List<Expression<Func<ControlDeBoletosPreCertificacion, object>>> { r => r.TipoOblea },
                    x => x.ControlDeBoletosId == controlDeBoletosId);

                foreach (var preCert in registrosVigentes)
                {
                    var ok = RegistrarDatosCertificacionParaPreCertificacion(preCert, oResultado, sinValores: false);
                    if (!ok) return oResultado;
                }

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex.Message);
                return oResultado;
            }
        }
        // Reutilizable: registra en SAP los datos de certificación para un registro de preBolsa-certificación
        private bool RegistrarDatosCertificacionParaPreCertificacion(ControlDeBoletosPreCertificacion controlDeBoletosPreCertificacion, Resultado oResultado, bool sinValores = false)
        {
            try
            {
                var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto();
                var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto();

                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(controlDeBoletosPreCertificacion.ControlDeBoletosId);
                if (controlDeBoletos == null)
                {
                    oResultado.Errores.Add(new ErrorMessage { Message = $"ControlDeBoletos Id {controlDeBoletosPreCertificacion.ControlDeBoletosId} no encontrado." });
                    return false;
                }

                var negocio = repositorio.Obtener<Negocio>(x => x.Id == controlDeBoletos.NegocioId);
                var tipoOblea = repositorio.Obtener<TipoOblea>(controlDeBoletosPreCertificacion.TipoObleaId);

                var ahora = DateTime.Now;
                datosCertificacionCabeceraDto.Contrato = negocio?.ContratoSAP ?? string.Empty;
                datosCertificacionCabeceraDto.Fecha = ahora.ToString("yyyy-MM-dd");
                datosCertificacionCabeceraDto.Hora = ahora.ToString("HH:mm:ss");
                datosCertificacionCabeceraDto.Fijacion = string.Empty;
                datosCertificacionCabeceraDto.Usuario = string.Empty;

                if (sinValores)
                {
                    // Registro eliminado: informar a SAP con todos los campos de detalle vacíos
                    datosCertificacionDetalleDto.Bolsa = string.Empty;
                    datosCertificacionDetalleDto.Oblea = string.Empty;
                    datosCertificacionDetalleDto.FeCertificacion = string.Empty;
                    datosCertificacionDetalleDto.FeVencCerti = string.Empty;
                    datosCertificacionDetalleDto.Rechazado = string.Empty;
                    datosCertificacionDetalleDto.Tipo = tipoOblea?.Codigo ?? string.Empty;
                }
                else
                {
                    var bolsa = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosPreCertificacion.BolsaCompraNetId);
                    datosCertificacionDetalleDto.Bolsa = bolsa?.CodigoSap ?? string.Empty;
                    datosCertificacionDetalleDto.Oblea = controlDeBoletosPreCertificacion.Oblea;
                    datosCertificacionDetalleDto.FeCertificacion = FormatDate(controlDeBoletosPreCertificacion.FechaCertificacion);
                    datosCertificacionDetalleDto.FeVencCerti = FormatDate(controlDeBoletosPreCertificacion.FechaVencimiento);
                    datosCertificacionDetalleDto.Rechazado = string.Empty;
                    datosCertificacionDetalleDto.Tipo = tipoOblea?.Codigo ?? string.Empty;
                }

                datosCertificacionCabeceraDto.Detalle = new List<RegistroDatosCertificacionControlDeBoletosDetalleDto> { datosCertificacionDetalleDto };

                datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
                return true;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = "Error al registrar datos de certificación en SAP" });
                logger.Error(ex, "Error al registrar datos de certificación en SAP");
                return false;
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
                EstadoDocumentoId = x.EstadoDocumentoId,
                CUIT = x.CUIT,
                RazonSocial = x.RazonSocial,
                Acciones = x.Acciones,
            }).ToList();
        }
        #endregion

        #region Metodo Tareas Programadas
        private string ToJson<T>(T obj)
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                return Encoding.UTF8.GetString(ms.ToArray());
            }
        }
        public Resultado ProcesarBoletosPendientesControl()
        {
            var oResultado = new Resultado();
            try
            {
                var boletosPendientes = repositorio.Listar<ControlDeBoletos>(x => x.EsConfirma == true &&
                                                                                  x.EsConfirmaAltaBorrador == false &&
                                                                                  x.AltaIdDocumentoConfirma > 0 &&
                                                                                  (x.EstadoConfirmaId == (int)EnumConfirmaEstadoDocumento.CONTRATO_PENDIENTE_DE_CONTROL ||
                                                                                   x.EstadoConfirmaId == (int)EnumConfirmaEstadoDocumento.CONTROLADO ||
                                                                                   x.EstadoConfirmaId == (int)EnumConfirmaEstadoDocumento.EN_FIRMA
                                                                                  )
                                                                                  );
                if (boletosPendientes.Count > 0)
                {
                    foreach (var boleto in boletosPendientes)
                    {
                        try
                        {
                            var bolsaConfirma = Convert.ToInt32(boleto.Negocio.Bolsa.CodigoConfirma);
                            var respuestaConsultaDocumentos = confirmaConsultaDocumentosAgent.ConsultaDocumentos(bolsaConfirma, boleto.AltaIdDocumentoConfirma.ToString());
                            if (respuestaConsultaDocumentos == null || respuestaConsultaDocumentos.Count == 0)
                                continue;

                            foreach (var documento in respuestaConsultaDocumentos)
                            {
                                var existe = this.repositorio.Obtener<ControlDeBoletoTracking>(x => x.ControlDeBoletosId == boleto.Id);
                                if (existe == null)
                                {
                                    var controlDeBoletoTracking = new ControlDeBoletoTracking()
                                    {
                                        ControlDeBoletosId = boleto.Id,
                                        CUIT              = documento.EnPoderDe?.CUIT.ToString(),
                                        RazonSocial       = documento.EnPoderDe?.RazonSocial,
                                        EstadoDocumentoId = documento.ConsultaEstadoDocumento,
                                        Acciones          = ToJson(documento.Acciones),
                                        FechaCreacion     = DateTime.Now
                                    };
                                    this.repositorio.Agregar(controlDeBoletoTracking);
                                    this.repositorio.GuardarCambios();
                                }
                                else
                                {
                                    existe.CUIT              = documento.EnPoderDe?.CUIT.ToString();
                                    existe.RazonSocial       = documento.EnPoderDe?.RazonSocial;
                                    existe.EstadoDocumentoId = documento.ConsultaEstadoDocumento;
                                    existe.Acciones          = ToJson(documento.Acciones);
                                    this.repositorio.GuardarCambios();
                                }

                                var controlDeBoletos = this.repositorio.Obtener<ControlDeBoletos>(boleto.Id);
                                controlDeBoletos.EstadoConfirmaId = documento.ConsultaEstadoDocumento;
                                this.repositorio.GuardarCambios();
                            }
                        }
                        catch (Exception exBoleto)
                        {
                            logger.Error(exBoleto, $"Error procesando boleto Id={boleto.Id}, Documento={boleto.AltaIdDocumentoConfirma}");
                            oResultado.Errores.Add(new ErrorMessage { Message = exBoleto.Message });
                        }
                    }
                }
                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex, "Error en ProcesarBoletosPendientesControl");
                return oResultado;
            }
        }
        #endregion

    }
}
