using Autofac.Extras.NLog;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Helpers;
using Molinos.DataAgro.Entities.Resources;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Repository;
using Molinos.DataAgro.Repository.ConsultasEF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Molinos.DataAgro.Business.Managers
{
    public class ReportesManager : IReportesManager
    {
        private readonly IRepositorio repositorio;
        private ILogger logger;

        public ReportesManager(ILogger logger, IRepositorio repositorio, IComercialManager oComercial)
        {
            this.logger = logger;
            this.repositorio = repositorio;
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
            return repositorio.Obtener<Reportes,ReportesDto>(x=>x.Identificador == identificador, x=> new ReportesDto { Identificador= x.Identificador, Contenido=x.Contenido, FileName=x.FileName});
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

            var query = repositorio.SelStore<FakeHome>("DataAgro_Comercial_TraerPorComerciales", 0, ComercialId);
            Datos.come = query.ToList().Select(s => new ComercialQry() { ComercialId = s.Id, IdActiveDirectory = s.Nombre }).ToList();

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
                var value = prop.GetValue(oParamReportes,null);

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
                    oParamReportes.PROVINCIA_ = repositorio.Obtener<Provincia, string>(x => x.ProvinciaId== oParamReportes.Provincia, x => x.Nombre);
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
                    oParamReportes.OBJETIVOS_ = repositorio.Obtener<Objetivo, string>(x => x.ObjetivoId== oParamReportes.Comercial, x => x.ToneladasObjetivos.ToString() + " Toneladas ");
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

        public List<ToneladasGranoTipoDto> TraerToneladasGranoTipo(DateTime fecha)
        {
            var sojaToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(3, fecha));
            sojaToneladas.Material = "Soja";
            var maizToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(1, fecha));
            maizToneladas.Material = "Maiz";
            var trigoCamaraToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fecha, false));
            trigoCamaraToneladas.Material = "Trigo Cámara";
            var trigoCalidadToneladas = repositorio.ObtenerConsultaEscalar(new TraerToneladasPorGrano(2, fecha, true));
            trigoCalidadToneladas.Material = "Trigo Calidad";

            return new List<ToneladasGranoTipoDto>() { sojaToneladas, maizToneladas, trigoCamaraToneladas, trigoCalidadToneladas };

        }
        public ReporteSojaSustDto TraerToneladasSojaSust(DateTime fecha)
        {
            return repositorio.ObtenerConsultaEscalar(new TraerToneladasSojaSustentable(fecha));
        }
        public List<PosicionComprasDto> TraerPosicionCompras(DateTime fecha)
        {
            var kilosPosicionSoja = repositorio.ListarConsulta(new TraerPosicionMaterial(3, fecha));
            var posicionSoja = new PosicionComprasDto
            {
                Material = "Soja",
                MaterialId = 3,
                PosicionKilos = kilosPosicionSoja,
                Total = kilosPosicionSoja.Sum(x=>x.Kilos)
            };
            var kilosPosicionMaiz = repositorio.ListarConsulta(new TraerPosicionMaterial(1, fecha));
            var posicionMaiz = new PosicionComprasDto
            {
                Material = "Maíz",
                MaterialId = 1,
                PosicionKilos = kilosPosicionMaiz,
                Total = kilosPosicionMaiz.Sum(x => x.Kilos)
            };
            var kilosPosicionTrigoCamara = repositorio.ListarConsulta(new TraerPosicionMaterial(2, fecha, false));
            var posicionTrigoCamara = new PosicionComprasDto
            {
                Material = "Trigo Cámara",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCamara,
                Total = kilosPosicionTrigoCamara.Sum(x => x.Kilos)
            };
            var kilosPosicionTrigoCalidad = repositorio.ListarConsulta(new TraerPosicionMaterial(2, fecha, true));
            var posicionTrigoCalidad = new PosicionComprasDto
            {
                Material = "Trigo Calidad",
                MaterialId = 2,
                PosicionKilos = kilosPosicionTrigoCalidad,
                Total = kilosPosicionTrigoCalidad.Sum(x => x.Kilos)
            };
            return new List<PosicionComprasDto> { posicionSoja, posicionMaiz, posicionTrigoCamara, posicionTrigoCalidad };
        }
        public List<PrecioCantidadDto> TraerMonedaCantidad(DateTime fecha)
        {
            var moneda = repositorio.ListarConsulta(new TraerMonedaKilo(fecha));

            return new List<PrecioCantidadDto>() { new PrecioCantidadDto {Moneda = "Pesos" , Cantidad= moneda.Exists(x=>x.Moneda == "ARP  ")?moneda.Where(x=>x.Moneda== "ARP  ").Select(x=>x.Cantidad).First():0},
                new PrecioCantidadDto {Moneda = "Dólares" , Cantidad=moneda.Exists(x=>x.Moneda == "USDM ")? moneda.Where(x=>x.Moneda== "USDM ").Select(x=>x.Cantidad).First():0}};            
        }
        public ExcelDetallePosicionDto DetallePosicion(int materialId, int mes,DateTime fecha, bool? calidad)
        {
            var excel = new ExcelDetallePosicionDto();
            excel.Headers = typeof(DetalleContratoDto).GetProperties().Select(p =>Text.ResourceManager.GetString(p.Name)).ToArray();
            excel.Data = repositorio.ListarConsulta(new TraerDetallePosicion(materialId, mes, fecha, calidad));
            excel.Name = "Detalle Posición de Negocios de " + (EnumMeses)Enum.ToObject(typeof(EnumMeses), mes)+".xlsx";
            excel.SheetName = "Posición";
            return excel;
        }

        public List<ExcelPosicionMaterialDto> PosicionPorMaterial( DateTime fecha)
        {            
            return repositorio.ListarConsulta(new TraerPosicionMaterialMes(fecha));
        }
    }
}
