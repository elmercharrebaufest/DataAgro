using System.Collections.Generic;
using System.IO;

namespace WebDataAgro.Helpers.Excel
{
    public interface IDataForExcel
    {
        void Initialize(List<string[]> data, string sheetName, uint rowIndexToBegin);

        void EditXlsxAndFillData(Stream stream);
    }
}
