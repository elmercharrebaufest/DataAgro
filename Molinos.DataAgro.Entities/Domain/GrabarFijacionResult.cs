using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class GrabarFijacionResult
    {
        public List<ErrorMessage> errores { get; set; }
        public int? FijacionDePrecioContratoId { get; set; }

        public GrabarFijacionResult()
        {

        }
    }
}
