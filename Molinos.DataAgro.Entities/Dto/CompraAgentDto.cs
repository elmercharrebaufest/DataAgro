using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class CompraAgentDto
    {
        public string VENDEDOR { get; set; }
        public decimal TN_COMPRADAS { get; set; }
        public string MES { get; set; }
        public string ANIO { get; set; }
        public string MATERIAL { get; set; }
        public string COSECHA { get; set; }
    }

}


