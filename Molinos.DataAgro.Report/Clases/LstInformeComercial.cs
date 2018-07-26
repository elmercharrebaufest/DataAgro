using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report.Clases
{
    public class LstInformeComercial
    {
        private IReportesManager reportesManager;

        public LstInformeComercial(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------

        public async Task<string> GenerarListadoAsync(RptInformeComercialInfo oParam)
        {
            var oRptInformeComercial = new RptInformeComercial();
            oRptInformeComercial.PageSettings.DefaultPaperSize = false;
            oRptInformeComercial.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            oRptInformeComercial.PageSettings.PaperName = "Mi Pagina";
            oRptInformeComercial.Document.Printer.PrinterName = "";

            var oDatos = new List<RptInformeComercialInfo>();           
            var dato = new RptInformeComercialInfo();                              
            var oRptProduccionInfo = new List<InformeComercialAcopiadores>();
            var oRptAlmacenamientoInfo = new List<InformeComercialAcopiadores>();
            var oRptObjetivosInfo = new List<RptObjetivosInfo>();
            
            var ProduccionInfo = new InformeComercialAcopiadores();
            var AlmacenamientoInfo = new InformeComercialAcopiadores();

            oDatos.Add(oParam);           

            oRptProduccionInfo.AddRange(oParam.CapProduccion);
            
            oRptAlmacenamientoInfo.AddRange(oParam.CapAlmacenaje);

            oRptInformeComercial.CapProduccion = oRptProduccionInfo;

            oRptInformeComercial.CapAlmacenamiento = oRptAlmacenamientoInfo;

            oRptInformeComercial.DataSource = oDatos;

            oRptInformeComercial.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptInformeComercial.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.CUIT.ToString() + " - " + oParam.RazonSocial.ToString() + ".pdf",
                    Contenido = ms.ToArray()
                };
                
                await reportesManager.GrabarReporteAsync(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarInformesExcelAsync(List<ResultCapacidadProductiva> oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Count > 0)
            {

                var oColumnas = oDatos;

                var workSheet = excel.Workbook.Worksheets.Add("Capacidad productiva");

                workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                workSheet.Cells[1, 1].Value = "Proveedor";
                workSheet.Cells[1, 2].Value = "Capacidad productiva Soja";
                workSheet.Cells[1, 3].Value = "Capacidad productiva Maiz";
                workSheet.Cells[1, 4].Value = "Capacidad Productiva Trigo";

                for (int i = 1; i < 5; i++)
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
                    FileName = "CapacidadProductiva.xlsx",
                    Contenido = ms.ToArray()
                };
                
                await reportesManager.GrabarReporteAsync(oReporte);
            }

            return identif;
        }

        public async Task<string> GenerarInformesExcelICAsync(List<ReportesList> oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Count > 0)
            {

                var oColumnas = oDatos;

                var workSheet = excel.Workbook.Worksheets.Add("Reporte Comercial");

                workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i < 4; i++)
                {
                    workSheet.Column(i).AutoFit();
                }

                workSheet.Cells[1, 1].Value = "Cuit";
                workSheet.Cells[1, 2].Value = "Razón Social";
                workSheet.Cells[1, 3].Value = "Fecha de Alta";
                workSheet.Cells[1, 4].Value = "Comercial";
                workSheet.Cells[1, 5].Value = "Estado";
                workSheet.Cells[1, 6].Value = "Material";
                workSheet.Cells[1, 7].Value = "Observaciones";

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };
            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "ReporteComercial.xlsx",
                    Contenido = ms.ToArray()
                };
                
                await reportesManager.GrabarReporteAsync(oReporte);
            }

            return identif;
        }


    }
}
