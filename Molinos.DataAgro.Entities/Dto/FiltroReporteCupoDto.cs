//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Molinos.DataAgro.Entities.Dto
//{
//    public class FiltroReporteCupoDto
//    {

//        public string FechaIngreso { get; set; }
//        public string CupoSap { get; set; }
//        public int MaterialId { get; set; }
//        public int[] ProveedorId { get; set; }
//        public int Destinatario { get; set; }
//        public string FechaGeneracion { get; set; }
//        public string Centro { get; set; }
//        public string Calidad { get; set; }
//        public int ZonaId { get; set; }
//        public int FleteProcedencia { get; set; }
//        public string Observacion { get; set; }
//        public int ComercialId { get; set; }
//        public int EstadoId { get; set; }




//        public int Page { get; set; }
//        public int PageSize { get; set; }
//        public IEnumerable<OrdenarFiltros> Sort { get; set; }



//        public override bool Equals(Object obj)
//        {
//            var result = false;
//            Type type = typeof(FiltroReporteCupoDto);
//            var propiedades = type.GetProperties();
//            propiedades = propiedades.Where(x => x.Name != "Page" && x.Name != "PageSize" && x.Name != "Sort").ToArray();

//            foreach (PropertyInfo pi in propiedades)
//            {
//                if (!result)
//                {
//                    dynamic value;
//                    if (pi.PropertyType == typeof(string))
//                    {
//                        value = (string)pi.GetValue(obj);
//                        result = string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? false : true;
//                    }
//                    else if (pi.PropertyType == typeof(int))
//                    {
//                        value = (int)pi.GetValue(obj);
//                        result = value == 0 || value == null ? false : true;

//                    }
//                    else if (pi.PropertyType == typeof(decimal?))
//                    {
//                        value = (decimal?)pi.GetValue(obj);
//                        result = value == 0 || value == null ? false : true;
//                    }
//                    else if (pi.PropertyType == typeof(bool))
//                    {
//                        value = (bool)pi.GetValue(obj);
//                        result = value;
//                    }
//                    else if (pi.PropertyType == typeof(int[]))
//                    {
//                        value = (int[])pi.GetValue(obj);
//                        result = value == null || value.Length == 0 ? false : true;
//                    }
//                    else if (pi.PropertyType == typeof(string[]))
//                    {
//                        value = (string[])pi.GetValue(obj);
//                        result = value == null || value.Length == 0 ? false : true;
//                    }
//                }
//                else
//                {
//                    return result;
//                }
//            }
//            return result;
//        }

//    }
//}
