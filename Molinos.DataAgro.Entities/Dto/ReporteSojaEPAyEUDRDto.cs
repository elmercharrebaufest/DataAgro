using System.Collections.Generic;

namespace Molinos.DataAgro.Entities.Dto
{
    public class ReporteSojaEPAyEUDRDto
    {
        public double Precio { get; set; }
        public double Fijar { get; set; }
        public double Total { get; set; }
        public IEnumerable<KeyValuePair<int, int>> Ids { get; set; } = new List<KeyValuePair<int, int>>();
    }
}
