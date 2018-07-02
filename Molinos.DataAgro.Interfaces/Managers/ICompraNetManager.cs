using System.Threading.Tasks;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICompraNetManager
    {        
        Task<DatosIniCompraNet> TraerDatosInicialesAsync(string ActiveDirectory);
    }
}