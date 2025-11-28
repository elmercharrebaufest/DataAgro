using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Molinos.DataAgro.Entities.Dto
{
    [DataContract]
    public class KendoDataSourceResultDto
    {
        [DataMember]
        //public List<BasicoContratoDto> Data { get; set; }
        public List<BasicoContrato> Data { get; set; }
        [DataMember]
        public int Total { get; set; }
        [DataMember]
        public string AggregatesJson { get; set; }
    }
}
