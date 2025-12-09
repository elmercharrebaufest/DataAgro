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
    public class LstCartaDePresentacion
    {
        private readonly IReportesManager reportesManager;

        public LstCartaDePresentacion(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        public async Task<string> GenerarAsync(RptCartaDePresentacionInfo oParam)
        {
            var oRptInformeComercial = new RptCartaDePresentacion();
            oRptInformeComercial.PageSettings.DefaultPaperSize = false;
            oRptInformeComercial.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            oRptInformeComercial.PageSettings.PaperName = "Mi Pagina";
            oRptInformeComercial.Document.Printer.PrinterName = "";

            var oDatos = new List<RptCartaDePresentacionInfo>();
            var oRptProduccionInfo = new List<CartaDePresentacionAcopiadores>();
            var oRptAlmacenamientoInfo = new List<CartaDePresentacionAcopiadores>();

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
                    FileName = oParam.corredorCuit.ToString() + " - " + oParam.vendedorCuit.ToString() + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public Reportes Generar(RptCartaDePresentacionInfo oParam)
        {
            var oReporte = new Reportes();
            var oRptInformeComercial = new RptCartaDePresentacion();
            oRptInformeComercial.PageSettings.DefaultPaperSize = false;
            oRptInformeComercial.PageSettings.PaperKind = System.Drawing.Printing.PaperKind.Custom;
            oRptInformeComercial.PageSettings.PaperName = "Mi Pagina";
            oRptInformeComercial.Document.Printer.PrinterName = "";

            var oDatos = new List<RptCartaDePresentacionInfo>();
            var oRptProduccionInfo = new List<CartaDePresentacionAcopiadores>();
            var oRptAlmacenamientoInfo = new List<CartaDePresentacionAcopiadores>();

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

                oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = oParam.corredorCuit.ToString() + " - " + oParam.vendedorCuit.ToString() + ".pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return oReporte;
        }
    }
}