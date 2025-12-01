using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class KendoGridResponseDto<T>
    {
        [DataMember] public List<T> Data { get; set; }
        [DataMember] public int Total { get; set; }

        public KendoGridResponseDto()
        {
            Data = new List<T>();
        }
    }
}