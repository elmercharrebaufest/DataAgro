
using Excel;
using System;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;

namespace Molinos.DataAgro.Entities.Helpers
{
    public class ExcelImport
    {
        public static DataSet LeerExcelDesdeHttpRequest(System.Web.HttpRequestBase request)
        {
            var dsRet = new DataSet();

            //foreach (string upload in request.Files)
            for(int i = 0; i< request.Files.Count; i++)
            {
                if (request.Files == null || request.Files[i] == null) continue;

                Stream fileStream = request.Files[i].InputStream;
                string fileName = Path.GetFileName(request.Files[i].FileName);

                if (fileName == null || !(fileName.EndsWith("xlsx") || fileName.EndsWith("xls"))) continue;

                DataTable dtExcel = LeerExcel(fileName, fileStream);

                if (dtExcel != null && dtExcel.Rows != null && dtExcel.Rows.Count > 0)
                {
                    dsRet.Tables.Add(dtExcel);
                }
            }

            return dsRet;
        }
        public static DataSet LeerExcelDesdePath(string path)
        {
            var dsRet = new DataSet();
            using (var stream = File.OpenRead(path))
            {

                DataTable dtExcel = LeerExcel(stream.Name, stream);

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

                excelReader.IsFirstRowAsColumnNames = true;

                DataSet result = excelReader.AsDataSet();

                excelReader.Close();
                if (result.Tables.Count == 0)
                {
                    throw new Exception("El documento no tiene formato correcto.");
                }
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
                    dt.TableName = result.Tables[0].TableName;
                    return dt;
                }
                else
                {
                    return new DataTable();
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
