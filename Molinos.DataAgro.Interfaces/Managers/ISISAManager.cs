using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;

namespace Molinos.DataAgro.Interfaces
{
    public interface ISISAManager
    {
        bool InsertarSISA(List<SISA> oDatos);
    }
}
