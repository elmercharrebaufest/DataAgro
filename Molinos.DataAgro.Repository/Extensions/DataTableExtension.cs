using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Molinos.DataAgro.Repository
{
    public static class DataTableExtension
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> entityList, bool sinId = false, List<KeyValuePair<string, string>> propertys = null) where T : class
        {
            var propertysInfo = typeof(T).GetProperties().Where(x =>
                                 x.PropertyType.Namespace != typeof(IList<object>).Namespace &&
                                 (!sinId || !Attribute.IsDefined(x, typeof(KeyAttribute)))
                             ).ToArray();
            propertys = propertys ?? propertysInfo.Select(x => new KeyValuePair<string, string>(x.Name, null)).ToList();
            
            var table = GenerarEstructuraDataTable(propertys, propertysInfo);
            
            CargarDatosEnDataTable(entityList, sinId, propertys, table, propertysInfo);

            return table;
        }

        private static void CargarDatosEnDataTable<T>(IEnumerable<T> entityList, bool sinId, List<KeyValuePair<string, string>> propertys,
            DataTable table, PropertyInfo[] propertysInfo) where T : class
        {
            foreach (var entity in entityList)
            {
                if (table.Rows.Count == 0)
                {
                    table.TableName = entity.GetType().Name;
                }

                var valores = new object[propertys.Count];
                for (var index = 0; index < propertys.Count; index++)
                {
                    var p = propertys[index];
                    var pInfo = propertysInfo.FirstOrDefault(x => x.Name == p.Key);
                    if (pInfo == null)
                    {
                        continue;
                    }

                    var value = pInfo.GetValue(entity, null);
                    if (pInfo.EsEntidad() && value != null)
                    {
                        var idProperty = pInfo.PropertyType.GetProperties().FirstOrDefault(x => Attribute.IsDefined(x, typeof(KeyAttribute))) ??
                                         pInfo.PropertyType.GetProperties().First();
                        value = idProperty.GetValue(value, null);
                    }
                    valores[index] = value;
                }
                table.Rows.Add(valores);
            }
        }

        private static DataTable GenerarEstructuraDataTable(IEnumerable<KeyValuePair<string, string>> propertys, PropertyInfo[] propertysInfo)
        {
            var table = new DataTable();

            //Armo la estructura del DataTable
            foreach (var property in propertys)
            {
                var propertyInfo = propertysInfo.FirstOrDefault(x => x.Name == property.Key);
                if (propertyInfo == null)
                {
                    table.Columns.Add(property.Value ?? property.Key, typeof(string));
                }
                else if (propertyInfo.EsEntidad())
                {
                    var entityProperties = propertyInfo.PropertyType.GetProperties();
                    var idProperty = entityProperties.FirstOrDefault(x => Attribute.IsDefined(x, typeof(KeyAttribute))) ?? entityProperties.First();
                    if (idProperty != null)
                    {
                        table.Columns.Add(property.Value ?? (property.Key + "Id"), idProperty.PropertyType);
                    }
                }
                else
                {
                    var type = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
                    table.Columns.Add(property.Value ?? property.Key, type);
                }
            }
            return table;
        }

        private static bool EsEntidad(this PropertyInfo p)
        {
            return p.PropertyType.Namespace != null && p.PropertyType.Namespace.Contains(typeof(Proveedor).Namespace ?? string.Empty);
        }
    }
}
