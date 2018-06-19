using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class GrabarPostItResult
    {
        public EntityErrors errores { get; set; }
        public int? ComercialId { get; set; }


        public GrabarPostItResult()
        {

        }
    }
}
