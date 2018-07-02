
using System.Collections.Generic;
using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IProvinciaManager
    {
        Task<ResultIniProvincia> TraerTodoProvinciaAsync();

        Task<Provincia> TraerProvinciaAsync(int intProvinciaId);

        Task<EntityErrors> GrabarProvinciaAsync(Provincia oProvincia);

        Task<EntityErrors> EliminarProvinciaAsync(int intProvinciaId);

        List<Provincia> ListarProvincia(string provincia);
    }
}


