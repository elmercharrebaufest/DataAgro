using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report.Clases
{
    public class LstFormularioAltaNoGranos
    {
        private readonly IReportesManager reportesManager;

        public LstFormularioAltaNoGranos(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        public async Task<string> GenerarAsync(ProveedorAltaDto oParam)
        {
            var report = new RptFormularioAltaNoGranos();
            report.PageSettings.DefaultPaperSize = false;
            report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            report.PageSettings.PaperName = "Mi Pagina";
            report.Document.Printer.PrinterName = "";

            var oDatos = new List<ProveedorAltaDto>
            {
                oParam
            };

            report.DataSource = oDatos;
            report.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(report.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.CUIT + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public Reportes Generar(ProveedorAltaDto oParam)
        {
            var oReporte = new Reportes();
            var report = new RptFormularioAltaNoGranos();
            report.PageSettings.DefaultPaperSize = false;
            report.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            report.PageSettings.PaperName = "Mi Pagina";
            report.Document.Printer.PrinterName = "";

            var oDatos = new List<ProveedorAltaDto>
            {
                oParam
            };

            report.DataSource = oDatos;
            report.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(report.Document, ms);

                oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.CUIT + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return oReporte;
        }
    }
}