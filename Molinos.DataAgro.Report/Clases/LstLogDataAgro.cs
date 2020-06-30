using DataDynamics.ActiveReports.Export.Pdf;
using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using Molinos.DataAgro.Entities.Seguridad;
using Molinos.DataAgro.Interfaces;
using Molinos.DataAgro.Report.ActiveReport;
using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Report
{
    public class LstLogDataAgro
    {
        private IReportesManager reportesManager;

        public LstLogDataAgro(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------


        public string GenerarExcel(List<LogDataAgroDto> oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Count > 0)
            {
                List<LogDataAgroDto> datos = new List<LogDataAgroDto>();
                foreach (var item in oDatos)
                {
                    foreach (var item2 in item.CamposCambiados)
                    {
                        datos.Add(new LogDataAgroDto
                        {
                            Id = item.Id,
                            Usuario = item.Usuario,
                            Fecha = item.Fecha,
                            AccionRealizada = item.AccionRealizada,
                            Clase = item.Clase,
                            ClaseId = item.ClaseId,
                            Campo = item2.Campo,
                            Actual = item2.Actual,
                            Anterior = item2.Anterior,


                        });
                    }
                }

                var oColumnas = datos.ToList();

                var workSheet = excel.Workbook.Worksheets.Add("Hoja1");

                workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

                var oPropRow = oColumnas[0].GetType().GetProperties();

                var cantColumns = oPropRow.Count();
                var j = 1;
                while (workSheet.Cells[1, j].Value != null)
                {
                    workSheet.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    workSheet.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);

                    workSheet.Cells[1, j].Style.Font.Bold = true;

                    j++;
                }
                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };


                //workSheet.Cells[1, 26].Value = "Flete Procedencia";
                workSheet.Column(1).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Reporte LogDataAgro.xlsx",
                    Contenido = ms.ToArray()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


    }
}



