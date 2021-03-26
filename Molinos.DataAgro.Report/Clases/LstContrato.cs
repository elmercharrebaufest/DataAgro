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
    public class LstContrato
    {
        private IReportesManager reportesManager;

        public LstContrato(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------


        public string GenerarExcel(List<BasicoContrato> oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Count > 0)
            {
                var query = oDatos.Select(x => new ContratoExcel
                {
                    Carga = x.Fecha,
                    Comercial = x.Comercial,
                    Grano = x.Material,
                    Tipo = x.TipoNegocio,
                    Contrato = (x.ContratoSAP != "0" && x.ContratoSAP != "" && x.ContratoSAP != null) ? x.ContratoSAP : x.Id.ToString(),
                    Destino = x.DestinoDescripcion,
                    Zona = x.ComercialZonaDescripcion,
                    Nombre = x.Proveedor,
                    CUIT = x.Cuit,
                    NombreCorredor = x.Corredor,
                    CUITCorredor = x.CUITCorredor,
                    Figura = x.ClasificacionDescripcion,
                    Cantidad = x.Cantidad,
                    Precio = x.PrecioNeto ?? x.Precio,
                    Moneda = x.MonedaId,
                    Camiones = x.CantidadCamiones,
                    Comision = (x.TipoNegocioId != 2 && x.TipoNegocioId != 3 && !(x.TipoNegocioId == 6 && x.Precio > 0)) ? 0 : decimal.Parse(((x.PorcentajeComision ?? 0) == 0 ? ((x.ImporteComision ?? 0) * 100 / x.Precio) : x.PorcentajeComision ?? 0).ToString("0.##")),
                    PorcentajeBonificacion = ((x.Descuentos != null && x.Descuentos.Count > 0) ? string.Join("/", x.Descuentos.Select(a => a.Porcentaje.ToString()).ToList()) : ""),
                    ImporteBonificacion = ((x.Descuentos != null && x.Descuentos.Count > 0) ? string.Join("/", x.Descuentos.Select(a => a.Importe.ToString()).ToList()) : ""),
                    MonedaBonificacion = ((x.Descuentos != null && x.Descuentos.Count > 0) ? string.Join("/", x.Descuentos.Select(a => (a.MonedaId ?? "").ToString()).ToList()) : ""),
                    MesPosicion = x.MesPosicion,
                    Procedencia = x.Localidad + " - " + x.Provincia,
                    Desde = x.FechaDesde,
                    Hasta = x.FechaHasta,
                    Cosecha = x.Campania,
                    FleteProcedencia = x.TarifaFlete,
                    Observaciones = x.Observacion
                });

                var oColumnas = query.ToList();

                var workSheet = excel.Workbook.Worksheets.Add("Sheet1");

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

                workSheet.Cells[1, 10].Value = "Nombre Corredor";
                workSheet.Cells[1, 11].Value = "CUIT Corredor";
                workSheet.Cells[1, 18].Value = "% Bonif por fuera";
                workSheet.Cells[1, 19].Value = "Imp. Bonif por fuera";
                workSheet.Cells[1, 20].Value = "Mon Bonif. por fuera";
                workSheet.Cells[1, 26].Value = "Flete Procedencia";
                workSheet.Column(1).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Reporte Comercial.xlsx",
                    Contenido = ms.ToArray()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


    }
}



