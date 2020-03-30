using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class FilterObjectWrapperModif
    {
        public string Logic { get; set; }
        public IEnumerable<FilterObjectWrapperModif> Filters { get; set; }
        public string value { get; set; }
        public string operador { get; set; }
        public string field { get; set; }
        public string LogicToken { get; }
    }
}
