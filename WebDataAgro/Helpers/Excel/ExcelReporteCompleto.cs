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
using NPOI.HSSF.Util;

namespace WebDataAgro.Helpers.Excel
{
    public class ExcelReporteCompleto
    {
        public static byte[] GenerarExcel(ReporteCompraNetModel model, List<ExcelPosicionMaterialDto> posicion, bool incluirHedge)
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
            cellcolorTan.BorderBottom = BorderStyle.Thin;
            cellcolorTan.BorderTop = BorderStyle.Thin;
            cellcolorTan.BorderLeft = BorderStyle.Thin;
            cellcolorTan.BorderRight = BorderStyle.Thin;
            cellcolorTan.Alignment = HorizontalAlignment.Center;
            cellcolorTan.SetFont(fontBold);
            cellcolorTan.FillForegroundColor = IndexedColors.Tan.Index;
            cellcolorTan.FillPattern = FillPattern.SolidForeground;
            var cellcolorCornflowerBlue = workbook.CreateCellStyle();
            cellcolorCornflowerBlue.BorderBottom = BorderStyle.Thin;
            cellcolorCornflowerBlue.BorderTop = BorderStyle.Thin;
            cellcolorCornflowerBlue.BorderLeft = BorderStyle.Thin;
            cellcolorCornflowerBlue.BorderRight = BorderStyle.Thin;
            cellcolorCornflowerBlue.Alignment = HorizontalAlignment.Center;
            cellcolorCornflowerBlue.SetFont(fontBold);
            cellcolorCornflowerBlue.FillForegroundColor = IndexedColors.CornflowerBlue.Index;
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
            var cellcolorSuperGreen = workbook.CreateCellStyle();
            cellcolorSuperGreen.BorderBottom = BorderStyle.Thin;
            cellcolorSuperGreen.BorderTop = BorderStyle.Thin;
            cellcolorSuperGreen.BorderLeft = BorderStyle.Thin;
            cellcolorSuperGreen.BorderRight = BorderStyle.Thin;
            cellcolorSuperGreen.Alignment = HorizontalAlignment.Center;
            cellcolorSuperGreen.SetFont(fontBold);
            cellcolorSuperGreen.FillForegroundColor = IndexedColors.LightGreen.Index;
            cellcolorSuperGreen.FillPattern = FillPattern.SolidForeground;
            var cellcolorDarkCyanHedge = workbook.CreateCellStyle();
            cellcolorDarkCyanHedge.BorderBottom = BorderStyle.Thin;
            cellcolorDarkCyanHedge.BorderTop = BorderStyle.Thin;
            cellcolorDarkCyanHedge.BorderLeft = BorderStyle.Thin;
            cellcolorDarkCyanHedge.BorderRight = BorderStyle.Thin;
            cellcolorDarkCyanHedge.Alignment = HorizontalAlignment.Center;
            cellcolorDarkCyanHedge.SetFont(fontBold);
            cellcolorDarkCyanHedge.FillPattern = FillPattern.SolidForeground;
            HSSFPalette palette = workbook.GetCustomPalette();
            HSSFColor colorCyan = palette.FindSimilarColor(0, 139, 139);
            cellcolorDarkCyanHedge.FillForegroundColor = colorCyan.Indexed;

            var cellcolorplumObjetivos = workbook.CreateCellStyle();
            cellcolorplumObjetivos.BorderBottom = BorderStyle.Thin;
            cellcolorplumObjetivos.BorderTop = BorderStyle.Thin;
            cellcolorplumObjetivos.BorderLeft = BorderStyle.Thin;
            cellcolorplumObjetivos.BorderRight = BorderStyle.Thin;
            cellcolorplumObjetivos.Alignment = HorizontalAlignment.Center;
            cellcolorplumObjetivos.SetFont(fontBold);
            cellcolorplumObjetivos.FillPattern = FillPattern.SolidForeground;
            HSSFColor colorPlum = palette.FindSimilarColor(221, 160, 221);
            cellcolorplumObjetivos.FillForegroundColor = colorPlum.Indexed;

            var cellcolorGoldObjetivos = workbook.CreateCellStyle();
            cellcolorGoldObjetivos.BorderBottom = BorderStyle.Thin;
            cellcolorGoldObjetivos.BorderTop = BorderStyle.Thin;
            cellcolorGoldObjetivos.BorderLeft = BorderStyle.Thin;
            cellcolorGoldObjetivos.BorderRight = BorderStyle.Thin;
            cellcolorGoldObjetivos.Alignment = HorizontalAlignment.Center;
            cellcolorGoldObjetivos.SetFont(fontBold);
            cellcolorGoldObjetivos.FillPattern = FillPattern.SolidForeground;
            HSSFColor colorGold = palette.FindSimilarColor(255, 215, 0);
            cellcolorGoldObjetivos.FillForegroundColor = colorGold.Indexed;

            var estiloCeldasColumnTitles = workbook.CreateCellStyle();
            estiloCeldasColumnTitles.Alignment = HorizontalAlignment.Center;
            estiloCeldasColumnTitles.BorderBottom = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderTop = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderLeft = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderRight = BorderStyle.Thin;
            estiloCeldasColumnTitles.SetFont(fontBold);
            #endregion 
            ICellStyle[] colores = new ICellStyle[] { cellcolorGreen, cellcolorTan, cellcolorCornflowerBlue, cellcolorBlue, cellcolorSuperGreen, cellcolorDarkCyanHedge, cellcolorplumObjetivos, cellcolorGoldObjetivos };


            #region row1
            var row = sheet.CreateRow(0);
            row = sheet.CreateRow(1);
            var celda = row.CreateCell(0);

            CrearCelda(row, 1, null, null);
            CrearCelda(row, 2, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 3, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 4, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 5, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 6, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 7, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 8, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 9, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 10, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 11, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 12, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 13, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 14, null, null);
            CrearCelda(row, 15, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 16, null, cellBorderStyleColumnTitles);
            CrearCelda(row, 17, null, cellBorderStyleColumnTitles);

            var cra = new CellRangeAddress(1, 1, 2, 4);
            var cra1 = new CellRangeAddress(1, 2, 5, 5);
            var cra2 = new CellRangeAddress(1, 1, 6, 8);
            var cra3 = new CellRangeAddress(1, 2, 9, 9);
            var cra4 = new CellRangeAddress(1, 1, 10, 12);
            var cra5 = new CellRangeAddress(1, 2, 13, 13);

            var cra6 = new CellRangeAddress(1, 1, 15, 17);
            sheet.AddMergedRegion(cra);
            sheet.AddMergedRegion(cra1);
            sheet.AddMergedRegion(cra2);
            sheet.AddMergedRegion(cra3);
            sheet.AddMergedRegion(cra4);
            sheet.AddMergedRegion(cra5);
            sheet.AddMergedRegion(cra6);

            var celdasMerge = sheet.GetRow(1).GetCell(2);
            celdasMerge.SetCellValue("DISPONIBLE");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(5);
            celdasMerge.SetCellValue("TOTAL DISP");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(6);
            celdasMerge.SetCellValue("FORWARD");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(9);
            celdasMerge.SetCellValue("TOTAL FWR");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(10);
            celdasMerge.SetCellValue("NEW CROP");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(13);
            celdasMerge.SetCellValue("TOTAL NC");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(15);
            celdasMerge.SetCellValue("SOJA SUSTENTABLE");
            celdasMerge.CellStyle = cellcolorTitles;
            #endregion
            #region row2
            row = sheet.CreateRow(2);
            CrearCelda(row, 0, null, null);
            CrearCelda(row, 1, "PRODUCTO", cellcolorTitles);

            //DISPONIBLE
            CrearCelda(row, 2, "A Fijar", cellcolorTitles);
            CrearCelda(row, 3, "A Precio", cellcolorTitles);
            CrearCelda(row, 4, "Fijacion", cellcolorTitles);

            //FORWARD
            CrearCelda(row, 6, "A Fijar", cellcolorTitles);
            CrearCelda(row, 7, "A Precio", cellcolorTitles);
            CrearCelda(row, 8, "Fijacion", cellcolorTitles);

            //NEW CROP
            CrearCelda(row, 10, "A Fijar", cellcolorTitles);
            CrearCelda(row, 11, "A Precio", cellcolorTitles);
            CrearCelda(row, 12, "Fijacion", cellcolorTitles);

            CrearCelda(row, 13, null, cellcolorTitles);


            //SOJA SUSTENTABLE
            CrearCelda(row, 15, "A Precio", cellcolorTitles);
            CrearCelda(row, 16, "A Fijar", cellcolorTitles);
            CrearCelda(row, 17, "Total", cellcolorTitles);

            #endregion
            #region PosicionToneladas
            var j = 3;
            foreach (var material in model.ToneladasGranoTipo)
            {
                row = sheet.CreateRow(j);
                CrearCelda(row, 0, null, null);

                //DISPONIBLE
                CrearCelda(row, 1, material.Material, cellBorderStyleColumnTitles);
                CrearCelda(row, 2, material.DispAFijar.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 3, material.DispAPrecio.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 4, (material.DispFijac + material.DispFason).ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 5, (material.DispAFijar + material.DispAPrecio + material.DispFijac + material.DispFason).ToString("N0"), cellBorderStyleColumnTitles);

                //FORWARD
                CrearCelda(row, 6, material.FrwAFijar.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 7, material.FrwAPrecio.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 8, (material.FrwFijac + material.FrwFason).ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 9, (material.FrwAFijar + material.FrwAPrecio + material.FrwFijac + material.FrwFason).ToString("N0"), cellBorderStyleColumnTitles);

                //NEW CROP
                CrearCelda(row, 10, material.NewAFijar.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 11, material.NewAPrecio.ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 12, (material.NewFijac + material.NewFason).ToString("N0"), cellBorderStyleColumnTitles);
                CrearCelda(row, 13, (material.NewAFijar + material.NewAPrecio + material.NewFijac + material.NewFason).ToString("N0"), cellBorderStyleColumnTitles);


                if (j == 3)
                {
                    celda = row.CreateCell(15);
                    celda.SetCellValue(model.SojaSustentable.Precio.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(16);
                    celda.SetCellValue(model.SojaSustentable.Fijar.ToString("N0"));
                    celda.CellStyle = cellBorderStyleColumnTitles;
                    celda = row.CreateCell(17);
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
            celda = sheet.GetRow(8).GetCell(1);
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
                    celda.SetCellValue("Posicion");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue("Ton");
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
                    celda.SetCellValue("Total");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.Total.ToString("N0"));
                    celda.CellStyle = colores[col];

                    c += 3;
                }
                else
                {
                    row = sheet.GetRow(i);
                    celda = row.CreateCell(c);
                    celda = row.CreateCell(c + 1);
                    celda.SetCellValue("Posicion");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue("Ton");
                    celda.CellStyle = colores[col];
                    row = sheet.GetRow(i + 1);
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
                    celda.SetCellValue("Total");
                    celda.CellStyle = colores[col];
                    celda = row.CreateCell(c + 2);
                    celda.SetCellValue(material.Total.ToString("N0"));
                    celda.CellStyle = colores[col];

                    c += 3;
                }
                col++;
            }
            #endregion

            #region Hedge
            if (incluirHedge)
            {


                CrearTablaHedge(sheet, colores[5], estiloCeldasColumnTitles, model, colores[6]);
                CrearTablaAgenteDeCompras(sheet, colores[7], estiloCeldasColumnTitles, model);
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
                celda.SetCellValue(moneda.Cantidad.HasValue ? moneda.Cantidad.Value.ToString("N2") : "0");
                celda.CellStyle = cellBorderStyleColumnTitles;
                i++;
            }
            #endregion
            for (i = 0; i <= 30; i++)
            {
                sheet.AutoSizeColumn(i, true);
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

        private static void CrearCelda(IRow row, int posicion, string valor, ICellStyle estilo)
        {
            var celda = row.CreateCell(posicion);
            celda.SetCellValue(valor);
            celda.CellStyle = estilo;
        }

        private static void CrearTablaHedge(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, ReporteCompraNetModel model, ICellStyle cellcolorTitlesObjectivos)
        {
            var row = sheet.CreateRow(25);

            row = sheet.CreateRow(26);
            int iC = 0;

            CrearCelda(row, iC + 1, "HEDGE", cellcolorTitles);
            CrearCelda(row, iC + 2, null, cellcolorTitles);
            CrearCelda(row, iC + 3, null, cellcolorTitles);
            CrearCelda(row, iC + 4, null, cellcolorTitles);
            var merge = new CellRangeAddress(26, 26, 1, 4);
            sheet.AddMergedRegion(merge);
            row = sheet.CreateRow(27);
            CrearCelda(row, iC + 1, "Producto", cellcolorTitles);
            CrearCelda(row, iC + 2, "Ddisponible", cellcolorTitles);
            CrearCelda(row, iC + 3, "Forward", cellcolorTitles);
            CrearCelda(row, iC + 4, "New Crop", cellcolorTitles);
            int iterFilas = 1;
            foreach (HedgeMaterialModel producto in model.HedgeMaterial)
            {
                row = sheet.CreateRow(27 + iterFilas);
                CrearCelda(row, iC + 1, producto.MaterialDescripcion, cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 2, producto.Disponible.ToString(), cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 3, producto.Forward.ToString(), cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 4, producto.NewCrop.ToString(), cellBorderStyleColumnTitles);
                iterFilas++;
            }

            row = sheet.GetRow(26);
            CrearCelda(row, iC + 7, "Dia", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 8, "Objetivo", cellcolorTitlesObjectivos);

            row = sheet.GetRow(27);
            CrearCelda(row, iC + 6, "Pricing", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 7, model.HedgeObjetivo.PricingCumplido.ToString(), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, model.HedgeObjetivo.PricingObjetivo.ToString(), cellBorderStyleColumnTitles);

            row = sheet.GetRow(28);
            if (row == null)
            {
                row = sheet.CreateRow(28);
            }
            CrearCelda(row, iC + 6, "A remitir", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 7, model.HedgeObjetivo.RemitirCumplido.ToString(), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, model.HedgeObjetivo.RemitirObjetivo.ToString(), cellBorderStyleColumnTitles);

            row = sheet.GetRow(30);
            if (row == null)
            {
                row = sheet.CreateRow(30);
            }
            CrearCelda(row, iC + 7, "TC promedio", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 8, "TC total", cellcolorTitlesObjectivos);

            row = sheet.GetRow(31);
            if (row == null)
            {
                row = sheet.CreateRow(31);
            }
            CrearCelda(row, iC + 7, model.TCPromedioDto.PromedioTC.ToString("n2"), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, model.TCPromedioDto.TotalTC.ToString("n2"), cellBorderStyleColumnTitles);

        }
        private static void CrearTablaAgenteDeCompras(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, ReporteCompraNetModel model)
        {
            int iC = 0;
            var row = sheet.GetRow(26);
            if (row == null)
            {
                row = sheet.CreateRow(26);
            }
            var cra = new CellRangeAddress(26, 26, 10, 10 + model.AgenteCompras.ListaOperadores.Count + 2);
            sheet.AddMergedRegion(cra);

            CrearCelda(row, iC + 10, "AGENTE DE COMPRAS", cellcolorTitles);
            for (var iter = 11; iter <= 10 + model.AgenteCompras.ListaOperadores.Count + 2; iter++)
            {
                CrearCelda(row, iter, null, cellcolorTitles);
            }
            row = sheet.GetRow(27);
            if (row == null)
            {
                row = sheet.CreateRow(27);
            }
            CrearCelda(row, iC + 10, "Producto", cellcolorTitles);
            CrearCelda(row, iC + 11, "Posicion", cellcolorTitles);
            int posicionOperador = 1;
            foreach (AgenteCompraDto.OperadorCantidad agente in model.AgenteCompras.ListaOperadores)
            {
                CrearCelda(row, iC + 11 + posicionOperador, agente.OperadorDesc, cellcolorTitles);
                posicionOperador++;
            }
            CrearCelda(row, iC + 11 + posicionOperador, "Total", cellcolorTitles);

            int filaOperador = 1;
            foreach (AgenteCompraDto agente in model.AgenteCompras.ListaAgenteCompras)
            {
                posicionOperador = 0;

                row = sheet.GetRow(27 + filaOperador);
                if (row == null)
                {
                    row = sheet.CreateRow(27 + filaOperador);
                }
                CrearCelda(row, iC + 10 + posicionOperador, agente.MaterialDesc, cellBorderStyleColumnTitles);
                posicionOperador++;

                var pos = agente.Posicion.Split('.');
                CrearCelda(row, iC + 10 + posicionOperador, (EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1], cellBorderStyleColumnTitles);
                posicionOperador++;

                foreach (var op in model.AgenteCompras.ListaOperadores)
                {
                    var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                    CrearCelda(row, iC + 10 + posicionOperador, cantidad != null ? cantidad : "0", cellBorderStyleColumnTitles);
                    posicionOperador++;
                }
                CrearCelda(row, iC + 10 + posicionOperador, agente.Operador.Sum(x => x.Cantidad).ToString("N0"), cellBorderStyleColumnTitles);
                filaOperador++;
            }
        }
    }
}