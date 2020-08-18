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
    public class LstContacto
    {
        private IReportesManager reportesManager;

        public LstContacto(IReportesManager reportesManager)
        {
            this.reportesManager = reportesManager;
        }

        //-----------------------------------------------------------------------------------
        //  Metodos Publicos
        //-----------------------------------------------------------------------------------

        public string GenerarListado(List<ContactoIni> oDatos)
        {
            var oRptContacto = new RptContacto();

            oRptContacto.DataSource = oDatos;

            oRptContacto.Run(false);

            var oExportPDF = new PdfExport();

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                oExportPDF.Export(oRptContacto.Document, ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Contactos.pdf",
                    Contenido = ms.ToArray().ReplaceText()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


        public string GenerarExcel(List<ContactoIni> oDatos)
        {
            var excel = new ExcelPackage();

            if (oDatos.Count > 0)
            {
                var query = oDatos.Select(x => new ContactoExcel
                {
                    RazonSocial = x.RazonSocial,
                    Calificacion = x.Calificacion,
                    Cuit = x.Cuit,
                    Mail = x.Mail,
                    Estado = x.Estado,
                    Telefono = x.Telefono,
                    UltimoContacto = x.UltimoContacto,
                    Condicion = x.TooltipNoOperable,
                    Operable = x.NoOperable == true ? "No operable" : "Operable",
                    ComercialCargo = x.ComercialCargo,
                    FechaAlta = x.FechaAlta,
                    GrupoDeCompras = x.GrupoDeCompras
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

                workSheet.Cells[1, cantColumns - 4].Value = "No Operable/Operable";
                workSheet.Column(cantColumns - 4).AutoFit();

                workSheet.Cells[1, cantColumns - 2].Value = "Fecha de Alta";
                workSheet.Column(cantColumns - 2).AutoFit();

                workSheet.Cells[1, cantColumns - 1].Value = "Grupo de Compras";
                workSheet.Column(cantColumns - 1).AutoFit();

                workSheet.Cells[1, cantColumns].Value = "Comercial a Cargo";
                workSheet.Column(cantColumns).AutoFit();

            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Contactos.xlsx",
                    Contenido = ms.ToArray()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }


        public string GenerarExcelExportAll(ExportAll oDatos)
        {

            var excel = new ExcelPackage();

            var oColumnas = oDatos.contacto;

            var workSheet = excel.Workbook.Worksheets.Add("Datos del Proveedor");

            workSheet.Cells[1, 1].LoadFromCollection(oColumnas, true);

            var workSheet2 = excel.Workbook.Worksheets.Add("Objetivo");

            workSheet2.Cells[1, 1].LoadFromCollection(oDatos.objetivo, true);

            var workSheet3 = excel.Workbook.Worksheets.Add("Datos de contacto");

            workSheet3.Cells[1, 1].LoadFromCollection(oDatos.ContactosPrincipales, true);

            var workSheet4 = excel.Workbook.Worksheets.Add("Produccion");

            workSheet4.Cells[1, 1].LoadFromCollection(oDatos.produccion, true);

            var workSheet5 = excel.Workbook.Worksheets.Add("Almacenamiento");

            workSheet5.Cells[1, 1].LoadFromCollection(oDatos.almacenamiento, true);

            var workSheet7 = excel.Workbook.Worksheets.Add("Compras");

            workSheet7.Cells[1, 1].LoadFromCollection(oDatos.compras, true);

            var workSheet8 = excel.Workbook.Worksheets.Add("Establecimiento");

            workSheet8.Cells[1, 1].LoadFromCollection(oDatos.establecimiento, true);
           
            var oPropRow = oColumnas.GetType().GetProperties();

            var cantColumns = oPropRow.Count();

            if (oColumnas.Count > 0)
            {
                oPropRow = oColumnas[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet.Column(i).AutoFit();
                };
            }

            var j = 1;
            while (workSheet.Cells[1, j].Value != null)
            {
                workSheet.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet.Cells[1, j].Style.Font.Bold = true;

                j++;
            }


            workSheet.Cells[1, 1].Value = "CUIT";
            workSheet.Column(1).AutoFit();

            workSheet.Cells[1, 2].Value = "Razón Social";
            workSheet.Column(2).AutoFit();

            workSheet.Cells[1, 4].Value = "Calificación";
            workSheet.Column(4).AutoFit();

            workSheet.Cells[1, 5].Value = "Segmentación";
            workSheet.Column(5).AutoFit();

            workSheet.Cells[1, 6].Value = "Domicilio de Actividad";
            workSheet.Column(6).AutoFit();

            workSheet.Cells[1, 7].Value = "Localidad";
            workSheet.Column(7).AutoFit();

            workSheet.Cells[1, 8].Value = "Provincia";
            workSheet.Column(8).AutoFit();

            workSheet.Cells[1, 9].Value = "Código Postal";
            workSheet.Column(9).AutoFit();

            workSheet.Cells[1, 10].Value = "Canal de Operación";
            workSheet.Column(10).AutoFit();

            workSheet.Cells[1, 11].Value = "Entrega A";
            workSheet.Column(11).AutoFit();

            workSheet.Cells[1, 12].Value = "Condiciones Preferentes";
            workSheet.Column(11).AutoFit();

            workSheet.Cells[1, 14].Value = "Área de Influencia";
            workSheet.Column(13).AutoFit();

            workSheet.Cells[1, 17].Value = "Cliente MOA";
            workSheet.Column(16).AutoFit();

            workSheet.Cells[1, 19].Value = "Fecha de Alta";
            workSheet.Column(19).AutoFit();

            workSheet.Cells[1, 20].Value = "Clasificacion";
            workSheet.Column(20).AutoFit();

            workSheet.Cells[1, 21].Value = "Boleto";
            workSheet.Column(21).AutoFit();

            workSheet.Cells[1, 22].Value = "Bolsa";
            workSheet.Column(22).AutoFit();

            workSheet.Cells[1, 24].Value = "Comisión";
            workSheet.Column(24).AutoFit();

            workSheet.Cells[1, 25].Value = "LocalidadCompraNet";
            workSheet.Column(25).AutoFit();

            workSheet.Cells[1, 26].Value = "ProvinciaCompraNet";
            workSheet.Column(26).AutoFit();

            if (oDatos.objetivo.Count > 0)
            {
                oPropRow = oDatos.objetivo[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {

                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet2.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet2.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet2.Cells[1, j].Value != null)
            {
                workSheet2.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet2.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet2.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet2.Cells[1, 1].Value = "CUIT";
            workSheet2.Column(1).AutoFit();

            workSheet2.Cells[1, 2].Value = "Razón Social";
            workSheet2.Column(2).AutoFit();

            if (oDatos.ContactosPrincipales.Count > 0)
            {
                oPropRow = oDatos.ContactosPrincipales[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet3.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet3.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet3.Cells[1, j].Value != null)
            {
                workSheet3.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet3.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet3.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet3.Cells[1, 1].Value = "CUIT";
            workSheet3.Column(1).AutoFit();

            workSheet3.Cells[1, 8].Value = "Profesión";
            workSheet3.Column(7).AutoFit();

            workSheet3.Cells[1, 11].Value = "Otros Intereses";
            workSheet3.Column(10).AutoFit();

            workSheet3.Cells[1, 7].Value = "Fecha de Nacimiento";
            workSheet3.Column(6).AutoFit();

            workSheet3.Cells[1, 2].Value = "Razón Social";
            workSheet3.Column(2).AutoFit();

            if (oDatos.produccion.Count > 0)
            {
                oPropRow = oDatos.produccion[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet4.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet4.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet4.Cells[1, j].Value != null)
            {
                workSheet4.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet4.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet4.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet4.Cells[1, 1].Value = "CUIT";
            workSheet4.Column(1).AutoFit();

            workSheet4.Cells[1, 2].Value = "Razón Social";
            workSheet4.Column(2).AutoFit();

            if (oDatos.almacenamiento.Count > 0)
            {

                oPropRow = oDatos.almacenamiento[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {

                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet5.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet5.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet5.Cells[1, j].Value != null)
            {
                workSheet5.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet5.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet5.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet5.Cells[1, 1].Value = "CUIT";
            workSheet5.Column(1).AutoFit();

            workSheet5.Cells[1, 2].Value = "Razón Social";
            workSheet5.Column(2).AutoFit();

            workSheet5.Cells[1, 5].Value = "Campaña";
            workSheet5.Column(5).AutoFit();

            workSheet5.Cells[1, 6].Value = "Capacidad Planta Ton";
            workSheet5.Column(6).AutoFit();

            workSheet5.Cells[1, 12].Value = "Volumen Anual Ton";
            workSheet5.Column(12).AutoFit();

            workSheet5.Cells[1, 13].Value = "Habilitado Soja Sustentable";
            workSheet5.Column(13).AutoFit();

            if (PermisosHelper.Is(PermisosDataAgro.DescargaExportAllComercial))
            {
                var workSheet6 = excel.Workbook.Worksheets.Add("Agenda");
                workSheet6.Cells[1, 1].LoadFromCollection(oDatos.agenda, true);

                if (oDatos.agenda.Count > 0)
                {
                    oPropRow = oDatos.agenda[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (var i = 1; i <= cantColumns; i++)
                    {

                        if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                        {
                            workSheet6.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                        }
                        workSheet6.Column(i).AutoFit();
                    };


                    j = 1;
                    while (workSheet6.Cells[1, j].Value != null)
                    {
                        workSheet6.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                        workSheet6.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                        workSheet6.Cells[1, j].Style.Font.Bold = true;

                        j++;
                    }


                    workSheet6.Cells[1, 1].Value = "CUIT";
                    workSheet6.Column(1).AutoFit();

                    workSheet6.Cells[1, 2].Value = "Razón Social";
                    workSheet6.Column(2).AutoFit();

                    workSheet6.Cells[1, 3].Value = "Tipo de Actividad";
                    workSheet6.Column(3).AutoFit();

                    workSheet6.Cells[1, 4].Value = "Detalle";
                    workSheet6.Column(4).AutoFit();

                    workSheet6.Cells[1, cantColumns - 3].Value = "Fecha Desde";
                    workSheet6.Column(cantColumns - 3).AutoFit();

                    workSheet6.Cells[1, cantColumns - 2].Value = "Hora Desde";
                    workSheet6.Column(cantColumns - 2).AutoFit();

                    workSheet6.Cells[1, cantColumns - 1].Value = "Fecha Hasta";
                    workSheet6.Column(cantColumns - 1).AutoFit();

                    workSheet6.Cells[1, cantColumns].Value = "Hora Hasta";
                    workSheet6.Column(cantColumns).AutoFit();
                }
            }

            if (oDatos.compras.Count > 0)
            {
                oPropRow = oDatos.compras[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet7.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet7.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet7.Cells[1, j].Value != null)
            {
                workSheet7.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet7.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet7.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet7.Cells[1, 1].Value = "CUIT";
            workSheet7.Column(1).AutoFit();

            workSheet7.Cells[1, 2].Value = "Razón Social";
            workSheet7.Column(2).AutoFit();


            if (oDatos.establecimiento.Count > 0)
            {
                oPropRow = oDatos.establecimiento[0].GetType().GetProperties();

                cantColumns = oPropRow.Count();

                for (int i = 1; i <= cantColumns; i++)
                {
                    if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                    {
                        workSheet8.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                    }
                    workSheet8.Column(i).AutoFit();
                };
            }

            j = 1;
            while (workSheet8.Cells[1, j].Value != null)
            {
                workSheet8.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet8.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet8.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            workSheet8.Cells[1, 1].Value = "CUIT";
            workSheet8.Column(1).AutoFit();

            workSheet8.Cells[1, 2].Value = "Razón Social";
            workSheet8.Column(2).AutoFit();

            workSheet8.Cells[1, 10].Value = "Has. Cultivables";
            workSheet8.Column(10).AutoFit();

            workSheet8.Cells[1, 11].Value = "Has. Totales";
            workSheet8.Column(11).AutoFit();

           var compraCampanaActual = oDatos.CompraCampanaActual;

           var workSheet9 = excel.Workbook.Worksheets.Add("Detalle Soja");
           workSheet9.Cells[1, 1].LoadFromCollection(compraCampanaActual.Where(x => x.Material == "Soja").ToList(), true);

           var workSheet10 = excel.Workbook.Worksheets.Add("Detalle Maíz");
           workSheet10.Cells[1, 1].LoadFromCollection(compraCampanaActual.Where(x => x.Material == "Maiz").ToList(), true);

           var workSheet11 = excel.Workbook.Worksheets.Add("Detalle Girasol");
           workSheet11.Cells[1, 1].LoadFromCollection(compraCampanaActual.Where(x => x.Material == "Girasol").ToList(), true);

           var workSheet12 = excel.Workbook.Worksheets.Add("Detalle Trigo");
           workSheet12.Cells[1, 1].LoadFromCollection(compraCampanaActual.Where(x => x.Material == "Trigo").ToList(), true);

                //Compra Soja

                if (compraCampanaActual.Count > 0)
                {
                    oPropRow = compraCampanaActual[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (int i = 1; i <= cantColumns; i++)
                    {
                        if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                        {
                            workSheet9.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                        }
                        workSheet9.Column(i).AutoFit();
                    };
                }

                j = 1;
                while (workSheet9.Cells[1, j].Value != null)
                {
                    workSheet9.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    workSheet9.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                    workSheet9.Cells[1, j].Style.Font.Bold = true;

                    j++;
                }
                //Maiz
                if (compraCampanaActual.Count > 0)
                {
                    oPropRow = compraCampanaActual[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (int i = 1; i <= cantColumns; i++)
                    {
                        if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                        {
                            workSheet10.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                        }
                        workSheet10.Column(i).AutoFit();
                    };
                }

                j = 1;
                while (workSheet10.Cells[1, j].Value != null)
                {
                    workSheet10.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    workSheet10.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                    workSheet10.Cells[1, j].Style.Font.Bold = true;

                    j++;
                }

                if (compraCampanaActual.Count > 0)
                {
                    oPropRow = compraCampanaActual[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (int i = 1; i <= cantColumns; i++)
                    {
                        if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                        {
                            workSheet11.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                        }
                        workSheet11.Column(i).AutoFit();
                    };
                }

                j = 1;
                while (workSheet11.Cells[1, j].Value != null)
                {
                    workSheet11.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                    workSheet11.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                    workSheet11.Cells[1, j].Style.Font.Bold = true;

                    j++;
                }

                if (compraCampanaActual.Count > 0)
                {
                    oPropRow = compraCampanaActual[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (int i = 1; i <= cantColumns; i++)
                    {
                        if (oPropRow[i - 1].PropertyType.FullName.IndexOf("System.DateTime") >= 0)
                        {
                            workSheet12.Column(i).Style.Numberformat.Format = "DD/MM/YYYY";

                        }
                        workSheet12.Column(i).AutoFit();
                    };
                }

                j = 1;
            while (workSheet12.Cells[1, j].Value != null)
            {
                workSheet12.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;

                workSheet12.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);

                workSheet12.Cells[1, j].Style.Font.Bold = true;

                j++;
            }

            var situacion = oDatos.Situacion;
            //Situacion Soja
            if (situacion.Count() > 0)
            {

                var workSheet13 = excel.Workbook.Worksheets.Add("Situación Compra");

                List<CampanaMaterialDetallePorMeseExcelDto> lista = new List<CampanaMaterialDetallePorMeseExcelDto>();

                lista.AddRange( situacion.Select(x => x.ListaConCorredor).SelectMany(x => x.ComprasConPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoAcopiador).SelectMany(x => x.ComprasConPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoProductor).SelectMany(x => x.ComprasConPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaConCorredor).SelectMany(x => x.RecibidoSinPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoAcopiador).SelectMany(x => x.RecibidoSinPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoProductor).SelectMany(x => x.RecibidoSinPrecio).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaConCorredor).SelectMany(x => x.ARecibirAFijar).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoAcopiador).SelectMany(x => x.ARecibirAFijar).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoProductor).SelectMany(x => x.ARecibirAFijar).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaConCorredor).SelectMany(x => x.FasonFas).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoAcopiador).SelectMany(x => x.FasonFas).ToList());
                //lista.AddRange(situacion.Select(x => x.ListaDirectoProductor).SelectMany(x => x.FasonFas).ToList());

                workSheet13.Cells[1, 1].LoadFromCollection(
                lista
                    //ComprasConPrecioCorredor
                //.Union(ComprasConPrecioAcopiador)
                //.Union(ComprasConPrecioProductor)
                //.Union(RecibidoSinPrecioCorredor)
                //.Union(RecibidoSinPrecioAcopiador)
                //.Union(RecibidoSinPrecioProductor)
                //.Union(ARecibirAFijarCorredor)
                //.Union(ARecibirAFijarAcopiador)
                //.Union(ARecibirAFijarProductor)
                //.Union(FasonFasConPrecioCorredor)
                //.Union(FasonFasAcopiador)
                //.Union(FasonFasProductor)
                , true);
                j = 1;
                while (workSheet13.Cells[1, j].Value != null)
                {
                    workSheet13.Cells[1, j].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    workSheet13.Cells[1, j].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightYellow);
                    workSheet13.Cells[1, j].Style.Font.Bold = true;

                    j++;
                }

                if (situacion.Count > 0)
                {
                    oPropRow = situacion[0].GetType().GetProperties();

                    cantColumns = oPropRow.Count();

                    for (int i = 1; i <= cantColumns; i++)
                    {
                        workSheet13.Column(i).AutoFit();
                    };
                }
            }

            var identif = Varios.GetIdentif();

            using (MemoryStream ms = new MemoryStream())
            {
                excel.SaveAs(ms);

                var oReporte = new Reportes()
                {
                    Identificador = identif,
                    FileName = "Contactos.xlsx",
                    Contenido = ms.ToArray()
                };

                reportesManager.GrabarReporte(oReporte);
            }

            return identif;
        }
    }
}



