using Excel;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace Molinos.DataAgro.Entities.Helpers
{
    public class ExcelHelper
    {
        public static DataSet LeerExcelDesdeHttpRequest(System.Web.HttpRequestBase request)
        {
            var dsRet = new DataSet();

            foreach (string upload in request.Files)
            {
                if (request.Files == null || request.Files[upload] == null) continue;

                Stream fileStream = request.Files[upload].InputStream;
                string fileName = Path.GetFileName(request.Files[upload].FileName);

                if (fileName == null || !(fileName.EndsWith("xlsx") || fileName.EndsWith("xls"))) continue;

                DataTable dtExcel = LeerExcel(fileName, fileStream);

                if (dtExcel != null && dtExcel.Rows != null && dtExcel.Rows.Count > 0)
                {
                    dsRet.Tables.Add(dtExcel);
                }
            }

            return dsRet;
        }

        private static DataTable LeerExcel(string fileName, Stream fileStream)
        {
            try
            {
                IExcelDataReader excelReader = (fileName.EndsWith("xlsx"))
                                             ? ExcelReaderFactory.CreateOpenXmlReader(fileStream)
                                             : ExcelReaderFactory.CreateBinaryReader(fileStream);

                excelReader.IsFirstRowAsColumnNames =
                    Convert.ToBoolean(ConfigurationManager.AppSettings["ExcelVariedadesContieneCabecera"]);

                DataSet result = excelReader.AsDataSet();

                excelReader.Close();

                //Eliminar todas las rows vacias
                var dd =
                    result.Tables[0].Rows.Cast<DataRow>()
                                    .Where(
                                        row =>
                                        !row.ItemArray.All(
                                            field =>
                                            field is System.DBNull ||
                                            (field != null && string.IsNullOrWhiteSpace(field.ToString()))));
                if (dd.Any())
                {
                    DataTable dt = dd.CopyToDataTable();
                    return dt;
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}


