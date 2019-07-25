
using Molinos.DataAgro.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.Dto
{
    public partial class LogDto
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Xml { get; set; }
        public List<Log> Log { get; set; }
    }
}
