using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IValidarLiquidacionParaFijacionAgent
    {
        string Validar(string contratoSap, string fijacion);

    }
}
