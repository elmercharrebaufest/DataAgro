using DataDynamics.ActiveReports.Document;
using DataDynamics.ActiveReports.Export.Pdf;
using Mastersoft.Framework.DataRepository;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public async Task<string> GenerarListadoAsync(List<AgendaStore> oDatos)
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
                    Contenido = ms.ToArray()
                };
                
                await reportesManager.GrabarReporteAsync(oReporte);
            }

            return identif;
        }


}
}
