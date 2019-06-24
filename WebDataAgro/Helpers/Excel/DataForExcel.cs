using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using DataValidation = DocumentFormat.OpenXml.Office2010.Excel.DataValidation;

namespace WebDataAgro.Helpers.Excel
{
    public class DataForExcel : IDataForExcel
    {
        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201:ElementsMustAppearInTheCorrectOrder", Justification = "Reviewed. Suppression is OK here.")]
        public enum DataType
        {
            String,

            Integer
        }

        private readonly string[] headers;

        private readonly DataType[] columnTypes;

        private readonly SortedList<string, int> sortedList = new SortedList<string, int>();

        private readonly bool updateFormulas;

        private List<string[]> data;

        private string sheetName = "Grid1";

        private uint rowIndex = 1;

        private string[] columnHeader;

        public static Cell AddValueToCell(Cell cell, string text)
        {
            cell.RemoveAllChildren();
            if (!string.IsNullOrWhiteSpace(text))
            {
                if (text.Remove('.', ',').All(char.IsDigit))
                {
                    cell.DataType = CellValues.Number;
                }
                else
                {
                    cell.DataType = CellValues.InlineString;
                }

                cell.AppendChild(new InlineString { Text = new Text(text) });
            }

            return cell;
        }

        public DataForExcel(List<string[]> data, string sheetName, uint rowIndexToBegin)
        {
            this.data = data;
            this.sheetName = sheetName;
            this.rowIndex = rowIndexToBegin;
        }

        public DataForExcel(
            string[] headers,
            List<string[]> data,
            string sheetName,
            uint rowIndexToBegin,
            bool updateFormulas = false)
            : this(data, sheetName, rowIndexToBegin)
        {
            this.headers = headers;
            this.updateFormulas = updateFormulas;
        }

        public DataForExcel(string[] headers, List<string[]> data, string sheetName)
        {
            this.headers = headers;
            this.data = data;
            this.sheetName = sheetName;
        }

        public DataForExcel(
            string[] headers,
            DataType[] columnTypes,
            List<string[]> data,
            string sheetName,
            bool updateFormulas = false)
        {
            this.headers = headers;
            this.columnTypes = columnTypes;
            this.data = data;
            this.sheetName = sheetName;
            this.updateFormulas = updateFormulas;
        }

        public DataForExcel()
        {
        }

        public void Initialize(List<string[]> dataParam, string sheetNameParam, uint rowIndexToBegin)
        {
            this.data = dataParam;
            this.sheetName = sheetNameParam;
            this.rowIndex = rowIndexToBegin;
        }

        public void CreateXlsxAndFillData(Stream stream)
        {
            // Create workbook document
            using (var spreadsheetDocument = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook))
            {
                this.FillSpreadsheetDocument(spreadsheetDocument);
            }
        }

        public void EditXlsxAndFillData(Stream stream)
        {
            using (var spreadsheetDocument = SpreadsheetDocument.Open(stream, true))
            {
                this.FillSpreadsheetForEditDocument(spreadsheetDocument);
            }
        }

        [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1407:ArithmeticExpressionsMustDeclarePrecedence", Justification = "Reviewed. Suppression is OK here.")]
        private static string IntToColumnHeader(int index, StringBuilder strb = null)
        {
            var sb = strb ?? new StringBuilder();
            while (index > 0)
            {
                if (index <= 'Z' - 'A')
                {
                    break;
                }

                IntToColumnHeader(index / ('Z' - 'A' + 1) - 1, sb);
                index = index % ('Z' - 'A' + 1);
            }

            sb.Append((char)('A' + index));
            return sb.ToString();
        }

        private static Row CloneRow(Row referenceRow, uint index, IList<string> data, bool deep, bool updateFormula)
        {
            // Clonar Fila
            var newRow = (Row)referenceRow.CloneNode(deep);
            var oldIndexString = referenceRow.RowIndex.Value.ToString();
            newRow.RowIndex = index;
            // Si se clonaron las Celdas
            if (deep)
            {
                var newRowCells = newRow.Descendants<Cell>().ToList();

                for (var i = 0; i < newRowCells.Count; i++)
                {
                    //Actualiza la referencia de las celdas a la nueva fila
                    string cellReference = newRowCells[i].CellReference.Value;
                    newRowCells[i].CellReference = cellReference.Replace(oldIndexString, index.ToString());
                    //Actualiza el valor de la Celda
                    if (i < data.Count)
                    {
                        AddValueToCell(newRowCells[i], data[i]);
                    }
                    else
                    {
                        //Si la Celda tiene fórmula mueve todas las referencias a otras celdas al valor de fila que corresponde
                        if (updateFormula && newRowCells[i].CellFormula != null)
                        {
                            var indexDiff = index - referenceRow.RowIndex.Value;
                            //Busca referencias a celdas del tipo [De 1 a 3 letras mayúsculas][números]
                            //Encuentra A6, AB6, ABC6 pero no A$6
                            string mainPattern = @"(?<column>\b[A-Z]{1,3})+([0-9]{1,6}\b)+";
                            var matches = Regex.Matches(newRowCells[i].CellFormula.Text, mainPattern);
                            //newRowCells[i].CellFormula.Text = Regex.Replace(newRowCells[i].CellFormula.Text, pattern, "${column}" + index.ToString());//Me quedo con el grupo <column> que son las letras mayúscula de la celda y le agrego el nuevo índice de la fila

                            var cellFormula = newRowCells[i].CellFormula.Text;
                            var displacement = 0;
                            foreach (Match match in matches)
                            {
                                //A cada referencia a celda del tipo A6, le suma al índice la cantidad de filas agregadas
                                var newIndex = (int.Parse(match.Groups[1].Value) + indexDiff).ToString();

                                var newCellFormula = cellFormula.Substring(0, match.Groups[1].Index + displacement);
                                newCellFormula += newIndex;
                                newCellFormula +=
                                    cellFormula.Substring(match.Groups[1].Index + displacement + match.Groups[1].Length);
                                displacement += newIndex.Length - match.Groups[1].Value.Length;
                                cellFormula = newCellFormula;
                            }

                            newRowCells[i].CellFormula.Text = cellFormula;
                        }
                    }
                }
            }

            return newRow;
        }

        private static Cell CreateTextCell(string header, uint index, string text)
        {
            // create Cell with InlineString as a child, which has Text as a child
            return new Cell(new InlineString(new Text
            {
                Text = text
            }))
            {
                // Cell properties
                DataType = CellValues.InlineString,
                CellReference = header + index
            };
        }

        private static Cell CreateNumberCell(string header, uint index, string numberAsString)
        {
            // create Cell with CellValue as a child, which has Text as a child
            return new Cell(new CellValue
            {
                Text = numberAsString
            })
            {
                // Cell properties
                CellReference = header + index
            };
        }

        private string ConvertIntToColumnHeader(int index)
        {
            int interval = 100;
            if (this.columnHeader == null)
            {
                this.columnHeader = new string[interval];
                for (int i = 0; i < interval; i++)
                {
                    this.columnHeader[i] = IntToColumnHeader(i);
                }
            }

            var prevLen = this.columnHeader.Length - 1;
            if (index > prevLen)
            {
                Array.Resize(ref this.columnHeader, this.columnHeader.Length + interval);
                for (int i = prevLen + 1; i < this.columnHeader.Length; i++)
                {
                    this.columnHeader[i] = IntToColumnHeader(i);
                }
            }

            return this.columnHeader[index];
        }

        private Row CreateRow(uint index, IList<string> rowData)
        {
            var r = new Row { RowIndex = index };
            for (var i = 0; i < rowData.Count; i++)
            {
                if (!string.IsNullOrEmpty(rowData[i]))
                {
                    r.Append(
                        new OpenXmlElement[] { CreateTextCell(this.ConvertIntToColumnHeader(i), index, rowData[i]) });
                }
            }

            return r;
        }

        private Row CreateRowWithSharedStrings(uint index, IList<string> rowData)
        {
            var r = new Row { RowIndex = index };
            for (var i = 0; i < rowData.Count; i++)
            {
                if (!string.IsNullOrEmpty(rowData[i]))
                {
                    r.Append(
                        new OpenXmlElement[]
                            {
                                this.CreateSharedTextCell(this.ConvertIntToColumnHeader(i), index, rowData[i])
                            });
                }
            }

            return r;
        }

        private Row CreateRowWithSharedStrings(uint index, IList<string> rowData, IList<DataType> rowColunmTypes)
        {
            var r = new Row { RowIndex = index };
            for (var i = 0; i < rowData.Count; i++)
            {
                if (!string.IsNullOrEmpty(rowData[i]))
                {
                    if (rowColunmTypes != null && i < rowColunmTypes.Count && rowColunmTypes[i] == DataType.Integer)
                    {
                        r.Append(
                            new OpenXmlElement[]
                                {
                                    CreateNumberCell(this.ConvertIntToColumnHeader(i), index, rowData[i])
                                });
                    }
                    else
                    {
                        r.Append(
                            new OpenXmlElement[]
                                {
                                    this.CreateSharedTextCell(this.ConvertIntToColumnHeader(i), index, rowData[i])
                                });
                    }
                }
            }

            return r;
        }

        private Cell CreateSharedTextCell(string header, uint index, string text)
        {
            var i = this.sortedList[text];

            if (text.Length > 0 && text.StartsWith("0"))
            {
                return
                    new Cell(
                        new CellFormula(
                            string.Format("TEXT(\"{0}\",\"{1}\")", text, string.Empty.PadLeft(text.Length, '0'))))
                    {
                        DataType = CellValues.String,
                        CellValue = new CellValue
                        {
                            Text = text
                        },
                        CellReference = header + index
                    };
            }
            if (text.Replace(",", "").Replace(".", "").Replace(" ", "").All(char.IsDigit))
            {
                return new Cell(
                    new CellValue
                    {
                        Text = text == "0.00" ? " " : text
                    })
                {
                    // Cell properties
                    DataType = CellValues.Number,
                    CellReference = header + index
                };
            }
            else
            {
                return new Cell(
                   new CellValue
                   {
                       Text = i.ToString()
                   })
                {
                    // Cell properties
                    DataType = CellValues.SharedString,
                    CellReference = header + index
                };
            }

            // create Cell with InlineString as a child, which has Text as a child
        }

        private void FillSharedStringTable(IEnumerable<string> tableData)
        {
            foreach (var item in tableData)
            {
                //os.Add(item);
                if ((!string.IsNullOrEmpty(item)) && (!this.sortedList.ContainsKey(item)))
                {
                    var idx = this.sortedList.Count;
                    this.sortedList.Add(item, idx);
                }
            }
        }

        private void FillSharedStringTable(IList<string> tableData, IList<DataType> tableColunmTypes)
        {
            for (var i = 0; i < tableData.Count; i++)
            {
                if (tableColunmTypes == null || i >= tableColunmTypes.Count || tableColunmTypes[i] == DataType.String)
                {
                    if ((!string.IsNullOrEmpty(tableData[i])) && (!this.sortedList.ContainsKey(tableData[i])))
                    {
                        var idx = this.sortedList.Count;
                        this.sortedList.Add(tableData[i], idx);
                    }
                }
            }
        }

        private void FillSpreadsheetDocument(SpreadsheetDocument spreadsheetDocument)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            // create and fill SheetData
            var sheetData = new SheetData();

            // first row is the header
            sheetData.AppendChild(this.CreateRow(1, this.headers));

            uint i = 2;

            // first of all collect all different strings 
            if (this.columnTypes != null)
            {
                foreach (var dataRow in this.data)
                {
                    this.FillSharedStringTable(dataRow, this.columnTypes);
                }

                foreach (var dataRow in this.data)
                {
                    sheetData.AppendChild(this.CreateRowWithSharedStrings(i++, dataRow, this.columnTypes));
                }
            }
            else
            {
                foreach (var dataRow in this.data)
                {
                    this.FillSharedStringTable(dataRow);
                }

                foreach (var dataRow in this.data)
                {
                    sheetData.AppendChild(this.CreateRowWithSharedStrings(i++, dataRow));
                }
            }

            var sst = new SharedStringTable();
            foreach (var text in this.sortedList.OrderBy(x => x.Value).Select(x => x.Key))
            {
                sst.AppendChild(new SharedStringItem(new Text(text)));
            }

            // add empty workbook and worksheet to the SpreadsheetDocument
            var workbookPart = spreadsheetDocument.AddWorkbookPart();
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();

            var shareStringPart = workbookPart.AddNewPart<SharedStringTablePart>();
            shareStringPart.SharedStringTable = sst;

            shareStringPart.SharedStringTable.Save();

            // add sheet data to Worksheet
            worksheetPart.Worksheet = new Worksheet(sheetData);

            this.HideColumns(worksheetPart);

            worksheetPart.Worksheet.Save();

            // fill workbook with the Worksheet
            spreadsheetDocument.WorkbookPart.Workbook =
                new Workbook(
                    new FileVersion { ApplicationName = "Microsoft Office Excel" },
                    new Sheets(
                        new Sheet
                        {
                            Name = this.sheetName,
                            SheetId = (UInt32Value)1U,
                            // generate the id for sheet
                            Id = workbookPart.GetIdOfPart(worksheetPart)
                        }));
            spreadsheetDocument.WorkbookPart.Workbook.Save();
            spreadsheetDocument.Close();

            stopwatch.Stop();
            Debug.WriteLine("7.Time elapsed: {0}", stopwatch.Elapsed);
            stopwatch.Start();
        }

        private void FillSpreadsheetForEditDocument(SpreadsheetDocument spreadsheetDocument)
        {
            // Access the main Workbook part, which contains all references.
            var workbookPart = spreadsheetDocument.WorkbookPart;
            // Get sheet by name
            var sheet = workbookPart.Workbook.Descendants<Sheet>().SingleOrDefault(s => s.Name == this.sheetName);

            if (sheet == null)
            {
                throw new Exception("Sheet not exists.");
            }
            // Get worksheetpart by sheet id
            var worksheetPart = workbookPart.GetPartById(sheet.Id.Value) as WorksheetPart;
            // The SheetData object will contain all the data.
            var sheetData = worksheetPart?.Worksheet.GetFirstChild<SheetData>();

            // Get all rows
            var excelRows = sheetData?.Descendants<Row>().ToList();

            if (this.headers != null && this.headers.Any() && excelRows != null)
            {
                var rowProyect = excelRows[0].Descendants<Cell>().ToList();
                AddValueToCell(rowProyect[2], this.headers[0]);

                var rowLocalidad = excelRows[1].Descendants<Cell>().ToList();
                AddValueToCell(rowLocalidad[2], this.headers[1]);

                var rowFecha = excelRows[2].Descendants<Cell>().ToList();
                AddValueToCell(rowFecha[2], this.headers[2]);
            }

            Row baseRow = null;
            if (this.rowIndex <= excelRows?.Count)
            {
                baseRow = excelRows[(int)this.rowIndex - 1];
            }

            foreach (var dataRow in this.data)
            {
                if (this.rowIndex > excelRows?.Count)
                {
                    Row newRow = baseRow == null
                                     ? this.CreateRow(this.rowIndex, dataRow) // Crear Fila Nueva
                                     : CloneRow(baseRow, this.rowIndex, dataRow, true, this.updateFormulas);
                    sheetData.AppendChild(newRow);
                }
                else if (excelRows != null)
                {
                    var indexCell = 0;
                    // Get all cells
                    var excelRowCells = excelRows[(int)this.rowIndex - 1].Descendants<Cell>().ToList();
                    foreach (var dataCell in dataRow)
                    {
                        AddValueToCell(excelRowCells[indexCell++], dataCell);
                    }
                }

                this.rowIndex++;
            }

            //Si se crearon nuevas filas a partir de la copia de una fila en particular, se controla si hay DataValidations 
            //y se extienden a las nuevas filas si corresponde
            if (baseRow != null && this.rowIndex > baseRow.RowIndex)
            {
                this.UpdateDataValidations(worksheetPart, baseRow.RowIndex, this.rowIndex - 1);
            }

            worksheetPart?.Worksheet.Save();
            spreadsheetDocument.WorkbookPart.Workbook.Save();
            spreadsheetDocument.Close();
        }

        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1303:ConstFieldNamesMustBeginWithUpperCaseLetter", Justification = "Reviewed. Suppression is OK here.")]
        private void UpdateDataValidations(WorksheetPart worksheetPart, uint baseRowIndex, uint newRowIndex)
        {
            //Para Spreadsheets creadas con Office 2007, los DataValidations están en 
            //Worksheet -> DataValidations
            DataValidations dataValidations = worksheetPart.Worksheet.GetFirstChild<DataValidations>();
            if (dataValidations == null)
            {
                //Para Spreadsheets creadas con Office 2010 en adelante, los DataValidations están en 
                //Worksheet -> WorksheetExtensionList -> WorksheetExtension -> DataValidations
                var worksheetExtensionList = worksheetPart.Worksheet.GetFirstChild<WorksheetExtensionList>();
                if (worksheetExtensionList != null)
                {
                    var worksheetExtension = worksheetExtensionList.GetFirstChild<WorksheetExtension>();
                    if (worksheetExtension != null)
                    {
                        var dataValidations2010 =
                            worksheetExtension.GetFirstChild<DocumentFormat.OpenXml.Office2010.Excel.DataValidations>();
                        if (dataValidations2010 != null)
                        {
                            foreach (
                                var openXmlElement in
                                    dataValidations2010.ToList())
                            {
                                var dataValidation = (DataValidation)openXmlElement;
                                var refGroups = dataValidation.ReferenceSequence.Text.Split(' ');
                                var refSequence = string.Empty;
                                for (int i = 0; i < refGroups.Count(); i++)
                                {
                                    var refParts = refGroups[i].Split(':');
                                    if (refParts.Count() == 2)
                                    {
                                        const string Pattern = @"(?<column>\b[A-Z]{1,3})+([0-9]{1,6}\b)+";
                                        var matchStart = Regex.Matches(refParts[0], Pattern);
                                        var matchEnd = Regex.Matches(refParts[1], Pattern);
                                        //Si el índice de la fila base está entre el inicio y el fin de las celdas que tienen la DataValidation,
                                        if (int.Parse(matchStart[0].Groups[1].Value) <= baseRowIndex
                                            && baseRowIndex <= int.Parse(matchEnd[0].Groups[1].Value))
                                        {
                                            //se amplia el rango donde la DataValidation tiene referencia hasta la última fila creada
                                            refGroups[i] = refParts[0] + ":" + matchEnd[0].Groups[2].Value
                                                           + newRowIndex.ToString();
                                        }
                                    }

                                    refSequence = refSequence + refGroups[i] + " ";
                                }

                                dataValidation.ReferenceSequence.Text = refSequence.Substring(0, refSequence.Length - 1);
                            }
                        }
                    }
                }
            }
        }
        
        private void HideColumns(WorksheetPart worksheetPart)
        {
            if (this.headers == null || this.headers.Length == 0)
            {
                return;
            }

            var hiddenColumnHeader = "Cabecera oculta";
            var sheet = worksheetPart.Worksheet;
            var columns = new Columns();

            for (var i = 0; i < this.headers.Length; i++)
            {
                if (this.headers[i] == hiddenColumnHeader)
                {
                    var column = new Column
                    {
                        Min = (uint)i + 1,
                        Max = (uint)i + 1,
                        Width = 0D,
                        CustomWidth = true,
                        Hidden = true
                    };
                    // ReSharper disable once PossiblyMistakenUseOfParamsMethod
                    columns.Append(column);
                }
            }

            if (columns.Any())
            {
                sheet.InsertAfter(columns, sheet.SheetFormatProperties);
            }
        }
    }
}
