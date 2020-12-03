using Molinos.DataAgro.Entities.Dto;

namespace Molinos.DataAgro.Interfaces
{
    public interface IAdministracionCuperaAgent
    {
        string AdministrarCupera(ConfiguracionCupoDto c);
    }
}