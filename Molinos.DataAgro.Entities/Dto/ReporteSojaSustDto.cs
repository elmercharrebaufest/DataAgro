using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteSojaSustDto
    {
        public double Precio { get; set; }
        public double Fijar { get; set; }
        public double Total { get; set; }
        public IEnumerable<KeyValuePair<int, int>> Ids { get; set; } = new List<KeyValuePair<int, int>>();
    }
}
