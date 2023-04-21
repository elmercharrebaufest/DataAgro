using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report
{
    public class LstIndicadores
    {

        private IReportesManager oReportesManager;

        public LstIndicadores(IReportesManager reportesManager)
        {
            this.oReportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------

        public async Task<string> GenerarListadoAsync(List<ContactoIni> oDatos)
        {
            var oRptContacto = new RptContacto();

            oRptContacto.DataSource = oDatos;

            oRptContacto.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptContacto.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Contactos.pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


        #region Exportacion Compras
        public async Task<string> GenerarComprasMapaExcelAsync(ResultComprasReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 11; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns-8].Value = "Razón Social";


            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };
                
                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarComprasBarraExcelAsync(ResultComprasBarrasReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 13; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns - 8].Value = "Razón Social";
                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };
                
                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarComprasMapaExcelAsync(ResultComprasTortaReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 11; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns - 8].Value = "Razón Social";
                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };
                
                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }
        #endregion


        public async Task<string> GenerarObjetivosGaugeExcelAsync(ResultObjetivoGaugeReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 13; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns - 10].Value = "Razón Social";
                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarObjetivosDBExcelAsync(ResultDBExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();


                

                

                /*if (oPropRow[7].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                {
                    workSheet.Column(7).Style.Numberformat.Format = "DD/MM/YYYY";

                }*/

                var cantColumns = oPropRow.Count();
                /*
                for (int i = 1; i < 9; i++)
                {
                    workSheet.Column(i).AutoFit();
                }
                */
                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };

                workSheet.Cells[ComienzaEn, cantColumns - 2].Value = "Razón Social";
                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


        #region Produccion
        public async Task<string> GenerarProduccionMapaExcelAsync(ResultProduccionMapaReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                workSheet.Cells[ComienzaEn, cantColumns - 4].Value = "Propia/Alquilada";
                workSheet.Cells[ComienzaEn, cantColumns - 3].Value = "Soja Sustentable";
                workSheet.Cells[ComienzaEn, cantColumns - 8].Value = "Razón Social";

                for (int i = 1; i < 11; i++)
                {
                    workSheet.Column(i).AutoFit();
                }


                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarProduccionBarraExcelAsync(ResultProduccionBarraReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                workSheet.Cells[ComienzaEn, cantColumns - 4].Value = "Propia/Alquilada";
                workSheet.Cells[ComienzaEn, cantColumns - 3].Value = "Soja Sustentable";
                workSheet.Cells[ComienzaEn, cantColumns - 8].Value = "Razón Social";

                for (int i = 1; i < 12; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }
        #endregion

        #region Acopio
        public async Task<string> GenerarAcopioMapaExcelAsync(ResultAcopioMapaReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 9; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns - 6].Value = "Razón Social";

                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarAcopioBarraExcelAsync(ResultAcopioBarraReportesExcel oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Lista.Count > 0)
            {

                var oColumnas = oDatos.Lista;

                var workSheet = excel.Workbook.Worksheets.Add(oDatos.Titulo);

                var milista = oDatos.oFiltros.Encontarvalidos().AsQueryable();
                int NumProp = milista.Count();
                int ComienzaEn = NumProp + 4;
                int Veces = 1;

                foreach (ExcelEncabezado OREP in milista)
                {

                    workSheet.Cells[Veces, 1].Value = OREP.NombreFiltro;
                    workSheet.Cells[Veces, 2].Value = OREP.Valor;
                    Veces = Veces + 1;
                }


                workSheet.Cells[ComienzaEn, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 10; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[ComienzaEn, cantColumns - 6].Value = "Razón Social";

                //workSheet.Cells[ComienzaEn, cantColumns].Value = "Toneladas";
                //workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Indicadores.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        } 
        #endregion

        public async Task<string> GenerarExcelExportAllAsync(ExportAll oDatos)
        {

            var excel = new ExcelPackage();

            var oColumnas = oDatos.Contacto;

            var workSheet = excel.Workbook.Worksheets.Add("Datos del Proveedor");

            workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

            var workSheet2 = excel.Workbook.Worksheets.Add("Objetivo");

            workSheet2.Cells[1, 1].LoadFromCollection(oDatos.Objetivo, true);

            var workSheet3 = excel.Workbook.Worksheets.Add("Datos de contacto");

            workSheet3.Cells[1, 1].LoadFromCollection(oDatos.ContactosPrincipales, true);

            var workSheet4 = excel.Workbook.Worksheets.Add("Produccion");

            workSheet4.Cells[1, 1].LoadFromCollection(oDatos.Produccion, true);

            var workSheet5 = excel.Workbook.Worksheets.Add("Almacenamiento");

            workSheet5.Cells[1, 1].LoadFromCollection(oDatos.Almacenamiento, true);

            var workSheet6 = excel.Workbook.Worksheets.Add("Agenda");

            workSheet6.Cells[1, 1].LoadFromCollection(oDatos.Agenda, true);

            var workSheet7 = excel.Workbook.Worksheets.Add("Compras");

            workSheet7.Cells[1, 1].LoadFromCollection(oDatos.Compras, true);

            var oPropRow = oColumnas.GetType().GetProperties();

            var cantColumns = oPropRow.Count();

            if (oColumnas.Count > 0)
            {
                oPropRow = oColumnas[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };
            }

            var j = 1;
            while (workSheet.Cells[1, j].Value != null)
            {
                workSheet.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet.Cells[1, j].Style.Font.Bold = true;

                j++;
            }


            workSheet.Cells[1, 1].Value = "CUIT";
            workSheet.Column(1).AutoFit();

            workSheet.Cells[1, 2].Value = "Razón Social";
            workSheet.Column(2).AutoFit();

            workSheet.Cells[1, 4].Value = "Calificación";
            workSheet.Column(4).AutoFit();

            workSheet.Cells[1, 5].Value = "Segmentación";
            workSheet.Column(5).AutoFit();

            workSheet.Cells[1, 6].Value = "Domicilio de Actividad";
            workSheet.Column(6).AutoFit();



            workSheet.Cells[1, 9].Value = "Código Postal";
            workSheet.Column(9).AutoFit();

            workSheet.Cells[1, 10].Value = "Canal de Operación";
            workSheet.Column(10).AutoFit();

            workSheet.Cells[1, 11].Value = "Entrega A";
            workSheet.Column(11).AutoFit();

            workSheet.Cells[1, 12].Value = "Condiciones Preferentes";
            workSheet.Column(11).AutoFit();

            workSheet.Cells[1, 14].Value = "Área de Influencia";
            workSheet.Column(13).AutoFit();

            workSheet.Cells[1, 17].Value = "Cliente MOA";
            workSheet.Column(16).AutoFit();

            workSheet.Cells[1, 19].Value = "Fecha de Alta";
            workSheet.Column(19).AutoFit();

            if (oDatos.Objetivo.Count > 0)
            {
                oPropRow = oDatos.Objetivo[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {

                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet2.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet2.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet2.Cells[1, j].Value != null)
            {
                workSheet2.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet2.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet2.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet2.Cells[1, 1].Value = "CUIT";
            workSheet2.Column(1).AutoFit();

            workSheet2.Cells[1, 2].Value = "Razón Social";
            workSheet2.Column(2).AutoFit();

            if (oDatos.ContactosPrincipales.Count > 0)
            {
                oPropRow = oDatos.ContactosPrincipales[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet3.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet3.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet3.Cells[1, j].Value != null)
            {
                workSheet3.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet3.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet3.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet3.Cells[1, 1].Value = "CUIT";
            workSheet3.Column(1).AutoFit();

            workSheet3.Cells[1, 8].Value = "Profesión";
            workSheet3.Column(7).AutoFit();

            workSheet3.Cells[1, 11].Value = "Otros Intereses";
            workSheet3.Column(10).AutoFit();

            workSheet3.Cells[1, 7].Value = "Fecha de Nacimiento";
            workSheet3.Column(6).AutoFit();

            workSheet3.Cells[1, 2].Value = "Razón Social";
            workSheet3.Column(2).AutoFit();

            if (oDatos.Produccion.Count > 0)
            {
                oPropRow = oDatos.Produccion[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet4.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet4.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet4.Cells[1, j].Value != null)
            {
                workSheet4.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet4.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet4.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet4.Cells[1, 1].Value = "CUIT";
            workSheet4.Column(1).AutoFit();

            workSheet4.Cells[1, 2].Value = "Razón Social";
            workSheet4.Column(2).AutoFit();

            if (oDatos.Almacenamiento.Count > 0)
            {

                oPropRow = oDatos.Almacenamiento[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {

                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet5.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet5.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet5.Cells[1, j].Value != null)
            {
                workSheet5.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet5.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet5.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet5.Cells[1, 1].Value = "CUIT";
            workSheet5.Column(1).AutoFit();

            workSheet5.Cells[1, 2].Value = "Razón Social";
            workSheet5.Column(2).AutoFit();

            workSheet5.Cells[1, 5].Value = "Campaña";
            workSheet5.Column(5).AutoFit();

            workSheet5.Cells[1, 6].Value = "Capacidad Planta Ton";
            workSheet5.Column(6).AutoFit();

            workSheet5.Cells[1, 12].Value = "Volumen Anual Ton";
            workSheet5.Column(12).AutoFit();

            workSheet5.Cells[1, 13].Value = "Habilitado Soja Sustentable";
            workSheet5.Column(13).AutoFit();

            if (oDatos.Agenda.Count > 0)
            {
                oPropRow = oDatos.Agenda[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (var i = 1; i <= cantColumns; i++)
                {

                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet6.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet6.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet6.Cells[1, j].Value != null)
            {
                workSheet6.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet6.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet6.Cells[1, j].Style.Font.Bold = true;

                j++;
            }


            workSheet6.Cells[1, 1].Value = "CUIT";
            workSheet6.Column(1).AutoFit();

            workSheet6.Cells[1, 2].Value = "Razón Social";
            workSheet6.Column(2).AutoFit();

            workSheet6.Cells[1, 3].Value = "Tipo de Actividad";
            workSheet6.Column(3).AutoFit();

            workSheet6.Cells[1, 4].Value = "Detalle";
            workSheet6.Column(4).AutoFit();

            workSheet6.Cells[1, cantColumns - 3].Value = "Fecha Desde";
            workSheet6.Column(cantColumns - 3).AutoFit();

            workSheet6.Cells[1, cantColumns - 2].Value = "Hora Desde";
            workSheet6.Column(cantColumns - 2).AutoFit();

            workSheet6.Cells[1, cantColumns - 1].Value = "Fecha Hasta";
            workSheet6.Column(cantColumns - 1).AutoFit();

            workSheet6.Cells[1, cantColumns].Value = "Hora Hasta";
            workSheet6.Column(cantColumns).AutoFit();

            if (oDatos.Compras.Count > 0)
            {
                oPropRow = oDatos.Compras[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet7.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet7.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet7.Cells[1, j].Value != null)
            {
                workSheet7.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet7.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet7.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet7.Cells[1, 1].Value = "CUIT";
            workSheet7.Column(1).AutoFit();

            workSheet7.Cells[1, 2].Value = "Razón Social";
            workSheet7.Column(2).AutoFit();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Contactos.xlsx",
                    Contenido = ms.ToArray()
                };

                oReportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }
    }
}



