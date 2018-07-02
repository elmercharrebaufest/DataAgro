using Molinos.DataAgro.Entities;
using System.Collections.Generic;


namespace Molinos.DataAgro.Interfaces
{
    public interface IRG2300Manager
    {
        bool InsetarRG2300(List<RG2300>oDatos, int cantProcesar);

    }
}
