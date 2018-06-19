using Molinos.DataAgro.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;
using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;



namespace Molinos.DataAgro.Interfaces
{
    public interface IRG2300Manager
    {
        void Inicializar(MSContext oContexto);

        bool InsetarRG2300(List<RG2300>oDatos, int cantProcesar);

    }
}
