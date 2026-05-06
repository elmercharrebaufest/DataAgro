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
using System.Linq;
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
            // Optimización: Pre-cargar ControlDeBoletos con filtros básicos antes de los joins
            var controlBoletosQuery = repositorio.Listar<ControlDeBoletos>().AsQueryable();

            // FILTROS GENERALES - Aplicar temprano para reducir dataset
            if (!string.IsNullOrEmpty(filtros.ContratoSAPDesde))
                controlBoletosQuery = controlBoletosQuery.Where(cb => string.Compare(cb.Negocio.ContratoSAP, filtros.ContratoSAPDesde) >= 0);

            if (!string.IsNullOrEmpty(filtros.ContratoSAPHasta))
                controlBoletosQuery = controlBoletosQuery.Where(cb => string.Compare(cb.Negocio.ContratoSAP, filtros.ContratoSAPHasta) <= 0);

            if (filtros.MaterialId.HasValue)
                controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.MaterialId == filtros.MaterialId.Value);

            if (filtros.Proveedor.HasValue)
                controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.ProveedorId == filtros.Proveedor.Value);

            if (filtros.BolsaId.HasValue)
                controlBoletosQuery = controlBoletosQuery.Where(cb => cb.Negocio.BolsaId == filtros.BolsaId.Value);

            // Query principal con LEFT JOINs
            var query = from cb in controlBoletosQuery
                        join pre in repositorio.Listar<ControlDeBoletosPreCertificacion>()
                            on cb.Id equals pre.ControlDeBoletosId into preJoin
                        from pre in preJoin.DefaultIfEmpty()
                        join seg in repositorio.Listar<ControlDeBoletosSeguimiento>()
                            on cb.Id equals seg.ControlDeBoletosId into segJoin
                        from seg in segJoin.DefaultIfEmpty()
                        select new { cb, pre, seg };

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
                    TipoBoleto = x.cb.Negocio.Boleto.Descripcion,
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
                    query = query.Where(x => x.FechaCreacion <= filtros.FechaCargaHasta.Value);

                if (filtros.Proveedor.HasValue)
                    query = query.Where(x => x.Negocio.ProveedorId == filtros.Proveedor.Value);

                if (filtros.BolsaId.HasValue)
                    query = query.Where(x => x.Negocio.BolsaId == filtros.BolsaId.Value);

                if (filtros.ComercialId.HasValue)
                    query = query.Where(x => x.Negocio.ComercialId == filtros.ComercialId.Value);
            }



            // Optimización: Usar query LINQ con LEFT JOIN en lugar de N+1 queries
            var result = (from cb in query
                          join pre in repositorio.Listar<ControlDeBoletosPreCertificacion>()
                              on cb.Id equals pre.ControlDeBoletosId into preJoin
                          from pre in preJoin.DefaultIfEmpty()
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
                              PreCertificacionId = pre != null ? (int?)pre.Id : null,
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
                try
                {
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
                }
                catch (Exception ex)
                {
                    resultado.Errores.Add(new ErrorMessage()
                    {
                        Message = "Ocurrió un error al modificar el contrato en SAP"
                    });
                    logger.Error(ex.Message);
                    return resultado;
                }
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
                try
                {
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
                    seguimientoControlDeBoletos.FecAcopio = string.Empty;
                    seguimientoControlDeBoletos.Fecha = DateTime.Now.ToString("yyyy-MM-dd");
                    seguimientoControlDeBoletos.Hora = DateTime.Now.ToString("HH:mm:ss");
                    seguimientoControlDeBoletos.ObsCtrlBoleto = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto;
                    seguimientoControlDeBoletos.ObsCtrlBoleto2 = controlDeBoletosDatosSeguimiento.ObsCtrlBoleto2;
                    seguimientoControlDeBoletos.TipoBoleto = repositorio.Obtener<BoletoCompraNet>(controlDeBoletosDatosSeguimiento.BoletoCompraNetId).Id.ToString("D2");
                    seguimientoControlDeBoletos.Usuario = string.Empty;
                    seguimientoControlBoletoAgent.RegistrarSeguimiento(seguimientoControlDeBoletos);
                }
                catch (Exception ex) {
                    oResultado.Errores.Add(new ErrorMessage()
                    {
                        Message = "Error al registrar datos de seguimiento en SAP"
                    });
                    logger.Error(ex.Message);
                    return oResultado;
                }

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
                try
                {
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
                }
                catch (Exception ex) {
                    oResultado.Errores.Add(new ErrorMessage()
                    {
                        Message = "Error al registrar datos de certificación en SAP",
                    });
                    logger.Error(ex.Message);
                    return oResultado;
                }

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
                EstadoDocumentoId = x.EstadoDocumentoId,
                CUIT = x.CUIT,
                RazonSocial = x.RazonSocial,
                Acciones = x.Acciones,
            }).ToList();
        }
        #endregion

        #region Metodo Tareas Programadas
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
                                                                                  ));
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
                                        CUIT              = documento.EnPoderDe?.CUIT,
                                        RazonSocial       = documento.EnPoderDe?.RazonSocial,
                                        EstadoDocumentoId = documento.ConsultaEstadoDocumento,
                                        Acciones          = JsonSerializer.Serialize(documento.Acciones),
                                        FechaCreacion     = DateTime.Now
                                    };
                                    this.repositorio.Agregar(controlDeBoletoTracking);
                                    this.repositorio.GuardarCambios();
                                }
                                else
                                {
                                    existe.CUIT              = documento.EnPoderDe?.CUIT;
                                    existe.RazonSocial       = documento.EnPoderDe?.RazonSocial;
                                    existe.EstadoDocumentoId = documento.ConsultaEstadoDocumento;
                                    existe.Acciones          = JsonSerializer.Serialize(documento.Acciones);
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
