using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using System.Collections.Generic;
using System.IO;

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


    }
}
