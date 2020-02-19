using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICapacidadProductivaAgent
    {
        string ObtenerCapacidadProductiva(string cuit, decimal cantidad, string centro, string cosecha, string material);
    }
}
