using Mastersoft.Framework.Standard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities
{
    public class GrabarProveedorResult
    {
        public EntityErrors Errores { get; set; }
        public int? ProveedorId { get; set; }


        public GrabarProveedorResult()
        {

        }

    }
}
