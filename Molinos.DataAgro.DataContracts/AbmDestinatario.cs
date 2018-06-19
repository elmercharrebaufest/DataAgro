
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.DataContracts
{
    public class WcfResultIniDestinatario
    {
        public EntityErrors EntityErrors { get; set; }
        public ResultIniDestinatario ResultIniDestinatario { get; set; }
    }

    public class WcfDestinatario
    {
        public EntityErrors EntityErrors { get; set; }
        public Destinatario Destinatario { get; set; }
    }

}
