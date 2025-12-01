using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class KendoGridRequestDto
    {
        [DataMember] public int? Take { get; set; }
        [DataMember] public int? Skip { get; set; }
        [DataMember] public int? Page { get; set; }
        [DataMember] public int? PageSize { get; set; }
        [DataMember] public string Logic { get; set; }
        [DataMember] public FilterObjectWrapperDto FilterObjectWrapper { get; set; }
        [DataMember] public List<SortObjectDto> SortObjects { get; set; }
        [DataMember] public List<GroupObjectDto> GroupObjects { get; set; }
        [DataMember] public List<AggregateObjectDto> AggregateObjects { get; set; }

        public KendoGridRequestDto()
        {
            SortObjects = new List<SortObjectDto>();
            GroupObjects = new List<GroupObjectDto>();
            AggregateObjects = new List<AggregateObjectDto>();
        }
    }

    [DataContract]
    public class SortObjectDto
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Dir { get; set; }
    }

    [DataContract]
    public class FilterObjectWrapperDto
    {
        [DataMember] public List<FilterObjectDto> Filters { get; set; }
        [DataMember] public string Logic { get; set; }
    }

    [DataContract]
    public class FilterObjectDto
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Operator { get; set; }
        [DataMember] public string Value { get; set; }
    }

    [DataContract]
    public class GroupObjectDto
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Direction { get; set; }
        [DataMember] public List<AggregateObjectDto> AggregateObjects { get; set; }

        public GroupObjectDto()
        {
            Direction = "asc";
            AggregateObjects = new List<AggregateObjectDto>();
        }
    }

    [DataContract]
    public class AggregateObjectDto
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Aggregate { get; set; }
        [DataMember] public string Direction { get; set; }

        public AggregateObjectDto()
        {
            Direction = "asc";
        }
    }

    [DataContract]
    public class FilterObjectWrapperSOAP
    {
        [DataMember] public string Field { get; set; }
        [DataMember] public string Operator { get; set; }
        [DataMember] public string Value { get; set; }
        [DataMember] public string Logic { get; set; }

        [DataMember] public List<FilterObjectWrapperSOAP> Filters { get; set; }

        public FilterObjectWrapperSOAP()
        {
            Filters = new List<FilterObjectWrapperSOAP>();
        }
    }
}
