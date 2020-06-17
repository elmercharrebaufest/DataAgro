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
            var celda = row.CreateCell(c);c++;

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

            var cra = new CellRangeAddress(1, 1, 2, 5);
            var cra1 = new CellRangeAddress(1, 2, 6, 6);
            var cra2 = new CellRangeAddress(1, 1, 7, 10);
            var cra3 = new CellRangeAddress(1, 2,11,11);
            var cra4 = new CellRangeAddress(1, 1, 12, 15);
            var cra5 = new CellRangeAddress(1, 2, 16, 16);

            var cra6 = new CellRangeAddress(1, 1, 18, 20);
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
            #endregion
            #region row2
            c = 0;
            row = sheet.CreateRow(r); r++;
            CrearCelda(row, c, null, null);c++;
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
            var merge = new CellRangeAddress(r-1, r-1, 1, 2);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r - 1).GetCell(1);
            celda.SetCellValue("SOJA");
            celda.CellStyle = colores[col]; col++;
            //Maiz
            c++;
            celda = row.CreateCell(c);c++;
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
            merge = new CellRangeAddress(r - 1, r - 1, 19,20);
            sheet.AddMergedRegion(merge);
            celda = sheet.GetRow(r-1).GetCell(19);
            celda.SetCellValue("GIRASOL ALTO OLEICO");
            celda.CellStyle = colores[col];
            c++;
            col=0;

            var filaPosicion= r;
            row = sheet.CreateRow(r);r++;
            var max = 0;
            foreach (var m in model.PosicionCompras) {
                max = max< m.PosicionKilos.Count()? m.PosicionKilos.Count():max;
            };
            for(var l=0;l<=max+1; l++)
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
                    CrearCelda(row, c + 1, (mes.Mes + "-"+ mes.Anio.ToString()), cellBorderStyleColumnTitles);
                    CrearCelda(row, c + 2, material.PosicionKilos.Sum(x=>x.KilosPesos+x.KilosDolares).ToString("N0"), cellBorderStyleColumnTitles);
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
                CrearTablaAgenteDeCompras(sheet, colores[10], estiloCeldasColumnTitles, model,r);
            }
            #endregion

            #region ToneladasKilos
            foreach (var moneda in model.PrecioCantidad)
            {
                row = sheet.GetRow(filaPosicion-1); filaPosicion++;
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
            }else
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
            

            row = sheet.GetRow(fila);fila++;
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

            row = sheet.GetRow(fila +2);
            if (row == null)
            {
                row = sheet.CreateRow(fila+2);
            }
      
            CrearCelda(row, iC + 7, "Hedge TC: $" + model.TCPromedioDto.PromedioTC.ToString("n2"), cellBorderStyleColumnTitles);
            CrearCelda(row, iC + 8, "$" + model.TCPromedioDto.TotalTC.ToString("n2"), cellBorderStyleColumnTitles);

        }
        private static void CrearTablaAgenteDeCompras(HSSFSheet sheet, ICellStyle cellcolorTitles, ICellStyle cellBorderStyleColumnTitles, ReporteCompraNetModel model,int fila)
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
    }
}