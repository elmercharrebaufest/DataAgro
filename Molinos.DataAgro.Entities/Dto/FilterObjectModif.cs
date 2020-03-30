using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class FilterObjectModif
    {
        public IEnumerable<FilterObjectWrapperModif> FilterObjectWrapper { get; set; }
        public string Field1 { get; set; }
        public string Operator1 { get; set; }
        public string Value1 { get; set; }
        public string IgnoreCase1 { get; set; }
        public string Field2 { get; set; }
        public string Operator2 { get; set; }
        public string Value2 { get; set; }
        public string IgnoreCase2 { get; set; }
        public string Logic { get; set; }
        public bool IsConjugate { get; }
        public string LogicToken { get; }

        //public string GetExpression1<TEntity>();
        //public string GetExpression2<TEntity>();
    }
}
