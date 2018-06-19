using Mastersoft.Framework.DataRepository;
using Mastersoft.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface IEstadoProveedorManager
    {
        void Inicializar(MSContext oContexto);

        void ActualizarProveedores();

        void ActualizarProveedoresPorComercial(int ComercialId);
    }

}
