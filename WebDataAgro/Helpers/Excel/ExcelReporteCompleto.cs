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
using Molinos.DataAgro.Entities.Entities;

namespace WebDataAgro.Helpers.Excel
{
    public class ExcelReporteCompleto
    {
        public static byte[] GenerarExcel(ReporteCompraNetModel model, List<ExcelPosicionMaterialDto> posicion, bool incluirHedge)
        {
            var c = 0;
            var r = 0;
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

            var cellcolorTrigoGrado2 = workbook.CreateCellStyle();
            cellcolorTrigoGrado2.BorderBottom = BorderStyle.Thin;
            cellcolorTrigoGrado2.BorderTop = BorderStyle.Thin;
            cellcolorTrigoGrado2.BorderLeft = BorderStyle.Thin;
            cellcolorTrigoGrado2.BorderRight = BorderStyle.Thin;
            cellcolorTrigoGrado2.Alignment = HorizontalAlignment.Center;
            cellcolorTrigoGrado2.SetFont(fontBold);
            cellcolorTrigoGrado2.FillPattern = FillPattern.SolidForeground;
            HSSFColor colorTeal = palette.FindSimilarColor(106, 230, 190);
            cellcolorTrigoGrado2.FillForegroundColor = colorTeal.Indexed;

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

            var cellcolorPink = workbook.CreateCellStyle();
            cellcolorPink.BorderBottom = BorderStyle.Thin;
            cellcolorPink.BorderTop = BorderStyle.Thin;
            cellcolorPink.BorderLeft = BorderStyle.Thin;
            cellcolorPink.BorderRight = BorderStyle.Thin;
            cellcolorPink.Alignment = HorizontalAlignment.Center;
            cellcolorPink.SetFont(fontBold);
            cellcolorPink.FillPattern = FillPattern.SolidForeground;
            HSSFColor colorPink = palette.FindSimilarColor(244, 193, 247);
            cellcolorPink.FillForegroundColor = colorPink.Indexed;

            var cellcolorDarkPink = workbook.CreateCellStyle();
            cellcolorDarkPink.BorderBottom = BorderStyle.Thin;
            cellcolorDarkPink.BorderTop = BorderStyle.Thin;
            cellcolorDarkPink.BorderLeft = BorderStyle.Thin;
            cellcolorDarkPink.BorderRight = BorderStyle.Thin;
            cellcolorDarkPink.Alignment = HorizontalAlignment.Center;
            cellcolorDarkPink.SetFont(fontBold);
            cellcolorDarkPink.FillPattern = FillPattern.SolidForeground;
            HSSFColor colorDarkPink = palette.FindSimilarColor(211, 96, 212);
            cellcolorDarkPink.FillForegroundColor = colorDarkPink.Indexed;

            var estiloCeldasColumnTitles = workbook.CreateCellStyle();
            estiloCeldasColumnTitles.Alignment = HorizontalAlignment.Center;
            estiloCeldasColumnTitles.BorderBottom = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderTop = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderLeft = BorderStyle.Thin;
            estiloCeldasColumnTitles.BorderRight = BorderStyle.Thin;
            estiloCeldasColumnTitles.SetFont(fontBold);
            #endregion 
            ICellStyle[] colores = new ICellStyle[] {
                cellcolorGreen,  cellcolorTan,
                cellcolorCornflowerBlue,cellcolorBlue,
                cellcolorTrigoGrado2, cellcolorPink,
                cellcolorDarkPink, cellcolorSuperGreen,
                cellcolorDarkCyanHedge, cellcolorplumObjetivos,
                cellcolorGoldObjetivos,
                 };

            #region row1
            var row = sheet.CreateRow(r); r++;
            row = sheet.CreateRow(r); r++;
            var celda = row.CreateCell(c); c++;

            CrearCelda(row, c, null, null); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, null); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, null); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;

            var cra = new CellRangeAddress(1, 1, 2, 5);
            var cra1 = new CellRangeAddress(1, 2, 6, 6);
            var cra2 = new CellRangeAddress(1, 1, 7, 10);
            var cra3 = new CellRangeAddress(1, 2, 11, 11);
            var cra4 = new CellRangeAddress(1, 1, 12, 15);
            var cra5 = new CellRangeAddress(1, 2, 16, 16);

            var cra6 = new CellRangeAddress(1, 1, 18, 20);
            var cra7 = new CellRangeAddress(1, 1, 22, 24);

            sheet.AddMergedRegion(cra);
            sheet.AddMergedRegion(cra1);
            sheet.AddMergedRegion(cra2);
            sheet.AddMergedRegion(cra3);
            sheet.AddMergedRegion(cra4);
            sheet.AddMergedRegion(cra5);
            sheet.AddMergedRegion(cra6);
            sheet.AddMergedRegion(cra7);

            var celdasMerge = sheet.GetRow(1).GetCell(2);
            celdasMerge.SetCellValue("DISPONIBLE");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(6);
            celdasMerge.SetCellValue("TOTAL DISP");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(7);
            celdasMerge.SetCellValue("FORWARD");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(11);
            celdasMerge.SetCellValue("TOTAL FWR");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(12);
            celdasMerge.SetCellValue("NEW CROP");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge = sheet.GetRow(1).GetCell(16);
            celdasMerge.SetCellValue("TOTAL NC");
            celdasMerge.CellStyle = cellcolorTitles;
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(18);
            celdasMerge.SetCellValue("SOJA SUSTENTABLE");
            celdasMerge.CellStyle = cellcolorTitles;

            //celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(22);
            celdasMerge.SetCellValue("SOJA EPA");
            celdasMerge.CellStyle = cellcolorTitles;
            #endregion
            #region row2
            c = 0;
            row = sheet.CreateRow(r); r++;
            CrearCelda(row, c, null, null); c++;
            CrearCelda(row, c, "PRODUCTO", cellcolorTitles); c++;

            //DISPONIBLE    
            CrearCelda(row, c, "A Fijar", cellcolorTitles); c++;
            CrearCelda(row, c, "A Precio", cellcolorTitles); c++;
            CrearCelda(row, c, "Fijacion", cellcolorTitles); c++;
            CrearCelda(row, c, "MAT", cellcolorTitles); c++;
            c++;
            //FORWARD       
            CrearCelda(row, c, "A Fijar", cellcolorTitles); c++;
            CrearCelda(row, c, "A Precio", cellcolorTitles); c++;
            CrearCelda(row, c, "Fijacion", cellcolorTitles); c++;
            CrearCelda(row, c, "MAT", cellcolorTitles); c++;
            c++;
            //NEW CROP      
            CrearCelda(row, c, "A Fijar", cellcolorTitles); c++;
            CrearCelda(row, c, "A Precio", cellcolorTitles); c++;
            CrearCelda(row, c, "Fijacion", cellcolorTitles); c++;
            CrearCelda(row, c, "MAT", cellcolorTitles); c++;
            CrearCelda(row, c, null, cellcolorTitles); c++;
            c++;

            //SOJA SUSTENTABLE
            CrearCelda(row, c, "A Precio", cellcolorTitles);
            c++;
            CrearCelda(row, c, "A Fijar", cellcolorTitles);
            c++;
            CrearCelda(row, c, "Total", cellcolorTitles);
            c++;
            //CrearCelda(row, c, null, cellcolorTitles); c++;
            CrearCelda(row, c, null, null); c++;
            //SOJA EPA
            CrearCelda(row, c, "A Precio", cellcolorTitles); c++;
            CrearCelda(row, c, "A Fijar", cellcolorTitles); c++;
            CrearCelda(row, c, "Total", cellcolorTitles); c++;

            #endregion
            #region PosicionToneladas
            foreach (var material in model.ToneladasGranoTipo)
            {
                var posicionMaterial = model.PosicionCompras.Where(x => x.Material.ToLower() == material.Material.ToLower());
                var totalDisp = posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.DispFijac + y.DispAFijar + y.DispAPrecio)).Sum() + material.DispAgente;
                var totalForw = posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar + y.FrwAPrecio + y.FrwFijac)).Sum() + material.FrwAgente;
                var totalNewC = posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar + y.NewAPrecio + y.NewFijac)).Sum() + material.NewAgente;
                c = 0;
                row = sheet.CreateRow(r);
                CrearCelda(row, c, null, null); c++;

                //DISPONIBLE
                CrearCelda(row, c, material.Material, cellBorderStyleColumnTitles); c++;
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.DispAFijar)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;               //A Fijar 
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.DispAPrecio)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;              //A Precio 
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.DispFijac)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;                //Fijacion 
                CrearCelda(row, c, material.DispAgente.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                      //MAT 
                CrearCelda(row, c, totalDisp.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                                //Total 

                //FORWARD       
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.FrwAFijar)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;                //A Fijar
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.FrwAPrecio)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;               //A Precio
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.FrwFijac)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;                 //Fijacion
                CrearCelda(row, c, material.FrwAgente.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                       //MAT
                CrearCelda(row, c, totalForw.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                                //Total

                //NEW CROP
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.NewAFijar)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;                //A Fijar
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.NewAPrecio)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;               //A Precio
                CrearCelda(row, c, posicionMaterial.Select(x => x.PosicionKilos.Sum(y => y.NewFijac)).Sum().ToString("N0"), cellBorderStyleColumnTitles); c++;                 //Fijacion
                CrearCelda(row, c, material.NewAgente.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                       //MAT
                CrearCelda(row, c, totalNewC.ToString("N0"), cellBorderStyleColumnTitles); c++;                                                                                //Total

                c++;
                if (r == 3)
                {
                    CrearCelda(row, c, model.SojaSustentable.Precio.ToString("N0"), cellBorderStyleColumnTitles); c++;
                    CrearCelda(row, c, model.SojaSustentable.Fijar.ToString("N0"), cellBorderStyleColumnTitles); c++;
                    CrearCelda(row, c, model.SojaSustentable.Total.ToString("N0"), cellBorderStyleColumnTitles); c++;
                    CrearCelda(row, c, null, null); c++;
                    CrearCelda(row, c, model.SojaEPA.Precio.ToString("N0"), cellBorderStyleColumnTitles); c++;
                    CrearCelda(row, c, model.SojaEPA.Fijar.ToString("N0"), cellBorderStyleColumnTitles); c++;
                    CrearCelda(row, c, model.SojaEPA.Total.ToString("N0"), cellBorderStyleColumnTitles); c++;
                }
                r++;
            }
            #endregion

            #region PosicionCompras
            //Encabeza Posicion
            var col = 0;
            c = 1;
            row = sheet.CreateRow(r); r++;
            row = sheet.CreateRow(r); r++;
            //SOJA
            CrearCelda(row, c, null, colores[col]); c++;
            CrearCelda(row, c, null, colores[col]); c++;
            var merge = new CellRangeAddress(r - 1, r - 1, 1, 2);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(1);
            celda.SetCellValue("SOJA");
            celda.CellStyle = colores[col]; col++;
            //Maiz
            c++;
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 4, 5);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(4);
            celda.SetCellValue("MAIZ");
            celda.CellStyle = colores[col]; col++;
            c++;
            //Trigo Cam.
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 7, 8);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(7);
            celda.SetCellValue("TRIGO CAMARA");
            celda.CellStyle = colores[col];
            col++;
            c++;
            //Trigo Cal.
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 10, 11);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(10);
            celda.SetCellValue("TRIGO CALIDAD");
            celda.CellStyle = colores[col];
            col++;
            c++;
            //Trigo Grado2
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 13, 14);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(13);
            celda.SetCellValue("TRIGO GRADO 2");
            celda.CellStyle = colores[col];
            col++;
            c++;
            //Girasol
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 16, 17);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(16);
            celda.SetCellValue("GIRASOL");
            celda.CellStyle = colores[col];
            col++;
            c++;
            //Girasol Alto
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            celda = row.CreateCell(c); c++;
            celda.CellStyle = colores[col];
            merge = new CellRangeAddress(r - 1, r - 1, 19, 20);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(19);
            celda.SetCellValue("GIRASOL ALTO OLEICO");
            celda.CellStyle = colores[col];
            c++;
            col = 0;

            var filaPosicion = r;
            row = sheet.CreateRow(r); r++;
            var max = 0;
            foreach (var m in model.PosicionCompras)
            {
                max = max < m.PosicionKilos.Count() ? m.PosicionKilos.Count() : max;
            };
            for (var l = 0; l <= max + 1; l++)
            {
                row = sheet.CreateRow(r); r++;
            }
            c = 0;
            foreach (var material in model.PosicionCompras)
            {
                var f = filaPosicion;
                row = sheet.GetRow(f); f++;
                celda = row.CreateCell(c);
                CrearCelda(row, c + 1, "Posicion", colores[col]);
                CrearCelda(row, c + 2, "Ton", colores[col]);

                foreach (var mes in material.PosicionKilos.OrderBy(x => x.Anio).ThenBy(x => x.Mes))
                {
                    row = sheet.GetRow(f); f++;
                    celda = row.CreateCell(c);
                    CrearCelda(row, c + 1, (mes.Mes + "-" + mes.Anio.ToString()), cellBorderStyleColumnTitles);
                    CrearCelda(row, c + 2, material.PosicionKilos.Sum(x => x.KilosPesos + x.KilosDolares).ToString("N0"), cellBorderStyleColumnTitles);
                }
                row = sheet.GetRow(f);
                CrearCelda(row, c + 1, "Total", colores[col]);
                CrearCelda(row, c + 2, material.Total.ToString("N0"), colores[col]);
                c += 3;
                col++;
            }
            #endregion

            #region Hedge
            if (incluirHedge)
            {
                CrearTablaHedge(sheet, colores[8], estiloCeldasColumnTitles, model, colores[9], r);
                CrearTablaAgenteDeCompras(sheet, colores[10], estiloCeldasColumnTitles, model, r);
            }
            #endregion

            #region ToneladasKilos
            foreach (var moneda in model.PrecioCantidad)
            {
                row = sheet.GetRow(filaPosicion - 1); filaPosicion++;
                celda = row.CreateCell(c);
                CrearCelda(row, c + 1, moneda.Moneda, cellcolorTitles);
                CrearCelda(row, c + 2, moneda.Cantidad.HasValue ? moneda.Cantidad.Value.ToString("N2") : "0", cellBorderStyleColumnTitles);
            }
            #endregion
            for (var i = 0; i <= 30; i++)
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
            var listaTrigoCamara = posicion.Where(x => x.Material == "Trigo" && x.CalidadEspecial != "Especial").ToList();
            CrearSheets(sheet4, cellcolorTitles, cellStyleColumnTitles, listaTrigoCamara);

            var sheet5 = (HSSFSheet)workbook.CreateSheet("Posición Trigo Calidad");
            var listaTrigoCalidad = posicion.Where(x => x.Material == "Trigo" && x.CalidadEspecial == "Especial").ToList();
            CrearSheets(sheet5, cellcolorTitles, cellStyleColumnTitles, listaTrigoCalidad);

            var sheet6 = (HSSFSheet)workbook.CreateSheet("Posición Girasol");
            var listaGirasolCalidad = posicion.Where(x => x.Material == "Girasol").ToList();
            CrearSheets(sheet6, cellcolorTitles, cellStyleColumnTitles, listaGirasolCalidad);

            var sheet7 = (HSSFSheet)workbook.CreateSheet("Posición Girasol Alto Oleico");
            var listaGirasolAltoCalidad = posicion.Where(x => x.Material == "Girasol Alto Oleico").ToList();
            CrearSheets(sheet7, cellcolorTitles, cellStyleColumnTitles, listaGirasolAltoCalidad);
            #endregion
            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        internal static byte[] GenerarExcelReporteEvolucionFijacion(ReporteEvolucionFijacionModel model)
        {

            var c = 0;
            var r = 0;
            var workbook = new HSSFWorkbook();
            var sheet = (HSSFSheet)workbook.CreateSheet("Resumen");
            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);

            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.White.Index;
            cellBorderStyleColumnTitles.SetFont(fontBold);


            #region row1
            var row = sheet.CreateRow(r); r++;
            row = sheet.CreateRow(r); r++;
            //var celda = row.CreateCell(c); c++;


            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;

            var cra = new CellRangeAddress(1, 2, 0, 0);//año
            sheet.AddMergedRegion(cra);
            var cra1 = new CellRangeAddress(1, 2, 1, 1);//mes
            sheet.AddMergedRegion(cra1);
            var cra2 = new CellRangeAddress(1, 1, 2, 4);//total
            sheet.AddMergedRegion(cra2);
            var cra3 = new CellRangeAddress(1, 2, 5, 5);//total
            sheet.AddMergedRegion(cra3);

            var celdasMerge = sheet.GetRow(1).GetCell(0);
            celdasMerge.SetCellValue("Año");
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(1);
            celdasMerge.SetCellValue("Mes");
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            celdasMerge = sheet.GetRow(1).GetCell(2);
            celdasMerge.SetCellValue("TOTAL");

            celdasMerge = sheet.GetRow(1).GetCell(5);
            celdasMerge.SetCellValue("%");
            celdasMerge.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            #endregion

            #region row2

            row = sheet.CreateRow(r); r++;

            CrearCelda(row, 0, "", cellBorderStyleColumnTitles);
            CrearCelda(row, 1, "", cellBorderStyleColumnTitles);
            CrearCelda(row, 2, "A fijar", cellBorderStyleColumnTitles);
            CrearCelda(row, 3, "Fijaciones", cellBorderStyleColumnTitles);
            CrearCelda(row, 4, "Semana", cellBorderStyleColumnTitles);
            CrearCelda(row, 5, "", cellBorderStyleColumnTitles);


            #endregion

            #region materiales      

            var colInicial = 3;
            foreach (var materiales in model.tablero.GroupBy(a => a.MaterialId))
            {
                r = 3;
                var anio = "";
                foreach (var meses in materiales)
                {

                    if (meses.MaterialId == 0)
                    {
                        row = sheet.CreateRow(r); r++;

                        c = 0;
                        CrearCelda(row, c, meses.Anio.ToString() == anio ? "" : meses.Anio.ToString(), null); c++;
                        anio = meses.Anio.ToString();
                        CrearCelda(row, c, meses.Mes.ToString(), null); c++;
                        CrearCelda(row, c, meses.TotalAfijar == 0 ? "-" : String.Format("{0:n0}", meses.TotalAfijar), null); c++;
                        CrearCelda(row, c, meses.TotalFijacion == 0 ? "-" : String.Format("{0:n0}", meses.TotalFijacion), null); c++;
                        CrearCelda(row, c, meses.UltimaSemana == 0 ? "-" : String.Format("{0:n0}", meses.UltimaSemana), null); c++;
                        CrearCelda(row, c, (Double.IsNaN(meses.Porcentaje) ? 0 : meses.Porcentaje).ToString() + "%", null); c++;

                    }
                    else
                    {
                        c = colInicial;
                        if (r == 3)
                        {
                            CrearCelda(sheet.GetRow(1), c, null, cellBorderStyleColumnTitles);
                            CrearCelda(sheet.GetRow(1), c + 1, null, cellBorderStyleColumnTitles);
                            CrearCelda(sheet.GetRow(1), c + 2, null, cellBorderStyleColumnTitles);
                            var cra5 = new CellRangeAddress(1, 1, c, c + 2);//material
                            sheet.AddMergedRegion(cra5);
                            var titulo = sheet.GetRow(1).GetCell(c);
                            titulo.SetCellValue(meses.Material.ToUpper());
                            titulo.CellStyle = cellBorderStyleColumnTitles;
                            CrearCelda(sheet.GetRow(2), c, "A fijar", cellBorderStyleColumnTitles);
                            CrearCelda(sheet.GetRow(2), c + 1, "Fijaciones", cellBorderStyleColumnTitles);
                            CrearCelda(sheet.GetRow(2), c + 2, "Semana", cellBorderStyleColumnTitles);
                        }

                        CrearCelda(sheet.GetRow(r), c, meses.TotalAfijar == 0 ? "-" : String.Format("{0:n0}", meses.TotalAfijar), null); c++;
                        CrearCelda(sheet.GetRow(r), c, meses.TotalFijacion == 0 ? "-" : String.Format("{0:n0}", meses.TotalFijacion), null); c++;
                        CrearCelda(sheet.GetRow(r), c, meses.UltimaSemana == 0 ? "-" : String.Format("{0:n0}", meses.UltimaSemana), null); c++;
                        r++;
                    }
                }
                colInicial += 4;

            }
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
                celda.SetCellValue(material.Cantidad);
                celda = row.CreateCell(8);
                celda.SetCellValue(material.CantidadCamiones);
                celda = row.CreateCell(9);
                celda.SetCellValue(material.Campana);
                celda = row.CreateCell(10);
                celda.SetCellValue(material.FechaDesde);
                celda = row.CreateCell(11);
                celda.SetCellValue(material.FechaHasta);
                celda = row.CreateCell(12);
                celda.SetCellValue((double)material.Precio);
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
                celda.SetCellValue(material.MercaderiaEnDeposito);
                celda = row.CreateCell(35);
                celda.SetCellValue(material.Observacion);
                celda = row.CreateCell(36);
            }
            for (var i = 0; i < 36; i++)
            {
                sheet.AutoSizeColumn(i);
            }
        }

        private static void CrearCelda(IRow row, int posicion, string valor, ICellStyle estilo)
        {
            var celda = row.CreateCell(posicion);
            double v;
            double.TryParse(valor, out v);
            if (v != 0)
            {
                celda.SetCellValue(v);
            }
            else
            {
                celda.SetCellValue(valor);
            }
            celda.CellStyle = estilo;
        }

        private static void CrearTablaHedge(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, ReporteCompraNetModel model, ICellStyle cellcolorTitlesObjectivos, int fila)
        {
            var row = sheet.CreateRow(fila);
            int iC = 0;

            CrearCelda(row, iC + 1, "HEDGE", cellcolorTitles);
            CrearCelda(row, iC + 2, null, cellcolorTitles);
            CrearCelda(row, iC + 3, null, cellcolorTitles);
            CrearCelda(row, iC + 4, null, cellcolorTitles);
            var merge = new CellRangeAddress(fila, fila, 1, 4);

            row = sheet.GetRow(fila);
            CrearCelda(row, iC + 7, "Dia", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 8, "Objetivo", cellcolorTitlesObjectivos);

            fila++;
            sheet.AddMergedRegion(merge);
            row = sheet.CreateRow(fila);
            CrearCelda(row, iC + 1, "Producto", cellcolorTitles);
            CrearCelda(row, iC + 2, "Ddisponible", cellcolorTitles);
            CrearCelda(row, iC + 3, "Forward", cellcolorTitles);
            CrearCelda(row, iC + 4, "New Crop", cellcolorTitles);

            int iterFilas = 1;
            foreach (HedgeMaterialModel producto in model.HedgeMaterial)
            {
                row = sheet.CreateRow(fila + iterFilas);
                CrearCelda(row, iC + 1, producto.MaterialDescripcion, cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 2, producto.Disponible.ToString(), cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 3, producto.Forward.ToString(), cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 4, producto.NewCrop.ToString(), cellBorderStyleColumnTitles);
                iterFilas++;
            }


            row = sheet.GetRow(fila); fila++;
            CrearCelda(row, iC + 6, "Pricing", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 7, model.HedgeObjetivo.PricingCumplido.ToString(), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, model.HedgeObjetivo.PricingObjetivo.ToString(), cellBorderStyleColumnTitles);

            row = sheet.GetRow(fila);
            if (row == null)
            {
                row = sheet.CreateRow(fila);
            }
            CrearCelda(row, iC + 6, "A remitir", cellcolorTitlesObjectivos);
            CrearCelda(row, iC + 7, model.HedgeObjetivo.RemitirCumplido.ToString(), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, model.HedgeObjetivo.RemitirObjetivo.ToString(), cellBorderStyleColumnTitles);

            row = sheet.GetRow(fila + 2);
            if (row == null)
            {
                row = sheet.CreateRow(fila + 2);
            }

            CrearCelda(row, iC + 7, "Hedge TC: $" + model.TCPromedioDto.PromedioTC.ToString("n2"), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, "$" + model.TCPromedioDto.TotalTC.ToString("n2"), cellBorderStyleColumnTitles);

        }
        private static void CrearTablaAgenteDeCompras(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, ReporteCompraNetModel model, int fila)
        {
            int iC = 0;
            var row = sheet.GetRow(fila);
            if (row == null)
            {
                row = sheet.CreateRow(fila);
            }
            var cra = new CellRangeAddress(fila, fila, 10, 10 + model.AgenteCompras.ListaOperadores.Count + 3);
            sheet.AddMergedRegion(cra);
            fila++;
            CrearCelda(row, iC + 10, "AGENTE DE COMPRAS", cellcolorTitles);
            for (var iter = 11; iter <= 10 + model.AgenteCompras.ListaOperadores.Count + 3; iter++)
            {
                CrearCelda(row, iter, null, cellcolorTitles);
            }
            row = sheet.GetRow(fila);
            if (row == null)
            {
                row = sheet.CreateRow(fila);
            }
            CrearCelda(row, iC + 10, "Agente de Compra", cellcolorTitles);
            CrearCelda(row, iC + 11, "Producto", cellcolorTitles);
            CrearCelda(row, iC + 12, "Posicion", cellcolorTitles);
            int posicionOperador = 1;
            foreach (AgenteCompraDto.OperadorCantidad agente in model.AgenteCompras.ListaOperadores)
            {
                CrearCelda(row, iC + 12 + posicionOperador, agente.OperadorDesc, cellcolorTitles);
                posicionOperador++;
            }
            CrearCelda(row, iC + 12 + posicionOperador, "Total", cellcolorTitles);

            int filaOperador = 1;
            foreach (AgenteCompraDto agente in model.AgenteCompras.ListaAgenteCompras)
            {
                posicionOperador = 0;

                row = sheet.GetRow(fila + filaOperador);
                if (row == null)
                {
                    row = sheet.CreateRow(fila + filaOperador);
                }
                CrearCelda(row, iC + 10 + posicionOperador, agente.TipoAgenteDesc, cellBorderStyleColumnTitles);
                CrearCelda(row, iC + 11 + posicionOperador, agente.MaterialDesc, cellBorderStyleColumnTitles);
                posicionOperador++;

                var pos = agente.Posicion.Split('.');
                CrearCelda(row, iC + 11 + posicionOperador, (EnumMeses)Enum.ToObject(typeof(EnumMeses), Int32.Parse(pos[0])) + " - " + pos[1], cellBorderStyleColumnTitles);
                posicionOperador++;

                foreach (var op in model.AgenteCompras.ListaOperadores)
                {
                    var cantidad = agente.Operador.Where(x => x.OperadorId == op.OperadorId).Select(x => x.Cantidad.ToString("N0")).FirstOrDefault();
                    CrearCelda(row, iC + 11 + posicionOperador, cantidad != null ? cantidad : "0", cellBorderStyleColumnTitles);
                    posicionOperador++;
                }
                CrearCelda(row, iC + 11 + posicionOperador, agente.Operador.Sum(x => x.Cantidad).ToString("N0"), cellBorderStyleColumnTitles);
                filaOperador++;
            }
        }

        public static byte[] ExcelModeloAltaMasiva(List<MaterialIni> materiales, List<CentroIni> destinos)
        {
            //Create workbook
            IWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("AltaMasiva");

            //Create dropdown list materiales
            IDataValidationHelper validationHelperMaterial = new XSSFDataValidationHelper(sheet);
            CellRangeAddressList addressListMaterial = new CellRangeAddressList(1, 999, 2, 2);
            IDataValidationConstraint constraintMaterial = validationHelperMaterial.CreateExplicitListConstraint(materiales.Where(x => x.MaterialId != 5).Select(a => a.Descripcion).ToArray());
            IDataValidation dataValidationMaterial = validationHelperMaterial.CreateValidation(constraintMaterial, addressListMaterial);
            dataValidationMaterial.SuppressDropDownArrow = true;
            sheet.AddValidationData(dataValidationMaterial);

            //Create dropdown list Clasificacion
            IDataValidationHelper validationHelperClasificacion = new XSSFDataValidationHelper(sheet);
            CellRangeAddressList addressListClasificacion = new CellRangeAddressList(1, 999, 9, 9);
            IDataValidationConstraint constraintClasificacion = validationHelperClasificacion.CreateExplicitListConstraint(new string[] { "Acopiador", "Otros", "Productor" });
            IDataValidation dataValidationClasificacion = validationHelperClasificacion.CreateValidation(constraintClasificacion, addressListClasificacion);
            dataValidationClasificacion.SuppressDropDownArrow = true;
            sheet.AddValidationData(dataValidationClasificacion);

            //Create dropdown list Centros
            IDataValidationHelper validationHelper = new XSSFDataValidationHelper(sheet);
            CellRangeAddressList addressList = new CellRangeAddressList(1, 999, 12, 12);
            IDataValidationConstraint constraint = validationHelper.CreateExplicitListConstraint(destinos.Where(a => a.Id != 10).Select(a => a.Descripcion).ToArray());
            IDataValidation dataValidation = validationHelper.CreateValidation(constraint, addressList);
            dataValidation.SuppressDropDownArrow = true;
            sheet.AddValidationData(dataValidation);

            //Create dropdown list materiales
            IDataValidationHelper validationHelperX = new XSSFDataValidationHelper(sheet);
            CellRangeAddressList addressListX = new CellRangeAddressList(1, 999, 10, 11);
            IDataValidationConstraint constraintX = validationHelperX.CreateExplicitListConstraint(new string[] { "x" });
            IDataValidation dataValidationX = validationHelperX.CreateValidation(constraintX, addressListX);
            dataValidationX.SuppressDropDownArrow = true;
            sheet.AddValidationData(dataValidationX);

            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.White.Index;
            cellBorderStyleColumnTitles.SetFont(fontBold);

            var c = 0;
            var r = 0;

            var row = sheet.CreateRow(r); r++;

            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            var celda = sheet.GetRow(0).GetCell(0);
            celda.SetCellValue("Contrato corredor");
            sheet.AutoSizeColumn(0);
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(1);
            celda.SetCellValue("Contrato vendedor");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(2);
            celda.SetCellValue("Grano");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(3);
            celda.SetCellValue("Cosecha");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(4);
            celda.SetCellValue("Fecha Operación");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(5);
            celda.SetCellValue("Fecha Desde Entrega");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(6);
            celda.SetCellValue("Fecha Vto.Entrega");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(7);
            celda.SetCellValue("TN");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(8);
            celda.SetCellValue("CUIT Vendedor");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(9);
            celda.SetCellValue("Clasificacion");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(10);
            celda.SetCellValue("Plan Canje");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(11);
            celda.SetCellValue("Consignatario");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(12);
            celda.SetCellValue("Destino");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(13);
            celda.SetCellValue("Procedencia");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(14);
            celda.SetCellValue("Provincia");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);
            sheet.AutoSizeColumn(3);
            sheet.AutoSizeColumn(4);
            sheet.AutoSizeColumn(5);
            sheet.AutoSizeColumn(6);
            sheet.AutoSizeColumn(7);
            sheet.AutoSizeColumn(8);
            sheet.AutoSizeColumn(9);
            sheet.AutoSizeColumn(10);
            sheet.AutoSizeColumn(11);
            sheet.AutoSizeColumn(12);
            sheet.AutoSizeColumn(13);
            sheet.AutoSizeColumn(14);

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        public static byte[] ExcelReportePagosDiferidos(ResultReportePagosDiferidos datos, DateTime desde, DateTime hasta)
        {
            if (datos == null || datos.Contratos == null || datos.Contratos.Count == 0)
            {
                return null;
            }

            //Create workbook
            IWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("Diferidos");
            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);

            byte[] celeste = new byte[3] { 155, 184, 230 };
            byte[] celesteOscuro = new byte[3] { 142, 169, 219 };
            byte[] rojo = new byte[3] { 233, 145, 127 };
            byte[] verde = new byte[3] { 169, 208, 142 };

            ICellStyle cellStyleDouble = workbook.CreateCellStyle();
            cellStyleDouble.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            ICellStyle cellStylePorcentaje = workbook.CreateCellStyle();
            cellStylePorcentaje.DataFormat = workbook.CreateDataFormat().GetFormat("0.00%");


            var estiloNegrita = workbook.CreateCellStyle();
            estiloNegrita.BorderBottom = BorderStyle.Thin;
            estiloNegrita.BorderTop = BorderStyle.Thin;
            estiloNegrita.BorderLeft = BorderStyle.Thin;
            estiloNegrita.BorderRight = BorderStyle.Thin;
            estiloNegrita.SetFont(fontBold);
            estiloNegrita.Alignment = HorizontalAlignment.Center;

            var estiloBordes = workbook.CreateCellStyle();
            estiloBordes.BorderBottom = BorderStyle.Thin;
            estiloBordes.BorderTop = BorderStyle.Thin;
            estiloBordes.BorderLeft = BorderStyle.Thin;
            estiloBordes.BorderRight = BorderStyle.Thin;
            estiloBordes.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

            var estilofondoGris = workbook.CreateCellStyle();
            estilofondoGris.BorderBottom = BorderStyle.Thin;
            estilofondoGris.BorderTop = BorderStyle.Thin;
            estilofondoGris.BorderLeft = BorderStyle.Thin;
            estilofondoGris.BorderRight = BorderStyle.Thin;
            estilofondoGris.Alignment = HorizontalAlignment.Center;
            estilofondoGris.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            estilofondoGris.FillPattern = FillPattern.SolidForeground;
            estilofondoGris.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            estilofondoGris.SetFont(fontBold);

            var estilofondoGrisSinBorde = workbook.CreateCellStyle();
            estilofondoGrisSinBorde.BorderBottom = BorderStyle.None;
            estilofondoGrisSinBorde.BorderTop = BorderStyle.None;
            estilofondoGrisSinBorde.BorderLeft = BorderStyle.None;
            estilofondoGrisSinBorde.BorderRight = BorderStyle.None;
            estilofondoGrisSinBorde.Alignment = HorizontalAlignment.Center;
            estilofondoGrisSinBorde.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            estilofondoGrisSinBorde.FillPattern = FillPattern.SolidForeground;
            estilofondoGrisSinBorde.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            estilofondoGrisSinBorde.SetFont(fontBold);

            var estilofondoGrisOscuroSinBorde = workbook.CreateCellStyle();
            estilofondoGrisOscuroSinBorde.BorderBottom = BorderStyle.None;
            estilofondoGrisOscuroSinBorde.BorderTop = BorderStyle.None;
            estilofondoGrisOscuroSinBorde.BorderLeft = BorderStyle.None;
            estilofondoGrisOscuroSinBorde.BorderRight = BorderStyle.None;
            estilofondoGrisOscuroSinBorde.Alignment = HorizontalAlignment.Center;
            estilofondoGrisOscuroSinBorde.FillForegroundColor = IndexedColors.Grey50Percent.Index;
            estilofondoGrisOscuroSinBorde.FillPattern = FillPattern.SolidForeground;
            estilofondoGrisOscuroSinBorde.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");

            var estilofondoCelesteOscuro = (XSSFCellStyle)workbook.CreateCellStyle();
            estilofondoCelesteOscuro.BorderBottom = BorderStyle.None;
            estilofondoCelesteOscuro.BorderTop = BorderStyle.None;
            estilofondoCelesteOscuro.BorderLeft = BorderStyle.None;
            estilofondoCelesteOscuro.BorderRight = BorderStyle.None;
            estilofondoCelesteOscuro.Alignment = HorizontalAlignment.Center;
            estilofondoCelesteOscuro.FillPattern = FillPattern.SolidForeground;
            estilofondoCelesteOscuro.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            estilofondoCelesteOscuro.SetFillForegroundColor(new XSSFColor(celesteOscuro));

            var estilofondoCeleste = (XSSFCellStyle)workbook.CreateCellStyle();
            estilofondoCeleste.BorderBottom = BorderStyle.None;
            estilofondoCeleste.BorderTop = BorderStyle.None;
            estilofondoCeleste.BorderLeft = BorderStyle.None;
            estilofondoCeleste.BorderRight = BorderStyle.None;
            estilofondoCeleste.Alignment = HorizontalAlignment.Center;
            estilofondoCeleste.FillPattern = FillPattern.SolidForeground;
            estilofondoCeleste.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            estilofondoCeleste.SetFillForegroundColor(new XSSFColor(celeste));

            var estilofondoCelesteNegrita = (XSSFCellStyle)workbook.CreateCellStyle();
            estilofondoCelesteNegrita.BorderBottom = BorderStyle.None;
            estilofondoCelesteNegrita.BorderTop = BorderStyle.None;
            estilofondoCelesteNegrita.BorderLeft = BorderStyle.None;
            estilofondoCelesteNegrita.BorderRight = BorderStyle.None;
            estilofondoCelesteNegrita.Alignment = HorizontalAlignment.Center;
            estilofondoCelesteNegrita.SetFillForegroundColor(new XSSFColor(celeste));
            estilofondoCelesteNegrita.FillPattern = FillPattern.SolidForeground;
            estilofondoCelesteNegrita.DataFormat = workbook.CreateDataFormat().GetFormat("#,##0.00");
            estilofondoCelesteNegrita.SetFont(fontBold);

            var estilofondoVerdeSinBorde = (XSSFCellStyle)workbook.CreateCellStyle();
            estilofondoVerdeSinBorde.BorderBottom = BorderStyle.None;
            estilofondoVerdeSinBorde.BorderTop = BorderStyle.None;
            estilofondoVerdeSinBorde.BorderLeft = BorderStyle.None;
            estilofondoVerdeSinBorde.BorderRight = BorderStyle.None;
            estilofondoVerdeSinBorde.Alignment = HorizontalAlignment.Center;
            estilofondoVerdeSinBorde.SetFillForegroundColor(new XSSFColor(verde));
            estilofondoVerdeSinBorde.FillPattern = FillPattern.SolidForeground;

            var estilofondoRojoSinBorde = (XSSFCellStyle)workbook.CreateCellStyle();
            estilofondoRojoSinBorde.BorderBottom = BorderStyle.None;
            estilofondoRojoSinBorde.BorderTop = BorderStyle.None;
            estilofondoRojoSinBorde.BorderLeft = BorderStyle.None;
            estilofondoRojoSinBorde.BorderRight = BorderStyle.None;
            estilofondoRojoSinBorde.Alignment = HorizontalAlignment.Center;
            estilofondoRojoSinBorde.SetFillForegroundColor(new XSSFColor(rojo));
            estilofondoRojoSinBorde.FillPattern = FillPattern.SolidForeground;

            var c = 0;
            var r = 0;

            var row = sheet.CreateRow(r); r++;

            CrearCelda(row, c, null, estiloNegrita); c++;
            CrearCelda(row, c, null, estilofondoCelesteOscuro); c++;

            var celda = sheet.GetRow(0).GetCell(0);
            celda.SetCellValue("Fecha");
            celda = sheet.GetRow(0).GetCell(1);
            celda.SetCellValue(DateTime.Now.ToString("dd-MM-yyyy"));
            row = sheet.CreateRow(r++); c = 0;
            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, c++, "Capital", estilofondoGris);
            CrearCelda(row, c++, Convert.ToDouble(datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital)).ToString(), estiloBordes);
            CrearCelda(row, 3, "Fecha Desde", estilofondoCelesteNegrita);
            CrearCelda(row, 4, desde.ToString("dd-MM-yyyy"), estilofondoCeleste);

            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, 0, "Tasa PP", estilofondoGris);
            CrearCelda(row, 1, Convert.ToDouble(
                datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital) != 0 ?
                    datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital * a.TEA) / datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital)
                :
                    0
                ).ToString(), estiloBordes);
            CrearCelda(row, 3, "Fecha Hasta", estilofondoCelesteNegrita);
            CrearCelda(row, 4, hasta.ToString("dd-MM-yyyy"), estilofondoCeleste);

            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, 0, "Vida PP", estilofondoGris);
            CrearCelda(row, 1, Convert.ToDouble(Convert.ToDouble(datos.Contratos.Sum(a => a.AlVencimiento)) / datos.Contratos.Count()).ToString(), estiloBordes);
            CrearCelda(row, 3, "Intereses Dv", estilofondoCelesteNegrita);
            CrearCelda(row, 4, Convert.ToDouble(datos.Contratos.Sum(a => a.DevengadoMes)).ToString(), estilofondoCeleste);

            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, 0, "Plazo PP", estilofondoGris);
            CrearCelda(row, 1, Convert.ToDouble(datos.Contratos.Sum(a => a.Plazo) / datos.Contratos.Count()).ToString(), estiloBordes);

            row = sheet.CreateRow(r++); c = 0;
            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, 18, "Fecha Desde", estilofondoCelesteNegrita);
            CrearCelda(row, 19, desde.ToString("dd-MM-yyyy"), estilofondoCeleste);

            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, 18, "Fecha Hasta", estilofondoCelesteNegrita);
            CrearCelda(row, 19, hasta.ToString("dd-MM-yyyy"), estilofondoCeleste);

            row = sheet.CreateRow(r++); c = 0;
            for (int i = 0; i < 17; i++)
            {
                CrearCelda(row, i, "", estilofondoGrisOscuroSinBorde);
            }

            CrearCelda(row, 18, null, null);
            CrearCelda(row, 19, null, null);
            CrearCelda(row, 20, null, null);
            var cra = new CellRangeAddress(row.RowNum, row.RowNum, 18, 20);
            sheet.AddMergedRegion(cra);
            celda = sheet.GetRow(row.RowNum).GetCell(18);
            celda.SetCellValue("DEVENGAMIENTO DE INTERESES");
            celda.CellStyle = estilofondoGrisOscuroSinBorde;

            row = sheet.CreateRow(r++); c = 0;
            CrearCelda(row, c++, "N° Cto", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Vendedor", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Corredor", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Estado", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Tn", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Precio USD", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "TC", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Capital", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "TNA", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "TEA", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Toma", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Plazo", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Vto", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "al Vto", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Capital + Intereses", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Int. Totales", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Int. X día", estilofondoGrisSinBorde);
            CrearCelda(row, c++, null, null);
            CrearCelda(row, c++, "Acumulado mes ant.", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "M2M Mes", estilofondoGrisSinBorde);
            CrearCelda(row, c++, "Devegado Mes", estilofondoGrisSinBorde);
            CrearCelda(row, c++, null, null);

            foreach (var dia in datos.Contratos.First().Dias)
            {
                CrearCelda(row, c++, dia.Dia.ToString("dd/MM"), estilofondoGrisSinBorde);
            }

            foreach (var cont in datos.Contratos)
            {
                row = sheet.CreateRow(r++); c = 0;
                CrearCelda(row, c++, cont.ContratoSAP, null);
                CrearCelda(row, c++, cont.Vendedor, null);
                CrearCelda(row, c++, cont.Corredor, null);
                CrearCelda(row, c++, cont.Estado, cont.Estado == "Vigente" ? estilofondoVerdeSinBorde : estilofondoRojoSinBorde);
                CrearCelda(row, c++, cont.Tn.ToString(), null);
                sheet.GetRow(row.RowNum).GetCell(c - 1).SetCellType(CellType.Numeric);
                CrearCelda(row, c++, cont.PrecioUSD.ToString(), null);
                sheet.GetRow(row.RowNum).GetCell(c - 1).CellStyle = cellStyleDouble;
                CrearCelda(row, c++, cont.TipoCambio.ToString(), null);
                CrearCelda(row, c++, cont.Capital.ToString(), null);
                sheet.GetRow(row.RowNum).GetCell(c - 1).CellStyle = cellStyleDouble;
                CrearCelda(row, c++, cont.TNA.ToString(), cellStylePorcentaje);
                CrearCelda(row, c++, cont.TEA.ToString(), cellStylePorcentaje);
                CrearCelda(row, c++, cont.Toma.ToString("dd-MM-yyyy"), null);
                CrearCelda(row, c++, cont.Plazo.ToString(), null);
                CrearCelda(row, c++, cont.Vencimiento.ToString("dd-MM-yyyy"), cellStyleDouble);
                CrearCelda(row, c++, cont.AlVencimiento.ToString(), cellStyleDouble);
                CrearCelda(row, c++, cont.CapitalMasIntereses.ToString(), cellStyleDouble);
                CrearCelda(row, c++, cont.InteresesTotales.ToString(), cellStyleDouble);
                CrearCelda(row, c++, cont.InteresesPorDia.ToString(), cellStyleDouble);
                CrearCelda(row, c++, null, null);
                CrearCelda(row, c++, cont.AcumuladoMesAnterior.ToString(), cellStyleDouble);
                CrearCelda(row, c++, cont.M2MMes.ToString(), cellStyleDouble);
                CrearCelda(row, c++, cont.DevengadoMes.ToString(), cellStyleDouble);
                CrearCelda(row, c++, null, null);

                foreach (var dia in cont.Dias)
                {
                    CrearCelda(row, c++, dia.Importe == 0 ? "-" : dia.Importe.ToString(), cellStyleDouble);
                }
            }

            row = sheet.CreateRow(r++); c = 0;
            row = sheet.CreateRow(r++); c = 0;
            for (int i = 0; i < 17; i++)
            {
                if (i != 4 && i != 5 && i != 7 && i != 11 && i != 16)
                {
                    CrearCelda(row, i, "", estilofondoGrisSinBorde);
                }
            }
            var totaltn = datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Tn);
            CrearCelda(row, 4, totaltn == 0 ? "-" : totaltn.ToString(), estilofondoGrisSinBorde);
            var total = datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.PrecioUSD);
            CrearCelda(row, 5, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            total = datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital);
            CrearCelda(row, 7, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            if (datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital) != 0)
            {
                total = datos.Contratos.Sum(a => a.Plazo * a.Capital) / datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital);
            }
            else
            {
                total = 0;
            }
            CrearCelda(row, 11, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            if (datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital) != 0)
            {
                total = datos.Contratos.Sum(a => a.AlVencimiento * a.Capital) / datos.Contratos.Where(a => a.Estado == "Vigente").Sum(a => a.Capital);
            }
            else
            {
                total = 0;
            }
            CrearCelda(row, 13, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            total = datos.Contratos.Sum(a => a.AcumuladoMesAnterior);
            CrearCelda(row, 18, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            sheet.GetRow(row.RowNum).GetCell(18).CellStyle = estilofondoGrisSinBorde;
            total = datos.Contratos.Sum(a => a.M2MMes);
            CrearCelda(row, 19, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            sheet.GetRow(row.RowNum).GetCell(19).CellStyle = estilofondoGrisSinBorde;
            total = datos.Contratos.Sum(a => a.DevengadoMes);
            CrearCelda(row, 20, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            sheet.GetRow(row.RowNum).GetCell(20).CellStyle = estilofondoGrisSinBorde;
            c = 21;
            CrearCelda(row, c++, null, null);

            foreach (var dia in datos.Contratos.First().Dias)
            {
                total = datos.Contratos.Sum(x => x.Dias.Where(a => a.Dia == dia.Dia).Sum(a => a.Importe));
                CrearCelda(row, c++, total == 0 ? "-" : total.ToString(), estilofondoGrisSinBorde);
            }


            for (int i = 0; i < 60; i++)
            {
                sheet.AutoSizeColumn(i);
            }

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }

        public static byte[] GenerarExcelLocalidades(ReporteLocalidadesModel model)
        {
            //Create workbook
            IWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("Localidades");


            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.White.Index;
            cellBorderStyleColumnTitles.SetFont(fontBold);

            var c = 0;
            var r = 0;

            var row = sheet.CreateRow(r); r++;

            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;

            var celda = sheet.GetRow(0).GetCell(0);

            celda.SetCellValue("Cod. Localidad");
            sheet.AutoSizeColumn(0);
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(1);
            celda.SetCellValue("Nombre");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(2);
            celda.SetCellValue("Provincia");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(3);
            celda.SetCellValue("Partido");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(4);
            celda.SetCellValue("Partido ID");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(5);
            celda.SetCellValue("Provincia ID");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);
            sheet.AutoSizeColumn(3);
            sheet.AutoSizeColumn(4);
            sheet.AutoSizeColumn(5);


            foreach (var localidad in model.tablero)
            {
                row = sheet.CreateRow(r); r++;

                c = 0;
                CrearCelda(row, c, localidad.CodLocalidad, null); c++;
                CrearCelda(row, c, localidad.Nombre, null); c++;
                CrearCelda(row, c, localidad.NombreProvincia, null); c++;
                CrearCelda(row, c, localidad.Descripcion != null ? localidad.Descripcion.ToString() : "", null); c++;
                CrearCelda(row, c, localidad.PartidoId.ToString(), null); c++;
                CrearCelda(row, c, localidad.ProvinciaId.ToString(), null); c++;
            }

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }


        public static byte[] GenerarExcelProveedorComerciales(ReporteProveedoresModel model)
        {
            //Create workbook
            IWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("Comerciales");


            var stylebold = workbook.CreateCellStyle();
            var fontBold = workbook.CreateFont();
            fontBold.Boldweight = (short)FontBoldWeight.Bold;
            stylebold.SetFont(fontBold);
            var cellBorderStyleColumnTitles = workbook.CreateCellStyle();
            cellBorderStyleColumnTitles.BorderBottom = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderTop = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderLeft = BorderStyle.Thin;
            cellBorderStyleColumnTitles.BorderRight = BorderStyle.Thin;
            cellBorderStyleColumnTitles.Alignment = HorizontalAlignment.Center;
            cellBorderStyleColumnTitles.FillForegroundColor = IndexedColors.White.Index;
            cellBorderStyleColumnTitles.SetFont(fontBold);

            var c = 0;
            var r = 0;

            var row = sheet.CreateRow(r); r++;

            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;
            CrearCelda(row, c, null, cellBorderStyleColumnTitles); c++;

            var celda = sheet.GetRow(0).GetCell(0);

            celda.SetCellValue("CUIT");
            sheet.AutoSizeColumn(0);
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(1);
            celda.SetCellValue("Razon Social");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(2);
            celda.SetCellValue("Estado Home");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(3);
            celda.SetCellValue("Estado");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;
            celda = sheet.GetRow(0).GetCell(4);
            celda.SetCellValue("Comercial Asignado");
            celda.CellStyle.VerticalAlignment = VerticalAlignment.Center;

            sheet.AutoSizeColumn(0);
            sheet.AutoSizeColumn(1);
            sheet.AutoSizeColumn(2);
            sheet.AutoSizeColumn(3);
            sheet.AutoSizeColumn(4);


            var estiloNegrita = workbook.CreateCellStyle();
            estiloNegrita.BorderBottom = BorderStyle.Thin;
            estiloNegrita.BorderTop = BorderStyle.Thin;
            estiloNegrita.BorderLeft = BorderStyle.Thin;
            estiloNegrita.BorderRight = BorderStyle.Thin;
            estiloNegrita.SetFont(fontBold);
            estiloNegrita.Alignment = HorizontalAlignment.Center;

            foreach (var comercial in model.tablero)
            {
                row = sheet.CreateRow(r); r++;

                c = 0;
                CrearCelda(row, c, comercial.Cuit, estiloNegrita); c++;
                CrearCelda(row, c, comercial.RazonSocial, estiloNegrita); c++;
                CrearCelda(row, c, comercial.EstadoHome, estiloNegrita); c++;
                CrearCelda(row, c, comercial.Estado, estiloNegrita); c++;
                CrearCelda(row, c, comercial.ComercialAsignado, estiloNegrita); c++;
            }

            using (var fileData = new MemoryStream())
            {
                workbook.Write(fileData);
                return fileData.ToArray();
            }
        }
    }
}