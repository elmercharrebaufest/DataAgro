
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities.Dto;
using Molinos.DataAgro.Entities.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

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


