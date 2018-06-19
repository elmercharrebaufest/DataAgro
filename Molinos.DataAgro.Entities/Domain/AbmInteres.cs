using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class ResultIniInteres
    {
        public List<InteresIni> Condicion { get; set; }
    }


    public class InteresIni
    {
        public int InteresId { get; set; }
        public string Descripcion { get; set; }
    }
}
