using DataDynamics.ActiveReports.Document;
using DataDynamics.ActiveReports.Export.Pdf;
using Mastersoft.Framework.DataRepository;
using Molinos.DataAgro.Business;
using Molinos.DataAgro.Business.Managers;
using Molinos.DataAgro.Entities;
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
        //-----------------------------------------------------------------------------------
        //  Variables Privadas
        //-----------------------------------------------------------------------------------

        private MSContext mobjMSContext;

        //-----------------------------------------------------------------------------------
        //  Constructor
        //-----------------------------------------------------------------------------------

        public LstAgendaActividad(MSContext oMSContext)
        {
            mobjMSContext = oMSContext;
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

                var oReportesManager = new ReportesManager();
                oReportesManager.Inicializar(mobjMSContext);

                await oReportesManager.GrabarReporteAsync(oReporte);
            }

            return identif;
        }


}
}
