using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public class KendoGridMvcRequestModif
    {
        public int? Take { get; set; }
        public int? Skip { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        //public string Logic { get; set; }
        public FilterObjectWrapperModif Filter { get; set; }
        //public IEnumerable<SortObject> SortObjects { get; set; }
        //public IEnumerable<GroupObject> GroupObjects { get; set; }
        //public IEnumerable<AggregateObject> AggregateObjects { get; set; }
    }
}
