using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class KendoDataSourceRequestDto
    {
        [DataMember]
        public int Take { get; set; }
        [DataMember]
        public int Skip { get; set; }
        [DataMember]
        public List<KendoSortDto> Sort { get; set; }
        [DataMember]
        public KendoFilterDto Filter { get; set; }
    }

    [DataContract]
    public class KendoSortDto
    {
        [DataMember(Name = "field")]
        public string Field { get; set; }
        [DataMember(Name = "dir")]
        public string Dir { get; set; }

        public string ToExpression()
        {
            return Field + " " + Dir;
        }
    }

    [DataContract]
    public class KendoFilterDto
    {
        private static readonly IDictionary<string, string> operators = new Dictionary<string, string>
        {
            { "eq", "=" },
            { "neq", "!=" },
            { "lt", "<" },
            { "lte", "<=" },
            { "gt", ">" },
            { "gte", ">=" },
            { "startswith", "StartsWith" },
            { "endswith", "EndsWith" },
            { "contains", "Contains" },
            { "doesnotcontain", "Contains" }
        };

        [DataMember(Name = "field")]
        public string Field { get; set; }
        [DataMember(Name = "operator")]
        public string Operator { get; set; }
        [DataMember(Name = "value")]
        public string Value { get; set; }
        [DataMember(Name = "logic")]
        public string Logic { get; set; }
        [DataMember(Name = "filters")]
        public List<KendoFilterDto> Filters { get; set; }

        public string ToExpression(IList<KendoFilterDto> filters)
        {
            if (Filters != null && Filters.Any())
            {
                return "(" + string.Join(" " + Logic + " ", Filters.Select((KendoFilterDto filter) => filter.ToExpression(filters)).ToArray()) + ")";
            }

            int num = filters.IndexOf(this);
            string text = operators[Operator];
            if (Operator == "doesnotcontain")
            {
                return $"!{Field}.{text}(@{num})";
            }

            switch (text)
            {
                case "StartsWith":
                case "EndsWith":
                case "Contains":
                    return $"{Field}.{text}(@{num})";
                default:
                    return $"{Field} {text} @{num}";
            }
        }
    }
}
