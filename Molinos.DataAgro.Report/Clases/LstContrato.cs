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
                    Fecha = x.Fecha,
                    Comercial = x.Comercial,
                    Grano = x.Material,
                    Tipo = x.TipoNegocio,
                    Contrato = x.Negocio,
                    Destino = x.DestinoDescripcion,
                    Zona = x.ZonaDescripcion,
                    Nombre = x.Proveedor,
                    CUIT = x.Cuit,
                    Figura = x.ClasificacionDescripcion,
                    Cantidad = x.Cantidad,
                    Precio = x.PrecioNeto??x.Precio,
                    Moneda = x.Moneda,
                    Camiones = x.CantidadCamiones,
                    Comision = x.PorcentajeComision,
                    PorcentajeBonificacion = x.PorcentajeBonificacion,
                    ImporteBonificacion = x.ImporteBonificacion,
                    MonedaBonificacion = x.MonedaBonificacion,
                    MesPosicion =string.IsNullOrEmpty(x.Posicion)?"": x.Posicion.Substring(0,2),
                    Procedencia = x.Localidad,
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

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };

                //workSheet.Cells[1, cantColumns].Value = "Comercial a Cargo";
                //workSheet.Column(cantColumns).AutoFit();

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



