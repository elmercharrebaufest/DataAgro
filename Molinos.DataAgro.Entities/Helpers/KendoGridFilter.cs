using KendoGridBinder.ModelBinder.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using System.Reflection.Emit;
using System.Threading;
using System.Linq.Dynamic;
using Kendo.DynamicLinq;

namespace Molinos.DataAgro.Entities.Helpers
{

    public static class KendoGridFilter
    {
        public static string GetSorting(this KendoGridMvcRequest request, Type t)
        {
            var expression = "";

            foreach (var sortObject in request.SortObjects)
            {
                expression += sortObject.Field + " " + sortObject.Direction + ", ";
            }

            if (expression.Length < 2)
                return "true";

            expression = expression.Substring(0, expression.Length - 2);

            return expression;
        }

        public static string GetFiltering(this KendoGridMvcRequest request, Type t)
        {
            var finalExpression = "";
            if (request.FilterObjectWrapper == null)
            {
                return "1=1";
            }
            foreach (var filterObject in request.FilterObjectWrapper.FilterObjects)
            {
                if (finalExpression.Length > 0)
                    finalExpression += " " + request.FilterObjectWrapper.LogicToken + " ";


                if (filterObject.IsConjugate)
                {
                    var expression1 = GetExpression(t, filterObject.Field1, filterObject.Operator1, filterObject.Value1);
                    var expression2 = GetExpression(t, filterObject.Field2, filterObject.Operator2, filterObject.Value2);
                    var combined = string.Format("({0} {1} {2})", expression1, request.FilterObjectWrapper.LogicToken, expression2);
                    finalExpression += combined;
                }
                else
                {
                    var expression = GetExpression(t, filterObject.Field1, filterObject.Operator1, filterObject.Value1);
                    finalExpression += expression;
                }
            }

            if (finalExpression.Length == 0)
                return "true";

            return finalExpression;
        }

        private static string GetExpression(Type t, string field, string op, string param)
        {
            var properties = t.GetProperty(field).PropertyType.GetProperties();
            var dataType = t.GetProperty(field).PropertyType.Name.ToLower();
            foreach (var prop in properties)
            {
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataType = propType.Name.ToLower();
            }



            var caseMod = "";

            if (dataType == "string" || dataType == "char")
            {
                param = @"""" + param.ToLower() + @"""";
                caseMod = ".ToLower()";
            }

            if (dataType == "datetime")
            {
                var date = DateTime.Parse(param);
                var str = string.Format("DateTime({0}, {1}, {2})", date.Year, date.Month, date.Day);
                param = str;
            }

            string exStr;

            switch (op)
            {
                case "eq":
                    exStr = string.Format("{0}{2} == {1}", field, param, caseMod);
                    break;

                case "neq":
                    exStr = string.Format("{0}{2} != {1}", field, param, caseMod);
                    break;

                case "contains":
                    exStr = string.Format("{0}{2}.Contains({1})", field, param, caseMod);
                    break;

                case "startswith":
                    exStr = string.Format("{0}{2}.StartsWith({1})", field, param, caseMod);
                    break;

                case "endswith":
                    exStr = string.Format("{0}{2}.EndsWith({1})", field, param, caseMod);
                    break;
                case "gte":
                    exStr = string.Format("{0}{2} >= {1}", field, param, caseMod);
                    break;
                case "gt":
                    exStr = string.Format("{0}{2} > {1}", field, param, caseMod);
                    break;
                case "lte":
                    exStr = string.Format("{0}{2} <= {1}", field, param, caseMod);
                    break;
                case "lt":
                    exStr = string.Format("{0}{2} < {1}", field, param, caseMod);
                    break;
                default:
                    exStr = "";
                    break;
            }

            return exStr;
        }

    }

    public static class GridHelper
    {
        public static void ProcessFilters<T>(Filter filter, ref IQueryable<T> queryable)
        {
            if (filter != null && filter.Filters != null)
            {
                var whereClause = string.Empty;
                var filters = filter.Filters;
                var parameters = new List<object>();
                for (int i = 0; i < filters.Count(); i++)
                {
                    var f = filters.ToList()[i];

                    if (f.Filters == null)
                    {
                        if (i == 0)
                            whereClause += BuildWherePredicate<T>(f, i, parameters) + " ";
                        if (i != 0)
                            whereClause += ToLinqOperator(filter.Logic) + BuildWherePredicate<T>(f, i, parameters) + " ";
                        if (i == (filters.Count() - 1))
                        {
                            TrimWherePredicate(ref whereClause);
                            queryable = queryable.Where(whereClause, parameters.ToArray());
                        }
                    }
                    else
                    {
                        ProcessFilters(f, ref queryable);
                        if (whereClause !="")
                        {
                        queryable = queryable.Where(whereClause, parameters.ToArray());
                        }
                    }
                }
            }

        }

        public static string TrimWherePredicate(ref string whereClause)
        {
            switch (whereClause.Trim().Substring(0, 2).ToLower())
            {
                case "&&":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
                case "||":
                    whereClause = whereClause.Trim().Remove(0, 2);
                    break;
            }

            return whereClause;
        }

        public static string BuildWherePredicate<T>(Kendo.DynamicLinq.Filter filter, int index, List<object> parameters)
        {
            var entityType = (typeof(T));
            PropertyInfo property;

            if (filter.Field.Contains("."))
                property = GetNestedProp<T>(filter.Field);
            else
                property = entityType.GetProperty(filter.Field);

            var parameterIndex = parameters.Count;


            var properties = property.PropertyType.GetProperties();
            var dataType = property.PropertyType.Name.ToLower();
            foreach (var prop in properties)
            {
                var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                dataType = propType.Name.ToLower();
            }

            switch (filter.Operator.ToLower())
            {
                case "eq":
                case "neq":
                case "gte":
                case "gt":
                case "lte":
                case "lt":
                    //if (typeof(DateTime).IsAssignableFrom(property.PropertyType))
                    if (dataType == "datetime")
                    {
                        //parameters.Add(DateTime.Parse(filter.Value.ToString()).Date);
                        parameters.Add(((DateTime)filter.Value).Date);
                        //return string.Format(" EntityFunctions.TruncateTime(" + filter.Field + ")" + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                        return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }
                    if (typeof(int).IsAssignableFrom(property.PropertyType))
                    {
                        parameters.Add(int.Parse(filter.Value.ToString()));
                        return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                    }
                    parameters.Add(filter.Value);
                    return string.Format(filter.Field + ToLinqOperator(filter.Operator) + "@" + parameterIndex);
                case "startswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".StartsWith(" + "@" + parameterIndex + ")";
                case "endswith":
                    parameters.Add(filter.Value);
                    return filter.Field + ".EndsWith(" + "@" + parameterIndex + ")";
                case "contains":
                    parameters.Add(filter.Value);
                    return filter.Field + ".Contains(" + "@" + parameterIndex + ")";
                default:
                    throw new ArgumentException(" This operator is not yet supported for this Grid", filter.Operator);
            }
        }

        public static string ToLinqOperator(string @operator)
        {
            switch (@operator.ToLower())
            {
                case "eq":
                    return " == ";
                case "neq":
                    return " != ";
                case "gte":
                    return " >= ";
                case "gt":
                    return " > ";
                case "lte":
                    return " <= ";
                case "lt":
                    return " < ";
                case "or":
                    return " || ";
                case "and":
                    return " && ";
                default:
                    return null;
            }
        }

        public static PropertyInfo GetNestedProp<T>(String name)
        {
            PropertyInfo info = null;
            var type = (typeof(T));
            foreach (var prop in name.Split('.'))
            {
                info = type.GetProperty(prop);
                type = info.PropertyType;
            }
            return info;
        }

        public static void TruncateTime<T>(Filter filter, ref IQueryable<T> queryable)
        {
            if (filter != null)
            {
                var filters = filter.Filters;
                for (int i = 0; i < filters.Count(); i++)
                {
                    var f = filters.ToList()[i];

                    if (f.Filters == null)
                    {
                        var entityType = (typeof(T));
                        PropertyInfo property;

                        if (f.Field.Contains("."))
                            property = GetNestedProp<T>(f.Field);
                        else
                            property = entityType.GetProperty(f.Field);
                        var properties = property.PropertyType.GetProperties();
                        var dataType = property.PropertyType.Name.ToLower();
                        foreach (var prop in properties)
                        {
                            var propType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                            dataType = propType.Name.ToLower();
                        }
                        if (dataType == "datetime")
                        {
                            f.Value = new DateTime(((DateTime)f.Value).Year, ((DateTime)f.Value).Month, ((DateTime)f.Value).Day);
                        }
                    }
                    else
                    {
                        TruncateTime(f, ref queryable);
                    }
                }
            }

        }
    }
}