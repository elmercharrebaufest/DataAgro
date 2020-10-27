using Autofac.Extras.NLog;
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
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Reflection;

namespace Molinos.DataAgro.Business.Managers
{
    public class ReportesManager : IReportesManager
    {
        private readonly IRepositorio repositorio;
        private readonly IComercialManager oComercial;
        private readonly ITipoDeCambioAgent tipoDeCambio;
        private ILogger logger;

        public ReportesManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial, ITipoDeCambioAgent tipoDeCambio)
        {
            this.logger = logger;
            this.repositorio = repositorio;
            this.oComercial = oComercial;
            this.tipoDeCambio = tipoDeCambio;
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
            var trigoCamaraToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, 3, centroId)) : new ToneladasGranoTipoDto();
            trigoCamaraToneladas.Material = "Trigo Cámara";
            trigoCamaraToneladas.MaterialId = 2;
            var trigoCalidadToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, 2, centroId)) : new ToneladasGranoTipoDto();
            trigoCalidadToneladas.Material = "Trigo Calidad";
            trigoCalidadToneladas.MaterialId = 2;
            var girasolToneladas = materialId.Contains(4) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(4, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            girasolToneladas.Material = "Girasol";
            girasolToneladas.MaterialId = 4;
            var girasolAltoToneladas = materialId.Contains(5) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, null, centroId)) : new ToneladasGranoTipoDto();
            girasolAltoToneladas.Material = "Girasol Alto Oleico";
            girasolAltoToneladas.MaterialId = 5;
            var trigoGradoToneladas = materialId.Contains(2) ? repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, 7, centroId)) : new ToneladasGranoTipoDto();
            trigoGradoToneladas.Material = "Trigo Grado 2";
            trigoGradoToneladas.MaterialId = 2;
            return new List<ToneladasGranoTipoDto>() { sojaToneladas, maizToneladas, trigoCamaraToneladas, trigoCalidadToneladas, trigoGradoToneladas, girasolToneladas, girasolAltoToneladas };

        }
        public ReporteSojaSustDto TraerToneladasSojaSust(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerToneladasSojaSustentable(fechaDesde, fechaHasta, centroId));
        }
        public List<PosicionComprasDto> TraerPosicionCompras(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0)
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
                Precio = x.Precio,
                Pizarra = x.Pizarra,
                TipoNegocioId = x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                MonedaId = x.MonedaId,
                OcultarEnTablero = x.OcultarEnTablero,
                Estado = x.EstadoId,
                MaterialId = x.MaterialId,
                StandardCalidadId = x.StandardDeCalidadId,
                DestinoId = x.DestinoId,
                EsFason = x is Contrato ? (x as Contrato).EsFason : null,
                ContratoAcuerdoId = x is Contrato ? (x as Contrato).ContratoAcuerdoId : null,
                TrigoEspecial = x.TrigoEspecial,
                Posicion = x.Posicion,
                ContratoSAP = x.ContratoSAP
            },
               x => x.OcultarEnTablero == false
               &&
               ((x is Contrato && DbFunctions.TruncateTime((x as Contrato).FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime((x as Contrato).FechaOperacion) <= fechaManana) ||
               (x is FijacionDePrecioContrato && DbFunctions.TruncateTime((x as FijacionDePrecioContrato).FechaOperacion) >= fechaHoy && DbFunctions.TruncateTime((x as FijacionDePrecioContrato).FechaOperacion) <= fechaManana) ||
               (!(x is Contrato) && !(x is FijacionDePrecioContrato) && DbFunctions.TruncateTime(x.Fecha) >= fechaHoy && DbFunctions.TruncateTime(x.Fecha) <= fechaManana))
               && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
               && (centroId == 0 || x.DestinoId == centroId)
               && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2 || x.TipoNegocioId == 3 || x.TipoNegocioId == 4 || x.TipoNegocioId == 6)
               && (x.TipoAgenteCompraId == null)
               && ((x is ContratoAcuerdo && (x as ContratoAcuerdo).TipoAgenteCompraId == null) || !(x is ContratoAcuerdo))
               );
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
            var kilosPosicionSoja = materialId.Contains(3) ? TraerPosicionMaterial(3, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionSoja = new PosicionComprasDto
            {
                Material = "Soja",
                MaterialId = 3,
                PosicionKilos = kilosPosicionSoja,
                Total = kilosPosicionSoja.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionMaiz = materialId.Contains(1) ? TraerPosicionMaterial(1, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionMaiz = new PosicionComprasDto
            {
                Material = "Maiz",
                MaterialId = 1,
                PosicionKilos = kilosPosicionMaiz,
                Total = kilosPosicionMaiz.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoCamara = materialId.Contains(2) ? TraerPosicionMaterial(2, fechaDesde, fechaHasta, 1, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionTrigoCamara = new PosicionComprasDto
            {
                Material = "Trigo Cámara",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCamara,
                Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoCalidad = materialId.Contains(2) ? TraerPosicionMaterial(2, fechaDesde, fechaHasta, 2, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionTrigoCalidad = new PosicionComprasDto
            {
                Material = "Trigo Calidad",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCalidad,
                Total = kilosPosicionTrigoCalidad.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoGrado = materialId.Contains(2) ? TraerPosicionMaterial(2, fechaDesde, fechaHasta, 7, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionTrigoGrado = new PosicionComprasDto
            {
                Material = "Trigo Grado 2",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoGrado,
                Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosiciongirasol = materialId.Contains(4) ? TraerPosicionMaterial(4, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionGirasol = new PosicionComprasDto
            {
                Material = "Girasol",
                MaterialId = 4,
                PosicionKilos = kilosPosiciongirasol,
                Total = kilosPosiciongirasol.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionGirasolAlto = materialId.Contains(5) ? TraerPosicionMaterial(5, fechaDesde, fechaHasta, null, precioPizarra, negocios, centroId) : new List<PosicionKilos>();
            var posicionGirasolAlto = new PosicionComprasDto
            {
                Material = "Girasol Alto Oleico",
                MaterialId = 5,
                PosicionKilos = kilosPosicionGirasolAlto,
                Total = kilosPosicionGirasolAlto.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            return new List<PosicionComprasDto> { posicionSoja, posicionMaiz, posicionTrigoCamara, posicionTrigoCalidad, posicionTrigoGrado, posicionGirasol, posicionGirasolAlto };
        }
        public List<PricingCampaniaDto> TraerPricingCampania(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var negocio = repositorio.Listar<Contrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = x.TipoNegocioId,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                SanLorenzo = x.Destino.Acopio == false ? Math.Round(x.Cantidad / 1000) : 0,
                Acopio = x.Destino.Acopio == true ? Math.Round(x.Cantidad / 1000) : 0
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false &&
            DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde && DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta
            && x.TipoNegocioId == 2 &&
            (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == x.DestinoId) && x.ContratoAcuerdoId == null && x.TipoAgenteCompraId == null);
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                TipoNegocioId = 3,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId ?? 0,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000),
                SanLorenzo = x.Destino.Acopio == false ? Math.Round(x.Cantidad / 1000) : 0,
                Acopio = x.Destino.Acopio == true ? Math.Round(x.Cantidad / 1000) : 0
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false &&  DbFunctions.TruncateTime(x.FechaOperacion) >= fechaDesde && 
            DbFunctions.TruncateTime(x.FechaOperacion) <= fechaHasta && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == x.DestinoId));
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
                && DbFunctions.TruncateTime(x.Fecha) >= fechaDesde && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta
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
                SanLorenzo = x.Destino.Acopio == false ? Math.Round((double)x.Cantidad / 1000) : 0,
                Acopio = x.Destino.Acopio == true ? Math.Round((double)x.Cantidad / 1000) : 0
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

        public List<PrecioCantidadDto> TraerMonedaCantidad(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId, int centroId = 0)
        {
            var moneda = repositorio.ListarConsulta(new TraerMonedaKilo(fechaDesde, fechaHasta, materialId, centroId));

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
        public HedgeCargaObjetivoDto TraerHedgeObjetivo(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();
            var obj = new HedgeCargaObjetivoDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeObjetivo>(x => materialId.Contains(x.MaterialId) && DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var cumplidosContratos = repositorio.Listar<Contrato>(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde && x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var cumplidosFijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde && x.CampanaId <= x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

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
        public HedgeTCPromedioDto TraerTcPromedio(DateTime fechaDesde, DateTime fechaHasta, List<int> materialId)
        {
            if (materialId == null || materialId.Count() == 0) materialId = repositorio.Listar<Material, int>(x => x.MaterialId).ToList();

            var obj = new HedgeTCPromedioDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeTC>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var contratos = repositorio.Listar<Contrato>(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && x.TipoAgenteCompraId == null && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);
                var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.FechaOperacion) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampaniaTableroId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);
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
                DbFunctions.TruncateTime(x.Fecha) >= fechaDesde &&
                DbFunctions.TruncateTime(x.Fecha) <= fechaHasta &&
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
                    agenteTemp.PrecioPonderado = agentesPorPosicionYMaterial.Sum(x => (x.MonedaId.Contains("ARP") ? x.Precio / precioDolar : x.Precio) * (decimal)x.Cantidad) / agentesPorPosicionYMaterial.Sum(x => (decimal)x.Cantidad);
                }
                listaAgentes.Add(agenteTemp);
            }
            //}
            return listaAgentes.OrderBy(x => x.MaterialId).ThenBy(x => new DateTime(int.Parse(x.Posicion.Split('.')[1]), int.Parse(x.Posicion.Split('.')[0]), 1)).ToList();
        }

        public ExcelDetallePosicionDto DetallePosicion(int materialId, int mes, int anio, DateTime fechadesde, DateTime fechaHasta, int? calidad, int centroId = 0)
        {
            var excel = new ExcelDetallePosicionDto();
            excel.Headers = typeof(DetalleContratoDto).GetProperties().Select(p => Text.ResourceManager.GetString(p.Name)).ToArray();
            excel.Data = ConvertirListadetalleContratoAListaString(TraerDetallePosicion(materialId, mes, anio, fechadesde, fechaHasta, calidad, centroId), mes, anio);
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
        public List<ExcelPosicionMaterialDto> PosicionPorMaterial(DateTime fechaDesde, DateTime fechaHasta)
        {
            return repositorio.ListarConsulta(new TraerPosicionMaterialMes(fechaDesde, fechaHasta));
        }
        private List<PosicionKilos> TraerPosicionMaterial(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad, List<PrecioPizarra> precioPizarra, List<BasicoContrato> negocios, int centroId = 0)
        {
            var standard = calidad.HasValue ? calidad.Value : 1;
            precioPizarra = precioPizarra.Where(x => x.MaterialId == materialId && x.FechaHasta <= fechaHasta).ToList();
            var precio = precioPizarra.Count != 0 ? precioPizarra.OrderByDescending(x => x.FechaHasta).FirstOrDefault() : new PrecioPizarra();
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(+1);
            var posicionKilos = new List<PosicionKilos>();
            var contratos = negocios.Where(x =>
                x.OcultarEnTablero == false
                //&& x.Fecha >= fechaHoy && x.Fecha <= fechaManana
                && (x.Estado == 2 || x.Estado == 4 || x.Estado == 5)
                && x.MaterialId == materialId
                && (calidad == null || (calidad != null && x.StandardCalidadId == standard))
                && (centroId == 0 || x.DestinoId == centroId)
                && x.ContratoAcuerdoId == null && x.EsFason != true
                && (x.TipoNegocioId == 1 || x.TipoNegocioId == 2)
            ).Select(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.FechaDesde.Value,
                FechaHasta = x.FechaHasta.Value,
                Cantidad = x.Cantidad,
                Precio = x.Pizarra == true ? precio.Precio : x.Precio,
                TipoNegocioId = x.TipoNegocioId,
                CampanaMaterialId = x.CampanaMaterialId,
                CampanaId = x.CampanaId,
                CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                MonedaId = x.Pizarra == true ? precio.MonedaId : x.MonedaId
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
            && (x.Estado == 2 || x.Estado == 4 || x.Estado == 5)
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
                    CampanaMaterialId = x.CampanaMaterialId,
                    CantidadPonderada = x.Pizarra == true && precio.Precio != 0 ? x.Cantidad : x.Precio != 0 ? x.Cantidad : 0,
                    MonedaId = x.Pizarra == true ? precio.MonedaId : x.MonedaId
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
                && (calidad == null || (calidad == 7 && x.TrigoEspecial == true) || (calidad == 3 && x.TrigoEspecial == false))
                && (centroId == 0 || centroId == 1)
            ).Select(x => new PosicionPorMaterial
            {
                Id = x.Id,
                FechaDesde = x.Fecha.Value,
                FechaHasta = x.Fecha.Value,
                Cantidad = x.Cantidad,
                TipoNegocioId = 4,
                Precio = x.Precio,
                CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                CampanaId = x.CampanaId,
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
                    FechaDesde = x.Fecha.Value,
                    FechaHasta = x.Fecha.Value,
                    Cantidad = x.Cantidad,
                    TipoNegocioId = 6,
                    Precio = x.Precio,
                    CantidadPonderada = x.Precio != 0 ? x.Cantidad : 0,
                    MonedaId = x.MonedaId,
                    CampanaId = x.CampanaId,
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
        public string DetallePosicionModalIds(List<int> negocios, string moneda)
        {
            var detalle = TraerDetallePosicion(negocios, moneda);
            detalle.ForEach(x => x.Cantidad = (int.Parse(x.Cantidad)).ToString("n0"));
            detalle.ForEach(x => x.Precio = decimal.Parse(x.Precio.Replace('.', ',')).ToString("n2"));
            detalle.ForEach(x => x.PrecioNeto = decimal.Parse(x.PrecioNeto.Replace('.', ',')).ToString("n2"));
            return JsonConvert.SerializeObject(new { items = detalle, total = detalle.Count() }); ;
        }
        private List<DetalleContratoDto> TraerDetallePosicion(List<int> negocios, string moneda)
        {
            if (string.IsNullOrWhiteSpace(moneda))
                moneda = "";
            var data = new List<DetalleContratoDto>();
            var contratos = repositorio.Listar<Contrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Madre == true ? "MADRE" : x.Madre == false ? "HIJO" : x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampaniaTableroId,
                CampanaId = x.CampanaId,
                FechaDesdeDate = x.FechaDesde,
                FechaHastaDate = x.FechaHasta,
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadD = x.Cantidad,
                CantidadCamiones = x.CantidadCamiones.ToString(),
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.FechaDesde) + "/" + SqlFunctions.DatePart("month", x.FechaDesde) + "/" + SqlFunctions.DateName("year", x.FechaDesde),
                FechaHasta = SqlFunctions.DateName("day", x.FechaHasta) + "/" + SqlFunctions.DatePart("month", x.FechaHasta) + "/" + SqlFunctions.DateName("year", x.FechaHasta),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
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
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString(),
                MercsDeposito = x.MercsDeposito == true ? "X" : "",
                Pizarra = x.Pizarra
            },
            x => negocios.Contains(x.Id) 
                && (moneda == "" || x.MonedaId == moneda)
            );

            if (contratos != null)
            {
                data.AddRange(contratos);
            }

            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Id = x.Id,
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
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
                MercsDeposito = "",
                Pizarra = x.Pizarra
            },
             x => negocios.Contains(x.Id));

            if (fijaciones != null)
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
                PrecioNeto = x.Precio.ToString(),
                MercsDeposito = "",
                Pizarra = x.Pizarra
            },
             x => negocios.Contains(x.Id));
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
                CantidadD = x.Cantidad,
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
                Pizarra = x.Pizarra
            },
             x => negocios.Contains(x.Id));


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
                Destino = "",
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
                Pizarra = x.Pizarra
            },
            x => negocios.Contains(x.Id));

            if (agente != null)
            {
                data.AddRange(agente);
            }

            if (data.Count > 0 && data.Any(a => a.Pizarra == true))
            {
                var maxFecha = data.Where(a => a.Pizarra == true).Max(a => a.FechaDate);
                var preciosPizarra = repositorio.Listar<PrecioPizarra>(x => x.FechaHasta <= maxFecha);
                foreach (var item in data.Where(a => a.Moneda == "").ToList())
                {
                    var precioPizarra = preciosPizarra.Where(x => x.Material.Descripcion == item.Material && x.FechaHasta <= item.FechaDate).ToList();
                    var precio = precioPizarra.Count != 0 ? precioPizarra.OrderByDescending(x => x.FechaHasta).FirstOrDefault() : new PrecioPizarra();
                    item.Moneda = precio.MonedaId;
                    item.Precio = precio.Precio.ToString();
                }
            }

            return data;
        }

        private List<DetalleContratoDto> TraerDetallePosicion(int materialId, int? mes, int? anio, DateTime fechaDesdeFiltro, DateTime fechaHastaFiltro, int? calidad, int centroId = 0)
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
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
                Cuit = x.Proveedor.CUIT,
                RazonCorredor = x.Corredor.RazonSocial,
                CuitCorredor = x.Corredor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Madre == true ? "MADRE" : x.Madre == false ? "HIJO" : x.TipoNegocio.Descripcion,
                TipoNegocioId = x.TipoNegocioId,
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
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda != null ? x.Moneda.Descripcion : "",
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
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString()
            },
            x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.StandardDeCalidadId == calidad))
             && (centroId == 0 || x.DestinoId == centroId)
             && x.ContratoAcuerdoId == null);
            var data = contratos;
            if (mes.HasValue && anio.HasValue)
            {
                data = FiltrardetalleContratosPorMesAnio(contratos, mes.Value, anio.Value);
            }


            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = (x.EstadoId == (int)EnumEstadoContrato.Finalizado && x.ContratoSAP != null && x.ContratoSAP != "") ? x.ContratoSAP : x.Id.ToString(),
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
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString()
            },
            x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && x.OcultarEnTablero == false
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.TrigoEspecial == true && calidad == 7) || (calidad != null && x.TrigoEspecial == false && calidad == 3))
             && (centroId == 0 || centroId == x.DestinoId));
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
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => materialId.Contains(x.MaterialId) && x.OcultarEnTablero == false && DbFunctions.TruncateTime(x.Fecha) == fechaHoy
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
        public string DetallePosicionModal(int materialId, int? mes, int? anio, DateTime fechaDesde, DateTime fechaHasta, int? calidad, int centroId = 0)
        {
            var detalle = TraerDetallePosicion(materialId, mes, anio, fechaDesde, fechaHasta, calidad, centroId);
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

        public ReporteCompraNetModel ObtenerDatosReporteCompraNet(DateTime fechaDesde, DateTime fechaHasta, string centroId, List<int> materialId)
        {
            int idCentro = int.Parse(centroId);
            bool filtrarAcopio = idCentro == 0 || idCentro == 1;
            var agentes = filtrarAcopio ? TraerAgenteDeCompra(fechaDesde, fechaHasta, materialId) : new List<AgenteCompraDto>();
            var op = agentes.SelectMany(x => x.Operador).GroupBy(x => x.OperadorId).Select(x => x.First()).ToList();
            agentes.ForEach(x => x.Operador.ForEach(y => y.Cantidad = y.Cantidad));

            var objetivos = TraerHedgeObjetivo(fechaDesde, fechaHasta, materialId);
            objetivos.PricingCumplido = objetivos.PricingCumplido;
            objetivos.PricingObjetivo = objetivos.PricingObjetivo;
            objetivos.RemitirCumplido = objetivos.RemitirCumplido;
            objetivos.RemitirObjetivo = objetivos.RemitirObjetivo;
            var result = new ReporteCompraNetModel
            {
                ToneladasGranoTipo = TraerToneladasGranoTipo(fechaDesde, fechaHasta, materialId, idCentro),
                SojaSustentable = (materialId == null || materialId.Contains(3)) ? TraerToneladasSojaSust(fechaDesde, fechaHasta, idCentro) : new ReporteSojaSustDto(),
                PosicionCompras = TraerPosicionCompras(fechaDesde, fechaHasta, materialId, idCentro),
                PricingCampania = TraerPricingCampania(fechaDesde, fechaHasta, materialId, idCentro),
                PrecioCantidad = TraerMonedaCantidad(fechaDesde, fechaHasta, materialId, idCentro),
                HedgeMaterial = TransformarAModel(TraerTodosHedgeMaterial(fechaDesde, fechaHasta, materialId)),
                HedgeObjetivo = objetivos,
                TCPromedioDto = TraerTcPromedio(fechaDesde, fechaHasta, materialId),
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
                foreach (var posicion in result.PosicionCompras)
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
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio + y.DispFijac + y.FrwAPrecio + y.FrwFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.DispAgente + x.FrwAgente));
                            datos.Pricing = sumaPricing;

                            break;
                        case 22:
                            sumaPricing = (result.PosicionCompras.Where(x => x.MaterialId == 2).Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio + y.NewFijac)).Sum() + result.ToneladasGranoTipo.Where(x => x.Material == "Trigo Cámara" || x.Material == "Trigo Calidad").Sum(x => x.NewAgente));
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
    }
}
