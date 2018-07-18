using System.Collections.Generic;
using System.Threading.Tasks;
using Mastersoft.Framework.Standard;
using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{

    public interface ICentroManager
    {
        Task<DatosIniAbmCentro> TraerDatosInicialesAsync();
        Task<ResultIniCentro> TraerTodoCentroAsync();
        Task<Centro> TraerCentroAsync(int id);
        Task<EntityErrors> GrabarCentroAsync(Centro oCentro);
        Task<EntityErrors> EliminarCentroAsync(int id);
    }
}