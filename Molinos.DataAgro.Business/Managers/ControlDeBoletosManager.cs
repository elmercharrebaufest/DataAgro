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
using System.Diagnostics.Contracts;
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
                    ControlDeBoletosEstado = x.cb.ControlDeBoletosEstado != null ? x.cb.ControlDeBoletosEstado.Descripcion : null,
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
                    Material = x.cb.Negocio.Material != null ? x.cb.Negocio.Material.Descripcion : null,
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
        public List<BoletoSapDto> GetBoletoSap(int boletoCompraNetId)
        {
            if (!Enum.IsDefined(typeof(EnumBoletoCompraNet), boletoCompraNetId))
            {
                return new List<BoletoSapDto>();
            }

            var enumValue = (EnumBoletoCompraNet)boletoCompraNetId;
            IEnumerable<BoletoSap> query = repositorio.Listar<BoletoSap>();

            switch (enumValue)
            {
                case EnumBoletoCompraNet.CONFIRMA:
                    query = query.Where(x => x.Confirma.HasValue && x.Confirma.Value);
                    break;

                case EnumBoletoCompraNet.CARTA_OFERTA:
                    query = query.Where(x => x.CartaOferta.HasValue && x.CartaOferta.Value);
                    break;

                case EnumBoletoCompraNet.SIN_BOLETO:
                    query = query.Where(x => x.SinBoleto.HasValue && x.SinBoleto.Value);
                    break;

                case EnumBoletoCompraNet.FISICO:
                    query = query.Where(x => x.Fisico.HasValue && x.Fisico.Value);
                    break;

                case EnumBoletoCompraNet.NINGUNO:
                    query = query.Where(x => x.Ninguno.HasValue && x.Ninguno.Value);
                    break;

                default:
                    return new List<BoletoSapDto>();
            }

            return query
                .Where(x => x != null)
                .OrderBy(x => x.Descripcion ?? string.Empty)
                .Select(x => new BoletoSapDto
                {
                    Id = x.Id,
                    Descripcion = x.Descripcion ?? string.Empty,
                    Caracter = x.Caracter ?? string.Empty,
                    CartaOferta = x.CartaOferta ?? false,
                    Confirma = x.Confirma ?? false,
                    Fisico = x.Fisico ?? false,
                    Ninguno = x.Ninguno ?? false,
                    SinBoleto = x.SinBoleto ?? false
                })
                .ToList();
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
                          join estado in repositorio.Listar<EstadoConfirma>()
                              on cb.EstadoConfirmaId equals estado.Id into estadoJoin
                          from estado in estadoJoin.DefaultIfEmpty()
                          select new ControlDeBoletosConsultaDto
                          {
                              Id = cb.Id,
                              NegocioId = cb.NegocioId,
                              ControlDeBoletosEstadoId = cb.ControlDeBoletosEstadoId,
                              ControlDeBoletosEstado = cb.ControlDeBoletosEstado != null ? cb.ControlDeBoletosEstado.Descripcion : null,
                              EsConfirma = cb.EsConfirma,
                              AltaIdLoteConfirma = cb.AltaIdLoteConfirma,
                              IdentificadorConfirma = cb.IdentificadorConfirma,
                              FechaCreacion = cb.FechaCreacion,
                              FechaModificacion = cb.FechaModificacion,
                              EstadoConfirmaId = cb.EstadoConfirmaId,
                              EstadoConfirma = estado != null ? estado.Descripcion : null,
                              TipoBoleto = cb.Negocio.Boleto != null ? cb.Negocio.Boleto.Descripcion : null,
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
                              Comercial = cb.Negocio.Comercial != null ? cb.Negocio.Comercial.Nombres + " " + cb.Negocio.Comercial.Apellido : null,
                              ContratoSAP = cb.Negocio.ContratoSAP,
                              ProveedorId = cb.Negocio.ProveedorId,
                              Proveedor = cb.Negocio.Proveedor != null ? cb.Negocio.Proveedor.RazonSocial : null,
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
                datosContrato.EsCartaOferta = contrato.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA;
                datosContrato.EsSinBoleto = contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO;
                datosContrato.BoletoCompraNetId = contrato.BoletoId;
            }

            var cuitProveedor = contrato.CorredorId > 0 ? contrato.Corredor.CUIT : contrato.Proveedor.CUIT;
            var tipoProveedor = contrato.CorredorId > 0 ? "CORR" : "PROV";
            var operaSinOblea = VerificarOperaSinOblea(cuitProveedor, tipoProveedor);
            datosContrato.OperaSinOblea = operaSinOblea == "SI";
            return datosContrato;
        }
        #endregion

        #region Datos de Seguimiento
        public ControlDeBoletosDatosSeguimientoDto ObtenerDatosDeSeguimiento(int controlDeBoletosId)
        {
            var seguimientoControlDeBoletos = new ControlDeBoletosDatosSeguimientoDto();
            var controlDeBoletosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosId);
            if (controlDeBoletosSeguimiento != null)
            {
                seguimientoControlDeBoletos.Id = controlDeBoletosSeguimiento.Id;
                seguimientoControlDeBoletos.ControlDeBoletosId = controlDeBoletosSeguimiento.ControlDeBoletosId;
                seguimientoControlDeBoletos.BoletoSapId = controlDeBoletosSeguimiento.BoletoSapId;
                seguimientoControlDeBoletos.BoletoSapCaracter = controlDeBoletosSeguimiento.BoletoSapCaracter;
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
                var mensajeSap = seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);

                // Validar el mensaje de respuesta de SAP
                if (string.IsNullOrWhiteSpace(mensajeSap))
                {
                    logger.Warn("SAP retornó un mensaje vacío al registrar seguimiento de boleto");
                }
                else
                {
                    logger.Info($"Respuesta de SAP al registrar seguimiento: {mensajeSap}");
                }
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage() { Message = "Error al registrar datos de seguimiento en SAP: " + ex.Message });
                logger.Error(ex, "Error al registrar seguimiento en SAP");
            }
            return oResultado;
        }

        private SeguimientoControlDeBoletosDto ConstruirSeguimientoDto(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            // Validar ControlDeBoletos
            var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(controlDeBoletosDatosSeguimiento.ControlDeBoletosId);
            if (controlDeBoletos == null)
            {
                logger.Error($"No se encontró ControlDeBoletos con Id: {controlDeBoletosDatosSeguimiento.ControlDeBoletosId}");
                throw new Exception($"No se encontró el control de boleto con ID {controlDeBoletosDatosSeguimiento.ControlDeBoletosId}");
            }

            // Validar Negocio y obtener ContratoSAP
            var negocio = repositorio.Obtener<Negocio>(controlDeBoletos.NegocioId);
            if (negocio == null)
            {
                logger.Error($"No se encontró Negocio con Id: {controlDeBoletos.NegocioId}");
                throw new Exception($"No se encontró el negocio asociado al control de boleto");
            }
            var contrato = negocio.ContratoSAP;

            // Validar BolsaCompraNet y obtener CodigoSap
            var bolsaEntity = repositorio.Obtener<BolsaCompraNet>(controlDeBoletosDatosSeguimiento.BolsaCompraNetId);
            if (bolsaEntity == null)
            {
                logger.Error($"No se encontró BolsaCompraNet con Id: {controlDeBoletosDatosSeguimiento.BolsaCompraNetId}");
                throw new Exception($"No se encontró la bolsa de compra especificada");
            }
            var bolsa = bolsaEntity.CodigoSap;

            // Validar BoletoSap y obtener Id
            var boletoSapEntity = repositorio.Obtener<BoletoSap>(controlDeBoletosDatosSeguimiento.BoletoSapId);
            if (boletoSapEntity == null)
            {
                logger.Error($"No se encontró BoletoSap con Id: {controlDeBoletosDatosSeguimiento.BoletoSapId}");
                throw new Exception($"No se encontró el tipo de boleto SAP especificado");
            }
            var tipoBoleto = boletoSapEntity.Id.ToString("D2");

            var ahora = DateTime.Now;

            return new SeguimientoControlDeBoletosDto
            {
                Bolsa = bolsa,
                BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado ?? string.Empty,
                Contrato = contrato,
                FeEnvio = string.Empty,
                FecAcopio = string.Empty,
                Fecha = ahora.ToString("yyyy-MM-dd"),
                Hora = ahora.ToString("HH:mm:ss"),
                TipoBoleto = controlDeBoletosDatosSeguimiento.BoletoSapCaracter ?? string.Empty,
                Usuario = string.Empty,
                FeRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto?.ToString("yyyy-MM-dd"),
                FeEnviadoFirma = controlDeBoletosDatosSeguimiento.FechaEnvioFirmas?.ToString("yyyy-MM-dd"),
                FeEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip?.ToString("yyyy-MM-dd"),
                FeEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa?.ToString("yyyy-MM-dd"),
                FeRecibFirma = controlDeBoletosDatosSeguimiento.FechaRecepcionFirma?.ToString("yyyy-MM-dd"),
                FeVueltaAfip = controlDeBoletosDatosSeguimiento.FechaRecepcionAfip?.ToString("yyyy-MM-dd"),
                FeVueltaBolsa = controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa?.ToString("yyyy-MM-dd"),
                ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto ?? string.Empty,
                ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2 ?? string.Empty,
                RechazadoAfip = string.Empty // TODO: Verificar si debe venir del DTO de entrada
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
            datosSeguimiento.BoletoSap = repositorio.Obtener<BoletoSap>(controlDeBoletosDatosSeguimiento.BoletoSapId);
            datosSeguimiento.BoletoSapCaracter = controlDeBoletosDatosSeguimiento.BoletoSapCaracter;
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
            EstablecerEstadoBoleto(datosSeguimiento.ControlDeBoletosId);
        }

        private void InsertarSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var datosSeguimiento = new ControlDeBoletosSeguimiento
            {
                ControlDeBoletosId = controlDeBoletosDatosSeguimiento.ControlDeBoletosId,
                BoletoSap = repositorio.Obtener<BoletoSap>(controlDeBoletosDatosSeguimiento.BoletoSapId),
                BoletoSapCaracter = controlDeBoletosDatosSeguimiento.BoletoSapCaracter,
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
            EstablecerEstadoBoleto(datosSeguimiento.ControlDeBoletosId);
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
                if (seguimiento.BoletoSapId > 0 && seguimiento.FechaRecepcionBoleto.HasValue)
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
            var controlDeBoletosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosId);
            listaDatosPreCertificacionDto.ControlDeBoletosId = controlDeBoletosId;
            listaDatosPreCertificacionDto.FechaRecepcionBoleto = controlDeBoletosSeguimiento?.FechaRecepcionBoleto;
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

                // Registros actuales en BD para este control (sin navegación para evitar conflictos FK)
                var registrosEnBd = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    x => x.ControlDeBoletosId == controlDeBoletosId);

                var ahora = DateTime.Now;

                // ── 1. Upsert: crear o actualizar los registros que vienen en el request ──
                if (controlDeBoletosPreCertificacion.Detalle != null)
                {
                    foreach (var item in controlDeBoletosPreCertificacion.Detalle)
                    {
                        var tipoOblea = repositorio.Listar<TipoOblea>(t => t.Codigo == item.CodigoTipoOblea).FirstOrDefault();
                        if (tipoOblea == null) continue;

                        var existente = registrosEnBd.FirstOrDefault(r => r.TipoObleaId == tipoOblea.Id);

                        if (existente != null)
                        {
                            // Modificar - solo actualizar propiedades escalares y FK, NO navegaciones
                            existente.Oblea = item.Oblea?.Trim();
                            existente.FechaCertificacion = item.FechaCertificacion ?? existente.FechaCertificacion;
                            existente.FechaVencimiento = item.FechaVencimiento ?? existente.FechaVencimiento;
                            existente.BolsaCompraNetId = item.BolsaCompraNetId ?? existente.BolsaCompraNetId;
                            existente.TipoObleaId = tipoOblea.Id;
                            existente.Rechazado = item.Rechazado ?? existente.Rechazado;
                            existente.FechaModificacion = ahora;
                            repositorio.GuardarCambios();
                            EstablecerEstadoBoleto(existente.ControlDeBoletosId);
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Modificar, existente.Id, "Modificacion de Certificacion - Control de Boletos");
                        }
                        else
                        {
                            // Crear
                            var nuevo = new ControlDeBoletosPreCertificacion
                            {
                                ControlDeBoletosId = item.ControlDeBoletosId,
                                Oblea = item.Oblea?.Trim(),
                                TipoObleaId = tipoOblea.Id,
                                BolsaCompraNetId = item.BolsaCompraNetId,
                                FechaCertificacion = item.FechaCertificacion,
                                FechaVencimiento = item.FechaVencimiento,
                                Rechazado = item.Rechazado ?? string.Empty,
                                FechaCreacion = ahora
                            };
                            repositorio.Agregar(nuevo);
                            repositorio.GuardarCambios();
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Crear, nuevo.Id, "Registro de Certificacion - Control de Boletos");
                            EstablecerEstadoBoleto(nuevo.ControlDeBoletosId);
                        }
                    }
                }
                // ── 2. Eliminar registros en BD que ya no están en el request ──
                // Recargar sin navegaciones para evitar conflictos
                var registrosActuales = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    x => x.ControlDeBoletosId == controlDeBoletosId);

                var tiposObleaEnRequest = repositorio.Listar<TipoOblea>(
                    t => codigosEnRequest.Contains(t.Codigo))
                    .ToDictionary(t => t.Codigo, t => t.Id);

                var registrosAEliminar = registrosActuales
                    .Where(r => !tiposObleaEnRequest.ContainsValue(r.TipoObleaId))
                    .ToList();

                foreach (var eliminado in registrosAEliminar)
                {
                    // Cargar solo para SAP notification
                    var preCertConNavegacion = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                        new List<Expression<Func<ControlDeBoletosPreCertificacion, object>>> { r => r.TipoOblea },
                        x => x.Id == eliminado.Id).FirstOrDefault();

                    if (preCertConNavegacion != null)
                    {
                        RegistrarDatosCertificacionParaPreCertificacion(preCertConNavegacion, oResultado, sinValores: true);
                    }

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

        #region Metodo para establecer el estado de Boleto
        private void EstablecerEstadoBoleto(int controlDeBoletoId)
        {
            var controlBoleto = repositorio.Obtener<ControlDeBoletos>(controlDeBoletoId);

            if (controlBoleto == null)
                return;

            var certificaciones = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                x => x.ControlDeBoletosId == controlDeBoletoId);

            var seguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(
                x => x.ControlDeBoletosId == controlDeBoletoId);

            // Pendiente de control
            if (!certificaciones.Any() && seguimiento == null)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_CONTROL);
                return;
            }

            var contrato = repositorio.Obtener<Negocio>(x => x.Id == controlBoleto.NegocioId);

            var cuitProveedor = contrato.CorredorId > 0
                ? contrato.Corredor.CUIT
                : contrato.Proveedor.CUIT;

            var tipoProveedor = contrato.CorredorId > 0 ? "CORR" : "PROV";

            bool operaSinOblea =
                VerificarOperaSinOblea(cuitProveedor, tipoProveedor) == "SI";

            bool esCartaOferta =
                contrato.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA;

            bool esSinBoleto =
                contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO;

            if (!certificaciones.Any() && !esSinBoleto)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                return;
            }

            var tipoObleaBolsa = repositorio.Listar<TipoOblea>(x => x.Codigo == "O")
                                            .FirstOrDefault();

            var tipoObleaArca = repositorio.Listar<TipoOblea>(x => x.Codigo == "A")
                                           .FirstOrDefault();

            bool tieneObleaBolsa =
                certificaciones.Any(x => x.TipoObleaId == tipoObleaBolsa.Id);

            bool tieneCodigoArca =
                certificaciones.Any(x => x.TipoObleaId == tipoObleaArca.Id);

            // Sin boleto
            if (esSinBoleto)
            {
                if (!tieneCodigoArca)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_CODIGO_ARCA);
                    return;
                }

                if (seguimiento?.FechaEnvioAfip.HasValue == true &&
                    seguimiento.FechaRecepcionAfip.HasValue)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.CONTROLADO);
                }

                return;
            }

            // Requiere Oblea Bolsa
            if (!operaSinOblea && !tieneObleaBolsa)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                return;
            }

            if (!tieneCodigoArca)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_CODIGO_ARCA);
                return;
            }

            bool seguimientoCompletoSinOblea =
                seguimiento != null &&
                seguimiento.FechaEnvioFirmas.HasValue &&
                seguimiento.FechaRecepcionFirma.HasValue &&
                seguimiento.FechaEnvioAfip.HasValue &&
                seguimiento.FechaRecepcionAfip.HasValue;

            bool seguimientoCompletoConOblea =
                seguimiento != null &&
                seguimiento.FechaEnvioBolsa.HasValue &&
                seguimiento.FechaRecepcionBolsa.HasValue &&
                seguimiento.FechaEnvioFirmas.HasValue &&
                seguimiento.FechaRecepcionFirma.HasValue &&
                seguimiento.FechaEnvioAfip.HasValue &&
                seguimiento.FechaRecepcionAfip.HasValue;

            bool requiereSellado = false;

            if (operaSinOblea)
            {
                requiereSellado =
                    !esCartaOferta &&
                    seguimiento?.FechaEnvioSellado.HasValue != true;
                if (seguimientoCompletoConOblea && !requiereSellado)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.CONTROLADO);
                }
            }
            else
            {
                requiereSellado =
                !esCartaOferta &&
                seguimiento?.FechaEnvioSellado.HasValue != true;
                if (seguimientoCompletoSinOblea && !requiereSellado)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.CONTROLADO);
                }
            }

        }

        private void ActualizarEstado(
            ControlDeBoletos boleto,
            EnumControlDeBoletosEstado estado)
        {
            boleto.ControlDeBoletosEstado =
                repositorio.Obtener<ControlDeBoletosEstado>(
                    x => x.Id == (int)estado);

            repositorio.GuardarCambios();
        }
        #endregion

    }
}
