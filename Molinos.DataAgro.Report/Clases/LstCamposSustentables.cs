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
    public class LstCamposSustentables
    {
        private readonly IReportesManager reportesManager;

        public LstCamposSustentables(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        public async Task<string> GenerarAsync(DeclaracionCampoSustentable oParam)
        {
            var oRptCamposSustentables = new RptCamposSustentables();
            oRptCamposSustentables.PageSettings.DefaultPaperSize = false;
            oRptCamposSustentables.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            oRptCamposSustentables.PageSettings.PaperName = "Mi Pagina";
            oRptCamposSustentables.Document.Printer.PrinterName = "";

            var oDatos = new List<DeclaracionCampoSustentable>
            {
                oParam
            };

            oRptCamposSustentables.Campos = oParam.Campos;
            oRptCamposSustentables.DataSource = oDatos;
            oRptCamposSustentables.Run(false);

            var oExportPDF = new PdfExport();
            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptCamposSustentables.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.RazonSocial.ToString() + " - " + oParam.CUIT.ToString() + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public Reportes Generar(DeclaracionCampoSustentable oParam)
        {
            var oReporte = new Reportes();
            var oRptCamposSustentables = new RptCamposSustentables();
            oRptCamposSustentables.PageSettings.DefaultPaperSize = false;
            oRptCamposSustentables.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            oRptCamposSustentables.PageSettings.PaperName = "Mi Pagina";
            oRptCamposSustentables.Document.Printer.PrinterName = "";

            var oDatos = new List<DeclaracionCampoSustentable>
            {
                oParam
            };

            oRptCamposSustentables.Campos = oParam.Campos;
            oRptCamposSustentables.DataSource = oDatos;
            oRptCamposSustentables.Run(false);

            var oExportPDF = new PdfExport();
            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptCamposSustentables.Document, ms);

                oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.RazonSocial.ToString() + " - " + oParam.CUIT.ToString() + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }
            return oReporte;
        }
    }
}