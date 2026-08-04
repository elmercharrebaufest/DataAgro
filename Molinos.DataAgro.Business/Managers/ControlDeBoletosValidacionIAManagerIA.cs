using Molinos.DataAgro.Business.ClausulasBoleto;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletos;
using Molinos.DataAgro.Entities.Dto.ControlDeBoletosValidacionIA;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Interfaces.Agent;
using Molinos.DataAgro.Interfaces.Clausulas;
using Molinos.DataAgro.Interfaces.Managers;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using NLog;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Results;

namespace Molinos.DataAgro.Business.Managers
{
    public class ControlDeBoletosValidacionIAManagerIA: IControlDeBoletosValidacionIAManagerIA
    {
        private readonly IRepositorio repositorio;
        private readonly ILogger logger;
        private readonly IMailManager mailManager;
        private readonly IServicioClausulasCartaOferta servicioClausulasCartaOferta;
        private readonly IServicioClausulasBoletoFisico servicioClausulasBoletoFisico;
        private readonly IServicioClausulasGenericos servicioClausulasGenericos;
        private readonly IServicioClausulasConfirma servicioClausulaConfirma;

        public ControlDeBoletosValidacionIAManagerIA(ILogger logger, IRepositorio repositorio, IMailManager mailManager, IServicioClausulasCartaOferta servicioClausulasCartaOferta, IServicioClausulasBoletoFisico servicioClausulasBoletoFisico, IServicioClausulasGenericos servicioClausulasGenericos, IServicioClausulasConfirma servicioClausulaConfirma)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.mailManager = mailManager;
            this.servicioClausulasCartaOferta = servicioClausulasCartaOferta;
            this.servicioClausulasBoletoFisico = servicioClausulasBoletoFisico;
            this.servicioClausulasGenericos = servicioClausulasGenericos;
            this.servicioClausulaConfirma = servicioClausulaConfirma;
        }

        public List<ValidacionBoletosEstadoDto> ListarEstados()
        {
            List<ValidacionBoletosEstadoDto> listarEstados = new List<ValidacionBoletosEstadoDto>();
            listarEstados = repositorio.Listar<ValidacionBoletosEstado, ValidacionBoletosEstadoDto>(x => new ValidacionBoletosEstadoDto()
            {
                Id = x.Id,
                Descripcion = x.Descripcion
            }).OrderBy(x => x.Id).ToList();
            return listarEstados;
        }
        public List<ValidacionDeBoletosIAConsultaDto> GetValidacionBoletosPendientes(ValidacionBoletoFiltroBusquedaDto filtros)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerValidacionBoletosPendientes(filtros));
        }

        public List<ValidacionDeBoletosIADatosContratoDto> GetValidacionBoletoSap(string contratoSAP)
        {
            List<ValidacionDeBoletosIADatosContratoDto> datosContrato = null;

            var negocio = repositorio.Obtener<Negocio>(x=> x.ContratoSAP == contratoSAP);
            if (negocio == null)
            {
                throw new Exception($"No se encontró un negocio con el contrato SAP: {contratoSAP}");
            }
            var controlBoleto = repositorio.Obtener<ControlDeBoletos>(x => x.NegocioId == negocio.Id);
            if (controlBoleto == null)
            {
                throw new Exception($"No se encontró un control de boletos para contrato : {contratoSAP}");
            }

            datosContrato = new List<ValidacionDeBoletosIADatosContratoDto>();
            datosContrato.Add(new ValidacionDeBoletosIADatosContratoDto()
            {
                TipoBoleto = negocio.Boleto != null ? negocio.Boleto.Descripcion : string.Empty,
                Bolsa = negocio.Bolsa != null ? negocio.Bolsa.Descripcion : string.Empty
            });
            return datosContrato;
        }

        #region Metodos Publicos para el Api
        public ValidacionDeBoletosIADatosContratoDto ObtenerDatosDeContrato(string contratoSAP)
        {
            ValidacionDeBoletosIADatosContratoDto datosContrato = null;
            var contrato = repositorio.ObtenerConsultaEscalar(new TraerContratoBoletoPorSAP(contratoSAP));
            if (contrato != null)
            {
                datosContrato = new ValidacionDeBoletosIADatosContratoDto();
                datosContrato.Material = contrato.Material;
                datosContrato.Precio = contrato.Precio;
                datosContrato.Destino = contrato.DestinoDescripcion;
                datosContrato.Cosecha = contrato.Campania.Replace("-","/");
                datosContrato.TipoBoleto = contrato.BoletoDescripcion;
                datosContrato.FechaOperacion = contrato.FechaOperacion != null ? contrato.FechaOperacion.Value.ToString("dd/MM/yyyy") : string.Empty;
                datosContrato.PeriodoEntrega = contrato.FechaDesde.Value.ToString("dd/MM/yyyy") + " - " + contrato.FechaHasta.Value.ToString("dd/MM/yyyy");
                datosContrato.CuitVendedor = contrato.Cuit;
                datosContrato.CuitCorredor = contrato.CorredorId > 0 ? contrato.CUITCorredor : string.Empty;
                datosContrato.Moneda = contrato.MonedaId.Trim();
                datosContrato.ProvinciaOrigen = contrato.Provincia;
                datosContrato.LocalidadOrigen = contrato.Localidad;
                datosContrato.Kilos = contrato.Cantidad;
                datosContrato.FechaFijacionDesde = contrato.DesdeFijacion != null ? contrato.DesdeFijacion.Value.ToString("dd/MM/yyyy") : string.Empty;
                datosContrato.FechaFijacionHasta = contrato.HastaFijacion != null ? contrato.HastaFijacion.Value.ToString("dd/MM/yyyy") : string.Empty;
                datosContrato.CantidadMinFijacion = contrato.KgMinimo > 0 ? $"{MetodosUtiles.NumeroConSeparadores(contrato.KgMinimo)} Kg" : string.Empty;
                datosContrato.CantidadMaxFijacion = contrato.KgMaximo > 0 ? $"{MetodosUtiles.NumeroConSeparadores(contrato.KgMaximo)} Kg" : string.Empty;
                datosContrato.Bolsa = contrato.BolsaDescripcion;
                datosContrato.Clasificacion = contrato.ClasificacionDescripcion;
                datosContrato.Consignatario = contrato.Consignatario != null && contrato.Consignatario.Value ? true : false;
                datosContrato.TipoNegocio = contrato.TipoNegocio;
            }
            return datosContrato;
        }
        public List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulas(string contratoSAP)
        {
            if (string.IsNullOrWhiteSpace(contratoSAP))
            {
                return new List<ValidacionDeBoletosIAResultadoClausulaDto>();
            }

            // Optimización: usar una sola consulta en lugar de dos
            var basicoContrato = repositorio.ObtenerConsultaEscalar(
                new TraerContratoBoletoPorSAP(contratoSAP)
            );

            if (basicoContrato == null)
            {
                logger.Warn($"Contrato no encontrado: {contratoSAP}");
                return new List<ValidacionDeBoletosIAResultadoClausulaDto>();
            }

            Func<BasicoContrato, List<ValidacionDeBoletosIAResultadoClausulaDto>> obtenerPorBoleto = null;

            switch (basicoContrato.BoletoId)
            {
                case (int)EnumBoletoCompraNet.FISICO:
                    obtenerPorBoleto = ObtenerClausulasBoletoFisico;
                    break;
                case (int)EnumBoletoCompraNet.CARTA_OFERTA:
                    obtenerPorBoleto = ObtenerClausulasCartaOferta;
                    break;
                case (int)EnumBoletoCompraNet.CONFIRMA:
                    obtenerPorBoleto = ObtenerClausulasConfirma;
                    break;
            }

            return obtenerPorBoleto != null
                ? obtenerPorBoleto(basicoContrato)
                : new List<ValidacionDeBoletosIAResultadoClausulaDto>();
        }

        #endregion

        #region Metodos Privados
        private List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulasConfirma(BasicoContrato basico)
        {
            var clausulas = repositorio.Listar<ClausulaConfirma>();
            return ProcesorClausulas(clausulas, basico, c => this.servicioClausulaConfirma.DevolverClausulas((dynamic)c));
        }
        private List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulasCartaOferta(BasicoContrato basico)
        {
            var clausulas = repositorio.Listar<ClausulaCartaOferta>();
            return ProcesorClausulas(clausulas, basico, c => this.servicioClausulasCartaOferta.DevolverClausulas((dynamic)c));
        }
        private List<ValidacionDeBoletosIAResultadoClausulaDto> ObtenerClausulasBoletoFisico(BasicoContrato basico)
        {
            var clausulas = repositorio.Listar<ClausulaBoletoFisico>();
            return ProcesorClausulas(clausulas, basico, c => this.servicioClausulasBoletoFisico.DevolverClausulas((dynamic)c));
        }

        /// <summary>
        /// Procesa un conjunto de cláusulas genéricas, ejecutando el servicio correspondiente
        /// </summary>
        private List<ValidacionDeBoletosIAResultadoClausulaDto> ProcesorClausulas<T>(
            IEnumerable<T> clausulas, 
            BasicoContrato basico, 
            Func<T, ResultadoClausula> devolverClausula) where T : class
        {
            Inicializar(basico);
            var result = new List<ValidacionDeBoletosIAResultadoClausulaDto>();
            var orden = 1;

            foreach (var item in clausulas)
            {
                dynamic clausulaConBasico = item;
                clausulaConBasico.Basico = basico;
                
                var clausula = devolverClausula(item);
                if (clausula != null && !string.IsNullOrEmpty(clausula.Texto))
                {
                    result.Add(new ValidacionDeBoletosIAResultadoClausulaDto
                    {
                        Texto = clausula.Texto,
                        Orden = orden++
                    });
                }
            }

            return AgregarClausulasGenericas(basico, orden, result);
        }
        private List<ValidacionDeBoletosIAResultadoClausulaDto> AgregarClausulasGenericas(
            BasicoContrato basico, 
            int ordenInicial, 
            List<ValidacionDeBoletosIAResultadoClausulaDto> result)
        {
            var clausulas = repositorio.Listar<ClausulaGenericos>();
            var orden = ordenInicial;

            foreach (var item in clausulas)
            {
                item.Basico = basico;
                var clausula = this.servicioClausulasGenericos.DevolverClausulas(item);
                if (clausula != null && !string.IsNullOrEmpty(clausula.Texto))
                {
                    result.Add(new ValidacionDeBoletosIAResultadoClausulaDto
                    {
                        Texto = clausula.Texto,
                        Orden = orden++
                    });
                }
            }
            return result;
        }
        private void Inicializar(BasicoContrato basico)
        {
            if (basico.Descuentos == null)
            {
                basico.Descuentos = CrearDescuentosPorDefecto();
            }
            else
            {
                AgregarDescuentosFaltantes(basico.Descuentos);
            }
        }
        private List<DescuentoBonificacionDto> CrearDescuentosPorDefecto()
        {
            return new List<DescuentoBonificacionDto>
            {
                new DescuentoBonificacionDto
                {
                    Importe = 0,
                    Porcentaje = 0,
                    TipoPeriodoDBId = (int)EnumTipoPeriodoDB.GENERALES,
                    TipoDBId = (int)EnumTipoDB.POR_FUERA_DEL_PRECIO
                },
                new DescuentoBonificacionDto
                {
                    Importe = 0,
                    Porcentaje = 0,
                    TipoPeriodoDBId = (int)EnumTipoPeriodoDB.GENERALES,
                    TipoDBId = (int)EnumTipoDB.SOBRE_EL_PRECIO
                }
            };
        }
        private void AgregarDescuentosFaltantes(List<DescuentoBonificacionDto> descuentos)
        {
            var tiposRequeridos = new[]
            {
                new { TipoPeriodo = (int)EnumTipoPeriodoDB.GENERALES, Tipo = (int)EnumTipoDB.POR_FUERA_DEL_PRECIO },
                new { TipoPeriodo = (int)EnumTipoPeriodoDB.GENERALES, Tipo = (int)EnumTipoDB.SOBRE_EL_PRECIO }
            };

            foreach (var tipoRequerido in tiposRequeridos)
            {
                var existe = descuentos.Any(x => 
                    x.TipoPeriodoDBId == tipoRequerido.TipoPeriodo && 
                    x.TipoDBId == tipoRequerido.Tipo);

                if (!existe)
                {
                    descuentos.Add(new DescuentoBonificacionDto
                    {
                        Importe = 0,
                        Porcentaje = 0,
                        TipoPeriodoDBId = tipoRequerido.TipoPeriodo,
                        TipoDBId = tipoRequerido.Tipo
                    });
                }
            }
        }       
        #endregion

    }
}
