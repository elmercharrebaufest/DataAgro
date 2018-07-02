using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IFijacionDePrecioManager
    {
        Task<DatosIniAbmFijacionDePrecio> TraerDatosInicialesAsync();

        Task<ResultIniFijacionDePrecio> TraerTodoFijacionDePrecioAsync();

        Task<FijacionDePrecio> TraerFijacionDePrecioAsync(int intFijacionId);

        Task<EntityErrors> GrabarFijacionDePrecioAsync(FijacionDePrecio oFijacionDePrecio, string idActiveDirectory);

        Task<EntityErrors> EliminarFijacionDePrecioAsync(int intFijacionId);

        FijacionDePrecio NuevoFijacionDePrecio();
    }
}


