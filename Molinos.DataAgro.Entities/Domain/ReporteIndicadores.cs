
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

using Mastersoft.Framework.Standard;
using System.Reflection;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Molinos.DataAgro.Entities
{




    public enum LosMesesDelAño
    {
        Enero = 1,
        Febrero ,
        Marzo , Abril,Mayo,Junio,Julio,Agosto,Septiembre,Octubre,Noviembre,Diciembre
    }

    public class ResultComprasReportesEXCEL
    {


    }

    public class ExcelEncabezado
    {
       public  string NombreFiltro { get; set; }
       public  string Valor { get; set; }
    }


    public class ParamReportesAuxiliar
    {

        public string Indicadores { get; set; }
        public string Grafico { get; set; }
       
        public string Grano { get; set; }
        public string Mes { get; set; }
        public string Provincia { get; set; }
        public string Cosecha { get; set; }
        public string Segmentacion { get; set; }
        public string Comercial { get; set; }
        public string Objetivos { get; set; }
        public Double? Toneladas { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
    }


    public class valoresGrilla
    {
        public string Cuit { get; set; }
        public string Segmentación { get; set; }
        public int? Toneladas { get; set; }
        public string Estado { get; set; }
        public string Grano { get; set; }
        public string RazonSocial { get; set; }
        public string Comercial { get; set; }
        public DateTime FechaAlta { get; set; }
    }

    public class graficoBaseDatos
    {
        public string segmentacion { get; set; }
        public int? Toneladas { get; set; }
    }

    public class BaseDeDatosReturn
    {
        public List<valoresGrilla> valoresGrilla { get; set; }
        public List<graficoBaseDatos> graficoBaseDatos { get; set; }
    }

    public class ParamReportes
    {

        public string Indicadores { get; set; }
        public string Grafico { get; set; }
        public int? Grano { get; set; }
        public int? Mes { get; set; }
        public int? Provincia { get; set; }
        public int? Cosecha { get; set; }
        public string Segmentacion { get; set; }
        public int? Comercial { get; set; }
        public int? Objetivos { get; set; }
        public string GRANO_ { get; set; }
        public string MES_ { get; set; }
        public string PROVINCIA_ { get; set; }
        public string COSECHA_ { get; set; }
        public string SEGMENTACION_ { get; set; }
        public string COMERCIAL_ { get; set; }
        public string OBJETIVOS_ { get; set; }
        public Double? Toneladas { get; set; }
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string TONELADAS_ { get; set; }
        public string FECHADESDE_ { get; set; }
        public string FECHAHASTA_ { get; set; }
        public int ComercialActual { get; set; }
        public string COMERCIALACTUAL_ { get; set; }


        public List<ExcelEncabezado> Encontarvalidos()

        {
            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            List<ExcelEncabezado> resultado = new List<ExcelEncabezado>();

          

            foreach (var prop in properties)
            {
                var nombre = prop.Name;
                var value = prop.GetValue(this, null);
                var tipo = prop.PropertyType.FullName.ToString();


                if (tipo.ToString() == "System.String" && nombre.ToLower() == "indicadores")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = "Indicadores",
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }
                else if (tipo.ToString() == "System.String" && nombre.ToLower() == "segmentacion_")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = "Segmentación",
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }
                else if (tipo.ToString() == "System.String" && nombre.ToLower() == "segmentacion_")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = "Segmentación",
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }
                else if (tipo.ToString() == "System.String" && nombre.ToLower() == "fechadesde_")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = "Fecha Desde",
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }
                else if (tipo.ToString() == "System.String" && nombre.ToLower() == "fechahasta_")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = "Fecha Hasta",
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }
                else if (nombre.ToLower() == "grafico" && value.ToString()== ("gaudge"))
                {
                    resultado.Add(new ExcelEncabezado()
                    {
                        NombreFiltro = nombre.ToTitleCase(),
                        Valor = "Objetivos por CUIT",

                    });
                }
                else if (tipo.ToString() == "System.String" && nombre != "Segmentacion" && nombre != "COMERCIALACTUAL_")
                {
                    if (value != null)
                    {
                        if (value.ToString() != "")
                        {
                            resultado.Add(new ExcelEncabezado()
                            {
                                NombreFiltro = prop.Name.ToString().Replace("_", "").ToTitleCase(),
                                Valor = value.ToString().ToTitleCase()

                            });
                        }
                    }
                }

                

                if (tipo.ToString().Contains ("Double") && nombre != "Toneladas")
                {
                    if (value != null)
                    {
                        resultado.Add(new ExcelEncabezado()
                        {
                            NombreFiltro = prop.Name.ToString().ToTitleCase(),
                                Valor = value.ToString().ToTitleCase()

                        });
                    }
                }


               


            }

           var upt =  resultado.ToList().Where(c=>c.Valor!="mapa" && c.Valor != "barra" && c.Valor != "torta" && c.Valor != "gauge").ToList();
            return resultado;
        }

        public int GetCountProp()
        {

            var properties = this.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            return properties.Count();
        }

        

    }


    public static class Extensores
    {
        public static string ToTitleCase(this string s)
        {

            //return Regex.Replace(s.ToLower(), @"(^\w)|(\s\w)", m => m.Value.ToUpper());
            return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower());
        }
    }


    public class ResultComprasReportesExcel 
    {
       public  List<ResultIndicadoresReportesmini> Lista { get; set; }
       public  string Titulo { get; set; }
       public ParamReportes oFiltros { get; set; }
      
    }


    public class ResultIndicadoresReportesmini
    {
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public Double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Mes { get; set; }
        public string Año { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }

        public ResultIndicadoresReportesmini()
        {
            Toneladas = 0;
        }
    }

    public class ResulIndicadores
    {
        public int? Cuit { get; set; }
        public Double? Tonelada { get; set; }
        public int? Segmentacion { get; set; }
        public int? Estado { get; set; }
        public int? Grano { get; set; }
        public string Provincia { get; set; }
        public int? CantidadDeCliente { get; set; }

        public ResulIndicadores()
        {
            Tonelada = 0;
            CantidadDeCliente = 0;
        }

    }

    public class ResultComprasExportacionReportes
    {
        public string Cuit { get; set; }
        public Double? Tonelada { get; set; }
         

        public ResultComprasExportacionReportes()
        {
            Tonelada = 0;
        }

    }

    #region Compras Barras

    public class ResultComprasBarrasReportes
    {
        public Double? MasTn5000 { get; set; }
        public Double? MasTn1000 { get; set; }
        public Double? MasTn10000 { get; set; }
        public Double? MasTn20000 { get; set; }
        public Double? MasTn40000 { get; set; }
        public Double? MasTn100 { get; set; }
        public Double? MenosTn10000 { get; set; }
        public int? MasCl5000 { get; set; }
        public int? MasCl1000 { get; set; }
        public int? MasCl10000 { get; set; }
        public int? MasCl100 { get; set; }
        public int? MenosCl10000 { get; set; }
        public int? MasCl20000 { get; set; }
        public int? MasCl40000 { get; set; }



        public ResultComprasBarrasReportes()
        {
            MasTn5000 = 0;
            MasTn1000 = 0;
            MasTn100 = 0;
            MasTn10000 = 0;
            MasTn20000 = 0;
            MasTn40000 = 0;
            MenosTn10000 = 0;
            MasCl5000 = 0;
            MasCl1000 = 0;
            MasCl100 = 0;
            MenosCl10000 = 0;
            MasCl10000 = 0;
            MasCl20000 = 0;
            MasCl40000 = 0;
        }

    }

    public class ResultComprasBarrasReportesExcel
    {
        public List<ResultComprasBarrasReportesmini> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    public class ResultComprasBarrasReportesmini
    {
        public string Criterio { get; set; }
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public Double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Mes { get; set; }
        public string Año { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }


        public ResultComprasBarrasReportesmini()
        {
            Toneladas = 0;
        }



    }

    #endregion

    #region Compras Tortas
    public class ResultComprasTortaReportesExcel
    {
        public List<ResultIndicadoresReportesTorta> Lista { get; set; }
        public string Titulo { get; set; }
        public ParamReportes oFiltros { get; set; }

    }


    public class ResultIndicadoresReportesTorta
    {
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public Double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Mes { get; set; }
        public string Año { get; set; }
        public string Comercial { get; set; }
        public string Segmentación { get; set; }
        public string Provincia { get; set; }

        public ResultIndicadoresReportesTorta()
        {
            Toneladas = 0;
        }
    } 
    #endregion

    #region Objetivo Gauget

    public class ResultObjetivoGaugeReportes
    {
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public Double? Objetivos { get; set; }
        public Double? Toneladas { get; set; }
        public Double? Porcentajes { get; set; }
       
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Mes { get; set; }
        public string Año { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }
        public ResultObjetivoGaugeReportes()
        {
            Porcentajes = 0;
            Toneladas = 0;
            Objetivos = 0;
        }
    }

    public class ResultObjetivoGaugeReportesExcel
    {
        public List<ResultObjetivoGaugeReportes> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion

    #region DB
    

    public class ResultDBExcel
    {
        public List<valoresGrilla> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion

    #region Produccion Mapa

    public class ResultProduccionMapaReportes
    {
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public int? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string PropiaAlquilada { get; set; }
        public string SojaSustentable { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }
        public ResultProduccionMapaReportes()
        {
            Toneladas = 0;
        }
    }

    public class ResultProduccionMapaReportesExcel
    {
        public List<ResultProduccionMapaReportes> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion

    #region Produccion Barra

    public class ResultProduccionBarraReportes
    {
        public string Criterio { get; set; }
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string PropiaAlquilada { get; set; }
        public string SojaSustentable { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }
        public ResultProduccionBarraReportes()
        {
            Toneladas = 0;
        }
    }

    public class ResultProduccionBarraReportesExcel
    {
        public List<ResultProduccionBarraReportes> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion


    #region Acopio Mapa

    public class ResultAcopioMapaReportes
    {
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }
        public ResultAcopioMapaReportes()
        {
            Toneladas = 0;
        }
    }

    public class ResultAcopioMapaReportesExcel
    {
        public List<ResultAcopioMapaReportes> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion

    #region Acopio Barra

    public class ResultAcopioBarraReportes
    {
        public string Criterio { get; set; }
        public string Cuit { get; set; }
        public string razonSocial { get; set; }
        public double? Toneladas { get; set; }
        public string Material { get; set; }
        public string Campaña { get; set; }
        public string Comercial { get; set; }
        public string Provincia { get; set; }
        public string Segmentación { get; set; }
        public ResultAcopioBarraReportes()
        {
            Toneladas = 0;
        }
    }

    public class ResultAcopioBarraReportesExcel
    {
        public List<ResultAcopioBarraReportes> Lista { get; set; }
        public string Titulo = "";
        public ParamReportes oFiltros { get; set; }
    }

    #endregion

}




