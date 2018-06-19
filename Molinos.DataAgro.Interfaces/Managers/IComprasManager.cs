using Mastersoft.Framework.DataRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IComprasManager
    {
        void Inicializar(MSContext oContexto);

        void ActualizarComprasProveedor();

        void ArmarCargaInicial();

    }
}
