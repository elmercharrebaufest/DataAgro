using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Molinos.DataAgro.Report.Clases
{
    public class LstAgendaActividad
    {
        private readonly IReportesManager reportesManager;

        public LstAgendaActividad(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------

        public string GenerarListado(List<AgendaStore> oDatos)
        {
            var oRptAgendaActividad = new RptAgendaActividad();

            oRptAgendaActividad.DataSource = oDatos;

            oRptAgendaActividad.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptAgendaActividad.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Agenda.pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }

        public string GenerarExcel(List<ActividadExportar> oDatos)
        {
            var excel = new ExcelPackage();
            var nombreArchivo = oDatos.Count > 0 ? ("Actividades-" + oDatos.First().Proveedor) : "Actividades";

            if (oDatos.Count > 0)
            {
                var query = oDatos.Select(x => new ActividadExportarExcel
                {
                    ActividadId = x.ActividadId,
                    TipoActividad = x.TipoActividad,
                    Detalle = x.Detalle,
                    FechaHoraActividad = x.FechaHoraActividad,
                    FechaHoraRecordatorio = x.FechaHoraRecordatorio,
                    Comercial = x.Comercial,
                    ContactoComercial = x.ContactoComercial,
                    FechaHoraRecordatorioFin = x.FechaHoraRecordatorioFin,
                    Asunto = x.Asunto
                });

                var oColumnas = query.ToList();

                var workSheet = excel.Workbook.Worksheets.Add("Actividades");

                workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY HH:mm";

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
                    FileName = nombreArchivo + ".xlsx",
                    Contenido = ms.ToArray()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


    }
}
