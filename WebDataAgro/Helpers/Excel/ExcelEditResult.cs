using System.Collections.Generic;
using System.IO;

namespace WebDataAgro.Helpers.Excel
{
    public class ExcelEditResult : ExcelResultBase
    {
        private readonly DataForExcel data;
        private readonly string filePath;
        
        public ExcelEditResult(string filePath, List<string[]> data, string fileName, string sheetName, uint rowIndexToBegin)
            : base(fileName)
        {
            this.data = new DataForExcel(data, sheetName, rowIndexToBegin);
            this.filePath = filePath;
        }
        
        public ExcelEditResult(string filePath, string[] headers, List<string[]> data, string fileName, string sheetName, uint rowIndexToBegin, bool updateFormulas = false)
            : base(fileName)
        {
            this.data = new DataForExcel(headers, data, sheetName, rowIndexToBegin, updateFormulas);
            this.filePath = filePath;
        }

        protected override void CreateFile(MemoryStream stream)
        {
            using (FileStream fs = File.OpenRead(this.filePath))
            {
                fs.CopyTo(stream);
                fs.Close();
            }

            this.data.EditXlsxAndFillData(stream);
        }
    }
}
