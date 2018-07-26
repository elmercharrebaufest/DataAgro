using Molinos.DataAgro.Entities.Dto;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces.Managers
{
    public interface IComprasManager
    {
        void ActualizarComprasProveedor(List<Datos> listProve);
    }
}
