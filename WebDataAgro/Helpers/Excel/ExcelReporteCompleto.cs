using Molinos.DataAgro.Entities.Common.Enums;
using Molinos.DataAgro.Entities.Dto;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebDataAgro.Models;
using Molinos.DataAgro.Entities.Resources;
using NPOI.XSSF.UserModel;

namespace WebDataAgro.Helpers.Excel
{
    public class ExcelReporteCompleto
    {    
        public static byte[] GenerarExcel(ReporteCompraNetModel model, List<ExcelPosicionMaterialDto> posicion)
        {
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("Tablero");
            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);
            #region colores
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.SetFont(fontBold);
            var cellcolorTitles = workbook.CreateCellStyle();
            cellcolorTitles.BorderBottom = BorderStyle.Thin;
            cellcolorTitles.BorderTop = BorderStyle.Thin;
            cellcolorTitles.BorderLeft = BorderStyle.Thin;
            cellcolorTitles.BorderRight = BorderStyle.Thin;
            cellcolorTitles.Alignment = HorizontalAlignment.Center;
            cellcolorTitles.SetFont(fontBold);
            cellcolorTitles.FillForegroundColor = IndexedColors.LightGreen.Index;
            cellcolorTitles.FillPattern = FillPattern.SolidForeground;
            var cellcolorGreen = workbook.CreateCellStyle();
            cellcolorGreen.BorderBottom = BorderStyle.Thin;
            cellcolorGreen.BorderTop = BorderStyle.Thin;
            cellcolorGreen.BorderLeft = BorderStyle.Thin;
            cellcolorGreen.BorderRight = BorderStyle.Thin;
            cellcolorGreen.Alignment = HorizontalAlignment.Center;
            cellcolorGreen.SetFont(fontBold);
            cellcolorGreen.FillForegroundColor = IndexedColors.Lime.Index;
            cellcolorGreen.FillPattern = FillPattern.SolidForeground;
            var cellcolorTan = workbook.CreateCellStyle();
            cellcolorTan .BorderBottom = BorderStyle.Thin;
            cellcolorTan .BorderTop = BorderStyle.Thin;
            cellcolorTan .BorderLeft = BorderStyle.Thin;
            cellcolorTan .BorderRight = BorderStyle.Thin;
            cellcolorTan .Alignment = HorizontalAlignment.Center;
            cellcolorTan .SetFont(fontBold);
            cellcolorTan .FillForegroundColor = IndexedColors.Tan.Index;
            cellcolorTan.FillPattern = FillPattern.SolidForeground;
            var cellcolorCornflowerBlue = workbook.CreateCellStyle();
            cellcolorCornflowerBlue .BorderBottom = BorderStyle.Thin;
            cellcolorCornflowerBlue .BorderTop = BorderStyle.Thin;
            cellcolorCornflowerBlue .BorderLeft = BorderStyle.Thin;
            cellcolorCornflowerBlue .BorderRight = BorderStyle.Thin;
            cellcolorCornflowerBlue .Alignment = HorizontalAlignment.Center;
            cellcolorCornflowerBlue .SetFont(fontBold);
            cellcolorCornflowerBlue .FillForegroundColor = IndexedColors.CornflowerBlue.Index;
            cellcolorCornflowerBlue.FillPattern = FillPattern.SolidForeground;
            var cellcolorBlue = workbook.CreateCellStyle();
            cellcolorBlue.BorderBottom = BorderStyle.Thin;
            cellcolorBlue.BorderTop = BorderStyle.Thin;
            cellcolorBlue.BorderLeft = BorderStyle.Thin;
            cellcolorBlue.BorderRight = BorderStyle.Thin;
            cellcolorBlue.Alignment = HorizontalAlignment.Center;
            cellcolorBlue.SetFont(fontBold);
            cellcolorBlue.FillForegroundColor = IndexedColors.PaleBlue.Index;
            cellcolorBlue.FillPattern = FillPattern.SolidForeground;
            #endregion 
            ICellStyle[] colores = new ICellStyle[] { cellcolorGreen, cellcolorTan, cellcolorCornflowerBlue, cellcolorBlue };

            #region row1
            var row = sheet.CreateRow(0);
            row = sheet.CreateRow(1);
            var celda = row.CreateCell(0);
            celda = row.CreateCell(1);
            celda = row.CreateCell(2);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(3);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(4);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(5);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(6);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(7);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(8);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(9);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(10);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(11);
            celda = row.CreateCell(12);
            celda = row.CreateCell(13);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(14);
            celda.CellStyle = cellBorderStyleColumnTitles;
            celda = row.CreateCell(15);
            celda.CellStyle = cellBorderStyleColumnTitles;

            var cra = new CellRangeAddress(1, 1, 2, 4);
            var cra1 = new CellRangeAddress(1, 1, 5, 7);
            var cra2 = new CellRangeAddress(1, 1, 8, 10);
            var cra3 = new CellRangeAddress(1, 1, 13, 15);
            sheet.AddMergedRegion(cra);
            sheet.AddMergedRegion(cra1);
            sheet.AddMergedRegion(cra2);
            sheet.AddMergedRegion(cra3);

            var celdasMerge = sheet.GetRow(1).GetCell(2);
            celdasMerge.SetCellValue("DISPONIBLE");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(5);
            celdasMerge.SetCellValue("FORWARD");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(8);
            celdasMerge.SetCellValue("NEW CROP");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(13);
            celdasMerge.SetCellValue("SOJA SUSTENTABLE");
            celdasMerge.CellStyle = cellcolorTitles;
            #endregion
            #region row2
            row = sheet.CreateRow(2);
            celda = row.CreateCell(0);
            celda = row.CreateCell(1);
            celda.SetCellValue("PRODUCTO");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(2);
            celda.SetCellValue("A FIJAR");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(3);
            celda.SetCellValue("A PRECIO");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(4);
            celda.SetCellValue("FIJACION");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(5);
            celda.SetCellValue("A FIJAR");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(6);
            celda.SetCellValue("A PRECIO");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(7);
            celda.SetCellValue("FIJACION");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(8);
            celda.SetCellValue("A FIJAR");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(9);
            celda.SetCellValue("A PRECIO");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(10);
            celda.SetCellValue("FIJACION");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(11);
            celda.SetCellValue("TOTAL");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(12);
            celda = row.CreateCell(13);
            celda.SetCellValue("A PRECIO");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(14);
            celda.SetCellValue("A FIJAR");
            celda.CellStyle = cellcolorTitles;
            celda = row.CreateCell(15);
            celda.SetCellValue("TOTAL");
            celda.CellStyle = cellcolorTitles;
            #endregion
            #region PosicionToneladas
            var j = 3;
            foreach (var material in model.ToneladasGranoTipo)
            {
                row = sheet.CreateRow(j);
                celda = row.CreateCell(0);
                celda = row.CreateCell(1);
                celda.SetCellValue(material.Material);
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(2);
                celda.SetCellValue(material.DispAFijar.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(3);
                celda.SetCellValue(material.DispAPrecio.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(4);
                celda.SetCellValue(material.DispFijac.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(5);
                celda.SetCellValue(material.FrwAFijar.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(6);
                celda.SetCellValue(material.FrwAPrecio.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(7);
                celda.SetCellValue(material.FrwFijac.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(8);
                celda.SetCellValue(material.NewAFijar.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(9);
                celda.SetCellValue(material.NewAPrecio.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(10);
                celda.SetCellValue(material.NewFijac.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                celda = row.CreateCell(11);
                celda.SetCellValue(material.Total.ToString("N0"));
                celda.CellStyle = cellBorderStyleColumnTitles;
                if (j == 3)
                {
                    celda = row.CreateCell(12);
                    celda = row.CreateCell(13);
                    celda.SetCellValue(model.SojaSustentable.Precio.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(14);
                    celda.SetCellValue(model.SojaSustentable.Fijar.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(15);
                    celda.SetCellValue(model.SojaSustentable.Total.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                }
                j++;
            }
            #endregion
            #region PosicionCompras
            var col = 0;
            row = sheet.CreateRow(7);
            row = sheet.CreateRow(8);
            celda = row.CreateCell(1);
            celda.CellStyle = colores[0];
            celda = row.CreateCell(2);
            celda.CellStyle = colores[0];
            var merge = new CellRangeAddress(8, 8, 1, 2);
            sheet.AddMergedRegion(merge);
            celda  = sheet.GetRow(8).GetCell(1);
            celda.SetCellValue("SOJA");
            celda.CellStyle = colores[0];
            celda = row.CreateCell(4);
            celda.CellStyle = colores[1];
            celda = row.CreateCell(5);
            celda.CellStyle = colores[1];
            merge = new CellRangeAddress(8, 8, 4, 5);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(8).GetCell(4);
            celda.SetCellValue("MAIZ");
            celda.CellStyle = colores[1];
            celda = row.CreateCell(7);
            celda.CellStyle = colores[2];
            celda = row.CreateCell(8);
            celda.CellStyle = colores[2];
            merge = new CellRangeAddress(8, 8, 7, 8);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(8).GetCell(7);
            celda.SetCellValue("TRIGO CAMARA");
            celda.CellStyle = colores[2];
            celda = row.CreateCell(10);
            celda.CellStyle = colores[3];
            celda = row.CreateCell(11);
            celda.CellStyle = colores[3];
            merge = new CellRangeAddress(8, 8, 10, 11);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(8).GetCell(10);
            celda.SetCellValue("TRIGO CALIDAD");
            celda.CellStyle = colores[3];
            row = sheet.CreateRow(9);
            row = sheet.CreateRow(10);
            row = sheet.CreateRow(11);
            row = sheet.CreateRow(12);
            row = sheet.CreateRow(13);
            row = sheet.CreateRow(14);
            row = sheet.CreateRow(15);
            row = sheet.CreateRow(16);
            row = sheet.CreateRow(17);
            row = sheet.CreateRow(18);
            row = sheet.CreateRow(19);
            row = sheet.CreateRow(20);
            row = sheet.CreateRow(21);
            row = sheet.CreateRow(22);
            row = sheet.CreateRow(23);
            row = sheet.CreateRow(24);
            int i;
            int c = 0;
            foreach (var material in model.PosicionCompras)
            {
                i = 9;
                if (material.Material == "Soja" || material.Material == "Maíz")
                {
                    row = sheet.GetRow(i);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("POSICION");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue("TON");
                    celda.CellStyle = colores[col];
                    foreach (var mes in (EnumMeses[])Enum.GetValues(typeof(EnumMeses)))
                    {
                        i++;
                        row = sheet.GetRow(i);
                        celda = row.CreateCell(c);
                        celda = row.CreateCell(c + 1);
                        celda.SetCellValue(mes.ToString());
                        celda.CellStyle = cellBorderStyleColumnTitles;
                        celda = row.CreateCell(c + 2);
                        celda.SetCellValue(material.PosicionKilos.Where(x => x.Mes == mes).Select(x => x.Kilos).DefaultIfEmpty(0).FirstOrDefault().ToString("N0"));
                        celda.CellStyle = cellBorderStyleColumnTitles;
                    }
                    row = sheet.GetRow(i + 1);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("TOTAL");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.Total.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;

                    c += 3;
                }
                else
                {
                    row = sheet.GetRow(i);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("POSICION");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue("TON");
                    celda.CellStyle = colores[col];
                    row = sheet.GetRow(i+1);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("Noviembre");
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.PosicionKilos.Where(x => x.Mes == EnumMeses.Noviembre).Select(x => x.Kilos).DefaultIfEmpty(0).FirstOrDefault().ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    row = sheet.GetRow(i + 2);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("Diciembre");
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.PosicionKilos.Where(x => x.Mes == EnumMeses.Diciembre).Select(x => x.Kilos).DefaultIfEmpty(0).FirstOrDefault().ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    row = sheet.GetRow(i + 3);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("Enero");
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.PosicionKilos.Where(x => x.Mes == EnumMeses.Enero).Select(x => x.Kilos).DefaultIfEmpty(0).FirstOrDefault().ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    row = sheet.GetRow(i + 4);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("Febrero");
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.PosicionKilos.Where(x => x.Mes == EnumMeses.Febrero).Select(x => x.Kilos).DefaultIfEmpty(0).FirstOrDefault().ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    row = sheet.GetRow(i + 5);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("TOTAL");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.Total.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    
                    c += 3;
                }
                col++;
            }
            #endregion
            #region ToneladasKilos
            i = 9; 
            foreach (var moneda in model.PrecioCantidad)
            {
                row = sheet.GetRow(i);
                celda = row.CreateCell(c);
                celda = row.CreateCell(c + 1);
                celda.SetCellValue(moneda.Moneda);
                celda.CellStyle = cellcolorTitles;
                celda = row.CreateCell(c + 2);
                celda.SetCellValue(moneda.Cantidad.HasValue? moneda.Cantidad.Value.ToString("N2"):"0");
                celda.CellStyle = cellBorderStyleColumnTitles;
                i++;
            }
            #endregion
            for(i = 0; i<=c+2; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            #region Posicion
            byte[] rgb = new byte[3] { 169, 208, 142 };
            var cellStyleColumnTitles = workbook.CreateCellStyle();            
            cellStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellStyleColumnTitles.SetFont(fontBold);

            var sheet2 = (HSSFSheet)workbook.CreateSheet("Posición Soja");
            var listaSoja = posicion.Where(x => x.Material == "Soja").ToList();
            CrearSheets(sheet2, cellcolorTitles, cellStyleColumnTitles, listaSoja);

            var sheet3 = (HSSFSheet)workbook.CreateSheet("Posición Maiz");
            var listaMaiz = posicion.Where(x => x.Material == "Maiz").ToList();
            CrearSheets(sheet3, cellcolorTitles, cellStyleColumnTitles, listaMaiz);

            var sheet4 = (HSSFSheet)workbook.CreateSheet("Posición Trigo Cámara");
            var listaTrigoCamara = posicion.Where(x => x.Material == "Trigo" && x.CalidadEspecial != "X").ToList();
            CrearSheets(sheet4, cellcolorTitles, cellStyleColumnTitles, listaTrigoCamara);

            var sheet5 = (HSSFSheet)workbook.CreateSheet("Posición Trigo Calidad");
            var listaTrigoCalidad = posicion.Where(x => x.Material == "Trigo" && x.CalidadEspecial == "X").ToList();
            CrearSheets(sheet5, cellcolorTitles, cellStyleColumnTitles, listaTrigoCalidad);
            #endregion
            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }
        private static void CrearSheets(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, List<ExcelPosicionMaterialDto> listaMaterial)
        {
            var cabeceras = 0;
            var filas = 0;
            var row = sheet.CreateRow(filas);
            foreach (var titulo in typeof(ExcelPosicionMaterialDto).GetProperties().Select(p => Text.ResourceManager.GetString(p.Name)).ToArray())
            {
                var celda = row.CreateCell(cabeceras);
                celda.SetCellValue(titulo);
                celda.CellStyle = cellcolorTitles;
                cabeceras++;
            }
            foreach (var material in listaMaterial)
            {
                filas++;
                row = sheet.CreateRow(filas);
                var celda = row.CreateCell(0);
                celda.SetCellValue(Text.ResourceManager.GetString(material.Mes));
                celda = row.CreateCell(1);
                celda.SetCellValue(material.Contrato);
                celda = row.CreateCell(2);
                celda.SetCellValue(material.RazonSocial);
                celda = row.CreateCell(3);
                celda.SetCellValue(material.Cuit);
                celda = row.CreateCell(4);
                celda.SetCellValue(material.Material);
                celda = row.CreateCell(5);
                celda.SetCellValue(material.TipoNegocio);
                celda = row.CreateCell(6);
                celda.SetCellValue(material.Comercial);
                celda = row.CreateCell(7);
                celda.SetCellValue(material.Cantidad.ToString("N0"));
                celda = row.CreateCell(8);
                celda.SetCellValue(material.CantidadCamiones.ToString("N0"));
                celda = row.CreateCell(9);
                celda.SetCellValue(material.Campana);
                celda = row.CreateCell(10);
                celda.SetCellValue(material.FechaDesde);
                celda = row.CreateCell(11);
                celda.SetCellValue(material.FechaHasta);
                celda = row.CreateCell(12);
                celda.SetCellValue(material.Precio.ToString("N2"));
                celda = row.CreateCell(13);
                celda.SetCellValue(material.Moneda);
                celda = row.CreateCell(14);
                celda.SetCellValue(material.Fecha);
                celda = row.CreateCell(15);
                celda.SetCellValue(material.Provincia);
                celda = row.CreateCell(16);
                celda.SetCellValue(material.Localidad);
                celda = row.CreateCell(17);
                celda.SetCellValue(material.Boleto);
                celda = row.CreateCell(18);
                celda.SetCellValue(material.Bolsa);
                celda = row.CreateCell(19);
                celda.SetCellValue(material.Destino);
                celda = row.CreateCell(20);
                celda.SetCellValue(material.CondicionFijacion);
                celda = row.CreateCell(21);
                celda.SetCellValue(material.DesdeFijacion);
                celda = row.CreateCell(22);
                celda.SetCellValue(material.HastaFijacion);
                celda = row.CreateCell(23);
                celda.SetCellValue(material.Base);
                celda = row.CreateCell(24);
                celda.SetCellValue(material.ImporteSustentable);
                celda = row.CreateCell(25);
                celda.SetCellValue(material.FechaDolarizado);
                celda = row.CreateCell(26);
                celda.SetCellValue(material.DiasPesificado);
                celda = row.CreateCell(27);
                celda.SetCellValue(material.NoInformaSio);
                celda = row.CreateCell(28);
                celda.SetCellValue(material.Ampliaciones);
                celda = row.CreateCell(29);
                celda.SetCellValue(material.Consignatario);
                celda = row.CreateCell(30);
                celda.SetCellValue(material.PlanCanje);
                celda = row.CreateCell(31);
                celda.SetCellValue(material.Pago);
                celda = row.CreateCell(32);
                celda.SetCellValue(material.CalidadEspecial);
                celda = row.CreateCell(33);
                celda.SetCellValue(material.EstablecimientoPropio);
                celda = row.CreateCell(34);
                celda.SetCellValue(material.Observacion);
                celda = row.CreateCell(35);
            }
            for (var i = 0; i < 35; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }
    }
}