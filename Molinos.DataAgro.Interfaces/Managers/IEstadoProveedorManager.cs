
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface IEstadoProveedorManager
    {
        void ActualizarProveedores();

        void ActualizarProveedoresPorComercial(int comercialId, List<int> equipo);
    }

}
