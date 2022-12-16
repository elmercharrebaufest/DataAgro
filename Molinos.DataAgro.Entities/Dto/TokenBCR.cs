using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class TokenBCR
    {
        public Status status { get; set; }
        public Metadata metadata { get; set; }
        public Data data { get; set; }
    }

    public class Status
    {
        public string code { get; set; }
        public string status { get; set; }
    }

    public class Metadata {}
    
    public class Data
    {
        public string token { get; set; }
    }

}
