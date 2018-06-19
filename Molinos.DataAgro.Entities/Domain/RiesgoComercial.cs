using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class RiesgoComercial
    {
        public string CUIT { get; set; }
        public string RiesgoComercialDesc { get; set; }

        public RiesgoComercial()
        {
            CUIT = String.Empty;
            RiesgoComercialDesc = String.Empty;
        }
    }
}
