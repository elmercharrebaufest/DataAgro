using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Resources;
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
            var equipo = oComercial.ListarEquipo(idActiveDirectory).EquipoReal;
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
        public List<ResulIndicadores> TraerComprasMapa(ParamReportes oParamReportes)

        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresComprasMapa", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultIndicadoresReportesmini> TraerComprasMapaExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultIndicadoresReportesmini>("DataAgro_IndicadoresComprasExportacionMapa", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #region Torta
        public List<ResulIndicadores> TraerComprasTorta(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresComprasTorta", 0, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultIndicadoresReportesTorta> TraerComprasTortaExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultIndicadoresReportesTorta>("DataAgro_IndicadoresComprasExportacionTorta", 0, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #region Barra
        public List<ResultComprasBarrasReportes> TraerComprasBarra(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("Reporte_ComprasBarra_Traer", 0, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultComprasBarrasReportesmini> TraerComprasBarraExportacion(ParamReportes oParamReportes)
        {

            return repositorio.SelStore<ResultComprasBarrasReportesmini>("Reporte_ComprasBarra_TraerExcel", 0, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #endregion

        #region Productiva

        #region Mapa
        public List<ResulIndicadores> TraerCapacidadProductivaMapa(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresProductiva", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultProduccionMapaReportes> TraerCapacidadProductivaMapaExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultProduccionMapaReportes>("DataAgro_IndicadoresExportacionMapaProductiva", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #region Barras
        public List<ResultComprasBarrasReportes> TraerCapacidadProductivaBarra(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("DataAgro_IndicadoresProductivaBarra", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultProduccionBarraReportes> TraerCapacidadProductivaBarraExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultProduccionBarraReportes>("DataAgro_IndicadoresExportacionBarraProductiva", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #endregion

        #region Acopio
        #region Mapa
        public List<ResulIndicadores> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResulIndicadores>("DataAgro_IndicadoresAcopio", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultAcopioMapaReportes> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultAcopioMapaReportes>("DataAgro_IndicadoresExportacionMapaAcopio", 0, oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion

        #region Barra
        public List<ResultComprasBarrasReportes> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultComprasBarrasReportes>("DataAgro_IndicadoresAcopioBarra", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }

        public List<ResultAcopioBarraReportes> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultAcopioBarraReportes>("DataAgro_IndicadoresExportacionBarraAcopio", 0, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual);
        }
        #endregion
        #endregion

        #region Objetivos
        public List<ResultObjetivoGaugeReportes> TraerObjetivosGauge(ParamReportes oParamReportes)
        {
            return repositorio.SelStore<ResultObjetivoGaugeReportes>("DataAgro_Gauget_Traer", 0, oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Cosecha, oParamReportes.Grano);
        }

        public List<ResultObjetivoGaugeReportes> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes)
        {

            return repositorio.SelStore<ResultObjetivoGaugeReportes>("DataAgro_Gauget_TraerExcel", 0, oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Cosecha, oParamReportes.Grano);

        }
        #endregion

        public BaseDeDatosReturn TraerDatosGrillaBD(ParamReportes oParamReportes)
        {

            BaseDeDatosReturn datosGrilla = new BaseDeDatosReturn();

            datosGrilla.valoresGrilla = repositorio.SelStore<valoresGrilla>("DataAgro_IndicadoresBaseDeDatos_Traer", 0, oParamReportes.FechaDesde, oParamReportes.FechaHasta, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Cosecha, oParamReportes.Toneladas, oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Grano);

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

        public List<ToneladasGranoTipoDto> TraerToneladasGranoTipo(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            var sojaToneladas =  repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(3, fechaDesde, fechaHasta, null, centroId));
            sojaToneladas.Material = "Soja";
            var maizToneladas =  repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(1, fechaDesde, fechaHasta, null, centroId));
            maizToneladas.Material = "Maiz";
            var trigoCamaraToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, 3, centroId));
            trigoCamaraToneladas.Material = "Trigo Cámara";
            var trigoCalidadToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fechaDesde, fechaHasta, 2, centroId));
            trigoCalidadToneladas.Material = "Trigo Calidad";
            var girasolToneladas =  repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(4, fechaDesde, fechaHasta, null, centroId));
            girasolToneladas.Material = "Girasol";
            var girasolAltoToneladas =  repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, null, centroId));
            girasolAltoToneladas.Material = "Girasol Alto Oleico";
            var trigoGradoToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(25, fechaDesde, fechaHasta, 7, centroId));
            trigoGradoToneladas.Material = "Trigo Grado 2";
            return new List<ToneladasGranoTipoDto>() { sojaToneladas, maizToneladas, trigoCamaraToneladas, trigoCalidadToneladas, trigoGradoToneladas, girasolToneladas, girasolAltoToneladas };

        }
        public ReporteSojaSustDto TraerToneladasSojaSust(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerToneladasSojaSustentable(fechaDesde, fechaHasta, centroId));
        }
        public List<PosicionComprasDto> TraerPosicionCompras(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            var kilosPosicionSoja = TraerPosicionMaterial(3, fechaDesde, fechaHasta, null, centroId);
            var posicionSoja = new PosicionComprasDto
            {
                Material = "Soja",
                MaterialId = 3,
                PosicionKilos = kilosPosicionSoja,
                Total = kilosPosicionSoja.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionMaiz = TraerPosicionMaterial(1, fechaDesde, fechaHasta, null, centroId);
            var posicionMaiz = new PosicionComprasDto
            {
                Material = "Maiz",
                MaterialId = 1,
                PosicionKilos = kilosPosicionMaiz,
                Total = kilosPosicionMaiz.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoCamara = TraerPosicionMaterial(2, fechaDesde, fechaHasta, 3, centroId);
            var posicionTrigoCamara = new PosicionComprasDto
            {
                Material = "Trigo Cámara",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCamara,
                Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoCalidad = TraerPosicionMaterial(2, fechaDesde, fechaHasta, 2, centroId);
            var posicionTrigoCalidad = new PosicionComprasDto
            {
                Material = "Trigo Calidad",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCalidad,
                Total = kilosPosicionTrigoCalidad.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionTrigoGrado = TraerPosicionMaterial(2, fechaDesde, fechaHasta, 7, centroId);
            var posicionTrigoGrado = new PosicionComprasDto
            {
                Material = "Trigo Grado 2",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoGrado,
                Total = kilosPosicionTrigoCamara.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosiciongirasol = TraerPosicionMaterial(4, fechaDesde, fechaHasta, null, centroId);
            var posicionGirasol = new PosicionComprasDto
            {
                Material = "Girasol",
                MaterialId = 4,
                PosicionKilos = kilosPosiciongirasol,
                Total = kilosPosiciongirasol.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            var kilosPosicionGirasolAlto = TraerPosicionMaterial(5, fechaDesde, fechaHasta, null, centroId);
            var posicionGirasolAlto = new PosicionComprasDto
            {
                Material = "Girasol Alto Oleico",
                MaterialId = 5,
                PosicionKilos = kilosPosicionGirasolAlto,
                Total = kilosPosicionGirasolAlto.Sum(x => x.KilosPesos + x.KilosDolares)
            };
            return new List<PosicionComprasDto> { posicionSoja, posicionMaiz, posicionTrigoCamara, posicionTrigoCalidad, posicionTrigoGrado, posicionGirasol, posicionGirasolAlto };
        }
        public List<PricingCampaniaDto> TraerPricingCampania(DateTime fechaDesde, DateTime fechaHasta, int centroId)
        {
            var negocio = repositorio.Listar<Contrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.ContratoId,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000)
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaDesde && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta && x.TipoNegocioId == 2 &&
            (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == x.DestinoId) && x.ContratoAcuerdoId == null);
            var fijaciones = repositorio.Listar<FijacionDePrecioContrato, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.FijacionDePrecioContratoId,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId.Value,
                Pricing = Math.Round(x.Cantidad / 1000)
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaDesde
                && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta && (x.MaterialId == 1 || x.MaterialId == 2 || x.MaterialId == 3) && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == x.DestinoId));
            var fasones = repositorio.Listar<Fason, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                Campania = x.Campana.Descripcion,
                CampaniaId = x.CampanaId,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000)
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaDesde
                    && DbFunctions.TruncateTime(x.Fecha) <= fechaHasta && (x.MaterialId == 1 || x.MaterialId == 2 || x.MaterialId == 3) && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1));
            var agentes = repositorio.Listar<AgenteCompra, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                Campania = x.Posicion,
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = Math.Round(x.Cantidad / 1000)
            }, x => fechaDesde == fechaHasta && DbFunctions.TruncateTime(x.Fecha) == fechaDesde
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1));
            var acuerdo = repositorio.Listar<ContratoAcuerdo, PricingCampaniaDto>(x => new PricingCampaniaDto
            {
                Id = x.Id,
                Campania = "17-18",
                Material = x.Material.Descripcion,
                MaterialId = x.MaterialId,
                Pricing = x.Cantidad
            }, x => fechaDesde == fechaHasta && DbFunctions.TruncateTime(x.Fecha) == fechaDesde
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5) && (centroId == 0 || centroId == 1));
            var pricing = new List<PricingCampaniaDto>();
            var materiales = repositorio.Listar<Material, MaterialDto>(x => new MaterialDto { MaterialId = x.MaterialId, CampañaId = x.CampañaId, Descripcion = x.Descripcion, Campana = x.Campaña.Descripcion });

            negocio.AddRange(fijaciones);
            negocio.AddRange(fasones);
            negocio.AddRange(acuerdo);
            foreach (var neg in negocio)
            {
                var campania = materiales.FirstOrDefault(x => x.MaterialId == neg.MaterialId);
                neg.Campania = campania.CampañaId < neg.CampaniaId ? "New Crop" : campania.Campana;
            }
            foreach (var agente in agentes)
            {
                var campania = materiales.FirstOrDefault(x => x.MaterialId == agente.MaterialId);
                var fechaNewCrop = new DateTime(DateTime.Now.AddYears(1).Year, agente.MaterialId == 3 ? 4 : agente.MaterialId == 1 ? 3 : 11, 1);
                var posicion = agente.Campania.Split('.');
                agente.Campania = fechaNewCrop <= new DateTime(int.Parse(posicion[1]), int.Parse(posicion[0]), 1) ? "New Crop" : campania.Campana;
            }
            negocio.AddRange(agentes);
            var group = negocio.GroupBy(x => new { x.Campania, x.Material });
            foreach (var e in group)
            {
                var price = new PricingCampaniaDto
                {
                    Id = e.Select(x => x.MaterialId).FirstOrDefault() * 10 + (e.Key.Campania == "New Crop" ? 2 : 1),
                    Campania = e.Key.Campania,
                    CampaniaId = e.Select(x => x.CampaniaId).FirstOrDefault(),
                    Material = e.Key.Material == "Semilla de Soja" ? "Soja" : e.Key.Material == "Maiz Duro Dentado" ? "Maiz" : e.Key.Material,
                    MaterialId = e.Select(x => x.MaterialId).FirstOrDefault(),
                    Pricing = Math.Round(e.Sum(x => x.Pricing) / 1000)
                };
                pricing.Add(price);
            }
            return pricing.OrderBy(x => x.Id).ToList();
        }

        public List<PrecioCantidadDto> TraerMonedaCantidad(DateTime fechaDesde, DateTime fechaHasta, int centroId = 0)
        {
            var moneda = repositorio.ListarConsulta(new TraerMonedaKilo(fechaDesde, fechaHasta, centroId));

            return new List<PrecioCantidadDto>() { new PrecioCantidadDto {Moneda = "Pesos" , Cantidad= moneda.Exists(x=>x.Moneda == "ARP  ")?moneda.Where(x=>x.Moneda== "ARP  ").Select(x=>x.Cantidad).First():0},
                new PrecioCantidadDto {Moneda = "Dólares" , Cantidad=moneda.Exists(x=>x.Moneda == "USDM ")? moneda.Where(x=>x.Moneda== "USDM ").Select(x=>x.Cantidad).First():0}};
        }
        public List<HedgeMaterialDto> TraerTodosHedgeMaterial(DateTime fechaDesde, DateTime fechaHasta)
        {
            var hedgeMat = new List<HedgeMaterialDto>();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                hedgeMat.AddRange(repositorio.Listar<HedgeMaterial, HedgeMaterialDto>(x => new HedgeMaterialDto
                {
                    MaterialId = x.MaterialId,
                    TipoHedgeMaterialId = x.TipoHedgeMaterialId,
                    Cantidad = x.Cantidad
                }, x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde));
            }
            return hedgeMat;
        }
        public HedgeCargaObjetivoDto TraerHedgeObjetivo(DateTime fechaDesde, DateTime fechaHasta)
        {
            var obj = new HedgeCargaObjetivoDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeObjetivo>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var cumplidosContratos = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.CampanaId <= x.Material.CampañaId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));
                var cumplidosFijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.CampanaId <= x.Material.CampañaId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5));

                obj.PricingObjetivo = objetivos.Where(x => x.TipoObjetivoId == 1).Sum(x => x.Cantidad);
                obj.RemitirObjetivo = objetivos.Where(x => x.TipoObjetivoId == 2).Sum(x => x.Cantidad);
                obj.PricingCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad) + cumplidosFijaciones.Sum(x => (decimal)x.Cantidad);
                obj.RemitirCumplido = cumplidosContratos.Where(x => x.TipoNegocioId == 1 || x.TipoNegocioId == 2).Sum(x => (decimal)x.Cantidad);
            }
            return obj;
        }
        public HedgeTCPromedioDto TraerTcPromedio(DateTime fechaDesde, DateTime fechaHasta)
        {
            var obj = new HedgeTCPromedioDto();
            if (fechaDesde == fechaHasta)
            {
                fechaDesde = fechaDesde.Date;
                var objetivos = repositorio.Listar<HedgeTC>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde);
                var contratos = repositorio.Listar<Contrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampañaId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);
                var fijaciones = repositorio.Listar<FijacionDePrecioContrato>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampañaId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);
                var fason = repositorio.Listar<Fason>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && x.MonedaId == "ARP  " && x.CampanaId == x.Material.CampañaId && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).Sum(x => x.Precio);

                if (objetivos != null)
                {
                    decimal sumProd = 0;
                    decimal total = 0;
                    foreach (var hT in objetivos)
                    {
                        sumProd += hT.TipoCambio * hT.HedgePesos;
                        total += hT.HedgePesos;
                    }
                    if (total > 0)
                    {
                        obj.PromedioTC = sumProd / total;
                    }
                    obj.TotalTC = objetivos.Sum(x => x.HedgePesos) - contratos - fijaciones - fason;
                }
            }
            return obj;
        }
        public List<AgenteCompraDto> TraerAgenteDeCompra(DateTime fechaDesde, DateTime fechaHasta)
        {
            var listaAgentes = new List<AgenteCompraDto>();
            if (fechaDesde == fechaHasta)
            {
                var precioDolar = tipoDeCambio.TraerTipoDeCambio();
                fechaDesde = fechaDesde.Date;
                var agentes = repositorio.Listar<AgenteCompra>(x => DbFunctions.TruncateTime(x.Fecha) == fechaDesde && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)).GroupBy(x => new { x.Posicion, x.MaterialId, x.TipoAgenteCompraId });
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
                        agenteTemp.Posicion = agentesPorPosicionYMaterial.Key.Posicion;
                        agenteTemp.MaterialId = agentesPorPosicionYMaterial.Key.MaterialId;
                        agenteTemp.MaterialDesc = agente.Material.Descripcion;
                        agenteTemp.TipoAgenteId = agentesPorPosicionYMaterial.Key.TipoAgenteCompraId;
                        agenteTemp.TipoAgenteDesc = agente.TipoAgenteCompra.Descripcion;
                        agenteTemp.PrecioPonderado = agentesPorPosicionYMaterial.Sum(x => (x.MonedaId.Contains("ARP") ? x.Precio / precioDolar : x.Precio) * (decimal)x.Cantidad) / agentesPorPosicionYMaterial.Sum(x => (decimal)x.Cantidad);
                    }
                    listaAgentes.Add(agenteTemp);
                }
            }
            return listaAgentes;
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

        public ExcelDetallePosicionDto DetalleAgente(DateTime fecha)
        {
            var excel = new ExcelDetallePosicionDto();
            excel.Headers = typeof(DetalleAgenteDto).GetProperties().Select(p => Text.ResourceManager.GetString(p.Name)).ToArray();
            excel.Data = ConvertirListaAgente(TraerDetalleAgente(fecha));
            excel.Name = "Detalle Agente de Compras.xlsx";
            excel.SheetName = "Agente";
            return excel;
        }
        public List<ExcelPosicionMaterialDto> PosicionPorMaterial(DateTime fechaDesde, DateTime fechaHasta)
        {
            return repositorio.ListarConsulta(new TraerPosicionMaterialMes(fechaDesde, fechaHasta));
        }
        private List<PosicionKilos> TraerPosicionMaterial(int materialId, DateTime fechaDesde, DateTime fechaHasta, int? calidad, int centroId = 0)
        {
            var standard = calidad.HasValue ? calidad.Value : 1;
            var fechaHoy = fechaDesde.Date;
            var fechaManana = fechaHasta.Date;
            var fechaPosicion = new DateTime(DateTime.Now.Year, DateTime.Now.AddMonths(+1).Month, 1);
            var posicionKilos = new List<PosicionKilos>();
            var contratos = repositorio.Listar<Contrato, PosicionPorMaterial>(x => new PosicionPorMaterial
            {
                Id = x.ContratoId,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                TipoNegocioId = x.TipoNegocioId,
                CampanaMaterialId = x.Material.CampañaId,
                CampanaId = x.CampanaId,
                CantidadPonderada = x.Precio > 0 ? x.Cantidad : 0,
                MonedaId = x.MonedaId
            },
                x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
                && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
                && x.MaterialId == materialId
                && (calidad == null || (calidad != null && x.StandardDeCalidadId == standard))
                && (centroId == 0 || x.DestinoId == centroId)
                && x.ContratoAcuerdoId == null);
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
                Id = x.FijacionDePrecioContratoId,
                FechaDesde = x.FechaDesde,
                FechaHasta = x.FechaHasta,
                Cantidad = x.Cantidad,
                Precio = x.Precio,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampañaId,
                CantidadPonderada = x.Precio > 0 ? x.Cantidad : 0,
                MonedaId = x.MonedaId
            },
                x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
                && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
                && x.MaterialId == materialId
                && (calidad == null || (calidad != null &&  (calidad==2|| calidad == 7 ? x.TrigoEspecial == true: x.TrigoEspecial == false)))
                && (centroId == 0 || centroId == x.DestinoId));
            foreach (var cont in fijaciones)
            {
                var posicion = cont.FechaDesde - DateTime.Now;

                cont.ClasificacionNegocio = (posicion.Days<=30 && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
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
                Precio = x.Precio,
                CantidadPonderada = x.Precio > 0 ? x.Cantidad : 0,
                CampanaId = x.CampanaId,
                CampanaMaterialId = x.Material.CampañaId,
                Posicion = x.Posicion,
                MonedaId = x.MonedaId
            },
                x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
                && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
                && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
                && x.MaterialId == materialId
                && (calidad == null || (calidad != null && (calidad == 2 ? x.Especial == true : x.Especial == false)))
                && (centroId == 0 || centroId == 1));
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
                Precio = x.Precio,
                CantidadPonderada = x.Precio > 0 ? x.Cantidad : 0,
                MonedaId = x.MonedaId,

            },
               x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
               && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
               && (x.EstadoId == 2 || x.EstadoId == 5)
               && x.MaterialId == materialId
               && (calidad == null || (calidad.Value!= 7 && calidad.Value != 2))
               && (centroId == 0 || x.DestinoId == centroId));

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
                cont.ClasificacionNegocio = (fechaPosicion >= posicion && cont.CampanaMaterialId == cont.CampanaId) || (cont.CampanaMaterialId > cont.CampanaId) ? EnumClasificacionNegocio.DisponibleFijacion :
                cont.TipoNegocioId == 1 && cont.CampanaMaterialId == cont.CampanaId && fechaPosicion < posicion ? EnumClasificacionNegocio.ForwardFijacion :
                EnumClasificacionNegocio.NewCropFijacion;
            }
            contratos.AddRange(acuerdos);

            foreach (var cont in contratos)
            {
                var posKil = new PosicionKilos();

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
                            DispFijac = Math.Round(x.Sum(y => y.DispFijac / 1000)),
                            FrwAFijar = Math.Round(x.Sum(y => y.FrwAFijar / 1000)),
                            FrwAPrecio = Math.Round(x.Sum(y => y.FrwAPrecio / 1000)),
                            FrwFijac = Math.Round(x.Sum(y => y.FrwFijac / 1000)),
                            NewAFijar = Math.Round(x.Sum(y => y.NewAFijar / 1000)),
                            NewAPrecio = Math.Round(x.Sum(y => y.NewAPrecio / 1000)),
                            NewFijac = Math.Round(x.Sum(y => y.NewFijac / 1000)),
                            PrecioPonderadoPesos = x.Where(y => y.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada) > 0 ? x.Sum(y => y.PrecioPonderadoPesos / (decimal)x.Where(f => f.PrecioPonderadoPesos != 0).Sum(f => f.CantidadPonderada)) : 0,
                            PrecioPonderadoDolares = x.Where(y => y.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada) > 0 ? x.Sum(y => y.PrecioPonderadoDolares / (decimal)x.Where(f => f.PrecioPonderadoDolares != 0).Sum(f => f.CantidadPonderada)) : 0
                        }).ToList();
            return posicionKilos;
        }
        private List<DetalleContratoDto> TraerDetallePosicion(int materialId, int mes, int anio, DateTime fechaDesdeFiltro, DateTime fechaHastaFiltro, int? calidad, int centroId = 0)
        {
            var fechaHoy = fechaDesdeFiltro.Date;
            var fechaManana = fechaHastaFiltro.Date;
            var listaDatos = new List<string[]>();
            var contratos = repositorio.Listar<Contrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = x.ContratoId.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = x.Madre == true ? "MADRE" : x.Madre == false ? "HIJO" : x.TipoNegocio.Descripcion,
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
                CalidadEspecial = x.TrigoEspecial == true ? "X" : "",
                EstablecimientoPropio = x.EstablecimientoPropio == true ? "Propio" : x.EstablecimientoPropio == false ? "Arrendado" : "",
                Observacion = x.Observacion ?? "",
                PrecioNeto = (x.PrecioNeto != null) ? x.PrecioNeto.ToString() : x.Precio.ToString()
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.StandardDeCalidadId == calidad))
             && (centroId == 0 || x.DestinoId == centroId)
             && x.ContratoAcuerdoId == null);

            contratos.AddRange(repositorio.Listar<FijacionDePrecioContrato, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = x.ContratoId.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "FIJACION",
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaHasta = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
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
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.TrigoEspecial == true && calidad == 2))
             && (centroId == 0 || centroId == x.DestinoId)));

            contratos.AddRange(repositorio.Listar<Fason, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = x.Id.ToString(),
                RazonSocial = x.Fasonero.RazonSocial,
                Cuit = x.Fasonero.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "FASÓN",
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = "",
                Campana = x.Campana != null ? x.Campana.Descripcion : "",
                FechaDesde = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaHasta = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
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
                CalidadEspecial = x.Especial == true ? "X" : "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString()
            },
            x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5)
             && x.MaterialId == materialId
             && (calidad == null || (calidad != null && x.Especial == true && calidad == 2))
             && (centroId == 0 || centroId == 1)));

            contratos.AddRange(repositorio.Listar<ContratoAcuerdo, DetalleContratoDto>(x => new DetalleContratoDto
            {
                Contrato = x.Id.ToString(),
                RazonSocial = x.Proveedor.RazonSocial,
                Cuit = x.Proveedor.CUIT,
                Material = x.Material.Descripcion,
                TipoNegocio = "Acuerdo",
                Comercial = x.Comercial != null ? x.Comercial.Nombres + " " + x.Comercial.Apellido : "",
                Cantidad = SqlFunctions.StringConvert((double)x.Cantidad),
                CantidadCamiones = "",
                Campana = "",
                FechaDesde = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                FechaHasta = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
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
                CalidadEspecial = "",
                EstablecimientoPropio = "",
                Observacion = "",
                PrecioNeto = x.Precio.ToString()
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && DbFunctions.TruncateTime(x.Fecha) <= fechaManana
             && (x.EstadoId == 2)
             && x.MaterialId == materialId
             && (!calidad.HasValue || calidad.Value == 3)
             && (centroId == 0 || x.DestinoId == centroId)));

            return contratos;
        }
        private List<DetalleAgenteDto> TraerDetalleAgente(DateTime fechaDesdeFiltro)
        {
            var fechaHoy = fechaDesdeFiltro.Date;
            var listaDatos = new List<string[]>();
            var agente = repositorio.Listar<AgenteCompra, DetalleAgenteDto>(x => new DetalleAgenteDto
            {
                Agente = x.Id.ToString(),
                Operador = x.Operador.Descripcion,
                Material = x.Material.Descripcion,
                Posicion = x.Posicion,
                Cantidad = x.Cantidad.ToString(),
                Precio = x.Precio.ToString(),
                Moneda = x.Moneda.Descripcion,
                Fecha = SqlFunctions.DateName("day", x.Fecha) + "/" + SqlFunctions.DatePart("month", x.Fecha) + "/" + SqlFunctions.DateName("year", x.Fecha),
                Comercial = x.Comercial.Nombres + " " + x.Comercial.Apellido
            }, x => DbFunctions.TruncateTime(x.Fecha) >= fechaHoy
             && (x.EstadoId == 2 || x.EstadoId == 4 || x.EstadoId == 5), 0, "Agente");

            return agente;
        }
        private List<string[]> ConvertirListadetalleContratoAListaString(List<DetalleContratoDto> contratos, int mes, int anio)
        {
            var listaDatos = new List<string[]>();
            foreach (var cont in contratos)
            {
                var fechaDesde = DateTime.Parse(cont.FechaDesde);
                var fechaHasta = DateTime.Parse(cont.FechaHasta);
                var dato = new string[34];
                if ((DateTime.DaysInMonth(fechaDesde.Year, fechaDesde.Month) - fechaDesde.Day) >= 10 && fechaDesde.Month == mes && fechaDesde.Year == anio)
                {
                    listaDatos.Add(CrearArray(cont));
                }
                else if (fechaDesde.AddMonths(1).Month <= fechaHasta.Month && fechaDesde.AddMonths(1).Month == mes && fechaDesde.Year == anio && (DateTime.DaysInMonth(fechaDesde.Year, fechaDesde.Month) - fechaDesde.Day) < 10)
                {
                    listaDatos.Add(CrearArray(cont));
                }
                else if (fechaDesde.AddMonths(1).Month > fechaHasta.Month && fechaHasta.Month == mes && fechaDesde.Year == anio && (DateTime.DaysInMonth(fechaDesde.Year, fechaDesde.Month) - fechaDesde.Day) < 10)
                {
                    listaDatos.Add(CrearArray(cont));
                }
            }
            return listaDatos;
        }

        private string[] CrearArray(DetalleContratoDto x)
        {
            return new string[]{
                x.Contrato ?? "",
                x.RazonSocial ?? "",
                x.Cuit ?? "",
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
                    agen.Cantidad,
                    agen.Precio,
                    agen.Moneda,
                    agen.Fecha,
                    agen.Comercial
                };
                listaDatos.Add(dato);
            }
            return listaDatos;
        }
        public string DetallePosicionModal(int materialId, int mes, int anio, DateTime fechaDesde, DateTime fechaHasta, int? calidad, int centroId = 0)
        {
            var detalle = TraerDetallePosicion(materialId, mes, anio, fechaDesde, fechaHasta, calidad, centroId);
            detalle.ForEach(x => x.Cantidad = int.Parse(x.Cantidad).ToString("n0"));
            detalle.ForEach(x => x.Precio = decimal.Parse(x.Precio.Replace('.', ',')).ToString("n2"));
            detalle.ForEach(x => x.PrecioNeto = decimal.Parse(x.PrecioNeto.Replace('.', ',')).ToString("n2"));
            var data = FiltrardetalleContratosPorMesAnio(detalle, mes, anio);
            return JsonConvert.SerializeObject(new { items = data, total = data.Count() }); ;
        }
        public string DetalleAgenteModal(DateTime fecha)
        {
            var data = TraerDetalleAgente(fecha);
            data.ForEach(x => x.Cantidad = int.Parse(x.Cantidad).ToString("n0"));
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
    }
}
