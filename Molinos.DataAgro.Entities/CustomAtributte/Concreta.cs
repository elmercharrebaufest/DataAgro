using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Entities.CustomAtributte
{
    [System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
    public class Concreta : System.Attribute
    {
        public  bool result;

        public Concreta(bool result)
        {
            this.result = result;
        }
    }
}
