using Molinos.DataAgro.Entities.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{
    public interface ICapacidadProductivaDisponibleAgent
    {
        List<CapacidadProductivaPendienteDto> ObtenerCapacidadProductivaPendiente(string cuit);
    }
}
