using System.Collections.Generic;
using System.IO;

namespace WebDataAgro.Helpers.Excel
{
    public class ExcelResult : ExcelResultBase
    {
        private readonly DataForExcel data;

        public ExcelResult(string[] headers, List<string[]> data, string fileName, string sheetName)
            : base(fileName)
        {
            this.data = new DataForExcel(headers, data, sheetName);
        }

        public ExcelResult(string[] headers, DataForExcel.DataType[] colunmTypes, List<string[]> data, string fileName, string sheetName)
            : base(fileName)
        {
            this.data = new DataForExcel(headers, colunmTypes, data, sheetName);
        }

        public DataForExcel Data
        {
            get { return this.data; }
        }

        public new string FileName
        {
            get { return base.FileName; }
        }

        protected override void CreateFile(MemoryStream stream)
        {
            this.data.CreateXlsxAndFillData(stream);
        }
    }
}
