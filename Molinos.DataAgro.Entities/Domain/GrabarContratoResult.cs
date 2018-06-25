using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class GrabarContratoResult
    {
        public List<ErrorMessage> Errores { get; set; }
        public int? ContratoId { get; set; }


        public GrabarContratoResult()
        {

        }

      

    }
}
