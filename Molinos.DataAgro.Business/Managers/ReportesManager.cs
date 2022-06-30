using Autofac.Extras.NLog;
using Kendo.DynamicLinq;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Resources;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;

namespace Molinos.DataAgro.Business.Managers
{
    public class ReportesManager : IReportesManager
    {
        private readonly IRepositorio repositorio;
        private readonly IComercialManager oComercial;
        private readonly ITipoDeCambioAgent tipoDeCambio;
        private ILogger logger;
        private readonly IContratosAPesificarAgent pesificarAgent;
        private readonly IMailManager mailManager;
        private readonly IHttpContextManager httpContextManager;

        public ReportesManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial,
            ITipoDeCambioAgent tipoDeCambio, IContratosAPesificarAgent pesificarAgent, IMailManager mailManager, IHttpContextManager httpContextManager)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComercial = oComercial;
            this.tipoDeCambio = tipoDeCambio;
            this.pesificarAgent = pesificarAgent;
            this.mailManager = mailManager;
            this.httpContextManager = httpContextManager;
        }

        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public Resultado GrabarReporte(Reportes oReporte)
        {
            var oEntityErrors = new Resultado();
            repositorio.Agregar(oReporte);
            try
            {
                repositorio.GuardarCambios();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                throw;
            }
            return oEntityErrors;
        }

        public ReportesDto ObtenerReporte(string identificador)
        {
            return repositorio.Obtener<Reportes, ReportesDto>(x => x.Identificador == identificador, x => new ReportesDto { Identificador = x.Identificador, Contenido = x.Contenido, FileName = x.FileName });
        }

        public DatosInicialesReportes TraerDatosIniciales(string idActiveDirectory)
        {
            int ComercialId = repositorio.Obtener<Comercial, int>(x => x.IdActiveDirectory == idActiveDirectory, x => x.ComercialId);

            var Datos = new DatosInicialesReportes
            {
                camp = repositorio.Listar<Campaña, CampañaQry>(x => new CampañaQry
                {
                    CampañaId = x.CampañaId,
                    Descripcion = x.Descripcion
                }, null, 3, "CampañaId", DirOrden.Desc)
            };

            //var query = repositorio.SelStore<FakeHome>("DataAgro_Comercial_TraerPorComerciales", 0, ComercialId);
            //Datos.come = query.Select(s => new ComercialQry() { ComercialId = s.Id, IdActiveDirectory = s.Nombre }).ToList();
            var lista = oComercial.ListarEquipo(idActiveDirectory);
            var equipo = PermisosHelper.Is(PermisosDataAgro.VerTodos) ? lista.Equipo : lista.EquipoReal;
            equipo.ForEach(x => Datos.come.Add(repositorio.Obtener<Comercial, ComercialQry>(y => y.ComercialId == x, y => new ComercialQry { ComercialId = x, Nombre = y.Apellido + " " + y.Nombres })));

            Datos.mat = repositorio.Listar<Material, MaterialesQry>(x => new MaterialesQry
            {
                Descripcion = x.Descripcion,
                MaterialId = x.MaterialId
            });

            Datos.provs = repositorio.Listar<Provincia, ProvinciaQry>(x => new ProvinciaQry
            {
                Nombre = x.Nombre,
                Provinciaid = x.ProvinciaId
            });

            Datos.segm = repositorio.Listar<Segmentacion, SegmentacionQry>(x => new SegmentacionQry
            {
                Descripcion = x.Descripcion,
                Grupo = x.Grupo,
                SegmentacionId = x.SegmentacionId
            });

            Datos.estic = repositorio.Listar<InformeComercialEstado, EstadoICQry>(x => new EstadoICQry
            {
                Descripcion = x.Descripcion,
                EstadoInformeId = x.EstadoInformeId
            });

            return Datos;
        }

        #region Compras

        #region Mapa
        public List<ResulIndicadores> TraerComprasMapa(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresComprasMapa", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultIndicadoresReportesmini> TraerComprasMapaExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultIndicadoresReportesmini>("DataAgro_IndicadoresComprasExportacionMapa", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #region Torta
        public List<ResulIndicadores> TraerComprasTorta(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresComprasTorta", 0, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultIndicadoresReportesTorta> TraerComprasTortaExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultIndicadoresReportesTorta>("DataAgro_IndicadoresComprasExportacionTorta", 0, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #region Barra
        public List<ResultComprasBarrasReportes> TraerComprasBarra(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("Reporte_ComprasBarra_Traer", 0, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultComprasBarrasReportesmini> TraerComprasBarraExportacion(ParamReportes oParamReportes, List<int> equipo)
        {

            return repositorio.SelStore<ResultComprasBarrasReportesmini>("Reporte_ComprasBarra_TraerExcel", 0, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #endregion

        #region Productiva

        #region Mapa
        public List<ResulIndicadores> TraerCapacidadProductivaMapa(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresProductiva", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultProduccionMapaReportes> TraerCapacidadProductivaMapaExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultProduccionMapaReportes>("DataAgro_IndicadoresExportacionMapaProductiva", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #region Barras
        public List<ResultComprasBarrasReportes> TraerCapacidadProductivaBarra(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("DataAgro_IndicadoresProductivaBarra", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultProduccionBarraReportes> TraerCapacidadProductivaBarraExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultProduccionBarraReportes>("DataAgro_IndicadoresExportacionBarraProductiva", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #endregion

        #region Acopio
        #region Mapa
        public List<ResulIndicadores> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresAcopio", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultAcopioMapaReportes> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultAcopioMapaReportes>("DataAgro_IndicadoresExportacionMapaAcopio", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion

        #region Barra
        public List<ResultComprasBarrasReportes> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("DataAgro_IndicadoresAcopioBarra", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }

        public List<ResultAcopioBarraReportes> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultAcopioBarraReportes>("DataAgro_IndicadoresExportacionBarraAcopio", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()));
        }
        #endregion
        #endregion

        #region Objetivos
        public List<ResultObjetivoGaugeReportes> TraerObjetivosGauge(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultObjetivoGaugeReportes>("DataAgro_Gauget_Traer", 0, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParamReportes.Cosecha, oParamReportes.Grano);
        }

        public List<ResultObjetivoGaugeReportes> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes, List<int> equipo)
        {
            return repositorio.SelStore<ResultObjetivoGaugeReportes>("DataAgro_Gauget_TraerExcel", 0, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParamReportes.Cosecha, oParamReportes.Grano);
        }
        #endregion

        public BaseDeDatosReturn TraerDatosGrillaBD(ParamReportes oParamReportes, List<int> equipo)
        {

            BaseDeDatosReturn datosGrilla = new BaseDeDatosReturn();

            datosGrilla.valoresGrilla = repositorio.SelStore<valoresGrilla>("DataAgro_IndicadoresBaseDeDatos_Traer", 0, oParamReportes.FechaDesde, oParamReportes.FechaHasta, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Cosecha, oParamReportes.Toneladas, oParamReportes.Comercial, string.Join(",", equipo.Select(n => n.ToString()).ToArray()), oParamReportes.Grano);

            datosGrilla.graficoBaseDatos = datosGrilla.valoresGrilla.GroupBy(x => x.Segmentación).Select(x => new graficoBaseDatos()
            {
                segmentacion = x.Key,
                Toneladas = 0   //x.Sum(y => y.Toneladas)
            }).ToList();

            datosGrilla.graficoBaseDatos.ForEach(x => x.Toneladas = datosGrilla.valoresGrilla.Where(y => y.Segmentación == x.segmentacion).Select(c => c.Cuit).Distinct().Count());

            datosGrilla.graficoBaseDatos = datosGrilla.graficoBaseDatos.OrderByDescending(x => x.Toneladas).ToList();

            return datosGrilla;
        }

        //BASEDEDATOS
        public List<ResulIndicadores> TraerBasedeDatos(ParamReportes oParamReportes)
        {
            return new List<ResulIndicadores>();
        }

        public ParamReportes TransformarFiltros(ParamReportes oParamReportes)
        {
            var properties = oParamReportes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<ExcelEncabezado> resultado = new List<ExcelEncabezado>();

            foreach (var prop in properties)
            {
                var nombre = prop.Name;
                var value = prop.GetValue(oParamReportes, null);

                if (nombre.ToLower() == "mes" && oParamReportes.Mes != null)
                {
                    oParamReportes.MES_ = ((LosMesesDelAño)oParamReportes.Mes).ToString();
                }

                if (nombre.ToLower() == "indicadores" && value != null)
                {
                    switch (oParamReportes.Indicadores)
                    {
                        case "compras":
                            oParamReportes.Indicadores = "Compras";
                            break;
                        case "objetivoscomercial":
                            oParamReportes.Indicadores = "Objetivos Comerciales";
                            break;
                        case "basededatos":
                            oParamReportes.Indicadores = "Base De Datos";
                            break;
                        case "capacidadproductiva":
                            oParamReportes.Indicadores = "Capacidad Productiva";
                            break;
                        case "capacidadacopio":
                            oParamReportes.Indicadores = "Capacidad De Acopio";
                            break;
                        default:
                            break;
                    }

                }

                if (nombre.ToLower() == "grano" && value != null)
                {
                    oParamReportes.GRANO_ = repositorio.Obtener<Material, string>(x => x.MaterialId == oParamReportes.Grano, x => x.Descripcion);
                }


                if (nombre.ToLower() == "provincia" && value != null)
                {
                    oParamReportes.PROVINCIA_ = repositorio.Obtener<Provincia, string>(x => x.ProvinciaId == oParamReportes.Provincia, x => x.Nombre);
                }


                if (nombre.ToLower() == "cosecha" && value != null)
                {
                    oParamReportes.COSECHA_ = repositorio.Obtener<Campaña, string>(x => x.CampañaId == oParamReportes.Cosecha, x => x.Descripcion);
                }

                if (nombre.ToLower() == "segmentacion" && value != null)
                {
                    var V1 = oParamReportes.Segmentacion.Split(',');
                    var segmenta = V1.ToList().Select(x => int.Parse(x)).ToList();
                    if (!segmenta.Any(x => x == 0))
                        oParamReportes.SEGMENTACION_ = string.Join(",", repositorio.Listar<Segmentacion, string>(x => x.Descripcion, x => segmenta.Contains(x.SegmentacionId)));
                }

                if (nombre.ToLower() == "objetivos" && value != null)
                {
                    oParamReportes.OBJETIVOS_ = repositorio.Obtener<Objetivo, string>(x => x.ObjetivoId == oParamReportes.Comercial, x => x.ToneladasObjetivos.ToString() + " Toneladas ");
                }

                if (nombre.ToLower() == "fechadesde" && value != null)
                {
                    oParamReportes.FECHADESDE_ = ((DateTime)oParamReportes.FechaDesde).ToString("dd/MM/yyyy");
                }
                if (nombre.ToLower() == "fechahasta" && value != null)
                {
                    oParamReportes.FECHAHASTA_ = ((DateTime)oParamReportes.FechaHasta).ToString("dd/MM/yyyy");
                }

                if (nombre.ToLower() == "toneladas" && value != null)
                {
                    switch ((int)oParamReportes.Toneladas)
                    {
                        case 1:
                            oParamReportes.TONELADAS_ = "Menos de 2500 Tn.";
                            break;
                        case 2:
                            oParamReportes.TONELADAS_ = "Entre 2500 y 5000 Tn.";
                            break;
                        case 3:
                            oParamReportes.TONELADAS_ = "Mas de 5000 Tn.";
                            break;
                        default:
                            break;
                    }
                }

                if (nombre.ToLower() == "comercial" && value != null)
                {
                    oParamReportes.COMERCIAL_ = repositorio.Obtener<Comercial, string>(x => x.ComercialId == oParamReportes.Comercial, x => x.Nombres + " " + x.Apellido);
                }

                if (nombre.ToLower() == "comercialactual" && value != null)
                {
                    oParamReportes.COMERCIALACTUAL_ = repositorio.Obtener<Comercial, string>(x => x.ComercialId == oParamReportes.ComercialActual, x => x.Nombres + " " + x.Apellido);
                }
            }
            return oParamReportes;
        }

        public List<ToneladasGranoTipoDto> TraerToneladasGranoTipo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var sojaToneladas = materialId.Contains(3) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(3, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            sojaToneladas.Material = "Soja";
            sojaToneladas.MaterialId = 3;
            var maizToneladas = materialId.Contains(1) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(1, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            maizToneladas.Material = "Maiz";
            maizToneladas.MaterialId = 1;
            var trigoCamaraToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            trigoCamaraToneladas.Material = "Trigo";
            trigoCamaraToneladas.MaterialId = 2;
            //var trigoCalidadToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, 2, centroId)) : new ToneladasGranoTipoDto();
            //trigoCalidadToneladas.Material = "Trigo Calidad";
            //trigoCalidadToneladas.MaterialId = 2;
            var girasolToneladas = materialId.Contains(4) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(4, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            girasolToneladas.Material = "Girasol";
            girasolToneladas.MaterialId = 4;
            var girasolAltoToneladas = materialId.Contains(5) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            girasolAltoToneladas.Material = "Girasol Alto Oleico";
            girasolAltoToneladas.MaterialId = 5;
            //var trigoGradoToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, 7, centroId)) : new ToneladasGranoTipoDto();
            //trigoGradoToneladas.Material = "Trigo Grado 2";
            //trigoGradoToneladas.MaterialId = 2;



            return new List<ToneladasGranoTipoDto>() { sojaToneladas, maizToneladas, trigoCamaraToneladas, /*trigoCalidadToneladas, trigoGradoToneladas,*/ girasolToneladas, girasolAltoToneladas };

        }
        public ReporteSojaSustDto TraerToneladasSojaSust(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerToneladasSojaSustentable(fechaDesde, fechaHasta, centroId));
        }
        public List<PosicionComprasDto> TraerPosicionCompras(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0, bool TraerPosicionMaterialCampaña = false, bool verFijaciones = true)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();
            var precioPizarra = repositorio.Listar<PrecioPizarra>(x => x.FechaHasta <= fechaHasta);
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var negocios = repositorio.Listar<Negocio, BasicoContrato>(x => new BasicoContrato
            {
                Id = x.Id,
                FechaDesde = DbFunctions.TruncateTime(x.FechaDesde),
                FechaHasta = DbFunctions.TruncateTime(x.FechaHasta),
                Fecha = DbFunctions.TruncateTime(x.Fecha),
                Cantidad = x.Cantidad,
                Precio = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? (x.PrecioNetoPonderado ?? 0) : x.Precio,
                Pizarra = x.Pizarra,
                TipoNegocioId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? 2 : x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                Campania = x.Campana.Descripcion,
                MonedaId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "USDM " : x.MonedaId,
                OcultarEnTablero = x.OcultarEnTablero,
                Estado = x.EstadoId,
                MaterialId = x.MaterialId,
                StandardCalidadId = x.StandardDeCalidadId,
                DestinoId = x.DestinoId,
                EsFason = x is Contrato ? (x as Contrato).EsFason : null,
                ContratoAcuerdoId = x is Contrato ? (x as Contrato).ContratoAcuerdoId : null,
                TrigoEspecial = x.TrigoEspecial,
                Posicion = x.Posicion,
                ContratoSAP = x.ContratoSAP,
                CantidadDeposito = x.CantidadDeposito,

            },
               x => x.OcultarEnTablero == false
               &&
               ((x is Contrato && DbFunctions.TruncateTime((x as Contrato).FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime((x as Contrato).FechaOperacion) <= fechaManana) ||
               (x is FijacionDePrecioContrato && DbFunctions.TruncateTime((x as FijacionDePrecioContrato).FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime((x as FijacionDePrecioContrato).FechaOperacion) <= fechaManana) ||
               (x is AgenteCompra && DbFunctions.TruncateTime((x as AgenteCompra).FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime((x as AgenteCompra).FechaOperacion) <= fechaManana) ||
               (!(x is Contrato) && !(x is FijacionDePrecioContrato) && !(x is AgenteCompra) && DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana))
               && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5 || x.EstadoId == 10)
               && (centroId == 0 || x.DestinoId == centroId)
               && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2 || x.TipoNegocioId == 3 || x.TipoNegocioId == 4 || x.TipoNegocioId == 6)
               && (x.TipoAgenteCompraId == null)
               && ((x is ContratoAcuerdo && (x as ContratoAcuerdo).TipoAgenteCompraId == null) || !(x is ContratoAcuerdo))
               //&& (((x is Contrato) && (x as Contrato).Canje != true) || !(x is Contrato))
               && (((x is Contrato) && (x as Contrato).PrestamoDevolucion != true) || !(x is Contrato))
               && (((x is Contrato) && (x as Contrato).Venta != true) || !(x is Contrato))
               && (((x is Contrato) && (x as Contrato).AnulaYReemplazaContratoId == null) || !(x is Contrato))

               && !(((x is FijacionDePrecioContrato) && (x as FijacionDePrecioContrato).Canje != true && (x as FijacionDePrecioContrato).Virtual != true && (x as FijacionDePrecioContrato).Contrato.Canje == true))


               && (((x is FijacionDePrecioContrato) && (x as FijacionDePrecioContrato).Canje != true) || !(x is FijacionDePrecioContrato))
               && (((x is FijacionDePrecioContrato) && (x as FijacionDePrecioContrato).TipoPosicionCBOTId != 3) || !(x is FijacionDePrecioContrato))
               );

            var negociosConDescarga = negocios.Where(a => a.TipoNegocioId == 2 && a.CantidadDeposito > 0).ToList();

            foreach (var x in negociosConDescarga)
            {
                var negocio = negocios.Where(y => y.Id == x.Id).Single();
                negocios.Add(new BasicoContrato
                {
                    Id = x.Id,
                    FechaDesde = x.FechaDesde,
                    FechaHasta = x.FechaHasta,
                    Fecha = x.Fecha,
                    Cantidad = x.CantidadDeposito.Value,
                    Precio = x.Precio,
                    Pizarra = x.Pizarra,
                    TipoNegocioId = 3,
                    CampanaMaterialId = x.CampanaMaterialId,
                    CampanaId = x.CampanaId,
                    Campania = x.Campania,
                    MonedaId = x.MonedaId,
                    OcultarEnTablero = x.OcultarEnTablero,
                    Estado = x.Estado,
                    MaterialId = x.MaterialId,
                    StandardCalidadId = x.StandardDeCalidadId,
                    DestinoId = x.DestinoId,
                    EsFason = x.EsFason,
                    ContratoAcuerdoId = x.ContratoAcuerdoId,
                    TrigoEspecial = x.TrigoEspecial,
                    Posicion = x.Posicion,
                    ContratoSAP = x.ContratoSAP,
                    CantidadDeposito = x.CantidadDeposito,
                });
                negocio.Cantidad -= negocio.CantidadDeposito.GetValueOrDefault(0);
            }


            if (verFijaciones == false)
            {
                negocios = negocios.Where(x => x.TipoNegocioId != 3).ToList();
            }
            var contratoSapFijaciones = negocios.Where(a => a.TipoNegocioId == 3 && a.ContratoSAP != null && a.ContratoSAP != "").Select(a => a.ContratoSAP).ToList();
            var contratosDeFijaciones = repositorio.Listar<Negocio>(x => x.TipoNegocioId == 1 && x.EstadoId == 5 && contratoSapFijaciones.Contains(x.ContratoSAP)).ToList();
            foreach (var fijacion in negocios.Where(a => a.TipoNegocioId == 3))
            {
                var contrato = contratosDeFijaciones.Where(a => a.ContratoSAP == fijacion.ContratoSAP).FirstOrDefault();
                if (contrato != null)
                {
                    fijacion.StandardCalidadId = contrato.StandardDeCalidadId;
                }
                else
                {
                    if (fijacion.StandardCalidadId == null)
                    {
                        fijacion.StandardCalidadId = fijacion.TrigoEspecial == true ? 7 : 1;
                    }
                }
            }
            var kilosPosicionSoja = materialId.Contains(3) ?
                (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(3, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) :
                TraerPosicionMaterial(3, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId)
                )
                : new List<PosicionKilos>();
            var posicionSoja = new PosicionComprasDto
            {
                Material = "Soja",
                MaterialId = 3,
                PosicionKilos = kilosPosicionSoja,
                Total = kilosPosicionSoja.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionMaiz = materialId.Contains(1) ?
                (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(1, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) :
                TraerPosicionMaterial(1, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId)
                )
                : new List<PosicionKilos>();
            var posicionMaiz = new PosicionComprasDto
            {
                Material = "Maiz",
                MaterialId = 1,
                PosicionKilos = kilosPosicionMaiz,
                Total = kilosPosicionMaiz.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoCamara = materialId.Contains(2) ?
                (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(2, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) :
                TraerPosicionMaterial(2, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId)
                ) : new List<PosicionKilos>();
            var posicionTrigoCamara = new PosicionComprasDto
            {
                Material = "Trigo",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCamara,
                Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            //var kilosPosicionTrigoCalidad = materialId.Contains(2) ?
            //    (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(2, fechaDesde, fechaHasta, 2, precioPizarra, negocios, centroId) :
            //    TraerPosicionMaterial(2, fechaDesde, fechaHasta, 2, precioPizarra, negocios, centroId)
            //    ) : new List<PosicionKilos>();
            //var posicionTrigoCalidad = new PosicionComprasDto
            //{
            //    Material = "Trigo Calidad",
            //    MaterialId = 2,
            //    PosicionKilos = kilosPosicionTrigoCalidad,
            //    Total = kilosPosicionTrigoCalidad.Sum(x => x.KilosPesos + x.KilosDolares)
            //};
            //var kilosPosicionTrigoGrado = materialId.Contains(2) ?
            //    (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(2, fechaDesde, fechaHasta, 7, precioPizarra, negocios, centroId) :
            //    TraerPosicionMaterial(2, fechaDesde, fechaHasta, 7, precioPizarra, negocios, centroId))
            //    : new List<PosicionKilos>();
            //var posicionTrigoGrado = new PosicionComprasDto
            //{
            //    Material = "Trigo Grado 2",
            //    MaterialId = 2,
            //    PosicionKilos = kilosPosicionTrigoGrado,
            //    Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            //};
            var kilosPosiciongirasol = materialId.Contains(4) ?
                (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(4, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) :
                TraerPosicionMaterial(4, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId))
                : new List<PosicionKilos>();
            var posicionGirasol = new PosicionComprasDto
            {
                Material = "Girasol",
                MaterialId = 4,
                PosicionKilos = kilosPosiciongirasol,
                Total = kilosPosiciongirasol.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionGirasolAlto = materialId.Contains(5) ?
                (TraerPosicionMaterialCampaña ? TraerPosicionMaterialPorCampaña(5, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) :
                TraerPosicionMaterial(5, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId))
                : new List<PosicionKilos>();
            var posicionGirasolAlto = new PosicionComprasDto
            {
                Material = "Girasol Alto Oleico",
                MaterialId = 5,
                PosicionKilos = kilosPosicionGirasolAlto,
                Total = kilosPosicionGirasolAlto.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            return new List<PosicionComprasDto> { posicionSoja, posicionMaiz, posicionTrigoCamara, /*posicionTrigoCalidad, posicionTrigoGrado,*/ posicionGirasol, posicionGirasolAlto };
        }
        public List<PricingCampaniaDto> TraerPricingCampania(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId, bool verFijaciones = true)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var negocio = repositorio.Listar<Contrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? 2 : x.TipoNegocioId,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                SanLorenzo = (x.Destino.Acopio == false || x.Destino.CodigoSap == "1074") ? Math.Round(x.Cantidad / 1000) : 0,
                Acopio = (x.Destino.Acopio == true && x.Destino.CodigoSap != "1074") ? Math.Round(x.Cantidad / 1000) : 0
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false &&
            DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta
            && (x.TipoNegocioId == 2 || (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1)) && x.Venta != true && x.AnulaYReemplazaContratoId == null &&
            (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5 || x.EstadoId == 10) && (centroId == 0 || centroId == x.DestinoId) && x.ContratoAcuerdoId == null && x.TipoAgenteCompraId == null);
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = 3,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                SanLorenzo = (x.Destino.Acopio == false || x.Destino.CodigoSap == "1074") ? Math.Round(x.Cantidad / 1000) : 0,
                Acopio = (x.Destino.Acopio == true && x.Destino.CodigoSap != "1074") ? Math.Round(x.Cantidad / 1000) : 0
            }, x => materialId.Contains(x.MaterialId) && x.Canje != true && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde &&
                    DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5 || x.EstadoId == 10) && (centroId == 0 || centroId == x.DestinoId) &&
                    !(x.Canje != true && x.Virtual != true && x.Contrato.Canje == true) && verFijaciones && x.TipoPosicionCBOTId != 3
            );
            var fasones = repositorio.Listar<Fason, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = 4,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                Acopio = 0,
                SanLorenzo = Math.Round(x.Cantidad / 1000)
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) >= fechaDesde
                    && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1));
            var agentes = repositorio.Listar<AgenteCompra, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = 5,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                SanLorenzo = Math.Round(x.Cantidad / 1000),
                Acopio = 0
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false
                && DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1));
            var acuerdo = repositorio.Listar<ContratoAcuerdo, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = 6,
                Campania = x.CampanaId != null ? x.Campana.Descripcion : "17-18",
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round((double)x.Cantidad / 1000),
                SanLorenzo = (x.Destino.Acopio == false || x.Destino.CodigoSap == "1074") ? Math.Round(x.Cantidad / 1000) : 0,
                Acopio = (x.Destino.Acopio == true && x.Destino.CodigoSap != "1074") ? Math.Round(x.Cantidad / 1000) : 0
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && (x.PrecioNeto != null && x.PrecioNeto != 0)
            /*&& fechaDesde == fechaHasta*/  && DbFunctions.TruncateTime(x.Fecha) >= fechaDesde && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1) && x.TipoAgenteCompraId == null);
            var pricing = new List<PricingCampaniaDto>();
            var materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, CampaniaTableroId = x.CampaniaTableroId, Descripcion = x.Descripcion, Campana = x.CampaniaTablero.Descripcion });

            negocio.AddRange(fijaciones);
            negocio.AddRange(fasones);
            negocio.AddRange(acuerdo);
            negocio.AddRange(agentes);
            foreach (var neg in negocio)
            {
                var campania = materiales.FirstOrDefault(x => x.MaterialId == neg.MaterialId);
                neg.Campania = campania.CampaniaTableroId < neg.CampaniaId ? "New Crop" : campania.Campana;
            }
            var group = negocio.GroupBy(x => new { x.Campania, x.Material }).ToList();
            foreach (var e in group)
            {
                var price = new PricingCampaniaDto
                {
                    Campania = e.Key.Campania,
                    CampaniaId = e.Select(x => x.CampaniaId).FirstOrDefault(),
                    Material = e.Key.Material,
                    MaterialId = e.Select(x => x.MaterialId).FirstOrDefault(),
                    Pricing = e.Sum(x => x.Pricing),
                    PricingIds = e.Select(a => a.Id).ToList(),
                    Acopio = e.Sum(x => x.Acopio),
                    AcopioIds = e.Where(a => a.Acopio != 0).Select(a => a.Id).ToList(),
                    SanLorenzo = e.Sum(x => x.SanLorenzo),
                    SanLorenzoIds = e.Where(a => a.SanLorenzo != 0).Select(a => a.Id).ToList(),
                    Id = e.Select(x => x.MaterialId).FirstOrDefault() * 10 + (e.Key.Campania == "New Crop" ? 2 : 1)

                };
                pricing.Add(price);
            }
            return pricing.OrderBy(x => x.Id).ToList();
        }

        public List<PrecioCantidadDto> TraerMonedaCantidad(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0, bool verFijaciones = true)
        {
            var moneda = repositorio.ListarConsulta(new TraerMonedaKilo(fechaDesde, fechaHasta, materialId, centroId, verFijaciones));

            return new List<PrecioCantidadDto>() { new PrecioCantidadDto {Moneda = "Pesos" , Cantidad= moneda.Exists(x=>x.Moneda == "ARP  ")?moneda.Where(x=>x.Moneda== "ARP  ").Select(x=>x.Cantidad).First():0},
                new PrecioCantidadDto {Moneda = "Dólares" , Cantidad=moneda.Exists(x=>x.Moneda == "USDM ")? moneda.Where(x=>x.Moneda== "USDM ").Select(x=>x.Cantidad).First():0}};
        }
        public List<HedgeMaterialDto> TraerTodosHedgeMaterial(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var hedgeMat = new List<HedgeMaterialDto>();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                hedgeMat.AddRange(repositorio.Listar<HedgeMaterial, HedgeMaterialDto>(x => new HedgeMaterialDto
                {
                    MaterialId = x.MaterialId,
                    TipoHedgeMaterialId = x.TipoHedgeMaterialId,
                    Cantidad = x.Cantidad
                }, x => materialId.Contains(x.MaterialId) && DbFunctions.TruncateTime(x.Fecha) == fechaDesde));
            }
            return hedgeMat;
        }
        public HedgeCargaObjetivoDto TraerHedgeObjetivo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, bool verFijaciones = true)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();
            var obj = new HedgeCargaObjetivoDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeObjetivo>(x => materialId.Contains(x.MaterialId) && DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var cumplidosContratos = repositorio.Listar<Contrato>(x =>
                materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde &&
                x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var cumplidosFijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => verFijaciones && x.TipoPosicionCBOTId != 3 &&
                materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde &&
                x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

                obj.PricingObjetivo = objetivos.Where(x => x.TipoObjetivoId == 1).Sum(x => x.Cantidad);
                obj.RemitirObjetivo = objetivos.Where(x => x.TipoObjetivoId == 2).Sum(x => x.Cantidad);
                obj.PricingCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad) + cumplidosFijaciones.Sum(x => (decimal)x.Cantidad);
                obj.RemitirCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 1 || x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad);
            }
            return obj;
        }
        public HedgeCargaObjetivoDto TraerUltimoHedgeObjetivo()
        {
            var obj = new HedgeCargaObjetivoDto();

            var fecha = repositorio.ObtenerMayor<HedgeObjetivo, DateTime, DateTime>(x => true, x => x.Fecha, x => x.Fecha).Date;
            var objetivos = repositorio.Listar<HedgeObjetivo>(x => DbFunctions.TruncateTime(x.Fecha) == fecha);
            var cumplidosContratos = repositorio.Listar<Contrato>(x => x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) == fecha && x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
            var cumplidosFijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) == fecha && x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

            obj.PricingObjetivo = objetivos.Where(x => x.TipoObjetivoId == 1).Sum(x => x.Cantidad);
            obj.RemitirObjetivo = objetivos.Where(x => x.TipoObjetivoId == 2).Sum(x => x.Cantidad);
            obj.PricingCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad) + cumplidosFijaciones.Sum(x => (decimal)x.Cantidad);
            obj.RemitirCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 1 || x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad);

            return obj;
        }
        public HedgeTCPromedioDto TraerTcPromedio(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, bool verFijaciones = true)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var obj = new HedgeTCPromedioDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeTC>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var contratos = repositorio.Listar<Contrato>(x =>
                materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && x.TipoAgenteCompraId == null &&
                x.AnulaYReemplazaContratoId == null && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde && x.MonedaId == "ARP  " &&
                x.CampanaId == x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) /*&& x.Canje != true*/)
                    .Sum(x => x.Precio);
                var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => verFijaciones && x.TipoPosicionCBOTId != 3 &&
                materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde &&
                x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) &&
                !(x.Canje != true && x.Virtual != true && x.Contrato.Canje == true) && x.Canje != true)
                    .Sum(x => x.Precio);
                var fason = repositorio.Listar<Fason>(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);

                if (objetivos != null)
                {
                    decimal sumProd = 0;
                    decimal total = 0;
                    foreach (var hT in objetivos)
                    {
                        sumProd += hT.TipoCambio * hT.HedgePesos;
                        total += hT.HedgePesos;
                    }
                    if (total != 0)
                    {
                        obj.PromedioTC = sumProd / total;
                    }
                    obj.TotalTC = objetivos.Sum(x => x.HedgePesos) - contratos - fijaciones - fason;
                }
            }
            return obj;
        }
        public List<AgenteCompraDto> TraerAgenteDeCompra(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();
            var listaAgentes = new List<AgenteCompraDto>();
            //if (fechaDesde == fechaHasta)
            //{
            var precioDolar = tipoDeCambio.TraerTipoDeCambio(null);
            fechaDesde = fechaDesde.Date;
            fechaHasta = fechaHasta.Date;
            var agentes = repositorio.Listar<AgenteCompra>(x => materialId.Contains(x.MaterialId) &&
                x.OcultarEnTablero == false &&
                DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde &&
                DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta &&
                (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).GroupBy(x => new { x.Posicion, x.MaterialId, TipoAgenteCompraId = x.TipoAgenteCompraId.Value });
            foreach (var agentesPorPosicionYMaterial in agentes)
            {
                var agenteTemp = new AgenteCompraDto() { Operador = new List<AgenteCompraDto.OperadorCantidad>() };
                foreach (var agente in agentesPorPosicionYMaterial)
                {
                    var operador = agenteTemp.Operador.FirstOrDefault(x => x.OperadorId == agente.OperadorId);
                    if (operador == null)
                    {
                        operador = new AgenteCompraDto.OperadorCantidad { OperadorDesc = agente.Operador.Descripcion, OperadorId = agente.OperadorId };
                        agenteTemp.Operador.Add(operador);
                    }
                    operador.Cantidad += Math.Round(agente.Cantidad / 1000);
                    operador.Ids.Add(agente.Id);
                    agenteTemp.Posicion = agentesPorPosicionYMaterial.Key.Posicion;
                    agenteTemp.MaterialId = agentesPorPosicionYMaterial.Key.MaterialId;
                    agenteTemp.MaterialDesc = agente.Material.Descripcion;
                    agenteTemp.TipoAgenteId = agentesPorPosicionYMaterial.Key.TipoAgenteCompraId;
                    agenteTemp.TipoAgenteDesc = agente.TipoAgenteCompra.Descripcion;
                    agenteTemp.PrecioPonderado = agentesPorPosicionYMaterial.Sum(x => (x.MonedaId.Contains("ARP") ? x.Precio / precioDolar : x.Precio) * (decimal)Math.Abs(x.Cantidad)) / agentesPorPosicionYMaterial.Sum(x => (decimal)Math.Abs(x.Cantidad));
                }
                listaAgentes.Add(agenteTemp);
            }
            //}
            return listaAgentes.OrderBy(x => x.MaterialId).ThenBy(x => new DateTime(int.Parse(x.Posicion.Split('.')[1]), int.Parse(x.Posicion.Split('.')[0]), 1)).ToList();
        }

        public ExcelDetallePosicionDto DetallePosicion(int materialId, int mes, int anio, DateTime fechadesde, DateTime fechaHasta, int? calidad, int centroId = 0, bool verFijaciones = true)
        {
            var excel = new ExcelDetallePosicionDto();
            excel.Headers = typeof(DetalleContratoDto).GetProperties().Select(p => Text.ResourceManager.GetString(p.Name)).ToArray();
            excel.Data = ConvertirListadetalleContratoAListaString(TraerDetallePosicion(materialId, mes, anio, fechadesde, fechaHasta, calidad, centroId, verFijaciones), mes, anio);
            excel.Name = "Detalle Posicion de Negocios de " + (EnumMeses)Enum.ToObject(typeof(EnumMeses), mes) + " " + anio + ".xlsx";
            excel.SheetName = "Posicion";
            return excel;
        }

        public ExcelDetallePosicionDto DetalleAgente(DateTime fecha, List<int> materialId)
        {
            var excel = new ExcelDetallePosicionDto();
            excel.Headers = typeof(DetalleAgenteDto).GetProperties().Select(p => Text.ResourceManager.GetString(p.Name)).ToArray();
            excel.Data = ConvertirListaAgente(TraerDetalleAgente(fecha, materialId));
            excel.Name = "Detalle Agente de Compras.xlsx";
            excel.SheetName = "Agente";
            return excel;
        }
        public List<ExcelPosicionMaterialDto> PosicionPorMaterial(DateTime fechaDesde, DateTime fechaHasta, bool verFijaciones = true)
        {
            return repositorio.ListarConsulta(new TraerPosicionMaterialMes(fechaDesde, fechaHasta, verFijaciones));
        }
        private List<PosicionKilos> TraerPosicionMaterial(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad, List<PrecioPizarra> precioPizarra, List<BasicoContrato> negocios, int centroId = 0)
        {
            var posicionKilos = new List<PosicionKilos>();
            ObtenerPosicionMaterialBase(materialId, fechaDesde, fechaHasta, calidad, precioPizarra, negocios, centroId, posicionKilos);
            posicionKilos = posicionKilos
                        .GroupBy(x => new { x.Anio, x.Mes })
                        .Select(x => new PosicionKilos
                        {
                            Anio = x.Key.Anio,
                            Mes = x.Key.Mes,
                            KilosPesos = x.Sum(y => y.KilosPesos),
                            KilosDolares = x.Sum(y => y.KilosDolares),
                            DispAFijar = Math.Round(x.Sum(y => y.DispAFijar / 1000)),

                            DispAPrecio = Math.Round(x.Sum(y => y.DispAPrecio / 1000)),
                            DispAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.DispAPrecio / 1000)),
                            DispAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.DispAPrecio / 1000)),

                            DispFijac = Math.Round(x.Sum(y => y.DispFijac / 1000)),
                            DispFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.DispFijac / 1000)),
                            DispFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.DispFijac / 1000)),

                            FrwAFijar = Math.Round(x.Sum(y => y.FrwAFijar / 1000)),

                            FrwAPrecio = Math.Round(x.Sum(y => y.FrwAPrecio / 1000)),
                            FrwAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.FrwAPrecio / 1000)),
                            FrwAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.FrwAPrecio / 1000)),

                            FrwFijac = Math.Round(x.Sum(y => y.FrwFijac / 1000)),
                            FrwFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.FrwFijac / 1000)),
                            FrwFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.FrwFijac / 1000)),

                            NewAFijar = Math.Round(x.Sum(y => y.NewAFijar / 1000)),

                            NewAPrecio = Math.Round(x.Sum(y => y.NewAPrecio / 1000)),
                            NewAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.NewAPrecio / 1000)),
                            NewAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.NewAPrecio / 1000)),

                            NewFijac = Math.Round(x.Sum(y => y.NewFijac / 1000)),
                            NewFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.NewFijac / 1000)),
                            NewFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.NewFijac / 1000)),

                            PrecioPonderadoPesos = x.Where(y => y.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada) != 0 ? x.Sum(y => y.PrecioPonderadoPesos / (decimal)x.Where(f => f.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada)) : 0,
                            PrecioPonderadoDolares = x.Where(y => y.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada) != 0 ? x.Sum(y => y.PrecioPonderadoDolares / (decimal)x.Where(f => f.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada)) : 0,
                            ListDispAFijar = x.Where(y => y.DispAFijar != 0).Select(y => y.NegocioId),
                            ListDispAPrecio = x.Where(y => y.DispAPrecio != 0).Select(y => y.NegocioId),
                            ListDispFijac = x.Where(y => y.DispFijac != 0).Select(y => y.NegocioId),
                            ListFrwAFijar = x.Where(y => y.FrwAFijar != 0).Select(y => y.NegocioId),
                            ListFrwAPrecio = x.Where(y => y.FrwAPrecio != 0).Select(y => y.NegocioId),
                            ListFrwFijac = x.Where(y => y.FrwFijac != 0).Select(y => y.NegocioId),
                            ListNewAFijar = x.Where(y => y.NewAFijar != 0).Select(y => y.NegocioId),
                            ListNewAPrecio = x.Where(y => y.NewAPrecio != 0).Select(y => y.NegocioId),
                            ListNewFijac = x.Where(y => y.NewFijac != 0).Select(y => y.NegocioId)
                        }).ToList();
            return posicionKilos;
        }
        private List<PosicionKilos> TraerPosicionMaterialPorCampaña(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad, List<PrecioPizarra> precioPizarra, List<BasicoContrato> negocios, int centroId = 0)
        {
            var posicionKilos = new List<PosicionKilos>();
            ObtenerPosicionMaterialBase(materialId, fechaDesde, fechaHasta, calidad, precioPizarra, negocios, centroId, posicionKilos);
            posicionKilos = posicionKilos
                        .GroupBy(x => new { x.Anio, x.Mes, x.CampanaId })
                        .Select(x => new PosicionKilos
                        {
                            Anio = x.Key.Anio,
                            Mes = x.Key.Mes,
                            CampanaId = x.Key.CampanaId,
                            Campana = x.First().Campana,
                            KilosPesos = x.Sum(y => y.KilosPesos),
                            KilosDolares = x.Sum(y => y.KilosDolares),
                            DispAFijar = Math.Round(x.Sum(y => y.DispAFijar / 1000)),

                            DispAPrecio = Math.Round(x.Sum(y => y.DispAPrecio / 1000)),
                            DispAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.DispAPrecio / 1000)),
                            DispAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.DispAPrecio / 1000)),

                            DispFijac = Math.Round(x.Sum(y => y.DispFijac / 1000)),
                            DispFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.DispFijac / 1000)),
                            DispFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.DispFijac / 1000)),

                            FrwAFijar = Math.Round(x.Sum(y => y.FrwAFijar / 1000)),

                            FrwAPrecio = Math.Round(x.Sum(y => y.FrwAPrecio / 1000)),
                            FrwAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.FrwAPrecio / 1000)),
                            FrwAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.FrwAPrecio / 1000)),

                            FrwFijac = Math.Round(x.Sum(y => y.FrwFijac / 1000)),
                            FrwFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.FrwFijac / 1000)),
                            FrwFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.FrwFijac / 1000)),

                            NewAFijar = Math.Round(x.Sum(y => y.NewAFijar / 1000)),

                            NewAPrecio = Math.Round(x.Sum(y => y.NewAPrecio / 1000)),
                            NewAPrecioPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.NewAPrecio / 1000)),
                            NewAPrecioDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.NewAPrecio / 1000)),

                            NewFijac = Math.Round(x.Sum(y => y.NewFijac / 1000)),
                            NewFijacPesos = Math.Round(x.Where(a => a.KilosPesos > 0).Sum(y => y.NewFijac / 1000)),
                            NewFijacDolares = Math.Round(x.Where(a => a.KilosDolares > 0).Sum(y => y.NewFijac / 1000)),

                            PrecioPonderadoPesos = x.Where(y => y.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada) != 0 ? x.Sum(y => y.PrecioPonderadoPesos / (decimal)x.Where(f => f.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada)) : 0,
                            PrecioPonderadoDolares = x.Where(y => y.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada) != 0 ? x.Sum(y => y.PrecioPonderadoDolares / (decimal)x.Where(f => f.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada)) : 0,
                            ListDispAFijar = x.Where(y => y.DispAFijar != 0).Select(y => y.NegocioId),
                            ListDispAPrecio = x.Where(y => y.DispAPrecio != 0).Select(y => y.NegocioId),
                            ListDispFijac = x.Where(y => y.DispFijac != 0).Select(y => y.NegocioId),
                            ListFrwAFijar = x.Where(y => y.FrwAFijar != 0).Select(y => y.NegocioId),
                            ListFrwAPrecio = x.Where(y => y.FrwAPrecio != 0).Select(y => y.NegocioId),
                            ListFrwFijac = x.Where(y => y.FrwFijac != 0).Select(y => y.NegocioId),
                            ListNewAFijar = x.Where(y => y.NewAFijar != 0).Select(y => y.NegocioId),
                            ListNewAPrecio = x.Where(y => y.NewAPrecio != 0).Select(y => y.NegocioId),
                            ListNewFijac = x.Where(y => y.NewFijac != 0).Select(y => y.NegocioId)
                        }).ToList();
            return posicionKilos;
        }
        private static void ObtenerPosicionMaterialBase(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad, List<PrecioPizarra> precioPizarra, List<BasicoContrato> negocios, int centroId, List<PosicionKilos> posicionKilos)
        {
            var standard = calidad.HasValue ? calidad.Value : 1;
            precioPizarra = precioPizarra.Where(x => x.MaterialId == materialId && x.FechaHasta <= fechaHasta).ToList();
            var precio = precioPizarra.Count != 0 ? precioPizarra.OrderByDescending(x => x.FechaHasta).FirstOrDefault() : new PrecioPizarra();
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(+1);
            var contratos = negocios.Where(x =>
                x.OcultarEnTablero == false
                //&& x.Fecha >= fechaHoy && x.Fecha <= fechaManana
                && (x.Estado == 2 || x.Estado == 4 || x.Estado == 5 || x.Estado == 10)
                && x.MaterialId == materialId
                && (calidad == null || (calidad != null && x.StandardCalidadId == standard))
                && (centroId == 0 || x.DestinoId == centroId)
                && x.ContratoAcuerdoId == null && x.EsFason != true
                && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2)
                //&& (x.Canje != true)
                && (x.PrestamoDevolucion != true)
                && (x.Venta != true)
            ).Select(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde.Value,
                FechaHasta = x.FechaHasta.Value,
                Cantidad = x.Cantidad,
                Precio = x.Pizarra == true ? (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? (x.PrecioPonderado ?? 0) : precio.Precio : x.Precio,
                TipoNegocioId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? 2 : x.TipoNegocioId,
                CampanaMaterialId = x.CampanaMaterialId,
                CampanaId = x.CampanaId,
                Campana = x.Campania,
                CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                MonedaId = x.Pizarra == true ? precio.MonedaId : (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "USDM " : x.MonedaId
            }).ToList();

            foreach (var cont in contratos)
            {
                var posicion = new DateTime();
                if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                {
                    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);
                }
                else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                {
                    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1).AddMonths(+1);
                }
                else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                {
                    posicion = new DateTime(cont.FechaHasta.Year, cont.FechaHasta.Month, 1);
                }
                cont.ClasificacionNegocio = cont.TipoNegocioId == 1 && ((fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId)) ? EnumClasificacionNegocio.DisponibleAFijar :
                cont.TipoNegocioId == 2 && ((fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId)) ? EnumClasificacionNegocio.DisponibleAPrecio :
                cont.TipoNegocioId == 1 && cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardAFijar :
                cont.TipoNegocioId == 2 && cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardAPrecio :
                cont.TipoNegocioId == 1 && cont.CampanaMaterialId < cont.CampanaId ? EnumClasificacionNegocio.NewCropAFijar : EnumClasificacionNegocio.NewCropAPrecio;
            }

            var fijaciones = negocios.Where(
                x => x.OcultarEnTablero == false
            //&& x.Fecha >= fechaHoy
            //&& x.Fecha <= fechaManana
            && (x.Estado == 2 || x.Estado == 4 || x.Estado == 5 || x.Estado == 10)
            && x.MaterialId == materialId
            && x.TipoNegocioId == 3
            && (calidad == null || (calidad == x.StandardCalidadId))
            && (centroId == 0 || centroId == x.DestinoId))
                .Select(
                x => new PosicionPorMaterial
                {
                    Id = x.Id,
                    FechaDesde = x.FechaDesde.Value,
                    FechaHasta = x.FechaHasta.Value,
                    TipoNegocioId = 3,
                    Cantidad = x.Cantidad,
                    Precio = x.Pizarra == true ? precio.Precio : x.Precio,
                    CampanaId = x.CampanaId,
                    Campana = x.Campania,
                    CampanaMaterialId = x.CampanaMaterialId,
                    CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                    MonedaId = x.Pizarra == true ? precio.MonedaId : x.MonedaId,
                    FijacionContratoConDescarga = x.CantidadDeposito > 0
                })
                .ToList();
            foreach (var cont in fijaciones)
            {
                var posicion = cont.FechaDesde - DateTime.Now;

                cont.ClasificacionNegocio = (posicion.Days <= 30 && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                cont.CampanaMaterialId == cont.CampanaId && posicion.Days > 30 ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(fijaciones);

            var fason = negocios.Where(x =>
                x.OcultarEnTablero == false
                && x.Fecha >= fechaHoy
                && x.Fecha <= fechaManana
                && (x.Estado == 2 || x.Estado == 4 || x.Estado == 5)
                && x.MaterialId == materialId
                && x.TipoNegocioId == 4
                && (calidad == null || (calidad == 7 && x.TrigoEspecial == true) || (calidad == 2 && x.TrigoEspecial == false))
                && (centroId == 0 || centroId == 1)
            ).Select(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde.Value,
                FechaHasta = x.FechaHasta.Value,
                Cantidad = x.Cantidad,
                TipoNegocioId = 4,
                Precio = x.Precio,
                CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                CampanaId = x.CampanaId,
                Campana = x.Campania,
                CampanaMaterialId = x.CampanaMaterialId,
                Posicion = x.Posicion,
                MonedaId = x.MonedaId
            }).ToList();
            foreach (var cont in fason)
            {
                //var mes = int.Parse(cont.Posicion.Substring(0, 2));
                //var anio = int.Parse(cont.Posicion.Substring(3, 4));
                //var posicion = new DateTime(anio, mes, 1);
                var posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);

                cont.ClasificacionNegocio = (fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(fason);

            var acuerdos = negocios.Where(x =>
            x.OcultarEnTablero == false
               && x.Fecha >= fechaHoy
               //&& (x.PrecioNeto != null && x.PrecioNeto != 0)
               && x.Fecha <= fechaManana
               && (x.Estado == 2 || x.Estado == 5)
               && x.MaterialId == materialId
               && x.TipoNegocioId == 6
               && (calidad == null || (calidad != null && x.StandardCalidadId == standard))
               && (centroId == 0 || x.DestinoId == centroId)
            ).Select(
                x => new PosicionPorMaterial
                {
                    Id = x.Id,
                    FechaDesde = x.FechaDesde.Value,
                    FechaHasta = x.FechaHasta.Value,
                    Cantidad = x.Cantidad,
                    TipoNegocioId = 6,
                    Precio = x.Precio,
                    CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                    MonedaId = x.MonedaId,
                    CampanaId = x.CampanaId,
                    Campana = x.Campania,
                    CampanaMaterialId = x.CampanaMaterialId,
                }
                ).ToList();


            foreach (var cont in acuerdos)
            {
                var posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);

                //if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                //{
                //    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);
                //}
                //else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                //{
                //    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1).AddMonths(+1);
                //}
                //else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                //{
                //    posicion = new DateTime(cont.FechaHasta.Year, cont.FechaHasta.Month, 1);
                //}
                cont.ClasificacionNegocio = (fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? (cont.Precio > 0 ? EnumClasificacionNegocio.DisponibleAPrecio : EnumClasificacionNegocio.DisponibleAFijar) :
                /*cont.TipoNegocioId == 1 &&*/
                cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? (cont.Precio > 0 ? EnumClasificacionNegocio.ForwardAPrecio : EnumClasificacionNegocio.ForwardAFijar) :
                (cont.Precio > 0 ? EnumClasificacionNegocio.NewCropAPrecio : EnumClasificacionNegocio.NewCropAFijar);
            }
            contratos.AddRange(acuerdos);

            foreach (var cont in contratos)
            {
                var posKil = new PosicionKilos();
                posKil.NegocioId = cont.Id;
                posKil.TipoNegocioId = cont.TipoNegocioId;
                if ((cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleFijacion &&
                    cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardFijacion &&
                    cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropFijacion) || cont.FijacionContratoConDescarga)
                {
                    if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                    {
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;
                    }
                    else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                    {
                        cont.FechaDesde = cont.FechaDesde.AddMonths(1);
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;

                    }
                    else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                    {
                        posKil.Mes = (EnumMeses)cont.FechaHasta.Month;
                        posKil.Anio = cont.FechaHasta.Year;
                    }
                }
                else
                {
                    if (new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1) <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                    {
                        posKil.Mes = (EnumMeses)DateTime.Now.Month;
                        posKil.Anio = DateTime.Now.Year;
                    }
                    else
                    {
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;
                    }
                }
                posKil.KilosPesos = cont.MonedaId == "ARP  "
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleAFijar
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardAFijar
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropAFijar
                        ? Math.Round(cont.Cantidad / 1000) : 0;
                posKil.KilosDolares = cont.MonedaId == "USDM "
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleAFijar
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardAFijar
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropAFijar
                    ? Math.Round(cont.Cantidad / 1000) : 0;
                posKil.DispAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleAFijar ? cont.Cantidad : 0;
                posKil.DispAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleAPrecio ? cont.Cantidad : 0;
                posKil.DispFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleFijacion ? cont.Cantidad : 0;
                posKil.FrwAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardAFijar ? cont.Cantidad : 0;
                posKil.FrwAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardAPrecio ? cont.Cantidad : 0;
                posKil.FrwFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardFijacion ? cont.Cantidad : 0;
                posKil.NewAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropAFijar ? cont.Cantidad : 0;
                posKil.NewAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropAPrecio ? cont.Cantidad : 0;
                posKil.NewFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropFijacion ? cont.Cantidad : 0;
                posKil.PrecioPonderadoPesos = cont.MonedaId == "ARP  " ? cont.Precio * (decimal)cont.CantidadPonderada : 0;
                posKil.PrecioPonderadoDolares = cont.MonedaId == "USDM " ? cont.Precio * (decimal)cont.CantidadPonderada : 0;
                posKil.CantidadPonderada = cont.CantidadPonderada;
                posKil.CampanaId = cont.CampanaId;
                posKil.Campana = cont.Campana;

                posicionKilos.Add(posKil);
            }
        }

        public void TraerPosicionNegocios(List<BasicoContrato> negocios)
        {
            List<int> negociosIds = negocios.Select(a => a.Id).ToList();
            var fechaHoy = DateTime.Now.Date;
            var fechaManana = DateTime.Now.Date;
            var precioPizarra = repositorio.Listar<PrecioPizarra>(x => x.FechaHasta <= fechaManana);
            var precio = precioPizarra.Count != 0 ? precioPizarra.OrderByDescending(x => x.FechaHasta).FirstOrDefault() : new PrecioPizarra();

            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(+1);
            var posicionKilos = new List<PosicionKilos>();
            var contratos = repositorio.Listar<Contrato, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                Cantidad = x.Cantidad,
                Precio = x.Pizarra == true ? precio.Precio : x.Precio,
                TipoNegocioId = x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                MonedaId = x.Pizarra == true ? precio.MonedaId : x.MonedaId
            },
                x => negociosIds.Contains(x.Id));
            foreach (var cont in contratos)
            {
                var posicion = new DateTime();
                if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                {
                    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);
                }
                else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                {
                    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1).AddMonths(+1);
                }
                else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                {
                    posicion = new DateTime(cont.FechaHasta.Year, cont.FechaHasta.Month, 1);
                }
                cont.ClasificacionNegocio = cont.TipoNegocioId == 1 && ((fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId)) ? EnumClasificacionNegocio.DisponibleAFijar :
                cont.TipoNegocioId == 2 && ((fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId)) ? EnumClasificacionNegocio.DisponibleAPrecio :
                cont.TipoNegocioId == 1 && cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardAFijar :
                cont.TipoNegocioId == 2 && cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardAPrecio :
                cont.TipoNegocioId == 1 && cont.CampanaMaterialId < cont.CampanaId ? EnumClasificacionNegocio.NewCropAFijar : EnumClasificacionNegocio.NewCropAPrecio;
            }

            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                TipoNegocioId = 3,
                Cantidad = x.Cantidad,
                Precio = x.Pizarra == true ? precio.Precio : x.Precio,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                MonedaId = x.Pizarra == true ? precio.MonedaId : x.MonedaId
            },
             x => negociosIds.Contains(x.Id));
            foreach (var cont in fijaciones)
            {
                var posicion = cont.FechaDesde - DateTime.Now;

                cont.ClasificacionNegocio = (posicion.Days <= 30 && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                cont.CampanaMaterialId == cont.CampanaId && posicion.Days > 30 ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(fijaciones);

            var fason = repositorio.Listar<Fason, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.Fecha,
                FechaHasta = x.Fecha,
                Cantidad = x.Cantidad,
                TipoNegocioId = 4,
                Precio = x.Precio,
                CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                Posicion = x.Posicion,
                MonedaId = x.MonedaId
            },
                x => negociosIds.Contains(x.Id));
            foreach (var cont in fason)
            {
                //var mes = int.Parse(cont.Posicion.Substring(0, 2));
                //var anio = int.Parse(cont.Posicion.Substring(3, 4));
                //var posicion = new DateTime(anio, mes, 1);
                var posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);

                cont.ClasificacionNegocio = (fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(fason);
            var acuerdos = repositorio.Listar<ContratoAcuerdo, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.Fecha,
                FechaHasta = x.Fecha,
                Cantidad = x.Cantidad,
                TipoNegocioId = 6,
                Precio = x.Precio,
                CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                MonedaId = x.MonedaId,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
            },
               x => negociosIds.Contains(x.Id));

            foreach (var cont in acuerdos)
            {
                var posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);

                //if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                //{
                //    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1);
                //}
                //else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                //{
                //    posicion = new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1).AddMonths(+1);
                //}
                //else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                //{
                //    posicion = new DateTime(cont.FechaHasta.Year, cont.FechaHasta.Month, 1);
                //}
                cont.ClasificacionNegocio = (fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? (cont.Precio > 0 ? EnumClasificacionNegocio.DisponibleAPrecio : EnumClasificacionNegocio.DisponibleAFijar) :
                /*cont.TipoNegocioId == 1 &&*/
                cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? (cont.Precio > 0 ? EnumClasificacionNegocio.ForwardAPrecio : EnumClasificacionNegocio.ForwardAFijar) :
                (cont.Precio > 0 ? EnumClasificacionNegocio.NewCropAPrecio : EnumClasificacionNegocio.NewCropAFijar);
            }
            contratos.AddRange(acuerdos);

            var agente = repositorio.Listar<AgenteCompra, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.Fecha,
                FechaHasta = x.Fecha,
                Cantidad = x.Cantidad,
                TipoNegocioId = 4,
                Precio = x.Precio,
                CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                Posicion = x.Posicion,
                MonedaId = x.MonedaId
            },
                x => negociosIds.Contains(x.Id));
            foreach (var age in agente)
            {
                var posicion = new DateTime(age.FechaDesde.Year, age.FechaDesde.Month, 1);

                age.ClasificacionNegocio = (fechaPosicion >= posicion && age.CampanaMaterialId == age.CampanaId) || (age.CampanaMaterialId > age.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                age.CampanaMaterialId == age.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(agente);


            foreach (var cont in contratos)
            {
                var posKil = new PosicionKilos();
                posKil.NegocioId = cont.Id;
                posKil.TipoNegocioId = cont.TipoNegocioId;
                if (cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleFijacion &&
                    cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardFijacion &&
                    cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropFijacion)
                {
                    if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
                    {
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;
                    }
                    else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
                    {
                        cont.FechaDesde = cont.FechaDesde.AddMonths(1);
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;

                    }
                    else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
                    {
                        posKil.Mes = (EnumMeses)cont.FechaHasta.Month;
                        posKil.Anio = cont.FechaHasta.Year;
                    }
                }
                else
                {
                    if (new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1) <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
                    {
                        posKil.Mes = (EnumMeses)DateTime.Now.Month;
                        posKil.Anio = DateTime.Now.Year;
                    }
                    else
                    {
                        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
                        posKil.Anio = cont.FechaDesde.Year;
                    }
                }
                posKil.KilosPesos = cont.MonedaId == "ARP  "
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleAFijar
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardAFijar
                        && cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropAFijar
                        ? Math.Round(cont.Cantidad / 1000) : 0;
                posKil.KilosDolares = cont.MonedaId == "USDM "
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleAFijar
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardAFijar
                    && cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropAFijar
                    ? Math.Round(cont.Cantidad / 1000) : 0;
                posKil.DispAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleAFijar ? cont.Cantidad : 0;
                posKil.DispAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleAPrecio ? cont.Cantidad : 0;
                posKil.DispFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.DisponibleFijacion ? cont.Cantidad : 0;
                posKil.FrwAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardAFijar ? cont.Cantidad : 0;
                posKil.FrwAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardAPrecio ? cont.Cantidad : 0;
                posKil.FrwFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.ForwardFijacion ? cont.Cantidad : 0;
                posKil.NewAFijar = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropAFijar ? cont.Cantidad : 0;
                posKil.NewAPrecio = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropAPrecio ? cont.Cantidad : 0;
                posKil.NewFijac = cont.ClasificacionNegocio == EnumClasificacionNegocio.NewCropFijacion ? cont.Cantidad : 0;
                posKil.PrecioPonderadoPesos = cont.MonedaId == "ARP  " ? cont.Precio * (decimal)cont.CantidadPonderada : 0;
                posKil.PrecioPonderadoDolares = cont.MonedaId == "USDM " ? cont.Precio * (decimal)cont.CantidadPonderada : 0;
                posKil.CantidadPonderada = cont.CantidadPonderada;

                posicionKilos.Add(posKil);
            }
            foreach (var negocio in negocios)
            {
                var pos = posicionKilos.Where(a => a.NegocioId == negocio.Id).Single();
                negocio.MesPosicion = pos.Mes.ToString() + " " + pos.Anio;
            }
        }
        public string DetallePosicionModalIds(List<int> negocios, string moneda, int? verDepositoTipoNegocio)
        {
            var detalle = TraerDetallePosicion(negocios, moneda, verDepositoTipoNegocio);
            detalle.ForEach(x => x.Cantidad = (int.Parse(x.Cantidad)).ToString("n0"));
            detalle.ForEach(x => x.Precio = decimal.Parse(x.Precio.Replace('.', ',')).ToString("n2"));
            detalle.ForEach(x => x.PrecioNeto = decimal.Parse(x.PrecioNeto.Replace('.', ',')).ToString("n2"));
            return JsonConvert.SerializeObject(new { items = detalle, total = detalle.Count() }); ;
        }

        public string SustentablePosicionModalIds(List<int> negocios, string moneda)
        {
            var detalle = TraerDetallePosicion(negocios, moneda, null);
            var resultado = detalle.GroupBy(a => a.FechaHastaDate.ToString("MMMM - yyyy").ToUpper()).Select(a => new { posicion = a.Key, cantidad = (a.Sum(x => x.CantidadD) / 1000) }).ToList();
            resultado.Add(new { posicion = "TOTAL", cantidad = resultado.Sum(x => x.cantidad) });
            return JsonConvert.SerializeObject(new { items = resultado, total = resultado.Count() - 1 });
            //return resultado;
        }
        private List<DetalleContratoDto> TraerDetallePosicion(List<int> negocios, string moneda, int? verDepositoTipoNegocio)
        {
            if (string.IsNullOrWhiteSpace(moneda))
                moneda = "";
            var data = new List<DetalleContratoDto>();
            var contratos = repositorio.Listar<Contrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = ((x.EstadoId == (int)EnumEstadoContrato.Finalizado || x.EstadoId == (int)EnumEstadoContrato.PreAnulado) && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Madre == true ? "MADRE" : x.Madre == false ? "HIJO" : (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "A PRECIO" : x.TipoNegocio.Descripcion,
                TipoNegocioId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? 2 : x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadD = x.Cantidad,
                CantidadDeposito = x.CantidadDeposito,
                CantidadCamiones = x.CantidadCamiones.ToString(),
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? (x.PrecioNetoPonderado ?? 0).ToString() : x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "USD" : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaDate = x.Fecha,
                Provincia = x.Provincia != null ? x.Provincia.Nombre : "",
                Localidad = x.Localidad != null ? x.Localidad.Nombre : "",
                Boleto = x.Boleto != null ? x.Boleto.Descripcion : "",
                Bolsa = x.Bolsa != null ? x.Bolsa.Descripcion : "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = x.CondicionFijacion != null ? x.CondicionFijacion.Descripcion : "",
                DesdeFijacion = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                HastaFijacion = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                Base = x.Base == true ? "X" : "",
                ImporteSustentable = x.ImporteSustentable.HasValue && x.MonedaSustentable != null ? x.ImporteSustentable.Value.ToString() + " " + x.MonedaSustentable.Descripcion : "0",
                FechaDolarizado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado) + "/" + SqlFunctions.DatePart("month", x.FechaDolarizado) + "/" + SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                DiasPesificado = x.DiasPesificado.ToString(),
                NoInformaSio = x.NoInformaSio == true ? "X" : "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = x.Consignatario == true ? "X" : "",
                PlanCanje = x.PlanCanje == true ? "X" : "",
                Pago = x.PagoDirectoVendedor == true ? "Pago Dir. Vend." : x.CD == true ? "CD" : x.Warrant == true ? "Warrant" : "",
                CalidadEspecial = x.StandardDeCalidadId == 2 || x.StandardDeCalidadId == 6 || x.StandardDeCalidadId == 7 ? "X" : "",
                EstablecimientoPropio = x.EstablecimientoPropio == true ? "Propio" : x.EstablecimientoPropio == false ? "Arrendado" : "",
                Observacion = x.Observacion ?? "",
                PrecioNeto = x.Condicional == true ?
                        (x.AperturaPrecio.Any(a => a.ConceptoAperturaPrecioId == 3 && a.Porcentaje > 0) ?
                         /*calculo con %*/(x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe) + ((x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe)) * x.AperturaPrecio.FirstOrDefault(a => a.ConceptoAperturaPrecioId == 3).Porcentaje / 100)) :
                        /*calculo sin % */x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe)
                        ).ToString() : (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString(),
                MercsDeposito = x.MercsDeposito == true ? "X" : "",
                Pizarra = x.Pizarra,
                FechaOperacion = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),
            },
            x => negocios.Contains(x.Id)
                && (moneda == "" || x.MonedaId == moneda || (moneda == "USDM " && x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) || (x.Pizarra == true && moneda == "ARP  "))
            );

            if (verDepositoTipoNegocio != null)
            {
                var negociosConDescarga = contratos.Where(a => a.TipoNegocioId == 2 && a.CantidadDeposito > 0).ToList();

                foreach (var x in negociosConDescarga)
                {
                    var negocio = contratos.Where(y => y.Contrato == x.Contrato).Single();
                    var fijacion = (DetalleContratoDto)negocio.Clone();
                    fijacion.CantidadD = fijacion.CantidadDeposito.GetValueOrDefault(0);
                    fijacion.Cantidad = fijacion.CantidadD.ToString();
                    contratos.Add(fijacion);
                    negocio.CantidadD -= negocio.CantidadDeposito.GetValueOrDefault(0);
                    negocio.CantidadDeposito = null;
                    negocio.Cantidad = negocio.CantidadD.ToString();
                }
                if (verDepositoTipoNegocio == (int)EnumTipoNegocio.A_PRECIO)
                {
                    contratos = contratos.Where(x => x.CantidadDeposito == 0 || x.CantidadDeposito == null).ToList();
                }
                else if (verDepositoTipoNegocio == (int)EnumTipoNegocio.FIJACION)
                {
                    contratos = contratos.Where(x => x.CantidadDeposito > 0).ToList();
                }
            }

            if (contratos != null)
            {
                data.AddRange(contratos);
            }

            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Id = x.Id,
                Contrato = ((x.EstadoId == (int)EnumEstadoContrato.Finalizado || x.EstadoId == (int)EnumEstadoContrato.PreAnulado) && x.FijacionSAP != null && x.FijacionSAP != "") ? x.FijacionSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Virtual == true ? "FIJACION VIRTUAL" : "FIJACION",
                TipoNegocioId = 3,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadD = x.Cantidad,
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaDate = x.Fecha,
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = x.Contrato == null ?
                    SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta) :
                    SqlFunctions.DateName("day", x.Contrato.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.Contrato.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.Contrato.HastaFijacion),
                Base = "",
                ImporteSustentable = "",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = x.Observacion ?? "",
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString(),
                MercsDeposito = "",
                Pizarra = x.Pizarra,
                FechaOperacion = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),

            },
             x => negocios.Contains(x.Id)
                && (moneda == "" || x.MonedaId == moneda || (x.Pizarra == true && moneda == "ARP  "))
            );

            foreach (var item in fijaciones.Where(a => a.TipoNegocio == "FIJACION VIRTUAL"))
            {
                item.PrecioNeto = ObtenerPrecioNetoFijacionVirtual(item.Id);
            }

            if (fijaciones != null)
            {
                data.AddRange(fijaciones);
            }

            //if (verDepositoTipoNegocio == -1)
            //{
            //    foreach (var cont in data)
            //    {
            //        if (cont.TipoNegocioId == 3)
            //        {
            //            if ((DateTime.DaysInMonth(cont.FechaDesdeDate.Year, cont.FechaDesdeDate.Month) - cont.FechaDesdeDate.Day) >= 10)
            //            {
            //                cont.mesPosision = cont.FechaDesdeDate.Month;
            //                cont.anioPosision = cont.FechaDesdeDate.Year;
            //            }
            //            else if (cont.FechaDesdeDate.AddMonths(1).Month <= cont.FechaHastaDate.Month)
            //            {
            //                cont.FechaDesdeDate = cont.FechaDesdeDate.AddMonths(1);
            //                cont.mesPosision = cont.FechaDesdeDate.Month;
            //                cont.anioPosision = cont.FechaDesdeDate.Year;

            //            }
            //            else if (cont.FechaDesdeDate.AddMonths(1).Month > cont.FechaHastaDate.Month)
            //            {
            //                cont.mesPosision = cont.FechaHastaDate.Month;
            //                cont.anioPosision = cont.FechaHastaDate.Year;
            //            }
            //        }
            //        else if (cont.TipoNegocioId == 2)
            //        {
            //            if (new DateTime(cont.FechaDesdeDate.Year, cont.FechaDesdeDate.Month, 1) <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            //            {
            //                cont.mesPosision = DateTime.Now.Month;
            //                cont.anioPosision = DateTime.Now.Year;
            //            }
            //            else
            //            {
            //                cont.mesPosision = cont.FechaDesdeDate.Month;
            //                cont.anioPosision = cont.FechaDesdeDate.Year;
            //            }
            //        }
            //    }
            //    //if (cont.ClasificacionNegocio != EnumClasificacionNegocio.DisponibleFijacion &&
            //    //    cont.ClasificacionNegocio != EnumClasificacionNegocio.ForwardFijacion &&
            //    //    cont.ClasificacionNegocio != EnumClasificacionNegocio.NewCropFijacion)
            //    //{
            //    //    if ((DateTime.DaysInMonth(cont.FechaDesde.Year, cont.FechaDesde.Month) - cont.FechaDesde.Day) >= 10)
            //    //    {
            //    //        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
            //    //        posKil.Anio = cont.FechaDesde.Year;
            //    //    }
            //    //    else if (cont.FechaDesde.AddMonths(1).Month <= cont.FechaHasta.Month)
            //    //    {
            //    //        cont.FechaDesde = cont.FechaDesde.AddMonths(1);
            //    //        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
            //    //        posKil.Anio = cont.FechaDesde.Year;

            //    //    }
            //    //    else if (cont.FechaDesde.AddMonths(1).Month > cont.FechaHasta.Month)
            //    //    {
            //    //        posKil.Mes = (EnumMeses)cont.FechaHasta.Month;
            //    //        posKil.Anio = cont.FechaHasta.Year;
            //    //    }
            //    //}
            //    //else
            //    //{
            //    //    if (new DateTime(cont.FechaDesde.Year, cont.FechaDesde.Month, 1) <= new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1))
            //    //    {
            //    //        posKil.Mes = (EnumMeses)DateTime.Now.Month;
            //    //        posKil.Anio = DateTime.Now.Year;
            //    //    }
            //    //    else
            //    //    {
            //    //        posKil.Mes = (EnumMeses)cont.FechaDesde.Month;
            //    //        posKil.Anio = cont.FechaDesde.Year;
            //    //    }
            //    //}
            //}


            var fasones = repositorio.Listar<Fason, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "FASÓN",
                TipoNegocioId = 4,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert(x.Cantidad),
                CantidadD = x.Cantidad,
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaDate = x.Fecha,
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString(),
                MercsDeposito = "",
                Pizarra = x.Pizarra,
                FechaOperacion = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),
            },
             x => negocios.Contains(x.Id)
                && (moneda == "" || x.MonedaId == moneda)
            );
            if (fasones != null)
            {
                data.AddRange(fasones);
            }




            var acuerdos = repositorio.Listar<ContratoAcuerdo, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "Acuerdo",
                TipoNegocioId = 6,
                CampanaId = null,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadD = (double)x.Cantidad,
                CantidadCamiones = "",
                Campana = "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda.Descripcion,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaDate = x.Fecha,
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "0",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = "",
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.StandardDeCalidadId == 2 || x.StandardDeCalidadId == 7 ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString(),
                MercsDeposito = "",
                Pizarra = x.Pizarra,
                FechaOperacion = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),
            },
             x => negocios.Contains(x.Id)
                && (moneda == "" || x.MonedaId == moneda)
            );


            if (acuerdos != null)
            {
                data.AddRange(acuerdos);
            }


            var agente = repositorio.Listar<AgenteCompra, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = "",
                Cuit = "",
                RazonCorredor = "",
                CuitCorredor = "",
                Material = x.Material.Descripcion,
                TipoNegocio = "Agente",
                TipoNegocioId = 6,
                CampanaId = null,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.Fecha,
                FechaHastaDate = x.Fecha,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadD = x.Cantidad,
                CantidadCamiones = "",
                Campana = "",
                FechaDesde = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaHasta = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda.Descripcion,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaDate = x.Fecha,
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "0",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = "",
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString(),
                MercsDeposito = "",
                Pizarra = x.Pizarra,
                FechaOperacion = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),

            },
            x => negocios.Contains(x.Id)
                && (moneda == "" || x.MonedaId == moneda)
            );

            if (agente != null)
            {
                data.AddRange(agente);
            }

            if (data.Count > 0 && data.Any(a => a.Pizarra == true))
            {
                var maxFecha = data.Where(a => a.Pizarra == true).Max(a => a.FechaDate);
                var preciosPizarra = repositorio.Listar<PrecioPizarra>(x => x.FechaHasta <= maxFecha);
                foreach (var item in data.Where(a => a.Pizarra == true && a.TipoNegocioId != 1).ToList())
                {
                    var precioPizarra = preciosPizarra.Where(x => x.Material.Descripcion == item.Material && x.FechaHasta <= item.FechaDate).ToList();
                    var precio = precioPizarra.Count != 0 ? precioPizarra.OrderByDescending(x => x.FechaHasta).FirstOrDefault() : new PrecioPizarra();
                    item.Moneda = precio.MonedaId;
                    item.Precio = precio.Precio.ToString();
                }
            }

            return data;
        }


        private string ObtenerPrecioNetoFijacionVirtual(int idFijacionVirtual)
        {
            var fijacionVirtual = repositorio.Obtener<FijacionDePrecioContrato>(idFijacionVirtual);
            decimal precioNeto = fijacionVirtual.Precio;
            if (fijacionVirtual.Contrato != null)
            {
                var desc = fijacionVirtual.Contrato.Descuentos.Where(y => y.TipoDBId == 1 && y.TipoPeriodoDBId == 1 && (y.Porcentaje != 0 || y.Importe != 0)).SingleOrDefault();
                if (desc != null)
                {
                    precioNeto += desc.Importe;
                    if (desc.Porcentaje != 0)
                    {
                        precioNeto += precioNeto * desc.Porcentaje / 100;
                    }
                }
            }
            precioNeto = Decimal.Parse(precioNeto.ToString("0.00"));
            return precioNeto.ToString("0.00").Replace(",", ".");

        }

        private List<DetalleContratoDto> TraerDetallePosicion(int materialId, int? mes, int? anio, DateTime fechaDesdeFiltro, DateTime fechaHastaFiltro, int? calidad, int centroId = 0, bool verFijaciones = true)
        {
            var fechaHoy = fechaDesdeFiltro.Date;
            var fechaManana = fechaHastaFiltro.Date;
            var listaDatos = new List<string[]>();
            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(+1);
            if (mes.HasValue && anio.HasValue)
            {
                fechaPosicion = new DateTime(anio.Value, mes.Value, 1);
            }

            var contratos = repositorio.Listar<Contrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = ((x.EstadoId == (int)EnumEstadoContrato.Finalizado || x.EstadoId == (int)EnumEstadoContrato.PreAnulado) && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Madre == true ? "MADRE" : x.Madre == false ? "HIJO" : (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "A PRECIO" : x.TipoNegocio.Descripcion,
                TipoNegocioId = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? 2 : x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = x.CantidadCamiones.ToString(),
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? (x.PrecioNetoPonderado ?? 0).ToString() : x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : (x.TipoPosicionCBOTId == 3 && x.TipoNegocioId == 1) ? "USD" : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Provincia = x.Provincia != null ? x.Provincia.Nombre : "",
                Localidad = x.Localidad != null ? x.Localidad.Nombre : "",
                Boleto = x.Boleto != null ? x.Boleto.Descripcion : "",
                Bolsa = x.Bolsa != null ? x.Bolsa.Descripcion : "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = x.CondicionFijacion != null ? x.CondicionFijacion.Descripcion : "",
                DesdeFijacion = x.DesdeFijacion != null ? SqlFunctions.DateName("day", x.DesdeFijacion) + "/" + SqlFunctions.DatePart("month", x.DesdeFijacion) + "/" + SqlFunctions.DateName("year", x.DesdeFijacion) : "",
                HastaFijacion = x.HastaFijacion != null ? SqlFunctions.DateName("day", x.HastaFijacion) + "/" + SqlFunctions.DatePart("month", x.HastaFijacion) + "/" + SqlFunctions.DateName("year", x.HastaFijacion) : "",
                Base = x.Base == true ? "X" : "",
                ImporteSustentable = x.ImporteSustentable.HasValue && x.MonedaSustentable != null ? x.ImporteSustentable.Value.ToString() + " " + x.MonedaSustentable.Descripcion : "0",
                FechaDolarizado = x.FechaDolarizado != null ? SqlFunctions.DateName("day", x.FechaDolarizado) + "/" + SqlFunctions.DatePart("month", x.FechaDolarizado) + "/" + SqlFunctions.DateName("year", x.FechaDolarizado) : "",
                DiasPesificado = x.DiasPesificado.ToString(),
                NoInformaSio = x.NoInformaSio == true ? "X" : "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = x.Consignatario == true ? "X" : "",
                PlanCanje = x.PlanCanje == true ? "X" : "",
                Pago = x.PagoDirectoVendedor == true ? "Pago Dir. Vend." : x.CD == true ? "CD" : x.Warrant == true ? "Warrant" : "",
                CalidadEspecial = x.StandardDeCalidadId == 2 || x.StandardDeCalidadId == 6 || x.StandardDeCalidadId == 7 ? "X" : "",
                EstablecimientoPropio = x.EstablecimientoPropio == true ? "Propio" : x.EstablecimientoPropio == false ? "Arrendado" : "",
                Observacion = x.Observacion ?? "",
                PrecioNeto = x.Condicional == true ?
                        (x.AperturaPrecio.Any(a => a.ConceptoAperturaPrecioId == 3 && a.Porcentaje > 0) ?
                         /*calculo con %*/(x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe) + ((x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe)) * x.AperturaPrecio.FirstOrDefault(a => a.ConceptoAperturaPrecioId == 3).Porcentaje / 100)) :
                        /*calculo sin % */x.Precio + x.AperturaPrecio.Where(a => a.ConceptoAperturaPrecioId != 4).Sum(a => a.Importe)
                        ).ToString() : (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString()
            },
            x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5 || x.EstadoId == 10)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.StandardDeCalidadId == calidad))
             && (centroId == 0 || x.DestinoId == centroId)
             && x.ContratoAcuerdoId == null && /*x.Canje != true &&*/ x.PrestamoDevolucion != true && x.Venta != true);
            var data = contratos;
            if (mes.HasValue && anio.HasValue)
            {
                data = FiltrardetalleContratosPorMesAnio(contratos, mes.Value, anio.Value);
            }


            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = ((x.EstadoId == (int)EnumEstadoContrato.Finalizado || x.EstadoId == (int)EnumEstadoContrato.PreAnulado) && x.FijacionSAP != null && x.FijacionSAP != "") ? x.FijacionSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "FIJACION",
                TipoNegocioId = 3,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = x.Observacion ?? "",
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString(),
                Virtual = x.Virtual,
                Id = x.Id
            },
            x => verFijaciones
             && DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5 || x.EstadoId == 10)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.TrigoEspecial == true && calidad == 7) || (calidad != null && x.TrigoEspecial == false && calidad == 3))
             && (centroId == 0 || centroId == x.DestinoId) && x.Canje != true && x.PrestamoDevolucion != true && x.Venta != true && x.TipoPosicionCBOTId != 3);

            foreach (var item in fijaciones.Where(a => a.Virtual == true))
            {

                item.PrecioNeto = ObtenerPrecioNetoFijacionVirtual(item.Id);
            }
            if (mes.HasValue && anio.HasValue)
            {
                foreach (var i in fijaciones)
                {
                    var fechaDesde = DateTime.Parse(i.FechaDesde);
                    var posicion = new DateTime(fechaDesde.Year, fechaDesde.Month, 1);
                    var hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    if (posicion <= hoy)
                    {
                        if (mes == hoy.Month && anio == hoy.Year)
                        {
                            data.Add(i);
                        }
                    }
                    else if (mes == posicion.Month && anio == posicion.Year)
                    {
                        data.Add(i);
                    }
                }
            }
            else
            {
                data.AddRange(fijaciones);
            }

            var fasones = repositorio.Listar<Fason, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "FASÓN",
                TipoNegocioId = 4,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert(x.Cantidad),
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = x.Ampliaciones.ToString(),
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString()
            },
            x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.TrigoEspecial == true && calidad == 7) || (calidad != null && x.TrigoEspecial == false && calidad == 3))
             && (centroId == 0 || centroId == 1));
            if (mes.HasValue && anio.HasValue)
            {
                foreach (var i in fasones)
                {
                    var fechaDesde = DateTime.Parse(i.Fecha);
                    var posicion = new DateTime(fechaDesde.Year, fechaDesde.Month, 1);
                    var hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    if (posicion <= hoy)
                    {
                        if (mes == hoy.Month && anio == hoy.Year)
                        {
                            data.Add(i);
                        }
                    }
                    else if (mes == posicion.Month && anio == posicion.Year)
                    {
                        data.Add(i);
                    }
                }
            }
            else
            {
                data.AddRange(fasones);
            }


            var acuerdos = repositorio.Listar<ContratoAcuerdo, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "Acuerdo",
                TipoNegocioId = 6,
                CampanaId = null,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = "",
                Campana = "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda.Descripcion,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Provincia = "",
                Localidad = "",
                Boleto = "",
                Bolsa = "",
                Destino = x.Destino != null ? x.Destino.Descripcion : "",
                CondicionFijacion = "",
                DesdeFijacion = "",
                HastaFijacion = "",
                Base = "",
                ImporteSustentable = "0",
                FechaDolarizado = "",
                DiasPesificado = "",
                NoInformaSio = "",
                Ampliaciones = "",
                Consignatario = "",
                PlanCanje = "",
                Pago = "",
                CalidadEspecial = x.StandardDeCalidadId == 2 || x.StandardDeCalidadId == 7 ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString()
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             //&& (x.PrecioNeto != null && x.PrecioNeto != 0)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.StandardDeCalidadId == calidad))
             && (centroId == 0 || x.DestinoId == centroId));
            if (mes.HasValue && anio.HasValue)
            {
                foreach (var i in acuerdos)
                {
                    var fechaDesde = DateTime.Parse(i.FechaDesde);
                    var posicion = new DateTime(fechaDesde.Year, fechaDesde.Month, 1);
                    var hoy = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    if (posicion <= hoy)
                    {
                        if (mes == hoy.Month && anio == hoy.Year)
                        {
                            data.Add(i);
                        }
                    }
                    else if (mes == posicion.Month && anio == posicion.Year)
                    {
                        data.Add(i);
                    }
                }
            }
            else
            {
                data.AddRange(acuerdos);
            }


            return data;
        }

        private List<DetalleAgenteDto> TraerDetalleAgente(DateTime fechaDesdeFiltro, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var fechaHoy = fechaDesdeFiltro.Date;
            var listaDatos = new List<string[]>();
            var agente = repositorio.Listar<AgenteCompra, DetalleAgenteDto>(x => new DetalleAgenteDto
            {
                Agente = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                Operador = x.Operador.Descripcion,
                Material = x.Material.Descripcion,
                Posicion = x.Posicion,
                Cantidad = x.Cantidad,
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda.Descripcion,
                Fecha = SqlFunctions.DateName("day", x.FechaOperacion) + "/" + SqlFunctions.DatePart("month", x.FechaOperacion) + "/" + SqlFunctions.DateName("year", x.FechaOperacion),
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaHoy
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5), 0, "Agente");

            return agente;
        }
        private List<string[]> ConvertirListadetalleContratoAListaString(List<DetalleContratoDto> contratos, int mes, int anio)
        {
            var listaDatos = new List<string[]>();
            foreach (var cont in contratos)
            {
                listaDatos.Add(CrearArray(cont));
            }
            return listaDatos;
        }

        private string[] CrearArray(DetalleContratoDto x)
        {
            return new string[]{
                x.Contrato ?? "",
                x.RazonSocial ?? "",
                x.Cuit ?? "",
                x.RazonCorredor ?? "",
                x.CuitCorredor ?? "",
                x.Material ?? "",
                x.TipoNegocio ?? "",
                x.Comercial ?? "",
                x.Cantidad ?? "",
                x.CantidadCamiones ?? "",
                x.Campana ?? "",
                x.FechaDesde ?? "",
                x.FechaHasta ?? "",
                x.Precio ?? "",
                x.PrecioNeto ?? "",
                x.Moneda ?? "",
                x.Fecha ?? "",
                x.Provincia ?? "",
                x.Localidad ?? "",
                x.Boleto ?? "",
                x.Bolsa ?? "",
                x.Destino ?? "",
                x.CondicionFijacion ?? "",
                x.DesdeFijacion ?? "",
                x.HastaFijacion ?? "",
                x.Base ?? "",
                x.ImporteSustentable ?? "",
                x.FechaDolarizado ?? "",
                x.DiasPesificado ?? "",
                x.NoInformaSio ?? "",
                x.Ampliaciones?? "",
                x.Consignatario ?? "",
                x.PlanCanje ?? "",
                x.Pago ?? "",
                x.CalidadEspecial ?? "",
                x.EstablecimientoPropio ?? "",
                x.Observacion ?? ""
            };
        }
        private List<string[]> ConvertirListaAgente(List<DetalleAgenteDto> agente)
        {
            var listaDatos = new List<string[]>();
            foreach (var agen in agente)
            {
                var pos = agen.Posicion.Split('.');
                var dato = new string[] {
                    agen.Agente,
                    agen.Operador,
                    agen.Material,
                    (EnumMeses)Enum.ToObject(typeof(EnumMeses), int.Parse(pos[0])) + " - " + pos[1],
                    String.Format("{0:0.#}",agen.Cantidad),
                    agen.Precio,
                    agen.Moneda,
                    agen.Fecha,
                    agen.Comercial
                };
                listaDatos.Add(dato);
            }
            return listaDatos;
        }
        public string DetallePosicionModal(int materialId, int? mes, int? anio, DateTime fechaDesde, DateTime fechaHasta, int? calidad, int centroId = 0, bool verFijaciones = true)
        {
            var detalle = TraerDetallePosicion(materialId, mes, anio, fechaDesde, fechaHasta, calidad, centroId, verFijaciones);
            detalle.ForEach(x => x.Cantidad = (int.Parse(x.Cantidad)).ToString("n0"));
            detalle.ForEach(x => x.Precio = decimal.Parse(x.Precio.Replace('.', ',')).ToString("n2"));
            detalle.ForEach(x => x.PrecioNeto = decimal.Parse(x.PrecioNeto.Replace('.', ',')).ToString("n2"));
            return JsonConvert.SerializeObject(new { items = detalle, total = detalle.Count() }); ;
        }
        public string DetalleAgenteModal(DateTime fecha, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var data = TraerDetalleAgente(fecha, materialId);
            data.ForEach(x => x.Cantidad = x.Cantidad);
            data.ForEach(x => x.Precio = decimal.Parse(x.Precio.Replace('.', ',')).ToString("n2"));
            return JsonConvert.SerializeObject(new { items = data, total = data.Count() }); ;
        }
        private List<DetalleContratoDto> FiltrardetalleContratosPorMesAnio(List<DetalleContratoDto> contratos, int mes, int anio)
        {
            var retorno = new List<DetalleContratoDto>();
            foreach (var cont in contratos)
            {
                var fechaDesde = DateTime.Parse(cont.FechaDesde);
                var fechaHasta = DateTime.Parse(cont.FechaHasta);
                var posicion = new DateTime();
                if ((DateTime.DaysInMonth(fechaDesde.Year, fechaDesde.Month) - fechaDesde.Day) >= 10)
                {
                    posicion = new DateTime(fechaDesde.Year, fechaDesde.Month, 1);
                }
                else if (fechaDesde.AddMonths(1).Month <= fechaHasta.Month)
                {
                    posicion = new DateTime(fechaDesde.Year, fechaDesde.Month, 1).AddMonths(+1);
                }
                else if (fechaDesde.AddMonths(1).Month > fechaHasta.Month)
                {
                    posicion = new DateTime(fechaHasta.Year, fechaHasta.Month, 1);
                }

                if (posicion.Month == mes && posicion.Year == anio)
                {
                    retorno.Add(cont);
                }
            }
            return retorno;
        }

        public ReporteCompraNetModel ObtenerDatosReporteCompraNet(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId, bool verFijaciones = true)
        {
            int idCentro = int.Parse(centroId);
            bool filtrarAcopio = idCentro == 0 || idCentro == 1;
            var agentes = filtrarAcopio ? TraerAgenteDeCompra(fechaDesde, fechaHasta, materialId) : new List<AgenteCompraDto>();
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

            var objetivos = TraerHedgeObjetivo(fechaDesde, fechaHasta, materialId, verFijaciones);
            objetivos.PricingCumplido = objetivos.PricingCumplido;
            objetivos.PricingObjetivo = objetivos.PricingObjetivo;
            objetivos.RemitirCumplido = objetivos.RemitirCumplido;
            objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
            var result = new ReporteCompraNetModel
            {
                ToneladasGranoTipo = TraerToneladasGranoTipo(fechaDesde, fechaHasta, materialId, idCentro),
                SojaSustentable = (materialId == null || materialId.Contains(3)) ? TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro) : new ReporteSojaSustDto(),
                PosicionCompras = TraerPosicionCompras(fechaDesde, fechaHasta, materialId, idCentro, false, verFijaciones),
                PricingCampania = TraerPricingCampania(fechaDesde, fechaHasta, materialId, idCentro, verFijaciones),
                PrecioCantidad = TraerMonedaCantidad(fechaDesde, fechaHasta, materialId, idCentro, verFijaciones),
                HedgeMaterial = TransformarAModel(TraerTodosHedgeMaterial(fechaDesde, fechaHasta, materialId)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = TraerTcPromedio(fechaDesde, fechaHasta, materialId, verFijaciones),
                AgenteCompras = new AgenteCompraModel { ListaAgenteCompras = agentes, ListaOperadores = op },
            };
            return result;
        }
        private List<HedgeMaterialModel> TransformarAModel(List<HedgeMaterialDto> hedgeMat)
        {
            var lista = new List<HedgeMaterialModel>()
            {
                new HedgeMaterialModel {MaterialId = 1, MaterialDescripcion ="Hedge Maíz",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 1 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad)},
                new HedgeMaterialModel {MaterialId = 3, MaterialDescripcion ="Hedge Soja",
                Disponible = hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 1).Sum(x=>x.Cantidad),
                Forward= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 2).Sum(x=>x.Cantidad),
                NewCrop= hedgeMat.Where(x=>x.MaterialId == 3 && x.TipoHedgeMaterialId == 3).Sum(x=>x.Cantidad) }
            };
            return lista;
        }

        public void GrabarDatosReporteCompraNet(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId)
        {
            try
            {
                logger.Debug("GrabarDatosReporteCompraNet - inicio obtener datos");
                ReporteCompraNetModel result = ObtenerDatosReporteCompraNet(fechaDesde, fechaHasta, centroId, materialId);
                logger.Debug("GrabarDatosReporteCompraNet - fin obtener datos");
                var PosicionCompras = TraerPosicionCompras(fechaDesde, fechaHasta, materialId, int.Parse(centroId), true);
                List<ReporteCompraNetPosicionCompras> posicionCompras = new List<ReporteCompraNetPosicionCompras>();
                List<ReporteCompraNetPrecioCantidad> precioCantidad = new List<ReporteCompraNetPrecioCantidad>();
                ReporteCompraNetSojaSustentable sojaSustentable = new ReporteCompraNetSojaSustentable();
                List<ReporteCompraNetHedgeMaterial> hedgeMaterial = new List<ReporteCompraNetHedgeMaterial>();
                ReporteCompraNetHedgeCargaObjetivo hedgeObjetivo = new ReporteCompraNetHedgeCargaObjetivo();
                ReporteCompraNetHedgeTCPromedio TCPromedioDto = new ReporteCompraNetHedgeTCPromedio();
                List<ReporteCompraNetAgenteCompra> agenteCompras = new List<ReporteCompraNetAgenteCompra>();
                List<ReporteCompraNetPricingCampania> pricingCampania = new List<ReporteCompraNetPricingCampania>();


                logger.Debug("GrabarDatosReporteCompraNet - inicio armadatos");
                logger.Debug("GrabarDatosReporteCompraNet - inicio armadatos posicioncompras");
                foreach (var posicion in PosicionCompras)
                {
                    foreach (var kilos in posicion.PosicionKilos)
                    {
                        posicionCompras.Add(new ReporteCompraNetPosicionCompras
                        {
                            Material = posicion.Material,
                            MaterialId = posicion.MaterialId,
                            Anio = kilos.Anio,
                            CantidadPonderada = kilos.CantidadPonderada,
                            DispAFijar = kilos.DispAFijar,
                            DispAPrecio = kilos.DispAPrecio,
                            DispFijac = kilos.DispFijac,
                            FrwAFijar = kilos.FrwAFijar,
                            FrwAPrecio = kilos.FrwAPrecio,
                            FrwFijac = kilos.FrwFijac,
                            KilosDolares = kilos.KilosDolares,
                            KilosPesos = kilos.KilosPesos,
                            Mes = kilos.Mes.ToString(),
                            NewAFijar = kilos.NewAFijar,
                            NewAPrecio = kilos.NewAPrecio,
                            NewFijac = kilos.NewFijac,
                            PrecioPonderadoDolares = kilos.PrecioPonderadoDolares,
                            PrecioPonderadoPesos = kilos.PrecioPonderadoPesos,
                            DispAPrecioPesos = kilos.DispAPrecioPesos,
                            DispAPrecioDolares = kilos.DispAPrecioDolares,
                            DispFijacPesos = kilos.DispFijacPesos,
                            DispFijacDolares = kilos.DispFijacDolares,
                            FrwAPrecioPesos = kilos.FrwAPrecioPesos,
                            FrwAPrecioDolares = kilos.FrwAPrecioDolares,
                            FrwFijacPesos = kilos.FrwFijacPesos,
                            FrwFijacDolares = kilos.FrwFijacDolares,
                            NewAPrecioPesos = kilos.NewAPrecioPesos,
                            NewAPrecioDolares = kilos.NewAPrecioDolares,
                            NewFijacPesos = kilos.NewFijacPesos,
                            NewFijacDolares = kilos.NewFijacDolares,
                            CampanaId = kilos.CampanaId,
                            Campana = kilos.Campana
                        });
                    }

                }
                logger.Debug("GrabarDatosReporteCompraNet - inicio preciocantidad");
                foreach (var moneda in result.PrecioCantidad)
                {
                    precioCantidad.Add(new ReporteCompraNetPrecioCantidad
                    {
                        Cantidad = moneda.Cantidad,
                        Moneda = moneda.Moneda
                    });
                }
                logger.Debug("GrabarDatosReporteCompraNet - inicio sojasustentable");
                sojaSustentable = new ReporteCompraNetSojaSustentable
                {
                    AFijar = result.SojaSustentable.Fijar,
                    APrecio = result.SojaSustentable.Precio,
                    Total = result.SojaSustentable.Total
                };
                logger.Debug("GrabarDatosReporteCompraNet - inicio hedge material");
                foreach (var material in result.HedgeMaterial)
                {
                    hedgeMaterial.Add(new ReporteCompraNetHedgeMaterial
                    {
                        Disponible = material.Disponible,
                        Forward = material.Forward,
                        Material = material.MaterialDescripcion,
                        NewCrop = material.NewCrop
                    });
                }
                logger.Debug("GrabarDatosReporteCompraNet - inicio hedge obejetivo");
                hedgeObjetivo = new ReporteCompraNetHedgeCargaObjetivo
                {
                    PricingCumplido = result.HedgeObjetivo.PricingCumplido,
                    PricingObjetivo = result.HedgeObjetivo.PricingObjetivo,
                    RemitirCumplido = result.HedgeObjetivo.RemitirCumplido,
                    RemitirObjetivo = result.HedgeObjetivo.RemitirObjetivo,
                };
                logger.Debug("GrabarDatosReporteCompraNet - inicio hedgetcpromedio");
                TCPromedioDto = new ReporteCompraNetHedgeTCPromedio
                {
                    PromedioTC = result.TCPromedioDto.PromedioTC,
                    TotalTC = result.TCPromedioDto.TotalTC
                };
                logger.Debug("GrabarDatosReporteCompraNet - inicio agente");
                foreach (var agente in result.AgenteCompras.ListaAgenteCompras)
                {
                    foreach (var item in agente.Operador)
                    {
                        agenteCompras.Add(new ReporteCompraNetAgenteCompra
                        {
                            Cantidad = item.Cantidad,
                            Material = agente.MaterialDesc,
                            MaterialId = agente.MaterialId,
                            Operador = item.OperadorDesc,
                            OperadorId = item.OperadorId,
                            Posicion = agente.Posicion,
                            PrecioPonderado = agente.PrecioPonderado,
                            TipoAgente = agente.TipoAgenteDesc,
                            TipoAgenteId = agente.TipoAgenteId

                        });
                    }

                }
                logger.Debug("GrabarDatosReporteCompraNet - inicio suma pricing");
                foreach (var datos in result.PricingCampania)
                {
                    double sumaPricing;
                    switch (datos.Id)
                    {
                        case 11:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 12:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 1).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Maiz").Sum(x => x.NewAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 21:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Trigo"/*"Trigo Cámara" ||*/ /*x.Material == "Trigo Calidad"*/).Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 22:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Trigo"/*"Trigo Cámara" ||*/ /*x.Material == "Trigo Calidad"*/).Sum(x => x.NewAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 31:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 32:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 3).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Soja").Sum(x => x.NewAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 41:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 42:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 4).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Girasol").Sum(x => x.NewAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 51:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 52:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 5).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Girasol Alto Oleico").Sum(x => x.NewAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        default:
                            sumaPricing = 0;
                            datos.Pricing = sumaPricing;

                            break;
                    }
                }
                logger.Debug("GrabarDatosReporteCompraNet - inicio pricing");
                foreach (var pricing in result.PricingCampania)
                {
                    pricingCampania.Add(new ReporteCompraNetPricingCampania
                    {
                        Material = pricing.Material,
                        MaterialId = pricing.MaterialId,
                        Acopio = pricing.Acopio,
                        Campania = pricing.Campania,
                        CampaniaId = pricing.CampaniaId,
                        Pricing = pricing.Pricing,
                        SanLorenzo = pricing.SanLorenzo
                    });
                }
                logger.Debug("GrabarDatosReporteCompraNet - fin armadatos");


                logger.Debug("GrabarDatosReporteCompraNet - inicio  removertodos");

                repositorio.RemoverTodos<ReporteCompraNetPosicionCompras>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetPrecioCantidad>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetSojaSustentable>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetHedgeMaterial>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetHedgeCargaObjetivo>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetHedgeTCPromedio>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetAgenteCompra>(a => true);
                repositorio.RemoverTodos<ReporteCompraNetPricingCampania>(a => true);
                repositorio.GuardarCambios();
                logger.Debug("GrabarDatosReporteCompraNet - fin removertodos");

                logger.Debug("GrabarDatosReporteCompraNet - inicio agregartodos");
                repositorio.AgregarTodos(posicionCompras);
                repositorio.AgregarTodos(precioCantidad);
                repositorio.Agregar(sojaSustentable);
                repositorio.AgregarTodos(hedgeMaterial);
                repositorio.Agregar(hedgeObjetivo);
                repositorio.Agregar(TCPromedioDto);
                repositorio.AgregarTodos(agenteCompras);
                repositorio.AgregarTodos(pricingCampania);
                repositorio.GuardarCambios();
                logger.Debug("GrabarDatosReporteCompraNet - fin agregartodos");

                logger.Debug("GrabarDatosReporteCompraNet - inicio migrar");
                repositorio.MigrarReporteCompraNetPosicionCompras();
                logger.Debug("GrabarDatosReporteCompraNet - fin migrar");
            }
            catch (Exception e)
            {
                logger.Error("GrabarDatosReporteCompraNet", e);
            }

        }

        public ReporteEvolucionFijacionModel ObtenerDatosReporteEvolucionFijacion(DateTime desde, DateTime hasta, int? ProveedorId, int? ComercialId, int? CampanaId,
            int? MaterialId, int? GrupoCompraId, int? ClasificacionId, int? DestinoId)
        {
            ReporteEvolucionFijacionModel result = new ReporteEvolucionFijacionModel();
            List<MaterialDto> materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, Descripcion = x.Descripcion });
            materiales.Add(new MaterialDto { Descripcion = "TOTAL", MaterialId = 0 });

            var afijar = repositorio.Listar<Contrato, BasicoContrato>(x => new BasicoContrato
            {
                ContratoSAP = x.ContratoSAP,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                FechaDesdeFormateado = SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" + SqlFunctions.DateName("year", x.FechaHasta),
                Fecha = x.Fecha,
                Cantidad = x.Cantidad,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                ClasificacionId = x.ClasificacionId,
                ClasificacionDescripcion = x.Clasificacion.Descripcion,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                ComercialId = x.ComercialId,
                ProveedorId = x.ProveedorId.Value,
                Proveedor = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT
            }, x => x.TipoNegocioId == 1 && x.FechaHasta >= desde && x.FechaHasta <= hasta && (x.EstadoId == 2 || x.EstadoId == 5)
             && (ProveedorId == null || x.ProveedorId == ProveedorId)
             && (ComercialId == null || x.ComercialId == ComercialId)
             && (CampanaId == null || x.CampanaId == CampanaId)
             && (MaterialId == null || x.MaterialId == MaterialId)
             && (GrupoCompraId == null || x.GrupoCompra == GrupoCompraId)
             && (ClasificacionId == null || x.ClasificacionId == ClasificacionId)
             && (DestinoId == null || x.DestinoId == DestinoId)
            );
            result.afijar = afijar;
            List<string> contratossap = afijar.Select(a => a.ContratoSAP).ToList();
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, BasicoContrato>(x => new BasicoContrato
            {
                ContratoSAP = x.FijacionSAP,
                ContratoMadre = x.ContratoSAP,
                FechaDesde = x.FechaDesde,
                FechaDesdeFormateado = SqlFunctions.StringConvert((double)x.FechaDesde.Month).TrimStart() + "-" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHastaFormateado = SqlFunctions.StringConvert((double)x.FechaHasta.Month).TrimStart() + "-" + SqlFunctions.DateName("year", x.FechaHasta),
                FechaHasta = x.FechaHasta,
                Fecha = x.Fecha,
                Cantidad = x.Cantidad,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido,
                ComercialId = x.ComercialId,
                ProveedorId = x.ProveedorId.Value,
                Proveedor = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT
            }, x =>
             (x.EstadoId == 2 || x.EstadoId == 5)
             && contratossap.Contains(x.ContratoSAP)
            );
            result.fijaciones = fijaciones;
            List<DateTime> meses = new List<DateTime>();
            var startdate = new DateTime(desde.Year, desde.Month, 1);
            var enddate = new DateTime(hasta.Year, hasta.Month, 1);
            //something on these lines 
            while (startdate <= enddate)
            {
                meses.Add(startdate);
                // pull out month and year
                startdate = startdate.AddMonths(1);
            }

            foreach (var item in meses)
            {
                foreach (var material in materiales)
                {
                    result.tablero.Add(new ReporteEvolucionFijacion
                    {
                        Anio = item.Year,
                        MesId = item.Month,
                        Mes = item.ToString("MMM").Replace(".", ""),
                        MesCompleto = item.ToString("MMMM"),
                        Material = material.Descripcion,
                        MaterialId = material.MaterialId,
                        TotalAfijar = 0,
                        TotalFijacion = 0,
                        UltimaSemana = 0,
                        Porcentaje = 0
                    });
                }

            }
            var ultimasemana = DateTime.Now.AddDays(-7).Date;
            foreach (var item in result.tablero)
            {
                var fecha = item.MesId.ToString() + "-" + item.Anio.ToString();
                var totalAFijar = afijar.Where(a => (item.MaterialId == 0 || item.MaterialId == a.MaterialId) && a.FechaHastaFormateado == fecha).Sum(x => x.Cantidad);
                var totalFijacion = fijaciones.Where(a => (item.MaterialId == 0 || item.MaterialId == a.MaterialId) && a.FechaHastaFormateado == fecha).Sum(x => x.Cantidad);
                var totalUltimaSemana = fijaciones.Where(a => (item.MaterialId == 0 || item.MaterialId == a.MaterialId) && a.FechaHastaFormateado == fecha && a.Fecha > ultimasemana).Sum(x => x.Cantidad);
                var porcentaje = item.MaterialId == 0 ? (totalFijacion * 100) / totalAFijar : 0;
                porcentaje = Math.Round(Double.IsNaN(porcentaje) ? 0 : porcentaje);

                item.TotalAfijar = totalAFijar / 1000;
                item.TotalFijacion = totalFijacion / 1000;
                item.UltimaSemana = totalUltimaSemana / 1000;
                item.Porcentaje = porcentaje;
            }
            result.tablero = result.tablero.OrderBy(a => a.MaterialId).ThenBy(a => a.Anio).ThenBy(a => a.MesId).ToList();
            return result;
        }

        public List<ClasificacionCompraNet> TraerTodoClasificacionCompraNet()
        {
            var combo = new CombosQueries(logger, repositorio);
            var result = combo.GetClasificacionCombo();
            return result;
        }

        public List<PesificarAgentDto> GrabarTodoDatoPesificar()
        {
            try
            {
                var proveedores = repositorio.Listar<Proveedor, ProveedorCombo>(
                    x => new ProveedorCombo
                    {
                        Cuit = x.CUIT,
                        ProveedorId = x.ProveedorId
                    });
                //proveedores = proveedores.Where(x => x.ProveedorId == 385).ToList();
                var cuits = proveedores.Select(y => y.ProveedorId);

                var corredores = repositorio.Listar<CorredorProveedor, ProveedorCombo>(
                     x => new ProveedorCombo
                     {
                         Cuit = x.Corredor.CUIT,
                         ProveedorId = x.CorredorId
                     }, x => cuits.Contains(x.CorredorId));

                foreach (var p in proveedores)
                {
                    p.Cuit = p.Cuit.Trim();

                    if (corredores.Any(x => x.ProveedorId == p.ProveedorId))
                    {
                        p.Cuit = "C" + p.Cuit.Remove(p.Cuit.Length - 1).Remove(0, 2);
                    }
                    else
                    {
                        p.Cuit = "00" + p.Cuit.Remove(p.Cuit.Length - 1).Remove(0, 2);
                    }
                }
                proveedores = proveedores.Distinct().ToList();
                var datos = new List<PesificarAgentDto>();
                while (proveedores.Count > 0)
                {
                    logger.Debug("Proveedores pesificados pendientes " + proveedores.Count());
                    var index = proveedores.Count >= 200 ? 200 : proveedores.Count;
                    var lista = proveedores.Take(index).ToList();
                    proveedores.RemoveRange(0, index);
                    datos.AddRange(pesificarAgent.ConsultarTodo(lista.Select(x => x.Cuit).Distinct().ToList()));
                }
                datos = BuscarPase(datos);

                List<ReportePesificado> items = ConvertPesificarAgent(datos.Where(x => x.Anticipo != "X").Distinct().ToList());
                items = items.Distinct().ToList();
                repositorio.TruncarTabla<ReportePesificado>();


                if (items.Count > 0)
                {
                    repositorio.AgregarTodos(items);
                    repositorio.GuardarCambios();
                }
                return datos;
            }
            catch (Exception e)
            {
                logger.Error("Error reporte");
                logger.Error(e);
                throw;
            }
        }

        private List<PesificarAgentDto> BuscarPase(List<PesificarAgentDto> pesificado)
        {
            var contratosAFijarPase = repositorio.Listar<Contrato>(a => a.TipoPosicionCBOTId == 3 && a.TipoNegocioId == 1 && a.EstadoId == 5);
            List<string> contratosAFijarPaseSAPList = contratosAFijarPase.Select(a => a.ContratoSAP).ToList();

            var fijacionesPase = repositorio.Listar<FijacionDePrecioContrato>(a => a.EstadoId == 5 && contratosAFijarPaseSAPList.Contains(a.ContratoSAP));

            var fijacionesKilos = fijacionesPase.GroupBy(a => a.ContratoSAP).Select(x => new BasicoContrato { ContratoSAP = x.Key, Cantidad = x.Sum(y => y.Cantidad) });

            List<Contrato> contratosAFijarPasePendientes = new List<Contrato>();
            foreach (var item in contratosAFijarPase)
            {
                var fijacionKg = fijacionesKilos.Where(x => x.ContratoSAP == item.ContratoSAP).SingleOrDefault();
                if (fijacionKg == null || item.Cantidad > fijacionKg.Cantidad)
                {
                    pesificado.Add(new PesificarAgentDto
                    {
                        Clasificacion = item.Clasificacion.Descripcion,
                        Comercial = item.Comercial.IdActiveDirectory,
                        Contrato = item.ContratoSAP,
                        CuitCorredor = item.Corredor == null ? "" : item.Corredor.CUIT,
                        CuitVendedor = item.Proveedor.CUIT,
                        Dolarizado = item.Corredor == null ? true : false,
                        DolarizadoExpress = false,
                        FechaFijacion = item.HastaFijacion,
                        DolarizadoNoProductor = item.Corredor == null ? false : true,
                        FechaUltimaAplicacion = null,
                        Fijacion = "",
                        KgNoPesificable = new DateTime(int.Parse(item.PosicionCBOT.Split('.').Last()), int.Parse(item.PosicionCBOT.Split('.').First()), 01) > DateTime.Now.Date ? Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)) : 0,
                        USDNoPesificable = new DateTime(int.Parse(item.PosicionCBOT.Split('.').Last()), int.Parse(item.PosicionCBOT.Split('.').First()), 01) > DateTime.Now.Date ? Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)) * Convert.ToInt32(item.PrecioPonderado) : 0,
                        KgVencimientoPesificable = new DateTime(int.Parse(item.PosicionCBOT.Split('.').Last()), int.Parse(item.PosicionCBOT.Split('.').First()), 01) <= DateTime.Now.Date ? Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)) : 0,
                        USDPesificable = new DateTime(int.Parse(item.PosicionCBOT.Split('.').Last()), int.Parse(item.PosicionCBOT.Split('.').First()), 01) <= DateTime.Now.Date ? Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)) * Convert.ToInt32(item.PrecioPonderado) : 0,
                        KgTotales = Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)),
                        CantidadPendiente = Convert.ToInt32(item.Cantidad - (fijacionKg == null ? 0 : fijacionKg.Cantidad)),
                        Material = item.Material.Codigo,
                        Unidad = "Kg",
                        Moneda = "USDM ",
                        Precio = item.PrecioPonderado ?? 0,
                        NombreCorredor = item.Corredor == null ? "" : item.Corredor.RazonSocial,
                        NombreVendedor = item.Proveedor.RazonSocial,
                        Pase = true,
                        Plus = item.Descuentos.Where(a => a.TipoPeriodoDBId == 1 && a.TipoDBId == 1).SingleOrDefault() != null ?
                        item.Descuentos.Where(a => a.TipoPeriodoDBId == 1 && a.TipoDBId == 1).SingleOrDefault().Importe : 0,
                        Posicion = item.PosicionCBOT,
                        KgTotalesPase = item.Cantidad,
                        FechaHastaDolarizado = new DateTime(int.Parse(item.PosicionCBOT.Split('.').Last()), int.Parse(item.PosicionCBOT.Split('.').First()), 01),
                    });
                }
            }

            return pesificado;
        }

        private List<ReportePesificado> ConvertPesificarAgent(List<PesificarAgentDto> datos)
        {

            var comerciales = repositorio.Listar<Comercial>();
            var materiales = repositorio.Listar<Material>();
            var monedas = repositorio.Listar<Moneda>();
            var contratosSap = datos.Where(x => string.IsNullOrEmpty(x.Fijacion)).Select(x => x.Contrato).ToList();
            var fijacionesSap = datos.Where(x => !string.IsNullOrEmpty(x.Fijacion)).Select(x => x.Fijacion).ToList();
            var contratos = repositorio.Listar<Contrato>(x => contratosSap.Contains(x.ContratoSAP)).Select(x => new KeyValuePair<string, int>(x.ContratoSAP, x.Id)).ToList();
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => fijacionesSap.Contains(x.FijacionSAP)).Select(x => new KeyValuePair<string, int>(x.FijacionSAP, x.Id)).ToList();
            logger.Debug(contratos.ToJson());
            logger.Debug(fijaciones.ToJson());
            return datos.Select(item => new ReportePesificado()
            {
                CantidadPendiente = item.CantidadPendiente,
                Clasificacion = item.Clasificacion,
                ComercialId = comerciales.Where(x => x.IdActiveDirectory == item.Comercial).FirstOrDefault() != null ? comerciales.Where(x => x.IdActiveDirectory == item.Comercial).FirstOrDefault().ComercialId : (int?)null,
                Contrato = item.Contrato,
                NegocioId = string.IsNullOrEmpty(item.Fijacion) ?
                (contratos.Any(x => x.Key == item.Contrato) ? contratos.Where(x => x.Key == item.Contrato).FirstOrDefault().Value : (int?)null) :
                (fijaciones.Any(x => x.Key == item.Fijacion) ? fijaciones.Where(x => x.Key == item.Fijacion).FirstOrDefault().Value : (int?)null),
                CuitCorredor = item.CuitCorredor,
                CuitVendedor = item.CuitVendedor,
                Dolarizado = item.Dolarizado,
                DolarizadoExpress = item.DolarizadoExpress,
                FechaFijacion = item.FechaFijacion,
                DolarizadoNoProductor = item.DolarizadoNoProductor,
                FechaHastaDolarizado = item.FechaHastaDolarizado,
                FechaUltimaAplicacion = item.FechaUltimaAplicacion,
                Fijacion = item.Fijacion,
                KgNoPesificable = item.KgNoPesificable,
                KgVencimientoPesificable = item.KgVencimientoPesificable,
                KgTotales = item.KgTotales,
                MaterialId = materiales.Where(x => x.Codigo == item.Material).FirstOrDefault() != null ?
                materiales.Where(x => x.Codigo == item.Material).FirstOrDefault().MaterialId : (int?)null,
                Unidad = item.Unidad,
                MonedaId = item.Moneda,
                Precio = item.Precio,
                NombreCorredor = item.NombreCorredor,
                NombreVendedor = item.NombreVendedor,
                Pase = item.Pase,
                Plus = item.Plus,
                Posicion = item.Posicion,
                KgTotalesPase = item.KgTotalesPase,
                Status = item.Status == "" ? "S" : item.Status,
                Cantidad = item.Cantidad,
                CantidadLiquidada = item.CantidadLiquidada,
                CantidadRecibida = item.CantidadRecibida,
                ConPrecio = item.ConPrecio,
                Cesion = item.Cesion
            }).ToList();
        }


        public DataSourceResult TraerTodoDatoPesificado(DataSourceRequest filtro, List<int> equipo)
        {
            List<string> cuits = new List<string>();

            if (filtro != null && filtro.Filter != null && filtro.Filter.Filters.Count() > 0)
            {
                foreach (var f in filtro.Filter.Filters)
                {
                    if (f.Filters != null && f.Filters.Count() > 0 && (f.Filters.First().Field == "CuitCorredor" || f.Filters.First().Field == "CuitVendedor"))
                    {
                        cuits.AddRange(f.Filters.Select(a => a.Value.ToString()).ToList());
                    }
                }

                FiltrarBooleano(filtro);
            }
            if (cuits.Count == 1 && filtro.Skip == 0)
            {
                var prov = repositorio.Listar<Proveedor, ProveedorCombo>(
                    x => new ProveedorCombo
                    {
                        Cuit = x.CUIT,
                        ProveedorId = x.ProveedorId
                    }, x => cuits.Contains(x.CUIT));
                var corredores = repositorio.Listar<CorredorProveedor>(x => cuits.Contains(x.Corredor.CUIT));
                foreach (var p in prov)
                {
                    p.Cuit = p.Cuit.Trim();
                    if (corredores.Any(x => x.ProveedorId == p.ProveedorId))
                    {
                        p.Cuit = "C" + p.Cuit.Remove(p.Cuit.Length - 1).Remove(0, 2);
                    }
                    else
                    {
                        p.Cuit = "00" + p.Cuit.Remove(p.Cuit.Length - 1).Remove(0, 2);
                    }
                }

                prov = prov.Distinct().ToList();
                var datosNuevo = pesificarAgent.ConsultarTodo(prov.Select(x => x.Cuit).Distinct().ToList());
                repositorio.RemoverTodos<ReportePesificado>(a => a.Pase == true);
                datosNuevo = BuscarPase(datosNuevo);
                try
                {
                    var cuitsVendedorCorredor = datosNuevo.Select(a => new { a.CuitVendedor, a.CuitCorredor }).Distinct().ToList();
                    if (cuitsVendedorCorredor != null && cuitsVendedorCorredor.Count > 0)
                    {
                        foreach (var item in cuitsVendedorCorredor)
                        {
                            repositorio.RemoverTodos<ReportePesificado>(a => a.CuitVendedor == item.CuitVendedor && a.CuitCorredor == item.CuitCorredor);
                        }
                        var datos = ConvertPesificarAgent(datosNuevo.Where(x => x.Anticipo != "X").ToList());
                        if (datos.Count > 0)
                        {
                            repositorio.AgregarTodos(datos);
                            repositorio.GuardarCambios();
                        }
                    }

                }
                catch (Exception e)
                {
                    logger.Error("Error actualizar ReportePesificado");
                    logger.Error(e);
                    throw;
                }

            }


            var result = repositorio.ObtenerConsultaEscalar(new TraerTodosReportePesificado(filtro, equipo));

            return result;
        }

        private static void FiltrarBooleano(DataSourceRequest filtro)
        {
            var dolarizado = "Dolarizado";
            var dolarizadoExpress = "DolarizadoExpress";
            var noProductor = "DolarizadoNoProductor";
            var ninguno = "NingunDolarizado";

            if (filtro.Filter.Filters.Where(x => x.Field == dolarizado || x.Field == dolarizadoExpress || x.Field == noProductor || x.Field == ninguno).ToList().Count > 1)
            {
                var filter = new List<Filter>();

                if (filtro.Filter.Filters.Any(x => x.Field == dolarizado))
                {
                    filter.Add(new Filter { Field = dolarizado, Operator = "eq", Value = true });

                }
                if (filtro.Filter.Filters.Any(x => x.Field == dolarizadoExpress))
                {
                    filter.Add(new Filter { Field = dolarizadoExpress, Operator = "eq", Value = true });
                }
                if (filtro.Filter.Filters.Any(x => x.Field == noProductor))
                {
                    filter.Add(new Filter { Field = noProductor, Operator = "eq", Value = true });
                }

                if (filtro.Filter.Filters.Any(x => x.Field == ninguno))
                {
                    filter.Add(new Filter { Field = ninguno, Operator = "eq", Value = true });
                }

                var filtroHijo = new Filter { Logic = "or", Filters = filter };
                var filtroPadre = new List<Filter>();
                foreach (var f in filtro.Filter.Filters)
                {
                    if (f.Field != dolarizado && f.Field != dolarizadoExpress && f.Field != noProductor && f.Field != ninguno)
                    {
                        filtroPadre.Add(f);
                    }
                }
                filtroPadre.Add(filtroHijo);
                filtro.Filter.Filters = filtroPadre;
            }
        }

        public DataSourceResult TraerTodoPrecioMoaPizarra(DataSourceRequest request)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodoPrecioMoaPizarra(request));
        }

        public ResultReportePagosDiferidos ObtenerDatosReportePagosDiferidos(DateTime desde, DateTime hasta)
        {
            var hoy = DateTime.Now.Date;

            decimal tna = decimal.Parse(ConfigurationManager.AppSettings["TNAReportePagosDiferidos"]) / 100;
            var dias = new List<DateTime>();
            var desdeDias = new DateTime(hoy.Year, hoy.Month, 1);
            var hastaDias = new DateTime(hoy.Year, hoy.Month, 1).AddMonths(1).AddDays(-1);
            while (desdeDias <= hastaDias)
            {
                dias.Add(desdeDias);
                desdeDias = desdeDias.AddDays(1);
            }
            desdeDias = new DateTime(hoy.Year, hoy.Month, 1);


            var cotizaciones = new Dictionary<DateTime, decimal>();
            var dia = desde;
            while (dia <= hasta)
            {
                cotizaciones.Add(dia, tipoDeCambio.TraerTipoDeCambio(dia));
                dia = dia.AddDays(1);
            }
            List<ReportePagosDiferidos> datos = repositorio.Listar<Negocio, ReportePagosDiferidos>(x => new ReportePagosDiferidos
            {
                ContratoSAP = x.ContratoSAP,
                Vendedor = x.Proveedor.RazonSocial,
                VendedorCUIT = x.Proveedor.CUIT,
                VendedorID = x.ProveedorId,
                Corredor = x.Corredor == null ? "" : x.Corredor.RazonSocial,
                CorredorCUIT = x.Corredor == null ? "" : x.Corredor.CUIT,
                CorredorId = x.CorredorId,
                Tn = x.Cantidad / 1000,
                TNA = tna,
                Precio = x.Precio,
                Plazo = x.DiasPesificado.Value,
                Toma = x.FechaOperacion,

            },
            x => (x.TipoNegocioId == 2 || x.TipoNegocioId == 3)
            && (x.EstadoId == (int)EnumEstadoContrato.Confirmado || x.EstadoId == (int)EnumEstadoContrato.Finalizado)
            && x.PagoDiferido == true && x.DiasPesificado > 0
            && x.FechaOperacion >= desde && x.FechaOperacion <= hasta
            );
            ////para las pruebas
            //datos = new List<ReportePagosDiferidos>();
            //datos.Add(new ReportePagosDiferidos { ContratoSAP = "1", Tn = 5000, PrecioUSD = 230, Precio = 230 * 94, Plazo = 33, Toma = desde, TNA = tna  });
            //datos.Add(new ReportePagosDiferidos { ContratoSAP = "6", Tn = 200, PrecioUSD = 270, Precio = 270 * 94, Plazo = 80, Toma = hasta, TNA = tna  });

            foreach (var x in datos)
            {
                x.TipoCambio = cotizaciones.Where(a => a.Key == x.Toma).First().Value;
                x.PrecioUSD = x.Precio / x.TipoCambio;
                x.Capital = x.Precio * Convert.ToDecimal(x.Tn);

                decimal Number1 = (tna * x.Plazo / 365m + 1);
                decimal Number2 = 365m / x.Plazo;
                decimal resultado = Convert.ToDecimal(Math.Pow(Convert.ToDouble(Number1), Convert.ToDouble(Number2)) - 1);
                x.TEA = resultado;
                x.Vencimiento = x.Toma.AddDays(x.Plazo);
                x.Estado = hoy >= x.Vencimiento ? "Cancelado" : hoy >= x.Toma ? "Vigente" : "";
                x.AlVencimiento = x.Estado == "Vigente" ? Convert.ToInt32((x.Vencimiento.Date - hoy).TotalDays) : 0;
                x.InteresesTotales = x.Capital * tna / 365 * x.Plazo;
                x.CapitalMasIntereses = x.Capital + x.InteresesTotales;
                x.InteresesPorDia = x.InteresesTotales / x.Plazo;
                x.AcumuladoMesAnterior = x.Vencimiento > hasta && x.Toma <= desde ? x.InteresesPorDia * (Convert.ToInt32((desdeDias - x.Toma).TotalDays) + 1) : 0;
                x.M2MMes = ObtenerM2MMes(x, desdeDias, hastaDias);
                x.DevengadoMes = ObtenerDebengadoMes(x, desdeDias, hastaDias);
                x.DevengadoMes = ObtenerDebengadoMes(x, desdeDias, hastaDias);
                x.Dias = ObtenerDias(x, dias);

            }
            return new ResultReportePagosDiferidos { Contratos = datos, Desde = desdeDias, Hasta = hastaDias };


        }

        public void EnviarMailReportePagosDiferidos(byte[] datos, DateTime desde, DateTime hoy)
        {

            string cuerpoMail = "";

            if (datos != null)
            {
                cuerpoMail = $"Se envian adjuntos los contratos con pago diferido en el rango de fechas {desde.ToString("dd-MM-yyyy")} - {hoy.ToString("dd-MM-yyyy")} ";
            }
            else
            {
                cuerpoMail = $"No hay contratos con pago diferido en el rango de fechas {desde.ToString("dd-MM-yyyy")} - {hoy.ToString("dd-MM-yyyy")} ";
            }

            mailManager.EnviarMail(repositorio.Listar<Comercial, string>(x => x.IdActiveDirectory, x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VisualizarReportePagoDiferido))),
                                                               "Diferidos " + hoy.Day + "/" + hoy.Month,
                                                                   string.Empty,
                                                                       null,
                                                                           AlternateView.CreateAlternateViewFromString(cuerpoMail, null, "text/html"),
                                                                               datos,
                                                                                   "Diferidos.xlsx");

        }

        private decimal ObtenerM2MMes(ReportePagosDiferidos x, DateTime desde, DateTime hasta)
        {
            decimal result;

            if (x.Toma <= desde && x.Vencimiento > hasta && desde <= hasta)
            {
                result = x.InteresesPorDia * Convert.ToDecimal((hasta - desde).TotalDays);
            }
            else
            {
                if (x.Toma > desde && x.Vencimiento > hasta && x.Toma <= hasta && x.Toma >= desde && desde <= hasta)
                {
                    result = x.InteresesPorDia * (Convert.ToDecimal((hasta - x.Toma).TotalDays) + 1);
                }
                else
                {
                    result = 0;
                }
            }

            return result;
        }
        private decimal ObtenerDebengadoMes(ReportePagosDiferidos x, DateTime desde, DateTime hasta)
        {
            decimal result;

            if (x.Toma <= desde && x.Vencimiento > hasta)
            {
                result = x.InteresesPorDia * Convert.ToDecimal((hasta - desde).TotalDays);
            }
            else
            {
                if (x.Toma <= desde && x.Vencimiento <= hasta && x.Vencimiento > desde)
                {
                    result = x.InteresesPorDia * (Convert.ToDecimal((x.Vencimiento - desde).TotalDays) - 1);
                }
                else
                {
                    if (x.Toma > desde && x.Vencimiento > hasta && x.Toma <= hasta)
                    {
                        result = x.InteresesPorDia * (Convert.ToDecimal((hasta - x.Toma).TotalDays) + 1);
                    }
                    else
                    {
                        if (x.Toma > desde && x.Vencimiento <= hasta)
                        {
                            result = x.InteresesPorDia * Convert.ToDecimal((x.Vencimiento - x.Toma).TotalDays);
                        }
                        else
                        {
                            result = 0;
                        }
                    }
                }
            }

            return result;
        }
        private List<ReportePagosDiferidosDia> ObtenerDias(ReportePagosDiferidos x, List<DateTime> dias)
        {
            List<ReportePagosDiferidosDia> result = new List<ReportePagosDiferidosDia>();
            foreach (var dia in dias)
            {
                var item = new ReportePagosDiferidosDia
                {
                    Dia = dia,
                    Importe = (dia >= x.Toma && dia < x.Vencimiento ? x.Capital : 0) + (dia >= x.Toma && dia < x.Vencimiento ? x.InteresesPorDia * (Convert.ToDecimal((dia - x.Toma).TotalDays) + 1) : 0)
                };
                result.Add(item);
            }


            return result;
        }

        public DataSourceResult BuscarDatosNegocioPesificacion(DataSourceRequest filtro, List<int> equipo)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerTodosPesificacion(filtro, equipo));

        }

        public Resultado ConfigurarExcedente(int id, bool excedente, int comercialId)
        {
            try
            {
                var resultado = new Resultado();
                var reportePesificado = repositorio.Obtener<ReportePesificado>(id);
                var n = repositorio.Obtener<Negocio>(x => x.Id == reportePesificado.NegocioId);

                var c = comercialId;

                if (n == null)
                {
                    resultado.Error("ConfigurarExcedente", $"No se puede realizar la acción. El contrato {reportePesificado.Contrato} no se encuentra registrado en DA");
                    return resultado;
                }
                var negocioPesificado = repositorio.ObtenerMayor<NegocioPesificacion, int>(x => x.NegocioId == n.Id, x => x.Id);
                if (negocioPesificado != null)
                {
                    negocioPesificado.FechaExcepcion = DateTime.Now;
                    negocioPesificado.Excepcion = excedente;
                    negocioPesificado.ComercialId = c;
                }
                else
                {
                    repositorio.Agregar(CrearNuevoNegocioPesificado(null, c, null, excedente, n.Id));
                }

                repositorio.GuardarCambios();
                return resultado;
            }
            catch (Exception e)
            {
                logger.Error("Error actualizar excedente");
                logger.Error(e);
                throw;
            }
        }

        public void EnviarMail(List<int> ids, DateTime fechaInstruccion, int comercialId)
        {
            var pesificados = repositorio.Listar<ReportePesificado, ReportePesificadoDto>(x => new ReportePesificadoDto
            {
                Id = x.Id,
                Cantidad = x.Cantidad,
                CuitVendedor = x.CuitVendedor,
                CuitCorredor = x.CuitCorredor,
                RazonSocialProveedor = x.NombreVendedor,
                RazonSocialCorredor = x.NombreCorredor,
                Contrato = x.Contrato,
                Fijacion = x.Fijacion,
                NegocioId = x.NegocioId,
                EsOperacionDirecta = string.IsNullOrEmpty(x.CuitCorredor) ? true : false,
                Clasificacion = x.Clasificacion,
                KgVencimientoPesificable = x.KgVencimientoPesificable
            }, x => ids.Contains(x.Id));
            EnviarMailPesificacionVencida(pesificados, fechaInstruccion, true);
            EnviarMailPesificacionVencida(pesificados, fechaInstruccion, false);
            GrabarFechaDeInstruccion(pesificados, fechaInstruccion, comercialId);

        }

        private void EnviarMailPesificacionVencida(List<ReportePesificadoDto> pesificado, DateTime fechaInstruccion, bool tieneCorredor)
        {
            var subject = "";
            List<string> fromEmail = new List<string>();
            List<string> mailAdmin = new List<string>();
            var hoy = DateTime.Today;
            fromEmail.Add(ConfigurationManager.AppSettings["CredentialUserNamePesificados"]);
            if (ConfigurationManager.AppSettings["AmbientePruebas"] == "1")
            {
                subject += "Mail Pruebas - PESIFICACION DE CONTRATOS - AVISO IMPORTANTE!";
            }
            else
            {
                subject += "PESIFICACION DE CONTRATOS - AVISO IMPORTANTE!";
            }
            mailAdmin.Add(ConfigurationManager.AppSettings["CredentialUserNamePesificados"]);
            mailAdmin.Add("Joaquin.Delfederico@molinosagro.com.ar");

            var comerciales = repositorio.Listar<Comercial>(x => x.RolesAsociados.Any(y => y.PermisosAsociados.Any(z => z.Permiso == PermisosDataAgro.VerCorredorComercial)));

            var path = httpContextManager.ObtenerPathLogoMail();
            var vendedor = repositorio.Listar<MailProveedor>();
            var copia = new List<string>();
            try
            {
                if (tieneCorredor)
                {

                    if (pesificado.Any(x => !string.IsNullOrEmpty(x.CuitCorredor)))
                    {
                        var corredor = pesificado.Where(x => !string.IsNullOrEmpty(x.CuitCorredor)).Distinct().GroupBy(x => x.RazonSocialCorredor);
                        foreach (var item in corredor)
                        {
                            copia = new List<string>();
                            var pesi = new ReportePesificadoDto
                            {
                                Corredor = item.Key,
                                Clasificacion = pesificado.Where(x => item.Key == x.RazonSocialCorredor).FirstOrDefault().Clasificacion,
                                EsOperacionDirecta = pesificado.Where(x => item.Key == x.RazonSocialCorredor).FirstOrDefault().EsOperacionDirecta,
                                AgrupracionPesificados =
                                item.Select(x => new AgrupacionPesificado { Proveedor = x.RazonSocialProveedor, 
                                    CantidadAgrupada = (x.FechaHastaDolarizado != null && x.FechaHastaDolarizado > hoy) ? (x.KgTotales ?? 0) : (x.KgVencimientoPesificable ?? 0), 
                                    Contrato = x.Contrato, 
                                    Fijacion = x.Fijacion }).ToList()
                            };
                            logger.Debug("fecha hasta dolarizado: " + item.First().FechaHastaDolarizado.Value.ToString("dd-MM-yyyy hh:mm") + "hoy: " + hoy.ToString("dd-MM-yyyy hh:mm"));
                            var mails = DevolverMailComercialDeNegocio(pesi);
                            mails = new List<string>() { "bmelgarejo@baufest.com" };

                            if (mails != null)
                            {
                                var view = CuerpoMailPesificadoVencidoVendedor(path, pesi, fechaInstruccion, true);
                                copia.AddRange(vendedor.Where(x => x.ProveedorId != null && x.Proveedor.CUIT == item.First().CuitCorredor).Select(x => x.Pesificado));
                                //if (!string.IsNullOrEmpty(mailAdmin)) { mails.Add(mailAdmin); }
                                if (mailAdmin.Count > 0)
                                {
                                    mails.AddRange(mailAdmin);
                                }
                                mailManager.EnviarMail(mails, subject, "", copia, view, null, null, fromEmail);
                            }
                        }
                    }
                }
                else
                {
                    if (pesificado.Any(x => string.IsNullOrEmpty(x.CuitCorredor)))
                    {
                        var proveedor = pesificado.Where(x => string.IsNullOrEmpty(x.CuitCorredor)).Distinct().GroupBy(x => x.RazonSocialProveedor);
                        var agrupacion = new List<ReportePesificadoDto>();
                        foreach (var item in proveedor)
                        {
                            copia = new List<string>();
                            var pesi = new ReportePesificadoDto
                            {
                                RazonSocialProveedor = item.Key,
                                Clasificacion = pesificado.Where(x => item.Key == x.RazonSocialProveedor).FirstOrDefault().Clasificacion,
                                EsOperacionDirecta = pesificado.Where(x => item.Key == x.RazonSocialProveedor).FirstOrDefault().EsOperacionDirecta,
                                AgrupracionPesificados =
                                item.Select(x => new AgrupacionPesificado { 
                                    Proveedor = x.RazonSocialProveedor, 
                                    CantidadAgrupada = (x.FechaHastaDolarizado != null && x.FechaHastaDolarizado > hoy) ? (x.KgTotales ?? 0) : (x.KgVencimientoPesificable ?? 0), 
                                    Contrato = x.Contrato, Fijacion = x.Fijacion }).ToList()
                            };
                            logger.Debug("fecha hasta dolarizado: " + item.First().FechaHastaDolarizado.Value.ToString("dd-MM-yyyy hh:mm") + "hoy: " + hoy.ToString("dd-MM-yyyy hh:mm"));

                            var mails = DevolverMailComercialDeNegocio(pesi);
                            mails = new List<string>() { "bmelgarejo@baufest.com" };
                            if (mails != null)
                            {
                                copia.AddRange(vendedor.Where(x => x.ProveedorId != null && x.Proveedor.CUIT == item.First().CuitVendedor).Select(x => x.Pesificado));
                                var view = CuerpoMailPesificadoVencidoVendedor(path, pesi, fechaInstruccion, false);
                                //if (!string.IsNullOrEmpty(mailAdmin)) { mails.Add(mailAdmin); }
                                if (mailAdmin.Count > 0)
                                {
                                    mails.AddRange(mailAdmin);
                                }
                                mailManager.EnviarMail(mails, subject, "", (copia.Count > 0 ? copia : null), view, null, null, fromEmail);
                            }

                        }
                    }
                }
            }
            catch (Exception e) { logger.Error(e); }
        }
        private List<string> DevolverMailComercialDeNegocio(ReportePesificadoDto reporte)
        {
            try
            {
                var contratos = reporte.AgrupracionPesificados.Select(y => y.Contrato).ToList();
                var negocio = repositorio.Listar<Negocio>(x => contratos.Contains(x.ContratoSAP));
                var listaDirectory = new List<string>();
                if (negocio != null && negocio.Count > 0)
                {
                    var todoComercial = repositorio.Listar<Comercial>();
                    foreach (var n in negocio)
                    {
                        var comercial = todoComercial.Where(x => x.ComercialId == n.ComercialId).Select(x => x.IdActiveDirectory).FirstOrDefault();
                        var comerecialCreador = todoComercial.Where(x => x.ComercialId == n.ComercialCreadorId).Select(x => x.IdActiveDirectory).FirstOrDefault();
                        if (!string.IsNullOrEmpty(comercial))
                        {
                            listaDirectory.Add(comercial);
                        }
                        if (!string.IsNullOrEmpty(comerecialCreador))
                        {
                            listaDirectory.Add(comerecialCreador);
                        }
                        listaDirectory.Distinct();
                    }

                    return DevolverMailDeActiveDirectoryId(listaDirectory);
                }
                return null;
            }
            catch (Exception e) { logger.Error(e); return null; }
        }

        private void GrabarFechaDeInstruccion(List<ReportePesificadoDto> pesificados, DateTime fechaInstruccion, int comercialId)
        {
            try
            {
                var ids = pesificados.Select(y => y.NegocioId).ToList();
                var negocio = repositorio.Listar<Negocio>(x => ids.Contains(x.Id));
                var negociosPesificadosNuevo = new List<NegocioPesificacion>();
                var negociosPesificados = repositorio.Listar<NegocioPesificacion>();

                if (negocio != null && negocio.Count() > 0)
                {
                    foreach (var n in negocio)
                    {

                        if (negociosPesificados != null && negociosPesificados.Count() > 0 && negociosPesificados.Any(x => x.Negocio.Id.Equals(n.Id)))
                        {
                            var pesificado = negociosPesificados.Where(x => x.Negocio.Id.Equals(n.Id)).LastOrDefault();

                            if (pesificado.FechaInstruccion.HasValue)
                            {
                                repositorio.Agregar(CrearNuevoNegocioPesificado(fechaInstruccion, comercialId, pesificado.FechaExcepcion, pesificado.Excepcion, n.Id));
                            }
                            else
                            {
                                pesificado.FechaEnvio = DateTime.Now;
                                pesificado.FechaInstruccion = fechaInstruccion;
                                pesificado.ComercialId = comercialId;
                            }

                        }
                        else
                        {
                            repositorio.Agregar(CrearNuevoNegocioPesificado(fechaInstruccion, comercialId, null, null, n.Id));
                        }
                    }
                }
                repositorio.GuardarCambios();
            }
            catch (Exception e) { logger.Error(e); }
        }
        private NegocioPesificacion CrearNuevoNegocioPesificado(DateTime? fechaInstruccion, int? comercialId, DateTime? fechaExcepcion, bool? excepcion, int id)
        {
            return new NegocioPesificacion()
            {
                FechaEnvio = fechaInstruccion != null ? DateTime.Now : (DateTime?)null,
                FechaInstruccion = fechaInstruccion,
                ComercialId = comercialId,
                Excepcion = excepcion,
                FechaExcepcion = fechaExcepcion != null ? fechaExcepcion : DateTime.Now,
                NegocioId = id
            };
        }

        private List<string> DevolverMailDeActiveDirectoryId(List<string> active)
        {
            var lista = new List<string>();
            logger.Debug("Active: " + active.ToJson());
            foreach (var item in active.Distinct())
            {
                try
                {
                    var emailComerciales = mailManager.GetEmailUserActiveDirectory(item);
                    logger.Debug("Mail encontrado para " + emailComerciales + "  " + item);
                    if (!String.IsNullOrEmpty(emailComerciales))
                    {
                        lista.Add(emailComerciales);
                    }
                }
                catch (Exception e) { logger.Error(e); }
            }

            return lista;
        }

        private AlternateView CuerpoMailPesificadoVencidoVendedor(String filePath, ReportePesificadoDto corredor, DateTime instruccion, bool tieneCorredor)
        {
            //var emailComercial = mailManager.GetEmailUserActiveDirectory(comercial.IdActiveDirectory);
            LinkedResource res = new LinkedResource(filePath);
            res.ContentId = Guid.NewGuid().ToString();
            string th;
            if (ConfigurationManager.AppSettings["AmbientePruebas"] != "1")
            {
                th = "<th style=\"border: 0px solid #AAAAAA; padding: 3px 2px;\">";
            }
            else
            {
                th = "<th style=\"border: 2px solid white; color: white; background-color: #400179; padding: 5px 0; width: 175px;\">";
            }
            var p = "<p>";
            var thHead = "style=\"font-size: 15px; font-weight: bold;color: #FFFFFF; text-align: center;border-left: 0px solid #D0E4F5; " +
                "border: 0px solid #AAAAAA;padding: 3px 2px;\"";
            var td = "style=\"border: 0px solid #AAAAAA;  padding: 3px 2px;\"";
            var linea = 0;
            string htmlBody = "";
            var cabecera = "";
            cabecera += $"{p}Estimado/s,</p>";
            cabecera += $"{p}Por la presente les notificamos que, en atención a que en su carácter de " + (corredor.EsOperacionDirecta && corredor.Clasificacion != "PRODUCTOR" ? "vendedor" : "corredor") + " en el/los boleto/s de referencia no han emitido" +
                $" a la fecha, la correspondiente liquidación de granos a pesar de haber entregado mercadería, Molinos Agro S.A.en calidad de comprador y en caso " +
                $"de continuar esta situación hasta el {instruccion.ToString("dd-MM-yyyy")} se considerará a los efectos de la liquidación pendiente, que el tipo de cambio a utilizar será " +
                $"el del cierre del día {FechaALetras(instruccion)} en las condiciones pactadas.</p>";
            cabecera += $"{p}A tales efectos, les solicitamos el inmediato informe de la/s liquidación/es correspondiente en nuestra web, en pesos argentinos al " +
                $"tipo de cambio que corresponda, cumpliendo el comprador con su obligación de pago en los términos que indica el correspondiente boleto.</p>";
            cabecera += "<br/>";

            if (tieneCorredor)
            {
                htmlBody += cabecera;
                htmlBody += "<table style=\"border: 1px solid #1C6EA4;background-color: #EEEEEE; width:70%; text-align: left;border-collapse:collapse;\">";
                htmlBody += "<thead style=\" font-size: 13px;background: #1C6EA4; border-bottom: 0px solid #444444;\">";
                htmlBody += "<tr >";
                htmlBody += $"<th {thHead}> Corredor: {corredor.Corredor} </th>";
                htmlBody += $"<th {thHead}>Contrato </th>";
                htmlBody += $"<th {thHead}>Fijación </th>";
                htmlBody += $"<th {thHead}>Total</th>";
                htmlBody += "</tr>";
                htmlBody += "</thead>";
                htmlBody += "<tbody>";
                foreach (var item in corredor.AgrupracionPesificados)
                {
                    htmlBody += "<tr style=\"text-align: center\">";
                    htmlBody += $"<td {td}> {item.Proveedor} </td>";
                    htmlBody += $"<td {td}> { Split(item.Contrato.TrimStart('0')) } </td>";
                    htmlBody += $"<td {td}> {(!string.IsNullOrEmpty(item.Fijacion) ? Split(item.Contrato.TrimStart('0')) + "-" + item.Fijacion.Substring((item.Fijacion.Length - 2), 2) : "") } </td>";
                    htmlBody += $"<td {td}> {Split(item.CantidadAgrupada.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))} </td>";
                    htmlBody += "</tr>";
                }
                htmlBody += $"<tr style=\"background: #1c6ea4; color:white; text-align: center\">";
                htmlBody += $"<td><strong>Total General</strong></td>";
                htmlBody += $"<td {td}></td>";
                htmlBody += $"<td {td}></td>";
                htmlBody += $"<td {td}> {Split(corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))}  </td>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table>";
            }         
            if((corredor.Clasificacion == "ACOPIADOR" || corredor.Clasificacion == "OTROS") && corredor.EsOperacionDirecta){
                htmlBody += cabecera;
                htmlBody += "<table style=\"border: 1px solid #1C6EA4;background-color: #EEEEEE; width:70%; text-align: left;border-collapse:collapse;\">";
                htmlBody += "<thead style=\" font-size: 13px;background: #1C6EA4; border-bottom: 0px solid #444444;\">";
                htmlBody += "<tr >";
                htmlBody += $"<th {thHead}>Vendedor </th>";
                htmlBody += $"<th {thHead}>Contrato </th>";
                htmlBody += $"<th {thHead}>Fijación </th>";
                htmlBody += $"<th {thHead}>Toneladas</th>";
                htmlBody += "</tr>";
                htmlBody += "</thead>";
                htmlBody += "<tbody>";
                foreach (var item in corredor.AgrupracionPesificados)
                {
                    htmlBody += "<tr style=\"text-align: center\">";
                    htmlBody += $"<td {td}> { item.Proveedor} </td>";
                    htmlBody += $"<td {td}> { Split(item.Contrato.TrimStart('0')) } </td>";
                    htmlBody += $"<td {td}> {(!string.IsNullOrEmpty(item.Fijacion) ? Split(item.Contrato.TrimStart('0')) + "-" + item.Fijacion.Substring((item.Fijacion.Length - 2), 2) : "") } </td>";
                    //htmlBody += $"<td {td}> {Math.Round((item.CantidadAgrupada / 1000), 3).ToString("00.000", CultureInfo.CreateSpecificCulture("da-DK"))} </td>"; // en toneladas
                    htmlBody += $"<td {td}> {Split(item.CantidadAgrupada.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))} </td>";
                    htmlBody += "</tr>";
                }
                htmlBody += $"<tr style=\"background: #1c6ea4; color:white; text-align: center\">";
                htmlBody += $"<td><strong>Total General</strong></td>";
                htmlBody += $"<td {td}></td>";
                htmlBody += $"<td {td}></td>";
                //htmlBody += $"<td {td}> {Math.Round((corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada))/1000, 3).ToString("00.000", CultureInfo.CreateSpecificCulture("da-DK"))}  </td>"; //toneladas
                htmlBody += $"<td {td}> {Split(corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))}  </td>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table>";
                htmlBody += "<br/>";
                htmlBody += $"Asimismo, de no haber generado la pesificación antes del  {instruccion.ToString("dd-MM-yyyy")}, no será necesario que ingresen a la página WEB para tomar el TC, tomándose como " +
                      $"pesificación efectiva esta comunicación con el TC del día {FechaALetras(instruccion)}.";
                htmlBody += $"{p}Saludos Cordiales</p>" +
                    $"{p}Molinos Agro S.A.</p>  <br /> <br />" +
                    @"<img src='cid:" + res.ContentId + @"'/>" +
                    $"{p} www.molinosagro.com.ar</p>";
                AlternateView alternateViewT = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                alternateViewT.LinkedResources.Add(res);
                return alternateViewT;
            }
            if (corredor.Clasificacion == "PRODUCTOR" && corredor.EsOperacionDirecta)
            {
                htmlBody = $"{p}Estimado/s,</p>";
                htmlBody += $"{p} Informamos que los siguientes contratos serán pesificados por MOA con TC de cierre del día {(instruccion.ToString("dd.MM.yyyy"))}:</p>";
                htmlBody += "<table style=\"border: 1px solid #1C6EA4;background-color: #EEEEEE; width:70%; text-align: left;border-collapse:collapse;\">";
                htmlBody += "<thead style=\" font-size: 13px;background: #1C6EA4; border-bottom: 0px solid #444444;\">";
                htmlBody += "<tr >";
                htmlBody += $"<th {thHead}>Productor </th>";
                htmlBody += $"<th {thHead}>Contrato </th>";
                htmlBody += $"<th {thHead}>Fijación </th>";
                htmlBody += $"<th {thHead}>Toneladas</th>";
                htmlBody += "</tr>";
                htmlBody += "</thead>";
                htmlBody += "<tbody>";
                foreach (var item in corredor.AgrupracionPesificados)
                {
                    htmlBody += "<tr style=\"text-align: center\">";
                    htmlBody += $"<td {td}> { item.Proveedor} </td>";
                    htmlBody += $"<td {td}> { Split(item.Contrato.TrimStart('0')) } </td>";
                    htmlBody += $"<td {td}> {(!string.IsNullOrEmpty(item.Fijacion) ? Split(item.Contrato.TrimStart('0')) + "-" + item.Fijacion.Substring((item.Fijacion.Length - 2), 2) : "") } </td>";
                    //htmlBody += $"<td {td}> {Math.Round((item.CantidadAgrupada / 1000), 3).ToString("00.000", CultureInfo.CreateSpecificCulture("da-DK"))} </td>"; // en toneladas
                    htmlBody += $"<td {td}> {Split(item.CantidadAgrupada.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))} </td>";
                    htmlBody += "</tr>";
                }
                htmlBody += $"<tr style=\"background: #1c6ea4; color:white; text-align: center\">";
                htmlBody += $"<td><strong>Total General</strong></td>";
                htmlBody += $"<td {td}></td>";
                htmlBody += $"<td {td}></td>";
                //htmlBody += $"<td {td}> {Math.Round((corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada))/1000, 3).ToString("00.000", CultureInfo.CreateSpecificCulture("da-DK"))}  </td>"; //toneladas
                htmlBody += $"<td {td}> {Split(corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))}  </td>";
                htmlBody += "</tr>";
                htmlBody += "</tbody>";
                htmlBody += "</table>";
                htmlBody += "<br/>";
                htmlBody += $"Asimismo, de no haber generado la pesificación antes del  {instruccion.ToString("dd-MM-yyyy")}, no será necesario que ingresen a la página WEB para tomar el TC, tomándose como " +
                      $"pesificación efectiva esta comunicación con el TC del día {FechaALetras(instruccion)}.";
                htmlBody += $"{p}Saludos Cordiales</p>" +
                    $"{p}Molinos Agro S.A.</p>  <br /> <br />" +
                    @"<img src='cid:" + res.ContentId + @"'/>" +
                    $"{p} www.molinosagro.com.ar</p>";
                AlternateView alternateViewT = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                alternateViewT.LinkedResources.Add(res);
                return alternateViewT;
            }
            //else
            //{
            //    htmlBody += "<table style=\"border: 1px solid #1C6EA4;background-color: #EEEEEE; width:70%; text-align: left;border-collapse:collapse;\">";
            //    htmlBody += "<thead style=\" font-size: 13px;background: #1C6EA4; border-bottom: 0px solid #444444;\">";
            //    htmlBody += "<tr >";
            //    htmlBody += $"<th {thHead}>Vendedor: {corredor.RazonSocialProveedor} </th>";
            //    htmlBody += $"<th {thHead}>Fijación </th>";
            //    htmlBody += $"<th {thHead}>Total</th>";
            //    htmlBody += "</tr>";
            //    htmlBody += "</thead>";
            //    htmlBody += "<tbody>";
            //    foreach (var item in corredor.AgrupracionPesificados)
            //    {
            //        htmlBody += "<tr style=\"text-align: center\">";
            //        htmlBody += $"<td {td}>  { Split(item.Contrato.TrimStart('0')) } </td>";
            //        htmlBody += $"<td {td}> {(!string.IsNullOrEmpty(item.Fijacion) ? Split(item.Contrato.TrimStart('0')) + "-" + item.Fijacion.Substring((item.Fijacion.Length - 2), 2) : "") } </td>";
            //        htmlBody += $"<td {td}> {Split(item.CantidadAgrupada.ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))} </td>";
            //        htmlBody += "</tr>";
            //    }
            //    htmlBody += $"<tr style=\"background: #1c6ea4; color:white; text-align: center\">";
            //    htmlBody += $"<td><strong>Total General</strong></td>";
            //    htmlBody += $"<td {td}></td>";
            //    htmlBody += $"<td {td}> {Split(corredor.AgrupracionPesificados.Sum(x => x.CantidadAgrupada).ToString("N0", CultureInfo.CreateSpecificCulture("es-AR")))} </td>";
            //    htmlBody += "</tr>";
            //    htmlBody += "</tbody>";
            //    htmlBody += "</table>";

            //}
            htmlBody += "<br/>";
            htmlBody += $"Asimismo, de no haber generado la pesificación antes del  {instruccion.ToString("dd-MM-yyyy")}, no será necesario que ingresen a la página WEB para tomar el TC, tomándose como " +
                  $"pesificación efectiva esta comunicación con el TC del día {FechaALetras(instruccion)}.";
            htmlBody += $"{p}Saludos Cordiales</p>" +
                $"{p}Molinos Agro S.A.</p>  <br /> <br />" +
                @"<img src='cid:" + res.ContentId + @"'/>" +
                $"{p} www.molinosagro.com.ar</p>";
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
        private string FechaALetras(DateTime fecha)
        {
            return $"{fecha.Day} de {fecha.ToString("MMMM")} de {fecha.Year}";
        }
    }
}
