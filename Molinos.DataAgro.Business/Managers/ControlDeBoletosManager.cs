using Molinos.DataAgro.Agent.Helpers;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
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
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using ControlBoletosSortDescriptor = Molinos.DataAgro.Entities.Dto.ControlDeBoletos.SortDescriptor;

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
        private readonly IConfirmaConsultaDocumentosRegistradosAgent confirmaConsultaDocumentosRegistradosAgent;
        private readonly IStatusContratoAgent statusContratoAgent;
        private readonly IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent;
        public ControlDeBoletosManager(ILogger logger, IRepositorio repositorio, IMailManager mailManager,
                                       IModificacionContratoControlBoletoAgent modificacionContratoControlBoletoAgent,
                                       ISeguimientoControlBoletoAgent seguimientoControlBoletoAgent,
                                       IConfirmaConsultaDocumentosAgent confirmaConsultaDocumentosAgent,
                                       IDatosCertificacionControlBoletoAgent datosCertificacionControlBoletoAgent,
                                       IAltaTempranaAgent altaTempranaAgent,
                                       ILogDataAgroManager logDataAgroManager,
                                       IConfirmaConsultaDocumentosRegistradosAgent confirmaConsultaDocumentosRegistradosAgent,
                                       IStatusContratoAgent statusContratoAgent,
                                       IConsultarEstadoBoletoAgent oConsultarEstadoBoletoAgent
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
            this.confirmaConsultaDocumentosRegistradosAgent = confirmaConsultaDocumentosRegistradosAgent;
            this.statusContratoAgent = statusContratoAgent;
            this.oConsultarEstadoBoletoAgent = oConsultarEstadoBoletoAgent;
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
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioFirma.HasValue && x.seg.FechaEnvioFirma.Value >= filtros.FechaEnviadoFirmaDesde.Value);

                if (filtros.FechaEnviadoFirmaHasta.HasValue)
                    query = query.Where(x => x.seg != null && x.seg.FechaEnvioFirma.HasValue && x.seg.FechaEnvioFirma.Value <= filtros.FechaEnviadoFirmaHasta.Value);

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
                    FechaEnviadoFirma = x.seg != null ? x.seg.FechaEnvioFirma : null,
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
        private ControlDeBoletos CrearControlDeBoletos(
            int negocioId,
            bool esConfirma,
            int? altaIdLoteConfirma = null,
            int? altaIdDocumentoConfirma = null,
            int? estadoConfirmaId = null,
            bool esConfirmaAltaBorrador = false,
            int? altaIdLoteConfirmaAnterior = null,
            int? altaIdDocumentoConfirmaAnterior = null)
        {
            return new ControlDeBoletos
            {
                NegocioId = negocioId,
                FechaCreacion = DateTime.Now,
                EsConfirma = esConfirma,
                AltaIdLoteConfirma = altaIdLoteConfirma,
                AltaIdDocumentoConfirma = altaIdDocumentoConfirma,
                ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL,
                EstadoConfirmaId = estadoConfirmaId,
                ControlIniciado = false,
                ControlFinalizado = false,
                CertificacionCompletada = false,
                RegistroDatosOblea = false,
                EsConfirmaAltaBorrador = esConfirmaAltaBorrador,
                AltaIdLoteConfirmaAnterior = altaIdLoteConfirmaAnterior,
                AltaIdDocumentoConfirmaAnterior = altaIdDocumentoConfirmaAnterior
            };
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
            return this.repositorio.Listar<TipoOblea>().OrderBy(x => x.Descripcion).ToList();
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
        public (List<ControlDeBoletosConsultaDto> Data, int Total) GetControlBoletosPendientes(ControlDeBoletoFiltroBusquedaDto filtros)
        {
            filtros = filtros ?? new ControlDeBoletoFiltroBusquedaDto();

            // Paso 1: detectar negocios SAP confirmados sin registro en ControlDeBoletos e insertarlos vía EF
            var negociosIds = repositorio.ObtenerConsultaEscalar(new TraerNegociosPendientesControlBoleto());

            if (negociosIds != null && negociosIds.Count > 0)
            {
                var controlBoletosNuevos = negociosIds
                    .Select(id => CrearControlDeBoletos(id, false))
                    .ToList();
                repositorio.AgregarTodos(controlBoletosNuevos);
                repositorio.GuardarCambios();
            }

            // Paso 1.1: si el filtro trae contratos en NegocioSAP, registrar los que todavía no tengan ControlDeBoletos
            var contratosNegocioSap = (filtros?.NegocioSAP ?? string.Empty)
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            if (contratosNegocioSap.Count > 0)
            {
                var negociosSolicitados = repositorio.Listar<Negocio>(x =>
                    contratosNegocioSap.Contains(x.ContratoSAP) &&
                    x.ConfirmadoSAP.HasValue &&
                    x.FechaConfirmadoSAP.HasValue).ToList();

                if (negociosSolicitados.Count > 0)
                {
                    var negociosSolicitadosIdsBase = negociosSolicitados.Select(x => x.Id).ToList();

                    var negociosConConfirma = repositorio.Listar<Confirma>(x =>
                        negociosSolicitadosIdsBase.Contains(x.NegocioId))
                        .Select(x => x.NegocioId)
                        .Distinct()
                        .ToHashSet();

                    var negociosConBoleto = repositorio.Listar<Boleto>(x =>
                        negociosSolicitadosIdsBase.Contains(x.NegocioId))
                        .Select(x => x.NegocioId)
                        .Distinct()
                        .ToHashSet();

                    var negociosSolicitadosValidos = negociosSolicitados
                        .Where(x =>
                            (x.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA && negociosConConfirma.Contains(x.Id)) ||
                            ((x.BoletoId == (int)EnumBoletoCompraNet.FISICO || x.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA) && negociosConBoleto.Contains(x.Id)) ||
                            (x.BoletoId != (int)EnumBoletoCompraNet.CONFIRMA && x.BoletoId != (int)EnumBoletoCompraNet.FISICO && x.BoletoId != (int)EnumBoletoCompraNet.CARTA_OFERTA)
                        )
                        .ToList();

                    var negociosSolicitadosIds = negociosSolicitadosValidos
                        .Select(x => x.Id)
                        .ToList();

                    var negociosConControl = repositorio.Listar<ControlDeBoletos>(x =>
                        negociosSolicitadosIds.Contains(x.NegocioId))
                        .Select(x => x.NegocioId)
                        .Distinct()
                        .ToList();

                    var controlBoletosNuevosPorContrato = negociosSolicitadosValidos
                        .Where(x => !negociosConControl.Contains(x.Id))
                        .Select(x => CrearControlDeBoletos(x.Id, x.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA))
                        .ToList();

                    if (controlBoletosNuevosPorContrato.Count > 0)
                    {
                        repositorio.AgregarTodos(controlBoletosNuevosPorContrato);
                        repositorio.GuardarCambios();
                    }
                }
            }

            // Paso 2: consulta principal vía SP (filtros + JOINs + seguimiento resueltos en SQL Server)
            var data = repositorio.ObtenerConsultaEscalar(new TraerControlBoletosPendientes(filtros)) ?? new List<ControlDeBoletosConsultaDto>();
            var total = data.Count;
            var query = AplicarOrdenControlBoletos(data.AsQueryable(), filtros.Sort);
            var skip = filtros.Skip < 0 ? 0 : filtros.Skip;
            var take = filtros.Take > 0 ? filtros.Take : total;

            var pagina = query
                .Skip(skip)
                .Take(take)
                .ToList();

            if (pagina.Count > 0)
            {
                foreach (var item in pagina.Where(x => !string.IsNullOrWhiteSpace(x.ContratoSAP) && (x.TipoBoletoId == (int) EnumBoletoCompraNet.CARTA_OFERTA ||
                                                                                                     x.TipoBoletoId == (int)EnumBoletoCompraNet.FISICO ||
                                                                                                     x.TipoBoletoId == (int)EnumBoletoCompraNet.CONFIRMA) ).ToList())
                {
                    var estado = ObtenerEstadoBoleto(new BasicoBoleto
                    {
                        Id = item.NegocioId,
                        ContratoSAP = item.ContratoSAP,
                        NegocioSAP = item.ContratoSAP,
                        BoletoId = item.TipoBoletoId,
                        Version = item.Version,
                        Estado_Version = "Pendiente"
                    });
                    item.EstadoVersion = estado;
                    if (estado == "Anulado")
                    {
                        item.FechaGeneracion = null;
                        item.Version = (item.Version ?? 0) + 1;
                    }
                }
            }
            
            return (pagina, total);
        }
        public Resultado RegistroContratoPendienteDeControl(int negocioId, int? altaIdLoteConfirma = null, int? altaIdDocumentoConfirma = null, bool? esConfirmaAltaBorrador = false)
        {
            var oResultado = new Resultado();
            var negocio = repositorio.Obtener<Negocio>(negocioId);
            if (negocio != null)
            {
                var controlDeBoletosExiste = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocioId);
                var esConfirma = negocio.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA;
                var estadoConfirmaId = esConfirma ? (int?)EnumEstadoConfirma.PENDIENTE : null;
                var flagAltaBorrador = esConfirma && esConfirmaAltaBorrador.GetValueOrDefault();
                var altaLote = esConfirma ? altaIdLoteConfirma : null;
                var altaDocumento = esConfirma ? altaIdDocumentoConfirma : null;

                if (controlDeBoletosExiste == null)
                {
                    var controlDeBoletos = CrearControlDeBoletos(
                        negocioId,
                        esConfirma,
                        altaLote,
                        altaDocumento,
                        estadoConfirmaId,
                        flagAltaBorrador);
                    repositorio.Agregar(controlDeBoletos);
                    repositorio.GuardarCambios();
                }
                else
                {
                    controlDeBoletosExiste.FechaAnulacionConfirma = DateTime.Now;
                    controlDeBoletosExiste.EstadoConfirmaId = (int)EnumEstadoConfirma.ANULADO;
                    // Guardarnos de datos huérfanos en la FK del maestro de estados.
                    if (!repositorio.Listar<ControlDeBoletosEstado>(e => e.Id == controlDeBoletosExiste.ControlDeBoletosEstadoId).Any())
                    {
                        controlDeBoletosExiste.ControlDeBoletosEstadoId = (int)EnumControlDeBoletosEstado.PENDIENTE_CONTROL;
                    }
                    repositorio.GuardarCambios();

                    this.EliminarDatosSeguimiento(controlDeBoletosExiste.Id);
                    var controlDeBoletosSustitutorio = CrearControlDeBoletos(
                        negocioId,
                        esConfirma,
                        altaLote,
                        altaDocumento,
                        estadoConfirmaId,
                        flagAltaBorrador,
                        controlDeBoletosExiste.AltaIdLoteConfirma,
                        controlDeBoletosExiste.AltaIdDocumentoConfirma);
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

                var provincia = repositorio.Obtener<Provincia>(dto.ProvinciaId);
                var campana = repositorio.Obtener<Campaña>(dto.CosechaId);
                var procedencia = repositorio.Obtener<Localidad>(dto.ProcedenciaId);
                var clasificacion = repositorio.Obtener<ClasificacionCompraNet>(dto.ClasificacionId);
                var ahora = DateTime.Now;

                var controlDeBoletosModificarContrato = new ControlDeBoletosModificarContratoDto
                {
                    Cosecha = campana.Descripcion,
                    Contrato = negocio.ContratoSAP,
                    Clasificacion = clasificacion.Descripcion,
                    Fecha = ahora.ToString("yyyy-MM-dd"),
                    Hora = ahora.ToString("HH:mm:ss"),
                    Procedencia = procedencia.CodLocalidad,
                    Provincia = provincia.ProvinciaId.ToString(),
                    Usuario = dto.Usuario
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
                var estadoContratoSap = this.statusContratoAgent.ValidarEstado(contrato.ContratoSAP);
                if (estadoContratoSap != null)
                {
                    datosContrato.NumeroSio = estadoContratoSap.NumeroSio;
                }
                datosContrato.NegocioId = contrato.Id;
                datosContrato.BolsaId = contrato.Bolsa != null ? contrato.Bolsa.Id : 0;
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
                datosContrato.Corredor = contrato.CorredorId > 0 && contrato.Corredor !=null? contrato.Corredor.RazonSocial : string.Empty;
                datosContrato.Moneda = contrato.Moneda?.MonedaId;
                datosContrato.CorredorId = contrato.CorredorId;
                datosContrato.PlanCanje = contrato.PlanCanje;
                datosContrato.EsCartaOferta = contrato.BoletoId == (int)EnumBoletoCompraNet.CARTA_OFERTA;
                datosContrato.EsSinBoleto = contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO;
                datosContrato.BoletoCompraNetId = contrato.BoletoId;
                datosContrato.Comercial = string.Format("{0} {1}", contrato.Comercial.Nombres, contrato.Comercial.Apellido);
                datosContrato.Proveedor = contrato.Proveedor.RazonSocial;
                datosContrato.TipoNegocio = contrato.TipoNegocio.Descripcion;
                datosContrato.PrecioNeto = contrato.PrecioNeto;
                datosContrato.PorcentajePago = contrato.PorcentajeDePago!=null ? string.Format("{0}%", contrato.PorcentajeDePago) : string.Empty;
                datosContrato.Bolsa = contrato.Bolsa != null ? contrato.Bolsa.Descripcion : string.Empty;
                datosContrato.MercaderaDeposito = (contrato.MercsDeposito != null && contrato.MercsDeposito == true) ? "SI" : "NO";
                datosContrato.CantidadDeposito = contrato.CantidadDeposito != null ? contrato.CantidadDeposito : 0;

                datosContrato.FechaHastaOriginal = contrato.FechaHastaOriginal.HasValue ? contrato.FechaHastaOriginal.Value.ToString("dd/MM/yyyy") : string.Empty;
                datosContrato.CantidadFijacionMaxima = (contrato is FijacionDePrecioContrato) && (contrato as FijacionDePrecioContrato).Contrato != null ? (contrato as FijacionDePrecioContrato).Contrato.KgMaximo ?? 0 : contrato.KgMaximo ?? 0;
                datosContrato.CantidadFijacionMinima = (contrato is FijacionDePrecioContrato) && (contrato as FijacionDePrecioContrato).Contrato != null ? (contrato as FijacionDePrecioContrato).Contrato.KgMinimo ?? 0 : contrato.KgMinimo ?? 0;
                datosContrato.FechaDolarizadoOriginal = contrato.FechaDolarizadoOriginal.HasValue ? contrato.FechaDolarizadoOriginal.Value.ToString("dd/MM/yyyy") : string.Empty;

                var aperturaPrecioRedespacho = contrato.AperturaPrecio.FirstOrDefault(t => t.ConceptoAperturaPrecioId == 2);
                datosContrato.MonedaRedespacho = aperturaPrecioRedespacho?.Moneda!=null ? aperturaPrecioRedespacho?.Moneda.Descripcion : string.Empty;
                datosContrato.ImporteRedespacho = aperturaPrecioRedespacho?.Importe;
                datosContrato.Consignatario = contrato.Consignatario != null && ((bool)contrato.Consignatario) ? "SI" : "NO";
                datosContrato.CD = contrato.CD != null && ((bool)contrato.CD) ? true : false;
                datosContrato.PagoDirectoVendedor = contrato.PagoDirectoVendedor != null && ((bool)contrato.PagoDirectoVendedor) ? true : false;
                datosContrato.FechaCierta = contrato.FechaCierta.HasValue ? contrato.FechaCierta.Value.ToString("dd/MM/yyyy") : string.Empty;

                datosContrato.Cesion = contrato.Cesion != null && ((bool)contrato.Cesion) ? true: false;
                datosContrato.Compensacion = contrato.Compensacion != null && ((bool)contrato.Compensacion) ? true : false;
                datosContrato.DolarizadoExpress = contrato.DolarizadoExpress != null && ((bool)contrato.DolarizadoExpress) ? true : false;
                datosContrato.EUDR = contrato.EUDR != null && ((bool)contrato.EUDR) ? true : false;
                datosContrato.EPA = contrato.EPA != null && ((bool)contrato.EPA) ? true : false;
                datosContrato.Sustentable = contrato.Sustentable != null && ((bool)contrato.Sustentable) ? true : false;
                datosContrato.Canje = contrato.Canje != null && ((bool)contrato.Canje) ? true : false;

                if (contrato.BoletoId == (int)EnumBoletoCompraNet.CONFIRMA)
                {
                    var confirmas = repositorio.Listar<Confirma>(x => x.NegocioId == contrato.Id);
                    if (confirmas!=null && confirmas.Count >0)
                    {
                        var maxVersion = confirmas.Max(x => x.Version);
                        if (maxVersion != null)
                        {
                            var confirma = confirmas.FirstOrDefault(x => x.Version == maxVersion);
                            datosContrato.VersionBoleto = confirma.Version;
                            datosContrato.FechaGeneracion = confirma.FechaGeneracion != null ? confirma.FechaGeneracion.ToString("dd/MM/yyyy") : string.Empty;
                        }
                    }
                }
                else
                {
                    var boletos = repositorio.Listar<Boleto>(x => x.NegocioId == contrato.Id);
                    if (boletos != null && boletos.Count > 0)
                    {
                        var maxVersion = boletos.Max(x => x.Version);
                        if (maxVersion != null)
                        {
                            var boleto = boletos.FirstOrDefault(x => x.Version == maxVersion);
                            datosContrato.VersionBoleto = boleto.Version;
                            datosContrato.FechaGeneracion = boleto.FechaGeneracion != null ? boleto.FechaGeneracion.ToString("dd/MM/yyyy") : string.Empty;
                        }
                    }
                }
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
                seguimientoControlDeBoletos.FechaEnvioFisicoBolsa = controlDeBoletosSeguimiento.FechaEnvioFisicoBolsa;
                seguimientoControlDeBoletos.FechaRecepcionBoletoOriginal = controlDeBoletosSeguimiento.FechaRecepcionBoletoOriginal;
                seguimientoControlDeBoletos.FechaEnvioFirma = controlDeBoletosSeguimiento.FechaEnvioFirma;
                seguimientoControlDeBoletos.FechaEnvioBolsa = controlDeBoletosSeguimiento.FechaEnvioBolsa;
                seguimientoControlDeBoletos.FechaEnvioAfip = controlDeBoletosSeguimiento.FechaEnvioAfip;

                seguimientoControlDeBoletos.FechaRecepcionFirma = controlDeBoletosSeguimiento.FechaRecepcionFirma;
                seguimientoControlDeBoletos.FechaRecepcionBolsa = controlDeBoletosSeguimiento.FechaRecepcionBolsa;
                seguimientoControlDeBoletos.FechaRecepcionAfip = controlDeBoletosSeguimiento.FechaRecepcionAfip;

                seguimientoControlDeBoletos.FechaEnvioSellado = controlDeBoletosSeguimiento.FechaEnvioSellado;

                seguimientoControlDeBoletos.RechazadoAfip = controlDeBoletosSeguimiento.RechazadoAfip;
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

                var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosDatosSeguimiento.ControlDeBoletosId);
                if (datosSeguimiento == null)
                {
                    if (!TieneDatosSeguimiento(controlDeBoletosDatosSeguimiento))
                    {
                        oResultado.Errores.Add(new ErrorMessage() { Message = "No se proporcionaron datos de seguimiento para registrar." });
                        return oResultado;
                    }
                }
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
        public Resultado EliminarDatosSeguimiento(int controlDeBoletosId)
        {
            var oResultado = new Resultado();
            try
            {
                var seguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosId);

                if (seguimiento != null) {
                    var contratoSAP = seguimiento.ControlDeBoletos.Negocio.ContratoSAP;
                    var seguimientoControlDeBoletos = new SeguimientoControlDeBoletosDto
                    {
                        Bolsa               = string.Empty,
                        BolsaSellado        = string.Empty,
                        Contrato            = contratoSAP ,
                        FecAcopio           = string.Empty,
                        Fecha               = string.Empty,
                        Hora                = string.Empty,
                        TipoBoleto          = string.Empty,
                        Usuario             = PermisosHelper.ObtenerUsuario(),
                        FechaRecepBoleto    = string.Empty,
                        FechaEnvioFirma     = string.Empty,
                        FechaEnvioAfip      = string.Empty,
                        FechaEnvioBolsa     = string.Empty,
                        FechaRecepcionFirma = string.Empty,
                        FechaRecepcionBolsa = string.Empty,
                        FechaRecepcionAfip  = string.Empty,
                        FechaEnvioSellado   = string.Empty,
                        ObsCtrlBoleto       = string.Empty,
                        ObsCtrlBoleto2      = string.Empty,
                        RechazadoAfip       = string.Empty,
                    };
                    seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);
                    repositorio.Remover(seguimiento);
                    repositorio.GuardarCambios();
                }

                EliminarPreCertificacion(controlDeBoletosId);

                if (seguimiento != null)
                {
                    this.logDataAgroManager.LogCambiosControlBoletos(seguimiento, TipoAccionLogDataAgro.Eliminar, seguimiento.Id, "Eliminar de Seguimiento - Control de Boletos");
                }
                this.EstablecerEstadoBoleto(controlDeBoletosId);

            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage() { Message = "Error al eliminar datos de seguimiento y pre-certificación: " + ex.Message });
                logger.Error(ex, "Error al eliminar datos de seguimiento y pre-certificación en SAP");
            }
            return oResultado;
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
        public string VerificarDuplicidadObleaCodigoArca(int controlDeBoletosId, string numeroOblea, string codigoArca)
        {
            string mensaje = string.Empty;
            var certificacionOblea = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId != controlDeBoletosId && x.Oblea.Equals(numeroOblea) && x.TipoOblea.Codigo.Equals("O"));
            var certificacionCodigoArca = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId != controlDeBoletosId && x.Oblea.Equals(codigoArca) && x.TipoOblea.Codigo.Equals("A"));
            var duplicidadOblea = certificacionOblea != null && certificacionOblea.Any();
            var duplicidadCodigoArca = certificacionCodigoArca != null && certificacionCodigoArca.Any();

            if (duplicidadOblea && duplicidadCodigoArca)
            {
                mensaje = "Ya existe el numero de oblea y codigo arca en otro contrato.";
            }
            else if (duplicidadOblea)
            {
                mensaje = "Ya existe el numero de oblea en otro contrato.";
            }
            else if (duplicidadCodigoArca)
            {
                mensaje = "Ya existe el codigo arca en otro contrato.";
            }

            return mensaje;
        }
        public ControlDeBoletosPreCertificacionDto ObtenerDatosPreCertificacion(int controlDeBoletosId)
        {
            var listaDatosPreCertificacionDto = new ControlDeBoletosPreCertificacionDto();
            listaDatosPreCertificacionDto.Detalle = new List<ControlDeBoletosDatosPreCertificacionDto>();

            //Para los casos que se trabaje con boleto fisico y opera sin oblea se grabara la oblea con valores por defecto en DataAgro y SAP
            RegistrarDatosSinObleaYSinBoleto(controlDeBoletosId);

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
                var codigosEnRequest = controlDeBoletosPreCertificacion.Detalle != null ? controlDeBoletosPreCertificacion.Detalle
                    .Select(d => d.CodigoTipoOblea)
                    .ToHashSet() : new HashSet<string>();

                // Registros actuales en BD para este control (sin navegación para evitar conflictos FK)
                var registrosEnBd = repositorio.Listar<ControlDeBoletosPreCertificacion>(
                    x => x.ControlDeBoletosId == controlDeBoletosId);

                var ahora = DateTime.Now;

                bool hayCambiosEnBd = false;

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
                            existente.FechaCertificacion = item.FechaCertificacion;
                            existente.FechaVencimiento = item.FechaVencimiento;
                            existente.BolsaCompraNetId = item.BolsaCompraNetId;
                            existente.TipoObleaId = tipoOblea.Id;
                            existente.Rechazado = item.Rechazado;
                            existente.FechaModificacion = ahora;
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Modificar, existente.Id, "Modificacion de Certificacion - Control de Boletos");
                            hayCambiosEnBd = true;
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
                                Rechazado = item.Rechazado,
                                FechaCreacion = ahora
                            };
                            repositorio.Agregar(nuevo);
                            this.logDataAgroManager.LogCambiosControlBoletos(item, TipoAccionLogDataAgro.Crear, nuevo.Id, "Registro de Certificacion - Control de Boletos");
                            hayCambiosEnBd = true;
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
                {
                    hayCambiosEnBd = true;
                }

                if (hayCambiosEnBd)
                {
                    repositorio.GuardarCambios();
                }

                // Ejecutar el estado al final, con la persistencia ya aplicada completa.
                EstablecerEstadoBoleto(controlDeBoletosId);

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
                datosCertificacionCabeceraDto.Usuario = PermisosHelper.ObtenerUsuario();

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
                    datosCertificacionDetalleDto.Rechazado = controlDeBoletosPreCertificacion.Rechazado;
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
        private void RegistrarDatosSinObleaYSinBoleto(int controlDeBoletosId)
        {
            var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(x => x.Id == controlDeBoletosId);
            var contrato = repositorio.Obtener<Negocio>(x => x.Id == controlDeBoletos.NegocioId);
            bool esSinBoleto = contrato.BoletoId == (int)EnumBoletoCompraNet.SIN_BOLETO;
            var cuitProveedor = contrato.CorredorId > 0 ? contrato.Corredor.CUIT : contrato.Proveedor.CUIT;
            var tipoProveedor = contrato.CorredorId > 0 ? "CORR" : "PROV";
            var verificaOperacionSinOblea = VerificarOperaSinOblea(cuitProveedor, tipoProveedor);
            var operaSinOblea = verificaOperacionSinOblea == "SI";
            if (esSinBoleto && operaSinOblea)
            {
                DateTime fechaActual = DateTime.Now.Date;
                DateTime ultimoDiaAnio = new DateTime(DateTime.Now.Year, 12, 31);
                var tipoOblea = repositorio.Listar<TipoOblea>(t => t.Codigo == "O").FirstOrDefault();
                var preCertificacion = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == controlDeBoletosId && x.TipoOblea.Id == tipoOblea.Id);
                if (preCertificacion == null || preCertificacion.Count == 0)
                {
                    var nuevo = new ControlDeBoletosPreCertificacion
                    {
                        ControlDeBoletosId = controlDeBoletosId,
                        Oblea = contrato.ContratoSAP,
                        TipoObleaId = tipoOblea.Id,
                        BolsaCompraNetId = contrato.Bolsa?.Id,
                        FechaCertificacion = fechaActual,
                        FechaVencimiento = ultimoDiaAnio,
                        Rechazado = string.Empty,
                        FechaCreacion = DateTime.Now
                    };
                    repositorio.Agregar(nuevo);
                    this.logDataAgroManager.LogCambiosControlBoletos(new ControlDeBoletosDatosPreCertificacionDto
                    {
                        ControlDeBoletosId = controlDeBoletosId,
                        CodigoTipoOblea = tipoOblea.Codigo
                    }, TipoAccionLogDataAgro.Crear, nuevo.Id, "Registro de Certificacion - Control de Boletos (Sin Oblea y Sin Boleto)");

                    var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto();
                    var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto();

                    datosCertificacionCabeceraDto.Contrato = contrato?.ContratoSAP ?? string.Empty;
                    datosCertificacionCabeceraDto.Fecha = fechaActual.ToString("yyyy-MM-dd");
                    datosCertificacionCabeceraDto.Hora = fechaActual.ToString("HH:mm:ss");
                    datosCertificacionCabeceraDto.Fijacion = string.Empty;
                    datosCertificacionCabeceraDto.Usuario = PermisosHelper.ObtenerUsuario();

                    datosCertificacionDetalleDto.Bolsa = contrato.Bolsa?.CodigoSap ?? string.Empty;
                    datosCertificacionDetalleDto.Oblea = contrato.ContratoSAP;
                    datosCertificacionDetalleDto.FeCertificacion = FormatDate(fechaActual);
                    datosCertificacionDetalleDto.FeVencCerti = FormatDate(ultimoDiaAnio);
                    datosCertificacionDetalleDto.Rechazado = string.Empty;
                    datosCertificacionDetalleDto.Tipo = tipoOblea?.Codigo ?? string.Empty;

                    datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
                    repositorio.GuardarCambios();
                }
            }
        }
        public Resultado EliminarPreCertificacion(int controlDeBoletosId)
        {
            var oResultado = new Resultado();
            var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto();
            var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto();
            try
            {
                var controlDeBoletos = repositorio.Obtener<ControlDeBoletos>(controlDeBoletosId);
                var precertificacion = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == controlDeBoletosId);

                if (precertificacion != null && precertificacion.Any())
                {
                    var negocio = repositorio.Obtener<Negocio>(x => x.Id == controlDeBoletos.NegocioId);

                    var ahora = DateTime.Now;
                    datosCertificacionCabeceraDto.Contrato = negocio?.ContratoSAP ?? string.Empty;
                    datosCertificacionCabeceraDto.Fecha = ahora.ToString("yyyy-MM-dd");
                    datosCertificacionCabeceraDto.Hora = ahora.ToString("HH:mm:ss");
                    datosCertificacionCabeceraDto.Fijacion = string.Empty;
                    datosCertificacionCabeceraDto.Usuario = PermisosHelper.ObtenerUsuario();

                    foreach (var datos in precertificacion)
                    {
                        var tipoOblea = repositorio.Obtener<TipoOblea>(datos.TipoObleaId);
                        datosCertificacionDetalleDto.Bolsa = string.Empty;
                        datosCertificacionDetalleDto.Oblea = string.Empty;
                        datosCertificacionDetalleDto.FeCertificacion = string.Empty;
                        datosCertificacionDetalleDto.FeVencCerti = string.Empty;
                        datosCertificacionDetalleDto.Rechazado = string.Empty;
                        datosCertificacionDetalleDto.Tipo = tipoOblea?.Codigo ?? string.Empty;
                        datosCertificacionCabeceraDto.Detalle = new List<RegistroDatosCertificacionControlDeBoletosDetalleDto> { datosCertificacionDetalleDto };
                        datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
                        repositorio.Remover(datos);
                    }
                    repositorio.GuardarCambios();
                }
                this.logDataAgroManager.LogCambiosControlBoletos(precertificacion, TipoAccionLogDataAgro.Eliminar, precertificacion.FirstOrDefault()?.Id ?? 0 , "Eliminar de Certificacion - Control de Boletos");
                this.EstablecerEstadoBoleto(controlDeBoletosId);
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage() { Message = "Error al eliminar datos de precertificación: " + ex.Message });
                logger.Error(ex, "Error al eliminar precertificación");
            }
            return oResultado;
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
        public Resultado ActualizarEstadoBoletosConfirma()
        {
            var oResultado = new Resultado();
            try
            {
                var boletosPendientes = repositorio.Listar<ControlDeBoletos>(x => x.EsConfirma == true &&
                                                                                  x.EsConfirmaAltaBorrador == false &&
                                                                                  x.AltaIdDocumentoConfirma > 0 &&
                                                                                  (x.EstadoConfirmaId != (int)EnumConfirmaEstadoDocumento.EN_FIRMA && x.ControlDeBoletosEstadoId != (int)EnumControlDeBoletosEstado.ANULADO)
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
                                        CUIT = documento.EnPoderDe?.CUIT.ToString(),
                                        RazonSocial = documento.EnPoderDe?.RazonSocial,
                                        EstadoDocumentoId = documento.ConsultaEstadoDocumento,
                                        Acciones = ToJson(documento.Acciones),
                                        FechaCreacion = DateTime.Now
                                    };
                                    this.repositorio.Agregar(controlDeBoletoTracking);
                                    this.repositorio.GuardarCambios();
                                }
                                else
                                {
                                    existe.CUIT = documento.EnPoderDe?.CUIT.ToString();
                                    existe.RazonSocial = documento.EnPoderDe?.RazonSocial;
                                    existe.EstadoDocumentoId = documento.ConsultaEstadoDocumento;
                                    existe.Acciones = ToJson(documento.Acciones);
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
        public void EstablecerEstadoBoleto(int controlDeBoletoId)
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

            bool esPlanCanje = contrato.PlanCanje != null ? (bool)contrato.PlanCanje : false;

            if (!certificaciones.Any() && !esSinBoleto)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                return;
            }

            var tipoObleaBolsa = repositorio.Listar<TipoOblea>(x => x.Codigo == "O")
                                            .FirstOrDefault();
            var tipoObleaCanje = repositorio.Listar<TipoOblea>(x => x.Codigo == "F")
                                            .FirstOrDefault();

            var tipoObleaArca = repositorio.Listar<TipoOblea>(x => x.Codigo == "A")
                                           .FirstOrDefault();

            bool tieneObleaBolsa =
                certificaciones.Any(x => x.TipoObleaId == tipoObleaBolsa.Id);

            bool tieneObleaCanje =
                certificaciones.Any(x => x.TipoObleaId == tipoObleaCanje.Id);

            bool tieneCodigoArca =
                certificaciones.Any(x => x.TipoObleaId == tipoObleaArca.Id && !string.IsNullOrEmpty(x.Oblea));

            bool sinNingunaOblea =!tieneObleaBolsa && !tieneObleaCanje;
            // Sin boleto
            if (esSinBoleto)
            {
                if (!tieneCodigoArca)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_CODIGO_ARCA);
                    return;
                }
            }

            // Requiere alguna Oblea 
            if (!operaSinOblea && sinNingunaOblea)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                return;
            }

            if (!esPlanCanje)
            {
                // Requiere Oblea Bolsa
                if (!operaSinOblea && !tieneObleaBolsa)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                    return;
                }
                if (tieneObleaBolsa)
                {
                    var obleaBolsa = certificaciones.FirstOrDefault(x => x.TipoObleaId == tipoObleaBolsa.Id);
                    var noTieneOblea = string.IsNullOrEmpty(obleaBolsa?.Oblea);
                    if (noTieneOblea)
                    {
                        ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                        return;
                    }
                }
            }
            else
            {
                // Requiere Oblea Canje
                if (!operaSinOblea && !tieneObleaCanje)
                {
                    ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                    return;
                }
                if (tieneObleaCanje)
                {
                    var obleaCanje = certificaciones.FirstOrDefault(x => x.TipoObleaId == tipoObleaCanje.Id);
                    var noTieneOblea = string.IsNullOrEmpty(obleaCanje?.Oblea);
                    if (noTieneOblea)
                    {
                        ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_OBLEA_BOLSA);
                        return;
                    }
                }
            }
            if (!tieneCodigoArca)
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.PENDIENTE_CODIGO_ARCA);
                return;
            }
            else
            {
                ActualizarEstado(controlBoleto, EnumControlDeBoletosEstado.CONTROLADO);
                return;
            }
        }

        private void ActualizarEstado(
           ControlDeBoletos boleto,
           EnumControlDeBoletosEstado estado)
        {
            if (boleto == null) return;

            var estadoId = (int)estado;

            var estadoEntidad = repositorio.Obtener<ControlDeBoletosEstado>(x => x.Id == estadoId);
            if (estadoEntidad == null)
            {
                logger.Error(
                    $"ControlDeBoletosEstado Id={estadoId} no existe en el catálogo. " +
                    $"Se aborta ActualizarEstado para ControlDeBoletos Id={boleto.Id}.");
                return;
            }

            // Setear SIEMPRE la FK escalar, no solo la navegación.
            // Así evitamos que EF genere UPDATE con ControlDeBoletosEstadoId = 0
            // si la nav property quedó en null por algún motivo (contexto compartido,
            // entidad ya trackeada, etc.).
            boleto.ControlDeBoletosEstadoId = estadoId;
            boleto.ControlDeBoletosEstado = estadoEntidad;

            repositorio.GuardarCambios();
        }
        #endregion

        #region Modificacion Masiva de Boletos
        public List<ControlDeBoletosParaModificarDto> GetBoletosParaModificar(ControlDeBoletosParaModificarFiltroDto filtros)
        {
            var query = new TraerBoletosParaModificar(filtros ?? new ControlDeBoletosParaModificarFiltroDto());
            var data = repositorio.ObtenerConsultaEscalar(query);
            foreach (var item in data)
            {
                string cuitProveedor = item.EsCorredor ? item.CUITCorredor : item.CUITProveedor;
                string tipoProveedor = item.EsCorredor ? "CORR" : "PROV";
                bool operaSinOblea = VerificarOperaSinOblea(cuitProveedor, tipoProveedor) == "SI";
                item.OperaSinOblea = operaSinOblea;
            }

            if (!string.IsNullOrWhiteSpace(filtros?.ContratoSAP) && data != null && data.Count > 1)
            {
                var ordenContratos = filtros.ContratoSAP
                    .Split(';')
                    .Select(x => x?.Trim())
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.TrimStart('0').PadLeft(10, '0'))
                    .Distinct()
                    .ToList();

                if (ordenContratos.Count > 0)
                {
                    var mapaOrden = ordenContratos
                        .Select((contrato, indice) => new { contrato, indice })
                        .ToDictionary(x => x.contrato, x => x.indice);

                    data = data
                        .Select((item, indiceOriginal) => new
                        {
                            item,
                            indiceOriginal,
                            contratoNormalizado = (item?.ContratoSAP ?? string.Empty).Trim().TrimStart('0').PadLeft(10, '0')
                        })
                        .OrderBy(x => mapaOrden.ContainsKey(x.contratoNormalizado) ? mapaOrden[x.contratoNormalizado] : int.MaxValue)
                        .ThenBy(x => x.indiceOriginal)
                        .Select(x => x.item)
                        .ToList();
                }
            }

            return data;
        }

        public Resultado GuardarBoletosParaModificarFechas(List<ControlDeBoletosParaModificarDto> boletos)
        {
            var resultado = new Resultado();

            try
            {
                if (boletos == null || boletos.Count == 0)
                {
                    return resultado;
                }

                var codigosBolsa = boletos
                    .SelectMany(x => new[] { x.BolsaSellado, x.PreCertificacionBolsa })
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Select(x => x.Trim())
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var mapaBolsas = codigosBolsa.Count == 0
                    ? new Dictionary<string, BolsaCompraNet>(StringComparer.OrdinalIgnoreCase)
                    : repositorio.Listar<BolsaCompraNet>(x => codigosBolsa.Contains(x.CodigoSap))
                        .Where(x => !string.IsNullOrWhiteSpace(x.CodigoSap))
                        .GroupBy(x => x.CodigoSap.Trim(), StringComparer.OrdinalIgnoreCase)
                        .ToDictionary(x => x.Key, x => x.First(), StringComparer.OrdinalIgnoreCase);

                var tipoObleaOId = repositorio.Obtener<TipoOblea>(x => x.Codigo == "O")?.Id ?? 0;

                var indiceContrato = 0;

                foreach (var boleto in boletos)
                {
                    indiceContrato++;

                    try
                    {
                        var resultadoSeguimiento = ProcesarModificacionMasivaSeguimiento(boleto, indiceContrato, mapaBolsas);
                        if (resultadoSeguimiento.HayError)
                        {
                            resultado.Errores.AddRange(resultadoSeguimiento.Errores);
                            continue;
                        }

                        var resultadoPreCertificacion = ProcesarModificacionMasivaPreCertificacion(boleto, indiceContrato, mapaBolsas, tipoObleaOId);
                        if (resultadoPreCertificacion.HayError)
                        {
                            resultado.Errores.AddRange(resultadoPreCertificacion.Errores);
                            continue;
                        }

                        EstablecerEstadoBoleto(boleto.ControlDeBoletosId);
                    }
                    catch (Exception exBoleto)
                    {
                        resultado.Errores.Add(new ErrorMessage
                        {
                            Item = indiceContrato,
                            Message = $"Error procesando boleto para el contrato {boleto?.ContratoSAP}: {exBoleto.Message}"
                        });
                        logger.Error($"Error GuardarBoletosParaModificarFechas para el contrato {boleto?.ContratoSAP}: {exBoleto.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                resultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex, "Error guardando modificación masiva de boletos");
            }

            return resultado;
        }

        private Resultado ProcesarModificacionMasivaSeguimiento(
            ControlDeBoletosParaModificarDto boleto,
            int indiceContrato,
            Dictionary<string, BolsaCompraNet> mapaBolsas)
        {
            var resultado = new Resultado();

            try
            {
                if (boleto == null)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Item = indiceContrato,
                        Message = "No se encontró el boleto a procesar."
                    });
                    return resultado;
                }

                var seguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == boleto.ControlDeBoletosId);
                if (seguimiento == null)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Item = indiceContrato,
                        Message = $"No se encontró el seguimiento para el contrato {boleto.ContratoSAP}."
                    });
                    return resultado;
                }

                BolsaCompraNet bolsa = null;
                if (!string.IsNullOrWhiteSpace(boleto.BolsaSellado))
                {
                    mapaBolsas.TryGetValue(boleto.BolsaSellado.Trim(), out bolsa);
                }
                var seguimientoBoleto = new ControlDeBoletosDatosSeguimientoDto
                {
                    Id = seguimiento.Id,
                    ControlDeBoletosId = seguimiento.ControlDeBoletosId,
                    BoletoSapId = seguimiento.BoletoSapId,
                    BoletoSapCaracter = seguimiento.BoletoSapCaracter,
                    BolsaCompraNetId = bolsa?.Id,
                    BolsaSellado = bolsa?.CodigoSap ?? string.Empty,
                    FechaRecepcionBoleto = boleto.FechaRecepBoleto,
                    FechaEnvioFisicoBolsa = boleto?.FechaEnvioFisicoBolsa,
                    FechaRecepcionBoletoOriginal = boleto?.FechaRecepcionBoletoOriginal,
                    FechaEnvioFirma = boleto.FechaEnviadoFirma,
                    FechaEnvioBolsa = boleto.FechaEnvioBolsa,
                    FechaEnvioAfip = boleto.FechaEnvioAfip,
                    FechaRecepcionFirma = boleto.FechaRecibFirma,
                    FechaRecepcionBolsa = boleto.FechaVueltaBolsa,
                    FechaRecepcionAfip = boleto.FechaVueltaAfip,
                    FechaEnvioSellado = boleto.FechaEnvioSellado,
                    ObsCtrlBoleto = seguimiento.ObsCtrlBoleto,
                    ObsCtrlBoleto2 = seguimiento.ObsCtrlBoleto2
                };

                var seguimientoControlDeBoletos = ConstruirSeguimientoDto(seguimientoBoleto);
                var resultadoRfc = seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);
                if (!string.IsNullOrWhiteSpace(resultadoRfc) && resultadoRfc.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Item = indiceContrato,
                        Message = $"Error en RFC SAP Seguimiento para el contrato {boleto.ContratoSAP}: {resultadoRfc}"
                    });
                    return resultado;
                }

                if (!string.IsNullOrWhiteSpace(boleto.BolsaSellado))
                {
                    seguimiento.BolsaCompraNet = bolsa;
                    seguimiento.BolsaSellado = boleto.BolsaSellado;
                }

                if (boleto.FechaRecepBoleto.HasValue)
                    seguimiento.FechaRecepcionBoleto = boleto.FechaRecepBoleto;
                if (boleto.FechaEnvioFisicoBolsa.HasValue)
                    seguimiento.FechaEnvioFisicoBolsa = boleto.FechaEnvioFisicoBolsa;
                if (boleto.FechaRecepcionBoletoOriginal.HasValue)
                    seguimiento.FechaRecepcionBoletoOriginal = boleto.FechaRecepcionBoletoOriginal; 
                if (boleto.FechaEnviadoFirma.HasValue)
                    seguimiento.FechaEnvioFirma = boleto.FechaEnviadoFirma;
                if (boleto.FechaEnvioBolsa.HasValue)
                    seguimiento.FechaEnvioBolsa = boleto.FechaEnvioBolsa;
                if (boleto.FechaEnvioAfip.HasValue)
                    seguimiento.FechaEnvioAfip = boleto.FechaEnvioAfip;
                if (boleto.FechaRecibFirma.HasValue)
                    seguimiento.FechaRecepcionFirma = boleto.FechaRecibFirma;
                if (boleto.FechaVueltaBolsa.HasValue)
                    seguimiento.FechaRecepcionBolsa = boleto.FechaVueltaBolsa;
                if (boleto.FechaVueltaAfip.HasValue)
                    seguimiento.FechaRecepcionAfip = boleto.FechaVueltaAfip;
                if (boleto.FechaEnvioSellado.HasValue)
                    seguimiento.FechaEnvioSellado = boleto.FechaEnvioSellado;

                seguimiento.FechaModificacion = DateTime.Now;
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Errores.Add(new ErrorMessage
                {
                    Item = indiceContrato,
                    Message = $"Error procesando seguimiento para el contrato {boleto?.ContratoSAP}: {ex.Message}"
                });
                logger.Error($"Error ProcesarModificacionMasivaSeguimiento para el contrato {boleto?.ContratoSAP}: {ex.Message}");
            }

            return resultado;
        }

        private Resultado ProcesarModificacionMasivaPreCertificacion(
            ControlDeBoletosParaModificarDto boleto,
            int indiceContrato,
            Dictionary<string, BolsaCompraNet> mapaBolsas,
            int tipoObleaOId)
        {
            var resultado = new Resultado();

            try
            {
                if (boleto == null)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Item = indiceContrato,
                        Message = "No se encontró el boleto a procesar."
                    });
                    return resultado;
                }

                BolsaCompraNet bolsa = null;
                if (!string.IsNullOrWhiteSpace(boleto.PreCertificacionBolsa))
                {
                    mapaBolsas.TryGetValue(boleto.PreCertificacionBolsa.Trim(), out bolsa);
                }
                var pre = repositorio.Obtener<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == boleto.ControlDeBoletosId && x.TipoOblea.Codigo == "O");
                if (pre == null)
                {
                    return resultado;
                }
                else
                {
                    if (!string.IsNullOrEmpty(boleto.Oblea) && !boleto.FechaCertificacion.HasValue && !boleto.FechaVencimientoCertificacion.HasValue && boleto.BolsaCompraNetId != null)
                    {
                        pre.ControlDeBoletosId = boleto.ControlDeBoletosId;
                        pre.Oblea = boleto.Oblea;
                        pre.FechaCertificacion = boleto.FechaCertificacion;
                        pre.FechaVencimiento = boleto.FechaVencimientoCertificacion;
                        pre.BolsaCompraNetId = bolsa?.Id ?? 0;
                        pre.FechaModificacion = DateTime.Now;
                        repositorio.GuardarCambios();
                        return resultado;
                    }
                }

                var ahora = DateTime.Now;
                var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto();
                var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto();

                datosCertificacionCabeceraDto.Contrato = boleto.ContratoSAP ?? string.Empty;
                datosCertificacionCabeceraDto.Fecha = ahora.ToString("yyyy-MM-dd");
                datosCertificacionCabeceraDto.Hora = ahora.ToString("HH:mm:ss");
                datosCertificacionCabeceraDto.Fijacion = string.Empty;
                datosCertificacionCabeceraDto.Usuario = PermisosHelper.ObtenerUsuario();
                datosCertificacionDetalleDto.Bolsa = bolsa?.CodigoSap ?? string.Empty;
                datosCertificacionDetalleDto.Oblea = boleto.Oblea;
                datosCertificacionDetalleDto.FeCertificacion = FormatDate(boleto.FechaCertificacion);
                datosCertificacionDetalleDto.FeVencCerti = FormatDate(boleto.FechaVencimientoCertificacion);
                datosCertificacionDetalleDto.Rechazado = string.Empty;
                datosCertificacionDetalleDto.Tipo = pre.TipoOblea?.Codigo ?? string.Empty;
                datosCertificacionCabeceraDto.Detalle = new List<RegistroDatosCertificacionControlDeBoletosDetalleDto> { datosCertificacionDetalleDto };

                var resultadoRfc = datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
                if (!string.IsNullOrWhiteSpace(resultadoRfc) && resultadoRfc.IndexOf("ERROR", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    resultado.Errores.Add(new ErrorMessage
                    {
                        Item = indiceContrato,
                        Message = $"Error en RFC SAP PreCertificación para el contrato {boleto.ContratoSAP}: {resultadoRfc}"
                    });
                    return resultado;
                }

                if (!string.IsNullOrWhiteSpace(boleto.Oblea))
                    pre.Oblea = boleto.Oblea;
                if (boleto.FechaCertificacion.HasValue)
                    pre.FechaCertificacion = boleto.FechaCertificacion;
                if (boleto.FechaVencimientoCertificacion.HasValue)
                    pre.FechaVencimiento = boleto.FechaVencimientoCertificacion;
                if (!string.IsNullOrWhiteSpace(boleto.PreCertificacionBolsa) && bolsa != null)
                    pre.BolsaCompraNetId = bolsa.Id;

                pre.FechaModificacion = DateTime.Now;
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                resultado.Errores.Add(new ErrorMessage
                {
                    Item = indiceContrato,
                    Message = $"Error procesando pre-certificación para el contrato {boleto?.ContratoSAP}: {ex.Message}"
                });
                logger.Error($"Error ProcesarModificacionMasivaPreCertificacion para el contrato {boleto?.ContratoSAP}: {ex.Message}");
            }

            return resultado;
        }
        #endregion

        #region Eliminar Control de Boletos
        public Resultado EliminarControlDeBoletos(EliminarControlDeBoletoDto eliminarControlDeBoleto)
        {
            var oResultado = new Resultado();
            try
            {
                var datosPreCertificacion = repositorio.Listar<ControlDeBoletosPreCertificacion>(x => x.ControlDeBoletosId == eliminarControlDeBoleto.ControlDeBoletosId);
                var datosSeguimiento = repositorio.Listar<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == eliminarControlDeBoleto.ControlDeBoletosId);

                NotificarEliminacionPreCertificacionesEnSap(datosPreCertificacion, eliminarControlDeBoleto.ContratoSAP);
                NotificarEliminacionSeguimientosEnSap(datosSeguimiento, eliminarControlDeBoleto.ContratoSAP);

                RemoverYGuardar(datosPreCertificacion);
                RemoverYGuardar(datosSeguimiento);

                this.logDataAgroManager.LogCambiosControlBoletos(datosPreCertificacion, TipoAccionLogDataAgro.Eliminar, datosPreCertificacion.FirstOrDefault()?.Id ?? 0, "Eliminar de Certificacion - Control de Boletos");
                this.logDataAgroManager.LogCambiosControlBoletos(datosSeguimiento, TipoAccionLogDataAgro.Eliminar, datosSeguimiento.FirstOrDefault()?.Id ?? 0, "Eliminar de Seguimiento - Control de Boletos");
                this.EstablecerEstadoBoleto(eliminarControlDeBoleto.ControlDeBoletosId);

                return oResultado;
            }
            catch (Exception ex)
            {
                oResultado.Errores.Add(new ErrorMessage { Message = ex.Message });
                logger.Error(ex.Message);
                return oResultado;
            }
        }

        private void NotificarEliminacionPreCertificacionesEnSap(List<ControlDeBoletosPreCertificacion> certificaciones, string contratoSap)
        {
            foreach (var certificacion in certificaciones)
            {
                var tipoObleaCodigo = certificacion.TipoOblea?.Codigo;
                if (string.IsNullOrWhiteSpace(tipoObleaCodigo))
                {
                    tipoObleaCodigo = repositorio.Obtener<TipoOblea>(certificacion.TipoObleaId)?.Codigo ?? string.Empty;
                }

                var datosCertificacionDetalleDto = new RegistroDatosCertificacionControlDeBoletosDetalleDto
                {
                    Bolsa = string.Empty,
                    FeCertificacion = string.Empty,
                    FeVencCerti = string.Empty,
                    Oblea = string.Empty,
                    Rechazado = string.Empty,
                    Tipo = tipoObleaCodigo
                };

                var datosCertificacionCabeceraDto = new RegistroDatosCertificacionControlDeBoletosDto
                {
                    Contrato = contratoSap,
                    Usuario = PermisosHelper.ObtenerUsuario(),
                    Fecha = string.Empty,
                    Hora = string.Empty,
                    Fijacion = string.Empty,
                    Detalle = new List<RegistroDatosCertificacionControlDeBoletosDetalleDto> { datosCertificacionDetalleDto }
                };

                datosCertificacionControlBoletoAgent.RegistrarDatosCertificacion(datosCertificacionCabeceraDto);
            }
        }

        private void NotificarEliminacionSeguimientosEnSap(List<ControlDeBoletosSeguimiento> seguimientos, string contratoSap)
        {
            foreach (var _ in seguimientos)
            {
                seguimientoControlBoletoAgent.RegistrarSeguimiento(CrearSeguimientoVacioParaSap(contratoSap));
            }
        }

        private static SeguimientoControlDeBoletosDto CrearSeguimientoVacioParaSap(string contratoSap)
        {
            return new SeguimientoControlDeBoletosDto
            {
                Bolsa = string.Empty,
                BolsaSellado = string.Empty,
                Contrato = contratoSap,
                FechaEnvioFirma = string.Empty,
                FechaEnvioSellado = string.Empty,
                FechaEnvioAfip = string.Empty,
                FechaEnvioBolsa = string.Empty,
                FechaRecepBoleto = string.Empty,
                FechaRecepcionFirma = string.Empty,
                FechaRecepcionAfip = string.Empty,
                FechaRecepcionBolsa = string.Empty,
                FecAcopio = string.Empty,
                Fecha = string.Empty,
                Hora = string.Empty,
                ObsCtrlBoleto = string.Empty,
                ObsCtrlBoleto2 = string.Empty,
                RechazadoAfip = string.Empty,
                TipoBoleto = string.Empty,
                Usuario = PermisosHelper.ObtenerUsuario()
            };
        }

        private void RemoverYGuardar<T>(List<T> entidades) where T : class
        {
            if (entidades == null || !entidades.Any())
                return;

            foreach (var entidad in entidades)
            {
                repositorio.Remover(entidad);
            }

            repositorio.GuardarCambios();
        }

        #endregion

        #region Descargar PDF Confirma
        public ConfirmaDocumentoRegistradoDto ObtenerDocumentoConfirma(int controlDeBoletoId)
        {
            var controlDeBoleto = repositorio.Obtener<ControlDeBoletos>(x => x.Id == controlDeBoletoId);
            var negocio = repositorio.Obtener<Negocio>(x => x.Id == controlDeBoleto.NegocioId);
            string documento = controlDeBoleto.AltaIdDocumentoConfirma.ToString();
            string bolsa = negocio.Bolsa?.CodigoConfirma ?? string.Empty;
            string cuit = negocio.CorredorId > 0 ? negocio.Corredor.CUIT : negocio.Proveedor.CUIT;
            var documentoConfirmaPDF = confirmaConsultaDocumentosRegistradosAgent.ConfirmaConsultaDocumentosRegistrados(bolsa, documento, cuit);
            return documentoConfirmaPDF;
        }
        #endregion

        #region Metodos Privados
        private string ObtenerEstadoBoleto(BasicoBoleto boleto)
        {
            var mensaje = boleto.Estado_Version;
            try
            {
                boleto.FijacionSAP = " ";

                var cacheKey = $"ControlBoleto_EstadoBoleto_{boleto.ContratoSAP}";
                var consultaBoleto = HttpRuntime.Cache[cacheKey] as DatosEstadoBoletoDto;

                if (consultaBoleto == null)
                {
                    consultaBoleto = oConsultarEstadoBoletoAgent.EstadoBoleto(boleto.ContratoSAP, boleto.FijacionSAP ?? string.Empty);
                    HttpRuntime.Cache.Insert(cacheKey, consultaBoleto, null,
                        DateTime.UtcNow.AddSeconds(30), System.Web.Caching.Cache.NoSlidingExpiration);
                }

                var resultadoVersion = EstadoBoletoVersionHelper.Evaluar(consultaBoleto, boleto.Version, mensaje);
                mensaje = resultadoVersion.Estado;

                if (resultadoVersion.TieneInconsistencia)
                {
                    logger.Info($"Control de Boleto - Error al consultar el status del contrato SAP {boleto.NegocioSAP}, Las Versiones No Coinciden.");
                }
            }
            catch (Exception ex)
            {
                logger.Info($"Control de Boleto - Listar Negocios - Error al consultar el status del contrato SAP {boleto.NegocioSAP}, Mensaje: {ex.Message}.");
            }

            return mensaje;
        }

        private static IQueryable<ControlDeBoletosConsultaDto> AplicarOrdenControlBoletos(IQueryable<ControlDeBoletosConsultaDto> query, List<ControlBoletosSortDescriptor> sort)
        {
            if (sort != null && sort.Any())
            {
                var orderBy = string.Join(",", sort.Select(s => s.Field + (s.Dir == "desc" ? " descending" : " ascending")));
                return query.OrderBy(orderBy);
            }

            return query;
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
            string tipoBoleto = string.Empty;
            string bolsa = string.Empty;

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
            if (bolsaEntity != null)
                bolsa = bolsaEntity.CodigoSap;

            var ahora = DateTime.Now;

            return new SeguimientoControlDeBoletosDto
            {
                Bolsa = bolsa,
                BolsaSellado = controlDeBoletosDatosSeguimiento.BolsaSellado,
                Contrato = contrato,
                FecAcopio = controlDeBoletosDatosSeguimiento.FechaRecepcionBoletoOriginal?.ToString("yyyy-MM-dd"),
                Fecha = ahora.ToString("yyyy-MM-dd"),
                Hora = ahora.ToString("HH:mm:ss"),
                TipoBoleto = controlDeBoletosDatosSeguimiento.BoletoSapCaracter,
                Usuario = PermisosHelper.ObtenerUsuario(),
                FechaRecepBoleto = controlDeBoletosDatosSeguimiento.FechaRecepcionBoleto?.ToString("yyyy-MM-dd"),
                FechaEnvioFirma = controlDeBoletosDatosSeguimiento.FechaEnvioFirma?.ToString("yyyy-MM-dd"),
                FechaEnvioAfip = controlDeBoletosDatosSeguimiento.FechaEnvioAfip?.ToString("yyyy-MM-dd"),
                FechaEnvioBolsa = controlDeBoletosDatosSeguimiento.FechaEnvioBolsa?.ToString("yyyy-MM-dd"),
                FechaRecepcionFirma = controlDeBoletosDatosSeguimiento.FechaRecepcionFirma?.ToString("yyyy-MM-dd"),
                FechaRecepcionBolsa = controlDeBoletosDatosSeguimiento.FechaRecepcionBolsa?.ToString("yyyy-MM-dd"),
                FechaRecepcionAfip = controlDeBoletosDatosSeguimiento.FechaRecepcionAfip?.ToString("yyyy-MM-dd"),
                FechaEnvioSellado = controlDeBoletosDatosSeguimiento.FechaEnvioSellado?.ToString("yyyy-MM-dd"),
                ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto,
                ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2,
                RechazadoAfip = controlDeBoletosDatosSeguimiento.RechazadoAfip
            };
        }
        private void PersistirDatosDeSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(x => x.ControlDeBoletosId == controlDeBoletosDatosSeguimiento.ControlDeBoletosId);

            if (datosSeguimiento != null)
            {
                controlDeBoletosDatosSeguimiento.Id = datosSeguimiento.Id;
                ActualizarSeguimientoLocal(controlDeBoletosDatosSeguimiento);
            }
            else
            {
                InsertarSeguimientoLocal(controlDeBoletosDatosSeguimiento);
            }
        }
        private static bool TieneDatosSeguimiento(ControlDeBoletosDatosSeguimientoDto dto)
        {
            if (dto == null)
                return false;

            return dto.BoletoSapId != null ||
                   dto.BolsaCompraNetId != null ||
                   !string.IsNullOrWhiteSpace(dto.BolsaSellado) ||
                   !string.IsNullOrWhiteSpace(dto.BoletoSapCaracter) ||
                   dto.FechaRecepcionBoleto.HasValue ||
                   dto.FechaRecepcionBoletoOriginal.HasValue ||
                   dto.FechaEnvioFisicoBolsa.HasValue ||
                   dto.FechaEnvioFirma.HasValue ||
                   dto.FechaEnvioBolsa.HasValue ||
                   dto.FechaEnvioAfip.HasValue ||
                   dto.FechaRecepcionFirma.HasValue ||
                   dto.FechaRecepcionBolsa.HasValue ||
                   dto.FechaRecepcionAfip.HasValue ||
                   dto.FechaEnvioSellado.HasValue ||
                   !string.IsNullOrWhiteSpace(dto.ObsCtrlBoleto) ||
                   !string.IsNullOrWhiteSpace(dto.ObsCtrlBoleto2);
        }
        private void MapearSeguimientoDesdeDto(ControlDeBoletosSeguimiento entidad, ControlDeBoletosDatosSeguimientoDto dto)
        {
            entidad.BoletoSapId = dto.BoletoSapId;
            entidad.BoletoSap = dto.BoletoSapId != null ? repositorio.Obtener<BoletoSap>(dto.BoletoSapId.Value) : null;
            entidad.BoletoSapCaracter = dto.BoletoSapCaracter;
            entidad.BolsaCompraNetId = dto.BolsaCompraNetId;
            entidad.BolsaCompraNet = dto.BolsaCompraNetId != null ? repositorio.Obtener<BolsaCompraNet>(dto.BolsaCompraNetId.Value) : null;
            entidad.BolsaSellado = dto.BolsaSellado;
            entidad.FechaRecepcionBoleto = dto.FechaRecepcionBoleto;
            entidad.FechaEnvioFisicoBolsa = dto.FechaEnvioFisicoBolsa;
            entidad.FechaRecepcionBoletoOriginal = dto.FechaRecepcionBoletoOriginal;
            entidad.FechaEnvioFirma = dto.FechaEnvioFirma;
            entidad.FechaEnvioBolsa = dto.FechaEnvioBolsa;
            entidad.FechaEnvioAfip = dto.FechaEnvioAfip;
            entidad.FechaRecepcionFirma = dto.FechaRecepcionFirma;
            entidad.FechaRecepcionBolsa = dto.FechaRecepcionBolsa;
            entidad.RechazadoAfip = dto.RechazadoAfip;
            entidad.FechaRecepcionAfip = dto.FechaRecepcionAfip;
            entidad.FechaEnvioSellado = dto.FechaEnvioSellado;
            entidad.ObsCtrlBoleto = dto.ObsCtrlBoleto;
            entidad.ObsCtrlBoleto2 = dto.ObsCtrlBoleto2;
        }
        private void ActualizarSeguimientoLocal(ControlDeBoletosDatosSeguimientoDto controlDeBoletosDatosSeguimiento)
        {
            var datosSeguimiento = repositorio.Obtener<ControlDeBoletosSeguimiento>(controlDeBoletosDatosSeguimiento.Id);

            if (!TieneDatosSeguimiento(controlDeBoletosDatosSeguimiento))
            {
                repositorio.Remover(datosSeguimiento);
                repositorio.GuardarCambios();
                this.logDataAgroManager.LogCambiosControlBoletos(controlDeBoletosDatosSeguimiento, TipoAccionLogDataAgro.Eliminar, datosSeguimiento.Id, "Eliminacion de Seguimiento - Control de Boletos");
                EstablecerEstadoBoleto(datosSeguimiento.ControlDeBoletosId);
                return;
            }

            MapearSeguimientoDesdeDto(datosSeguimiento, controlDeBoletosDatosSeguimiento);
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
                FechaCreacion = DateTime.Now,
            };
            MapearSeguimientoDesdeDto(datosSeguimiento, controlDeBoletosDatosSeguimiento);
            repositorio.Agregar(datosSeguimiento);
            repositorio.GuardarCambios();
            EstablecerEstadoBoleto(datosSeguimiento.ControlDeBoletosId);
            this.logDataAgroManager.LogCambiosControlBoletos(controlDeBoletosDatosSeguimiento, TipoAccionLogDataAgro.Crear, datosSeguimiento.Id, "Registro de Seguimiento - Control de Boletos");
        }
        #endregion
    }
}
