using Molinos.DataAgro.Entities.Dto;
using System.Threading.Tasks;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICompraNetManager
    {        
        Task<DatosIniCompraNet> TraerDatosInicialesAsync(string ActiveDirectory);
    }
}