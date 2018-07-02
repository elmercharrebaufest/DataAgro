using System.Threading.Tasks;

using Mastersoft.Framework.Standard;

using Molinos.DataAgro.Entities;

namespace Molinos.DataAgro.Interfaces
{
    public interface IPreslipManager
    {
        Task<DatosIniAbmPreslip> TraerDatosInicialesAsync();

        Task<ResultIniPreslip> TraerTodoPreslipAsync();

        Task<Preslip> TraerPreslipAsync(int intPreslipId);

        Task<EntityErrors> GrabarPreslipAsync(Preslip oPreslip);

        Task<EntityErrors> EliminarPreslipAsync(int intPreslipId);

        Preslip NuevoPreslip();
    }
}


