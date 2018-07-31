using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICompraNetManager
    {        
        Task<DatosIniCompraNet> TraerDatosInicialesAsync(string ActiveDirectory);        
    }
}