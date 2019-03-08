using System;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace Molinos.DataAgro.Repository
{
    public static class SqlBulkExtension
    {
        public static void SqlBulkInsert(this DbContext session, DataTable dataTable, string tableName)
        {
            var conn = session.Database.Connection.ConnectionString;
            using (var copy = new SqlBulkCopy(conn))
            {
                copy.BulkCopyTimeout = 10000;
                copy.DestinationTableName = tableName;
                foreach (DataColumn column in dataTable.Columns)
                {
                    copy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                copy.WriteToServer(dataTable);
            }
        }
        
        public static void SqlBulkUpdate(this DbContext session, DataTable dataTable, string tableName)
        {
            var conn = (SqlConnection)session.Database.Connection;
            using (SqlCommand command = new SqlCommand(string.Empty, conn))
            {
                var setColumns = string.Empty;
                var idType = string.Empty;
                for (var i = 0; i < dataTable.Columns.Count; i++)
                {
                    var column = dataTable.Columns[i];
                    if (column.ColumnName != "Id")
                    {
                        if (setColumns != string.Empty)
                        {
                            setColumns += ",";
                        }

                        setColumns += "T." + column.ColumnName + " = Temp." + column.ColumnName;
                    }
                    else
                    {
                        idType = IdType(column.DataType);
                    }
                }

                command.CommandText = string.Format(@"Select top 0 * Into #TmpTable{0} From {0};
                                                    ALTER TABLE #TmpTable{0} DROP COLUMN Id;
                                                    ALTER TABLE #TmpTable{0} ADD Id {1} NOT NULL;                                                  
                                                    ALTER TABLE #TmpTable{0} DROP COLUMN Rowguid;
                                                    
                ", tableName, idType);
                //Creating temp table on database
                command.ExecuteNonQuery();

                session.SqlBulkInsert(dataTable, "#TmpTable" + tableName);

                // Updating destination table, and dropping temp table
                command.CommandTimeout = 300;
                command.CommandText = string.Format(@"UPDATE T SET {1} FROM {0} T INNER JOIN #TmpTable{0} Temp ON Temp.Id = T.Id;
                                                      DROP TABLE #TmpTable{0};
                ", tableName, setColumns);
                command.ExecuteNonQuery();
            }
        }

        private static string IdType(Type type)
        {
            var idType = string.Empty;
            if (type == typeof(int))
            {
                idType = "int";
            }
            if (type == typeof(Guid))
            {
                idType = "uniqueidentifier";
            }
            if (type == typeof(long))
            {
                idType = "BIGINT";
            }
            if (type == typeof(string))
            {
                idType = "nvarchar(255) COLLATE DATABASE_DEFAULT";
            }
            return idType;
        }
    }
}
