using Autofac.Extras.NLog;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Mapping.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Business.Managers
{
    public class ReportesManager : IReportesManager
    {
        private IUnitOfWorkAsync mobjUnitOfWork;
        private ILogger logger;

        public ReportesManager(ILogger logger, IMSContextProvider oMSContextProvider, IComercialManager oComercial, ICampañaMaterial oCampañaMaterial)
        {
            this.logger = logger;
            mobjUnitOfWork = new UnitOfWork(oMSContextProvider.GetMSContext(), new DataAgroContext(oMSContextProvider.GetMSContext()));
        }


        //--------------------------------------------------
        //  Metodos Publicos
        //--------------------------------------------------

        public async Task<EntityErrors> GrabarReporteAsync(Reportes oReporte)
        {
            var oEntityErrors = new EntityErrors();

            var oRepository = mobjUnitOfWork.Repository<Reportes>();

            oRepository.Insert(oReporte);

            await mobjUnitOfWork.SaveChangesAsync();

            return oEntityErrors;
        }

        /*public void Inicializar(MSContext oContexto)
        {
            throw new NotImplementedException();
        }

        public void Inicializar(MSContext oContexto, IUnitOfWorkAsync oUnitOfWork)
        {
            throw new NotImplementedException();
        }*/

        public async Task<Reportes> ObtenerReporteAsync(string identificador)
        {
            var oReportes = mobjUnitOfWork.Repository<Reportes>().Queryable();

            var query = oReportes
                        .Where(x => x.Identificador == identificador);

            var result = await query.FirstOrDefaultAsync();
            
            return result;
        }

        public async Task<DatosInicialesReportes> TraerDatosIniciales(string idActiveDirectory)
        {


            int ComercialId = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking().FirstOrDefault(x => x.IdActiveDirectory == idActiveDirectory).ComercialId;

            DatosInicialesReportes Datos = new DatosInicialesReportes();
            Datos.camp = await mobjUnitOfWork.Repository<Campaña>().
                                Queryable().AsNoTracking()
                                .OrderByDescending(x=> x.CampañaId)
                                .Select(x=> new CampañaQry {
                                    CampañaId = x.CampañaId,
                                    Descripcion = x.Descripcion
                                }).Take(3)
                                .ToListAsync();
            /*
            Datos.come = await mobjUnitOfWork.Repository<Comercial>().
                                Queryable().AsNoTracking().
                                Select(x => new ComercialQry
                                {
                                    ComercialId = x.ComercialId,
                                    IdActiveDirectory = x.IdActiveDirectory
                                }).ToListAsync();
            */
            var query = mobjUnitOfWork.SelStore<FakeHome>("DataAgro_Comercial_TraerPorComerciales", ComercialId);
            Datos.come = query.ToList().Select(s => new ComercialQry() { ComercialId = s.Id, IdActiveDirectory = s.Nombre }).ToList();

            Datos.mat = await mobjUnitOfWork.Repository<Material>().
                                Queryable().AsNoTracking().
                                Select(x => new MaterialesQry
                                {
                                    Descripcion = x.Descripcion,
                                    MaterialId = x.MaterialId
                                }).ToListAsync();

            Datos.provs = await mobjUnitOfWork.Repository<Provincia>().
                                Queryable().AsNoTracking().
                                Select(x => new ProvinciaQry
                                {
                                    Nombre = x.Nombre,
                                    Provinciaid = x.ProvinciaId
                                }).ToListAsync();

            Datos.segm = await mobjUnitOfWork.Repository<Segmentacion>().
                                Queryable().AsNoTracking().
                                Select(x => new SegmentacionQry
                                {
                                    Descripcion = x.Descripcion,
                                    Grupo = x.Grupo,
                                    SegmentacionId = x.SegmentacionId
                                }).ToListAsync();

            Datos.estic = await mobjUnitOfWork.Repository<InformeComercialEstado>().
                                Queryable().AsNoTracking().
                                Select(x => new EstadoICQry
                                {
                                    Descripcion = x.Descripcion,
                                    EstadoInformeId = x.EstadoInformeId
                                }).ToListAsync();


            return Datos;
        }

        #region Compras

        #region Mapa
        public async Task<List<ResulIndicadores>> TraerComprasMapa(ParamReportes oParamReportes)

        {
            var actividadhistoria = await mobjUnitOfWork.SelStoreAsync<ResulIndicadores>("DataAgro_IndicadoresComprasMapa", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();

            return (actividadhistoria);


        }

        public async Task<List<ResultIndicadoresReportesmini>> TraerComprasMapaExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultIndicadoresReportesmini>("DataAgro_IndicadoresComprasExportacionMapa", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return  resultados;
        }
        #endregion

        #region Torta
        public async Task<List<ResulIndicadores>> TraerComprasTorta(ParamReportes oParamReportes)
        {
            var actividadhistoria = await mobjUnitOfWork.SelStoreAsync<ResulIndicadores>("DataAgro_IndicadoresComprasTorta", oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (actividadhistoria);
        }


        public async Task<List<ResultIndicadoresReportesTorta>> TraerComprasTortaExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultIndicadoresReportesTorta>("DataAgro_IndicadoresComprasExportacionTorta", oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();

            return (resultados);

        }
        #endregion

        #region Barra
        public async Task<List<ResultComprasBarrasReportes>> TraerComprasBarra(ParamReportes oParamReportes)
        {
            var comprasBarra = await mobjUnitOfWork.SelStoreAsync<ResultComprasBarrasReportes>("Reporte_ComprasBarra_Traer", oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();

            return comprasBarra;
        }

        public async Task<List<ResultComprasBarrasReportesmini>> TraerComprasBarraExportacion(ParamReportes oParamReportes)
        {

            var comprasBarra = await mobjUnitOfWork.SelStoreAsync<ResultComprasBarrasReportesmini>("Reporte_ComprasBarra_TraerExcel", oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();

            return comprasBarra;

        }
        #endregion

        #endregion

        #region Productiva

        #region Mapa
        public async Task<List<ResulIndicadores>> TraerCapacidadProductivaMapa(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResulIndicadores>("DataAgro_IndicadoresProductiva", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }

        public async Task<List<ResultProduccionMapaReportes>> TraerCapacidadProductivaMapaExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultProduccionMapaReportes>("DataAgro_IndicadoresExportacionMapaProductiva", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }
        #endregion



        #region Barras
        public async Task<List<ResultComprasBarrasReportes>> TraerCapacidadProductivaBarra(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultComprasBarrasReportes>("DataAgro_IndicadoresProductivaBarra", oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();

            return (resultados);
        }

        public async Task<List<ResultProduccionBarraReportes>> TraerCapacidadProductivaBarraExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultProduccionBarraReportes>("DataAgro_IndicadoresExportacionBarraProductiva", oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }
        #endregion

        #endregion

        #region Acopio
        #region Mapa
        public async Task<List<ResulIndicadores>> TraerCapacidadDeAcopioMapa(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResulIndicadores>("DataAgro_IndicadoresAcopio", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }

        public async Task<List<ResultAcopioMapaReportes>> TraerCapacidadDeAcopioMapaExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultAcopioMapaReportes>("DataAgro_IndicadoresExportacionMapaAcopio", oParamReportes.Provincia, oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial,oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }
        #endregion

        #region Barra
        public async Task<List<ResultComprasBarrasReportes>> TraerCapacidadDeAcopioBarra(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultComprasBarrasReportes>("DataAgro_IndicadoresAcopioBarra", oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }

        public async Task<List<ResultAcopioBarraReportes>> TraerCapacidadDeAcopioBarraExportacion(ParamReportes oParamReportes)
        {
            var resultados = await mobjUnitOfWork.SelStoreAsync<ResultAcopioBarraReportes>("DataAgro_IndicadoresExportacionBarraAcopio", oParamReportes.Segmentacion, oParamReportes.Grano, oParamReportes.Cosecha, oParamReportes.Comercial, oParamReportes.ComercialActual).ToListAsync();
            return (resultados);
        }
        #endregion 
        #endregion


        #region Objetivos
        public async Task<List<ResultObjetivoGaugeReportes>> TraerObjetivosGauge(ParamReportes oParamReportes)
        {
            var GaugeReporte = await mobjUnitOfWork.SelStoreAsync<ResultObjetivoGaugeReportes>("DataAgro_Gauget_Traer", oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Cosecha, oParamReportes.Grano).ToListAsync();
            return GaugeReporte;
        }

        public async Task<List<ResultObjetivoGaugeReportes>> TraerObjetivosGaugeExportacion(ParamReportes oParamReportes)
        {

            var comprasBarra = await mobjUnitOfWork.SelStoreAsync<ResultObjetivoGaugeReportes>("DataAgro_Gauget_TraerExcel", oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Cosecha, oParamReportes.Grano).ToListAsync();

            return comprasBarra;

        } 
        #endregion

        public async Task<BaseDeDatosReturn> TraerDatosGrillaBD(ParamReportes oParamReportes)
        {

            BaseDeDatosReturn datosGrilla = new BaseDeDatosReturn();

            datosGrilla.valoresGrilla = await mobjUnitOfWork.SelStoreAsync<valoresGrilla>("DataAgro_IndicadoresBaseDeDatos_Traer", oParamReportes.FechaDesde, oParamReportes.FechaHasta, oParamReportes.Mes, oParamReportes.Segmentacion, oParamReportes.Cosecha, oParamReportes.Toneladas, oParamReportes.Comercial, oParamReportes.ComercialActual, oParamReportes.Grano).ToListAsync();

            datosGrilla.graficoBaseDatos = datosGrilla.valoresGrilla.GroupBy(x => x.Segmentación).Select(x => new graficoBaseDatos()
            {
                segmentacion = x.Key,
                Toneladas = 0   //x.Sum(y => y.Toneladas)
            }).ToList();

            datosGrilla.graficoBaseDatos.ForEach(x => x.Toneladas= datosGrilla.valoresGrilla.Where(y => y.Segmentación == x.segmentacion).Select(c => c.Cuit).Distinct().Count());

            datosGrilla.graficoBaseDatos = datosGrilla.graficoBaseDatos.OrderByDescending(x => x.Toneladas).ToList();

            return datosGrilla;
        }


        //BASEDEDATOS

        public async Task<List<ResulIndicadores>> TraerBasedeDatos(ParamReportes oParamReportes)
        {
            return new List<ResulIndicadores>();
        }

        public async Task<List<ResultIndicadoresReportesmini>> TraerBaseDeDatosExportacion(ParamReportes oParamReportes)
        {
            var provincia = mobjUnitOfWork.Repository<Provincia>().Queryable().AsNoTracking();
            var proveedorcomercial = mobjUnitOfWork.Repository<ProveedorComercial>().Queryable().AsNoTracking();
            var campanamaterial = mobjUnitOfWork.Repository<CampañaMaterial>().Queryable().AsNoTracking();
            var campanamaterialxMes = mobjUnitOfWork.Repository<CampañaMaterialPorMes>().Queryable().AsNoTracking();
            var acopiocampana = mobjUnitOfWork.Repository<AcopioCampaña>().Queryable().AsNoTracking();
            var proveedor = mobjUnitOfWork.Repository<Proveedor>().Queryable().AsNoTracking();
            var acopio = mobjUnitOfWork.Repository<Acopio>().Queryable().AsNoTracking();
            var datos = await proveedor

                .GroupJoin(provincia, x => x.ProvinciaId, y => y.ProvinciaId, (x, y) => new { x.FechaAlta, x.SegmentacionId, x.CUIT, x.ProveedorId, NombreProvincia = y.FirstOrDefault().Nombre, y.FirstOrDefault().ProvinciaId })
                .GroupJoin(campanamaterial, x => x.ProveedorId, y => y.ProveedorId, (x, y) => new { x.FechaAlta, x.SegmentacionId, y.FirstOrDefault().CampañaId, x.ProveedorId, x.NombreProvincia, x.CUIT, x.ProvinciaId, y.FirstOrDefault().CampañaMaterialId, y.FirstOrDefault().MaterialId, y.FirstOrDefault().ToneladasCompradas })
                .GroupJoin(campanamaterialxMes, x => x.CampañaMaterialId, y => y.CampañaMaterialId, (x, y) => new { x.FechaAlta, x.MaterialId, y.FirstOrDefault().Mes, x.SegmentacionId, x.CampañaId, x.ProveedorId, x.NombreProvincia, x.ProvinciaId, x.CUIT, y.FirstOrDefault().Toneladas })
                .GroupJoin(proveedorcomercial, x => x.ProveedorId, y => y.ProveedorId, (x, y) => new { x.FechaAlta, x.MaterialId, x.Mes, x.SegmentacionId, x.CampañaId, x.ProveedorId, x.NombreProvincia, x.ProvinciaId, x.CUIT, x.Toneladas, y.FirstOrDefault().ComercialId })



                .Where(x => (oParamReportes.Grano == x.MaterialId || oParamReportes.Grano == null)
                 && (oParamReportes.Cosecha == x.CampañaId || oParamReportes.Cosecha == null)
                 && (oParamReportes.Provincia == x.ProvinciaId || oParamReportes.Provincia == null)
                 //Todo Segmentacion
                 //&& (oParamReportes.Segmentacion == x.SegmentacionId || oParamReportes.Segmentacion == null)
                 && (oParamReportes.FechaDesde <= x.FechaAlta && oParamReportes.FechaHasta >= x.FechaAlta)
                 && (oParamReportes.Comercial == x.ComercialId || oParamReportes.Comercial == null)
                  && (oParamReportes.Mes == x.Mes || oParamReportes.Mes == null)
                 )


                //.GroupBy(grp => grp.ProvinciaId)




                .Select(X => new ResultIndicadoresReportesmini
                {

                    Cuit = X.CUIT,
                    Toneladas = X.Toneladas




                }).ToListAsync();



            var m = datos;




            return (datos);







        }





        public ParamReportes TransformarFiltros(ParamReportes oParamReportes)
        {
            var properties = oParamReportes.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<ExcelEncabezado> resultado = new List<ExcelEncabezado>();


            var modelgrano = mobjUnitOfWork.Repository<Material>().Queryable().AsNoTracking();
            var modelProvincia = mobjUnitOfWork.Repository<Provincia>().Queryable().AsNoTracking();
            var modelCosecha = mobjUnitOfWork.Repository<Campaña>().Queryable().AsNoTracking();
            var modelSegmentacion = mobjUnitOfWork.Repository<Segmentacion>().Queryable().AsNoTracking();
            var modelComercial = mobjUnitOfWork.Repository<Comercial>().Queryable().AsNoTracking();
            var modelObjetivos = mobjUnitOfWork.Repository<Objetivo>().Queryable().AsNoTracking();




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
                      oParamReportes.GRANO_ = modelgrano.Where(x => x.MaterialId == oParamReportes.Grano).Select(x => x.Descripcion).FirstOrDefault();
                }


                if (nombre.ToLower() == "provincia" && value != null)
                {
                    oParamReportes.PROVINCIA_ = modelProvincia.Where(x => x.ProvinciaId== oParamReportes.Provincia).Select(x => x.Nombre).FirstOrDefault();
                }


                if (nombre.ToLower() == "cosecha" && value != null)
                {
                    oParamReportes.COSECHA_ = modelCosecha.Where(x => x.CampañaId == oParamReportes.Cosecha).Select(x => x.Descripcion).FirstOrDefault();
                }

                if (nombre.ToLower() == "segmentacion" && value != null)
                {
                    var V1 = oParamReportes.Segmentacion.Split(',');
                    var segmenta = V1.ToList().Select(x => int.Parse(x)).ToList();
                    if (!segmenta.Any(x => x == 0))
                        oParamReportes.SEGMENTACION_ = string.Join(",", modelSegmentacion.Where(x => segmenta.Contains(x.SegmentacionId)).Select(x => x.Descripcion));
                }

                if (nombre.ToLower() == "objetivos" && value != null)
                {
                    oParamReportes.OBJETIVOS_ = modelObjetivos.Where(x => x.ObjetivoId== oParamReportes.Comercial).Select(x => x.ToneladasObjetivos.ToString() + " Toneladas ").FirstOrDefault();
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
                    oParamReportes.COMERCIAL_ = modelComercial.Where(x => x.ComercialId == oParamReportes.Comercial).Select(x => x.Nombres + " " + x.Apellido).FirstOrDefault();
                }

                if (nombre.ToLower() == "comercialactual" && value != null)
                {
                    oParamReportes.COMERCIALACTUAL_ = modelComercial.Where(x => x.ComercialId == oParamReportes.ComercialActual).Select(x => x.Nombres + " " + x.Apellido).FirstOrDefault();
                }

            }


            return oParamReportes;

         
        }

       
     
    }
}
